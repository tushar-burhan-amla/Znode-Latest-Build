using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ERPConnectorController : BaseController
    {
        #region Private Variables
        private readonly IERPConfiguratorAgent _erpConfiguratorAgent;
        private readonly IERPConnectorAgent _erpConnectorAgent;
        #endregion

        #region Constructor
        public ERPConnectorController(IERPConfiguratorAgent erpConfiguratorAgent, IERPConnectorAgent erpConnectorAgent)
        {
            _erpConfiguratorAgent = erpConfiguratorAgent;
            _erpConnectorAgent = erpConnectorAgent;
        }
        #endregion

        #region Public Action Methods
        [HttpGet]
        public ActionResult CreateConnectionAttributes()
        {
            //Get Active ERP ClassName
            string erpClassName = _erpConfiguratorAgent.GetERPClassName();
            if (!string.IsNullOrEmpty(erpClassName))
            {
                ERPConnectorListViewModel erpConnectorListViewModel = new ERPConnectorListViewModel();
                erpConnectorListViewModel = _erpConnectorAgent.GetERPConnectorControls();
                erpConnectorListViewModel.ERPClassName = _erpConfiguratorAgent.GetERPClassName();
                return View("~/Views/ERPConnector/CreateConnectionAttribute.cshtml", erpConnectorListViewModel);
            }
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(ERP_Resources.ERPClassFailedMessage));
                return RedirectToAction<ProviderEngineController>(x => x.ERPConfiguratorList(null));
            }
        }

        //POST: Action to Save ERP Control Data in json file.
        [HttpPost]
        public virtual ActionResult CreateConnectionAttributes([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            ERPConnectorListViewModel viewModel = _erpConnectorAgent.CreateERPControlData(model);
            if (!viewModel.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<ERPConnectorController>(x => x.CreateConnectionAttributes());
            }
            SetNotificationMessage(GetErrorNotificationMessage(viewModel.ErrorMessage));
            return ActionView(AdminConstants.CreateEdit, viewModel);
        }
        #endregion
    }
}
