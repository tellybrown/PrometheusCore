namespace PrometheusCore
{
    public interface IGauge : ICollector
    {
        void Inc(double increment = 1);
        void Set(double val);
        void Dec(double decrement = 1);
        double Value { get; }
    }
}