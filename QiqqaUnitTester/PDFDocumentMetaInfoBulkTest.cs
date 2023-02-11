using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using QiqqaTestHelpers;
using Utilities.PDF.MuPDF;
using Alphaleonis.Win32.Filesystem;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Utilities;
using System.Text.RegularExpressions;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaUnitTester.PDFDocument
{
    [TestClass]
    public class PDFDocumentMetaInfoBulkTest
    {
        public static string QiqqaEvilPDFCollectionBasePath = null;

        public static string GetNormalizedPathToPDFTestFile(string test_filepath)
        {
            string basepath = QiqqaEvilPDFCollectionBasePath;
            if (String.IsNullOrEmpty(basepath) || !Directory.Exists(basepath))
            {
                throw new AssertFailedException($"QiqqaEvilPDFCollection: QiqqaEvilPDFCollectionBasePath directory does not exist / has not been set up in your QiqqaTest.runsettings. All bulk tests will fail. (path: \"{basepath}\")");
            }

            string fnpath = Path.GetFullPath(Path.Combine(QiqqaEvilPDFCollectionBasePath, @"TestData/data/fixtures/PDF", test_filepath));
            if (!File.Exists(fnpath))
            {
                throw new AssertFailedException($"QiqqaEvilPDFCollection: PDF file does not exist. This test will fail. (path: \"{fnpath}\")");
            }

            return fnpath;
        }

        public static string GetNormalizedPathToJSONOutputFile(string test_filepath)
        {
            string basepath = QiqqaEvilPDFCollectionBasePath;
            if (String.IsNullOrEmpty(basepath) || !Directory.Exists(basepath))
            {
                throw new AssertFailedException($"QiqqaEvilPDFCollection: QiqqaEvilPDFCollectionBasePath directory does not exist / has not been set up in your QiqqaTest.runsettings. All bulk tests will fail. (path: \"{basepath}\")");
            }

            string fnpath = Path.GetFullPath(Path.Combine(QiqqaEvilPDFCollectionBasePath, @"TestResults/TestData/data/fixtures/PDF", test_filepath));
            fnpath = Regex.Replace(fnpath, @"\\.[^./\\]+$", "") + ".json";
            return fnpath;
        }

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Executes once for the test class. (Optional)
            String path = context.Properties["QiqqaEvilPDFCollectionBasePath"].ToString();
            if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                Logging.Error($"QiqqaEvilPDFCollectionBasePath directory does not exist / has not been set up in your QiqqaTest.runsettings. All bulk tests will fail. (path: \"{path}\")");
            }
            QiqqaEvilPDFCollectionBasePath = path;
        }

        [TestInitialize]
        public void Setup()
        {
            // Runs before each test. (Optional)
        }

        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            // Runs once after all tests in this class are executed. (Optional)
            // Not guaranteed that it executes instantly after all tests from the class.
        }

        [TestCleanup]
        public void TearDown()
        {
            // Runs after each test. (Optional)
        }


        internal class PDFDocumentMuPDFMetaInfoEx4Test
        {
            public PDFDocumentMuPDFMetaInfo info;
            public object complete_json_doc;
        }

        internal string ProduceJSONtext4Comparison(PDFDocumentMuPDFMetaInfo info)
        {
            string raw_text = info.raw_metadump_text;

            info.raw_decoded_json = null;
            info.raw_metadump_text = null;

            PDFDocumentMuPDFMetaInfoEx4Test data = new PDFDocumentMuPDFMetaInfoEx4Test();
            data.info = info;
            string json_text;
            try
            {
                data.complete_json_doc = JsonConvert.DeserializeObject(raw_text);
                json_text = JsonConvert.SerializeObject(data, Formatting.Indented).Replace("\r\n", "\n");
            }
            catch (Exception ex)
            {
                json_text = @"{
                    Exception: {
                      Message: '{0}',
                      Stack: '{1}'
                    }, RAW:
                      {2}
                }";
                json_text = String.Format(json_text.Replace("'", "\""), ex.Message, ex.StackTrace, raw_text).Replace("\r\n", "\n");
            }

            return json_text;
        }

        // NOTE: the code for the first DataTestMethod serves as the template for all the others which follow.
        // The patch script will sample this one's code and duplicate it across the remaining tests in this file.

        [DataRow("./annots.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0001(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./atlas_over.pdf")]
        [DataRow("./btxdoc.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0002(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/testPDFPackage.pdf")]
        [DataRow("./attachment.formular.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0003(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./btxhak.pdf")]
        [DataRow("./cblas.pdf")]
        [DataRow("./crc-doc.1.0.pdf")]
        [DataRow("./Drezner - Computation of the Bivariate Normal Integral.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0004(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./eisenstein-nlp-notes.pdf")]
        [DataRow("./Genz - Numerical Computation of Multivariate Normal Probabilities.pdf")]
        [DataRow("./printf[1]2.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0005(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfmark_reference.pdf")]
        [DataRow("./IEEEtran_bst_HOWTO.pdf")]
        [DataRow("./IEEEtran_HOWTO.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0006(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./keyboard-shortcuts-windows.pdf")]
        [DataRow("./LittleCMS fast float extensions 1.0.pdf")]
        [DataRow("./LittleCMS floating point extensions 1.4.pdf")]
        [DataRow("./LittleCMS2.12 API.pdf")]
        [DataRow("./LittleCMS2.12 Plugin API.pdf")]
        [DataRow("./LittleCMS2.12 tutorial.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0007(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./html-standard.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0008(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./readme.pdf")]
        [DataRow("./solr-word.pdf")]
        [DataRow("./Steen - Gaussian Quadratures for the Integrals xxx.pdf")]
        [DataRow("./tb87nemeth.pdf")]
        [DataRow("./test.pdf")]
        [DataRow("./testflow_ctl_A4.pdf")]
        [DataRow("./testflow_ctl_LTR.pdf")]
        [DataRow("./testflow_doc.pdf")]
        [DataRow("./text_graphic_image.pdf")]
        [DataRow("./turtle-star.pdf")]
        [DataRow("./tux.pdf")]
        [DataRow("./zlib.3.2.pdf")]
        [DataRow("./zlib.3.pdf")]
        [DataRow("./中文分词十年又回顾- 2007-2017 CWS-10Year-Review-2.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0009(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./testfile1.pdf")]
        [DataRow("./testfile2.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0010(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./mupdf/annots.pdf")]
        [DataRow("./mupdf/black.pdf")]
        [DataRow("./mupdf/color.pdf")]
        [DataRow("./mupdf/duotone.pdf")]
        [DataRow("./mupdf/ei03.pdf")]
        [DataRow("./mupdf/ghostpdl.pdf")]
        [DataRow("./mupdf/GS9_Color_Management.pdf")]
        [DataRow("./mupdf/ijs_spec.pdf")]
        [DataRow("./mupdf/LittleCMS2.12 API.pdf")]
        [DataRow("./mupdf/LittleCMS2.12 Plugin API.pdf")]
        [DataRow("./mupdf/LittleCMS2.12 tutorial.pdf")]
        [DataRow("./mupdf/LittleCMS2.9 API.pdf")]
        [DataRow("./mupdf/LittleCMS2.9 Plugin API.pdf")]
        [DataRow("./mupdf/LittleCMS2.9 tutorial.pdf")]
        [DataRow("./mupdf/named_colors.pdf")]
        [DataRow("./mupdf/text_graphic_image.pdf")]
        [DataRow("./mupdf/text_graph_image_cmyk_rgb.pdf")]
        [DataRow("./mupdf/tritone.pdf")]
        [DataRow("./mupdf/zlib.3.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0011(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/4PP-Highlighting.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-DifferentExportValues-WasMaster.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-DifferentExportValues.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-DifferentFieldType-WasMaster.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-DifferentFieldType.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-DifferentOptions-WasMaster.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-DifferentOptions.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-SameMerged.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-SameNameNode.pdf")]
        [DataRow("./pdfbox/AcrobatMerge-TextFieldsOnly-SameMerged.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0012(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/Acroform-PDFBOX-2333.pdf")]
        [DataRow("./pdfbox/AcroFormForMerge-DifferentExportValues.pdf")]
        [DataRow("./pdfbox/AcroFormForMerge-DifferentFieldType.pdf")]
        [DataRow("./pdfbox/AcroFormForMerge-DifferentOptions.pdf")]
        [DataRow("./pdfbox/AcroFormForMerge-SameNameNode.pdf")]
        [DataRow("./pdfbox/AcroFormForMerge-TextFieldsOnly.pdf")]
        [DataRow("./pdfbox/AcroFormForMerge.pdf")]
        [DataRow("./pdfbox/AcroFormsBasicFields.pdf")]
        [DataRow("./pdfbox/AcroFormsRotation.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0013(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/AlignmentTests.pdf")]
        [DataRow("./pdfbox/badpagelabels.pdf")]
        [DataRow("./pdfbox/BidiSample.pdf")]
        [DataRow("./pdfbox/ControlCharacters.pdf")]
        [DataRow("./pdfbox/custom-render-demo.pdf")]
        [DataRow("./pdfbox/cweb.pdf")]
        [DataRow("./pdfbox/data-000001.pdf")]
        [DataRow("./pdfbox/debug.xml.pdf")]
        [DataRow("./pdfbox/DifferentDALevels.pdf")]
        [DataRow("./pdfbox/embedded_zip.pdf")]
        [DataRow("./pdfbox/excel.pdf")]
        [DataRow("./pdfbox/F001u_3_7j.pdf")]
        [DataRow("./pdfbox/FC60_Times.pdf")]
        [DataRow("./pdfbox/FillFormField.pdf")]
        [DataRow("./pdfbox/hello3.pdf")]
        [DataRow("./pdfbox/JBIG2Image.pdf")]
        [DataRow("./pdfbox/jpegrgb.pdf")]
        [DataRow("./pdfbox/jpeg_demo.pdf")]
        [DataRow("./pdfbox/JPXTestCMYK.pdf")]
        [DataRow("./pdfbox/JPXTestGrey.pdf")]
        [DataRow("./pdfbox/JPXTestRGB.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0014(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/Liste732004001452_001_0.pdf_0_.pdf")]
        [DataRow("./pdfbox/MissingCatalog.pdf")]
        [DataRow("./pdfbox/MultilineFields.pdf")]
        [DataRow("./pdfbox/multitiff.pdf")]
        [DataRow("./pdfbox/null_PDComplexFileSpecification.pdf")]
        [DataRow("./pdfbox/ollix_test_2005-03-11_bin.pdf")]
        [DataRow("./pdfbox/openoffice-test-document.pdf")]
        [DataRow("./pdfbox/page_tree_multiple_levels.pdf")]
        [DataRow("./pdfbox/pdfa-with-annotations-square.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0015(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/PDFBOX-2984-rotations.pdf")]
        [DataRow("./pdfbox/PDFBOX-3025.pdf")]
        [DataRow("./pdfbox/PDFBOX-3038-001033-p2.pdf")]
        [DataRow("./pdfbox/PDFBOX-3044-010197-p5-ligatures.pdf")]
        [DataRow("./pdfbox/PDFBOX-3053-reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-3061-092465-reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-3062-002207-p1.pdf")]
        [DataRow("./pdfbox/PDFBOX-3062-005717-p1.pdf")]
        [DataRow("./pdfbox/PDFBOX-3062-N2MOQ7YZICIYGTPLQJAWJ4HLN6CCEMHZ-reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-3067-negativeTf.pdf")]
        [DataRow("./pdfbox/PDFBOX-3068.pdf")]
        [DataRow("./pdfbox/PDFBOX-3110-poems-beads-cropbox.pdf")]
        [DataRow("./pdfbox/PDFBOX-3110-poems-beads.pdf")]
        [DataRow("./pdfbox/PDFBOX-3123-ADSFWTRB3HBZBZKEVESVTBRZC2MNKZF5_reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-3127-RAU4G6QMOVRYBISJU7R6MOVZCRFUO7P4-VFont.pdf")]
        [DataRow("./pdfbox/PDFBOX-3195.pdf")]
        [DataRow("./pdfbox/PDFBOX-3498-Y5TLCWTIAE3FYDVJTV2TXRZGXLEDUNSW.pdf")]
        [DataRow("./pdfbox/PDFBOX-3741.pdf")]
        [DataRow("./pdfbox/PDFBOX-3833-reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-4322-Empty-ToUnicode-reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-4372-2DAYCLVOFG3FTVO4RMAJJL3VTPNYDFRO-p4_reduced.pdf")]
        [DataRow("./pdfbox/PDFBOX-4417-054080.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0016(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/PDFBox.GlobalResourceMergeTest.Doc01.decoded.pdf")]
        [DataRow("./pdfbox/PDFBox.GlobalResourceMergeTest.Doc01.pdf")]
        [DataRow("./pdfbox/PDFBox.GlobalResourceMergeTest.Doc02.decoded.pdf")]
        [DataRow("./pdfbox/PDFBox.GlobalResourceMergeTest.Doc02.pdf")]
        [DataRow("./pdfbox/PDFBoxLegacyMerge-SameMerged.pdf")]
        [DataRow("./pdfbox/pdfbox_webpage.pdf")]
        [DataRow("./pdfbox/preEnc_20141025_105451.pdf")]
        [DataRow("./pdfbox/raw_image_demo.pdf")]
        [DataRow("./pdfbox/rotation.pdf")]
        [DataRow("./pdfbox/sampleForSpec.pdf")]
        [DataRow("./pdfbox/sample_fonts_solidconvertor.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0017(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/sign_me.pdf")]
        [DataRow("./pdfbox/sign_me_tsa.pdf")]
        [DataRow("./pdfbox/sign_me_visible.pdf")]
        [DataRow("./pdfbox/simple-openoffice.pdf")]
        [DataRow("./pdfbox/SimpleForm2Fields.pdf")]
        [DataRow("./pdfbox/source.pdf")]
        [DataRow("./pdfbox/test.pdf")]
        [DataRow("./pdfbox/testPDF_multiFormatEmbFiles.pdf")]
        [DataRow("./pdfbox/test_pagelabels.pdf")]
        [DataRow("./pdfbox/tiger-as-form-xobject.pdf")]
        [DataRow("./pdfbox/transitions_test.pdf")]
        [DataRow("./pdfbox/with_outline.pdf")]
        [DataRow("./pdfbox/yaddatest.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0018(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/PDFAMetaDataValidationTestMiddleControlChar.pdf")]
        [DataRow("./pdfbox/PDFAMetaDataValidationTestMiddleNul.pdf")]
        [DataRow("./pdfbox/PDFAMetaDataValidationTestTrailingControlChar.pdf")]
        [DataRow("./pdfbox/PDFAMetaDataValidationTestTrailingNul.pdf")]
        [DataRow("./pdfbox/PDFAMetaDataValidationTestTrailingSpaces.pdf")]
        [DataRow("./pdfbox/ccitt4-cib-test.pdf")]
        [DataRow("./pdfbox/PDFBOX-3042-003177-p2.pdf")]
        [DataRow("./pdfbox/png_demo.pdf")]
        [DataRow("./pdfbox/test.unc.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0019(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/PasswordSample-128bit.pdf")]
        [DataRow("./pdfbox/PasswordSample-256bit.pdf")]
        [DataRow("./pdfbox/PasswordSample-40bit.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0020(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./pdfbox/document.pdf")]
        [DataRow("./pdfbox/PDFBOX-4417-001031.pdf")]
        [DataRow("./pdfbox/test-landscape2.pdf")]
        [DataRow("./pdfbox/survey.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0021(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./Python2.pdf")]
        [DataRow("./QiqqaFeaturePoster.pdf")]
        [DataRow("./QiqqaPosterPack.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0022(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/1.pdf")]
        [DataRow("./qpdf/11-pages-with-labels.pdf")]
        [DataRow("./qpdf/11-pages.pdf")]
        [DataRow("./qpdf/2.pdf")]
        [DataRow("./qpdf/5.pdf")]
        [DataRow("./qpdf/a.pdf")]
        [DataRow("./qpdf/add-contents.pdf")]
        [DataRow("./qpdf/appearances-1.pdf")]
        [DataRow("./qpdf/appearances-11.pdf")]
        [DataRow("./qpdf/appearances-12.pdf")]
        [DataRow("./qpdf/appearances-2.pdf")]
        [DataRow("./qpdf/appearances-a-more.pdf")]
        [DataRow("./qpdf/appearances-a-more2.pdf")]
        [DataRow("./qpdf/appearances-a.pdf")]
        [DataRow("./qpdf/appearances-b.pdf")]
        [DataRow("./qpdf/appearances-quack.pdf")]
        [DataRow("./qpdf/append-page-content-damaged.pdf")]
        [DataRow("./qpdf/append-page-content.pdf")]
        [DataRow("./qpdf-manual.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0023(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/bad-data-out.pdf")]
        [DataRow("./qpdf/bad-data.pdf")]
        [DataRow("./qpdf/bad-jpeg-out.pdf")]
        [DataRow("./qpdf/bad-jpeg.pdf")]
        [DataRow("./qpdf/bad-token-startxref.pdf")]
        [DataRow("./qpdf/bad-xref-entry.pdf")]
        [DataRow("./qpdf/bad10.pdf")]
        [DataRow("./qpdf/bad11.pdf")]
        [DataRow("./qpdf/bad12.pdf")]
        [DataRow("./qpdf/bad13.pdf")]
        [DataRow("./qpdf/bad14.pdf")]
        [DataRow("./qpdf/bad15.pdf")]
        [DataRow("./qpdf/bad16.pdf")]
        [DataRow("./qpdf/bad17.pdf")]
        [DataRow("./qpdf/bad18.pdf")]
        [DataRow("./qpdf/bad19.pdf")]
        [DataRow("./qpdf/bad2.pdf")]
        [DataRow("./qpdf/bad20.pdf")]
        [DataRow("./qpdf/bad21.pdf")]
        [DataRow("./qpdf/bad22.pdf")]
        [DataRow("./qpdf/bad23.pdf")]
        [DataRow("./qpdf/bad24.pdf")]
        [DataRow("./qpdf/bad25.pdf")]
        [DataRow("./qpdf/bad26.pdf")]
        [DataRow("./qpdf/bad27.pdf")]
        [DataRow("./qpdf/bad28.pdf")]
        [DataRow("./qpdf/bad29.pdf")]
        [DataRow("./qpdf/bad3.pdf")]
        [DataRow("./qpdf/bad30.pdf")]
        [DataRow("./qpdf/bad31.pdf")]
        [DataRow("./qpdf/bad32.pdf")]
        [DataRow("./qpdf/bad33.pdf")]
        [DataRow("./qpdf/bad34.pdf")]
        [DataRow("./qpdf/bad35.pdf")]
        [DataRow("./qpdf/bad36.pdf")]
        [DataRow("./qpdf/bad37.pdf")]
        [DataRow("./qpdf/bad38.pdf")]
        [DataRow("./qpdf/bad4.pdf")]
        [DataRow("./qpdf/bad5.pdf")]
        [DataRow("./qpdf/bad6.pdf")]
        [DataRow("./qpdf/bad7.pdf")]
        [DataRow("./qpdf/bad8.pdf")]
        [DataRow("./qpdf/bad9.pdf")]
        [DataRow("./qpdf/badlin1.pdf")]
        [DataRow("./qpdf/big-ostream.pdf")]
        [DataRow("./qpdf/broken-decode-parms-no-filter.pdf")]
        [DataRow("./qpdf/broken-lzw.pdf")]
        [DataRow("./qpdf/button-set-broken-out.pdf")]
        [DataRow("./qpdf/button-set-broken.pdf")]
        [DataRow("./qpdf/button-set-out.pdf")]
        [DataRow("./qpdf/button-set.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0024(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/bad-encryption-length.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0025(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/bad1.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0026(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/c-check-clear-in.pdf")]
        [DataRow("./qpdf/c-check-warn-in.pdf")]
        [DataRow("./qpdf/c-decrypt-R5-with-user.pdf")]
        [DataRow("./qpdf/c-decrypt-R6-with-owner.pdf")]
        [DataRow("./qpdf/c-decrypt-with-owner.pdf")]
        [DataRow("./qpdf/c-decrypt-with-user.pdf")]
        [DataRow("./qpdf/c-ignore-xref-streams.pdf")]
        [DataRow("./qpdf/c-info-out.pdf")]
        [DataRow("./qpdf/c-info2-in.pdf")]
        [DataRow("./qpdf/c-linearized.pdf")]
        [DataRow("./qpdf/c-no-options.pdf")]
        [DataRow("./qpdf/c-no-original-object-ids.pdf")]
        [DataRow("./qpdf/c-normalized-content.pdf")]
        [DataRow("./qpdf/c-object-streams.pdf")]
        [DataRow("./qpdf/c-qdf.pdf")]
        [DataRow("./qpdf/c-uncompressed-streams.pdf")]
        [DataRow("./qpdf/coalesce-out.pdf")]
        [DataRow("./qpdf/coalesce-split-1-2.pdf")]
        [DataRow("./qpdf/coalesce.pdf")]
        [DataRow("./qpdf/collate-even.pdf")]
        [DataRow("./qpdf/collate-odd.pdf")]
        [DataRow("./qpdf/comment-annotation-direct-out.pdf")]
        [DataRow("./qpdf/comment-annotation-direct.pdf")]
        [DataRow("./qpdf/comment-annotation-out.pdf")]
        [DataRow("./qpdf/comment-annotation.pdf")]
        [DataRow("./qpdf/compress-objstm-xref-qdf.pdf")]
        [DataRow("./qpdf/compress-objstm-xref.pdf")]
        [DataRow("./qpdf/compressed-metadata.pdf")]
        [DataRow("./qpdf/content-stream-errors.pdf")]
        [DataRow("./qpdf/copy-foreign-objects-in.pdf")]
        [DataRow("./qpdf/copy-foreign-objects-out1.pdf")]
        [DataRow("./qpdf/copy-foreign-objects-out2.pdf")]
        [DataRow("./qpdf/copy-foreign-objects-out3.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0027(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/custom-pipeline.pdf")]
        [DataRow("./qpdf/damaged-inline-image-out.pdf")]
        [DataRow("./qpdf/damaged-inline-image.pdf")]
        [DataRow("./qpdf/damaged-stream.pdf")]
        [DataRow("./qpdf/dangling-refs-dangling-out.pdf")]
        [DataRow("./qpdf/dangling-refs.pdf")]
        [DataRow("./qpdf/decrypted-crypt-filter.pdf")]
        [DataRow("./qpdf/decrypted-positive-P.pdf")]
        [DataRow("./qpdf/deep-duplicate-pages.pdf")]
        [DataRow("./qpdf/delete-and-reuse.pdf")]
        [DataRow("./qpdf/deterministic-id-in.pdf")]
        [DataRow("./qpdf/deterministic-id-nn.pdf")]
        [DataRow("./qpdf/deterministic-id-ny.pdf")]
        [DataRow("./qpdf/deterministic-id-yn.pdf")]
        [DataRow("./qpdf/deterministic-id-yy.pdf")]
        [DataRow("./qpdf/digitally-signed.pdf")]
        [DataRow("./qpdf/direct-outlines.pdf")]
        [DataRow("./qpdf/direct-pages-fixed.pdf")]
        [DataRow("./qpdf/direct-pages.pdf")]
        [DataRow("./qpdf/duplicate-pages.pdf")]
        [DataRow("./qpdf/empty-decode-parms-out.pdf")]
        [DataRow("./qpdf/empty-decode-parms.pdf")]
        [DataRow("./qpdf/empty-info.pdf")]
        [DataRow("./qpdf/empty-object.pdf")]
        [DataRow("./qpdf/enc-base.pdf")]
        [DataRow("./qpdf/eof-in-inline-image.pdf")]
        [DataRow("./qpdf/eof-reading-token.pdf")]
        [DataRow("./qpdf/eof-terminates-literal.pdf")]
        [DataRow("./qpdf/exp1.pdf")]
        [DataRow("./qpdf/exp2.pdf")]
        [DataRow("./qpdf/extensions-adbe-force-1.8.5.pdf")]
        [DataRow("./qpdf/extensions-adbe-other-force-1.8.5.pdf")]
        [DataRow("./qpdf/extensions-adbe-other.pdf")]
        [DataRow("./qpdf/extensions-adbe.pdf")]
        [DataRow("./qpdf/extensions-none-force-1.8.5.pdf")]
        [DataRow("./qpdf/extensions-other-force-1.8.5.pdf")]
        [DataRow("./qpdf/extensions-other.pdf")]
        [DataRow("./qpdf/extra-header-lin-newline.pdf")]
        [DataRow("./qpdf/extra-header-lin-no-newline.pdf")]
        [DataRow("./qpdf/extra-header-newline.pdf")]
        [DataRow("./qpdf/extra-header-no-newline.pdf")]
        [DataRow("./qpdf/extract-duplicate-page.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0028(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/fax-decode-parms.pdf")]
        [DataRow("./qpdf/field-types (2).pdf")]
        [DataRow("./qpdf/field-types.pdf")]
        [DataRow("./qpdf/filter-abbreviation.pdf")]
        [DataRow("./qpdf/form-bad-fields-array.pdf")]
        [DataRow("./qpdf/form-empty-from-odt.pdf")]
        [DataRow("./qpdf/form-errors.pdf")]
        [DataRow("./qpdf/form-filled-by-acrobat-out.pdf")]
        [DataRow("./qpdf/form-filled-by-acrobat.pdf")]
        [DataRow("./qpdf/form-filled-with-atril.pdf")]
        [DataRow("./qpdf/form-in.pdf")]
        [DataRow("./qpdf/form-mod1.pdf")]
        [DataRow("./qpdf/form-no-need-appearances-filled.pdf")]
        [DataRow("./qpdf/form-no-need-appearances.pdf")]
        [DataRow("./qpdf/form-out.pdf")]
        [DataRow("./qpdf/form-xobjects-out.pdf")]
        [DataRow("./qpdf/form.pdf")]
        [DataRow("./qpdf/from-scratch-0.pdf")]
        [DataRow("./qpdf/fx-overlay-56.pdf")]
        [DataRow("./qpdf/fx-overlay-57.pdf")]
        [DataRow("./qpdf/fx-overlay-58.pdf")]
        [DataRow("./qpdf/fx-overlay-59.pdf")]
        [DataRow("./qpdf/fx-overlay-64.pdf")]
        [DataRow("./qpdf/fx-overlay-65.pdf")]
        [DataRow("./qpdf/fx-overlay-66.pdf")]
        [DataRow("./qpdf/fx-overlay-67.pdf")]
        [DataRow("./qpdf/fxo-bigsmall.pdf")]
        [DataRow("./qpdf/fxo-blue.pdf")]
        [DataRow("./qpdf/fxo-green.pdf")]
        [DataRow("./qpdf/fxo-red.pdf")]
        [DataRow("./qpdf/fxo-smallbig.pdf")]
        [DataRow("./qpdf/gen1.pdf")]
        [DataRow("./qpdf/good1.pdf")]
        [DataRow("./qpdf/good10.pdf")]
        [DataRow("./qpdf/good11.pdf")]
        [DataRow("./qpdf/good12.pdf")]
        [DataRow("./qpdf/good13.pdf")]
        [DataRow("./qpdf/good14.pdf")]
        [DataRow("./qpdf/good15.pdf")]
        [DataRow("./qpdf/good16.pdf")]
        [DataRow("./qpdf/good17-not-qdf.pdf")]
        [DataRow("./qpdf/good17-not-recompressed.pdf")]
        [DataRow("./qpdf/good17.pdf")]
        [DataRow("./qpdf/good18.pdf")]
        [DataRow("./qpdf/good19.pdf")]
        [DataRow("./qpdf/good2.pdf")]
        [DataRow("./qpdf/good20.pdf")]
        [DataRow("./qpdf/good21.pdf")]
        [DataRow("./qpdf/good3.pdf")]
        [DataRow("./qpdf/good4.pdf")]
        [DataRow("./qpdf/good5.pdf")]
        [DataRow("./qpdf/good6.pdf")]
        [DataRow("./qpdf/good7.pdf")]
        [DataRow("./qpdf/good8.pdf")]
        [DataRow("./qpdf/good9.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0029(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/hybrid-xref.pdf")]
        [DataRow("./qpdf/image-streams-small.pdf")]
        [DataRow("./qpdf/image-streams.pdf")]
        [DataRow("./qpdf/in (2).pdf")]
        [DataRow("./qpdf/in (3).pdf")]
        [DataRow("./qpdf/in (4).pdf")]
        [DataRow("./qpdf/in (5).pdf")]
        [DataRow("./qpdf/in (6).pdf")]
        [DataRow("./qpdf/in.pdf")]
        [DataRow("./qpdf/indirect-decode-parms-out.pdf")]
        [DataRow("./qpdf/indirect-decode-parms.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0030(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/indirect-r-arg.pdf")]
        [DataRow("./qpdf/inline-image-colorspace-lookup-out.pdf")]
        [DataRow("./qpdf/inline-image-colorspace-lookup.pdf")]
        [DataRow("./qpdf/inline-images-ii-all.pdf")]
        [DataRow("./qpdf/inline-images-ii-some.pdf")]
        [DataRow("./qpdf/inline-images.pdf")]
        [DataRow("./qpdf/input (2).pdf")]
        [DataRow("./qpdf/input.pdf")]
        [DataRow("./qpdf/invalid-id-xref.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0031(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/issue-100.pdf")]
        [DataRow("./qpdf/issue-101.pdf")]
        [DataRow("./qpdf/issue-106.pdf")]
        [DataRow("./qpdf/issue-117.pdf")]
        [DataRow("./qpdf/issue-119.pdf")]
        [DataRow("./qpdf/issue-120.pdf")]
        [DataRow("./qpdf/issue-141a.pdf")]
        [DataRow("./qpdf/issue-143.pdf")]
        [DataRow("./qpdf/issue-146.pdf")]
        [DataRow("./qpdf/issue-147.pdf")]
        [DataRow("./qpdf/issue-148.pdf")]
        [DataRow("./qpdf/issue-149.pdf")]
        [DataRow("./qpdf/issue-150.pdf")]
        [DataRow("./qpdf/issue-179.pdf")]
        [DataRow("./qpdf/issue-202.pdf")]
        [DataRow("./qpdf/issue-335a.pdf")]
        [DataRow("./qpdf/issue-99.pdf")]
        [DataRow("./qpdf/issue-99b.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0032(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/issue-141b.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0033(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/issue-263.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0034(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/issue-335b.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0035(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/jpeg-qstream.pdf")]
        [DataRow("./qpdf/labels-split-01-06.pdf")]
        [DataRow("./qpdf/labels-split-07-11.pdf")]
        [DataRow("./qpdf/large-inline-image-ii-all.pdf")]
        [DataRow("./qpdf/large-inline-image-ii-some.pdf")]
        [DataRow("./qpdf/large-inline-image.pdf")]
        [DataRow("./qpdf/leading-junk.pdf")]
        [DataRow("./qpdf/lin-delete-and-reuse.pdf")]
        [DataRow("./qpdf/lin-special.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0036(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/lin0.pdf")]
        [DataRow("./qpdf/lin1.pdf")]
        [DataRow("./qpdf/lin2.pdf")]
        [DataRow("./qpdf/lin3.pdf")]
        [DataRow("./qpdf/lin4.pdf")]
        [DataRow("./qpdf/lin5.pdf")]
        [DataRow("./qpdf/lin6.pdf")]
        [DataRow("./qpdf/lin7.pdf")]
        [DataRow("./qpdf/lin8.pdf")]
        [DataRow("./qpdf/lin9.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0037(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/linearization-bounds-1.pdf")]
        [DataRow("./qpdf/linearization-bounds-2.pdf")]
        [DataRow("./qpdf/linearization-large-vector-alloc.pdf")]
        [DataRow("./qpdf/linearize-duplicate-page.pdf")]
        [DataRow("./qpdf/linearized-and-warnings.pdf")]
        [DataRow("./qpdf/long-id-linearized.pdf")]
        [DataRow("./qpdf/long-id.pdf")]
        [DataRow("./qpdf/manual-appearances-out.pdf")]
        [DataRow("./qpdf/manual-appearances-print-out.pdf")]
        [DataRow("./qpdf/manual-appearances-screen-out.pdf")]
        [DataRow("./qpdf/manual-appearances.pdf")]
        [DataRow("./qpdf/merge-dict.pdf")]
        [DataRow("./qpdf/merge-implicit-ranges.pdf")]
        [DataRow("./qpdf/merge-multiple-labels.pdf")]
        [DataRow("./qpdf/merge-three-files-1.pdf")]
        [DataRow("./qpdf/merge-three-files-2.pdf")]
        [DataRow("./qpdf/minimal-1.pdf")]
        [DataRow("./qpdf/minimal-9.pdf")]
        [DataRow("./qpdf/minimal-dangling-out.pdf")]
        [DataRow("./qpdf/minimal-linearize-pass1.pdf")]
        [DataRow("./qpdf/minimal-linearized.pdf")]
        [DataRow("./qpdf/minimal-rotated.pdf")]
        [DataRow("./qpdf/minimal.pdf")]
        [DataRow("./qpdf/more-choices.pdf")]
        [DataRow("./qpdf/name-pound-images.pdf")]
        [DataRow("./qpdf/name-tree.pdf")]
        [DataRow("./qpdf/need-appearances-more-out.pdf")]
        [DataRow("./qpdf/need-appearances-more.pdf")]
        [DataRow("./qpdf/need-appearances-more2.pdf")]
        [DataRow("./qpdf/need-appearances-out.pdf")]
        [DataRow("./qpdf/need-appearances.pdf")]
        [DataRow("./qpdf/new-streams.pdf")]
        [DataRow("./qpdf/newline-before-endstream-nl-objstm.pdf")]
        [DataRow("./qpdf/newline-before-endstream-nl-qdf.pdf")]
        [DataRow("./qpdf/newline-before-endstream-nl.pdf")]
        [DataRow("./qpdf/newline-before-endstream-qdf.pdf")]
        [DataRow("./qpdf/no-contents-coalesce-contents.pdf")]
        [DataRow("./qpdf/no-contents-none.pdf")]
        [DataRow("./qpdf/no-contents-qdf.pdf")]
        [DataRow("./qpdf/no-contents.pdf")]
        [DataRow("./qpdf/no-info.pdf")]
        [DataRow("./qpdf/no-pages-types-fixed.pdf")]
        [DataRow("./qpdf/no-pages-types.pdf")]
        [DataRow("./qpdf/no-space-in-xref.pdf")]
        [DataRow("./qpdf/number-tree.pdf")]
        [DataRow("./qpdf/numeric-and-string-1.pdf")]
        [DataRow("./qpdf/numeric-and-string-2.pdf")]
        [DataRow("./qpdf/numeric-and-string-3.pdf")]
        [DataRow("./qpdf/obj0.pdf")]
        [DataRow("./qpdf/object-stream.pdf")]
        [DataRow("./qpdf/object-types-os.pdf")]
        [DataRow("./qpdf/object-types.pdf")]
        [DataRow("./qpdf/out (2).pdf")]
        [DataRow("./qpdf/out (3).pdf")]
        [DataRow("./qpdf/out (4).pdf")]
        [DataRow("./qpdf/out.pdf")]
        [DataRow("./qpdf/outlines-split-01-10.pdf")]
        [DataRow("./qpdf/outlines-split-11-20.pdf")]
        [DataRow("./qpdf/outlines-split-21-30.pdf")]
        [DataRow("./qpdf/outlines-with-actions.pdf")]
        [DataRow("./qpdf/outlines-with-loop.pdf")]
        [DataRow("./qpdf/outlines-with-old-root-dests.pdf")]
        [DataRow("./qpdf/override-compressed-object.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0038(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/p1-a-p2-a.pdf")]
        [DataRow("./qpdf/p1-a-p2-b.pdf")]
        [DataRow("./qpdf/p1-a.pdf")]
        [DataRow("./qpdf/p1-b.pdf")]
        [DataRow("./qpdf/page-labels-and-outlines.pdf")]
        [DataRow("./qpdf/page-labels-no-zero.pdf")]
        [DataRow("./qpdf/page-labels-num-tree.pdf")]
        [DataRow("./qpdf/page-no-content.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0039(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/pages-copy-encryption.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0040(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/pages-is-page-out.pdf")]
        [DataRow("./qpdf/pages-is-page.pdf")]
        [DataRow("./qpdf/pages-loop.pdf")]
        [DataRow("./qpdf/page_api_1-out.pdf")]
        [DataRow("./qpdf/page_api_1-out2.pdf")]
        [DataRow("./qpdf/page_api_1-out3.pdf")]
        [DataRow("./qpdf/page_api_1.pdf")]
        [DataRow("./qpdf/page_api_2.pdf")]
        [DataRow("./qpdf/pclm-in.pdf")]
        [DataRow("./qpdf/pclm-out.pdf")]
        [DataRow("./qpdf/png-filters-decoded.pdf")]
        [DataRow("./qpdf/png-filters.pdf")]
        [DataRow("./qpdf/pound-in-name.pdf")]
        [DataRow("./qpdf/qpdf-manual.pdf")]
        [DataRow("./qpdf/qstream.pdf")]
        [DataRow("./qpdf/really-shared-images-pages-out.pdf")]
        [DataRow("./qpdf/remove-labels.pdf")]
        [DataRow("./qpdf/replace-input.pdf")]
        [DataRow("./qpdf/replaced-stream-data-flate.pdf")]
        [DataRow("./qpdf/replaced-stream-data.pdf")]
        [DataRow("./qpdf/reserved-objects.pdf")]
        [DataRow("./qpdf/rotated.pdf")]
        [DataRow("./qpdf/sample-form-out.pdf")]
        [DataRow("./qpdf/sample-form.pdf")]
        [DataRow("./qpdf/shallow_array-out.pdf")]
        [DataRow("./qpdf/shallow_array.pdf")]
        [DataRow("./qpdf/short-id-linearized.pdf")]
        [DataRow("./qpdf/short-id.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0041(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/short-O-U.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0042(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/small-images.pdf")]
        [DataRow("./qpdf/source1.pdf")]
        [DataRow("./qpdf/source2.pdf")]
        [DataRow("./qpdf/split-content-stream-errors.pdf")]
        [DataRow("./qpdf/split-content-stream.pdf")]
        [DataRow("./qpdf/split-exp-01.Pdf")]
        [DataRow("./qpdf/split-exp-02.Pdf")]
        [DataRow("./qpdf/split-exp-03.Pdf")]
        [DataRow("./qpdf/split-exp-04.Pdf")]
        [DataRow("./qpdf/split-exp-05.Pdf")]
        [DataRow("./qpdf/split-exp-06.Pdf")]
        [DataRow("./qpdf/split-exp-07.Pdf")]
        [DataRow("./qpdf/split-exp-08.Pdf")]
        [DataRow("./qpdf/split-exp-09.Pdf")]
        [DataRow("./qpdf/split-exp-1.pdf")]
        [DataRow("./qpdf/split-exp-10.Pdf")]
        [DataRow("./qpdf/split-exp-11.Pdf")]
        [DataRow("./qpdf/split-exp-group-01-05.pdf")]
        [DataRow("./qpdf/split-exp-group-06-10.pdf")]
        [DataRow("./qpdf/split-exp-group-11-11.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0043(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/stamp.pdf")]
        [DataRow("./qpdf/stream-data.pdf")]
        [DataRow("./qpdf/stream-line-enders.pdf")]
        [DataRow("./qpdf/streams-with-newlines.pdf")]
        [DataRow("./qpdf/terminate-parsing.pdf")]
        [DataRow("./qpdf/test14-in.pdf")]
        [DataRow("./qpdf/test14-out.pdf")]
        [DataRow("./qpdf/test4-1.pdf")]
        [DataRow("./qpdf/test4-2.pdf")]
        [DataRow("./qpdf/test4-3.pdf")]
        [DataRow("./qpdf/test4-4.pdf")]
        [DataRow("./qpdf/three-files-collate-out.pdf")]
        [DataRow("./qpdf/tiff-predictor.pdf")]
        [DataRow("./qpdf/to-rotate.pdf")]
        [DataRow("./qpdf/token-filters-out.pdf")]
        [DataRow("./qpdf/tokenize-content-streams.pdf")]
        [DataRow("./qpdf/tokens.pdf")]
        [DataRow("./qpdf/unfilterable.pdf")]
        [DataRow("./qpdf/unique-resources.pdf")]
        [DataRow("./qpdf/unreferenced-dropped.pdf")]
        [DataRow("./qpdf/unreferenced-indirect-scalar.pdf")]
        [DataRow("./qpdf/unreferenced-objects.pdf")]
        [DataRow("./qpdf/unreferenced-preserved.pdf")]
        [DataRow("./qpdf/unsupported-optimization.pdf")]
        [DataRow("./qpdf/uo-1.pdf")]
        [DataRow("./qpdf/uo-2.pdf")]
        [DataRow("./qpdf/uo-3.pdf")]
        [DataRow("./qpdf/uo-4.pdf")]
        [DataRow("./qpdf/uo-5.pdf")]
        [DataRow("./qpdf/uo-6.pdf")]
        [DataRow("./qpdf/uo-7.pdf")]
        [DataRow("./qpdf/warn-replace.pdf")]
        [DataRow("./qpdf/xref-errors.pdf")]
        [DataRow("./qpdf/xref-with-short-size.pdf")]
        [DataRow("./qpdf/zero-offset.pdf")]
        [DataRow("./qpdf-manual.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0044(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("./qpdf/fuzz-16214.pdf")]
        [DataRow("./qpdf/20-pages.pdf")]
        [DataRow("./qpdf/3.pdf")]
        [DataRow("./qpdf/4.pdf")]
        [DataRow("./qpdf/append-xref-loop-fixed.pdf")]
        [DataRow("./qpdf/append-xref-loop.pdf")]
        [DataRow("./qpdf/c-r2.pdf")]
        [DataRow("./qpdf/c-r3.pdf")]
        [DataRow("./qpdf/c-r4.pdf")]
        [DataRow("./qpdf/c-r5-in.pdf")]
        [DataRow("./qpdf/c-r6-in.pdf")]
        [DataRow("./qpdf/copied-positive-P.pdf")]
        [DataRow("./qpdf/enc-long-password.pdf")]
        [DataRow("./qpdf/enc-R2,V1,O=master.pdf")]
        [DataRow("./qpdf/enc-R2,V1,U=view,O=master.pdf")]
        [DataRow("./qpdf/enc-R2,V1,U=view,O=view.pdf")]
        [DataRow("./qpdf/enc-R2,V1.pdf")]
        [DataRow("./qpdf/enc-R3,V2,O=master.pdf")]
        [DataRow("./qpdf/enc-R3,V2,U=view,O=master.pdf")]
        [DataRow("./qpdf/enc-R3,V2,U=view,O=view.pdf")]
        [DataRow("./qpdf/enc-R3,V2.pdf")]
        [DataRow("./qpdf/enc-XI-attachments-base.pdf")]
        [DataRow("./qpdf/enc-XI-base.pdf")]
        [DataRow("./qpdf/enc-XI-long-password.pdf")]
        [DataRow("./qpdf/enc-XI-R6,V5,O=master.pdf")]
        [DataRow("./qpdf/enc-XI-R6,V5,U=attachment,encrypted-attachments.pdf")]
        [DataRow("./qpdf/enc-XI-R6,V5,U=view,attachments,cleartext-metadata.pdf")]
        [DataRow("./qpdf/enc-XI-R6,V5,U=view,O=master.pdf")]
        [DataRow("./qpdf/enc-XI-R6,V5,U=wwwww,O=wwwww.pdf")]
        [DataRow("./qpdf/encrypted-40-bit-R3.pdf")]
        [DataRow("./qpdf/encrypted-positive-P.pdf")]
        [DataRow("./qpdf/encrypted-with-images.pdf")]
        [DataRow("./qpdf/metadata-crypt-filter.pdf")]
        [DataRow("./qpdf/unfilterable-with-crypt.pdf")]
        [DataRow("./qpdf/V4-aes-clearmeta.pdf")]
        [DataRow("./qpdf/V4-aes.pdf")]
        [DataRow("./qpdf/V4-clearmeta.pdf")]
        [DataRow("./qpdf/V4.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0045(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("asbach uralt - dual column scanned old.pdf")]
        [DataRow("issue-0007-sorax-blank-page.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0046(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("novapdf/active-pdf-links.pdf")]
        [DataRow("novapdf/pdf-example-bookmarks.pdf")]
        [DataRow("novapdf/pdf-example-encryption.pdf")]
        [DataRow("novapdf/pdf-example-subsetted-fonts.pdf")]
        [DataRow("novapdf/pdf-example-watermarks.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0047(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("qpdf/shared-font-xobject-split-1.pdf")]
        [DataRow("qpdf/shared-font-xobject-split-2.pdf")]
        [DataRow("qpdf/shared-font-xobject-split-3.pdf")]
        [DataRow("qpdf/shared-font-xobject-split-4.pdf")]
        [DataRow("qpdf/shared-font-xobject.pdf")]
        [DataRow("qpdf/shared-form-images-merged.pdf")]
        [DataRow("qpdf/shared-form-images-xobject.pdf")]
        [DataRow("qpdf/shared-form-images.pdf")]
        [DataRow("qpdf/shared-form-split-1.pdf")]
        [DataRow("qpdf/shared-form-split-2.pdf")]
        [DataRow("qpdf/shared-form-split-3.pdf")]
        [DataRow("qpdf/shared-form-split-4.pdf")]
        [DataRow("qpdf/shared-form-split-5.pdf")]
        [DataRow("qpdf/shared-form-split-6.pdf")]
        [DataRow("qpdf/shared-form-xobject-split-1.pdf")]
        [DataRow("qpdf/shared-form-xobject-split-2.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0048(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("qpdf/shared-images-errors-1-3-out.pdf")]
        [DataRow("qpdf/shared-images-errors-1-out.pdf")]
        [DataRow("qpdf/shared-images-errors-2-out.pdf")]
        [DataRow("qpdf/shared-images-errors.pdf")]
        [DataRow("qpdf/shared-images-pages-out.pdf")]
        [DataRow("qpdf/shared-images.pdf")]
        [DataRow("qpdf/shared-split-01-04.pdf")]
        [DataRow("qpdf/shared-split-05-08.pdf")]
        [DataRow("qpdf/shared-split-09-10.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0049(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("tcpdf/example_003.pdf")]
        [DataRow("tcpdf/example_008.pdf")]
        [DataRow("tcpdf/example_012.pdf")]
        [DataRow("tcpdf/example_013.pdf")]
        [DataRow("tcpdf/example_014.pdf")]
        [DataRow("tcpdf/example_015.pdf")]
        [DataRow("tcpdf/example_016.bad.XML-metadata-SHOULD-have-been-crypted.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0050(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("tcpdf/example_017.pdf")]
        [DataRow("tcpdf/example_018.pdf")]
        [DataRow("tcpdf/example_020.pdf")]
        [DataRow("tcpdf/example_022.pdf")]
        [DataRow("tcpdf/example_023.pdf")]
        [DataRow("tcpdf/example_024.pdf")]
        [DataRow("tcpdf/example_025.pdf")]
        [DataRow("tcpdf/example_026.pdf")]
        [DataRow("tcpdf/example_027.pdf")]
        [DataRow("tcpdf/example_028.pdf")]
        [DataRow("tcpdf/example_030.pdf")]
        [DataRow("tcpdf/example_031.pdf")]
        [DataRow("tcpdf/example_032.pdf")]
        [DataRow("tcpdf/example_033.pdf")]
        [DataRow("tcpdf/example_034.pdf")]
        [DataRow("tcpdf/example_035.pdf")]
        [DataRow("tcpdf/example_036.pdf")]
        [DataRow("tcpdf/example_037.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0051(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("tcpdf/example_038.pdf")]
        [DataRow("tcpdf/example_040.pdf")]
        [DataRow("tcpdf/example_041.pdf")]
        [DataRow("tcpdf/example_042.pdf")]
        [DataRow("tcpdf/example_046.pdf")]
        [DataRow("tcpdf/example_050.pdf")]
        [DataRow("tcpdf/example_051.pdf")]
        [DataRow("tcpdf/example_052.pdf")]
        [DataRow("tcpdf/example_053.pdf")]
        [DataRow("tcpdf/example_054.pdf")]
        [DataRow("tcpdf/example_055.pdf")]
        [DataRow("tcpdf/example_056.pdf")]
        [DataRow("tcpdf/example_057.pdf")]
        [DataRow("tcpdf/example_058.pdf")]
        [DataRow("tcpdf/example_060.pdf")]
        [DataRow("tcpdf/example_061.pdf")]
        [DataRow("tcpdf/example_062.pdf")]
        [DataRow("tcpdf/example_063.pdf")]
        [DataRow("tcpdf/example_064.pdf")]
        [DataRow("tcpdf/example_065.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0052(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("encrypted-with-known-passwords/How to Password Protect PDF Documents.pdf")]
        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-not-encrypted.pdf")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0053(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("novapdf/pdf-example-password=test.pdf")]
        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-password=test.pdf")]
        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-passwords=test,bugger.pdf")]
        [DataRow("encrypted-with-known-passwords/pdf-example-password=test.pdf")]
        [DataRow("encrypted-with-known-passwords/pdfpostman-pdf-sample-password=test123.PDF")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0054(string filepath)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-password=test.pdf", "test")]
        [DataRow("encrypted-with-known-passwords/pdf-example-password=test.pdf", "test")]
        [DataRow("encrypted-with-known-passwords/pdfpostman-pdf-sample-password=test123.PDF", "test123")]
        [DataRow("novapdf/pdf-example-password=test.pdf", "test")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0055_password_protected(string filepath, string password)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, password, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-passwords=test,bugger.pdf", "test")]
        [DataTestMethod]
        public void Test_PDF_bulk_chunk0056_password_protected(string filepath, string password)
        {
            string pdf_filename = GetNormalizedPathToPDFTestFile(filepath);
            string json_filename = GetNormalizedPathToJSONOutputFile(filepath);

            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, password, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }
    }
}

