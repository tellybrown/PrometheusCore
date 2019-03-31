using Microsoft.AspNetCore.Http;
using PrometheusCore;
using System;
using System.Threading.Tasks;

namespace PrometheusCore.AspNet
{
    internal class PrometheusMiddleware : IPrometheusMiddleware
    {
        private IRequestStatistics _requestStatistics;
        public PrometheusMiddleware(IRequestStatistics requestStatistics)
        {
            _requestStatistics = requestStatistics;
        }
        public async Task Handle(HttpContext context, Func<Task> next)
        {
            var labels = GetLabels(context);
            try
            {
                using (var timer = new TimerScope(_requestStatistics.RequestDuration.WithLabels(labels)))
                {
                    _requestStatistics.ActiveUriRequests.WithLabels(labels).Inc();
                    _requestStatistics.TotalUriRequestCount.WithLabels(labels).Inc();
                    await next();
                }
            }
            finally
            {
                _requestStatistics.ActiveUriRequests.WithLabels(labels).Dec();
            }
        }
        private string[] GetLabels(HttpContext context)
        {
            return new string[]
            {
                context.Request.Method.ToLowerInvariant(),
                context.Request.Path.HasValue ? context.Request.Path.Value.ToLowerInvariant() : "/"
            };
        }
    }
}
