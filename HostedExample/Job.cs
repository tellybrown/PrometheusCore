using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HostedExample
{
    public class Job : IHostedService
    {
        private readonly Collectors.IJobCollectors _jobCollectors;
        public Job(Collectors.IJobCollectors jobCollectors)
        {
            _jobCollectors = jobCollectors;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _jobCollectors.ProcessedCount.Inc();
                    await Task.Delay(1000);
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
