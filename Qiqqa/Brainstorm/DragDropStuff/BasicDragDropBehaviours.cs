using System.Collections.Generic;
using System.Windows;
using Qiqqa.Brainstorm.Nodes;
using Qiqqa.Documents.PDF;

namespace Qiqqa.Brainstorm.DragDropStuff
{
    /// <summary>
    /// Manages several of the Qiqqa-wide drag and drop behaviours that Qiqqa brainstorming supports by default.
    /// </summary>
    class BasicDragDropBehaviours
    {
        DragDropManager drag_drop_manager;
        
        internal BasicDragDropBehaviours(DragDropManager drag_drop_manager)
        {
            this.drag_drop_manager = drag_drop_manager;
        }

        internal void RegisterBehaviours()
        {
            drag_drop_manager.RegisterDropType(typeof(PDFDocument), OnDrop_PDFDocument);
            drag_drop_manager.RegisterDropType(typeof(List<PDFDocument>), OnDrop_PDFDocumentList);
            drag_drop_manager.RegisterDropType(typeof(PDFAnnotation), OnDrop_PDFAnnotation);
        }

        void OnDrop_PDFDocument(object drop_object, Point mouse_current_virtual)
        {
            PDFDocument pdf_document = (PDFDocument)drop_object;
            PDFDocumentNodeContent document_node_content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.Library.WebLibraryDetail.Id);
            drag_drop_manager.SceneRenderingControl.AddNewNodeControl(document_node_content, mouse_current_virtual.X, mouse_current_virtual.Y);
        }

        void OnDrop_PDFDocumentList(object drop_object, Point mouse_current_virtual)
        {
            List<PDFDocument> pdf_documents = (List<PDFDocument>)drop_object;

            List<object> node_contents = new List<object>();

            foreach (PDFDocument pdf_document in pdf_documents)
            {
                PDFDocumentNodeContent document_node_content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.Library.WebLibraryDetail.Id);
                node_contents.Add(document_node_content);
            }

            drag_drop_manager.SceneRenderingControl.AddNewNodeControls(node_contents, mouse_current_virtual.X, mouse_current_virtual.Y);

        }

        void OnDrop_PDFAnnotation(object drop_object, Point mouse_current_virtual)
        {
            PDFAnnotation pdf_annotation = (PDFAnnotation)drop_object;
            PDFAnnotationNodeContent panc = new PDFAnnotationNodeContent(null, pdf_annotation.DocumentFingerprint, pdf_annotation.Guid.Value);
            drag_drop_manager.SceneRenderingControl.AddNewNodeControl(panc, mouse_current_virtual.X, mouse_current_virtual.Y);
        }
    }
}
