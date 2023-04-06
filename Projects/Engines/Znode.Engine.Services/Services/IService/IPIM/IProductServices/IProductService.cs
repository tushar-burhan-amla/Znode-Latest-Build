using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IProductService
    {
        #region Product
        /// <summary>
        /// Get product list from database.
        /// </summary>
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns ProductDetailsListModel</returns>
        ProductDetailsListModel GetProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of products according to parameter model.
        /// </summary>
        /// <param name="model">Parameter model containing product IDs</param>
        /// <param name="expands">Expands for product list.</param>
        /// <param name="filters">filters for product list.</param>
        /// <param name="sorts">Sorts for product list.</param>
        /// <param name="page">Pagination value for product list.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetProductList(ParameterModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of products according to parameter model.
        /// </summary>
        /// <param name="expands">Expands for product list.</param>
        /// <param name="filters">filters for product list.</param>
        /// <param name="sorts">Sorts for product list.</param>
        /// <param name="page">Pagination value for product list.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetProductBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Save/Update product details in database.
        /// </summary>
        /// <param name="model">ProductModel</param>
        /// <returns>Returns ProductModel</returns>
        ProductModel CreateProduct(ProductModel model);

        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="model">PIMGetProductModel</param>
        /// <returns>ProductModel</returns>
        PIMFamilyDetailsModel GetProduct(PIMGetProductModel model);

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="productId">product id</param>
        /// <returns>True/false</returns>
        bool DeleteProduct(ParameterModel productIds);

        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProduts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <param name="expands">Expands to be retrieved along with category list.</param>
        /// <param name="filters">Filters to be applied on category list.</param>
        /// <param name="sorts">Sorting to be applied on category list.</param>
        /// <param name="page">Page Number</param>
        /// <returns>Category Product ListModel</returns>
        CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get assigned personalized attribute list.
        /// </summary>
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns PIMProductAttributeValuesListModel</returns>
        PIMProductAttributeValuesListModel GetAssignedPersonalizedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts);

        /// <summary>
        /// Assign personalized attributes
        /// </summary>
        /// <param name="model">PIM attribute value list model.</param>
        /// <param name="filters">Filter Collection.</param>
        /// <returns>Returns true or false.</returns>
        bool AssignPersonalizedAttributes(PIMAttributeValueListModel model, FilterCollection filters);

        /// <summary>
        /// Get Product Attributes as per selected Family.
        /// </summary>
        /// <param name="model">PIMFamilyModel</param>
        /// <returns>Returns PIMFamilyDetailsModel.</returns>
        PIMFamilyDetailsModel GetProductFamilyDetails(PIMFamilyModel model);

        /// <summary>
        /// Get all configure attributes associated with familyID
        /// </summary>
        /// <param name="pimFamilyModel"></param>
        /// <returns></returns>
        PIMFamilyDetailsModel GetConfigureAttributes(PIMFamilyModel pimFamilyModel);

        /// <summary>
        /// Gets list of associated products as per selected productId on basis of associatedAttributeIds selected.
        /// </summary>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="associatedProductIds">Product IDs to be associated.</param>
        /// <param name="associatedAttributeIds">associatedAttributeIds to be associated.</param>
        /// <param name="expands">Expands for associated products.</param>
        /// <param name="filters">Filters for associated products.</param>
        /// <param name="sorts">Sorts for associated products.</param>
        /// <param name="page">Page values for associated products.</param>
        /// <returns>ProductDetailsListModel.</returns>
        ProductDetailsListModel GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Activate/Deactivate products in bulk
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ActivateDeactivateProducts(ActivateDeactivateProductsModel model);

        /// <summary>
        /// Update Product Attribute Code Value.
        /// </summary>
        /// <param name="model">Model with attribute code and values.</param>
        /// <returns>Returns model with status.</returns>
        ProductAttributeCodeValueListModel UpdateProductAttributeValue(AttributeCodeValueModel model);
        #endregion

        #region ProductType

        /// <summary>
        /// Gets list of associated products as per selected parent productId.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <param name="expands">Expands for associated products.</param>
        /// <param name="filters">Filters for associated products.</param>
        /// <param name="sorts">Sorts for associated products.</param>
        /// <param name="page">Page values for associated products.</param>
        /// <returns>ProductDetailsListModel.</returns>
        ProductDetailsListModel GetAssociatedProducts(int parentProductId, int attributeId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Send mail to a friend.
        /// </summary>
        /// <param name="model">EmailAFriendListModel</param>
        /// <returns>EmailAFriendListModel</returns>
        bool SendMailToFriend(EmailAFriendListModel model);

        /// <summary>
        /// Gets list of unassociated products as per selected parent productId.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="alreadyAssociatedProductIds">Already associated product Id</param>
        /// <param name="expands">Expands for unassociated products.</param>
        /// <param name="filters">Filters for unassociated products.</param>
        /// <param name="sorts">Sorts for unassociated products.</param>
        /// <param name="page">Page values for unassociated products.</param>
        /// <returns>ProductDetailsListModel.</returns>
        ProductDetailsListModel GetUnassociatedProducts(int parentProductId, string alreadyAssociatedProductIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Adds product type association entries.
        /// </summary>
        /// <param name="listModel">Product Type List Association Model to be added.</param>
        /// <returns>True/False value according the status of assignment operation.</returns>
        bool AssociateProduct(ProductTypeAssociationListModel listModel);

        /// <summary>
        /// Removes a product type association entry.
        /// </summary>
        /// <param name="productTypeAssociationId">ID of product type association to be deleted.</param>
        /// <returns>True if product type association is removed;False if removal of product type association failed.</returns>
        bool UnassociateProduct(ParameterModel productTypeAssociationId);

        #endregion

        #region Link Products
        /// <summary>
        /// Gets list of products which are associated as Link to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link Attribute Id.</param>
        /// <param name="expands">Expands for associated link products.</param>
        /// <param name="filters">Filters for associated link products.</param>
        /// <param name="sorts">Sorts for associated link products.</param>
        /// <param name="page">Page values for associated link products.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetAssociatedLinkProducts(int parentProductId, int linkAttributeId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of list of products which are not associated as Link to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link Attribute Id.</param>
        /// <param name="expands">Expands for unassociated link products.</param>
        /// <param name="filters">Filters for unassociated link products.</param>
        /// <param name="sorts">Sorts for unassociated link products.</param>
        /// <param name="page">Page values for unassociated link products.</param>
        /// <returns>Product list model.</returns>
        ProductDetailsListModel GetUnAssociatedLinkProducts(int parentProductId, int linkAttributeId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Adds a product Link association entry.
        /// </summary>
        /// <param name="model">Link details model to be added.</param>
        /// <returns>True if link products are associated successfully; False if link products are failed to be associated.</returns>
        bool AssignLinkProduct(LinkProductDetailListModel model);

        /// <summary>
        /// Removes a product link association entry.
        /// </summary>
        /// <param name="linkProductDetailId">ID of the link product details to be deleted.</param>
        /// <returns>True if product link association is removed;False if removal of product link association is failed.</returns>
        bool UnassignLinkProduct(ParameterModel linkProductDetailId);


        /// <summary>
        /// Update assign product display Order.
        /// </summary>
        /// <param name="LinkProductDetailModel"></param>
        /// <returns>true or false response</returns>
        bool UpdateAssignLinkProducts(LinkProductDetailModel linkProductDetailModel);



        #endregion

        #region Custom Field
        /// <summary>
        /// Add Custom Field Locale to Product.
        /// </summary>
        /// <param name="customFieldModel">CustomFieldModel to create custom field</param>
        /// <returns>Added custom field</returns>
        CustomFieldModel AddCustomField(CustomFieldModel customFieldModel);

        /// <summary>
        /// Get Custom Field list from database.
        /// </summary>
        /// <param name="expands">Expands collection.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns CustomFieldListModel</returns>
        CustomFieldListModel GetCustomFieldList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Custom Field by Custom Field id.
        /// </summary>
        /// <param name="customFieldId">Get Custom Field on the basis of customFieldId.</param>
        /// <param name="expands">Expands for CustomField.</param>
        /// <returns>Returns CustomFieldModel.</returns>
        CustomFieldModel GetCustomField(int customFieldId, NameValueCollection expands);

        /// <summary>
        /// Update CustomField on the basis of customFieldId.
        /// </summary>
        /// <param name="customFieldModel">CustomFieldModel to update CustomField.</param>
        /// <returns>Returns CustomFieldModel.</returns>
        bool UpdateCustomField(CustomFieldModel customFieldModel);

        /// <summary>
        /// Delete CustomField on the basis of CustomField Id.
        /// </summary>
        /// <param name="customFieldId">CustomField Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteCustomField(ParameterModel customFieldId);

        /// <summary>
        /// Gets list of unassigned personalized attributes.
        /// </summary>
        /// <param name="expands">Expands for unassigned personalized attributes.</param>
        /// <param name="filters">Filters for unassigned personalized attributes.</param>
        /// <param name="sorts"></param>
        /// <returns>Unassigned personalized attributes.</returns>
        PIMAttributeListModel GetUnassignedPersonalizedAttributes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts);
        #endregion

        /// <summary>
        /// Get associated product By PimProductTypeAssociationId
        /// </summary>
        /// <param name="PimProductTypeAssociationId">PimProductTypeAssociationId to get associated product</param>
        /// <returns>Product Type Association Model</returns>
        ProductTypeAssociationModel GetAssociatedProduct(int PimProductTypeAssociationId);

        /// <summary>
        /// Update associated product
        /// </summary>
        /// <param name="productTypeAssociationModel">ProductTypeAssociationModel to be updated</param>
        /// <returns>Return True/False</returns>
        bool UpdateAssociatedProduct(ProductTypeAssociationModel productTypeAssociationModel);

        /// <summary>
        /// Unassign personalized attributes.
        /// </summary>
        /// <param name="parameters">Parameter containing attribute IDs to be unassigned.</param>
        /// <param name="pimParentProductId">Parent product ID.</param>
        /// <returns>Returns True if Deleted successfully;False if delete action fails.</returns>
        bool UnassignPersonalizedAttributes(ParameterModel parameters, int pimParentProductId);

        /// <summary>
        /// Add entry if personalized attributes local againt product.
        /// </summary>
        /// <param name="assignPersonalizedAttributes">list of assign personalize attributes</param>
        void AddPersonalizedAttributesInLocale(List<PIMProductAttributeValuesModel> assignPersonalizedAttributes);

        #region Publish Product
        /// <summary>
        ///  Publish Product.
        /// </summary>
        /// <param name="parameterModel">Product Id to be published.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel Publish(ParameterModel parameterModel);
        #endregion

        #region Associated add-ons
        /// <summary>
        /// Associates an add-on group to a product.
        /// </summary>
        /// <param name="addonProducts">Addon product list model.</param>
        /// <returns>True if addon group is associated;False if addon group fails to associate.</returns>
        bool AssociateAddon(AddonProductListModel addonProducts);

        /// <summary>
        /// Deletes association of product and add-on group.
        /// </summary>
        /// <param name="addonProductIds">Addon product IDs to be deleted.</param>
        /// <returns>True if addon product is deleted;False if addon product fails to delete.</returns>
        bool DeleteAssociatedAddons(ParameterModel addonProductIds);

        /// <summary>
        /// Gets list of associated addon group with associated child products as addons.
        /// </summary>
        /// <param name="productId">Parent Product ID.</param>
        /// <param name="expands">Expands for associated addon group list.</param>
        /// <param name="filters">Filters for associated addon group list.</param>
        /// <param name="page">Pagination values for associated addon group list.</param>
        /// <param name="sorts">Sorts for associated addon group list.</param>
        /// <returns>List of associated addon group along with child products associated as addons.</returns>
        AddonGroupListModel GetAssociatedAddonDetails(int productId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Creates association of associated addon group and child product as add-on to parent product.
        /// </summary>
        /// <param name="addonProductDetails">Addon Product detail ID.</param>
        /// <returns>True if addon product details is created;False if addon product details fails to create.</returns>
        bool CreateAddonProductDetail(AddOnProductDetailListModel addonProductDetails);

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
        /// <param name="page">Pagination values for unassociated addon group list.</param>
        /// <param name="sorts">Sorts for unassociated addon group list.</param>
        /// <returns>List of unassociated add-on groups.</returns>
        AddonGroupListModel GetUnassociatedAddonGroups(int parentProductId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of unassociated child products as add-ons.
        /// </summary>
        /// <param name="addonProductId"></param>
        /// <param name="expands">Expands for unassociated child products as add-ons.</param>
        /// <param name="filters">Filters for unassociated child products as add-ons.</param>
        /// <param name="page">Pagination values for unassociated child products as add-ons.</param>
        /// <param name="sorts">Sorts for unassociated child products as add-ons.</param>
        /// <returns>Unassociated child products as add-ons</returns>
        ProductDetailsListModel UnassociatedAddonProducts(int addonProductId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Updates Product- Addon group association data.
        /// </summary>
        /// <param name="addonProductModel">Addon product model.</param>
        /// <returns>True if product-addon association is updated;False if product-addon association fails to update.</returns>
        bool UpdateProductAddonAssociation(AddOnProductModel addonProductModel);

        #endregion

        #region Product SKU list for Autocomplete feature
        /// <summary>
        /// Get product sku list by attribute code as SKU and attribute value. 
        /// </summary>
        /// <param name="attributeValue">Attribute value to get in list.</param>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sorts</param>
        /// <param name="page">Name value collection.</param>
        /// <returns>Returns product sku list.</returns>
        PIMProductAttributeValuesListModel GetProductSKUsByAttributeCode(string attributeValue, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion

        #region WebStore Product
        /// <summary>
        /// get list of product for webstore
        /// </summary>
        /// <param name="expands">expand for product list.</param>
        /// <param name="filters">filter for product list.</param>
        /// <param name="sorts">sorts for products</param>
        /// <param name="page">paging parameter for product list</param>
        /// <returns>product list model</returns>
        WebStoreProductListModel ProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="productId">product id</param>
        /// <param name="expands">expands to get product</param>
        /// <returns>product model</returns>
        WebStoreProductModel GetProduct(int productId, NameValueCollection expands);

        /// <summary>
        /// get associated products
        /// </summary>
        /// <param name="productIDs">products ids</param>
        /// <returns>product list model</returns>
        WebStoreProductListModel GetAssociatedProducts(ParameterModel productIDs);

        /// <summary>
        /// Get product list by skus.
        /// </summary>
        /// <param name="skus">Multiple sku</param>
        /// <returns>Product list by skus</returns>
        WebStoreProductListModel GetProductsBySkus(ParameterModel skus);

        /// <summary>
        /// Get Product HighLights
        /// </summary>
        /// <param name="parameterModel">Parameter  model with highlights codes</param>
        /// <param name="productId"></param>
        /// <returns>HighlightListModel</returns>
        HighlightListModel GetProductHighlights(ParameterProductModel parameterModel, int productId);

        /// <summary>
        /// Send Compare Product Mail.
        /// </summary>
        /// <param name="model">Model with compare product data.</param>
        /// <returns>True/False</returns>
        bool SendComparedProductMail(ProductCompareModel model);

        /// <summary>
        /// Update Addon Display Order
        /// </summary>
        /// <param name="addOnProductDetailModel">Addon product model containing display order and other values</param>
        /// <returns>True/False</returns>
        bool UpdateAddonDisplayOrder(AddOnProductDetailModel addOnProductDetailModel);

        #endregion

        #region Product Update Import
        /// <summary>
        /// Process the data from the file.  
        /// This method will fetch the product import data from file and insert it into DB and then
        /// inserted data will be processed.
        /// </summary>
        /// <param name="importModel">ImportModel</param>
        /// <returns>ImportModel</returns>
        int ProcessProductUpdateData(ImportModel importModel);

        #endregion

        ProductDetailsListModel GetXmlProduct(FilterCollection filters, PageListModel pageListModel, string pimProductIdsNotIn, bool pimProductIdsIn = false);

        DataSet GetXmlProductsDataSet(FilterCollection filters, PageListModel pageListModel, string pimProductIdsNotIn, ref bool pimProductIdsIn);

    }
}
