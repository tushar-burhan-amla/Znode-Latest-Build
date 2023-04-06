using System;

namespace Znode.Engine.Api.Models
{
    public class PublishSearchProfileModel
    {
        public int PublishSearchProfileId { get; set; }
        public int SearchProfileId { get; set; }
        public string SearchProfileName { get; set; }
        public int PublishCatalogId { get; set; }
        public string FeaturesList { get; set; }
        public string QueryTypeName { get; set; }
        public string SearchQueryType { get; set; }
        public string QueryBuilderClassName { get; set; }
        public string SubQueryType { get; set; }
        public string FieldValueFactor { get; set; }
        public string Operator { get; set; }
        public int PublishStateId { get; set; }
        public string SearchProfileAttributeMappingJson { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int SearchProfilePublishLogId { get; set; }
    }
}
