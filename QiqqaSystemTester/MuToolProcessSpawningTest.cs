using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using Utilities.Images;
using Utilities.PDF.MuPDF;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaSystemTester
{
    [TestClass]
    public class MuToolProcessSpawningTest
    {
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Executes once for the test class. (Optional)
        }

        [TestInitialize]
        public void Setup()
        {
            // Runs before each test. (Optional)
        }

        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            // Runs once after all tests in this class are executed. (Optional)
            // Not guaranteed that it executes instantly after all tests from the class.
        }

        [TestCleanup]
        public void TearDown()
        {
            // Runs after each test. (Optional)
        }

        // Mark that this is a unit test method. (Required)
        [TestMethod]
        public void YouTestMethod()
        {
            // Your test code goes here.
        }

        [DataRow("./annots.pdf")]
        [DataRow("./atlas_over.pdf")]
        [DataRow("./btxdoc.pdf")]
        [DataRow("./pdfbox/testPDFPackage.pdf")]
        [DataRow("./attachment.formular.pdf")]
        [DataRow("./html-standard.pdf")]
        [DataTestMethod]
        public void TestRenderASinglePDFPageAsImageAndCropIt(string filepath)
        {
            string DocumentPath = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(DocumentPath);

            string PDFPassword = null;
            int page = 1;
            int dpi = 80;
            byte[] imgbytes = MuPDFRenderer.GetPageByHeightAsImage(DocumentPath, PDFPassword, page, dpi * 12);
            ASSERT.IsGreaterOrEqual(imgbytes.Length, 256);

            using (MemoryStream ms = new MemoryStream(imgbytes))
            {
                Image cropped_image = null;
                Image resized_image = null;
                Image image = Image.FromStream(ms);

                ASSERT.IsGreaterThan(image.Width, 10);
                ASSERT.IsGreaterThan(image.Height, 10);

                try
                {
                    // resize image to given dpi
                    int new_width = (int)Math.Round(image.Width * dpi / image.HorizontalResolution);
                    int new_height = (int)Math.Round(image.Height * dpi / image.VerticalResolution);
                    if (Math.Abs(new_width - image.Width) > 2 || Math.Abs(new_height - image.Height) > 2)
                    {
                        resized_image = BitmapImageTools.ResizeImage(image, new_width, new_height);
                        image.Dispose();
                        image = resized_image;
                        resized_image = null;
                    }
                    else
                    {
                        ASSERT.IsNull("Should never get here");
                    }

                    ASSERT.IsGreaterThan(image.Width, 10);
                    ASSERT.IsGreaterThan(image.Height, 10);

                    //PDFOverlayRenderer.RenderHighlights(image, pdf_document, pdf_annotation.Page);

                    // We rescale in the Drawing.Bitmap world because the WPF world uses so much memory
                    double Left = 50;
                    double Top = 50;
                    double Width = 200;
                    double Height = 200;
                    cropped_image = BitmapImageTools.CropImageRegion(image, Left, Top, Width, Height);

                    ASSERT.IsGreaterThan(cropped_image.Width, 10);
                    ASSERT.IsGreaterThan(cropped_image.Height, 10);

                    image.Save(@"G:\output.png", ImageFormat.Png);
                    cropped_image.Save(@"G:\cropped_output.png", ImageFormat.Png);
                }
                finally
                {
                    image.Dispose();
                    resized_image?.Dispose();
                    cropped_image.Dispose();
                }
            }
        }
    }
}
