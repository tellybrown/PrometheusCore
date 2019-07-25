using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusCore.Collectors
{
    public class QueryCollectors : IQueryCollectors
    {
        public ICollectorBuilder<IHistogram> Duration { get; }
        public ICollectorBuilder<ICounter> Count { get; }
        public ICollectorBuilder<ICounter> Error { get; }

        public QueryCollectors(ICollectorFactory collectorFactory)
        {
            collectorFactory.InjectCollectors<IQueryCollectors, QueryCollectors>(this);
        }
    }
}
