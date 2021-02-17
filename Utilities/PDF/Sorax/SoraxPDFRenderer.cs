using Utilities.GUI;

namespace Utilities.PDF.Sorax
{
    public static class SoraxPDFRenderer
    {
        public static byte[] GetPageByHeightAsImage(string pdf_filename, string pdf_user_password, int page, int height, int width)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // TODO: check if we have a higher size image cached already: use that one instead of bothering the PDF renderer again
            byte[] bitmap = SoraxPDFRendererDLLWrapper.GetPageByHeightAsImage(pdf_filename, pdf_user_password, page, height, width);

            return bitmap;
        }

        public static byte[] GetPageByDPIAsImage(string pdf_filename, string pdf_user_password, int page, int dpi)
        {
            // TODO: check if we have a higher size image cached already: use that one instead of bothering the PDF renderer again
            return SoraxPDFRendererDLLWrapper.GetPageByDPIAsImage(pdf_filename, pdf_user_password, page, dpi);
        }
    }
}
