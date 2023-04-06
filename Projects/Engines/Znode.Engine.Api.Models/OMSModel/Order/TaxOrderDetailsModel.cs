using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TaxOrderDetailsModel : BaseModel
    {
        public int OmsTaxOrderDetailsId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public decimal SalesTax { get; set; }
        public decimal VAT { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }
        public decimal HST { get; set; }
        public decimal ImportDuty { get; set; }

        public List<TaxOrderLineDetailsModel> TaxOrderLineDetails { get; set; }
    }
}
