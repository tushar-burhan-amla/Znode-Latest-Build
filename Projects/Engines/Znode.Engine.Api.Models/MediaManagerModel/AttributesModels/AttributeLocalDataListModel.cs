using System.Collections.Generic;
namespace Znode.Engine.Api.Models
{
    public class AttributeLocalDataListModel : BaseListModel
    {        
        public List<AttributeLocalDataModel> AttributeLocalList { get; set; }

        public AttributeLocalDataListModel()
        {
            AttributeLocalList = new List<AttributeLocalDataModel>();
        }
    }
}
