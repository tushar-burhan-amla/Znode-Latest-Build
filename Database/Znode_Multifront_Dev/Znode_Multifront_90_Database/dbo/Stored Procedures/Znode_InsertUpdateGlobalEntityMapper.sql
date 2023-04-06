CREATE PROCEDURE [dbo].[Znode_InsertUpdateGlobalEntityMapper]  
(  
	@EntiryMapper XML,  
	@UserId INT,  
	@Action nvarchar(50),  
	@Status INT = 0 OUT    
)  
AS  
BEGIN  
 BEGIN TRY    
 BEGIN TRAN SaveOrUpdateEntityMapper;   
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate()
	DECLARE @GlobalEntityId INT;

	IF OBJECT_ID('tempdb..#EntityMapper') IS NOT NULL     
	DROP TABLE #EntityMapper  
    
	--Getting xml data into table    
	SELECT     
	Tbl.Col.value ('GlobalAttributeId[1]' , 'INT') AS AttributeFamilyId,   
	Tbl.Col.value ('GlobalAttributeGroupId[1]' , 'INT') AS AttributeGroupId,    
	Tbl.Col.value ('AttributeDisplayOrder[1]' , 'INT') AS GroupDisplayOrder  
	INTO #EntityMapper    
	FROM @EntiryMapper.nodes ( '//ArrayOfGlobalAttributeGroupMapperModel/GlobalAttributeGroupMapperModel'  ) AS Tbl(Col)   

	SELECT @GlobalEntityId=GlobalEntityId
	FROM ZnodeGlobalAttributeFamily
	WHERE GlobalAttributeFamilyId=(SELECT DISTINCT AttributeFamilyId FROM #EntityMapper);
  
	IF @action='Insert'  
	BEGIN  
		INSERT INTO ZnodeGlobalFamilyGroupMapper(GlobalAttributeFamilyId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)  
		SELECT AttributeFamilyId,AttributeGroupId,GroupDisplayOrder,@UserId,@GetDate,@UserId,@GetDate 
		FROM #EntityMapper hmapper
		LEFT JOIN ZnodeGlobalFamilyGroupMapper tmapper ON hmapper.AttributeFamilyId=tmapper.GlobalAttributeFamilyId AND hmapper.AttributeGroupId=tmapper.GlobalAttributeGroupId
		WHERE tmapper.GlobalAttributeFamilyId is null and tmapper.GlobalAttributeGroupId is null;
  
		INSERT INTO ZnodeGlobalGroupEntityMapper(GlobalEntityId,GlobalAttributeGroupId,AttributeGroupDisplayOrder,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)  
		SELECT @GlobalEntityId,AttributeGroupId,GroupDisplayOrder,@UserId,@GetDate,@UserId,@GetDate  
		FROM #EntityMapper;
		SET @Status = 1  
		SELECT 1 AS ID,CAST(@Status AS BIT) AS Status;     
	END  
	IF @action='Update'  
	BEGIN   
		UPDATE ZnodeGlobalFamilyGroupMapper SET AttributeGroupDisplayOrder=hmapper.GroupDisplayOrder,ModifiedBy=@UserId,ModifiedDate=@GetDate
		FROM ZnodeGlobalFamilyGroupMapper tmapper  
		INNER JOIN #EntityMapper hmapper on tmapper.GlobalAttributeFamilyId=hmapper.AttributeFamilyId AND tmapper.GlobalAttributeGroupId=hmapper.AttributeGroupId  
		--WHERE tmapper.GlobalAttributeFamilyId=hmapper.AttributeFamilyId AND tmapper.GlobalAttributeGroupId=hmapper.AttributeGroupId  
  
		UPDATE ZnodeGlobalGroupEntityMapper SET AttributeGroupDisplayOrder=hmapper.GroupDisplayOrder,ModifiedBy=@UserId,ModifiedDate=@GetDate FROM ZnodeGlobalGroupEntityMapper tmapper  
		INNER JOIN #EntityMapper hmapper on tmapper.GlobalEntityId=hmapper.AttributeFamilyId AND tmapper.GlobalAttributeGroupId=hmapper.AttributeGroupId  
		--WHERE tmapper.GlobalEntityId=hmapper.AttributeFamilyId AND tmapper.GlobalAttributeGroupId=hmapper.AttributeGroupId  
		SET @Status = 1  
		SELECT 1 AS ID,CAST(@Status AS BIT) AS Status;     
	END  
COMMIT TRAN SaveOrUpdateEntityMapper;  
END TRY    
BEGIN CATCH                      
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),    
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_SaveOrUpdateEntityMapper @entiryMapper = '+ cast(@entiryMapper as varchar(2000));    
	SET @Status = 0                  
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
	ROLLBACK TRAN SaveOrUpdateEntityMapper;       
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_SaveOrUpdateEntityMapper',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;    
END CATCH;    
  
END