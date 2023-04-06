using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class CategoryCache : BaseCache, ICategoryCache
    {
        #region Private Variables
        private readonly ICategoryService _service;
        #endregion

        #region Constructor
        public CategoryCache(ICategoryService categoryService)
        {
            _service = categoryService;
        }
        #endregion

        #region Public Methods

        //Get a list of categories.
        public virtual string GetCategories(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                //Get Menu list
                CategoryListModel list = _service.GetCategories(Expands, Filters, Sorts, Page);
                //Create response.
                CategoryListResponse response = new CategoryListResponse { CategoriesList = list, Locale = list.Locale };

                //apply pagination parameters.
                response.MapPagingDataFromModel(list);
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get category using categoryId.
        public virtual string GetCategory(int categoryId, int familyId, int localeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                PIMFamilyDetailsModel category = _service.GetCategory(categoryId, familyId, localeId);
                if (!Equals(category, null))
                {
                    PIMAttributeFamilyResponse response = new PIMAttributeFamilyResponse { PIMFamilyDetails = category };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        //Get associated or unassociated Products with Category.
        public virtual string GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CategoryProductListModel list = _service.GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProducts, Expands, Filters, Sorts, Page);
                if (list?.CategoryProducts?.Count > 0)
                {
                    CategoryProductListResponse response = new CategoryProductListResponse { CategoryProducts = list?.CategoryProducts };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get associated Products with Category.
        public virtual string GetAssociatedCategoryProducts(int categoryId, bool associatedProducts, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CategoryProductListModel list = _service.GetAssociatedCategoryProducts(categoryId, associatedProducts, Expands, Filters, Sorts, Page);
                if (list?.CategoryProducts?.Count > 0)
                {
                    CategoryProductListResponse response = new CategoryProductListResponse { CategoryProducts = list?.CategoryProducts };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        /// <summary>
        /// Get the list of associated categories to Product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="associatedProducts"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        public virtual string GetAssociatedCategoriesToProduct(int productId, bool associatedProducts, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CategoryProductListModel list = _service.GetAssociatedCategoriesToProducts(productId, associatedProducts, Expands, Filters, Sorts, Page);
                if (list?.CategoryProducts?.Count > 0)
                {
                    CategoryProductListResponse response = new CategoryProductListResponse { CategoryProducts = list?.CategoryProducts };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}