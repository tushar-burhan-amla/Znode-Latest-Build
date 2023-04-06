using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeGroupMapperListViewModel : BaseViewModel
    {
        public GridModel GridModel { get; set; }
        public List<GlobalAttributeGroupMapperViewModel> AttributeGroupMappers { get; set; }

        public int GlobalAttributeGroupId { get; set; }
    }
}