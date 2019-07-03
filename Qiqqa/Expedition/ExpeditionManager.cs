using System;
using System.IO;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Files;
using Utilities.Misc;

namespace Qiqqa.Expedition
{
    public class ExpeditionManager
    {
        private Library library;

        public string Filename_Store
        {
            get
            {
                return library.LIBRARY_BASE_PATH + "Qiqqa.expedition";
            }
        }

        public ExpeditionManager(Library library)
        {
            this.library = library;
        }

        public delegate void RebuiltExpeditionCompleteDelegate();
        public void RebuildExpedition(int num_topics, bool add_autotags, bool add_tags, RebuiltExpeditionCompleteDelegate rebuiltexpeditioncompletedelegate)
        {
            Logging.Info("+Rebuilding Expedition");
            StatusManager.Instance.ClearCancelled("Expedition");
            ExpeditionDataSource eds = ExpeditionBuilder.BuildExpeditionDataSource(library, num_topics, add_autotags, add_tags, ExpeditionBuilderProgressUpdate);
            SerializeFile.SaveSafely(Filename_Store, eds);
            expedition_data_source = eds;
            Logging.Info("-Rebuilding Expedition");

            if (null != rebuiltexpeditioncompletedelegate)
            {
                Logging.Info("+Notifying of rebuilt Expedition");
                rebuiltexpeditioncompletedelegate();
                Logging.Info("-Notifying of rebuilt Expedition");
            }
        }

        public int RecommendedThemeCount
        {
            get
            {
                return (int)Math.Ceiling(Math.Sqrt(this.library.PDFDocuments.Count));
            }
        }

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
                            Logging.Info("+Loading Expedition");
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

        private bool ExpeditionBuilderProgressUpdate(string message, double percentage_complete)
        {
            if (1 != percentage_complete)
            {
                StatusManager.Instance.UpdateStatus("Expedition", message, percentage_complete, true);
            }
            else
            {
                StatusManager.Instance.UpdateStatus("Expedition", message);
            }

            return !StatusManager.Instance.IsCancelled("Expedition");
        }

        public bool IsStale
        {
            get
            {
                if (null == this.expedition_data_source) return false;
                return (DateTime.UtcNow.Subtract(this.expedition_data_source.date_created).TotalDays > 28);
            }
        }
    }
}
