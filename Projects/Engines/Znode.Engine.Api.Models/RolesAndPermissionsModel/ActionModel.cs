namespace Znode.Engine.Api.Models
{
    public class ActionModel : BaseModel
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
    }
}
