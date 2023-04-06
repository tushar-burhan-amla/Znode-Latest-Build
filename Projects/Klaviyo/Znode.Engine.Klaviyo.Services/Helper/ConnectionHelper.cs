using klaviyo.net;

using Znode.Engine.klaviyo.Models;
using Znode.Engine.Klaviyo.Services;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.Klaviyo
{
    public class ConnectionHelper : IConnectionHelper
    {
        #region Private Variables
        private readonly IZnodeklaviyoService znodeklaviyoService;
        #endregion

        #region Constructor
        public ConnectionHelper()
        {
            znodeklaviyoService = GetService<IZnodeklaviyoService>();
        }
        #endregion

        #region Public Methods
        //Get Klaviyo Identify details for the Portal.
        public bool GetKlaviyoIdentify(IdentifyModel userModel, string publickey)
        {
            SubmitStatus submitStatus = znodeklaviyoService.KlaviyoIdentify(userModel, publickey);
            return submitStatus == SubmitStatus.Success;
        }

        //Get Klaviyo Track details for the Portal
        public bool GetKlaviyoTrack(OrderDetailsModel klaviyoProductDetailModel, string publickey)
        {
            SubmitStatus submitStatus = znodeklaviyoService.KlaviyoOrder(klaviyoProductDetailModel, publickey);
            return submitStatus == SubmitStatus.Success;
        }
        #endregion
     }
}
