using System;

namespace Znode.Engine.ABSConnector
{
    public class ABSOrderResponseTop5Model : ABSRequestBaseModel
    {
        public string SoldTo { get; set; }
        public string SummaryDate { get; set; }
        public string SummaryType { get; set; }
        public string SummaryUnits { get; set; }
        public string SummaryAmount { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CancelDate { get; set; }
        public string CustomerOrderNumber { get; set; }
        public int OrderStatus { get; set; }
        public string OrderSource { get; set; }
        public string InvoiceNumber1 { get; set; }
        public string InvoiceNumber2 { get; set; }
        public string InvoiceNumber3 { get; set; }
        public string InvoiceNumber4 { get; set; }
        public string InvoiceNumber5 { get; set; }
        public string TrackingNumber01 { get; set; }
        public string TrackingNumber02 { get; set; }
        public string TrackingNumber03 { get; set; }
        public string TrackingNumber04 { get; set; }
        public string TrackingNumber05 { get; set; }
        public string TrackingNumber06 { get; set; }
        public string TrackingNumber07 { get; set; }
        public string TrackingNumber08 { get; set; }
        public string TrackingNumber09 { get; set; }
        public string TrackingNumber10 { get; set; }
        public string TrackingNumber11 { get; set; }
        public string TrackingNumber12 { get; set; }
    }
}
