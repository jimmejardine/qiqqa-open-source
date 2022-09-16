using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using icons;
using Microsoft.Win32;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Syncfusion.Windows.Controls.Grid;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Mathematics;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.LibraryPivotReport
{
    /// <summary>
    /// Interaction logic for LibraryPivotReportControl.xaml
    /// </summary>
    public partial class LibraryPivotReportControl : UserControl
    {
        public WebLibraryDetail LibraryRef { get; set; }
        public List<PDFDocument> PDFDocuments { get; set; }

        private LibraryPivotReportBuilder.PivotResult last_pivot_result = null;
        private GridControl last_ObjGridControl = null;

        public LibraryPivotReportControl()
        {
            InitializeComponent();

            ButtonRegenerate.Icon = Icons.GetAppIcon(Icons.Refresh);
            ButtonRegenerate.Caption = "Regenerate";
            ButtonRegenerate.Click += ButtonRegenerate_Click;

            ButtonExportExcel.Icon = Icons.GetAppIcon(Icons.PivotExportToExcel);
            ButtonExportExcel.Caption = "Export\nto Excel";
            ButtonExportExcel.Click += ButtonExportExcel_Click;

            ObjYAxis.ItemsSource = LibraryPivotReportBuilder.AXIS_CHOICES;
            ObjYAxis.SelectedIndex = 4;
            ObjXAxis.ItemsSource = LibraryPivotReportBuilder.AXIS_CHOICES;
            ObjXAxis.SelectedIndex = 1;
            ObjIdentifier.ItemsSource = LibraryPivotReportBuilder.IDENTIFIER_CHOICES;
            ObjIdentifier.SelectedIndex = 0;
        }

        private void ButtonExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (null == last_ObjGridControl)
            {
                MessageBoxes.Error("Please regenerate your Pivot before trying to export it.");
                return;
            }


            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.AddExtension = true;
            save_file_dialog.CheckPathExists = true;
            save_file_dialog.DereferenceLinks = true;
            save_file_dialog.OverwritePrompt = true;
            save_file_dialog.ValidateNames = true;
            save_file_dialog.DefaultExt = "csv";
            save_file_dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            save_file_dialog.FileName = "QiqqaLibraryPivot.csv";

            if (true == save_file_dialog.ShowDialog())
            {
                string target_filename = save_file_dialog.FileName;
                ExportToExcel(target_filename);
            }
        }


        private void ExportToExcel(string filename)
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < last_ObjGridControl.Model.RowCount; ++y)
            {
                for (int x = 0; x < last_ObjGridControl.Model.ColumnCount; ++x)
                {
                    object value = last_ObjGridControl.Model[y, x].CellValue;
                    sb.Append('"');
                    sb.Append(value);
                    sb.Append('"');
                    sb.Append(',');
                }

                sb.Append("\r\n");
            }

            File.WriteAllText(filename, sb.ToString());
            Process.Start(filename);
        }

        private void ButtonRegenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Regenerate();
            }
            catch (OutOfMemoryException)
            {
                MessageBoxes.Error("The pivot that you have requested requires too much memory to be calculated.  Please try a smaller analysis.");
            }
        }

        private void Regenerate()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            HashSet<string> parent_fingerprints = null;
            if (null != PDFDocuments && 0 < PDFDocuments.Count)
            {
                parent_fingerprints = new HashSet<string>();
                foreach (var pdf_document in PDFDocuments)
                {
                    parent_fingerprints.Add(pdf_document.Fingerprint);
                }
            }

            string map_y_axis_name = (string)ObjYAxis.SelectedItem;
            string map_x_axis_name = (string)ObjXAxis.SelectedItem;
            string identifier_name = (string)ObjIdentifier.SelectedItem;

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                MultiMapSet<string, string> map_y_axis = LibraryPivotReportBuilder.GenerateAxisMap(map_y_axis_name, LibraryRef, parent_fingerprints);
                MultiMapSet<string, string> map_x_axis = LibraryPivotReportBuilder.GenerateAxisMap(map_x_axis_name, LibraryRef, parent_fingerprints);

                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    LibraryPivotReportBuilder.IdentifierImplementations.IdentifierImplementationDelegate identifier_implementation = LibraryPivotReportBuilder.IdentifierImplementations.GetIdentifierImplementation(identifier_name);

                    LibraryPivotReportBuilder.PivotResult pivot_result = LibraryPivotReportBuilder.GeneratePivot(map_y_axis, map_x_axis);

                    GridControl ObjGridControl = new GridControl();
                    ObjGridControlHolder.Content = ObjGridControl;
                    ObjGridControl.Model.RowCount = map_y_axis.Count + 2;
                    ObjGridControl.Model.ColumnCount = map_x_axis.Count + 2;

                    // ROW/COLUMN Titles
                    for (int y = 0; y < pivot_result.y_keys.Count; ++y)
                    {
                        ObjGridControl.Model[y + 1, 0].CellValue = pivot_result.y_keys[y];
                        ObjGridControl.Model[y + 1, 0].CellValueType = typeof(string);
                    }
                    for (int x = 0; x < pivot_result.x_keys.Count; ++x)
                    {
                        ObjGridControl.Model[0, x + 1].CellValue = pivot_result.x_keys[x];
                        ObjGridControl.Model[0, x + 1].CellValueType = typeof(string);
                    }

                    // Grid contents
                    StatusManager.Instance.ClearCancelled("LibraryPivot");
                    for (int y = 0; y < pivot_result.y_keys.Count; ++y)
                    {
                        if (General.HasPercentageJustTicked(y, pivot_result.y_keys.Count))
                        {
                            StatusManager.Instance.UpdateStatus("LibraryPivot", "Building library pivot grid", y, pivot_result.y_keys.Count, true);

                            if (StatusManager.Instance.IsCancelled("LibraryPivot"))
                            {
                                Logging.Warn("User cancelled library pivot grid generation");
                                break;
                            }
                        }

                        for (int x = 0; x < pivot_result.x_keys.Count; ++x)
                        {
                            identifier_implementation(LibraryRef, pivot_result.common_fingerprints[y, x], ObjGridControl.Model[y + 1, x + 1]);
                        }
                    }
                    StatusManager.Instance.UpdateStatus("LibraryPivot", "Finished library pivot");

                    // ROW/COLUMN Totals
                    {
                        int y_total = 0;
                        {
                            for (int y = 0; y < pivot_result.y_keys.Count; ++y)
                            {
                                int total = 0;
                                for (int x = 0; x < pivot_result.x_keys.Count; ++x)
                                {
                                    if (null != pivot_result.common_fingerprints[y, x])
                                    {
                                        total += pivot_result.common_fingerprints[y, x].Count;
                                    }
                                }

                                ObjGridControl.Model[y + 1, pivot_result.x_keys.Count + 1].CellValue = total;
                                ObjGridControl.Model[y + 1, pivot_result.x_keys.Count + 1].CellValueType = typeof(int);

                                y_total += total;
                            }
                        }

                        int x_total = 0;
                        {
                            for (int x = 0; x < pivot_result.x_keys.Count; ++x)
                            {
                                int total = 0;
                                for (int y = 0; y < pivot_result.y_keys.Count; ++y)
                                {
                                    if (null != pivot_result.common_fingerprints[y, x])
                                    {
                                        total += pivot_result.common_fingerprints[y, x].Count;
                                    }
                                }

                                ObjGridControl.Model[pivot_result.y_keys.Count + 1, x + 1].CellValue = total;
                                ObjGridControl.Model[pivot_result.y_keys.Count + 1, x + 1].CellValueType = typeof(int);

                                x_total += total;
                            }
                        }

                        int common_total = (x_total + y_total) / 2;
                        if (common_total != x_total || common_total != y_total) throw new GenericException("X and Y totals do not match?!");
                        ObjGridControl.Model[pivot_result.y_keys.Count + 1, pivot_result.x_keys.Count + 1].CellValue = common_total;
                        ObjGridControl.Model[pivot_result.y_keys.Count + 1, pivot_result.x_keys.Count + 1].CellValueType = typeof(int);

                        ObjGridControl.Model[0, pivot_result.x_keys.Count + 1].CellValue = "TOTAL";
                        ObjGridControl.Model[0, pivot_result.x_keys.Count + 1].CellValueType = typeof(string);
                        ObjGridControl.Model[pivot_result.y_keys.Count + 1, 0].CellValue = "TOTAL";
                        ObjGridControl.Model[pivot_result.y_keys.Count + 1, 0].CellValueType = typeof(string);
                    }

                    // Store the results for the toolbar buttons
                    last_pivot_result = pivot_result;
                    last_ObjGridControl = ObjGridControl;
                });
            });
        }
    }
}
