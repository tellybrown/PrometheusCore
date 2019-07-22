using PrometheusCore;

namespace HostedExample.Collectors
{
    public interface IJobCollectors
    {
        [CollectorRegistry("job_processed", "Number of items processed for the job")]
        ICounter ProcessedCount { get; }
        [CollectorRegistry("job_failed", "Number of errors for the job")]
        ICounter FailedCount { get; }
    }
}