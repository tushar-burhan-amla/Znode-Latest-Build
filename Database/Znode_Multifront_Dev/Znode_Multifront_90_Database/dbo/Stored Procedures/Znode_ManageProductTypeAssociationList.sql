CREATE PROCEDURE [dbo].[Znode_ManageProductTypeAssociationList]    
(   @WhereClause      NVARCHAR(MAX) = '',    
    @Rows             INT           = 10,    
    @PageNo           INT           = 1,    
    @Order_BY         VARCHAR(1000) = '',    
    @RelatedProductId INT           = 0,    
    @IsAssociated     BIT           = 0,    
    @RowsCount        INT OUT,    
    @LocaleId         INT           = 1)    
AS    
/*    
Summary: This Procedure is used to manage Product association    
Unit Testing :    
 EXEC [Znode_ManageProductTypeAssociationList] '', @RowsCount = 0,@RelatedProductId = 44    
*/    
     BEGIN    
         SET NOCOUNT ON;    
         BEGIN TRY    
             DECLARE @SQL NVARCHAR(MAX), @AlternetOrderBy NVARCHAR(2000),@OutPimProductIds VARCHAR(max);    
             DECLARE @DefaultLocaleId INT= Dbo.Fn_GetDefaultValue('Locale');    
             DECLARE @DefaultAttributeFamily INT= Dbo.Fn_GetDefaultValue('PimFamily');    
    DECLARE @ProductIdTable TABLE (  PimProductId int, CountId int, RowId int IDENTITY(1,1));    
    DECLARE @ProductAttributeDetials TABLE ( PimProductId int, AttributeCode nvarchar(600), AttributeValue nvarchar(max), LocaleId int);    
    DECLARE @OrderByDisplay INT= 0;    
    DECLARE @ProductFinalDetails TABLE( PimProductId int, ProductName nvarchar(max), SKU nvarchar(max));                 
    DECLARE @PimProductId VARCHAR(MAX)= '';    
             DECLARE @TransferPimProductId TransferId     
    DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))    
    INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)    
    SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()    
    
    IF @Order_BY LIKE '%DisplayOrder%'    
             BEGIN    
                SET @OrderByDisplay = 1;    
             END;    
    
            INSERT INTO @TransferPimProductId      
   SELECT PimProductId    
   FROM ZnodePimProductTypeAssociation     
   WHERE PimParentProductId = @RelatedProductId    
            ORDER BY CASE WHEN @Order_By LIKE '% DESC%' THEN CASE WHEN @OrderByDisplay = 1 THEN DisplayOrder ELSE 1 END ELSE 1 END DESC,    
                    CASE WHEN @Order_By LIKE '% ASC%'  THEN CASE WHEN @OrderByDisplay = 1 THEN DisplayOrder ELSE 1 END ELSE 1 END    
         
 SET @IsAssociated = CASE WHEN @IsAssociated = 1 THEN 0     
  WHEN @IsAssociated = 0 THEN 1  END    
    
      
     DECLARE  @ProductListIdRTR TransferId    
  DECLARE @TAb Transferid     
  DECLARE @tBL_mainList TABLE(id INT , RowId INT )    
     
  INSERT INTO @ProductListIdRTR    
  EXEC Znode_GetProductList @IsAssociated ,@TransferPimProductId    
    
  IF CAST(@WhereClause AS NVARCHAR(max))<> N''    
  BEGIN     
       
   SET @SQL = 'SELECT PimProductId FROM ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))    
       
   EXEC Znode_GetFilterPimProductId @WhereClause,@ProductListIdRTR,@localeId    
       
      INSERT INTO @TAB     
   EXEC (@SQL)    
      
  END     
  DECLARE @AttributeCode varchar(600)    
  IF EXISTS (SELECT Top 1 1 FROM @TAb ) OR CAST(@WhereClause AS NVARCHAR(max)) <> N''    
  BEGIN     
     
  SET @AttributeCode = dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC',''))    
    
  INSERT INTO @TBL_MainList    
  EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId    
     
  END     
  ELSE     
  BEGIN    
   SET @AttributeCode = dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC',''))    
        
      
  INSERT INTO @TBL_MainList    
  EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId     
  END      
      
     
    INSERT INTO @ProductIdTable    
             (PimProductId)     
   SELECT id FROM @TBL_MainList                
    
    DECLARE @PimProductIds TransferId    
    
    INSERT INTO @PimProductIds ( Id )     
    SELECT PimProductId FROM @ProductIdTable    
    
    DECLARE @DefaultAttributeCode  TRANSFERID    
     INSERT INTO @DefaultAttributeCode    
     SELECT  PimAttributeId FROM [dbo].[Fn_GetProductGridAttributes]()     
                
                
    --select * from @DefaultAttributeCode    
    
    DECLARE @TBL_AttributeDetails AS TABLE (PimProductId int, AttributeValue nvarchar(max), AttributeCode varchar(600), PimAttributeId int, AttributeDefaultValue NVARCHAR(MAX));    
    
        
             DECLARE @TBL_AttributeDefaultValue TABLE (PimAttributeId INT, AttributeDefaultValueCode VARCHAR(100), IsEditable BIT,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder INT);    
                
    INSERT INTO @TBL_AttributeDetails( PimProductId, AttributeValue, AttributeCode, PimAttributeId , AttributeDefaultValue)    
    EXEC Znode_GetProductsAttributeValue_newTesting @PimProductIds, @DefaultAttributeCode, @LocaleId;      
       
       
        
   INSERT INTO @TBL_AttributeDetails             (PimProductId,              AttributeValue,              AttributeCode,              PimAttributeId             )    
   SELECT a.PimProductId ,CASE WHEN IsProductPublish = 1 THEN   'Published' WHEN IsProductPublish = 0 THEN 'Draft'  ELSE 'Not Published' END, 'PublishStatus',NULL    
   FROM @ProductIdTable a     
   INNER JOIN ZnodePimProduct b ON (b.PimProductId = a.PimProductId)    
    
   --select * from @TBL_AttributeDetails    
    
   ------------------------------------------------------------------------------------------------    
    
   declare @SKU SelectColumnList    
   declare @TBL_Inventorydetails table (Quantity NVARCHAR(MAx),PimProductId INT)    
    
   INSERT INTO @SKU    
   SELECT AttributeValue FROM @TBL_AttributeDetails    
   WHERE AttributeCode = 'SKU'    
    
   --select * from @SKU    
       
   INSERT INTO @TBL_InventoryDetails(Quantity,PimProductId)    
   EXEC Znode_GetPimProductAttributeInventory @SKU=@SKU, @LocaleId=@LocaleId
    
    
   ---------------------------------------------    
    
             DECLARE @FamilyDetails TABLE    
             (PimProductId         INT,    
              PimAttributeFamilyId INT,    
              FamilyName           NVARCHAR(3000)    
             );    
             INSERT INTO @FamilyDetails    
             (PimAttributeFamilyId,    
              PimProductId    
             )    
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId]    
                  @PimProductIds,    
                  1;     
             -- find the product families      
    --;WITH Cte_ProductMedia    
    --           AS (SELECT TBA.PimProductId , TBA.PimAttributeId     
    --  , SUBSTRING( ( SELECT ','+URL+ZMSM.ThumbnailFolderName+'/'+ zm.PATH     
    --  FROM ZnodeMedia AS ZM    
    --           INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)    
    --  INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)    
    --  INNER JOIN @TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)))    
    --  INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)    
    --  WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId     
    --  FOR XML PATH('')), 2 , 4000) AS AttributeValue     
    --  FROM @TBL_AttributeDetails AS TBA     
    --  INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId ))    
                              
    --    UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue    
    -- FROM @TBL_AttributeDetails TBAV     
    -- INNER JOIN Cte_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId     
    -- AND CTPM.PimAttributeId = TBAV.PimAttributeId;    
         
             UPDATE a    
               SET    
                   FamilyName = b.AttributeFamilyName    
             FROM @FamilyDetails a    
                  INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId    
                                                       AND LocaleId = @LocaleId);    
             UPDATE a    
               SET    
                   FamilyName = b.AttributeFamilyName    
             FROM @FamilyDetails a    
                  INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId    
                                                       AND LocaleId = @DefaultLocaleId)    
             WHERE a.FamilyName IS NULL    
                   OR a.FamilyName = '';    
       
   INSERT INTO @TBL_AttributeDetails             (PimProductId,              AttributeValue,              AttributeCode,              PimAttributeId             )    
   SELECT PimProductId ,FamilyName, 'AttributeFamily',NULL    
   FROM @FamilyDetails     
        
      
             --- Update the  product families name locale wise       
        UPDATE  @TBL_AttributeDetails SET PimAttributeId = 0 WHERE PimAttributeId IS nULL     
      DECLARE @ProductXML XML     
    
    
    
      SET @ProductXML =   '<MainProduct>'+ STUFF( (  SELECT '<Product>'    
                                                      +'<PimProductTypeAssociationId>'+CAST(ISNULL(ZPTA.PimProductTypeAssociationId,'') AS VARCHAR(50))+'</PimProductTypeAssociationId>'    
               +'<PimProductId>'+CAST(zpp.PimProductId AS VARCHAR(50))+'</PimProductId>'    
               +'<RelatedProductId>'+CAST(ISNULL(ZPTA.PimParentProductId,'') AS VARCHAR(50))+'</RelatedProductId>'    
               +'<DisplayOrder>'+CAST(ZPTA.[DisplayOrder] AS VARCHAR(50))+'</DisplayOrder>'    
       +'<BundleQuantity>'+CAST(ISNULL(ZPTA.[BundleQuantity],'1') AS VARCHAR(50))+'</BundleQuantity>'    
               +'<AvailableInventory>'+CAST(ISNULL(IDD.[Quantity],'') AS VARCHAR(50))+'</AvailableInventory>'    
    
   + STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST( (SELECT ''+TBADI.AttributeValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'       
               FROM @TBL_AttributeDetails TBADI          
                WHERE TBADI.PimProductId = zpp.PimProductId     
                ORDER BY TBADI.PimProductId DESC    
                FOR XML PATH (''), TYPE    
                ).value('.', ' Nvarchar(max)'), 1, 0, '')+'</Product>'        
    
    FROM @ProductIdTable AS zpp    
    INNER JOIN @TBL_MainList TMM ON (TMM.id = zpp.PimProductId)    
    LEFT JOIN @TBL_InventoryDetails IDD ON (zpp.PimProductId = IDD.PimProductId)    
             LEFT JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimProductId = Zpp.PimProductId    
                                                                    AND ZPTA.PimParentProductId = @RelatedProductId)    
             ORDER BY CASE    
                          WHEN @Order_By LIKE '% DESC%'    
                          THEN CASE    
                                   WHEN @OrderByDisplay = 1    
                                   THEN ZPTA.DisplayOrder    
           ELSE 1    
                               END    
                          ELSE 1    
                      END DESC,    
                      CASE    
                          WHEN @Order_By LIKE '% ASC%'    
                          THEN CASE    
                                   WHEN @OrderByDisplay = 1    
                                   THEN ZPTA.DisplayOrder    
                                   ELSE 1    
                               END    
                          ELSE 1    
                      END,TMM.RowId    
   FOR XML PATH (''),TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')+'</MainProduct>'    
     
   SELECT  CAST(@ProductXML AS XML )  ProductXMl    
         
       SELECT AttributeCode ,  ZPAL.AttributeName    
    FROM ZnodePimAttribute ZPA     
    LEFT JOIN ZnodePiMAttributeLOcale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId )    
             WHERE LocaleId = 1      
    AND  IsCategory = 0     
    AND ZPA.IsShowOnGrid = 1      
    UNION ALL     
    SELECT 'PublishStatus','Publish Status'    
    
    
      IF EXISTS (SELECT Top 1 1 FROM @TAb )    
    BEGIN     
    
     SELECT (SELECT COUNT(1) FROM @TAb) AS RowsCount       
    END     
    ELSE     
    BEGIN    
       SELECT (SELECT COUNT(1) FROM @ProductListIdRTR) AS RowsCount       
    END     
    
         END TRY    
         BEGIN CATCH    
               DECLARE @Status BIT ;    
       SET @Status = 0;    
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageProductTypeAssociationList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RelatedProductId='+CAST(@RelatedProductId AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
             EXEC Znode_InsertProcedureErrorLog    
    @ProcedureName = 'Znode_ManageProductTypeAssociationList',    
    @ErrorInProcedure = @Error_procedure,    
    @ErrorMessage = @ErrorMessage,    
    @ErrorLine = @ErrorLine,    
    @ErrorCall = @ErrorCall;    
         END CATCH;    
END;