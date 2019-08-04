using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using icons;

namespace Utilities.GUI
{
    /// <summary>
    /// Interaction logic for AugmentedButton.xaml
    /// </summary>
    [ContentProperty("Caption")]
    public partial class AugmentedButton : Button
    {
        public AugmentedButton()
        { 
            InitializeComponent();
            
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            
            RenderOptions.SetBitmapScalingMode(this.ImageIcon, BitmapScalingMode.HighQuality);

            this.IsEnabledChanged += AugmentedButton_IsEnabledChanged;

            this.ImagePopupIndicator.Source = Icons.GetAppIcon(Icons.AugmentedButtonDown);
            this.PanelPopupPanel.Visibility = Visibility.Collapsed;

            // Initialise
            this.Icon = null;
            this.CaptionDock = Dock.Bottom;

            ApplyStyling(this);

            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;

            ObjPanelCentered.Visibility = System.Windows.Visibility.Collapsed;
        }

        public bool CenteredMode
        {
            set
            {
                if (value)
                {
                    ObjPanelDetailed.Visibility = System.Windows.Visibility.Collapsed;
                    ObjPanelCentered.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    ObjPanelDetailed.Visibility = System.Windows.Visibility.Visible;
                    ObjPanelCentered.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        public static void ApplyStyling(Button button)
        {
            button.Background = ThemeColours.Background_Brush_Blue_VeryDark;
            button.BorderBrush = ThemeColours.Background_Brush_Blue_Dark;
            button.Style = null;
        }

        void AugmentedButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                ImageIcon.Opacity = 1;
            }
            else
            {
                ImageIcon.Opacity = 0.5;
            }
        }

        public string Caption
        {
            set
            {
                if (null != value)
                {
                    TextCaption.Visibility = Visibility.Visible;
                    TextCaption.Text = value;
                    TextCaptionCentered.Text = value;

                    RecheckSpacerVisibility();
                }
                else
                {
                    TextCaption.Visibility = Visibility.Collapsed;
                    TextCaption.Text = value;
                    TextCaptionCentered.Text = value;

                    RecheckSpacerVisibility();
                }
            }
        }

        private void RecheckSpacerVisibility()
        {
            if (Visibility.Collapsed == TextCaption.Visibility || Visibility.Collapsed == ImageIcon.Visibility)
            {
                ObjSpacer.Visibility = Visibility.Collapsed;
            }
            else
            {
                ObjSpacer.Visibility = Visibility.Visible;
            }

            if (Visibility.Visible == TextCaption.Visibility)
            {
                MinWidth = 80;
            }
            else
            {
                MinWidth = 0;
            }            
        }

        public Visibility IconVisibility
        {
            set
            {
                ImageIcon.Visibility = value;

                RecheckSpacerVisibility();
            }
        }
        
        public ImageSource Icon
        {
            set
            {   
                ImageIcon.Source = value;
                if (null == value)
                {
                    ImageIcon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ImageIcon.Visibility = Visibility.Visible;
                }

                RecheckSpacerVisibility();
            }
        }

        public double IconWidth
        {
            set
            {
                ImageIcon.Width = value;
            }

            get
            {
                return ImageIcon.Width;
            }
        }

        public double IconHeight
        {
            set
            {
                ImageIcon.Height = value;
            }

            get
            {
                return ImageIcon.Height;
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
                        DockPanel.SetDock(ImageIcon, Dock.Left);
                        DockPanel.SetDock(ObjSpacer, Dock.Left);
                        DockPanel.SetDock(TextCaption, Dock.Left);
                        break;
                    default:
                        break;
                }
            }
        }

        Popup attached_popup = null;
        public void AttachPopup(Popup popup)
        {
            this.PanelPopupPanel.Visibility = Visibility.Visible;
            this.attached_popup = popup;
            this.Click += AugmentedButtonPopup_Click;
        }

        void AugmentedButtonPopup_Click(object sender, RoutedEventArgs e)
        {
            if (null != attached_popup)
            {
                attached_popup.PlacementTarget = this;
                attached_popup.MinWidth = 300;
                attached_popup.StaysOpen = false;
                attached_popup.IsOpen = true;
                e.Handled = true;
            }
        }
    }
}
