using Qiqqa.Main.SplashScreenStuff;
using Qiqqa.UpgradePaths.V003To004;
using Qiqqa.UpgradePaths.V037To038;
using Qiqqa.UpgradePaths.V043To044;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.UpgradePaths
{
    public static class UpgradeManager
    {
        public static void RunUpgrades()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            Logging.Info("+UpgradeManager is running upgrades");

            RenewMessage();
            Upgrade.RunUpgrade();

            RenewMessage();
            V012To013.Upgrade.RunUpgrade();

            RenewMessage();
            SQLiteUpgrade.RunUpgrade();

            RenewMessage();
            MoveOCRDirs.RunUpgrade();

            StatusManager.Instance.ClearStatus("DBUpgrade");

            Logging.Info("-UpgradeManager is running upgrades");
        }

        private static void RenewMessage()
        {
            StatusManager.Instance.UpdateStatus("DBUpgrade", "Running upgrades.  Please be patient.");
        }
    }
}
