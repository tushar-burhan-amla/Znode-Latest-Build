CREATE VIEW [dbo].[View_GetMediaPathDetail]  
AS  
     SELECT MediaCategoryId,  
            MediaPathId,  
            [Folder],  
            [FileName],  
            Size,  
   Height,  
   Width,  
      Type,  
            [MediaType],  
            CreatedDate,  
            ModifiedDate,  
            MediaId,  
            Path,  
            MediaServerPath MediaServerPath,  
            MediaThumbnailPath MediaServerThumbnailPath,  
            FamilyCode,  
            CreatedBy,  
            [DisplayName] [DisplayName],  
            [Description] [ShortDescription] ,  
			[Version]    
  
     /* INTO #temp2*/  
  
     FROM  
     (  
         SELECT Zmc.MediaCategoryId,  
                ZMPL.MediaPathId,  
                ZMPL.[PathName] [Folder],  
                zM.[FileName],  
                Zm.Size,  
    Zm.Height,  
    Zm.Width,  
    Zm.Type,  
                Zm.Type [MediaType],  
                CONVERT( DATE, zm.CreatedDate) CreatedDate,  
                CONVERT( DATE, zm.ModifiedDate) ModifiedDate,  
                Zm.MediaId,  
                zma.AttributeCode,  
                Zmav.AttributeValue,  
                ISNULL(CASE WHEN ZMCF.CDNURL = '' THEN NULL ELSE ZMCF.CDNURL END,ZMCF.URL)+ZMSM.ThumbnailFolderName+'\'+zM.Path MediaThumbnailPath,    
     ISNULL(CASE WHEN ZMCF.CDNURL = '' THEN NULL ELSE ZMCF.CDNURL END,ZMCF.URL)+zM.Path  MediaServerPath,   
    zM.Path,  
               zmafl.FamilyCode FamilyCode,  
                Zm.CreatedBy 
				, Zm.Version 
         FROM ZnodeMediaCategory ZMC  
              LEFT JOIN ZnodeMediaAttributeFamily zmafl ON(zmc.MediaAttributeFamilyId = zmafl.MediaAttributeFamilyId)  
     INNER JOIN ZnodeMediaPathLocale ZMPL ON(ZMC.MediaPathId = ZMPL.MediaPathId)  
              INNER JOIN ZnodeMedia zM ON(Zm.MediaId = Zmc.MediaId)  
        LEFT JOIN ZnodeMediaConfiguration ZMCF ON (ZMCF.MediaConfigurationId = ZM.MediaConfigurationId AND ZMCF.IsActive = 1)  
     LEFT JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMCF.MediaServerMasterId)  
              LEFT JOIN dbo.ZnodeMediaAttributeValue Zmav ON(zmav.MediaCategoryId = zmc.MediaCategoryId)  
              LEFT JOIN dbo.ZnodeMediaAttribute zma ON(zma.MediaAttributeId = Zmav.MediaAttributeId  
                                                       AND AttributeCode IN('DisplayName', 'Description'))    
      
     ) v PIVOT(MAX(AttributeValue) FOR AttributeCode IN([DisplayName],  
                                                        [Description])) PV;

GO
