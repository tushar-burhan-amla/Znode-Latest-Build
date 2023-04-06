using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;


namespace Znode.Engine.WebStore.ViewModels
{
    public class StoreLocatorViewModel : BaseViewModel
    {
        public int AddressId { get; set; }
        public int PortalID { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelSearchMiles, ResourceType = typeof(WebStore_Resources))]
        public int Radius { get; set; }

        [Required]
        [Display(Name = ZnodeWebStore_Resources.LabelStoreName, ResourceType = typeof(WebStore_Resources))]
        public string StoreName { get; set; }
        public int PortalAddressId { get; set; }
        public string MapQuestURL { get; set; }

        public List<SelectListItem> RadiusList { get; set; }
        public List<StoreLocatorViewModel> PortalList { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelState, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(50, ErrorMessageResourceName = ZnodeWebStore_Resources.LengthStateCode, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string StateName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelCity, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(1000, ErrorMessageResourceName = ZnodeWebStore_Resources.CityCodeLengthErrorMessage, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string CityName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelPostalCode, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(50, ErrorMessageResourceName = ZnodeWebStore_Resources.PostalCodeLengthErrorMessage, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string PostalCode { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }
}