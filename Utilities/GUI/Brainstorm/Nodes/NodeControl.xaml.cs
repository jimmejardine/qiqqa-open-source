using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utilities.GUI.Brainstorm.Connectors;
using Utilities.GUI.Brainstorm.Nodes.SimpleNodes;
using Utilities.GUI.Brainstorm.SceneManager;

namespace Utilities.GUI.Brainstorm.Nodes
{
    public partial class NodeControl : Canvas
    {
        internal SceneRenderingControl scene_rendering_control;
        internal NodeControlSceneData scene_data;

        FrameworkElement node_content_control = null;

        internal bool is_on_screen = false;

        internal PropertyShadowForPanels shadow;

        public NodeControl(SceneRenderingControl scene_rendering_control) : 
            this(scene_rendering_control, new NodeControlSceneData())
        { 
        }

        public NodeControl(SceneRenderingControl scene_rendering_control, NodeControlSceneData scene_data)
        {
            this.scene_rendering_control = scene_rendering_control;
            this.scene_data = scene_data;

            InitializeComponent();

            this.Focusable = true;

            this.SizeChanged += NodeControl_SizeChanged;
            this.GotFocus += NodeControl_GotFocus;
            this.MouseDown += NodeControl_MouseDown;
            this.MouseUp += NodeControl_MouseUp;
            this.MouseMove += NodeControl_MouseMove;
            this.PreviewKeyDown += NodeControl_PreviewKeyDown;
            this.KeyDown += NodeControl_KeyDown;

            shadow = new PropertyShadowForPanels(this);
        }

        void NodeControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (FrameworkElement child in this.Children)
            {
                child.Width = this.ActualWidth;
                child.Height = this.ActualHeight;
            }
        }

        void NodeControl_GotFocus(object sender, RoutedEventArgs e)
        {
//            scene_rendering_control.SetSelectedNodeControl(sender as NodeControl, false);
//            scene_rendering_control.selected_connector_control.Selected = null;
        }

        void NodeControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CaptureMouse();

            scene_rendering_control.UpdateMouseTracking(e, true);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                scene_rendering_control.selected_connector_control.Selected = null;

                if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
                {
                    NodeControl nc_linked = sender as NodeControl;

                    foreach (NodeControl node_control in scene_rendering_control.GetSelectedNodeControls())
                    {
                        ConnectorControl cc = new ConnectorControl(scene_rendering_control);
                        cc.SetNodes(node_control, nc_linked);
                        scene_rendering_control.AddNewConnectorControl(cc);
                    }
                }
                else
                {
                    if (!scene_rendering_control.IsSelectedNodeControl(this))
                    {
                        scene_rendering_control.SetSelectedNodeControl(sender as NodeControl, scene_rendering_control.IsUserIndicatingAddToSelection());
                        scene_rendering_control.selected_connector_control.Selected = null;
                    }
                }

