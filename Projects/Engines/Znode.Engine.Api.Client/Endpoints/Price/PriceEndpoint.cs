namespace Znode.Engine.Api.Client.Endpoints
{
    public class PriceEndpoint : BaseEndpoint
    {
        //Create Price Endpoint.
        public static string CreatePrice() => $"{ApiRoot}/price/create";

        //Get Price on the basis of Price list id Endpoint.
        public static string GetPrice(int priceListId) => $"{ApiRoot}/price/getprice/{priceListId}";

        //Update Price Endpoint.
        public static string UpdatePrice() => $"{ApiRoot}/price/update";

        //Get Price list Endpoint.
        public static string GetPriceList() => $"{ApiRoot}/pricelist";

        //Delete price Endpoint.
        public static string Delete() => $"{ApiRoot}/price/delete";

        //Copy price Endpoint.
        public static string Copy() => $"{ApiRoot}/price/copy";

        #region SKU Price.
        //Add SKU Price endpoint.
        public static string AddSKUPrice() => $"{ApiRoot}/skuprice/create";

        //Get SKU Price endpoint.
        public static string GetSKUPrice(int priceId) => $"{ApiRoot}/skuprice/get/{priceId}";

        //SKU Price List endpoint.
        public static string SKUPriceList() => $"{ApiRoot}/skuprice/list";
                            
        //Update SKU Price endpoint.
        public static string UpdateSKUPrice() => $"{ApiRoot}/skuprice/update";

        //Get price by sku endpoint.
        public static string GetPriceBySku() => $"{ApiRoot}/skuprice/getpricebysku";

        //Get pricing details by sku endpoint.
        public static string GetProductPricingDetailsBySku() => $"{ApiRoot}/skuprice/getproductpricingdetailsbysku";

        //Delete SKU Price endpoint.
        public static string DeleteSKUPrice() => $"{ApiRoot}/skuprice/delete";

        //Uom Price List endpoint.
        public static string UomList() => $"{ApiRoot}/uom/list";

        //SKU and its Price List endpoint.
        public static string GetPagedSkuPrice() => $"{ApiRoot}/price/getpagedpricesku";
        #endregion

        #region Tier Price
        //Add Tier Price endpoint.
        public static string AddTierPrice() => $"{ApiRoot}/tierprice/create";

        //Get Tier Price endpoint.
        public static string GetTierPrice(int tierPriceId) => $"{ApiRoot}/tierprice/get/{tierPriceId}";

        //Tier Price List endpoint.
        public static string TierPriceList() => $"{ApiRoot}/tierprice/list";

        //Update Tier Price endpoint.
        public static string UpdateTierPrice() => $"{ApiRoot}/tierprice/update";

        //Delete Tier Price endpoint.
        public static string DeleteTierPrice(int priceTierId) => $"{ApiRoot}/tierprice/delete/{priceTierId}";
        #endregion

        #region Associate Store
        //Price Portal List endpoint.
        public static string PricePortalList() => $"{ApiRoot}/associatedstore/list";

        //Get UnAssigned store list.
        public static string GetUnAssociatedStoreList() => $"{ApiRoot}/price/unassociatedstore/list";

        //Endpoint for associate store.
        public static string AssociateStore() => $"{ApiRoot}/price/associatestore";

        //Remove associated stores endpoint.
        public static string RemoveAssociatedStores() => $"{ApiRoot}/price/removeassociatedstores";

        //Get associated stores precedence endpoint.
        public static string GetAssociatedStorePrecedence(int priceListPortalId) => $"{ApiRoot}/price/getassociatedstoreprecedence/{priceListPortalId}";

        //Update  associated stores precedence endpoint.
        public static string UpdateAssociatedStorePrecedence() => $"{ApiRoot}/price/updateassociatedstoreprecedence";
        #endregion

        #region Associate Profile
        //Price Profile List endpoint.
        public static string PriceProfileList() => $"{ApiRoot}/associatedprofile/list";

        //Get UnAssigned profile list.
        public static string GetUnAssociatedProfileList() => $"{ApiRoot}/price/unassociatedprofile/list";

        //Endpoint for associate profile.
        public static string AssociateProfile() => $"{ApiRoot}/price/associateprofile";

        //Remove associated profiles endpoint.
        public static string RemoveAssociatedProfiles() => $"{ApiRoot}/price/removeassociatedprofiles";

        //Get associated profiles precedence endpoint.
        public static string GetAssociatedProfilePrecedence(int priceListProfileId) => $"{ApiRoot}/price/getassociatedprofileprecedence/{priceListProfileId}";

        //Update associated profiles precedence endpoint.
        public static string UpdateAssociatedProfilePrecedence() => $"{ApiRoot}/price/updateassociatedprofileprecedence";
        #endregion

        #region Associate Customer
        //Price Customer List endpoint.
        public static string PriceCustomerList() => $"{ApiRoot}/price/associatedcustomer/list";

        //Get UnAssigned Customer list.
        public static string GetUnAssociatedCustomerList() => $"{ApiRoot}/price/unassociatedcustomer/list";

        //Endpoint for associate Customer.
        public static string AssociateCustomer() => $"{ApiRoot}/price/associatecustomer";

        //Delete associate Customer.
        public static string DeleteAssociatedCustomer() => $"{ApiRoot}/price/associatedcustomer/delete";

        //Get associated customer precedence endpoint.
        public static string GetAssociatedCustomerPrecedence(int priceListUserId) => $"{ApiRoot}/price/getassociatedcustomerprecedence/{priceListUserId}";

        //Update  associated customer precedence endpoint.
        public static string UpdateAssociatedCustomerPrecedence() => $"{ApiRoot}/price/updateassociatedcustomerprecedence";
        #endregion

        #region Associate Account
        //Price Account List endpoint.
        public static string PriceAccountList() => $"{ApiRoot}/price/associatedaccount/list";

        //Get UnAssociated Account list endpoint.
        public static string GetUnAssociatedAccountList() => $"{ApiRoot}/price/unassociatedaccount/list";

        //Endpoint for associated Accounts.
        public static string AssociateAccount() => $"{ApiRoot}/price/associateaccount";

        //Delete associated Accounts.
        public static string RemoveAssociatedAccounts() => $"{ApiRoot}/price/associatedaccount/delete";

        //Get associated account precedence endpoint.
        public static string GetAssociatedAccountPrecedence(int priceListUserId) => $"{ApiRoot}/price/getassociatedaccountprecedence/{priceListUserId}";

        //Update associated account precedence endpoint.
        public static string UpdateAssociatedAccountPrecedence() => $"{ApiRoot}/price/updateassociatedaccountprecedence";
        #endregion

        #region Price Management
        //Get unassociated price list.
        public static string GetUnAssociatedPriceList() => $"{ApiRoot}/price/unassociatedprice/list";

        //Remove associated price lists from store endpoint.
        public static string RemoveAssociatedPriceListToStore() => $"{ApiRoot}/price/removeassociatedpricelisttostore";

        //Remove associated price lists from profile endpoint.
        public static string RemoveAssociatedPriceListToProfile() => $"{ApiRoot}/price/removeassociatedpricelisttoprofile";

        //Get associated price list precedence data for Store/Profile endpoint.
        public static string GetAssociatedPriceListPrecedence() => $"{ApiRoot}/price/getassociatedpricelistprecedence";

        //Update associated price list precedence data for Store/Profile endpoint.
        public static string UpdateAssociatedPriceListPrecedence() => $"{ApiRoot}/price/updateassociatedpricelistprecedence";
        #endregion

        public static string GetExportPriceData(string priceListIds) => $"{ApiRoot}/price/getexportpricedata/{priceListIds}";
    }
}
