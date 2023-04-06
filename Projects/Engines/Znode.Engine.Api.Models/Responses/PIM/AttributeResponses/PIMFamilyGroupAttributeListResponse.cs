using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class PIMFamilyGroupAttributeListResponse : BaseListResponse
    {
        public List<PIMFamilyGroupAttributeListModel> PIMFamilyGroupAttributes { get; set; }
    }
}
