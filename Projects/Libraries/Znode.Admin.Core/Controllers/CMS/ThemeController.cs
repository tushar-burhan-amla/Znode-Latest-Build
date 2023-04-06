using MvcSiteMapProvider;
using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ThemeController : BaseController
    {
        #region Private Variables
        private IThemeAgent _cmsThemeAgent;
        #endregion

        #region Public Constructor
        public ThemeController(IThemeAgent themeAgent)
        {
            _cmsThemeAgent = themeAgent;
        }
        #endregion

        //Get the index view for CMS
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleCMS", Key = "CMS", Area = "", ParentKey = "Home")]
        public virtual ActionResult Index() => View();

        #region Theme Configuration
        //Method return a list view to display list of themes.
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleCMS", Key = "Theme", Area = "", ParentKey = "CMS")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSTheme.ToString(), model);
            ThemeListViewModel themeList = _cmsThemeAgent.GetThemeList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            themeList.GridModel = FilterHelpers.GetDynamicGridModel(model, themeList.ThemeList, GridListType.ZnodeCMSTheme.ToString(), string.Empty, null, true, true, themeList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            themeList.GridModel.TotalRecordCount = themeList.TotalResults;
            return ActionView(themeList);
        }

        //Method return a View to add new theme.
        [HttpGet]
        public virtual ActionResult Create()
        {
            ThemeViewModel model = new ThemeViewModel();

            model.ThemeList = _cmsThemeAgent.GetParentThemeList();            

            model.IsParentTheme = true;

            return ActionView(model);
        }

        //Method to add a new theme, having parameter ThemeViewModel contains theme name.
        [HttpPost]
        public virtual ActionResult Create(ThemeViewModel themeViewModel)
        {
            //Get details of newly created theme.
            themeViewModel = _cmsThemeAgent.CreateTheme(themeViewModel);
            if (themeViewModel?.CMSThemeId > 0)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<ThemeController>(x => x.EditTheme(Convert.ToInt32(themeViewModel.CMSThemeId)));
            }

            themeViewModel.ThemeList = _cmsThemeAgent.GetParentThemeList();
            //Set error message for already exists theme or failed to create theme.            
            SetNotificationMessage(GetErrorNotificationMessage(themeViewModel.ErrorMessage));
            return ActionView("Create", themeViewModel);
        }

        //Get:Edit theme.
        [HttpGet]
        public virtual ActionResult EditTheme(int cmsThemeId)
        {
            ActionResult action = GotoBackURL();
            if (HelperUtility.IsNotNull(action))
                return action;
            return ActionView("Create", _cmsThemeAgent.GetTheme(cmsThemeId));
        }
        //Post:Edit theme.
        [HttpPost]
        public virtual ActionResult EditTheme(ThemeViewModel themeViewModel)
        {
            SetNotificationMessage(_cmsThemeAgent.UpdateTheme(themeViewModel).HasError
            ? GetErrorNotificationMessage(themeViewModel.ErrorMessage)
            : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
            return RedirectToAction<ThemeController>(x => x.EditTheme(Convert.ToInt32(themeViewModel.CMSThemeId)));
        }

        //Check theme name already exists or not.
        public virtual JsonResult IsThemeNameExist(string Name, int CMSThemeId = 0)
          => Json(!_cmsThemeAgent.CheckThemeNameExist(Name, CMSThemeId), JsonRequestBehavior.AllowGet);

        //Get theme revision list.
        public virtual ActionResult GetThemeRevisionList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int CMSThemeId, string Name)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeThemeRevision.ToString(), model);
            ThemeListViewModel themeList = _cmsThemeAgent.GetThemeRevisionList(CMSThemeId, Name, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            //Get the grid model.
            themeList.GridModel = FilterHelpers.GetDynamicGridModel(model, themeList.ThemeList, GridListType.ZnodeThemeRevision.ToString(), string.Empty, null, true, true, themeList?.GridModel?.FilterColumn?.ToolMenuList);
            themeList.CMSThemeId = CMSThemeId;
            //Set the total record count
            themeList.GridModel.TotalRecordCount = themeList.TotalResults;
            return ActionView("ThemeRevisionList", themeList);
        }

        //Get the details of theme by theme Id.
        public virtual ActionResult Manage(int cmsThemeId, [ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSThemeCSS.ToString(), model);
            CSSListViewModel cssList = _cmsThemeAgent.GetCssList(cmsThemeId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            cssList.GridModel = FilterHelpers.GetDynamicGridModel(model, cssList.CssList, GridListType.ZnodeCMSThemeCSS.ToString(), string.Empty, null, true, true, cssList?.GridModel?.FilterColumn?.ToolMenuList);
            cssList.CMSThemeId = cmsThemeId;
            //Set the total record count
            cssList.GridModel.TotalRecordCount = cssList.TotalResults;
            return ActionView(cssList);
        }

        //Update revised theme.
        public virtual ActionResult UpdateRevisedTheme(int cmsThemeId, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                SetNotificationMessage(_cmsThemeAgent.UpdateRevisedTheme(cmsThemeId, name)
                ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage)
                : GetErrorNotificationMessage(Admin_Resources.UpdateRevisedThemeError));
                return RedirectToAction<ThemeController>(x => x.List(null));
            }
            return RedirectToAction<ThemeController>(x => x.List(null));
        }

        //Delete theme.
        public virtual ActionResult Delete(string cmsThemeId, string name)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(cmsThemeId))
            {
                status = _cmsThemeAgent.DeleteTheme(cmsThemeId, name, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Delete Revised theme.
        public virtual ActionResult DeleteRevisedTheme(int cmsThemeId, string name)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(name))
            {
                status = _cmsThemeAgent.DeleteRevisedTheme(name, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Method to download theme by passing theme name.
        public virtual ActionResult DownloadTheme(string Name)
        {
            string filePath = _cmsThemeAgent.DownloadTheme(Name);
            if (!string.IsNullOrEmpty(filePath))
            {
                byte[] fileBytes = _cmsThemeAgent.GetZipFile(filePath, Name);
                return File(fileBytes, "application/zip", $"{ Name}.zip");
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorDownloadTheme));
            return RedirectToAction<ThemeController>(x => x.List(null));
        }

        //Create Css.
        [HttpGet]
        public virtual ActionResult AddCSS(int cmsThemeId, string cmsThemeName)
            => PartialView("_AddCss", new CSSViewModel { CMSThemeId = cmsThemeId, ThemeName = cmsThemeName });

        //Create Css.
        [HttpPost]
        public virtual ActionResult AddCSS(CSSViewModel cssViewModel)
        {
            if (ModelState.IsValid)
            {
                cssViewModel = _cmsThemeAgent.CreateCSS(cssViewModel);

                if (cssViewModel?.CMSThemeCSSId > 0)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(string.IsNullOrEmpty(cssViewModel.ErrorMessage) ? Admin_Resources.InvalidFolderStructure : cssViewModel.ErrorMessage));

                return RedirectToAction<ThemeController>(x => x.Manage(cssViewModel.CMSThemeId, null));
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorSelectCSSFile));
            return RedirectToAction<ThemeController>(x => x.Manage(cssViewModel.CMSThemeId, null));
        }

        //Delete Css.
        public virtual ActionResult DeleteCSS(string cmsThemeCssId, string cssName, string themeName)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(cmsThemeCssId))
            {
                status = _cmsThemeAgent.DeleteCss(cmsThemeCssId, cssName, themeName, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }


        //Download Css.
        public virtual ActionResult DownloadCSS(int cmsThemeId, string CSSName, string themeName)
        {
            string filePath = _cmsThemeAgent.DownloadCss(cmsThemeId, CSSName, themeName);
            if (!string.IsNullOrEmpty(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes($"{filePath}/{CSSName}");
                string fileName = CSSName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorDownloadCSS));
            return RedirectToAction<ThemeController>(x => x.Manage(cmsThemeId, null));
        }
        #endregion        

        #region Associate Store

        //Associate store to theme.
        public virtual JsonResult AssociateStore(int cmsThemeId, string storeIds)
        {
            bool status = false;
            string message = string.Empty;
            status = _cmsThemeAgent.AssociateStore(cmsThemeId, storeIds);
            return Json(new { status = status, message = status ? Admin_Resources.StoreThemeSuccessMessage : Admin_Resources.ErrorAssociateStoreToTheme }, JsonRequestBehavior.AllowGet);
        }

        //Unassociate store from theme.
        public virtual JsonResult RemoveAssociatedStores(string priceListPortalId)
        {
            if (!string.IsNullOrEmpty(priceListPortalId))
            {
                bool status = _cmsThemeAgent.RemoveAssociatedStores(priceListPortalId);
                return Json(new { status = status, message = status ? Admin_Resources.UnassignSuccessful : Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = Admin_Resources.ErrorFailedToDelete }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}