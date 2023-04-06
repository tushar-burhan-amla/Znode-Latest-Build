using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ImportTemplateListResponse : BaseListResponse
    {     
        public List<ImportManageTemplateModel> ImportTemplateList { get; set; }
    }
}