using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class AttributeGroupMapperListViewModel : BaseViewModel
    {
        public List<AttributeGroupMapperViewModel> AttributeGroupMappers { get; set; }

        public GridModel GridModel { get; set; }

        public int MediaAttributeGroupId { get; set; }
        public AttributeGroupMapperListViewModel()
        {
            AttributeGroupMappers = new List<AttributeGroupMapperViewModel>();
        }
    }
}