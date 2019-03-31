using PrometheusCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetExample.Collectors
{
    public class HomeCollectors : IHomeCollectors
    {
        public ICounter PageViewsCount { get; private set; }
        public ICounter PageErrorsCount { get; private set; }

        public HomeCollectors(ICollectorFactory collectorFactory)
        {
            collectorFactory.InjectCollectors<IHomeCollectors, HomeCollectors>(this);
        }
    }
}
