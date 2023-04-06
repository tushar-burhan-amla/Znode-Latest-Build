using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class MediaAttributeValuesListModel : BaseListModel  
    {
        public List<MediaAttributeValuesModel> MediateAttributeValues { get; set; }
        public string MediaVirtualPath { get; set; }
        public MediaManagerModel Media { get; set; }

        public MediaAttributeValuesListModel()
        {
            MediateAttributeValues = new List<MediaAttributeValuesModel>();
        }
    }
}
