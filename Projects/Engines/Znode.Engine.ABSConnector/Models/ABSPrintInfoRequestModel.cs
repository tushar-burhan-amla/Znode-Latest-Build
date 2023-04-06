namespace Znode.Engine.ABSConnector
{
    public class ABSPrintInfoRequestModel : ABSRequestBaseModel
    {
        public string SoldTo { get; set; }
        public string ReferenceNumber { get; set; }
        public string BillOfLading { get; set; }
        public string ReferenceType { get; set; }
        public string EmailAddress { get; set; }
    }
}
