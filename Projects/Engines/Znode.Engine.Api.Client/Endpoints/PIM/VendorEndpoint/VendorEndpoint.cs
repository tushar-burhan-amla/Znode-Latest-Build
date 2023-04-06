namespace Znode.Engine.Api.Client.Endpoints
{
    public class VendorEndpoint : BaseEndpoint
    {
        //Get vendor endpoint
        public static string Get(int PimVendorId) => $"{ApiRoot}/vendor/getvendor/{PimVendorId}";

        //Create vendor endpoint
        public static string CreateVendor() => $"{ApiRoot}/vendor/create";

        //Update vendor endpoint
        public static string UpdateVendor() => $"{ApiRoot}/vendor/update";

        //Get vendor list endpoint
        public static string GetVendorList() => $"{ApiRoot}/vendor/list";

        //Delete vendor endpoint
        public static string DeleteVendor() => $"{ApiRoot}/vendor/delete";

        public static string ActiveInactiveVendor() => $"{ApiRoot}/vendor/activeinactivevendor";

        //Get vendor code list endpoint
        public static string VendorCodeList(string attributeCode) => $"{ApiRoot}/vendor/vendorcodelist/{attributeCode}";

        //Associate vendor to product
        public static string AssociateAndUnAssociateProduct() => $"{ApiRoot}/vendor/associatevendorproduct";
    }
}
