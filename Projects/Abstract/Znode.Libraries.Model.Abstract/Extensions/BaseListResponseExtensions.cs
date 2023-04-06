using Znode.Libraries.Abstract.Models.Responses;

namespace Znode.Libraries.Abstract.Models.Extensions
{
    public static class BaseListResponseExtensions
	{
		public static void MapPagingDataFromModel(this BaseListResponse listResponse, BaseListModel listModel)
		{
			listResponse.PageIndex = listModel.PageIndex;
			listResponse.PageSize = listModel.PageSize;
			listResponse.TotalPages = listModel.TotalPages;
			listResponse.TotalResults = listModel.TotalResults;
		}
	}
}
