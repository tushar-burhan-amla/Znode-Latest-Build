using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class NoteListModel : BaseListModel
    {
        public List<NoteModel> Notes { get; set; }
        public string CustomerName { get; set; }
        public string AccountName { get; set; }
    }
}
