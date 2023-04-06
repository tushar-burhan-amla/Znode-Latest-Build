CREATE PROCEDURE [dbo].[Znode_GetPublishAssociatedProducts]
(   
	@PublishCatalogId VARCHAR(MAX) = '',
    @PimProductId     TransferId Readonly,
    @ProductType      VARCHAR(300) = 'BundleProduct',
    @VersionId        INT          = 0,
    @UserId           INT,
	@PimCategoryHierarchyId int = 0,
	@PublishStateId INT = 0  
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
			 
			 IF OBJECT_ID('tempdb..#TBL_PublishCatalogId') is not null
				DROP TABLE #TBL_PublishCatalogId
			
			  CREATE TABLE #TBL_PublishCatalogId (PublishCatalogId INT,PublishProductId INT,PimProductId  INT , VersionId INT,LocaleId INT  );
			  DECLARE @PimAttributeId INT = [dbo].[Fn_GetProductTypeAttributeId]()
					  ,@PimAttributeDefaultValueId INT = (SELECT TOP 1 PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue WHERE AttributeDefaultValueCode = @ProductType)
					,@DefaultLocaleId INT = dbo.fn_getDefaultlocaleId() 
			 DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
			 
			 DECLARE @TBL_PublisshIds TABLE (PublishProductId INT , PimProductId INT , PublishCatalogId INT)
			 DECLARE  @PimProductId_New TransferId
					 
			 IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			 BEGIN 
			   INSERT INTO @TBL_PublisshIds 
			   EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1
			   
			   --SET @PimProductId = SUBSTRING((SELECT DISTINCT ','+CAST(PimProductId AS VARCHAr(50)) FROM @TBL_PublisshIds FOR XML PATH ('')),2,8000 )

			   INSERT INTO @PimProductId_New
			   SELECT DISTINCT PimProductId FROM @TBL_PublisshIds

			  -- SELECT 	@PimProductId	
			 END 
			 
			 IF  ISnull(@PimCategoryHierarchyId,0) <> 0 
			 BEGIN 
			 
			   INSERT INTO @TBL_PublisshIds 
			   EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1,@PimCategoryHierarchyId
			   
			   --SET @PimProductId = SUBSTRING((SELECT DISTINCT ','+CAST(PimProductId AS VARCHAr(50)) FROM @TBL_PublisshIds FOR XML PATH ('')),2,8000 )

			   INSERT INTO @PimProductId_New
			   SELECT PimProductId FROM @TBL_PublisshIds

			   
			 END 

			

			 IF  ISnull(@PimCategoryHierarchyId,0) <> 0 
			 BEGIN 
				 INSERT INTO #TBL_PublishCatalogId 
				 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,MAX(ZPCP.PublishCatalogLogId) ,ZPCP.LocaleID  
				 FROM ZnodePublishProduct ZPP 
				 INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )
				 INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)
				 LEFT JOIN  ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId   ) 
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
					 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,  MAX(ZPCP.PublishCatalogLogId) PublishCatalogLogId	,ZPCP.LocaleID 
					 FROM ZnodePublishProduct ZPP 
					 INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )
					 INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)
					 LEFT JOIN  ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId AND ISNULL(ZPCP.LocaleId,0) <> 0 )			 			 
					 WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  AND  @PublishCatalogId = '' ) 
					 OR (ZPP.PublishCatalogId =  @PublishCatalogId ))
					 AND ZPV.PimAttributeId  = @PimAttributeId
					 AND ZPAVL.PimAttributeDefaultValueId= @PimAttributeDefaultValueId
					 AND ZPAVL.LocaleId = @DefaultLocaleId
					 AND EXISTS(SELECT * FROM ZnodeLocale ZL where ZL.IsActive = 1  and ZPCP.LocaleId = ZL.LocaleId )
					 --AND ISNULL(ZPCP.LocaleId,0) <> 0 
					 --AND CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM @PimProductId ) AND @PublishCatalogId <> 0   THEN  @PublishStateId ELSE  ZPCP.Publishstateid END  = @PublishStateId
					 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,ZPCP.LocaleID
				END
				ELSE
				BEGIN
					 INSERT INTO #TBL_PublishCatalogId 
					 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,  MAX(ZPCP.PublishCatalogLogId) PublishCatalogLogId	,ZPCP.LocaleID 
					 FROM ZnodePublishProduct ZPP 
					 INNER JOIN ZnodePimAttributeValue ZPV ON (ZPV.PimProductId = ZPP.PimProductId )
					 INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = ZPV.PimAttributeValueId)
					 LEFT JOIN  ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId AND ISNULL(ZPCP.LocaleId,0) <> 0 )			 			 
					 WHERE (EXISTS (SELECT TOP 1 1 FROM @TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  AND  @PublishCatalogId = '' ) 
					 OR (ZPP.PublishCatalogId =  @PublishCatalogId ))
					 AND ZPV.PimAttributeId  = @PimAttributeId
					 AND ZPAVL.PimAttributeDefaultValueId= @PimAttributeDefaultValueId
					 AND ZPAVL.LocaleId = @DefaultLocaleId
					 AND EXISTS(SELECT * FROM ZnodeLocale ZL where ZL.IsActive = 1  and ZPCP.LocaleId = ZL.LocaleId )
					 --AND ISNULL(ZPCP.LocaleId,0) <> 0 
					 --AND CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM @PimProductId ) AND @PublishCatalogId <> 0   THEN  @PublishStateId ELSE  ZPCP.Publishstateid END  = @PublishStateId
					 AND ZPCP.Publishstateid = @PublishStateId
					 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,ZPP.PimProductId,ZPCP.LocaleID
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
			  ZnodeProductId                  INT,
              ZnodeCatalogId                  INT,
              AsscociadedZnodeProductIds  VARCHAR(MAX),
			  ConfigurableProductEntity       NVARCHAR(MAX),
              LocaleId                        INT,
			  DisplayOrder					  INT,
              VersionId                       INT
             );
			
		
		     SET @versionid  =(SELECT TOP 1 VersionId FROM #TBL_PublishCatalogId TBLV )

             IF @ProductType = 'BundleProduct'
             BEGIN
				    
					IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			        BEGIN 
			 		 
						 DELETE FROM ZnodePublishedXML WHERE  IsBundleProductXML = 1 
						 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )

					 END 
					 ELSE 
					 BEGIN 
				 
						 DELETE FROM ZnodePublishedXML WHERE  IsBundleProductXML = 1 
						 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )
			           
					 END 

					 IF OBJECT_ID('tempdb..#BundleProductPublishedXML') is not null
						drop table #BundleProductPublishedXML

					 SELECT TBP.PublishProductId, TBP.VersionId, '<BundleProductEntity><VersionId>'+CAST(TBP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeCatalogId>'+CAST(TBP.PublishCatalogId AS VARCHAR(50))+'</ZnodeCatalogId><ZnodeProductId>'
					 +CAST(TBP.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><AssociatedZnodeProductId>'
					 +CAST(TBPU.PublishProductId AS VARCHAR(50))+'</AssociatedZnodeProductId><AssociatedProductDisplayOrder>'+CAST(ISNULL(ZPTA.DisplayOrder,0) AS VARCHAR(50))+'</AssociatedProductDisplayOrder></BundleProductEntity>' ReturnXML 
					 INTO #BundleProductPublishedXML
					 FROM #TBL_PublishCatalogId TBP
					 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
					 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
					 
					 UPDATE TARGET
					 SET  PublishedXML = ReturnXML
						 ,ModifiedBy = @userId 
						 ,ModifiedDate = @GetDate
					 FROM ZnodePublishedXML TARGET
					 INNER JOIN #BundleProductPublishedXML SOURCE ON TARGET.PublishCatalogLogId = SOURCE.versionId AND TARGET.PublishedId = SOURCE.PublishProductId
																	AND TARGET.IsBundleProductXML = 1 AND TARGET.LocaleId = 0 

				 	 INSERT  INTO ZnodePublishedXML(PublishCatalogLogId ,PublishedId ,PublishedXML ,IsBundleProductXML ,LocaleId ,CreatedBy ,CreatedDate ,ModifiedBy ,ModifiedDate)
					 SELECT SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate
					 FROM #BundleProductPublishedXML SOURCE
					 WHERE NOT EXISTS(select * from ZnodePublishedXML TARGET where TARGET.PublishCatalogLogId = SOURCE.versionId AND TARGET.PublishedId = SOURCE.PublishProductId
																	AND TARGET.IsBundleProductXML = 1 AND TARGET.LocaleId = 0 )
                                         
             END;
             ELSE
             IF @ProductType = 'GroupedProduct'
             BEGIN
				  
				     IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			         BEGIN 
			 		 
						 DELETE FROM ZnodePublishedXML WHERE  IsGroupProductXML = 1 
						 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )

					 END 
					 ELSE 
					 BEGIN 					 

						 DELETE FROM ZnodePublishedXML WHERE  IsGroupProductXML = 1 
						 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )
			           
					 END 
				     
					 IF OBJECT_ID('tempdb..#GroupedProductPublishedXML') is not null
						drop table #GroupedProductPublishedXML

					 SELECT TBP.PublishProductId, TBP.VersionId, '<GroupProductEntity><VersionId>'+CAST(TBP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeCatalogId>'+CAST(TBP.PublishCatalogId AS VARCHAR(50))+'</ZnodeCatalogId><ZnodeProductId>'
					 +CAST(TBP.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><AssociatedZnodeProductId>'
					 +CAST(TBPU.PublishProductId AS VARCHAR(50))+'</AssociatedZnodeProductId><AssociatedProductDisplayOrder>'+CAST(ISNULL(ZPTA.DisplayOrder,0) AS VARCHAR(50))+'</AssociatedProductDisplayOrder></GroupProductEntity>'  ReturnXML
					 INTO #GroupedProductPublishedXML
					 FROM #TBL_PublishCatalogId TBP
					 INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
					 INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )

					 UPDATE TARGET
					 SET PublishedXML = ReturnXML
						 ,ModifiedBy = @userId 
						 ,ModifiedDate = @GetDate
					 FROM ZnodePublishedXML TARGET
					 INNER JOIN #GroupedProductPublishedXML SOURCE ON TARGET.PublishCatalogLogId = SOURCE.versionId AND TARGET.PublishedId = SOURCE.PublishProductId
																	AND TARGET.IsGroupProductXML = 1 AND TARGET.LocaleId = 0 

				 	 INSERT  INTO ZnodePublishedXML(PublishCatalogLogId ,PublishedId ,PublishedXML ,IsGroupProductXML ,LocaleId ,CreatedBy ,CreatedDate ,ModifiedBy ,ModifiedDate)
					 SELECT SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate
					 FROM #GroupedProductPublishedXML SOURCE
					 WHERE NOT EXISTS(select * from ZnodePublishedXML TARGET where TARGET.PublishCatalogLogId = SOURCE.versionId AND TARGET.PublishedId = SOURCE.PublishProductId
										AND TARGET.IsGroupProductXML = 1 AND TARGET.LocaleId = 0 )

             END;
             ELSE
             IF @ProductType = 'ConfigurableProduct'
             BEGIN
					IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
					BEGIN 	
						 		 
						DELETE FROM ZnodePublishedXML WHERE  IsConfigProductXML = 1 
						AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )
		        
					END 
					ELSE 
					BEGIN 						
							DELETE FROM ZnodePublishedXML WHERE  IsConfigProductXML = 1 
							AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV 
							WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   
							AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )
			           
							;WITH CTE_ConfigProductXML as
							(
								SELECT DISTINCT TBP.PublishProductId , TBP.VersionId, TBPU.PublishProductId as AssociatedZnodeProductId
								FROM #TBL_PublishCatalogId TBP
								INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
								INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
							)
							,CTE_PublishedXML as
							(
								SELECT ZPX.PublishCatalogLogId,ZPX.PublishedId,ZPX.IsAddonXML,ZPX.IsConfigProductXML, p.value('(./AssociatedZnodeProductId)[1]', 'INT')  as AssociatedZnodeProductId
								FROM ZnodePublishedXML ZPX
								CROSS APPLY ZPX.PublishedXML.nodes('/ConfigurableProductEntity') t(p)
								where ZPX.IsConfigProductXML = 1
							)
							DELETE ZPXML  
							FROM ZnodePublishedXML ZPXML
							INNER JOIN CTE_PublishedXML CPX	ON ZPXML.PublishCatalogLogId = CPX.PublishCatalogLogId AND ZPXML.PublishedId = CPX.PublishedId AND ZPXML.IsConfigProductXML = CPX.IsConfigProductXML		
							INNER JOIN CTE_ConfigProductXML CAX on CPX.PublishCatalogLogId = CAX.VersionId 
							AND CPX.PublishedId = CAX.PublishProductId
							AND CPX.IsConfigProductXML = 1
							AND CPX.AssociatedZnodeProductId = CAX.AssociatedZnodeProductId
					 END 
						IF OBJECT_ID('tempdb..#ConfigurableProductPublishedXML') is not null
							drop table #ConfigurableProductPublishedXML

						SELECT DISTINCT TBP.PublishProductId , TBP.VersionId, '<ConfigurableProductEntity><VersionId>'+CAST(TBP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeCatalogId>'+CAST(TBP.PublishCatalogId AS VARCHAR(50))+'</ZnodeCatalogId><ZnodeProductId>'
						+CAST(TBP.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><AssociatedZnodeProductId>'
						+CAST(TBPU.PublishProductId AS VARCHAR(50))+'</AssociatedZnodeProductId><AssociatedProductDisplayOrder>'+CAST(ISNULL(ZPTA.DisplayOrder,0) AS VARCHAR(50))+'</AssociatedProductDisplayOrder>'
						+(SELECT DISTINCT  ZPA.AttributeCode ConfigurableAttributeCode 
														FROM ZnodePimConfigureProductAttribute ZPCPA 
														LEFT JOIN ZnodePimAttribute ZPA ON (Zpa.PimAttributeId = ZPCPA.PimAttributeId) 
														WHERE  ZPCPA.PimProductId = TBP.PimProductId 
														FOR XML PATH('ConfigurableAttributeCodes')) +'</ConfigurableProductEntity>'  ReturnXML
						INTO #ConfigurableProductPublishedXML
						FROM #TBL_PublishCatalogId TBP
						INNER JOIN ZnodePimProductTypeAssociation ZPTA ON(ZPTA.PimParentProductId = TBP.PimProductId)
						INNER JOIN ZnodePublishProduct TBPU ON (TBPU.PimProductId = ZPTA.PimProductId AND TBPU.PublishCatalogId = TBP.PublishCatalogId )
			
						UPDATE TARGET
						SET PublishedXML = ReturnXML
								, ModifiedBy = @userId 
								,ModifiedDate = @GetDate
						FROM ZnodePublishedXML TARGET
						INNER JOIN #ConfigurableProductPublishedXML SOURCE ON TARGET.PublishCatalogLogId = SOURCE.versionId AND TARGET.PublishedId = SOURCE.PublishProductId AND TARGET.IsConfigProductXML = 1 AND TARGET.LocaleId = 0
																			   AND TARGET.IsConfigProductXML = 1 AND TARGET.LocaleId = 0

						INSERT  INTO ZnodePublishedXML(PublishCatalogLogId ,PublishedId ,PublishedXML ,IsConfigProductXML ,LocaleId ,CreatedBy ,CreatedDate ,ModifiedBy ,ModifiedDate)
						SELECT SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate
						FROM #ConfigurableProductPublishedXML SOURCE
						WHERE NOT EXISTS(select * from ZnodePublishedXML TARGET where TARGET.PublishCatalogLogId = SOURCE.versionId AND TARGET.PublishedId = SOURCE.PublishProductId
										 AND TARGET.IsConfigProductXML = 1 AND TARGET.LocaleId = 0 )

             END;

				

			SELECT PublishedXML ReturnXML
			FROM ZnodePublishedXML  ZPXM 
			WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLP WHERE TBLP.VersionId = ZPXM.PublishCatalogLogId and TBLP.PublishProductid = ZPXm.PublishedId )
			AND IsConfigProductXML = CASE WHEN @ProductType = 'ConfigurableProduct' THEN  1 ELSE 0 END 
			AND IsGroupProductXML = CASE WHEN @ProductType = 'GroupedProduct' THEN  1 ELSE 0 END 
			AND IsBundleProductXML = CASE WHEN @ProductType = 'BundleProduct' THEN  1 ELSE 0 END 
				  

			IF OBJECT_ID('tempdb..#TBL_PublishCatalogId') is not null
				drop table #TBL_PublishCatalogId

			IF OBJECT_ID('tempdb..#ConfigurableProductPublishedXML') is not null
				drop table #ConfigurableProductPublishedXML
		
		    COMMIT TRAN GetPublishAssociatedProducts;
			
         END TRY
         BEGIN CATCH
		    SELECT ERROR_MESSAGE()
            DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAssociatedProducts @PublishCatalogId = '+@PublishCatalogId+',@ProductType= '+@ProductType+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
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