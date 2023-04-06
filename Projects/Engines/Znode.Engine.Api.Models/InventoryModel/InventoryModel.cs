using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class InventoryModel : BaseModel
    {
        public int InventoryListId { get; set; }
        [Required]
        public string ListCode { get; set; }
        [Required]
        public string ListName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public List<ImportInventoryModel> ImportInventoryList { get; set; }
    }
}
