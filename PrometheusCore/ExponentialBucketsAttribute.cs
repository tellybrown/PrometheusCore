namespace PrometheusCore
{
    public class ExponentialBucketsAttribute : HistogramBucketsAttribute
    {
        public ExponentialBucketsAttribute(double start, double factor, int count) : base(PrometheusCore.Buckets.Exponential(start, factor, count))
        {
        }
    }
}
