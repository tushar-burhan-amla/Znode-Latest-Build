using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Engine.Api.Models;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using Newtonsoft.Json;

namespace Znode.Engine.Services
{
    public class DynamicContentService : BaseService, IDynamicContentService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalCustomCss> _portalCustomCssRepository;
        #endregion

        #region Constructor
        public DynamicContentService()
        {
            _portalCustomCssRepository = new ZnodeRepository<ZnodePortalCustomCss>();           
        }
        #endregion

        #region Public Methods
        public EditorFormatListModel GetEditorFormats(int portalId)
        {
            ZnodeLogging.LogMessage("GetDynamicStyles method execution started.", ZnodeLogging.Components.API.ToString(), TraceLevel.Info);

            string formats = _portalCustomCssRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.WYSIWYGFormatStyle;

            EditorFormatListModel model = null;

            if(!string.IsNullOrEmpty(formats))
                model = JsonConvert.DeserializeObject<EditorFormatListModel>(formats);

            return model;
        }
        #endregion
    }
}
