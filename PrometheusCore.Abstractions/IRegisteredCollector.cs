using PrometheusCore;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public interface IRegisteredCollector
    {
        string Name { get; }
        string Help { get; }
        string[] LabelNames { get; }
        Type CollectorType { get; }
        ConcurrentDictionary<string, ICollector> Collectors { get; }
        object Data { get; }

        Task SerializeAsync(Stream stream);
    }
}