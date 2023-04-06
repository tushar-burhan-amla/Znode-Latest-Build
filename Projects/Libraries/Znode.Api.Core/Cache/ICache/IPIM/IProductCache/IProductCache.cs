using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    /// <summary>
    ///This is the root Interface ProductCache.
    /// </summary>
    public interface IProductCache
    {
        /// <summary>
        /// Get Product from Cache By Product Id.
        /// </summary>
        /// <param name="model">PIMGetProductModel</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns Product on the basis of Product Id.</returns>
        string GetProduct(PIMGetProductModel model, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Product list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of Product.</returns>
        string GetProducts(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Product list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of Product.</returns>
        string GetBrandProducts(string routeUri, string routeTemplate);

        /// <summary>
        /// Get associated or unassociated Products with Category.
        /// </summary>
        /// <param name="categoryId">category id</param>
        /// <param name="associatedProduts">Set true for associatedProduts and false for unAssociatedProduts</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProduts, string routeUri, string routeTemplate);

        /// <summary>
        /// Get products associated with parent productId.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="attributeId">Attribtue Id.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>List of ProductDetailsModel.</returns>
        string GetAssociatedProducts(int parentProductId, int attributeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get products those are not associated with parent productId.
        /// </summary>
        ///<param name="parentProductId">Parent product ID.</param>
        /// <param name="associatedProductIds">Already associated product Id.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>List of ProductDetailsModel.</returns>
        string GetUnassociatedProducts(int parentProductId, string associatedProductIds, string routeUri, string routeTemplate);

        /// <summary>
        /// Get products those are associated with productId.
        /// </summary>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="associatedProductIds">Product IDs to be associated.</param>
        /// <param name="associatedAttributeIds">associatedAttributeIds to be associated.</param>
        /// <param name="pimProductIdsIn">pim product id status</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>List of ProductDetailsModel.</returns>
        string GetAssociatedUnAssociatedConfigureProducts(int parentProductId, string associatedProductIds, string associatedAttributeIds, bool pimProductIdsIn, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of products which are associated as link products to the parent product.
        /// </summary>
        /// <param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link Attribtue Id.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>List of product models.</returns>
        string GetAssociatedLinkProducts(int parentProductId, int linkAttributeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of list of products which are not associated as link products to the parent product.
        /// </summary>
        ///<param name="parentProductId">Parent product ID.</param>
        /// <param name="linkAttributeId">Link attribute Id.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>List of product models.</returns>
        string GetUnassociatedLinkProducts(int parentProductId, int linkAttributeId, string routeUri, string routeTemplate);

        #region Custom Field
        /// <summary>
        /// Get Custom Field from Cache By custom field Id.
        /// </summary>
        /// <param name="customFieldId">customField id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns CustomField on the basis of CustomField Id.</returns>
        string GetCustomField(int customFieldId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Custom Field list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Custom Field.</returns>
        string GetCustomFields(string routeUri, string routeTemplate);
        #endregion

        /// <summary>
        /// Get assigned personalized attribute list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of assigned Personalized Attribute List.</returns>
        string GetAssignedPersonalizedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of unassigned personalized attributes.
        /// </summary>
        /// <param name="routeUri">String Route URI.</param>
        /// <param name="routeTemplate">String Route Template.</param>
        /// <returns>String data for unassigned personalized attributes.</returns>
        string GetUnassignedPersonalizedAttributes(string routeUri, string routeTemplate);

        /// <summary>
        ///  Get associated product By PimProductTypeAssociationId
        /// </summary>
        /// <param name="pimProductTypeAssociationId">PimProductTypeAssociationId to getCategory Product</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetAssociatedProduct(int pimProductTypeAssociationId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get associated add-on details
        /// </summary>
        /// <param name="productId">Parent product ID.</param>
        /// <param name="routeUri">String route URI.</param>
        /// <param name="routeTemplate">String route template.</param>
        /// <returns>String data.</returns>
        string GetAssociatedAddonDetails(int productId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets unassociated addon groups.
        /// </summary>
        /// <param name="productId">Parent Product ID.</param>
        /// <param name="routeUri">String route URI.</param>
        /// <param name="routeTemplate">String route template.</param>
        /// <returns>String data.</returns>
        string GetUnassociatedAddonGroups(int productId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets unassociated child product as addons.
        /// </summary>
        /// <param name="addonProductId">Addon Product UID.</param>
        /// <param name="routeUri">String route URI.</param>
        /// <param name="routeTemplate">String route template.</param>
        /// <returns>String data.</returns>
        string GetUnassociatedAddonProducts(int addonProductId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of product according to parameters
        /// </summary>
        /// <param name="parameter">Parameters containing product IDs</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String data.</returns>
        string GetProducts(ParameterModel parameter, string routeUri, string routeTemplate);

        #region Product SKU list for Autocomplete feature
        /// <summary>
        /// Get product sku list by attribute code as SKU and attribute value. 
        /// </summary>
        /// <param name="attributeValue">Attribute value to get in list.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String data.</returns>
        string GetProductSKUsByAttributeCode(string attributeValue, string routeUri, string routeTemplate);

        /// <summary>
        /// To add filter in AssignPersonalizedAttributes method
        /// </summary>
        /// <param name="model">PIM attribute list model</param>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>String data</returns>
        string AssignedPersonalizedAttribute(PIMAttributeValueListModel model, string routeUri, string routeTemplate);
        #endregion        

    }
}