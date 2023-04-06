using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.Agents
{
    public class MediaManagerAgent : BaseAgent, IMediaManagerAgent
    {
        #region Private Variables
        private IMediaManagerClient _mediaManagerClient;
        #endregion


        #region Constructor
        public MediaManagerAgent(IMediaManagerClient mediaManagerClient)
        {
            _mediaManagerClient = GetClient<IMediaManagerClient>(mediaManagerClient);

        }
        #endregion

        #region Public Methods
   
        //Get Media Details By Id
        public virtual MediaDetailModel GetMediaDetailsById(int mediaId)
            => _mediaManagerClient.GetMediaDetailsById(mediaId);

        // Get Media Details by Guid
        public virtual MediaDetailModel GetMediaDetailsByGuid(string mediaGuid)
            => _mediaManagerClient.GetMediaDetailsByGuid(mediaGuid);
        #endregion
    }
}