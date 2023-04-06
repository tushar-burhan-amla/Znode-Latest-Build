using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SMSResponse : BaseResponse
    {
        public SMSModel Sms { get; set; }
        public List<SMSProviderModel> SmsProviderModelList { get; set; }
    }
}