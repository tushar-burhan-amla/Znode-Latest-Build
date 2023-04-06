namespace Znode.Engine.ABSConnector
{
    public class ABSEmailRequestModel : ABSRequestBaseModel
    {
        public string SoldTo { get; set; }
        public string EmailAddressType { get; set; }
        public string EmailAddress { get; set; }
    }
}
