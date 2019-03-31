using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public class RegisteredCollector : IRegisteredCollector
    {
        private readonly byte[] _headerBytes;
        public RegisteredCollector(Type type, Type iface, string name, string help, object data, params string[] labelNames)
        {
            Name = name;
            Help = help;
            LabelNames = labelNames;
            CollectorType = type;
            InterfaceType = iface;
            Collectors = new ConcurrentDictionary<string, ICollector>();
            Data = data;

            _headerBytes = Constants.ExportEncoding.GetBytes($"# TYPE {name} {GetCollectorType(iface)}{Constants.NewLine}# HELP {name} {help}{Constants.NewLine}");
        }
        private string GetCollectorType(Type iface)
        {
            if (typeof(ICounter).IsAssignableFrom(iface))
            {
                return "counter";
            }

            if (typeof(IGauge).IsAssignableFrom(iface))
            {
                return "gauge";
            }

            if (typeof(IHistogram).IsAssignableFrom(iface))
            {
                return "histogram";
            }

            //if (typeof(ISummary).IsAssignableFrom(iface))
            //{
            //    return "summary";
            //}
            return "unknown";
        }

        public string Name { get; }
        public string Help { get; }
        public Type CollectorType { get; }
        public Type InterfaceType { get; }
        public string[] LabelNames { get; }
        public ConcurrentDictionary<string, ICollector> Collectors { get; private set; }
        public object Data { get; }

        public async Task SerializeAsync(Stream stream)
        {
            await stream.WriteAsync(_headerBytes, 0, _headerBytes.Length);

            var collectorEnumerator = Collectors.GetEnumerator();
            while (collectorEnumerator.MoveNext())
            {
                await collectorEnumerator.Current.Value.SerializeAsync(stream);
            }
        }
    }
}