using System;
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


namespace Znode.Engine.Api.Controllers
{
    public class CMSWidgetConfigurationController : BaseController
    {
        #region Private Variables
        private readonly ICMSWidgetConfigurationCache _cache;
        private readonly ICMSWidgetConfigurationService _service;
        #endregion

        #region Constructor
        public CMSWidgetConfigurationController(ICMSWidgetConfigurationService service)
        {
            _service = service;
            _cache = new CMSWidgetConfigurationCache(_service);
        }
        #endregion

        #region Public Methods

        #region CMSWidgetProduct 
        /// <summary>
        /// Get associated product list.
        /// </summary>
        /// <returns>Returns list of associated products .</returns>
        [ResponseType(typeof(CMSWidgetProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedProductList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSWidgetProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get un associated product list .
        /// </summary>
        /// <returns>Returns list of unassociated products.</returns>
        [ResponseType(typeof(ProductDetailsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssociatedProductList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssociatedProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductDetailsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductDetailsListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate product to widget.
        /// </summary>
        /// <param name="cmsWidgetProductListModel">CMSWidgetProductListModel.</param>
        /// <returns>Returns true if product associated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateProduct([FromBody] CMSWidgetProductListModel cmsWidgetProductListModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateProduct(cmsWidgetProductListModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        [ResponseType(typeof(ProductDetailsResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCMSAssociateProduct([FromBody] ProductDetailsModel productDetailsModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update Product.
                response = _service.UpdateCMSAssociateProduct(productDetailsModel) ? CreateOKResponse(new ProductDetailsResponse { ProductDetails = productDetailsModel, ErrorCode = 0, HasError = false }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(productDetailsModel.CMSWidgetProductId)));
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Unassociate products from widgets.
        /// </summary>
        /// <param name="cmsWidgetProductId">cmsWidgetProductId contains data to remove.</param>
        /// <returns>Returns true if portals unassociated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnassociateProduct([FromBody] ParameterModel cmsWidgetProductId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.UnassociateProduct(cmsWidgetProductId) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Form Widget Configration 

        /// <summary>
        /// Get Form widget configuration list.
        /// </summary>
        /// <returns>Returns list of form widget configuration.</returns>
        [ResponseType(typeof(CMSFormWidgetConfigurationListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage FormWidgetConfigurationList()
        {
            HttpResponseMessage response;
            try
            {
                //Get the CMS Text Widget Configuration List.
                string data = _cache.GetFormWidgetConfigurationList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSFormWidgetConfigurationListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSFormWidgetConfigurationListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create Form Widget Configuration.
        /// </summary>
        /// <param name="model">CMSFormWidgetConfigrationModel model.</param>
        /// <returns>Returns CMS from widget configuration.</returns>
        [ResponseType(typeof(CMSFormWidgetConfigrationModel))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateFormWidgetConfiguration([FromBody] CMSFormWidgetConfigrationModel model)
        {
            HttpResponseMessage response;

            try
            {
                CMSFormWidgetConfigrationModel widgetConfiguration = _service.CreateFormWidgetConfiguration(model);

                if (!Equals(widgetConfiguration, null))
                {
                    response = CreateCreatedResponse(new CMSFormWidgetConfigurationResponse { CMSFormWidgetConfiguration = widgetConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(widgetConfiguration.CMSFormWidgetConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSFormWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSFormWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Form Widget Configuration.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(CMSFormWidgetConfigurationResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateFormWidgetConfiguration([FromBody] CMSFormWidgetConfigrationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Widget Configuration.
                bool widgetConfiguration = _service.UpdateFormWidgetConfiguration(model);
                response = widgetConfiguration ? CreateCreatedResponse(new CMSFormWidgetConfigurationResponse { CMSFormWidgetConfiguration = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSFormWidgetConfigurationId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSFormWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSFormWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        #region Text Widget Configuration
        /// <summary>
        /// Get text widget configuration list.
        /// </summary>
        /// <returns>Returns list of text widget configuration.</returns>
        [ResponseType(typeof(CMSTextWidgetConfigurationListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage TextWidgetConfigurationList()
        {
            HttpResponseMessage response;
            try
            {
                //Get the CMS Text Widget Configuration List.
                string data = _cache.GetTextWidgetConfigurationList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSTextWidgetConfigurationListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSTextWidgetConfigurationListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Text Widget Configuration by widgetConfigurationId.
        /// </summary>
        /// <param name="textWidgetConfigurationId">Text Widget Configuration Id to get Text WidgetConfiguration details.</param>
        /// <returns>Returns TextWidgetConfigurationmodel.</returns>
        [ResponseType(typeof(CMSWidgetConfigurationResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTextWidgetConfiguration(int textWidgetConfigurationId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetTextWidgetConfiguration(textWidgetConfigurationId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSWidgetConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSTextWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create Text Widget Configuration.
        /// </summary>
        /// <param name="model">CMSTextWidgetConfigurationModel model.</param>
        /// <returns>Returns CMS text widget configuration.</returns>
        [ResponseType(typeof(CMSTextWidgetConfigurationResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateTextWidgetConfiguration([FromBody] CMSTextWidgetConfigurationModel model)
        {
            HttpResponseMessage response;

            try
            {
                CMSTextWidgetConfigurationModel widgetConfiguration = _service.CreateTextWidgetConfiguration(model);

                if (!Equals(widgetConfiguration, null))
                {
                    response = CreateCreatedResponse(new CMSTextWidgetConfigurationResponse { CMSTextWidgetConfiguration = widgetConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(widgetConfiguration.CMSTextWidgetConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSTextWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSTextWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Text Widget Configuration.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(CMSTextWidgetConfigurationResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateTextWidgetConfiguration([FromBody] CMSTextWidgetConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Widget Configuration.
                bool widgetConfiguration = _service.UpdateTextWidgetConfiguration(model);
                response = widgetConfiguration ? CreateCreatedResponse(new CMSTextWidgetConfigurationResponse { CMSTextWidgetConfiguration = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSTextWidgetConfigurationId)));
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSTextWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Media Widget
        /// <summary>
        /// Save and Update Media Widget Configuration.
        /// </summary>
        /// <param name="model">CMSMediaWidgetConfigurationModel model.</param>
        /// <returns>Returns CMSMediaWidgetConfigurationModel.</returns>
        [ResponseType(typeof(CMSMediaWidgetConfigurationResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveAndUpdateMediawidgetConfiguration([FromBody] CMSMediaWidgetConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                CMSMediaWidgetConfigurationModel widgetConfiguration = _service.SaveAndUpdateMediawidgetConfiguration(model);

                if (!Equals(widgetConfiguration, null))
                {
                    response = CreateCreatedResponse(new CMSMediaWidgetConfigurationResponse { CMSMediaWidgetConfigurationModel = widgetConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(widgetConfiguration.CMSMediaConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSMediaWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSMediaWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;

        }

        #endregion
        #region Remove Media Files Image or Video

        /// <summary>
        /// Remove Media Widget Configuration.
        /// </summary>
        /// <param name="model">CMSMediaWidgetConfigurationModel model.</param>
        /// <returns>true/false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage RemoveWidgetDataFromContentPageConfiguration(CmsContainerWidgetConfigurationModel removeWidgetConfigurationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.RemoveWidgetDataFromContentPageConfiguration(removeWidgetConfigurationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion
        #region Link Widgets Configuration
        /// <summary>
        /// Get link widgets configuration.
        /// </summary>
        /// <returns>Return link widget configuration list.</returns>
        [ResponseType(typeof(LinkWidgetConfigurationListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage LinkWidgetConfigurationList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetLinkWidgetConfigurationList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LinkWidgetConfigurationListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LinkWidgetConfigurationListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Create link widgets configuration.
        /// </summary>
        /// <param name="model">model with link widgets configuration data</param>
        /// <returns>Return CMS widget configuration.</returns>
        [ResponseType(typeof(CMSWidgetConfigurationResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateUpdateLinkWidgetConfiguration([FromBody] LinkWidgetConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                LinkWidgetConfigurationModel widgetConfiguration = _service.CreateUpdateLinkWidgetConfiguration(model);

                if (!Equals(widgetConfiguration, null))
                {
                    response = CreateCreatedResponse(new CMSWidgetConfigurationResponse { LinkWidgetConfiguration = widgetConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(widgetConfiguration.CMSWidgetTitleConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }


            return response;
        }

        /// <summary>
        /// Delete link widgets configuration by id.
        /// </summary>
        /// <param name="cmsWidgetTitleConfigurationId">id to delete link widgets configuration.</param>
        /// <param name="localeId">localeId</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteLinkWidgetConfiguration(ParameterModel cmsWidgetTitleConfigurationId, int localeId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteLinkWidgetConfiguration(cmsWidgetTitleConfigurationId, localeId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region CMS Widget Slider Banner
        /// <summary>
        /// Get the CMS Widget Slider Banner Details by cmsMappingId, cmsWidgetsId and widgetsKey.
        /// </summary>
        /// <returns>Returns WidgetConfigurationmodel.</returns>
        [ResponseType(typeof(CMSWidgetConfigurationResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCMSWidgetSliderBanner()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCMSWidgetSliderBanner(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSWidgetConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Save New CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="model">model to save CMS Widget Slider Banner details.</param>
        /// <returns>Returns CMS widget configuration.</returns>
        [ResponseType(typeof(CMSWidgetConfigurationResponse))]
        [HttpPut]
        public virtual HttpResponseMessage SaveCMSWidgetSliderBanner([FromBody] CMSWidgetConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Save New CMS Widget Slider Banner Details.
                bool widgetConfiguration = _service.SaveCMSWidgetSliderBanner(model);
                response = widgetConfiguration ? CreateCreatedResponse(new CMSWidgetConfigurationResponse { CMSWidgetConfiguration = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSWidgetSliderBannerId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Category Association
        /// <summary>
        /// Get list of unassociate categories.
        /// </summary>
        /// <returns>Return unassociate categories list.</returns>
        [ResponseType(typeof(CategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssociatedCategory()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssociatedCategories(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of associate categories based on cms widgets.
        /// </summary>
        /// <returns>Return list of associate categories.</returns>
        [ResponseType(typeof(CategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedCategory()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedCategories(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Remove association of categories and widget.
        /// </summary>
        /// <param name="cmsWidgetCategoryId">cmsWidgetCategoryId to unassociate categories.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteCategories([FromBody] ParameterModel cmsWidgetCategoryId)
        {
            HttpResponseMessage response;

            try
            {
                //Remove associated categories.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteCategories(cmsWidgetCategoryId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate categories to widget.
        /// </summary>
        /// <param name="model">model with cms widget id and multiple categories id.</param>
        /// <returns>Returns associate categories to widget if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateCategories([FromBody] ParameterModelForWidgetCategory model)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.AssociateCategories(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(CategoryResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCMSWidgetCategory([FromBody] CategoryModel categoryModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update CAtegory.
                response = _service.UpdateCMSWidgetCategory(categoryModel) ? CreateOKResponse(new CategoryResponse { Category = categoryModel, ErrorCode = 0, HasError = false }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(categoryModel.CMSWidgetCategoryId)));
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Brand Association
        /// <summary>
        /// Get list of unassociate brands.
        /// </summary>
        /// <returns> Returns unassociate brands list.</returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssociatedBrand()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssociatedBrands(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BrandListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BrandListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of associate brands on cms widgets.
        /// </summary>
        /// <returns>Returns list of associate brands.</returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedBrand()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedBrands(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BrandListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BrandListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Remove association of brands and widget.
        /// </summary>
        /// <param name="cmsWidgetBrandId">Brand Id to unassociate brands.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteBrands([FromBody] ParameterModel cmsWidgetBrandId)
        {
            HttpResponseMessage response;

            try
            {
                //Remove associated categories.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteBrands(cmsWidgetBrandId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate brands to widget.
        /// </summary>
        /// <param name="model">Model with cms widget id and multiple brand ids.</param>
        /// <returns>Returns associated brands.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateBrands([FromBody] ParameterModelForWidgetBrand model)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.AssociateBrands(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(BrandResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCMSWidgetBrand([FromBody] BrandModel brandModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update Brand.
                response = _service.UpdateCMSWidgetBrand(brandModel) ? CreateOKResponse(new BrandResponse { Brand = brandModel, ErrorCode = 0, HasError = false }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(brandModel.CMSWidgetBrandId)));
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        #endregion

        #region Search Widget Configuration
        /// <summary>
        /// Get Text Widget Configuration by widgetConfigurationId.
        /// </summary>
        /// <param name="searchWidgetConfigurationId">Text Widget Configuration Id to get Text WidgetConfiguration details.</param>
        /// <returns>Returns TextWidgetConfigurationmodel.</returns>
        [ResponseType(typeof(CMSSearchWidgetConfigurationResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSearchWidgetConfiguration()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSearchWidgetConfiguration(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CMSSearchWidgetConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSSearchWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSSearchWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create Text Widget Configuration.
        /// </summary>
        /// <param name="model">CMSTextWidgetConfigurationModel model.</param>
        /// <returns>Returns CMS text widget configuration.</returns>
        [ResponseType(typeof(CMSTextWidgetConfigurationResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateSearchWidgetConfiguration([FromBody] CMSSearchWidgetConfigurationModel model)
        {
            HttpResponseMessage response;

            try
            {
                CMSSearchWidgetConfigurationModel widgetConfiguration = _service.CreateSearchWidgetConfiguration(model);

                if (!Equals(widgetConfiguration, null))
                {
                    response = CreateCreatedResponse(new CMSSearchWidgetConfigurationResponse { SearchWidgetConfiguration = widgetConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(widgetConfiguration.CMSSearchWidgetId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSSearchWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSSearchWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Text Widget Configuration.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(CMSTextWidgetConfigurationResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateSearchWidgetConfiguration([FromBody] CMSSearchWidgetConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Widget Configuration.
                bool widgetConfiguration = _service.UpdateSearchWidgetConfiguration(model);
                response = widgetConfiguration ? CreateCreatedResponse(new CMSSearchWidgetConfigurationResponse { SearchWidgetConfiguration = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSSearchWidgetId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSSearchWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSSearchWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
        #endregion

        #region form widget email configuration

        /// <summary>
        /// Get the Form Widget Email Configuration details by cMSContentPagesId .
        /// </summary>
        /// <param name="portalId">Id of portal to get portal details.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(FormWidgetEmailConfigurationResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFormWidgetEmailConfiguration(int cMSContentPagesId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetFormWidgetEmailConfiguration(cMSContentPagesId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FormWidgetEmailConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Create Form Widget Email Configuration.
        /// </summary>
        /// <param name="model">CMSFormWidgetemailConfigrationModel model.</param>
        /// <returns>Returns CMS from widget email configuration.</returns>FormWidgetEmailConfigurationModel
        [ResponseType(typeof(FormWidgetEmailConfigurationModel))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateFormWidgetEmailConfiguration([FromBody] FormWidgetEmailConfigurationModel model)
        {
            HttpResponseMessage response;

            try
            {
                FormWidgetEmailConfigurationModel widgetEmailConfiguration = _service.CreateFormWidgetEmailConfiguration(model);

                if (!Equals(widgetEmailConfiguration, null))
                {
                    response = CreateCreatedResponse(new FormWidgetEmailConfigurationResponse { FormWidgetEmailConfiguration = widgetEmailConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(widgetEmailConfiguration.FormWidgetEmailConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new FormWidgetEmailConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new FormWidgetEmailConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Form Widget Email Configuration.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        /// 
        [ResponseType(typeof(FormWidgetEmailConfigurationResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateFormWidgetEmailConfiguration([FromBody] FormWidgetEmailConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Widget email Configuration.
                bool widgetEmailConfiguration = _service.UpdateFormWidgetEmailConfiguration(model);
                response = widgetEmailConfiguration ? CreateCreatedResponse(new FormWidgetEmailConfigurationResponse { FormWidgetEmailConfiguration = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.FormWidgetEmailConfigurationId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new FormWidgetEmailConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new FormWidgetEmailConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        #region CMS Widget Content Container
        /// <summary>
        /// Save CMS Widget Content Container Details.
        /// </summary>
        /// <param name="model">model to save CMS Widget Content Container details.</param>
        /// <returns>Returns CMS widget configuration.</returns>
        [ResponseType(typeof(CMSContainerWidgetConfigurationResponse))]
        [HttpPut]
        public virtual HttpResponseMessage SaveCmsContainerDetails([FromBody] CmsContainerWidgetConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Save New CMS Widget Container Details.
                bool widgetConfiguration = _service.SaveCmsContainerDetails(model);
                response = widgetConfiguration ? CreateCreatedResponse(new CMSContainerWidgetConfigurationResponse { ContainerWidgetConfigurationResponse = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSContainerConfigurationId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new CMSContainerWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CMSContainerWidgetConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}