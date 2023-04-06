using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class NoteListResponse : BaseListResponse
    {
        public List<NoteModel> Notes { get; set; }
        public string CustomerName { get; set; }
    }
}
