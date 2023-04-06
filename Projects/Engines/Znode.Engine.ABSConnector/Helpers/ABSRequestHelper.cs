namespace Znode.Engine.ABSConnector
{
    public class ABSRequestHelper
    {
        //Get the request XMLs.
        public string GetRequestXML<T>(T requestModel, string requestType)
        {
            switch (requestType)
            {
                case ABSRequestTypes.ARList:
                    return ARListRequestXML(requestModel as ABSARListRequestModel);
                case ABSRequestTypes.ARPayment:
                    return ARPaymentRequestXML(requestModel as ABSARPaymentRequestModel);
                case ABSRequestTypes.Email:
                    return EmailUpdateRequestXML(requestModel as ABSEmailRequestModel);
                case ABSRequestTypes.Inventory:
                    return InventoryRequestXML(requestModel as ABSInventoryRequestModel);
                case ABSRequestTypes.OrderNumber:
                    return NextOrderNumberRequestXML(requestModel as ABSOrderNumberRequestModel);
                case ABSRequestTypes.OrderHistory:
                    return OrderHistoryRequestXML(requestModel as ABSOrderInfoRequestModel);
                case ABSRequestTypes.PriceTier:
                    return PriceTierRequestXML(requestModel as ABSTierPriceRequestModel);
                case ABSRequestTypes.PDFPrint:
                    return PrintInfoRequestXML(requestModel as ABSPrintInfoRequestModel);
                case ABSRequestTypes.ShipToHold:
                    return ShipToChangeRequestXML(requestModel as ABSChangeShipToRequestModel);
                case ABSRequestTypes.BillToAddress:
                    return BillToChangeRequestXML(requestModel as ABSBillToChangeRequestModel);
            }
            return null;
        }

        //AR List Request XML.
        private string ARListRequestXML(ABSARListRequestModel model)
            => $"<?xml version=\"1.0\" encoding=\"UTF-8\" ?><ArListInfoRequest><RequestInfo><Company>{model.Company}</Company><Division>{model.Division}</Division><SoldTo>{model.SoldTo}</SoldTo><NumberOfRecords>{model.NumberOfRecords}</NumberOfRecords><CurrentRecord>{model.CurrentRecord}</CurrentRecord><ShipTo>{model.ShipTo}</ShipTo></RequestInfo></ArListInfoRequest>";

        //AR Payment Request XML.
        private string ARPaymentRequestXML(ABSARPaymentRequestModel model)
           => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><ArPaymentRequest><PaymentDetail><Company>{model.Company}</Company><Division>{model.Division}</Division><AccountNumber>{model.AccountNumber}</AccountNumber><TransactionType>{model.TransactionType}</TransactionType><ReferenceNumber>{model.ReferenceNumber}</ReferenceNumber><TransactionAmount>{model.TransactionAmount}</TransactionAmount><CreditCardType>{model.CreditCardType}</CreditCardType><CreditCardNumber>{model.CreditCardNumber}</CreditCardNumber><CreditCardExpDate>{model.CreditCardExpDate}</CreditCardExpDate><CreditCardAuthorizedAmount>{model.CreditCardAuthorizedAmount}</CreditCardAuthorizedAmount><CreditCardAuthorizationCode>{model.CreditCardAuthorizationCode}</CreditCardAuthorizationCode><CreditCardTransactionId>{model.CreditCardTransactionId}</CreditCardTransactionId><CreditCardName>{model.CreditCardName}</CreditCardName><CreditCardAddress1>{model.CreditCardAddress1}</CreditCardAddress1><CreditCardAddress2>{model.CreditCardAddress2}</CreditCardAddress2><CreditCardCity>{model.CreditCardCity}</CreditCardCity><CreditCardState>{model.CreditCardState}</CreditCardState><CreditCardZip>{model.CreditCardZip}</CreditCardZip><CustomerToken>{model.CustomerToken}</CustomerToken><PaymentToken>{model.PaymentToken}</PaymentToken><ECheckCheckNumber>{model.ECheckCheckNumber}</ECheckCheckNumber><ECheckBankRoutingNumber>{model.ECheckBankRoutingNumber}</ECheckBankRoutingNumber><ECheckBankAccountNumber>{model.ECheckBankAccountNumber}</ECheckBankAccountNumber><ECheckCheckAmount>{model.ECheckCheckAmount}</ECheckCheckAmount><ECheckTransactionId>{model.ECheckTransactionId}</ECheckTransactionId></PaymentDetail><PaymentDetail><Company>{model.Company}</Company><Division>{model.Division}</Division><AccountNumber>{model.AccountNumber}</AccountNumber><TransactionType>{model.TransactionType}</TransactionType><ReferenceNumber>{model.ReferenceNumber}</ReferenceNumber><TransactionAmount>{model.TransactionAmount}</TransactionAmount></PaymentDetail></ArPaymentRequest>";

        //Email Update Request XML.
        private string EmailUpdateRequestXML(ABSEmailRequestModel model)
          => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><ChangeEmailAddressRequest><ChangeInfo><Company>{model.Company}</Company><Division>{model.Division}</Division><SoldTo>{model.SoldTo}</SoldTo><EmailAddressType>{model.EmailAddressType}</EmailAddressType><EmailAddress>{model.EmailAddress}</EmailAddress></ChangeInfo></ChangeEmailAddressRequest>";

        //Inventory Request XML.
        private string InventoryRequestXML(ABSInventoryRequestModel model)
          => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><InventoryRequest><Item><Company>{model.Company}</Company><Division>{model.Division}</Division><Season>{model.Season}</Season><Seayr>{model.Seayr}</Seayr><Style>{model.Style}</Style><Color>{model.Color}</Color><Piece>{model.Piece}</Piece><Dim>{model.Dim}</Dim><UpcNumber>{model.UpcNumber}</UpcNumber></Item></InventoryRequest>";

        //Next Order Number Request XML.
        private string NextOrderNumberRequestXML(ABSOrderNumberRequestModel model)
          => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><NextOrderNumberRequest><RequestInfo><Company>{model.Company}</Company><Division>{model.Division}</Division></RequestInfo></NextOrderNumberRequest>";

        //Order History Request XML.
        private string OrderHistoryRequestXML(ABSOrderInfoRequestModel model)
          => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><OrderInfoRequest><RequestInfo><Company>{model.Company}</Company><Division>{model.Division}</Division><SoldTo>{model.SoldTo}</SoldTo><PoNumber>{model.PoNumber}</PoNumber><SummaryRequested>{model.SummaryRequested}</SummaryRequested><SummaryDate>{model.SummaryDate}</SummaryDate><OrderNumberRequested>{model.OrderNumberRequested}</OrderNumberRequested><NumberOfRecords>{model.NumberOfRecords}</NumberOfRecords><CurrentRecord>{model.CurrentRecord}</CurrentRecord><ShipTo>{model.ShipTo}</ShipTo></RequestInfo></OrderInfoRequest>";

        //Price Tier Request XML.
        private string PriceTierRequestXML(ABSTierPriceRequestModel model)
          => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><PriceTierChangeRequest><Company>{model.Company}</Company><Division>{model.Division}</Division><Account>{model.Account}</Account><NewPriceTier>{model.NewPriceTier}</NewPriceTier></PriceTierChangeRequest>";

        //Bill To Change Request XML.
        private string BillToChangeRequestXML(ABSBillToChangeRequestModel model)
         => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><BillToInformation><Company>{model.Company}</Company><Division>{model.Division}</Division><BillToAccount>{model.BillToAccount}</BillToAccount><BillToName>{model.BillToName}</BillToName><BillToAddress1>{model.BillToAddress1}</BillToAddress1><BillToAddress2>{model.BillToAddress2}</BillToAddress2><BillToAddress3>{model.BillToAddress3}</BillToAddress3><BillToCity>{model.BillToCity}</BillToCity><BillToState>{model.BillToState}</BillToState><BillToZip>{model.BillToZip}</BillToZip><BillToPhone>{model.BillToPhone}</BillToPhone><BillToFax>{model.BillToFax}</BillToFax><BillToCountry>{model.BillToCountry}</BillToCountry></BillToInformation>";

        //Ship To Change Request XML.
        private string ShipToChangeRequestXML(ABSChangeShipToRequestModel model)
         => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><ChangeShipToRequest><ChangeInfo><Company>{model.Company}</Company><Division>{model.Division}</Division><SoldTo>{model.SoldTo}</SoldTo><ShipTo>{model.ShipTo}</ShipTo></ChangeInfo></ChangeShipToRequest>";

        //Retrieve Price Info XML.
        private string RetrievePriceInfoXML(ABSPriceRequestModel model)
          => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><RetrievePriceInformation><Company>{model.Company}</Company><Division>{model.Division}</Division><Account>{model.Account}</Account><TierPriceCode>{model.TierPriceCode}</TierPriceCode><UpcCode>{model.UpcCode}</UpcCode></RetrievePriceInformation>";

        //Print Info Request XML.
        private string PrintInfoRequestXML(ABSPrintInfoRequestModel model)
        => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><PrintInfoRequest><RequestInfo><Company>{model.Company}</Company><Division>{model.Division}</Division><SoldTo>{model.SoldTo}</SoldTo><ReferenceNumber>{model.ReferenceNumber}</ReferenceNumber><BillOfLading>{model.BillOfLading}</BillOfLading><ReferenceType>{model.ReferenceType}</ReferenceType><EmailAddress>{model.EmailAddress}</EmailAddress></RequestInfo></PrintInfoRequest>  ";

        //Sold Out Reasons XML.
        private string SoldOutReasonsXML(ABSSoldOutRequestModel model)
         => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><SoldoutReasons><Company>{model.Company}</Company><Division>{model.Division}</Division><Season>{model.Season}</Season><Seayr>{model.Seayr}</Seayr><Style>{model.Style}</Style><Color>{model.Color}</Color><Piece>{model.Piece}</Piece><Dim>{model.Dim}</Dim></SoldoutReasons>";
    }
}
