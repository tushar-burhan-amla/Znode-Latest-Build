using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Controllers
{
    public class PowerBIController : BaseController
    {
        #region Private Variables
        private readonly IPowerBIAgent _powerBIAgent;
        private readonly IGeneralSettingAgent _generalSettingAgent;
        #endregion

        #region Public Constructor
        public PowerBIController(IPowerBIAgent powerBIAgent, IGeneralSettingAgent generalSettingAgent)
        {
            _generalSettingAgent = generalSettingAgent;
            _powerBIAgent = powerBIAgent;
        }
        #endregion       

        #region Public Methods
        //Method to get PowerBI reports
        public virtual async Task<ActionResult> GetPowerBIReport()
            =>  View("PowerBIReport", await _powerBIAgent.GetPowerBIReportsData(_generalSettingAgent.GetPowerBISettings()));
        #endregion

    }
}
