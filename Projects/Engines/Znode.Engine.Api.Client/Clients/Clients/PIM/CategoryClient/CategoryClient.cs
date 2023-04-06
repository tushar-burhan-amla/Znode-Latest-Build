using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using System;

namespace Znode.Engine.Api.Client
{
    public class CategoryClient : BaseClient, ICategoryClient
    {
        #region Public Methods

        // Gets the list of Categories.
        public virtual CategoryListModel GetCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCategoryList(expands, filters, sorts, null, null);

        // Gets the list of Categories.
        public virtual CategoryListModel GetCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CategoryEndpoint.GetCategoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CategoryListResponse response = GetResourceFromEndpoint<CategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryListModel list = new CategoryListModel { XmlDataList = response?.CategoriesList?.XmlDataList, Locale = response?.Locale, AttrubuteColumnName = response?.CategoriesList.AttrubuteColumnName };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets a Category by category ID.
        public virtual PIMFamilyDetailsModel GetCategory(int categoryId, int familyId, int localeId)
        {
            // Get Endpoint.
            string endpoint = CategoryEndpoint.Get(categoryId, familyId, localeId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = GetResourceFromEndpoint<PIMAttributeFamilyResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMFamilyDetails;
        }

        // Creates a Category.
        public virtual CategoryValuesListModel CreateCategory(CategoryValuesListModel model)
        {
            //Get Endpoint.
            string endpoint = CategoryEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CategoryResponse response = PostResourceToEndpoint<CategoryResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CategoryValues;
        }

        // Updates a Category.
        public virtual CategoryValuesListModel UpdateCategory(CategoryValuesListModel model)
        {
            //Get Endpoint
            string endpoint = CategoryEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CategoryResponse response = PutResourceToEndpoint<CategoryResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CategoryValues;
        }

        // Deletes a Category.
        public virtual bool DeleteCategory(string categoryIds)
        {
            //Get Endpoint.
            string endpoint = CategoryEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = categoryIds }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }



        // Delete Category Product
        public virtual bool DeleteCategoryProduct(string pimCategoryProductId)
        {
            string endpoint = CategoryEndpoint.DeleteCategoryProduct();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = pimCategoryProductId }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        // Delete categories associated to Product.
        public virtual bool DeleteAssociatedCategoriesToProduct(string pimCategoryProductId)
        {
            string endpoint = CategoryEndpoint.DeleteAssociatedCategoriesToProduct();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = pimCategoryProductId }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate Products to Category 
        public virtual bool AssociateCategoryProduct(List<CategoryProductModel> categoryProductsList)
        {
            string endpoint = CategoryEndpoint.AssociateCategoryProduct();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(categoryProductsList), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }



        //Get details(Display order, active status, etc.)of category associated to category.
        public virtual CategoryProductListModel GetAssociatedUnAssociatedCategoryProducts(int categoryId, bool associatedProducts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CategoryEndpoint.GetAssociatedUnAssociatedCategoryProducts(categoryId, associatedProducts);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CategoryProductListResponse response = GetResourceFromEndpoint<CategoryProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryProductListModel list = new CategoryProductListModel { CategoryProducts = (Equals(response, null)) ? null : response.CategoryProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get associated category products.
        public virtual CategoryProductListModel GetAssociatedCategoryProducts(int categoryId, bool associatedProducts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CategoryEndpoint.GetAssociatedCategoryProducts(categoryId, associatedProducts);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CategoryProductListResponse response = GetResourceFromEndpoint<CategoryProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryProductListModel list = new CategoryProductListModel { CategoryProducts = (Equals(response, null)) ? null : response.CategoryProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion

        #region Publish Category

        //Publish Category
        public virtual PublishedModel PublishCategory(ParameterModel parameterModel)
        {
            string endpoint = CategoryEndpoint.Publish();

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }

        #endregion Publish Category

        // Associate categories to product.
        public bool AssociateCategoriesToProduct(List<CategoryProductModel> categoryProductsList)
        {
            string endpoint = CategoryEndpoint.AssociateCategoriesToProduct();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(categoryProductsList), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }


        // Get associated categories to products.
        public CategoryProductListModel GetAssociatedCategoriesToProduct(int productId, bool associatedProducts, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CategoryEndpoint.GetAssociatedCategoriesToProduct(productId, associatedProducts);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CategoryProductListResponse response = GetResourceFromEndpoint<CategoryProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryProductListModel list = new CategoryProductListModel { CategoryProducts = (Equals(response, null)) ? null : response.CategoryProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update product details associated to category.
        public virtual bool UpdateCategoryProductDetail(CategoryProductModel categoryProductModel)
        {
            string endpoint = CategoryEndpoint.UpdateCategoryProductDetail();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(categoryProductModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
