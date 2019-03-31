using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrometheusCore;
using System;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class CollectorsBuilderTest
    {
        private const string TEST_NAME = "Test";

      
        [TestMethod]
        public void Inject_Gauage()
        {
            //IServiceCollection services = new ServiceCollection();
            //services.AddMetrics();

            //IMetricsBuilder b = new MetricsBuilder(services);
            //b.Register<ITestInjection, TestInjection>().Build();

            //ServiceProvider serviceProvider = services.BuildServiceProvider();

            //var injected = serviceProvider.GetService<ITestInjection>();

            //Assert.IsNotNull(injected.Instance1);
            //Assert.IsNotNull(injected.Instance2);
            //Assert.IsNotNull(injected.Builder1);
            //Assert.IsNotNull(injected.Builder2);
        }
    }
}
