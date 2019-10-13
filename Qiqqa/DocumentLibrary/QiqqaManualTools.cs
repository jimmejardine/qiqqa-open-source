using System.Collections.Generic;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.DocumentLibrary
{
    public class QiqqaManualTools
    {
        private static string QiqqaManualFilename
        {
            get
            {
                return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"The Qiqqa Manual.pdf"));
            }
        }

        private static string LoexManualFilename
        {
            get
            {
                return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"The Qiqqa Manual - LOEX.pdf"));
            }
        }


        private static PDFDocument AddQiqqaManualToLibrary(Library library)
        {
            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.filename = QiqqaManualFilename;
            fwmi.tags.Add("manual");
            fwmi.tags.Add("help");
            fwmi.bibtex =
                "@booklet{qiqqa_manual" + "\n" +
                ",	title	= {The Qiqqa Manual}" + "\n" +
                ",	author	= {The Qiqqa Team,}" + "\n" +
                ",	year	= {2013}" + "\n" +
                "}"
                ;

            PDFDocument pdf_document = ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, true, true, new FilenameWithMetadataImport[] { fwmi });

            return pdf_document;
        }

        private static PDFDocument AddLoexManualToLibrary(Library library)
        {
            FilenameWithMetadataImport fwmi = new FilenameWithMetadataImport();
            fwmi.filename = LoexManualFilename;
            fwmi.tags.Add("manual");
            fwmi.tags.Add("help");
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

            PDFDocument pdf_document = ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, true, true, new FilenameWithMetadataImport[] { fwmi });

            return pdf_document;
        }

        public static PDFDocument AddManualsToLibrary(Library library)
        {   
            AddLoexManualToLibrary(library);
            return AddQiqqaManualToLibrary(library);
        }
    }
}
