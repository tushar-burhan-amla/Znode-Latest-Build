namespace Znode.Engine.ABSConnector
{
    public class ABSInventoryRequestModel : ABSRequestBaseModel
    {
        public string Season { get; set; }
        public string Seayr { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string Piece { get; set; }
        public string Dim { get; set; }
        public string UpcNumber { get; set; }
    }
}
