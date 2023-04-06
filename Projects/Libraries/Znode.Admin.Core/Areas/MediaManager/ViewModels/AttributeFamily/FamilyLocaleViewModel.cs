
namespace Znode.Engine.Admin.ViewModels
{
    public class FamilyLocaleViewModel : BaseViewModel
    {
        public int FamilyLocaleId { get; set; }
        public int? LocaleId { get; set; }
        public int? AttributeFamilyId { get; set; }
        public string AttributeFamilyName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}