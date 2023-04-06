using System.ComponentModel.DataAnnotations;


namespace Znode.Engine.Api.Models
{
    public class ContentPageParameterModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string Ids { get; set; }

        public int localeId { get; set; } = 0;

        public int portalId { get; set; } = 0;

        public string TargetPublishState { get; set; }

        public bool TakeFromDraftFirst { get; set; }
    }
}
