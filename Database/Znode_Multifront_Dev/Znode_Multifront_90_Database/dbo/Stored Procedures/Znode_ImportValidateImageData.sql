
CREATE  PROCEDURE [dbo].[Znode_ImportValidateImageData]  
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
	@ImportHeadId       INT           = 0)
AS

/* 
Summary: Validate Image type data , Also validate range of date if data validationrule is available
		 - also check for invalid image
Unit Testing:
EXEC Znode_ImportValidateDate

*/
     BEGIN
         BEGIN TRY
		
             DECLARE @SQLQuery NVARCHAR(MAX), @ImportHeadName NVARCHAR(100),@IsInvalidImage  Bit =0 ,@IsAllowMultiUpload bit  = 0 ;
		  -- Select @SourceColumnName, @TableName
		   IF NOT EXISTS ( SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '#InvalidImagesLog')
			 CREATE TABLE #InvalidImagesLog (RowNumber int , ImageName nVarchar(max), ColumnName NVARCHAR(600))
		   ELSE 
			 DROP TABLE #InvalidImagesLog
		
		  IF Exists (Select TOP 1 1 from ZnodeImportLog where ImportProcessLogId= @ImportProcessLogId AND ErrorDescription = '45' AND ColumnName = @SourceColumnName )
		  BEGIN
			 SET @IsInvalidImage  =1 
		  END
	
		  IF @IsInvalidImage  = 0 
		  Begin  
			 SET @ImportHeadName = DBO.Fn_GetDefaultImportHead(@ImportHeadId);
                BEGIN
				
				   SET @IsAllowMultiUpload = CASE WHEN @ValidationName = 'IsAllowMultiUpload' AND ISNULL(@ValidationValue , 'false') = 'false' THEN 0 ELSE 1 END 

				   --select  = CASE when  isnull(ZPAV.Name, 'false') = 'false' then 0 else 1 END from ZnodeAttributeInputValidation ZAIV INNER JOIN ZnodePimAttributeValidation ZPAV on 
				   --ZAIV.InputValidationId  =  ZPAV.InputValidationId where AttributeTypeId = 4  and ZAIV.Name = 'IsAllowMultiUpload' and ZPAV.PimAttributeId = @AttributeId 

				   ---Verify Image file is exists in media table or not 
				   	SET @SQLQuery = ' INSERT INTO #InvalidImagesLog (RowNumber, ImageName, ColumnName) 
					SELECT ROWNUMBER , (Select TOP 1 Item from dbo.split(' + @SourceColumnName + ','','')  SP WHERE NOT EXISTS 
					(Select ToP 1 1 FROM ZnodeMedia WHERE ltrim(rtrim(SP.Item) ) = ZnodeMedia.Filename)), ''' + @SourceColumnName + ''' as [ColumnName]  FROM ' + @TableName
					+ ' Where ISnull(' + @SourceColumnName +  ','''') <> '''''
					EXEC sys.sp_sqlexec @SQLQuery;


					-- Check Invalid Image 
					SET @SQLQuery = 'SELECT ''45 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], 
					ImageName AS  AttributeValue,RowNumber ,'''+@NewGUID+''',  '+@CreateDateString+' FROM #InvalidImagesLog Where ImageName IS NOT NULL'

					 
					INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, RowNumber, GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
					EXEC sys.sp_sqlexec @SQLQuery;
					-----------------------------
				
					--- Verify Image file is multiselect true or not 
					If @IsAllowMultiUpload = 0 
					BEGIN
						Delete from #InvalidImagesLog 
						SET @SQLQuery = ' INSERT INTO #InvalidImagesLog (RowNumber, ImageName, ColumnName) 
						SELECT ROWNUMBER ,  ' + @SourceColumnName + ',''' + @SourceColumnName + ''' as [ColumnName]  FROM ' + @TableName
						+ ' Where  ' + @SourceColumnName + ' like ''%,%'''
						EXEC sys.sp_sqlexec @SQLQuery;
						
						-- Check Invalid Image 
						SET @SQLQuery = 'SELECT ''45 '' ErrorDescription,'''+@SourceColumnName+''' as [ColumnName], 
						ImageName AS  AttributeValue,RowNumber ,'''+@NewGUID+''',  '+@CreateDateString+' FROM #InvalidImagesLog Where ImageName IS NOT NULL'

						INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, RowNumber, GUID,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ImportProcessLogId)
						EXEC sys.sp_sqlexec @SQLQuery;
					END
					----------------------------
					  -- Select 'Hack'
				
				SET @SQLQuery = ' UPDATE A SET A.' + @SourceColumnName +'  
					= SUBSTRING ((SELECT '',''+CAST(MediaId AS VARCHAR(50)) FROM ZnodeMedia ZM WHERE EXISTS
					(SELECT TOP 1 1 FROM dbo.split(' +@SourceColumnName+ ' ,'','') SP WHERE ltrim(rtrim(sp.Item)) = ZM.FileName )
					AND MediaId in (Select TOP 1 MediaId from ZnodeMedia ZMN where  ZM.FileName =  ZMN.FileName )
					 FOR XML PATH ('''') ) ,2,4000)   
					FROM ' +  @TableName  + '  A ' 
			
					EXEC sys.sp_sqlexec @SQLQuery;


			 END;
	    END
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportValidateImageData @TableName = '+@TableName+',@SourceColumnName='+@SourceColumnName+',@CreateDateString='+@CreateDateString+',@ValidationName='+@ValidationName+',@ControlName = '+@ControlName+',@ValidationValue='+@ValidationValue+',@NewGUID='+@NewGUID+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@DefaultLocaleId='+CAST(@DefaultLocaleId AS VARCHAR(50))+',@AttributeId='+CAST(@AttributeId AS VARCHAR(50))+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportValidateImageData',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;