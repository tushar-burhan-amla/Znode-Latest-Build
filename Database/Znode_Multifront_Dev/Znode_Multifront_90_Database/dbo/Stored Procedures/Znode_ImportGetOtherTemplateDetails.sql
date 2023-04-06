
CREATE  PROCEDURE [dbo].[Znode_ImportGetOtherTemplateDetails](
	  @TemplateId int,  
	  @ImportHeadId int = 0 ) 
AS 
/*
Summary: This Procedure is used to get template details
Unit Testing:
Exec Znode_ImportGetOtherTemplateDetails
*/
	  Begin 
	  BEGIN TRY
	  SET NOCOUNT ON
		IF  @TemplateId >0 and  @ImportHeadId > 0 
		BEGIN
				SELECT zitv.AttributeTypeName , zitv.AttributeCode , zitm.SourceColumnName , zitv.IsRequired , zitv.ControlName , zitv.ValidationName , zitv.SubValidationName , zitv.ValidationValue , zitv.RegExp
				FROM ZnodeImportTemplate AS zit INNER JOIN ZnodeImportAttributeValidation AS zitv ON zit.ImportHeadId = zitv.ImportHeadId
				LEFT OUTER JOIN ZnodeImportTemplateMapping AS zitm ON zit.ImportTemplateId = zitm.ImportTemplateId	AND
				zitv.AttributeCode = zitm.TargetColumnName WHERE zit.ImportHeadId = @ImportHeadId	AND	zit.ImportTemplateId = @TemplateId;
		END
		END TRY
		BEGIN CATCH

		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetOtherTemplateDetails @TemplateId = '+CAST(@TemplateId AS VARCHAR(max))+',@ImportHeadId='+CAST(@ImportHeadId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportGetOtherTemplateDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
		END