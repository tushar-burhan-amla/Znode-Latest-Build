using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class InventorySKUModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.SKURequiredMessage)]
        public string SKU { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.QuantityRequiredMessage)]
        public decimal Quantity { get; set; }

        public decimal BackOrderQuantity { get; set; }

        public DateTime? BackOrderExpectedDate { get; set; }

        public decimal? ReOrderLevel { get; set; }
        public int InventoryId { get; set; }
        public int? WarehouseId { get; set; }
        public string ListName { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ExternalId { get; set; }
        public bool IsDownloadable { get; set; }
        public int PimProductId { get; set; }

        public bool IsFromWarehouse { get; set; }

        public List<DownloadableProductKeyModel> DownloadableProductKeys { get; set; }
        public int PortalId { get; set; }

        public decimal QuantityExpected { get; set; }
        public DateTime DateExpected { get; set; }
        public decimal? AllLocationQuantity { get; set; }
        public bool IsDefaultWarehouse { get; set; }
        public string WarehouseCityName { get; set; }
        public string WarehouseStateName { get; set; }
        public string WarehousePostalCode { get; set; }
        public string WarehouseAddress { get; set; }
        public string DefaultInventoryCount { get; set; }
        public string ModifiedByName { get; set; }
    }
}
