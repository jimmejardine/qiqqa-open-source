using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Utilities.Files;

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
            string ghostscript_parameters = GhostscriptBinaries.GenerateGhostscriptParameters(pdf_filename, device, dpi, page_number, 0, @"-");
            Process process = GhostscriptBinaries.StartGhostscriptProcess(ghostscript_parameters, priority_class);
            Logging.Info("Process started!");

            // Read image from stdout
            StreamReader sr = process.StandardOutput;
            FileStream fs = (FileStream)sr.BaseStream;
            MemoryStream ms = new MemoryStream(128 * 1024);
            int total_size = StreamToFile.CopyStreamToStream(fs, ms);
            Logging.Debug("Image size was {0}", total_size);

            // Check that the process has exited properly
            process.WaitForExit(1000);
            if (!process.HasExited)
            {
                Logging.Error("Ghostscript process did not terminate");
            }

            return ms;
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
