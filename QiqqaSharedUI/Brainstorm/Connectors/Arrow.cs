using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Qiqqa.Brainstorm.Nodes;
using Qiqqa.Brainstorm.SceneManager;
using Utilities.GUI;

namespace Qiqqa.Brainstorm.Connectors
{
    public sealed class ConnectorControl : Shape
    {
        private double X1, Y1;
        private double X2, Y2;
        private double HeadWidth, HeadHeight;

        #region OLDConnectorControl

        private SceneRenderingControl scene_rendering_control;

        internal Guid guid = Guid.NewGuid();
        internal NodeControl node_from;
        internal NodeControl node_to;
        internal bool Deleted { get; set; }

        internal bool is_on_screen = false;
        private PropertyShadow shadow;
        private NodeControl.DimensionsChangedDelegate Node_OnDimensionsChangedDelegate;
        private NodeControl.DeletedDelegate Node_OnDeletedDelegate;
        private static readonly SolidColorBrush STROKE_BRUSH;
        static ConnectorControl()
        {
            STROKE_BRUSH = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
            STROKE_BRUSH.Freeze();
        }

        public ConnectorControl(SceneRenderingControl scene_rendering_control)
        {
            this.scene_rendering_control = scene_rendering_control;

            Node_OnDimensionsChangedDelegate = Node_OnDimensionsChanged;
            Node_OnDeletedDelegate = Node_OnDeleted;

            //this.Stroke = STROKE_BRUSH;
            //this.StrokeThickness = 0.5;
            Fill = STROKE_BRUSH;
            HeadWidth = 0;
            HeadHeight = 2;

            Focusable = true;
            GotFocus += ConnectorControl_GotFocus;
            MouseDown += ConnectorControl_MouseDown;
            PreviewKeyDown += ConnectorControl_PreviewKeyDown;

            shadow = new PropertyShadow(this);
        }

        public NodeControl NodeFrom => node_from;
        public NodeControl NodeTo => node_to;

        private void ConnectorControl_GotFocus(object sender, RoutedEventArgs e)
        {
            //            scene_rendering_control.SetSelectedNodeControl(null);
            //            scene_rendering_control.selected_connector_control.Selected = sender as ConnectorControl;
        }

        private void ConnectorControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();

            scene_rendering_control.UpdateMouseTracking(e, true);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                scene_rendering_control.SetSelectedNodeControl(null, false);
                scene_rendering_control.selected_connector_control.Selected = sender as ConnectorControl;
            }

