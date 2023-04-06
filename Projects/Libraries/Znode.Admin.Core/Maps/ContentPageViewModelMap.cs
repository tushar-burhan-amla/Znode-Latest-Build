using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Maps
{
    public class ContentPageViewModelMap
    {
        //Bind CMS Template List
        public static List<SelectListItem> ToListItems(IEnumerable<CMSContentPageTemplateModel> model, int cmsStaticPageTemplateId)
        {
            List<SelectListItem> lstTemplate = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                lstTemplate = (from item in model
                            orderby item.Name ascending
                            select new SelectListItem
                            {
                                Text = item.Name,
                                Value = item.CMSTemplateId.ToString(),
                                Selected = item.CMSTemplateId == cmsStaticPageTemplateId ? true : false
                            }).ToList();
            }
            return lstTemplate;
        }

        // Bind Portal Profile list.
        public static List<SelectListItem> ToProfileListItems(IEnumerable<PortalProfileModel> model)
        {
            List<SelectListItem> lstProfile = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                lstProfile = (from item in model
                              orderby item.ProfileName ascending
                            select new SelectListItem
                            {
                                Text = item.ProfileName,
                                Value = item.ProfileId.ToString(),
                            }).ToList();
            }
            return lstProfile;
        }

        // Bind Template list.
        public static List<SelectListItem> ToTemplateListItems(IEnumerable<TemplateModel> model)
        {
            List<SelectListItem> lstTemplate = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                lstTemplate = (from item in model
                            orderby item.Name ascending
                            select new SelectListItem
                            {
                                Text = item.Name,
                                Value = item.CMSTemplateId.ToString(),
                            }).ToList();
            }
            return lstTemplate;
        }

        // Bind Template list With Image.
        public static List<TemplateViewModel> ToTemplateListItemsWithImage(IEnumerable<TemplateModel> model)
        {
            List<TemplateViewModel> lstTemplate = new List<TemplateViewModel>();
            if (IsNotNull(model))
            {
                lstTemplate = (from item in model
                               orderby item.Name ascending
                               select new TemplateViewModel
                               {
                                   Name = item.Name,
                                   CMSTemplateId = item.CMSTemplateId,
                                   MediaPath =string.IsNullOrEmpty(item.MediaThumbNailPath)? ZnodeAdminSettings.DefaultImagePath: item.MediaThumbNailPath,
                               }).ToList();
            }
            return lstTemplate;
        }


        // Bind Portal list.
        public static List<SelectListItem> ToListItems(IEnumerable<PortalModel> model, int portalId = 0)
        {
            List<SelectListItem> portals = new List<SelectListItem>();
            if (IsNotNull(model))
            {
                portals = (from item in model
                           orderby item.StoreName ascending
                            select new SelectListItem
                            {
                                Text = item.StoreName,
                                Value = item.PortalId.ToString(),
                                Selected = item.PortalId == portalId ? true : false
                            }).ToList();
            }
            return portals;
        }

        //Mapping ID which is going to share and IDs of publish product .
        public static CMSWidgetProductListModel ToAssociateProductListModel(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string SKUs)
        {
            CMSWidgetProductListModel listModel = new CMSWidgetProductListModel { CMSWidgetProducts = new List<CMSWidgetProductModel>() };
            if (!string.IsNullOrEmpty(SKUs))
            {
                foreach (string sku in SKUs.Split(','))
                    listModel.CMSWidgetProducts.Add(new CMSWidgetProductModel { CMSWidgetsId = cmsWidgetsId, CMSMappingId = cmsMappingId, WidgetsKey = widgetKey, TypeOfMapping = typeOfMapping, SKU = sku });
                return listModel;
            }
            return listModel;
        }
    }
}