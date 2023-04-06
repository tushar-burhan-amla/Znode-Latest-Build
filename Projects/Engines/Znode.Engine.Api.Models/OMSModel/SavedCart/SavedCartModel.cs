namespace Znode.Engine.Api.Models
{
    public class SavedCartModel : BaseModel
    {
        public int OmsSavedCartId { get; set; }
        public int OmsCookieMappingId { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? RecurringSalesTax { get; set; }
    }
}
