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

            ASSERT.AreEqual<int>(1, info.PageInfoSeries.Count);
            var pageSeries = info.PageInfoSeries[0];
            ASSERT.AreEqual<string>("AllPages", pageSeries.InfoMode);
            ASSERT.IsNull(pageSeries.GatheredErrors);
            ASSERT.IsNotNull(pageSeries.DocumentGeneralInfo);
            var ginf = pageSeries.DocumentGeneralInfo;
            ASSERT.AreEqual<string>("PDF 1.3", ginf.Format);
            ASSERT.AreEqual<bool>(false, ginf.WasRepaired.Value);
            ASSERT.AreEqual<int>(4, ginf.MetaInfoDictionary.Count);

            ASSERT.AreEqual<int>(1, pageSeries.PageSequence.Count);
            MultiPurpPageSequenceItem seq = pageSeries.PageSequence[0];
            ASSERT.IsNotNull(seq);
            ASSERT.AreEqual<int>(1, seq.FirstPage.Value);
            ASSERT.AreEqual<int>(8, seq.LastPage.Value);
            var pages = seq.Info;
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
        public void TestMuPDF_multipurp_JSON_snippet1_parses_okay()
        {
            string json_filename = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile(@"fixtures/mutool/multipurp/json-snippets/pdf-info1.json");
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
            ASSERT.IsLessOrEqual(10000, info.raw_multipurp_text.Length);
            TestJSONoutputIsCorrectForPDFdoc1(info.raw_decoded_json);

            object json_doc = JsonConvert.DeserializeObject(info.raw_multipurp_text);
            string json_text = JsonConvert.SerializeObject(json_doc, Formatting.Indented).Replace("\r\n", "\n");

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string path = MiscTestHelpers.GetNormalizedPathToAnyTestDataTestFile(@"fixtures/1.Doc-Many.Metadata.Formats/0001-LDA-paper/2004.04.PNAS.ef997ae1b01762b57b75d8c22fb8cec87406-multipurp.json");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_text, path),
                ApprovalTests.Approvals.GetReporter()
            );

            info.ClearRawContent();
            ASSERT.IsNull(info.raw_multipurp_text);
            ASSERT.IsNull(info.raw_decoded_json);
        }

        [TestMethod]
        public void TestMethod1()
        {
            ASSERT.IsTrue(true);
        }
    }
}
