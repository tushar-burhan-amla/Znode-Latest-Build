using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace Znode.Libraries.DevExpress.Report
{
    public class ReportExportOptionsHelper
    {

        public void SetDefaultExportOptionsSetting(XtraReport report)
        {
            //Set default setting for Xlsx file.
            report.ExportOptions.Xlsx.ExportMode = XlsxExportMode.SingleFile;
            report.ExportOptions.Xlsx.TextExportMode = TextExportMode.Value;
            report.ExportOptions.Xlsx.RawDataMode = false;
            report.ExportOptions.Xlsx.ShowGridLines = true;


            //Set default setting for Xls file.
            report.ExportOptions.Xls.ExportMode = XlsExportMode.SingleFile;
            report.ExportOptions.Xls.TextExportMode = TextExportMode.Value;
            report.ExportOptions.Xls.RawDataMode = false;
            report.ExportOptions.Xls.ShowGridLines = true;

            //Set default setting for csv file.
            report.ExportOptions.Csv.TextExportMode = TextExportMode.Value;
            report.ExportOptions.Csv.Separator = ",";

        }

    }
}
