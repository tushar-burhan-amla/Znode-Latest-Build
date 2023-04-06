using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IDynamicContentClient : IBaseClient
    {
        /// <summary>
        /// To get WYSISYG editor formats.
        /// </summary>
        /// <param name="portalId">To specify portal Id</param>
        /// <returns>List of formats for WYSIWYG editor</returns>
        EditorFormatListModel GetEditorFormats(int portalId);
    }
}
