using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSWidgetsListViewModel : BaseViewModel
    {
        public List<CMSWidgetsViewModel> CMSWidgetsList { get; set; }

        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }

        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public CMSWidgetsListViewModel Widgets { get; set; }
        public string ParentContentPageSEOUrl { get; set; }
        public string PreviewUrl { get; set; }
        public string ProductionUrl { get; set; }
        public int MediaId { get; set; }
        public string MediaPath { get; set; }
    }
}