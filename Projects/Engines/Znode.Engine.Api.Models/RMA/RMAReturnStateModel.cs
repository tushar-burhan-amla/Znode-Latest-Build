namespace Znode.Engine.Api.Models
{
    public class RMAReturnStateModel : BaseModel
    {
        public int RmaReturnStateId { get; set; }
        public string ReturnStateName { get; set; }
        public string Description { get; set; }
        public bool? IsReturnState { get; set; }
        public bool? IsReturnLineItemState { get; set; }
        public int DisplayOrder { get; set; }
    }
}
