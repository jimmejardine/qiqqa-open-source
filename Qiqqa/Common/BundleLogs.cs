using System;
using System.Diagnostics;
using Microsoft.Win32;
using Qiqqa.Common.Configuration;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
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
            string target_filename = null;

            try
            {
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
                    target_filename = save_file_dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem zipping logs");
                MessageBoxes.Error("Unfortunately there was a problem creating the log bundle. Please zip them manually, they are found at C:\\Temp\\Qiqqa.log*. There may be more than one. Thanks!");
                target_filename = null;
            }

            if (target_filename != null)
            {
                int progress = 1;
                int wait_period = 300;
                const int MAX_PROGRESS = 100;
                StatusManager.Instance.UpdateStatus("LogBundler", "Bundling the logfile. Please wait...", progress, MAX_PROGRESS);

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    string environment_details_filename = null;

                    try
                    {
                        // Delete the target filename if it exists...
                        FileTools.Delete(target_filename);

                        // Note: Path.GetFullPath() throws an exception when you feed it wildcards, e.g. '*'
                        // hence we construct the search path in two steps:
                        const string MAGIC_FILENAME = @"QQQ";
                        string file_list = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), @"Quantisle/Qiqqa/Logs", MAGIC_FILENAME));
                        // and then we replace the 'magic file name' with the real thing:
                        file_list = $"\"{Path.GetSuffixedDirectoryName(file_list)}Qiqqa*.log*\"";

                        // Try get this info for us which could be useful...
                        // If things are really broken, it may fail, that's ok.
                        try
                        {
                            string environmentDetails = "Generated at:" + DateTime.UtcNow.ToString("yyyyMMdd HH:mm:ss") + Environment.NewLine;
                            environmentDetails += ComputerStatistics.GetCommonStatistics();
                            environmentDetails += "\r\nConfiguration Bits:\r\n";
                            environmentDetails += $"Background Tasks:      {(ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks ? "Disabled ALL" : "Normal (Enabled)")}\r\n";
                            environmentDetails += $"Library OCR Task:      {(ConfigurationManager.Instance.ConfigurationRecord.Library_OCRDisabled ? "Disabled" : "Normal (Enabled)")}\r\n";

                            environment_details_filename = TempFile.GenerateTempFilename("txt");
                            File.WriteAllText(environment_details_filename, environmentDetails);
                        }
                        catch (Exception ex)
                        {
                            Logging.Warn(ex, "Could not get environment details");
                        }

                        if (environment_details_filename != null)
                        {
                            file_list += " \"" + environment_details_filename + "\"";
                        }

                        // STDOUT/STDERR
                        string process_parameters = String.Format("a -t7z -mmt=on -mx9 -ssw \"{0}\" {1}", target_filename, file_list);
                        Logging.Info($"Bundling the logfiles via command:\n    {ConfigurationManager.Instance.Program7ZIP}  {process_parameters}");
                        using (Process process = ProcessSpawning.SpawnChildProcess(ConfigurationManager.Instance.Program7ZIP, process_parameters, ProcessPriorityClass.Normal))
                        {
                            using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                            {
                                while (!process.WaitForExit(wait_period))
                                {
                                    progress = Math.Min(MAX_PROGRESS - 5, progress + 1);
                                    if (progress >= 25)
                                    {
                                        wait_period = 1000;
                                    }

                                    StatusManager.Instance.UpdateStatus("LogBundler", "Bundling the logfile. Please wait...", progress, MAX_PROGRESS);
                                }

                                Logging.Info("7ZIP Log Bundling progress:\n{0}", process_output_reader.GetOutputsDumpString());
                            }

                            MessageBoxes.Info($"The Qiqqa logs with some diagnostic info have been zipped to the location you specified:\n{target_filename}\n\nPlease upload it as issue attachment in your issue filed at https://github.com/jimmejardine/qiqqa-open-source/issues if the support team has requested it. Many thanks!");
                            FileTools.BrowseToFileInExplorer(target_filename);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "Problem zipping logs");
                        MessageBoxes.Error("Unfortunately there was a problem creating the log bundle. Please zip them manually, they are found at C:\\Temp\\Qiqqa.log*. There may be more than one. Thanks!");
                        target_filename = null;
                    }

                    if (environment_details_filename != null)
                    {
                        FileTools.Delete(environment_details_filename);
                    }
                });
            }
        }
    }
}
