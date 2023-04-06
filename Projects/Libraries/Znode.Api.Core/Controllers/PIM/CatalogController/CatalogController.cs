using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class CatalogController : BaseController
    {
        #region Private Variables

        private readonly ICatalogService _service;
        private readonly ICatalogCache _cache;

        #endregion Private Variables

        #region Default Constructor

        public CatalogController(ICatalogService service)
        {
            _service = service;
            _cache = new CatalogCache(_service);
        }

        #endregion Default Constructor

        /// <summary>
        /// Gets list of catalogs.
        /// </summary>
        /// <returns>It will return list of catalogs.</returns>
        [ResponseType(typeof(CatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            try
            {
                Func<string> method = () => _cache.GetCatalogs(RouteUri, RouteTemplate);
                return CreateResponse<CatalogListResponse>(method, ZnodeLogging.Components.PIM.ToString());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return CreateInternalServerErrorResponse(new CatalogListResponse { HasError = true, ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Gets a catalog by catalogId.
        /// </summary>
        /// <param name="pimCatalogId">ID of catalog to be retrieved.</param>
        /// <returns>It will return specific catalog base on catalogId.</returns>
        [ResponseType(typeof(CatalogResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int pimCatalogId)
        {
            try
            {
                Func<string> method = () => _cache.GetCatalog(pimCatalogId, RouteUri, RouteTemplate);
                return CreateResponse<CatalogResponse>(method, ZnodeLogging.Components.PIM.ToString());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                return CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                return CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Get Catalog details by Catalog Code.
        /// </summary>
        /// <param name="catalogcode">catalogcode</param>
        /// <returns>Get catalog details.</returns>
        [ResponseType(typeof(CatalogResponse))]
        public HttpResponseMessage GetCatalogByCatalogCode(string catalogcode)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCatalogByCatalogCode(catalogcode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CatalogResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to create catalog.
        /// </summary>
        /// <param name="model">Catalog model.</param>
        /// <returns>Returns created catalog.</returns>
        [ResponseType(typeof(CatalogResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]CatalogModel model)
        {
            HttpResponseMessage response;
            try
            {
                CatalogModel catalog = _service.CreateCatalog(model);
                if (!Equals(catalog, null))
                {
                    response = CreateCreatedResponse(new CatalogResponse { Catalog = catalog });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(catalog.PimCatalogId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                CatalogResponse data = new CatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                CatalogResponse data = new CatalogResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to update catalog.
        /// </summary>
        /// <param name="model">Catalog model to be updated.</param>
        /// <returns>Returns updated catalog model.</returns>
        [ResponseType(typeof(CatalogResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]CatalogModel model)
        {
            HttpResponseMessage response;
            try
            {
                var catalog = _service.UpdateCatalog(model);
                response = catalog ? CreateCreatedResponse(new CatalogResponse { Catalog = model, ErrorCode = 0 }) :
                               CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                var data = new CatalogResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to create catalog.
        /// </summary>
        /// <param name="model">Catalog model.</param>
        /// <returns>Returns created catalog.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CopyCatalog([FromBody]CatalogModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.CopyCatalog(model) });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to delete catalog.
        /// </summary>
        /// <param name="model">Catalog Model with Catalog Ids and flag to delete Publish Catalog.</param>
        /// <returns>Returns status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(CatalogDeleteModel model)
        {
            HttpResponseMessage response;
            try
            {
                var deleted = _service.DeleteCatalog(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete catalog.
        /// </summary>
        /// <param name="catalogCodes">Add comma separated catalog Codes in the field of Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteCatalogByCode(CatalogDeleteModel catalogCodes)
        {
            HttpResponseMessage response;

            try
            {
                bool isDeleted = _service.DeleteCatalogByCode(catalogCodes);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted, booleanModel = new BooleanModel { IsSuccess = isDeleted, SuccessMessage = (isDeleted)? Api_Resources.DeleteCatalogSuccessMessage : "", ErrorMessage = (!isDeleted) ? Api_Resources.ErrorCatalogDelete: "" } });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// This method is used get category tree structure.
        /// </summary>
        /// <param name="catalogAssociationModel"> Catalog Association Model</param>
        /// <returns>Return category tree structure.</returns>
        [ResponseType(typeof(CategoryTreeResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetCategoryTree(CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                //Get Tree folder structure from database.
                ContentPageTreeModel categoryTree = _service.GetCatgoryTreeNode(catalogAssociationModel);
                response = IsNotNull(categoryTree) ? CreateOKResponse(new CategoryTreeResponse { Tree = categoryTree }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                CategoryTreeResponse data = new CategoryTreeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode};
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                CategoryTreeResponse data = new CategoryTreeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// This method is used get Associated Catalo tree structure.
        /// </summary>
        /// <param name="pimProductId">pim Product Id</param>
        /// <returns>Return Catalog tree structure.</returns>
        [ResponseType(typeof(CategoryTreeResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedCatalogTree(int pimProductId)
        {
            HttpResponseMessage response;
            try
            {
                //Get Tree folder structure from database.
                List<CatalogTreeModel> categoryTree = _service.GetAssociatedCatalogHierarchy(pimProductId);
                response = IsNotNull(categoryTree) ? CreateOKResponse(new CatalogTreeResponse { Tree = categoryTree }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                CategoryTreeResponse data = new CategoryTreeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                CategoryTreeResponse data = new CategoryTreeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Method to Get categories which are associated to catalog.
        /// </summary>
        /// <returns>Returns associated list of category.</returns>
        [ResponseType(typeof(CatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedCategoryList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedCategories(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CatalogListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                CatalogListResponse data = new CatalogListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to Un Associate categories from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogId.</param>
        /// <returns>Returns status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        [Obsolete("This action is not in use now, as new method has been introduced to unassociate category")]
        public virtual HttpResponseMessage UnAssociateCategory([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.UnAssociateCategoryFromCatalog(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Method to Un Associate categories from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel containing category Ids and CatalogId.</param>
        /// <returns>Returns status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        [Obsolete("This action is not in use now, As product association/unAssociation has been removed from catalog category")]
        public virtual HttpResponseMessage UnAssociateProduct([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.UnAssociateProductFromCatalog(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get list of products Associated to categories.
        /// </summary>
        /// <param name="catalogAssociationModel">catalog Association Model having values for CatalogId CategoryId and LocaleId.</param>
        /// <returns>It will return list of product Associated to categories.</returns>
        [ResponseType(typeof(ProductListResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage GetCategoryAssociatedProducts([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCategoryAssociatedProducts(catalogAssociationModel, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Method to publish catalog with associated product which has draft status.
        /// </summary>
        /// <param name="pimCatalogId">ID of catalog to be published.</param>
        /// <param name="revisionType"></param>
        /// <returns>Returns status as per published operation.</returns>
        [ResponseType(typeof(PublishedResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Publish(int pimCatalogId, string revisionType)
        {
            return Publish(pimCatalogId, revisionType,isDraftProductsOnly: true);
        }

        /// <summary>
        /// Method to publish catalog .
        /// </summary>
        /// <param name="pimCatalogId">ID of catalog to be published.</param>
        /// <param name="revisionType"></param>
        /// <param name="isDraftProductsOnly">Publish Draft Products Only.</param>
        /// <returns>Returns status as per published operation.</returns>
        [ResponseType(typeof(PublishedResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Publish(int pimCatalogId, string revisionType,bool isDraftProductsOnly)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.Publish(pimCatalogId, revisionType,isDraftProductsOnly);
                response = !Equals(published, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = published }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Method to publish catalog category products.
        /// </summary>
        /// <param name="pimCatalogId">ID of catalog to be published.</param>
        /// <param name="pimCategoryHierarchyId">ID of category to be published.</param>
        /// <param name="revisionType">For publish preview selection</param>
        /// <returns>Returns status as per published operation.</returns>
        [ResponseType(typeof(PublishedResponse))]
        [HttpGet]
        public virtual HttpResponseMessage PublishCatalogCategoryProducts(int pimCatalogId, int pimCategoryHierarchyId, string revisionType)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.PublishCategoryProducts(pimCatalogId, pimCategoryHierarchyId, revisionType);
                response = !Equals(published, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = published }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete publish Catalog along with associated category and products.
        /// </summary>
        /// <param name="publishCatalogId">ID of publish catalog to be deleted.</param>
        /// <returns>Returns status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage DeletePublishCatalog(int publishCatalogId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeletePublishCatalog(publishCatalogId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update details(Display order, active status, etc.)of category associated to catalog.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog associate Category model to be updated.</param>
        /// <returns>Returns updated catalog associate category model.</returns>
        [ResponseType(typeof(CatalogResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAssociateCategoryDetails([FromBody]CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateAssociateCategoryDetails(catalogAssociateCategoryModel);
                response = isUpdated ? CreateCreatedResponse(new CatalogResponse { AssociateCategory = catalogAssociateCategoryModel, ErrorCode = 0 }) :
                               CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(catalogAssociateCategoryModel.PimCategoryId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets a catalog by catalogId.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog Associate Category Model.</param>
        /// <returns>Returns catalog associate category model.</returns>
        [ResponseType(typeof(CatalogResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetAssociateCategoryDetails([FromBody] CatalogAssociateCategoryModel catalogAssociateCategoryModel)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociateCategoryDetails(catalogAssociateCategoryModel, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CatalogResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CatalogResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// This method is used to move category to other category.
        /// </summary>
        /// <param name="model">CatalogAssociateCategoryModel</param>
        /// <returns>Returns true/false on move folder.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage MoveCategory(CatalogAssociateCategoryModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.MoveCategory(model);
                response = data ? CreateOKResponse(new TrueFalseResponse { IsSuccess = data }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of catalogs.
        /// </summary>
        /// <returns>It will return status of publish catalog </returns>
        [ResponseType(typeof(PublishCatalogLogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCatalogPublishStatus()
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new PublishCatalogLogListResponse { PublishCatalogLogs = _cache.GetCatalogPublishStatus(RouteUri, RouteTemplate) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogLogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update display order with associated category and product.
        /// </summary>
        /// <param name="catalogAssociationModel">ID of publish catalog to be updated.</param>
        /// <returns>Returns status as per update operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCatalogCategoryProduct(CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool updated = _service.UpdateCatalogCategoryProduct(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = updated });
            }
            catch(ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Associate the product(s) to catalog categories
        /// </summary>
        /// <param name="catalogAssociationModel">Model will carry products id and category id</param>
        /// <returns>Returns true successful else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateProductsToCatalogCategory([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.AssociateProductsToCatalogCategory(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// UnAssociate the product(s) from catalog category
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationViewModel</param>
        /// <returns>Returns if products removed successfully true else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateProductsFromCatalogCategory([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUnAssociated = _service.UnAssociateProductsFromCatalogCategory(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isUnAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate the category(s) to catalog
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <returns>Return if category associate true else false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateCategoryToCatalog([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.AssociateCategoryToCatalog(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// UnAssociate the category(s) from catalog.
        /// </summary>
        /// <param name="catalogAssociationModel">CatalogAssociationModel</param>
        /// <returns>Returns if category unassociated successfully true else false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateCategoryFromCatalog([FromBody] CatalogAssociationModel catalogAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUnAssociated = _service.UnAssociateCategoryFromCatalogCategory(catalogAssociationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isUnAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
    }
}