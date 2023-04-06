


CREATE View [dbo].[View_GetAssociatedPortalToPriceList] 
AS
SELECT a.PortalId,   a.StoreName ,CONVERT(DATE,b.CreatedDate)CreatedDate , CONVERt(DATE,b.ModifiedDate)ModifiedDate,Case WHEN b.PortalId IS NULL THEN 0 ELSE 1 END IsAssociated ,c.PriceListId, b.PriceListPortalId,b.Precedence,f.CatalogName
FROM ZnodePortal a 
CROSS JOIN ZnodePriceList c 
LEFT JOIN ZnodePriceListPortal b ON (a.PortalId= b.PortalId AND b.PriceListId = c.PriceListId)
LEFT JOIN ZnodePortalUnit d ON (d.PortalId = a.PortalId)
LEFT JOIN ZnodePortalCatalog e ON (e.PortalId = a.PortalId)
LEFT JOIN ZnodePublishCatalog f ON (e.PublishCatalogId = f.PublishCatalogId)
WHERE d.CurrencyId = c.CurrencyId