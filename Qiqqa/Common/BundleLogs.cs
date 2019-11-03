using System;
using System.Diagnostics;
using Microsoft.Win32;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.ProcessTools;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Common
{
    internal class BundleLogs
    {
        internal static void DoBundle()
        {
            try
            {
                // Try get this info for us which could be useful...
                // If things are really broken, it may fail, that's ok. 
                string environment_details_filename = null;
                try
                {
                    string environmentDetails = "Generated at:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss") + Environment.NewLine;
                    environmentDetails += ComputerStatistics.GetCommonStatistics();
                    environment_details_filename = TempFile.GenerateTempFilename("txt");
                    File.WriteAllText(environment_details_filename, environmentDetails);
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "Could not get environment details");
                }

                // Get the destination location
                SaveFileDialog save_file_dialog = new SaveFileDialog();
                save_file_dialog.AddExtension = true;
                save_file_dialog.CheckPathExists = true;
                save_file_dialog.DereferenceLinks = true;
                save_file_dialog.OverwritePrompt = true;
                save_file_dialog.ValidateNames = true;
                save_file_dialog.DefaultExt = "7z";
                save_file_dialog.Filter = "7Z files (*.7z)|*.7z|All files (*.*)|*.*";
                save_file_dialog.FileName = "QiqqaLogs.7z";

                // Generate and save
                if (true == save_file_dialog.ShowDialog())
                {
                    string target_filename = save_file_dialog.FileName;

                    string file_list = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), @"Quantisle/Qiqqa/Logs", @"Qiqqa*.log*"));
                    if (environment_details_filename != null)
                    {
                        file_list += " \"" + environment_details_filename + "\"";
                    }

                    // Delete the target filename if it exists...
                    FileTools.Delete(target_filename);

                    // STDOUT/STDERR
                    string process_parameters = String.Format("a -t7z -mmt=on -mx9 -ssw \"{0}\" \"{1}\"", target_filename, file_list);
                    using (Process process = ProcessSpawning.SpawnChildProcess(ConfigurationManager.Instance.Program7ZIP, process_parameters, ProcessPriorityClass.Normal))
                    {
                        using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                        {
                            process.WaitForExit();

                            Logging.Info("7ZIP Log Bundling progress:\n{0}", process_output_reader.GetOutputsDumpString());
                        }

                        MessageBoxes.Info("The Qiqqa logs with some diagnostic info have been zipped to the location you specified. Please upload it as issue attachment in your issue filed at https://github.com/jimmejardine/qiqqa-open-source/issues if the support team has requested it. Many thanks!");
                        FileTools.BrowseToFileInExplorer(target_filename);
                    }
                }

                FileTools.Delete(environment_details_filename);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem zipping logs");
                MessageBoxes.Error("Unfortunately there was a problem creating the log bundle. Please zip them manually, they are found at C:\\Temp\\Qiqqa.log*. There may be more than one. Thanks!");
            }
        }
    }
}
