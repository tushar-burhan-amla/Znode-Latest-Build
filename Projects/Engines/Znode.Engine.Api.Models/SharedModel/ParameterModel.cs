using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class ParameterModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string Ids { get; set; }
        public string TargetPublishState { get; set; }
        public bool TakeFromDraftFirst { get; set; }
        public int LocaleId { get; set; }
        public List<int> LocaleIds { get; set; }
        public int PortalId { get; set; }
        public string RevisionType { get; set; }
        public int publishCataLogId { get; set; }
        public int PimProductId { get; set; }
        public bool EnableCMSPreview { get; set; }

        // To determine whether search profile published data needs to be deleted or not.
        // If this flag is set to true an index creation will be initialed for respective catalogs by considering the system defined search profile.
        public bool IsDeletePublishSearchProfile { get; set; }
    }
}
