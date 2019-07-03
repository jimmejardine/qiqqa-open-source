using System.Windows.Controls;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for TagsControl.xaml
    /// </summary>
    public partial class TagsControl : UserControl
    {
        public TagsControl()
        {
            InitializeComponent();

            ObjTagEditorControl.TagFeature_Add = Features.Document_AddTag;
            ObjTagEditorControl.TagFeature_Remove = Features.Document_RemoveTag;
        }
    }
}
