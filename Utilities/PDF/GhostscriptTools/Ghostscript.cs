#if false

using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Utilities.Files;
using Utilities.ProcessTools;

namespace Utilities.PDF.GhostscriptTools
{
    public class Ghostscript
    {
        public class Device
        {
            public static readonly string png256 = "png256";
            public static readonly string pnggray = "pnggray";
            public static readonly string pngmono = "pngmono";
        }

        public static MemoryStream RenderPage_AsMemoryStream(string pdf_filename, int page_number, int dpi, string device, ProcessPriorityClass priority_class)
        {
            // STDOUT/STDERR
            string ghostscript_parameters = GhostscriptBinaries.GenerateGhostscriptParameters(pdf_filename, device, dpi, page_number, 0, @"-");
            using (Process process = GhostscriptBinaries.StartGhostscriptProcess(ghostscript_parameters, priority_class))
            {
                Logging.Info("Process started!");

                // Read image from stdout
                using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process, stdout_is_binary: true))
                {
                    using (StreamReader sr = process.StandardOutput)
                    {
                        using (FileStream fs = (FileStream)sr.BaseStream)
                        {
                            MemoryStream ms = new MemoryStream(128 * 1024);
                            int total_size = StreamToFile.CopyStreamToStream(fs, ms);
                            Logging.Debug特("Image size was {0} for PDF file {1}, page {2} @ dpi {3}", total_size, pdf_filename, page_number, dpi);

                            // Check that the process has exited properly
                            process.WaitForExit(1000);

                            bool has_exited = process.HasExited;

                            if (!has_exited)
                            {
                                try
                                {
                                    process.Kill();

                                    // wait for the completion signal; this also helps to collect all STDERR output of the application (even while it was KILLED)
                                    process.WaitForExit(1000);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Error(ex, "There was a problem killing the GhostScript process after timeout");
                                }
                            }

                            // Check that we had a clean exit
                            if (!has_exited || 0 != process.ExitCode)
                            {
                                Logging.Error("Ghostscript process did not terminate.\n{0}", process_output_reader.GetOutputsDumpString());
                            }

                            return ms;
                        }
                    }
                }
            }
        }

        public static BitmapImage RenderPage_AsBitmapImage(string pdf_filename, int page_number, int dpi, string device, ProcessPriorityClass priority_class)
        {
            MemoryStream ms = RenderPage_AsMemoryStream(pdf_filename, page_number, dpi, device, priority_class);

            BitmapImage bitmap_image = new BitmapImage();
            bitmap_image.BeginInit();
            bitmap_image.StreamSource = ms;
            bitmap_image.EndInit();
            return bitmap_image;
        }

        public static Bitmap RenderPage_AsBitmap(string pdf_filename, int page_number, int dpi, string device, ProcessPriorityClass priority_class)
        {
            MemoryStream ms = RenderPage_AsMemoryStream(pdf_filename, page_number, dpi, device, priority_class);
            Bitmap bitmap = new Bitmap(ms);
            return bitmap;
        }
    }
}

#endif
