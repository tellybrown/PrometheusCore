using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PrometheusCore
{
    public class CollectorsBuilder : ICollectorsBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IDictionary<string, IRegisteredCollector> _registeredCollectors;
        private readonly IList<IBackground> _backgroundCollectors;
        public CollectorsBuilder(IServiceCollection services)
        {
            _services = services;
            _registeredCollectors = new Dictionary<string, IRegisteredCollector>();
            _backgroundCollectors = new List<IBackground>();
        }

        public void Build()
        {
            _services.AddSingleton(_registeredCollectors);
            _services.AddSingleton(_backgroundCollectors);
        }

        public ICollectorsBuilder RegisterCounter(string name, string help, params string[] labelNames)
        {
            return Register<ICounter, Counter>(name, help, null, labelNames);
        }
        public ICollectorsBuilder RegisterGauge(string name, string help, params string[] labelNames)
        {
            return Register<IGauge, Gauge>(name, help, null, labelNames);
        }
        public ICollectorsBuilder RegisterHistogram(string name, string help, double[] buckets, params string[] labelNames)
        {
            return Register<IHistogram, Histogram>(name, help, buckets, labelNames);
        }

        private ICollectorsBuilder Register<I, T>(string name, string help, object data, params string[] labelNames)
            where T : class, I, ICollector
            where I : class
        {
            _registeredCollectors.Add(name, new RegisteredCollector(typeof(T), typeof(I), name, help, data, labelNames));
            return this;
        }



        public ICollectorsBuilder Register<I, T>(bool background = false)
            where T : class, I
            where I : class
        {
            PropertyInfo[] props = typeof(I).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                double[] buckets = null;
                if (prop.TryGetAttribute(out HistogramBucketsAttribute bucketsAttrib))
                {
                    buckets = bucketsAttrib.Buckets;
                }

                if (prop.TryGetAttribute(out CollectorRegistryAttribute registry))
                {
                    if (typeof(ICollector).IsAssignableFrom(prop.PropertyType))
                    {
                        var implType = FindService(prop.PropertyType);
                        _registeredCollectors.Add(registry.Name, new RegisteredCollector(implType, prop.PropertyType, registry.Name, registry.Help, buckets, registry.LabelNames));
                    }
                    else if (prop.PropertyType.Name == "ICollectorBuilder`1" && prop.PropertyType.IsGenericType && typeof(ICollector).IsAssignableFrom(prop.PropertyType.GenericTypeArguments[0]))
                    {
                        var implType = FindService(prop.PropertyType.GenericTypeArguments[0]);
                        _registeredCollectors.Add(registry.Name, new RegisteredCollector(implType, prop.PropertyType.GenericTypeArguments[0], registry.Name, registry.Help, buckets, registry.LabelNames));
                    }
                }
            }

            if (background)
            {
                _backgroundCollectors.Add(new Background(typeof(I)));
            }
            _services.AddSingleton<I, T>();

            return this;
        }
        private Type FindService(Type t)
        {
            var enumerator = _services.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.ServiceType == t)
                {
                    return enumerator.Current.ImplementationType;
                }
            }
            throw new Exception($"Could not find type {t.Name} in ServicesCollection");
        }
    }
}
