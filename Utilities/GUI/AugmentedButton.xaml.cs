using System;
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
        private double cachedDefaultFontSize;

        public AugmentedButton()
        {
            Theme.Initialize();

            InitializeComponent();

            SizeChanged += Button_SizeChanged;

            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            RenderOptions.SetBitmapScalingMode(ImageIcon, BitmapScalingMode.HighQuality);

            IsEnabledChanged += AugmentedButton_IsEnabledChanged;

            ImagePopupIndicator.Source = Icons.GetAppIcon(Icons.AugmentedButtonDown);
            PanelPopupPanel.Visibility = Visibility.Collapsed;

            // Initialise
            Icon = null;
            CaptionDock = Dock.Bottom;

            ApplyStyling(this);

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            ObjPanelCentered.Visibility = System.Windows.Visibility.Collapsed;

            cachedDefaultFontSize = FontSize;
            if (cachedDefaultFontSize < 1)        // don't check against exact 0 as size is a `double` type
            {
                cachedDefaultFontSize = 12;
            }
        }

        public bool CenteredMode
        {
            get => (ObjPanelCentered.Visibility == System.Windows.Visibility.Visible);
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

        private void AugmentedButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
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
            get => TextCaptionCentered.Text;

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
                MinWidth = (IconVisibility == Visibility.Visible ? IconWidth : 25);
            }
            else
            {
                MinWidth = 0;
            }
        }

        public Visibility IconVisibility
        {
            get => ImageIcon.Visibility;
            set
            {
                ImageIcon.Visibility = value;

                RecheckSpacerVisibility();
            }
        }

        public ImageSource Icon
        {
            get => ImageIcon.Source;
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
            set => ImageIcon.Width = value;

            get => ImageIcon.Width;
        }

        public double IconHeight
        {
            set => ImageIcon.Height = value;

            get => ImageIcon.Height;
        }

        public Dock CaptionDock
        {
            get => Dock.Bottom;

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

        private bool _AutoScaleText = false;
        public bool AutoScaleText
        {
            get => _AutoScaleText;
            set => _AutoScaleText = value;
        }

        private Popup attached_popup = null;
        public void AttachPopup(Popup popup)
        {
            PanelPopupPanel.Visibility = Visibility.Visible;
            attached_popup = popup;
            Click += AugmentedButtonPopup_Click;
        }

        private void AugmentedButtonPopup_Click(object sender, RoutedEventArgs e)
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

        private void Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const double THRESHOLD = 48;

            //if (!this.NeverMeasured)
            if (AutoScaleText)
            {
                if (ActualWidth > 0)
                {
                    if (ActualWidth < THRESHOLD)
                    {
                        FontSize = Math.Max(2, (cachedDefaultFontSize * ActualWidth) / THRESHOLD);
                    }
                    else
                    {
                        FontSize = cachedDefaultFontSize;
                    }
                }
                else
                {
                    FontSize = cachedDefaultFontSize;
                }
            }
        }
    }
}
