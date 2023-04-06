CREATE PROCEDURE [dbo].[Znode_ImportCatalogCategoryHierarchyAssociation]
(
	@TableName NVARCHAR(100), 
	@Status BIT OUT, 
	@UserId INT, 
	@ImportProcessLogId INT, 
	@NewGUId NVARCHAR(200),
	@PimCatalogId INT
)
AS
BEGIN 
BEGIN TRAN A;
BEGIN TRY
SET NOCOUNT ON;

	DECLARE @GetDate datetime= dbo.Fn_GetDate()
	DECLARE @SSQL VARCHAR(1000)

	IF OBJECT_ID('tempdb..#Category') IS NOT NULL
		DROP TABLE #Category;

	Declare @FeatureValue VARCHAR(500)
	--If the CatalogCategoryHierarchyAssociationAutoCreate value is true, 1 or Yes then input parent code hierarchy will be generate thogh the import
	--If the CatalogCategoryHierarchyAssociationAutoCreate value is false, 0 or No then input parent code hierarchy is not present in the database then it will give an error log
	SELECT TOP 1 @FeatureValue = FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'CatalogCategoryHierarchyAssociationAutoCreate'

	--Getting existing categories
	SELECT A.PimCategoryId,B.CategoryValue CategoryCode 
	INTO #Category 
	FROM ZnodePimCategoryAttributeValue A 
	INNER JOIN ZnodePimCategoryAttributeValueLocale B ON A.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId 
	WHERE EXISTS(SELECT TOP 1 1 FROM ZnodePimAttribute X WHERE X.IsCategory =1 and X.AttributeCode = 'CategoryCode' and A.PimAttributeId = X.PimAttributeId )

	IF OBJECT_ID('tempdb..#HierarchyImportData') IS NOT NULL
		DROP TABLE #HierarchyImportData;

	CREATE TABLE #HierarchyImportData (RowNumber INT,ParentCode NVARCHAR(MAX),CategoryCode NVARCHAR(MAX), DisplayOrder VARCHAR(10),Action VARCHAR(100),GUID VARCHAR(100));

	SET @SSQL = 'Select RowNumber,LTRIM(RTRIM(ParentCode)), LTRIM(RTRIM(CategoryCode)), DisplayOrder, Action ,GUID FROM '+@TableName;
	INSERT INTO #HierarchyImportData( RowNumber,ParentCode, CategoryCode, DisplayOrder,Action ,GUID)
	EXEC sys.sp_sqlexec @SSQL;

	
	IF OBJECT_ID('tempdb..#SplitedCategoryHierarchy') IS NOT NULL
		DROP TABLE #SplitedCategoryHierarchy

	--Created table for category hierarchy split
	CREATE TABLE #SplitedCategoryHierarchy (PimCategoryHierarchyId INT, ParentPimCategoryHierarchyId INT
	,PimCatalogId INT, PimCategoryId INT,RowNumber INT ,PimCategoryCode VARCHAR(300),RowId INT,Action varchar(100),DisplayOrder VARCHAR(10), ParentCategoryId INT)

	DECLARE @RecordCount INT=(SELECT COUNT(1) FROM #HierarchyImportData) , @Cnt INT = 1 ,@FirstCategoryId  INT 
	WHILE @Cnt <= @RecordCount  
	BEGIN
		SET @FirstCategoryId = 0 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '122', 'ParentCode / CategoryCode', '', @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE ISNULL(ParentCode,'') = '' AND ISNULL(CategoryCode,'') = '' AND ii.RowNumber  = @Cnt 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '131', 'CategoryCode', CategoryCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE ISNULL(CategoryCode,'') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '16', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE (ii.DisplayOrder > 99999 OR ii.DisplayOrder <= 0) AND ISNULL(ii.DisplayOrder,'') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '115', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE ISNUMERIC(ii.DisplayOrder)=0 AND ISNULL(ii.DisplayOrder,'') <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '125', 'Action', Action, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE isnull(Action,'') NOT IN ('ADD','DELETE')
		
		DELETE FROM #HierarchyImportData WHERE RowNumber IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )
		
		IF OBJECT_ID('tempdb..#ExistingHierarchy') IS NOT NULL
			DROP TABLE #ExistingHierarchy;
	
		TRUNCATE TABLE #SplitedCategoryHierarchy

		--Splitting category hierarchy and inserted into temp table #SplitedCategoryHierarchy
		INSERT INTO #SplitedCategoryHierarchy (PimCatalogId , PimCategoryCode ,RowNumber,RowId,Action,DisplayOrder)
		SELECT @PimCatalogId, LTRIM(RTRIM(B.item)) as PimCategoyHierarchyCode , Id, RowNumber, Action, CASE WHEN ISNULL(DisplayOrder,'') = '' THEN 0 ELSE DisplayOrder END
		FROM #HierarchyImportData A 
		CROSS APPLY  dbo.split(isnull(A.ParentCode,'')+CASE WHEN isnull(A.ParentCode,'') = '' THEN '' ELSE '/' END+A.CategoryCode,'/') B 
		Where A.RowNumber  = @Cnt  

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '127', 'ParentCode / CategoryCode', PimCategoryCode, @NewGUId, RowId, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #SplitedCategoryHierarchy AS ii
		WHERE NOT EXISTS(SELECT * FROM #Category C WHERE LTRIM(RTRIM(ii.PimCategoryCode)) = c.CategoryCode)

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '123', 'ParentCode / CategoryCode', PimCategoryCode, @NewGUId, RowId, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #SplitedCategoryHierarchy AS ii
		GROUP BY PimCategoryCode ,RowId
		HAVING COUNT(*) > 1

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '124', 'ParentCode', ParentCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE ISNULL(replace(ii.ParentCode,' ',''),'') like '%//%'
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '128', 'CategoryCode', CategoryCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #HierarchyImportData AS ii
		WHERE ISNULL(replace(LTRIM(RTRIM(ii.CategoryCode)),' ',''),'') like '%[^a-zA-Z0-9]%'

		--Deleting the records which was invalid and added into error log
		DELETE FROM #SplitedCategoryHierarchy WHERE RowId IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )
		DELETE FROM #HierarchyImportData WHERE RowNumber IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )

		IF OBJECT_ID('tempdb..#CategoryCodeCheck') IS NOT NULL
			DROP TABLE #CategoryCodeCheck;

		IF EXISTS(SELECT * FROM #SplitedCategoryHierarchy)
		BEGIN
			Update A SET A.PimCategoryId = D.PimCategoryId FROM #SplitedCategoryHierarchy A INNER JOIN #Category D ON LTRIM(RTRIM(A.PimCategoryCode)) = LTRIM(RTRIM(D.CategoryCode))

			Select TOP 1 @FirstCategoryId = X.PimCategoryId FROM #SplitedCategoryHierarchy X
	
			--Getting existing category hierarchy
			;WITH CTE_ZnodePimCategoryHierarchy as 
			(
				SELECT PimCategoryHierarchyId,ParentPimCategoryHierarchyId,PimCategoryId,PimCatalogId
				FROM ZnodePimCategoryHierarchy 
				WHERE PimCatalogId = @PimCatalogId 
			)
			,Hierarchy AS
			(
				-- Anchor
				SELECT  A.PimCategoryHierarchyId,A.ParentPimCategoryHierarchyId,A.PimCategoryId,A.PimCatalogId --, dense_rank() over(order by a.ParentPimCategoryHierarchyId) as RowNumber
				FROM CTE_ZnodePimCategoryHierarchy A
				WHERE A.ParentPimCategoryHierarchyId IS NULL AND  
				A.PimCategoryId =@FirstCategoryId 	AND A.PimCatalogId = @PimCatalogId
				UNION ALL
				-- Recursive query
				SELECT  E.PimCategoryHierarchyId,E.ParentPimCategoryHierarchyId,E.PimCategoryId,E.PimCatalogId--, ROW_NUMBER() over(order by E.ParentPimCategoryHierarchyId,E.PimCategoryHierarchyId) as RowNumber
				FROM CTE_ZnodePimCategoryHierarchy E
				JOIN Hierarchy H ON E.ParentPimCategoryHierarchyId = H.PimCategoryHierarchyId
				WHERE  Exists(Select * FROM #SplitedCategoryHierarchy X WHERE E.PimCategoryId = X.PimCategoryId)
			)
			SELECT * INTO #ExistingHierarchy FROM Hierarchy --Where RowNumber  = @Cnt  --
			--order by PimCategoryHierarchyId

			--Adding required columns into temp table
			Alter table #ExistingHierarchy Add Id INT Identity(1,1), RowNumber INT, ParentCategoryId INT

			--Updating the row number for parent category
			UPDATE #ExistingHierarchy SET RowNumber = 1 WHERE ParentPimCategoryHierarchyId IS NULL

			DECLARE @RecordCount1 INT=(SELECT COUNT(1) FROM #ExistingHierarchy) , @Cnt1 INT = 1 --,@FirstCategoryId1  INT 
			DECLARE @ParentPimCategoryHierarchyId1 INT , @RowNumber INT

			--Updating the row number according to category hierarhcy level
			WHILE @Cnt1 <= @RecordCount1  
			BEGIN
				
				SELECT @ParentPimCategoryHierarchyId1 = PimCategoryHierarchyId FROM #ExistingHierarchy WHERE Id = @Cnt1
				SET @RowNumber = (SELECT TOP 1 RowNumber FROM #ExistingHierarchy WHERE PimCategoryHierarchyId = @ParentPimCategoryHierarchyId1) 

				UPDATE #ExistingHierarchy SET RowNumber = @RowNumber + 1
				WHERE ParentPimCategoryHierarchyId = @ParentPimCategoryHierarchyId1
				
				SET @ParentPimCategoryHierarchyId1 = 0
				SET @RowNumber = 0
				SET @Cnt1 = @Cnt1+1
			END

			--Updating the parent category on existing category hierarchy
			UPDATE b SET ParentCategoryId = a.PimCategoryId
			from #ExistingHierarchy a
			inner join #ExistingHierarchy b on b.ParentPimCategoryHierarchyId = a.PimCategoryHierarchyId
			
			--Updating the parent category on input record
			UPDATE  A SET A.ParentCategoryId = B.PimCategoryId
			FROM #SplitedCategoryHierarchy A  
			INNER JOIN #SplitedCategoryHierarchy B ON B.RowNumber = A.RowNumber-1
						
			--Existing Hierarchy has been updated on the input categories
			Update B SET B.PimCategoryHierarchyId= A.PimCategoryHierarchyId, 
			B.ParentPimCategoryHierarchyId = A.ParentPimCategoryHierarchyId
			FROM #ExistingHierarchy  A INNER JOIN #SplitedCategoryHierarchy B ON ISNULL(A.PimCategoryId,0) = ISNULL(B.PimCategoryId,0) AND ISNULL(A.ParentCategoryId,0) = ISNULL(B.ParentCategoryId,0) and A.RowNumber = B.RowNumber 

			--Removing hierarchy ids of categoris if its parent dont have any hierarchy for input	
			UPDATE CH SET PimCategoryHierarchyId = NULL ,ParentPimCategoryHierarchyId = NULL
			FROM #SplitedCategoryHierarchy CH
			WHERE CH.PimCategoryHierarchyId IS NOT NULL AND EXISTS(SELECT * FROM #SplitedCategoryHierarchy CH1 WHERE CH1.PimCategoryHierarchyId IS NULL AND CH.RowNumber-1 = CH1.RowNumber)

			--Removing hierarchy ids of categories if its parent doesent present in the input hierarchy
			UPDATE A SET PimCategoryHierarchyId = NULL ,ParentPimCategoryHierarchyId = NULL 
			FROM #SplitedCategoryHierarchy A 
			WHERE NOT EXISTS(SELECT * FROM #SplitedCategoryHierarchy X WHERE X.PimCategoryHierarchyId = A.ParentPimCategoryHierarchyId)
			AND A.ParentPimCategoryHierarchyId IS NOT NULL 
			
			IF isnull(@FeatureValue,'') IN ('False','0','No','')
			BEGIN
				--If root level category is not present
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '130', 'ParentCode', ParentCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #HierarchyImportData h
				WHERE RowNumber IN (
					SELECT RowId FROM #SplitedCategoryHierarchy a
					WHERE NOT EXISTS(SELECT * FROM #HierarchyImportData b WHERE LTRIM(RTRIM(A.PimCategoryCode)) = LTRIM(RTRIM(b.CategoryCode)) AND a.RowId = b.RowNumber)
					AND A.ParentCategoryId IS NULL AND A.PimCategoryHierarchyId IS NULL)
				
				--If child level category is not present
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '130', 'ParentCode', ParentCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #HierarchyImportData H
				WHERE RowNumber IN (
					SELECT RowId FROM #SplitedCategoryHierarchy a
					WHERE NOT EXISTS(SELECT * FROM #HierarchyImportData b WHERE LTRIM(RTRIM(A.PimCategoryCode)) = LTRIM(RTRIM(b.CategoryCode)) AND a.RowId = b.RowNumber)
					AND A.ParentCategoryId IS NOT NULL AND A.ParentPimCategoryHierarchyId IS NULL)
				AND NOT EXISTS(SELECT * FROM ZnodeImportLog X WHERE H.RowNumber = X.RowNumber AND X.ImportProcessLogId=@ImportProcessLogId)
				
				--Deleting the records which was invalid and added into error log
				DELETE FROM #SplitedCategoryHierarchy WHERE RowId IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )
				DELETE FROM #HierarchyImportData WHERE RowNumber IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )
			END

			--Adding new hierarchy into ZnodePimCategoryHierarchy if input having Add action
			IF EXISTS(SELECT * FROM #SplitedCategoryHierarchy WHERE Action = 'Add')
			BEGIN
				
				DROP TABLE IF EXISTS #InsertedPimCategoryHierarchy

				CREATE TABLE #InsertedPimCategoryHierarchy (PimCategoryHierarchyId INT, PimCategoryId INT);

				--Inserting new category hierarchy if not present
				INSERT INTO ZnodePimCategoryHierarchy (PimCatalogId,ParentPimCategoryHierarchyId,PimCategoryId,DisplayOrder,IsActive,
					ActivationDate,ExpirationDate,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PimParentCategoryId)
				OUTPUT inserted.PimCategoryHierarchyId,inserted.PimCategoryId
				INTO #InsertedPimCategoryHierarchy (PimCategoryHierarchyId, PimCategoryId)
				Select PimCatalogId,ParentPimCategoryHierarchyId,PimCategoryId,DisplayOrder,1,NULL,NULL,2,@GetDate,2,@GetDate,NULL
				FROM #SplitedCategoryHierarchy WHERE PimCategoryHierarchyId is null 

				-- Newly created Hierarchy has been updated into input category hierarchy
				UPDATE A 
				SET A.PimCategoryHierarchyId=B.PimCategoryHierarchyId
				FROM #SplitedCategoryHierarchy A INNER JOIN #InsertedPimCategoryHierarchy B
					ON A.PimCategoryId=B.PimCategoryId
				WHERE A.PimCategoryHierarchyId IS NULL

				-- Updating new parent hierarchy has been updated into input category hierarchy
				UPDATE A SET A.ParentPimCategoryHierarchyId =(Select PimCategoryHierarchyId FROM #SplitedCategoryHierarchy X WHERE X.RowNumber = A.RowNumber -1 )
				FROM #SplitedCategoryHierarchy A 
		
				-- Updating new parent hierarchy has been updated newly added input category hierarchy into table ZnodePimCategoryHierarchy
				UPDATE ZnodePimCategoryHierarchy SET ParentPimCategoryHierarchyId = A.ParentPimCategoryHierarchyId
				FROM #SplitedCategoryHierarchy A WHERE ZnodePimCategoryHierarchy.PimCategoryHierarchyId = A.PimCategoryHierarchyId
				AND EXISTS(SELECT * FROM #InsertedPimCategoryHierarchy B WHERE A.PimCategoryHierarchyId = B.PimCategoryHierarchyId)
			END
			ELSE
			BEGIN
			
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '126', 'ParentCode / CategoryCode', ParentCode + case when isnull(ParentCode,'') <> '' then '/' else '' end + CategoryCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #HierarchyImportData AS ii
				WHERE ii.RowNumber = (SELECT TOP 1 RowId FROM #SplitedCategoryHierarchy where PimCategoryHierarchyId is null ORDER BY RowNumber DESC)

				DELETE FROM #SplitedCategoryHierarchy WHERE RowId IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )
				DELETE FROM #HierarchyImportData WHERE RowNumber IN (SELECT RowNumber FROM ZnodeImportLog WHERE ImportProcessLogId = @ImportProcessLogId )

				--Delete the category hierarchy and its child category 
				DECLARE @PimCategoryHierarchyId INT
				SET @PimCategoryHierarchyId = (SELECT TOP 1 PimCategoryHierarchyId FROM #SplitedCategoryHierarchy ORDER BY RowNumber DESC)
				EXECUTE [Znode_DeletePimCategoryHierarchy] @PimCategoryHierarchyId = @PimCategoryHierarchyId, @PimCatalogId = @PimCatalogId, @Status = 0
			END
		END

		SET @Cnt = @Cnt + 1 
	END

	DECLARE @FailedRecordCount BIGINT
	DECLARE @SuccessRecordCount BIGINT

	SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
	SELECT @SuccessRecordCount = COUNT(DISTINCT RowNumber) FROM #HierarchyImportData

	SET @GetDate = dbo.Fn_GetDate();
	--Updating the import process status
	UPDATE ZnodeImportProcessLog
	SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
						WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
						WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
					END, 
		ProcessCompletedDate = @GetDate
	WHERE ImportProcessLogId = @ImportProcessLogId;

	UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount , 
	TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
	WHERE ImportProcessLogId = @ImportProcessLogId;

COMMIT TRAN A;
END TRY
BEGIN CATCH
ROLLBACK TRAN A;

	SET @Status = 0;
	SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

	DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportCatalogCategoryHierarchyAssociation @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(200));


	---Import process updating fail due to database error
	UPDATE ZnodeImportProcessLog
	SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
	WHERE ImportProcessLogId = @ImportProcessLogId;

	---Loging error for Import process due to database error
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

	--Updating total and fail record count
	UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
	TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId)
	WHERE ImportProcessLogId = @ImportProcessLogId;

	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_ImportCatalogCategoryHierarchyAssociation',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
		
		
END CATCH;
END;