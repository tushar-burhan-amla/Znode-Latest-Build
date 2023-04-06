using System;
using System.Collections.Generic;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Maps
{
    public class CategoryViewModelMap
    {
        //Map categoryId and comma seperated productIds to List<CategoryProductModel> along with status.
        public static List<CategoryProductModel> ToAssociateStoreListModel(int categoryId, string productIds)
        {
            List<CategoryProductModel> categoryProductlistModel = new List<CategoryProductModel>();

            if (!string.IsNullOrEmpty(productIds))
            {
                foreach (string pimProductId in productIds.Split(','))
                    categoryProductlistModel.Add(new CategoryProductModel { PimCategoryId = categoryId, PimProductId = Convert.ToInt32(pimProductId), Status = true, DisplayOrder = AdminConstants.DefaultDisplayOrder });
            }
            return categoryProductlistModel;
        }

        public static List<CategoryProductModel> ToAssociateCategoriesToProductListModel(int productId, string categoryIds)
        {
            List<CategoryProductModel> categoryProductlistModel = new List<CategoryProductModel>();

            if (!string.IsNullOrEmpty(categoryIds))
            {
                foreach (string pimCategoryId in categoryIds.Split(','))
                    categoryProductlistModel.Add(new CategoryProductModel { PimProductId = productId, PimCategoryId = Convert.ToInt32(pimCategoryId), Status = true, DisplayOrder = AdminConstants.DefaultDisplayOrder });
            }
            return categoryProductlistModel;
        }
    }
}