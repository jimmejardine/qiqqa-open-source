using System.Globalization;
using Microsoft.Win32;

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

        public void Write(string key, string data)
        {
            using (RegistryKey app_key = GetAppKey())
            {
                app_key.SetValue(key, data);
                app_key.Close();
            }
        }

        public string Read(string key)
        {
            using (RegistryKey app_key = GetAppKey())
            {
                string data = (string)app_key.GetValue(key);
                app_key.Close();

                if (null == data) data = "";
                return data;
            }
        }

        public bool IsSet(string key)
        {
            string value = Read(key);
            if (null == key) return false;
            value = value.ToLower(CultureInfo.CurrentCulture);
            if (0 == value.CompareTo("y")) return true;
            if (0 == value.CompareTo("t")) return true;
            if (0 == value.CompareTo("yes")) return true;
            if (0 == value.CompareTo("true")) return true;
            return false;
        }

        public void Write(string section, string key, string data)
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

        public string Read(string section, string key)
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
    }
}
