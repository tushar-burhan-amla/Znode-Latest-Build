using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IBarcodeHelper
    {
        /// <summary>
        /// This method is used to generate the barcode
        /// </summary>
        /// <param name="barcodeModel">BarcodeModel</param>
        /// <returns>return barcode string</returns>
        string GenerateBarcode(BarcodeModel barcodeModel);
    }
}