            e.Handled = true;
        }

        private void ConnectorControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                if (true == MessageBoxes.AskQuestion("Are you sure you want to delete this connector?"))
                {
                    Deleted = true;
                    RealignToNodes();
                    RaiseOnDeleted();
                }
                e.Handled = true;
            }
        }

        public void SetNodes(NodeControl node_from_, NodeControl node_to_)
        {
            // Unregister the existing nodes
            if (null != node_from)
            {
                node_from.OnDimensionsChanged -= Node_OnDimensionsChangedDelegate;
                node_from.OnDeleted -= Node_OnDeletedDelegate;
            }
            if (null != node_to)
            {
                node_to.OnDimensionsChanged -= Node_OnDimensionsChangedDelegate;
                node_to.OnDeleted -= Node_OnDeletedDelegate;
            }

            // Know our new nodes
            node_from = node_from_;
            node_to = node_to_;

            // Match our nodes' zorder
            int min_z_index = Math.Min(Canvas.GetZIndex(node_from), Canvas.GetZIndex(node_to));
            Canvas.SetZIndex(this, min_z_index - 1);

            // Register with them
            if (null != node_from)
            {
                node_from.OnDimensionsChanged += Node_OnDimensionsChangedDelegate;
                node_from.OnDeleted += Node_OnDeletedDelegate;
            }
            if (null != node_to)
            {
                node_to.OnDimensionsChanged += Node_OnDimensionsChangedDelegate;
                node_to.OnDeleted += Node_OnDeletedDelegate;
            }

            RealignToNodes();
        }

        private void Node_OnDimensionsChanged(NodeControl nc)
        {
            RealignToNodes();
        }

        private void Node_OnDeleted(NodeControl nc)
        {
            if (node_from.scene_data.Deleted || node_to.scene_data.Deleted)
            {
                Deleted = true;
                RealignToNodes();
                RaiseOnDeleted();
            }
        }

        private void RealignToNodes()
        {
            bool is_off_screen =
                false
                || (!node_from.is_on_screen && !node_to.is_on_screen)
                || Deleted
                ;

            if (is_off_screen && is_on_screen)
            {
                scene_rendering_control.ObjNodesLayer.Children.Remove(this);
                is_on_screen = false;
            }
            else if (!is_off_screen && !is_on_screen)
            {
                scene_rendering_control.ObjNodesLayer.Children.Add(this);
                is_on_screen = true;
            }

            if (is_on_screen)
            {
                double min_x = Math.Min(node_from.scene_data.CentreX, node_to.scene_data.CentreX);
                double min_y = Math.Min(node_from.scene_data.CentreY, node_to.scene_data.CentreY);
                double max_x = Math.Max(node_from.scene_data.CentreX, node_to.scene_data.CentreX);
                double max_y = Math.Max(node_from.scene_data.CentreY, node_to.scene_data.CentreY);

                double BORDER_PADDING = 60;

                double opacity = 0;
                if (node_from.is_on_screen) opacity = Math.Max(opacity, node_from.shadow.GetShadowOpacity());
                if (node_to.is_on_screen) opacity = Math.Max(opacity, node_to.shadow.GetShadowOpacity());
                shadow.SetShadowOpacity(opacity);

                double left = (min_x - scene_rendering_control.current_viewport_topleft.X) * scene_rendering_control.CurrentPowerScale - BORDER_PADDING;
                double top = (min_y - scene_rendering_control.current_viewport_topleft.Y) * scene_rendering_control.CurrentPowerScale - BORDER_PADDING;
                shadow.SetShadowLeft(left);
                shadow.SetShadowTop(top);


                bool arrow_requires_change = false;
                double width = (max_x - min_x) * scene_rendering_control.CurrentPowerScale + 2 * BORDER_PADDING;
                double height = (max_y - min_y) * scene_rendering_control.CurrentPowerScale + 2 * BORDER_PADDING;
                if (shadow.SetShadowWidth(width))
                {
                    arrow_requires_change = true;
                }
                if (shadow.SetShadowHeight(height))
                {
                    arrow_requires_change = true;
                }

                if (arrow_requires_change)
                {
                    double point_from_X = (node_from.scene_data.CentreX - min_x) * scene_rendering_control.CurrentPowerScale + BORDER_PADDING;
                    double point_from_Y = (node_from.scene_data.CentreY - min_y) * scene_rendering_control.CurrentPowerScale + BORDER_PADDING;
                    double point_to_X = (node_to.scene_data.CentreX - min_x) * scene_rendering_control.CurrentPowerScale + BORDER_PADDING;
                    double point_to_Y = (node_to.scene_data.CentreY - min_y) * scene_rendering_control.CurrentPowerScale + BORDER_PADDING;

                    // Join the connector line

                    if (true)
                    {
                        X1 = point_from_X;
                        Y1 = point_from_Y;
                        X2 = point_to_X;
                        Y2 = point_to_Y;
                    }

                    // --------------------------------------------------------------------------------------------------------------
                    // --- This will eventually be the code that makes up the adornments
                    // --------------------------------------------------------------------------------------------------------------                    
#if false
                    {
                        double TAIL_SIZE = 20;

                        ConnectorTailFrom.X1 = point_from_X;
                        ConnectorTailFrom.Y1 = point_from_Y;
                        if (length_parallel > 0)
                        {
                            ConnectorTailFrom.X2 = point_from_X + TAIL_SIZE * direction_parallel.X / length_parallel;
                            ConnectorTailFrom.Y2 = point_from_Y + TAIL_SIZE * direction_parallel.Y / length_parallel;
                        }
                        else
                        {
                            ConnectorTailFrom.X2 = point_from_X;
                            ConnectorTailFrom.Y2 = point_from_Y;
                        }

                        ConnectorTailTo.X2 = point_to_X;
                        ConnectorTailTo.Y2 = point_to_Y;

                        if (length_parallel > 0)
                        {
                            ConnectorTailTo.X1 = point_to_X - TAIL_SIZE * direction_parallel.X / length_parallel;
                            ConnectorTailTo.Y1 = point_to_Y - TAIL_SIZE * direction_parallel.Y / length_parallel;
                        }
                        else
                        {
                            ConnectorTailTo.X1 = point_to_X;
                            ConnectorTailTo.Y1 = point_to_Y;
                        }
                    }
#endif
                }
            }

            RaiseOnDimensionsChanged();
        }

        public delegate void DeletedDelegate(ConnectorControl cc);
        public event DeletedDelegate OnDeleted;
        
        internal void RaiseOnDeleted()
        {
            OnDeleted?.Invoke(this);
        }

        public delegate void DimensionsChangedDelegate(ConnectorControl cc);
        public event DimensionsChangedDelegate OnDimensionsChanged;

        internal void RaiseOnDimensionsChanged()
        {
            OnDimensionsChanged?.Invoke(this);
        }


        #endregion OLDConnectorControl



        #region Shape Overrides

        private StreamGeometry geometry = null;
        private Point pt1 = new Point();
        private Point pt2 = new Point();
        private Point pt3 = new Point();
        private Point pt4 = new Point();

        protected override Geometry DefiningGeometry
        {
            get
            {
                bool need_new_geometry =
                    null == geometry
                    || pt1.X != X1
                    || pt1.Y != Y1
                    || pt2.X != X2
                    || pt2.Y != Y2
                    ;

                if (need_new_geometry)
                {
                    if (null == geometry)
                    {
                        geometry = new StreamGeometry();
                    }

                    geometry.Clear();
                    using (StreamGeometryContext context = geometry.Open())
                    {
                        InternalDrawArrowGeometry(context);
                    }
                }

                return geometry;
            }
        }

        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            pt1.X = X1;
            pt1.Y = Y1;
            pt2.X = X2;
            pt2.Y = Y2;

            pt3.X = pt1.X + (HeadWidth * cost - HeadHeight * sint);
            pt3.Y = pt1.Y + (HeadWidth * sint + HeadHeight * cost);

            pt4.X = pt1.X + (HeadWidth * cost + HeadHeight * sint);
            pt4.Y = pt1.Y - (HeadHeight * cost - HeadWidth * sint);

            // Draw the line and arrow
            context.BeginFigure(pt2, true, true);
            context.LineTo(pt3, false, false);
            context.LineTo(pt4, false, false);
        }

        #endregion
    }
}
