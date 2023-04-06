namespace Znode.Engine.ABSConnector
{
    public class ABSARPaymentRequestModel : ABSRequestBaseModel
    {
        public string AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal TransactionAmount { get; set; }
        public string CreditCardType { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardExpDate { get; set; }
        public string CreditCardAuthorizedAmount { get; set; }
        public string CreditCardAuthorizationCode { get; set; }
        public string CreditCardTransactionId { get; set; }
        public string CreditCardName { get; set; }
        public string CreditCardAddress1 { get; set; }
        public string CreditCardAddress2 { get; set; }
        public string CreditCardCity { get; set; }
        public string CreditCardState { get; set; }
        public string CreditCardZip { get; set; }
        public string CustomerToken { get; set; }
        public string PaymentToken { get; set; }
        public string ECheckCheckNumber { get; set; }
        public string ECheckBankRoutingNumber { get; set; }
        public string ECheckBankAccountNumber { get; set; }
        public string ECheckCheckAmount { get; set; }
        public string ECheckTransactionId { get; set; }
        public string OpenTermsDescription { get; set; }
    }
}
