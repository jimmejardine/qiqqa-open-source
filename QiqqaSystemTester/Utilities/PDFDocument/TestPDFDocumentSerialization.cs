using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using static QiqqaTestHelpers.MiscTestHelpers;
using Qiqqa.Documents.PDF;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.DocumentLibrary;

namespace QiqqaSystemTester
{
    [TestClass]
    public class TestPDFDocumentSerialization
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [DataRow("PDFDocument/v80-record-0001.json")]
        [DataTestMethod]
        public void Deserialize_v80_ClassicalQiqqaRecord(string input_path)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToSerializationTestFile(input_path);
            ASSERT.FileExists(path);

            string input = GetTestFileContent(path);

            PDFDocument record = JsonConvert.DeserializeObject<PDFDocument>(input);
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void SerializeBasicLibrary()
        {
            // TODO: overrule DB directory etc by overriding LIBRARY_BASE_PATH et al...

            // set up a clean library with the default two manual files:
            WebLibraryDetail new_web_library_detail = new WebLibraryDetail();
            new_web_library_detail.Id = "UnitTest";
            new_web_library_detail.Title = "Local UnitTest Library";
            new_web_library_detail.Description = "This is the library that comes with your Qiqqa unit test(s).";
            new_web_library_detail.Deleted = false;
            new_web_library_detail.IsLocalGuestLibrary = true;
            new_web_library_detail.library = new Library(new_web_library_detail);

            PDFDocument doc2 = QiqqaManualTools.AddManualsToLibrary(new_web_library_detail.library);
            ASSERT.IsNotNull(doc2);
            ASSERT.AreEqual<int>(new_web_library_detail.library.PDFDocuments.Count, 2);

            string json = JsonConvert.SerializeObject(doc2, Formatting.Indented);

            // Perform comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(doc2, Formatting.Indented).Replace("\r\n", "\n");
            string path = GetNormalizedPathToSerializationTestFile("PDFDocument/SerializeBasicLibrary/Manual-Doc-02-serialized.json");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new DataTestApprovalTextWriter(json_out, path),
                new DataTestLocationNamer(path) /* GetDefaultNamer() */,
                ApprovalTests.Approvals.GetReporter()
            );

            //new_web_library_detail.library.Dispose();

            ASSERT.IsTrue(true);
        }
    }
}
