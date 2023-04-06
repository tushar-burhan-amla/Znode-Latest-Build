using Znode.Engine.Api.Models;
using Znode.Engine.Shipping;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class ShippingTypeMap
    {
        public static ShippingTypeModel ToShippingTypeModel(ZnodeShippingType shippingTypeEntity)
        {
            if (Equals(shippingTypeEntity, null))
                return null;

            return new ShippingTypeModel
            {
                ShippingTypeId = shippingTypeEntity.ShippingTypeId,
                ClassName = shippingTypeEntity.ClassName,
                Name = shippingTypeEntity.Name,
                IsActive = shippingTypeEntity.IsActive,
                Description = shippingTypeEntity.Description,
            };
        }

        public static ZnodeShippingType ToShippingTypeEntity(ShippingTypeModel shippingTypeModel)
        {
            if (Equals(shippingTypeModel, null))
                return null;

            ZnodeShippingType shippingTypeEntity = new ZnodeShippingType
            {
                ShippingTypeId = shippingTypeModel.ShippingTypeId,
                ClassName = shippingTypeModel.ClassName,
                Name = shippingTypeModel.Name,
                IsActive = shippingTypeModel.IsActive,
                Description = shippingTypeModel.Description,
            };
            return shippingTypeEntity;
        }

        public static ShippingTypeModel ToModel(IZnodeShippingsType znodeShippingType)
        {
            if (Equals(znodeShippingType, null))
                return null;

            ShippingTypeModel shippingTypeModel = new ShippingTypeModel
            {
                ClassName = znodeShippingType.ClassName,
                Description = znodeShippingType.Description,
                Name = znodeShippingType.Name,
            };
            return shippingTypeModel;
        }
    }
}
