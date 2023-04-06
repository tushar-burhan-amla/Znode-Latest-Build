namespace Znode.Engine.Admin.ViewModels
{
    public class PaymentGatewayViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public int PaymentGatewayId { get; set; }
        public string Url { get; set; }
    }
}