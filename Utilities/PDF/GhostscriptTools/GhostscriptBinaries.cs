using System;
using System.Diagnostics;
using System.IO;
using Utilities.ProcessTools;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.PDF.GhostscriptTools
{
    public class GhostscriptBinaries
    {
        public static bool AreInstalled => (null != LocatedPath);

        public static string ExecutablePath
        {
            get
            {
                if (null != LocatedPath)
                {
                    return LocatedPath;
                }
                else
                {
                    throw new Exception("Unable to locate Ghostscript");
                }
            }
        }

        private static string located_path = null;

        private static string LocatedPath
        {
            get
            {
                if (null == located_path)
                {
                    // First try the predefined ghostscript directories
                    foreach (string potential_directory in potential_directories)
                    {
                        Logging.Info("Searching for Ghostscript in {0}", potential_directory);

                        try
                        {
                            if (Directory.Exists(potential_directory))
                            {
                                string[] potential_binaries = Directory.GetFiles(potential_directory, "gswin32c.exe", SearchOption.AllDirectories);
                                if (0 != potential_binaries.Length)
                                {
                                    located_path = potential_binaries[0];
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "Exception while trying to locate Ghostscript in the predefined locations");
                        }
                    }
                }

                if (null == located_path)
                {
                    // If we get this far, it is not in the standard place...so ask the user
                }

                if (null != located_path)
                {
                    Logging.Info("Found ghostscript at {0}", located_path);
                }
                else
                {
                    Logging.Error("Can't find ghostscript");
                }
                return located_path;
            }
        }

        private static readonly string[] potential_directories = new string[]
            {
                Environment.CurrentDirectory,
                @"C:\Program Files\gs",
                @"C:\Program Files (x86)\gs",
                @"D:\Program Files\gs",
                @"D:\Program Files (x86)\gs",
                @"E:\Program Files\gs",
                @"E:\Program Files (x86)\gs",
                @"F:\Program Files\gs",
                @"F:\Program Files (x86)\gs"
            };

        /// <summary>
        /// Generates the parameters for Ghostscript
        /// </summary>
        /// <param name="format">Any format supported by Ghostscript - jpeg, png256, png16m</param>
        /// <param name="dpi"></param>
        /// <param name="page_number"></param>
        /// <param name="output_filename">A filename of "-" will write to the console</param>
        /// <returns></returns>
        public static string GenerateGhostscriptParameters(string pdf_filename, string format, int dpi, int? page_number, int additional_pages_to_render, string output_filename)
        {
            string page_number_parameter;
            if (null == page_number || !page_number.HasValue)
            {
                page_number_parameter = "";
            }
            else
            {
                page_number_parameter = String.Format(" -dFirstPage={0} -dLastPage={1}", page_number.Value, page_number.Value + additional_pages_to_render);
            }

            string ghostscript_parameters = String.Format
                (
                " "
                + " -q"
                + " -dQUIET"
                + " -dPARANOIDSAFER"        // Run this command in safe mode
                + " -dBATCH"                // Keep gs from going into interactive mode
                + " -dNOPAUSE"              // Do not prompt and pause for each page
                + " -dNOPROMPT"             // Disable prompts for user interaction           
                + " -dMaxBitmap=500000000"  // Set high for better performance

                // Configure the output anti-aliasing, resolution, etc
                + " -dAlignToPixels=0"
                + " -dGridFitTT=0"
                + " -dTextAlphaBits=4"
                + " -dGraphicsAlphaBits=4"
                + String.Format(" -sDEVICE={0}", format)
                + String.Format(" -r{0}", dpi)

                // Set the starting and ending pages
                + page_number_parameter

                // Set the input and output files
                + String.Format(" -sOutputFile=\"{0}\"", output_filename)
                + " "
                + "\"" + pdf_filename + "\""
                );

            return ghostscript_parameters;
        }

        public static Process StartGhostscriptProcess(string ghostscript_parameters, ProcessPriorityClass priority_class)
        {
            // STDOUT/STDERR
            return ProcessSpawning.SpawnChildProcess(ExecutablePath, ghostscript_parameters, priority_class, stdout_is_binary: true);
        }
    }
}
