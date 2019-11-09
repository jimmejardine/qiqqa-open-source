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
        private ConnectorControl connector_control = null;
        private ConnectorControl.DimensionsChangedDelegate SelectedConnector_OnDimensionsChangedDelegate;
        private ConnectorControl.DeletedDelegate SelectedConnector_OnDeletedDelegate;

        public SelectedConnectorControl()
        {
            InitializeComponent();

            SelectedConnector_OnDimensionsChangedDelegate = SelectedConnector_OnDimensionsChanged;
            SelectedConnector_OnDeletedDelegate = SelectedConnector_OnDeleted;

            KeyDown += SelectedConnectorControl_KeyDown;
            MouseDown += SelectedConnectorControl_MouseDown;
            Focusable = true;

            Visibility = Visibility.Collapsed;
        }

        private void SelectedConnectorControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Logging.Debug特("SelectedConnectorControl_MouseDown");
        }

        private void SelectedConnectorControl_KeyDown(object sender, KeyEventArgs e)
        {
            Logging.Debug特("SelectedConnectorControl_KeyDown");
        }

        public ConnectorControl Selected
        {
            get => connector_control;

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
                    Visibility = Visibility.Collapsed;
                }
                else
                {
                    SelectedConnector_OnDimensionsChanged(connector_control);
                    Visibility = Visibility.Visible;
                }
            }
        }

        private void SelectedConnector_OnDeleted(ConnectorControl cc)
        {
            Selected = null;
        }

        private void SelectedConnector_OnDimensionsChanged(ConnectorControl cc)
        {
            const double SPACER = 4;

            Width = cc.Width + SPACER + SPACER;
            Height = cc.Height + SPACER + SPACER;

            Canvas.SetLeft(this, Canvas.GetLeft(cc) - SPACER);
            Canvas.SetTop(this, Canvas.GetTop(cc) - SPACER);
            Canvas.SetZIndex(this, Canvas.GetZIndex(cc) + 1);
        }
    }
}
