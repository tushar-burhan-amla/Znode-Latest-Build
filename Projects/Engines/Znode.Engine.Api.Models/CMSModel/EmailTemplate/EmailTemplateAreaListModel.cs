using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class EmailTemplateAreaListModel: BaseListModel
    {
        public List<EmailTemplateAreaModel> EmailTemplatesAreaList { get; set; }
        public EmailTemplateAreaListModel()
        {
            EmailTemplatesAreaList = new List<EmailTemplateAreaModel>();
        }
    }
}
