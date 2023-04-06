using System;

namespace Znode.Engine.Api.Models
{
    public class PublishHistoryModel : BaseModel
    {
        public int VersionId { get; set; }
        public string SourcePublishState { get; set; }
        public string EntityId { get; set; }
        public string EntityName { get; set; }
        public string LocaleDisplayValue { get; set; }
        public string LogMessage { get; set; }
        public DateTime LogCreatedDate { get; set; }
        public int? PreviousVersionId { get; set; }
    }
}
