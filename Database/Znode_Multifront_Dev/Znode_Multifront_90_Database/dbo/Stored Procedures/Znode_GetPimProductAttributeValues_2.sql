-- EXEC Znode_GetPimProductAttributeValues 144,2,1
CREATE   PROCEDURE [dbo].[Znode_GetPimProductAttributeValues_2] 
		@PimProductId int = 0,
		@LocaleId INT = 0 , 
		@IsCopy   BIT = 0 
AS
BEGIN
BEGIN TRY  
	DECLARE @V_Locale INT 
	   SELECT @V_Locale =  FeatureValues FROM ZnodeGlobalSetting  WHERE FeatureName='Locale'
	DECLARE @PimAttributeFamilyId int = 0
    IF @PimAttributeFamilyID = 0 
	   BEGIN 
	   SELECT @PimAttributeFamilyID = PimAttributeFamilyId FROM ZnodePimAttributeFamily  WHERE  IsDefaultFamily = 1 AND IsCategory = 0 
	   END 

	DECLARE @PIMfamilyIds TABLE (PimAttributefamilyid INT )

	INSERT INTO @PIMfamilyIds
	SELECT DISTINCT PimAttributeFamilyId
	FROM View_PimProductAttributeValueLocale 
	WHERE PimProductId = @PimProductId   
	UNION 
	SELECT @PimAttributeFamilyID PimAttributeFamilyId

	DECLARE @PimAttributeId Table (PimAttributeId INT ) 
	INSERT INTO @PimAttributeId
	select PimAttributeId From ZnodePimAttributevalue a  where --PimAttributeFamilyId =@PimAttributeFamilyID 
	 a.PimProductId = @PimProductId 
	 AND NOT EXISTS (SELECT TOP 1 1 FROM  ZnodePimConfigureProductAttribute d WHERE d.PimAttributeId= a.PimAttributeId AND d.PimFamilyId = a.PimAttributeFamilyId)
	union  
	select PimAttributeId From ZnodePimFamilyGroupMapper a where a.PimAttributeFamilyId =@PimAttributeFamilyID 
	AND NOT EXISTS (SELECT TOP 1 1 FROM  ZnodePimConfigureProductAttribute d WHERE d.PimAttributeId= a.PimAttributeId AND d.PimFamilyId = a.PimAttributeFamilyId)
	UNION 
	SELECT a.PimAttributeId From ZnodePimFamilyGroupMapper a INNER JOIN View_PimAttributeLocale b ON (a.PimAttributeId = b.PimAttributeId AND B.LocaleId = @LocaleId AND b.IsConfigurable = 1 ) 
	where EXISTS (SELECT TOP 1 1 FROM @PIMfamilyIds c WHERE c.PimAttributeFamilyId = a.PimAttributeFamilyId)
	AND NOT EXISTS (SELECT TOP 1 1 FROM  ZnodePimConfigureProductAttribute d WHERE d.PimAttributeId= a.PimAttributeId AND d.PimFamilyId = a.PimAttributeFamilyId)
	UNION 
	SELECT a.PimAttributeId From ZnodePimFamilyGroupMapper a INNER JOIN View_PimAttributeLocale b ON (a.PimAttributeId = b.PimAttributeId AND B.LocaleId = @V_Locale AND b.IsConfigurable = 1 ) 
	where EXISTS (SELECT TOP 1 1 FROM @PIMfamilyIds c WHERE c.PimAttributeFamilyId = a.PimAttributeFamilyId)
	AND NOT EXISTS (SELECT TOP 1 1 FROM  ZnodePimConfigureProductAttribute d WHERE d.PimAttributeId= a.PimAttributeId AND d.PimFamilyId = a.PimAttributeFamilyId)
	--UNION 
	--SELECT PimATTributeId FROM ZnodePimConfigureProductAttribute a  WHERE PimProductId = @PimProductId  


	DECLARE @Attributevales TABLE ( PimAttributeFamilyId	int	,FamilyCode	varchar	(200),PimAttributeId	int	,PimAttributeGroupId	int,AttributeTypeId	int	,AttributeTypeName	varchar	(300)
		,AttributeCode	varchar	(300),IsRequired	bit	,IsLocalizable	bit	,IsFilterable	bit	,AttributeName	nvarchar (600),AttributeValue	nvarchar(MAX),PimAttributeValueId	int	
		,PimAttributeDefaultValueId	int	,AttributeDefaultValue	nvarchar(600),RowId	int	,IsEditable	bit	,ControlName	varchar	(300),ValidationName	varchar	(100),SubValidationName	varchar	(300)
		,RegExp	varchar	(300),ValidationValue	varchar	(300),IsRegExp	bit	,DispalyOrder INt,HelpDescription	varchar(max) )

   	DECLARE @Attributevales_ForUnion TABLE ( PimAttributeFamilyId	int	,FamilyCode	varchar	(200),PimAttributeId	int	,PimAttributeGroupId	int,AttributeTypeId	int	,AttributeTypeName	varchar	(300)
		,AttributeCode	varchar	(300),IsRequired	bit	,IsLocalizable	bit	,IsFilterable	bit	,AttributeName	nvarchar (600),AttributeValue	nvarchar(MAX),PimAttributeValueId	int	
		,PimAttributeDefaultValueId	int	,AttributeDefaultValue	nvarchar(600),RowId	int	,IsEditable	bit	,ControlName	varchar	(300),ValidationName	varchar	(100),SubValidationName	varchar	(300)
		,RegExp	varchar	(300),ValidationValue	varchar	(300),IsRegExp	bit	,DispalyOrder INt,HelpDescription	varchar(max) )

    INSERT INTO @Attributevales_ForUnion

	SELECT DISTINCT  h.PimAttributeFamilyId, h.FamilyCode,
		a.PimAttributeId,g.PimAttributeGroupId,a.AttributeTypeId,j.AttributeTypeName,a.AttributeCode,a.IsRequired,a.IsLocalizable,a.IsFilterable,
		a.AttributeName , CASE WHEN  j.AttributeTypeName IN ('Image','Audio','Video','File','GalleryImages')
		THEN dbo.FN_GetThumbnailMediaPath(i.AttributeValue,0) + '~'+i.AttributeValue  ELSE case when @IsCopy = 1 and d.Name= 'UniqueValue'and e.ValidationName = 'Yes' then '' else  i.AttributeValue end END  AttributeValue, i.PimAttributeValueId,
		b.PimAttributeDefaultValueId,b.AttributeDefaultValue,ISNULL(NULL,0) RowId,ISNULL(b.IsEditable,1) AS IsEditable
		,d.ControlName, d.Name ValidationName,e.ValidationName SubValidationName,e.RegExp,c.Name ValidationValue,CAST (CASE WHEN  e.RegExp IS NULL THEN  0 ELSE 1  END AS BIT )  IsRegExp ,a.DisplayOrder,a.HelpDescription
		
	FROM View_PimAttributeLocale a 
		INNER JOIN  @PimAttributeId ww ON (a.PimAttributeId = ww.PimAttributeId AND a.LocaleId = @LocaleId AND a.IsPersonalizable = 0)
		INNER JOIN  ZnodePimFamilyGroupMapper g ON (  ww.PimAttributeId = g.PimAttributeId  AND EXISTS (SELECT TOP 1 1 FROM @PIMfamilyIds ss WHERE ss.PimAttributefamilyid = g.PimAttributeFamilyId))
		LEFT JOIN   ZnodePimAttributeGroupMapper qq ON (ww.PimAttributeId = qq.PimAttributeId ) 
		INNER JOIN  View_PimAttributeFamilyLocale h ON (h.PimAttributeFamilyId = g.PimAttributeFamilyId  AND h.LocaleId = @LocaleId)
		left JOIN   ZnodeAttributeType j ON (a.AttributeTypeId = j.AttributeTypeId)
		INNER JOIN  View_PimDefaultValue b ON (a.PimAttributeId = b.PimAttributeId AND b.LocaleId = @LocaleId)
		left JOIN   ZnodePimAttributeValidation c ON (a.PimAttributeId = c.PimAttributeId) 
		left JOIN   ZnodeAttributeInputValidation d ON (c.InputValidationId=d.InputValidationId)
		left JOIN   ZnodeAttributeInputValidationRule e ON (c.InputValidationRuleId=e.InputValidationRuleId And e.InputValidationId= c.InputValidationId)  
		LEFT  JOIN  View_PimProductAttributeValueLocale i   ON (i.PimAttributeId = ww.PimAttributeId  AND i.LocaleId = @LocaleId  AND i.PimProductId = @PimProductId AND i.PimAttributeFamilyId = g.PimAttributeFamilyId )
	  

	  
	  INSERT INTO @Attributevales
	  SELECT * 
	  FROM @Attributevales_ForUnion
	  UNION ALL 
	  	SELECT DISTINCT  h.PimAttributeFamilyId, h.FamilyCode,
		a.PimAttributeId,g.PimAttributeGroupId,a.AttributeTypeId,j.AttributeTypeName,a.AttributeCode,a.IsRequired,a.IsLocalizable,a.IsFilterable,
		a.AttributeName , CASE WHEN  j.AttributeTypeName IN ('Image','Audio','Video','File','GalleryImages')
		THEN dbo.FN_GetThumbnailMediaPath(i.AttributeValue,0) + '~'+i.AttributeValue  ELSE case when @IsCopy = 1 and d.Name= 'UniqueValue'and c.Name = 'true' then '' else  i.AttributeValue end END  AttributeValue, i.PimAttributeValueId,
		b.PimAttributeDefaultValueId,b.AttributeDefaultValue,ISNULL(NULL,0) RowId,ISNULL(b.IsEditable,1) AS IsEditable
		,d.ControlName, d.Name ValidationName,e.ValidationName SubValidationName,e.RegExp,c.Name ValidationValue,CAST (CASE WHEN  e.RegExp IS NULL THEN  0 ELSE 1  END AS BIT )  IsRegExp ,a.DisplayOrder,a.HelpDescription
		
	FROM View_PimAttributeLocale a 
		INNER JOIN @PimAttributeId ww ON (a.PimAttributeId = ww.PimAttributeId AND a.LocaleId = @V_Locale AND a.IsPersonalizable = 0)
		Left JOIN ZnodePimFamilyGroupMapper g ON (  ww.PimAttributeId = g.PimAttributeId  AND EXISTS (SELECT TOP 1 1 FROM @PIMfamilyIds ss WHERE ss.PimAttributefamilyid = g.PimAttributeFamilyId))
		LEFT JOIN ZnodePimAttributeGroupMapper qq ON (ww.PimAttributeId = qq.PimAttributeId ) 
		LEFT JOIN View_PimAttributeFamilyLocale h ON (h.PimAttributeFamilyId = g.PimAttributeFamilyId  AND h.LocaleId = @V_Locale)
		left JOIN ZnodeAttributeType j ON (a.AttributeTypeId = j.AttributeTypeId)
		LEFT JOIN View_PimDefaultValue b ON (a.PimAttributeId = b.PimAttributeId AND b.LocaleId = @V_Locale)
		left JOIN ZnodePimAttributeValidation c ON (a.PimAttributeId = c.PimAttributeId) 
		left JOIN ZnodeAttributeInputValidation d ON (c.InputValidationId=d.InputValidationId)
		left JOIN ZnodeAttributeInputValidationRule e ON (c.InputValidationRuleId=e.InputValidationRuleId And e.InputValidationId= c.InputValidationId)  
		LEFT  JOIN View_PimProductAttributeValueLocale i   ON (i.PimAttributeId = ww.PimAttributeId  AND i.LocaleId = @V_Locale  AND i.PimProductId = @PimProductId AND i.PimAttributeFamilyId = h.PimAttributeFamilyId )
     WHERE NOT EXISTS (SELECT TOP 1 1 FROM @Attributevales_ForUnion sada WHERE sada.PimAttributeId =  a.PimAttributeId)



     SELECT  PimAttributeFamilyId	,FamilyCode	,PimAttributeId	,PimAttributeGroupId	,AttributeTypeId	,AttributeTypeName	,AttributeCode	,IsRequired	,IsLocalizable	,IsFilterable	
			,AttributeName,	AttributeValue	,PimAttributeValueId	,PimAttributeDefaultValueId,AttributeDefaultValue	,RowId	,IsEditable	,ControlName	,ValidationName	,SubValidationName	,RegExp	
			,ValidationValue,IsRegExp	,HelpDescription 
	 FROM @Attributevales a
	 

	 ORDER BY DispalyOrder,PimAttributeId


END TRY 
BEGIN CATCH 
SELECT 0 ID , CAST(0 AS BIT ) Status 
END CATCH
END