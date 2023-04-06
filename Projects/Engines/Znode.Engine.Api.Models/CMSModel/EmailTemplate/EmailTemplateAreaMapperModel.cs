namespace Znode.Engine.Api.Models
{
    public class EmailTemplateAreaMapperModel : BaseModel
    {
        public int EmailTemplateMapperId { get; set; }
        public int EmailTemplateId { get; set; }
        public int EmailTemplateAreasId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSMSNotificationActive { get; set; }
        public bool IsEnableBcc { get; set; }
        public string EmailTemplateName { get; set; }
        public string EmailTemplateAreaName { get; set; }
        public int? PortalId { get; set; }
    }
}
