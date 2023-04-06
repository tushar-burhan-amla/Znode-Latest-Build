using System.Collections.Generic;

namespace Znode.Engine.Api.Models 
{
    public class AccountTemplateListModel : BaseListModel
    {
        public List<AccountTemplateModel> AccountTemplates { get; set; }
    }
}
