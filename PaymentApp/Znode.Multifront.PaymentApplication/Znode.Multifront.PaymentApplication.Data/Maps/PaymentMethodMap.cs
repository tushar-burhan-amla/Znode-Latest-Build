using System;
using System.Collections.Generic;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Data.Maps
{
    public class PaymentMethodMap
    {
        public static ZnodePaymentMethod ToEntity(PaymentModel paymentModel)
        {
            ZnodePaymentMethod paymentMethods = new ZnodePaymentMethod();
            paymentMethods = paymentModel.ToEntity<ZnodePaymentMethod>();
            if (string.IsNullOrEmpty(paymentModel.AddressId))
            {
                paymentMethods.PaymentGUID = Guid.NewGuid();
                paymentMethods.Token = EncryptionHelper.EncryptToken(paymentModel.CustomerPaymentProfileId);
            }
            else
            {
                paymentMethods.PaymentGUID = Guid.NewGuid();
                paymentMethods.CreditCardAddressId = new Guid(paymentModel.AddressId);
                paymentMethods.CustomersGUID = new Guid(paymentModel.CustomerGUID);
                paymentMethods.Token = paymentModel.CustomerPaymentProfileId != null ? EncryptionHelper.EncryptToken(paymentModel.CustomerPaymentProfileId) : paymentModel.CustomerPaymentProfileId;
            }
            return paymentMethods;
        }


        public static List<PaymentMethodCCDetailsModel> ToPaymentMethodCCDetailsModel(List<ZnodePaymentMethod> paymentSetting)
        {
            List<PaymentMethodCCDetailsModel> paymentMethods = new List<PaymentMethodCCDetailsModel>();
            foreach (ZnodePaymentMethod item in paymentSetting)
            {
                paymentMethods.Add(new PaymentMethodCCDetailsModel
                {
                    CreditCardImageUrl = item.CreditCardImageUrl,
                    PaymentGUID = item.PaymentGUID,
                    CreditCardLastFourDigit = HelperMethods.FormatCreditCardNumber(item.CreditCardLastFourDigit),
                    CardType=item.CardType
                });
            }

            return paymentMethods;
        }

        //Convert To Payment Method ACH Details Model.
        public static List<PaymentMethodCCDetailsModel> ToPaymentMethodACHDetailsModel(List<ZnodePaymentMethod> paymentSetting)
        {
            List<PaymentMethodCCDetailsModel> paymentMethods = new List<PaymentMethodCCDetailsModel>();
            foreach (ZnodePaymentMethod item in paymentSetting)
            {
                paymentMethods.Add(new PaymentMethodCCDetailsModel
                {
                    CreditCardImageUrl = item.CreditCardImageUrl,
                    PaymentGUID = item.PaymentGUID,
                    CreditCardLastFourDigit = HelperMethods.FormatCreditCardNumber(item.CreditCardLastFourDigit),
                    Token = item.Token
                });
            }

            return paymentMethods;
        }
    }
}
