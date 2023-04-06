using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.WebStore.Core.ViewModels
{
    public class OrderWarehouseViewModel
    {
        public int OrderId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string WarehouseName { get; set; }
    }
}
