namespace Znode.Engine.ABSConnector
{
    public class ABSShipToInformationModel : ABSRequestBaseModel
    {
        public string ShipToName { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }
    }
}
