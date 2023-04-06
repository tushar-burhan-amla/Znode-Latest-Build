using System;

namespace Znode.Engine.Api.Models
{
    public class ImportManageTemplateModel: BaseModel
    {
        public int ImportTemplateId { get; set; }
        public string TemplateName { get; set; }
        public string ImportType { get; set; }
        public string ImportName { get; set; }
        public string Status { get; set; }
    }
}