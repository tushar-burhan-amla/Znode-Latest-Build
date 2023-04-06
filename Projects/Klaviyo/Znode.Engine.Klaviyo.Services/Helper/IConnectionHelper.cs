using Znode.Engine.klaviyo.Models;

namespace Znode.Libraries.Klaviyo
{
    public interface IConnectionHelper
    {
        //Get Klaviyo Identify Details
        bool GetKlaviyoIdentify(IdentifyModel userModel, string publickey);

        //Get Klaviyo Track Details
        bool GetKlaviyoTrack(OrderDetailsModel klaviyoProductDetailModel, string publickey);

    }
}
