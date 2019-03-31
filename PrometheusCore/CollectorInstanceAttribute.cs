using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusCore
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class CollectorInstanceAttribute : Attribute
    {
        public string[] Values { get; }

        public CollectorInstanceAttribute(params string[] values)
        {
            Values = values;
        }
    }
}
