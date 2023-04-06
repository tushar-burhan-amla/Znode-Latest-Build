using Newtonsoft.Json;
using System;
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

namespace Znode.Engine.Api.Client
{
    public class CatalogClient : BaseClient, ICatalogClient
    {
        #region Public Methods

        // Gets the list of Catalogs.
        public virtual CatalogListModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetCatalogList(expands, filters, sorts, null, null);

        // Gets the list of Catalogs.
        public virtual CatalogListModel GetCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.GetCatalogList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CatalogListResponse response = GetResourceFromEndpoint<CatalogListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CatalogListModel list = new CatalogListModel { Catalogs = response?.Catalogs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets a Catalog by catalog ID.
        public virtual CatalogModel GetCatalog(int pimCatalogId)
        {
            string endpoint = CatalogEndpoint.Get(pimCatalogId);

            ApiStatus status = new ApiStatus();
            CatalogResponse response = GetResourceFromEndpoint<CatalogResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Catalog;
        }

        // Creates a Catalog.
        public virtual CatalogModel CreateCatalog(CatalogModel model)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CatalogResponse response = PostResourceToEndpoint<CatalogResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Catalog;
        }

        // Updates a Catalog.
        public virtual CatalogModel UpdateCatalog(CatalogModel model)
        {
            //Get Endpoint
            string endpoint = CatalogEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CatalogResponse response = PutResourceToEndpoint<CatalogResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Catalog;
        }

        // Copy a Catalog.
        public virtual bool CopyCatalog(CatalogModel model)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.CopyCatalog();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        // Deletes a Catalog.
        public virtual bool DeleteCatalog(CatalogDeleteModel model)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get the category tree.
        public virtual ContentPageTreeModel GetCategoryTree(CatalogAssociationModel catalogAssociationModel)
        {
            string endpoint = CatalogEndpoint.GetCategoryTree();
            ApiStatus status = new ApiStatus();

            CategoryTreeResponse response = PostResourceToEndpoint<CategoryTreeResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Tree;
        }

        //Associate category to catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to associate category")]
        public virtual bool AssociateCategory(CatalogAssociationModel catalogAssociationModel)
        {
            //creating endpoint here.
            string endpoint = CatalogEndpoint.AssociateCategory();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };

            return response.IsSuccess;
        }

        //Get Associated categories to catalog.
        public virtual CatalogAssociateCategoryListModel GetAssociatedCategoryList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.GetAssociatedCategoryList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            CatalogListResponse response = GetResourceFromEndpoint<CatalogListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CatalogAssociateCategoryListModel catalogAssociateCategoryList = new CatalogAssociateCategoryListModel
            {
                catalogAssociatedCategoryList = response?.AssociateCategories,
                XmlDataList = response?.XmlDataList,
                AttributeColumnName = response?.AttributeColumnName
            };

            catalogAssociateCategoryList.MapPagingDataFromResponse(response);

