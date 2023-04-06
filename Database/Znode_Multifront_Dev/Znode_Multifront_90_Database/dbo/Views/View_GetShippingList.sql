
CREATE VIEW [dbo].[View_GetShippingList]
AS
     SELECT zs.ShippingId,
            Zs.ShippingTypeId,
            Zst.Name ShippingType,
            zs.ShippingCode,
            zps.ProfileId,
            zp.ProfileName,
            zs.Description,
            zs.DestinationCountryCode CountryCode,
            zs.HandlingCharge,
            zs.DisplayOrder,
            zs.IsActive
     FROM ZnodeShipping zs
	      INNER JOIN ZnodeProfileShipping zps ON (zs.ShippingId = ZPS.ShippingId)
          INNER JOIN ZnodeShippingTypes zst ON(zst.ShippingTypeId = zs.ShippingTypeId)
          INNER JOIN ZnodeProfile zp ON(zp.ProfileId = zps.ProfileId);