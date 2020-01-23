using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using icons;
using Qiqqa.Documents.Common;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for PDFAnnotationEditorControl.xaml
    /// </summary>
    public partial class PDFAnnotationEditorControl : UserControl
    {
        private PDFAnnotation pdf_annotation;

        public PDFAnnotationEditorControl()
        {
            Theme.Initialize();

            InitializeComponent();

            Background = ThemeColours.Background_Brush_Blue_LightToDark;

            ButtonColor1.Background = Brushes.LightPink;
            ButtonColor2.Background = Brushes.LightSalmon;
            ButtonColor3.Background = Brushes.LightGreen;
            ButtonColor4.Background = Brushes.SkyBlue;
            ButtonColor5.Background = Brushes.Yellow;

            ButtonColor1.Click += ButtonColor_Click;
            ButtonColor2.Click += ButtonColor_Click;
            ButtonColor3.Click += ButtonColor_Click;
            ButtonColor4.Click += ButtonColor_Click;
            ButtonColor5.Click += ButtonColor_Click;

            ButtonDeleteAnnotation.Icon = Icons.GetAppIcon(Icons.Delete);
            ButtonDeleteAnnotation.CaptionDock = Dock.Right;
            ButtonDeleteAnnotation.Caption = "Delete annotation";
            ButtonDeleteAnnotation.Click += ButtonDeleteAnnotation_Click;

            ComboBoxRating.ItemsSource = Choices.Ratings;

            ObjColorPicker.SelectedColorChanged += ObjColorPicker_SelectedColorChanged;

            ObjTagEditorControl.TagFeature_Add = Features.Document_AddAnnotationTag;
            ObjTagEditorControl.TagFeature_Remove = Features.Document_RemoveAnnotationTag;
        }

        private void ButtonDeleteAnnotation_Click(object sender, RoutedEventArgs e)
        {
            pdf_annotation.Deleted = true;
            pdf_annotation.Bindable.NotifyPropertyChanged(nameof(pdf_annotation.Deleted));
        }

        private void ButtonColor_Click(object sender, RoutedEventArgs e)
        {
            AugmentedButton button = (AugmentedButton)sender;
            SolidColorBrush brush = (SolidColorBrush)button.Background;
            ObjColorPicker.SelectedColor = brush.Color;
        }

        private static Color last_annotation_color = Colors.SkyBlue;
        public static Color LastAnnotationColor => last_annotation_color;

        private void ObjColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            last_annotation_color = ObjColorPicker.SelectedColor;

            if (null != pdf_annotation)
            {
                pdf_annotation.Color = ObjColorPicker.SelectedColor;
                pdf_annotation.Bindable.NotifyPropertyChanged(nameof(pdf_annotation.Color));
            }
        }

        public void SetAnnotation(PDFAnnotation value)
        {
            pdf_annotation = value;
            DataContext = pdf_annotation.Bindable;

            if (null != pdf_annotation)
            {
                ObjColorPicker.SelectedColor = pdf_annotation.Color;
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
