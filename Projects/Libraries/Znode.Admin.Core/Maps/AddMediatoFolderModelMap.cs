using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public static class AddMediaToFolderModelMap
    {
        public static AddMediaToFolderModel ToModel(int folderId, string mediaIds)
        {
            AddMediaToFolderModel model = new AddMediaToFolderModel();
            model.FolderId = folderId;
            model.MediaIds = mediaIds;
            return model;
        }
    }
}