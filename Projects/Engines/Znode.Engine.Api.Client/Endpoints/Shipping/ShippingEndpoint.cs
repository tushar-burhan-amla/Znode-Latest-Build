namespace Znode.Engine.Api.Client.Endpoints
{
    public class ShippingEndpoint : BaseEndpoint
    {
        //Create shipping endpoint.
        public static string Create() => $"{ApiRoot}/shipping/create";

        //Get shipping list endpoint.
        public static string GetShippingList() => $"{ApiRoot}/shipping/list";

        //Delete shipping endpoint.
        public static string Delete() => $"{ApiRoot}/shipping/delete";

        //Update shipping endpoint.
        public static string Update() => $"{ApiRoot}/shipping/update";

        //Get shipping by shipping id endpoint.
        public static string GetShippingById(int shippingId) => $"{ApiRoot}/shipping/getshipping/{shippingId}";

        //Get shipping service code list endpoint.
        public static string GetShippingServiceCodeList() => $"{ApiRoot}/shippingservicecode/list";

        //Get shipping by shipping service code by id endpoint.
        public static string GetShippingServiceCodeById(int shippingserviceCodeId) => $"{ApiRoot}/shippingservicecode/getshippingservicecode/{shippingserviceCodeId}";

        #region Shipping SKU.

        //Shipping SKU List endpoint.
        public static string ShippingSKUList() => $"{ApiRoot}/shippingsku/list";


        //Add Shipping SKU endpoint.
        public static string AddShippingSKU() => $"{ApiRoot}/shippingsku/create";


        //Delete Shipping SKU endpoint.
        public static string DeleteShippingSKU() => $"{ApiRoot}/shippingsku/delete";
        #endregion

        #region Shipping Rule.

        //Shipping Rule List endpoint.
        public static string ShippingRuleList() => $"{ApiRoot}/shippingrule/list";

        //Get Shipping Rule endpoint.
        public static string GetShippingRule(int taxRuleId) => $"{ApiRoot}/shippingrule/get/{taxRuleId}";

        //Add Shipping Rule endpoint.
        public static string AddShippingRule() => $"{ApiRoot}/shippingrule/create";

        //Update Shipping Rule endpoint.
        public static string UpdateShippingRule() => $"{ApiRoot}/shippingrule/update";

        //Delete Shipping Rule endpoint.
        public static string DeleteShippingRule() => $"{ApiRoot}/shippingrule/delete";

        #endregion

        #region Shipping Rule Type

        //Get shipping rule type list endpoint.
        public static string GetShippingRuleTypeList() => $"{ApiRoot}/shippingruletype/list";

        #endregion

        #region Portal/Profile Shipping 
        //Get associated shipping list for Profile/Portal endpoint.
        public static string GetAssociatedShippingList() => $"{ApiRoot}/portalprofileshipping/getassociatedshippinglist";

        //Get list of unassociated shipping to Profile/Portal endpoint.
        public static string GetUnAssociatedShippingList() => $"{ApiRoot}/portalprofileshipping/getunassociatedshippinglist";

        //Associate shipping to Profile/Portal endpoint.
        public static string AssociateShipping() => $"{ApiRoot}/portalprofileshipping/associateshipping";

        //Remove associated shipping from Profile/Portal endpoint.
        public static string UnAssociateAssociatedShipping() => $"{ApiRoot}/portalprofileshipping/unassociateassociatedshipping";
        #endregion

        //Check shipping address is valid or not.
        public static string IsAddressValid() => $"{ApiRoot}/shipping/isshippingaddressvalid";

        // Get recommended address list.
        public static string RecommendedAddress() => $"{ApiRoot}/shipping/recommendedaddress";

        // Get recommended address list.
        public static string UpdateShippingToPortal() => $"{ApiRoot}/portalprofileshipping/UpdateShippingToPortal";

        // Update profile shipping.
        public static string UpdateProfileShipping() => $"{ApiRoot}/portalprofileshipping/updateprofileshipping";

    }
}
