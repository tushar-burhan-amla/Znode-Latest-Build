	CREATE VIEW [dbo].[View_GetAllMediaInRoot]
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
				[Description] [ShortDescription]
				,[Version]
		 FROM
		 (
			 SELECT 0 AS MediaCategoryId,
					ZMPL.MediaPathId,
					'-1' ParentMediaPathId,
					CASE
						WHEN ZMPL.[PathName] IS NULL
						THEN 'Root'
						ELSE ZMPL.[PathName]
					END AS Folder,
					zM.FileName,
					zM.Size,
					zM.Height,
					zM.Width,
					zM.Type,
					zM.Type AS MediaType,
					CONVERT( DATE, zm.CreatedDate) CreatedDate,
					CONVERT( DATE, zm.ModifiedDate) ModifiedDate,
					zM.MediaId,
					NULL AS MediaAttributeName,
					NULL AS MediaAttributeValue,
					zmf.FamilyCode,
					[dbo].[Fn_GetMediaThumbnailMediaPath]( zM.Path) MediaThumbnailPath,
					[dbo].[Fn_GetMediaPathServer]( zM.Path)  MediaServerPath,
					zM.Path,
					zM.CreatedBy,
					zmae.AttributeCode,
					zmav.AttributeValue,
					zM.Version
			 FROM dbo.ZnodeMedia AS zM
				  LEFT JOIN ZnodeMediaCategory zma ON(zm.mediaid = zma.MediaId)
		 --         LEFT JOIN ZnodeMediaConfiguration ZMC ON (ZMC.MediaConfigurationId = ZM.MediaConfigurationId )
				  --LEFT JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
				  LEFT JOIN ZnodeMediaAttributeFamily zmf ON(zma.MediaAttributeFamilyId = zmf.MediaAttributeFamilyId)
				  LEFT OUTER JOIN dbo.ZnodeMediaPath AS zmp ON(zma.MediaPathId = zmp.MediaPathId)
				  LEFT OUTER JOIN dbo.ZnodeMediaPathLocale AS ZMPL ON zmp.MediaPathId = ZMPL.MediaPathId
				  LEFT JOIN dbo.ZnodeMediaAttributeValue Zmav ON(zmav.MediaCategoryId = zma.MediaCategoryId)
				  LEFT JOIN dbo.ZnodeMediaAttribute zmae ON(zmae.MediaAttributeId = Zmav.MediaAttributeId
															AND zmae.AttributeCode IN('DisplayName', 'Description'))) v 
		 PIVOT(MAX(AttributeValue) FOR AttributeCode IN([DisplayName],
															[Description])) PV;
