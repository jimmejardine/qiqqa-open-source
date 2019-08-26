using System.Reflection;

namespace Utilities
{
    /// <summary>
    /// Uses the major revision number from the entry exe to determine the current version.
    /// </summary>
    public class ClientVersion
    {
        /// <summary>
        /// Produces the major revision number from the entry exe, i.e. "the current version".
        /// </summary>
        public static int CurrentVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.Major;
            }
        }

        /// <summary>
        /// Produces the full build version string from the entry exe, i.e. "the current build".
        /// </summary>
        public static string CurrentBuild
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }
    }
}
