using System;
using System.Collections.Generic;
using System.Diagnostics;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary.AITagsStuff
{

#if SAMPLE_LOAD_CODE

    public class AITagManager
    {
        private TypedWeakReference<Library> library;
        public Library LibraryRef => library?.TypedTarget;

        private AITags current_ai_tags_record;
        private object in_progress_lock = new object();
        private bool regenerating_in_progress = false;

        public AITagManager(Library library)
        {
            this.library = new TypedWeakReference<Library>(library);

            current_ai_tags_record = new AITags();

            // Attempt to load the existing tags
            try
            {
                if (File.Exists(Filename_Store))
                {
                    Stopwatch clk = Stopwatch.StartNew();
                    Logging.Info("+Loading AutoTags");
                    current_ai_tags_record = SerializeFile.ProtoLoad<AITags>(Filename_Store);
                    Logging.Info("-Loading AutoTags (time spent: {0} ms)", clk.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem loading existing AutoTags");
            }
        }

        private string Filename_Store => Path.GetFullPath(Path.Combine(LibraryRef.LIBRARY_BASE_PATH, @"Qiqqa.autotags"));

        public AITags AITags => current_ai_tags_record;
    }

#endif

}
