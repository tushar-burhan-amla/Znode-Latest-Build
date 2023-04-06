

  CREATE View [dbo].[View_PimProductImage]
   AS 
  SELECT a.PimProductId, a.PimAttributeId, c.AttributeCode AS PimAttributeCode, b.Path, b.FileName, b.MediaId
  FROM            dbo.ZnodePimProductImage AS a INNER JOIN
                         dbo.ZnodeMedia AS b ON a.MediaId = b.MediaId INNER JOIN
                         dbo.ZnodePimAttribute AS c ON a.PimAttributeId = c.PimAttributeId