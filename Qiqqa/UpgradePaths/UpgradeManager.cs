using Qiqqa.Main.SplashScreenStuff;
using Qiqqa.UpgradePaths.V003To004;
using Qiqqa.UpgradePaths.V037To038;
using Qiqqa.UpgradePaths.V043To044;
using Utilities;

namespace Qiqqa.UpgradePaths
{
    public static class UpgradeManager
    {
        public static void RunUpgrades(SplashScreenWindow splashscreen_window)
        {
            Logging.Info("+UpgradeManager is running upgrades");

            RenewMessage(splashscreen_window);
            Upgrade.RunUpgrade();

            RenewMessage(splashscreen_window);
            V012To013.Upgrade.RunUpgrade();

            RenewMessage(splashscreen_window);
            SQLiteUpgrade.RunUpgrade(splashscreen_window);

            RenewMessage(splashscreen_window);
            MoveOCRDirs.RunUpgrade(splashscreen_window);


            Logging.Info("-UpgradeManager is running upgrades");
        }

        private static void RenewMessage(SplashScreenWindow splashscreen_window)
        {
            splashscreen_window.UpdateMessage("Running upgrades.  Please be patient.");
        }
    }
}
