using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SupplierTypeListResponse : BaseListResponse
    {
        public List<SupplierTypeModel> SupplierTypeList { get; set; }
    }
}
