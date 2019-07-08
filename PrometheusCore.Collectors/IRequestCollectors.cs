using PrometheusCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusCore.AspNet
{
    public interface IRequestCollectors
    {
        [CollectorRegistry("aspnet_requests_uri_active", "Number of active requests per uri", "method", "uri")]
        ICollectorBuilder<IGauge> ActiveUriRequests { get;  }
        [CollectorRegistry("aspnet_requests_uri_total", "Total requests per uri", "method", "uri")]
        ICollectorBuilder<ICounter> TotalUriRequestCount { get; }
        [CollectorRegistry("aspnet_requests_total", "Total requests")]
        ICounter TotalRequestCount { get; }
        [CollectorRegistry("aspnet_requests_duration", "Request durations", "method", "uri")]
        [HistogramBuckets(.5, 1, 2, 3, 5, 10, 30, 60)]
        ICollectorBuilder<IHistogram> RequestDuration { get; }
    }
}
