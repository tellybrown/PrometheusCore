using PrometheusCore;

namespace PrometheusCore.AspNet
{
    public class RequestCollectors : IRequestCollectors
    {
        public ICollectorBuilder<IGauge> ActiveUriRequests { get; private set; }
        public ICollectorBuilder<ICounter> TotalUriRequestCount { get; private set; }
        public ICounter TotalRequestCount { get; private set; }
        public ICollectorBuilder<IHistogram> RequestDuration { get; private set; }

        public RequestCollectors(ICollectorFactory metricFactory)
        {
            metricFactory.InjectCollectors<IRequestCollectors, RequestCollectors>(this);
        }
    }
}
