#if SORAX_ANTIQUE

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using Utilities.GUI;

namespace Utilities.PDF.Sorax
{
    public class SoraxPDFRendererDLLWrapper
    {
        private class HDOCWrapper : IDisposable
        {
            public string filename;
            public IntPtr HDOC;

            public HDOCWrapper(string filename, string pdf_user_password, string pdf_owner_password)
            {
                this.filename = filename;
                HDOC = SoraxDLL.SPD_Open(filename, pdf_user_password, pdf_owner_password);

                if (IntPtr.Zero == HDOC)
                {
                    throw new Exception(String.Format("There was a problem opening the PDF '{0}'{1}", filename, !File.Exists(filename) ? " :: File does not exist." : ""));
                }
            }

            ~HDOCWrapper()
            {
                Logging.Debug("~HDOCWrapper()");
                Dispose(false);
            }

            public void Dispose()
            {
                Logging.Debug("Disposing HDOCWrapper");
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private int dispose_count = 0;
            protected virtual void Dispose(bool disposing)
            {
                Logging.Debug("HDOCWrapper::Dispose({0}) @{1}", disposing, dispose_count);

                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        // Get rid of managed resources
                        if (IntPtr.Zero != HDOC)
                        {
                            SoraxDLL.SPD_Close(HDOC);
                            HDOC = IntPtr.Zero;
                        }
                    }
                });

                ++dispose_count;
            }
        }

        static SoraxPDFRendererDLLWrapper()
        {
            Logging.Debug特("+Initialising SoraxPDFRendererDLLWrapper");
            string config_filename = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"SPdf.ini"));
            SoraxDLL.SPD_ResetConfig(config_filename);
            Logging.Debug特("-Initialising SoraxPDFRendererDLLWrapper");
        }

        // ------------------------------------------------------------------------------------------------------------------------

        public static int GetPageCount(string filename, string pdf_user_password, string pdf_owner_password)
        {
            using (HDOCWrapper hdoc = new HDOCWrapper(filename, pdf_user_password, pdf_owner_password))
            {
                return SoraxDLL.SPD_GetPageCount(hdoc.HDOC);
            }
        }

        public static byte[] GetPageByHeightAsImage(string filename, string pdf_user_password, string pdf_owner_password, int page, double height)
        {
            float actual_dpi = 0;

            using (HDOCWrapper hdoc = new HDOCWrapper(filename, pdf_user_password, pdf_owner_password))
            {
                // We want the exact height...
                float REFERENCE_DPI = 600;
                SoraxDLL.Size size = new SoraxDLL.Size();
                bool result_get_size = SoraxDLL.SPD_GetPageSizeEx(hdoc.HDOC, page, REFERENCE_DPI, ref size);
                actual_dpi = (float)height * REFERENCE_DPI / size.Y;

                if (0 == size.Y)
                {
                    Logging.Warn("Sorax is telling us that the page is of zero height!");
                }

                return GetPageByDPIAsImage_LOCK(hdoc, page, actual_dpi);
            }
        }


        public static byte[] GetPageByDPIAsImage(string filename, string pdf_user_password, string pdf_owner_password, int page, float dpi)
        {
            using (HDOCWrapper hdoc = new HDOCWrapper(filename, pdf_user_password, pdf_owner_password))
            {
                return GetPageByDPIAsImage_LOCK(hdoc, page, dpi);
            }
        }

        private static byte[] GetPageByDPIAsImage_LOCK(HDOCWrapper hdoc, int page, float dpi)
        {
            IntPtr HDC_HDC = SoraxDLL.GetDC(IntPtr.Zero);

            try
            {
                IntPtr hbitmap = SoraxDLL.SPD_GetPageBitmap(hdoc.HDOC, HDC_HDC, page, 0, dpi);
                Bitmap bitmap = Image.FromHbitmap(hbitmap);
                SoraxDLL.DeleteObject(hbitmap);

                //using (FileStream fs = new FileStream(@"C:\temp\aax.png", FileMode.Create))
                //{
                //    bitmap.Save(fs, ImageFormat.Png);
                //}

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                bitmap.Dispose();
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new GenericException(ex, "Error while rasterising page {0} at {1}dpi of '{2}'", page, dpi, hdoc.filename);
            }
            finally
            {
                SoraxDLL.ReleaseDC(IntPtr.Zero, HDC_HDC);
            }
        }
    }
}

#endif
