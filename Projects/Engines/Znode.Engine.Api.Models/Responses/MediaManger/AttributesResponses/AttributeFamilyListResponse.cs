using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class AttributeFamilyListResponse : BaseListResponse
    {
        //List response for attribute families.
        public List<AttributeFamilyModel> AttributeFamilies { get; set; }
    }
}
