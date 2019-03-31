using PrometheusCore;

namespace PrometheusCore.AspNet
{
    internal class RequestStatistics : IRequestStatistics
    {
        public ICollectorBuilder<IGauge> ActiveUriRequests { get; private set; }
        public ICollectorBuilder<ICounter> TotalUriRequestCount { get; private set; }
        public ICounter TotalRequestCount { get; private set; }
        public ICollectorBuilder<IHistogram> RequestDuration { get; private set; }

        public RequestStatistics(ICollectorFactory metricFactory)
        {
            metricFactory.InjectCollectors<IRequestStatistics, RequestStatistics>(this);
        }
    }
}
