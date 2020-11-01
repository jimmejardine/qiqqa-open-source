using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Synchronisation
{
    internal class HistoricalSyncFile
    {
        internal static string GetSyncDbFilename(WebLibraryDetail web_library_detail)
        {
            return Path.GetFullPath(Path.Combine(web_library_detail.LIBRARY_BASE_PATH, @"Qiqqa.sync_md5"));
        }

        internal static Dictionary<string, string> GetHistoricalSyncFile(WebLibraryDetail web_library_detail)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), "Loading sync history");

            Dictionary<string, string> historical_sync_file = (Dictionary<string, string>)SerializeFile.LoadSafely(GetSyncDbFilename(web_library_detail));
            if (null == historical_sync_file)
            {
                Logging.Info("There is no sync history, so creating a new one");
                historical_sync_file = new Dictionary<string, string>();
            }

            return historical_sync_file;
        }

        internal static void PutHistoricalSyncFile(WebLibraryDetail web_library_detail, Dictionary<string, string> historical_sync_file)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), "Saving sync history");
            SerializeFile.SaveSafely(GetSyncDbFilename(web_library_detail), historical_sync_file);
        }
    }
}
