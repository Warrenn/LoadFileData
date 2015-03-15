using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFileData.Tests.Controllers
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            KeyValuePair<string, object> d = new KeyValuePair<string, object>();
            var df = default(KeyValuePair<string, object>);
            var e = default(IEnumerable<string>);
            Assert.AreEqual(df, d);
        }
    }
}
