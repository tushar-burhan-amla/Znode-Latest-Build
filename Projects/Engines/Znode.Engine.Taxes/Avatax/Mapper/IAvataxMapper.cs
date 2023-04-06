using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Avalara.AvaTax.RestClient;

namespace Znode.Engine.Taxes
{
    public interface IAvataxMapper
    {
        /// <summary>
        /// To get the populated AvataxSettings model.
        /// </summary>
        /// <param name="taxportalModel">TaxPortalModel</param>
        /// <returns>Returns populated AvataxSettings model</returns>
        AvataxSettings GetAvataxSetting(TaxPortalModel taxportalModel);

        /// <summary>
        /// To map valid details in ZnodeShoppingCart model.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="taxRequest">TransactionModel</param>
        /// <param name="taxRuleId">int taxRuleId</param>
        /// <returns>Returns the data for ShoppingCart.</returns>
        void MapDetailsInShoppingCart(ZnodeShoppingCart shoppingCart, TransactionModel taxRequest, int taxRuleId);

        /// <summary>
        /// To get the populated CreateTransactionModel model for tax request.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="shippingTaxInd">bool shippingTaxInd</param>
        /// <param name="isPostSubmitCall">bool isPostSubmitCall</param>
        /// <returns>Returns the data for TransationModel.</returns>
        CreateTransactionModel SetTransationModel(ZnodeShoppingCart shoppingCart, bool shippingTaxInd, bool isPostSubmitCall = false);

        /// <summary>
        /// To get the VoidTransactionModel with valid code.
        /// </summary>
        /// <returns>Returns the data for VoidTransactionModel.</returns>
        VoidTransactionModel GetVoidTransactionModel();

        /// <summary>
        /// This method is used to set Return Line Item
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <param name="orderModel">ShoppingCartModel</param>
        /// <param name="auditTransactionResponse">AuditTransactionModel</param>
        /// <param name="avalaraTaxRequest">CreateTransactionModel</param>
        void BindReturnLineItem(ZnodeShoppingCart shoppingCart, ShoppingCartModel orderModel, AuditTransactionModel auditTransactionResponse, CreateTransactionModel avalaraTaxRequest);

        /// <summary>s
        /// To set return tax request.
        /// </summary>
        /// <param name="orderModel">ZnodeShoppingCart</param>
        /// <param name="avalaraTaxRequest">CreateTransactionModel</param>          
        void SetReturnTaxRequest(ZnodeShoppingCart orderModel, CreateTransactionModel avalaraTaxRequest);

        /// <summary>
        /// To map tax summary details in ZnodeShoppingCart.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="omsTaxOrderSummaries">TransactionModel</param>
        void MapSummaryDetailsInShoppingCart(ZnodeShoppingCart shoppingCart, TransactionModel transactionModel);

        /// <summary>
        /// To map details in ZnodeShoppingCart.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="transactionModel">TransactionModel</param>
        void MapMessageDetailsInShoppingCart(ZnodeShoppingCart shoppingCart, TransactionModel transactionModel);

        /// <summary>
        /// To bind valid values in line items of cancel request.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="auditTransactionResponse">AuditTransactionModel</param>
        /// <param name="avalaraTaxRequest">CreateTransactionModel</param>
        void BindCancelLineItemDetail(ZnodeShoppingCart shoppingCart, AuditTransactionModel auditTransactionResponse, CreateTransactionModel avalaraTaxRequest);

        /// <summary>
        /// To set cancel tax tax request.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="avalaraTaxRequest">CreateTransactionModel</param>
        void SetCancelTaxRequest(ZnodeShoppingCart shoppingCart, CreateTransactionModel avalaraTaxRequest);

        /// <summary>
        /// This method populates the CreateTransactionModel with valid details for request.
        /// </summary>
        /// <param name="shoppingCart">ZnodeShoppingCart</param>
        /// <param name="createTransactionModel">CreateTransactionModel</param>
        /// <param name="docType">DocumentType</param>
        /// <param name="isPostSubmitCall">bool</param>
        /// <returns>CreateTransactionModel</returns>
        CreateTransactionModel BuildTaxRequest(ZnodeShoppingCart shoppingCart, CreateTransactionModel createTransactionModel, DocumentType docType, bool isPostSubmitCall);
    }
}
