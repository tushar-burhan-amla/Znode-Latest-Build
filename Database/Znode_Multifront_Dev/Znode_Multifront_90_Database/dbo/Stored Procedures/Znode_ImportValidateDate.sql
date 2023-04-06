CREATE PROCEDURE [dbo].[Znode_ImportValidateDate]
   (@TableName        VARCHAR(200),
    @SourceColumnName NVARCHAR(600),
    @CreateDateString NVARCHAR(300),
    @ValidationName   VARCHAR(100),
    @ControlName      VARCHAR(300),
    @ValidationValue  VARCHAR(300),
    @NewGUID          NVARCHAR(200),
    @ImportHeadId     INT,
    @ImportProcessLogId INT )
AS
/* 
Summary: Validate Date type data , Also validate range of date if data validationrule is available
Unit Testing:
EXEC Znode_ImportValidateDate

*/
     BEGIN
         BEGIN TRY
             DECLARE @SQLQuery NVARCHAR(MAX), @ImportHeadName NVARCHAR(100),@IsDate  BIT = 0 ;
             SET @ImportHeadName = DBO.Fn_GetDefaultImportHead(@ImportHeadId);

		   IF Exists (Select TOP 1 1 from ZnodeImportLog where ImportProcessLogId= @ImportProcessLogId AND ErrorDescription = '5' AND ColumnName = @SourceColumnName )
		   BEGIN
			 SET @IsDate  =1 
		   END
		   
		   IF @IsDate = 0 
		   Begin
			  SET @SQLQuery = @TableName+' WHERE  IsDate('+@SourceColumnName+') = 0 and Isnull('+@SourceColumnName+','''') <> ''''';
			  EXEC Znode_ImportGenerateErrorLog
				  @ImportHeadName = @ImportHeadName,
				  @QueryCriteria = @SQLQuery,
				  @SourceColumnName = @SourceColumnName,
				  @CreateDateString = @CreateDateString,
				  @ErrorCode = '5';
		   END

             IF Exists (Select TOP 1 1 from ZnodeImportLog where ImportProcessLogId= @ImportProcessLogId AND ErrorDescription = '5' AND ColumnName = @SourceColumnName )
		   BEGIN
			 SET @IsDate  =1 
		   END

		   ----Remove wrong data from table 
             --SET @SQLQuery = 'DELETE FROM '+@TableName+' WHERE IsDate('+@SourceColumnName+') = 0 and Isnull('+@SourceColumnName+','''') <> ''''';
             --EXEC sys.sp_sqlexec
             -- @SQLQuery;
             IF @ControlName = 'Date'  AND @ValidationName IN('MaxDate', 'MinDate') AND ISNULL(@ValidationValue, '') <> '' AND @IsDate = 0 
                 BEGIN
                     SET @SQLQuery = @TableName+' where '+@SourceColumnName+CASE
                                                                                WHEN @ValidationName = 'MaxDate'
                                                                                THEN '>'+@ValidationValue
                                                                                ELSE '<'+@ValidationValue
                                                                            END;
                     IF @ValidationName = 'MaxDate'
                         EXEC Znode_ImportGenerateErrorLog
                              @ImportHeadName = @ImportHeadName,
                              @QueryCriteria = @SQLQuery,
                              @SourceColumnName = @SourceColumnName,
                              @CreateDateString = @CreateDateString,
                              @ErrorCode = '6',
                              @ValidationValue = @ValidationValue;
                     ELSE
					EXEC Znode_ImportGenerateErrorLog
						@ImportHeadName = @ImportHeadName,
						@QueryCriteria = @SQLQuery,
						@SourceColumnName = @SourceColumnName,
						@CreateDateString = @CreateDateString,
						@ErrorCode = '7',
						@ValidationValue = @ValidationValue;
                 
                 END;
             -- END
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportValidateDate @TableName = '+@TableName+',@SourceColumnName='+@SourceColumnName+',@CreateDateString='+@CreateDateString+',@ValidationName='+@ValidationName+',@ControlName = '+@ControlName+',@ValidationValue='+@ValidationValue+',@NewGUID='+@NewGUID+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportValidateDate',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;