using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using icons;
using Utilities.Misc;

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

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            TextCaption.HorizontalAlignment = HorizontalAlignment.Stretch;
            TextCaption.VerticalAlignment = VerticalAlignment.Stretch;

            RenderOptions.SetBitmapScalingMode(ImageIcon, BitmapScalingMode.HighQuality);

            IsEnabledChanged += AugmentedButton_IsEnabledChanged;

            ImagePopupIndicator.Source = Icons.GetAppIcon(Icons.AugmentedButtonDown);
            PanelPopupPanel.Visibility = Visibility.Collapsed;

            // Initialise
            Icon = null;
            CaptionDock = Dock.Bottom;

            ApplyStyling(this);

            cachedDefaultFontSize = FontSize;
            if (cachedDefaultFontSize < 1)        // don't check against exact 0 as size is a `double` type
            {
                cachedDefaultFontSize = 12;
            }

            // When in Designer mode, show a bit of stuff or the thing looks weird:
            if (Runtime.IsRunningInVisualStudioDesigner && false)
            {
                if (String.IsNullOrWhiteSpace(Caption))
                {
                    Caption = "Sample in DesignMode";
                }
                if (ImageIcon == null || ImageIcon.Source == null || ImageIcon.Width < 1 || ImageIcon.Height < 1)
                {
                    ImageIcon.Source = Icons.GetAppIcon(Icons.SaveAs);
                }
            }
        }

        public bool CenteredMode
        {
            // Visual Studio b0rks in Designer when this one doesn't have a get method:
            get
            {
                return TextCaption.HorizontalAlignment == HorizontalAlignment.Center;
            }
            set
            {
                if (value)
                {
                    TextCaption.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    TextCaption.HorizontalAlignment = HorizontalAlignment.Left;
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
            get => TextCaption.Text;

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    TextCaption.Visibility = Visibility.Visible;
                    TextCaption.Text = value;

                    RecheckVisibility();
                }
                else
                {
                    TextCaption.Visibility = Visibility.Collapsed;
                    TextCaption.Text = value;

                    RecheckVisibility();
                }
            }
        }

        private void RecheckVisibility()
        {
            if (Visibility.Visible == TextCaption.Visibility && Visibility.Visible == ImageIcon.Visibility)
            {
                //TextCaption.HorizontalAlignment = HorizontalAlignment.Left;
                MinWidth = IconWidth + 25;
            }
            else if (Visibility.Visible == TextCaption.Visibility)
            {
                //TextCaption.HorizontalAlignment = HorizontalAlignment.Center;
                MinWidth = 25;
            }
            else if (Visibility.Visible == ImageIcon.Visibility)
            {
                MinWidth = IconWidth;
            }
            else
            {
                MinWidth = 25;
            }
        }

        public Visibility IconVisibility
        {
            get => ImageIcon.Visibility;
            set
            {
                ImageIcon.Visibility = value;

                RecheckVisibility();
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

                RecheckVisibility();
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

        private Dock _dockmode = Dock.Bottom;

        public Dock CaptionDock
        {
            get => _dockmode;

            set
            {
                _dockmode = value;

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
