using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class MediaManagerTreeViewModel 
    {
        public int id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public List<MediaManagerTreeViewModel> children { get; set; }
        public State state { get; set; }
    }

    public class State
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }
}