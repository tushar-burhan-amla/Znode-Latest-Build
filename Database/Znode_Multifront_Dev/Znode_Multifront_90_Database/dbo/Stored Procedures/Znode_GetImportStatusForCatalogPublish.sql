
CREATE PROCEDURE [dbo].[Znode_GetImportStatusForCatalogPublish]
(
   @Status bit = 0 Out
)
AS 
--EXEC Znode_GetImportStatusForCatalogPublish
BEGIN

SET NOCOUNT ON
 BEGIN TRY
	 DECLARE @TemplateName  NVARCHAR(MAX) = N'Product,Pricing,Inventory,Category,ProductAttribute,CategoryAssociation,ProductAssociation,SEODetails,ProductUpdate,AddonAssociation,AttributeDefaultValue';
   
	DECLARE @TBL_Template TABLE (TemplateId INT);

	INSERT INTO @TBL_Template  
	SELECT ZIPL.ImportTemplateId FROM ZnodeImportHead ZIH INNER JOIN
	                            ZnodeImportTemplate ZIPL on ZIPL.ImportHeadId = ZIH.ImportHeadId
								WHERE ZIH.Name IN (SELECT CAST(value AS nvarchar) FROM STRING_SPLIT(@TemplateName,','))
	
	IF EXISTS (SELECT TOP 1 1 FROM ZnodeImportProcessLog ZIPL INNER JOIN 
	          @TBL_Template TBL ON ZIPL.ImportTemplateId = TBL.TemplateId
	          WHERE ZIPL.Status = dbo.Fn_GetImportStatus(0) OR ZIPL.Status = dbo.Fn_GetImportStatus(1))
	 BEGIN
	   SET @Status = 0;
	   SELECT 0 Id,@Status [Status]
	 END
	ELSE
	 BEGIN
	   SET @Status = 0;
	   SELECT 1 Id,@Status [Status]
	 END
 END TRY
 BEGIN CATCH
		     SET @Status = 0;
			 SELECT 2 Id,@Status [Status]

		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
			 @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			 @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetImportStatusForCatalogPublish';                   

             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetImportStatusForCatalogPublish',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
 END CATCH
END