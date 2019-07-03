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
    }
}
