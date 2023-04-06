using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    /// <summary>
    /// Model for list of Catalog History.
    /// </summary>
    public class CatalogHistoryListModel : BaseListModel
    {
        public List<CatalogHistoryModel> CatalogHistories { get; set; }

        public CatalogHistoryListModel()
        {
            CatalogHistories = new List<CatalogHistoryModel>();
        }
    }
}
