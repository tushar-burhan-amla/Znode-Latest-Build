using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class MenuController : BaseController
    {
        #region Private Variables
        private readonly IMenuService _service;
        private readonly IMenuCache _cache;
        #endregion

        #region Default Constructor
        public MenuController(IMenuService service)
        {
            _service = service;
            _cache = new MenuCache(_service);
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Gets list of menus.
        /// </summary>
        /// <returns>Returns menu list.</returns>
        [ResponseType(typeof(MenuListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get list of menus.
                string data = _cache.GetMenus(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MenuListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                MenuListResponse data = new MenuListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                MenuListResponse data = new MenuListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to create menus.
        /// </summary>
        /// <param name="model">MenuModel model.</param>
        /// <returns>Returns created menu.</returns>
        [ResponseType(typeof(MenuResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] MenuModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create menu.
                MenuModel menu = _service.CreateMenu(model);
                if (!Equals(menu, null))
                {
                    response = CreateCreatedResponse(new MenuResponse { Menu = menu });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(menu.MenuId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new MenuResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                MenuResponse data = new MenuResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of menus by parent menu id.
        /// </summary>
        /// <param name="model">ParameterModel model</param>
        /// <param name="preSelectedMenuIds">Pre Selected Menu Ids</param>
        /// <returns>Returns list of menus on the basis of ParentMenuId.</returns>
        [ResponseType(typeof(MenuListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetMenusByParentMenuId([FromBody] ParameterModel model, string preSelectedMenuIds)
        {
            HttpResponseMessage response;

            try
            {
                //Get Menus By Parent Menu Id 
                MenuListModel list = _service.GetMenusByParentMenuId(model.Ids, preSelectedMenuIds);
                response = CreateCreatedResponse(new MenuListResponse { Menus = list.Menus });
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(list.Menus)));
            }
            catch (Exception ex)
            {
                MenuListResponse data = new MenuListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Deletes an existing menu
        /// </summary>
        /// <param name="menuIds">Menu Id</param>
        /// <returns>Returns deleted menu.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel menuIds)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteMenu(menuIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a menu.
        /// </summary>
        /// <param name="menuId">The ID of the menu.</param>
        /// <returns>Returns menu on the basis of menu id.</returns>
        [ResponseType(typeof(MenuResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetMenu(int menuId)
        {
            HttpResponseMessage response;

            try
            {
                //Get menu by menu id.
                string data = _cache.GetMenu(menuId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MenuResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                MenuResponse data = new MenuResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates an existing menu.
        /// </summary>
        /// <param name="model">The model of the menu.</param>
        /// <returns>Returns updated menu.</returns>
        [ResponseType(typeof(MenuResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] MenuModel model)
        {
            HttpResponseMessage response;

            try
            {
                //Update menu.
                response = _service.UpdateMenu(model) ? CreateOKResponse(new MenuResponse { Menu = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ShippingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                MenuResponse data = new MenuResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get unselected menus by menu id.
        /// </summary>
        /// <param name="menuIds">Menu Ids.</param>
        /// <returns>Returns unselected menus by menu id.</returns>
        [ResponseType(typeof(MenuListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetUnSelectedMenus([FromBody] ParameterModel menuIds)
        {
            HttpResponseMessage response;

            try
            {
                //Get list of unselected menus.
                MenuListModel list = _service.GetUnSelectedMenus(menuIds);
                response = CreateCreatedResponse(new MenuListResponse { Menus = list.Menus, ParentMenus = list.ParentMenus });
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(list.Menus)));
            }
            catch (Exception ex)
            {
                MenuListResponse data = new MenuListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a menu.
        /// </summary>
        /// <param name="menuId">The ID of the menu.</param>
        /// <returns>Returns menu on the basis of menu id.</returns>
        [ResponseType(typeof(MenuResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetMenuActionsPermissionList(int menuId)
        {
            HttpResponseMessage response;
            try
            {
                //Get menu's action's permission by menu id.
                string data = _cache.GetMenuActionsPermissionList(menuId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<MenuResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                MenuResponse data = new MenuResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update the permission values against action.
        /// </summary>
        /// <param name="menuActionsPermissionModel">MenuActionsPermissionModel</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(MenuResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateActionPermissions([FromBody] MenuActionsPermissionModel menuActionsPermissionModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UpdateMenuActionPermissions(menuActionsPermissionModel);
                response = CreateCreatedResponse(new MenuResponse { MenuActionPermission = menuActionsPermissionModel });
                response.Headers.Add("Location", Convert.ToString(menuActionsPermissionModel.ActionId));
            }
            catch (Exception ex)
            {
                MenuResponse theme = new MenuResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(theme);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}