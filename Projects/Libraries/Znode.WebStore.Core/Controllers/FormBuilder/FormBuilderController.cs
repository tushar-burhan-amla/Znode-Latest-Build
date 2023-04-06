using System.Web.Mvc;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.WebStore.Core.Agents;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Controllers
{
    public class FormBuilderController : BaseController
    {
        #region Private Members
        private IFormBuilderAgent _formBuilderAgent;
        #endregion

        #region Constructor
        public FormBuilderController(IFormBuilderAgent formBuilderAgent)
        {
            _formBuilderAgent = formBuilderAgent;
        }
        #endregion

        #region Public Method
        //Get Entity Attribute Details based on EntityId & Entity Type. 
        [HttpGet]
        public virtual ActionResult Get(int formBuilderId, int localeId, int mappingId = 0)
            => ActionView("FormTemplate", _formBuilderAgent.GetFormTemplate(formBuilderId, localeId, mappingId));

        [HttpPost]
        public virtual JsonResult Create([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            FormSubmitViewModel viewmodel = _formBuilderAgent.CreateFormTemplate(model);

            return Json(new
            {
                message = viewmodel.SuccessMessage,
            }, JsonRequestBehavior.AllowGet);
        }

        //Check value of attribute is already exists or not.
        [HttpGet]
        public virtual ActionResult FormAttributeValueUnique(GlobalAttributeValueParameterModel model)
           => Json(new { data = _formBuilderAgent.IsFormAttributeValueUnique(model) }, JsonRequestBehavior.AllowGet);
        #endregion
    }
}
