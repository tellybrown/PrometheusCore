using PrometheusCore;
using System;

namespace PrometheusCore
{
    public class CollectorBuilder<T> : ICollectorBuilder<T> where T : ICollector
    {
        private string _name;
        private Func<string, string[], ICollector> _factory;

        public CollectorBuilder(string name, Func<string, string[], ICollector> factory)
        {
            _name = name;
            _factory = factory;
        }

        public T WithLabels(params string[] values)
        {
            return (T)_factory(_name, values);
        }
    }
}