using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeLocaleListResponse : BaseListResponse
    {
        public List<AttributeLocalDataModel> AttributeLocales { get; set; }
    }
}
