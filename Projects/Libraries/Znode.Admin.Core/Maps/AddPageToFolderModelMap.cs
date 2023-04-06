using Znode.Engine.Api.Models;

namespace Znode.Admin.Core.Maps
{
    public static class AddPageToFolderModelMap
    {
        public static AddPageToFolderModel ToModel(int folderId, string pageIds)
        {
            AddPageToFolderModel model = new AddPageToFolderModel();
            model.FolderId = folderId;
            model.PageIds = pageIds;
            return model;
        }
    }
}
