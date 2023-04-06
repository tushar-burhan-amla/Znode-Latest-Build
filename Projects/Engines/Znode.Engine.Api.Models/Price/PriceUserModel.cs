namespace Znode.Engine.Api.Models
{
    public class PriceUserModel : BaseModel
    {
        public int PriceListId { get; set; }
        public int? PriceListUserId { get; set; }
        public int UserId { get; set; }
        public int? AccountId { get; set; }
        public int? Precedence { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string PriceListIds { get; set; }
        public string EmailId { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
    }
}
