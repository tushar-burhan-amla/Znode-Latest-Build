using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class SearchProfileParameterModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string Ids { get; set; }

        public int SearchProfileId { get; set; }

        public string PortalSearchProfileIds { get; set; }

        public bool IsAssociate { get; set; }
    }
}
