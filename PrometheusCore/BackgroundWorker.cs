using Microsoft.Extensions.DependencyInjection;
using PrometheusCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusCore
{
    public interface IBackgroundWorker
    {
        void Load();
    }
    public class BackgroundWorker : IBackgroundWorker
    {
        private readonly IServiceProvider _provider;
        private readonly ICollectorFactory _factory;
        private readonly IList<object> _objects;

        public BackgroundWorker(IServiceProvider provider, ICollectorFactory factory)
        {
            _provider = provider;
            _factory = factory;
            _objects = new List<object>();
        }

        public void Load()
        {
            foreach(var background in _factory.Backgrounds)
            {
                _objects.Add(_provider.GetService(background.Type));
            }
        }
    }
}
