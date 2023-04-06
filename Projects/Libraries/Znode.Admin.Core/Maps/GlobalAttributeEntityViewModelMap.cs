using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public static class GlobalAttributeEntityViewModelMap
    {
        public static List<BaseDropDownList> ToBaseDropDownList(GlobalAttributeGroupListModel attributeGroupList)
        {
            List<BaseDropDownList> unassignedAttributeGroups = new List<BaseDropDownList>();
            if (attributeGroupList?.AttributeGroupList?.Count > 0)
            {
                unassignedAttributeGroups = attributeGroupList.AttributeGroupList.Select(x => new BaseDropDownList
                {
                    id = x.GlobalAttributeGroupId.ToString(),
                    name = x.AttributeGroupName,
                }).ToList();
            }
            return unassignedAttributeGroups;
        }

    }
}
