using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IProductAgent
    {
        /// <summary>
        ///Create review for product.
        /// </summary>
        /// <param name="reviewModel">Model with customer review data.</param>
        /// <returns>View model with customer review data.</returns>
        ProductReviewViewModel CreateReview(ProductReviewViewModel reviewModel);

        /// <summary>
        /// Get Product details by product id.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <returns>Returns ProductViewModel.</returns>       
        ProductViewModel GetProduct(int productId);

        /// <summary>
        /// Get product Details associated with category
        /// </summary>
        /// <param name="productId"> Product Id</param>
        /// <param name="isCategoryAssociated">Add isCategoryAssociated to get additional filters</param>
        /// <returns>Returns ProductViewModel</returns>
        ProductViewModel GetProduct(int productId, bool isCategoryAssociated);

        /// <summary>
        /// This method only returns the brief details of a published product.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>Returns ProductViewModel.</returns>
        ProductViewModel GetProductBrief(int productID);

		/// <summary>
		/// This method only returns the extended details of a published product.
		/// </summary>
		/// <param name="productID"></param>
		/// <param name="expandKeys">Pass appropriate expand keys to get the corresponding details in response.</param>
		/// <returns>Returns ShortProductViewModel.</returns>
		ShortProductViewModel GetExtendedProductDetails(int productID, string[] expandKeys);

		/// <summary>
		/// Get product list by product sku.
		/// </summary>
		/// <param name="sku">Product SKU.</param>
		/// <returns>Returns ProductListViewModel.</returns>
		List<AutoComplete> GetProductList(string sku);

        /// <summary>
        /// get product price and inventory
        /// </summary>
        /// <param name="productSKU">product sku</param>
        /// <param name="quantity">quantity of product</param>
        /// <param name="message">inventory message.</param>
        /// <param name="parentProductSKU">parent product sku.</param>
        /// <returns>ProductViewModel</returns>
        ProductViewModel GetProductPriceAndInventory(string productSKU, string quantity, string addOnIds, string parentProductSKU = "", int parentProductId = 0);


        /// <summary>
        /// get product price or inventory
        /// </summary>
        /// <param name="productSKU">product sku</param>
        /// <param name="quantity">quantity of product</param>
        /// <param name="message">inventory message.</param>
        /// <param name="parentProductSKU">parent product sku.</param>
        /// <returns>List<ProductPriceViewModel></returns>
        List<ProductPriceViewModel> GetPriceWithInventory(List<ProductPriceViewModel> products);

        /// <summary>
        /// Get Product details by product id.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <returns>Returns ProductViewModel.</returns>
        ProductReviewViewModel GetProductForReview(int productId, string productName,decimal? rating);

        /// <summary>
        /// Get bundle product list
        /// </summary>
        /// <param name="productId">product id.</param>
        /// <returns>list of bundle product</returns>
        List<BundleProductViewModel> GetBundleProduct(int productId);


        /// <summary>
        /// Get product highlights list.
        /// </summary>
        /// <param name="productId">publish product id.</param>
        /// <param name="highLightsCodes">Highlights codes.</param>
        /// <returns>list of highlights.</returns>
        List<HighlightsViewModel> GetProductHighlights(int productId, string highLightsCodes);

        /// <summary>
        /// Get Configurable product.
        /// </summary>
        /// <param name="productId">Product id</param>
        /// <param name="sku">Product SKU.</param>
        /// <param name="codes">Attribute Codes</param>
        /// <param name="values">Attribute values</param>
        /// <param name="selectedCode">selected Codes</param>
        /// <param name="selectedValue">selected values</param>
        /// <returns>Return product view model.</returns>

        /// <summary>
        ///Get group product list.
        /// </summary>
        /// <param name="productId">product id.</param>
        /// <returns>list of associated products.</returns>
        List<GroupProductViewModel> GetGroupProductList(int productId, bool checkInventory = true);

        /// <summary>
        /// Check Group Product Inventory
        /// </summary>
        /// <param name="productSKU">Product skus.</param>
        /// <param name="quantity">quantity selected</param>
        /// <returns>product view model</returns>
        GroupProductViewModel CheckGroupProductInventory(int mainProductId, string productSKU, string quantity);

        /// <summary>
        /// Get list of product which are available for compare
        /// </summary>
        /// <returns>true/false</returns>
        bool GetCompareProducts();

        /// <summary>
        /// Add Product to comparison list.
        /// </summary>
        /// <param name="productId">product id.</param>
        /// <param name="categoryId">category id.</param>
        /// <param name="message">Message.</param>
        /// <param name="errorCode">Error code.</param>
        /// <returns>true ot false</returns>
        bool GlobalAddProductToCompareList(int productId, int categoryId, out string message, out int errorCode);

        /// <summary>
        /// Get list of comare product list.
        /// </summary>
        /// <returns>list of product</returns>
        List<ProductViewModel> GetCompareProductsDetails(bool isProductDetail = false);

        /// <summary>
        /// Remove all the product from comparison
        /// </summary>
        void DeleteComparableProducts();

        /// <summary>
        /// Remove single product
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>true or false</returns>
        bool RemoveProductFormSession(int productId);

       

        /// <summary>
        /// Send Compare Product Mail.
        /// </summary>
        /// <param name="productCompareModel">Model with product data.</param>
        /// <returns>True/False</returns>
        bool SendComparedProductMail(ProductCompareViewModel viewModel);

        /// <summary>
        /// Get product url by product sku.
        /// </summary>
        /// <param name="productSKU">SKUof product.</param>
        /// <param name="urlHelper">URLHelper.</param>
        /// <returns>Return product URL.</returns>
        string GetProductUrl(string productSKU, UrlHelper urlHelper);

        /// <summary>
        /// Check inventory of add on products associated to product.
        /// </summary>
        /// <param name="model">Product View Model.</param>
        /// <param name="addOnIds">add on skus.</param>
        /// <param name="quantity">selected quantity.</param>
        void CheckAddOnInvenTory(ProductViewModel model, string addOnIds, decimal quantity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="quantity"></param>
        void CheckInventory(ProductViewModel viewModel, decimal? quantity);

        /// <summary>
        /// Send A Mail to a Friend.
        /// </summary>
        /// <param name="viewModel">viewModel</param>
        /// <returns>bool</returns>
        bool SendMailToFriend(EmailAFriendViewModel viewModel);

        /// <summary>
        /// Get message for group product.
        /// </summary>
        /// <param name="associatedProductList">associated product list.</param>
        /// <returns>price message.</returns>
        string GetGroupProductMessage(List<GroupProductViewModel> associatedProductList);

        /// <summary>
        /// Gets an highlight according to highlightId.
        /// </summary>
        /// <param name="highLightId">ID of the highlight.</param>
        /// <param name="productId">product id.</param>
        /// <param name="sku">SKU of the product.</param>
        /// <returns>Highlight of the specified ID.</returns>
        HighlightsViewModel GetHighlightInfo(int highLightId, int productId, string sku);

        /// <summary>
        /// Gets an highlight according to highlightCode.
        /// </summary>
        /// <param name="highLightCode">Code of the highlight.</param>
        /// <param name="sku">SKU of the product.</param>
        /// <returns>Highlight of the specified Code.</returns>
        HighlightsViewModel GetHighlightInfoByCode(string highLightCode, string sku);

        /// <summary>
        /// Gets the breadcrumb of the product.
        /// </summary>
        /// <param name="categoryId">Category id of the last selected category ID.</param>
        /// <param name="checkFromSession">Bool value to check if categoryId is to be taken from session.</param>
        /// <returns>Breadcrumb string.</returns>
        string GetBreadCrumb(int categoryId, string[] productAssociatedCategoryIds, bool checkFromSession);

        /// <summary>
        /// Get string to generate see more url.
        /// </summary>
        /// <param name="breadCrumb">Breadcrum url through which see more url will be generated.</param>
        /// <returns>Returns see more string.</returns>
        string GetSeeMoreString(string breadCrumb);

        /// <summary>
        ///  Get price for products through ajax async call.
        /// </summary>
        /// <param name="products">parameters to get product price</param>
        /// <returns></returns>
        List<ProductPriceViewModel> GetProductPrice(List<ProductPriceViewModel> products);

        /// <summary>
        /// Get Configurable product.
        /// </summary>
        /// <returns>Return product view model.</returns>
        ProductViewModel GetConfigurableProduct(ParameterProductModel model);
        /// <summary>
        /// Get list of product reviews.
        /// </summary>
        /// <param name="id">Product id.</param>
        /// <returns>Returns list of reviews.</returns>
        ProductAllReviewListViewModel GetAllReviews(int id, string sortingChoice, int? pageSize, int pageNo);

        /// <summary>
        /// Gets property of auto complete item.
        /// </summary>
        /// <param name="productId">Seleted item product ID.</param>
        /// <returns>Auto complete Item.</returns>
        AutoComplete GetAutoCompleteProductProperties(int productId);
        /// <summary>
        /// Get Product SEO details
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        SEOViewModel GetSEODetails(int itemId,string seoCode);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        List<RecentViewModel> GetRecentProductList(int productId);        

        /// <summary>
        /// Get product search based on Search Term
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortValue"></param>
        /// <returns></returns>
        ProductListViewModel GetProductSearch(string searchTerm, bool enableSpecificSearch = false, int pageNum = 1, int pageSize = 1, int sortValue = 0);


        /// <summary>
        /// Get Product details by product id.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <returns>Returns Product inventory.</returns>
        ProductInventoryDetailViewModel GetProductInventory(int productID, bool isAllLocationsInventoryFlag);

        /// <summary>
        /// This method is Add valid SKUs to Quick Order Grid
        /// </summary>
        /// <param name="multipleItems">contain SKUS entered by users</param>
        /// <returns>returns count of valid/invalid SKU </returns>
        QuickOrderViewModel AddProductsToQuickOrder(string multipleItems);

        /// <summary>
        /// /This method to download quick order template
        /// </summary>
        /// <param name="response">contains response which help to download template</param>
        void DownloadQuickOrderTemplate(HttpResponseBase response);

        /// <summary>
        /// Map configurable product data.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="sku"></param>
        /// <param name="viewModel"></param>
        /// <param name="configurableData"></param>
        void MapConfigurableProductData(int productId, string sku, ProductViewModel viewModel, ConfigurableAttributeViewModel configurableData);

        /// <summary>
        /// Get associated configurable variants.
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Variants of configurable product</returns>
        List<ConfigurableProductViewModel> GetAssociatedConfigurableVariants(int productId);


        /// <summary>
        /// Get Configurable Product variant image.
        /// </summary>
        /// <param name="productDetailViewModel">ProductDetailViewModel</param>
        /// <returns>Images of the selected variant.</returns>
        ProductViewModel GetProductImage(ProductDetailViewModel productDetailViewModel);

        /// <summary>
        /// Submit stock request.
        /// </summary>
        /// <param name="StockNotificationViewModel">StockNotificationViewModel</param>
        /// <returns>Status</returns>
        bool SubmitStockRequest(StockNotificationViewModel StockNotificationViewModel);

        /// <summary>
        /// Check associated configurable product inventory.
        /// </summary>
        /// <param name="childSKUs">Selected child sku.</param>
        /// <param name="childQuantities">Selected child quantity.</param>
        /// <returns>product view model</returns>
        ConfigurableProductViewModel CheckConfigurableChildProductInventory(int parentProductId, string childSKUs, string childQuantities);
        QuickOrderViewModel AddProductsToQuickOrderUsingExcel(HttpPostedFileBase filename);

        /// <summary>
        /// Get Product Details by SKU.
        /// </summary>
        /// <param name="sku">Selected item sku.</param>
        /// <returns>Auto complete Item.</returns>
        AutoComplete GetProductDetailsBySKU(string sku);
    }
}
