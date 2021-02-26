using System;
using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.Brainstorm.Nodes;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.WebcastStuff;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for BrainstormControl.xaml
    /// </summary>
    public partial class BrainstormControl : UserControl, IDisposable
    {
        public BrainstormControl()
        {
            Logging.Debug("+BrainstormControl()");

            InitializeComponent();

            SceneRenderingControl.SceneRenderingControl_PostConstructor(ObjBrainstormMetadata);

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
            SceneRenderingControl.SelectedNodeControlChanged += ObjSceneRenderingControl_SelectedNodeControlChanged;

            Logging.Debug("-BrainstormControl()");
        }

        private void ButtonAutoArrange_Click(object sender, RoutedEventArgs e)
        {
            SceneRenderingControl.AutoArranger.Enabled(ButtonAutoArrange.IsChecked ?? false);
        }

        private void RadioNodeAdditionMode_Click(object sender, RoutedEventArgs e)
        {
            if (RadioNodeAdditionMode_Connect.IsChecked ?? false)
            {
                SceneRenderingControl.NodeAdditionPolicy = SceneRenderingControl.NodeAdditionPolicyEnum.AlwaysUseExisting;
            }
            else
            {
                SceneRenderingControl.NodeAdditionPolicy = SceneRenderingControl.NodeAdditionPolicyEnum.AlwaysCreateNew;
            }
        }

        private void TextBoxFind_OnHardSearch()
        {
            DoSearch();
            TextBoxFind.FocusSearchArea();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            DualTabControlArea.MakeActive("Help");
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
#if false
            PrintDialog print_dialog = new System.Windows.Controls.PrintDialog();
            if (print_dialog.ShowDialog() ?? false)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_Print);
                print_dialog.PrintVisual(SceneRenderingControl, "Qiqqa Brainstorm");
            }
#endif

            FrameworkElementPrinter.Print(SceneRenderingControl, "Qiqqa Brainstorm");
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            if (Runtime.IsRunningInVisualStudioDesigner) return;

            SceneRenderingControl.New();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (Runtime.IsRunningInVisualStudioDesigner) return;

            SceneRenderingControl.SaveToDisk();
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (Runtime.IsRunningInVisualStudioDesigner) return;

            SceneRenderingControl.SaveAsToDisk();
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Runtime.IsRunningInVisualStudioDesigner) return;

            SceneRenderingControl.OpenFromDisk();
        }

        private void ButtonNextClickMode_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ButtonHand)
            {
                ButtonHand.IsChecked = true;
                ButtonAddText.IsChecked = false;
                SceneRenderingControl.SetNextClickMode(SceneRenderingControl.NextClickMode.Hand);
            }
            else if (sender == ButtonAddText)
            {
                ButtonHand.IsChecked = false;
                ButtonAddText.IsChecked = true;
                SceneRenderingControl.SetNextClickMode(SceneRenderingControl.NextClickMode.AddText);
            }
            else
            {
                Logging.Warn("An unknown control called ButtonNextClickMode_Click: {0}", sender);
            }
        }

        private void ButtonAddConnector_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxes.Info("To connect two nodes in your brainstorm, (1) select the FROM node; (2) hold down ALT; (3) select the TO node.  They will become linked.  You can delete this link by selecting it and pressing DEL.");
        }

        #region --- Search --------------------------------------------------------------------------------------------------------

        private void DoSearch()
        {
            SceneRenderingControl.Search(TextBoxFind.Text);
            TextBoxFind.SelectAll();
        }

        #endregion

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            SceneRenderingControl.ZoomIn();
        }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            SceneRenderingControl.ZoomOut();
        }

        public void OpenSample()
        {
            SceneRenderingControl.OpenSample();
        }

        public SceneRenderingControl SceneRenderingControl
        {
            get => ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl;
            private set => ObjSceneRenderingControlScrollWrapper.ObjSceneRenderingControl = value;
        }

        private bool is_first_selection = true;

        private void ObjSceneRenderingControl_SelectedNodeControlChanged(NodeControl node_control)
        {
            WPFDoEvents.SafeExec(() =>
            {
                // The first time they select something, select the edit tab...
                if (is_first_selection)
                {
                    is_first_selection = false;
                    DualTabControlArea.MakeActive("Edit");
                    SceneRenderingControl.Focus();
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
            });
        }

        private void LoadEditorFrameworkElement(FrameworkElement fe)
        {
            ObjStackEditor.Children.Clear();
            ObjStackEditor.Children.Add(fe);
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~BrainstormControl()
        {
            Logging.Debug("~BrainstormControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing BrainstormControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("BrainstormControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        // Get rid of managed resources
                        SceneRenderingControl?.Dispose();
                    }
                    SceneRenderingControl = null;
                });

                ++dispose_count;
            });
        }

        #endregion

        public void AutoArrange(bool value)
        {
            ButtonAutoArrange.IsChecked = value;
            SceneRenderingControl.AutoArranger.Enabled(value);
        }
    }
}
