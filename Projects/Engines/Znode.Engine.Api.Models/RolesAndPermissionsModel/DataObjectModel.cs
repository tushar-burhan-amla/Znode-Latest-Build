namespace Znode.Engine.Api.Models
{
    //Data Object Model
    public class DataObjectModel : BaseModel
    {
        public int MenuId { get; set; }
        public int[] Rights { get; set; }
    }
}
