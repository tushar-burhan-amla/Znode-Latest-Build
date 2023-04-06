namespace Znode.Engine.WebStore.Models
{
    public class ToolMenuModel
    {
        public string ControlId { get; set; }
        public string Url { get; set; }
        public string DataToggleModel { get; set; }
        public string DataTarget { get; set; }
        public string JSFunctionName { get; set; }
        public bool? DataDisable { get; set; }
        public string DisplayText { get; set; }

        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}