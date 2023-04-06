
CREATE PROCEDURE [dbo].[Znode_GetPublishAssociatedProductsInnerOut]
(   
	@PublishCatalogId VARCHAR(MAX) = '',
    @PimProductId     VARCHAR(MAX) = '',
    @ProductType      VARCHAR(300) = 'BundleProduct',
    @VersionId        INT          = 0,
    @UserId           INT
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
    EXEC [dbo].[Znode_GetPublishAssociatedProducts_2] @PublishCatalogId = 3 ,@PimProductId = '' , @ProductType = 'BundleProduct' ,@userId = 2 
    EXEC [dbo].[Znode_GetPublishAssociatedProducts] @PublishCatalogId =12 ,@PimProductId ='' , @ProductType = 'ConfigurableProduct',@userId = 2 
    Group Product
    EXEC [dbo].[Znode_GetPublishAssociatedProducts]  @PublishCatalogId ='2',@PimProductIdh =''  , @PimProducttType = 'GroupedProduct'
    EXEC [dbo].[Znode_GetPublishAssociatedProducts]  @PublishCatalogId ='',@PimProductId ='200066'  , @PimProducttType = 'GroupedProduct'
    EXEC [dbo].[Znode_GetPublishAssociatedProductsInnerOut] @PublishCatalogId =3  , @ProductType = 'BundleProduct',@VersionId =  0 ,@UserId = 2 
						, @PimProductId='1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,80,84,89,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,113,114'
   */
     BEGIN
         BEGIN TRAN GetPublishAssociatedProducts;
         BEGIN TRY
             SET NOCOUNT ON;
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
             DECLARE @TBL_PublishCatalogId TABLE(PublishCatalogId INT);
             DECLARE @TBL_PimAssociatedEntity TABLE
             (
			  ZnodeProductId                  INT,
              ZnodeCatalogId                  INT,
              AsscociadedZnodeProductIds  VARCHAR(MAX),
			  ConfigurableProductEntity       NVARCHAR(MAX),
              LocaleId                        INT,
			  DisplayOrder					  INT,
              VersionId                       INT
             );
			 DECLARE @TBL_ProductIdDescription  TABLE (PublishCatalogId INT ,PublishProductId INT ,AsscociadedZnodeProductIds VARCHAR(max),ConfigurableProductEntity NVARCHAR(max),DisplayOrder int)
			 DECLARE @TBL_ConfigAttribute TABLE (PublishCatalogId INT, PublishProductId INT , ConfigXML NVARCHAR(max) )
             
			INSERT INTO @TBL_PublishCatalogId(PublishCatalogId) 
			SELECT [Item] FROM [dbo].[Split](@PublishCatalogId, ',');

            INSERT INTO @TBL_PimProductId(PimProductId, PublishCatalogId, PublishProductId)
            SELECT PimProductid,PublishCatalogId,PublishProductId
			FROM ZnodePublishProduct ZPP 
			WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Split](@PimProductId, ',') SP 
			WHERE Sp.Item = ZPP.PimProductid) 
			AND EXISTS(SELECT TOP 1 1 FROM View_LoadManageProduct VILMP WHERE VILMP.PimProductId = ZPP.PimProductId
			AND VILMP.AttributeCode = [dbo].[Fn_GetProductTypeAttributeCode]()
			AND VILMP.AttributeValue = @ProductType)
			AND PublishCatalogId = @PublishCatalogId;

			INSERT INTO @TBL_ProductIdDescription(PublishCatalogId  ,PublishProductId  ,AsscociadedZnodeProductIds,ConfigurableProductEntity ,DisplayOrder )
			 SELECT TBP.PublishCatalogId,TBP.PublishProductId,ZPP.PublishProductId AsscociadedZnodeProductIds,ZPA.AttributeCode ConfigurableProductEntity,ZPTA.DisplayOrder FROM @TBL_PimProductId TBP
             INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
             INNER JOIN ZnodePublishProduct ZPP ON(Zpp.PimProductId = ZPTA.PimProductId AND ZPP.PublishCatalogId = TBP.PublishCatalogId)
             LEFT JOIN ZnodePimConfigureProductAttribute ZPCPA ON (ZPCPA.PimProductId = TBP.PimProductId)
             LEFT JOIN ZnodePimAttribute ZPA ON (Zpa.PimAttributeId = ZPCPA.PimAttributeId) WHERE((ZPTA.PimAttributeId = [dbo].[Fn_GetProductTypeAttributeId]()
             OR ZPTA.PimAttributeId IS NULL) OR EXISTS(SELECT TOP 1 1FROM ZnodePimConfigureProductAttribute ZPCPA WHERE ZPTA.PimParentProductId = ZPCPA.PimProductId
             AND @ProductType = 'ConfigurableProduct')) GROUP BY TBP.PublishCatalogId, TBP.PublishProductId,ZPP.PublishProductId,ZPA.AttributeCode,ZPTA.DisplayOrder						   
						   

             INSERT INTO @TBL_PimAssociatedEntity(ZnodeCatalogId,ZnodeProductId,AsscociadedZnodeProductIds,DisplayOrder)
             SELECT PublishCatalogId,PublishProductId,AsscociadedZnodeProductIds AsscociadedZnodeProductIds,DisplayOrder								
             FROM @TBL_ProductIdDescription CTPD GROUP BY PublishCatalogId,PublishProductId,AsscociadedZnodeProductIds,DisplayOrder;

             INSERT INTO @TBL_ConfigAttribute (PublishCatalogId,PublishProductId,ConfigXML)
             SELECT PublishCatalogId,PublishProductId, (SELECT DISTINCT  ConfigurableProductEntity ConfigurableAttributeCode FROM @TBL_ProductIdDescription CTPDI
                                                        WHERE CTPD.PublishCatalogId = CTPDI.PublishCatalogId AND CTPD.PublishProductId = CTPDI.PublishProductId
                                                        FOR XML PATH('ConfigurableAttributeCodes'))ConfigurableProductEntity 
			 FROM @TBL_ProductIdDescription CTPD GROUP BY PublishCatalogId,PublishProductId;
				
             UPDATE TBAE SET VersionId = ZPCL.PublishCatalogLogId FROM @TBL_PimAssociatedEntity TBAE INNER JOIN ZnodePublishCatalogLog ZPCL
			 ON(ZPCL.PublishCatalogLogId = (SELECT MAX(ZPCLI.PublishCatalogLogId) FROM ZnodePublishCatalogLog ZPCLI WHERE ZPCLI.PublishcatalogId = TBAE.ZnodeCatalogId))
             WHERE @PublishCatalogId = '';

             IF @ProductType = 'BundleProduct'
                 BEGIN
                     INSERT INTO @TBL_ProductTypeXML (PublishCatalogId,PublishProductId,ReturnXML,VersionId)
                     SELECT ZnodeCatalogId,ZnodeProductId,(SELECT ZnodeCatalogId,ZnodeProductId,AsscociadedZnodeProductIds AssociatedZnodeProductId,DisplayOrder AssociatedProductDisplayOrder,ISNULL(TBPAEI.VersionId, @VersionId) VersionId
                     FROM @TBL_PimAssociatedEntity TBPAEI 
					 WHERE TBPAEI.ZnodeCatalogId = TBPAE.ZnodeCatalogId AND TBPAE.ZnodeProductId = TBPAEI.ZnodeProductId 	 AND TBPAEI.AsscociadedZnodeProductIds = TBPAE.AsscociadedZnodeProductIds
					  FOR XML PATH(''), ROOT('BundleProductEntity'))
					 ReturnXML,ISNULL(TBPAE.VersionId, @VersionId) VersionId 
					 FROM @TBL_PimAssociatedEntity TBPAE;
                                                               
                 END;
             ELSE
             IF @ProductType = 'GroupedProduct'
                 BEGIN
                     INSERT INTO @TBL_ProductTypeXML(PublishCatalogId,PublishProductId,ReturnXML,VersionId)
                     SELECT ZnodeCatalogId,ZnodeProductId,(SELECT ZnodeCatalogId,ZnodeProductId,AsscociadedZnodeProductIds AssociatedZnodeProductId,DisplayOrder AssociatedProductDisplayOrder,ISNULL(TBPAEI.VersionId, @VersionId) VersionId
                     FROM @TBL_PimAssociatedEntity TBPAEI WHERE TBPAEI.ZnodeCatalogId = TBPAE.ZnodeCatalogId AND TBPAE.ZnodeProductId = TBPAEI.ZnodeProductId 
					 AND TBPAEI.AsscociadedZnodeProductIds = TBPAE.AsscociadedZnodeProductIds
					 FOR XML PATH(''), ROOT('GroupProductEntity'))
					 ReturnXML,ISNULL(TBPAE.VersionId, @VersionId) VersionId 
					 FROM @TBL_PimAssociatedEntity TBPAE;
                 END;
             ELSE
             IF @ProductType = 'ConfigurableProduct'
                 BEGIN
                     INSERT INTO @TBL_ProductTypeXML(PublishCatalogId,PublishProductId,ReturnXML,VersionId)
                     SELECT ZnodeCatalogId,ZnodeProductId, REPLACE(CAST((SELECT ZnodeCatalogId,ZnodeProductId,AsscociadedZnodeProductIds AssociatedZnodeProductId,DisplayOrder AssociatedProductDisplayOrder,ISNULL(TBPAEI.VersionId, @VersionId) VersionId
                     FROM @TBL_PimAssociatedEntity TBPAEI WHERE TBPAEI.ZnodeCatalogId = TBPAE.ZnodeCatalogId AND TBPAE.ZnodeProductId = TBPAEI.ZnodeProductId AND TBPAEI.AsscociadedZnodeProductIds = TBPAE.AsscociadedZnodeProductIds
                     FOR XML PATH(''), ROOT('ConfigurableProductEntity')) AS NVARCHAR(max) ),'</ConfigurableProductEntity>' , ConfigXML+'</ConfigurableProductEntity>' )  ReturnXML,
                     ISNULL(TBPAE.VersionId, @VersionId) VersionId FROM @TBL_PimAssociatedEntity TBPAE
                     LEFT JOIN @TBL_ConfigAttribute TBT ON (TBT.PublishCatalogId = TBPAE.ZnodeCatalogId AND TBPAE.ZnodeProductId = TBT.PublishProductId );
                 END;

				 --DELETE FROM ZnodePublishedXML WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_ProductTypeXML )

				 IF  @VersionId <> 0  
				 BEGIN 

				 MERGE INTO ZnodePublishedXML TARGET 
				 USING @TBL_ProductTypeXML SOURCE
				 ON (
				     TARGET.PublishCatalogLogId  = SOURCE.VersionId
					 AND TARGET.PublishedId = SOURCE.PublishProductId 
					 AND TARGET.IsGroupProductXML =  CASE WHEN @ProductType = 'GroupedProduct' THEN 1 ELSE 0 END 
					 AND TARGET.IsBundleProductXML =  CASE WHEN @ProductType = 'BundleProduct' THEN 1 ELSE 0 END 
					 AND TARGET.IsConfigProductXML = CASE WHEN @ProductType = 'ConfigurableProduct' THEN 1 ELSE 0 END 
					
				 )
				 WHEN MATCHED THEN 
				 UPDATE 
				 SET TARGEt.PublishedXML =  SOURCE.ReturnXML
				      ,TARGET.ModifiedBy = @UserId 
					  ,TARGET.ModifiedDate = GETDATE()
				WHEN NOT MATCHED THEN 
				INSERT (PublishCatalogLogId,PublishedId,PublishedXML,IsGroupProductXML,IsBundleProductXML,IsConfigProductXML
							,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)

				VALUES (SOURCE.VersionId,SOURCE.PublishProductId,SOURCE.ReturnXML,CASE WHEN @ProductType = 'GroupedProduct' THEN 1 ELSE 0 END 
						,CASE WHEN @ProductType = 'BundleProduct' THEN 1 ELSE 0 END 
						,CASE WHEN @ProductType = 'ConfigurableProduct' THEN 1 ELSE 0 END 
						,@UserId,GETDATE()
						,@UserId ,GETDATE())
				--WHEN NOT MATCHED BY SOURCE AND EXISTS (SELECT TOP 1 1 FROM @TBL_ProductTypeXML TBL 
				--			WHERE  TARGET.PublishCatalogLogId  = TBL.VersionId AND TARGET.PublishedId = TBL.PublishProductId 
				--			 AND TARGET.IsGroupProductXML =  CASE WHEN @ProductType = 'GroupedProduct' THEN 1 ELSE 0 END 
				--			 AND TARGET.IsBundleProductXML =  CASE WHEN @ProductType = 'BundleProduct' THEN 1 ELSE 0 END 
				--			AND TARGET.IsConfigProductXML = CASE WHEN @ProductType = 'ConfigurableProduct' THEN 1 ELSE 0 END 
				--			 ) THEN 
				--DELETE 
				;
				END 
             COMMIT TRAN GetPublishAssociatedProducts;
			
         END TRY
         BEGIN CATCH
		 SELECT ERROR_MESSAGE()
            DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAssociatedProducts @PublishCatalogId = '+@PublishCatalogId+',@PimProductId='+@PimProductId+',@ProductType= '+@ProductType+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			ROLLBACK TRANSACTION GetPublishAssociatedProducts;
			EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPublishAssociatedProducts',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;