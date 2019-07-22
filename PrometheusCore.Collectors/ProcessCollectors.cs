using System;
using System.Diagnostics;

namespace PrometheusCore.Collectors
{
    public class ProcessCollectors : IProcessCollectors
    {
        public ICollectorBuilder<ICounter> CollectionCounts { get; private set; }
        public ICollectorBuilder<IGauge> StartTime { get; private set; }
        public ICollectorBuilder<ICounter> CpuTotal { get; private set; }
        public ICollectorBuilder<IGauge> VirtualMemorySize { get; private set; }
        public ICollectorBuilder<IGauge> WorkingSet { get; private set; }
        public ICollectorBuilder<IGauge> PrivateMemorySize { get; private set; }
        public ICollectorBuilder<IGauge> OpenHandles { get; private set; }
        public ICollectorBuilder<IGauge> NumThreads { get; private set; }
        public ICollectorBuilder<IGauge> TotalMemory { get; private set; }

        private readonly Process _process;
        public ProcessCollectors(ICollectorFactory collectorFactory)
        {
            collectorFactory.InjectCollectors<IProcessCollectors, ProcessCollectors>(this);
            _process = Process.GetCurrentProcess();
            collectorFactory.AddBeforeCollectCallback(RefreshProcessCallback);

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            StartTime.WithLabels(GetLabels()).Set((_process.StartTime.ToUniversalTime() - epoch).TotalSeconds);
        }
        private string[] GetLabels()
        {
            return new string[] { _process.ProcessName };
        }

        public void RefreshProcessCallback()
        {
            try
            {
                _process.Refresh();

                for (var gen = 0; gen <= GC.MaxGeneration; gen++)
                {
                    var collectionCount = CollectionCounts.WithLabels(_process.ProcessName, gen.ToString());
                    collectionCount.Inc(GC.CollectionCount(gen) - collectionCount.Value);
                }

                TotalMemory.WithLabels(GetLabels()).Set(GC.GetTotalMemory(false));
                CpuTotal.WithLabels(GetLabels()).Inc(Math.Max(0, _process.TotalProcessorTime.TotalSeconds - CpuTotal.WithLabels(GetLabels()).Value));
                OpenHandles.WithLabels(GetLabels()).Set(_process.HandleCount);
                NumThreads.WithLabels(GetLabels()).Set(_process.Threads.Count);

                VirtualMemorySize.WithLabels(GetLabels()).Set(_process.VirtualMemorySize64);
                WorkingSet.WithLabels(GetLabels()).Set(_process.WorkingSet64);
                PrivateMemorySize.WithLabels(GetLabels()).Set(_process.PrivateMemorySize64);
            }
            catch (Exception)
            {
            }
        }
    }
}
