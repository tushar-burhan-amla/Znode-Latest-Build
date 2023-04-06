using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public class ProductAssociationHelper
    {
        private readonly IZnodeRepository<ZnodePimProduct> _PimProduct;

        public ProductAssociationHelper()
        {
            _PimProduct = new ZnodeRepository<ZnodePimProduct>();
        }

        public virtual void SaveProductAsDraft(int pimProductId)
        {
            if (pimProductId > 0)
            {
                ZnodePimProduct pimProductEntity = _PimProduct.Table.FirstOrDefault(x => x.PimProductId == pimProductId);
                if (pimProductEntity != null)
                {
                    pimProductEntity.PublishStateId = (byte?)ZnodePublishStatesEnum.DRAFT;
                    _PimProduct.Update(pimProductEntity);
                }
            }
        }
    }
}