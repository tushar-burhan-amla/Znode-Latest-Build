CREATE PROCEDURE [dbo].[Znode_ImportGetDefaultFamilyAttribute]
(
	@ImportHeadId			INT = 0,
	@PimAttributeFamilyId	INT = 0,
	@PromotionTypeId		INT = 0
)
AS
/*
	Summary : - This procedure is used to get the default family attribute used in import/export (to download sample format for import) 
	Unit Testing
	EXEC Znode_ImportGetDefaultFamilyAttribute @ImportHeadId = 1, @PimAttributeFamilyId = 1
	EXEC Znode_ImportGetDefaultFamilyAttribute @ImportHeadId = 21, @PimAttributeFamilyId = 0, @PromotionTypeId = 17
	EXEC Znode_ImportGetDefaultFamilyAttribute @ImportHeadId = 21, @PimAttributeFamilyId = 0, @PromotionTypeId = 7
	EXEC Znode_ImportGetDefaultFamilyAttribute @ImportHeadId = 21, @PimAttributeFamilyId = 0, @PromotionTypeId = 13
	EXEC Znode_ImportGetDefaultFamilyAttribute @ImportHeadId = 22, @PimAttributeFamilyId = 0, @PromotionTypeId = 8
*/
 BEGIN 
 BEGIN TRY
	SET NOCOUNT ON

    DECLARE @ImportHead NVARCHAR(100) = (SELECT TOP 1 Name FROM ZnodeImportHead WHERE ImportHeadId = @ImportHeadId)

    DECLARE @DefaultFamilyId INT

	DECLARE @Tlb_AttributeCode TABLE (AttributeCode VARCHAR(300), GroupDisplayOrder INT,DisplayOrder INT)

	DECLARE @Tlb_OtherTemplateData TABLE (AttributeCode NVARCHAR(300), DisplayOrder INT) 

	IF @ImportHead IN ('Product')
	BEGIN 
	  	SET @DefaultFamilyId = dbo.Fn_GetDefaultPimProductFamilyId();
		
		INSERT INTO @Tlb_AttributeCode (AttributeCode,GroupDisplayOrder,DisplayOrder)	
	    SELECT DISTINCT AttributeCode ,ZPFM.GroupDisplayOrder,ZPA.DisplayOrder 
 		FROM ZnodePimAttribute ZPA 
		INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON (ZPFM.PimAttributeId = ZPA.PimAttributeId )
		INNER JOIN ZnodePimAttributeGroup ZPAG ON ZPFM.PimAttributeGroupId = ZPAG.PimAttributeGroupId
		WHERE ZPA.IsCategory = 0  AND PimAttributeFamilyId = @PimAttributeFamilyId 
		
		INSERT INTO @Tlb_AttributeCode (AttributeCode,GroupDisplayOrder,DisplayOrder)	 
		SELECT DISTINCT AttributeCode, ZPFM.GroupDisplayOrder,ZPA.DisplayOrder
		FROM ZnodePimAttribute ZPA 
		INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON (ZPFM.PimAttributeId = ZPA.PimAttributeId )
		INNER JOIN ZnodePimAttributeGroup ZPAG ON ZPFM.PimAttributeGroupId = ZPAG.PimAttributeGroupId
		WHERE ZPA.IsCategory = 0 AND PimAttributeFamilyId = @DefaultFamilyId
			AND AttributeCode NOT IN (SELECT AttributeCode FROM @Tlb_AttributeCode)
		
		SELECT AttributeCode TargetColumnName FROM @Tlb_AttributeCode 
		ORDER BY 
		--ISNULL(GroupDisplayOrder,0) ,ISNULL(DisplayOrder,0)
		CASE WHEN GroupDisplayOrder IS NULL THEN 1 ELSE 0 END , GroupDisplayOrder ,
		CASE WHEN DisplayOrder IS NULL THEN 1 ELSE 0 END , DisplayOrder		
	END
	ELSE IF @ImportHead IN ('Category')
	BEGIN
		SET @DefaultFamilyId = dbo.Fn_GetCategoryDefaultFamilyId();

		INSERT INTO @Tlb_AttributeCode (AttributeCode,DisplayOrder,GroupDisplayOrder)	
		SELECT DISTINCT AttributeCode ,DisplayOrder ,ZPA.DisplayOrder
 		FROM ZnodePimAttribute ZPA 
		INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON (ZPFM.PimAttributeId = ZPA.PimAttributeId )
		WHERE ZPA.IsCategory = 1 AND PimAttributeFamilyId = @PimAttributeFamilyId
		
		INSERT INTO @Tlb_AttributeCode (AttributeCode,DisplayOrder,GroupDisplayOrder) 
		SELECT DISTINCT AttributeCode ,DisplayOrder ,ZPA.DisplayOrder
 		FROM ZnodePimAttribute ZPA 
		INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
		INNER JOIN ZnodePimFamilyGroupMapper ZPFM ON (ZPFM.PimAttributeId = ZPA.PimAttributeId )
		WHERE ZPA.IsCategory = 1  AND PimAttributeFamilyId = @DefaultFamilyId
			AND AttributeCode NOT IN (SELECT AttributeCode FROM @Tlb_AttributeCode)

		SELECT AttributeCode TargetColumnName FROM @Tlb_AttributeCode 
		ORDER BY 
		CASE WHEN GroupDisplayOrder IS NULL THEN 1 ELSE 0 END , GroupDisplayOrder ,
		CASE WHEN DisplayOrder IS NULL THEN 1 ELSE 0 END , DisplayOrder		
	END 
	ELSE
	BEGIN
		INSERT INTO @Tlb_OtherTemplateData (AttributeCode,DisplayOrder)
		SELECT DISTINCT ZiA.AttributeCode,ZiA.SequenceNumber FROM ZnodeImportAttributeValidation ZiA
		WHERE ZiA.importHeadId = @ImportHeadId
			AND ZiA.AttributeCode NOT IN (SELECT AttributeCode FROM @Tlb_OtherTemplateData)
			
		IF @ImportHead IN ('B2BCustomer')
		BEGIN
			INSERT INTO @Tlb_OtherTemplateData (AttributeCode,DisplayOrder)
			SELECT ZGA.AttributeCode, ZGA.DisplayOrder
			FROM ZnodeGlobalGroupEntityMapper zggem 
			INNER JOIN ZnodeGlobalEntity ZGE ON zggem.GlobalEntityId = ZGE.GlobalEntityId
			INNER JOIN ZnodeGlobalAttributeGroupMapper ZGAGM ON  zggem.GlobalAttributeGroupId = ZGAGM.GlobalAttributeGroupId
			INNER JOIN ZnodeGlobalAttribute ZGA ON ZGAGM.GlobalAttributeId = ZGA.GlobalAttributeId  
			WHERE ZGE.EntityName = 'User' AND ZGA.AttributeCode NOT IN (SELECT AttributeCode FROM @Tlb_OtherTemplateData )
		END

		ELSE IF @ImportHead IN ('Promotions')
		BEGIN
			DECLARE	@DiscountTypeName NVARCHAR(500) = (SELECT TOP 1 [Name] FROM ZnodePromotionType WHERE PromotionTypeId = @PromotionTypeId)
			
			INSERT INTO @Tlb_OtherTemplateData (AttributeCode,DisplayOrder)
			SELECT DISTINCT AttributeCode, 999 As DisplayOrder
			FROM ZnodePromotionAttribute 
			WHERE PromotionAttributeId IN (SELECT PromotionAttributeId FROM ZnodePromotionDiscountAttributeMapper WHERE DiscountTypeName=@DiscountTypeName)
				AND AttributeCode NOT IN (SELECT AttributeCode FROM @Tlb_OtherTemplateData);
		END
			 
		SELECT AttributeCode TargetColumnName FROM @Tlb_OtherTemplateData ORDER BY DisplayOrder
	END
	END TRY

	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportGetDefaultFamilyAttribute 
					@ImportHeadId = '+CAST(@ImportHeadId AS VARCHAR(max))+',
					@PimAttributeFamilyId='+CAST(@PimAttributeFamilyId AS VARCHAR(50))+',
					@PromotionTypeId='+CAST(@PromotionTypeId AS VARCHAR(50))+',
					@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportGetDefaultFamilyAttribute',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END