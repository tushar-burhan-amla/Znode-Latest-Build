using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class RMAReturnNotesViewModel : BaseViewModel
    {
        public int RmaReturnNotesId { get; set; }
        public int RmaReturnDetailsId { get; set; }
        public string Notes { get; set; }
        public string UserEmailId { get; set; }
        public string ModifiedTime { get; set; }
    }
}
