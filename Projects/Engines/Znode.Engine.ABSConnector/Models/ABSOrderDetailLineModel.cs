namespace Znode.Engine.ABSConnector
{
    public class ABSOrderDetailLineModel : ABSRequestBaseModel
    {
        public string OrderNumber { get; set; }
        public string LineSeason { get; set; }
        public string LineSeasonYear { get; set; }
        public string LineStyleNumber { get; set; }
        public string LineStyleDescription { get; set; }
        public string LineColorCode { get; set; }
        public string LineColorDescription { get; set; }
        public string LinePieceCode { get; set; }
        public string LinePieceDescription { get; set; }
        public string LineDimensionCode { get; set; }
        public string LineDimensionDescription { get; set; }
        public string LineSizeScale { get; set; }
        public string LineUpcCode { get; set; }
        public string LinePrice { get; set; }
        public string LineItemStatus { get; set; }
        public string LineSize { get; set; }
        public string LineQuantity { get; set; }
    }
}
