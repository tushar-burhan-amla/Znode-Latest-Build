using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class EmailTemplateAreaMapperListModel:BaseListModel
    {
        public List<EmailTemplateAreaMapperModel> EmailTemplatesAreaMapperList { get; set; }
        public EmailTemplateAreaMapperListModel()
        {
            EmailTemplatesAreaMapperList = new List<EmailTemplateAreaMapperModel>();
        }
    }
}
