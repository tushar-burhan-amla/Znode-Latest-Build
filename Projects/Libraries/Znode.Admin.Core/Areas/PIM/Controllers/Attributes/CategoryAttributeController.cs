using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.PIM.Controllers.Attributes
{
    public class CategoryAttributeController : BaseController
    {
        #region Private Variables
        private IPIMAttributeAgent _attributesAgent;
        private string categoryAttributeCreateView = "~/Areas/PIM/Views/ProductAttribute/Create.cshtml";
        #endregion

        #region Public Constructor
        public CategoryAttributeController(IPIMAttributeAgent pIMAttributeAgent)
        {
            _attributesAgent = pIMAttributeAgent;
        }
        #endregion

        #region Public Methods

        // GET:AttributeList
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleAttribute", Key = "CategoryAttribute", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCategoryAttribute.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCategoryAttribute.ToString(), model);
            return ActionView("~/Areas/PIM/Views/ProductAttribute/List.cshtml", _attributesAgent.AttributeList(model, GridListType.ZnodeCategoryAttribute.ToString(), "1"));
        }

        //Get:CreateAttribute
        public virtual ActionResult Create() => View(categoryAttributeCreateView, _attributesAgent.Create(true));

        //Post:Create Attribute
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeDataViewModel attribute = _attributesAgent.Save(model);
            if (!attribute.HasError)
            {
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                return RedirectToAction<CategoryAttributeController>(x => x.Edit(attribute.AttributeViewModel.PimAttributeId));
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attribute.ErrorMessage, NotificationType.error);
            return RedirectToAction<CategoryAttributeController>(x => x.Create());
        }

        //Get:Attribute Update
        public virtual ActionResult Edit(int PimAttributeId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(categoryAttributeCreateView, _attributesAgent.GetAttribute(PimAttributeId, true));
        }
        //Post: Attribute Update
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeDataViewModel attribute = _attributesAgent.Update(model);

            if (!attribute.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<CategoryAttributeController>(x => x.Edit(attribute.AttributeViewModel.PimAttributeId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));
            return RedirectToAction<CategoryAttributeController>(x => x.Edit(attribute.AttributeViewModel.PimAttributeId));
        }

        //Get:method To delete multiple exiting attribute value by attribute id
        public virtual ActionResult Delete(string pimAttributeId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(pimAttributeId))
            {
                status = _attributesAgent.DeleteAttribute(pimAttributeId, out message);
                if (status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult Attribute(int id = 0) => PartialView("~/Areas/PIM/Views/ProductAttribute/_Attribute.cshtml", _attributesAgent.Attribute(id, true));

        //Get: Attribute Locale values
        public virtual ActionResult Locale(int id = 0) => PartialView("~/Areas/PIM/Views/ProductAttribute/_AttributeLocale.cshtml", _attributesAgent.GetLocales(id));

        //Get: Attribute Default Values
        public virtual ActionResult DefaultValues(int attributeId = 0) => PartialView("~/Areas/PIM/Views/ProductAttribute/_DefaultValues.cshtml", _attributesAgent.DefaultValues(attributeId));

        //Get:Validation Rule For Attribute
        public virtual ActionResult ValidationRule(int AttributeTypeId, int attributeId = 0) => PartialView("~/Areas/PIM/Views/ProductAttribute/_ValidationRule.cshtml", _attributesAgent.AttributeInputValidations(AttributeTypeId, attributeId));

        //Get AttributeGroup List Data
        public virtual ActionResult AttributeGroup(int attributeGroupId = 0) => PartialView("~/Areas/PIM/Views/ProductAttribute/_AttributeGroup.cshtml", _attributesAgent.AttributeGroupList(attributeGroupId, "1"));

        //Create Attribute Default Values
        public virtual JsonResult SaveDefaultValues(string model, int attributeId, string defaultvaluecode, int displayOrder = 0, int defaultvalueId = 0)
            => Json(new { defaultvalueId = _attributesAgent.SaveDefaultValues(model, attributeId, defaultvaluecode, false, false, null, displayOrder, defaultvalueId), mode = defaultvalueId > 0 ? AdminConstants.Edit : AdminConstants.Create }, JsonRequestBehavior.AllowGet);

        //Delete defaultvalues.
        public virtual JsonResult DeleteDefaultValues(int defaultvalueId)
        {
            if (defaultvalueId <= 0)
                return null;

            bool status = false;
            string errorMessage = string.Empty;
            status = _attributesAgent.DeleteDefaultValues(defaultvalueId, out errorMessage);
            return status ? Json(new { success = status, statusMessage = Admin_Resources.DeleteMessage }, JsonRequestBehavior.AllowGet) : Json(new { success = status, statusMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}