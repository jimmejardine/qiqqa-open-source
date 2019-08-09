using System;
using System.IO;
using System.Reflection;

namespace Utilities.Misc
{
    public static class Runtime
    {
        public static string StartupDirectory
        {
            get {
                return Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath);
            }
        }

        public static bool IsRunningInVisualStudioDesigner
        {
            get
            {
                // Are we looking at this dialog in the Visual Studio Designer?
                string appname = System.Reflection.Assembly.GetEntryAssembly().FullName;
                return appname.Contains("XDesProc");
            }
        }
    }
}
