using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class FamilyGroupAttributeListResponse : BaseListResponse
    {
        //List response for attribute families.
        public List<FamilyGroupAttributeModel> FamilyGroupAttributes { get; set; }
    }
}
