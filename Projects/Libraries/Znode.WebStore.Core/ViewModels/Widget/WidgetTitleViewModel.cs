using System;
namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetTitleViewModel : BaseViewModel
    {
        public int WidgetTitleConfigurationId { get; set; }
        public int PortalId { get; set; }
        public int MappingId { get; set; }
        public string MediaPath { get; set; }
        public string WidgetTitle { get; set; }
        public string Url { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public bool IsActive { get; set; }

        public bool IsNewTab { get; set; }
    }
}