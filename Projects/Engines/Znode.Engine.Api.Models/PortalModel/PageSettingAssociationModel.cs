namespace Znode.Engine.Api.Models
{
    public class PageSettingAssociationModel : BaseModel
    {
        public int PortalId { get; set; }
        public string PageSettingIds { get; set; }
        public string PortalPageSettingIds { get; set; }        
    }
}
