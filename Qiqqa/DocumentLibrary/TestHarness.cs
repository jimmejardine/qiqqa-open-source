using Utilities;

namespace Qiqqa.DocumentLibrary
{
    public class TestHarness
    {
        public static void TestWatchFolder()
        {
            Daemon daemon = new Daemon("DUMMY");
            daemon.Start(DaemonEntry, daemon);
        }

        static void DaemonEntry(object daemon_object)
        {
            Library library = Library.GuestInstance;
            library.WebLibraryDetail.FolderToWatch = @"C:\\temp\\qiqqawatch";

            Daemon daemon = (Daemon)daemon_object;
            while (true)
            {
                library.FolderWatcherManager.TaskDaemonEntryPoint(daemon);
                daemon.Sleep(1000);
            }
        }

    }
}
