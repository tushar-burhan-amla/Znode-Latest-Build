
CREATE VIEW [dbo].[View_GetProfileCatalog]
AS
     SELECT ZP.ProfileId,
            ZP.PimCatalogId,
            ZP.ProfileName,
            ZC.CatalogName
     FROM ZnodeProfile ZP 
	 INNER JOIN ZnodePimCatalog ZC On (ZC.PimCatalogId = ZP.PimCatalogId)