namespace Znode.Engine.Api.Models
{
    public class LocaleModel : BaseModel
    {
        public int LocaleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int PortalId { get; set; }
        public int? PortalLocaleId { get; set; }
        public string StoreName { get; set; }
    }
}
