namespace Znode.Engine.ABSConnector
{
    public class ABSARListRequestModel : ABSRequestBaseModel
    {
        public string SoldTo { get; set; }
        public int NumberOfRecords { get; set; }
        public int CurrentRecord { get; set; }
        public string ShipTo { get; set; }
    }
}
