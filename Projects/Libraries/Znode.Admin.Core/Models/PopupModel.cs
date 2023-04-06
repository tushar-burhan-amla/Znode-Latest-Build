using System.Collections.Generic;

namespace Znode.Engine.Admin.Models
{
    public class PopupModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string NoChoiceDescription { get; set; }
        public string ModalPopUpId { get; set; }
        public List<PopUpButtonModel> Buttons { get; set; }
        public bool IsErrorPopUp { get; set; }
        public string SpanError { get; set; }
        public string OnCancelClickFunctionName { get; set; }
        public string PartialDivName { get; set; }
        public string SpanErrorDivName { get; set; }
        public string AdditionalNote { get; set; }
        public List<PopupInputField> InputFields { get; set; }
        public string ModalSizeClass { get; set; }
    }

    public class PopupInputField
    {
        public string InputFieldLabel { get; set; }
        public PopupInputFieldTypeEnum InputFieldType { get; set; }
        public List<PopupInputFieldDataItem> InputFieldData { get; set; }
        public string ControlGroupClientSideId { get; set; }
        public int? RequiredNumberOfItemsToDisplay { get; set; }
    }

    public class PopupInputFieldDataItem
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public string HelpText { get; set; }
        public bool Disabled { get; set; }
        public bool IsChecked { get; set; }
        public string ApplicationType { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsHidden { get; set; }
    }

    public enum PopupInputFieldTypeEnum
    {
        CheckBoxGroup,
        RadioButtonGroup
    }
}