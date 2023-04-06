using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class ProductMap
    {
        //Convert ZnodeProduct entity in to ProductModel.
        public static ProductModel ToModel(ZnodePimProduct entity)
        {
            if (!Equals(entity, null))
            {
                return new ProductModel
                {
                    ProductId = entity.PimProductId
                };
            }
            else
                return null;
        }

        //Convert ZnodeProduct entity in to ProductModel.
        public static ProductListModel ToModel(IList<ZnodePimProduct> entityList)
        {
            if (!Equals(entityList, null))
            {
                ProductListModel productListModel = new ProductListModel();
                foreach (ZnodePimProduct productEntity in entityList)
                {
                    productListModel.Products.Add(ToModel(productEntity));
                }
                return productListModel;
            }
            return null;
        }

        //Convert ProductModel model in to ZnodeProduct entity.
        public static ZnodePimProduct ToEntity(ProductModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodePimProduct
                {

                };
            }
            else
                return null;
        }

    }
}
