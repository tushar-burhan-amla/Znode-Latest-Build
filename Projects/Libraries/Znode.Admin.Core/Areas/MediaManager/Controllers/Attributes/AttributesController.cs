using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
namespace Znode.Engine.Admin.Areas.MediaManager.Attributes.Controllers
{
    public class AttributesController : BaseController
    {
        #region Private Variables
        private IAttributesAgent _attributesAgent;
        private string AttributeLocaleView = "_AttributeLocale";
        private string DefaultValuesView = "_DefaultAttributeValues";
        private string attributeView = "_Attribute";
        private string validationRuleView = "~/Areas/PIM/Views/ProductAttribute/_ValidationRule.cshtml";
        private string attributeGroupView = "_AttributeGroup";
        #endregion

        #region Default Constructor
        public AttributesController(IAttributesAgent attributesAgent)
        {
            _attributesAgent = attributesAgent;
        }
        #endregion

        //Get Media Attribute List
        [MvcSiteMapNode(Title = "$Resources:MediaManager_Resources.MediaManager_Resources, TitleAttributes", Key = "Attribute", Area = "MediaManager", ParentKey = "MediaManager")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeMediaAttribute.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeMediaAttribute.ToString(), model);
            return ActionView(_attributesAgent.AttributeList(model));
        }
        //Get:CreateAttribute
        public virtual ActionResult Create() => View(_attributesAgent.Create());

        //Post:Create Attribute
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            AttributesViewModel attribute = _attributesAgent.Save(model);
            if (!attribute.HasError)
            {
                TempData[AdminConstants.Notifications] = GenerateNotificationMessages(Admin_Resources.RecordCreationSuccessMessage, NotificationType.success);
                return RedirectToAction<AttributesController>(x => x.Edit(attribute.MediaAttributeId));
            }
            TempData[AdminConstants.Notifications] = GenerateNotificationMessages(attribute.ErrorMessage, NotificationType.error);
            return RedirectToAction<AttributesController>(x => x.Create());
        }

        //Get:Attribute Update
        public virtual ActionResult Edit(int mediaAttributeId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(AdminConstants.Create, _attributesAgent.GetAttribute(mediaAttributeId));
        }

        //Post:Attribute Update
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            AttributesViewModel attribute = _attributesAgent.Update(model);
            if (!attribute.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<AttributesController>(x => x.Edit(attribute.MediaAttributeId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateError));
            return RedirectToAction<AttributesController>(x => x.Edit(attribute.MediaAttributeId));
        }

        //Delete Attribute By Media Attribute Ids
        [HttpGet]
        public virtual JsonResult Delete(string mediaAttributeId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(mediaAttributeId))
            {
                status = _attributesAgent.DeleteAttribute(mediaAttributeId, out message);

                if (status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get: Attribute Default Values
        public virtual ActionResult DefaultValues(int mediaAttributeId = 0) => PartialView(DefaultValuesView, _attributesAgent.DefaultValues(mediaAttributeId));

        //Create Attribute Default Values
        public virtual JsonResult SaveDefaultValues(string model, int attributeId, string defaultvaluecode, int defaultvalueId = 0) =>
                Json(new { defaultvalueId = _attributesAgent.SaveDefaultValues(model, attributeId, defaultvaluecode, defaultvalueId), mode = defaultvalueId > 0 ? AdminConstants.Edit : AdminConstants.Create }, JsonRequestBehavior.AllowGet);

        //Delete Default Values
        public virtual JsonResult DeleteDefaultValues(int defaultvalueId)
        {
            if (defaultvalueId <= 0)
                return null;

            bool status = false;
            string errorMessage = string.Empty;
            status = _attributesAgent.DeleteDefaultValues(defaultvalueId, out errorMessage);
            return status ? Json(new { success = status, statusMessage = Admin_Resources.DeleteMessage }, JsonRequestBehavior.AllowGet)
                : Json(new { success = status, statusMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get: Attribute Locale values
        public virtual ActionResult Locale(int id = 0) => PartialView(AttributeLocaleView, _attributesAgent.GetLocales(id));

        //Get attribute data by id
        public virtual ActionResult Attribute(int id = 0) => PartialView(attributeView, _attributesAgent.GetAttribute(id));

        //Get:Validation Rule For Attribute
        public virtual ActionResult ValidationRule(int AttributeTypeId, int attributeId = 0)
        {
            var model = _attributesAgent.AttributeInputValidations(AttributeTypeId, attributeId);
            model.Remove(model?.Find(x => x.Name == "UniqueValue"));
            return PartialView(validationRuleView, model);
        }

        //Get attribute groups
        public virtual ActionResult GetAttributeGroups(int attributeGroupId = 0)
            => PartialView(attributeGroupView, _attributesAgent.AttributeGroupList(attributeGroupId));

        //Get:Validation Rule For Attribute
        public virtual ActionResult ValidationRuleRegularExpression(int AttributeTypeId, string ruleName = null)
            => Json(new { data = _attributesAgent.GetValidationRuleRegularExpression(AttributeTypeId, ruleName) }, JsonRequestBehavior.AllowGet);

        //Check attribute Code already exist or not
        [HttpGet]
        public virtual ActionResult IsAttributeCodeExist(string attributeCode)
            => Json(new { data = _attributesAgent.IsAttributeCodeExist(attributeCode) }, JsonRequestBehavior.AllowGet);
        
        //Check attribute default value code already exist or not.
        [HttpGet]
        public virtual ActionResult IsAttributeDefaultValueCodeExist(int attributeId = 0, string attributeDefaultValueCode = null, int defaultValueId = 0)
           => Json(new { data = _attributesAgent.CheckAttributeDefaultValueCodeExist(attributeId, attributeDefaultValueCode, defaultValueId) }, JsonRequestBehavior.AllowGet);
    }
}
