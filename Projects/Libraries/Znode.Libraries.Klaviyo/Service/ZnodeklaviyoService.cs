using klaviyo.net;
using System;
using System.Collections.Generic;

namespace Znode.Libraries.Klaviyo
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

            return gateway.Identify(pe);
        }

        //// Set the data for KlaviyoOrder
        public virtual SubmitStatus KlaviyoOrder(OrderDetailsModel orderDetails, string publicKey)
        {
            KlaviyoGateway gateway = new KlaviyoGateway(publicKey);
            KlaviyoEvent ev = new KlaviyoEvent()
            {
                Token = gateway.Token,
                Event = orderDetails.IsProductDisplayPage ? ZnodeKlaviyoConstant.Order : ZnodeKlaviyoConstant.OrderedProduct
            };

            ev.CustomerProperties.Email = orderDetails.Email;
            ev.CustomerProperties.FirstName = orderDetails.FirstName;
            ev.CustomerProperties.LastName = orderDetails.LastName;

            foreach (OrderLineItemDetailsModel orderLineItem in orderDetails.OrderLineItems)
            {
                ev.Properties.NotRequiredProperties = new List<NotRequiredProperty>();
                ev.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.Itemname, orderLineItem.ItemName));
                ev.Properties.NotRequiredProperties.Add(new NotRequiredProperty(ZnodeKlaviyoConstant.Value, orderLineItem.Value));
            }
            ev.Properties.EventId = Guid.NewGuid().ToString();
            ev.Properties.Value = 0M;

            return gateway.Track(ev);
        }
        #endregion
    }
}
