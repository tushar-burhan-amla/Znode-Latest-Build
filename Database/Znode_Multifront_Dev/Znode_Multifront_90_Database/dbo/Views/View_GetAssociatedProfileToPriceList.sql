
CREATE view [dbo].[View_GetAssociatedProfileToPriceList]
AS
		SELECT ZPR.PortalId, ZPR.StoreName, ZP.ProfileId, ZP.ProfileName, CONVERT(date, ZPLP.CreatedDate) AS CreatedDate, CONVERT(date, ZPLP.ModifiedDate) AS ModifiedDate,
		CASE
		WHEN ZPLP.PortalProfileID IS NULL THEN 0
		ELSE 1
		END AS IsAssociated, ZPL.PriceListId, ZPLP.PriceListProfileId, ZPLP.Precedence
		FROM [dbo].[ZnodeProfile] AS ZP	CROSS JOIN	ZnodePriceList AS ZPL	
		Inner join [ZnodePortalprofile] Zpp On ZP.ProfileId = Zpp.ProfileId 
		LEFT JOIN	[dbo].[ZnodePriceListProfile] AS ZPLP	ON (Zpp.PortalProfileID = ZPLP.PortalProfileID AND ZPLP.PriceListId = ZPL.PriceListId	)
		LEFT OUTER JOIN ZnodePortal ZPR on Zpp.PortalId = ZPR.PortalId