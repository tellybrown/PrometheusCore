using PrometheusCore;

namespace AspNetExample.Collectors
{
    public interface IHomeCollectors
    {
        [CollectorRegistry("AspNetExample_Home_View_Count", "Number of view on the Home Controller")]
        ICounter PageViewsCount { get; }
        [CollectorRegistry("AspNetExample_Home_Error_Count", "Number of errors on the Home Controller")]
        ICounter PageErrorsCount { get; }
    }
}