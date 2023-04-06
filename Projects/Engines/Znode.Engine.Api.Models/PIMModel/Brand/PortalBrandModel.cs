namespace Znode.Engine.Api.Models
{
    public class PortalBrandModel : BaseModel
    {
        public int PortalId { get; set; }
        public int? LocaleId { get; set; }
        public string StoreName { get; set; }
        public string CompanyName { get; set; }
        public string PublishStatus { get; set; }
        public string StoreLogo { get; set; }
        public decimal? OrderAmount { get; set; }

        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
    }
}
