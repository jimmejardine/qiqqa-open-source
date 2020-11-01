using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{

#if SAMPLE_LOAD_CODE

    internal class WebLibraryManager
    {
        public static string KNOWN_WEB_LIBRARIES_FILENAME => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForUser, @"Qiqqa.known_web_libraries"));

        private void LoadKnownWebLibraries(string filename, bool only_load_those_libraries_which_are_actually_present)
        {
            Logging.Info("+Loading known Web Libraries");
            try
            {
                if (File.Exists(filename))
                {
                    KnownWebLibrariesFile known_web_libraries_file = SerializeFile.ProtoLoad<KnownWebLibrariesFile>(filename);
                    if (null != known_web_libraries_file.web_library_details)
                    {
                        //...
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem loading the known Web Libraries from config file {0}", filename);
            }
            Logging.Info("-Loading known Web Libraries");
        }

        private void SaveKnownWebLibraries(string filename = null)
        {
            if (null == filename)
            {
                filename = KNOWN_WEB_LIBRARIES_FILENAME;
            }

            Logging.Info("+Saving known Web Libraries to {0}", filename);

            try
            {
                KnownWebLibrariesFile known_web_libraries_file = new KnownWebLibrariesFile();
                known_web_libraries_file.web_library_details = new List<WebLibraryDetail>();
                foreach (WebLibraryDetail web_library_detail in web_library_details.Values)
                {
                    // *************************************************************************************************************
                    // *** MIGRATION TO OPEN SOURCE CODE ***************************************************************************
                    // *************************************************************************************************************
                    // Don't remember the web libraries - let them be discovered by this
                    if ("Legacy" == web_library_detail.LibraryType())
                    {
                        continue;
                    }
                    // *************************************************************************************************************

                    known_web_libraries_file.web_library_details.Add(web_library_detail);
                }
                SerializeFile.ProtoSave<KnownWebLibrariesFile>(filename, known_web_libraries_file);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem saving the known web libraries to file {0}", filename);
            }

            Logging.Info("-Saving known Web Libraries to {0}", filename);
        }
    }

#endif

}
