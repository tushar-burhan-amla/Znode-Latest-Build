using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FormBuilderAttributeGroupModel : BaseModel
    {
        public int FormBuilderId { get; set; }
        public int LocaleId { get; set; }
        public string FormCode { get; set; }
        public string FormTitle { get; set; }
        public string ButtonText { get; set; }
        public string TextMessage { get; set; }
        public string RedirectURL { get; set; }
        public bool? IsTextMessage { get; set; }
        public bool? IsShowCaptcha { get; set; }
        public List<GlobalAttributeGroupModel> Groups { get; set; }
        public List<GlobalAttributeValuesModel> Attributes { get; set; }
    }
}
