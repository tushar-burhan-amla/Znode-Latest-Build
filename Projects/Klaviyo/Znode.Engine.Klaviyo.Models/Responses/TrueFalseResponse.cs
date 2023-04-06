
using Znode.Libraries.Abstract.Models.Responses;

namespace Znode.Engine.klaviyo.Models.Responses
{
    //Boolean response for Post
    public class TrueFalseResponse : BaseResponse
    {
        public BooleanModel booleanModel { get; set; }

        public bool IsSuccess { get; set; }
    }
}
