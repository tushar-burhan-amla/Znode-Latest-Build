using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class GlobalAttributeLocaleListResponse : BaseListResponse
    {
        public List<GlobalAttributeLocaleModel> AttributeLocales { get; set; }

        public List<GlobalAttributeDefaultValueModel> DefaultValues { get; set; }
    }
}

