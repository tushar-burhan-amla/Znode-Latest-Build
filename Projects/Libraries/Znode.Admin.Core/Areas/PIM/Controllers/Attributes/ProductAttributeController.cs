using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Areas.PIM.Attributes.Controllers
{
    public class ProductAttributeController : BaseController
    {
        #region Private Variables
        private IPIMAttributeAgent _attributesAgent;
        private string AttributeLocaleView = "_AttributeLocale";
        private string DefaultValuesView = "_DefaultValues";
        private string attributeView = "_Attribute";
        private string validationRuleView = "_ValidationRule";
        private string attributeGroupView = "_AttributeGroup";
        private string frontEndPropertiesView = "_FrontEndProperties";
        private const string SwatchTypeView = "_SwatchType";

        #endregion

        #region Public Constructor
        public ProductAttributeController(IPIMAttributeAgent pIMAttributeAgent)
        {
            _attributesAgent = pIMAttributeAgent;
        }
        #endregion

        #region Public Methods

        // GET:AttributeList
        [MvcSiteMapNode(Title = "$Resources:PIM_Resources.PIM_Resources,TitleAttribute", Key = "ProductAttribute", Area = "PIM", ParentKey = "PIM")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePimAttribute.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePimAttribute.ToString(), model);

            return ActionView(_attributesAgent.AttributeList(model, GridListType.ZnodePimAttribute.ToString(), "0"));

        }
        //Get:CreateAttribute
        public virtual ActionResult Create() => View(_attributesAgent.Create(false));

        //Post:Create Attribute
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeDataViewModel attribute = _attributesAgent.Save(model);
            if (!attribute.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<ProductAttributeController>(x => x.Edit(attribute.AttributeViewModel.PimAttributeId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(attribute.ErrorMessage));
            return RedirectToAction<ProductAttributeController>(x => x.Create());
        }

        //Get:Attribute Update
        public virtual ActionResult Edit(int PimAttributeId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(AdminConstants.Create, _attributesAgent.GetAttribute(PimAttributeId, false));
        }
        //Post: Attribute Update
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            PIMAttributeDataViewModel attribute = _attributesAgent.Update(model);
            if (!attribute.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<ProductAttributeController>(x => x.Edit(attribute.AttributeViewModel.PimAttributeId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(attribute.ErrorMessage));
            return RedirectToAction<ProductAttributeController>(x => x.Edit(attribute.AttributeViewModel.PimAttributeId));
        }

        //Get:method To delete multiple exiting attribute value by attribute id
        public virtual JsonResult Delete(string pimAttributeId)
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

        public virtual ActionResult Attribute(int id = 0) => PartialView(attributeView, _attributesAgent.Attribute(id, false));

        //Get: Attribute Locale values
        public virtual ActionResult Locale(int id = 0) => PartialView(AttributeLocaleView, _attributesAgent.GetLocales(id));

        //Get: Attribute Default Values
        public virtual ActionResult DefaultValues(int attributeId = 0) => PartialView(DefaultValuesView, _attributesAgent.DefaultValues(attributeId));

        //Get:Validation Rule For Attribute
        public virtual ActionResult ValidationRule(int AttributeTypeId, int attributeId = 0) => PartialView(validationRuleView, _attributesAgent.AttributeInputValidations(AttributeTypeId, attributeId));

        //Get: FrontEndProperties for attribute
        public virtual ActionResult FrontEndProperties(int AttributeId = 0) => PartialView(frontEndPropertiesView, _attributesAgent.FrontEndProperties(AttributeId));

        //Get AttributeGroup List Data
        public virtual ActionResult AttributeGroup(int attributeGroupId = 0) => PartialView(attributeGroupView, _attributesAgent.AttributeGroupList(attributeGroupId, "0"));

        //Create Attribute Default Values
        public virtual JsonResult SaveDefaultValues(string model, int attributeId, string defaultvaluecode, bool isDefault, bool isswatch, string swatchtext, int displayOrder = 0, int defaultvalueId = 0) =>
         Json(new { defaultvalueId = _attributesAgent.SaveDefaultValues(model, attributeId, defaultvaluecode, isDefault, isswatch, swatchtext, displayOrder, defaultvalueId), mode = defaultvalueId > 0 ? AdminConstants.Edit : AdminConstants.Create }, JsonRequestBehavior.AllowGet);


        //Delete default attribute value.
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

        //Check attribute Code already exist or not
        [HttpGet]
        public virtual ActionResult IsAttributeCodeExist(string attributeCode, bool isCategory)
            => Json(new { data = _attributesAgent.IsAttributeCodeExist(attributeCode, isCategory) }, JsonRequestBehavior.AllowGet);

        //Check value of attribute is already exists or not.
        [HttpGet]
        public virtual ActionResult IsAttributeValueUnique(string attributeCodeValues, int id, bool isCategory)
           => Json(new { data = _attributesAgent.IsAttributeValueUnique(attributeCodeValues, id, isCategory) }, JsonRequestBehavior.AllowGet);

        //Check attribute default value code exist or not.
        [HttpGet]
        public virtual ActionResult IsAttributeDefaultValueCodeExist(int attributeId = 0, string attributeDefaultValueCode = null, int defaultValueId = 0)
            => Json(new { data = _attributesAgent.CheckAttributeDefaultValueCodeExist(attributeId, attributeDefaultValueCode, defaultValueId) }, JsonRequestBehavior.AllowGet);

        //Get: Attribute swatch image type
        public virtual ActionResult SwatchType(int id = 0) => PartialView(SwatchTypeView, _attributesAgent.Attribute(id, false));
        #endregion

    }
}