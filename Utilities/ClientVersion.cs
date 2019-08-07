using System.Reflection;

namespace Utilities
{
    /// <summary>
    /// Uses the major revision number from the entry exe to determine the current version.
    /// </summary>
    public class ClientVersion
    {
        public static int CurrentVersion
        {
            get { return Assembly.GetEntryAssembly().GetName().Version.Major; }
        }

        public static string CurrentBuild
        {
            get {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }
    }
}
