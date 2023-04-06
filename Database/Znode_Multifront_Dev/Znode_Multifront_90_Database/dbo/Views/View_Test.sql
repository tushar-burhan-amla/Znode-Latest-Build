CREATE  View [dbo].[View_Test]
AS 

   
   SELECT pv.*  
   FROM (select Zmc.MediaCategoryId ,ZMPL.MediaPathId  , ZMPL.[PathName] [Folder],zM.[FileName] ,Zm.Size , Zm.Type [MediaType] 
   ,ZM.CreatedDate   
   ,ZM.ModifiedDate  
   ,Zm.MediaId ,zal.AttributeName , Zmav.AttributeValue  , ZM.Path   
   FROM  ZnodeMediaCategory ZMC   
   inner join ZnodeMediaPathLocale ZMPL on (ZMC.MediaPathId = ZMPL.MediaPathId) 
   INNER JOIN ZnodeMedia zM ON (Zm.MediaId = Zmc.MediaId) 
   LEFT JOIN dbo.ZnodeMediaAttributeValue Zmav ON (zmav.MediaCategoryId = zmc.MediaCategoryId)
   left JOIN dbo.ZnodeMediaAttributeLocale ZAL ON (zal.MediaAttributeId = Zmav.MediaAttributeId))v
   PIVOT (
   MAX(AttributeValue )  FOR AttributeName IN ([ShortDescription] )
   )PV