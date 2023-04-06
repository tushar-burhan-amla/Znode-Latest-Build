using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Agents;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using System.Web;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Controllers
{
    public class RecommendationController : BaseController
    {
        #region Private Variables
        private readonly IRecommendationAgent _recommendationAgent;
        private readonly IERPTaskSchedulerAgent _erpTaskSchedulerAgent;
        private const string _ManagePartialView = "_RecommendationSettingPartial";
        private const string createSchedulerView = "~/Views/TouchPointConfiguration/Create.cshtml";
        #endregion

        #region Constructors
        public RecommendationController(IRecommendationAgent recommendationAgent, IERPTaskSchedulerAgent erpTaskSchedulerAgent)
        {
            _recommendationAgent = recommendationAgent;
            _erpTaskSchedulerAgent = erpTaskSchedulerAgent;
        }
        #endregion

        #region Public Methods
        public virtual ActionResult GetRecommendationSetting(int portalId)
        {
            //Get the available WebSite Logo details.
            RecommendationSettingViewModel model = _recommendationAgent.GetRecommendationSetting(portalId, "RecommendationData_" + portalId + "_true");
            if (model?.PortalId > 0)
            {
                return ActionView(_ManagePartialView, model);
            }
            //To show message box on store experience list page.
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorPortalNotFound));
            return RedirectToAction<Znode.Admin.Core.Controllers.StoreExperienceController>(x => x.List(null));
        }

        [HttpPost]
        public virtual ActionResult SaveRecommendationSetting(RecommendationSettingViewModel model)
        {
            ActionResult action = GotoBackURL();
            if (ModelState.IsValid)
            {
                RecommendationSettingViewModel recommendationSettingsModel = _recommendationAgent.SaveRecommendationSetting(model);
                string message = IsNotNull(recommendationSettingsModel) && !recommendationSettingsModel.HasError ? Admin_Resources.UpdateMessage : Admin_Resources.UpdateErrorMessage;

                //To show message box on store experience list page after click on save & close.
                if (IsNotNull(action))
                {
                    SetNotificationMessage(IsNotNull(recommendationSettingsModel) && !recommendationSettingsModel.HasError ? GetSuccessNotificationMessage(Admin_Resources.UpdateMessage) : GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                    message = "";
                }

                return Json(new { status = IsNotNull(recommendationSettingsModel) && !recommendationSettingsModel.HasError, message = message, Url = IsNull(action) ? "" : ((RedirectResult)action).Url }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //To show message box on store experience list page after click on save & close.
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
                return Json(new { status = false, Url = IsNull(action) ? "" : ((RedirectResult)action).Url }, JsonRequestBehavior.AllowGet);
            }            
        }

        [HttpPost]
        public virtual ActionResult GenerateRecommendationData(RecommendationDataGenerateViewModel recommendationDataGenerateViewModel)
        {
            if (ModelState.IsValid)
            {
                RecommendationGeneratedDataViewModel recommendationDataViewModel = _recommendationAgent.GenerateRecommendationData(recommendationDataGenerateViewModel);

                string message = Admin_Resources.RecommendationDataGenerationFailed;
                bool status = false;

                if (recommendationDataViewModel?.IsDataGenerationStarted == true)
                {
                    message = Admin_Resources.RecommendationDataGenerationStarted;
                    status = true;
                }                    
                else if (recommendationDataViewModel?.HasError == true)
                    message = recommendationDataViewModel.ErrorMessage;

                return Json(new { status = status, message = message, hasError = recommendationDataViewModel.HasError }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, hasError = true, message = Admin_Resources.RecommendationDataGenerationFailed }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get type method to create new ERP task scheduler.
        [HttpGet]
        public virtual ActionResult CreateScheduler(string connectorTouchPoints, string indexName = "", string schedulerCallFor = "", int portalId = 0, int catalogId = 0, int catalogIndexId = 0)
        {
            if (!string.IsNullOrEmpty(connectorTouchPoints))
            {
                connectorTouchPoints = HttpUtility.UrlDecode(connectorTouchPoints);
                int erpTaskSchedulerId = _erpTaskSchedulerAgent.GetSchedulerIdByTouchPointName(connectorTouchPoints, schedulerCallFor);
                ERPTaskSchedulerViewModel erpTaskSchedulerViewModel = new ERPTaskSchedulerViewModel();

                if (erpTaskSchedulerId == 0)
                    erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.SetTaskSchedulerData(connectorTouchPoints, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId);
                else
                    erpTaskSchedulerViewModel = _erpTaskSchedulerAgent.GetTaskSchedulerDataForUpdate(erpTaskSchedulerId, indexName, schedulerCallFor, portalId, catalogId, catalogIndexId);

                return ActionView(createSchedulerView, erpTaskSchedulerViewModel);
            }
            else
                return new EmptyResult();
        }
        #endregion
    }
}
