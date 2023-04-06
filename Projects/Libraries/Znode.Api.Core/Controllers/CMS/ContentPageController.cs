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
    public class ContentPageController : BaseController
    {
        #region Private Variables
        private readonly IContentPageCache _cache;
        private readonly IContentPageService _service;
        #endregion

        #region Constructor
        public ContentPageController(IContentPageService service)
        {
            _service = service;
            _cache = new ContentPageCache(_service);
        }
        #endregion

        #region Public Methods
        #region Content Page
        /// <summary>
        /// Create Content Page.
        /// </summary>
        /// <param name="model">ContentPageModel.</param>
        /// <returns>Returns created Content page.</returns>
        [ResponseType(typeof(ContentPageResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateContentPage([FromBody] ContentPageModel model)
        {
            HttpResponseMessage response;
            try
            {
                ContentPageModel ContentPage = _service.CreateContentPage(model);

                if (!Equals(ContentPage, null))
                {
                    response = CreateCreatedResponse(new ContentPageResponse { ContentPage = ContentPage });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(ContentPage.CMSContentPagesId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Content Page List.
        /// </summary>
        /// <returns>Returns Content Page List.</returns>
        [ResponseType(typeof(ContentPageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetContentPageList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetContentPageList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentPageListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Content Page details by Content Page Id.
        /// </summary>       
        /// <returns>Returns Content Page model.</returns>
        [ResponseType(typeof(ContentPageResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetContentPage()
        {
            HttpResponseMessage response;

            try
            {
                //Get Content page by id.
                string data = _cache.GetContentPage(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentPageResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Content Page.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated ContentPage.</returns>
        [ResponseType(typeof(ContentPageResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateContentPage([FromBody] ContentPageModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Content Page.
                response = _service.UpdateContentPage(model) ? CreateCreatedResponse(new ContentPageResponse { ContentPage = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSContentPagesId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Content Page.
        /// </summary>
        /// <param name="ContentPageId">Content page Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteContentPage([FromBody] ParameterModel ContentPageId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete Content Page.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteContentPage(ContentPageId) });
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
        /// Publish the content pages
        /// </summary>
        /// <param name="parameterModel"></param>
        /// <returns>Returns the httpresponse model with result true if published successfully else return model with false and error message.</returns>
        [ResponseType(typeof(PublishedResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PublishContentPageWithPreview(ContentPageParameterModel parameterModel)
        {
            HttpResponseMessage response;
            try
            {
                int contentPageId = 0;
                int.TryParse(parameterModel.Ids, out contentPageId);

                PublishedModel publishedModel = _service.PublishContentPage(contentPageId, parameterModel.portalId, parameterModel.localeId, parameterModel.TargetPublishState, parameterModel.TakeFromDraftFirst);
                response = !Equals(publishedModel, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = publishedModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        #region Content Page Tree
        /// <summary>
        /// Gets the content page tree.
        /// </summary>
        /// <returns>Returns content page tree.</returns>
        [ResponseType(typeof(ContentPageResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTree()
        {
            HttpResponseMessage response;
            try
            {
                //Get Tree folder structure from database.
                string data = _cache.GetTree(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentPageResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// This method is used to add folder to database.
        /// </summary>
        /// <param name="model">model to add folder</param>
        /// <returns>Returns content page.</returns>
        [ResponseType(typeof(ContentPageResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AddFolder(ContentPageFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.AddFolder(model) ? CreateOKResponse(new ContentPageResponse { ContentPageFolder = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContentPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// This method is used to delete folder.
        /// </summary>
        /// <param name="model">cms content page id to delete.</param>
        /// <returns>Returns delete folder if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteFolder(ParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteFolder(model) });
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
        /// This method is used to rename folder.
        /// </summary>
        /// <param name="model">model to rename folder</param>
        /// <returns>Returns rename folder rename folder if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage RenameFolder(ContentPageFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.RenameFolder(model) });
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
        /// This method is used to move one folder to other folder.
        /// </summary>
        /// <param name="model">ContentPageFolderModel</param>
        /// <returns>Returns true/false on move folder.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage MoveContentPagesFolder(ContentPageFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.MoveContentPagesFolder(model);
                response = data ? CreateOKResponse(new TrueFalseResponse { IsSuccess = data }) : CreateInternalServerErrorResponse();
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
        /// Move content pages from one folder to another.
        /// </summary>
        /// <param name="model">AddPageToFolderModel model.</param>
        /// <returns>Returns true/false on move pages.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage MovePage(AddPageToFolderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.MovePageToFolder(model);
                response = data ? CreateOKResponse(new TrueFalseResponse { IsSuccess = data }) : CreateInternalServerErrorResponse();
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
        #endregion
    }
}