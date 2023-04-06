using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.AttributeValidationHelpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Helpers;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin
{
    public class GlobalAttributeFamilyViewModelMap
    {

        public static List<LocaleDataModel> ToLocaleDataModel(LocaleListModel model, GlobalAttributeFamilyLocaleListModel values)
        {
            return (from _list in model.Locales
                    select new LocaleDataModel
                    {
                        LocaleId = _list.LocaleId,
                        Name = _list.Name,
                        Code = _list.Code,
                        IsActive = _list.IsActive,
                        IsDefault = _list.IsDefault,
                        Value = (IsNull(values) || IsNull(values.AttributeFamilyLocales)) ? null : IsNotNull(values.AttributeFamilyLocales?.FirstOrDefault(x => x.LocaleId == _list.LocaleId)) ? values.AttributeFamilyLocales.FirstOrDefault(x => x.LocaleId == _list.LocaleId).AttributeFamilyName : null
                    }).ToList();
        }


        //Maps bind data model to global attribute family model.
        public static GlobalAttributeFamilyUpdateModel ToModel(BindDataModel model)
        {
            GlobalAttributeFamilyUpdateModel globalAttributeFamilyModel = new GlobalAttributeFamilyUpdateModel();
            if (IsNotNull(model))
            {
                globalAttributeFamilyModel.FamilyCode = Convert.ToString(model.GetValue("FamilyCode"));
                globalAttributeFamilyModel.AttributeFamilyLocales = ToLocaleListModel(model).AttributeFamilyLocales;

            }
            return globalAttributeFamilyModel;
        }

        //Maps bind data model to global attribute family model.
        public static GlobalAttributeFamilyCreateModel ToCreateModel(BindDataModel model)
        {
            GlobalAttributeFamilyCreateModel globalAttributeFamilyModel = new GlobalAttributeFamilyCreateModel();
            if (IsNotNull(model))
            {
                globalAttributeFamilyModel.FamilyCode = Convert.ToString(model.GetValue("FamilyCode")); 
                globalAttributeFamilyModel.AttributeFamilyLocales = ToLocaleListModel(model).AttributeFamilyLocales;
                globalAttributeFamilyModel.GlobalEntityId = Convert.ToInt32(model.GetValue("GlobalEntityId"));

            }
            return globalAttributeFamilyModel;
        }

        //Maps bind data model to global attribute family locale list model.
        public static GlobalAttributeFamilyLocaleListModel ToLocaleListModel(BindDataModel model)
        {
            var values = model.GetValue("LocaleLabel");
            int GlobalAttributeFamilyId = Convert.ToInt32(model.GetValue("GlobalAttributeFamilyId"));
            var list = (Convert.ToString(values)?.Split(',').ToList());
            var FamilyCode = Convert.ToString(model.GetValue("FamilyCode"));

            GlobalAttributeFamilyLocaleListModel _locales = new GlobalAttributeFamilyLocaleListModel();
            if (list?.Count > 1)
            {
                _locales.AttributeFamilyLocales = new List<GlobalAttributeFamilyLocaleModel>();
                if (list.Count > 2)
                {
                    for (int i = 0; i < list.Count; i = i + 2)
                    {
                        _locales.AttributeFamilyLocales.Add(new GlobalAttributeFamilyLocaleModel()
                        {
                            LocaleId = Convert.ToInt32(list[i + 1]),
                            AttributeFamilyName = list[i],
                            GlobalAttributeFamilyId = GlobalAttributeFamilyId,
                        });
                        _locales.FamilyCode = FamilyCode;
                    }
                }
                else
                {
                    _locales.AttributeFamilyLocales.Add(new GlobalAttributeFamilyLocaleModel()
                    {
                        LocaleId = Convert.ToInt32(list[1]),
                        AttributeFamilyName = list[0],
                        GlobalAttributeFamilyId = GlobalAttributeFamilyId
                    });
                    _locales.FamilyCode = FamilyCode;
                }
            }

            return _locales;
        }

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
