using System.Collections.Generic;
using System.Reflection;

namespace QiqqaLegacyFileFormats          // namespace Utilities.ClientVersioning
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
    }
}
