using System;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Brainstorm.Nodes;

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

        void ObjSceneRenderingControl_ScrollInfoChanged()
        {
            DoHorizontal();
            DoVertical();
        }

        bool horizonal_changing = false;
        bool vertical_changing = false;

        void DoHorizontal()
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

            horizonal_changing = true;
            ScrollHorizonal.Minimum = min;
            ScrollHorizonal.Maximum = max;
            ScrollHorizonal.Value = mid;
            ScrollHorizonal.ViewportSize = visible;
            horizonal_changing = false;
        }

        void DoVertical()
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

            vertical_changing = true;
            ScrollVertical.Minimum = min;
            ScrollVertical.Maximum = max;
            ScrollVertical.Value = mid;
            ScrollVertical.ViewportSize = visible;
            vertical_changing = false;
        }

        void ScrollHorizonal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!horizonal_changing)
            {
                ObjSceneRenderingControl.current_viewport_topleft.X = e.NewValue - ObjSceneRenderingControl.ActualWidth / ObjSceneRenderingControl.CurrentPowerScale / 2;
                ObjSceneRenderingControl.ViewportHasChanged();
            }
        }

        void ScrollVertical_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!vertical_changing)
            {
                ObjSceneRenderingControl.current_viewport_topleft.Y = e.NewValue - ObjSceneRenderingControl.ActualHeight / ObjSceneRenderingControl.CurrentPowerScale / 2;
                ObjSceneRenderingControl.ViewportHasChanged();                
            }
        }
    }
}
