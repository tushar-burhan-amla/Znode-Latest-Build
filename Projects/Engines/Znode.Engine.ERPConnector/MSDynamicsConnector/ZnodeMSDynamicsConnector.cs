namespace Znode.Engine.ERPConnector
{
    public class ZnodeMSDynamicsConnector : BaseERP
    {
        public ZnodeMSDynamicsConnector()
        {
        }

        //AR History from ERP to ZNODE
        public override bool ARHistory() => true;

        //AR Payment Details from ERP to ZNODE
        public override bool ARPaymentDetails() => true;

        //AR Balance from ERP to ZNODE
        public override bool ARBalance() => true;

        //Refresh Product Content from ERP to Znode
        public override bool ProductRefresh() => true;

        //Refresh Category from ERP to Znode
        public override bool CategoryRefresh() => true;

        //Refresh Product Category Link from ERP to Znode                            
        public override bool ProductCategoryLinkRefresh() => true;

        //Refresh  Contact List from ERP to Znode          
        public  override bool ContactListRefresh() => true;

        //Get Contact List on real-time from ERP to Znode   
        public override bool GetContactList() => true;

        //Refresh Contact Details from ERP to Znode           
        public override bool ContactDetailsRefresh() => true;

        //Get Contact Details on real-time from   ERP to Znode  
        public override bool GetContactDetails() => true;

        //Get Login from ZNODE to ERP
        public override bool Login() => true;

        //Create Contact from ZNODE to ERP
        public override bool CreateContact() => true;

        //Create Update from ZNODE to ERP
        public override bool UpdateContact() => true;

        public override bool PaymentAuthorization() => true;
        public override bool SaveCreditCard() => true;

        //Refresh customer details from ERP to Znode
        public override bool CustomerDetailsRefresh() => true;

        //Get Customer Details on real-time from   ERP to Znode  
        public override bool GetCustomerDetails() => true;

        //Refresh ShipToList from ERP to Znode
        public override bool ShipToListRefresh() => true;

        //Get ShipToList on real-time from   ERP to Znode  
        public override bool GetShipToList() => true;

        //Locate My Account Or Match Customer from Znode to ERP
        public override bool LocateMyAccountOrMatchCustomer() => true;

        //Create Customer from Znode to ERP
        public override bool CreateCustomer() => true;

        //Update Customer from Znode to ERP
        public override bool UpdateCustomer() => true;

        //Create ShipTo from Znode to ERP
        public override bool CreateShipTo() => true;

        //Update ShipTo from Znode to ERP
        public override bool UpdateShipTo() => true;

        //Refresh inventory for products by warehouse/DC from ERP to Znode on a scheduled basis.
        public override bool InventoryRefresh() => true;

        //Get inventory on real-time from ERP to Znode              
        public override bool GetInventoryRealtime() => true;

        //Get Invoice History from ERP to Znode       
        public override bool InvoiceHistory() => true;

        //Get Invoice Details Status from ERP to Znode       
        public override bool InvoiceDetailsStatus() => true;

        public override bool RequestACatalog() => true;

        //Submit A Prospect from ERP to Znode       
        public override bool SubmitAProspect() => true;

        //OrderSimulate from Znode  to ERP      
        public override bool OrderSimulate() => true;

        //Create Order from Znode  to ERP   
        public override bool OrderCreate() => true;

        //Get Order History from ERP to Znode       
        public override bool OrderHistory() => true;

        //Get Order Details Status from ERP to Znode       
        public override bool OrderDetailsStatus() => true;

        //Pay Online from Znode to ERP
        public override bool PayOnline() => true;

        //Refresh standard price list from ERP to Znode on a scheduled basis.
        public override bool PricingStandardPriceListRefresh() => true;

        //Refresh customer / contract price list from ERP to Znode on a scheduled basis.
        public override bool PricingCustomerPriceListRefresh() => true;

        //Get list or customer specific price on real-time from ERP to Znode      
        public override bool GetPricing() => true;

        //Create Quote from Znode  to ERP   
        public override bool QuoteCreate() => true;

        //Get Quote History from ERP to Znode 
        public override bool QuoteHistory() => true;

        //Get Quote Details Status from ERP to Znode 
        public override bool QuoteDetailsStatus() => true;

        public override bool ShippingOptions() => true;
        public override bool ShippingNotification() => true;

        //Tax calculation from ERP to Znode 
        public override bool TaxCalculation() => true;
    }
}
