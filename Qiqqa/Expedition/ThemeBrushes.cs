using System.Windows.Media;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities.Mathematics.Topics.LDAStuff;

namespace Qiqqa.Expedition
{
    class ThemeBrushes
    {
        public static Brush UNKNOWN_BRUSH = Brushes.WhiteSmoke;

        public static Brush GetBrushForDocument(PDFDocument pdf_document)
        {
            int doc_id;
            int num_topics;
            float[] density_of_topics_in_document;
            GetDensityForDocument(pdf_document, out doc_id, out num_topics, out density_of_topics_in_document);
            if (null == density_of_topics_in_document)
            {
                return UNKNOWN_BRUSH;
            }

            return GetBrushForDistribution(pdf_document.Library, num_topics, density_of_topics_in_document);
        }
        
        public static Brush GetBrushForDistribution(Library library, int num_topics, float[] distribution)
        {
            Color[] colours = library.ExpeditionManager.ExpeditionDataSource.Colours;            
            
            int num_stops = 2 * num_topics;
            
            GradientStopCollection gradient_stop_collection = new GradientStopCollection(num_stops);
            double previous_offset = 0.0;
            for (int i = 0; i < num_topics; ++i)
            {
                gradient_stop_collection.Add(new GradientStop(colours[i], previous_offset));
                previous_offset += distribution[i];
                gradient_stop_collection.Add(new GradientStop(colours[i], previous_offset));
            }

            LinearGradientBrush lgb = new LinearGradientBrush(gradient_stop_collection);
            lgb.Freeze();

            return lgb;
        }

        private static void GetDensityForDocument(PDFDocument pdf_document, out int doc_id, out int num_topics, out float[] density_of_topics_in_document)
        {
            doc_id = -1;
            num_topics = -1;
            density_of_topics_in_document = null;

            if (null == pdf_document)
            {
                return;
            }
            
            if (null == pdf_document.Library.ExpeditionManager.ExpeditionDataSource)
            {
                return;
            }

            ExpeditionDataSource eds = pdf_document.Library.ExpeditionManager.ExpeditionDataSource;
            LDAAnalysis lda_analysis = eds.LDAAnalysis;
            if (!pdf_document.Library.ExpeditionManager.ExpeditionDataSource.docs_index.ContainsKey(pdf_document.Fingerprint))
            {
                return;
            }

            // Result!
            doc_id = pdf_document.Library.ExpeditionManager.ExpeditionDataSource.docs_index[pdf_document.Fingerprint];
            num_topics = lda_analysis.NUM_TOPICS;
            density_of_topics_in_document = new float[num_topics];
            for (int i = 0; i < lda_analysis.NUM_TOPICS; ++i)
            {
                density_of_topics_in_document[i] = lda_analysis.DensityOfTopicsInDocuments[doc_id, i];
            }
        }
    }
}
