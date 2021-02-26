using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Documents.PDF;
using Syncfusion.Windows.Chart;
using Utilities;
using Utilities.GUI;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Expedition
{
    /// <summary>
    /// Interaction logic for ExpeditionPaperThemesControl.xaml
    /// </summary>
    public partial class ExpeditionPaperThemesControl : UserControl
    {
        public ExpeditionPaperThemesControl()
        {
            InitializeComponent();

            DataContextChanged += ExpeditionPaperThemesControl_DataContextChanged;

            // This is a bid to stop the exception happening when you double-click
            ChartTopics.PreviewMouseDown += ChartTopics_PreviewMouseDown;
        }

        private void ChartTopics_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Logging.Info("Ignoring mouse down on chart");
            e.Handled = true;
        }

        private void ExpeditionPaperThemesControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                // Clear the old
                ObjSeriesTopics.DataSource = null;
                TxtPleaseRunExpedition.Visibility = Visibility.Visible;
                ChartTopics.Visibility = Visibility.Collapsed;

                AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
                if (null == pdf_document_bindable)
                {
                    return;
                }

                PDFDocument pdf_document = pdf_document_bindable.Underlying;

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    ExpeditionDataSource eds = pdf_document.LibraryRef.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;

                    if (null == eds)
                    {
                        return;
                    }
                    else
                    {
                        LDAAnalysis lda_analysis = eds.LDAAnalysis;

                        // Draw the pie chart
                        try
                        {
                            if (!eds.docs_index.ContainsKey(pdf_document.Fingerprint))
                            {
                                return;
                            }

                            int doc_id = eds.docs_index[pdf_document.Fingerprint];
                            TopicProbability[] topics = lda_analysis.DensityOfTopicsInDocsSorted[doc_id];

                            int ITEMS_IN_CHART = Math.Min(topics.Length, 3);

                            WPFDoEvents.InvokeAsyncInUIThread(() =>
                            {
                                Brush[] brushes = new Brush[ITEMS_IN_CHART + 1];

                                List<ChartItem> chart_items = new List<ChartItem>();
                                double remaining_segment_percentage = 1.0;
                                for (int t = 0; t < ITEMS_IN_CHART; ++t)
                                {
                                    string topic_name = eds.GetDescriptionForTopic(topics[t].topic);
                                    double percentage = topics[t].prob;

                                    chart_items.Add(new ChartItem { Topic = topic_name, Percentage = percentage });
                                    brushes[t] = new SolidColorBrush(eds.Colours[topics[t].topic]);

                                    remaining_segment_percentage -= percentage;
                                }

                                chart_items.Add(new ChartItem { Topic = "Others", Percentage = remaining_segment_percentage });
                                brushes[ITEMS_IN_CHART] = new SolidColorBrush(Colors.White);

                                ObjChartTopicsArea.ColorModel.CustomPalette = brushes;
                                ObjChartTopicsArea.ColorModel.Palette = ChartColorPalette.Custom;
                                ObjSeriesTopics.DataSource = chart_items;

                                // Silly
                                ObjSeriesTopics.AnimationDuration = TimeSpan.FromMilliseconds(1000);
                                ObjSeriesTopics.EnableAnimation = false;
                                ObjSeriesTopics.AnimateOneByOne = true;
                                ObjSeriesTopics.AnimateOption = AnimationOptions.Fade;
                                ObjSeriesTopics.EnableAnimation = true;

                                TxtPleaseRunExpedition.Visibility = Visibility.Collapsed;
                                ChartTopics.Visibility = Visibility.Visible;
                            });
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "There was a problem while generating the topics chart for document {0}", pdf_document.Fingerprint);
                        }
                    }
                });
            });
        }
    }

    public class ChartItem
    {
        public string Topic { get; set; }
        public double Percentage { get; set; }
    }

    public class ChartLabelConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChartItem chart_item = value as ChartItem;
            if (null != chart_item)
            {
                return chart_item.Topic;
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
