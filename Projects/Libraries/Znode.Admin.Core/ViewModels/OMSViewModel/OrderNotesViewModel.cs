namespace Znode.Engine.Admin.ViewModels
{
    public class OrderNotesViewModel : BaseViewModel
    {
        public int OmsNotesId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
    }
}