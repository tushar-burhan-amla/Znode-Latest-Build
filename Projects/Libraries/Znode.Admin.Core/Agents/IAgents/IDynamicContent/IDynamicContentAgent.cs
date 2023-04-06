using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IDynamicContentAgent
    {
        /// <summary>
        /// To get WYSISYG editor formats.
        /// </summary>
        /// <param name="portalId">To specify portal Id</param>
        /// <returns>List of formats for WYSIWYG editor</returns>
        EditorFormatListViewModel GetEditorFormats(int portalId);
    }
}
