using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrometheusCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class CounterTest
    {
        private ICounter Create()
        {
            return new Counter("Test", Labels.Empty);
        }
        private ICounter Create_With_Labels()
        {
            return new Counter("Test", new Labels(new string[] { "TestName" }, new string[] { "TestValue" }));
        }
        [TestMethod]
        public void Increment_No_Labels()
        {
            var collector = Create();

            collector.Inc();
            Assert.AreEqual(1.0, collector.Value);
            collector.Inc(2);
            Assert.AreEqual(3.0, collector.Value);
            collector.Inc(4);
            Assert.AreEqual(7.0, collector.Value);
            collector.Inc();
            Assert.AreEqual(8.0, collector.Value);
        }
        [TestMethod]
        public void Increment_With_Labels()
        {
            var collector = Create_With_Labels();

            collector.Inc();
            Assert.AreEqual(1.0, collector.Value);
            collector.Inc(2);
            Assert.AreEqual(3.0, collector.Value);
            collector.Inc(4);
            Assert.AreEqual(7.0, collector.Value);
            collector.Inc();
            Assert.AreEqual(8.0, collector.Value);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Increment_Negative()
        {
            var collector = Create();

            collector.Inc(-1);
        }
        [TestMethod]
        public async Task CollectAsync()
        {
            var collector = Create();

            collector.Inc();
            Assert.AreEqual(1.0, collector.Value);

            using (var stream = new MemoryStream())
            {
                await collector.SerializeAsync(stream);

                string result = System.Text.Encoding.ASCII.GetString(stream.ToArray());

                Assert.AreEqual("Test 1\n", result);
            }
        }
    }
}
