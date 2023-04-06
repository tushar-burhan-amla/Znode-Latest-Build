using Newtonsoft.Json;
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
    public class CMSWidgetConfigurationClient : BaseClient, ICMSWidgetConfigurationClient
    {
        //Get the Widget Configuration List.
        public virtual CMSTextWidgetConfigurationListModel GetTextWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.TextWidgetConfigurationList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, null, null);

            ApiStatus status = new ApiStatus();
            CMSTextWidgetConfigurationListResponse response = GetResourceFromEndpoint<CMSTextWidgetConfigurationListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSTextWidgetConfigurationListModel list = new CMSTextWidgetConfigurationListModel { TextWidgetConfigurationList = response?.TextWidgetConfigurationList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get the Text Widget Configuration based on the Id.
        public virtual CMSTextWidgetConfigurationModel GetTextWidgetConfiguration(int textWidgetConfigurationId)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetTextWidgetConfiguration(textWidgetConfigurationId);

            ApiStatus status = new ApiStatus();
            CMSTextWidgetConfigurationResponse response = GetResourceFromEndpoint<CMSTextWidgetConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CMSTextWidgetConfiguration;
        }
        //Create New Text Widget Configuration
        public virtual CMSTextWidgetConfigurationModel CreateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.CreateTextWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSTextWidgetConfigurationResponse response = PostResourceToEndpoint<CMSTextWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CMSTextWidgetConfiguration;
        }

        //Update the Text Widget Configuration
        public virtual CMSTextWidgetConfigurationModel UpdateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateTextWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSTextWidgetConfigurationResponse response = PutResourceToEndpoint<CMSTextWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CMSTextWidgetConfiguration;
        }

        #region Media Widget Configration
        //Save and update Media Details
        public virtual CMSMediaWidgetConfigurationModel SaveAndUpdateMediaWidgetConfiguration(CMSMediaWidgetConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.SaveAndUpdateMediaWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSMediaWidgetConfigurationResponse response = PostResourceToEndpoint<CMSMediaWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CMSMediaWidgetConfigurationModel;

        }
        #endregion

        #region Remove Widget Configuration Data
        // Remove Widget Configuration Data
        public virtual bool RemoveWidgetDataFromContentPageConfiguration(CmsContainerWidgetConfigurationModel removeWidgetConfigurationModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.RemoveWidgetDataFromContentPageConfiguration();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(removeWidgetConfigurationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion 

        #region Form Widget Configration

        //Get the Form Widget Configuration List.
        public virtual CMSFormWidgetConfigurationListModel GetFormWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.FormWidgetConfigurationList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, null, null);

            ApiStatus status = new ApiStatus();
            CMSFormWidgetConfigurationListResponse response = GetResourceFromEndpoint<CMSFormWidgetConfigurationListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSFormWidgetConfigurationListModel list = new CMSFormWidgetConfigurationListModel { FormWidgetConfigurationList = response?.FormWidgetConfigurationList };
            list.MapPagingDataFromResponse(response);
            return list;
        }


        //Create New Form Widget Configuration
        public virtual CMSFormWidgetConfigrationModel CreateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.CreateFormWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSFormWidgetConfigurationResponse response = PostResourceToEndpoint<CMSFormWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CMSFormWidgetConfiguration;
        }

        //Update Form Widget Configuration
        public virtual CMSFormWidgetConfigrationModel UpdateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateFormWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSFormWidgetConfigurationResponse response = PutResourceToEndpoint<CMSFormWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CMSFormWidgetConfiguration;
        }

        #endregion

        #region  CMS Widget Slider Banner
        //Get the CMS Widget Slider Banner Details.
        public virtual CMSWidgetConfigurationModel GetCMSWidgetSliderBanner(FilterCollection filters)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetCMSWidgetSliderBanner();
            endpoint += BuildEndpointQueryString(null, filters, null, 0, 0);

            ApiStatus status = new ApiStatus();
            CMSWidgetConfigurationResponse response = GetResourceFromEndpoint<CMSWidgetConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.CMSWidgetConfiguration;
        }

        //Save New CMS Widget Slider Banner Details.
        public virtual CMSWidgetConfigurationModel SaveCMSWidgetSliderBanner(CMSWidgetConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.SaveCMSWidgetSliderBanner();

            ApiStatus status = new ApiStatus();
            CMSWidgetConfigurationResponse response = PutResourceToEndpoint<CMSWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CMSWidgetConfiguration;
        }
        #endregion

        #region Link Widget Configuration
        //Create Link Widget Configuration for selected portal in Website Configuration.
        public virtual LinkWidgetConfigurationModel CreateUpdateLinkWidgetConfiguration(LinkWidgetConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.CreateUpdateLinkWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSWidgetConfigurationResponse response = PostResourceToEndpoint<CMSWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.LinkWidgetConfiguration;
        }

        //Get Link Widget Configuration List.
        public virtual LinkWidgetConfigurationListModel LinkWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int pageIndex, int recordPerPage)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.LinkWidgetConfigurationList();
            endpoint += BuildEndpointQueryString(expands, filters, sortCollection, pageIndex, recordPerPage);

            ApiStatus status = new ApiStatus();

            LinkWidgetConfigurationListResponse response = GetResourceFromEndpoint<LinkWidgetConfigurationListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LinkWidgetConfigurationListModel list = new LinkWidgetConfigurationListModel { LinkWidgetConfigurationList = response?.LinkWidgetConfigurationList };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete Link Widget Configuration.
        public virtual bool DeleteLinkWidgetConfiguration(ParameterModel cmsWidgetTitleConfigurationIds, int localeId)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.DeleteLinkWidgetConfiguration(localeId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsWidgetTitleConfigurationIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion

        #region Category association.
        // Get unassociated category list.
        public virtual CategoryListModel GetUnAssociatedCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetUnassociatedCategoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CategoryListResponse response = GetResourceFromEndpoint<CategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryListModel list = new CategoryListModel { Categories = response?.Categories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get associated category list based on cms widgets.
        public virtual CategoryListModel GetAssociatedCategorylist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetAssociatedCategorylist();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CategoryListResponse response = GetResourceFromEndpoint<CategoryListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CategoryListModel list = new CategoryListModel { CMSWidgetProductCategories = response?.CMSWidgetProductCategories };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //UnAssociate associated categories by cmsWidgetCategoryId.
        public virtual bool DeleteCategories(ParameterModel cmsWidgetCategoryId)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.DeleteCategories();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsWidgetCategoryId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Associate unassociated categories.
        public virtual bool AssociateCategories(ParameterModelForWidgetCategory parameterModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.AssociateCategories();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update CMS Widget Category
        public virtual CategoryModel UpdateCMSWidgetCategory(CategoryModel categoryModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateCMSWidgetCategory();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            CategoryResponse response = PutResourceToEndpoint<CategoryResponse>(endpoint, JsonConvert.SerializeObject(categoryModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Category;
        }
        #endregion

        #region Brand association.
        // Get unassociated brand list.
        public virtual BrandListModel GetUnAssociatedBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetUnassociatedBrandList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            BrandListResponse response = GetResourceFromEndpoint<BrandListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BrandListModel list = new BrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get associated brand list based on cms widgets.
        public virtual BrandListModel GetAssociatedBrandlist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetAssociatedBrandList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            BrandListResponse response = GetResourceFromEndpoint<BrandListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BrandListModel list = new BrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Unassociate associated brands by cmsWidgetBrandId.
        public virtual bool DeleteBrands(ParameterModel cmsWidgetBrandId)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.DeleteBrands();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsWidgetBrandId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Associate brands.
        public virtual bool AssociateBrands(ParameterModelForWidgetBrand parameterModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.AssociateBrands();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update CMS Widget Brand
        public virtual BrandModel UpdateCMSWidgetBrand(BrandModel brandModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateCMSWidgetBrand();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            BrandResponse response = PutResourceToEndpoint<BrandResponse>(endpoint, JsonConvert.SerializeObject(brandModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Brand;
        }      
        #endregion

        #region CMSWidgetProduct
        //Get associated product list 
        public virtual CMSWidgetProductListModel GetAssociatedProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetCMSOfferPageProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            CMSWidgetProductListResponse response = GetResourceFromEndpoint<CMSWidgetProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSWidgetProductListModel listModel = new CMSWidgetProductListModel { CMSWidgetProductCategories = response?.CMSWidgetProductCategories };
            listModel.MapPagingDataFromResponse(response);

            return listModel;
        }

        //Get unassociated product list.
        public virtual ProductDetailsListModel GetUnAssociatedProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetUnAssociatedProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductDetailsListResponse response = GetResourceFromEndpoint<ProductDetailsListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = new ProductDetailsListModel { ProductDetailList = response?.ProductDetailList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate product.
        public virtual bool AssociateProduct(CMSWidgetProductListModel cmsWidgetProductListModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.AssociateProduct();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsWidgetProductListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Unassociate associated products .
        public virtual bool UnassociateProduct(ParameterModel cmsWidgetProductId)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UnAssociateProduct();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsWidgetProductId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update CMS Widget Product
        public virtual ProductDetailsModel UpdateCMSAssociateProduct(ProductDetailsModel productDetailsModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateCMSAssociateProduct();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ProductDetailsResponse response = PutResourceToEndpoint<ProductDetailsResponse>(endpoint, JsonConvert.SerializeObject(productDetailsModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ProductDetails;
        }
        #endregion

        #region Form Widget Email Configuration 

        //Get the Form Widget Email Configuration .
        public virtual FormWidgetEmailConfigurationModel GetFormWidgetEmailConfiguration(int cMSContentPagesId)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetFormWidgetEmailConfiguration(cMSContentPagesId);
            ApiStatus status = new ApiStatus();
            FormWidgetEmailConfigurationResponse response = GetResourceFromEndpoint<FormWidgetEmailConfigurationResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.FormWidgetEmailConfiguration;
        }


        //Create New Form Widget Email Configuration
        public virtual FormWidgetEmailConfigurationModel CreateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.CreateFormWidgetEmailConfiguration();
            ApiStatus status = new ApiStatus();
            FormWidgetEmailConfigurationResponse response = PostResourceToEndpoint<FormWidgetEmailConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.FormWidgetEmailConfiguration;
        }

        //Update Form Widget Email Configuration
        public virtual FormWidgetEmailConfigurationModel UpdateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateFormWidgetEmailConfiguration();
            ApiStatus status = new ApiStatus();
            FormWidgetEmailConfigurationResponse response = PutResourceToEndpoint<FormWidgetEmailConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.FormWidgetEmailConfiguration;
        }
        #endregion

        //Get the Search Widget Configuration.
        public virtual CMSSearchWidgetConfigurationModel GetSearchWidgetConfiguration(FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.GetSearchWidgetConfiguration();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            CMSSearchWidgetConfigurationResponse response = GetResourceFromEndpoint<CMSSearchWidgetConfigurationResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchWidgetConfiguration;
        }
      
        //Create New Search Widget Configuration
        public virtual CMSSearchWidgetConfigurationModel CreateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel cmsSearchWidgetConfigurationModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.CreateSearchWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSSearchWidgetConfigurationResponse response = PostResourceToEndpoint<CMSSearchWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(cmsSearchWidgetConfigurationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchWidgetConfiguration;
        }

        //Update the search widget configuration.
        public virtual CMSSearchWidgetConfigurationModel UpdateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel cmsSearchWidgetConfigurationModel)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.UpdateSearchWidgetConfiguration();

            ApiStatus status = new ApiStatus();
            CMSSearchWidgetConfigurationResponse response = PutResourceToEndpoint<CMSSearchWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(cmsSearchWidgetConfigurationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchWidgetConfiguration;
        }

        //Save the CMS Widget Content Container Details.
        public virtual CmsContainerWidgetConfigurationModel SaveCmsContainerDetails(CmsContainerWidgetConfigurationModel model)
        {
            string endpoint = CMSWidgetConfigurationEndpoint.SaveCmsContainerDetails();

            ApiStatus status = new ApiStatus();
            CMSContainerWidgetConfigurationResponse response = PutResourceToEndpoint<CMSContainerWidgetConfigurationResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContainerWidgetConfigurationResponse;
        }
    }
}
