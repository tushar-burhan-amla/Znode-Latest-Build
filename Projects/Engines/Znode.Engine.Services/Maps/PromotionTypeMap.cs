using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class PromotionTypeMap
    {
        public static PromotionTypeModel ToPromotionTypeModel(ZnodePromotionType promotionTypeEntity)
        {
            if (Equals(promotionTypeEntity, null))
                return null;

            PromotionTypeModel promotionTypeModel = new PromotionTypeModel
            {
                PromotionTypeId = promotionTypeEntity.PromotionTypeId,
                ClassName = promotionTypeEntity.ClassName,
                Name = promotionTypeEntity.Name,
                ClassType = promotionTypeEntity.ClassType,
                IsActive = promotionTypeEntity.IsActive,
                Description = promotionTypeEntity.Description,
            };
            return promotionTypeModel;
        }

        public static ZnodePromotionType ToPromotionTypeEntity(PromotionTypeModel promotionTypeModel)
        {
            if (Equals(promotionTypeModel, null))
                return null;

            ZnodePromotionType promotionTypeEntity = new ZnodePromotionType
            {
                PromotionTypeId = promotionTypeModel.PromotionTypeId,
                ClassName = promotionTypeModel.ClassName,
                Name = promotionTypeModel.Name,
                ClassType = promotionTypeModel.ClassType,
                IsActive = promotionTypeModel.IsActive,
                Description = promotionTypeModel.Description,
            };
            return promotionTypeEntity;
        }

        public static PromotionTypeModel ToModel(IZnodePromotionsType znodePromotionType)
        {
            if (Equals(znodePromotionType, null))
                return null;

            PromotionTypeModel promotionTypeModel = new PromotionTypeModel
            {
                ClassName = znodePromotionType.ClassName,
                Description = znodePromotionType.Description,
                Name = znodePromotionType.Name,
                ClassType = GetClassTypeByClassName(znodePromotionType)
            };
            return promotionTypeModel;
        }

        private static string GetClassTypeByClassName(IZnodePromotionsType znodePromotionType)
        {
            string classType = string.Empty;
            if (znodePromotionType != null)
            {
                if (Equals(znodePromotionType.GetType().BaseType, typeof(ZnodeCartPromotionType))) { classType = "CART"; }
                if (Equals(znodePromotionType.GetType().BaseType, typeof(ZnodePricePromotionType))) { classType = "PRICE"; }
                if (Equals(znodePromotionType.GetType().BaseType, typeof(ZnodeProductPromotionType))) { classType = "PRODUCT"; }
            }
            return classType;
        }
    }
}
