CREATE PROCEDURE [dbo].[Znode_GetPublishAttributeDetail]  
(  
	@AttributeCode VARCHAR(MAX),  
	@LocaleId      INT          = 0
)  
AS   
   /*  Summary :- This procedure is used in publish to get the personalies attribute validation while data insert by Uers  
     Unit Testing   
     EXEC [dbo].[Znode_GetPublishAttributeDetail] 'DisplayText,Personalizable',1  
 */  
BEGIN  
BEGIN TRY  
SET NOCOUNT ON;  
	DECLARE @TBL_AttributIds TABLE  
	(
		PimAttributeId INT,  
		AttributeCode  VARCHAR(600)  
	); 
	
	DECLARE @TBL_AttributeDetail TABLE  
	(
		PimAttributeId       INT,  
		ParentPimAttributeId INT,  
		AttributeTypeId      INT,  
		AttributeCode        VARCHAR(600),  
		IsRequired           BIT,  
		IsLocalizable        BIT,  
		IsFilterable         BIT,  
		IsSystemDefined      BIT,  
		IsConfigurable       BIT,  
		IsPersonalizable     BIT,  
		DisplayOrder         INT,  
		HelpDescription      VARCHAR(MAX),  
		IsCategory           BIT,  
		IsHidden             BIT,  
		CreatedDate          DATETIME,  
		ModifiedDate         DATETIME,  
		AttributeName        NVARCHAR(MAX),  
		AttributeTypeName    VARCHAR(MAX)  
	);  
	DECLARE @PimAttributeIds VARCHAR(MAX);  
  
	INSERT INTO @TBL_AttributIds(PimAttributeId,AttributeCode)  
	SELECT PimAttributeId,AttributeCode FROM ZnodePimAttribute ZPA WHERE EXISTS(SELECT TOP 1 1 FROM dbo.split(@AttributeCode, ',') SP WHERE Sp.Item = ZPA.AttributeCode);  
	SET @PimAttributeIds = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) FROM @TBL_AttributIds FOR XML PATH('')), 2, 4000);  
  
	INSERT INTO @TBL_AttributeDetail(PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,  
	IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName)  
	EXEC [dbo].[Znode_GetPimAttributesDetails]@PimAttributeIds,@LocaleId;  
  
	SELECT TBAD.PimAttributeId,ParentPimAttributeId,TBAD.AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable,  
	IsPersonalizable,TBAD.DisplayOrder,HelpDescription,IsCategory,IsHidden,TBAD.CreatedDate,TBAD.ModifiedDate,AttributeName,AttributeTypeName,ZAIV.ControlName,  
	ZAIV.Name AS ValidationName,ZAIVR.ValidationName AS SubValidationName,ZAIVR.RegExp,ZPAV.Name AS ValidationValue,  
	CAST(CASE WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp FROM @TBL_AttributeDetail TBAD  
	LEFT OUTER JOIN [dbo].ZnodePimAttributeValidation AS ZPAV ON(ZPAV.PimAttributeId = TBAD.PimAttributeId)  
	LEFT OUTER JOIN [dbo].ZnodeAttributeInputValidation AS ZAIV ON(ZPAV.InputValidationId = ZAIV.InputValidationId)  
	LEFT OUTER JOIN [dbo].ZnodeAttributeInputValidationRule AS ZAIVR ON(ZPAV.InputValidationRuleId = ZAIVR.InputValidationRuleId) WHERE IsCategory = 0 
	ORDER BY TBAD.DisplayOrder
          
END TRY  
BEGIN CATCH  
DECLARE @Status BIT ;  
	SET @Status = 0;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAttributeDetail @AttributeCode = '+@AttributeCode+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_GetPublishAttributeDetail',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
END CATCH;  
END;  