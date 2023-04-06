using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class TypeaheadRequestModel : BaseModel
    {
        public string Searchterm { get; set; }

        public string FieldName { get; set; }

        public ZnodeTypeAheadEnum Type { get; set; }

        public string TypeName { get; set; }

        public int MappingId { get; set; }

        public int PageSize { get; set; }
    }    
}
