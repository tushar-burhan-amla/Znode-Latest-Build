using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class ShippingMap
    {
        public static ZnodeShipping ToZnodeShipping(OrderShippingModel model)
        {
            if (HelperUtility.IsNull(model))
                return new ZnodeShipping { ShippingID = 0 };

            ZnodeShipping znodeShipping = new ZnodeShipping
            {
                ResponseCode = model.ResponseCode,
                ResponseMessage = model.ResponseMessage,
                ShippingDiscount = model.ShippingDiscount,
                ShippingHandlingCharge = model.ShippingHandlingCharge,
                ShippingID = model.ShippingId,
                ShippingName = model.ShippingName,
                ShippingCountryCode = string.IsNullOrEmpty(model.ShippingCountryCode) ? string.Empty : model.ShippingCountryCode,
                ShippingDiscountDescription = model.ShippingDiscountDescription,
                ShippingCode = model.ShippingCode

            };

            return znodeShipping;
        }

        //Bind Znode.Libraries.ECommerce.Entities.ZNodeShipping data to ShippingModel.
        public static OrderShippingModel ToModel(ZnodeShipping znodeShipping)
            => HelperUtility.IsNull(znodeShipping) ? new OrderShippingModel() : new OrderShippingModel
            {
                ResponseCode = znodeShipping.ResponseCode,
                ResponseMessage = znodeShipping.ResponseMessage,
                ShippingDiscount = znodeShipping.ShippingDiscount,
                ShippingHandlingCharge = znodeShipping.ShippingHandlingCharge,
                ShippingName = znodeShipping.ShippingName,
                ShippingId = znodeShipping.ShippingID,
                ShippingDiscountDescription = znodeShipping.ShippingDiscountDescription,
                ShippingCountryCode = string.IsNullOrEmpty(znodeShipping.ShippingCountryCode) ? string.Empty : znodeShipping.ShippingCountryCode,
                IsValidShippingSetting = znodeShipping.IsValidShippingSetting,
                ShippingDiscountApplied = znodeShipping.ShippingDiscountApplied
            };
    }
}
