using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QiqqaSystemTester
{
    [TestClass]
    class TestJSONDeserializeFromArbitrary
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void DeserializeWhenMissingProperties()
        {
            //configuration_record = JsonConvert.DeserializeObject<ConfigurationRecord>(input);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DeserializeWithSurplusProperties()
        {
            //configuration_record = JsonConvert.DeserializeObject<ConfigurationRecord>(input);
            Assert.IsTrue(true);
        }
    }

    internal class ConfigurationRecord
    {
    }
}
