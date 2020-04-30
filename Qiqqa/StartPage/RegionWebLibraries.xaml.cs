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

            SafeThreadPool.QueueUserWorkItem(o =>
            {
                // This particular action would BLOCK a very long time on `WebLibraryManager.Instance` as another background
                // task is busy loading all libraries as part of the WebLibraryManager initialization.
                WebLibraryManager.Instance.WebLibrariesChanged += Instance_WebLibrariesChanged;

                WPFDoEvents.InvokeAsyncInUIThread(() => Refresh());
            });
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
            Library library = WebLibraryManager.Instance.GetLibrary(web_library_detail);
            MainWindowServiceDispatcher.Instance.OpenLibrary(library);
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
