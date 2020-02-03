using System;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Expedition
{
    public class ExpeditionManager
    {
        private TypedWeakReference<Library> library;
        public Library Library => library?.TypedTarget;

        public string Filename_Store => Path.GetFullPath(Path.Combine(Library.LIBRARY_BASE_PATH, @"Qiqqa.expedition"));

        public ExpeditionManager(Library library)
        {
            this.library = new TypedWeakReference<Library>(library);
        }

        public delegate void RebuiltExpeditionCompleteDelegate();

        public void RebuildExpedition(int num_topics, bool add_autotags, bool add_tags, RebuiltExpeditionCompleteDelegate rebuiltexpeditioncompletedelegate)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            StatusManager.Instance.UpdateStatus("Expedition", "Rebuilding Expedition");

            try
            {
                Library.IsBusyRegeneratingTags = true;

                ExpeditionDataSource eds = ExpeditionBuilder.BuildExpeditionDataSource(Library, num_topics, add_autotags, add_tags, ExpeditionBuilderProgressUpdate);
                SerializeFile.SaveSafely(Filename_Store, eds);
                expedition_data_source = eds;
            }
            finally
            {
                Library.IsBusyRegeneratingTags = false;

                StatusManager.Instance.ClearCancelled("Expedition");
            }
            Logging.Info("-Rebuilding Expedition");

            if (null != rebuiltexpeditioncompletedelegate)
            {
                Logging.Info("+Notifying of rebuilt Expedition");
                rebuiltexpeditioncompletedelegate();
                Logging.Info("-Notifying of rebuilt Expedition");
            }
        }

        public int RecommendedThemeCount => (int)Math.Ceiling(Math.Sqrt(Library.PDFDocuments.Count));

        private ExpeditionDataSource expedition_data_source = null;
        public ExpeditionDataSource ExpeditionDataSource
        {
            get
            {
                if (null == expedition_data_source)
                {
                    try
                    {
                        if (File.Exists(Filename_Store))
                        {
                            Logging.Info("+Loading Expedition: {0}", Filename_Store);
                            // TODO: Analyse code flow and find out if we can DELAY-LOAD expedition and brainstorm
                            // data as those CAN be wild & huge and cause OutOfMemory issues, which we cannot fix in-app
                            // as these buggers load as part of the init phase. :-(
                            expedition_data_source = (ExpeditionDataSource)SerializeFile.LoadSafely(Filename_Store);
                            Logging.Info("-Loading Expedition");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem reloading the saved Expedition");
                    }
                }

                return expedition_data_source;
            }
        }

        private bool ExpeditionBuilderProgressUpdate(string message, long current_update_number, long total_update_count)
        {
            StatusManager.Instance.UpdateStatus("Expedition", message, current_update_number, total_update_count, cancellable: current_update_number != total_update_count);

            return !StatusManager.Instance.IsCancelled("Expedition");
        }

        public bool IsStale
        {
            get
            {
                if (null == expedition_data_source) return false;
                return (DateTime.UtcNow.Subtract(expedition_data_source.date_created).TotalDays > 28);
            }
        }
    }
}
