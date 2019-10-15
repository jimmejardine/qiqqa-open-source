using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Files;
using Utilities.Misc;

namespace Qiqqa.Synchronisation
{
    internal class HistoricalSyncFile
    {
        static string GetFILENAME(Library library)
        {
            return Path.GetFullPath(Path.Combine(library.LIBRARY_BASE_PATH, @"Qiqqa.sync_md5"));
        }

        internal static Dictionary<string, string> GetHistoricalSyncFile(Library library)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(library), "Loading sync history");

            Dictionary<string, string> historical_sync_file = (Dictionary<string, string>)SerializeFile.LoadSafely(GetFILENAME(library));
            if (null == historical_sync_file)
            {
                Logging.Info("There is no sync history, so creating a new one");
                historical_sync_file = new Dictionary<string, string>();
            }

            return historical_sync_file;
        }

        internal static void PutHistoricalSyncFile(Library library, Dictionary<string, string> historical_sync_file)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(library), "Saving sync history");
            SerializeFile.SaveSafely(GetFILENAME(library), historical_sync_file);
        }
    }
}
