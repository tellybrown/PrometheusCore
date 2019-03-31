using System;

namespace PrometheusCore
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CollectorRegistryAttribute : Attribute
    {
        public string Name { get; }
        public string Help { get; }
        public string[] LabelNames { get; }

        public CollectorRegistryAttribute(string name, string help, params string[] labelNames)
        {
            Name = name;
            Help = help;
            LabelNames = labelNames;
        }
    }
}
