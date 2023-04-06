namespace Znode.Engine.Admin.ViewModels
{
    public class ActionViewModel : BaseViewModel
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }        
    }
}