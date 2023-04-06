using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class WarehouseModel : BaseModel
    {
        public int WarehouseId { get; set; }

        public int InventoryListId { get; set; }

        public string ListName { get; set; }

        [Required]
        public string WarehouseCode { get; set; }

        [Required]
        public string WarehouseName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string ExternalId { get; set; }
        public AddressModel Address { get; set; }
    }
}
