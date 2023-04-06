using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class ParameterModelForPortalCountries : BaseModel
    {
        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public int PortalId { get; set; }
        public string CountryCode { get; set; }
        public bool IsDefault { get; set; }
        public int PortalCountryId { get; set; }
        public string PortalCountryIds { get; set; }
    }
}
