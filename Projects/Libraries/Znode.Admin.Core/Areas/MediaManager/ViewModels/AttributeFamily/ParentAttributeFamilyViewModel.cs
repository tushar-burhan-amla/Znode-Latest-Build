using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ParentAttributeFamilyViewModel
    {
        public AttributeFamilyViewModel AttributeFamily { get; set; }
        public EditFamilyLocaleViewModel EditFamilyLocale { get; set; }
        public NavigationViewModel navigationModel { get; set; }
        public string ViewModeType { get; set; }
        public TabViewListModel TabViewModel { get; set; }
    }
}