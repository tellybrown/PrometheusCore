using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PrometheusCore.Hosting
{
    public class GatewayClientOptions
    {
        public string GatewayUri { get; set; }
        public TimeSpan Interval { get; set; }
        public string Job { get; set; }
        public string Instance { get; set; }
    }
    public class GatewayClient : IHostedService
    {
        private readonly ICollectorFactory _factory;
        private readonly IOptions<GatewayClientOptions> _options;
        public GatewayClient(ICollectorFactory factory, IOptions<GatewayClientOptions> options)
        {
            _factory = factory;
            _options = options;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_options.Value.GatewayUri))
            {
                throw new ArgumentNullException("gatewayUri");
            }
            if (string.IsNullOrEmpty(_options.Value.Job))
            {
                throw new ArgumentNullException("job");
            }
            if (_options.Value.Interval.TotalMilliseconds <= 0)
            {
                throw new ArgumentException("Interval must be greater than zero", "interval");
            }

            UriBuilder uriBuilder = new UriBuilder(_options.Value.GatewayUri);
            uriBuilder.Path += $"job/{_options.Value.Job}";
            if (!string.IsNullOrEmpty(_options.Value.Instance))
            {
                uriBuilder.Path += $"instance/{_options.Value.Instance}";
            }

            return Task.Run(async () =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(_options.Value.Interval, cancellationToken);

                        cancellationToken.ThrowIfCancellationRequested();

                        using (HttpClient httpClient = new HttpClient())
                        {
                            using (var stream = new MemoryStream())
                            {
                                await _factory.CollectAsync(stream);
                                HttpContent content = new StreamContent(stream);
                                await httpClient.PostAsync(uriBuilder.Uri, content);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
