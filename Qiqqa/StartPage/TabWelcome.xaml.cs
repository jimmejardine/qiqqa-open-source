using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Qiqqa.Main.SplashScreenStuff;
using System.Windows.Input;

namespace Qiqqa.StartPage
{
    /// <summary>
    /// Interaction logic for TabWelcome.xaml
    /// </summary>
    public partial class TabWelcome : UserControl
    {
        public delegate void GetGoingDelegate();
        public event GetGoingDelegate GetGoing;

        public TabWelcome()
        {
            InitializeComponent();
            
            ObjBackgroundImage.Source = SplashScreenWindow.GetSplashImage();
            ObjBackgroundImage.Stretch = Stretch.Fill;
            RenderOptions.SetBitmapScalingMode(ObjBackgroundImage, BitmapScalingMode.HighQuality);

            ObjFlowDocument.Background = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Black, 192));

            this.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ObjGetGoing.Text = "Let's get started! >>>";
            ObjGetGoingBorder.MouseDown += ObjGetGoing_MouseDown;
            ObjGetGoingBorder.Cursor = Cursors.Hand;
        }

        void ObjGetGoing_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_TermsAndConditions);

            ConfigurationManager.Instance.ConfigurationRecord.TermsAndConditionsAccepted = true;
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.TermsAndConditionsAccepted);

            if (null != GetGoing) GetGoing();
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Feedback);
        }
    }
}
