using PrometheusCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PrometheusCore
{
    public interface ICollectorFactory
    {
        ICollectorBuilder<ICounter> GetCounter(string name);
        ICollectorBuilder<IGauge> GetGauge(string name);
        void AddBeforeCollectCallback(Action callback);

        IList<IBackground> Backgrounds { get; }
        ConcurrentBag<Action> BeforeCollectCallbacks { get; }
        Task CollectAsync(Stream stream);
        void InjectCollectors<I, T>(T t) where T : I;
    }
}