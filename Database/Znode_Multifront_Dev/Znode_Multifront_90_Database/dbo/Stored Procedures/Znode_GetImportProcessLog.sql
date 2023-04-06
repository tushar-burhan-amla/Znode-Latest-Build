CREATE PROCEDURE [dbo].[Znode_GetImportProcessLog]
( @WhereClause VARCHAR(max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT)
AS
  /*
    Summary : Get import process log details include rowwise errors in details.    	
	Unit Testing   
	begin tran 
	DECLARE @RowsCount INT;
    EXEC Znode_GetImportProcessLog @WhereClause = 'ImportProcessLogId = 1151',@Rows = 1000,@PageNo = 0,@Order_BY = '',@RowsCount = @RowsCount OUT;
	rollback tran
    SELECT @RowsCount;
    
  */
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_ImportLog TABLE(ImportLogId INT,ImportProcessLogId INT,RowNumber BIgInt,ColumnName NVARCHAR(1000),ColumnValue NVARCHAR(max),ErrorDescription NVARCHAR(max)
										,RowId INt , CountNo Int )  ;
             SET @SQL = ' 
							;with Cte_ErrorLog AS 
							(
								SELECT zil.ImportLogId, zil.ImportProcessLogId, ISNULL(zil.RowNumber, 0) [RowNumber], ISNULL(zil.ColumnName, '''') [ColumnName],
								ISNULL(zil.Data, '''') [ColumnValue], zm.MessageName + 
								CASE 
								WHEN zm.MessageCode IN (17)	AND Name in (''Pricing'') AND ISNULL(zil.ColumnName, '''') NOT like ''%Quantity%'' THEN +''  ''+ dbo.Fn_GetDefaultPriceRoundOff(isnull(DefaultErrorValue,''0000000.00'') - 1)
								WHEN zm.MessageCode IN (17)	AND Name in (''Pricing'') AND ISNULL(zil.ColumnName, '''') like ''%Quantity%''  THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'') -1) 
								WHEN zm.MessageCode IN (17)	AND Name in (''Inventory'')  THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'') - 1) 
								WHEN zm.MessageCode IN (41) AND Name in (''Pricing'') AND ISNULL(zil.ColumnName, '''') NOT like ''%Quantity%'' THEN +''  ''+ dbo.Fn_GetDefaultPriceRoundOff(isnull(DefaultErrorValue,''0000000.00'' ))
								WHEN zm.MessageCode IN (41) AND Name in (''Pricing'') AND ISNULL(zil.ColumnName, '''')  like ''%Quantity%'' THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'' ))
								WHEN zm.MessageCode IN (41) AND Name in (''Inventory'') THEN +''  ''+ dbo.Fn_GetDefaultInventoryRoundOff(isnull(DefaultErrorValue,''0000000.00'' ))
								WHEN zm.MessageCode IN (44) AND Name in (''Pricing'') THEN +''  ''+ isnull(DefaultErrorValue,''0000000.00'' )
								WHEN zm.MessageCode IN (129) AND Name NOT IN (''Product'') THEN +'' ''+ isnull(DefaultErrorValue,''0000000.00'')+''.''
						

								ELSE ''''END ''ErrorDescription'' ,zil.ModifiedDate,zil.GUID
								FROM ZnodeImportLog AS zil WITH (NOLOCK) INNER JOIN ZnodeMessage AS zm WITH (NOLOCK) ON zil.ErrorDescription = CONVERT(VARCHAR(50) , zm.MessageCode)
								INNER JOIN ZnodeImportProcessLog zipl WITH (NOLOCK) ON zil.ImportProcessLogId = zipl.ImportProcessLogId
								LEFT Outer JOIN ZnodeImportTemplate zit WITH (NOLOCK) ON zipl.ImportTemplateId = zit.ImportTemplateId
								LEFT Outer JOIN ZnodeImportHead zih WITH (NOLOCK) ON zit.ImportHeadId =zih.ImportHeadId 
								)
								,Cte_ErrorLogFilter As ( SELECT ImportLogId,ImportProcessLogId,[RowNumber],[ColumnName],[ColumnValue],[ErrorDescription],[ModifiedDate],GUID
												,'+dbo.Fn_GetPagingRowId(@Order_BY , 'RowNumber,ImportLogId')+',Count(*)Over() CountNo 
								 FROM Cte_ErrorLog
								 WHERE 1 = 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
							 )

							SELECT ImportLogId,ImportProcessLogId,RowNumber,ColumnName,ColumnValue,ErrorDescription,RowId, CountNo 
							FROM Cte_ErrorLogFilter '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

			 INSERT INTO @TBL_ImportLog (ImportLogId,ImportProcessLogId,RowNumber,ColumnName,ColumnValue,ErrorDescription,RowId, CountNo)
			 EXEC (@SQL)
				 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ImportLog), 0);
             SELECT ImportLogId,ImportProcessLogId,RowNumber,ColumnName,ColumnValue,ErrorDescription
			 FROM @TBL_ImportLog
			

		 END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportProcessLog @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetImportProcessLog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                    
         END CATCH;
     END;


