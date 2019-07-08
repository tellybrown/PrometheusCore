using PrometheusCore;
using System;
using System.Diagnostics;

namespace PrometheusCore.Collectors
{
    public class ProcessCollectors : IProcessCollectors
    {
        public ICollectorBuilder<ICounter> CollectionCounts { get; private set; }
        public IGauge StartTime { get; private set; }
        public ICounter CpuTotal { get; private set; }
        public IGauge VirtualMemorySize { get; private set; }
        public IGauge WorkingSet { get; private set; }
        public IGauge PrivateMemorySize { get; private set; }
        public IGauge OpenHandles { get; private set; }
        public IGauge NumThreads { get; private set; }
        public IGauge TotalMemory { get; private set; }

        private readonly Process _process;
        public ProcessCollectors(ICollectorFactory collectorFactory)
        {
            collectorFactory.InjectCollectors<IProcessCollectors, ProcessCollectors>(this);
            _process = Process.GetCurrentProcess();
            collectorFactory.AddBeforeCollectCallback(RefreshProcessCallback);

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            StartTime.Set((_process.StartTime.ToUniversalTime() - epoch).TotalSeconds);
        }

        public void RefreshProcessCallback()
        {
            try
            {
                _process.Refresh();

                for (var gen = 0; gen <= GC.MaxGeneration; gen++)
                {
                    var collectionCount = CollectionCounts.WithLabels(gen.ToString());
                    collectionCount.Inc(GC.CollectionCount(gen) - collectionCount.Value);
                }

                TotalMemory.Set(GC.GetTotalMemory(false));
                CpuTotal.Inc(Math.Max(0, _process.TotalProcessorTime.TotalSeconds - CpuTotal.Value));
                OpenHandles.Set(_process.HandleCount);
                NumThreads.Set(_process.Threads.Count);

                VirtualMemorySize.Set(_process.VirtualMemorySize64);
                WorkingSet.Set(_process.WorkingSet64);
                PrivateMemorySize.Set(_process.PrivateMemorySize64);
            }
            catch (Exception)
            {
            }
        }
    }
}
