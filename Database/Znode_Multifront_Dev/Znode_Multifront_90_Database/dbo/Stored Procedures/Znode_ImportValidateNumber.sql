CREATE PROCEDURE [dbo].[Znode_ImportValidateNumber]
(   @TableName        VARCHAR(200),
    @SourceColumnName NVARCHAR(600),
    @CreateDateString NVARCHAR(300),
    @ValidationName   VARCHAR(100),
    @ControlName      VARCHAR(300),
    @ValidationValue  VARCHAR(300),
    @NewGUID          NVARCHAR(200),
    @ImportHeadId     INT      = 0,
    @ImportProcessLogId int )
AS
/*
Summary: --First Validate numeric datatype then it check its functional validation such as Number ( MaxNo/ MinNo )  and Yes/No (-ve / Decimal Value )
             Number
             --------------------------------
              Control  Validation Rule
             --------------------------------
             1 Yes/No	AllowNegative
             2 Yes/No	AllowDecimals
             3 Number	MinNumber
             4 Number	MaxNumber

Unit Testing:
EXEC Znode_ImportValidateNumber
*/
     BEGIN
         BEGIN TRY 
            SET NOCOUNT ON
             DECLARE @SQLQuery NVARCHAR(MAX), @ImportHeadName NVARCHAR(100);
             SET @ImportHeadName = DBO.Fn_GetDefaultImportHead(@ImportHeadId);
             DECLARE @RoundOffValue INT,@IsNumeric bit = 0 ; 

             -- Retrive RoundOff Value from global setting 

			 IF @SourceColumnName in ('Quantity', 'ReOrderLevel','TierStartQuantity')
                 SELECT @RoundOffValue = dbo.[Fn_GetInventoryRoundOffValue]();
             ELSE
				 SELECT @RoundOffValue = dbo.[Fn_GetPriceRoundOffValue]();
             
		  -- IF Exists (Select TOP 1 1 from ZnodeImportLog where ImportProcessLogId= @ImportProcessLogId AND ErrorDescription = '2' AND ColumnName = @SourceColumnName)
		  -- BEGIN
			 --SET @IsNumeric  =1 
		  -- END

		   IF @IsNumeric  =0 
		   BEGIN
			  SET @SQLQuery = @TableName+' WHERE  Isnumeric('+@SourceColumnName+') = 0 and Isnull('+@SourceColumnName+','''') <> ''''
			  AND NOT EXISTS (Select TOP 1 1 from ZnodeImportLog where ImportProcessLogId= ' +  Convert(Varchar(100), @ImportProcessLogId  ) + ' AND ErrorDescription = ''2'' 
			  AND ColumnName = ''' + @SourceColumnName + ''') ';
             
			  EXEC Znode_ImportGenerateErrorLog
				  @ImportHeadName = @ImportHeadName,
				  @QueryCriteria = @SQLQuery,
				  @SourceColumnName = @SourceColumnName,
				  @CreateDateString = @CreateDateString,
				  @ErrorCode = '2'
		   END

		   IF @IsNumeric  = 0   
		   BEGIN
			  IF Exists (Select TOP 1 1 from ZnodeImportLog where ImportProcessLogId= @ImportProcessLogId AND ErrorDescription = '2' AND ColumnName = @SourceColumnName )
			  BEGIN
				SET @IsNumeric  =1 
			  END
		   END
             
		   IF @ControlName = 'Number'  AND @ValidationName IN('MaxNumber', 'MinNumber') AND ISNULL(@ValidationValue, '') <> '' AND ISNULL(@ValidationValue, '') > 0 AND @IsNumeric  = 0  
                 BEGIN
                     SET @SQLQuery = @TableName+'  WHERE Convert(money, '+@SourceColumnName+')'+CASE
                                                                                                    WHEN @ValidationName = 'MaxNumber'
                                                                                                    THEN '>'+@ValidationValue
                                                                                                    ELSE '<'+@ValidationValue
                                                                                                END+' AND Isnull('+@SourceColumnName+','''') <> ''''';
                     IF @ValidationName = 'MaxNumber'
                         EXEC Znode_ImportGenerateErrorLog
                              @ImportHeadName = @ImportHeadName,
                              @QueryCriteria = @SQLQuery,
                              @SourceColumnName = @SourceColumnName,
                              @CreateDateString = @CreateDateString,
                              @ErrorCode = '16',
                              @ValidationValue = @ValidationValue;
                     ELSE
					EXEC Znode_ImportGenerateErrorLog
						@ImportHeadName = @ImportHeadName,
						@QueryCriteria = @SQLQuery,
						@SourceColumnName = @SourceColumnName,
						@CreateDateString = @CreateDateString,
						@ErrorCode = '16',
						@ValidationValue = @ValidationValue;
		 
                     --Remove wrong data from table 
                     --SET @SQLQuery = 'DELETE FROM '+@TableName+'    WHERE Convert(money,'+@SourceColumnName+')'+CASE
                     --                                                                                               WHEN @ValidationName = 'MaxNumber'
                     --                                                                                               THEN '>'+@ValidationValue
                     --                                                                                               ELSE '<'+@ValidationValue
                     --                                                                                           END+' AND Isnull('+@SourceColumnName+','''') <> ''''';
                     --EXEC sys.sp_sqlexec
                     --     @SQLQuery;
                     -- END
                 END;
             -- 1
             IF @ControlName = 'Yes/No' AND @ValidationName IN('AllowNegative') AND @ValidationValue = 'false' AND @IsNumeric  = 0 
                 BEGIN
                     BEGIN
                         SET @SQLQuery = @TableName+' WHERE  Convert( Money,'+@SourceColumnName+' ) < 0  AND Isnull('+@SourceColumnName+', '''') <> ''''';
                         EXEC Znode_ImportGenerateErrorLog
                              @ImportHeadName = @ImportHeadName,
                              @QueryCriteria = @SQLQuery,
                              @SourceColumnName = @SourceColumnName,
                              @CreateDateString = @CreateDateString,
                              @ErrorCode = '4',
                              @ValidationValue = '' ;
			
                         --Remove wrong data from table 
                         --SET @SQLQuery = 'DELETE FROM '+@TableName+'    WHERE Convert( Money,'+@SourceColumnName+' ) < 0  AND Isnull('+@SourceColumnName+', '''') <> ''''';
                         --EXEC sys.sp_sqlexec  @SQLQuery;
                     END;
                 END;
             IF @ControlName = 'Number' AND @SourceColumnName <> 'RowNumber' AND @ImportHeadName in ( 'Inventory' ,'Pricing') AND @IsNumeric  = 0 
                 BEGIN
                     ---Validate roundoff value after decimal place should be between value which is define in global settings
                     SET @SQLQuery = @TableName+'  WHERE ( CASE WHEN '+@SourceColumnName+' LIKE ''%.%'' THEN LEN(SUBSTRING('+@SourceColumnName+' , CHARINDEX(''.'' , '+@SourceColumnName+')+1 , 4000)) ELSE 0 END > ( '+CONVERT(VARCHAR(100), @RoundOffValue)+') )'+'  AND Isnull('+@SourceColumnName+', '''') <> ''''';
                     EXEC Znode_ImportGenerateErrorLog
                          @ImportHeadName = @ImportHeadName,
                          @QueryCriteria = @SQLQuery,
                          @SourceColumnName = @SourceColumnName,
                          @CreateDateString = @CreateDateString,
                          @ErrorCode = '111',
                          @ValidationValue = '0.999999';
			
                     --Remove wrong data from table 
                     --SET @SQLQuery = 'DELETE FROM '+@TableName+'  WHERE ( CASE WHEN '+@SourceColumnName+' LIKE ''%.%'' THEN LEN(SUBSTRING('+@SourceColumnName+' , CHARINDEX(''.'' , '+@SourceColumnName+')+1 , 4000)) ELSE 0 END > ( '+CONVERT(VARCHAR(100), @RoundOffValue)+') )'+'  AND Isnull('+@SourceColumnName+', '''') <> ''''';
                     --EXEC sys.sp_sqlexec  @SQLQuery;
                 END;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXECZnode_ImportValidateNumber @TableName = '+@TableName+',@SourceColumnName='+@SourceColumnName+',@CreateDateString='+@CreateDateString+',@ValidationName='+@ValidationName+',@ControlName = '+@ControlName+',@ValidationValue='+@ValidationValue+',@NewGUID='+@NewGUID+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportValidateNumber',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;

