namespace Znode.Engine.Api.Models
{
    public class PortalUnitModel : BaseModel
    {
        public int PortalUnitId { get; set; }
        public int? PortalId { get; set; }
        public int? CurrencyTypeID { get; set; }
        public int CurrencyId { get; set; }
        public int OldCurrencyId { get; set; }
        public string WeightUnit { get; set; }
        public string DimensionUnit { get; set; }
        public string CurrencySuffix { get; set; }
        public string PortalName { get; set; }
        public int? CultureId { get; set; }
        public int OldCultureId { get; set; }
    }
}
