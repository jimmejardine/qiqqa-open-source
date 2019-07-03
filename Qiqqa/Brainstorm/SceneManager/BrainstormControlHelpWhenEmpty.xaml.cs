using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;

namespace Qiqqa.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for BrainstormControlHelpWhenEmpty.xaml
    /// </summary>
    public partial class BrainstormControlHelpWhenEmpty : UserControl
    {
        public BrainstormControlHelpWhenEmpty()
        {
            InitializeComponent();

            lnkMoreTips.Click += lnkMoreTips_Click;
            lnkSampleBrainstorm.Click += lnkSampleBrainstorm_Click;
        }

        void lnkSampleBrainstorm_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenSampleBrainstorm();
        }

        void lnkMoreTips_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenHelp();
        }       
    }
}
