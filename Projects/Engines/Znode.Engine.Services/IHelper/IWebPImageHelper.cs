using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Services.IHelper
{
    public  interface IWebPImageHelper
    {
        Bitmap Load(string pathFileName);

        Bitmap Decode(byte[] rawWebP);

        void GetImageParameter(Image sourceImage, out int newWidth, out int newHeight);

        Bitmap GetThumbnailQuality(byte[] rawWebP, int width, int height);
        
    }
}
