using System;
using ProtoBuf;
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
        public bool IsLocalGuestLibrary { get; set; }

        /* Only valid for web libraries */
        [ProtoMember(8)]
        public string ShortWebId { get; set; }
        [ProtoMember(9)]
        public bool IsAdministrator { get; set; }
        [ProtoMember(14)]
        public bool IsReadOnly { get; set; }

        public bool IsWebLibrary => !String.IsNullOrEmpty(ShortWebId);

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
        public DateTime LastServerSyncNotificationDate { get; set; }
        [ProtoMember(12)]
        public bool AutoSync { get; set; }

        [NonSerialized]
        public Library library;

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
            if (IsLocalGuestLibrary) return "Guest";
            if (IsIntranetLibrary) return "Intranet";
            if (IsBundleLibrary) return "Bundle";
            if (IsWebLibrary) return "Web";
            return "Legacy";
        }

#if false
        // additional hacky API to assist the codebase while we weren't using TypedWeakReference object(s) for this:

        public WebLibraryDetail CloneSansLibraryReference()
        {
            // only clone the important fields:
            WebLibraryDetail rv = new WebLibraryDetail();
            rv.Id = this.Id;
            rv.Title = this.Title;
            rv.Description = this.Description;
            rv.Deleted = this.Deleted;
            //LastSynced
            //FolderToWatch
            rv.IsLocalGuestLibrary = this.IsLocalGuestLibrary;
            //ShortWebId
            //IsAdministrator
            //IsReadOnly
            //IntranetPath
            //BundleManifestJSON
            //LastBundleManifestDownloadTimestampUTC
            //LastBundleManifestIgnoreVersion
            rv.IsPurged = this.IsPurged;
            //LastServerSyncNotificationDate
            rv.AutoSync = false;

            return rv;
        }
#endif
    }
}
