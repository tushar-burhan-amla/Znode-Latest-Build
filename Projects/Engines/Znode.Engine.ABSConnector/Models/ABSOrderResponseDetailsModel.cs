namespace Znode.Engine.ABSConnector
{
    public class ABSOrderResponseDetailsModel : ABSRequestBaseModel
    {
        public ABSSoldToInformationModel SoldToInformation { get; set; }
        public ABSShipToInformationModel ShipToInformation { get; set; }
        public ABSBillToChangeRequestModel BillToInformation { get; set; }
        public ABSARPaymentRequestModel PaymentInformation { get; set; }
        public ABSTrackingInformationModel TrackingInformation { get; set; }
        public ABSOrderHeaderModel OrderHeader { get; set; }
    }
}
