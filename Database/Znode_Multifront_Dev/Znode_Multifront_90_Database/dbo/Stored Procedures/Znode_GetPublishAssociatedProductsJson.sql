
CREATE PROCEDURE [dbo].[Znode_GetPublishAssociatedProductsJson]    
(       
     @PublishCatalogId VARCHAR(MAX) = '',    
     @PimProductId     TransferId Readonly,    
     @ProductType      VARCHAR(300) = 'BundleProduct',    
     @VersionId        INT          = 0,    
     @UserId           INT,    
     @PimCategoryHierarchyId int = 0,    
     @PublishStateId INT = 0  ,    
     @VersionIdString  VARCHAR(2000)= '',    
     @Status  BIT  Output,    
     @RevisionType Varchar(50) = ''    
)    
AS    
  /*    
    Summary : If PimcatalogId is provided then get all  Bundles / Group / Configurable product and  provide above mentioned data    
              If PimProductId is provided then get all Bundles / Group / Configurable if associated with given product id and provide above mentioned data    
       Input: @PublishCatalogId or @PimProductId    
       Output should be in XML format    
             Required o/p    
       <BundleProductEntity>    
       <ZnodeProductId></ZnodeProductId>    
       <ZnodeCatalogId></ZnodeCatalogId>    
       <AsscociadedZnodeProductIds></AsscociadedZnodeProductIds>    
       </BundleProductEntity>    
    Unit Testing     
    BundleProduct    
 DECLARE     
    EXEC [dbo].[Znode_GetPublishAssociatedProducts] @PublishCatalogId = 1  , @ProductType = 'BundleProduct' ,@userId = 2     
    EXEC [dbo].[Znode_GetPublishAssociatedProducts] @PublishCatalogId =2 , @ProductType = 'ConfigurableProduct',@userId = 2 ,@PimCategoryHierarchyId = 7     
    Group Product    
    EXEC [dbo].[Znode_GetPublishAssociatedProducts]  @PublishCatalogId ='2',@PimProductIdh =''  , @PimProducttType = 'GroupedProduct'    
    EXEC [dbo].[Znode_GetPublishAssociatedProducts]  @PublishCatalogId ='',@PimProductId ='200066'  , @PimProducttType = 'GroupedProduct'    
    EXEC [dbo].[Znode_GetPublishAssociatedProducts] @PimProductId ='200066'  , @PimProducttType = 'GroupedProduct'    
   */    
