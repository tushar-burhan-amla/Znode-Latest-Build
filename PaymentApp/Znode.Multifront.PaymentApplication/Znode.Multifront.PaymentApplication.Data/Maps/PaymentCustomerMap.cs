using System;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data.Maps
{
    public class PaymentCustomerMap
    {
        public static ZnodePaymentCustomer ToEntity(PaymentModel paymentModel)
       => Equals(paymentModel, null) ? null :
            new ZnodePaymentCustomer
            {
                CustomersGUID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                FirstName = paymentModel.CardHolderFirstName,
                LastName = paymentModel.CardHolderLastName
            };
    }
}
