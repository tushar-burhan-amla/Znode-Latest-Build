
CREATE PROCEDURE dbo.Znode_ImportValidatePimProductPriceData(
      @TableName  VARCHAR(200) ,
      @NewGUID    NVARCHAR(200) ,
      @TemplateId NVARCHAR(200) ,
      @UserId     INT ,
      @LocaleId   INT)
AS

/*
    Summary :  Import Pim Product Price Data 
    Process :  Admin site will upload excel / csv file in database and create global temporary table
			Procedure Znode_ImportValidatePimProductPriceData will validate data with Price validation rule
			If datatype validation issue found in input data will logged into table "ZnodeImportLog"
			If Data is correct and record count in table ZnodeImportLog will be 0 then process for import data into Base tables
			To import data call procedure "Znode_ImportPriceList_Ver1"
    		  
    SourceColumnName : CSV file column headers
    TargetColumnName : Attributecode from ZnodeImportAttributeValidation Table (Consider those Attributecodes configured with default family only)
    		 
*/
     BEGIN
         BEGIN TRY
             DECLARE @SQLQuery NVARCHAR(MAX) , @AttributeTypeName NVARCHAR(10) , @AttributeCode NVARCHAR(300) , @AttributeId INT , @IsRequired BIT , @SourceColumnName NVARCHAR(600) , @ControlName VARCHAR(300) , @ValidationName VARCHAR(100) , @SubValidationName VARCHAR(300) , @ValidationValue VARCHAR(300) , @RegExp VARCHAR(300) , @CreateDateString NVARCHAR(300) , @DefaultLocaleId INT , @ImportHeadId INT , @ImportProcessLogId INT= 0 , @Status BIT= 0 , @CheckedSourceColumn NVARCHAR(600)= '';
             SET @DefaultLocaleId = dbo.Fn_GetDefaultLocaleId();
             SELECT TOP 1 @ImportHeadId = ImportHeadId FROM ZnodeImportTemplate WHERE ImportTemplateId = @TemplateId;

             --Remove old error log 
             --DELETE FROM ZnodeImportPricingLog
             --WHERE ImportProcessLogId IN ( SELECT ImportProcessLogId
             --                              FROM ZnodeImportProcessLog
             --                              WHERE ImportTemplateId = @TemplateId
             --                            );
             ----GUID = @NewGUID;
             --DELETE FROM ZnodeImportProcessLog
             --WHERE ImportTemplateId = @TemplateId; 

             --Generate new process for current import 
             INSERT INTO ZnodeImportProcessLog ( ImportTemplateId , Status , ProcessStartedDate , ProcessCompletedDate , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate)
                    SELECT @TemplateId , dbo.Fn_GetImportStatus (0) , GETUTCDATE() , NULL , @UserId , GETUTCDATE() , @UserId , GETUTCDATE();
             SET @ImportProcessLogId = @@IDENTITY;
             SET @CreateDateString = CONVERT(VARCHAR(100) , @UserId)+','''+CONVERT(VARCHAR(100) , GETUTCDATE())+''','+CONVERT(VARCHAR(100) , @UserId)+','''+CONVERT(VARCHAR(100) , GETUTCDATE())+''', '+CONVERT(VARCHAR(100) , @ImportProcessLogId);

             ---- if Exists (SELECT top 1 1 FROM @FamilyAttributeDetail where  iSNULL(SourceColumnName,'') <> ''  and IsRequired = 1)
             ---- RAISERROR (0,-1,-1, 'Required Manditory Columns');     
             ---- Read all attribute details with their datatype 
           
                      Declare @AttributeDetail TABLE (
                                  PimAttributeId    INT ,
                                  AttributeTypeName VARCHAR(300) ,
                                  AttributeCode     VARCHAR(300) ,
                                  SourceColumnName  NVARCHAR(600) ,
                                  IsRequired        BIT ,
                                  ControlName       VARCHAR(300) ,
                                  ValidationName    VARCHAR(1000) ,
                                  SubValidationName VARCHAR(300) ,
                                  ValidationValue   VARCHAR(300) ,
                                  RegExp            VARCHAR(300)
                                                   );
                     INSERT INTO @AttributeDetail ( AttributeTypeName , AttributeCode , SourceColumnName , IsRequired , ControlName , ValidationName , SubValidationName , ValidationValue , RegExp
                                                  )
                            SELECT zitv.AttributeTypeName , zitv.AttributeCode , zitm.SourceColumnName , zitv.IsRequired , zitv.ControlName , zitv.ValidationName , zitv.SubValidationName , zitv.ValidationValue , zitv.RegExp
                            FROM ZnodeImportTemplate AS zit INNER JOIN ZnodeImportAttributeValidation AS zitv ON zit.ImportHeadId = zitv.ImportHeadId
                                                            LEFT OUTER JOIN ZnodeImportTemplateMapping AS zitm ON zit.ImportTemplateId = zitm.ImportTemplateId
                                                                                                                  AND
                                                                                                                  zitv.AttributeCode = zitm.TargetColumnName
                            WHERE zit.ImportHeadId = @ImportHeadId
                                  AND
                                  zit.ImportTemplateId = @TemplateId;
             
     
             --Check attributes are not mapped with any family of Pim Product
             INSERT INTO ZnodeImportLog ( ErrorDescription , ColumnName , Data , GUID , CreatedBy , CreatedDate , ModifiedBy , ModifiedDate , ImportProcessLogId
                                        )
                    SELECT DISTINCT
                           '14' AS ErrorDescription , AttributeCode , '' , @NewGUID , @UserId , GETUTCDATE() , @UserId , GETUTCDATE() , @ImportProcessLogId
                    FROM @AttributeDetail
                    WHERE ISNULL(SourceColumnName , '') = '';
   
             --Verify data in global temporary table (column wise)
             DECLARE Cr_Attribute CURSOR LOCAL FAST_FORWARD
             FOR SELECT PimAttributeId , AttributeTypeName , AttributeCode , IsRequired , SourceColumnName , ControlName , ValidationName , SubValidationName , ValidationValue , RegExp
                 FROM @AttributeDetail WHERE SourceColumnName  <> '';
             OPEN Cr_Attribute;
             FETCH NEXT FROM Cr_Attribute INTO @AttributeId , @AttributeTypeName , @AttributeCode , @IsRequired , @SourceColumnName , @ControlName , @ValidationName , @SubValidationName , @ValidationValue , @RegExp;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                     IF @AttributeTypeName = 'Number'
                         BEGIN
		  			    EXEC Znode_ImportValidateNumber @TableName = @TableName , @SourceColumnName = @SourceColumnName , @CreateDateString = @CreateDateString , @ValidationName = @ValidationName , @ControlName = @ControlName , @ValidationValue = @ValidationValue , @NewGUID = @NewGUID , @ImportHeadId = @ImportHeadId;
                         END;
                     -- check invalid date
                     IF @AttributeTypeName = 'Date'
                         BEGIN
                             EXEC Znode_ImportValidateDate @TableName = @TableName , @SourceColumnName = @SourceColumnName , @CreateDateString = @CreateDateString , @ValidationName = @ValidationName , @ControlName = @ControlName , @ValidationValue = @ValidationValue , @NewGUID = @NewGUID , @ImportHeadId = @ImportHeadId;
                         END;
                     -- Check Manditory Data
                     IF @IsRequired = 1
                        AND
                        @CheckedSourceColumn <> @SourceColumnName
                         BEGIN
					    SET @CheckedSourceColumn = @SourceColumnName;
                             EXEC Znode_ImportValidateManditoryData @TableName = @TableName , @SourceColumnName = @SourceColumnName , @CreateDateString = @CreateDateString , @ValidationName = @ValidationName , @ControlName = @ControlName , @ValidationValue = @ValidationValue , @NewGUID = @NewGUID , @ImportHeadId = @ImportHeadId;
                         END;
                     --IF @AttributeTypeName = 'Text'
                     --BEGIN
                     --EXEC Znode_ImportValidateManditoryText @TableName = @TableName , @SourceColumnName =@SourceColumnName, @CreateDateString =@CreateDateString , 
                     --@ValidationName = @ValidationName, @ControlName =@ControlName ,@ValidationValue =@ValidationValue  ,@NewGUID =@NewGUID ,@LocaleId =@LocaleId  ,
                     --@DefaultLocaleId =@DefaultLocaleId  ,@AttributeId =@AttributeId 
                     --END;
                     FETCH NEXT FROM Cr_Attribute INTO @AttributeId , @AttributeTypeName , @AttributeCode , @IsRequired , @SourceColumnName , @ControlName , @ValidationName , @SubValidationName , @ValidationValue , @RegExp;
                 END;
             CLOSE Cr_Attribute;
             DEALLOCATE Cr_Attribute;
             --- Finally call product insert process if error not found in error log table 
		  IF NOT EXISTS ( SELECT TOP 1 1
                        FROM ZnodeImportLog
                        WHERE ImportProcessLogId = @ImportProcessLogId
                    )
            BEGIN
	           EXEC [Znode_ImportPriceList] @TableName = @TableName , @Status = @Status , @UserId = @UserId , @ImportProcessLogId = @ImportProcessLogId , @NewGUID = @NewGUID;
			 EXEC Znode_ImportReadErrorLog @ImportHeadName = 'Pricing' , @ImportProcessLogId = @ImportProcessLogId , @NewGUID = @NewGUID;
		  END
		  ELSE 
		  BEGIN
			 EXEC Znode_ImportReadErrorLog @ImportHeadName = 'Product' , @ImportProcessLogId = @ImportProcessLogId , @NewGUID = @NewGUID;
		  END
	    
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE() , ERROR_LINE() , ERROR_PROCEDURE();
             EXEC Znode_ImportReadErrorLog @ImportHeadName = 'Pricing' , @ImportProcessLogId = @ImportProcessLogId , @NewGUID = @NewGUID;
         END CATCH;
     END;