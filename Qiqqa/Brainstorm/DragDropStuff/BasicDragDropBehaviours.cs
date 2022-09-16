using System;
using System.Collections.Generic;
using System.Windows;
using Qiqqa.Brainstorm.Nodes;
using Qiqqa.Documents.PDF;
using Utilities.Misc;

namespace Qiqqa.Brainstorm.DragDropStuff
{
    /// <summary>
    /// Manages several of the Qiqqa-wide drag and drop behaviours that Qiqqa brainstorming supports by default.
    /// </summary>
    internal class BasicDragDropBehaviours
    {
        private WeakReference<DragDropManager> drag_drop_manager;

        internal BasicDragDropBehaviours(DragDropManager drag_drop_manager)
        {
            this.drag_drop_manager = new WeakReference<DragDropManager>(drag_drop_manager);
        }

        internal DragDropManager DragDropManager
        {
            get
            {
                if (drag_drop_manager != null && drag_drop_manager.TryGetTarget(out var control) && control != null)
                {
                    return control;

                }
                return null;
            }
        }

        internal void RegisterBehaviours()
        {
            DragDropManager?.RegisterDropType(typeof(PDFDocument), OnDrop_PDFDocument);
            DragDropManager?.RegisterDropType(typeof(List<PDFDocument>), OnDrop_PDFDocumentList);
            DragDropManager?.RegisterDropType(typeof(PDFAnnotation), OnDrop_PDFAnnotation);
        }

        private void OnDrop_PDFDocument(object drop_object, Point mouse_current_virtual)
        {
            PDFDocument pdf_document = (PDFDocument)drop_object;
            ASSERT.Test(pdf_document != null);

            PDFDocumentNodeContent document_node_content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.LibraryRef.Id);
            DragDropManager?.SceneRenderingControl?.AddNewNodeControl(document_node_content, mouse_current_virtual.X, mouse_current_virtual.Y);
        }

        private void OnDrop_PDFDocumentList(object drop_object, Point mouse_current_virtual)
        {
            List<PDFDocument> pdf_documents = (List<PDFDocument>)drop_object;
            ASSERT.Test(pdf_documents != null);

            List<object> node_contents = new List<object>();

            foreach (PDFDocument pdf_document in pdf_documents)
            {
                PDFDocumentNodeContent document_node_content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.LibraryRef.Id);
                node_contents.Add(document_node_content);
            }

            DragDropManager?.SceneRenderingControl?.AddNewNodeControls(node_contents, mouse_current_virtual.X, mouse_current_virtual.Y);

        }

        private void OnDrop_PDFAnnotation(object drop_object, Point mouse_current_virtual)
        {
            PDFAnnotation pdf_annotation = (PDFAnnotation)drop_object;
            ASSERT.Test(pdf_annotation != null);

            PDFAnnotationNodeContent panc = new PDFAnnotationNodeContent(null, pdf_annotation.DocumentFingerprint, pdf_annotation.Guid.Value);
            DragDropManager?.SceneRenderingControl?.AddNewNodeControl(panc, mouse_current_virtual.X, mouse_current_virtual.Y);
        }
    }
}
