using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.Expedition
{
    /// <summary>
    /// Interaction logic for ExpeditionInstructionsControl.xaml
    /// </summary>
    public partial class ExpeditionInstructionsControl : UserControl
    {
        public ExpeditionInstructionsControl()
        {
            InitializeComponent();

            ImageExpedition.Source = Icons.GetAppIcon(Icons.ModuleExpedition);

            ReflectLibrary(null);
        }

        public void ReflectLibrary(WebLibraryDetail web_library_detail)
        {
            // Reset
            RegionNoLibrary.Visibility = Visibility.Collapsed;
            RegionNoExpedition.Visibility = Visibility.Collapsed;
            RegionStaleExpedition.Visibility = Visibility.Collapsed;
            RegionExpeditionTooSmall.Visibility = Visibility.Collapsed;

            // Reflect
            if (null == web_library_detail)
            {
                RegionNoLibrary.Visibility = Visibility.Visible;
            }
            else
            {
                if (null == web_library_detail.library.ExpeditionManager.ExpeditionDataSource)
                {
                    RegionNoExpedition.Visibility = Visibility.Visible;
                }
                else
                {
                    // Is this expedition getting old?
                    if (web_library_detail.library.ExpeditionManager.IsStale)
                    {
                        RegionStaleExpedition.Visibility = Visibility.Visible;
                    }

                    // Is this expedition too small?
                    if (web_library_detail.library.ExpeditionManager.ExpeditionDataSource.docs.Count < 20 || web_library_detail.library.ExpeditionManager.ExpeditionDataSource.words.Count < 5)
                    {
                        RegionExpeditionTooSmall.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
