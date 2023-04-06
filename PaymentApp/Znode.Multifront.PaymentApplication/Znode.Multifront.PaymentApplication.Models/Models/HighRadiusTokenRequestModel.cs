namespace Znode.Multifront.PaymentApplication.Models
{
    public class HighRadiusTokenRequestModel
    {
        public string CLIENT_SYSTEM_ID { get; set; }
        public string SECURITY_KEY { get; set; }
        public string MERCHANT_ID { get; set; }
        public string PROCESSOR { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string REQUEST_ID { get; set; }
        public string REFERENCE_NUMBER { get; set; }
        public string DATA_LEVEL { get; set; }
        public string POST_BACK_URL { get; set; }
        public string STRING_URL { get; set; }
        public string CALLING_APP { get; set; }
        public string CURRENCY_CODE { get; set; }
        public string BILL_TO_AVSCHECK_REQUIRED { get; set; }
        public string REQUESTOR { get; set; }
        public string BILL_TO_STREET1 { get; set; }
        public string BILL_TO_STREET2 { get; set; }
        public string BILL_TO_COUNTRY { get; set; }
        public string BILL_TO_CITY { get; set; }
        public string BILL_TO_STATE { get; set; }
        public string BILL_TO_FIRST_NAME { get; set; }
        public string BILL_TO_LAST_NAME { get; set; }
        public string BILL_TO_POSTAL_CODE { get; set; }
        public string PAYMENT_AMOUNT { get; set; }
        public string LANGUAGE_CODE { get; set; }
        public string CALLING_APP_RESPONSE { get; set; }
        public bool IS_CREDIT_ENABLED { get; set; }
        public bool IS_CVV_MANDATORY { get; set; }
        public string CSS_FILE_NAME { get; set; }   
    }
}
