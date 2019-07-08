using Microsoft.AspNetCore.Http;
using PrometheusCore;
using System;
using System.Threading.Tasks;
using PrometheusCore.Collectors;

namespace PrometheusCore.AspNet
{
    internal class PrometheusMiddleware : IPrometheusMiddleware
    {
        private IRequestCollectors _requestCollectors;
        public PrometheusMiddleware(IRequestCollectors requestCollectors)
        {
            _requestCollectors = requestCollectors;
        }
        public async Task Handle(HttpContext context, Func<Task> next)
        {
            var labels = GetLabels(context);
            try
            {
                using (var timer = new TimerScope(_requestCollectors.RequestDuration.WithLabels(labels)))
                {
                    _requestCollectors.ActiveUriRequests.WithLabels(labels).Inc();
                    _requestCollectors.TotalUriRequestCount.WithLabels(labels).Inc();
                    await next();
                }
            }
            finally
            {
                _requestCollectors.ActiveUriRequests.WithLabels(labels).Dec();
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
