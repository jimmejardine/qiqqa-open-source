using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Brainstorm.Connectors;
using Utilities;

namespace Qiqqa.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for SelectedConnectorControl.xaml
    /// </summary>
    public partial class SelectedConnectorControl : UserControl    
    {
        ConnectorControl connector_control = null;

        ConnectorControl.DimensionsChangedDelegate SelectedConnector_OnDimensionsChangedDelegate;
        ConnectorControl.DeletedDelegate SelectedConnector_OnDeletedDelegate;

        public SelectedConnectorControl()
        {
            InitializeComponent();

            SelectedConnector_OnDimensionsChangedDelegate = SelectedConnector_OnDimensionsChanged;
            SelectedConnector_OnDeletedDelegate = SelectedConnector_OnDeleted;

            this.KeyDown += SelectedConnectorControl_KeyDown;
            this.MouseDown += SelectedConnectorControl_MouseDown;
            this.Focusable = true;

            this.Visibility = Visibility.Collapsed;
        }

        void SelectedConnectorControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Logging.Debug特("SelectedConnectorControl_MouseDown");
        }

        void SelectedConnectorControl_KeyDown(object sender, KeyEventArgs e)
        {
            Logging.Debug特("SelectedConnectorControl_KeyDown");
        }

        public ConnectorControl Selected
        {
            get
            {
                return connector_control;
            }

            set
            {
                // Do nothing if they are reselecting the same Connector control
                if (value == connector_control)
                {
                    return;
                }

                // Unselect the existing Connector
                if (null != connector_control)
                {
                    connector_control.OnDimensionsChanged -= SelectedConnector_OnDimensionsChangedDelegate;
                    connector_control.OnDeleted -= SelectedConnector_OnDeletedDelegate;
                    connector_control = null;
                }

                // Select the new Connector
                if (null != value)
                {
                    connector_control = value;
                    connector_control.OnDimensionsChanged += SelectedConnector_OnDimensionsChangedDelegate;
                    connector_control.OnDeleted += SelectedConnector_OnDeletedDelegate;
                }
                
                // Are we visible or not?
                if (null == connector_control)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    SelectedConnector_OnDimensionsChanged(connector_control);
                    this.Visibility = Visibility.Visible;
                }
            }
        }

        void SelectedConnector_OnDeleted(ConnectorControl cc)
        {
            this.Selected = null;
        }

        void SelectedConnector_OnDimensionsChanged(ConnectorControl cc)
        {
            double SPACER = 4;

            this.Width = cc.Width + SPACER + SPACER;
            this.Height = cc.Height + SPACER + SPACER;

            Canvas.SetLeft(this, Canvas.GetLeft(cc) - SPACER);
            Canvas.SetTop(this, Canvas.GetTop(cc) - SPACER);
            Canvas.SetZIndex(this, Canvas.GetZIndex(cc)+1);
        }
    }
}
