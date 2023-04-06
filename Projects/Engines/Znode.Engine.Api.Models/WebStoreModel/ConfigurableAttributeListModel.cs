using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ConfigurableAttributeListModel : BaseListModel
    {
        public List<PublishAttributeModel> Attributes { get; set; }
    }
}
