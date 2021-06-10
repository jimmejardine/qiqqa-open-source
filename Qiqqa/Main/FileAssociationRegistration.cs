using System;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Main
{
    internal class FileAssociationRegistration
    {
        internal static void DoRegistration()
        {
            //if (!Qiqqa.Common.RegistrySettings.Instance.GetPortableApplicationMode())
            //
            // Always change & point the file extensions and URI type to the last run Qiqqa instance,
            // even when it is run as a Portable Application.
            //
            // Do however reckon with the posibility of the user of the Portable Application having
            // very tightly restricted access rights to the operating system, e.g. limited or blocked
            // access to the registry.
            try
            {
                Utilities.Files.FileAssociationRegistration.DoFileExtensionRegistration(DocumentLibrary.BundleLibrary.Common.EXT_BUNDLE_MANIFEST, "QiqqaBundleLibrary", "Install Qiqqa Bundle Library");
                Utilities.Files.FileAssociationRegistration.DoFileExtensionRegistration(DocumentLibrary.OmnipatentsImport.Common.EXT_IMPORT_FROM_OMNIPATENTS, "QiqqaImportFromOmnipatents", "Import Omnipatents Portfolio into Qiqqa");

                Utilities.Files.FileAssociationRegistration.DoUrlProtocolRegistration("qiqqa");
            }
            catch (Exception ex)
            {
                if (Qiqqa.Common.RegistrySettings.Instance.GetPortableApplicationMode())
                {
                    string msg = $"The Qiqqa Portable Application failed to register the 'qiqqa://' URI type and Qiqqa-associated file extensions. This means the Portable Application will not respond to qiqqa://... links in web pages and elsewhere, and Qiqqa Portable Application will not automatically start again when you doubleclick on a qiqqa backup archive file to have it restore your libraries' backup.\n\nThe cause of this issue is probably tightly restricted user access rights on your machine. More info may be found and reported at the Qiqqa support website & issue tracker: { WebsiteAccess.Url_Support4Qiqqa }";
                    Logging.Error(ex, msg);
                    MessageBoxes.Warn(msg);
                }
                else
                {
                    string msg = $"Qiqqa failed to register the 'qiqqa://' URI type and Qiqqa-associated file extensions. This is probably a user access rights issue on your machine. Please report the issue at the Qiqqa support website & issue tracker: { WebsiteAccess.Url_Support4Qiqqa }";
                    Logging.Error(ex, msg);
                }
            }
        }
    }
}
