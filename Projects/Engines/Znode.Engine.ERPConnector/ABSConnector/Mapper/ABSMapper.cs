using System;
using System.Linq;
using Znode.Engine.ABSConnector;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.ERPConnector
{
    public static class ABSMapper
    {
        public static void MapABSARListRequestModel(ABSARListRequestModel param)
        {
            param.Company = "AB";
            param.Division = "01";
            param.SoldTo = "5611";
            param.NumberOfRecords = 25;
            param.CurrentRecord = 0;
            param.ShipTo = "0";
        }

        public static ABSARPaymentRequestModel MapABSARPaymentRequestModel(OrderModel model)
        {
            return new ABSARPaymentRequestModel
            {
                Company = "90",
                Division = "01",
                AccountNumber = "100",
                TransactionType = "I",
                ReferenceNumber = "16279",
                TransactionAmount = Math.Round(model.Total, 2) ,
                CreditCardType = "V",
                CreditCardNumber = model.CreditCardNumber,
                PaymentToken = model.PaymentTransactionToken,
            };
        }

        public static ABSInventoryRequestModel MapABSInventoryRequestModel(InventorySKUModel param)
        {
            return new ABSInventoryRequestModel
            {
                Company = "90",
                Division = "01",
                UpcNumber = param.SKU,
            };
        }

        public static void MapABSPriceRequestModel(ABSTierPriceRequestModel param)
        {
            param.Company = "AB";
            param.Division = "01";
            param.Account = "1234567";
            param.NewPriceTier = "X";
        }

        public static void MapABSPrintInfoRequestModel(ABSPrintInfoRequestModel param)
        {
            param.Company = "90";
            param.Division = "01";
            param.EmailAddress = "michell@apparelbusiness.com";
            param.ReferenceNumber = "16193";
            param.ReferenceType = "I";
            param.SoldTo = "100";
        }

        public static ABSEmailRequestModel MapABSEmailRequestModel(ZnodeUser entity)
        {
            return new ABSEmailRequestModel
            {
                Company = "AB",
                Division = "01",
                EmailAddressType = "*ESTA",
                EmailAddress = entity.Email,
                SoldTo = "1234567",
            };
        }

        public static ABSBillToChangeRequestModel MapABSBillToChangeRequestModel(ZnodeAddress entity)
        {
            return new ABSBillToChangeRequestModel
            {
                Company = "90",
                Division = "01",
                BillToAccount = entity.ZnodeUserAddresses.Count > 0 ? entity.ZnodeUserAddresses?.FirstOrDefault()?.UserId : entity.ZnodeAccountAddresses?.FirstOrDefault()?.AccountId,
                BillToAddress1 = entity.Address1,
                BillToAddress2 = entity.Address2,
                BillToAddress3 = entity.Address3,
                BillToCity = entity.CityName,
                BillToCountry = entity.CountryName,
                BillToFax = entity.FaxNumber,
                BillToName = $"{entity.FirstName} {entity.LastName}",
                BillToPhone = entity.PhoneNumber,
                BillToState = entity.StateName,
                BillToZip = entity.PostalCode,
            };
        }

        public static ABSOrderNumberRequestModel MapABSOrderNumberRequestModel(SubmitOrderModel entity)
        {
            return new ABSOrderNumberRequestModel
            {
                Company = "90",
                Division = "01",
            };
        }

        public static void MapABSOrderInfoRequestModel(ABSOrderInfoRequestModel param)
        {
            param.Company = "AB";
            param.Division = "01";
            param.CurrentRecord = "";
            param.NumberOfRecords = 25;
            param.OrderNumberRequested = "2051";
            param.PoNumber = "";
            param.SoldTo = "5611";
            param.SummaryRequested = "O";
            param.ShipTo = "";
            param.SummaryDate = "";
        }
    }
}
