using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class BoostAndBuryAutocompleteResponse : BaseResponse
    {
        public List<string> AutoCompleteList { get; set; }
    }
}
