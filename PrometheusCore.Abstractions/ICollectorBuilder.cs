using PrometheusCore;

namespace PrometheusCore
{
    public interface ICollectorBuilder<T> where T : ICollector
    {
        T WithLabels(params string[] values);
    }
}