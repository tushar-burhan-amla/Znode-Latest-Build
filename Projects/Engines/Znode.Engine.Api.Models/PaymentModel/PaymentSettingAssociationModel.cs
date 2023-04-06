namespace Znode.Engine.Api.Models
{
    public class PaymentSettingAssociationModel : BaseModel
    {
        public int PortalId { get; set; }
        public string PaymentSettingId { get; set; }
        public int ProfileId { get; set; }
        public bool IsPortalProfileAssociation { get; set; }
        public int DisplayOrder { get; set; }
        public int ProfilePaymentSettingId { get; set; }
        public string PublishState { get; set; }
        public bool IsUserForOfflinePayment { get; set; }
    }
}
