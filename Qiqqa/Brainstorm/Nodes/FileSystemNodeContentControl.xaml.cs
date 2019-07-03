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
        FileSystemNodeContent fsnc;

        public FileSystemNodeContentControl(NodeControl node_control, FileSystemNodeContent fsnc)
        {
            this.fsnc = fsnc;

            InitializeComponent();

            this.Focusable = true;

            Image.Source = FileTypeIconCache.Instance[fsnc.path];

            this.Image.ToolTip = fsnc.path;
            this.Image.Stretch = Stretch.Uniform;
            this.Image.HorizontalAlignment = HorizontalAlignment.Center;

            this.Text.Text = Path.GetFileName(fsnc.path);
            this.Text.HorizontalAlignment = HorizontalAlignment.Center;
            this.Text.TextTrimming = TextTrimming.CharacterEllipsis;

            this.MouseDoubleClick += FileSystemNodeContentControl_MouseDoubleClick;
        }

        void FileSystemNodeContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
