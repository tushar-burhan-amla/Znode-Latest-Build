CREATE PROCEDURE [dbo].[Znode_GetPublishAssociatedAddons](@PublishCatalogId NVARCHAR(MAX) = 0,
                                                         @PimProductId    TransferId Readonly,
                                                         @VersionId        INT           = 0,
                                                         @UserId           INT,														 
														 @PimCategoryHierarchyId int = 0, 
														 @LocaleId       TransferId READONLY,
														 @PublishStateId INT = 0 
														   )
AS 
   
/*
    Summary : If PimcatalogId is provided get all products with Addons and provide above mentioned data
              If PimProductId is provided get all Addons if associated with given product id and provide above mentioned data
    			Input: @PublishCatalogId or @PimProductId
    		    output should be in XML format
              sample xml5
              <AddonEntity>
              <ZnodeProductId></ZnodeProductId>
              <ZnodeCatalogId></ZnodeCatalogId>
              <AddonGroupName></AddonGroupName>
              <TempAsscociadedZnodeProductIds></TempAsscociadedZnodeProductIds>
              </AddonEntity>
    <AddonEntity>
      <ZnodeProductId>6</ZnodeProductId>
      <ZnodeCatalogId>2</ZnodeCatalogId>
      <AddonGroupName>RadioButton</AddonGroupName>
      <TempAsscociadedZnodeProductIds>53,54,55,56,57,82</TempAsscociadedZnodeProductIds>
      <ZnodeProductId>14</ZnodeProductId>
      <ZnodeCatalogId>2</ZnodeCatalogId>
      <AddonGroupName>RadioButton</AddonGroupName>
      <TempAsscociadedZnodeProductIds>6,7</TempAsscociadedZnodeProductIds>
      <ZnodeProductId>16</ZnodeProductId>
      <ZnodeCatalogId>2</ZnodeCatalogId>
      <AddonGroupName>RadioButton</AddonGroupName>
      <TempAsscociadedZnodeProductIds>7,14,54,6</TempAsscociadedZnodeProductIds>
    </AddonEntity>
    Unit Testing 
     SELECT * FROM ZnodePublishcatalog
	begin tran
     EXEC [dbo].[Znode_GetPublishAssociatedAddons] @PublishCatalogId = '3',@userId= 2  ,@PimProductId=  '29' ,@UserId=2
	rollback tran
     EXEC [dbo].[Znode_GetPublishAssociatedAddons] @PublishCatalogId = 3 ,@PimProductId=  '' ,@UserId=2
     EXEC [dbo].[Znode_GetPublishAssociatedAddons] @PublishCatalogId =null ,@PimProductId=  6   

	DECLARE	@return_value int

	EXEC	@return_value = [dbo].[Znode_GetPublishAssociatedAddons]
	@PublishCatalogId = 3,
	@UserId = 2,
	@PimCategoryHierarchyId = 125

	SELECT	'Return Value' = @return_value


   
	*/

     BEGIN
        -- BEGIN TRANSACTION GetPublishAssociatedAddons;
         BEGIN TRY
          SET NOCOUNT ON 
			 DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
             DECLARE @LocaleIdIn INT, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId()
			 , @Counter INT= 1
			 , @MaxRowId INT= 0;

            -- DECLARE @PimAddOnGroupId VARCHAR(MAX);

			 CREATE TABLE #TBL_PublisshIds  (PublishProductId INT , PimProductId INT , PublishCatalogId INT)

             DECLARE @TBL_LocaleId TABLE
             (RowId    INT IDENTITY(1, 1),
              LocaleId INT
             );


			 IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			 BEGIN 
			 		 
			   INSERT INTO #TBL_PublisshIds 
			   EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1
			   
			  -- SET @PimProductId = SUBSTRING((SELECT DISTINCT ','+CAST(PimProductId AS VARCHAr(50)) FROM #TBL_PublisshIds FOR XML PATH ('')),2,8000 )

			  -- SELECT 	@PimProductId	
			 END 
			 IF  ISnull(@PimCategoryHierarchyId,0) <> 0 
			 BEGIN 
			 		 
			   INSERT INTO #TBL_PublisshIds 
			   EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1,@PimCategoryHierarchyId 
	
			 END 

			 CREATE TABLE #TBL_PublishCatalogId (PublishCatalogId INT,PublishProductId INT,PimProductId  INT , VersionId INT ,LocaleId INT  );

			 IF  ISnull(@PimCategoryHierarchyId,0) <> 0 
			 BEGIN 
				 INSERT INTO #TBL_PublishCatalogId 
				 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId, MAX(ZPCP.PublishCatalogLogId)  ,LocaleId 
				 FROM ZnodePublishProduct ZPP 
				 INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId   ) 
				 AND ZPCP.Publishstateid = @PublishStateId
				 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,LocaleId 
			 END 
			 ELSE 
			 Begin
				 BEGIN 
				 INSERT INTO #TBL_PublishCatalogId  
				 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,MAX(PublishCatalogLogId)  ,LocaleId 
				 FROM ZnodePublishProduct ZPP INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 WHERE (EXISTS (SELECT TOP 1 1 FROM #TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  AND  @PublishCatalogId = '0' ) 
				 OR (ZPP.PublishCatalogId =  @PublishCatalogId ))
				 AND CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM @PimProductId ) AND @PublishCatalogId <> 0   THEN  @PublishStateId ELSE  ZPCP.Publishstateid END  = @PublishStateId
				 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,LocaleId 
			 END 

			 End
			
             DECLARE @TBL_AddonGroupLocale TABLE
             (PimAddonGroupId INT,
              DisplayType     NVARCHAR(400),
              AddonGroupName  NVARCHAR(MAX),
			  LocaleId INT 
             );
           
             INSERT INTO @TBL_LocaleId(LocaleId)
                    SELECT LocaleId
                    FROM ZnodeLocale MT 
                    WHERE IsActive = 1
					AND (EXISTS (SELECT TOP 1 1  FROM @LocaleId RT WHERE RT.Id = MT.LocaleId )
					OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleId ));

          
             SET @MaxRowId = ISNULL(
                                   (
                                       SELECT MAX(RowId)
                                       FROM @TBL_LocaleId
                                   ), 0);
    
             WHILE @Counter <= @MaxRowId
                 BEGIN
                     SET @LocaleIdIn =
                     (
                         SELECT LocaleId
                         FROM @TBL_LocaleId
                         WHERE RowId = @Counter
                     );
                     INSERT INTO @TBL_AddonGroupLocale
                     (PimAddonGroupId,
                      DisplayType,
                      AddonGroupName					  
                     )
                     EXEC Znode_GetAddOnGroupLocale
                          '',
                          @LocaleIdIn;

					UPDATE @TBL_AddonGroupLocale SET LocaleId = @LocaleIdIn WHERE LocaleId IS NULL 

                    SET @Counter = @Counter + 1;
                 END;
				     
				 IF  @PublishCatalogId IS NULL  OR @PublishCatalogId = 0 
			     BEGIN 
			 		 
			         DELETE FROM ZnodePublishedXML WHERE IsAddOnXML =1  
					 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId TBLV WHERE ZnodePublishedXML.PublishedId = TBLV.PublishProductId   AND ZnodePublishedXML.PublishCatalogLogId = TBLV.VersionId )
			    
			  
				 END 
				 ELSE 
				 BEGIN 
					
					 ;WITH CTE_AddOnXML as
						 (
							 SELECT ZPPP.PublishProductId,ZPPP.PublishCatalogId,ZPPD.LocaleId,ZPPP.VersionId, ZPP.PublishProductId as AssociatedZnodeProductId  				 
							 FROM [ZnodePimAddOnProductDetail] AS ZPOPD
							 INNER JOIN [ZnodePimAddOnProduct] AS ZPAOP ON ZPOPD.[PimAddOnProductId] = ZPAOP.[PimAddOnProductId]
							 INNER JOIN #TBL_PublishCatalogId ZPPP ON (ZPPP.PimProductId = ZPAOP.PimProductId )
							 INNER JOIN #TBL_PublishCatalogId ZPP ON(ZPP.PimProductId = ZPOPD.[PimChildProductId] AND ZPP.PublishCatalogId = ZPPP.PublishCatalogId and ZPPP.LocaleId  = ZPP.LocaleId )
							 INNER JOIN ZnodePublishProductDetail ZPPD ON (ZPPD.PublishProductId = ZPPP.PublishProductId)
							 INNER JOIN @TBL_AddonGroupLocale TBAG ON (TBAG.PimAddonGroupId   = ZPAOP.PimAddonGroupId AND TBAG.LocaleId = ZPPD.LocaleId )
							 WHERE  ZPP.LocaleId = ZPPD.LocaleId AND ZPPP.LocaleId =  ZPPD.LocaleId 
						)
						,CTE_PublishedXML as
						(
							SELECT ZPX.PublishCatalogLogId,ZPX.PublishedId,ZPX.IsAddonXML, p.value('(./AssociatedZnodeProductId)[1]', 'INT')  as AssociatedZnodeProductId, p.value('(./LocaleId)[1]', 'INT') as LocaleId1
							FROM ZnodePublishedXML ZPX
							CROSS APPLY ZPX.PublishedXML.nodes('/AddonEntity') t(p)
							where ZPX.IsAddonXML = 1
						)
						DELETE ZPXML  
						FROM ZnodePublishedXML ZPXML
						INNER JOIN CTE_PublishedXML CPX	ON ZPXML.PublishCatalogLogId = CPX.PublishCatalogLogId AND ZPXML.PublishedId = CPX.PublishedId AND ZPXML.IsAddonXML = CPX.IsAddonXML		
						INNER JOIN CTE_AddOnXML CAX on --CPX.PublishCatalogLogId = CAX.VersionId AND
							 CPX.PublishedId = CAX.PublishProductId
							AND ZPXML.IsAddonXML = 1 
							AND CPX.LocaleId1 = CAX.LocaleId 
							AND CPX.AssociatedZnodeProductId = CAX.AssociatedZnodeProductId
				 END 
			
					--SELECT * FROM #TBL_PublishCatalogId

					DELETE FROM ZnodePublishedXml WHERE PublishCatalogLogId IN (SELECT VersionId 
					FROM #TBL_PublishCatalogId ) AND IsAddOnXML = 1 

					IF OBJECT_ID('tempdb..#AddonProductPublishedXML') is not null
						drop table #AddonProductPublishedXML

					SELECT   ZPPP.PublishProductId,ZPPP.PublishCatalogId,ZPPD.LocaleId,ZPPP.VersionId,'<AddonEntity><VersionId>'+CAST(ZPPP.VersionId AS VARCHAR(50))+'</VersionId><ZnodeProductId>'+CAST(ZPPP.[PublishProductId] AS VARCHAR(50))+'</ZnodeProductId><ZnodeCatalogId>'
				     +CAST(ZPPP.[PublishCatalogId] AS VARCHAR(50))+'</ZnodeCatalogId><AssociatedZnodeProductId>'+CAST(ZPP.PublishProductId  AS VARCHAR(50))
					 +'</AssociatedZnodeProductId><DisplayOrder>'+CAST( ISNULL(ZPAOP.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder><AssociatedProductDisplayOrder>'
					 +CAST(ISNULL(ZPOPD.DisplayOrder,'') AS VARCHAR(50))+'</AssociatedProductDisplayOrder><RequiredType>'+ISNULL(RequiredType,'')+'</RequiredType><DisplayType>'
					 + ISNULL(DisplayType,'')+'</DisplayType><GroupName>'+ISNULL((select ''+AddonGroupName for xml path('')),'')+'</GroupName><LocaleId>'+CAST(ZPPD.LocaleId AS VARCHAR(50))+'</LocaleId><IsDefault>'+CAST(ISNULL(IsDefault,0) AS VARCHAR(50))+'</IsDefault></AddonEntity>'  ReturnXML		   
				      INTO #AddonProductPublishedXML
                      FROM [ZnodePimAddOnProductDetail] AS ZPOPD
						INNER JOIN [ZnodePimAddOnProduct] AS ZPAOP ON ZPOPD.[PimAddOnProductId] = ZPAOP.[PimAddOnProductId]
						INNER JOIN #TBL_PublishCatalogId ZPPP ON (ZPPP.PimProductId = ZPAOP.PimProductId )
						INNER JOIN #TBL_PublishCatalogId ZPP ON(ZPP.PimProductId = ZPOPD.[PimChildProductId] AND ZPP.PublishCatalogId = ZPPP.PublishCatalogId )
						INNER JOIN ZnodePublishProductDetail ZPPD ON (ZPPD.PublishProductId = ZPPP.PublishProductId)
						INNER JOIN @TBL_AddonGroupLocale TBAG ON (TBAG.PimAddonGroupId   = ZPAOP.PimAddonGroupId AND TBAG.LocaleId = ZPPD.LocaleId )
						WHERE  ZPP.LocaleId = ZPPD.LocaleId AND ZPPP.LocaleId =  ZPPD.LocaleId 


						UPDATE TARGET
						SET PublishedXML = ReturnXML
								, ModifiedBy = @userId 
								,ModifiedDate = @GetDate
						FROM ZnodePublishedXML TARGET
						INNER JOIN #AddonProductPublishedXML SOURCE ON TARGET.PublishCatalogLogId = SOURCE.VersionId AND TARGET.PublishedId = SOURCE.PublishProductId
														AND TARGET.IsAddonXML = 1 AND TARGET.LocaleId = SOURCE.LocaleId 

						INSERT  INTO ZnodePublishedXML(PublishCatalogLogId ,PublishedId ,PublishedXML ,IsAddonXML ,LocaleId ,CreatedBy ,CreatedDate ,ModifiedBy ,ModifiedDate)
						SELECT SOURCE.versionid , Source.PublishProductid,Source.ReturnXML,1,0,@userId,@getDate,@userId,@getDate
						FROM #AddonProductPublishedXML SOURCE
						WHERE NOT EXISTS(select * from ZnodePublishedXML TARGET where TARGET.PublishCatalogLogId = SOURCE.VersionId AND TARGET.PublishedId = SOURCE.PublishProductId
														AND TARGET.IsAddonXML = 1 AND TARGET.LocaleId = SOURCE.LocaleId  )
					
					SELECT Cast(PublishedXML as xml) ReturnXML
					FROM #TBL_PublishCatalogId TBLPP 
					INNER JOIN ZnodePublishedXML ZPX ON (ZPX.PublishCatalogLogId = TBLPP.VersionId AND ZPX.PublishedId = TBLPP.publishProductid )
					WHERE ZPX.IsAddonXML = 1
             
           --  COMMIT TRANSACTION GetPublishAssociatedAddons;
         END TRY
         BEGIN CATCH
		     SELECT ERROR_MESSAGE(),ERROR_PROCEDURE()
             DECLARE @Status BIT;
             SET @Status = 0;
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAssociatedAddons @PublishCatalogId = '+@PublishCatalogId+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
           --  ROLLBACK TRANSACTION GetPublishAssociatedAddons;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPublishAssociatedAddons',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;