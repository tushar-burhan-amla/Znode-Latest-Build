using System.Collections.Generic;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IProductAgent
    {
        /// <summary>
        /// Get the list of all products.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <param name="locale">product locale</param>
        /// <param name="pimCatalogId">pimCatalogId for catalog filter</param>
        /// <param name="catalogName">catalogName for catalog filter</param>
        /// <returns>Product List ViewModel.</returns>
        ProductDetailsListViewModel GetProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int pimCatalogId = 0, string catalogName = null, int locale=0);

        /// <summary>
        /// Get the list of all products for Export.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <param name="locale">product locale</param>
        /// <returns>Product List ViewModel.</returns>
        List<dynamic> GetExportProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int locale = 0);
        
        /// <summary>
        ///  Get Product Attributes and Family as per selected Family.
        /// </summary>
        /// <param name="familyId">Product Family ID</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>Returns PIMFamilyDetailsViewModel.</returns>
        PIMFamilyDetailsViewModel GetAttributeFamilyDetails(int familyId = 0);

        /// <summary>
        /// Gets list of assigned link products.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <param name="filters">Filters for assigned link products</param>
        /// <param name="sortCollection">Sort collection for assigned link products.</param>
        /// <param name="pageIndex">Page index for assigned link products.</param>
        /// <param name="recordPerPage">Record per page for assigned link products.</param>
        /// <returns>Product detail list model.</returns>
        ProductDetailsListViewModel GetAssignedLinkProducts(int parentProductId, int attributeId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Assigns link product to a product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribute ID.</param>
        /// <param name="productIds">Product IDs to be assigned as link products.</param>
        /// <returns>True if product IDs are associated as link products; False if product IDs fails to be associated as link products.</returns>
        bool AssignLinkProducts(int parentProductId, int attributeId, string productIds);

        /// <summary>
        /// Removes association of product and link product.
        /// </summary>
        /// <param name="linkProductDetailId">Link Product Detail Id.</param>
        /// <returns>True if a link product is successfully unassigned; False if a link product fails to ne unassigned.</returns>
        bool UnassignLinkProducts(string linkProductDetailId, out string errorMessage);


        /// <summary>
        /// Get Assigned personalized attributes list.
        /// </summary>
        /// <param name="filters">Filters</param>
        /// <param name="sortCollection">Sort Collection</param>
        /// <param name="expands">Expands</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns assigned Personalized attributes list.</returns>
        PIMProductAttributeValuesListViewModel GetAssignedPersonalizedAttributes(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get unassigned Personalized attributes.
        /// </summary>
        /// <param name="pimProductId">pim product id.</param>
        /// <returns>Returns unassigned personalized attributes.</returns>
        List<BaseDropDownList> GetUnAssignedPersonalizedAttributes(int pimProductId, ExpandCollection expands = null, FilterCollection filters = null);

        /// <summary>
        /// assign personalized attributes
        /// </summary>
        /// <param name="attributeIds">Attributes id to associate</param>
        /// <param name="pimProductId">group to associate attribute</param>
        /// <param name="message">error message</param>
        /// <returns>returns true false</returns>
        bool AssignPersonalizedAttributes(string attributeIds, int pimProductId, out string message);

        /// <summary>
        /// UnAssign personalized attributes.
        /// </summary>
        /// <param name="attributeIds">Attribute Ids to be unassigned.</param>
        /// <param name="pimProductId">PIM product ID.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns True is Deleted successfully;False if delete action fails.</returns>
        bool UnassignPersonalizeAttributes(string attributeIds, int pimProductId, out string errorMessage);

        /// <summary>
        /// Get list of products associated with parent product Id.
        /// </summary>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <param name="filters">Filters</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns associated product list.</returns>
        ProductDetailsListViewModel GetAssociatedProducts(int parentProductId, int attributeId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Adds product type association entries
        /// </summary>
        /// <param name="associatedProductIds">product ids to associate</param>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="attributeId">Attribute Id.</param>
        /// <returns>True/False value according the status of assignment operation.</returns>
        bool AssociateProducts(string associatedProductIds, int parentProductId, int attributeId);

        /// <summary>
        /// Removes a product type association entry.
        /// </summary>
        /// <param name="productTypeAssociationId">ID of product type association to be deleted.</param>
        /// <returns>True if product type association is removed;False if removal of product type association failed.</returns>
        bool UnassociatedProduct(string productTypeAssociationId);

        /// <summary>
        /// Gets list of products to be associated with parent Product.
        /// </summary>
        /// <param name="productIds">Product IDs to be associated.</param>
        /// <param name="filters">Filters for products to be associated with parent Product.</param>
        /// <param name="sorts">Sorts for products to be associated with parent Product.</param>
        /// <param name="pageIndex">Page Index for products to be associated with parent Product.</param>
        /// <param name="pageSize">Page size for products to be associated with parent Product.</param>
        /// <returns></returns>
        ProductDetailsListViewModel GetProductsToBeAssociated(string productIds, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        ///Get Configure Products To Be Associated or unassociated
        /// </summary>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="associatedProductIds">Product IDs to be associated.</param>
        /// <param name="associatedAttributeIds">associatedAttributeIds to be associated.</param>
        /// <param name="filters">Filters for products to be associated with parent Product.</param>
        /// <param name="sorts">Sorts for products to be associated with parent Product.</param>
        /// <param name="pageIndex">Page Index for products to be associated with parent Product.</param>
        /// <param name="pageSize">Page size for products to be associated with parent Product.</param>
        /// <returns></returns>
        ProductDetailsListViewModel GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Sets the filters.
        /// </summary>
        /// <param name="filters">Filter to set.</param>
        /// <param name="pimProductId">pim attribute is to set/</param>
        void SetFilters(FilterCollection filters, int pimProductId);

        /// <summary>
        /// Add new Custom Field to Product
        /// </summary>
        /// <param name="customFieldViewModel">CustomFieldViewModel model</param>
        /// <returns>Added custom field</returns>
        CustomFieldViewModel AddCustomField(CustomFieldViewModel customFieldViewModel);

        /// <summary>
        /// Get all locales.
        /// </summary>
        /// <returns>LocaleListViewModel model.</returns>
        LocaleListViewModel GetLocalList();

        /// <summary>
        /// Get all the CustomFields from specified customfield id
        /// </summary>
        /// <param name="productId">product id to get the associated CustomField.</param>
        /// <param name="expands">expands across CustomFieldlocales.</param>
        /// <param name="filters">filter list across CustomField.</param>
        /// <param name="sortCollection">sort collection for CustomField.</param>
        /// <param name="pageIndex">pageIndex for CustomField record. </param>
        /// <param name="recordPerPage">paging CustomField record per page.</param>
        /// <returns></returns>
        CustomFieldListViewModel GetCustomFields(int productId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Error view model custom field.
        /// </summary>
        /// <param name="customFieldViewModel">custom field model</param>
        /// <param name="message">error message</param>
        /// <returns>view model with error details and model data.</returns>
        CustomFieldViewModel GetErrorViewModel(CustomFieldViewModel customFieldViewModel, string message);

        /// <summary>
        /// Get Existing CustomField by CustomField id.
        /// </summary>
        /// <param name="customFieldId">customFieldId to get the CustomField.</param>
        /// <returns>Returns CustomFieldViewModel</returns>
        CustomFieldViewModel GetCustomField(int customFieldId, ExpandCollection expands = null);

        /// <summary>
        /// Update Existing CustomField.
        /// </summary>
        /// <param name="customFieldViewModel">CustomFieldViewModel to update CustomField.</param>
        /// <returns>Returns true if CustomField Updated else returns false.</returns>
        CustomFieldViewModel UpdateCustomField(CustomFieldViewModel customFieldViewModel);

        /// <summary>
        /// Delete Existing CustomField by customFieldId Id.
        /// </summary>
        /// <param name="customFieldId">customFieldId Id of CustomField to be deleted.</param>
        /// <returns>Returns true if CustomField Deleted else returns false.</returns>
        bool DeleteCustomField(string customFieldId, out string message);

        /// <summary>
        /// Get Locales
        /// </summary>
        /// <returns>CustomField locales.</returns>
        CustomFieldViewModel GetLocales();

        /// <summary>
        /// Save Product Details
        /// </summary>
        /// <param name="viewModelList"></param>
        /// <returns>Created model.</returns>
        ProductViewModel CreateProduct(BindDataModel viewModelList, out string message);

        /// <summary>
        /// Get Product details from Product Id.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns>PIMFamilyDetailsViewModel</returns>
        PIMFamilyDetailsViewModel GetProduct(int productId, bool isCopy);

        /// <summary>
        /// Delete existing product
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns>True/false</returns>
        bool DeleteProduct(string ProductId);

        /// <summary>
        /// Update Addon Display Order
        /// </summary>
        /// <param name="pimAddonProductDetailId">Addon product model containing display order and other values</param>
        /// <param name="data">data contain parameters required to update display order</param>
        /// <returns></returns>
        AddonProductDetailViewModel UpdateAddonDisplayOrder(int pimAddonProductDetailId, string data);


        /// <summary>
        /// Edit assign link products
        /// </summary>
        /// <param name="linkProductDetailViewModel"></param>
        /// <returns></returns>
        bool EditAssignLinkProducts(LinkProductDetailViewModel linkProductDetailViewModel);



        /// <summary>
        ///  Get all attributes associated with productId with default family and provided familyID
        /// </summary>
        /// <param name="pimProductId"></param>
        /// <param name="familyId"></param>
        /// <param name="localeId"></param>
        /// <returns>PIMFamilyDetailsViewModel</returns>
        PIMFamilyDetailsViewModel GetProductAttributes(int pimProductId, int familyId);

        /// <summary>
        /// Get list of attribute Associated with family
        /// </summary>
        /// <param name="familyId"></param>
        /// <returns>PIMFamilyDetailsViewModel</returns>
        PIMFamilyDetailsViewModel GetConfigureAttribute(int familyId, int productId);

        /// <summary>
        /// Gets list of unassociated products.
        /// </summary>
        /// <param name="parentProductId">parent Product ID.</param>
        /// <param name="associatedProductIds">Already associated product Ids.</param>
        /// <param name="listType">Type of list: Addon, Product Type, Link Product.</param>
        /// <param name="filters">Filters for unassociated products.</param>
        /// <param name="sorts">Sort for unassociated products.</param>
        /// <param name="pageIndex">Start page index.</param>
        /// <param name="pageSize">Record per page count of the list.</param>
        /// <returns>Product detail list view model.</returns>
        ProductDetailsListViewModel GetUnassociatedProducts(int parentProductId, string associatedProductIds, int addonProductId, int listType, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get associated Product
        /// </summary>
        /// <param name="PimProductTypeAssociationId">PimProductTypeAssociationId to get AssociatedProduct</param>
        /// <returns>ProductTypeAssociationViewModel</returns>
        ProductTypeAssociationViewModel GetAssociatedProduct(int PimProductTypeAssociationId);

        /// <summary>
        /// Update associated product
        /// </summary>
        /// <param name="productTypeAssociationViewModel">productTypeAssociationViewModel</param>
        /// <returns>Returns true if updated else false</returns>
        bool UpdateAssociatedProduct(ProductTypeAssociationViewModel productTypeAssociationViewModel);

        /// <summary>
        /// Creates addon group and product association.
        /// </summary>
        /// <param name="parentProductId">Parent product ID to which add-on group will be associated.</param>
        /// <param name="ids">Addon Group IDs to be associated.</param>
        /// <returns>True if addons are associated;False if addon fails to associate.</returns>
        bool AssociateAddonGroups(int parentProductId, string ids);

        /// <summary>
        /// Gets list of unassociated add-on groups.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="filters">Filters for list of unassociated add-on groups.</param>
        /// <param name="sorts">Sorts for list of unassociated add-on groups.</param>
        /// <param name="pageIndex">Start page index for list of unassociated add-on groups.</param>
        /// <param name="pageSize">Page size for list of unassociated add-on groups.</param>
        /// <returns>List of unassociated add-on groups.</returns>
        AddonGroupListViewModel GetUnassociatedAddonGroups(int parentProductId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets associated add-on groups
        /// </summary>
        /// <param name="parentProductId">Parent product ID for associated add-on groups</param>
        /// <param name="filters">Filters for associated add-on groups</param>
        /// <param name="sortCollection">Sorts for associated add-on groups</param>
        /// <param name="expands">Expands for associated add-on groups</param>
        /// <param name="pageIndex">Page index for associated add-on groups</param>
        /// <param name="pageSize">Page size for associated add-on groups</param>
        /// <returns>List of associated add-on groups.</returns>
        AddonGroupListViewModel GetAssociatedAddonGroup(int parentProductId, FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Deletes addon product. 
        /// </summary>
        /// <param name="addonProductId">Addon product Id.</param>
        /// <param name="parentProductId">Parent PimProductId</param>
        /// <returns>True/ false.</returns>
        bool DeleteAddonProduct(int addonProductId, int parentProductId = 0);

        /// <summary>
        /// Creates add-on product detail.
        /// </summary>
        /// <param name="addonProductId">Add-on product ID.</param>
        /// <param name="productIds">Product IDs to be associated.</param>
        /// <param name="displayOrder">Display Order.</param>
        /// <param name="pimProductId">Pim Product Id.</param>
        /// <returns>True/False.</returns>
        bool AssociateAddonProduct(int addonProductId, string productIds, int displayOrder,bool? isDefault, int pimProductId = 0);

        /// <summary>
        /// Deletes addon product detail entry.
        /// </summary>
        /// <param name="addonProductDetailId">Ids to be deleted.</param>
        /// <param name="PimProductId">Pim product id.</param>
        /// <returns>True/False</returns>
        bool DeleteAddonProductDetail(string addonProductDetailId, int PimProductId = 0);

        /// <summary>
        /// Update Product-addon association data.
        /// </summary>
        /// <param name="addonProductViewModel">AddonProduct view model.</param>
        /// <returns>Updated addon product view model.</returns>
        AddOnProductViewModel UpdateProductAddonAssociation(AddOnProductViewModel addonProductViewModel);

        /// <summary>
        /// Get Similar Combination for attribute in list
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        string GetSimilarCombination(int productId);

        /// <summary>
        /// Activate/Deactivate bulk product
        /// </summary>
        /// <param name="productIds">product ids seperated by commas</param>
        /// <param name="isActive">True/False</param>
        /// <returns></returns>
        bool ActivateDeactivateProducts(string productIds, bool isActive);

        #region Publish Product.
        /// <summary>
        /// Publish Product.
        /// </summary>
        /// <param name="productIds">Product Id to publish.</param>
        /// <param name="revisionType">Preview or publish.</param>
        /// <param name="errorMessage">error Message</param>
        /// <returns>Returns true if published successfully else return false.</returns>
        bool PublishProduct(string productIds, string revisionType, out string errorMessage);
        #endregion

        #region Product SKU list for Autocomplete feature
        /// <summary>
        /// Get product sku list by attribute code as SKU and attribute value. 
        /// </summary>
        /// <param name="attributeValue">Attribute value to get in list.</param>
        /// <returns>Returns product sku list.</returns>
        PIMProductAttributeValuesListViewModel GetSkuProductListBySKU(string attributeValue, FilterCollectionDataModel model);
        #endregion

        /// <summary>
        /// get product update file sample file content.
        /// </summary>
        /// <returns>return file content</returns>
        string getFileContent();

        /// <summary>
        /// This method process the product update data from the file for import.
        /// </summary>
        /// <param name="importViewModel"></param>
        /// <returns>bool</returns>
        bool ImportProductUpdateData(ImportViewModel importViewModel, out string statusMessage);


    }
}
