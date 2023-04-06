using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ContentContainerListResponseModel :  BaseListResponse
    {
        public List<AssociatedVariantModel> AssociatedVariants { get; set; }
        public List<ContentContainerResponseModel> ContainerList { get; set; }
    }
}
