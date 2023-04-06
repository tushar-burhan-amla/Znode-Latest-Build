namespace Znode.Engine.ABSConnector
{
    public class ABSPriceResponseModel : ABSRequestBaseModel
    {
        public string TierPrice { get; set; }
        public string UpcCode { get; set; }
    }
}
