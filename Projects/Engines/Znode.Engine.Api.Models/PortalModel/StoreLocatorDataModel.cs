using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class StoreLocatorDataModel : BaseModel
    {
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeGreaterThan0)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public int PortalId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeGreaterThan0)]
        public int? DisplayOrder { get; set; }

        [MaxLength(600, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public string StoreName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [MaxLength(200, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string StoreLocationCode { get; set; }

        public int AddressId { get; set; }
        public int PortalAddressId { get; set; }
        public bool IsActive { get; set; }
        public string MapQuestURL { get; set; }
        public string MediaPath { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public int MediaId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string Address1 { get; set; }

        [MaxLength(300, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        [MaxLength(600)]
        public string DisplayName { get; set; }

        [MaxLength(3000, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public string CountryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [MaxLength(3000, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string StateName { get; set; }
        public string StateCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        [MaxLength(3000, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string CityName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public string PostalCode { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredField)]
        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }
        public string ExternalId { get; set; }
        public string Portalname { get; set; }
        [MaxLength(300)]
        public string CompanyName { get; set; }
    }
}
