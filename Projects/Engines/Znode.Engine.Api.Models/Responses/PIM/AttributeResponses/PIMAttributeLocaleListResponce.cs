using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PIMAttributeLocaleListResponce : BaseListResponse
    {
        public List<PIMAttributeLocaleModel> AttributeLocales { get; set; }

       public List<PIMAttributeDefaultValueModel> DefaultValues { get; set; } 
    }
}
