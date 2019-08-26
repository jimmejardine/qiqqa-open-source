using System;
using System.IO;
using System.Reflection;

namespace Utilities.Misc
{
    public static class Runtime
    {
        public static string StartupDirectory
        {
            get
            {
                return Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath);
            }
        }

        public static bool IsRunningInVisualStudioDesigner
        {
            get
            {
#if DEBUG
                // Are we looking at this dialog in the Visual Studio Designer or Blend?
                string appname = System.Reflection.Assembly.GetEntryAssembly().FullName;
                bool rv = appname.Contains("XDesProc");
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                {
                    return true;
                }
#endif
                return false;
            }
        }
    }
}
