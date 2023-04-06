using ZNode.Libraries.Data;
using System.Linq;
using ZNode.Libraries.Data.DataModel;
using ZNode.Libraries.ECommerce.Entities;
using ZNode.Libraries.ECommerce.Utilities;
using static ZNode.Libraries.ECommerce.Utilities.HelperUtility;

namespace ZNode.Libraries.Admin
{
    public class ZnodePaymentHelper : ZNodePayment
    {
        #region Member Variables

        #endregion

        #region Public Methods

        /// <summary>
        /// This method orchestrates the payment submission process
        /// </summary>
        /// <returns>Returns the payment response</returns>
        public virtual ZNodePaymentResponse SubmitPayment(string paymentTypeName, bool isPreAuthorize, string token)
        {
            // submit card payment
            if (string.IsNullOrEmpty(paymentTypeName))
            {
                ZNodePaymentResponse PaymentResponse = new ZNodePaymentResponse();
                PaymentResponse.IsSuccess = true;
                return PaymentResponse;
            }
            else if(paymentTypeName.Equals(ZNodePaymentTypeEnum.CREDIT_CARD.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return this.GetPaymentResponse((isPreAuthorize) ? ZnodePaymentStatusEnum.CC_AUTHORIZED : ZnodePaymentStatusEnum.CC_CAPTURED, token);
            else if (paymentTypeName.Equals(ZNodePaymentTypeEnum.PAYPAL_EXPRESS.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return this.GetPaymentResponse((isPreAuthorize) ? ZnodePaymentStatusEnum.CC_AUTHORIZED : ZnodePaymentStatusEnum.CC_CAPTURED, token);
            else if (paymentTypeName.Equals(ZNodePaymentTypeEnum.AMAZON_PAY.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return this.GetPaymentResponse((isPreAuthorize) ? ZnodePaymentStatusEnum.CC_AUTHORIZED : ZnodePaymentStatusEnum.CC_CAPTURED, token);
            else if (paymentTypeName.Equals(ZNodePaymentTypeEnum.COD.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return this.GetPaymentResponse(ZnodePaymentStatusEnum.COD_PENDING, null);
            else if (paymentTypeName.Equals(ZNodePaymentTypeEnum.PURCHASE_ORDER.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return this.GetPaymentResponse(ZnodePaymentStatusEnum.PO_PENDING, null);             
            else
            {
                ZNodePaymentResponse pr = new ZNodePaymentResponse();
                return pr;
            }
        }

        public virtual int? GetPaymentStatusId(string paymentStatus)
        {
            IZnodeRepository<ZnodeOmsPaymentState> _paymentStateRepository = new ZnodeRepository<ZnodeOmsPaymentState>();
           return _paymentStateRepository.Table.FirstOrDefault(x => x.Name == paymentStatus)?.OmsPaymentStateId;
        }

        #endregion

        #region Private Method
        /// <summary>
        /// Get payment response. 
        /// </summary>
        /// <returns>Returns the Payment Response</returns>
        protected virtual ZNodePaymentResponse GetPaymentResponse(ZnodePaymentStatusEnum paymentType, string token)
        {
            ZNodePaymentResponse PaymentResponse = new ZNodePaymentResponse();
            PaymentResponse.IsSuccess = true;
            PaymentResponse.TransactionId = token;
            PaymentResponse.PaymentStatus = paymentType;

            return PaymentResponse;
        }

        #endregion
    }
}
