using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class AddressModelV2 : BaseModel
    {        
        [Required]
        [MaxLength(300)]
        public string Address1 { get; set; }
        [MaxLength(300)]
        public string Address2 { get; set; }
        [MaxLength(300)]
        public string Address3 { get; set; }                
        [Required]
        [MaxLength(300)]
        public string FirstName { get; set; }        
        [Required]
        [MaxLength(300)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(300)]
        public string CountryName { get; set; }
        [MaxLength(150)]
        public string StateName { get; set; }
        [Required]
        [MaxLength(150)]
        public string CityName { get; set; }
        [MaxLength(10)]
        public string StateCode { get; set; }        
        [Required]
        [MaxLength(50)]
        public string PostalCode { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }        
    }
}
