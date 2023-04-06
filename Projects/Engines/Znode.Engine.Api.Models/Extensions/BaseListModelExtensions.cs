using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Models.Extensions
{
    public static class BaseListModelExtensions
    {
        public static void MapPagingDataFromResponse(this BaseListModel listModel, BaseListResponse listResponse)
        {
            // We don't set TotalPages because it only has a getter, which handles the calculation
            if (!Equals(listResponse, null))
            {
                listModel.PageIndex = listResponse.PageIndex;
                listModel.PageSize = listResponse.PageSize;
                listModel.TotalResults = listResponse.TotalResults;
            }
        }
    }
}
