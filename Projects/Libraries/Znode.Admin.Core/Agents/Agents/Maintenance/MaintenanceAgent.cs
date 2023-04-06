using System;
using System.Diagnostics;

using Znode.Engine.Api.Client;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class MaintenanceAgent : BaseAgent, IMaintenanceAgent
    {
        #region Private Variables

        private readonly IMaintenanceClient _maintenanceClient;

        #endregion Private Variables

        #region Constructor

        public MaintenanceAgent(IMaintenanceClient maintenanceClient)
        {
            _maintenanceClient = GetClient<IMaintenanceClient>(maintenanceClient);
        }

        #endregion Constructor

        #region Public Methods

        //To delete published data of all catalog, store,cms & elastic search.
        public virtual bool PurgeAllPublishedData()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", "Maintenance", TraceLevel.Info);

            bool status = false;
            try
            {
                status = _maintenanceClient.PurgeAllPublishedData();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Maintenance", TraceLevel.Error);
            }

            ZnodeLogging.LogMessage("Agent method execution completed.", "Maintenance", TraceLevel.Info);
            return status;
        }

        #endregion Public Methods
    }
}
