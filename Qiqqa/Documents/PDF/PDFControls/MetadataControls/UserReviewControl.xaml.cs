using System.Windows;
using System.Windows.Controls;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.Common;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for UserReviewControl.xaml
    /// </summary>
    public partial class UserReviewControl : UserControl
    {
        public UserReviewControl()
        {
            InitializeComponent();

            ComboBoxReadingStage.ItemsSource = ReadingStageManager.Instance.ReadingStages;
            ComboBoxRating.ItemsSource = Choices.Ratings;
        }

        public void SetDatesVisible(bool value)
        {
            ObjDatesPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
