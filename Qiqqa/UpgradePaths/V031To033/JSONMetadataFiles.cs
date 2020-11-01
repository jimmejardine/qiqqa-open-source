using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;

namespace Qiqqa.UpgradePaths.V031To033
{
    internal class JSONMetadataFiles
    {
        public static void Upgrade()
        {
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries)
            {
                foreach (PDFDocument pdf_document in web_library_detail.Xlibrary.PDFDocuments_IncludingDeleted)
                {
                    pdf_document.QueueToStorage();
                }
            }
        }
    }
}
