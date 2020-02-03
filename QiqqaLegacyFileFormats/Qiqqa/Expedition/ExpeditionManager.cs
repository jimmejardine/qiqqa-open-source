using System;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Expedition
{

#if SAMPLE_LOAD_CODE

    public class ExpeditionManager
    {
        public string Filename_Store => Path.GetFullPath(Path.Combine(Library.LIBRARY_BASE_PATH, @"Qiqqa.expedition"));

        public void RebuildExpedition(int num_topics, bool add_autotags, bool add_tags)
        {
            try
            {
                ExpeditionDataSource eds = ExpeditionBuilder.BuildExpeditionDataSource(Library, num_topics, add_autotags, add_tags);
                SerializeFile.SaveSafely(Filename_Store, eds);
                expedition_data_source = eds;
            }
            finally
            {
            }
            Logging.Info("-Rebuilding Expedition");
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
    }

#endif

}
