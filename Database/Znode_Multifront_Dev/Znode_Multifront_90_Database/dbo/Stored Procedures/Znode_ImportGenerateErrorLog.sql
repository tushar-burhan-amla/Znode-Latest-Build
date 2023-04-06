CREATE PROCEDURE [dbo].[Znode_ImportGenerateErrorLog]
(   @ImportHeadName   NVARCHAR(100),
    @QueryCriteria    NVARCHAR(MAX),
    @SourceColumnName NVARCHAR(600),
    @CreateDateString NVARCHAR(300),
    @ErrorCode        NVARCHAR(100),
    @ValidationValue  NVARCHAR(600) = '')
AS
     BEGIN
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             BEGIN
		
					SET @SQL = 'SELECT '+@ErrorCode+'  ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,
					GUID,RowNumber, '+@CreateDateString+','''+@ValidationValue+''' FROM '+@QueryCriteria;

					INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId, DefaultErrorValue )
					EXEC sys.sp_sqlexec @SQL;

	
             END;
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGenerateErrorLog @ImportHeadName = '+@ImportHeadName+',@QueryCriteria='+@QueryCriteria+',@SourceColumnName='+@SourceColumnName+',@CreateDateString='+@CreateDateString+',@ErrorCode = '+@ErrorCode+',@ValidationValue='+@ValidationValue+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportGenerateErrorLog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;