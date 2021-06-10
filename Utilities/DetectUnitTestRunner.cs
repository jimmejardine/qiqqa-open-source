using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities
{
    /// <summary>
    /// Determine if we're running/debugging a Unit/System Test or running an actual application.
    ///
    /// Inspired by https://stackoverflow.com/questions/3167617/determine-if-code-is-running-as-part-of-a-unit-test
    /// </summary>
    public static class UnitTestDetector
    {
        internal static readonly HashSet<string> UnitTestAttributes = new HashSet<string>
        {
            "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute",
            "NUnit.Framework.TestFixtureAttribute",
        };

        private static int _runningFromNUnitHeuristic = 0;

        public static bool IsRunningInUnitTest
        {
            get
            {
                // are we certain already or do we want to collect more datums still?
                if (_runningFromNUnitHeuristic <= -50 || _runningFromNUnitHeuristic >= 50)
                {
                }
                else
                {
                    bool hit = false;
                    foreach (System.Reflection.Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        // Can't do something like this as it will load the nUnit assembly
                        // if (assem == typeof(NUnit.Framework.Assert))

                        string a = assem.FullName.ToLowerInvariant();
                        if (a.StartsWith("nunit.framework"))
                        {
                            hit = true;
                            break;
                        }
                        if (a.StartsWith("microsoft.testplatform"))
                        {
                            hit = true;
                            break;
                        }
                        if (a.StartsWith("microsoft.visualstudio.testplatform"))
                        {
                            hit = true;
                            break;
                        }
                    }

                    bool in_test_hit = false;
                    foreach (var f in new System.Diagnostics.StackTrace().GetFrames())
                    {
                        var l = f.GetMethod().DeclaringType.GetCustomAttributes(false);
                        List<object> lst = new List<object>(l);
                        if (lst.Any(x => UnitTestAttributes.Contains(x.GetType().FullName)))
                        {
                            in_test_hit = true;
                            break;
                        }
                    }

                    _runningFromNUnitHeuristic += (!hit && !in_test_hit) ? -10 : ((hit ? 11 : 0) + (in_test_hit ? 20 : 0));
                }

                return (_runningFromNUnitHeuristic > 0);
            }

            set => _runningFromNUnitHeuristic = (value ? -100 : +100);
        }

        private static readonly Lazy<string> _StartupDirectoryForQiqqa = new Lazy<string>(() =>
        {
            // Are we looking at this dialog in the Visual Studio Designer?
            if (Runtime.IsRunningInVisualStudioDesigner)
            {
                string loc = Path.Combine(Utilities.Constants.QiqqaDevSolutionDir, "Qiqqa/bin/", Utilities.Constants.QiqqaDevBuild);
                string basedir = Path.GetFullPath(loc);
                return basedir;
            }

            // are we certain already or do we want to collect more datums still?
            if (_runningFromNUnitHeuristic <= -50 || _runningFromNUnitHeuristic >= 50)
            {
                return null;
            }
            else
            {
                // https://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
                //string s1 = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                string loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //string s3 = System.Windows.Forms.Application.StartupPath;
                bool we_are_in_test_env = UnitTestDetector.IsRunningInUnitTest;
                string basedir = Path.GetFullPath(Path.GetDirectoryName(Path.GetFullPath(loc)));
                //we_are_in_test_env = (basedir.ToLowerInvariant() != s3.ToLowerInvariant());
                if (we_are_in_test_env)
                {
                    return basedir;
                }
                else
                {
                    return Path.GetFullPath(System.Windows.Forms.Application.StartupPath);
                }
            }
        });
        public static string StartupDirectoryForQiqqa => _StartupDirectoryForQiqqa.Value;
    }
}
