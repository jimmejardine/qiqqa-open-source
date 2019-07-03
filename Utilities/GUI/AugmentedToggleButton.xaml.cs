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

            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;

            RenderOptions.SetBitmapScalingMode(this.ImageIcon, BitmapScalingMode.HighQuality);

            CaptionDock = Dock.Bottom;

            //this.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            //this.BorderBrush = ThemeColours.Background_Brush_Blue_LightToDark;
            this.Style = FindResource(ToolBar.ToggleButtonStyleKey) as Style;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
        }

        public string Caption
        {
            set
            {
                if (null != value)
                {
                    ObjSpacer.Visibility = Visibility.Visible;
                    TextCaption.Visibility = Visibility.Visible;
                    TextCaption.Text = value;
                }
                else
                {
                    ObjSpacer.Visibility = Visibility.Collapsed;
                    TextCaption.Visibility = Visibility.Collapsed;
                    TextCaption.Text = value;
                }
            }
        }

        public ImageSource Icon
        {
            set
            {
                ImageIcon.Source = value;
            }
        }

        public Dock CaptionDock
        {
            set
            {
                switch (value)
                {
                    case Dock.Top:
                        TextCaption.TextAlignment = TextAlignment.Center;
                        DockPanel.SetDock(ImageIcon, Dock.Bottom);
                        DockPanel.SetDock(ObjSpacer, Dock.Bottom);
                        DockPanel.SetDock(TextCaption, Dock.Bottom);                        
                        break;
                    case Dock.Bottom:
                        TextCaption.TextAlignment = TextAlignment.Center;
                        DockPanel.SetDock(ImageIcon, Dock.Top);
                        DockPanel.SetDock(ObjSpacer, Dock.Top);
                        DockPanel.SetDock(TextCaption, Dock.Top);
                        break;
                    case Dock.Left:
                        TextCaption.TextAlignment = TextAlignment.Right;
                        DockPanel.SetDock(ImageIcon, Dock.Right);
                        DockPanel.SetDock(ObjSpacer, Dock.Right);
                        DockPanel.SetDock(TextCaption, Dock.Right);
                        break;
                    case Dock.Right:
                        TextCaption.TextAlignment = TextAlignment.Left;
                        DockPanel.SetDock(ObjSpacer, Dock.Left);
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
