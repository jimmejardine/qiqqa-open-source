using System.Collections.Generic;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;

namespace Qiqqa.DocumentLibrary
{
    class QiqqaManualTools
    {
        private static string QiqqaManualFilename
        {
            get
            {
                return ConfigurationManager.Instance.StartupDirectoryForQiqqa + "The Qiqqa Manual.pdf";
            }
        }

        private static string LoexManualFilename
        {
            get
            {
                return ConfigurationManager.Instance.StartupDirectoryForQiqqa + "The Qiqqa Manual - LOEX.pdf";
            }
        }


        private static void AddQiqqaManualToLibrary(Library library, LibraryPdfActionCallbacks post_partum)
        {
            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.filename = QiqqaManualFilename;
            fwmi.tags = new List<string> { "manual", "help" };
            fwmi.bibtex =
                "@booklet{qiqqa_manual" + "\n" +
                ",	title	= {The Qiqqa Manual}" + "\n" +
                ",	author	= {The Qiqqa Team,}" + "\n" +
                ",	year	= {2013}" + "\n" +
                "}"
                ;

            library.AddNewDocumentToLibrary_SYNCHRONOUS(fwmi, true, post_partum);
        }

        private static void AddLoexManualToLibrary(Library library)
        {
            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.filename = LoexManualFilename;
            fwmi.tags.AddRange(new List<string> { "manual", "help" } );
            fwmi.bibtex =
                "@article{qiqqatechmatters" + "\n" +
                ",	title	= {TechMatters: “Qiqqa” than you can say Reference Management: A Tool to Organize the Research Process}" + "\n" +
                ",	author	= {Krista Graham}" + "\n" +
                ",	year	= {2014}" + "\n" +
                ",	publication	= {LOEX Quarterly}" + "\n" +
                ",	volume	= {40}" + "\n" +
                ",	pages	= {4-6}" + "\n" +
                "}"
                ;

            library.AddNewDocumentToLibrary_SYNCHRONOUS(fwmi, true);
        }

        internal static void AddManualsToLibrary(Library library, LibraryPdfActionCallbacks post_partum)
        {   
            AddLoexManualToLibrary(library);
            AddQiqqaManualToLibrary(library, post_partum);
        }
    }
}
