CREATE PROCEDURE [dbo].[Znode_GetUserGlobalAttributeValue]
(
    @EntityName NVARCHAR(200) = 0,
    @GlobalEntityValueId INT = 0,
	@LocaleCode VARCHAR(100) = '',
    @GroupCode NVARCHAR(200) = NULL,
	@SELECTedValue BIT = 0
)
AS
/*
	 Summary :- This procedure is used to get the Attribute and EntityValue attribute value as per filter pass 
	 Unit Testing 
	 BEGIN TRAN
	 EXEC [Znode_GetGlobalEntityAttributeValue] 'USER',1
	 ROLLBACK TRAN

*/	 
BEGIN
BEGIN TRY
SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @EntityValue NVARCHAR(200), @LocaleId INT

	DECLARE @GlobalFamilyId int
	SET @GlobalFamilyId = (SELECT FM.GlobalAttributeFamilyId FROM ZnodeGlobalEntity GE 
		INNER JOIN  ZnodeGlobalEntityFamilyMapper FM ON GE.GlobalEntityId = FM.GlobalEntityId
		WHERE GE.EntityName =  @EntityName and  (FM.GlobalEntityValueId = @GlobalEntityValueId or FM.GlobalEntityValueId IS NULL))
  
	DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);
	--Set the MediaServerThumbnailPath
	SET @V_MediaServerThumbnailPath =
	(
		SELECT ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL)+ZMSM.ThumbnailFolderName+'/'  
		FROM ZnodeMediaConfiguration ZMC 
		INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
		WHERE IsActive = 1 
	);

	--Getting user name
	SELECT @EntityValue=Isnull(FirstName,'')+' '+Isnull(LastName,'')
	FROM ZnodeUser
	WHERE UserId=@GlobalEntityValueId

	--Getting GlobalEntityId from @EntityName
	DECLARE @GlobalEntityId INT
	SET @GlobalEntityId =( SELECT GlobalEntityId FROM dbo.ZnodeGlobalEntity WHERE EntityName = @EntityName )

	CREATE TABLE #EntityAttributeList
	(
		GlobalEntityId INT,EntityName NVARCHAR(300),EntityValue NVARCHAR(MAX),GlobalAttributeGroupId INT,
		GlobalAttributeId INT,AttributeTypeId INT,AttributeTypeName NVARCHAR(300), AttributeCode NVARCHAR(300) ,
		IsRequired BIT,IsLocalizable BIT,AttributeName  NVARCHAR(300) , HelpDescription NVARCHAR(MAX),DisplayOrder INT
	) 
			 
	CREATE TABLE #EntityAttributeValidationList
	( 
		GlobalAttributeId INT, ControlName NVARCHAR(300), ValidationName NVARCHAR(300),SubValidationName NVARCHAR(300),
		RegExp NVARCHAR(300), ValidationValue NVARCHAR(300),IsRegExp Bit
	)

	CREATE TABLE #EntityAttributeValueList 
	(
		GlobalAttributeId INT,AttributeValue NVARCHAR(MAX),GlobalAttributeValueId INT,GlobalAttributeDefaultValueId INT,
		AttributeDefaultValueCode NVARCHAR(300),AttributeDefaultValue NVARCHAR(300),
		MediaId INT,MediaPath NVARCHAR(300),IsEditable BIT,DisplayOrder INT 
	)

	CREATE TABLE #EntityAttributeDefaultValueList 
	(	
		GlobalAttributeDefaultValueId INT,GlobalAttributeId INT,AttributeDefaultValueCode NVARCHAR(300),
		AttributeDefaultValue NVARCHAR(300),RowId INT,IsEditable BIT,DisplayOrder INT 
	)

	SET @LocaleId = (SELECT top 1 LocaleId FROM ZnodeLocale WHERE Code = @LocaleCode)
	--Getting user global attribute list
	INSERT INTO #EntityAttributeList
	(   
		GlobalEntityId ,EntityName ,EntityValue ,GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,
		AttributeTypeName ,AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder 
	) 
	SELECT @GlobalEntityId,@EntityName,@EntityValue EntityValue,ww.GlobalAttributeGroupId,
		c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
		c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
	FROM dbo.ZnodeGlobalAttributeGroupMapper AS ww 
	INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
	INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
	INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
	WHERE ( f.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
	AND EXISTS(SELECT * FROM dbo.ZnodeGlobalAttributeFamily AS GAF 
			INNER JOIN dbo.ZnodeGlobalEntity W ON GAF.GlobalEntityId = w.GlobalEntityId  
			INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = GAF.GlobalAttributeFamilyId
			WHERE GAF.GlobalAttributeFamilyId = @GlobalFamilyId AND w.GlobalEntityId = @GlobalEntityId AND ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId)
	AND EXISTS( SELECT 1 FROM ZnodeGlobalAttributeGroup g WHERE ww.GlobalAttributeGroupId = g.GlobalAttributeGroupId 
					AND (g.GroupCode = ISNULL(@GroupCode,'') OR ISNULL(@GroupCode,'') = '' ))

	--Getting globat attribute validation data
	IF EXISTS(SELECT * FROM #EntityAttributeList)
	BEGIN
		INSERT INTO #EntityAttributeValidationList(GlobalAttributeId,ControlName , ValidationName ,SubValidationName ,RegExp, ValidationValue,IsRegExp)
		SELECT aa.GlobalAttributeId,i.ControlName,i.Name AS ValidationName,j.ValidationName AS SubValidationName,
			j.RegExp,k.Name AS ValidationValue,CAST(CASE WHEN j.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp	
		FROM #EntityAttributeList aa
		INNER JOIN dbo.ZnodeGlobalAttributeValidation AS k ON k.GlobalAttributeId = aa.GlobalAttributeId
		INNER JOIN dbo.ZnodeAttributeInputValidation AS i ON k.InputValidationId = i.InputValidationId
		LEFT JOIN dbo.ZnodeAttributeInputValidationRule AS j ON k.InputValidationRuleId = j.InputValidationRuleId
	END

	---Getting globat attribute values
	INSERT INTO #EntityAttributeValueList(GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeValue ,MediaId,MediaPath)
	SELECT GlobalAttributeId,aa.UserGlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,
		CASE WHEN bb.MediaPath IS NOT NULL THEN @V_MediaServerThumbnailPath+bb.MediaPath--+'~'+convert(NVARCHAR(10),bb.MediaId) 
		ELSE bb.AttributeValue END AS AttributeValue,bb.MediaId,bb.MediaPath
	FROM  dbo.ZnodeUserGlobalAttributeValue aa
	INNER JOIN ZnodeUserGlobalAttributeValueLocale bb ON bb.UserGlobalAttributeValueId = aa.UserGlobalAttributeValueId 
	WHERE aa.UserId=@GlobalEntityValueId

	--Updating default globat attribute data into table #EntityAttributeValueList
	IF EXISTS(SELECT * FROM #EntityAttributeValueList)
	BEGIN
		UPDATE aa
		SET AttributeDefaultValueCode= h.AttributeDefaultValueCode,
			AttributeDefaultValue=g.AttributeDefaultValue,
			GlobalAttributeDefaultValueId=g.GlobalAttributeDefaultValueId,
			AttributeValue= CASE WHEN aa.AttributeValue IS NULL THEN h.AttributeDefaultValueCode ELSE aa.AttributeValue END, 
			IsEditable = ISNULL(h.IsEditable, 1),DisplayOrder = h.DisplayOrder
		FROM  #EntityAttributeValueList aa
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeDefaultValueId = aa.GlobalAttributeDefaultValueId                                       
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
    END

	--Getting globat attribute default values
	IF EXISTS(SELECT * FROM #EntityAttributeList)
	BEGIN
		INSERT INTO #EntityAttributeDefaultValueList(GlobalAttributeDefaultValueId,GlobalAttributeId,AttributeDefaultValueCode,AttributeDefaultValue ,RowId ,IsEditable ,DisplayOrder )
		SELECT  h.GlobalAttributeDefaultValueId, aa.GlobalAttributeId,h.AttributeDefaultValueCode,g.AttributeDefaultValue,0,ISNULL(h.IsEditable, 1),h.DisplayOrder
		FROM  #EntityAttributeList aa
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeId = aa.GlobalAttributeId
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
	END  

	IF NOT EXISTS (SELECT 1 FROM #EntityAttributeList )
	BEGIN
		INSERT INTO #EntityAttributeList
		(
			GlobalEntityId ,EntityName ,EntityValue ,GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,
			AttributeTypeName ,	AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription  
		) 
		SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,0 GlobalAttributeGroupId,
		0 GlobalAttributeId,0 AttributeTypeId,''AttributeTypeName,''AttributeCode,0 IsRequired,
		0 IsLocalizable,'' AttributeName,'' HelpDescription
		FROM dbo.ZnodeGlobalEntity AS qq
		WHERE qq.EntityName=@EntityName 
	END
				

	SELECT GlobalEntityId,EntityName,EntityValue,GlobalAttributeGroupId,AA.GlobalAttributeId,AttributeTypeId,AttributeTypeName,
		AttributeCode,IsRequired,IsLocalizable,AttributeName,ControlName,ValidationName,SubValidationName,RegExp,
		ValidationValue,cast(isnull(IsRegExp,0) as bit)  IsRegExp,HelpDescription,AttributeValue,GlobalAttributeValueId,
		bb.GlobalAttributeDefaultValueId,aab.AttributeDefaultValueCode,aab.AttributeDefaultValue,isnull(aab.RowId,0)   
		RowId,cast(isnull(aab.IsEditable,0) as bit)   IsEditable,bb.MediaId,AA.DisplayOrder
	FROM #EntityAttributeList AA				
	LEFT JOIN #EntityAttributeDefaultValueList aab on aab.GlobalAttributeId=AA.GlobalAttributeId	
	LEFT JOIN #EntityAttributeValidationList vl on vl.GlobalAttributeId=aa.GlobalAttributeId			
	LEFT JOIN #EntityAttributeValueList BB ON BB.GlobalAttributeId=AA.GlobalAttributeId		 
	AND ( (aab.GlobalAttributeDefaultValueId=bb.GlobalAttributeDefaultValueId	)
		OR  ( bb.MediaId IS NOT NULL AND ISNULL(vl.ValidationName,'')='IsAllowMultiUpload'  and bb.GlobalAttributeDefaultValueId IS NULL )
		OR  ( bb.MediaId IS NULL AND bb.GlobalAttributeDefaultValueId is null ))
	ORDER BY AA.DisplayOrder,aab.DisplayOrder

	SELECT 1 AS ID,CAST(1 AS BIT) AS Status;       
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE()
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
	@ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= null       			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		 
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetGlobalEntityValueAttributeValues',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END;
