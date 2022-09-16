using System;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Brainstorm.Nodes;
using Utilities.Misc;

namespace Qiqqa.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for SceneRenderingControlScrollWrapper.xaml
    /// </summary>
    public partial class SceneRenderingControlScrollWrapper : Grid
    {
        public SceneRenderingControlScrollWrapper()
        {
            InitializeComponent();

            ObjSceneRenderingControl.ScrollInfoChanged += ObjSceneRenderingControl_ScrollInfoChanged;

            ScrollHorizonal.ValueChanged += ScrollHorizonal_ValueChanged;
            ScrollVertical.ValueChanged += ScrollVertical_ValueChanged;
        }

        private void ObjSceneRenderingControl_ScrollInfoChanged()
        {
            DoHorizontal();
            DoVertical();
        }

        private bool horizonal_changing = false;
        private bool vertical_changing = false;

        private void DoHorizontal()
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (NodeControl node_control in ObjSceneRenderingControl.NodeControls)
            {
                min = Math.Min(min, node_control.NodeControlSceneData.CentreX);
                max = Math.Max(max, node_control.NodeControlSceneData.CentreX);
            }

            double mid = (ObjSceneRenderingControl.current_viewport_topleft.X + ObjSceneRenderingControl.ActualWidth / ObjSceneRenderingControl.CurrentPowerScale / 2);
            double visible = ObjSceneRenderingControl.ActualWidth / ObjSceneRenderingControl.CurrentPowerScale;

#if DEBUG
            // Are we looking at this dialog in the Visual Studio Designer?
            if (Runtime.IsRunningInVisualStudioDesigner && 0 == ObjSceneRenderingControl.NodeControls.Count && Double.IsNaN(mid))
            {
                min = Math.Min(min, 0);
                max = Math.Max(max, 8000);
                mid = 2500;
                visible = 225;
            }
#endif

            horizonal_changing = true;
            ScrollHorizonal.Minimum = min;
            ScrollHorizonal.Maximum = max;
            ScrollHorizonal.Value = mid;
            ScrollHorizonal.ViewportSize = visible;
            horizonal_changing = false;
        }

        private void DoVertical()
        {
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (NodeControl node_control in ObjSceneRenderingControl.NodeControls)
            {
                min = Math.Min(min, node_control.NodeControlSceneData.CentreY);
                max = Math.Max(max, node_control.NodeControlSceneData.CentreY);
            }

            double mid = (ObjSceneRenderingControl.current_viewport_topleft.Y + ObjSceneRenderingControl.ActualHeight / ObjSceneRenderingControl.CurrentPowerScale / 2);
            double visible = ObjSceneRenderingControl.ActualHeight / ObjSceneRenderingControl.CurrentPowerScale;

#if DEBUG
            // Are we looking at this dialog in the Visual Studio Designer?
            if (Runtime.IsRunningInVisualStudioDesigner && 0 == ObjSceneRenderingControl.NodeControls.Count && Double.IsNaN(mid))
            {
                min = Math.Min(min, 0);
                max = Math.Max(max, 8000);
                mid = 2500;
                visible = 225;
            }
#endif

            vertical_changing = true;
            ScrollVertical.Minimum = min;
            ScrollVertical.Maximum = max;
            ScrollVertical.Value = mid;
            ScrollVertical.ViewportSize = visible;
            vertical_changing = false;
        }

        private void ScrollHorizonal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!horizonal_changing)
            {
                ObjSceneRenderingControl.current_viewport_topleft.X = e.NewValue - ObjSceneRenderingControl.ActualWidth / ObjSceneRenderingControl.CurrentPowerScale / 2;
                ObjSceneRenderingControl.ViewportHasChanged();
            }
        }

        private void ScrollVertical_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!vertical_changing)
            {
                ObjSceneRenderingControl.current_viewport_topleft.Y = e.NewValue - ObjSceneRenderingControl.ActualHeight / ObjSceneRenderingControl.CurrentPowerScale / 2;
                ObjSceneRenderingControl.ViewportHasChanged();
            }
        }
    }
}
