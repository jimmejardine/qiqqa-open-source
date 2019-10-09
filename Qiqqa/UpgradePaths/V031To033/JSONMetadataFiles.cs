using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.Common;
using Qiqqa.Documents.PDF;

namespace Qiqqa.UpgradePaths.V031To033
{
    class JSONMetadataFiles
    {
        public static void Upgrade()
        {
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries_All)
            {
                foreach (PDFDocument pdf_document in web_library_detail.library.PDFDocuments_IncludingDeleted)
                {
                    pdf_document.QueueToStorage();
                }
            }
        }
    }
}
