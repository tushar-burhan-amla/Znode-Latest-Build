using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSMediaWidgetConfigurationViewModel : BaseViewModel

    {
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int MediaId { get; set; }
        public bool EnableCMSPreview { get; set; }

        public int CMSMediaConfigurationId { get; set; }

    }
}
