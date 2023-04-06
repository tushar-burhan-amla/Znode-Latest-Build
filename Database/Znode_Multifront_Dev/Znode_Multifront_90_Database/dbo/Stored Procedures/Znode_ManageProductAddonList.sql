

CREATE PROCEDURE [dbo].[Znode_ManageProductAddonList]
(    @PimProductId INT,
     @LocaleId     INT = 1)
AS  
   /*  Summary :- This Procedure is used to addon details of the product  
     Unit Testing
     EXEC Znode_ManageProductAddonList 61,1
    */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_AttributeDefaultValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder INT
             );
             DECLARE @TBL_AttributeDetails AS TABLE
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT
             );
             DECLARE @TBL_FamilyDetails TABLE
             (PimProductId         INT,
              PimAttributeFamilyId INT,
              FamilyName           NVARCHAR(3000)
             );
             DECLARE @TBL_AddOnGroupDetail TABLE
             (PimAddOnProductId INT,
              PimProductId      INT,
              PimAddonGroupId   INT,
              AddonGroupName    NVARCHAR(MAX),
              RequiredType      VARCHAR(50),
              DisplayType       NVARCHAR(400),
              DisplayOrder      INT
             );
             
			 DECLARE @PimProductIds TransferId, --VARCHAR(MAX), 
				     @PimAttributeId VARCHAR(MAX);

             DECLARE @TBL_AddOnGroupRelatedProductIds TABLE
             (RelatedProductId        INT,
              PimAddonGroupId         INT,
              PimProductId            INT,
              PimAddOnProductDetailId INT,
			  AddOnDisplayOrder INT,
			  IsDefault BIT
             );


             WITH Cte_AddonGroupListBothLocale
                  AS (SELECT ZPAOP.PimAddOnProductId,
                             PimProductId,
                             ZPAOP.PimAddonGroupId,
                             ZPAGL.AddonGroupName,
                             ZPAOP.RequiredType,
                             ZPAG.DisplayType,
                             ZPAOP.DisplayOrder,
                             ZPAGL.LocaleId
                      FROM ZnodePimAddOnProduct ZPAOP
                           LEFT JOIN ZnodePimAddonGroup ZPAG ON(ZPAG.PimAddonGroupId = ZPAOp.PimAddonGroupId)
                           LEFT JOIN ZnodePimAddonGroupLocale ZPAGL ON(ZPAGL.PimAddonGroupId = ZPAG.PimAddonGroupId)
                      WHERE PimProductId = @PimProductId
                            AND ZPAGL.LocaleId IN(@DefaultLocaleId, @LocaleId)),
                  Cte_AddOnGroupListFirstLocale
                  AS (SELECT PimAddOnProductId,
                             PimProductId,
                             PimAddonGroupId,
                             AddonGroupName,
                             RequiredType,
                             DisplayType,
                             DisplayOrder
                      FROM Cte_AddonGroupListBothLocale CTAGLBL
                      WHERE CTAGLBL.LocaleId = @LocaleId),
                  Cte_AddonGroupListDefaultLocale
                  AS (
                  SELECT PimAddOnProductId,
                         PimProductId,
                         PimAddonGroupId,
                         AddonGroupName,
                         RequiredType,
                         DisplayType,
                         DisplayOrder
                  FROM Cte_AddOnGroupListFirstLocale
                  UNION ALL
                  SELECT PimAddOnProductId,
                         PimProductId,
                         PimAddonGroupId,
                         AddonGroupName,
                         RequiredType,
                         DisplayType,
                         DisplayOrder
                  FROM Cte_AddonGroupListBothLocale CTAGLBL
                  WHERE CTAGLBL.LocaleId = @DefaultLocaleId
                        AND NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_AddOnGroupListFirstLocale CTAOGLFL
                      WHERE CTAOGLFL.PimAddonGroupId = CTAGLBL.PimAddonGroupId
                  ))
                  INSERT INTO @TBL_AddOnGroupDetail
                  (PimAddOnProductId,
                   PimProductId,
                   PimAddonGroupId,
                   AddonGroupName,
                   RequiredType,
                   DisplayType,
                   DisplayOrder
                  )
                         SELECT PimAddOnProductId,
                                PimProductId,
                                PimAddonGroupId,
                                AddonGroupName,
                                RequiredType,
                                DisplayType,
                                DisplayOrder
                         FROM Cte_AddonGroupListDefaultLocale;
             INSERT INTO @TBL_AddOnGroupRelatedProductIds
             (RelatedProductId,
              PimAddonGroupId,
              PimProductId,
              PimAddOnProductDetailId,
			  AddOnDisplayOrder,
			  IsDefault
             )
                    SELECT ZPAP.PimProductId RelatedProductId,
                           ZPAP.PimAddonGroupId,
                           PimChildProductId PimProductId,
                           PimAddOnProductDetailId,
						   ZPAPD.DisplayOrder AddOnDisplayOrder, 
						  ZPAPD.IsDefault
                    FROM  ZnodePimAddOnProduct ZPAP  
                         INNER JOIN ZnodePimAddOnProductDetail ZPAPD ON(ZPAP.PimAddOnProductId = ZPAPD.PimAddOnProductId)
                    WHERE ZPAP.PimProductId = @PimProductId;
             --SET @PimProductIds = SUBSTRING(
             --                              (
             --                                  SELECT ','+CAST(PimProductId AS VARCHAR(100))
             --                                  FROM @TBL_AddOnGroupRelatedProductIds
             --                                  FOR XML PATH('')
             --                              ), 2, 4000);

			 INSERT INTO @PimProductIds ( Id )
			 SELECT DISTINCT PimProductId FROM @TBL_AddOnGroupRelatedProductIds

             SET @PimAttributeId = [dbo].[Fn_GetProductGridDefaultAttributeId]();
             INSERT INTO @TBL_AttributeDefaultValue
             (PimAttributeId,
              AttributeDefaultValueCode,
              IsEditable,
              AttributeDefaultValue,
			  DisplayOrder
             )
             EXEC Znode_GetAttributeDefaultValueLocale
                  @PimAttributeId,
                  @LocaleId;
             INSERT INTO @TBL_AttributeDetails
             (PimProductId,
              AttributeValue,
              AttributeCode,
              PimAttributeId
             )
             EXEC [Znode_GetProductsAttributeValue]
                  @PimProductIds,
                  @PimAttributeId,
                  @LocaleId;
             WITH Cte_UpdateDefaultAttributeValue
                  AS (SELECT PimProductId,
                             AttributeCode,
                             AttributeValue,
                             SUBSTRING(
                                      (
                                          SELECT ','+TBADV.AttributeDefaultValue
                                          FROM @TBL_AttributeDefaultValue AS TBADV
                                               INNER JOIN ZnodePimAttribute AS TBAC ON(TBADV.PimAttributeId = TBAC.PimAttributeId)
                                          WHERE TBAC.AttributeCode = TBAD.AttributeCode
                                                AND EXISTS
                                          (
                                              SELECT TOP 1 1
                                              FROM dbo.split(TBAD.AttributeValue, ',') AS SP
                                              WHERE Sp.item = TBADV.AttributeDefaultValueCode
                                          )
                                          FOR XML PATH('')
                                      ), 2, 4000) AS AttributeDefaultValue
                      FROM @TBL_AttributeDetails AS TBAD)
                  UPDATE TBAD
                    SET
                        AttributeValue = CTUDAV.AttributeDefaultValue
                  FROM @TBL_AttributeDetails TBAD
                       INNER JOIN Cte_UpdateDefaultAttributeValue CTUDAV ON(CTUDAV.PimProductId = TBAD.PimProductId
                                                                            AND CTUDAV.AttributeCode = TBAD.AttributeCode)
                  WHERE AttributeDefaultValue IS NOT NULL;
             INSERT INTO @TBL_FamilyDetails
             (PimAttributeFamilyId,
              PimProductId
             )
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId]
                  @PimProductIds,
                  1;
             UPDATE TBFD
               SET
                   FamilyName = ZPFL.AttributeFamilyName
             FROM @TBL_FamilyDetails TBFD
                  INNER JOIN ZnodePimFamilyLocale ZPFL ON(TBFD.PimAttributeFamilyId = ZPFL.PimAttributeFamilyId
                                                          AND LocaleId = @LocaleId);
             UPDATE TBFD
               SET
                   FamilyName = ZPFL.AttributeFamilyName
             FROM @TBL_FamilyDetails TBFD
                  INNER JOIN ZnodePimFamilyLocale ZPFL ON(TBFD.PimAttributeFamilyId = ZPFL.PimAttributeFamilyId
                                                          AND LocaleId = @DefaultLocaleId)
             WHERE TBFD.FamilyName IS NULL
                   OR TBFD.FamilyName = '';
             SELECT TBAGRP.PimProductId ProductId,
                    [ProductName],
                    ProductType,
                    ISNULL(TBFD.FamilyName, '') AttributeFamily,
                    [SKU],
                    [Price],
                    [Quantity],
                    [IsActive],
                    RelatedProductId,
                    Assortment,
                    TBAGD.PimAddonGroupId,
                    AddonGroupName,
                    DisplayOrder,
                    DisplayType,
                    RequiredType,
                    PimAddOnProductId,
                    PimAddOnProductDetailId,
					AddOnDisplayOrder,
					IsDefault
             FROM @TBL_AddOnGroupDetail TBAGD
                  LEFT JOIN @TBL_AddOnGroupRelatedProductIds TBAGRP ON(TBAGRP.PimAddonGroupId = TBAGD.PimAddonGroupId
                                                                       AND TBAGRP.RelatedProductId = TBAGD.PimProductId)
                  LEFT JOIN @TBL_FamilyDetails TBFD ON(TBFD.PimProductId = TBAGRP.PimProductId)
                  LEFT JOIN
             (
                 SELECT PimProductId,
                        AttributeValue,
                        AttributeCode
                 FROM @TBL_AttributeDetails
             ) TBAD PIVOT(MAX(AttributeValue) FOR AttributeCode IN([ProductName],
                                                                   [SKU],
                                                                   [Price],
                                                                   [Quantity],
                                                                   [IsActive],
                                                                   [ProductType],
                                                                   [ProductImage],
                                                                   [Assortment])) PIV ON(PIV.PimProductId = TBAGRP.PimProductId)
		    ORDER BY AddOnDisplayOrder ASC
			;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageProductAddonList @PimProductId = '+CAST(@PimProductId AS VARCHAR(max))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ManageProductAddonList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;