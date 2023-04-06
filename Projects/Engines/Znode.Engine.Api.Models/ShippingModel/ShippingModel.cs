using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ShippingModel : BaseModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.PleaseSelectShippingType, ErrorMessageResourceType = typeof(Api_Resources))]
        public int ShippingTypeId { get; set; }
        public int ShippingId { get; set; }
        public int? ProfileId { get; set; }
        public string ShippingCode { get; set; }
        public string DestinationCountryCode { get; set; }
        public decimal HandlingCharge { get; set; }
        public string HandlingChargeBasedOn { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? CurrencyId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.PleaseEnterShippingName, ErrorMessageResourceType = typeof(Api_Resources))]
        public string ShippingName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.PleaseEnterDisplayOrder, ErrorMessageResourceType = typeof(Api_Resources))]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.InvalidShippingDisplayOrder)]
        public int DisplayOrder { get; set; }
        public string ExternalId { get; set; }
        public string ShippingTypeName { get; set; }
        public string ImportedSkus { get; set; }
        public int ShippingServiceCodeId { get; set; }
        public string StateCode { get; set; }
        public string CountyFIPS { get; set; }

        [MaxLength(5000, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ZipCodeLengthErrorMessage)]
        public string ZipCode { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingHandlingCharge { get; set; }
        public string ResponseCode { get; set; } = "0";
        public string ResponseMessage { get; set; }
        public bool IsHandlingChargeInPercent { get; set; }

        public ShippingTypeModel ShippingTypeModel { get; set; }
        public decimal? ShippingRate { get; set; }
        public decimal? ShippingRateWithoutDiscount { get; set; }
        public string ApproximateArrival { get; set; }
        public int? PortalId { get; set; }
        public string ClassName { get; set; }
        public string TrackingUrl { get; set; }
        public int PromotionId { get; set; }
        public string DeliveryTimeframe { get; set; }
        public bool IsExpedited { get; set; }
        public string EstimateDate { get; set; }
        public string PublishState { get; set; }
    }
}
