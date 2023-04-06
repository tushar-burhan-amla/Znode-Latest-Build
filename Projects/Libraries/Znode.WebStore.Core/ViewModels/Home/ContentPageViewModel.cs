using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ContentPageViewModel : BaseViewModel
    {
        public int ContentPageId { get; set; }
        public string FileName { get; set; }
        public int[] ProfileId { get; set; }
        public int PortalId { get; set; }
        public string PageTitle { get; set; }
        public string PageName { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int TextWidgetConfigurationId { get; set; }
        public int MappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int LocaleId { get; set; }
        public string WidgetsKey { get; set; }
        public string Text { get; set; }
        public string SEOTitle { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOUrl { get; set; }
        public string CanonicalURL { get; set; }
        public string RobotTag { get; set; }
    }
}