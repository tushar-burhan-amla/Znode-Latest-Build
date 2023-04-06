namespace Znode.Engine.ABSConnector
{
    public class ABSChangeShipToRequestModel : ABSRequestBaseModel
    {
        public string SoldTo { get; set; }
        public string ShipTo { get; set; }
    }
}
