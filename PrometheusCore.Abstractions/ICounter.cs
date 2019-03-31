namespace PrometheusCore
{
    public interface ICounter : ICollector
    {
        void Inc(double increment = 1);
        double Value { get; }
    }
}