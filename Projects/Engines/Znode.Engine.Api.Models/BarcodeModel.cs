using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace Znode.Engine.Api.Models
{
    public class BarcodeModel : BaseModel
    {
        [Required]
        public string BarcodeText { get; set; }
        [Required]
        public string FontName { get; set; }
        [Required]
        public int FontSize { get; set; }
        [Required]
        public int Length { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public float PointX { get; set; }
        [Required]
        public float PointY { get; set; }
        [Required]
        public Color BarcodeLineColor { get; set; }
        [Required]
        public Color BarcodeBackgroundColor { get; set; }
        [Required]
        public ImageFormat BarcodeImageFormat { get; set; }
    }
}
