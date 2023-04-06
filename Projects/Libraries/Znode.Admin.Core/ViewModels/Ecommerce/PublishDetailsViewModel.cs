using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{

    public class PublishDetailsViewModel : BaseViewModel
    {
        public PublishDetailsViewModel()
        {
            Attributes = new List<Property>();
        }
        public List<Property> Attributes { get; set; }
    }
}