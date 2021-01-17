using System;
using System.Collections.Generic;
using System.Linq;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.Misc;

namespace Qiqqa.Synchronisation.MetadataSync
{
    internal class SynchronisationExecutor
    {
        internal static void Sync(WebLibraryDetail web_library_detail, bool is_readonly, Dictionary<string, string> historical_sync_file, SynchronisationAction synchronisation_action)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), "Performing metadata transfers");

            // For read only libraries, we need to overwrite any files that have been changed locally since the last sync
            if (is_readonly)
            {
                if (0 < synchronisation_action.states_to_upload.Count)
                {
                    Logging.Info("We are discarding {0} upload items because library is read-only.", synchronisation_action.states_to_upload.Count);
                    synchronisation_action.states_to_download.AddRange(synchronisation_action.states_to_upload);
                    synchronisation_action.states_to_upload.Clear();
                }

                if (0 < synchronisation_action.states_to_merge.Count)
                {
                    Logging.Info("We are discarding {0} merge items because library is read-only.", synchronisation_action.states_to_merge.Count);
                    synchronisation_action.states_to_download.AddRange(synchronisation_action.states_to_merge);
                    synchronisation_action.states_to_merge.Clear();
                }
            }

            DoMerges(web_library_detail, historical_sync_file, synchronisation_action);
            DoUploads(web_library_detail, historical_sync_file, synchronisation_action);
            int download_count = DoDownloads(web_library_detail, historical_sync_file, synchronisation_action);

            if (0 < download_count)
            {
                DoNotifyLibraryOfChanges(web_library_detail, historical_sync_file, synchronisation_action);
            }

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), "Finished metadata transfers");
        }

        private static void DoMerges(WebLibraryDetail web_library_detail, Dictionary<string, string> historical_sync_file, SynchronisationAction synchronisation_action)
        {
            // For now we are going to treat all conflicted files as downloads (i.e. server wins)
            synchronisation_action.states_to_download.AddRange(synchronisation_action.states_to_merge);
            synchronisation_action.states_to_merge.Clear();
        }

        private static void DoUploads(WebLibraryDetail web_library_detail, Dictionary<string, string> historical_sync_file, SynchronisationAction synchronisation_action)
        {
            int upload_count = 0;

            StatusManager.Instance.ClearCancelled(StatusCodes.SYNC_META(web_library_detail));
            foreach (SynchronisationState ss in synchronisation_action.states_to_upload)
            {
                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), String.Format("Uploading metadata to your Web/Intranet Library ({0} to go)", synchronisation_action.states_to_upload.Count - upload_count), upload_count, synchronisation_action.states_to_upload.Count, true);
                ++upload_count;

                // Has the user canceled?
                if (StatusManager.Instance.IsCancelled(StatusCodes.SYNC_META(web_library_detail)))
                {
                    Logging.Info("User has canceled their metadata upload");
                    break;
                }

                // Upload the file
                {
                    Logging.Info("+Uploading {0}", ss.filename);

                    // TODO: Replace this with a pretty interface class ------------------------------------------------
                    if (web_library_detail.IsIntranetLibrary)
                    {
                        SynchronisationExecutor_Intranet.DoUpload(web_library_detail, ss);
                    }
                    else
                    {
                        throw new Exception(String.Format("Did not understand how to upload for library {0}", web_library_detail.Title));
                    }
                    // -----------------------------------------------------------------------------------------------------

                    Logging.Info("-Uploading {0}");

                    historical_sync_file[ss.filename] = ss.library_item.md5;
                }
            }

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), String.Format("Uploaded {0} metadata to your Web/Intranet Library", upload_count));
        }


        private static int DoDownloads(WebLibraryDetail web_library_detail, Dictionary<string, string> historical_sync_file, SynchronisationAction synchronisation_action)
        {
            int download_count = 0;

            StatusManager.Instance.ClearCancelled(StatusCodes.SYNC_META(web_library_detail));
            foreach (SynchronisationState ss in synchronisation_action.states_to_download)
            {
                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), String.Format("Downloading metadata from your Web/Intranet Library ({0} to go)", synchronisation_action.states_to_download.Count - download_count), download_count, synchronisation_action.states_to_download.Count, true);
                ++download_count;

                // Has the user canceled?
                if (StatusManager.Instance.IsCancelled(StatusCodes.SYNC_META(web_library_detail)))
                {
                    Logging.Info("User has canceled their metadata download");
                    break;
                }

                try
                {
                    Logging.Info("+Downloading {0}", ss.filename);

                    StoredUserFile stored_user_file = null;

                    // TODO: Replace this with a pretty interface class ------------------------------------------------
                    if (web_library_detail.IsIntranetLibrary)
                    {
                        stored_user_file = SynchronisationExecutor_Intranet.DoDownload(web_library_detail, ss);
                    }
                    else
                    {
                        throw new Exception(String.Format("Did not understand how to download for library '{0}'", web_library_detail.Title));
                    }
                    // -----------------------------------------------------------------------------------------------------

                    Logging.Info("-Downloading {0}", ss.filename);

                    {
                        // Check that the MD5s match, or we have had some issue in the download
                        Logging.Info("Checking content");
                        string md5_metadata = StreamMD5.FromBytes(stored_user_file.Content);
                        string header_etag = stored_user_file.Md5;
                        string header_etag_nik = header_etag;
                        if (null != header_etag && !String.IsNullOrEmpty(header_etag) && 0 != String.Compare(md5_metadata, header_etag, true) && 0 != String.Compare(md5_metadata, header_etag_nik, true))
                        {
                            throw new Exception(String.Format("Local and remote MD5s do not match. local={0} remote={1} remote_nik={2}", md5_metadata, header_etag, header_etag_nik));
                        }

                        Logging.Info("Copying content");
                        web_library_detail.Xlibrary.LibraryDB.PutBlob(ss.fingerprint, ss.extension, stored_user_file.Content);

                        // Remember this MD5 for our sync clash detection
                        Logging.Info("Remembering md5");
                        historical_sync_file[ss.filename] = md5_metadata;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem downloading one of your sync files: file '{0}' for library '{1}'", ss.filename, web_library_detail.Title);
                }
            }

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), String.Format("Downloaded {0} metadata from your Web/Intranet Library", download_count));

            return download_count;
        }

        private static void DoNotifyLibraryOfChanges(WebLibraryDetail web_library_detail, Dictionary<string, string> historical_sync_file, SynchronisationAction synchronisation_action)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), "Notifying library of changes");

            HashSet<string> fingerprints_that_have_changed = new HashSet<string>();

            foreach (SynchronisationState ss in synchronisation_action.states_to_download)
            {
                fingerprints_that_have_changed.Add(ss.fingerprint);
            }

            foreach (string fingerprint in fingerprints_that_have_changed)
            {
                web_library_detail.Xlibrary.NotifyLibraryThatDocumentHasChangedExternally(fingerprint, web_library_detail);
            }

            // Update grid
            web_library_detail.Xlibrary.NotifyLibraryThatDocumentListHasChangedExternally();

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(web_library_detail), "Notified library of changes");
        }
    }
}
