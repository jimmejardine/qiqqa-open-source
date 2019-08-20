using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Synchronisation.BusinessLogic;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Synchronisation.GUI
{
    /// <summary>
    /// Interaction logic for SyncControl.xaml
    /// </summary>
    public partial class SyncControl : StandardWindow
    {
        public static readonly string TITLE = "Web Library Sync";

        SyncControlGridItemSet sync_control_grid_item_set;        

        public SyncControl()
        {
            Theme.Initialize();

            InitializeComponent();

            this.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            MouseWheelDisabler.DisableMouseWheelForControl(GridLibraryGrid);

            this.Title = TITLE;
            this.Closing += SyncControl_Closing;
            this.KeyUp += SyncControl_KeyUp;

            ButtonSyncMetadata.Icon = Icons.GetAppIcon(Icons.SyncWithCloud);
            ButtonSyncDocuments.Icon = Icons.GetAppIcon(Icons.SyncPDFsWithCloud);

            ButtonSync.Icon = Icons.GetAppIcon(Icons.SyncWithCloud);
            ButtonSync.Caption = "Start sync";
            ButtonSync.Click += ButtonSync_Click;
            ButtonRefresh.Icon = Icons.GetAppIcon(Icons.Refresh);
            ButtonRefresh.Caption = "Refresh";
            ButtonRefresh.Click += ButtonRefresh_Click;
            ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            ButtonCancel.Caption = "Cancel sync";
            ButtonCancel.Click += ButtonCancel_Click;

            IsVisibleChanged += SyncControl_IsVisibleChanged;
        }
        
        void GRIDCHECKBOX_Checked(object sender, RoutedEventArgs e)
        {
            // THIS HACK IS NEEDED BECAUSE I DONT KNOW HOW TO GET THE CHECKBOX TO UPDATE ITS BINDINGS NICELY WITH A SINGLE CLICK :-(
            CheckBox cb = (CheckBox)sender;
            var a = cb.DataContext; cb.BindingGroup.CommitEdit();            
        }

        void SyncControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ButtonSync_Click(null, null);
                    e.Handled = true;
                    break;

                case Key.Escape:
                    ButtonCancel_Click(null, null);
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        void SyncControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // The guest area
            if (!ConfigurationManager.Instance.IsGuest)
            {
                RegionMustRegister.Visibility = Visibility.Collapsed;
            }
        }

        void HyperlinkPremium_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.GetPremiumUrl("SYNC_INSTRUCTIONS"));
        }
        
        void HyperlinkPremiumPlus_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.GetPremiumPlusUrl("SYNC_INSTRUCTIONS"));
        }

        void SyncControl_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
        }

        void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            LibrarySyncManager.Instance.Sync(sync_control_grid_item_set);
            if (!KeyboardTools.IsCTRLDown())
            {
                this.Close();
            }
        }

        void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LibrarySyncManager.Instance.RefreshSyncControl(sync_control_grid_item_set, this);
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal void SetSyncParameters(SyncControlGridItemSet sync_control_grid_item_set)
        {
            this.sync_control_grid_item_set = sync_control_grid_item_set;

            // Clear up
            GridLibraryGrid.ItemsSource = null;

            // Freshen up
            if (null != sync_control_grid_item_set)
            {
                // Populate the grid
                GridLibraryGrid.ItemsSource = sync_control_grid_item_set.grid_items;
            }
        }
    }
}
