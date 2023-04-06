using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class BarcodeHelper : IBarcodeHelper
    {
        public virtual string GenerateBarcode(BarcodeModel barcodeModel)
        {
            if (!string.IsNullOrEmpty(barcodeModel.BarcodeText))
            {
                string fontFile = HttpContext.Current.Server.MapPath("/Fonts/" + barcodeModel.FontName + ".ttf");
                if (File.Exists(fontFile))
                {
                    try
                    {
                        FontFamily fontFamily = LoadFontFamily(fontFile);
                        if (!string.Equals(fontFamily, null))
                        {
                            using (Bitmap bitMap = new Bitmap(barcodeModel.BarcodeText.Length * barcodeModel.Length, barcodeModel.Height))
                            {
                                using (Graphics graphics = Graphics.FromImage(bitMap))
                                {
                                    Font oFont = new Font(fontFamily, barcodeModel.FontSize);
                                    PointF point = new PointF(barcodeModel.PointX, barcodeModel.PointY);
                                    SolidBrush barcodeLineColor = new SolidBrush(barcodeModel.BarcodeLineColor);
                                    SolidBrush barcodeBackgroundColor = new SolidBrush(barcodeModel.BarcodeBackgroundColor);
                                    graphics.FillRectangle(barcodeBackgroundColor, 0, 0, bitMap.Width, bitMap.Height);
                                    graphics.DrawString("*" + barcodeModel.BarcodeText + "*", oFont, barcodeLineColor, point);
                                }
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    bitMap.Save(ms, barcodeModel.BarcodeImageFormat);
                                    byte[] byteImage = ms.ToArray();
                                    return "data:image/"+ barcodeModel.BarcodeImageFormat + ";base64," + Convert.ToBase64String(byteImage);
                                }
                            }
                        }
                        else
                        {
                            ZnodeLogging.LogMessage("Font family not load for barcode.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                            return string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage("Generate barcode failed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, ex);
                        return string.Empty;
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage("Font file not found for barcode.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                    return string.Empty;
                }
            }
            else
            {
                ZnodeLogging.LogMessage("Barcode text not getting.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return string.Empty;
            }
        }



        protected virtual FontFamily LoadFontFamily(string fileName)
        {
            PrivateFontCollection fontCollection = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                //Assign memory space to fontCollection
                fontCollection = new PrivateFontCollection();
                //Add the full path of the ttf file
                fontCollection.AddFontFile(fileName);
                //Returns the family object as usual.
                return fontCollection.Families[0];
            }
            else
            {
                return null;
            }
        }
    }
}
