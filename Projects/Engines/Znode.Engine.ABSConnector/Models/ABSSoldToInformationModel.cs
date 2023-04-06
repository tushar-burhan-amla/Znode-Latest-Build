namespace Znode.Engine.ABSConnector
{
    public class ABSSoldToInformationModel : ABSRequestBaseModel
    {
        public string SoldToName { get; set; }
        public string SoldToAddress1 { get; set; }
        public string SoldToAddress2 { get; set; }
        public string SoldToCity { get; set; }
        public string SoldToState { get; set; }
        public string SoldToZip { get; set; }
    }
}
