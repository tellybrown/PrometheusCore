using System;

namespace PrometheusCore
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class CollectorDefaultAttribute : Attribute
    {
        public double Value { get; protected set; }

        public CollectorDefaultAttribute(double value)
        {
            Value = value;
        }
    }
}
