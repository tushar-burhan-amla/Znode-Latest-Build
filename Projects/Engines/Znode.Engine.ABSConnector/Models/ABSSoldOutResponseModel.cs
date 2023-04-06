namespace Znode.Engine.ABSConnector
{
    public  class ABSSoldOutResponseModel : ABSRequestBaseModel
    {
        public string UpcNumber { get; set; }
      	public string SoldOutFlag { get; set; }
        public string SoldOutCode { get; set; }
        public string SoldOutDescription { get; set; }
        public string SoldOutDate { get; set; }
    }
}
