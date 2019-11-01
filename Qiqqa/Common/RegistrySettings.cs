using Utilities.Misc;

namespace Qiqqa.Common
{
    public class RegistrySettings : QuantisleUserRegistry
    {
        public static RegistrySettings Instance = new RegistrySettings();

        public static readonly string DebugConsole = "DebugConsole";

        public static readonly string FirstInstallNotification = "FirstInstallNotification";

        public static readonly string DisableGlobalKeyHook = "DisableGlobalKeyHook";

        public static readonly string AllowMultipleQiqqaInstances = "AllowMultipleQiqqaInstances";

        public static readonly string SkipAdverts = "SkipAdverts";

        public static readonly string SampleBrainstorm = "SampleBrainstorm";

        public static readonly string UrlWebRoot = "UrlWebRoot";  //e.g. test.qiqqa.com

        public static readonly string SuppressDaemon = "SuppressDaemon";

        public static readonly string LastLoginUsername = "LastLoginUsername";
        public static readonly string LastLoginGuid = "LastLoginGuid";

        public static readonly string BibTeXSearchSearchUrl = "BibTeXSearchSearchUrl";

        public static readonly string PremiumForceYes = "PremiumForceYes";
        public static readonly string PremiumForceNo = "PremiumForceNo";
        public static readonly string PremiumPlusForceYes = "PremiumPlusForceYes";
        public static readonly string PremiumPlusForceNo = "PremiumPlusForceNo";
        public static readonly string ForceGuest = "ForceGuest";

        public static readonly string SnapToPixels = "SnapToPixels";
        public static readonly string StartNotMaximized = "StartNotMaximized";

        public static readonly string BaseDataDirectory = "BaseDataDirectory";

        public static readonly string WriteJson = "WriteJson";

        private RegistrySettings() : base("Qiqqa")
        {
        }
    }
}
