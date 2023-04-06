using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class PIMCategoryViewModelMap
    {

        //Map BindDataModel to CategoryValuesListViewModel
        public static CategoryValuesListModel ToListModel(BindDataModel bindDataModel, CategoryValuesListModel categoryValuesListModel)
        {
            bindDataModel.ControlsData.ToList().ForEach(item =>
             {
                //Appended keys with property name {AttributeCode}[0]_{PimAttributeId}[1]_{PimAttributeDefaultValueId}[2]_{PimAttributeValueId}[3]_{PimAttributeFamilyId}[4].
                List<object> itemList = new List<object>();
                 itemList.AddRange(item.Key.Split('_'));
                 if (itemList.Count() >= 5)
                 {
                     categoryValuesListModel.AttributeValues.Add(new PIMCategoryValuesListModel
                     {
                         AttributeCode = itemList[0].ToString(),
                         PimAttributeId = Convert.ToInt32(itemList[1]),
                         PimAttributeDefaultValueId = Convert.ToInt32(itemList[2]),
                         PimAttributeValueId = Convert.ToInt32(itemList[3]),
                         PimAttributeFamilyId = Convert.ToInt32(itemList[4]),
                         AttributeValue = item.Value.ToString(),
                         LocaleId = categoryValuesListModel.LocaleId,
                         PimCategoryId = categoryValuesListModel.PimCategoryId,
                     });
                 }
             });
            return categoryValuesListModel;
        }
    }
}