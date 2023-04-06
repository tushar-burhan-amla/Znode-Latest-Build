namespace Znode.Engine.Admin.ViewModels
{
    public class PaymentTypeViewModel : BaseViewModel
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public int PaymentTypeId { get; set; }
    }
}  