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
        double X1, Y1;
        double X2, Y2;
        double HeadWidth, HeadHeight;

        #region OLDConnectorControl

        SceneRenderingControl scene_rendering_control;

        internal Guid guid = Guid.NewGuid();
        internal NodeControl node_from;
        internal NodeControl node_to;
        internal bool Deleted { get; set; }

        internal bool is_on_screen = false;

        PropertyShadow shadow;

        NodeControl.DimensionsChangedDelegate Node_OnDimensionsChangedDelegate;
        NodeControl.DeletedDelegate Node_OnDeletedDelegate;


        static readonly SolidColorBrush STROKE_BRUSH;
        static ConnectorControl()
        {
            STROKE_BRUSH = new SolidColorBrush(Color.FromArgb(128, 0,0,0));
            STROKE_BRUSH.Freeze();
        }
        
        public ConnectorControl(SceneRenderingControl scene_rendering_control)
        {
            this.scene_rendering_control = scene_rendering_control;

            Node_OnDimensionsChangedDelegate = Node_OnDimensionsChanged;
            Node_OnDeletedDelegate = Node_OnDeleted;

            //this.Stroke = STROKE_BRUSH;
            //this.StrokeThickness = 0.5;
            this.Fill = STROKE_BRUSH;
            this.HeadWidth = 0;
            this.HeadHeight = 2;

            this.Focusable = true;
            this.GotFocus += ConnectorControl_GotFocus;
            this.MouseDown += ConnectorControl_MouseDown;
            this.PreviewKeyDown += ConnectorControl_PreviewKeyDown;

            shadow = new PropertyShadow(this);
        }

        public NodeControl NodeFrom
        {
            get
            {
                return node_from;
            }
        }
        public NodeControl NodeTo
        {
            get
            {
                return node_to;
            }
        }

        void ConnectorControl_GotFocus(object sender, RoutedEventArgs e)
        {
//            scene_rendering_control.SetSelectedNodeControl(null);
//            scene_rendering_control.selected_connector_control.Selected = sender as ConnectorControl;
        }

        void ConnectorControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();

            scene_rendering_control.UpdateMouseTracking(e, true);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                scene_rendering_control.SetSelectedNodeControl(null, false);
                scene_rendering_control.selected_connector_control.Selected = sender as ConnectorControl;
            }

            e.Handled = true;
        }

        void ConnectorControl_PreviewKeyDown(object sender, KeyEventArgs e)
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
            if (null != this.node_from)
            {
                this.node_from.OnDimensionsChanged -= Node_OnDimensionsChangedDelegate;
                this.node_from.OnDeleted -= Node_OnDeletedDelegate;
            }
            if (null != this.node_to)
            {
                this.node_to.OnDimensionsChanged -= Node_OnDimensionsChangedDelegate;
                this.node_to.OnDeleted -= Node_OnDeletedDelegate;
            }

            // Know our new nodes
            this.node_from = node_from_;
            this.node_to = node_to_;

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

        void Node_OnDimensionsChanged(NodeControl nc)
        {
            RealignToNodes();
        }

        void Node_OnDeleted(NodeControl nc)
        {
            if (node_from.scene_data.Deleted || node_to.scene_data.Deleted)
            {
                Deleted = true;
                RealignToNodes();
                RaiseOnDeleted();
            }
        }

        void RealignToNodes()
        {
            bool is_off_screen =
                false
                || (!node_from.is_on_screen && !node_to.is_on_screen)
                || this.Deleted
                ;

            if (is_off_screen && this.is_on_screen)
            {
                scene_rendering_control.ObjNodesLayer.Children.Remove(this);
                this.is_on_screen = false;
            }
            else if (!is_off_screen && !this.is_on_screen)
            {
                scene_rendering_control.ObjNodesLayer.Children.Add(this);
                this.is_on_screen = true;
            }

            if (this.is_on_screen)
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
                        this.X1 = point_from_X;
                        this.Y1 = point_from_Y;
                        this.X2 = point_to_X;
                        this.Y2 = point_to_Y;
                    }

                    // --------------------------------------------------------------------------------------------------------------
                    // --- This will eventually be the code that makes up the adornments
                    // --------------------------------------------------------------------------------------------------------------                    
                    //if (true)
                    //{
                    //    double TAIL_SIZE = 20;

                    //    ConnectorTailFrom.X1 = point_from_X;
                    //    ConnectorTailFrom.Y1 = point_from_Y;
                    //    if (length_parallel > 0)
                    //    {
                    //        ConnectorTailFrom.X2 = point_from_X + TAIL_SIZE * direction_parallel.X / length_parallel;
                    //        ConnectorTailFrom.Y2 = point_from_Y + TAIL_SIZE * direction_parallel.Y / length_parallel;
                    //    }
                    //    else
                    //    {
                    //        ConnectorTailFrom.X2 = point_from_X;
                    //        ConnectorTailFrom.Y2 = point_from_Y;
                    //    }

                    //    ConnectorTailTo.X2 = point_to_X;
                    //    ConnectorTailTo.Y2 = point_to_Y;

                    //    if (length_parallel > 0)
                    //    {
                    //        ConnectorTailTo.X1 = point_to_X - TAIL_SIZE * direction_parallel.X / length_parallel;
                    //        ConnectorTailTo.Y1 = point_to_Y - TAIL_SIZE * direction_parallel.Y / length_parallel;
                    //    }
                    //    else
                    //    {
                    //        ConnectorTailTo.X1 = point_to_X;
                    //        ConnectorTailTo.Y1 = point_to_Y;
                    //    }
                    //}
                }
            }

            RaiseOnDimensionsChanged();
        }

        public delegate void DeletedDelegate(ConnectorControl cc);
        public event DeletedDelegate OnDeleted;
        internal void RaiseOnDeleted()
        {
            if (null != OnDeleted)
            {
                OnDeleted(this);
            }
        }

        public delegate void DimensionsChangedDelegate(ConnectorControl cc);
        public event DimensionsChangedDelegate OnDimensionsChanged;
        internal void RaiseOnDimensionsChanged()
        {
            if (null != OnDimensionsChanged)
            {
                OnDimensionsChanged(this);
            }
        }


        #endregion OLDConnectorControl



        #region Shape Overrides

        StreamGeometry geometry = null;
        
        Point pt1 = new Point();
        Point pt2 = new Point();
        Point pt3 = new Point();
        Point pt4 = new Point();
        
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
