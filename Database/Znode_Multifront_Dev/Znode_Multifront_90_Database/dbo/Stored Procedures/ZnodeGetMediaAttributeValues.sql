
CREATE PROCEDURE [dbo].[ZnodeGetMediaAttributeValues]
( 
	@MediaID  INT = 0,
	@LocaleId INT = 1
)
AS
/*
Summary: This Procedure is used to get values of media attribute
Unit Testing:
Exec ZnodeGetMediaAttributeValues

*/
BEGIN
BEGIN TRY
SET NOCOUNT ON

	Declare @ServerPath varchar(500)
	SET @ServerPath = 
	(
		SELECT top 1 ZnodeMediaConfiguration.URL+ZnodeMediaServerMaster.ThumbnailFolderName    
		FROM [dbo].[ZnodeMediaServerMaster] 
		INNER JOIN [dbo].[ZnodeMediaConfiguration] on [dbo].[ZnodeMediaServerMaster].MediaServerMasterId=ZnodeMediaConfiguration.MediaServerMasterId 
		WHERE ZnodeMediaConfiguration.IsActive='True' 
	)

    DECLARE @AttributeValue TABLE
    (
		[MediaCategoryId]              [INT] NOT NULL,
		[MediaId]                      [INT] NULL,
		[MediaPathId]                  [INT] NULL,
		[MediaAttributeFamilyId]       [INT] NULL,
		[FamilyCode]                   [VARCHAR](200) NULL,
		[MediaAttributeId]             [INT] NULL,
		[AttributeTypeId]              [INT] NULL,
		[AttributeTypeName]            [VARCHAR](300) NULL,
		[AttributeCode]                [VARCHAR](300) NULL,
		[IsRequired]                   [BIT] NULL,
		[IsLocalizable]                [BIT] NULL,
		[IsFilterable]                 [BIT] NULL,
		[AttributeName]                [NVARCHAR](300) NULL,
		[AttributeValue]               [VARCHAR](300) NULL,
		[MediaAttributeValueId]        [INT] NULL,
		[MediaAttributeDefaultValueId] [INT] NULL,
		[DefaultAttributeValue]        [NVARCHAR](300) NULL,
		[MediaPath]                    [VARCHAR](8000) NULL,
		[RowId]                        [INT] NOT NULL,
		[IsEditable]                   [BIT] NOT NULL,
		[ControlName]                  [VARCHAR](300) NULL,
		[ValidationName]               [VARCHAR](100) NULL,
		[SubValidationName]            [VARCHAR](300) NULL,
		[RegExp]                       [VARCHAR](300) NULL,
		[ValidationValue]              [VARCHAR](300) NULL,
		[IsRegExp]                     [BIT] NULL,
		[AttributeGroupName]           [NVARCHAR](300) NULL,
		DisplayOrder                   INT,
		[HelpDescription]              [NVARCHAR](MAX) NULL,
		GroupDisplayOrder              INT,
		MediaAttributeThumbnailPath    varchar(1000)
    );
    INSERT INTO @AttributeValue([MediaCategoryId]  ,[MediaId],[MediaPathId],[MediaAttributeFamilyId] ,[FamilyCode],[MediaAttributeId] ,           
				[AttributeTypeId],[AttributeTypeName] ,[AttributeCode] ,[IsRequired],[IsLocalizable],[IsFilterable] ,               
				[AttributeName],[AttributeValue],[MediaAttributeValueId],[MediaAttributeDefaultValueId],[DefaultAttributeValue],       
				[MediaPath],[RowId],[IsEditable] ,[ControlName] ,[ValidationName],[SubValidationName],[RegExp],[ValidationValue],             
				[IsRegExp],[AttributeGroupName],DisplayOrder,[HelpDescription] ,GroupDisplayOrder )
    SELECT DISTINCT
            a.MediaCategoryId,
            a.MediaId,
            a.MediaPathId,
            a.MediaAttributeFamilyId,
            qq.FamilyCode,
            c.MediaAttributeId,
            c.AttributeTypeId,
            Q.AttributeTypeName,
            c.AttributeCode,
            c.IsRequired,
            c.IsLocalizable,
            c.IsFilterable,
            f.AttributeName,
            b.AttributeValue,
            b.MediaAttributeValueId,
            h.MediaAttributeDefaultValueId,
            g.DefaultAttributeValue,
            dbo.Fn_ZnodeMediaRecurcivePath(a.MediaPathId, @LocaleId) AS MediaPath,
            ISNULL(NULL, 0) AS RowId,
            ISNULL(h.IsEditable, 1) AS IsEditable,
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
            c.DisplayOrder,
            c.HelpDescription,
            Zmag.DisplayOrder	
    FROM [dbo].[ZnodeMediaCategory] AS a
            INNER JOIN dbo.ZnodeMediaAttributeFamily AS qq ON(a.MediaAttributeFamilyId = qq.MediaAttributeFamilyId)
            INNER JOIN dbo.ZnodeMediaFamilyGroupMapper AS w ON(qq.MediaAttributeFamilyId = w.MediaAttributeFamilyId)
            INNER JOIN dbo.ZnodeMediaAttributeGroupMapper AS t ON(t.MediaAttributeGroupId = w.MediaAttributeGroupId)
            INNER JOIN dbo.ZnodeMediaAttributeGroup AS Zmag ON(t.MediaAttributeGroupId = Zmag.MediaAttributeGroupId
                                                            AND Zmag.IsHidden = 0)
            INNER JOIN ZnodeMediaAttributeGroupLocale AS Zmagm ON t.MediaAttributeGroupId = Zmagm.MediaAttributeGroupId
                                                                AND Zmagm.LocaleId = @LocaleId
            LEFT JOIN [dbo].[ZnodeMediaAttributeValue] AS b ON(t.MediaAttributeId = b.MediaAttributeId
                                                            AND a.MediaCategoryId = b.MediaCategoryId)
            LEFT JOIN [dbo].[ZnodeMediaAttribute] AS c ON(c.MediaAttributeId = t.MediaAttributeId)
            LEFT JOIN [dbo].ZnodeAttributeType AS q ON(c.AttributeTypeId = Q.AttributeTypeId)
            LEFT JOIN [dbo].[ZnodeMediaAttributeLocale] AS f ON(c.MediaAttributeId = f.MediaAttributeId
                                                                AND f.LocaleId = @LocaleId)
            LEFT JOIN [dbo].[ZnodeMediaAttributeDefaultValue] AS h ON(h.MediaAttributeDefaultValueId = b.MediaAttributeDefaultValueId
                                                                    OR h.MediaAttributeId = t.MediaAttributeId)
            LEFT JOIN [dbo].[ZnodeMediaAttributeDefaultValueLocale] AS g ON(h.MediaAttributeDefaultValueId = g.MediaAttributeDefaultValueId
                                                                            AND g.LocaleId = @LocaleId)
            LEFT JOIN [dbo].ZnodeMediaAttributeValidation AS k ON(k.MediaAttributeId = c.MediaAttributeId)
            LEFT JOIN [dbo].ZnodeAttributeInputValidation AS i ON(k.InputValidationId = i.InputValidationId)
            LEFT JOIN [dbo].ZnodeAttributeInputValidationRule AS j ON(k.InputValidationRuleId = j.InputValidationRuleId)			
	WHERE CASE WHEN @MediaID = 0 THEN  0 ELSE  a.MediaId END  = ISNULL(@MediaID, 0);

	update a set MediaAttributeThumbnailPath = @ServerPath+'/'+ZM.Path+'?'+'V='+cast(ZM.Version as varchar(10))
	from @AttributeValue a
	INNER JOIN ZnodeMedia ZM ON a.AttributeValue = ZM.MediaId 
	where a.AttributeTypeName = 'Image' 

    SELECT MediaCategoryId,
        MediaId,
        MediaPathId,
        MediaAttributeFamilyId,
        FamilyCode,
        MediaAttributeId,
        AttributeTypeId,
        AttributeTypeName,
        AttributeCode,
        IsRequired,
        IsLocalizable,
        IsFilterable,
        AttributeName,
        AttributeValue,
        MediaAttributeValueId,
        MediaAttributeDefaultValueId,
        DefaultAttributeValue,
        MediaPath,
        RowId,
        IsEditable,
        ControlName,
        ValidationName,
        SubValidationName,
        RegExp,
        ValidationValue,
        IsRegExp,
        AttributeGroupName,
        HelpDescription,
        GroupDisplayOrder,
		MediaAttributeThumbnailPath
    FROM @AttributeValue
	
    ORDER BY GroupDisplayOrder,
            DisplayOrder;
			
END TRY
BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeGetMediaAttributeValues @MediaID = '+CAST(@MediaID AS VARCHAR(100))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'ZnodeGetMediaAttributeValues',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END;



            