using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary
{
    public static class QiqqaManualTools
    {
        private static string QiqqaManualFilename => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"The Qiqqa Manual.pdf"));

        private static string LoexManualFilename => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"The Qiqqa Manual - LOEX.pdf"));


        private static void AddQiqqaManualToLibrary(Library library, LibraryPdfActionCallbacks post_partum)
        {
            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.Filename = QiqqaManualFilename;
            fwmi.Tags.Add("manual");
            fwmi.Tags.Add("help");
            fwmi.BibTex = BibTexParser.ParseOne(
                "@booklet{qiqqa_manual" + "\n" +
                ",	title	= {The Qiqqa Manual}" + "\n" +
                ",	author	= {The Qiqqa Team,}" + "\n" +
                ",	year	= {2013}" + "\n" +
                "}", true);

            library.AddNewDocumentToLibrary_SYNCHRONOUS(fwmi, true, post_partum);
        }

        private static void AddLoexManualToLibrary(Library library)
        {
            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.Filename = LoexManualFilename;
            fwmi.Tags.Add("manual");
            fwmi.Tags.Add("help");
            fwmi.BibTex = BibTexParser.ParseOne(
                "@article{qiqqatechmatters" + "\n" +
                ",	title	= {TechMatters: “Qiqqa” than you can say Reference Management: A Tool to Organize the Research Process}" + "\n" +
                ",	author	= {Krista Graham}" + "\n" +
                ",	year	= {2014}" + "\n" +
                ",	publication	= {LOEX Quarterly}" + "\n" +
                ",	volume	= {40}" + "\n" +
                ",	pages	= {4-6}" + "\n" +
                "}", true);

            library.AddNewDocumentToLibrary_SYNCHRONOUS(fwmi, true);
        }

        public static void AddManualsToLibrary(Library library, LibraryPdfActionCallbacks post_partum)
        {   
            AddLoexManualToLibrary(library);
            AddQiqqaManualToLibrary(library, post_partum);
        }
    }
}
