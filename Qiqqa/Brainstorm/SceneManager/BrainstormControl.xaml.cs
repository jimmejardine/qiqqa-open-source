using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.Brainstorm.Nodes;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.WebcastStuff;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for BrainstormControl.xaml
    /// </summary>
    public partial class BrainstormControl : UserControl, IDisposable
    {
        public BrainstormControl()
        {
            Logging.Info("+BrainstormControl()");

            InitializeComponent();

            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.SceneRenderingControl_PostConstructor(ObjBrainstormMetadata);

            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;

            if (!ADVANCED_MENUS) ButtonNew.Caption = "New\nBrainstorm";
            ButtonNew.Icon = Icons.GetAppIcon(Icons.New);
            ButtonNew.ToolTip = "Create a new brainstorm";
            ButtonNew.Click += ButtonNew_Click;
            if (!ADVANCED_MENUS) ButtonOpen.Caption = "Open\nBrainstorm";
            ButtonOpen.Icon = Icons.GetAppIcon(Icons.Open);
            ButtonOpen.ToolTip = "Open an existing brainstorm";
            ButtonOpen.Click += ButtonOpen_Click;
            if (!ADVANCED_MENUS) ButtonSave.Caption = "Save\nBrainstorm";
            ButtonSave.Icon = Icons.GetAppIcon(Icons.Save);
            ButtonSave.ToolTip = "Save this brainstorm";
            ButtonSave.Click += ButtonSave_Click;
            if (!ADVANCED_MENUS) ButtonSaveAs.Caption = "Save As\nBrainstorm";
            ButtonSaveAs.Icon = Icons.GetAppIcon(Icons.SaveAs);
            ButtonSaveAs.ToolTip = "Save this brainstorm as another name";
            ButtonSaveAs.Click += ButtonSaveAs_Click;
            TextBoxFind.ToolTip = "Enter your search words here and press <ENTER> to search this brainstorm";
            TextBoxFind.OnHardSearch += TextBoxFind_OnHardSearch;
            if (!ADVANCED_MENUS) ButtonPrint.Caption = "Print";
            ButtonPrint.Icon = Icons.GetAppIcon(Icons.Printer);
            ButtonPrint.ToolTip = "Print this brainstorm";
            ButtonPrint.Click += ButtonPrint_Click;
            if (!ADVANCED_MENUS) ButtonZoomIn.Caption = "Zoom\nIn";
            ButtonZoomIn.Icon = Icons.GetAppIcon(Icons.ZoomIn);
            ButtonZoomIn.ToolTip = "Zoom in";
            ButtonZoomIn.Click += ButtonZoomIn_Click;
            if (!ADVANCED_MENUS) ButtonZoomOut.Caption = "Zoom\nOut";
            ButtonZoomOut.Icon = Icons.GetAppIcon(Icons.ZoomOut);
            ButtonZoomOut.ToolTip = "Zoom out";
            ButtonZoomOut.Click += ButtonZoomOut_Click;
            if (!ADVANCED_MENUS) ButtonHelp.Caption = "Toggle\nHelp";
            ButtonHelp.Icon = Icons.GetAppIcon(Icons.Help);
            ButtonHelp.ToolTip = "Toggle help";
            ButtonHelp.Click += ButtonHelp_Click;

            if (!ADVANCED_MENUS) ButtonHand.Caption = "Move\n& Pan";
            ButtonHand.Icon = Icons.GetAppIcon(Icons.Hand);
            ButtonHand.ToolTip = "Move around brainstorm";
            ButtonHand.Click += ButtonNextClickMode_Click;
            if (!ADVANCED_MENUS) ButtonAddText.Caption = "Add\nNode";
            ButtonAddText.Icon = Icons.GetAppIcon(Icons.BrainstormAddText);
            ButtonAddText.ToolTip = "Add a text node";
            ButtonAddText.Click += ButtonNextClickMode_Click;
            if (!ADVANCED_MENUS) ButtonAddConnector.Caption = "Add\nConnector";
            ButtonAddConnector.Icon = Icons.GetAppIcon(Icons.BrainstormAddConnector);
            ButtonAddConnector.ToolTip = "Connect two nodes in this brainstorm";
            ButtonAddConnector.Click += ButtonAddConnector_Click;
            if (!ADVANCED_MENUS) ButtonAddImage.Caption = "Add\nImage";
            ButtonAddImage.Icon = Icons.GetAppIcon(Icons.BrainstormAddImage);
            ButtonAddImage.ToolTip = "Add an image node";
            ButtonAddImage.Click += ButtonNextClickMode_Click;

            if (!ADVANCED_MENUS) ButtonAutoArrange.Caption = "Auto\nArrange";
            ButtonAutoArrange.Icon = Icons.GetAppIcon(Icons.BrainstormAutoArrange);
            ButtonAutoArrange.ToolTip = "Automatically arrange your brainstorm.";
            ButtonAutoArrange.Click += ButtonAutoArrange_Click;
            
            RadioNodeAdditionMode_Connect.Click += RadioNodeAdditionMode_Click;
            RadioNodeAdditionMode_Create.Click += RadioNodeAdditionMode_Click;
            {
                string tooltip = "When exploring your library, you can choose either to:\n\treuse nodes that are already on your brainstorm (to see existing relationships); or to\n\tcreate duplicates (to explore your library deeply without getting too cluttered).";
                RadioNodeAdditionMode_Connect.ToolTip = tooltip;
                RadioNodeAdditionMode_Create.ToolTip = tooltip;
            }

            Webcasts.FormatWebcastButton(ButtonWebcast_Themes, Webcasts.BRAINSTORM_THEMES);
            if (!ADVANCED_MENUS) ButtonWebcast_Themes.Caption = "Tutorial 1\n";
            Webcasts.FormatWebcastButton(ButtonWebcast_Intro, Webcasts.BRAINSTORM);
            if (!ADVANCED_MENUS) ButtonWebcast_Intro.Caption = "Tutorial 2\n";

            // The tabs in the editor area
            DualTabControlArea.Children.Clear();
            DualTabControlArea.AddContent("Edit", "Edit", null, false, false, ObjStackEditor);
            DualTabControlArea.AddContent("Help", "Help", null, false, false, ObjBrainstormControlHelpWhenEmpty);
            DualTabControlArea.AddContent("Info", "Info", null, false, false, ObjBrainstormMetadata);
            DualTabControlArea.MakeActive("Help");
            LoadEditorFrameworkElement(ObjStackEditorHelpWhenEmpty);

            // Listen for node changes
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.SelectedNodeControlChanged += ObjSceneRenderingControl_SelectedNodeControlChanged;

            Logging.Info("-BrainstormControl()");
        }

        void ButtonAutoArrange_Click(object sender, RoutedEventArgs e)
        {
            SceneRenderingControl.AutoArranger.Enabled(ButtonAutoArrange.IsChecked ?? false);
        }

        void RadioNodeAdditionMode_Click(object sender, RoutedEventArgs e)
        {
            if (RadioNodeAdditionMode_Connect.IsChecked ?? false)
            {
                ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.NodeAdditionPolicy = SceneRenderingControl.NodeAdditionPolicyEnum.AlwaysUseExisting;
            }
            else
            {
                ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.NodeAdditionPolicy = SceneRenderingControl.NodeAdditionPolicyEnum.AlwaysCreateNew;
            }
        }

        void TextBoxFind_OnHardSearch()
        {
            DoSearch();
            TextBoxFind.FocusSearchArea();
        }

        void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            DualTabControlArea.MakeActive("Help");
        }

        void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            /*
            PrintDialog print_dialog = new System.Windows.Controls.PrintDialog();
            if (print_dialog.ShowDialog() ?? false)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_Print);
                print_dialog.PrintVisual(ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl, "Qiqqa Brainstorm");
            }
             * */

            FrameworkElementPrinter.Print(ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl, "Qiqqa Brainstorm");            
        }

        void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.New();
        }

        void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.SaveToDisk();
        }

        void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.SaveAsToDisk();
        }

        void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.OpenFromDisk();
        }

        void ButtonNextClickMode_Click(object sender, RoutedEventArgs e)
        {
            if (false) { }
            else if (sender == ButtonHand)
            {
                ButtonHand.IsChecked = true;
                ButtonAddText.IsChecked = false;
                ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.SetNextClickMode(SceneRenderingControl.NextClickMode.Hand);
            }
            else if (sender == ButtonAddText)
            {
                ButtonHand.IsChecked = false;
                ButtonAddText.IsChecked = true;
                ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.SetNextClickMode(SceneRenderingControl.NextClickMode.AddText);
            }
            else Logging.Warn("An unknown control called ButtonNextClickMode_Click: {0}", sender);
        }

        void ButtonAddConnector_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxes.Info("To connect two nodes in your brainstorm, (1) select the FROM node; (2) hold down ALT; (3) select the TO node.  They will become linked.  You can delete this link by selecting it and pressing DEL.");
        }

        #region --- Search --------------------------------------------------------------------------------------------------------

        void DoSearch()
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.Search(TextBoxFind.Text);
            TextBoxFind.SelectAll();
        }

        #endregion

        void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.ZoomIn();
        }

        void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.ZoomOut();
        }

        public void OpenSample()
        {
            ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.OpenSample();
        }

        public SceneRenderingControl SceneRenderingControl
        {
            get
            {
                return ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl;
            }
        }

        bool is_first_selection = true;
        void ObjSceneRenderingControl_SelectedNodeControlChanged(NodeControl node_control)
        {
            // The first time they select something, select the edit tab...
            if (is_first_selection)
            {
                is_first_selection = false;
                DualTabControlArea.MakeActive("Edit");
                ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl.Focus();
            }

            // Clear out the old selected node editor
            if (0 < ObjStackEditor.Children.Count)
            {
                FrameworkElement fe = ObjStackEditor.Children[0] as FrameworkElement;
                if (null != fe)
                {
                    fe.DataContext = null;
                }
                else
                {
                    Logging.Warn("It is strange that there was a non-FrameworkElement child of the brainstorm node editor");
                }
                ObjStackEditor.Children.Clear();
            }

            // Create a new selected node editor (if we have one)
            FrameworkElement fe_to_load = null;
            if (null != node_control)
            {
                fe_to_load = NodeContentControlRegistry.Instance.GetContentEditor(node_control, node_control.scene_data.node_content);
            }
            if (null == fe_to_load)
            {
                fe_to_load = ObjStackEditorHelpWhenEmpty;
            }
            LoadEditorFrameworkElement(fe_to_load);
        }

        void LoadEditorFrameworkElement(FrameworkElement fe)
        {
            ObjStackEditor.Children.Clear();
            ObjStackEditor.Children.Add(fe);
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~BrainstormControl()
        {
            Logging.Info("~BrainstormControl()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing BrainstormControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                this.SceneRenderingControl.Dispose();
            }

            // Get rid of unmanaged resources 
        }

        #endregion


        public bool AutoArrange
        {
            set
            {
                this.ButtonAutoArrange.IsChecked = value;
                this.SceneRenderingControl.AutoArranger.Enabled(value);
            }
        }
    }
}
