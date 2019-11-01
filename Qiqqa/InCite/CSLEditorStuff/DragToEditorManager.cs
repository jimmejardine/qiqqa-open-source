using System.IO;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace Qiqqa.InCite.CSLEditorStuff
{
    internal class DragToEditorManager
    {
        public static void RegisterControl(TextEditor element)
        {
            element.AllowDrop = true;
            element.DragOver += OnDragOver;
            element.Drop += OnDrop;
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (false)
            {
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Link;
            }

            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (false) { }

            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);

                // If they have dragged and dropped a single directory
                TextEditor te = sender as TextEditor;
                te.Text = File.ReadAllText(filenames[0]);
            }

            e.Handled = true;
        }
    }
}
