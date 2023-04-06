using klaviyo.net;

using System.Dynamic;

using Znode.Engine.klaviyo.Models;
using Znode.Libraries.Klaviyo;
using Znode.Libraries.Klaviyo.Helper;
using Znode.Libraries.Klaviyo.Model;

namespace Znode.Engine.Klaviyo.Services
{
    public class ZnodeklaviyoService : IZnodeklaviyoService
    {
        #region Public Methods 
        // Set the data for KlaviyoIdentify
        public virtual SubmitStatus KlaviyoIdentify(IdentifyModel identify, string publicKey)
        {
            KlaviyoGateway gateway = new KlaviyoGateway(publicKey);
            KlaviyoPeople pe = new KlaviyoPeople()
            {
                Token = gateway.Token,
            };

            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.Email, identify.Email));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.FirstName, identify.FirstName));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.LastName, identify.LastName));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.StoreCode, identify.StoreCode));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.StoreName, identify.StoreName));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.UserName, identify.UserName));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.CompanyName, identify.CompanyName));
            pe.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.PhoneNumber, identify.PhoneNumber));
            return gateway.Identify(pe);
        }

        //// Set the data for KlaviyoOrder
        public virtual SubmitStatus KlaviyoOrder(OrderDetailsModel orderDetails, string publicKey)
        {
            KlaviyoGateway gateway = new KlaviyoGateway(publicKey);
            KlaviyoEventModel model = new KlaviyoEventModel();
            model.token = publicKey;
            model.customer_properties = new CustomerProperty();
            model.customer_properties.email = orderDetails.Email;
            model.customer_properties.FirstName = orderDetails.FirstName;
            model.customer_properties.LastName = orderDetails.LastName;
            model.properties = new ExpandoObject();

            if (orderDetails.PropertyType == (int)KlaviyoEventType.AddToCartEvent)
            {
                model.Event = orderDetails.StoreName + " " + "-" + " " + ZnodeKlaviyoConstant.AddToCart;
                model.properties.LineItemDetails = orderDetails.OrderLineItems;
            }
            else if (orderDetails.PropertyType == (int)KlaviyoEventType.ProductEvent)
            {
                model.Event = orderDetails.StoreName + " " + "-" + " " + ZnodeKlaviyoConstant.ProductView;
                model.properties.ProductDetails = orderDetails.OrderLineItems;
            }
            else
            {
                model.Event = orderDetails.StoreName + " " + "-" + " " + ZnodeKlaviyoConstant.SuccessfulCheckout;
                model.properties.LineItemDetails = orderDetails.OrderLineItems;
            }
            return gateway.Track(model);
        }
        #endregion
    }
}
