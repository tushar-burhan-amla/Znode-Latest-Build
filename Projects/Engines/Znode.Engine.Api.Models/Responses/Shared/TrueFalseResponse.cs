namespace Znode.Engine.Api.Models.Responses
{
    //Boolean response for Post
    public class TrueFalseResponse : BaseResponse
    {
        public BooleanModel booleanModel { get; set; }

        public bool IsSuccess { get; set; }
    }
}
