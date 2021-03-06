﻿namespace PrometheusCore.Collectors
{
    public interface IProcessCollectors
    {
        [CollectorRegistry("dotnet_collection_count_total", "GC collection count", "process", "generation")]
        ICollectorBuilder<ICounter> CollectionCounts { get; }
        [CollectorRegistry("process_start_time_seconds", "Start time of the process since unix epoch in seconds.", "process")]
        ICollectorBuilder<IGauge> StartTime { get; }
        [CollectorRegistry("process_cpu_seconds_total", "Total user and system CPU time spent in seconds.", "process")]
        ICollectorBuilder<ICounter> CpuTotal { get; }
        [CollectorRegistry("process_virtual_memory_bytes", "Virtual memory size in bytes.", "process")]
        ICollectorBuilder<IGauge> VirtualMemorySize { get; }
        [CollectorRegistry("process_working_set_bytes", "Process working set", "process")]
        ICollectorBuilder<IGauge> WorkingSet { get; }
        [CollectorRegistry("process_private_memory_bytes", "Process private memory size", "process")]
        ICollectorBuilder<IGauge> PrivateMemorySize { get; }
        [CollectorRegistry("process_open_handles", "Number of open handles", "process")]
        ICollectorBuilder<IGauge> OpenHandles { get; }
        [CollectorRegistry("process_num_threads", "Total number of threads", "process")]
        ICollectorBuilder<IGauge> NumThreads { get; }
        [CollectorRegistry("dotnet_total_memory_bytes", "Total known allocated memory", "process")]
        ICollectorBuilder<IGauge> TotalMemory { get; }
    }
}
