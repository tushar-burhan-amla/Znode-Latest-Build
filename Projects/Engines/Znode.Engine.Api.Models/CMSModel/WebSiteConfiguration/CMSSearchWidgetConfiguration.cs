using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSSearchWidgetConfigurationModel : BaseModel
    {
        public int CMSSearchWidgetId { get; set; }
        public int LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public string AttributeCode { get; set; }
        public string SearchKeyWord { get; set; }
        public bool EnableCMSPreview { get; set; }
        public List<PublishAttributeModel> SearchableAttributes { get; set; }
    }
}
