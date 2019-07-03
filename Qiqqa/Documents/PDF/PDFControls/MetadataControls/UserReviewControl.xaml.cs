using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.Common;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for UserReviewControl.xaml
    /// </summary>
    [Obfuscation(Feature = "properties renaming")]
    public partial class UserReviewControl : UserControl
    {
        public UserReviewControl()
        {
            InitializeComponent();

            ComboBoxReadingStage.ItemsSource = ReadingStageManager.Instance.ReadingStages;
            ComboBoxRating.ItemsSource = Choices.Ratings;
        }

        [Obfuscation(Feature = "properties renaming")]
        public bool DatesVisible
        {
            set
            {
                ObjDatesPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
