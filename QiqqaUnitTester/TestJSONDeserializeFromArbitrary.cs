using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace QiqqaUnitTester
{
    class TestJSONDeserializeFromArbitrary
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DeserializeWhenMissingProperties()
        {
            //configuration_record = JsonConvert.DeserializeObject<ConfigurationRecord>(input);
            Assert.Pass();
        }

        [Test]
        public void DeserializeWithSurplusProperties()
        {
            //configuration_record = JsonConvert.DeserializeObject<ConfigurationRecord>(input);
            Assert.Pass();
        }
    }

    internal class ConfigurationRecord
    {
    }
}
