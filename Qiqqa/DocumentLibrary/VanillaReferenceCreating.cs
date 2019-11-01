using System;

namespace Qiqqa.DocumentLibrary
{
    public static class VanillaReferenceCreating
    {
        internal static bool IsVanillaReferenceFingerprint(string fingerprint)
        {
            return fingerprint.EndsWith("_REF");
        }

        internal static string CreateVanillaReferenceFingerprint()
        {
            string fingerprint = Guid.NewGuid().ToString().Replace("-", "").ToUpper() + "_REF";
            return fingerprint;
        }
    }
}