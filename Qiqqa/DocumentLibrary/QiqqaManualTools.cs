using System.Collections.Generic;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using Utilities.BibTex.Parsing;

namespace Qiqqa.DocumentLibrary
{
    public class QiqqaManualTools
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


        private static PDFDocument AddQiqqaManualToLibrary(Library library)
        {
            ImportingIntoLibrary.FilenameWithMetadataImport fwmi = new ImportingIntoLibrary.FilenameWithMetadataImport();
            fwmi.filename = QiqqaManualFilename;
            fwmi.tags = new List<string> { "manual", "help" };
            fwmi.bibtex = BibTexParser.ParseOne(
                "@booklet{qiqqa_manual" + "\n" +
                ",	title	= {The Qiqqa Manual}" + "\n" +
                ",	author	= {The Qiqqa Team,}" + "\n" +
                ",	year	= {2013}" + "\n" +
                "}", true);

            PDFDocument pdf_document = ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, true, true, new ImportingIntoLibrary.FilenameWithMetadataImport[] { fwmi });

            return pdf_document;
        }

        private static PDFDocument AddLoexManualToLibrary(Library library)
        {
            ImportingIntoLibrary.FilenameWithMetadataImport fwmi = new ImportingIntoLibrary.FilenameWithMetadataImport();
            fwmi.filename = LoexManualFilename;
            fwmi.tags.AddRange(new List<string> { "manual", "help" } );
            fwmi.bibtex = BibTexParser.ParseOne(
                "@article{qiqqatechmatters" + "\n" +
                ",	title	= {TechMatters: “Qiqqa” than you can say Reference Management: A Tool to Organize the Research Process}" + "\n" +
                ",	author	= {Krista Graham}" + "\n" +
                ",	year	= {2014}" + "\n" +
                ",	publication	= {LOEX Quarterly}" + "\n" +
                ",	volume	= {40}" + "\n" +
                ",	pages	= {4-6}" + "\n" +
                "}", true);

            PDFDocument pdf_document = ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, true, true, new ImportingIntoLibrary.FilenameWithMetadataImport[] { fwmi });

            return pdf_document;
        }

        public static PDFDocument AddManualsToLibrary(Library library)
        {   
            AddLoexManualToLibrary(library);
            return AddQiqqaManualToLibrary(library);
        }
    }
}
