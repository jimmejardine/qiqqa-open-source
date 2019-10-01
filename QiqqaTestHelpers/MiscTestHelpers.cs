using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace QiqqaTestHelpers
{
    public static class MiscTestHelpers
    {
        public static string GetNormalizedPathToBibTeXTestFile(string bibtex_filepath)
        {
            string fnpath = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"../../data/fixtures/bibtex", bibtex_filepath));
            return fnpath;
        }

        public static string GetNormalizedPathToPubMedXMLTestFile(string pubmed_filepath)
        {
            string fnpath = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"../../data/fixtures/pubmed", pubmed_filepath));
            return fnpath;
        }

        public static string GetNormalizedPathToRISTestFile(string ris_filepath)
        {
            string fnpath = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"../../data/fixtures/RIS", ris_filepath));
            return fnpath;
        }

        public static string GetNormalizedPathToSerializationTestFile(string test_filepath)
        {
            string fnpath = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"../../data/fixtures/serialization", test_filepath));
            return fnpath;
        }

        public static string GetTestFileContent(string bibtex_filepath)
        {
            string data = File.ReadAllText(bibtex_filepath, Encoding.UTF8);
            return data;
        }

        public static string GetNormalizedPathToAnyFile(string filepath)
        {
            string fnpath = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"../../..", filepath));
            return fnpath;
        }

    }
}
