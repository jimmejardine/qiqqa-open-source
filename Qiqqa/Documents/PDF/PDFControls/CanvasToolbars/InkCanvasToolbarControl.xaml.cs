using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using icons;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.CanvasToolbars
{
    /// <summary>
    /// Interaction logic for InkCanvasToolbarControl.xaml
    /// </summary>
    public partial class InkCanvasToolbarControl : UserControl
    {
        public InkCanvasToolbarControl()
        {
            InitializeComponent();

            ButtonDraw.Icon = Icons.GetAppIcon(Icons.InkDraw);
            ButtonStrokeErase.Icon = Icons.GetAppIcon(Icons.InkStrokeErase);
            ButtonPointErase.Icon = Icons.GetAppIcon(Icons.InkPointErase);
            ButtonSelect.Icon = Icons.GetAppIcon(Icons.InkSelect);

            ButtonDraw.ToolTip = "Draw";
            ButtonStrokeErase.ToolTip = "Stroke erase";
            ButtonPointErase.ToolTip = "Point erase";
            ButtonSelect.ToolTip = "Select, cut & move";
            ObjColorPicker.ToolTip = "Draw colour";

            ButtonDraw.Click += ButtonDraw_Click;
            ButtonStrokeErase.Click += ButtonStrokeErase_Click;
            ButtonPointErase.Click += ButtonPointErase_Click;
            ButtonSelect.Click += ButtonSelect_Click;

            ObjColorPicker.SelectedColorChanged += ObjColorPicker_SelectedColorChanged;
        }

        private void ObjColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                RebuildInkParameters();
            });
        }

        private WeakReference<PDFRendererControl> pdf_renderer_control = null;
        public PDFRendererControl PDFRendererControl
        {
            get
            {
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


        private void RaiseInkChange(InkCanvasEditingMode inkCanvasEditingMode)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_ChangeInkEditingMode);

            PDFRendererControl.RaiseInkChange(inkCanvasEditingMode);
        }

        private void RebuildInkParameters()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_ChangeInkParameters);

            DrawingAttributes drawingAttributes = new DrawingAttributes();

            drawingAttributes.Color = ObjColorPicker.SelectedColor;

            PDFRendererControl.RaiseInkChange(drawingAttributes);
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            RaiseInkChange(InkCanvasEditingMode.Select);
        }

        private void ButtonPointErase_Click(object sender, RoutedEventArgs e)
        {
            RaiseInkChange(InkCanvasEditingMode.EraseByPoint);
        }

        private void ButtonStrokeErase_Click(object sender, RoutedEventArgs e)
        {
            RaiseInkChange(InkCanvasEditingMode.EraseByStroke);
        }

        private void ButtonDraw_Click(object sender, RoutedEventArgs e)
        {
            RaiseInkChange(InkCanvasEditingMode.Ink);
        }

    }
}
