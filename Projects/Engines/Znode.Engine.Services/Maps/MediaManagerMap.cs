using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
namespace Znode.Engine.Services.Maps
{
    public static class MediaManagerMap
    {
        public static ZnodeMedia ToEntity(MediaManagerModel mediaManagerModel)
        {
            if (HelperUtility.IsNotNull(mediaManagerModel))
            {
                //Assign values of model to database entity
                ZnodeMediaCategory category = new ZnodeMediaCategory();
                category.MediaPathId = mediaManagerModel.MediaPathId;
                category.MediaAttributeFamilyId = mediaManagerModel.AttributeFamilyId;

                ZnodeMedia Model = new ZnodeMedia();
                Model.Path = mediaManagerModel.Path;
                Model.Size = mediaManagerModel.Size;
                Model.Height = mediaManagerModel.Height;
                Model.Width = mediaManagerModel.Width;
                Model.MediaConfigurationId = mediaManagerModel.MediaConfigurationId;
                Model.Type = mediaManagerModel.MediaType;
                Model.FileName = mediaManagerModel.FileName;
                Model.Length = mediaManagerModel.Length;
				Model.Version = mediaManagerModel.Version;
                Model.ZnodeMediaCategories.Add(category);

                return Model;
            }
            else
                return null;
        }

        //Set media entity.
        public static ZnodeMedia SetMediaEntity(MediaManagerModel mediaManagerModel, ZnodeMedia entity)
        {
            entity.Size = mediaManagerModel.Size;
            entity.Height = mediaManagerModel.Height;
            entity.Width = mediaManagerModel.Width;
            entity.Path = mediaManagerModel.Path;
            entity.FileName = mediaManagerModel.FileName;
			entity.Version = mediaManagerModel.Version;
            entity.ModifiedDate = HelperUtility.GetDateTime();
            return entity;
        }

        //Maps MediaManagerFolderModel to ZnodeMediaPath.
        public static ZnodeMediaPath ToModel(MediaManagerFolderModel model)
        {
            //created new instance of ZnodeMediaPathLocale
            ZnodeMediaPathLocale pathLocale = new ZnodeMediaPathLocale();

            //assing all property to model
            pathLocale.LocaleId = 1;
            pathLocale.PathName = model.FolderName;

            //created new instance of ZnodeMediaPath
            ZnodeMediaPath newMediaFolder = new ZnodeMediaPath();

            //assing all property to model
            newMediaFolder.ParentMediaPathId = model.Id;
            newMediaFolder.PathCode = MediaManager_Resources.MediaPathCode;

            //add ZnodeMediaPathLocales to ZnodeMediaPath collection.
            newMediaFolder.ZnodeMediaPathLocales.Add(pathLocale);
            return newMediaFolder;
        }

        //Mapping ShareMediaFolderModel to ZnodeMediaUsers entity
        public static ZnodeMediaFolderUser ToShareMediaFolderEntity(ShareMediaFolderModel shareMediaModel)
        {
            if (HelperUtility.IsNotNull(shareMediaModel))
            {
                return new ZnodeMediaFolderUser
                {
                    UserId = shareMediaModel.AccountId,
                    MediaPathId = shareMediaModel.ShareMediaFolderId
                };
            }
            else
                return null;
        }

        //Mapping list of ShareMediaFolderModel to list of ZnodeMediaUsers entity
        public static List<ZnodeMediaFolderUser> ToShareMediaFolderListEntity(List<ShareMediaFolderModel> shareMediaFolderListModel)
        {
            List<ZnodeMediaFolderUser> mediaFolderUserList = new List<ZnodeMediaFolderUser>();
            if (shareMediaFolderListModel?.Count > 0)
            {
                foreach (var shareMediaFolder in shareMediaFolderListModel)
                    mediaFolderUserList.Add(ToShareMediaFolderEntity(shareMediaFolder));
            }
            return mediaFolderUserList;
        }
    }
}
