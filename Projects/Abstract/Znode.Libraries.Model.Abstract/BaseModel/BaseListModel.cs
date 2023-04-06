namespace Znode.Libraries.Abstract.Models
{
	public abstract class BaseListModel : BaseModel
	{
		public int? PageIndex { get; set; }
		public int? PageSize { get; set; }
		public int? TotalResults { get; set; }

		public int? TotalPages
		{
			get
			{
				if (PageSize > 0)
				{
					if (TotalResults % PageSize == 0)
					{
						return TotalResults / PageSize;
					}
					else
					{
						return (TotalResults / PageSize) + 1;
					}
				}

				return null;
			}
		}
	}
}
