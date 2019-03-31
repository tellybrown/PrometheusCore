namespace PrometheusCore
{
    public class LinearBucketsAttribute : HistogramBucketsAttribute
    {
        public LinearBucketsAttribute(double start, double increment, int count) : base(PrometheusCore.Buckets.Linear(start, increment, count))
        {
        }
    }
}
