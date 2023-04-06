using System;

namespace Znode.Engine.ABSConnector
{
    public class ABSInventoryResponseModel : ABSRequestBaseModel
    {
        public string UpcNumber { get; set; }
        public string InventoryCode { get; set; }
        public decimal InventoryQty { get; set; }
        public string InventoryCutNumber { get; set; }
        public DateTime? InventoryExpectedDate { get; set; }
        public string InventroyCloseOut { get; set; }
        public string SoldOutFlag { get; set; }
        public string SoldOutCode { get; set; }
        public string SoldOutDescription { get; set; }
        public DateTime? SoldOutDate { get; set; }
    }
}
