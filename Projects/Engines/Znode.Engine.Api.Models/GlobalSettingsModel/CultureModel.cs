namespace Znode.Engine.Api.Models
{
    public class CultureModel : BaseModel
    {
        public int CultureId { get; set; }
        public string CultureCode { get; set; }
        public string CultureName { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string Symbol { get; set; }
        public int CurrencyId { get; set; }        
    }
}
