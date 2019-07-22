using PrometheusCore;
using PrometheusCore.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyExtensions
    {
        public static ICollectorsBuilder AddPrometheus(this IServiceCollection services)
        {
            services.AddSingleton<ICollectorFactory, CollectorFactory>();
            services.AddSingleton<IBackgroundWorker, BackgroundWorker>();

            services.AddTransient<ICounter, Counter>();
            services.AddTransient<IGauge, Gauge>();
            services.AddTransient<IHistogram, Histogram>();

            return new CollectorsBuilder(services);
        }
        public static IServiceCollection UsePrometheusGatewayClient(this IServiceCollection services)
        {
            return services.AddHostedService<GatewayClient>();
        }
        public static IServiceCollection UsePrometheusListener(this IServiceCollection services)
        {
            return services.AddHostedService<ListenerService>();
        }
    }
}
