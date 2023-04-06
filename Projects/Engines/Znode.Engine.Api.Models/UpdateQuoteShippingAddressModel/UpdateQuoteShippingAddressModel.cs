namespace Znode.Engine.Api.Models
{
    public class UpdateQuoteShippingAddressModel : BaseListModel
    {
        public int OmsQuoteId { get; set; }
        public AddressModel ShippingAddressModel { get; set; }
    }
}
