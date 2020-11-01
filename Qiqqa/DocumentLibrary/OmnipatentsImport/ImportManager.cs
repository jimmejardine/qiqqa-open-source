using Qiqqa.DocumentLibrary.Import.Manual;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.DocumentLibrary.OmnipatentsImport
{
    internal class ImportManager
    {
        internal static void Go(string filename)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_ImportFromOmnipatents);

            WebLibraryDetail picked_web_library_detail = WebLibraryPicker.PickWebLibrary("Please select the Qiqqa library into which you want to import your Omnipatents portfolio.");
            if (null != picked_web_library_detail)
            {
                ImportFromThirdParty win = new ImportFromThirdParty(picked_web_library_detail);
                win.DoAutomatedBibTeXImport(filename);
                win.ShowDialog();
            }
        }
    }
}
