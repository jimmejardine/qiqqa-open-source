using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Qiqqa.Common.Configuration;
using Qiqqa.Main.SplashScreenStuff;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

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

#if false
            // https://stackoverflow.com/questions/34340134/how-to-know-when-a-frameworkelement-has-been-totally-rendered
            Application.Current.Dispatcher.Invoke(new Action(() => {
                WPFDoEvents.ResetHourglassCursor();
            }), DispatcherPriority.ContextIdle, null);
#endif

            ObjBackgroundImage.Source = SplashScreenWindow.GetSplashImage();
            ObjBackgroundImage.Stretch = Stretch.Fill;
            RenderOptions.SetBitmapScalingMode(ObjBackgroundImage, BitmapScalingMode.HighQuality);

            ObjFlowDocument.Background = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Black, 192));

            DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            ObjGetGoing.Text = "Let's get started! >>>";
            ObjGetGoingBorder.MouseDown += ObjGetGoing_MouseDown;
            ObjGetGoingBorder.Cursor = Cursors.Hand;
        }

        private void ObjGetGoing_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.App_TermsAndConditions);

            ConfigurationManager.Instance.ConfigurationRecord.TermsAndConditionsAccepted = true;
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.TermsAndConditionsAccepted);

            GetGoing?.Invoke();
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Feedback);
        }
    }
}
