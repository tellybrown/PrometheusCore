namespace PrometheusCore
{
    public interface IHistogram : ICollector
    {
        double Sum { get; }
        long Count { get; }
        long this[int index] { get; }
        int Length { get; }
        void Observe(double amt);
    }
}
