using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ContainerTemplateListResponse : BaseListResponse
    {
        public ContainerTemplateModel ContainerTemplate { get; set; }

        public List<ContainerTemplateModel> ContainerTemplates { get; set; }
    }
}
