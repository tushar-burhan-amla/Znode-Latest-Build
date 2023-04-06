CREATE PROCEDURE [dbo].[Znode_ImportValidateManditoryText]
(
	@TableName          VARCHAR(200),
	@SourceColumnName   NVARCHAR(600),
	@CreateDateString   NVARCHAR(300),
	@ValidationName     VARCHAR(100),
	@ControlName        VARCHAR(300),
	@ValidationValue    VARCHAR(300),
	@NewGUID            NVARCHAR(200),
	@LocaleId           INT = 1 ,
	@DefaultLocaleId    INT,
	@AttributeId        INT,
	@ImportProcessLogId INT,
	@ImportHeadId     INT           = 0,
	@IsIgnoreProcess   BIT = 0 
)
AS
     
 /*
	Summary:  Text 
             --------------------------
              Control   Validation Rule
             --------------------------
             1 Select	ValidationRule
             2 Text	RegularExpression
             3 Number	MaxCharacters
             4 Yes/No	UniqueValue

	Unit Testing:
	EXEC Znode_ImportValidateManditoryText
	 */
	 
	 BEGIN
        BEGIN TRY 
            SET NOCOUNT ON


             DECLARE @SQLQuery NVARCHAR(MAX), @ImportHeadName NVARCHAR(100);
             SET @ImportHeadName = DBO.Fn_GetDefaultImportHead(@ImportHeadId);
			 
			 IF @ControlName = 'Number' AND @ValidationName IN('MaxCharacters')  AND ISNULL(@ValidationValue, '') <> ''
                 BEGIN
                     SET @SQLQuery = @TableName+'  WHERE LEN('+@SourceColumnName+') > ' +   @ValidationValue + ' AND Isnull('+@SourceColumnName+','''') <> ''''';
                    

					IF @ValidationName = 'MaxCharacters'
                         EXEC Znode_ImportGenerateErrorLog
                              @ImportHeadName = @ImportHeadName,
                              @QueryCriteria = @SQLQuery,
                              @SourceColumnName = @SourceColumnName,
                              @CreateDateString = @CreateDateString,
                              @ErrorCode = '129',
                              @ValidationValue = @ValidationValue;
                  END;
             IF @ControlName = 'Yes/No' AND @ValidationName IN('UniqueValue') AND @ValidationValue = 'true'
                 BEGIN
					-- Check duplicate value exist in global temporary table 
					SET @SQLQuery = 'SELECT ''53'' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,RowNumber ,GUID,  '+@CreateDateString+' 
					FROM '+@TableName+'   WHERE RowNumber in (Select RowNumber from '+@TableName+' where '+@SourceColumnName+' in (Select '+@SourceColumnName+' from '+@TableName+' GROUP BY '+@SourceColumnName+' having COUNT(*) > 1) 
					)
					AND '+CAST(@IsIgnoreProcess AS varchar(100))+' = 0  
					';
                     
					INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, RowNumber, GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
					EXEC sys.sp_sqlexec @SQLQuery;

					---- Check duplicate value exist in znode database
					--If (@ImportHeadName in  ('Product'))
					--BEGIN
					-- SET @SQLQuery = 'SELECT ''10 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,tlb.RowNumber ,GUID,  '+@CreateDateString+' 
					--				  FROM ZnodePimAttributeValue AS ZPAV INNER JOIN ZnodePimAttributeValueLocale AS ZPAVL ON 
					--				  (ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId AND ZPAVL.LocaleId IN ('+CONVERT(VARCHAR(100), @LocaleId)+','+CONVERT(VARCHAR(100), @DefaultLocaleId)+')) 
					--				  INNER JOIN '+@TableName+' tlb ON ZPAVL.AttributeValue = tlb.'+@SourceColumnName+' 
					--				  WHERE ZPAV.PimAttributeId = '+CONVERT(VARCHAR(100), @AttributeId)+' AND ZPAVL.AttributeValue <> ''''';

					-- INSERT INTO ZnodeImportLog
					-- (ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
					-- EXEC sys.sp_sqlexec @SQLQuery;

					-- --Remove wrong data from table 
					-- --SET @SQLQuery = 'DELETE FROM '+@TableName+' WHERE RowNumber in (Select Isnull(RowNumber,0) FROM ZnodeImportLog 
					--	--			  WHERE ErrorDescription =10 AND ImportProcessLogId  = '+CONVERT(VARCHAR(100), @ImportProcessLogId)+');';
					-- --EXEC sys.sp_sqlexec @SQLQuery;
					--END

					--If (@ImportHeadName in  ('Category'))
					--BEGIN
				
					--	SET @SQLQuery = 'SELECT ''10 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,tlb.RowNumber ,GUID,  '+@CreateDateString+' 
					--	FROM ZnodePimCategoryAttributeValue AS ZPAV INNER JOIN ZnodePimCategoryAttributeValueLocale AS ZPAVL ON 
					--	(ZPAVL.PimCategoryAttributeValueId = ZPAV.PimCategoryAttributeValueId AND ZPAVL.LocaleId IN ('+CONVERT(VARCHAR(100), @LocaleId)+','+CONVERT(VARCHAR(100), @DefaultLocaleId)+')) 
					--	INNER JOIN '+@TableName+' tlb ON ZPAVL.CategoryValue = tlb.'+@SourceColumnName+' 
					--	WHERE ZPAV.PimAttributeId = '+CONVERT(VARCHAR(100), @AttributeId)+' AND ZPAVL.CategoryValue <> ''''';
					
					--	INSERT INTO ZnodeImportLog
					--	(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
					--	EXEC sys.sp_sqlexec @SQLQuery;
					
					-- END
					---- Check duplicate value exist in znode database
				
				     If (@ImportHeadName in  ('Category'))
					BEGIN
						SET @SQLQuery = 'SELECT ''79 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,tlb.RowNumber ,GUID,  '+@CreateDateString+' 
						FROM '+@TableName+' tlb
						WHERE ltrim(rtrim(isnull(tlb.CategoryCode,''''))) like ''% %'''
						;

						INSERT INTO ZnodeImportLog
						(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
						EXEC sys.sp_sqlexec @SQLQuery;

						SET @SQLQuery = 'SELECT ''79 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,tlb.RowNumber ,GUID,  '+@CreateDateString+' 
						FROM '+@TableName+' tlb
						WHERE ltrim(rtrim(isnull(tlb.CategoryCode,''''))) like ''%[^0-9A-Za-z]%'''
						;

						INSERT INTO ZnodeImportLog
						(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
						EXEC sys.sp_sqlexec @SQLQuery;

						SET @SQLQuery = 'SELECT ''79 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], '+@SourceColumnName+' AS  AttributeValue,tlb.RowNumber ,GUID,  '+@CreateDateString+' 
						FROM '+@TableName+' tlb
						WHERE ltrim(rtrim(isnull(tlb.CategoryCode,''''))) NOT LIKE ''%[^0-9]%'''
						;

						INSERT INTO ZnodeImportLog
						(ErrorDescription,ColumnName,Data,RowNumber,GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
						EXEC sys.sp_sqlexec @SQLQuery;

						
					END

                 END;
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
			 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportValidateManditoryText @TableName = '+@TableName+',@SourceColumnName='+@SourceColumnName+',@CreateDateString='+@CreateDateString+',@ValidationName='+@ValidationName+',@ControlName = '+@ControlName+',@ValidationValue='+@ValidationValue+',@NewGUID='+@NewGUID+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@DefaultLocaleId='+CAST(@DefaultLocaleId AS VARCHAR(50))+',@AttributeId='+CAST(@AttributeId AS VARCHAR(50))+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportValidateManditoryText',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;