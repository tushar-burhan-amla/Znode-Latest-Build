using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class MediaManagerModelMap
    {
        public static string[] mediaFamilies = { "Audio", "Video", "File", "Image" };

        //Mapping MediaManagerListModel to MediaManagerListViewModel
        public static MediaManagerListViewModel ToListVieWModel(MediaManagerListModel models)
        {
            if (models?.MediaList?.Count > 0)
            {
                MediaManagerListViewModel listViewModel = new MediaManagerListViewModel();
                if (HelperUtility.IsNotNull(models))
                {
                    foreach (var data in models.MediaList)
                        listViewModel.MediaList.Add(ToViewModel(data));
                }

                listViewModel.Page = Convert.ToInt32(models.PageIndex);
                listViewModel.RecordPerPage = Convert.ToInt32(models.PageSize);
                listViewModel.TotalPages = Convert.ToInt32(models.TotalPages);
                listViewModel.TotalResults = Convert.ToInt32(models.TotalResults);

                return listViewModel;
            }
            return new MediaManagerListViewModel();
        }

        //Mapping MediaManagerListModel to MediaManagerViewModel
        public static MediaManagerViewModel ToViewModel(MediaManagerModel mediaManagerModel)
        {
            //Checks if mediaManagerModel is null
            if (HelperUtility.IsNull(mediaManagerModel))
                return new MediaManagerViewModel();

            return new MediaManagerViewModel()
            {
                MediaId = mediaManagerModel.MediaId,
                FileName = mediaManagerModel.FileName,
                ModifiedDate = mediaManagerModel.ModifiedDate.ToDateTimeFormat(),
                CreatedDate = mediaManagerModel.CreatedDate.ToDateTimeFormat(),
                Size = mediaManagerModel.Size.ToDisplayUnitFormat(),
                MediaType = mediaManagerModel.MediaType,
                Folder = mediaManagerModel.Folder,
                ShortDescription = mediaManagerModel.ShortDescription,
                Path = $"{(CheckMediaType(mediaManagerModel.FamilyCode) ? mediaManagerModel.MediaServerPath : mediaManagerModel.MediaServerThumbnailPath)}",
                OriginalImagePath = mediaManagerModel.MediaServerPath,
                FamilyCode = mediaManagerModel.FamilyCode,
                Length = mediaManagerModel.Length,
                DisplayName = mediaManagerModel.DisplayName,
                Width = mediaManagerModel.Width,
                Height = mediaManagerModel.Height,
            };
        }

        //Check Media Type.
        private static bool CheckMediaType(string familyCode)
        => mediaFamilies.Contains(familyCode, StringComparer.OrdinalIgnoreCase);

        //Map Media manager move folder properties.
        public static MediaManagerMoveFolderModel ToMoveFolderModel(int folderID, int addtoFolderId)
        {
            MediaManagerMoveFolderModel model = new MediaManagerMoveFolderModel();
            model.FolderId = folderID;
            model.ParentId = addtoFolderId;
            return model;
        }

        //Map media manager folder model properties.
        public static MediaManagerFolderModel ToFolderModel(int ParentId, string folderName)
        {
            MediaManagerFolderModel folderModel = new MediaManagerFolderModel();
            folderModel.Id = ParentId;
            folderModel.FolderName = folderName;
            return folderModel;
        }

        //Map MediaManagerTreeModel to MediaManagerTreeViewModel.
        public static MediaManagerTreeViewModel ToTreeViewModel(MediaManagerTreeModel model)
        {
            if (HelperUtility.IsNull(model))
                return new MediaManagerTreeViewModel();
            return new MediaManagerTreeViewModel
            {
                icon = model.Icon,
                id = model.Id,
                text = model.Text,
                children = model.Children.Select(x => ToTreeViewModel(x)).ToList()
            };
        }

        //Mapping ID of folder which is going to share and IDs of account with whom folder is going to share with list of ShareMediaFolderModel.
        public static List<ShareMediaFolderModel> ToShareMediaFolderListModel(int folderId, string accountIds)
        {
            List<ShareMediaFolderModel> listModel = new List<ShareMediaFolderModel>();

            if (!string.IsNullOrEmpty(accountIds))
            {
                foreach (string accountId in accountIds.Split(','))
                    listModel.Add(new ShareMediaFolderModel { ShareMediaFolderId = folderId, AccountId = Convert.ToInt32(accountId) });
            }
            return listModel;
        }
    }
}