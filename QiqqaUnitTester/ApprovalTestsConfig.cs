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

using ApprovalTests.Approvers;
using ApprovalTests.Core;
using ApprovalTests.Core.Exceptions;
using static QiqqaTestHelpers.MiscTestHelpers;
using ApprovalTests.Reporters;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using System.Text.RegularExpressions;
using Utilities.Files;
using System;

[assembly: UseReporter(typeof(QiqqaTestHelpers.DiffReporterWithApprovalPower))]



// Helper classes:

namespace QiqqaTestHelpers
{
    public class DataTestLocationNamer : ApprovalTests.Namers.AssemblyLocationNamer
    {
        public DataTestLocationNamer(string data_filepath)
        {
            DataFile = data_filepath;
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
            // ensure the filename/path has a sane length:
            return MkLegalSizedPath(basename, ApprovalTests.Writers.WriterUtils.Approved);
        }

        public override string GetReceivedFilename(string basename)
        {
            // ensure the filename/path has a sane length:
            return MkLegalSizedPath(basename, ApprovalTests.Writers.WriterUtils.Received);
        }

        private static string CamelCaseShorthand(string s)
        {
            // uppercase every first character of a 'word'
            s = Regex.Replace(" " + s, " [^a-z0-9]*([a-z])", delegate (Match match)
            {
                string v = match.Groups[0].Value;
                return v.ToUpper();
            }, RegexOptions.IgnoreCase);

            // strip out anything that's not a capital
            s = Regex.Replace(s, "[^A-Z]+", "");

            return s;
        }

        private static string SanitizeFilename(string s)
        {
            // The following characters are invalid as file or folder names on Windows using NTFS:
            //     / ? < > \ : * | "
            // and any character you can type with the Ctrl key
            //
            // In addition to the above illegal characters the caret ^ is also not permitted under
            // Windows Operating Systems using the FAT file system.
            //
            // --> replace any run of these characters with a single underscore.
            s = Regex.Replace(s.Trim(), "[\x00-\x1F/?<>\\*|\":^]+", "_");
            // filenames may not start or end with a dot either:
            while (s.StartsWith("."))
            {
                s = s.Substring(1);
            }
            while (s.EndsWith("."))
            {
                s = s.Substring(0, s.Length - 1);
            }

            if (s.Length == 0)
            {
                s = "T";
            }
            return s;
        }

        private static string SubStr(string s, int startIndex, int trimLength = int.MaxValue)
        {
            int l = s.Length;
            if (startIndex >= l)
            {
                return "";
            }
            startIndex = Math.Max(0, startIndex);
            int subLen = Math.Max(0, Math.Min(trimLength, l - startIndex));
            if (subLen == 0)
            {
                return "";
            }
            return s.Substring(startIndex, subLen);
        }

