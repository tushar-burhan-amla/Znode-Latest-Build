namespace Znode.Engine.Api.Client.Endpoints
{
    public class CustomerEndpoint : BaseEndpoint
    {
        #region Profile Association
        //Get list of unassociate profiles.
        public static string GetUnAssociatedProfileList() => $"{ApiRoot}/customer/getunassociatedprofilelist";

        //Get associated profile list based on customer.
        public static string GetAssociatedProfilelist() => $"{ApiRoot}/customer/getassociatedprofilelist";

        //Unassociate profiles by profile id.
        public static string UnAssociateProfiles(int userId) => $"{ApiRoot}/customer/unassociateprofiles/{userId}";

        //Associate profiles by profile id.
        public static string AssociateProfiles() => $"{ApiRoot}/customer/associateprofiles";

        //Set default profile for user.
        public static string SetDefaultProfile() => $"{ApiRoot}/customer/setdefaultprofile";

        //Get associated profile list based on portal.
        public static string GetCustomerPortalProfilelist() => $"{ApiRoot}/customer/getcustomerportalprofilelist";

        #endregion

        #region Affiliate
        //Get list of referral commission type.
        public static string GetReferralCommissionTypeList() => $"{ApiRoot}/customer/getreferralcommissiontypelist";

        //Get customer affiliate data. 
        public static string GetCustomerAffiliate(int userId) => $"{ApiRoot}/customer/getcustomeraffiliate/{userId}";

        //Update customer affiliate data. 
        public static string UpdateCustomerAffiliate() => $"{ApiRoot}/customer/updatecustomeraffiliate";

        //Get referral commission list.
        public static string GetReferralCommissionlist() => $"{ApiRoot}/customer/getreferralcommissionlist";
        #endregion

        #region Address
        //Get customer address list.
        public static string GetAddressList() => $"{ApiRoot}/customer/addresslist";

        //Create customer list.
        public static string CreateCustomerAddress() => $"{ApiRoot}/customer/createcustomeraddress";

        //Get customer address.
        public static string GetCustomerAddress() => $"{ApiRoot}/customer/getcustomeraddress";

        //Update customer address.
        public static string UpdateCustomerAddress() => $"{ApiRoot}/customer/updatecustomeraddress";

        //Delete customer address.
        public static string DeleteCustomerAddress() => $"{ApiRoot}/customer/deletecustomeraddress";

        //Get search location.
        public static string GetSearchLocation(int portalId, string searchTerm) => $"{ApiRoot}/customer/getsearchlocation/{portalId}/{searchTerm}";

        //Update address.
        public static string UpdateSearchAddress() => $"{ApiRoot}/customer/updatesearchaddress";

        #endregion

        #region Associate Price
        //Endpoint to Associate Price List.
        public static string AssociatePriceList() => $"{ApiRoot}/customer/associatepricelist";

        //Remove associated price lists from customer endpoint.
        public static string UnAssociatePriceList() => $"{ApiRoot}/customer/unassociatepricelist";

        //Get associated price list precedence data for customer endpoint.
        public static string GetAssociatedPriceListPrecedence() => $"{ApiRoot}/customer/getassociatedpricelistprecedence";

        //Update associated price list precedence data for customer endpoint.
        public static string UpdateAssociatedPriceListPrecedence() => $"{ApiRoot}/customer/updateassociatedpricelistprecedence";
        #endregion
    }
}
