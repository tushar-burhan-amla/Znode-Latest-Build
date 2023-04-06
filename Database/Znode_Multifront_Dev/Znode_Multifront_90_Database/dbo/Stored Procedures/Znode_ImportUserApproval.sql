CREATE PROCEDURE [dbo].[Znode_ImportUserApproval]
(
	  @TableName nvarchar(100), 
	  @Status bit OUT, 
	  @UserId int, 
	  @ImportProcessLogId int, 
	  @NewGUId nvarchar(200), 
	  @LocaleId int= 0,
	  @PortalId int ,
	  @CsvColumnString nvarchar(max)
	  )
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import SEO Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max),@AspNetZnodeUserId nvarchar(256),@ASPNetUsersId nvarchar(256),
		@PasswordHash nvarchar(max),@SecurityStamp nvarchar(max),@RoleId nvarchar(256),@IsAllowGlobalLevelUserCreation nvarchar(10)
		Declare @ProfileId  int 
	
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive RoundOff Value from global setting 

		-- Three type of import required three table varible for product , category and brand
		DECLARE @InsertZnodeUserApprovers TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int,UserName nvarchar(512),LevelCode VARCHAR(200) ,ApproverUserName nvarchar(512),ApproverOrder int ,
			IsNotifyEmail  bit ,IsMandatory bit,Custom1 nvarchar(max) ,Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4 nvarchar(max),Custom5 nvarchar(max),
			FromBudgetAmount Numeric(28,6),ToBudgetAmount Numeric(28,6) ,IsNoLimit bit 			, GUID NVARCHAR(400)
		);


		SET @SSQL = 'SELECT RowNumber,' + @CsvColumnString + ',GUID FROM '+ @TableName;
		INSERT INTO @InsertZnodeUserApprovers( RowNumber ,UserName ,LevelCode  ,ApproverUserName ,ApproverOrder  ,
			IsNotifyEmail   ,IsMandatory ,Custom1  ,Custom2 ,Custom3 ,Custom4 ,Custom5 ,
			FromBudgetAmount ,ToBudgetAmount ,IsNoLimit  			, GUID )
		EXEC sys.sp_sqlexec @SSQL;
		
		-- start Functional Validation 

		-----------------------------------------------
		If @IsAllowGlobalLevelUserCreation = 'true'
			BEGIN 
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
					   SELECT '14', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
					   FROM @InsertZnodeUserApprovers AS ii
					   WHERE ii.UserName not in 
					   (
						   SELECT UserName FROM AspNetZnodeUser   where PortalId = @PortalId
					   );
		 END 
		Else 
		BEGIN 
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
					   SELECT '14', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
					   FROM @InsertZnodeUserApprovers AS ii
					   WHERE ii.UserName not in 
					   (
						   SELECT UserName FROM AspNetZnodeUser   
					   );
         END 
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
					   SELECT '14', 'ApproverUserName', ApproverUserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
					   FROM @InsertZnodeUserApprovers AS ii
					   WHERE ii.ApproverUserName not in 
					   (
						   SELECT UserName FROM AspNetZnodeUser   
					   );
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
					   SELECT '14', 'LevelCode', LevelCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
					   FROM @InsertZnodeUserApprovers AS ii
					   WHERE ii.LevelCode not in 
					   (
						   SELECT LevelCode FROM ZnodeApproverLevel   
					   );

			SELECT * FROM ZnodeMessage

		
		--Note : Content page import is not required 
		
		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  
		
		DELETE FROM @InsertZnodeUserApprovers
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		
		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM @InsertZnodeUserApprovers
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
			TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End
		SELECT DISTINCT UserId , m.UserName 
		INTO #ZnodeUser
		FROM AspNetZnodeUser m 
		INNER JOIN AspNetUsers a ON (a.UserName = m.AspNetZnodeUserId) 
		INNER JOIN ZnodeUser b ON (b.AspNetUserId  = a.id)	
		WHERE EXISTS (SELECT TOP 1 1 FROM @InsertZnodeUserApprovers Y WHERE Y.UserName = M.UserName )
			 OR EXISTS (SELECT TOP 1 1 FROM @InsertZnodeUserApprovers Y WHERE Y.ApproverUserName = M.UserName )

		UPDATE a 
		SET a.UserName = (SELECT TOP 1 UserId FROM  #ZnodeUser m WHERE m.UserName = a.UserName )
		, a.ApproverUserName = (SELECT TOP 1 UserId FROM  #ZnodeUser m WHERE m.UserName = a.ApproverUserName )
		, a.LevelCode = (SELECT TOP 1 ApproverLevelId FROM ZnodeApproverLevel m WHERE m.LevelCode = a.LevelCode)
		FROM @InsertZnodeUserApprovers a 
	  
	    UPDATE  ZUA
		SET  ApproverOrder = IZUA.ApproverOrder
		,IsNotifyEmail = IZUA.IsNotifyEmail
		,IsMandatory = IZUA.IsMandatory
		,Custom1 = IZUA.Custom1
		,Custom2 = IZUA.Custom2
		,Custom3 = IZUA.Custom3
		,Custom4 = IZUA.Custom4
		,Custom5 = IZUA.Custom5 
		,ModifiedBy= @UserId
		,ModifiedDate = @GetDate
		,FromBudgetAmount = IZUA.FromBudgetAmount
		,ToBudgetAmount = IZUA.ToBudgetAmount
		,IsNoLimit = IZUA.IsNoLimit
		FROM ZnodeUserApprovers ZUA
		INNER JOIN @InsertZnodeUserApprovers IZUA ON (IZUA.Username = ZUA.UserId AND IZUA.ApproverUsername = ZUA.ApproverUserId AND IZUA.LevelCode = ZUA.ApproverLevelId)
		-- Insert Product Data 
		INSERT INTO ZnodeUserApprovers
				(UserId,ApproverLevelId,ApproverUserId,ApproverOrder,IsNotifyEmail,IsMandatory,Custom1,Custom2,Custom3,Custom4,Custom5
					,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,FromBudgetAmount,ToBudgetAmount,IsNoLimit)
		SELECT Username,LevelCode,ApproverUsername,ApproverOrder,IsNotifyEmail,IsMandatory,Custom1,Custom2,Custom3,Custom4,Custom5
					,@UserId,@GetDate,@UserId,@GetDate,FromBudgetAmount,ToBudgetAmount,IsNoLimit
		FROM @InsertZnodeUserApprovers
				
		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN A;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportUserApproval @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(max)) +',@PortalId='+CAST(@PortalId AS VARCHAR(max)) +',@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max)) ;


		---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
		TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportUserApproval',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH;
END;