namespace Znode.Engine.ABSConnector
{
    public class ABSPriceRequestModel : ABSRequestBaseModel
    {
        public string Account { get; set; }
        public string TierPriceCode { get; set; }
        public string UpcCode { get; set; }
    }
}
