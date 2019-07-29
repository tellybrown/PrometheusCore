using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusCore.Collectors
{
    public interface IQueryCollectors
    {
        [CollectorRegistry("database_query_duration_seconds", "How long a specific query has taken to run", "service_name", "database_name", "query_name")]
        [HistogramBuckets(.5, 1, 2, 3, 5, 10, 30, 60)]
        ICollectorBuilder<IHistogram> Duration { get; }

        [CollectorRegistry("database_query_count", "How many times a query has been ran", "service_name", "database_name", "query_name")]
        ICollectorBuilder<ICounter> Count { get; }

        [CollectorRegistry("database_query_error_count", "How many times a query has errored", "service_name", "database_name", "query_name")]
        ICollectorBuilder<ICounter> Error { get; }
    }
}
