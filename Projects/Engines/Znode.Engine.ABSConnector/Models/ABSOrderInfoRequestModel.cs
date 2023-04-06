namespace Znode.Engine.ABSConnector
{
    public class ABSOrderInfoRequestModel : ABSRequestBaseModel
    {
        public string SoldTo { get; set; }
        public string PoNumber { get; set; }
        public string SummaryRequested { get; set; }
        public string SummaryDate { get; set; }
        public string OrderNumberRequested { get; set; }
        public int NumberOfRecords { get; set; }
        public string CurrentRecord { get; set; }
        public string ShipTo { get; set; }
    }
}
