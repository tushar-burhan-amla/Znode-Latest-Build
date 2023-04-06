namespace Znode.Engine.Api.Models
{
    public class ECertTotalBalanceModel : BaseModel
    {
        public decimal AvailableTotal { get; set; }

        public string TraceMessage { get; set; }
    }
}
