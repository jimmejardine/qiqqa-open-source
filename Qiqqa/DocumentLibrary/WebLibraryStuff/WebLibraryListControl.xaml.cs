using System.Collections.Generic;
using System.Windows.Controls;
using Qiqqa.Common.Configuration;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    /// <summary>
    /// Interaction logic for WebLibraryListControl.xaml
    /// </summary>
    public partial class WebLibraryListControl : UserControl
    {
        public delegate void WebLibrarySelectedDelegate(WebLibraryDetail web_library_detail);
        public event WebLibrarySelectedDelegate OnWebLibrarySelected;

        private bool concise_view = false;

        public WebLibraryListControl()
        {
            InitializeComponent();
        }

        public bool ConciseView
        {
            get => concise_view;
            set => concise_view = value;
        }

        internal void Refresh()
        {
            RepopulateLibraries();
        }

        private void RepopulateLibraries()
        {
            const int MAX_EXPANDED_CHILD_COUNT = 1;

            // Sort the children by last accessed order
            List<WebLibraryDetail> children = new List<WebLibraryDetail>();
            children.AddRange(WebLibraryManager.Instance.WebLibraryDetails_All_IncludingDeleted);
            WebLibraryManager.Instance.SortWebLibraryDetailsByLastAccessed(children);

            // Add the children to our control
            PanelWebLibraries.Children.Clear();
            for (int i = 0; i < children.Count; ++i)
            {
                WebLibraryDetail web_library_detail = children[i];

                // Only allow the first few to be mega expanded
                bool open_cover_flow = !concise_view && i < MAX_EXPANDED_CHILD_COUNT && !web_library_detail.Deleted;

                WebLibraryDetailControl web_library_detail_control = new WebLibraryDetailControl(concise_view, open_cover_flow, OnChildWebLibrarySelected);
                web_library_detail_control.DataContext = web_library_detail;

                if (web_library_detail.Deleted)
                {
                    web_library_detail_control.Opacity = 0.5;
                }

                PanelWebLibraries.Children.Add(web_library_detail_control);
                PanelWebLibraries.Children.Add(new AugmentedSpacer());
            }
        }

        private void OnChildWebLibrarySelected(WebLibraryDetail web_library_detail)
        {
            // Remember it by tacking it onto the front and removing it from the middle
            ConfigurationManager.Instance.ConfigurationRecord.GUI_LastSelectedLibraryId = web_library_detail.Id + ConfigurationManager.Instance.ConfigurationRecord.GUI_LastSelectedLibraryId.Replace(web_library_detail.Id, "");
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.GUI_LastSelectedLibraryId);

            // Callback
            OnWebLibrarySelected?.Invoke(web_library_detail);
        }
    }
}
