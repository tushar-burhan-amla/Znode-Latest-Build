using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContentPageTreeViewModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public List<ContentPageTreeViewModel> children { get; set; }
        public State state { get; set; }
        public Dictionary<string, string> data { get; set; }
    }
}