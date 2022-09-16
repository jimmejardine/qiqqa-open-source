using System.Collections.Generic;
using System.Reflection;
using Utilities.Strings;

namespace Utilities.ClientVersioning
{
    /// <summary>
    /// Object representation of the file the client loads from the server to check version information details.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public class ClientVersionInformation
    {
        /// <summary>
        /// The latest version, this should always be increasing.
        /// </summary>
        public int LatestVersion;
        /// <summary>
        /// Previous version (inclusive) from which the latest version is similar - these won't trigger update request notifications.  If null, then all previous versions arent compliant.
        /// </summary>
        public int? CompliantFromVersion;
        /// <summary>
        /// Previous version (inclusive) from which Qiqqa is no longer supported.  If null, then all versions are valid and will be supported.
        /// </summary>
        public int? ObsoleteToVersion;
        /// <summary>
        /// Download location(s) for the latest version, should be at least one.
        /// </summary>
        public List<string> DownloadLocations;
        /// <summary>
        /// User-friendly release notes to show to the user.  Shouldn't be null or empty.
        /// </summary>
        public string ReleaseNotes;

        public override string ToString()
        {
            return string.Format("LatestVersion: {0}, CompliantFromVersion: {1}, ObsoleteToVersion: {2}, DownloadLocations: {3}", LatestVersion, CompliantFromVersion, ObsoleteToVersion, StringTools.ConcatenateStrings(DownloadLocations));
        }


        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestSerialize()
        {
            ClientVersionInformation test = new ClientVersionInformation
            {
                LatestVersion = 123,
                CompliantFromVersion = 120,
                ObsoleteToVersion = 10,
                DownloadLocations = new List<string> { "http://test.qiqqa.com/download/qiqqa-123.com", "http://test.download.other.com/qiqqa-123.com" },
                ReleaseNotes = "Version 123:\nChange #1\nChange#2\n\nVersion 122:\nChange #1"
            };
            new XmlSerializer(typeof(ClientVersionInformation)).Serialize(File.OpenWrite(@"C:\client.version.xml"), test);

            test.CompliantFromVersion = null;
            test.ObsoleteToVersion = null;
            new XmlSerializer(typeof(ClientVersionInformation)).Serialize(File.OpenWrite(@"C:\client.version2.xml"), test);
        }
#endif

        #endregion
    }
}
