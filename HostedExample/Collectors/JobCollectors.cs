using PrometheusCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HostedExample.Collectors
{
    public class JobCollectors : IJobCollectors
    {
        public ICounter ProcessedCount { get; private set; }
        public ICounter FailedCount { get; private set; }

        public JobCollectors(ICollectorFactory collectorFactory)
        {
            collectorFactory.InjectCollectors<IJobCollectors, JobCollectors>(this);
        }
    }
}
