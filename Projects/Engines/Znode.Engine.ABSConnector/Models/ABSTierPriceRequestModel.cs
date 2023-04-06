namespace Znode.Engine.ABSConnector
{
    public class ABSTierPriceRequestModel : ABSRequestBaseModel
    {
        public string Account { get; set; }
        public string NewPriceTier { get; set; }
    }
}
