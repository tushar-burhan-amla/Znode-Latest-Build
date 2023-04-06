using System.Collections.Generic;

namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentTypeModel : BaseModel
    {
        public int PaymentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string BehaviorType { get; set; }
        public bool IsActive { get; set; }
    }

    public class PaymentTypeListModel
    {      
        public List<PaymentTypeModel> PaymentTypes { get; set; }
    }
}
