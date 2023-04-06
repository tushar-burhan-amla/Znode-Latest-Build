using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ContainerTemplateListModel : BaseListModel
    {
        public List<ContainerTemplateModel> ContainerTemplates { get; set; }
    }
}
