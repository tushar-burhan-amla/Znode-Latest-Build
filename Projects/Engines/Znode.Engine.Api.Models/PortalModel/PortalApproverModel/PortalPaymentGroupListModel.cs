using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalPaymentGroupListModel : BaseListModel
    {
        public List<PortalPaymentGroupListModel> PortalPaymentGroups { get; set; }

        public PortalPaymentGroupListModel()
        {
            PortalPaymentGroups = new List<PortalPaymentGroupListModel>();
        }
    }
}
