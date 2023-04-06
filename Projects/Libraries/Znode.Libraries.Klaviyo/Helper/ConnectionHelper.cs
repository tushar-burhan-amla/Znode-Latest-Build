using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Engine.Api.Models;
using klaviyo.net;

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
        public bool GetKlaviyoIdentify(UserModel userModel, string publickey)
        {
            SubmitStatus submitStatus = znodeklaviyoService.KlaviyoIdentify(KlaviyoMapper.ToIdentifyModel(userModel), publickey);
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
