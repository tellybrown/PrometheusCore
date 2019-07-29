using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusCore.Collectors
{
    public class QueryCollectors : IQueryCollectors
    {
        public ICollectorBuilder<IHistogram> Duration { get; private set; }
        public ICollectorBuilder<ICounter> Count { get; private set; }
        public ICollectorBuilder<ICounter> Error { get; private set; }

        public QueryCollectors(ICollectorFactory collectorFactory)
        {
            collectorFactory.InjectCollectors<IQueryCollectors, QueryCollectors>(this);
        }
    }
}
