using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using System.Collections.Generic;

namespace Znode.Libraries.DevExpress.Report
{
    public class ReportModel
    {
        public List<FilterModel> Filters { get; set; }

        public XtraReport ReportObject { get; set; }

        public string ReportName { get; set; }

        public List<WebDocumentViewerMenuItem> CustomButtons { get; set; }

        public string MustHideColumns { get; set; }
    }
}
