using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrometheusCore;
using System;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class CollectorFactoryTest
    {
        private ICollectorFactory Create()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddPrometheus()
                    .RegisterCounter("TestCounter", "test")
                    .RegisterGauge("TestGauge", "help instance2")
                    .Build();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            return  serviceProvider.GetService<ICollectorFactory>();
        }
        private ICollectorFactory Create_With_Labels()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddPrometheus()
                    .RegisterCounter("TestCounter", "test", "Label1")
                    .RegisterGauge("TestGauge", "help instance2", "Label2")
                    .Build();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetService<ICollectorFactory>();
        }
        [TestMethod]
        public void Get_Counter()
        {
            var factory = Create();

            var builder = factory.GetCounter("TestCounter");
            var counter = builder.WithLabels();
            Assert.IsNotNull(builder);
            Assert.IsNotNull(counter);
            Assert.AreEqual(typeof(Counter), counter.GetType());
        }
        [TestMethod]
        public void Get_Gauge()
        {
            var factory = Create();

            var builder = factory.GetGauge("TestGauge");
            var gauge = builder.WithLabels();
            Assert.IsNotNull(builder);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(typeof(Gauge), gauge.GetType());
        }

        [TestMethod]
        public void Get_Counter_With_Label()
        {
            var factory = Create_With_Labels();

            var builder = factory.GetCounter("TestCounter");
            var counter = builder.WithLabels("Label Value 1");
            Assert.IsNotNull(builder);
            Assert.IsNotNull(counter);
            Assert.AreEqual(typeof(Counter), counter.GetType());
        }
        [TestMethod]
        public void Get_Gauge_With_Label()
        {
            var factory = Create_With_Labels();

            var builder = factory.GetGauge("TestGauge");
            var gauge = builder.WithLabels("Label Value 2");
            Assert.IsNotNull(builder);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(typeof(Gauge), gauge.GetType());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Get_Counter_With_Label_Exception()
        {
            var factory = Create_With_Labels();

            var builder = factory.GetCounter("TestCounter");
            var counter = builder.WithLabels();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Get_Gauge_With_Label_Exception()
        {
            var factory = Create_With_Labels();

            var builder = factory.GetGauge("TestGauge");
            var gauge = builder.WithLabels();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Get_Gauge_Unregistered_Exception()
        {
            var factory = Create();

            var builder = factory.GetGauge("Bad Name");
            var gauge = builder.WithLabels();
        }
        [TestMethod]
        public void Get_Gauge_Cached()
        {
            var factory = Create();

            var builder = factory.GetGauge("TestGauge");
            var gauge = builder.WithLabels();
            Assert.IsNotNull(builder);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(typeof(Gauge), gauge.GetType());

            gauge = builder.WithLabels();
            Assert.IsNotNull(builder);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(typeof(Gauge), gauge.GetType());
        }

        [TestMethod]
        public void Get_Gauge_With_Label_Cached()
        {
            var factory = Create_With_Labels();

            var builder = factory.GetGauge("TestGauge");
            var gauge = builder.WithLabels("Label Value 2");
            Assert.IsNotNull(builder);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(typeof(Gauge), gauge.GetType());

            gauge = builder.WithLabels("Label Value 2");
            Assert.IsNotNull(builder);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(typeof(Gauge), gauge.GetType());
        }

        [TestMethod]
        public void Add_Callback()
        {
            var factory = Create();

            factory.AddBeforeCollectCallback(() => { Console.WriteLine(); });

            Assert.AreEqual(1, factory.BeforeCollectCallbacks.Count);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Callback_Exception()
        {
            var factory = Create();

            factory.AddBeforeCollectCallback(null);
        }

        [TestMethod]
        public void Inject_With_AutoRegister()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddPrometheus()
                    .Register<ITestInjection, TestInjection>().Build();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var injected = serviceProvider.GetService<ITestInjection>();

            Assert.IsNotNull(injected.Instance1);
            Assert.IsNotNull(injected.Instance2);
            Assert.IsNotNull(injected.Builder1);
            Assert.IsNotNull(injected.Builder2);
        }
        [TestMethod]
        public void Inject_With_Manual_Register()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddPrometheus()
                    .RegisterCounter("instance1", "help instance1")
                    .RegisterCounter("instance2", "help instance2", "testinstance2")
                    .RegisterCounter("builder1", "help builder1")
                    .RegisterCounter("builder2", "help builder2", "testbuilder2")
                    .Build();

            services.AddSingleton<ITestInjection, TestInjection>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var injected = serviceProvider.GetService<ITestInjection>();

            Assert.IsNotNull(injected.Instance1);
            Assert.IsNotNull(injected.Instance2);
            Assert.IsNotNull(injected.Builder1);
            Assert.IsNotNull(injected.Builder2);
        }
        [TestMethod]
        public void Manual_Objects()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddPrometheus()
                    .RegisterCounter("instance1", "help instance1")
                    .RegisterCounter("instance2", "help instance2", "testinstance2")
                    .RegisterCounter("builder1", "help builder1")
                    .RegisterCounter("builder2", "help builder2", "testbuilder2")
                    .Build();

            services.AddSingleton<ITestInjection, TestInjection>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetService<ICollectorFactory>();

            var Instance1 = factory.GetCounter("instance1").WithLabels();
            var Instance2 = factory.GetCounter("instance2").WithLabels("testvalue1");
            var Builder1 = factory.GetCounter("builder1").WithLabels();
            var Builder2 = factory.GetCounter("builder2").WithLabels("testvalue1");

            Assert.IsNotNull(Instance1);
            Assert.IsNotNull(Instance2);
            Assert.IsNotNull(Builder1);
            Assert.IsNotNull(Builder2);
        }
        public interface ITestInjection
        {
            [CollectorRegistry("instance1", "help instance1")]
            ICounter Instance1 { get; }

            [CollectorRegistry("builder1", "help builder1")]
            ICollectorBuilder<ICounter> Builder1 { get; }

            [CollectorRegistry("instance2", "help instance2", "testinstance2")]
            ICounter Instance2 { get; }

            [CollectorRegistry("builder2", "help builder2", "testbuilder2")]
            ICollectorBuilder<ICounter> Builder2 { get; }
        }

        public class TestInjection : ITestInjection
        {
            public ICounter Instance1 { get; private set; }

            public ICollectorBuilder<ICounter> Builder1 { get; private set; }

            [CollectorInstance("testvalue1")]
            public ICounter Instance2 { get; private set; }

            public ICollectorBuilder<ICounter> Builder2 { get; private set; }

            public TestInjection(ICollectorFactory collectorFactory)
            {
                collectorFactory.InjectCollectors<ITestInjection, TestInjection>(this);
            }
        }
    }
}
