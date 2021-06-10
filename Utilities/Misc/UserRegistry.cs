using System.Collections.Generic;
using System.Globalization;
using Microsoft.Win32;
using Utilities.Shutdownable;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Misc
{
    public class UserRegistry
    {
        private string company_name;
        private string app_name;

        public UserRegistry(string company_name, string app_name)
        {
            ShutdownableManager.Instance.Register(Shutdown);

            this.company_name = company_name;
            this.app_name = app_name;
        }

        private RegistryKey GetAppKey()
        {
            if (!portable_mode)
            {
                return Registry.CurrentUser.CreateSubKey("Software").CreateSubKey(company_name).CreateSubKey(app_name);
            }
            else
            {
                return null;
            }
        }

        public string AppKeyDescription()
        {
            if (!portable_mode)
            {
                using (RegistryKey app_key = GetAppKey())
                {
                    return app_key.ToString();
                }
            }
            else
            {
                return "PortableApplication";
            }
        }

        public void Write(string key, string data)
        {
            if (!portable_mode)
            {
                using (RegistryKey app_key = GetAppKey())
                {
                    app_key.SetValue(key, data);
                    app_key.Close();
                }
            }
            else
            {
                if (DeveloperOverridesDB.TryGetValue(key, out object old_v))
                {
                    string old_data = old_v as string;
                    if (old_data == data)
                        return;
                    DeveloperOverridesDB.Remove(key);
                }
                DeveloperOverridesDB.Add(key, data);
                registry_overrides_db_dirty = true;
            }
        }

        public string Read(string key)
        {
            if (!portable_mode)
            {
                using (RegistryKey app_key = GetAppKey())
                {
                    string data = (string)app_key.GetValue(key);
                    app_key.Close();

                    if (null == data) data = "";
                    return data;
                }
            }
            else
            {
                if (DeveloperOverridesDB.TryGetValue(key, out object v))
                {
                    string data = v as string;
                    if (null == data) data = "";
                    return data;
                }
                return "";
            }
        }

        public bool IsSet(string key)
        {
            string value = Read(key);
            if (null == key) return false;
            value = value.ToLower();
            if (0 == value.CompareTo("y")) return true;
            if (0 == value.CompareTo("t")) return true;
            if (0 == value.CompareTo("yes")) return true;
            if (0 == value.CompareTo("true")) return true;
            return false;
        }

        public void Write(string section, string key, string data)
        {
            if (!portable_mode)
            {
                using (RegistryKey app_key = GetAppKey())
                {
                    using (RegistryKey sub_app_key = app_key.CreateSubKey(section))
                    {
                        sub_app_key.SetValue(key, data);
                        sub_app_key.Close();
                    }
                }
            }
            else
            {
                string compound_key = $"{ section }/{ key }";
                if (DeveloperOverridesDB.TryGetValue(compound_key, out object old_v))
                {
                    string old_data = old_v as string;
                    if (old_data == data)
                        return;
                    DeveloperOverridesDB.Remove(compound_key);
                }
                DeveloperOverridesDB.Add(compound_key, data);
                registry_overrides_db_dirty = true;
            }
        }

        public string Read(string section, string key)
        {
            if (!portable_mode)
            {
                using (RegistryKey app_key = GetAppKey())
                {
                    using (RegistryKey sub_app_key = app_key.CreateSubKey(section))
                    {
                        string data = (string)sub_app_key.GetValue(key);
                        sub_app_key.Close();

                        if (null == data) data = "";
                        return data;
                    }
                }
            }
            else
            {
                string compound_key = $"{ section }/{ key }";
                if (DeveloperOverridesDB.TryGetValue(compound_key, out object v))
                {
                    string data = v as string;
                    if (null == data) data = "";
                    return data;
                }
                return "";
            }
        }

        // ----------------------------------------------

        protected static bool portable_mode = false;
        protected static Dictionary<string, object> registry_overrides_db = null;
        protected static bool registry_overrides_db_dirty = false;

        public static void DetectPortableApplicationMode()
        {
            if (registry_overrides_db == null)
            {
                portable_mode = UnitTestDetector.HasPortableApplicationConfigFilename();
                registry_overrides_db = UnitTestDetector.LoadDeveloperConfiguration();
            }
        }

        public static bool GetPortableApplicationMode()
        {
            return portable_mode;
        }

        public static Dictionary<string, object> DeveloperOverridesDB
        {
            get
            {
                return registry_overrides_db != null ? registry_overrides_db : new Dictionary<string, object>();
            }
        }

        private void Shutdown()
        {
            if (portable_mode && registry_overrides_db_dirty)
            {
                Logging.Info("UserRegistry is saving the (modified) configuration at (portable application) shutdown");
                SavePortableApplicationSettings();
            }
        }

        public static void SavePortableApplicationSettings()
        {
            UnitTestDetector.SavePortableApplicationConfiguration(registry_overrides_db);
            registry_overrides_db_dirty = false;
        }
    }
}
