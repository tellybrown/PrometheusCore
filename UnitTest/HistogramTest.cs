using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrometheusCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public sealed class HistogramTest
    {
        private IHistogram Create()
        {
            return new Histogram("Test", Labels.Empty);
        }
        private IHistogram Create_With_Labels()
        {
            return new Histogram("Test", new Labels(new string[] { "TestName" }, new string[] { "TestValue" }));
        }
        [TestMethod]
        public void Observe_No_Labels()
        {
            var collector = Create();
            collector.Observe(2);
            Assert.AreEqual(1.0, collector.Count);
            Assert.AreEqual(2.0, collector.Sum);
            Assert.AreEqual(0.0, collector[0]); //.005
            Assert.AreEqual(1.0, collector[10]); //2.5

            collector.Observe(4);

            Assert.AreEqual(2.0, collector.Count);
            Assert.AreEqual(6.0, collector.Sum);
            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5
            Assert.AreEqual(2.0, collector[11]); //5
            Assert.AreEqual(2.0, collector[12]); //7.5
            Assert.AreEqual(2.0, collector[13]); //10
            Assert.AreEqual(2.0, collector[14]); //+inf
        }
        [TestMethod]
        public void Observe_Boundary_No_Labels()
        {
            var collector = Create();
            collector.Observe(2.5);
            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5

            collector.Observe(double.PositiveInfinity);

            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5
            Assert.AreEqual(1.0, collector[11]); //5
            Assert.AreEqual(1.0, collector[12]); //7.5
            Assert.AreEqual(1.0, collector[13]); //10
            Assert.AreEqual(2.0, collector[14]); //+inf
        }
        [TestMethod]
        public void Observe_With_Labels()
        {
            var collector = Create_With_Labels();
            collector.Observe(2);
            Assert.AreEqual(1.0, collector.Count);
            Assert.AreEqual(2.0, collector.Sum);
            Assert.AreEqual(0.0, collector[0]); //.005
            Assert.AreEqual(1.0, collector[10]); //2.5

            collector.Observe(4);

            Assert.AreEqual(2.0, collector.Count);
            Assert.AreEqual(6.0, collector.Sum);
            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5
            Assert.AreEqual(2.0, collector[11]); //5
            Assert.AreEqual(2.0, collector[12]); //7.5
            Assert.AreEqual(2.0, collector[13]); //10
            Assert.AreEqual(2.0, collector[14]); //+inf
        }
        [TestMethod]
        public void Observe_Boundary_With_Labels()
        {
            var collector = Create_With_Labels();
            collector.Observe(2.5);
            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5

            collector.Observe(double.PositiveInfinity);

            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5
            Assert.AreEqual(1.0, collector[11]); //5
            Assert.AreEqual(1.0, collector[12]); //7.5
            Assert.AreEqual(1.0, collector[13]); //10
            Assert.AreEqual(2.0, collector[14]); //+inf
        }
        [TestMethod]
        public async Task CollectAsync()
        {
            var collector = Create();
            collector.Observe(2.5);

            Assert.AreEqual(0.0, collector[9]); //1
            Assert.AreEqual(1.0, collector[10]); //2.5

            using (var stream = new MemoryStream())
            {
                await collector.SerializeAsync(stream);

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.005\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.01\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.025\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.05\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.075\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.1\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.25\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.5\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"0.75\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"1\"} 0");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"2.5\"} 1");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"5\"} 1");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"7.5\"} 1");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"10\"} 1");
                    await VerifyLineAsync(reader, "Test_bucket{le=\"+Inf\"} 1");
                    await VerifyLineAsync(reader, "Test_count 1");
                    await VerifyLineAsync(reader, "Test_sum 2.5");
                }
            }
        }
        private async Task VerifyLineAsync(StreamReader reader, string expected)
        {
            var line = await reader.ReadLineAsync();
            Assert.AreEqual(expected, line);
        }
    }
}
