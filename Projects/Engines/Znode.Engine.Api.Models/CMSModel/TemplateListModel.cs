using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TemplateListModel : BaseListModel
    {
        public List<TemplateModel> Templates { get; set; }
    }
}
