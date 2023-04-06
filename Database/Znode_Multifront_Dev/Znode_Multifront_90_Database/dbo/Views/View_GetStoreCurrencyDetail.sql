

CREATE View [dbo].[View_GetStoreCurrencyDetail]
AS 

	
		SELECT DISTINCT ZP.PortalId,ZP.StoreName, ZP.StoreName + ' ('+ZCL.Symbol+')' AS StoreNameWithCurrencySymbol 
		FROM ZnodePortal ZP
		INNER JOIN ZnodePortalunit ZPU on   (ZPU.PortalId =ZP.PortalId)
		INNER JOIN ZnodeCulture ZCL on   (ZCL.CultureId =ZPU.CultureId)
		INNER JOIN ZnodeCurrency ZC on   (ZCL.CurrencyId =ZC.CurrencyId)