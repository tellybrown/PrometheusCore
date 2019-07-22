using HostedExample.Collectors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrometheusCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HostedExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ListenerOptions>(option =>
                    {
                        option.Url = "http://localhost/job/metric/";
                    });
                    services.AddPrometheus()
                        .Register<IJobCollectors, JobCollectors>()
                        .Build();
                    services.UsePrometheusListener();
                    services.AddHostedService<Job>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
