



CREATE View [dbo].[View_GetAssociatedPortalToShipping] 
AS
SELECT a.PortalId, a.StoreName, CONVERT(DATE,b.CreatedDate)CreatedDate, CONVERt(DATE,b.ModifiedDate)ModifiedDate,Case WHEN b.PortalId IS NULL THEN 0 ELSE 1 END IsAssociated ,c.ShippingId, b.ShippingPortalId
FROM ZnodePortal a 
CROSS JOIN ZnodeShipping c 
LEFT JOIN ZnodeShippingPortal b ON (a.PortalId= b.PortalId )