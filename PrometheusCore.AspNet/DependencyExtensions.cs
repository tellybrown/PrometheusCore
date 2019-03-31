using Microsoft.AspNetCore.Builder;
using PrometheusCore;
using PrometheusCore.AspNet;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyExtensions
    {
        public static ICollectorsBuilder AddPrometheus(this IServiceCollection services)
        {
            services.AddSingleton<ICollectorFactory, CollectorFactory>();
            services.AddSingleton<IBackgroundWorker, BackgroundWorker>();
            services.AddSingleton<IPrometheusMiddleware, PrometheusMiddleware>();

            services.AddTransient<ICounter, Counter>();
            services.AddTransient<IGauge, Gauge>();
            services.AddTransient<IHistogram, Histogram>();

            return new CollectorsBuilder(services)
                    .Register<IRequestStatistics, RequestStatistics>();
        }
        public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app)
        {
            var worker = app.ApplicationServices.GetService<IBackgroundWorker>();

            worker.Load();

            return app;
        }
        public static IApplicationBuilder UseRequestStatistics(this IApplicationBuilder app)
        {
            var middleware = app.ApplicationServices.GetService<IPrometheusMiddleware>();

            app.Use(async (context, next) =>
            {
                await middleware.Handle(context, next);
            });
            
            return app;
        }
    }
}
