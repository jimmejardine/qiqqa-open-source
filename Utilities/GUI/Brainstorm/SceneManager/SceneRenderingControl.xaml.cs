using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Utilities.Files;
using Utilities.GUI.Brainstorm.Common;
using Utilities.GUI.Brainstorm.Common.Searching;
using Utilities.GUI.Brainstorm.Connectors;
using Utilities.GUI.Brainstorm.DragDropStuff;
using Utilities.GUI.Brainstorm.Nodes;
using Utilities.GUI.Brainstorm.Nodes.SimpleNodes;
using Utilities.Internet;

namespace Utilities.GUI.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for SceneRenderingControl.xaml
    /// </summary>
    public partial class SceneRenderingControl : Grid, IDisposable
    {
        BrainstormMetadataControl brainstorm_metadata_control;
        BrainstormMetadata brainstorm_metadata;

        AutoArranger auto_arranger;
        DragDropManager drag_drop_manager;
        BasicDragDropBehaviours basic_drag_drop_behaviours;
        
        internal SelectedConnectorControl selected_connector_control;
        internal SelectingNodesControl selecting_nodes_control;

        private List<NodeControl> node_controls = new List<NodeControl>();
        private ConnectorControlManager connector_control_manager = new ConnectorControlManager();

        public delegate void SelectedNodeControlChangedDelegate(NodeControl node_control);
        public event SelectedNodeControlChangedDelegate SelectedNodeControlChanged;

        public enum NodeAdditionPolicyEnum
        {
            AlwaysUseExisting,
            AlwaysCreateNew
        }
        NodeAdditionPolicyEnum node_addition_policy = NodeAdditionPolicyEnum.AlwaysUseExisting;
        public NodeAdditionPolicyEnum NodeAdditionPolicy
        {
            get { return node_addition_policy; }
            set { node_addition_policy = value; }
        }

        public List<NodeControl> NodeControls
        {
            get
            {
                return node_controls;
            }
        }

        public ConnectorControlManager ConnectorControlManager
        {
            get
            {
                return connector_control_manager;                
            }
        }

        public AutoArranger AutoArranger
        {
            get
            {
                return auto_arranger;
            }
        }

        public DragDropManager DragDropManager
        {
            get
            {
                return drag_drop_manager;
            }
        }

        public IWebProxy Proxy { get; set; }

        #region --- Viewport management ----------------------------------------------------------------------------------------

        internal delegate void ScrollInfoChangedDelegate();
        internal event ScrollInfoChangedDelegate ScrollInfoChanged;
        private void FireScrollInfoChanged()
        {
            if (null != ScrollInfoChanged)
            {
                ScrollInfoChanged();
            }
        }

        internal Point mouse_previous = new Point(0.0, 0.0);
        internal Point mouse_current = new Point(0.0, 0.0);
        internal Point mouse_initial = new Point(0.0, 0.0);

        internal Point mouse_delta_virtual = new Point(0.0, 0.0);
        internal Point mouse_current_virtual = new Point(0.0, 0.0);
        internal Point mouse_initial_virtual = new Point(0.0, 0.0);        

        internal Point current_viewport_topleft = new Point(0.0, 0.0);
        internal Point current_viewport_bottomright = new Point(0.0, 0.0);

        double _current_scale;
        double _current_power_scale;

        internal double CurrentScale
        {
            get
            {
                return _current_scale;
            }
            private set
            {
                _current_scale = value;                
                _current_power_scale = Math.Pow(1.2, _current_scale);
            }
        }

        internal double CurrentPowerScale
        {
            get
            {
                return _current_power_scale;
            }
            private set
            {
                _current_power_scale = value;
                _current_scale = Math.Log(_current_power_scale, 1.2);
            }
        }

        internal void ViewportHasChanged()
        {
            current_viewport_bottomright.X = current_viewport_topleft.X + this.ActualWidth / CurrentPowerScale;
            current_viewport_bottomright.Y = current_viewport_topleft.Y + this.ActualHeight / CurrentPowerScale;

            RecalculateAllNodeControlDimensions();

            FireScrollInfoChanged();
        }

        #endregion
        
        #region --- Creation ----------------------------------------------------------------------------------------

        public SceneRenderingControl()
        {
            InitializeComponent();

            this.Background = ThemeColours.Background_Brush_Blue_LightToDark;

            this.ClipToBounds = true;
            this.Focusable = true;

            auto_arranger = new AutoArranger(this);
            drag_drop_manager = new DragDropManager(this);
            basic_drag_drop_behaviours = new BasicDragDropBehaviours(drag_drop_manager);
            basic_drag_drop_behaviours.RegisterBehaviours();
            
            this.AllowDrop = true;
            this.DragOver += SceneRenderingControl_DragOver;
            this.Drop += SceneRenderingControl_Drop;

            this.MouseDown += SceneRenderingControl_MouseDown;
            this.MouseUp += SceneRenderingControl_MouseUp;
            this.MouseMove += SceneRenderingControl_MouseMove;
            this.MouseWheel += SceneRenderingControl_MouseWheel;
            this.SizeChanged += SceneRenderingControl_SizeChanged;

            this.KeyDown += SceneRenderingControl_KeyDown;
        }

        internal void SceneRenderingControl_PostConstructor(BrainstormMetadataControl brainstorm_metadata_control)
        {
            this.brainstorm_metadata_control = brainstorm_metadata_control;

            New();

            RecalculateAllNodeControlDimensions();

            this.Focus();
        }


        void SceneRenderingControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (false) {}

            else if (e.Key == Key.V && KeyboardTools.IsCTRLDown())
            {
                DoPaste();
            }
            else if (Key.Delete == e.Key)
            {
                List<NodeControl> selected_node_controls = GetSelectedNodeControls();
                if (0 < selected_node_controls.Count)
                {
                    if (true == MessageBoxes.AskQuestion(String.Format("Are you sure you want to delete the {0} selected node(s)?", selected_node_controls.Count)))
                    {
                        foreach (NodeControl node_control in GetSelectedNodeControls())
                        {
                            node_control.scene_data.Deleted = true;
                            node_control.RaiseOnDeleted();
                            node_control.RecalculateChildDimension();
                        }
                    }
                }

                e.Handled = true;
            }
            else
            {
                PropagateKeyPressToNodes(e);
            }

        }

        private void PropagateKeyPressToNodes(KeyEventArgs e)
        {
            // Propagate the key press to the selected node controls (if any)
            List<NodeControl> selected_node_controls = GetSelectedNodeControls();
            foreach (NodeControl node_control in selected_node_controls)
            {
                node_control.ProcessKeyPress(e);
            }
        }

        void ResetViewport()
        {
            mouse_previous.X = 0;
            mouse_previous.Y = 0;
            mouse_current.X = 0;
            mouse_current.Y = 0;

            mouse_delta_virtual.X = 0;
            mouse_delta_virtual.Y = 0;
            mouse_current_virtual.X = 0;
            mouse_current_virtual.Y = 0;

            // Centre the viewport on 0,0
            if (0 != this.ActualWidth && 0 != this.ActualHeight)
            {
                current_viewport_topleft.X = -this.ActualWidth / CurrentPowerScale / 2.0;
                current_viewport_topleft.Y = -this.ActualHeight / CurrentPowerScale / 2.0;
            }

            CurrentScale = 0;

            ViewportHasChanged();            
        }
        
        void AddMetaControls()
        {
            selected_connector_control = new SelectedConnectorControl();
            selected_connector_control.Selected = null;
            ObjControlsLayer.Children.Add(selected_connector_control);

            selecting_nodes_control = new SelectingNodesControl();
            ObjControlsLayer.Children.Add(selecting_nodes_control);
        }

        #endregion

        #region --- Clipboard and drag and drop ------------------------------------------------------------------------------------

        private void DragDropClipboard_AddText(string text)
        {
            // First try it as a url
            try
            {
                Uri uri = new Uri(text);
                DragDropClipboard_AddUrl(text);
            }
            catch (Exception)
            {
                StringNodeContent snc = new StringNodeContent(Clipboard.GetText());
                AddNewNodeControl(snc, mouse_current_virtual.X, mouse_current_virtual.Y);
            }
        }

        private void DragDropClipboard_AddFiles(List<string> filenames)
        {
            List<object> node_contents = new List<object>();
            for (int i = 0; i < filenames.Count; ++i)
            {
                string filename = filenames[i];
                if (ImageNodeContent.IsSupportedImagePath(filename))
                {
                    node_contents.Add(new ImageNodeContent(filename));
                }
                else
                {
                    node_contents.Add(new FileSystemNodeContent(filename));
                }
            }

            AddNewNodeControls(node_contents, mouse_current_virtual.X, mouse_current_virtual.Y);
        }

        private void DragDropClipboard_AddUrl(string url)
        {
            string url_lower = url.ToLower();
            if (
                false
                || url_lower.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase)
                || url_lower.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase)
                || url_lower.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)
                || url_lower.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase)
                )
            {
                MemoryStream ms_image;
                UrlDownloader.DownloadWithBlocking(Proxy, url, out ms_image);
                ImageNodeContent inc = new ImageNodeContent(ms_image);
                AddNewNodeControl(inc, mouse_current_virtual.X, mouse_current_virtual.Y);
            }
            else
            {
                WebsiteNodeContent wnc = new WebsiteNodeContent();
                wnc.Url = url;
                wnc.LastVisited = DateTime.UtcNow;
                wnc.VisitedCount = 1;
                AddNewNodeControl(wnc, mouse_current_virtual.X, mouse_current_virtual.Y);
            }
        }

        #endregion

        #region --- Drag and drop ------------------------------------------------------------------------------------

        void SceneRenderingControl_DragOver(object sender, DragEventArgs e)
        {
            UpdateMouseTracking(e, false);

            if (drag_drop_manager.CanDrop(e))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }

            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }

            else if (e.Data.GetDataPresent("UniformResourceLocator"))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }

            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }

            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        void SceneRenderingControl_Drop(object sender, DragEventArgs e)
        {
            if (drag_drop_manager.OnDrop(e, mouse_current_virtual))
            {
                e.Handled = true;
            }

            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                DragDropClipboard_AddFiles(new List<string>(filenames));
                e.Handled = true;
            }

            else if (e.Data.GetDataPresent("UniformResourceLocator"))
            {
                MemoryStream ms = (MemoryStream)e.Data.GetData("UniformResourceLocator");
                byte[] ms_bytes = ms.ToArray();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ms_bytes.Length && 0 != ms_bytes[i]; ++i)
                {
                    sb.Append((char)ms_bytes[i]);
                }
                Logging.Info(sb.ToString());

                string url = sb.ToString();
                DragDropClipboard_AddUrl(url);
                e.Handled = true;
            }

            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.Data.GetData(DataFormats.Text);
                DragDropClipboard_AddText(text);
                e.Handled = true;
            }

            else
            {
                string msg = DragDropManager.DumpUnkownDropTypes(e);
                Logging.Info(msg);
            }
        }

        #endregion

        #region --- Clipboard management ------------------------------------------------------------------------------

        private void DoPaste()
        {
            Logging.Info("Clipboard paste");

            if (false) {}

            else if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                DragDropClipboard_AddText(text);
            }

            else if (Clipboard.ContainsImage())
            {
                BitmapSource bitmap = Clipboard.GetImage();

                string filename = TempFile.GenerateTempFilename("jpg");
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    TextBlock myTextBlock = new TextBlock();
                    encoder.QualityLevel = 80;
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                }

                ImageNodeContent inc = new ImageNodeContent(filename);
                AddNewNodeControl(inc, mouse_current_virtual.X, mouse_current_virtual.Y);
                FileTools.Delete(filename);
            }

            else if (Clipboard.ContainsFileDropList())
            {
                List<string> filenames = new List<string>();
                foreach (string filename in Clipboard.GetFileDropList())
                {
                    filenames.Add(filename);                    
                }

                DragDropClipboard_AddFiles(new List<string>(filenames));
            }
        }

        #endregion


        #region --- Adding to scene ---------------------------------------------------------------------------------------------------

        DateTime scene_changed_timestamp = DateTime.MinValue;
        public DateTime SceneChangedTimestamp
        {
            get
            {
                return scene_changed_timestamp;
            }
        }

        public ConnectorControl AddNewConnectorControl(NodeControl node_from, NodeControl node_to)
        {
            ConnectorControl connector_control = new ConnectorControl(this);
            connector_control.SetNodes(node_from, node_to);
            return AddNewConnectorControl(connector_control);                
        }
        
        public ConnectorControl AddNewConnectorControl(ConnectorControl connector_control)
        {
            scene_changed_timestamp = DateTime.UtcNow;
            connector_control_manager.Add(connector_control);
            return connector_control;
        }

        // Add a new node to the centre of the screen with default dimensions
        public NodeControl AddNewNodeControlInScreenCentre(object node_content)
        {
            double left = (current_viewport_topleft.X + current_viewport_bottomright.X) / 2.0;
            double top = (current_viewport_topleft.Y + current_viewport_bottomright.Y) / 2.0;
            return AddNewNodeControl(node_content, left, top);
        }

        public NodeControl AddNewNodeControlInScreenCentre(object node_content, double width, double height)
        {
            double left = (current_viewport_topleft.X + current_viewport_bottomright.X) / 2.0;
            double top = (current_viewport_topleft.Y + current_viewport_bottomright.Y) / 2.0;
            return AddNewNodeControl(node_content, left, top, width, height);
        }

        public List<NodeControl> AddNewNodeControlsInScreenCentre(List<object> node_contents)
        {
            double left = (current_viewport_topleft.X + current_viewport_bottomright.X) / 2.0;
            double top = (current_viewport_topleft.Y + current_viewport_bottomright.Y) / 2.0;
            return AddNewNodeControls(node_contents, left, top);
        }

        public List<NodeControl> AddNewNodeControls(List<object> node_contents, double left, double top)
        {
            List<NodeControl> node_controls = new List<NodeControl>();

            
            
            if (0 == node_contents.Count)
            {
                return null;
            }

            else if (1 == node_contents.Count)
            {
                NodeControl node_control = AddNewNodeControl(node_contents[0], left, top);
                node_controls.Add(node_control);
            }

            // If we get here, we have many node controls, so splay them out...
            else
            {
                for (int i = 0; i < node_contents.Count; ++i)
                {
                    double circle_left = left - 50 / CurrentPowerScale * node_contents.Count * Math.Cos(i * 2 * Math.PI / node_contents.Count);
                    double circle_top = top + 50 / CurrentPowerScale * node_contents.Count * Math.Sin(i * 2 * Math.PI / node_contents.Count);

                    NodeControl node_control = AddNewNodeControl(node_contents[i], circle_left, circle_top);
                    node_controls.Add(node_control);
                }
            }

            return node_controls;
        }

        public NodeControl AddNewNodeControl(object node_content, double left, double top)
        {
            return AddNewNodeControl(node_content, left, top, 0, 0);
        }

        public NodeControl AddNewNodeControl(object node_content, double left, double top, double width, double height)
        {
            scene_changed_timestamp = DateTime.UtcNow;

            // Check that we don't already have a recurrent node we can use            
            if (NodeAdditionPolicyEnum.AlwaysUseExisting == node_addition_policy)
            {
                RecurrentNodeContent recurrent_node_content = node_content as RecurrentNodeContent;
                if (null != recurrent_node_content)
                {
                    foreach (NodeControl node_control in node_controls)
                    {
                        if (!node_control.NodeControlSceneData.Deleted && recurrent_node_content.Equals(node_control.NodeControlSceneData.node_content))
                        {
                            return node_control;                            
                        }
                    }
                }
            }

            // If we get here, we have to create a new node
            {
                NodeControlSceneData node_control_scene_data = new NodeControlSceneData();
                node_control_scene_data.node_content = node_content;
                node_control_scene_data.SetWidth(width);
                node_control_scene_data.SetHeight(height);
                node_control_scene_data.SetCentreX(left);
                node_control_scene_data.SetCentreY(top);

                return AddNewNodeControlSceneData(node_control_scene_data);
            }
        }

        private NodeControl AddNewNodeControlSceneData(NodeControlSceneData node_control_scene_data)
        {
            scene_changed_timestamp = DateTime.UtcNow;

            NodeControl nc = new NodeControl(this, node_control_scene_data);

            FrameworkElement node_content_control = null;
            try
            {
                node_content_control = NodeContentControlRegistry.Instance.GetContentControl(nc, node_control_scene_data.node_content);
            }
            catch (Exception ex)
            {
                // TODO Make a specific node control type for errors
                StringNodeContent snc = new StringNodeContent();
                snc.Text = String.Format("There was a problem while trying to load one of your nodes: {0}:\n{1}", node_control_scene_data.node_content, ex.Message);
                node_content_control = NodeContentControlRegistry.Instance.GetContentControl(nc, snc);
            }

            nc.NodeContentControl = node_content_control;

            // Get the control to measure itself based on its new bindings...
            node_content_control.UpdateLayout();

            // For some reason we have to do this or sometimes it has zero size :-(
            {
                Size INFINITE_SIZE = new Size(double.PositiveInfinity, double.PositiveInfinity);
                node_content_control.Measure(INFINITE_SIZE);
                node_content_control.UpdateLayout();
            }

            // Check that reasonable dimensions were provided
            if (node_control_scene_data.Width <= 0 || node_control_scene_data.Height <= 0)
            {
                Size INFINITE_SIZE = new Size(double.PositiveInfinity, double.PositiveInfinity);
                node_content_control.Measure(INFINITE_SIZE);

                node_control_scene_data.SetWidth(node_content_control.DesiredSize.Width / CurrentPowerScale / 2.0);
                node_control_scene_data.SetHeight(node_content_control.DesiredSize.Height / CurrentPowerScale / 2.0);

                if (0 == node_control_scene_data.Width)
                {
                    node_control_scene_data.SetWidth(20);
                }
                if (0 == node_control_scene_data.Height)
                {
                    node_control_scene_data.SetHeight(20);
                }
            }

            scene_changed_timestamp = DateTime.UtcNow;
            node_controls.Add(nc);

            nc.RecalculateChildDimension();

            return nc;
        }

        public BrainstormMetadata BrainstormMetadata
        {
            get
            {
                return brainstorm_metadata;
            }
        }

        #endregion

        #region --- Mouse management ------------------------------------------------------------------------------

        void SceneRenderingControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double delta_x = (e.PreviousSize.Width - e.NewSize.Width);
            double delta_y = (e.PreviousSize.Height - e.NewSize.Height);

            current_viewport_topleft.X += delta_x / 2.0 / CurrentPowerScale;
            current_viewport_topleft.Y += delta_y / 2.0 / CurrentPowerScale;
            ViewportHasChanged();

            RecalculateAllNodeControlDimensions();
        }

        internal void UpdateMouseTracking(MouseEventArgs e, bool update_initial)
        {
            UpdateMouseTracking(e.GetPosition(this), update_initial);
        }

        internal void UpdateMouseTracking(DragEventArgs e, bool update_initial)
        {
            UpdateMouseTracking(e.GetPosition(this), update_initial);
        }

        internal void UpdateMouseTracking(Point p, bool update_initial)
        {
            mouse_previous.X = mouse_current.X;
            mouse_previous.Y = mouse_current.Y;

            mouse_current.X = p.X;
            mouse_current.Y = p.Y;

            mouse_current_virtual.X = mouse_current.X / CurrentPowerScale + current_viewport_topleft.X;
            mouse_current_virtual.Y = mouse_current.Y / CurrentPowerScale + current_viewport_topleft.Y;

            mouse_delta_virtual.X = (mouse_current.X - mouse_previous.X) / CurrentPowerScale;
            mouse_delta_virtual.Y = (mouse_current.Y - mouse_previous.Y) / CurrentPowerScale;

            if (update_initial)
            {
                mouse_initial.X = p.X;
                mouse_initial.Y = p.Y;

                mouse_initial_virtual.X = mouse_initial.X / CurrentPowerScale + current_viewport_topleft.X;
                mouse_initial_virtual.Y = mouse_initial.Y / CurrentPowerScale + current_viewport_topleft.Y;
            }
        }

        void SceneRenderingControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double delta = Math.Sign(e.Delta);
            DoViewportScale_AroundMouse(delta);
            e.Handled = true;
        }

        void SceneRenderingControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();

            UpdateMouseTracking(e, true);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Deselect any existing node or edge
                SetSelectedNodeControl(null, IsUserIndicatingAddToSelection());
                selected_connector_control.Selected = null;

                if (false) { }

                else if (next_click_mode == NextClickMode.Hand)
                {
                    if (KeyboardTools.IsShiftDown())
                    {
                        mouse_drag_mode = MouseDragMode.Select;

                        Canvas.SetLeft(selecting_nodes_control, Math.Min(mouse_initial.X, mouse_current.X));
                        Canvas.SetTop(selecting_nodes_control, Math.Min(mouse_initial.Y, mouse_current.Y));
                        selecting_nodes_control.Width = Math.Abs(mouse_initial.X - mouse_current.X);
                        selecting_nodes_control.Height = Math.Abs(mouse_initial.Y - mouse_current.Y);

                        selecting_nodes_control.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mouse_drag_mode = MouseDragMode.Pan;
                    }
                    
                    this.CaptureMouse();
                    e.Handled = true;
                }

                else if (next_click_mode == NextClickMode.AddText)
                {
                    StringNodeContent new_node = new StringNodeContent();
                    new_node.Text = StringNodeContent.DEFAULT_NODE_CONTENT;
                    NodeControl new_node_control = AddNewNodeControl(new_node, mouse_current_virtual.X, mouse_current_virtual.Y);
                    
                    this.SetSelectedNodeControl(new_node_control, false);
                    new_node_control.AttempToEnterEditMode();

                    e.Handled = true;
                }

                else
                {
                    Logging.Warn("Unknown NextClickMode {0}", next_click_mode);
                }
            }
        }

        void SceneRenderingControl_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateMouseTracking(e, false);

            if (next_click_mode == NextClickMode.Hand)
            {
                if (false) { }
                else if (MouseDragMode.Pan == mouse_drag_mode)
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                    {
                        double delta = Math.Sign(mouse_delta_virtual.Y);
                        DoViewportScale_AroundMouse(delta / 2.0);
                        e.Handled = true;
                    }
                    else
                    {
                        current_viewport_topleft.X -= mouse_delta_virtual.X;
                        current_viewport_topleft.Y -= mouse_delta_virtual.Y;
                        ViewportHasChanged();
                        e.Handled = true;
                    }

                    RecalculateAllNodeControlDimensions();
                }
                else if (MouseDragMode.Select == mouse_drag_mode)
                {
                    Canvas.SetLeft(selecting_nodes_control, Math.Min(mouse_initial.X, mouse_current.X));
                    Canvas.SetTop(selecting_nodes_control, Math.Min(mouse_initial.Y, mouse_current.Y));
                    selecting_nodes_control.Width = Math.Abs(mouse_initial.X - mouse_current.X);
                    selecting_nodes_control.Height = Math.Abs(mouse_initial.Y - mouse_current.Y);
                }
            }

            e.Handled = true;
        }

        void SceneRenderingControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UpdateMouseTracking(e, false);

            if (false) { }

            else if (next_click_mode == NextClickMode.Hand)
            {
                // Have they selected a whole lot of nodes?
                if (MouseDragMode.Select == mouse_drag_mode)
                {                    
                    selecting_nodes_control.Visibility = Visibility.Collapsed;

                    double min_x = Math.Min(mouse_current_virtual.X, mouse_initial_virtual.X);
                    double max_x = Math.Max(mouse_current_virtual.X, mouse_initial_virtual.X);
                    double min_y = Math.Min(mouse_current_virtual.Y, mouse_initial_virtual.Y);
                    double max_y = Math.Max(mouse_current_virtual.Y, mouse_initial_virtual.Y);

                    // Find all the nodes in the selected region
                    List<NodeControl> newly_selected_node_controls = new List<NodeControl>();
                    foreach (NodeControl node_control in node_controls)
                    {
                        if (
                            true
                            && !node_control.NodeControlSceneData.Deleted
                            && node_control.NodeControlSceneData.CentreX - node_control.NodeControlSceneData.Width / 2 >= min_x
                            && node_control.NodeControlSceneData.CentreX + node_control.NodeControlSceneData.Width / 2 <= max_x
                            && node_control.NodeControlSceneData.CentreY + node_control.NodeControlSceneData.Height / 2 >= min_y
                            && node_control.NodeControlSceneData.CentreY + node_control.NodeControlSceneData.Height /2 <= max_y
                            )
                        {
                            newly_selected_node_controls.Add(node_control);
                        }
                    }

                    // Add the newly selected nodes...
                    if (0 < newly_selected_node_controls.Count)
                    {   
                        this.SetSelectedNodeControls(newly_selected_node_controls, IsUserIndicatingAddToSelection());
                        this.selected_connector_control.Selected = null;
                    }
                }
                
                mouse_drag_mode = MouseDragMode.None;

                this.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        #endregion

        internal void RecalculateNodeControlDimensions(NodeControl nc)
        {
            nc.RecalculateChildDimension();
        }

        public void RecalculateAllNodeControlDimensions()
        {
            foreach (var x in this.node_controls)
            {
                NodeControl nc = x as NodeControl;
                if (null != nc)
                {
                    nc.RecalculateChildDimension();
                }
            }
        }

        #region --- File management ------------------------------------------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the user proceeded with clearing the brainstorm.</returns>
        public void New()
        {
            New(false);
        }

        public void New(bool force_new)
        {
            if (!force_new &&  this.node_controls.Count > 0 && !MessageBoxes.AskQuestion("Are you sure you want to start a new brainstorm?  The existing brainstorm will be lost unless you have saved it."))
            {
                return;
            }

            ObjNodesLayer.Children.Clear();
            ObjControlsLayer.Children.Clear();

            scene_changed_timestamp = DateTime.UtcNow;
            this.node_controls.Clear();
            this.connector_control_manager.Clear();

            AddMetaControls();
            ResetViewport();            

            brainstorm_metadata = new BrainstormMetadata();
            this.DataContext = brainstorm_metadata.AugmentedBindable;
            if (null != brainstorm_metadata_control)
            {
                brainstorm_metadata_control.DataContext = brainstorm_metadata.AugmentedBindable;
            }
        }

        void SaveAsToDisk(string filename)
        {
            //FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_Save);

            List<NodeControlSceneData> node_control_scene_datas = new List<NodeControlSceneData>();
            foreach (NodeControl node_control in this.node_controls)
            {
                node_control_scene_datas.Add(node_control.NodeControlSceneData);
            }

            List<BrainstormFileFormat.ConnectorV1> connectors = new List<BrainstormFileFormat.ConnectorV1>();
            foreach (ConnectorControl connector_control in this.connector_control_manager.ConnectorControls)
            {
                BrainstormFileFormat.ConnectorV1 connector = new BrainstormFileFormat.ConnectorV1
                {
                    guid_connector = connector_control.guid,
                    guid_node_from = connector_control.node_from.scene_data.guid,
                    guid_node_to = connector_control.node_to.scene_data.guid,
                    deleted = connector_control.Deleted
                };

                connectors.Add(connector);
            }

            brainstorm_metadata.LastSaveDate = DateTime.UtcNow;
            brainstorm_metadata.LastOpenLocation = filename;
            brainstorm_metadata.AugmentedBindable.NotifyPropertyChanged(() => brainstorm_metadata.LastSaveDate);
            brainstorm_metadata.AugmentedBindable.NotifyPropertyChanged(() => brainstorm_metadata.LastOpenLocation);

            BrainstormFileFormat brainstorm_file_format = new BrainstormFileFormat();
            brainstorm_file_format.BrainstormMetadata = brainstorm_metadata;
            brainstorm_file_format.NodesV1 = node_control_scene_datas;
            brainstorm_file_format.ConnectorsV1 = connectors;
            brainstorm_file_format.CurrentViewport = new Point((current_viewport_topleft.X + current_viewport_bottomright.X) / 2.0, (current_viewport_topleft.Y + current_viewport_bottomright.Y) / 2.0);
            brainstorm_file_format.CurrentScale = CurrentScale;
            
            SerializeFile.Save(filename, brainstorm_file_format);
        }
        
        public void SaveToDisk()
        {
            if (!String.IsNullOrEmpty(brainstorm_metadata.LastOpenLocation))
            {
                SaveAsToDisk(brainstorm_metadata.LastOpenLocation);
            }
            else
            {
                SaveAsToDisk();
            }
        }

        public void SaveAsToDisk()
        {
            string filename = @"Untitled.brain";
            string path = "";
            if (null != brainstorm_metadata.LastOpenLocation && 0 != brainstorm_metadata.LastOpenLocation.Length)
            {
                filename = Path.GetFileName(brainstorm_metadata.LastOpenLocation);
                path = Path.GetDirectoryName(brainstorm_metadata.LastOpenLocation);
            }

            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.AddExtension = true;
            save_file_dialog.CheckPathExists = true;
            save_file_dialog.DereferenceLinks = true;
            save_file_dialog.OverwritePrompt = true;
            save_file_dialog.ValidateNames = true;
            save_file_dialog.DefaultExt = "brain";
            save_file_dialog.Filter = "Brainstorms (*.brain)|*.brain|All files (*.*)|*.*";
            save_file_dialog.FileName = filename;
            save_file_dialog.InitialDirectory = path;

            if (true == save_file_dialog.ShowDialog())
            {
                SaveAsToDisk(save_file_dialog.FileName);
            }
        }

        public void OpenFromDisk(string filename)
        {
            ObjControlsLayer.Children.Clear();
            ObjNodesLayer.Children.Clear();
            
            scene_changed_timestamp = DateTime.UtcNow;
            this.node_controls.Clear();
            this.connector_control_manager.Clear();

            AddMetaControls();
            ResetViewport();

            BrainstormFileFormat brainstorm_file_format = (BrainstormFileFormat)SerializeFile.Load(filename);

            Dictionary<Guid, NodeControl> node_control_lookup = new Dictionary<Guid, NodeControl>();

            brainstorm_metadata = brainstorm_file_format.BrainstormMetadata;
            if (null == brainstorm_metadata)
            {
                brainstorm_metadata = new BrainstormMetadata();
            }

            brainstorm_metadata.LastOpenLocation = filename;
            brainstorm_metadata_control.DataContext = brainstorm_metadata.AugmentedBindable;
            this.DataContext = brainstorm_metadata.AugmentedBindable;

            List<NodeControlSceneData> node_control_scene_datas = brainstorm_file_format.NodesV1;
            foreach (NodeControlSceneData node_control_scene_data in node_control_scene_datas)
            {
                NodeControl node_control = AddNewNodeControlSceneData(node_control_scene_data);
                node_control_lookup[node_control_scene_data.guid] = node_control;
            }

            List<BrainstormFileFormat.ConnectorV1> connectors = brainstorm_file_format.ConnectorsV1;
            foreach (BrainstormFileFormat.ConnectorV1 connector in connectors)
            {
                NodeControl node_control_from = node_control_lookup[connector.guid_node_from];
                NodeControl node_control_to = node_control_lookup[connector.guid_node_to];

                ConnectorControl connector_control = new ConnectorControl(this);
                connector_control.guid = connector.guid_connector;
                connector_control.Deleted = connector.deleted;
                connector_control.SetNodes(node_control_from, node_control_to);
                AddNewConnectorControl(connector_control);
            }

            // Return the screen to where last it was
            {
                CurrentScale = brainstorm_file_format.CurrentScale ?? 1.0;

                Point point = brainstorm_file_format.CurrentViewport ?? new Point();
                current_viewport_topleft.X = point.X - this.ActualWidth / CurrentPowerScale / 2.0;
                current_viewport_topleft.Y = point.Y - this.ActualHeight / CurrentPowerScale / 2.0;                

                ViewportHasChanged();
                RecalculateAllNodeControlDimensions();
            }
        }

        public void OpenFromDisk()
        {
            //FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_Open);

            if (this.node_controls.Count > 0 && !MessageBoxes.AskQuestion("Are you sure you want to open a different brainstorm?  The existing brainstorm will be cleared."))
            {
                return;
            }

            string filename = @"Untitled.brain";
            string path = "";
            if (null != brainstorm_metadata.LastOpenLocation && 0 != brainstorm_metadata.LastOpenLocation.Length)
            {
                filename = Path.GetFileName(brainstorm_metadata.LastOpenLocation);
                path = Path.GetDirectoryName(brainstorm_metadata.LastOpenLocation);
            }

            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.AddExtension = true;
            open_file_dialog.CheckPathExists = true;
            open_file_dialog.CheckFileExists = true;
            open_file_dialog.DereferenceLinks = true;
            open_file_dialog.ValidateNames = true;
            open_file_dialog.DefaultExt = "brain";
            open_file_dialog.Filter = "Brainstorms (*.brain)|*.brain|All files (*.*)|*.*";
            open_file_dialog.FileName = filename;
            open_file_dialog.InitialDirectory = path;

            if (true == open_file_dialog.ShowDialog())
            {
                OpenFromDisk(open_file_dialog.FileName);
            }
        }

        #endregion

        #region --- Searching ------------------------------------------------------------------------------------------------

        int last_search_child_offset = -1;

        public void Search(string keyword)
        {
            //FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_Search);

            // Must have a keyword
            if (null == keyword || 0 == keyword.Length)
            {
                return;
            }

            keyword = keyword.ToLower();
            List<Searchable> searchables = new List<Searchable>();

            // Work out where we are starting in the list
            int start_search_child_offset = last_search_child_offset + 1;
            if (start_search_child_offset < 0)
            {
                start_search_child_offset = 0;
            }
            if (start_search_child_offset > node_controls.Count)
            {
                start_search_child_offset = 0;
            }


            int first_matching_node_ever = -1;
            int first_matching_node_after_last = -1;

            List<NodeControl> matching_nodes = new List<NodeControl>();
            for (int i = 0; i < node_controls.Count; ++i)
            {
                NodeControl node_control = node_controls[i];
                Searchable searchable = node_control.scene_data.node_content as Searchable;
                if (null != searchable)
                {
                    if (searchable.MatchesKeyword(keyword))
                    {
                        // Remember the important last-search positions
                        if (-1 == first_matching_node_ever) first_matching_node_ever = i;
                        if (-1 == first_matching_node_after_last && i > last_search_child_offset) first_matching_node_after_last = i;

                        // This is one of the search results
                        matching_nodes.Add(node_control);
                    }
                }
            }

            // Which is the winning node control
            if (-1 != first_matching_node_ever)
            {
                last_search_child_offset = first_matching_node_ever;
            }
            if (-1 != first_matching_node_after_last)
            {
                last_search_child_offset = first_matching_node_after_last;
            }

            // Focus on the matching node
            if (-1 != last_search_child_offset)
            {
                NodeControl node_control = node_controls[last_search_child_offset];

                // Get the min dimension to be about 100 in size
                double extent = Math.Min(node_control.scene_data.Width, node_control.scene_data.Height);
                CurrentPowerScale = 50 / extent;

                current_viewport_topleft.X = node_control.scene_data.CentreX - this.ActualWidth / CurrentPowerScale / 2.0;
                current_viewport_topleft.Y = node_control.scene_data.CentreY - this.ActualHeight / CurrentPowerScale / 2.0;
                ViewportHasChanged();
                RecalculateAllNodeControlDimensions();
            }

            // Select the matching nodes
            if (true)
            {
                SetSelectedNodeControls(matching_nodes, false);
            }
        }

        #endregion

        #region --- Zooming and scaling ------------------------------------------------------------------------------------

        internal void ZoomIn()
        {
            DoViewportScale_AroundScreenCentre(+1);            
        }

        internal void ZoomOut()
        {
            DoViewportScale_AroundScreenCentre(-1);
        }


        void DoViewportScale(double direction, double focus_left, double focus_top)
        {
            double delta_viewport_x = focus_left / CurrentPowerScale + current_viewport_topleft.X;
            double delta_viewport_y = focus_top / CurrentPowerScale + current_viewport_topleft.Y;

            CurrentScale += direction;

            current_viewport_topleft.X = delta_viewport_x - focus_left / CurrentPowerScale;
            current_viewport_topleft.Y = delta_viewport_y - focus_top / CurrentPowerScale;

            ViewportHasChanged();
        }


        void DoViewportScale_AroundScreenCentre(double direction)
        {
            double screen_centre_left_current = this.ActualWidth / 2.0;
            double screen_centre_top_current = this.ActualHeight / 2.0;

            DoViewportScale(direction, screen_centre_left_current, screen_centre_top_current);

            RecalculateAllNodeControlDimensions();
        }

        void DoViewportScale_AroundMouse(double direction)
        {
            DoViewportScale(direction, mouse_current.X, mouse_current.Y);

            mouse_current_virtual.X = mouse_current.X / CurrentPowerScale + current_viewport_topleft.X;
            mouse_current_virtual.Y = mouse_current.Y / CurrentPowerScale + current_viewport_topleft.Y;

            RecalculateAllNodeControlDimensions();
        }

        #endregion

        #region --- Next click mode ---------------------------------------------------------------------------------

        internal enum NextClickMode
        {
            Hand,
            AddText
        }

        NextClickMode next_click_mode = NextClickMode.Hand;

        internal void SetNextClickMode(NextClickMode next_click_mode)
        {
            this.next_click_mode = next_click_mode;

            if (false) { }
            else if (next_click_mode == NextClickMode.Hand) this.Cursor = Cursors.Hand;
            else if (next_click_mode == NextClickMode.AddText) this.Cursor = Cursors.Pen;
        }

        #endregion ---------------------------------------------------------------------------------

        #region --- Mouse drag mode ---------------------------------------------------------------------------------

        internal enum MouseDragMode
        {
            None,            
            Select,
            Pan
        }

        MouseDragMode mouse_drag_mode = MouseDragMode.None;

        #endregion ---------------------------------------------------------------------------------


        #region --- Selecting nodes --------------------------------------

        private List<SelectedNodeControl> selected_node_controls = new List<SelectedNodeControl>();

        internal void SelectAll()
        {
            SetSelectedNodeControls(node_controls, false);
        }


        internal List<NodeControl> GetSelectedNodeControls()
        {
            List<NodeControl> results = new List<NodeControl>();
            selected_node_controls.ForEach(o => results.Add(o.Selected));
            return results;
        }

        internal bool IsSelectedNodeControl(NodeControl target_node_control)
        {
            foreach (SelectedNodeControl selected_node_control in selected_node_controls)
            {
                if (target_node_control == selected_node_control.Selected) return true;
            }

            return false;
        }

        internal void SetSelectedNodeControl(NodeControl node_new, bool add_to_selection)
        {
            if (null != node_new)
            {
                List<NodeControl> nodes_new = new List<NodeControl>();
                nodes_new.Add(node_new);
                SetSelectedNodeControls(nodes_new, add_to_selection);
            }
            else
            {
                SetSelectedNodeControls(null, add_to_selection);
            }
        }

        internal void SetSelectedNodeControls(List<NodeControl> nodes_new, bool add_to_selection)
        {
            // Make sure we are working with something - even if they pass in null
            if (null == nodes_new)
            {
                nodes_new = new List<NodeControl>();
            }

            // Obliterate the old selected nodes (if we are not adding to the selection)
            if (!add_to_selection)
            {
                foreach (SelectedNodeControl selected_node_control in selected_node_controls)
                {
                    ObjControlsLayer.Children.Remove(selected_node_control);
                    selected_node_control.Selected = null;
                }
                selected_node_controls.Clear();
            }

            // Create the new nodes
            foreach (NodeControl node_new in nodes_new)
            {
                if (!IsSelectedNodeControl(node_new))
                {
                    SelectedNodeControl selected_node_control = new SelectedNodeControl(this);
                    ObjControlsLayer.Children.Add(selected_node_control);
                    selected_node_controls.Add(selected_node_control);
                    selected_node_control.Selected = node_new;
                }
            }

            // Indicate that the selection has changed...
            if (0 < nodes_new.Count())
            {
                NotifySelectedNodeControlChanged(nodes_new[0]);
            }
            else
            {
                NotifySelectedNodeControlChanged(null);
            }
        }

        internal void RemoveSelectedNodeControl(NodeControl node_control)
        {
            for (int i = selected_node_controls.Count-1; i >= 0 ; --i)                
            {
                SelectedNodeControl selected_node_control = selected_node_controls[i];
                if (selected_node_control.Selected == node_control)
                {   
                    ObjControlsLayer.Children.Remove(selected_node_control);
                    selected_node_controls.RemoveAt(i);
                    selected_node_control.Selected = null;
                }
            }
        }

        internal void NotifySelectedNodeControlChanged(NodeControl node_control)
        {
            if (null != SelectedNodeControlChanged)
            {
                SelectedNodeControlChanged(node_control);
            }
        }

        internal bool IsUserIndicatingAddToSelection()
        {
            return ((Keyboard.Modifiers & ModifierKeys.Shift) > 0);
        }

        #endregion

        #region --- IDisposable ------------------------------------------------------------------------

        ~SceneRenderingControl()
        {
            Logging.Info("~SceneRenderingControl()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing SceneRenderingControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        private void Dispose(bool disposing)
        {
            Logging.Debug("SceneRenderingControl::Dispose({0}) @{1}", disposing ? "true" : "false", ++dispose_count);
            if (disposing)
            {
                // Get rid of managed resources
                this.AutoArranger.Enabled(false);

                node_controls.Clear();
            }

            brainstorm_metadata_control = null;
            brainstorm_metadata = null;

            auto_arranger = null;
            drag_drop_manager = null;
            basic_drag_drop_behaviours = null;

            selected_connector_control = null;
            selecting_nodes_control = null;

            node_controls.Clear();
            connector_control_manager = null;

            // Get rid of unmanaged resources 
        }

        #endregion
    }
}
