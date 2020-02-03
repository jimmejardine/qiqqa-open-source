using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.Exporting
{
    internal class BibTexExport
    {
        internal static void Export(List<PDFDocument> pdf_documents)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_BibTexExport);

            string initial_directory = null;
            if (null == initial_directory) initial_directory = Path.GetDirectoryName(ConfigurationManager.Instance.ConfigurationRecord.System_LastBibTexExportFile);
            if (null == initial_directory) initial_directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string initial_filename = null;
            if (null == initial_filename) initial_filename = Path.GetFileName(ConfigurationManager.Instance.ConfigurationRecord.System_LastBibTexExportFile);
            if (null == initial_filename) initial_filename = "Qiqqa2BibTexExport.bib";

            using (SaveFileDialog dlg = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = false,
                Filter = "BibTex Files|*.bib|All Files|*.*",
                InitialDirectory = initial_directory,
                FileName = initial_filename,
                Title = "Where would you like to save your exported BibTex file?"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Remember the filename for next time
                    string filename = dlg.FileName;
                    ConfigurationManager.Instance.ConfigurationRecord.System_LastBibTexExportFile = filename;
                    ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.System_LastBibTexExportFile));

                    LibraryExporter_BibTeX.ExportBibTeX(pdf_documents, filename, true);
                }
            }
        }
    }
}
