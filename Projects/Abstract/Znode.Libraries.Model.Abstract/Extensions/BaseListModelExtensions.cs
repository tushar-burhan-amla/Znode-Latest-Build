using Znode.Libraries.Abstract.Models.Responses;

namespace Znode.Libraries.Abstract.Models.Extensions
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
