namespace Znode.Engine.Admin.ViewModels
{
    public class PIMFrontPropertiesViewModel:BaseViewModel
    {
        //Frontend Properties
        public bool? IsComparable { get; set; } = true;
        public bool? IsUseInSearch { get; set; } 
        public bool? IsAllowHtmlTag { get; set; } = true;
        public bool? IsFacets { get; set; }
    }
}
