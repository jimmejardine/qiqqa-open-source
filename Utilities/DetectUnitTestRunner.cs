using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public static string PortableApplicationConfigFilename => Path.Combine(StartupDirectoryForQiqqa, @"Qiqqa.Portable.Settings.json5");

        public static string DeveloperTestSettingsFilename_1_App => Path.Combine(StartupDirectoryForQiqqa, @"Qiqqa.Developer.Settings.json5");


        // TODO: refactor the next bit; plonking it in here for now until I know how I want to untangle the otherwise circular dependencies of Utilities <-> Qiqqa namespaces.


        public static bool HasPortableApplicationConfigFilename()
        {
            return File.Exists(PortableApplicationConfigFilename);
        }

        public static Dictionary<string, object> LoadDeveloperConfiguration()
        {
            // Procedure:
            // - load PortableApplication config record, if available
            // - load application-level developer config record, if available: override existing entries
            // - load BaseDirectory-level developer config record, if available: override existing entries

            Dictionary<string, object> cfg = LoadConfigFile(PortableApplicationConfigFilename);
            foreach (KeyValuePair<string, object> entry in LoadConfigFile(DeveloperTestSettingsFilename_1_App))
            {
                if (cfg.ContainsKey(entry.Key))
                {
                    cfg.Remove(entry.Key);
                }
                cfg.Add(entry.Key, entry.Value);
            }
            return cfg;
        }

        public static void AugmentDeveloperConfiguration(ref Dictionary<string, object> cfg, string extra_config_filepath)
        {
            foreach (KeyValuePair<string, object> entry in LoadConfigFile(extra_config_filepath))
            {
                if (cfg.ContainsKey(entry.Key))
                {
                    cfg.Remove(entry.Key);
                }
                cfg.Add(entry.Key, entry.Value);
            }
        }

        private static Dictionary<string, object> LoadConfigFile(string cfg_filepath)
        {
            try
            {
                if (!String.IsNullOrEmpty(cfg_filepath) && File.Exists(cfg_filepath))
                {
                    Logging.Info("Loading developer test settings file {0}", cfg_filepath);

                    // see also https://www.newtonsoft.com/json/help/html/SerializationErrorHandling.htm

                    List<string> errors = new List<string>();

                    Dictionary<string, object> developer_test_settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                        File.ReadAllText(cfg_filepath),
                        new JsonSerializerSettings
                        {
                            Error = delegate (object sender, ErrorEventArgs args)
                            {
                                errors.Add(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            },
                            //Converters = { new IsoDateTimeConverter() }
                        });

                    Logging.Info("Loaded developer test settings file {0}: {1}", cfg_filepath, errors.Count == 0 ? "no errors" : errors.ToString());

                    return developer_test_settings;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem loading developer test settings file {0}", cfg_filepath);
            }

            return new Dictionary<string, object>();
        }

        public static void SavePortableApplicationConfiguration(Dictionary<string, object> cfg)
        {
            string cfg_filepath = PortableApplicationConfigFilename;
            // UnitTestDetector.SavePortableApplicationConfiguration(registry_overrides_db);

            try
            {
                // only save config to REPLACE an already existing config file
                if (!String.IsNullOrEmpty(cfg_filepath) && File.Exists(cfg_filepath))
                {
                    Logging.Info("Saving portable application settings file {0}", cfg_filepath);

#if false
                    // Note: only replace/update existing config entries; anything else should have landed
                    // in the Developer Test config file(s).
                    //
                    // The exceptions to this rule are ... TODO
                    Dictionary<string, object> old_cfg = LoadConfigFile(cfg_filepath);

                    //... TODO
#endif

                    string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);

                    // keep all comments in the JSON5 file which precede the config data:
                    string[] old_json = File.ReadAllLines(cfg_filepath);
                    int i;
                    for (i = 0; i < old_json.Length; i++)
                    {
                        if (old_json[i].StartsWith("{"))
                            break;
                    }
                    string header = "";
                    if (i > 0)
                    {
                        header = String.Join("\n", old_json, 0, i) + "\n";
                    }
                    json = header + json;

                    string new_filepath = cfg_filepath + ".new";
                    File.WriteAllText(new_filepath, json);
                    File.Delete(cfg_filepath);
                    File.Move(new_filepath, cfg_filepath);

                    Logging.Info("Saved portable application settings file {0}", cfg_filepath);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem saving portable application settings file {0}", cfg_filepath);
            }
        }
    }
}
