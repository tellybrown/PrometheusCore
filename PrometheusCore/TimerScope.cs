using System;
using System.Diagnostics;

namespace PrometheusCore
{
    public class TimerScope : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly IGauge _gauge;
        private readonly ICounter _counter;
        private readonly IHistogram _histogram;
        public TimerScope(IGauge gauge)
        {
            _gauge = gauge;
            _stopwatch = Stopwatch.StartNew();
        }
        public TimerScope(ICounter counter)
        {
            _counter = counter;
            _stopwatch = Stopwatch.StartNew();
        }
        public TimerScope(IHistogram histogram)
        {
            _histogram = histogram;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            var duration = _stopwatch.Elapsed.TotalSeconds;
            if (_counter != null)
            {
                _counter.Inc(duration);
            }
            if (_gauge != null)
            {
                _gauge.Set(duration);
            }
            if (_histogram != null)
            {
                _histogram.Observe(duration);
            }
        }
    }
}