BEGIN    
BEGIN TRAN GetPublishAssociatedProducts;    
BEGIN TRY    
SET NOCOUNT ON;    
      
    IF OBJECT_ID('tempdb..#VesionIds') is not null    
    DROP TABLE #VesionIds    
    Create Table #VesionIds(ZnodeCatalogId int , VersionId int , LocaleId int , RevisionType varchar(50) )    
    
      If @VersionIdString <> ''       
    Insert into #VesionIds (ZnodeCatalogId, VersionId, LocaleId, RevisionType)     
    SELECT PV.ZnodeCatalogId, PV.VersionId, PV.LocaleId, PV.RevisionType FROM ZnodePublishVersionEntity PV Inner join Split(@VersionIdString,',') S ON PV.VersionId = S.Item    
    Else     
    Begin    
     If  (@RevisionType like '%Preview%'  OR @RevisionType like '%Production%' )    
      Insert into #VesionIds (ZnodeCatalogId, VersionId, LocaleId, RevisionType)     
      SELECT PV.ZnodeCatalogId, PV.VersionId, PV.LocaleId, PV.RevisionType FROM ZnodePublishVersionEntity PV  where PV.IsPublishSuccess =1     
      AND PV.RevisionType ='Preview'    
    
     If  (@RevisionType like '%Production%' OR @RevisionType = 'None')    
      Insert into #VesionIds (ZnodeCatalogId, VersionId, LocaleId, RevisionType)     
      SELECT PV.ZnodeCatalogId, PV.VersionId, PV.LocaleId, PV.RevisionType FROM ZnodePublishVersionEntity PV  where PV.IsPublishSuccess =1     
      AND PV.RevisionType ='Production'    
     end     
    
   IF OBJECT_ID('tempdb..#TBL_PublishCatalogId') is not null    
    DROP TABLE #TBL_PublishCatalogId    
       
   CREATE TABLE #TBL_PublishCatalogId (PublishCatalogId INT,PublishProductId INT,PimProductId  INT , VersionId INT,LocaleId INT  );    
   DECLARE  @PimAttributeId INT = [dbo].[Fn_GetProductTypeAttributeId]()    
     ,@PimAttributeDefaultValueId INT = (SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue WHERE AttributeDefaultValueCode = @ProductType)    
     ,@DefaultLocaleId INT = dbo.fn_getDefaultlocaleId()     
   DECLARE @GetDate DATETIME =dbo.Fn_GetDate()    
        
   IF OBJECT_ID('tempdb..#TBL_PublisshIds') is not null    
   DROP TABLE #TBL_PublisshIds    
   Create TABLE #TBL_PublisshIds (PublishProductId INT , PimProductId INT , PublishCatalogId INT)    
    
   DECLARE  @PimProductId_New TransferId    
          
    IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0     
    BEGIN     
    INSERT INTO #TBL_PublisshIds     
    Select ZPP.PublishProductId ,ZPP.PimProductId , ZPP.PublishCatalogId from ZnodePublishProduct ZPP Inner join @PimProductId  PPI on ZPP.PimProductId = PPI.ID    
      --EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1    
          
      INSERT INTO @PimProductId_New    
      SELECT DISTINCT PimProductId FROM #TBL_PublisshIds    
    
     -- SELECT  @PimProductId     
    END     
        
    IF  ISnull(@PimCategoryHierarchyId,0) <> 0     
    BEGIN     
        
      INSERT INTO #TBL_PublisshIds     
      EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1,@PimCategoryHierarchyId    
          
      --SET @PimProductId = SUBSTRING((SELECT DISTINCT ','+CAST(PimProductId AS VARCHAr(50)) FROM #TBL_PublisshIds FOR XML PATH ('')),2,8000 )    
    
      INSERT INTO @PimProductId_New    
      SELECT PimProductId FROM #TBL_PublisshIds    
    
          
    END     
    
    IF  ISnull(@PimCategoryHierarchyId,0) <> 0     
    BEGIN     
     INSERT INTO #TBL_PublishCatalogId     
     SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,MAX(ZPCP.PublishCatalogLogId) ,ZPCP.LocaleID      
     FROM ZnodePublishProduct ZPP     
     INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )    
     INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)    
     LEFT JOIN  ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)    
     WHERE (EXISTS (SELECT TOP 1 1 FROM #TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId   )     
     AND  (ZPP.PublishCatalogId =  @PublishCatalogId ))    
     AND ZPV.PimAttributeId  = @PimAttributeId    
     AND ZPAVL.PimAttributeDefaultValueId= @PimAttributeDefaultValueId    
     AND ZPAVL.LocaleId = @DefaultLocaleId    
     AND ISNULL(ZPCP.LocaleId,0) <> 0     
     AND ZPCP.PublishStateId= @PublishStateId    
     AND EXISTS(SELECT * FROM ZnodeLocale ZL where ZL.IsActive = 1  and ZPCP.LocaleId = ZL.LocaleId )    
     GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,ZPCP.LocaleID     
         
    END    
    ELSE     
    BEGIN     
        
    IF NOT EXISTS (SELECT TOP 1 1 FROM @PimProductId ) AND @PublishCatalogId <> 0    
    BEGIN    
      INSERT INTO #TBL_PublishCatalogId     
      SELECT distinct ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,  ZPCP.VersionId PublishCatalogLogId ,ZPCP.LocaleID     
      FROM ZnodePublishProduct ZPP     
      INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )    
      INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)    
      INNER JOIN  #VesionIds  ZPCP ON (ZPCP.ZnodeCatalogId  = ZPP.PublishCatalogId AND ISNULL(ZPCP.LocaleId,0) <> 0 )            
      WHERE     
      --(EXISTS (SELECT TOP 1 1 FROM #TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  AND  @PublishCatalogId = '' )     
      --OR     
      (ZPP.PublishCatalogId =  @PublishCatalogId )    
      AND ZPV.PimAttributeId  = @PimAttributeId    
      AND ZPAVL.PimAttributeDefaultValueId= @PimAttributeDefaultValueId    
      AND ZPAVL.LocaleId = @DefaultLocaleId    
      AND EXISTS(SELECT * FROM ZnodeLocale ZL where ZL.IsActive = 1  and ZPCP.LocaleId = ZL.LocaleId )    
    END    
    ELSE    
    BEGIN    
      INSERT INTO #TBL_PublishCatalogId     
          
      SELECT distinct ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,  ZPCP.VersionId PublishCatalogLogId ,ZPCP.LocaleID     
      FROM ZnodePublishProduct ZPP     
      INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )    
      INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)    
      LEFT JOIN  #VesionIds ZPCP ON (ZPCP.ZnodeCatalogId  = ZPP.PublishCatalogId AND ISNULL(ZPCP.LocaleId,0) <> 0 )            
      WHERE (EXISTS (SELECT TOP 1 1 FROM #TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  AND ZPP.PublishCatalogId = SP.PublishCatalogId) )    
      AND ZPV.PimAttributeId  = @PimAttributeId    
      AND ZPAVL.PimAttributeDefaultValueId= @PimAttributeDefaultValueId    
      AND ZPAVL.LocaleId = @DefaultLocaleId    
      AND EXISTS(SELECT * FROM ZnodeLocale ZL where ZL.IsActive = 1  and ZPCP.LocaleId = ZL.LocaleId )    
      --GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,ZPCP.LocaleID,    
    
    END     
         
    END    
        
             DECLARE @TBL_ProductTypeXML TABLE    
             (PublishProductId INT,    
			  PublishCatalogId INT,    
              ReturnXML        XML,    
              VersionId        INT    
             );    
             DECLARE @TBL_PimProductId TABLE    
             ([PimProductId]   INT,    
              PublishCatalogId INT,    
			  PublishProductId INT    
             );    
                
             DECLARE @TBL_PimAssociatedEntity TABLE    
             (    
				ZnodeProductId INT,    
				ZnodeCatalogId INT,    
				AsscociadedZnodeProductIds VARCHAR(MAX),    
				ConfigurableProductEntity NVARCHAR(MAX),    
				LocaleId INT,    
				DisplayOrder INT,    
				VersionId INT    
             );    
    
    If @ProductType = 'BundleProduct' AND Exists (Select TOP 1 1 from #TBL_PublishCatalogId)    
    Begin    
               IF OBJECT_ID('tempdb..#BundleProductPublishedXML') is not null    
					drop table #BundleProductPublishedXML    
    
		 IF (isnull(@PublishCatalogId ,'') = '' or isnull(@PublishCatalogId,0) = 0) and @VersionIdString = ''    
		 Begin     
			  UPDATE ZnodePublishBundleProductEntity SET ElasticSearchEvent = 2 
			  where not exists(select * from ZnodePublishProduct where ZnodePublishBundleProductEntity.ZnodeProductId = ZnodePublishProduct.PublishProductId)    
			  AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZnodePublishBundleProductEntity.VersionId)

			  UPDATE ZnodePublishBundleProductEntity SET ElasticSearchEvent = 2 
			  where Exists ( Select TOP 1 1 from     
			  #TBL_PublishCatalogId A where ZnodePublishBundleProductEntity.ZnodeCatalogId =A.PublishCatalogId AND     
			  ZnodePublishBundleProductEntity.ZnodeProductId  = A.PublishProductId )    
			  AND Exists (SELECT TOP 1 1 FRoM #VesionIds     
			  Where ZnodePublishBundleProductEntity.VersionId =  #VesionIds.VersionId)    
		END     
    
		 UPDATE ZPBP SET AssociatedProductDisplayOrder = ISNULL(ZPTA.DisplayOrder,0),AssociatedProductBundleQuantity = ISNULL(ZPTA.BundleQuantity,1)
		 FROM #TBL_PublishCatalogId TBP    
		 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)    
		 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
		 INNER JOIN ZnodePublishBundleProductEntity ZPBP on  TBP.VersionId = ZPBP.VersionId AND TBP.PublishProductId = ZPBP.ZnodeProductId
			AND TBP.PublishCatalogId = ZPBP.ZnodeCatalogId AND TBPU.PublishProductId = ZPBP.AssociatedZnodeProductId
		 WHERE ZPBP.ElasticSearchEvent <> 2 
		 AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPBP.VersionId)


		 INSERT INTO ZnodePublishBundleProductEntity     
		 (VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder,AssociatedProductBundleQuantity)    
		 SELECT DISTINCT TBP.VersionId, TBP.PublishProductId, TBP.PublishCatalogId ,    
		 TBPU.PublishProductId AssociatedZnodeProductId,ISNULL(ZPTA.DisplayOrder,0)  AssociatedProductDisplayOrder,ISNULL(ZPTA.BundleQuantity,1)  AS AssociatedProductBundleQuantity  
		 FROM #TBL_PublishCatalogId TBP    
		 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)    
		 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
		 WHERE NOT EXISTS(SELECT * FROM ZnodePublishBundleProductEntity ZPBP WHERE  TBP.VersionId = ZPBP.VersionId AND TBP.PublishProductId = ZPBP.ZnodeProductId
			AND TBP.PublishCatalogId = ZPBP.ZnodeCatalogId AND TBPU.PublishProductId = ZPBP.AssociatedZnodeProductId AND ZPBP.ElasticSearchEvent <> 2 )
		
   End     
   
   If @ProductType = 'GroupedProduct' AND Exists (Select TOP 1 1 from #TBL_PublishCatalogId)    
   Begin    
		 IF (isnull(@PublishCatalogId ,'') = '' or isnull(@PublishCatalogId,0) = 0) and @VersionIdString = ''    
		 Begin     
			  UPDATE ZnodePublishGroupProductEntity SET ElasticSearchEvent = 2     
			  WHERE NOT EXISTS(SELECT * FROM ZnodePublishProduct WHERE ZnodePublishGroupProductEntity.ZnodeProductId = ZnodePublishProduct.PublishProductId)    
              AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZnodePublishGroupProductEntity.VersionId)

			  UPDATE ZnodePublishGroupProductEntity SET ElasticSearchEvent = 2  
			  where Exists ( Select TOP 1 1 from     
				  #TBL_PublishCatalogId A where ZnodePublishGroupProductEntity.ZnodeCatalogId =A.PublishCatalogId AND     
				  ZnodePublishGroupProductEntity.ZnodeProductId  = A.PublishProductId )    
				  AND Exists (SELECT TOP 1 1 FRoM #VesionIds     
			   Where ZnodePublishGroupProductEntity.VersionId =  #VesionIds.VersionId)    
		 end     
    
	  UPDATE ZPGP SET AssociatedProductDisplayOrder = ISNULL(ZPTA.DisplayOrder,0),ElasticSearchEvent = 1
	  FROM #TBL_PublishCatalogId TBP    
      INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)    
      INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )    
	  INNER JOIN ZnodePublishGroupProductEntity ZPGP ON ZPGP.VersionId = TBP.VersionId AND ZPGP.ZnodeProductId = TBP.PublishProductId 
		AND ZPGP.ZnodeCatalogId = TBP.PublishCatalogId AND ZPGP.AssociatedZnodeProductId = TBPU.PublishProductId AND ZPGP.ElasticSearchEvent <> 2  
	  AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPGP.VersionId)

      INSERT INTO ZnodePublishGroupProductEntity(VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder)    
      SELECT TBP.VersionId, TBP.PublishProductId ,TBP.PublishCatalogId ,TBPU.PublishProductId ,ISNULL(ZPTA.DisplayOrder,0)    
      FROM #TBL_PublishCatalogId TBP    
      INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)    
      INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )    
	  WHERE NOT EXISTS(SELECT * FROM ZnodePublishGroupProductEntity ZPGP WHERE ZPGP.VersionId = TBP.VersionId AND ZPGP.ZnodeProductId = TBP.PublishProductId 
		AND ZPGP.ZnodeCatalogId = TBP.PublishCatalogId AND ZPGP.AssociatedZnodeProductId = TBPU.PublishProductId AND ZPGP.ElasticSearchEvent <> 2  )
   End     
            
			
   If @ProductType = 'ConfigurableProduct' AND Exists (Select TOP 1 1 from #TBL_PublishCatalogId)    
   Begin    
		 If (isnull(@PublishCatalogId ,'') = '' or isnull(@PublishCatalogId,0) = 0) and @VersionIdString = ''    
		 Begin    
			  UPDATE ZnodePublishConfigurableProductEntity SET ElasticSearchEvent = 2       
			  where not exists(select * from ZnodePublishProduct where ZnodePublishConfigurableProductEntity.ZnodeProductId = ZnodePublishProduct.PublishProductId)    
              AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZnodePublishConfigurableProductEntity.VersionId)

			  UPDATE ZnodePublishConfigurableProductEntity SET ElasticSearchEvent = 2  
			  where Exists ( Select TOP 1 1 from     
			  #TBL_PublishCatalogId A where ZnodePublishConfigurableProductEntity.ZnodeCatalogId =A.PublishCatalogId AND     
			  ZnodePublishConfigurableProductEntity.ZnodeProductId  = A.PublishProductId )     
			  AND Exists (SELECT TOP 1 1 FRoM #VesionIds     
			   Where ZnodePublishConfigurableProductEntity.VersionId =  #VesionIds.VersionId)    
		 end     
    
		SELECT DISTINCT TBP.VersionId, TBP.PublishProductId ,    
			  TBP.PublishCatalogId ,    
			  Isnull(TBPU.PublishProductId,0) AssociatedZnodeProductId,     
			  ISNULL(ZPTA.DisplayOrder,0) DisplayOrder,'[]' SelectValues,          
			  '[' + Isnull(SUBSTRING((SELECT Distinct ','+ +'"'+CAST(ZPA.AttributeCode AS VARCHAR(50)) +'"'     
			  FROM ZnodePimConfigureProductAttribute ZPCPA     
			  LEFT JOIN ZnodePimAttribute ZPA ON Zpa.PimAttributeId = ZPCPA.PimAttributeId 
			  WHERE ZPTA.PimParentProductId = ZPCPA.PimProductId
			  FOR XML PATH ('') ),2,2000),Null) +']' as ConfigurableAttributeCodes    
			  , ZPTA.IsDefault  
		  INTO #TempConfigurableProductEntity 
		 FROM #TBL_PublishCatalogId TBP    
		 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)    
		 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )    
    
		 UPDATE ZPCP SET ZPCP.AssociatedProductDisplayOrder = AssociatedProductDisplayOrder,
			ConfigurableAttributeCodes = TBP.ConfigurableAttributeCodes,IsDefault = TBP.IsDefault,ZPCP.ElasticSearchEvent =1
		 FROM #TempConfigurableProductEntity TBP    
		 INNER JOIN ZnodePublishConfigurableProductEntity ZPCP ON ZPCP.VersionId = TBP.VersionId 
			AND ZPCP.ZnodeProductId = TBP.PublishProductId AND ZPCP.ZnodeCatalogId = TBP.PublishCatalogId AND ZPCP.AssociatedZnodeProductId = TBP.AssociatedZnodeProductId
		 AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPCP.VersionId)

		  Insert into ZnodePublishConfigurableProductEntity 
		  (VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder,    
		  SelectValues,ConfigurableAttributeCodes, IsDefault,ElasticSearchEvent )    
		  SELECT DISTINCT TBP.VersionId, TBP.PublishProductId ,    
			  TBP.PublishCatalogId , TBP.AssociatedZnodeProductId,TBP.DisplayOrder ,TBP.SelectValues,          
			  TBP.ConfigurableAttributeCodes, TBP.IsDefault  ,1 as ElasticSearchEvent
		 FROM #TempConfigurableProductEntity TBP    
		 WHERE NOT EXISTS(SELECT * FROM ZnodePublishConfigurableProductEntity ZPCP WHERE ZPCP.VersionId = TBP.VersionId 
			AND ZPCP.ZnodeProductId = TBP.PublishProductId AND ZPCP.ZnodeCatalogId = TBP.PublishCatalogId AND ZPCP.AssociatedZnodeProductId = TBP.AssociatedZnodeProductId and isnull(ZPCP.ElasticSearchEvent,0) <> 2)
		 

		 Update ZPC SET ZPC.ElasticSearchEvent =2 from ZnodePublishConfigurableProductEntity  ZPC  Where Exists 
		 (Select TOP 1 1 From #TempConfigurableProductEntity X Where ZPC.ZnodeProductId = X.PublishProductId and ZPC.ZnodeCatalogId = X.PublishCatalogId)
		 AND NOT Exists 
		(Select TOP 1 1 From #TempConfigurableProductEntity XP Where ZPC.ZnodeProductId = XP.PublishProductId and ZPC.ZnodeCatalogId = XP.PublishCatalogId
		 and XP.AssociatedZnodeProductId = ZPC.AssociatedZnodeProductId)
		 AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPC.VersionId)

      IF OBJECT_ID('tempdb..#TBL_PublishCatalogId') is not null    
       drop table #TBL_PublishCatalogId    
    
      IF OBJECT_ID('tempdb..#ConfigurableProductPublishedXML') is not null    
       drop table #ConfigurableProductPublishedXML    
    
End 

	  DELETE FROM ZnodePublishBundleProductEntity WHERE ISNULL(ElasticSearchEvent,0) = 2  
	  DELETE FROM ZnodePublishGroupProductEntity WHERE ISNULL(ElasticSearchEvent,0) = 2    
	  DELETE FROM ZnodePublishConfigurableProductEntity WHERE ISNULL(ElasticSearchEvent,0) = 2 

COMMIT TRAN GetPublishAssociatedProducts;    
SET @Status = 1;    
END TRY    
	BEGIN CATCH    
	SELECT ERROR_MESSAGE()    
	SET @Status = 0;    
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAssociatedProductsJson @PublishCatalogId = '+@PublishCatalogId+',@ProductType= '+@ProductType+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
	ROLLBACK TRANSACTION GetPublishAssociatedProducts;    
	EXEC Znode_InsertProcedureErrorLog    
	@ProcedureName = 'Znode_GetPublishAssociatedProductsJson',    
	@ErrorInProcedure = @Error_procedure,    
	@ErrorMessage = @ErrorMessage,    
	@ErrorLine = @ErrorLine,    
	@ErrorCall = @ErrorCall;    
END CATCH;    
END;