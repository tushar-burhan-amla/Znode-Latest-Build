using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SupplierTypeListModel :BaseListModel
    {
        public SupplierTypeListModel()
        {
            SupplierTypes = new List<SupplierTypeModel>();
        }

        public List<SupplierTypeModel> SupplierTypes { get; set; }

    }
}
