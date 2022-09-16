using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utilities.Files;
using Utilities.GUI;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for FileSystemNodeContentControl.xaml
    /// </summary>
    public partial class FileSystemNodeContentControl : UserControl
    {
        private FileSystemNodeContent fsnc;

        public FileSystemNodeContentControl(NodeControl node_control, FileSystemNodeContent fsnc)
        {
            this.fsnc = fsnc;

            InitializeComponent();

            Focusable = true;

            Image.Source = FileTypeIconCache.Instance[fsnc.path];

            Image.ToolTip = fsnc.path;
            Image.Stretch = Stretch.Uniform;
            Image.HorizontalAlignment = HorizontalAlignment.Center;

            Text.Text = Path.GetFileName(fsnc.path);
            Text.HorizontalAlignment = HorizontalAlignment.Center;
            Text.TextTrimming = TextTrimming.CharacterEllipsis;

            MouseDoubleClick += FileSystemNodeContentControl_MouseDoubleClick;
        }

        private void FileSystemNodeContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(fsnc.path);
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex, "There was a problem launching your application");
            }
        }
    }
}
