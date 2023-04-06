using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ContentPageTreeModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        public string Icon { get; set; }
        public int DisplayOrder { get; set; }
        public int PimCategoryId { get; set; }

        
        public int PimCategoryHierarchyId { get; set; }
        [JsonProperty("children")]
        public List<ContentPageTreeModel> Children { get; set; }
        public int? PimCatalogId { get; set; }
        public string CategoryValue { get; set; }
    }

    public struct CatalogTreeModel
    {
        [JsonProperty("text")]
        public string CatalogName { get; set; }
        public bool IsCatalogPublished { get; set; }
        [JsonProperty("children")]
        public List<ContentPageTreeModel> Children { get; set; }
    }  
}
