using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FormSubmitModel:BaseModel
    {
        public int FormBuilderId { get; set; }
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public string FormCode { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsSuccess { get; set; }
        public List<FormSubmitAttributeModel> Attributes { get; set; }
        public FormSubmitModel()
        {
            Attributes = new List<FormSubmitAttributeModel>();
        }
    }
}
