using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Controllers;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin
{
    public class GlobalAttributeController : BaseController
    {
        #region Private Variables
        private readonly IGlobalAttributeAgent _globalAttributeAgent;
        private readonly  IGlobalAttributeGroupAgent _globalAttributeGroupAgent;
        private readonly IGlobalAttributeEntityAgent _globalAttributeEntityAgent;
        private const string AttributeLocaleView = "_AttributeLocale";
        private const string DefaultValuesView = "_DefaultValues";
        private const string attributeView = "_Attribute";
        private const string validationRuleView = "_ValidationRule";
        private const string SwatchTypeView = "_SwatchType";
        private const string AssociateGroupToEntity = "AssociateGroupToEntity";
        private const string _AttributeEntityGroupListView = "_AttributeEntiyGroupList";
        private const string UnAssignedAttributeEntityGroups = "_UnAssignedAttributeEntityGroups";
        #endregion

        #region Public Constructor
        public GlobalAttributeController(IGlobalAttributeAgent globalAttributeAgent, IGlobalAttributeGroupAgent globalAttributeGroupAgent, IGlobalAttributeEntityAgent globalAttributeEntityAgent)
        {
            _globalAttributeAgent = globalAttributeAgent;
            _globalAttributeGroupAgent = globalAttributeGroupAgent;
            _globalAttributeEntityAgent = globalAttributeEntityAgent;
        }
        #endregion

        #region Public Methods
        // GET:AttributeList
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,TitleGlobalAttribute", Key = "GlobalAttribute", Area = "", ParentKey = "Admin")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model,int entityId = 0, string entityType = null ) =>
           ActionView(_globalAttributeAgent.AttributeList(model, GridListType.ZnodeGlobalAttribute.ToString(), entityId,  entityType ));

        //Get:CreateAttribute
        public virtual ActionResult Create() => View("Create", _globalAttributeAgent.Create());

        //Post:Create Attribute
        [HttpPost]
        public virtual ActionResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            GlobalAttributeViewModel attribute = _globalAttributeAgent.Save(model);
            if (!attribute.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<GlobalAttributeController>(x => x.Edit(attribute.GlobalAttributeId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(attribute.ErrorMessage));
            return RedirectToAction<GlobalAttributeController>(x => x.Create());
        }

        //Get:Attribute Update
        public virtual ActionResult Edit(int globalAttributeId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return View(AdminConstants.Create, _globalAttributeAgent.GetAttributeData(globalAttributeId));
        }

        //Post: Attribute Update
        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            GlobalAttributeViewModel attribute = _globalAttributeAgent.Update(model);
            if (!attribute.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<GlobalAttributeController>(x => x.Edit(attribute.GlobalAttributeId));
            }

            SetNotificationMessage(GetErrorNotificationMessage(attribute.ErrorMessage));
            return RedirectToAction<GlobalAttributeController>(x => x.Edit(attribute.GlobalAttributeId));
        }

        //Get: Method To delete multiple exiting attributes by attribute id
        public virtual JsonResult Delete(string globalAttributeId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(globalAttributeId))
            {
                status = _globalAttributeAgent.DeleteAttribute(globalAttributeId, out message);

                if (status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Get attribute details.
        public virtual ActionResult Attribute(int id = 0) => PartialView(attributeView, _globalAttributeAgent.GetAttributeData(id));

        //Get: Attribute Locale values
        public virtual ActionResult Locale(int id = 0) => PartialView(AttributeLocaleView, _globalAttributeAgent.GetLocales(id));

        //Get: Attribute Default Values
        public virtual ActionResult DefaultValues(int attributeId = 0) => PartialView(DefaultValuesView, _globalAttributeAgent.GetDefaultValues(attributeId));

        //Get:Validation Rule For Attribute
        public virtual ActionResult ValidationRule(int AttributeTypeId, int attributeId = 0) => PartialView(validationRuleView, _globalAttributeAgent.AttributeInputValidations(AttributeTypeId, attributeId));

        //Check attribute code already exist or not
        [HttpGet]
        public virtual ActionResult IsAttributeCodeExist(string attributeCode)
            => Json(new { data = _globalAttributeAgent.IsAttributeCodeExist(attributeCode) }, JsonRequestBehavior.AllowGet);

        //Create Attribute Default Values
        public virtual JsonResult SaveDefaultValues(string model, int attributeId, string defaultvaluecode, bool isDefault, bool isswatch, string swatchtext, int displayOrder = 0, int defaultvalueId = 0) =>
         Json(new { defaultvalueId = _globalAttributeAgent.SaveDefaultValues(model, attributeId, defaultvaluecode, isDefault, isswatch, swatchtext, displayOrder, defaultvalueId), mode = defaultvalueId > 0 ? AdminConstants.Edit : AdminConstants.Create }, JsonRequestBehavior.AllowGet);

        //Check attribute default value code exist or not.
        [HttpGet]
        public virtual ActionResult IsAttributeDefaultValueCodeExist(int attributeId = 0, string attributeDefaultValueCode = null, int defaultValueId = 0)
            => Json(new { data = _globalAttributeAgent.CheckAttributeDefaultValueCodeExist(attributeId, attributeDefaultValueCode, defaultValueId) }, JsonRequestBehavior.AllowGet);

        //Delete default attribute value.
        public virtual JsonResult DeleteDefaultValues(int defaultvalueId)
        {
            if (defaultvalueId <= 0)
                return null;

            bool status = false;
            string errorMessage = string.Empty;
            status = _globalAttributeAgent.DeleteDefaultValues(defaultvalueId, out errorMessage);
            return status ? Json(new { success = status, statusMessage = Admin_Resources.DeleteMessage }, JsonRequestBehavior.AllowGet)
                : Json(new { success = status, statusMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Get: Attribute swatch image type
        public virtual ActionResult SwatchType(int id = 0) => PartialView(SwatchTypeView, _globalAttributeAgent.GetAttributeData(id));

        //Check value of attribute is already exists or not.
        [HttpGet]
        public virtual ActionResult IsGlobalAttributeValueUnique(GlobalAttributeValueParameterModel model)
           => Json(new { data = _globalAttributeAgent.IsAttributeValueUnique(model) }, JsonRequestBehavior.AllowGet);

        #region AssociateEntity
        public virtual ActionResult AssociateGroupEntity() => View(AssociateGroupToEntity, _globalAttributeEntityAgent.GetGlobalEntity());


        [HttpGet]
        public virtual ActionResult AssignedEntityAttributeGroupList(int id)
        {
            if (id > 0)
            {
                TempData[AdminConstants.EntityId] = id;
                AssignedEntityGroupListViewModel listViewModel = _globalAttributeEntityAgent.GetAssignedEntityAttributeGroups(id);

                return PartialView(_AttributeEntityGroupListView, listViewModel);
            }
            return PartialView(_AttributeEntityGroupListView, new AssignedEntityGroupListViewModel());
        }

        public virtual ActionResult GetUnAssignedAttributeEntityGroups(int id)
           => PartialView(UnAssignedAttributeEntityGroups, _globalAttributeEntityAgent.GetUnAssignedEntityAttributeGroups(id));

        public virtual JsonResult AssignAttributeEntityToGroups(string selectedIds)
        {
            bool status = false;

            if (selectedIds?.Length > 0)
            {
                string message = string.Empty;
                status = _globalAttributeEntityAgent.AssignAttributeEntityToGroups(selectedIds, (int)TempData[AdminConstants.EntityId], out message);
                return Json(new { HasNoError = status, Message = status ? Admin_Resources.AssignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { HasNoError = status, Message = Admin_Resources.PleaseSelectAtleastOneRecord }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult UnAssignAttributeGroups(int groupId, int entityId)
        {
            bool status = false;
            string message = string.Empty;

            if (entityId > 0 && groupId > 0)
            {
                status = (_globalAttributeEntityAgent.UnAssignEntityGroups(entityId, groupId, out message));
                return Json(new { status = status, Message = status ? Admin_Resources.UnassignSuccessful : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = status, Message = Admin_Resources.TextInvalidData }, JsonRequestBehavior.AllowGet);
        }

        //Update attribute group display order.
        public virtual ActionResult UpdateAttributeGroupDisplayOrder(int globalAttributeGroupId = 0, int globalAttributeEntityId = 0, int displayOrder = 0)
        {
            GlobalAttributeGroupViewModel attributeGroup = _globalAttributeEntityAgent.UpdateAttributeGroupDisplayOrder(globalAttributeGroupId, globalAttributeEntityId, displayOrder);

            if (!attributeGroup.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                return Json(new { status = false, message = Admin_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get Entity Attribute Details based on EntityId & Entity Type. 
        [HttpGet]
        public virtual ActionResult GetEntityAttributeDetails(int entityId, string entityType)
            => View(AdminConstants.GlobalAttributeEntityView, _globalAttributeEntityAgent.GetEntityAttributeDetails(entityId, entityType));

        //Get assigned global attributes for a group.
        [HttpGet]
        public virtual ActionResult AssignedGlobalAttributes(int attributeGroupId, [ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model)
        {
            GlobalAttributeGroupMapperListViewModel attributeGroupMappers;
            if (attributeGroupId > 0)
            {
                //If global attribute group id already present in filters then remove it.
                model.Filters.RemoveAll(x => x.Item1 == FilterKeys.GlobalAttributeGroupId);

                //Add new global attribute group id filter.
                model.Filters.Add(new FilterTuple(FilterKeys.GlobalAttributeGroupId, FilterOperators.Equals, attributeGroupId.ToString()));
                attributeGroupMappers = _globalAttributeGroupAgent.GetAssignedAttributes(model.Filters, model.SortCollection, null, 1, int.MaxValue);
                return PartialView("_GlobalAttributeData", attributeGroupMappers);
            }
            return null;
        }

        //Save Entity Attribute Details based on EntityId & Entity Type. 
        [HttpPost]
        public virtual ActionResult SaveEntityDetails([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            ActionResult action = GotoBackURL();
            string errorMessage = string.Empty;

            EntityAttributeViewModel entityAttributeViewModel = _globalAttributeEntityAgent.SaveEntityAttributeDetails(model, out errorMessage);
            SetNotificationMessage(entityAttributeViewModel.IsSuccess ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(errorMessage));

            return  IsNotNull(action) ? action : RedirectToAction<GlobalAttributeController>(x => x.GetEntityAttributeDetails(entityAttributeViewModel.EntityValueId, entityAttributeViewModel.EntityType));
        }

        //Create Tab structure for global attribute entity.
        public virtual ActionResult GetTabStructure(int globalEntityId = 0)
        => PartialView(AdminConstants.TabStructurePath, _globalAttributeEntityAgent.CreateTabStructure(globalEntityId));
        #endregion
        #endregion
    }
}