using System.Collections.Generic;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Synchronisation
{

#if SAMPLE_LOAD_CODE

    internal class HistoricalSyncFile
    {
        private static string GetFILENAME(Library library)
        {
            return Path.GetFullPath(Path.Combine(library.LIBRARY_BASE_PATH, @"Qiqqa.sync_md5"));
        }

        internal static Dictionary<string, string> GetHistoricalSyncFile(Library library)
        {
            Dictionary<string, string> historical_sync_file = (Dictionary<string, string>)SerializeFile.LoadSafely(GetFILENAME(library));
            if (null == historical_sync_file)
            {
                Logging.Info("There is no sync history, so creating a new one");
                historical_sync_file = new Dictionary<string, string>();
            }

            return historical_sync_file;
        }

        internal static void PutHistoricalSyncFile(WebLibraryDetail web_library_detail, Dictionary<string, string> historical_sync_file)
        {
            SerializeFile.SaveSafely(GetFILENAME(library), historical_sync_file);
        }
    }

#endif

}
