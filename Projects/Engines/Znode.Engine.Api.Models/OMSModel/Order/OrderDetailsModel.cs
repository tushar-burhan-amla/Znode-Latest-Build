using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class OrderDetailsModel : BaseModel
    {
        public string OmsOrderNumber { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLength)]
        public string ExternalId { get; set; }
        public string OrderStateName { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLength)]
        public string OrderNotes { get; set; }

        public int? OmsOrderStateId { get; set; }
    }
}
