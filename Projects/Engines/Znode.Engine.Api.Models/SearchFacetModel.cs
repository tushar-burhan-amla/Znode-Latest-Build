using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchFacetModel : BaseModel
    {
        public string AttributeName { set; get; }
        public string AttributeCode { get; set; }
        public List<SearchFacetValueModel> AttributeValues { set; get; }
        public int ControlTypeId { get; set; }
        public int DisplayOrder { get; set; }
        public string ControlType { get; set; }
    }
}