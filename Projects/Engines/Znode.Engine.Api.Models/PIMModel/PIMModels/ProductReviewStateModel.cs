namespace Znode.Engine.Api.Models
{
    public class ProductReviewStateModel : BaseModel
    {
        public int ReviewStateID { get; set; }
        public string ReviewStateName { get; set; }
        public string Description { get; set; }
    }
}
