using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class ThemeViewModelMap
    {
        //Convert CMSAreaListModel to list of select list items.
        public static List<SelectListItem> ToSelectListItemsForCMSArea(IEnumerable<CMSAreaModel> cmsAreaModel)
        {
            List<SelectListItem> cmsAreas = new List<SelectListItem>();
            if (!Equals(cmsAreaModel, null))
            {
                cmsAreas = (from item in cmsAreaModel
                            select new SelectListItem
                            {
                                Text = item.AreaName,
                                Value = item.CMSAreaId.ToString(),
                            }).ToList();
            }
            return cmsAreas;
        }

        //Mapping ID of ThemeList which is going to share and IDs of widgets with whom theme list is going to share with list of ThemeWidgetsModel.
        public static CMSThemeWidgetsListModel ToAssociateThemeWidgetsListModel(CMSDataViewModel dataModel)
        {
            string[] widgetIds = dataModel?.CMSAreaWidgetsData?.WidgetIds;
            if (widgetIds.Length > 0)
            {
                CMSThemeWidgetsListModel listModel = new CMSThemeWidgetsListModel { CMSThemeWidgets = new List<CMSThemeWidgetsModel>() };
                foreach (string widgetId in widgetIds)
                    listModel.CMSThemeWidgets.Add(new CMSThemeWidgetsModel { CMSThemeId = dataModel.CMSThemeId, CMSWidgetsId = Convert.ToInt32(widgetId) });

                listModel.CMSAreaId = dataModel.CMSAreaId.Value;
                return listModel;
            }
            return new CMSThemeWidgetsListModel();

        }

        //Map Portal to Drop Down List.
        public static List<SelectListItem> ToPortalList(List<PortalModel> portalList)
        {
            List<SelectListItem> lstPortal = new List<SelectListItem>();
            if (!Equals(portalList, null))
            {
                lstPortal = (from item in portalList
                            orderby item.StoreName ascending
                            select new SelectListItem
                            {
                                Text = item.StoreName,
                                Value = item.PortalId.ToString(),
                            }).ToList();
            }
            return lstPortal;
        }
        //Converts Theme List to Select Item List to select from dropdown.
        public static List<SelectListItem> ToSelectListItemForThemes(List<ThemeViewModel> themeList)
        {
            List<SelectListItem> lstPortal = new List<SelectListItem>();
            if (!Equals(themeList, null))
            {
                lstPortal = (from item in themeList
                             orderby item.Name ascending
                             select new SelectListItem
                             {
                                 Text = !item.IsParentTheme ? string.Format("{0} ({1})", item.Name, item.ParentThemeName) : item.Name,
                                 Value = item.CMSThemeId.ToString(),
                             }).ToList();
            }
            return lstPortal;
        }
    }
}