using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Utilities.Files
{   
    public class FileAssociationRegistration
    {
        public static void DoUrlProtocolRegistration(string protocol)
        {
            Logging.Info("Performing url protocol association for {0}.", protocol);

            using (RegistryKey key_software = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                using (RegistryKey key_classes = key_software.OpenSubKey("Classes", true))
                {
                    // Set up the Extension
                    using (RegistryKey key_protocol = key_classes.CreateSubKey(protocol))
                    {                        
                        key_protocol.SetValue("URL Protocol", "");

                        using (RegistryKey key_shell = key_protocol.CreateSubKey("shell"))
                        {
                            using (RegistryKey key_open = key_shell.CreateSubKey("open"))
                            {
                                using (RegistryKey key_command = key_open.CreateSubKey("command"))
                                {
                                    string binary_path = Application.ExecutablePath;
                                    key_command.SetValue("", String.Format("\"{0}\" \"%1\"", binary_path));                                    
                                }
                            }
                        }
                    }
                }
            }
        }


        
        public static void DoFileExtensionRegistration(string extension_including_dot, string prog_id, string shell_menu_prompt)
        {
            Logging.Info("Performing file association for {0}.", extension_including_dot);

            using (RegistryKey key_software = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                using (RegistryKey key_classes = key_software.OpenSubKey("Classes", true))
                {
                    // Set up the Extension
                    using (RegistryKey key_extension_including_dot = key_classes.CreateSubKey(extension_including_dot))
                    {
                        key_extension_including_dot.SetValue("", prog_id);
                    }

                    // Set up prog_id
                    CreateProgId(prog_id, shell_menu_prompt, key_classes);
                }
            }
        }

        private static void CreateProgId(string prog_id, string shell_menu_prompt, RegistryKey key_classes)
        {
            using (RegistryKey key_shell_menu_group_name = key_classes.CreateSubKey(prog_id))
            {
                key_shell_menu_group_name.SetValue("", prog_id + " file");

                string binary_path = Application.ExecutablePath;

                using (RegistryKey key_icon = key_shell_menu_group_name.CreateSubKey("DefaultIcon"))
                {
                    key_icon.SetValue("", String.Format("\"{0}\",0", binary_path));
                }

                using (RegistryKey key_shell = key_shell_menu_group_name.CreateSubKey("shell"))
                {
                    using (RegistryKey key_install = key_shell.CreateSubKey(shell_menu_prompt))
                    {
                        using (RegistryKey key_command = key_install.CreateSubKey("command"))

                            key_command.SetValue("", String.Format("\"{0}\" \"%1\"", binary_path));
                    }
                }
            }
        }

        private static bool JoinProgId(string prog_id, string shell_menu_prompt, RegistryKey key_classes)
        {
            using (RegistryKey key_shell_menu_group_name = key_classes.OpenSubKey(prog_id, true))
            {
                if (null != key_shell_menu_group_name)
                {
                    string binary_path = Application.ExecutablePath;

                    using (RegistryKey key_shell = key_shell_menu_group_name.CreateSubKey("shell"))
                    {
                        using (RegistryKey key_install = key_shell.CreateSubKey(shell_menu_prompt))
                        {
                            using (RegistryKey key_command = key_install.CreateSubKey("command"))

                                key_command.SetValue("", String.Format("\"{0}\" \"%1\"", binary_path));
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public static void DoWeakRegistration(string extension_including_dot, string prog_id, string shell_menu_prompt)
        {
            using (RegistryKey key_software = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                using (RegistryKey key_classes = key_software.OpenSubKey("Classes", true))
                {
                    using (RegistryKey key_prog_id = key_classes.CreateSubKey(extension_including_dot))
                    {
                        string associated_prog_id = key_prog_id.GetValue("") as string;
                        if (null != associated_prog_id)
                        {
                            bool success = false;
                            if (!success) success = JoinProgId(associated_prog_id, shell_menu_prompt, Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true));
                            if (!success) success = JoinProgId(associated_prog_id, shell_menu_prompt, Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true));
                        }
                    }
                }
            }
        }
    }
}