        private string MkLegalSizedPath(string basename, string typeIdStr)
        {
            const int PATH_MAX = 240;  // must be less than 255 / 260 - see also https://kb.acronis.com/content/39790

            string root = Path.GetDirectoryName(basename);
            string name = Path.GetFileName(basename);
            string dataname = Path.GetFileNameWithoutExtension(DataFile);
            string ext = SubStr(Path.GetExtension(DataFile), 1).Trim();   // produce the extension without leading dot
            if (ext.StartsWith("bib"))
            {
                ext = SubStr(ext, 3).Trim();
            }
            if (ext.Length > 0)
            {
                ext = "." + ext;
            }

            // UNC long filename/path support by forcing this to be a UNC path:
            string filenamebase = $"{dataname}.{name}{ext}{ExtensionWithDot}";

            // first make the full path without the approved/received, so that that bit doesn't make a difference
            // in the length check and subsequent decision to produce a shorthand filename path or not:

            // It's not always needed, but do the different shorthand conversions anyway and pick the longest fitting one:
            string short_tn = SanitizeFilename(CamelCaseShorthand(name));
            string short_dn = SanitizeFilename(SubStr(dataname, 0, 10) + CamelCaseShorthand(dataname));

            string hash = StreamMD5.FromText(filenamebase).ToUpper();
            string short_hash = SubStr(hash, 0, Math.Max(6, 11 - short_tn.Length));

            // this variant will fit in the length criterium, guaranteed:
            string alt_filepath0 = Path.GetFullPath(Path.Combine(root, $"{short_dn}.{short_hash}_{short_tn}{ext}{typeIdStr}{ExtensionWithDot}"));
            string filepath = alt_filepath0;

            // next, we construct the longer variants to check if they fit.
            //
            // DO NOTE that we create a path without typeIdStr part first, because we want both received and approved files to be based
            // on the *same* alt selection decision!

            string picked_alt_filepath = Path.GetFullPath(Path.Combine(root, $"{short_dn}.{short_hash}_{short_tn}{ext}.APPROVEDXYZ{ExtensionWithDot}"));

            name = SanitizeFilename(name);
            dataname = SanitizeFilename(dataname);

            string alt_filepath1 = Path.GetFullPath(Path.Combine(root, $"{short_dn}_{short_hash}.{name}{ext}.APPROVEDXYZ{ExtensionWithDot}"));
            if (alt_filepath1.Length < PATH_MAX)
            {
                filepath = Path.GetFullPath(Path.Combine(root, $"{short_dn}_{short_hash}.{name}{ext}{typeIdStr}{ExtensionWithDot}"));
                picked_alt_filepath = alt_filepath1;
            }

            // second alternative: only pick this one if it fits and produces a longer name:
            string alt_filepath2 = Path.GetFullPath(Path.Combine(root, $"{dataname}.{short_hash}_{short_tn}{ext}.APPROVEDXYZ{ExtensionWithDot}"));
            if (alt_filepath2.Length < PATH_MAX && alt_filepath2.Length > picked_alt_filepath.Length)
            {
                filepath = Path.GetFullPath(Path.Combine(root, $"{dataname}.{short_hash}_{short_tn}{ext}{typeIdStr}{ExtensionWithDot}"));
                picked_alt_filepath = alt_filepath2;
            }
            else
            {
                // third alt: the 'optimally trimmed' test name used as part of the filename:
                int trim_length = PATH_MAX - alt_filepath0.Length + 10 - 1;
                string short_dn2 = SanitizeFilename(SubStr(dataname, 0, trim_length) + CamelCaseShorthand(dataname));
                string alt_filepath3 = Path.GetFullPath(Path.Combine(root, $"{short_dn2}.{short_hash}_{short_tn}{ext}{typeIdStr}{ExtensionWithDot}"));
                if (alt_filepath3.Length < PATH_MAX && alt_filepath3.Length > picked_alt_filepath.Length)
                {
                    filepath = Path.GetFullPath(Path.Combine(root, $"{short_dn2}.{short_hash}_{short_tn}{ext}{typeIdStr}{ExtensionWithDot}"));
                    picked_alt_filepath = alt_filepath3;
                }
            }

            // fourth alt: the full, unadulterated path; if it fits in the length criterium, take it anyway
            string alt_filepath4 = Path.GetFullPath(Path.Combine(root, $"{dataname}.{name}{ext}.APPROVEDXYZ{ExtensionWithDot}"));
            if (alt_filepath4.Length < PATH_MAX)
            {
                // UNC long filename/path support by forcing this to be a UNC path:
                filepath = Path.GetFullPath(Path.Combine(root, $"{dataname}.{name}{ext}{typeIdStr}{ExtensionWithDot}"));
                picked_alt_filepath = alt_filepath4;
            }

            return filepath;
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

            // BC4 cannot handle UNC paths, which is what AlphaFS uses to support overlong paths (> 260 chars)
            // hence we'll have convert these UNC paths back to local/native format for BC4 to be able to open
            // the received+approved files for comparison. The rest of the test application should use the
            // UNC paths though.
            if (approved.StartsWith("\\\\?\\"))
            {
                approved = approved.Substring(4);
            }
            if (received.StartsWith("\\\\?\\"))
            {
                received = received.Substring(4);
            }
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
            if (!File.Exists(received)) return false;
#if false
                File.Delete(this.approved);
#endif
            if (File.Exists(approved)) return false;
            File.Copy(received, approved);
            return File.Exists(approved);
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

