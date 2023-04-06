namespace Znode.Engine.ABSConnector
{
    public class ABSBillToChangeRequestModel : ABSRequestBaseModel
    {
        public int? BillToAccount { get; set; }
        public string BillToName { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToAddress3 { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToZip { get; set; }
        public string BillToPhone { get; set; }
        public string BillToFax { get; set; }
        public string BillToCountry { get; set; }
    }
}
