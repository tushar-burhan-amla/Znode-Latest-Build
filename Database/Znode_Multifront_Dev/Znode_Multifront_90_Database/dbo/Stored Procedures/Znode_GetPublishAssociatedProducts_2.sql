CREATE  PROCEDURE [dbo].[Znode_GetPublishAssociatedProducts_2]
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
    EXEC [dbo].[Znode_GetPublishAssociatedProducts] @PimProductId ='200066'  , @PimProducttType = 'GroupedProduct'
   */
     BEGIN
         BEGIN TRAN GetPublishAssociatedProducts;
         BEGIN TRY
             SET NOCOUNT ON;

			  DECLARE @TBL_PublishCatalogId TABLE(PublishCatalogId INT,PublishProductId INT,PimProductId  INT , VersionId INT );
			  DECLARE @PimAttributeId INT = [dbo].[Fn_GetProductTypeAttributeId]()
					  ,@PimAttributeDefaultValueId INT = (SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue WHERE AttributeDefaultValueCode = @ProductType)
					,@DefaultLocaleId INT = dbo.fn_getDefaultlocaleId() 
			 DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
			 INSERT INTO @TBL_PublishCatalogId 
			 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,CASE WHEN @VersionId = 0 OR @VersionId IS NULL THEN  MAX(PublishCatalogLogId) ELSE @VersionId END 
			 FROM ZnodePublishProduct ZPP 
			 INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )
			 INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)
			 LEFT JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
			 WHERE (EXISTS (SELECT TOP 1 1 FROM dbo.Split(@PimProductId,',') SP WHERE SP.Item = ZPP.PimProductId  AND  @PublishCatalogId = '' ) 
			 OR (ZPP.PublishCatalogId =  @PublishCatalogId ))
			 AND ZPV.PimAttributeId  = @PimAttributeId
			 AND ZPAVL.PimAttributeDefaultValueId= @PimAttributeDefaultValueId
			 AND LocaleId = @DefaultLocaleId
			 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId
			
			--SELECT * FROM @TBL_PublishCatalogId

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
			  ZnodeProductId                  INT,
              ZnodeCatalogId                  INT,
              AsscociadedZnodeProductIds  VARCHAR(MAX),
			  ConfigurableProductEntity       NVARCHAR(MAX),
              LocaleId                        INT,
			  DisplayOrder					  INT,
              VersionId                       INT
             );
			

             IF @ProductType = 'BundleProduct'
                 BEGIN
                     DELETE FROM ZnodePublishedXML 
								WHERE  IsBundleProductXML = 1 
								AND EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId TBLPC WHERE TBLPC.VersionId = PublishCatalogLogId  )

				 	 MERGE INTO ZnodePublishedXML TARGET 
					 USING (        
				     SELECT TBP.PublishProductId, TBP.VersionId, '<BundleProductEntity><VersionId>'+CAST(TBP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeCatalogId>'+CAST(TBP.PublishCatalogId AS VARCHAR(50))+'</ZnodeCatalogId><ZnodeProductId>'
					 +CAST(TBP.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><AssociatedZnodeProductId>'
					 +CAST(TBPU.PublishProductId AS VARCHAR(50))+'</AssociatedZnodeProductId><AssociatedProductDisplayOrder>'+CAST(ZPTA.DisplayOrder AS VARCHAR(50))+'</AssociatedProductDisplayOrder></BundleProductEntity>' ReturnXML 
					 FROM @TBL_PublishCatalogId TBP
					 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
					 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
					 ) SOURCE 
					ON (
								 TARGET.PublishCatalogLogId = SOURCE.versionId 
								 AND TARGET.PublishedId = SOURCE.PublishProductId
								 AND TARGET.IsBundleProductXML = 1 
								 AND TARGET.LocaleId = 0 
							)
							WHEN MATCHED THEN 
							UPDATE 
							SET  PublishedXML = ReturnXML
							   , ModifiedBy = @userId 
							   ,ModifiedDate = @GetDate
							WHEN NOT MATCHED THEN 
							INSERT (PublishCatalogLogId
							,PublishedId
							,PublishedXML
							,IsBundleProductXML
							,LocaleId
							,CreatedBy
							,CreatedDate
							,ModifiedBy
							,ModifiedDate)

							VALUES (SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate);
						                                         
                 END;
             ELSE
             IF @ProductType = 'GroupedProduct'
                 BEGIN
				       DELETE FROM ZnodePublishedXML 
								WHERE  IsGroupProductXML = 1 
								AND EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId TBLPC WHERE TBLPC.VersionId = PublishCatalogLogId  )

				 	 MERGE INTO ZnodePublishedXML TARGET 
					 USING (  
				     SELECT TBP.PublishProductId, TBP.VersionId, '<GroupProductEntity><VersionId>'+CAST(TBP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeCatalogId>'+CAST(TBP.PublishCatalogId AS VARCHAR(50))+'</ZnodeCatalogId><ZnodeProductId>'
					 +CAST(TBP.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><AssociatedZnodeProductId>'
					 +CAST(TBPU.PublishProductId AS VARCHAR(50))+'</AssociatedZnodeProductId><AssociatedProductDisplayOrder>'+CAST(ZPTA.DisplayOrder AS VARCHAR(50))+'</AssociatedProductDisplayOrder></GroupProductEntity>'  ReturnXML
					 FROM @TBL_PublishCatalogId TBP
					 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
					 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
					  ) SOURCE 
					ON (
								 TARGET.PublishCatalogLogId = SOURCE.versionId 
								 AND TARGET.PublishedId = SOURCE.PublishProductId
								 AND TARGET.IsGroupProductXML = 1 
								 AND TARGET.LocaleId = 0 
							)
							WHEN MATCHED THEN 
							UPDATE 
							SET  PublishedXML = ReturnXML
							   , ModifiedBy = @userId 
							   ,ModifiedDate = @GetDate
							WHEN NOT MATCHED THEN 
							INSERT (PublishCatalogLogId
							,PublishedId
							,PublishedXML
							,IsGroupProductXML
							,LocaleId
							,CreatedBy
							,CreatedDate
							,ModifiedBy
							,ModifiedDate)

							VALUES (SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate);
    
                 END;
             ELSE
             IF @ProductType = 'ConfigurableProduct'
                 BEGIN
				     DELETE FROM ZnodePublishedXML 
								WHERE  IsConfigProductXML = 1 
								AND EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId TBLPC WHERE TBLPC.VersionId = PublishCatalogLogId  )

				 	 MERGE INTO ZnodePublishedXML TARGET 
					 USING (  
				     SELECT TBP.PublishProductId, TBP.VersionId, '<ConfigurableProductEntity><VersionId>'+CAST(TBP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeCatalogId>'+CAST(TBP.PublishCatalogId AS VARCHAR(50))+'</ZnodeCatalogId><ZnodeProductId>'
					 +CAST(TBP.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><AssociatedZnodeProductId>'
					 +CAST(TBPU.PublishProductId AS VARCHAR(50))+'</AssociatedZnodeProductId><AssociatedProductDisplayOrder>'+CAST(ZPTA.DisplayOrder AS VARCHAR(50))+'</AssociatedProductDisplayOrder>'
					 +(SELECT DISTINCT  ZPA.AttributeCode ConfigurableAttributeCode 
														FROM ZnodePimConfigureProductAttribute ZPCPA 
														LEFT JOIN ZnodePimAttribute ZPA ON (Zpa.PimAttributeId = ZPCPA.PimAttributeId) 
                                                        WHERE  ZPCPA.PimProductId = TBP.PimProductId 
                                                        FOR XML PATH('ConfigurableAttributeCodes')) +'</ConfigurableProductEntity>'  ReturnXML
					 FROM @TBL_PublishCatalogId TBP
					 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
					 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
				    ) SOURCE 
					ON (
								 TARGET.PublishCatalogLogId = SOURCE.versionId 
								 AND TARGET.PublishedId = SOURCE.PublishProductId
								 AND TARGET.IsConfigProductXML = 1 
								 AND TARGET.LocaleId = 0 
							)
							WHEN MATCHED THEN 
							UPDATE 
							SET  PublishedXML = ReturnXML
							   , ModifiedBy = @userId 
							   ,ModifiedDate = @GetDate
							WHEN NOT MATCHED THEN 
							INSERT (PublishCatalogLogId
							,PublishedId
							,PublishedXML
							,IsConfigProductXML
							,LocaleId
							,CreatedBy
							,CreatedDate
							,ModifiedBy
							,ModifiedDate)

							VALUES (SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate);
    
                 END;

				 SELECT PublishedXML RetunXML
				 FROM ZnodePublishedXML  ZPXM 
				 WHERE IsConfigProductXML = CASE WHEN @ProductType = 'ConfigurableProduct' THEN  1 ELSE 0 END 
				 AND IsGroupProductXML = CASE WHEN @ProductType = 'GroupedProduct' THEN  1 ELSE 0 END 
				 AND IsBundleProductXML = CASE WHEN @ProductType = 'BundleProduct' THEN  1 ELSE 0 END 
				 AND EXISTS (SELECT TOP 1 1 FROM @TBL_PublishCatalogId TBLP WHERE TBLP.VersionId = ZPXM.PublishCatalogLogId )


				-- SELECT ReturnXML FROM @TBL_ProductTypeXML;
		
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