CREATE VIEW [dbo].[View_MediaAttributeValues]
AS

     SELECT a.MediaCategoryId,
            a.MediaId,
            a.MediaPathId,
            a.MediaAttributeFamilyId,
            qq.FamilyCode,
            c.MediaAttributeId,
            c.AttributeTypeId,
            q.AttributeTypeName,
            c.AttributeCode,
            c.IsRequired,
            c.IsLocalizable,
            c.IsFilterable,
            f.AttributeName,
            b.AttributeValue,
            b.MediaAttributeValueId,
            b.MediaAttributeDefaultValueId,
            g.DefaultAttributeValue,
            dbo.Fn_ZnodeMediaRecurcivePath(a.MediaPathId, 1) AS MediaPath,
            ISNULL(NULL, 0) AS RowId,
            h.IsEditable,
            i.ControlName,
            i.Name AS ValidationName,
            j.ValidationName AS SubValidationName,
            j.RegExp,
            k.Name AS ValidationValue,
            CAST(CASE
                     WHEN j.RegExp IS NULL
                     THEN 0
                     ELSE 1
                 END AS BIT) AS IsRegExp,
            Zmagm.AttributeGroupName,
            c.HelpDescription,
			Z.ServerPath+'/'+ZM.Path+'?'+'V='+cast(ZM.Version as varchar(10)) as MediaAttributeThumbnailPath	
     FROM dbo.ZnodeMediaCategory AS a
          INNER JOIN dbo.ZnodeMediaAttributeFamily AS qq ON a.MediaAttributeFamilyId = qq.MediaAttributeFamilyId
          INNER JOIN dbo.ZnodeMediaFamilyGroupMapper AS w ON qq.MediaAttributeFamilyId = w.MediaAttributeFamilyId
          INNER JOIN dbo.ZnodeMediaAttributeGroupMapper AS t ON t.MediaAttributeGroupId = w.MediaAttributeGroupId
          INNER JOIN ZnodeMediaAttributeGroupLocale Zmagm ON t.MediaAttributeGroupId = Zmagm.MediaAttributeGroupId
          LEFT OUTER JOIN dbo.ZnodeMediaAttributeValue AS b ON t.MediaAttributeId = b.MediaAttributeId
                                                               AND a.MediaCategoryId = b.MediaCategoryId
          LEFT OUTER JOIN dbo.ZnodeMediaAttribute AS c ON c.MediaAttributeId = t.MediaAttributeId
          LEFT OUTER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
          LEFT OUTER JOIN dbo.ZnodeMediaAttributeLocale AS f ON c.MediaAttributeId = f.MediaAttributeId
          LEFT OUTER JOIN dbo.ZnodeMediaAttributeDefaultValue AS h ON h.MediaAttributeDefaultValueId = b.MediaAttributeDefaultValueId
          LEFT OUTER JOIN dbo.ZnodeMediaAttributeDefaultValueLocale AS g ON b.MediaAttributeDefaultValueId = g.MediaAttributeDefaultValueId
          LEFT OUTER JOIN dbo.ZnodeMediaAttributeValidation AS k ON k.MediaAttributeId = c.MediaAttributeId
          LEFT OUTER JOIN dbo.ZnodeAttributeInputValidation AS i ON k.InputValidationId = i.InputValidationId
          LEFT OUTER JOIN dbo.ZnodeAttributeInputValidationRule AS j ON k.InputValidationRuleId = j.InputValidationRuleId
		  LEFT OUTER JOIN ZnodeMedia ZM ON a.MediaId = ZM.MediaId
		  CROSS APPLY (
						SELECT top 1 ZnodeMediaConfiguration.URL+ZnodeMediaServerMaster.ThumbnailFolderName  AS ServerPath  
						FROM [dbo].[ZnodeMediaServerMaster] 
						INNER JOIN [dbo].[ZnodeMediaConfiguration] on [dbo].[ZnodeMediaServerMaster].MediaServerMasterId=ZnodeMediaConfiguration.MediaServerMasterId 
						WHERE ZnodeMediaConfiguration.IsActive='True' 
					)z
;
GO
