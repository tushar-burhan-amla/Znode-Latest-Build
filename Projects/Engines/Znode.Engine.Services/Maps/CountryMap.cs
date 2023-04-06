using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;

namespace Znode.Engine.Services.Maps
{
    public static class CountryMap
    {
        //Convert ZnodeCountry entity into CountryModel.
        public static CountryModel ToModel(ZnodeCountry entity)
        {
            if (!Equals(entity, null))
            {
                return new CountryModel
                {
                    CountryId = entity.CountryId,
                    CountryCode = entity.CountryCode,
                    CountryName = entity.CountryName,
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

        //Convert IList<ZnodeCountry> into CountryListModel.
        public static CountryListModel ToListModel(IList<ZnodeCountry> entity)
        {
            if (!Equals(entity, null))
            {
                var model = new CountryListModel();
                foreach (var item in entity)
                {
                    model.Countries.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }
    }
}
