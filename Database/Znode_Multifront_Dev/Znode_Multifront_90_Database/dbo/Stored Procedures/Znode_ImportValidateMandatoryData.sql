CREATE PROCEDURE [dbo].[Znode_ImportValidateMandatoryData]
   (@TableName        VARCHAR(200),
    @SourceColumnName NVARCHAR(600),
    @CreateDateString NVARCHAR(300),
    @ValidationName   VARCHAR(100),
    @ControlName      VARCHAR(300),
    @ValidationValue  VARCHAR(300),
    @NewGUID          NVARCHAR(200),
    @ImportHeadId     INT           = 0)
AS

/* 
Summary: Validate Manditory data for import process , Also generate error log
Unit Testing:
EXEC Znode_ImportValidateMandatoryData

*/
     BEGIN
         BEGIN TRY
		 SET NOCOUNT ON
             DECLARE @SQLQuery NVARCHAR(MAX), @ImportHeadName NVARCHAR(100);
             SET @ImportHeadName = DBO.Fn_GetDefaultImportHead(@ImportHeadId);
             SET @SQLQuery = @TableName+' WHERE '+DBO.Fn_Trim(@SourceColumnName)+'= '''' OR '+@SourceColumnName+' Is Null ;';

             EXEC Znode_ImportGenerateErrorLog
                  @ImportHeadName = @ImportHeadName,
                  @QueryCriteria = @SQLQuery,
                  @SourceColumnName = @SourceColumnName,
                  @CreateDateString = @CreateDateString,
                  @ErrorCode = '8';
         END TRY
         BEGIN CATCH
				DECLARE @Status BIT ;
				SET @Status = 0;
				DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportValidateMandatoryData @TableName = '+@TableName+',@SourceColumnName='+@SourceColumnName+',@CreateDateString='+@CreateDateString+',@ValidationName='+@ValidationName+',@ControlName = '+@ControlName+',@ValidationValue='+@ValidationValue+',@NewGUID='+@NewGUID+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
				SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
				EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportValidateMandatoryData',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;