                e.Handled = true;
            }
        }

        void NodeControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            scene_rendering_control.UpdateMouseTracking(e, false);

            ReleaseMouseCapture();            
        }

        void NodeControl_MouseMove(object sender, MouseEventArgs e)
        {
            scene_rendering_control.UpdateMouseTracking(e, false);

            if (MouseButtonState.Pressed == e.LeftButton)
            {
                // Are we resizing
                if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                {
                    foreach (NodeControl node_control in scene_rendering_control.GetSelectedNodeControls())
                    {
                        node_control.scene_data.SetDeltaWidth(scene_rendering_control.mouse_delta_virtual.X);
                        node_control.scene_data.SetDeltaHeight(scene_rendering_control.mouse_delta_virtual.Y);
                        node_control.RecalculateChildDimension();
                    }
                }
                else // Are we moving?
                {
                    if (0 != scene_rendering_control.mouse_delta_virtual.X || 0 != scene_rendering_control.mouse_delta_virtual.Y)
                    {
                        RecursivelySetNodeDeltas(scene_rendering_control.GetSelectedNodeControls(), scene_rendering_control.mouse_delta_virtual.X, scene_rendering_control.mouse_delta_virtual.Y);
                    }
                }
            }

            e.Handled = true;
        }

        private void RecursivelySetNodeDeltas(List<NodeControl> node_controls, double delta_x, double delta_y)
        {
            bool recurse = (Keyboard.Modifiers & ModifierKeys.Shift) > 0;

            HashSet<NodeControl> nodes_to_process = new HashSet<NodeControl>();
            HashSet<NodeControl> nodes_already_processed = new HashSet<NodeControl>();

            // Populate the source moving nodes
            node_controls.ForEach(o => nodes_to_process.Add(o));

            while (0 < nodes_to_process.Count)
            {
                // Get the next node to process
                NodeControl parent_node_control = nodes_to_process.First();
                nodes_to_process.Remove(parent_node_control);

                // Have we processed it before?  If so, skip it
                if (nodes_already_processed.Contains(parent_node_control))
                {
                    continue;
                }

                // Process this node
                parent_node_control.scene_data.SetDeltaCentreX(delta_x);
                parent_node_control.scene_data.SetDeltaCentreY(delta_y);
                parent_node_control.RecalculateChildDimension();

                // Queue its children
                if (recurse)
                {
                    IEnumerable<ConnectorControl> child_connector_controls = scene_rendering_control.ConnectorControlManager.GetNodesFrom(parent_node_control);
                    if (null != child_connector_controls)
                    {
                        foreach (var child in child_connector_controls)
                        {
                            if (!child.Deleted)
                            {
                                nodes_to_process.Add(child.node_to);
                            }
                        }
                    }
                }

                // Mark that we have processed this node now
                nodes_already_processed.Add(parent_node_control);
            }
        }

        public FrameworkElement NodeContentControl
        {
            set
            {
                if (0 < Children.Count)
                {
                    Children.Clear();
                }

                node_content_control = value;
                if (null != node_content_control)
                {
                    Children.Add(node_content_control);
                }
                else
                {
                    Logging.Warn("Null node content??!");
                }
            }

            get
            {
                return node_content_control;
            }
        }

        public NodeControlSceneData NodeControlSceneData
        {
            get
            {
                return scene_data;
            }
        }

        private static readonly double THRESHOLD_MIN_INVISIBLE = 3;
        private static readonly double THRESHOLD_MIN_START_TO_FADE = 5;
        private static readonly double THRESHOLD_MAX_START_TO_FADE = 500;
        private static readonly double THRESHOLD_MAX_INVISIBLE = 1000;

        public void RecalculateChildDimension()
        {
            // These are all screen coordinates
            double left = (this.scene_data.CentreX - this.scene_data.Width / 2 - scene_rendering_control.current_viewport_topleft.X) * scene_rendering_control.CurrentPowerScale;
            double top = (this.scene_data.CentreY - this.scene_data.Height / 2 - scene_rendering_control.current_viewport_topleft.Y) * scene_rendering_control.CurrentPowerScale;
            double width = this.scene_data.Width * scene_rendering_control.CurrentPowerScale;
            double height = this.scene_data.Height * scene_rendering_control.CurrentPowerScale;

            double extent_min = Math.Min(width, height);
            double extent_max = Math.Max(width, height);


            bool is_outside_viewport = 
                false
                || this.scene_data.CentreX + this.scene_data.Width / 2 < scene_rendering_control.current_viewport_topleft.X
                || this.scene_data.CentreX - this.scene_data.Width / 2 > scene_rendering_control.current_viewport_bottomright.X
                || this.scene_data.CentreY + this.scene_data.Height / 2 < scene_rendering_control.current_viewport_topleft.Y
                || this.scene_data.CentreY - this.scene_data.Height / 2 > scene_rendering_control.current_viewport_bottomright.Y
                ;

            bool is_too_small =
                false
                || extent_min < THRESHOLD_MIN_INVISIBLE
                ;

            bool is_too_large =
                false
                || extent_min > THRESHOLD_MAX_INVISIBLE
                ;

            bool is_selected = 
                scene_rendering_control.IsSelectedNodeControl(this);
            
            bool is_off_screen =
                !is_selected
                &&
                (
                    false
                    || is_outside_viewport
                    || is_too_small
                    || is_too_large
                    || this.scene_data.Deleted
                )
                ;

            bool position_changed = false;

            // Have we changed our on/off screenliness?
            if (is_off_screen && this.is_on_screen)
            {
                scene_rendering_control.ObjNodesLayer.Children.Remove(this);
                this.is_on_screen = false;
                position_changed = true;
            }
            else if (!is_off_screen && !this.is_on_screen)
            {
                scene_rendering_control.ObjNodesLayer.Children.Add(this);
                this.is_on_screen = true;
                position_changed = true;
            }

            // Look at dimensions - we only need to do this if we are on the screen
            if (this.is_on_screen)
            {
                // First our position
                if (shadow.SetShadowLeft(left))
                {
                    position_changed = true;
                }

                if (shadow.SetShadowTop(top))
                {
                    position_changed = true;
                }

                // Then our size
                bool size_changed = false;

                if (shadow.SetShadowWidth(width))
                {
                    size_changed = true;
                    position_changed = true;
                }
                if (shadow.SetShadowHeight(height))
                {
                    size_changed = true;
                    position_changed = true;
                }

                if (size_changed)
                {
                    ReconsiderVisibility(width, height, extent_min, extent_max, is_selected);
                }
            }

            if (position_changed)
            {
                RaiseOnDimensionsChanged();
            }
        }

        private void ReconsiderVisibility(double width, double height, double extent_min, double extent_max, bool is_selected)
        {
            if (false)
            {
            }
            else if (is_selected) // Selected trumps all
            {
                shadow.SetShadowBackground(Brushes.Transparent);
                shadow.SetShadowIsHitTestVisible(true);
                shadow.SetShadowOpacity(1);
            }
            else if (extent_min < THRESHOLD_MIN_START_TO_FADE)
            {
                shadow.SetShadowBackground(null);
                shadow.SetShadowIsHitTestVisible(false);
                shadow.SetShadowOpacity(Math.Max(0, 0.5 * (extent_min - THRESHOLD_MIN_INVISIBLE) / (THRESHOLD_MIN_START_TO_FADE - THRESHOLD_MIN_INVISIBLE)));
            }
            else if (extent_min > THRESHOLD_MAX_START_TO_FADE)
            {
                this.ReleaseMouseCapture();

                shadow.SetShadowBackground(null);
                shadow.SetShadowIsHitTestVisible(false);
                shadow.SetShadowOpacity(Math.Max(0, 0.5 * (THRESHOLD_MAX_INVISIBLE - extent_min) / (THRESHOLD_MAX_INVISIBLE - THRESHOLD_MAX_START_TO_FADE)));
            }
            else
            {
                shadow.SetShadowBackground(Brushes.Transparent);
                shadow.SetShadowIsHitTestVisible(true);
                shadow.SetShadowOpacity(1);
            }
        }

        void NodeControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Logging.Info("NodeControl_PreviewKeyDown");

            if (false) { }

            else if (Key.Insert == e.Key)
            {
                NodeControl node_control = (NodeControl)sender;

                if (KeyboardTools.IsCTRLDown())
                {
                    object content = new StringNodeContent("Child node");
                    NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content);
                }
                else
                {
                    NodeControlAddingByKeyboard.AddSiblingToNodeControl(node_control);
                }

                e.Handled = true;
            }
            
        }

        void NodeControl_KeyDown(object sender, KeyEventArgs e)
        {
            //Logging.Info("NodeControl_KeyDown");
        }

        public delegate void DeletedDelegate(NodeControl nc);
        public event DeletedDelegate OnDeleted;
        internal void RaiseOnDeleted()
        {
            if (null != OnDeleted)
            {
                OnDeleted(this);
            }
        }

        public delegate void DimensionsChangedDelegate(NodeControl nc);
        public event DimensionsChangedDelegate OnDimensionsChanged;
        internal void RaiseOnDimensionsChanged()
        {
            if (null != OnDimensionsChanged)
            {
                OnDimensionsChanged(this);
            }
        }

        public override string ToString()
        {
            return this.node_content_control.ToString();
        }

        public void AttempToEnterEditMode()
        {
            IEditableNodeContentControl editable_node_content_control = node_content_control as IEditableNodeContentControl;
            if (null != editable_node_content_control)
            {
                editable_node_content_control.EnterEditMode();
            }
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            IKeyPressableNodeContentControl key_pressable_node_content_control = node_content_control as IKeyPressableNodeContentControl;
            if (null != key_pressable_node_content_control)
            {
                key_pressable_node_content_control.ProcessKeyPress(e);
            }
        }
    }
}
