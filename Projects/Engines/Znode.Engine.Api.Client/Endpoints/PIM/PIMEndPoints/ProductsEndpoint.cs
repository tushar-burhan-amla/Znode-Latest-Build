namespace Znode.Engine.Api.Client.Endpoints
{
    public class ProductsEndpoint : BaseEndpoint
    {
        #region Product
        //Get Product list endpoint.
        public static string List() => $"{ApiRoot}/product/list";

        //Get Product by Product Id endpoint.
        public static string Get() => $"{ApiRoot}/products/getproduct";

        //Create new Product endpoint.
        public static string Create() => $"{ApiRoot}/products";

        //Delete Product by ProductIds endpoint.
        public static string Delete(string productId) => $"{ApiRoot}/products/delete";

        //Update assign product display order.
        public static string UpdateAssignLinkProducts() => $"{ApiRoot}/products/updateassignlinkproducts";


        //Get all configure attributes associated with family with provided familyID.
        public static string GetConfigureAttributes() => $"{ApiRoot}/products/getconfigureattributes";

        //Get Product By Category
        public static string GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts) => $"{ApiRoot}/products/getassociatedunassociatedcategoryproducts/{categoryId}/{associatedProduts}";

        //Get Product Attribute By Family
        public static string GetProductFamilyDetails() => $"{ApiRoot}/products/getproductfamilydetails";

        //Get Personalized attribute list.
        public static string GetAssignedPersonalizedAttributes() => $"{ApiRoot}/products/assignedpersonalizedattribute/list";

        //Gets unassigned personalized attribute list.
        public static string GetUnassignedPersonalizedAttributes() => $"{ApiRoot}/products/unassignedpersonalizedattribute/list";

        //Get Assign personalized attributes
        public static string AssignPersonalizedAttributes() => $"{ApiRoot}/products/assignpersonalizedattributes";

        //Activate/Deactivate bulk product
        public static string ActivateDeactivateProducts() => $"{ApiRoot}/products/activatedeactivateproducts";

        //Get Product by Product Id endpoint.
        public static string GetBrandProductList() => $"{ApiRoot}/product/getbrandproductlist";
        #endregion

        #region Product Type
        //Gets products associated with parent product.
        public static string GetAssociatedProducts(int parentProductId, int attributeId) => $"{ApiRoot}/associatedproductlist/{parentProductId}/{attributeId}";

        //Gets products those are not associated with parent product.
        public static string GetUnassociatedProducts(int parentProductId) => $"{ApiRoot}/unassociatedproductlist/{parentProductId}";

        //Adds product type association.
        public static string AssociateProducts() => $"{ApiRoot}/associateproduct";

        //Deletes product type association.
        public static string UnassociateProduct() => $"{ApiRoot}/unassociateproduct";
        #endregion

        #region Link Product
        //Gets associated link product for the parent product.
        public static string GetAssociatedLinkProducts(int parentProductId, int linkAttributeId) => $"{ApiRoot}/linkproductdetails/associated/{parentProductId}/{linkAttributeId}";

        //Get unassociated link product for the parent product.
        public static string GetUnassociatedLinkProducts(int parentProductId, int linkAttributeId) => $"{ApiRoot}/linkproductdetails/unassociated/{parentProductId}/{linkAttributeId}";

        //Creates a product and link product association.
        public static string AssignLinkProducts() => $"{ApiRoot}/linkproductdetails";

        //Deletes a product link product association.
        public static string UnassignLinkProducts() => $"{ApiRoot}/linkproductdetails/delete";
        #endregion

        //Associates addons.
        public static string AssociateAddon() => $"{ApiRoot}/product/associateaddon";

        //Deletes association of addons from product.
        public static string DeleteAssociatedAddons() => $"{ApiRoot}/product/deleteassociatedaddons";

        //Gets associated add-ons details.
        public static string GetAssociatedAddonDetails(int parentProductId) => $"{ApiRoot}/product/getassociatedaddondetails{parentProductId}";

        //Creates association of child products as add-ons to parent product.
        public static string CreateAddonProductDetail() => $"{ApiRoot}/product/createaddonproductdetail";

        //Deletes addon product details.
        public static string DeleteAddonProductDetails() => $"{ApiRoot}/product/deleteaddonproductdetails";

        //Get list of unassociated addon groups from parent product.
        public static string GetUnassociatedAddonGroups(int parentProductId) => $"{ApiRoot}/product/getunassociatedaddongroups/{parentProductId}";

        //Gets list of unassociated products as addons.
        public static string GetUnassociatedAddonProducts(int addonProductId) => $"{ApiRoot}/product/getunassociatedaddonproducts/{addonProductId}";

        //Updates product-Addon association.
        public static string UpdateProductAddonAssociation() => $"{ApiRoot}/product/updateaddonproductassociation";

        //Update Addon Display Order 
        public static string UpdateAddonDisplayOrder() => $"{ApiRoot}/product/updateaddondisplayorder";
        #region Custom Field
        //Create CustomField endpoint
        public static string CreateCustomField() => $"{ApiRoot}/customfield/create";

        //Get CustomField endpoint
        public static string GetCustomField(int customFieldId) => $"{ApiRoot}/customfield/get/{customFieldId}";

        //CustomField List endpoint
        public static string CustomFieldList() => $"{ApiRoot}/customfield/list";

        //Update CustomField endpoint
        public static string UpdateCustomField() => $"{ApiRoot}/customfield/update";

        //Delete CustomField endpoint
        public static string DeleteCustomField() => $"{ApiRoot}/customfield/delete";
        #endregion

        //Get associated product by PimProductTypeAssociationId
        public static string GetAssociatedProduct(int PimProductTypeAssociationId) => $"{ApiRoot}/products/getassociatedproduct/{PimProductTypeAssociationId}";

        //Update associated Product
        public static string UpdateAssociatedProduct() => $"{ApiRoot}/products/updateAssociatedproduct";

        //Unassociate personalized attribute for a parent product.
        public static string UnassignPersonalizedAttributes(int parentProductId) => $"{ApiRoot}/products/unassignpersonalizedattributes/{parentProductId}";

        //Publish Product endpoint.
        public static string Publish() => $"{ApiRoot}/products/publish";

        //Gets products according to product Ids.
        public static string List(string productIds) => $"{ApiRoot}/products/{productIds}";

        //Gets products according to product Ids.
        public static string GetProductsToBeAssociated() => $"{ApiRoot}/products/GetProductsToBeAssociated";

        // Method for Get Configure Products To Be Associated or unassociated
        public static string GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn) => $"{ApiRoot}/GetAssociatedUnAssociatedConfigureProducts/{parentProductId}/{associatedProductIds}/{associatedAttributeIds}/{pimProductIdsIn}";

        #region Product SKU list for Autocomplete feature
        //Get Product SKUs By Attribute Code and Attribute Value.
        public static string GetProductSKUsByAttributeCode(string attributeValue) => $"{ApiRoot}/products/getproductskusbyattributecode/{attributeValue}";
        #endregion

        #region Product Update Import
        //Post and process the update product import data.
        public static string ImportProductUpdateData() => $"{ApiRoot}/products/importproductupdatedata";
        #endregion
    }
}
