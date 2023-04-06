using Znode.Engine.Api.Client;

namespace Znode.Engine.Admin.Agents
{
    public class HelperAgent : BaseAgent, IHelperAgent
    {
        #region Private Variables
        private readonly IHelperClient _helperClient;
        #endregion

        #region Constructor
        public HelperAgent(IHelperClient helperClient)
        {
            _helperClient = GetClient<IHelperClient>(helperClient);
        }
        #endregion

        public bool IsCodeExists(string code, string service, string methodName)
              => (_helperClient.CheckCodeExist(code, service, methodName));
    }
}
