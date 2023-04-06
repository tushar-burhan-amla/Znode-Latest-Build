using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class EmailTemplateListModel : BaseListModel
    {
        public List<EmailTemplateModel> EmailTemplatesList { get; set; }
        public EmailTemplateListModel()
        {
            EmailTemplatesList = new List<EmailTemplateModel>();
        }
    }
}