namespace Znode.Engine.Api.Client.Endpoints
{
    class BrandEndpoint : BaseEndpoint
    {
        //Get brand list endpoint
        public static string GetBrandList() => $"{ApiRoot}/brand/list";

        //Get brand endpoint
        public static string Get(int brandId, int localeId) => $"{ApiRoot}/brand/getbrand/{brandId}/{localeId}";

        //Create brand endpoint
        public static string Create() => $"{ApiRoot}/brand/create";

        //Update brand endpoint
        public static string Update() => $"{ApiRoot}/brand/update";

        //Delete brand endpoint
        public static string Delete() => $"{ApiRoot}/brand/delete";

        //Get brand code list endpoint
        public static string BrandCodeList(string attributeCode) => $"{ApiRoot}/brand/brandcodelist/{attributeCode}";
        
        //Associate brand to product
        public static string AssociateAndUnAssociateProduct() => $"{ApiRoot}/brand/associatebrandproduct";

        //Active/Inactive Brands
        public static string ActiveInactiveBrand() => $"{ApiRoot}/brand/activeinactivebrand";

        //Get Portal List for Brands
        public static string GetPortalList() => $"{ApiRoot}/brand/getbrandportallist";

        //Associate brand to portal
        public static string AssociateAndUnAssociatePortal() => $"{ApiRoot}/brand/associatebrandportal";

        //Check unique brand code. 
        public static string CheckUniqueBrandCode(string code) => $"{ApiRoot}/brand/checkuniquebrandcode/{code}";

        //Associate / UnAssociate brand to portal
        public static string AssociateAndUnAssociatePortalBrand() => $"{ApiRoot}/brand/associateandunassociateportalbrand";

        //Associate / UnAssociate portal brandlist.
        public static string GetPortalBrandList() => $"{ApiRoot}/brand/portalbrandlist";

        // Update Associate portal brand.
        public static string UpdateAssociatedPortalBrandDetail() => $"{ApiRoot}/brand/updateassociatedportalbranddetail";

        
    }
}
