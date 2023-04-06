CREATE PROCEDURE [dbo].[Znode_GetPromotionAttributeValues]
(@DiscountTypeName Nvarchar(max))
AS
 /*  
     Summary : Get Promotion Attribute with their validation rule and control name 
    Unit Testing 
    Exec Znode_GetPromotionAttributeValues 'Amount Off Brand'
   
*/
     BEGIN
         BEGIN TRY
             DECLARE @TBL_Detailrecord TABLE
             (PromotionAttributeId INT,
              AttributeTypeId      INT,
              AttributeTypeName    VARCHAR(300),
              AttributeCode        VARCHAR(300),
              IsRequired           BIT,
              IsLocalizable        BIT,
              AttributeName        NVARCHAR(600),
              RowId                INT,
              ControlName          VARCHAR(300),
              ValidationName       VARCHAR(100),
              SubValidationName    VARCHAR(300),
              RegExp               VARCHAR(300),
              ValidationValue      VARCHAR(300),
			  HelpDescription	   VARCHAR(MAX),
              IsRegExp             BIT
             );
             -- temp table for temporary store value 

             INSERT INTO @TBL_Detailrecord (PromotionAttributeId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,IsLocalizable,AttributeName,RowId,ControlName,
			 ValidationName,SubValidationName,RegExp,ValidationValue,HelpDescription,IsRegExp)

             SELECT DISTINCT ZPA.PromotionAttributeId,ZPA.AttributeTypeId,ZAT.AttributeTypeName,ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,ZPA.AttributeName,
			 ISNULL(NULL, 0) AS RowId,ZAIV.ControlName,ZAIV.Name AS ValidationName,ZAIVR.ValidationName AS SubValidationName,ZAIVR.RegExp,ZPAV.Name AS ValidationValue,
			 ZPA.HelpDescription,CAST(CASE WHEN ZAIVR.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp FROM [dbo].[ZnodePromotionAttribute] AS ZPA
			 INNER JOIN [dbo].[ZnodePromotionDiscountAttributeMapper] ZPDAM ON(ZPDAM.PromotionAttributeId = ZPA.PromotionAttributeId)
			 LEFT OUTER JOIN [dbo].ZnodePromotionAttributeValidation AS ZPAV ON(ZPAV.PromotionAttributeId = ZPA.PromotionAttributeId)
			 LEFT OUTER JOIN [dbo].ZnodeAttributeType AS ZAT ON(ZPA.AttributeTypeId = ZAT.AttributeTypeId)
			 LEFT OUTER JOIN [dbo].ZnodeAttributeInputValidation AS ZAIV ON(ZPAV.InputValidationId = ZAIV.InputValidationId)
			 LEFT OUTER JOIN [dbo].ZnodeAttributeInputValidationRule AS ZAIVR ON(ZPAV.InputValidationRuleId = ZAIVR.InputValidationRuleId)
             WHERE  (ZPDAM.DiscountTypeName = @DiscountTypeName OR @DiscountTypeName = '')
		
             -- chnages for isconfigurable attribute 
             SELECT PromotionAttributeId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,IsLocalizable,AttributeName,RowId,ControlName,ValidationName,
             SubValidationName,HelpDescription,RegExp,ValidationValue,IsRegExp FROM @TBL_Detailrecord ORDER BY PromotionAttributeId;
		
         END TRY
         BEGIN CATCH
             DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPromotionAttributeValues @DiscountTypeName = '+CAST(@DiscountTypeName AS nvarchar(500));
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPromotionAttributeValues',
                  @ErrorInProcedure = @ERROR_PROCEDURE,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;