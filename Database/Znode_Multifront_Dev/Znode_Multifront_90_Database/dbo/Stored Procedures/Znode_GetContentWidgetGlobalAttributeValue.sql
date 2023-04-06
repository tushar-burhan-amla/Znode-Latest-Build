CREATE PROCEDURE [dbo].[Znode_GetContentWidgetGlobalAttributeValue]
(
    @EntityName       NVARCHAR(500) = 0,
	@FamilyCode   VARCHAR(200),
	@LocaleCode       VARCHAR(100) = '',
    @GroupCode  NVARCHAR(200) = null,
    @SelectedValue BIT = 0
)
AS
 BEGIN
 BEGIN TRY
	DECLARE @LocaleId INT
	DECLARE @GlobalFamilyId   INT =(SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily WHERE FamilyCode = @FamilyCode)
	DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);
          SET @V_MediaServerThumbnailPath =
         (
             SELECT ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL)+ZMSM.ThumbnailFolderName+'/'  
             FROM ZnodeMediaConfiguration ZMC 
			 INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
		     WHERE IsActive = 1 
         );

        Declare	@EntityAttributeList AS TABLE  (GlobalEntityId INT,EntityName NVARCHAR(300),
		GlobalAttributeGroupId INT,GlobalAttributeId INT,AttributeTypeId INT,AttributeTypeName NVARCHAR(300),
			AttributeCode NVARCHAR(300) ,IsRequired bit,IsLocalizable bit,AttributeName  NVARCHAR(300) , HelpDescription NVARCHAR(max),DisplayOrder INT
		) 
			 
		Declare @EntityAttributeValidationList  AS TABLE  
		( GlobalAttributeId INT, ControlName NVARCHAR(300), ValidationName NVARCHAR(300),SubValidationName NVARCHAR(300),
			RegExp NVARCHAR(300), ValidationValue NVARCHAR(300),IsRegExp Bit)

		Declare	@EntityAttributeDefaultValueList AS TABLE  (GlobalAttributeDefaultValueId INT,GlobalAttributeId INT,
		AttributeDefaultValueCode NVARCHAR(300),AttributeDefaultValue NVARCHAR(300),RowId INT,IsEditable bit,DisplayOrder INT )

		set @LocaleId = (select top 1 LocaleId FROM ZnodeLocale WHERE Code = @LocaleCode)

        IF ISnull(@GroupCode, '') = ''
        BEGIN
			INSERT INTO @EntityAttributeList
				(	GlobalEntityId ,EntityName  ,
				GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
				AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder  ) 
			SELECT qq.GlobalEntityId,qq.EntityName,ww.GlobalAttributeGroupId,
				c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
				c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
			FROM dbo.ZnodeGlobalEntity AS qq
				INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
				INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
				INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
				INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
				INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
				INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
				WHERE qq.EntityName=@EntityName --AND ( f.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
				and w.GlobalAttributeFamilyId = @GlobalFamilyId

		END
		Else

            BEGIN
                    INSERT INTO @EntityAttributeList
                            ( GlobalEntityId ,EntityName  ,
                            GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
                            AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder  )
                            SELECT qq.GlobalEntityId,qq.EntityName,ww.GlobalAttributeGroupId,
                            c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
                            c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
                    FROM dbo.ZnodeGlobalEntity AS qq
					INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
					INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
					INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
					INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
					INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
					INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
					WHERE qq.EntityName=@EntityName AND ( f.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
					and w.GlobalAttributeFamilyId = @GlobalFamilyId	
                                AND exists( select 1 FROM ZnodeGlobalAttributeGroup g WHERE ww.GlobalAttributeGroupId = g.GlobalAttributeGroupId and g.GroupCode = @GroupCode )	
        
			END

			INSERT INTO @EntityAttributeValidationList
			(GlobalAttributeId,ControlName , ValidationName ,SubValidationName , RegExp, ValidationValue,IsRegExp)
			Select aa.GlobalAttributeId,i.ControlName,i.Name AS ValidationName,j.ValidationName AS SubValidationName,
			j.RegExp,k.Name AS ValidationValue,CAST(CASE WHEN j.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp		
			FROM @EntityAttributeList aa
			INNER JOIN dbo.ZnodeGlobalAttributeValidation AS k ON k.GlobalAttributeId = aa.GlobalAttributeId
			INNER JOIN dbo.ZnodeAttributeInputValidation AS i ON k.InputValidationId = i.InputValidationId
			LEFT JOIN dbo.ZnodeAttributeInputValidationRule AS j ON k.InputValidationRuleId = j.InputValidationRuleId

			INSERT INTO @EntityAttributeDefaultValueList
			(GlobalAttributeDefaultValueId,GlobalAttributeId,AttributeDefaultValueCode,
			AttributeDefaultValue ,RowId ,IsEditable ,DisplayOrder )
			Select  h.GlobalAttributeDefaultValueId, aa.GlobalAttributeId,h.AttributeDefaultValueCode,g.AttributeDefaultValue,0,ISNULL(h.IsEditable, 1),
			h.DisplayOrder
			FROM  @EntityAttributeList aa
			INNER JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeId = aa.GlobalAttributeId
			INNER JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
		  
		 
			IF NOT EXISTS (SELECT 1 FROM @EntityAttributeList )
			BEGIN
				INSERT INTO @EntityAttributeList
				(	GlobalEntityId ,EntityName  ,GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
				AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription  ) 
				SELECT qq.GlobalEntityId,qq.EntityName,0 GlobalAttributeGroupId,
				0 GlobalAttributeId,0 AttributeTypeId,''AttributeTypeName,''AttributeCode,0 IsRequired,
				0 IsLocalizable,'' AttributeName,'' HelpDescription
				FROM dbo.ZnodeGlobalEntity AS qq
				WHERE qq.EntityName=@EntityName 
			End
				

			SELECT GlobalEntityId,EntityName,GlobalAttributeGroupId,
			AA.GlobalAttributeId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,
			IsLocalizable,AttributeName,ControlName,ValidationName,SubValidationName,RegExp,
			ValidationValue,cast(isnull(IsRegExp,0) as bit)  IsRegExp,
			HelpDescription,NULL AS AttributeValue,NULL AS GlobalAttributeValueId,NULL AS GlobalAttributeDefaultValueId,
			aab.AttributeDefaultValueCode,
			aab.AttributeDefaultValue,isnull(aab.RowId,0)   RowId,cast(isnull(aab.IsEditable,0) as bit)   IsEditable
			,NULL AS MediaId,AA.DisplayOrder
			FROM @EntityAttributeList AA				
			LEFT JOIN @EntityAttributeDefaultValueList aab on aab.GlobalAttributeId=AA.GlobalAttributeId	
			LEFT JOIN @EntityAttributeValidationList vl on vl.GlobalAttributeId=aa.GlobalAttributeId			
			ORDER BY AA.DisplayOrder, aab.DisplayOrder

			SELECT 1 AS ID,CAST(1 AS BIT) AS Status;       
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()
		DECLARE @Status BIT ;
	SET @Status = 0;

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
	@ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetContentWidgetGlobalAttributeValue 
	@EntityName = '+@EntityName+',@LocaleCode='+@LocaleCode+',
	@GroupCode = '+@GroupCode+',@SelectedValue = '+CAST(@SelectedValue AS VARCHAR(10))+',@GlobalFamilyId = '+CAST(@GlobalFamilyId AS VARCHAR(10));   
		 
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetContentWidgetGlobalAttributeValue',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END;
