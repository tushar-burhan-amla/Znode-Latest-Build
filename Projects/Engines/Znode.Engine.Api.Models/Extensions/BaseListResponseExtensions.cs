using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Models.Extensions
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
