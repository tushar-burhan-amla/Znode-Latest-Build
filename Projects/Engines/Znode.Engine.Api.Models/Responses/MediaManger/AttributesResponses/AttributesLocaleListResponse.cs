using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributesLocaleListResponse:BaseListResponse
    {
        public List<AttributesLocaleModel> AttributeLocales { get; set; }

        public List<AttributesDefaultValueModel> DefaultValues { get; set; }
    }
}
