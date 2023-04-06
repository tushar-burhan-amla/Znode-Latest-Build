namespace Znode.Engine.Api.Models.Responses
{
    public class RMAReturnOrderNumberListResponse : BaseListResponse
    {
        public RMAReturnOrderNumberListModel ReturnEligibleOrderNumberList { get; set; }
    }
}