            return catalogAssociateCategoryList;
        }

        //UnAssociate category from catalog.
        [Obsolete("This method is not in use now, as new method has been introduced to unassociate category")]
        public virtual bool UnAssociateCategory(CatalogAssociationModel catalogAssociationModel)
        {
            //creating endpoint here.
            string endpoint = CatalogEndpoint.UnAssociateCategory();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };

            return response.IsSuccess;
        }

        //UnAssociate product from catalog.
        [Obsolete("This method is not in use now, As product association/unassociation has been removed from catalog category")]
        public virtual bool UnAssociateProduct(CatalogAssociationModel catalogAssociationModel)
        {
            //creating endpoint here.
            string endpoint = CatalogEndpoint.UnAssociateProduct();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };

            return response.IsSuccess;
        }

        public virtual ProductDetailsListModel GetCategoryAssociatedProducts(CatalogAssociationModel catalogAssociationModel, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.GetCategoryAssociatedProducts();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ProductListResponse response = PutResourceToEndpoint<ProductListResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = new ProductDetailsListModel { ProductDetailList = response?.ProductDetails };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //publish catalog with associated product which has draft status.
        public virtual PublishedModel PublishCatalog(int pimCatalogId, string revisionType)
        {
            return PublishCatalog(pimCatalogId, revisionType,isDraftProductsOnly :true);
        }

        //Publish Catalog published with associated product as per isDraftProductsOnly flag passed 
        //isDraftProductsOnly:true=Publish only draft status product, 
        //isDraftProductsOnly:false:All products
        public virtual PublishedModel PublishCatalog(int pimCatalogId, string revisionType,bool isDraftProductsOnly)
        {
            string endpoint = CatalogEndpoint.Publish(pimCatalogId, revisionType, isDraftProductsOnly);

            ApiStatus status = new ApiStatus();
            PublishedResponse response = GetResourceFromEndpoint<PublishedResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }

        //Publish catalog category associated products.
        public virtual PublishedModel PublishCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType)
        {
            string endpoint = CatalogEndpoint.PublishCategoryProducts(pimCatalogId, pimCategoryHierarchyId, revisionType);

            ApiStatus status = new ApiStatus();
            PublishedResponse response = GetResourceFromEndpoint<PublishedResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }

        //Get details(Display order, active status, etc.)of category associated to catalog.
        public virtual CatalogAssociateCategoryModel GetAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            //Get Endpoint
            string endpoint = CatalogEndpoint.GetAssociateCategoryDetails();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CatalogResponse response = PutResourceToEndpoint<CatalogResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociateCategoryModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AssociateCategory;
        }

        //Update details(Display order, active status, etc.)of category associated to catalog.
        public virtual CatalogAssociateCategoryModel UpdateAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            //Get Endpoint
            string endpoint = CatalogEndpoint.UpdateAssociateCategoryDetails();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CatalogResponse response = PutResourceToEndpoint<CatalogResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociateCategoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AssociateCategory;
        }

        //Add folder to anther folder.
        public virtual bool MoveCategory(CatalogAssociateCategoryModel model)
        {
            //Get Endpoint
            string endpoint = CatalogEndpoint.MoveFolder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        // Get Catalog Publish Status.
        public virtual PublishCatalogLogListModel GetCatalogPublishStatus(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = CatalogEndpoint.GetCatalogPublishStatus();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishCatalogLogListResponse response = GetResourceFromEndpoint<PublishCatalogLogListResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishCatalogLogs;
        }

        //Update the product(s) from catalog category
        public virtual bool UpdateCatalogCategoryProduct(CatalogAssociationModel catalogAssociationModel)
        {
            string endpoint = CatalogEndpoint.UpdateCatalogCategoryProduct();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get AssociatedCatalogs and Categoires by productId
        public virtual List<CatalogTreeModel> GetCatalogCategoryHierarchy(int pimProductId)
        {

            string endpoint = CatalogEndpoint.GetCatalogCategoryTree(pimProductId);
            ApiStatus status = new ApiStatus();

            CatalogTreeResponse response = GetResourceFromEndpoint<CatalogTreeResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Tree;
        }


        #region Product(s) & Category(s) operation to catalog category  

        //Associate the product(s) to catalog categories
        public virtual bool AssociateProductsToCatalogCategory(CatalogAssociationModel catalogAssociationModel)
        {
            string endpoint = CatalogEndpoint.AssociateProductsToCatalogCategory();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //UnAssociate the product(s) from catalog category
        public virtual bool UnAssociateProductsFromCatalogCategory(CatalogAssociationModel catalogAssociationModel)
        {
            string endpoint = CatalogEndpoint.UnAssociateProductsFromCatalogCategory();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }


        //Associate category(s) to catalog tree
        public virtual bool AssociateCategoryToCatalog(CatalogAssociationModel catalogAssociationModel)
        {
            string endpoint = CatalogEndpoint.AssociateCategoryToCatalog();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }


        //UnAssociate category from catalog.
        public virtual bool UnAssociateCategoryFromCatalog(CatalogAssociationModel catalogAssociationModel)
        {
            //creating endpoint here.
            string endpoint = CatalogEndpoint.UnAssociateCategoryFromCatalog();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(catalogAssociationModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Product(s) & Category(s) operation to catalog category

        #endregion Public Methods
    }
}