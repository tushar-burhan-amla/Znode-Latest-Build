using Znode.Engine.Api.Models;


namespace Znode.Engine.WebStore.Agents
{
    public interface IMediaManagerAgent
    {
        /// <summary>
        ///  Get MediaDetail Model is a light weighted model which returns basic properties
        /// </summary>
        /// <param name="mediaId">MediaId</param>
        /// <returns>MediaDetailModel</returns>
        MediaDetailModel GetMediaDetailsById(int mediaId);

        /// <summary>
        /// Get MediaDetail Model is a light weighted model which returns basic properties
        /// </summary>
        /// <param name="mediaGuid">MediaGuid</param>
        /// <returns>MediaDetailModel</returns>
        MediaDetailModel GetMediaDetailsByGuid(string mediaGuid);

    }
}
