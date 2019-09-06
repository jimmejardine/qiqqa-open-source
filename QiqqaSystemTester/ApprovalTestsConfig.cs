/*
* Welcome to ApprovalTests.
* ====
* 
* Please add:
* 
* ```
* [UseReporter(typeof(DiffReporter))]
* ```
* 
* to your class, test method or assembly.
* 
* Why:
* ----
* 
* ApprovalTests uses the `[UseReporter]` attribute from your test class, method or assembly. When you do this ApprovalTest will launch the result using that reporter, for example in your diff tool.
* 
* You can find several reporters in `ApprovalTests.Reporters` namespace, or create your own by extending the `ApprovalTests.Core.IApprovalFailureReporter` interface.
* 
* Find more at: http://blog.approvaltests.com/2011/12/using-reporters-in-approval-tests.html
* 
* Best Practice:
* ----
* 
* Add an *assembly* level configuration. Create a file in your base directory with the name `ApprovalTestsConfig.cs`, and the contents:
* 
* ```
* using ApprovalTests.Reporters;
* 
* [assembly: UseReporter(typeof(DiffReporter))]
* ```
* 
*/

using System.IO;
using static QiqqaTestHelpers.MiscTestHelpers;
using ApprovalTests.Reporters;

[assembly: UseReporter(typeof(DiffReporter))]



// Helper classes:

namespace QiqqaSystemTester
{
    public class DataTestLocationNamer : ApprovalTests.Namers.AssemblyLocationNamer
    {
        public DataTestLocationNamer(string data_filepath)
        {
            this.DataFile = data_filepath;
        }

        public string DataFile;

        public override string SourcePath
        {
            get
            {
                //string path = base.SourcePath;
                string path = GetNormalizedPathToBibTeXTestFile(DataFile);
                path = Path.GetFullPath(Path.Combine(path, "../output"));
                return path;
            }
        }
    }


    public class DataTestApprovalTextWriter : ApprovalTests.ApprovalTextWriter
    {
        public DataTestApprovalTextWriter(string data, string data_filepath, string extensionWithoutDot = "json")
            : base(data, extensionWithoutDot)
        {
            DataFile = data_filepath;
        }

        private string DataFile;

        public override string GetApprovalFilename(string basename)
        {
            string root = Path.GetDirectoryName(basename);
            string name = Path.GetFileName(basename);
            string dataname = Path.GetFileNameWithoutExtension(DataFile);
            return Path.GetFullPath(Path.Combine(root, $"{dataname}.{name}{ApprovalTests.Writers.WriterUtils.Approved}{ExtensionWithDot}"));
        }

        public override string GetReceivedFilename(string basename)
        {
            string root = Path.GetDirectoryName(basename);
            string name = Path.GetFileName(basename);
            string dataname = Path.GetFileNameWithoutExtension(DataFile);
            return Path.GetFullPath(Path.Combine(root, $"{dataname}.{name}{ApprovalTests.Writers.WriterUtils.Received}{ExtensionWithDot}"));
        }
    }
}


