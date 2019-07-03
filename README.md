# prometheusCore

This is a .NET Core library for instrumenting your applications and exporting metrics to [Prometheus](http://prometheus.io/).

This library was written to provide full support for dependency injection in .Net Core to reduce overall coding and testing.
A BIG thanks to those at Prometheus-net for their library but its usage just wasn't how I wanted to do things.

# Installation

Nuget package for general use and metrics export : [prometheusCore](https://www.nuget.org/packages/prometheusCore)

>Install-Package prometheusCore

Nuget package for ASP.NET Core middleware and metrics api controller: [prometheusCore.AspNetCore](https://www.nuget.org/packages/prometheusCore.AspNet)

>Install-Package prometheusCore.AspNet

Nuget package for Hosting in a console service: [prometheusCore.AspNetCore](https://www.nuget.org/packages/prometheusCore.Hosting)

>Install-Package prometheusCore.Hosting

# Counter Collector

Counters only increase in value and reset to zero when the process restarts.

```csharp
ICounter counter = new Counter("name", Labels.Empty);
counter.Inc();
```

# Gauge Collector

Gauges can have any numeric value and change arbitrarily.

```csharp
IGauge gauge = new Gauge("name", Labels.Empty);
gauge.Inc();
gauge.Dec();
```

# Summary Collector

Summaries track the trends in events over time (10 minutes by default).

WIP

# Histogram Collector

Histograms track the size and number of events in buckets. This allows for aggregatable calculation of quantiles.

```csharp
IHistogram histogram = new Histogram("name", Labels.Empty);
histogram.Observe(2);
```

# Labels

All metrics can have labels, allowing grouping of related time series.

See the best practices on [naming](http://prometheus.io/docs/practices/naming/)
and [labels](http://prometheus.io/docs/practices/instrumentation/#use-labels).

See the Histogram Collector below for an example using a runtime Label value

# Injectable Collection of Collectors

You can create an interface/class to be injected into your other objects.  This will handle creating each collector for you and add it as a singleton to the IServiceCollection.

```csharp
public interface ITestCollectors
{
    [CollectorRegistry("got_something_count", "got something count")]
    ICounter GotSomethingCount { get; }
    [CollectorRegistry("got_something_active_count", "actively got something count")]
    IGauge GotSomethingActiveCount { get; }
    [CollectorRegistry("got_something_error_count", "got something error count")]
    ICounter GotSomethingErrorCount { get; }
    [CollectorRegistry("got_something_duration", "go something duration", "user")]
    [HistogramBuckets(.5, 1, 2, 3, 5, 10, 30, 60)]
    ICollectorBuilder<IHistogram> GotSomethingDuration { get; }
}
public class TestCollectors : ITestCollectors
{
    public ICounter GotSomethingCount { get; private set; }
    public IGauge GotSomethingActiveCount { get; private set; }
    public ICounter GotSomethingErrorCount { get; private set; }
    public ICollectorBuilder<ICounter> GotSomethingDuration { get; private set; }

    public HomeCollectors(ICollectorFactory collectorFactory)
    {
        collectorFactory.InjectCollectors<ITestCollectors, TestCollectors>(this);
    }
}
public class TestController
{
    private readonly ITestCollectors _testCollectors;
    public TestController(ITestCollectors testCollectors)
    {
        _testCollectors = testCollectors;
    }

    public IActionResult GetSomething()
    {
        using (var timer = new TimerScope(_testCollectors.GotSomethingDuration.WithLabels(HttpContext.User.ToString)))
        {
            try
            {
                _testCollectors.GotSomethingActiveCount.Inc();
                GoGetSomething();
                _testCollectors.GotSomethingCount.Inc();
            }
            catch(Exception)
            {
                _testCollectors.GotSomethingErrorCount.Inc();
            }
            finally
            {
                _testCollectors.GotSomethingActiveCount.Dec();
            }
        }
    }
}
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddPrometheus()
                .Register<ITestCollectors, TestCollectors>()
                .Build();
    }
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UsePrometheus();
        app.UseMvc();
    }
}


```

# ASP.NET Core exporter middleware

For projects built with ASP.NET Core, a middleware plugin is provided.

If you use the default Visual Studio project template, modify *Startup.cs* as follows:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UsePrometheus();
    app.UseMvc();
}
```


The default configuration will publish metrics on the /metrics URL.

This functionality is delivered in the `prometheusCore.AspNet` NuGet package.









