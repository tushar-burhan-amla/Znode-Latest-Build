namespace Znode.Engine.Api.Models
{
    public class CMSTypeMappingModel : BaseModel
    {
        public int LocaleId { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOfMapping { get; set; }
        public int PortalId { get; set; }        
        public bool EnableCMSPreview { get; set; }
    }
}
