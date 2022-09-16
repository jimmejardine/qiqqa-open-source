namespace Qiqqa.Main
{
    internal class FileAssociationRegistration
    {
        internal static void DoRegistration()
        {
            Utilities.Files.FileAssociationRegistration.DoFileExtensionRegistration(DocumentLibrary.BundleLibrary.Common.EXT_BUNDLE_MANIFEST, "QiqqaBundleLibrary", "Install Qiqqa Bundle Library");
            Utilities.Files.FileAssociationRegistration.DoFileExtensionRegistration(DocumentLibrary.OmnipatentsImport.Common.EXT_IMPORT_FROM_OMNIPATENTS, "QiqqaImportFromOmnipatents", "Import Omnipatents Portfolio into Qiqqa");

            Utilities.Files.FileAssociationRegistration.DoUrlProtocolRegistration("qiqqa");
        }
    }
}
