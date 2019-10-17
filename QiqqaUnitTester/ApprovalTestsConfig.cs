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
using ApprovalTests.Core;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;
using ApprovalTests.Core.Exceptions;
using ApprovalTests.Approvers;

[assembly: UseReporter(typeof(QiqqaTestHelpers.DiffReporterWithApprovalPower))]



// Helper classes:

namespace QiqqaTestHelpers
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
                path = Path.GetFullPath(Path.Combine(path, @"../ref-output"));
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
            string ext = Path.GetExtension(DataFile).Substring(1);   // produce the extension without leading dot
            if (ext.StartsWith("bib"))
            {
                ext = ext.Substring(3);
            }
            if (ext.Length > 0)
            {
                ext = "." + ext;
            }

            // UNC long filename/path support by forcing this to be a UNC path:
            return Path.GetLongPath(Path.GetFullPath(Path.Combine(root, $"{dataname}.{name}{ext}{ApprovalTests.Writers.WriterUtils.Approved}{ExtensionWithDot}")));
        }

        public override string GetReceivedFilename(string basename)
        {
            string root = Path.GetDirectoryName(basename);
            string name = Path.GetFileName(basename);
            string dataname = Path.GetFileNameWithoutExtension(DataFile);
            string ext = Path.GetExtension(DataFile).Substring(1);   // produce the extension without leading dot
            if (ext.StartsWith("bib"))
            {
                ext = ext.Substring(3);
            }
            if (ext.Length > 0)
            {
                ext = "." + ext;
            }

            return Path.GetLongPath(Path.GetFullPath(Path.Combine(root, $"{dataname}.{name}{ext}{ApprovalTests.Writers.WriterUtils.Received}{ExtensionWithDot}")));
        }
    }


    // https://stackoverflow.com/questions/37604285/how-do-i-automatically-approve-approval-tests-when-i-run-them
    public class DiffReporterWithApprovalPower : DiffReporter, IReporterWithApprovalPower
    {
        private string approved;
        private string received;

        public DiffReporterWithApprovalPower()
            : base()
        { }

        public override void CleanUp(string approved, string received)
        {
            base.CleanUp(approved, received);
        }

        public override bool IsWorkingInThisEnvironment(string forFile)
        {
            return base.IsWorkingInThisEnvironment(forFile);
        }

        public override void Report(string approved, string received)
        {
            this.approved = approved;
            this.received = received;

            base.Report(approved, received);
        }

        /// <summary>
        /// Executed as part of the `Approver` logic in
        /// 
        /// ```
        /// public static void Verify(IApprovalApprover approver, IApprovalFailureReporter reporter)
        /// {
        ///     if (approver.Approve())
        ///     {
        ///         approver.CleanUpAfterSuccess(reporter);
        ///     }
        ///     else
        ///     {
        ///         approver.ReportFailure(reporter);
        /// 
        ///         if (reporter is IReporterWithApprovalPower power && power.ApprovedWhenReported())
        ///                                                                   ^^^^^^^^^^^^^^^^^^^^^^
        ///         {
        ///             approver.CleanUpAfterSuccess(power);
        ///         }
        ///         else
        ///         {
        ///             approver.Fail();
        ///         }
        ///     }
        /// }
        /// ```
        /// </summary>
        /// <returns>Return `false` when NOT approved automatically after all; `true` when this code auto-approves the test output, 
        /// e.g. when the `*.approved.*` reference file does not (yet) exist.</returns>
        bool IReporterWithApprovalPower.ApprovedWhenReported()
        {
            if (!File.Exists(this.received)) return false;
#if false
            File.Delete(this.approved);
#endif
            if (File.Exists(this.approved)) return false;
            File.Copy(this.received, this.approved);
            return File.Exists(this.approved);
        }
    }


    // A variant on
	// https://stackoverflow.com/questions/37604285/how-do-i-automatically-approve-approval-tests-when-i-run-them
	// but now with the added tweak that this bugger gets to approve
	// **before Beyond Compare (or your favorite compare app) gets invoked**: *that*
	// happens in the `Report()` call in the Approver *before* 
	// `DiffReporterWithApprovalPower` would get a chance to 'approve' the
	// received content!
    public class QiqqaApprover : FileApprover /* IApprovalApprover */
    {
        public QiqqaApprover(string json_out, string bibtex_filepath) 
            : base(
                new DataTestApprovalTextWriter(json_out, bibtex_filepath),
                new DataTestLocationNamer(bibtex_filepath) /* GetDefaultNamer() */,
                normalizeLineEndingsForTextFiles: true)
        {
        }

        public override ApprovalException Approve(string approvedPath, string receivedPath)
        {
            if (!File.Exists(approvedPath))
            {
                //return new ApprovalMissingException(receivedPath, approvedPath);

                // Auto-approve BEFORE BeyondCompare gets invoked: generate the approved file on the spot.
                File.Copy(receivedPath, approvedPath, overwrite: true);

                return null;
            }

            return base.Approve(approvedPath, receivedPath);
        }
    }
}


