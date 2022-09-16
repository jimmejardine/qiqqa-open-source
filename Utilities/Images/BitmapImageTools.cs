using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace Utilities.Images
{
    public class BitmapImageTools
    {
        public static Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmap_source)
        {
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bitmap_source));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                png.Save(memoryStream);
                return new Bitmap(memoryStream);
            }
        }

        public static Bitmap CropImageRegion(Image image, double left, double top, double width, double height)
        {
            Bitmap bitmap = new Bitmap(image);
            return CropBitmapRegion(bitmap, left, top, width, height);
        }

        public static Bitmap CropBitmapRegion(Bitmap bitmap, double left, double top, double width, double height)
        {
            Rectangle rectangle = new Rectangle((int)(bitmap.Width * left), (int)(bitmap.Height * top), (int)(bitmap.Width * width), (int)(bitmap.Height * height));

            if (rectangle.Right > bitmap.Width)
            {
                rectangle.Width = bitmap.Width - rectangle.Left;
            }
            if (rectangle.Bottom > bitmap.Height)
            {
                rectangle.Height = bitmap.Height - rectangle.Top;
            }

            return bitmap.Clone(rectangle, bitmap.PixelFormat);
        }

        public static Bitmap LoadImageRegion(string filename, double left, double top, double width, double height)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(filename);
            Rectangle rectangle = new Rectangle((int)(bitmap.Width * left), (int)(bitmap.Height * top), (int)(bitmap.Width * width), (int)(bitmap.Height * height));
            return bitmap.Clone(rectangle, bitmap.PixelFormat);
        }

        [Obsolete("Use the byte[] version directly", true)]
        public static BitmapSource LoadBitmapImageRegion(string filename, double left, double top, double width, double height)
        {
            Image image = LoadImageRegion(filename, left, top, width, height);
            return FromImage(image);
        }

        public static BitmapSource LoadFromFile(string filename)
        {
            double maximum_height;
            return LoadFromBytes(File.ReadAllBytes(filename), null, out maximum_height);
        }

        public static BitmapSource LoadFromBytes(byte[] image_data)
        {
            double maximum_height;
            return LoadFromBytes(image_data, null, out maximum_height);
        }

        private static bool VOID_CALLBACK_METHOD() { return false; }
        private static Image.GetThumbnailImageAbort VOID_CALLBACK = VOID_CALLBACK_METHOD;
        public static byte[] ShrinkPNG(byte[] image_data_original, double new_height, out double maximum_height)
        {
            using (MemoryStream ms = new MemoryStream(image_data_original))
            {
                Image image = Image.FromStream(ms, false, false);

                // We only do shrinking of the image - not growing...
                maximum_height = image.Height;
                if (image.Height < new_height)
                {
                    Logging.Warn("Not going to grow the image");
                    return image_data_original;
                }

                double new_width = image.Width * new_height / image.Height;
                Image image_thumbnail = image.GetThumbnailImage((int)new_width, (int)new_height, VOID_CALLBACK, IntPtr.Zero);

                // Sharpen the image a bit
                //             Bitmap bitmap = new Bitmap(image_thumbnail);
                //                Utilities.Images.BitmapFilter.Sharpen(bitmap, 11);
                //Utilities.Images.BitmapFilter.MeanRemoval(bitmap, 9);
                //           image_thumbnail = bitmap;

                using (MemoryStream ms_new = new MemoryStream())
                {
                    image_thumbnail.Save(ms_new, ImageFormat.Png);
                    byte[] image_data_result = ms_new.ToArray();
                    return image_data_result;
                }
            }
        }

        public static BitmapSource LoadFromBytes(byte[] image_data_original, int? optional_height, out double maximum_height)
        {
            maximum_height = Int32.MaxValue;
            byte[] image_data = optional_height.HasValue ? ShrinkPNG(image_data_original, optional_height.Value, out maximum_height) : image_data_original;

            using (MemoryStream ms = new MemoryStream(image_data))
            {
                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.StreamSource.Close();
                    image.StreamSource = null;
                    // This is a bid to remove the memory leaks exhibited by this object as it retains pointers to the underlying byte array...
                    image.Freeze();
                    return image;
                }
                catch (Exception ex)
                {
                    // If there was an exception, log the contents of the memory stream as it may be a useful error message)
                    try
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        var ascii_encoding = new ASCIIEncoding();
                        string output = ascii_encoding.GetString(ms.ToArray(), 0, 1024);
                        Logging.Error("Could not decode BitmapImage.  First few bytes are: {0}", output);
                        Logging.Error(ex);
                    }
                    catch (Exception ex2)
                    {
                        Logging.Warn(ex2, "Error reporting image problem");
                    }

                    // Throw the exception anyway
                    throw;
                }
            }
        }

        public static BitmapSource FromImage(Image image)
        {
            MemoryStream ms = new MemoryStream();

            // Save the image
            image.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            // Restore the image...
            BitmapImage bitmap_image = new BitmapImage();
            bitmap_image.BeginInit();
            bitmap_image.CacheOption = BitmapCacheOption.OnLoad;
            bitmap_image.StreamSource = ms;
            bitmap_image.EndInit();
            bitmap_image.StreamSource.Close();
            bitmap_image.StreamSource = null;
            // This is a bid to remove the memory leaks exhibited by this object as it retains pointers to the underlying byte array...
            bitmap_image.Freeze();
            return bitmap_image;
        }

        public static BitmapSource FromImage(Image image, int new_width, int new_height)
        {
            using (Bitmap bitmap = new Bitmap(image, new_width, new_height))
            {
                return FromBitmap(bitmap);
            }
        }

        public static BitmapSource FromBitmap(Bitmap bitmap)
        {
            return FromImage(bitmap);
        }

        public static BitmapSource CreateBitmapSourceFromImage(Image image)
        {
            return FromImage(image);
        }

        private static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr h_object);
        }
    }
}
