using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class EditFamilyLocaleViewModel
    {
        public List<FamilyLocaleDetails> FamilyLocaleDetails { get; set; }
        public string FamilyCode { get; set; }
        public LocaleListViewModel Locales { get; set; }
        public int AttributeFamilyId { get; set; }

        public EditFamilyLocaleViewModel()
        {
            FamilyLocaleDetails = new List<FamilyLocaleDetails>();
            Locales = new LocaleListViewModel();
        }
    }
    public class FamilyLocaleDetails
    {
        public int FamilyLocaleId { get; set; }
        public int? LocaleId { get; set; }
        public int? AttributeFamilyId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterFamilyLocale, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string AttributeFamilyName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string LocaleName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}