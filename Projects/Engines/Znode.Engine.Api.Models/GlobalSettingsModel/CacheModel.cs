using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CacheModel : BaseModel
    {
        public int ApplicationCacheId { get; set; }
        public int? Duration { get; set; }
        public string ApplicationType { get; set; }
        public List<int> DomainIds { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public List<CloudflareErrorResponseModel> CloudflareErrorList { get; set; }
    }
}
