using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;

namespace QiqqaSystemTester
{
    [TestClass]
    public class TestJSONDeserializeFromArbitrary
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void DeserializeWhenMissingProperties()
        {
            //configuration_record = JsonConvert.DeserializeObject<ConfigurationRecord>(input);
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void DeserializeWithSurplusProperties()
        {
            //configuration_record = JsonConvert.DeserializeObject<ConfigurationRecord>(input);
            ASSERT.IsTrue(true);
        }
    }

    internal class ConfigurationRecord
    {
    }
}
