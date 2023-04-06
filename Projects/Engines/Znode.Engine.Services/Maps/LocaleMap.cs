using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class LocaleMap
    {
        public static LocaleModel ToModel(ZnodeLocale entity)
        {
            if (!Equals(entity, null))
            {
                return new LocaleModel
                {
                    LocaleId = entity.LocaleId,
                    Name = entity.Name,
                    Code = entity.Code,
                    IsActive = entity.IsActive,
                    IsDefault = entity.IsDefault,
                    CreatedBy = entity.CreatedBy,
                    CreatedDate = entity.CreatedDate,
                    ModifiedBy = entity.ModifiedBy,
                    ModifiedDate = entity.ModifiedDate
                };
            }
            else
                return null;
        }

        //Convert IList<ZnodeLocale> in to LocaleListModel model.
        public static LocaleListModel ToListModel(IList<ZnodeLocale> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new LocaleListModel();
                foreach (var item in entity)
                {
                    model.Locales.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Convert IList<ZnodeLocale> in to List<LocaleModel> model.
        public static List<LocaleModel> ToModelList(IList<ZnodeLocale> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new List<LocaleModel>();
                foreach (var item in entity)
                {
                    model.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Converts LocaleModel model to ZnodeLocale entity.
        public static ZnodeLocale ToEntity(LocaleModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeLocale
                {
                    LocaleId = model.LocaleId,
                    Name = model.Name,
                    Code = model.Code,
                    IsActive = model.IsActive,
                    IsDefault = model.IsDefault,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate,
                    ModifiedBy = model.ModifiedBy,
                    ModifiedDate = model.ModifiedDate
                };
            }
            return null;
        }
    }
}
