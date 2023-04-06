using System.Collections.Generic;

namespace Znode.Engine.WebStore.Models
{
    public class DropDownOptions
    {
        public bool assignable { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string SuccessCallBack { get; set; }
        public bool IsAjax { get; set; } = false;
        public bool IsMultiple { get; set; }
        public string SelectOptionName { get; set; }
        public bool IsDraggable { get; set; }
        public bool IsAllowCheckAll { get; set; }
        public string DropDownId { get; set; }
        public string SortAction { get; set; }
        public List<BaseDropDownList> DropDownList { get; set; }
        public bool ShowSubmitButton { get; set; }
        public bool IsSubOptions { get; set; }
        public string HiddenItemDataStorage { get; set; }
        public bool IsDropDownHidden { get; set; }
    }
}