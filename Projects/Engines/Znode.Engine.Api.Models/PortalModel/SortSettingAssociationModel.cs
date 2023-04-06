namespace Znode.Engine.Api.Models
{
    public class SortSettingAssociationModel : BaseModel
    {
        public int PortalId { get; set; }
        public string SortSettingIds { get; set; }
        public string PortalSortSettingIds { get; set; }        
    }
}
