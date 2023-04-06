using System.Collections.Generic;
namespace Znode.Engine.Api.Models.Responses
{
    public class MediaTypeResponse : BaseResponse
    {
        public IEnumerable<MediaTypeModel> Types { get; set; }
    }
}
