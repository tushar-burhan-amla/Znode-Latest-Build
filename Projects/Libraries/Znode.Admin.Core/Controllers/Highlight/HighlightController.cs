using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class HighlightController : BaseController
    {
        #region Private Variable
        private readonly IHighlightAgent _highlightAgent;
        #endregion

        public HighlightController(IHighlightAgent highlightAgent)
        {
            _highlightAgent = highlightAgent;
        }

        //Get highlight list.     
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int localeId = 0)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeHighlight.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeHighlight.ToString(), model);
            //Get list of highlight.
            HighlightListViewModel highlightList = _highlightAgent.Highlights(model.Filters, model.SortCollection, model.Page, model.RecordPerPage, localeId);

            //Get the grid model.
            highlightList.GridModel = FilterHelpers.GetDynamicGridModel(model, highlightList.HighlightList, GridListType.ZnodeHighlight.ToString(), string.Empty, null, true, true, highlightList?.GridModel?.FilterColumn?.ToolMenuList);

            highlightList.GridModel.TotalRecordCount = highlightList.TotalResults;

            return ActionView(highlightList);
        }

        //Get:Create Highlight.
        [HttpGet]
        public virtual ActionResult Create()
        {
            HighlightViewModel highlightViewModel = new HighlightViewModel();
            _highlightAgent.BindDropdownValues(highlightViewModel);
            _highlightAgent.BindDefaultValues(highlightViewModel);
            return (Request.IsAjaxRequest()) ? PartialView(AdminConstants.CreateEditHighlightView, highlightViewModel) : ActionView(AdminConstants.ManageView, highlightViewModel);
        }

        //Post:Create Highlight.
        [HttpPost]
        public virtual ActionResult Create(HighlightViewModel highlightViewModel)
        {
            if (ModelState.IsValid)
            {
                highlightViewModel = _highlightAgent.CreateHighlight(highlightViewModel);
                if (!highlightViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<HighlightController>(x => x.Edit(highlightViewModel.HighlightId, highlightViewModel.LocaleId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(highlightViewModel.ErrorMessage));
            _highlightAgent.BindDropdownValues(highlightViewModel);
            return ActionView(AdminConstants.ManageView, highlightViewModel);
        }

        //Get:Edit Highlight.
        [HttpGet]
        public virtual ActionResult Edit(int highlightId, int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return RedirectToAction<HighlightController>(x => x.List(null, localeId));

            return Request.IsAjaxRequest() ? PartialView("_HighlightForLocale", _highlightAgent.GetHighlight(highlightId, localeId)) : ActionView(AdminConstants.ManageView, _highlightAgent.GetHighlight(highlightId, localeId));
        }

        //Post:Edit Highlight.
        [HttpPost]
        public virtual ActionResult Edit(HighlightViewModel highlightViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_highlightAgent.UpdateHighlight(highlightViewModel).HasError)
                    SetNotificationMessage(GetErrorNotificationMessage(highlightViewModel.ErrorMessage));
                else
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));

                return RedirectToAction<HighlightController>(x => x.Edit(highlightViewModel.HighlightId, highlightViewModel.LocaleId));
            }
            return ActionView(AdminConstants.ManageView, _highlightAgent.GetHighlight(highlightViewModel.HighlightId, highlightViewModel.LocaleId));
        }

        //Delete Highlight.
        public virtual JsonResult Delete(string highlightId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(highlightId))
            {
                status = _highlightAgent.DeleteHighlight(highlightId);

                message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Action for highlight Products list.
        public virtual ActionResult HighlightProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int localeId = 0, int highlightId = 0, string highlightCode = null)
         => ActionView(_highlightAgent.GetHighlightProductList(model, localeId, highlightId, highlightCode));

        //Action for highlight Products list.
        public virtual ActionResult GetUnassociatedHighlightProductList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int localeId = 0, string highlightCode = null)
        => ActionView("ProductPopupList", _highlightAgent.GetUnAssociatedProductList(model, localeId, highlightCode));

        //Action to associate highlight products.
        public virtual JsonResult AssociateHighlightProducts(string highlightCode, string productIds)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (HelperUtility.IsNotNull(productIds) && HelperUtility.IsNotNull(highlightCode))
            {
                status = _highlightAgent.AssociateHighlightProducts(highlightCode, productIds);
                errorMessage = status ? Admin_Resources.AssociateProductSuccessMessage : Admin_Resources.ErrorAssociateProductMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete associated products.
        public virtual ActionResult UnAssociateHighlightProducts(string pimProductId, string attributevalue)
        {
            string errorMessage = Admin_Resources.ErrorUnAssociateHighlightProducts;
            bool status = false;
            if (HelperUtility.IsNotNull(pimProductId))
            {
                status = _highlightAgent.UnAssociateHighlightProduct(attributevalue, pimProductId);
                errorMessage = status ? Admin_Resources.ProductUnassociatedSuccessfully : Admin_Resources.Unabletounassociateproduct;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}