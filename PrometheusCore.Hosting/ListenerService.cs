using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrometheusCore.Hosting
{
    public class ListenerOptions
    {
        public string Url { get; set; }
    }
    public class ListenerService : IListenerService, IHostedService
    {
        private readonly HttpListener _httpListener;
        private readonly ICollectorFactory _factory;
        private readonly IOptions<ListenerOptions> _options;

        public ListenerService(ICollectorFactory factory, IOptions<ListenerOptions> options)
        {
            _factory = factory;
            _httpListener = new HttpListener();
            _options = options;
        }

        private Task _executingTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private async Task Listen(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await _httpListener.GetContextAsync();
                await HandleContextAsync(context);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _httpListener.Prefixes.Add(_options.Value.Url);
            _httpListener.Start();

            _executingTask = Listen(cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        private async Task HandleContextAsync(HttpListenerContext context)
        {
            try
            {
                await _factory.CollectAsync(context.Response.OutputStream);
                context.Response.OutputStream.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
