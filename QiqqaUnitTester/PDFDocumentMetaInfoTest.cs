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

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaUnitTester.PDFDocument
{
    [TestClass]
    public class PDFDocumentMetaInfoTest
    {
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Executes once for the test class. (Optional)
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

        // Mark that this is a unit test method. (Required)
        [DataRow("2009-06-12")]
        [DataRow("2009/06/12")]
        [DataRow("2009-06-12 16:38")]
        [DataRow("2009-06-12T16:38")]
        [DataRow("2009-06-12 16:38:52")]
        [DataRow("2009-06-12T16:38:52")]
        [DataRow("2009-06-12 16:38:52GMT")]
        [DataRow("2009-06-12 16:38:52+02:00")]
        [DataRow("2009-06-12 16:38:52 UTC")]
        [DataRow("2009/06/12 16:38:52+02:00")]
        [DataRow("2009-06-12 16:38:52 GMT")]
        [DataRow("2015-02-14T19:09:26+05:30")]
        [DataRow("2015-02-14T19:09:26")]
        [DataRow("D:20120208094057-08'00'")]
        [DataRow("D:20090612163852+02'00'")]
        [DataRow("D:20070122155202")]
        [DataRow("D:20000420231130Z")]
        [DataTestMethod]
        public void TestMuPDF_ParsePDFTimestampAsProducedByMultipurpJSON(string timestamp_str)
        {
            DateTime? t = MuPDFRenderer.ParsePDFTimestamp(timestamp_str);
            ASSERT.IsNotNull(t, $"input value: {timestamp_str}");
            ASSERT.IsGreaterOrEqual(t.Value.Year, 2000, $"expected a decoded year 2000 or later for input value: {timestamp_str}");
            ASSERT.IsLessOrEqual(t.Value.Year, 2015, $"expected a decoded year 2012 or earlier for input value: {timestamp_str}");
        }

        // Helper function: these tests should exhibit that the decoded JSON is okay and matched the first PDF document sample
        private void TestJSONoutputIsCorrectForPDFdoc1(List<MultiPurpDocumentInfoObject> infos_list)
        {
            ASSERT.AreEqual<int>(infos_list.Count, 1);
            var info = infos_list[0];
            ASSERT.IsNotNull(info.GlobalInfo);
            ASSERT.IsNotNull(info.DocumentFilePath);
            ASSERT.IsNull(info.GatheredErrors);
            var globalinfo = info.GlobalInfo;
            ASSERT.AreEqual<int>(1, globalinfo.Chapters.Value);
            ASSERT.AreEqual<int>(0, globalinfo.DocumentOutlines.Count);
            ASSERT.AreEqual<int>(4, globalinfo.Info.Count);
            ASSERT.IsTrue(globalinfo.GatheredErrors.Log.Contains("first object in xref is not free"));
            ASSERT.AreEqual<string>("PDF-1.3", globalinfo.Version);

            ASSERT.IsNotNull(info.DocumentGeneralInfo);
            var ginf = info.DocumentGeneralInfo;
            ASSERT.AreEqual<string>("PDF 1.3", ginf.Format);
            ASSERT.AreEqual<bool>(false, ginf.WasRepaired.Value);
            ASSERT.AreEqual<int>(4, ginf.MetaInfoDictionary.Count);

            ASSERT.AreEqual<int>(1, info.FirstPage.Value);
            ASSERT.AreEqual<int>(8, info.LastPage.Value);
            var pages = info.PageInfo;
            ASSERT.IsNotNull(pages);
            ASSERT.AreEqual<int>(8, pages.Count);
            var p1 = pages[0];
            ASSERT.AreEqual<int>(11, p1.Fonts.Count);
            ASSERT.AreEqual<int>(1, p1.PageNumber.Value);
            ASSERT.IsNull(p1.PageError);
            var p8 = pages[7];
            ASSERT.AreEqual<int>(8, p8.Fonts.Count);
            ASSERT.AreEqual<int>(8, p8.PageNumber.Value);
            ASSERT.IsNull(p8.PageError);
        }

        // Mark that this is a unit test method. (Required)
        [TestMethod]
        public void TestMuPDF_multipurp_JSON_formatted_snippet1_parses_okay()
        {
            string json_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile(@"fixtures/mutool/multipurp/json-snippets/pdf-info1-formatted.json");
            ASSERT.FileExists(json_filename);

            string json = File.ReadAllText(json_filename);
            ASSERT.IsNotNull(json);

            List<MultiPurpDocumentInfoObject> infos_list = JsonConvert.DeserializeObject<List<MultiPurpDocumentInfoObject>>(json);

            TestJSONoutputIsCorrectForPDFdoc1(infos_list);
        }

        [TestMethod]
        public void TestGetDocumentMetaInfo_on_doc1()
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile(@"fixtures/1.Doc-Many.Metadata.Formats/0001-LDA-paper/2004.04.PNAS.ef997ae1b01762b57b75d8c22fb8cec87406.pdf");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);
            ASSERT.AreEqual<int>(8, info.PageCount);
            ASSERT.AreEqual<bool>(false, info.DocumentIsCorrupted);
            ASSERT.IsLessOrEqual(10000, info.raw_metadump_text.Length);
            TestJSONoutputIsCorrectForPDFdoc1(info.raw_decoded_json);

            object json_doc = JsonConvert.DeserializeObject(info.raw_metadump_text);
            string json_text = JsonConvert.SerializeObject(json_doc, Formatting.Indented).Replace("\r\n", "\n");

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );

            info.ClearRawContent();
            ASSERT.IsNull(info.raw_metadump_text);
            ASSERT.IsNull(info.raw_decoded_json);
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

        [DataRow("./annots.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0010_annotations(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("./atlas_over.pdf")]
        [DataRow("./btxdoc.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0011_misc_imgHEX(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("./pdfbox/testPDFPackage.pdf")]
        [DataRow("./attachment.formular.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0012_attachedFiles(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("./html-standard.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0021_document_outlines_and_int_plus_ext_URL_links(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("./html-standard.pdf.multipurp.json")]
        [DataRow("./bad-crash-due-to-illegal-pdf.json")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0022_load_from_cached_json(string filepath)
        {
            string json_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/mutool/multipurp/json-snippets/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(json_filename);

            // Same as the full-fledged metadata extractor, only this time we SKIP/IGNORE a lot of the incoming JSON
            // by not defining it in the deserialization target class structure.
            //
            // MAYBE that will improve our load/deserialize timings significantly for huge JSON streams (such as the one
            // produced by html-standard.pdf, currently clocking at about ~50MByte!)
            //
            // [Edit:] turns out the most time is spent in metadump itself in `pdf_load_page()` which is
            // used to obtain the more advanced metadata bits (annotations and such).
            //
            // ... Now how about performance when we wish to only dig out the PageCount from an already
            // *saved* JSON file like that?
            string json = File.ReadAllText(json_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.ParseDocumentMetaInfo(json);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, json_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("issue-0007-sorax-blank-page.pdf")]
        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-not-encrypted.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0081_encryption_and_permissions(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("tcpdf/example_016.bad.XML-metadata-SHOULD-have-been-crypted.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0082_ill_formatted_pdfs(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("asbach uralt - dual column scanned old.pdf")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0090_no_text_needs_ocr(string filepath)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, null, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }



        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-password=test.pdf", "test")]
        [DataRow("encrypted-with-known-passwords/keyboard-shortcuts-windows-passwords=test,bugger.pdf", "test")]
        [DataRow("encrypted-with-known-passwords/pdf-example-password=test.pdf", "test")]
        [DataRow("encrypted-with-known-passwords/pdfpostman-pdf-sample-password=test123.PDF", "test123")]
        [DataRow("novapdf/pdf-example-password=test.pdf", "test")]
        [DataTestMethod]
        public void Test_PDF_metadata_extraction_via_multipurp_chunk0100_crypted_with_access_password(string filepath, string password)
        {
            string pdf_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile($"fixtures/PDF/{ filepath.Replace("./", "") }");
            ASSERT.FileExists(pdf_filename);
            PDFDocumentMuPDFMetaInfo info = MuPDFRenderer.GetDocumentMetaInfo(pdf_filename, password, ProcessPriorityClass.Normal);

            string json_text = ProduceJSONtext4Comparison(info);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, pdf_filename),
                ApprovalTests.Approvals.GetReporter()
            );
        }
    }
}

