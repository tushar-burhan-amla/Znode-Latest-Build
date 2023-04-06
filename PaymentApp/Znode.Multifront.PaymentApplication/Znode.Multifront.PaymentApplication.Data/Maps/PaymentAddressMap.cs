using System;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data.Maps
{
    public class PaymentAddressMap
    {
        public static ZnodePaymentAddress ToEntity(PaymentModel paymentModel)
        => Equals(paymentModel, null) ? null :
        new ZnodePaymentAddress
        {
            CreditCardAddressId = Guid.NewGuid(),
            CardHolderFirstName = paymentModel.CardHolderFirstName,
            CardHolderLastName = paymentModel.CardHolderLastName,
            AddressLine1 = paymentModel.BillingStreetAddress1,
            AddressLine2 = paymentModel.BillingStreetAddress2,
            City = paymentModel.BillingCity,
            State = paymentModel.BillingStateCode,
            Country = paymentModel.BillingCountryCode,
            ZipCode = paymentModel.BillingPostalCode
        };
    }
}
