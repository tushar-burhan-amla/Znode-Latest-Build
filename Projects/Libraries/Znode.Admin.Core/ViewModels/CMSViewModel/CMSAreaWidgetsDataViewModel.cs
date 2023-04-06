using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSAreaWidgetsDataViewModel : BaseViewModel
    {
        public List<CMSWidgetsViewModel> AvailableWidgets { get; set; }
        public List<CMSWidgetsViewModel> SelectedWidgets { get; set; }
        public CMSAreaViewModel CMSArea { get; set; }

        //this array will be used to POST values from the form to the controller
        public string[] WidgetIds { get; set; }
    }
}
