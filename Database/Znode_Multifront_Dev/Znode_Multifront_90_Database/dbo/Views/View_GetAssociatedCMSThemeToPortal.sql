




CREATE View [dbo].[View_GetAssociatedCMSThemeToPortal]
AS 
SELECT a.PortalId,a.StoreName,b.CMSThemeId,c.CMSPortalThemeId,CONVERT(DATE,c.CreatedDate)CreatedDate,CONVERt(DATE,c.ModifiedDate)ModifiedDate , CASE WHEN c.CMSPortalThemeId IS NULL THEN  0 ELSE 1 END IsAssociated
FROM ZnodePortal a 
Cross JOIN ZnodeCMSTheme b 
LEFT JOIN  [dbo].[ZnodeCMSPortalTheme] c ON (c.PortalId = a.PortalId AND c.CMSThemeId = b.CMSThemeId)