using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceViewModel : BaseViewModel
    {
        public int PriceListId { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.CodeMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelListCode, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PricingListCodeRequiredMessage)]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9-+~@#$%&|,._]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericWithSpecialCharecterStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string ListCode { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.NameMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PricingListNameRequiredMessage)]
        public string ListName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TextCurrency, ResourceType = typeof(Admin_Resources))]
        public int CurrencyId { get; set; }
        public int OldCurrencyId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelActivationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        public string CurrencyName { get; set; }
        public string CultureName { get; set; }
        public string ProfileName { get; set; }
        public string AccountName { get; set; }
        public string UserName { get; set; }
        public HttpPostedFileBase ImportFile { get; set; }
        public GridModel GridModel { get; set; }
        public List<ImportPriceViewModel> ImportPriceList { get; set; }
        public List<SelectListItem> FileTypes { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public int Precedence { get; set; }
        public int PriceListProfileId { get; set; }
        public bool IsParentAccount { get; set; }
        public int PriceListUserId { get; set; }
        public int PriceListAccountId { get; set; }
        public int PriceListPortalId { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        public int PortalProfileId { get; set; }
        [FileTypeValidation(AdminConstants.CSVFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CSVFileTypeErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TemplateId, ResourceType = typeof(Admin_Resources))]
        public int TemplateId { get; set; }

        public List<SelectListItem> TemplateTypeList { get; set; }
        public int ImportHeadId { get; set; }
        public string ImportType { get; set; }
        public string ChangedFileName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TextCurrencyCulture, ResourceType = typeof(Admin_Resources))]
        public int CultureId { get; set; }
        public int OldCultureId { get; set; }
    }
}