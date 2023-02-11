using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Utilities.GUI
{
    /// <summary>
    /// Interaction logic for AugmentedToggleButton.xaml
    /// </summary>
    public partial class AugmentedToggleButton : ToggleButton
    {
        public AugmentedToggleButton()
        {
            InitializeComponent();

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            //RenderOptions.SetBitmapScalingMode(ImageIcon, BitmapScalingMode.HighQuality);

            CaptionDock = Dock.Bottom;

            //this.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            //this.BorderBrush = ThemeColours.Background_Brush_Blue_LightToDark;
            Style = FindResource(ToolBar.ToggleButtonStyleKey) as Style;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        public string Caption
        {
            get => TextCaption.Text;
            set
            {
                if (null != value)
                {
                    TextCaption.Visibility = Visibility.Visible;
                    TextCaption.Text = value;
                }
                else
                {
                    TextCaption.Visibility = Visibility.Collapsed;
                    TextCaption.Text = value;
                }
            }
        }

        public ImageSource Icon
        {
            get => ImageIcon.Source;
            set => ImageIcon.Source = value;
        }

        public Dock CaptionDock
        {
            get
            {
                switch (DockPanel.GetDock(ImageIcon))
                {
                    default:
                    case Dock.Bottom:
                        return Dock.Top;
                    case Dock.Top:
                        return Dock.Bottom;
                    case Dock.Right:
                        return Dock.Left;
                    case Dock.Left:
                        return Dock.Right;
                }
            }
            set
            {
                switch (value)
                {
                    case Dock.Top:
                        TextCaption.TextAlignment = TextAlignment.Center;
                        DockPanel.SetDock(ImageIcon, Dock.Bottom);
                        DockPanel.SetDock(TextCaption, Dock.Bottom);
                        break;
                    case Dock.Bottom:
                        TextCaption.TextAlignment = TextAlignment.Center;
                        DockPanel.SetDock(ImageIcon, Dock.Top);
                        DockPanel.SetDock(TextCaption, Dock.Top);
                        break;
                    case Dock.Left:
                        TextCaption.TextAlignment = TextAlignment.Right;
                        DockPanel.SetDock(ImageIcon, Dock.Right);
                        DockPanel.SetDock(TextCaption, Dock.Right);
                        break;
                    case Dock.Right:
                        TextCaption.TextAlignment = TextAlignment.Left;
                        DockPanel.SetDock(ImageIcon, Dock.Left);
                        DockPanel.SetDock(TextCaption, Dock.Left);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
