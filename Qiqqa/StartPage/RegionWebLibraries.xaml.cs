using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Synchronisation.BusinessLogic;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.StartPage
{
    /// <summary>
    /// Interaction logic for RegionWebLibraries.xaml
    /// </summary>
    public partial class RegionWebLibraries : UserControl
    {
        public RegionWebLibraries()
        {
            InitializeComponent();

            ObjWebLibraryListControl.OnWebLibrarySelected += ObjWebLibraryListControl_OnWebLibrarySelected;

            // NOTE: in older Qiqqa code `WebLibraryManager.Instance` was taking *ages*. This has changed since Nov/2020
            // as the constructor inside `WebLibraryManager.Instance` getter will now execute swiftly, only
            // pushing the larger work onto the ThreadPool. Hence this statement should execute very swiftly!
            //
            // This particular action would previously BLOCK a very long time on `WebLibraryManager.Instance` as another background
            // task is busy loading all libraries as part of the WebLibraryManager initialization.
            WebLibraryManager.Instance.WebLibrariesChanged += Instance_WebLibrariesChanged;
        }

        private void Instance_WebLibrariesChanged()
        {
            WPFDoEvents.InvokeInUIThread(() =>
            {
                Refresh();
            },
            priority: DispatcherPriority.Background);
        }

        private void ObjWebLibraryListControl_OnWebLibrarySelected(WebLibraryDetail web_library_detail)
        {
            MainWindowServiceDispatcher.Instance.OpenLibrary(web_library_detail);
        }

        private void Refresh()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            ObjWebLibraryListControl.Refresh();
        }

        public void DoSync()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Sync_Stats);
            LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(true, true, true, false));
        }
    }
}
