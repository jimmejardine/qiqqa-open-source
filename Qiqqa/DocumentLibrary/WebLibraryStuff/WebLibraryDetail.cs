using System;
using Alphaleonis.Win32.Filesystem;
using ProtoBuf;
using Qiqqa.Common.Configuration;
using Utilities.Strings;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    [Serializable]
    [ProtoContract]
    public class WebLibraryDetail
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string Title { get; set; }
        [ProtoMember(3)]
        public string Description { get; set; }

        [ProtoMember(4)]
        public bool Deleted { get; set; }
        [ProtoMember(5)]
        public DateTime? LastSynced { get; set; }

        [ProtoMember(6)]
        public string FolderToWatch { get; set; }
        [ProtoMember(7)]
        public bool XIsLocalGuestLibrary { get; set; }

        /* Only valid for web libraries */
        [ProtoMember(8)]
        public string XShortWebId { get; set; }
        [ProtoMember(9)]
        public bool XIsAdministrator { get; set; }
        [ProtoMember(14)]
        public bool XIsReadOnly {
            get;
            set; }
        // Bundles can never sync
        public bool IsReadOnlyLibrary => IsBundleLibrary;

        /* Only valid for intranet libraries */
        [ProtoMember(13)]
        public string IntranetPath { get; set; }
        public bool IsIntranetLibrary => !String.IsNullOrEmpty(IntranetPath);

        /* Only valid for Bundle Libraries */
        [ProtoMember(15)]
        public string BundleManifestJSON { get; set; }
        public bool IsBundleLibrary => !String.IsNullOrEmpty(BundleManifestJSON);

        [ProtoMember(16)]
        public DateTime? LastBundleManifestDownloadTimestampUTC { get; set; }
        [ProtoMember(17)]
        public string LastBundleManifestIgnoreVersion { get; set; }

        [ProtoMember(10)]
        public bool IsPurged { get; set; }

        [ProtoMember(11)]
        public DateTime XLastServerSyncNotificationDate { get; set; }
        [ProtoMember(12)]
        public bool AutoSync {
            get;
            set;
        }

        public Library Xlibrary {
            get;
            set;
        }

        public override string ToString()
        {
            return String.Format("Library {0}: {1}", Id, Title);
        }

        public string DescriptiveTitle
        {
            get
            {
                string s = StringTools.TrimToLengthWithEllipsis(Title);
                if (!String.IsNullOrWhiteSpace(s)) return s;
                s = StringTools.TrimToLengthWithEllipsis(Description);
                if (!String.IsNullOrWhiteSpace(s)) return s;
                return Id;
            }
        }

        public string LibraryType()
        {
            if (IsIntranetLibrary) return "Intranet";
            if (IsBundleLibrary) return "Bundle";
            return "Local";
        }

        // -----------------------------------------------------------------------

#region --- File locations ------------------------------------------------------------------------------------

        public static string GetLibraryBasePathForId(string id)
        {
            return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, id));
        }

        public string LIBRARY_BASE_PATH => GetLibraryBasePathForId(Id);

        public string LIBRARY_DOCUMENTS_BASE_PATH
        {
            get
            {
                string folder_override = ConfigurationManager.Instance.ConfigurationRecord.System_OverrideDirectoryForPDFs;
                if (!String.IsNullOrEmpty(folder_override))
                {
                    return Path.GetFullPath(folder_override + @"\");
                }
                else
                {
                    return Path.GetFullPath(Path.Combine(LIBRARY_BASE_PATH, @"documents"));
                }
            }
        }

        public string LIBRARY_INDEX_BASE_PATH => Path.GetFullPath(Path.Combine(LIBRARY_BASE_PATH, @"index"));

        public string FILENAME_DOCUMENT_PROGRESS_LIST => Path.GetFullPath(Path.Combine(LIBRARY_INDEX_BASE_PATH, @"DocumentProgressList.dat"));


        #endregion

    }
}
