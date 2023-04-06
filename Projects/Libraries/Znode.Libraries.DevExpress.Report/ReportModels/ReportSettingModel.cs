using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.DevExpress.Report
{
    public class ReportSettingModel
    {
        public string ReportSettingXML { get; set; }
        public string OperatorXML { get; set; }
        public bool DisplayMode { get; set; }
        public string StyleSheetXml { get; set; }
        public string DefaultLayoutXML { get; set; }
        public List<FilterModel> Filters { get; set; }
        public List<DevExpressReportParameterModel> MustShowColumns { get; set; }
        public List<DevExpressReportParameterModel> MustHideColumns { get; set; }
        public List<DevExpressReportParameterModel> PortalList { get; set; }
        public List<DevExpressReportParameterModel> WareHouseList { get; set; }
        public List<DevExpressReportParameterModel> OrderStatusList { get; set; }
        public List<DevExpressReportParameterModel> DiscountTypeList { get; set; }
        public string DefaultPriceRoundOff { get; set; }
    }
}
