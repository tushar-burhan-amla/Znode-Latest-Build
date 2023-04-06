using System.Diagnostics;
using Znode.Engine.Api.Client;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public class DynamicContentAgent : BaseAgent, IDynamicContentAgent
    {
        #region Private Variables
        private readonly IDynamicContentClient _dynamicContentClient;
        #endregion

        #region Constructors
        public DynamicContentAgent(IDynamicContentClient dynamicContentClient)
        {
            _dynamicContentClient = GetClient<IDynamicContentClient>(dynamicContentClient); 
        }
        #endregion

        #region Public methods
        //to get WYSIWYG editor formats by portal Id.
        public virtual EditorFormatListViewModel GetEditorFormats(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            EditorFormatListViewModel modelList = _dynamicContentClient.GetEditorFormats(portalId)?.ToViewModel<EditorFormatListViewModel>();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            return modelList;
        }
        #endregion
    }
}
