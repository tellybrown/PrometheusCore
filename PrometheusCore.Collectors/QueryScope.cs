using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PrometheusCore.Collectors
{
    public class QueryScope : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly IQueryCollectors _queryCollectors;
        private readonly string _serviceName;
        private readonly string _databaseName;
        private readonly string _queryName;
        private bool _completed;
        public QueryScope(IQueryCollectors queryCollectors, string serviceName, string databaseName, string queryName)
        {
            _queryCollectors = queryCollectors;
            _stopwatch = Stopwatch.StartNew();
            _serviceName = serviceName;
            _databaseName = databaseName;
            _queryName = queryName;
            _completed = false;
        }

        public void Complete()
        {
            _completed = true;
        }

        public void Dispose()
        {
            string[] labels = new string[] { _serviceName, _databaseName,_queryName };

            var duration = _stopwatch.Elapsed.TotalSeconds;

            _queryCollectors.Count.WithLabels(labels).Inc();
            _queryCollectors.Duration.WithLabels(labels).Observe(duration);

            if (!_completed)
            {
                _queryCollectors.Error.WithLabels(labels).Inc();
            }
        }
    }
}
