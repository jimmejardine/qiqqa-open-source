using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Files;

namespace QiqqaHasher
{
    /// <summary>
    /// A simple fingerprinting tool to serve as CLI utility to help calculate the precise (PDF) Document hash
    /// as used by Qiqqa for referencing/indexing any document it manages in its libraries.
    /// 
    /// The Qiqqa hash is basically a SHA1 hash, but it has a twist due to historical reasons:
    /// originally Qiqqa used a buggy/inprecise binary-to-hex stringifier and we've been stuck 
    /// with that one ever since. It doesn't really matter much with regards to producing
    /// a unique reference string for every different document, but it also means we cannot use
    /// the standard SHA1 hash calculating tools, such as the OpenSSL CLI tools, for calculating
    /// PDF/document hashes in our shell scripts, so we need to provide our own CLI hasher tool
    /// to ensure all our scripts and Qiqqa itself all produce the same hash for the same document
    /// and no mistake.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string filepath = args[0];
            
                string fingerprint = StreamFingerprint.FromFile(filepath);

                Console.WriteLine(fingerprint);
            }
            else
            {
                Console.WriteLine(@"
QiqqaHasher <filepath>

Calculates the Qiqqa compatible fingerprint hash for the given file.
");
            }
        }
    }
}
