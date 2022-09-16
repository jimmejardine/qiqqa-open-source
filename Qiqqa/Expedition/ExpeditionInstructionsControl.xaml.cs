using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.Misc;

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
                ExpeditionDataSource eds = web_library_detail.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;

                if (null == eds)
                {
                    RegionNoExpedition.Visibility = Visibility.Visible;
                }
                else
                {
                    ASSERT.Test(eds.words != null);
                    ASSERT.Test(eds.docs != null);

                    // Is this expedition getting old?
                    if (web_library_detail.Xlibrary.ExpeditionManager.IsStale)
                    {
                        RegionStaleExpedition.Visibility = Visibility.Visible;
                    }

                    // Is this expedition too small?
                    if (eds.docs.Count < 20 || eds.words.Count < 5)
                    {
                        RegionExpeditionTooSmall.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
