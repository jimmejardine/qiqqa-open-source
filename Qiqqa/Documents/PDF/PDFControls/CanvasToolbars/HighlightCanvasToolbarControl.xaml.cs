using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using icons;
using Qiqqa.Common.Common;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.CanvasToolbars
{
    /// <summary>
    /// Interaction logic for HighlightCanvasToolbarControl.xaml
    /// </summary>
    public partial class HighlightCanvasToolbarControl : UserControl
    {
        private List<AugmentedButton> buttons;

        public HighlightCanvasToolbarControl()
        {
            InitializeComponent();

            ButtonPointErase.Icon = Icons.GetAppIcon(Icons.InkPointErase);
            ButtonPointErase.Click += ButtonPointErase_Click;
            ButtonPointErase.ToolTip = "Erase your highlights.";

            // Populate our buttons array for easy operation
            buttons = new List<AugmentedButton>();
            buttons.Add(ButtonColor1);
            buttons.Add(ButtonColor2);
            buttons.Add(ButtonColor3);
            buttons.Add(ButtonColor4);
            buttons.Add(ButtonColor5);

            // Set up the buttons
            for (int i = 0; i < buttons.Count; ++i)
            {
                var button = buttons[i];
                button.Background = new SolidColorBrush(StandardHighlightColours.GetColor(i));

                button.Click += ButtonColor_Click;
                button.ToolTip = "Select a new highlighting colour.";
            }
        }

        private void ButtonPointErase_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_HighlightSelectErase);

            RebuildHighlightParameters(-1);
        }

        private void ButtonColor_Click_Premium(object sender, RoutedEventArgs e)
        {
            MessageBoxes.Info("Please consider Qiqqa Premium to highlight in this colour.");
        }

        private void ButtonColor_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_ChangeHighlightColour);

            int colourNumber = 0;

            // Check which number button sent this
            for (int i = 0; i < buttons.Count; ++i)
            {
                if (sender == buttons[i])
                {
                    colourNumber = i;
                    break;
                }
            }

            RebuildHighlightParameters(colourNumber);
        }

        private WeakReference<PDFRendererControl> pdf_renderer_control = null;
        public PDFRendererControl PDFRendererControl
        {
            get {
                if (pdf_renderer_control != null && pdf_renderer_control.TryGetTarget(out var control) && control != null)
                {
                    return control;
                }
                return null;
            }
            set
            {
                if (pdf_renderer_control == null)
                {
                    pdf_renderer_control = new WeakReference<PDFRendererControl>(value);
                }
                else
                {
                    pdf_renderer_control.SetTarget(value);
                }
            }
        }

        private void RebuildHighlightParameters(int colourNumber)
        {
            if (null != PDFRendererControl)
            {
                PDFRendererControl.RaiseHighlightChange(colourNumber);
            }
        }
    }
}
