using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class SearchProfileMap
    {
        public static List<SearchProfileModel> ToModel(List<ZnodePublishCatalogSearchProfile> entityList)
        {
            if (entityList?.Count > 0)
            {
                IEnumerable<SearchProfileModel> searchProfileList = entityList.Select(
                    model => new SearchProfileModel()
                    {
                        SearchProfileId = model.SearchProfileId,
                        PublishCatalogId = model.PublishCatalogId,
                        ProfileName = model.ZnodeSearchProfile.ProfileName,
                    });
                return searchProfileList.ToList();
            }
            else
                return null;
        }
    }
}
