using System.Collections.Generic;
using Znode.Engine.Api.Client;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class ServerValidationAgent : BaseAgent, IServerValidationAgent
    {
        #region Private Variables
        private readonly IServerValidationClient _ServerValidationClient;
        #endregion

        #region Constructor
        public ServerValidationAgent(IServerValidationClient serverValidationClient)
        { 
            _ServerValidationClient = GetClient<IServerValidationClient>(serverValidationClient);
        }
        #endregion

        #region Public Methods
        public virtual Dictionary<string, string> Validate(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            return _ServerValidationClient.validateControl(ServerValidateViewModelMap.ToModel(model));
        }
        #endregion
    }
}