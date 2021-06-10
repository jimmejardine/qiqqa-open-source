using System.Globalization;
using Microsoft.Win32;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Misc
{
    public class UserRegistry
    {
        private string company_name;
        private string app_name;

        public UserRegistry(string company_name, string app_name)
        {
            this.company_name = company_name;
            this.app_name = app_name;
        }

        private RegistryKey GetAppKey()
        {
            return Registry.CurrentUser.CreateSubKey("Software").CreateSubKey(company_name).CreateSubKey(app_name);
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
                return "abacadabra";
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
                if (key == "DebugConsole")
                    return "false";
                if (key == "AllowMultipleQiqqaInstances")
                    return "true";
                if (key == "BaseDataDirectory")
                    return Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"../My.Qiqqa.Libraries"));
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
                return "";
            }
        }

        // ----------------------------------------------

        private static bool portable_mode = false;

        public void SetPortableApplicationMode(string cfg_file_path)
        {
            portable_mode = true;
        }

        public bool GetPortableApplicationMode()
        {
            return portable_mode;
        }
    }
}
