namespace Qiqqa.Synchronisation.BusinessLogic
{
    public static class SyncConstants
    {
        public static readonly long PremiumLibrarySizeMaximum_Megs = 10 * 1024;
        public static readonly long PremiumPlusLibrarySizeMaximum_Megs = 50 * 1024;

        public static long PremiumLibrarySizeMaximum_Gb
        {
            get
            {
                return PremiumLibrarySizeMaximum_Megs / 1024;
            }
        }

        public static long PremiumPlusLibrarySizeMaximum_Gb
        {
            get
            {
                return PremiumPlusLibrarySizeMaximum_Megs / 1024;
            }
        }
    }
}
