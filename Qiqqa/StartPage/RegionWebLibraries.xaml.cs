using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Synchronisation.BusinessLogic;
using Qiqqa.UtilisationTracking;

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

            WebLibraryManager.Instance.WebLibrariesChanged += Instance_WebLibrariesChanged;
            
            Refresh();
        }

        void Instance_WebLibrariesChanged()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Instance_WebLibrariesChanged_THREAD();
            }
            ), DispatcherPriority.Background);
        }

        private void Instance_WebLibrariesChanged_THREAD()
        {
            Refresh();
        }
        
        void ObjWebLibraryListControl_OnWebLibrarySelected(WebLibraryDetail web_library_detail)
        {
            Library library = WebLibraryManager.Instance.GetLibrary(web_library_detail);
            MainWindowServiceDispatcher.Instance.OpenLibrary(library);
        }

        void Refresh()
        {
            ObjWebLibraryListControl.Refresh();

            if (WebLibraryManager.Instance.HaveOnlyOneWebLibrary())
            {
                RegionMoreWebLibraries.Visibility = Visibility.Visible;
            }
            else
            {
                RegionMoreWebLibraries.Visibility = Visibility.Collapsed;
            }
        }

        public void DoSync()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Sync_Stats);
            LibrarySyncManager.Instance.RequestSync(new LibrarySyncManager.SyncRequest(true, true, true, false));
        }
    }
}
