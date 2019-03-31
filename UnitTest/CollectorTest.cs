using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrometheusCore;
using System;

namespace UnitTest
{
    [TestClass]
    public class CollectorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Name_Invalid()
        {
            var collector = new Counter("\\//", new Labels(new string[0], new string[0]));
        }
        [TestMethod]
        public void To_String()
        {
            var collector = new Counter("Test", new Labels(new string[0], new string[0]));
            var result = collector.ToString();
            Assert.AreEqual("Test []", result);
        }
        [TestMethod]
        public void To_String_With_Params()
        {
            var collector = new Counter("Test", new Labels(new string[] { "Param1"}, new string[] { "Value1"}));
            var result = collector.ToString();
            Assert.AreEqual("Test [Param1=\"Value1\"]", result);
        }
        [TestMethod]
        public void To_String_With_Multiple_Params()
        {
            var collector = new Counter("Test", new Labels(new string[] { "Param1", "Param2" }, new string[] { "Value1", "Value2" }));
            var result = collector.ToString();
            Assert.AreEqual("Test [Param1=\"Value1\",Param2=\"Value2\"]", result);
        }
    }
}
