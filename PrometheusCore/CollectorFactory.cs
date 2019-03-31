using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public class CollectorFactory : ICollectorFactory
    {
        private IDictionary<string, IRegisteredCollector> _registeredCollectors;

        public IList<IBackground> Backgrounds { get; private set; }
        public ConcurrentBag<Action> BeforeCollectCallbacks { get; private set; }
        public CollectorFactory(IDictionary<string, IRegisteredCollector> registeredCollectors, IList<IBackground> backgrounds)
        {
            _registeredCollectors = registeredCollectors;

            Backgrounds = backgrounds;
            BeforeCollectCallbacks = new ConcurrentBag<Action>();
        }
        public void InjectCollectors<I, T>(T t) where T : I
        {
            PropertyInfo[] iProps = typeof(I).GetProperties();
            foreach (PropertyInfo iProp in iProps)
            {
                var iAttribute = iProp.GetCustomAttribute<CollectorRegistryAttribute>();
                if (iAttribute != null)
                {
                    var tProp = typeof(T).GetProperty(iProp.Name);
                    var factory = new Func<string, string[], ICollector>(GetInstance);
                    if (typeof(ICollector).IsAssignableFrom(iProp.PropertyType))
                    {
                        var collectorBuilder = Activator.CreateInstance(typeof(CollectorBuilder<>).MakeGenericType(iProp.PropertyType), new object[] { iAttribute.Name, factory });
                        if (typeof(ICollector).IsAssignableFrom(iProp.PropertyType))
                        {
                            var tAttribute = tProp.GetCustomAttribute<CollectorInstanceAttribute>();
                            var withLabels = collectorBuilder.GetType().GetMethod("WithLabels");
                            object collector;
                            if (tAttribute != null)
                            {
                                collector = withLabels.Invoke(collectorBuilder, new object[] { tAttribute.Values });
                            }
                            else
                            {
                                collector = withLabels.Invoke(collectorBuilder, new object[] { new string[0] });
                            }
                            tProp.SetValue(t, collector);
                        }
                    }
                    else if (iProp.PropertyType.Name == "ICollectorBuilder`1" && iProp.PropertyType.IsGenericType && typeof(ICollector).IsAssignableFrom(iProp.PropertyType.GenericTypeArguments[0]))
                    {
                        var collectorBuilder = Activator.CreateInstance(typeof(CollectorBuilder<>).MakeGenericType(iProp.PropertyType.GenericTypeArguments[0]), new object[] { iAttribute.Name, factory });
                        tProp.SetValue(t, collectorBuilder);
                    }
                }
            }
        }
        public void AddBeforeCollectCallback(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            BeforeCollectCallbacks.Add(callback);
        }
        public ICollectorBuilder<ICounter> GetCounter(string name)
        {
            if (TryGetRegistered(name, out IRegisteredCollector registeredCollector))
            {
                return new CollectorBuilder<ICounter>(name, GetInstance);
            }

            return null;
        }
        public ICollectorBuilder<IGauge> GetGauge(string name)
        {
            if (TryGetRegistered(name, out IRegisteredCollector registeredCollector))
            {
                return new CollectorBuilder<IGauge>(name, GetInstance);
            }

            return null;
        }

        private bool TryGetRegistered(string name, out IRegisteredCollector registeredCollector)
        {
            if (!_registeredCollectors.ContainsKey(name))
            {
                throw new ArgumentException($"Collector is not registered {name}.");
            }

            registeredCollector = _registeredCollectors[name];
            return true;
        }
        private ICollector GetInstance(string name, params string[] values)
        {
            if (TryGetRegistered(name, out IRegisteredCollector registeredCollector))
            {
                var identifier = CreateIdentifier(name, new Labels(registeredCollector.LabelNames, values));

                if (registeredCollector.Collectors.TryGetValue(identifier, out ICollector collector))
                {
                    return collector;
                }
                else
                {
                    ICollector t = null;
                    switch (registeredCollector.CollectorType)
                    {
                        case Type counterType when counterType == typeof(Counter):
                        case Type gaugeType when gaugeType == typeof(Gauge):
                            t = (ICollector)Activator.CreateInstance(registeredCollector.CollectorType, new object[] { name, new Labels(registeredCollector.LabelNames, values)});
                            break;
                        case Type histogramType when histogramType == typeof(Histogram):
                            t = (ICollector)Activator.CreateInstance(registeredCollector.CollectorType, new object[] { name, new Labels(registeredCollector.LabelNames, values), (double[])registeredCollector.Data });
                            break;
                    }
                    if (registeredCollector.Collectors.TryAdd(identifier, t))
                    {
                        return t;
                    }
                }
            }
            return null;
        }
        internal static string CreateIdentifier(string name, ILabels labels, string postfix = null, params (string name, string value)[] extraLabels)
        {
            var fullName = postfix != null ? $"{name}_{postfix}" : name;
            var labelsCombined = new Labels(labels);

            for (int i = 0; i < extraLabels.Length; i++)
            {
                labelsCombined.Add(new Label(extraLabels[i].name, extraLabels[i].value));
            }
            if (labelsCombined.Count != 0)
            {
                return $"{fullName}{{{labelsCombined.ToString()}}}";
            }
            else
            {
                return fullName;
            }
        }
        public async Task CollectAsync(Stream stream)
        {
            foreach (var callback in BeforeCollectCallbacks)
            {
                callback();
            }

            var registeredEnumerator = _registeredCollectors.GetEnumerator();
            while (registeredEnumerator.MoveNext())
            {
                await registeredEnumerator.Current.Value.SerializeAsync(stream);
            }
        }
    }
}