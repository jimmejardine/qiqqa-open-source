using Qiqqa.DocumentLibrary.Import.Manual;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.DocumentLibrary.OmnipatentsImport
{
    class ImportManager
    {
        internal static void Go(string filename)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ImportFromOmnipatents);

            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary("Please select the Qiqqa library into which you want to import your Omnipatents portfolio.");
            if (null != web_library_detail)
            {
                ImportFromThirdParty win = new ImportFromThirdParty(web_library_detail.library);
                win.DoAutomatedBibTeXImport(filename);
                win.ShowDialog();
            }
        }
    }
}
