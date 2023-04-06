using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IProductsClient : IBaseClient
    {
        #region Product

        /// <summary>
        /// Get product by ProductId
        /// </summary>
        /// <param name="model">PIMGetProductModel.</param>
        /// <returns>productmodel</returns>
        PIMFamilyDetailsModel GetProduct(PIMGetProductModel model);


        /// <summary>
        ///  Get product list.  
        /// </summary>
        /// <param name="expands">Expands for Product</param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <returns>Returns ProductDetailsListModel</returns>
        ProductDetailsListModel GetProducts(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get product list.
        /// </summary>
        /// <param name="expands">Expands for Product  </param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ProductDetailsListModel</returns>
        ProductDetailsListModel GetProducts(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///  Get product list.  
        /// </summary>
        /// <param name="productIds">Product Ids for parameter.</param>
        /// <param name="expands">Expands for Product</param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <returns>Returns ProductDetailsListModel</returns>
        ProductDetailsListModel GetProducts(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get product list.
        /// </summary>
        /// <param name="productIds">Product Ids for parameter.</param>
        /// <param name="expands">Expands for Product  </param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ProductDetailsListModel</returns>
        ProductDetailsListModel GetProducts(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get product list.
        /// </summary>
        /// <param name="productIds">Product Ids for parameter.</param>
        /// <param name="expands">Expands for Product  </param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ProductDetailsListModel</returns>
        ProductDetailsListModel GetProductsToBeAssociated(string productIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets list of products to be associated or unassociated with parent Product.
        /// </summary>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="associatedProductIds">Product IDs to be associated.</param>
        /// <param name="associatedAttributeIds">associatedAttributeIds to be associated.</param>
        /// <param name="filters">Filters for products to be associated with parent Product.</param>
        /// <param name="sorts">Sorts for products to be associated with parent Product.</param>
        /// <param name="pageIndex">Page Index for products to be associated with parent Product.</param>
        /// <param name="pageSize">Page size for products to be associated with parent Product.</param>
        /// <returns></returns>
        ProductDetailsListModel GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create Product
        /// </summary>
        /// <param name="model">ProductModel</param>
        /// <returns>Returns ProductModel</returns>
        ProductModel CreateProduct(ProductModel model);

        /// <summary>
        /// Delete existing product
        /// </summary>
        /// <param name="productIds">product id</param>
        /// <returns>true or false</returns>
        bool DeleteProduct(string productIds);



        /// <summary>
        ///  Update assign product display order.
        /// </summary>
        /// <param name="linkProductDetailModel"></param>
        /// <returns>true/false response</returns>
        LinkProductDetailModel UpdateAssignLinkProducts(LinkProductDetailModel linkProductDetailModel);



        /// <summary>
        /// Get products by category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProduts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <returns>CategoryProductListModel</returns>
        CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, ExpandCollection expands, FilterCollection filters, SortCollection sorts);


        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProduts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>CategoryProductListModel</returns>
        CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        ///  Get Personalized Attribute list.  
        /// </summary>
        /// <param name="expands">Expands for Product</param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <returns>Returns PIMProductAttributeValuesListModel</returns>
        PIMProductAttributeValuesListModel GetAssignedPersonalizedAttributes(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        PIMAttributeListModel GetUnassignedPersonalizedAttributes(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Assign personalized attributes
        /// </summary>
        /// <param name="model">PIM attribute value list model.</param>
        /// <param name="filters">Filter Collection.</param>
        /// <returns>Returns True or false.</returns>
        bool AssignPersonalizedAttributes(PIMAttributeValueListModel model, FilterCollection filters);

        /// <summary>
        /// Unassign personalized attributes.
        /// </summary>
        /// <param name="parameter">Parameter Model containing personalized attribute IDs to be unassociated.</param>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <returns>Returns True is Deleted successfully;False if delete action fails.</returns>
        bool UnassignPersonalizedAttributes(ParameterModel parameter, int parentProductId);

        /// <summary>
        ///  Get Product Attributes and Family as per selected Family.
        /// </summary>
        /// <param name="model">PIMFamilyModel</param>
        /// <returns>Returns PIMFamilyDetailsModel.</returns>
        PIMFamilyDetailsModel GetProductFamilyDetails(PIMFamilyModel model);

        /// <summary>
        ///  Get all configure attributes associated to family with provided familyID
        /// </summary>
        /// <param name="pimFamilyModel"></param>
        /// <returns>PIMFamily Details Model model.</returns>
        PIMFamilyDetailsModel GetConfigureAttribute(PIMFamilyModel pimFamilyModel);

        /// <summary>
        ///  Get product list.  
        /// </summary>
        /// <param name="productIds">product ids seperated by commas</param>
        /// <param name="expands">Expands for Product</param>
        /// <param name="filters">Filters for Product</param>
        /// <param name="sorts">Sorts for Product</param>
        /// <returns>Returns ProductDetailsListModel</returns>
        ProductDetailsListModel GetBrandProducts(string productIds,ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        #endregion

        #region Product Type

        /// <summary>
        /// Gets list of associated products as per parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <param name="expands">Expands for associated product list.</param>
        /// <param name="filters">Filters for associated product list.</param>
        /// <param name="sorts">Sorts for associated product list.</param>
        /// <param name="pageIndex">Start page index of product associated product list.</param>
        /// <param name="pageSize">Page size of product associated product list.</param>
        /// <returns>Product Details List Model.</returns>
        ProductDetailsListModel GetAssociatedProducts(int parentProductId, int attributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of associated products as per parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <param name="expands">Expands for associated product list.</param>
        /// <param name="filters">Filters for associated product list.</param>
        /// <param name="sorts">Sorts for associated product list.</param>
        /// <param name="pageIndex">Start page index of product associated product list.</param>
        /// <param name="pageSize">Page size of product associated product list.</param>
        /// <returns>Product Details List Model.</returns>
        ProductDetailsListModel GetAssociatedProducts(int parentProductId, int attributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets list of products those are not associated with parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="associatedProductIds">Already associated product Ids.</param>
        /// <param name="expands">Expands for unassociated product list.</param>
        /// <param name="filters">Filters for unassociated product list.</param>
        /// <param name="sorts">Sorts for unassociated product list.</param>
        /// <returns>Product Details List Model.</returns>
        ProductDetailsListModel GetUnassociatedProducts(int parentProductId, string associatedProductIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of products those are not associated with parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="associatedProductIds">Already associated product Ids.</param>
        /// <param name="expands">Expands for unassociated product list.</param>
        /// <param name="filters">Filters for unassociated product list.</param>
        /// <param name="sorts">Sorts for unassociated product list.</param>
        /// <param name="pageIndex">Start page index of product unassociated product list.</param>
        /// <param name="pageSize">Page size of product unassociated product list.</param>
        /// <returns>Product Details List Model.</returns>
        ProductDetailsListModel GetUnassociatedProducts(int parentProductId, string associatedProductIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Adds product type association entries.
        /// </summary>
        /// <param name="model">Product Type Association List Model to be created.</param>
        /// <returns>Created ProductTypeAssociationList model.</returns>
        ProductTypeAssociationListModel AssociateProducts(ProductTypeAssociationListModel model);

        /// <summary>
        /// Removes a product type association entry.
        /// </summary>
        /// <param name="productTypeAssociationId">product type association Ids to be deleted.</param>
        /// <returns>True/False value according to the status of delete operation.</returns>
        bool UnassociateProduct(ParameterModel productTypeAssociationId);

        #endregion

        #region Link products

        /// <summary>
        /// Gets list of products which are associated as link product to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link attribute Id.</param>
        /// <param name="expands">Expands for Associated link product list.</param>
        /// <param name="filters">Filters for Associated link product list.</param>
        /// <param name="sorts">Sorts for Associated link product list.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetAssignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of products which are associated as Add-ons to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link attribute Id.</param>
        /// <param name="expands">Expands for Associated link product list.</param>
        /// <param name="filters">Filters for Associated link product list.</param>
        /// <param name="sorts">Sorts for Associated link product list.</param>
        /// <param name="pageIndex">Start page index of product associated link product list.</param>
        /// <param name="pageSize">Page size of product associated link product list.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetAssignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets list of list of products which are not associated as link products to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link attribute Id.</param>
        /// <param name="expands">Expands for unassociated link product list.</param>
        /// <param name="filters">Filters for unassociated link product list.</param>
        /// <param name="sorts">Sorts for unassociated link product list.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetUnassignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of list of products which are not associated as link products to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link attribute Id.</param>
        /// <param name="expands">Expands for unassociated link product list.</param>
        /// <param name="filters">Filters for unassociated link product list.</param>
        /// <param name="sorts">Sorts for unassociated link product list.</param>
        /// <param name="pageIndex">Start page index of product unassociated link product list.</param>
        /// <param name="pageSize">Page size of product unassociated link product list.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetUnassignedLinkProducts(int parentProductId, int linkAttributeId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Adds a product link product association entry.
        /// </summary>
        /// <param name="model">List of link product detail model to be created.</param>
        /// <returns>Created link product detail model.</returns>
        LinkProductDetailListModel AssignLinkProducts(LinkProductDetailListModel model);

        /// <summary>
        /// Removes a product link product association entry.
        /// </summary>
        /// <param name="linkProductDetailId">Link product detail ID to be deleted.</param>
        /// <returns>Boolean value True/False in response.</returns>
        bool UnassignLinkProducts(ParameterModel linkProductDetailId);
        #endregion

        #region CustomFields
        /// <summary>
        /// Add Custom Field to Product
        /// </summary>
        /// <param name="customFieldModel">CustomFieldModel to create custom field</param>
        /// <returns>Added custom field</returns>
        CustomFieldModel AddCustomField(CustomFieldModel customFieldModel);

        /// <summary>
        /// Get Custom Field by customFieldId
        /// </summary>
        /// <param name="customFieldId">Id of the Custom Field.</param>
        /// <param name="expands">expands</param>
        /// <returns>productmodel</returns>
        CustomFieldModel GetCustomField(int customFieldId, ExpandCollection expands);

        /// <summary>
        /// Get Custom Field list.
        /// </summary>
        /// <param name="expands">Expands for Custom Field  </param>
        /// <param name="filters">Filters for Custom Field</param>
        /// <param name="sorts">Sorts for Custom Field</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>ProductDetailsListModel</returns>
        CustomFieldListModel GetCustomFields(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update product.
        /// </summary>
        /// <param name="customFieldModel">customFieldModel to updateexisting custom field.</param>
        /// <returns>Returns CustomFieldModel</returns>
        CustomFieldModel UpdateCustomField(CustomFieldModel customFieldModel);

        /// <summary>
        /// Delete existing CustomField
        /// </summary>
        /// <param name="customFieldId">customFieldId to delete custom filed</param>
        /// <returns>true or false</returns>
        bool DeleteCustomField(string customFieldId);

        #endregion


        /// <summary>
        /// Get associated product
        /// </summary>
        /// <param name="pimProductTypeAssociationId">AssociatedProductId to get associated product</param>
        /// <returns>Product Type Association Model</returns>
        ProductTypeAssociationModel GetAssociatedProduct(int pimProductTypeAssociationId);

        /// <summary>
        /// Update associated Product
        /// </summary>
        /// <param name="productTypeAssociationModel">ProductTypeAssociationModel</param>
        /// <returns>Product Type Association Model</returns>
        ProductTypeAssociationModel UpdateAssociatedProduct(ProductTypeAssociationModel productTypeAssociationModel);

        /// <summary>
        /// Activate/Deactivate bulk product
        /// </summary>
        /// <param name="productIds">product ids seperated by commas</param>
        /// <param name="isActive">True/False</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns></returns>
        bool ActivateDeactivateProducts(string productIds, bool isActive, int localeId);

        #region Associate Addons
        /// <summary>
        /// Associates an add-on group to a product.
        /// </summary>
        /// <param name="model">Addon product list model.</param>
        /// <returns>Associated addon product list.</returns>
        AddonProductListModel AssociateAddon(AddonProductListModel model);

        /// <summary>
        /// Deletes association of product and add-on group.
        /// </summary>
        /// <param name="addonProductId">Add-on product ID.</param>
        /// <returns>True if addon product is deleted;False if addon product fails to delete.</returns>
        bool DeleteAssociatedAddons(ParameterModel addonProductId);

        /// <summary>
        /// Gets list of associated addon group with associated child products as addons.
        /// </summary>
        /// <param name="parentProductId">Parent Product ID.</param>
        /// <param name="expands">Expands for  associated addon group with associated child products as addons</param>
        /// <param name="filters">Filters for associated addon group with associated child products as addons </param>
        /// <param name="sorts">Sorts for associated addon group with associated child products as addons</param>
        /// <returns>List of associated addon group along with child products associated as addons.</returns>
        AddonGroupListModel GetAssociatedAddonDetails(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Creates association of associated addon group and child product as add-on to parent product.
        /// </summary>
        /// <param name="parentProductId">Addon Product detail ID.</param>
        /// <param name="parentProductId">Parent Product ID.</param>
        /// <param name="expands">Expands for  associated addon group with associated child products as addons</param>
        /// <param name="filters">Filters for associated addon group with associated child products as addons </param>
        /// <param name="sorts">Sorts for associated addon group with associated child products as addons</param>
        /// <param name="pageIndex">Page index for addon group with associated child products as addons</param>
        /// <param name="pageSize">Page size for addon group with associated child products as addons</param>
        /// <returns>Addon Group list model.</returns>
        AddonGroupListModel GetAssociatedAddonDetails(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Creates association of associated addon group and child product as add-on to parent product.
        /// </summary>
        /// <param name="model">Addon Product detail ID.</param>
        /// <returns> Add-on product detail list model</returns>
        AddOnProductDetailListModel CreateAddonProductDetail(AddOnProductDetailListModel model);

        /// <summary>
        /// Deletes association of associated addon group and child product as add-on to parent product.
        /// </summary>
        /// <param name="addonProductDetailIds">Addon Product detail IDs to be deleted.</param>
        /// <returns>True if addon product details is deleted;False if addon product details fails to delete.</returns>
        bool DeleteAddonProductDetails(ParameterModel addonProductDetailIds);

        /// <summary>
        /// Gets list of unassociated addon groups.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="expands">Expands for unassociated addon group list.</param>
        /// <param name="filters">Filters for unassociated addon group list.</param>
        /// <param name="sorts">Sorts for unassociated addon group list.</param>
        /// <returns>List of unassociated add-on groups.</returns>
        AddonGroupListModel GetUnassociatedAddonGroups(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of unassociated addon groups.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="expands">Expands for unassociated addon group list.</param>
        /// <param name="filters">Filters for unassociated addon group list.</param>
        /// <param name="sorts">Sorts for unassociated addon group list.</param>
        /// <param name="pageIndex">Page index for addon group list.</param>
        /// <param name="pageSize">Page size for addon group list.</param>
        /// <returns>List of unassociated add-on groups.</returns>
        AddonGroupListModel GetUnassociatedAddonGroups(int parentProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets list of unassociated child products as add-ons.
        /// </summary>
        /// <param name="addonProductId">Add-on Product ID.</param>
        /// <param name="expands">Expands for unassociated addon group list.</param>
        /// <param name="filters">Filters for unassociated child products as add-ons.</param>
        /// <param name="sorts">Sorts for unassociated child products as add-ons.</param>
        /// <returns>Unassociated child products as add-ons</returns>
        ProductDetailsListModel GetUnassociatedAddonProducts(int addonProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets list of unassociated child products as add-ons.
        /// </summary>
        /// <param name="addonProductId">Add-on Product ID.</param>
        /// <param name="expands">Expands for unassociated child products as add-ons.</param>
        /// <param name="filters">Filters for unassociated child products as add-ons.</param>
        /// <param name="sorts">Sorts for unassociated child products as add-ons.</param>
        /// <param name="pageIndex">Page index for unassociated child products as add-ons.</param>
        /// <param name="pageSize">Page size for unassociated child products as add-ons.</param>
        /// <returns>Unassociated child products as add-ons</returns>
        ProductDetailsListModel GetUnassociatedAddonProducts(int addonProductId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Updates product-addon association data.
        /// </summary>
        /// <param name="addonProductModel">Addon product model.</param>
        /// <returns>Updated Add-on product model.</returns>
        AddOnProductModel UpdateProductAddonAssociation(AddOnProductModel addonProductModel);
        #endregion

        #region Publish Product.
        /// <summary>
        /// Publish Product.
        /// </summary>
        /// <param name="parameterModel">Parameter model containing product id for publishing.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel PublishProduct(ParameterModel parameterModel);

        #endregion

        #region Product SKU list for Autocomplete feature
        /// <summary>
        /// Get product sku list by attribute code as SKU and attribute value. 
        /// </summary>
        /// <param name="attributeValue">Attribute value to get in list.</param>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns product sku list.</returns>
        PIMProductAttributeValuesListModel GetProductSKUsByAttributeCode(string attributeValue, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update Addon Display Order
        /// </summary>
        /// <param name="addOnProductDetailModel">Addon product model containing display order and other values </param>
        /// <returns>Return Addon product model</returns>
        AddOnProductDetailModel UpdateAddonDisplayOrder(AddOnProductDetailModel addOnProductDetailModel);

        #endregion

        #region Product Update Import
        /// <summary>
        /// Post and process the update product import data
        /// </summary>
        /// <param name="model">Import Model</param>
        /// <returns>bool</returns>
        bool ImportProductUpdateData(ImportModel model);
        #endregion
    }
}
