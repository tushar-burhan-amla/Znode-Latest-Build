using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    /// <summary>
    /// Model for list of Category History.
    /// </summary>
    public class CategoryHistoryListModel : BaseListModel
    {
        public List<CategoryHistoryModel> CategoryHistories  { get; set; }

        public CategoryHistoryListModel()
        {
            CategoryHistories = new List<CategoryHistoryModel>();
        }
    }
}
