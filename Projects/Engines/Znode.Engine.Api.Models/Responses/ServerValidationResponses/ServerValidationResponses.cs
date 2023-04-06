using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ServerValidationResponses : BaseResponse
    {
        public ValidateServerModel Validate { get; set; }

        public Dictionary<string, string> ErrorDictionary { get; set; }
    }
}
