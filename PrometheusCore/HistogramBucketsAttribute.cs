using System;

namespace PrometheusCore
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class HistogramBucketsAttribute : Attribute
    {
        public double[] Buckets { get; protected set; }

        public HistogramBucketsAttribute()
        {
            Buckets = PrometheusCore.Buckets.Default;
        }
        public HistogramBucketsAttribute(params double[] buckets)
        {
            Buckets = buckets;
        }
    }
}
