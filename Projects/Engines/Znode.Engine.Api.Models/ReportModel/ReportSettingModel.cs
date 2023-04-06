namespace Znode.Engine.Api.Models
{
    public class ReportSettingModel : BaseModel
    {
        public int ReportSettingId { get; set; }
        public string ReportCode { get; set; }
        public string SettingXML { get; set; }
        public bool DisplayMode { get; set; }
        public string StyleSheetXml { get; set; }
        public string DefaultLayoutXML { get; set; }
    }
}
