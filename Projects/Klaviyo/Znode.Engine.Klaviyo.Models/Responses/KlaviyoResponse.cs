using System.Collections.Generic;


using Znode.Libraries.Abstract.Models.Responses;

namespace Znode.Engine.klaviyo.Models.Responses
{
    public class KlaviyoResponse : BaseResponse
    {
        public KlaviyoModel Klaviyo { get; set; }
        public List<EmailProviderModel> EmailProviderModelList { get; set; }
    }
}


