using System.Collections.Generic;

namespace Znode.Engine.WebStore.Models
{
    public class PopupModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ModalPopUpId { get; set; }
        public List<PopUpButtonModel> Buttons { get; set; }
        public bool IsErrorPopUp { get; set; }
        public string SpanError { get; set; }
        public string OnCancelClickFunctionName { get; set; }
        public string PartialDivName { get; set; }
        public string SpanErrorDivName { get; set; }
    }
}