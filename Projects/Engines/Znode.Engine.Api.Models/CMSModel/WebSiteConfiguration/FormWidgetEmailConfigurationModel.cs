namespace Znode.Engine.Api.Models
{
    public class FormWidgetEmailConfigurationModel : BaseModel
    {
        public int FormWidgetEmailConfigurationId { get; set; }
        public int CMSContentPagesId { get; set; }
        public string NotificationEmailId { get; set; }
        public int NotificationEmailTemplateId { get; set; }
        public int AcknowledgementEmailTemplateId { get; set; }
        public int LocaleId { get; set; }
        public string NotificationEmailTemplate { get; set; }
        public string AcknowledgementEmailTemplate { get; set; }

    }
}
