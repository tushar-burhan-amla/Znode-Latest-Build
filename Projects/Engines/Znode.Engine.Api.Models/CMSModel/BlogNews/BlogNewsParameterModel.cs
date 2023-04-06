using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class BlogNewsParameterModel
    {
        [Required]
        public string BlogNewsId { get; set; }

        public bool IsTrueOrFalse { get; set; }

        public string Activity { get; set; }

        public int LocaleId { get; set; } = 0;

        public int PortalId { get; set; } = 0;

        public string TargetPublishState { get; set; }

        public bool TakeFromDraftFirst { get; set; }

        public bool IsCMSPreviewEnable { get; set; }
    }
}
