using System;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Synchronisation.MetadataSync
{
    public class SynchronisationState
    {
        public string filename;             // fingerprint.extension
        public string fingerprint;
        public string extension;

        public string md5_previous;         // From global .sync file

        public string md5_local;            // From local filesystem
        public LibraryDB.LibraryItem library_item;

        public string md5_remote;           // From S3        

        public override string ToString()
        {
            return String.Format("{0}{1}", filename, md5_local == md5_remote ? "" : " CHANGED");
        }
    }
}
