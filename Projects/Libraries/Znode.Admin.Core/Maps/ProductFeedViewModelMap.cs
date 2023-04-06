using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Maps
{
    public class ProductFeedViewModelMap
    {
        public static ProductFeedModel ToModel(ProductFeedViewModel model)
        {
            ProductFeedModel productFeedModel = model.ToModel<ProductFeedModel>();
            productFeedModel.StoreList = new PortalListModel();
            productFeedModel.StoreList.PortalList = model.StoreList?.StoreList?.ToModel<PortalModel>().ToList();
            return productFeedModel;
        }

        // This method returns the List of frequency
        public static List<SelectListItem> ToFrequencyListItems()
        {
            List<SelectListItem> frequencyItem = new List<SelectListItem>();
            List<Frequency> frequency = Enum.GetValues(typeof(Frequency)).Cast<Frequency>().ToList();

            frequency.ForEach(x =>
            {
                frequencyItem.Add(new SelectListItem()
                {
                    Text = x.ToString(),
                    Value = x.ToString()
                });
            });
            return frequencyItem;
        }


      

        //This method returns the List of format in which last modified date is required
        public static List<SelectListItem> ToXMLSiteMapListItems(List<ProductFeedTypeModel> productFeedTypeList)
        {
            List<SelectListItem> productFeedTypeItem = new List<SelectListItem>();
            string value = string.Empty;
            productFeedTypeList.ForEach(x =>
            {
                productFeedTypeItem.Add(new SelectListItem()
                {
                    Text = x.ProductFeedTypeName.ToString(),
                    Value = x.ProductFeedTypeCode.ToString()
                });
            });

            return productFeedTypeItem;
        }

        // This method returns the List of XML Site Map Type
        public static List<SelectListItem> ToXMLSiteMapTypeListItems(List<ProductFeedSiteMapTypeModel> productFeedSiteMapTypeList)
        {
            List<SelectListItem> productFeedTypeItem = new List<SelectListItem>();
            productFeedSiteMapTypeList.ForEach(x =>
            {
                productFeedTypeItem.Add(new SelectListItem()
                {
                    Text = x.ProductFeedSiteMapTypeName.ToString(),
                    Value = x.ProductFeedSiteMapTypeCode.ToString()
                });
            });

            return productFeedTypeItem;
        }


    }
}