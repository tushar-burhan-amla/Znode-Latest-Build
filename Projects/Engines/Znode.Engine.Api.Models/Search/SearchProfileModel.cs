using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchProfileModel : BaseModel
    {
        public int SearchProfileId { get; set; }
        public string ProfileName { get; set; }
        public string QueryTypeName { get; set; }
        public string SubQueryType { get; set; }
        public string CatalogCode { get; set; }

        public string QueryBuilderClassName { get; set; }

        public int SearchQueryTypeId { get; set; }
        public int SearchSubQueryTypeId { get; set; }
        public int? PortalId { get; set; }
        public int PublishCatalogId { get; set; }
        public string CatalogName { get; set; }
        public string PortalName { get; set; }
        public string SearchText { get; set; }
        public string Operator { get; set; } = "AND";
        public bool IsDefault { get; set; }

        public List<SearchFeatureModel> FeaturesList { get; set; }
        public List<SearchQueryTypeModel> QueryTypes { get; set; }
        public List<SearchAttributesModel> SearchableAttributesList { get; set; }
        public List<SearchAttributesModel> FacetAttributesList { get; set; }

        public List<KeyValuePair<string, int>> FieldValueFactors { get; set; }
        public List<string> FieldList { get; set; }
        public List<FieldValueModel> FieldValueList { get; set; }
        public bool IsIndexExist { get; set; }
        public string PublishStatus { get; set; }
    }
}
