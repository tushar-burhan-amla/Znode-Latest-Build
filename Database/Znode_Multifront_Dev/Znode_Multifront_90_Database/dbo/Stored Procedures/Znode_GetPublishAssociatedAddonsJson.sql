CREATE PROCEDURE [dbo].[Znode_GetPublishAssociatedAddonsJson]
(
	@PublishCatalogId NVARCHAR(MAX) = 0,
	@PimProductId    TransferId Readonly,
	@VersionId        INT           = 0,
	@UserId           INT,														 
	@PimCategoryHierarchyId int = 0, 
	@LocaleId       TransferId READONLY,
	@PublishStateId INT = 0, 
	@VersionIdString  VARCHAR(2000)= '',
	@Status		   Bit  OutPut,
	@RevisionType Varchar(50)
)
AS 
   
/*
    Summary : If PimcatalogId is provided get all products with Addons and provide above mentioned data
       If PimProductId is provided get all Addons if associated with given product id and provide above mentioned data
    			Input: @PublishCatalogId or @PimProductId
    		    output should be in XML format
       sample xml5
    Begin transaction  
	 declare  @PimProductId TransferId 
	 Declare @Status bit 
	 INSERT INTO @PimProductId  values (768)

			Exec [_Znode_GetPublishAssociatedAddonsJson]
				@PublishCatalogId = 5 ,
				--@PimProductId   = @PimProductId,
				@UserId =2,														 
				@VersionIdString = '',--'1662,1664,1663,1665',
				@Status	 =@Status Out,
				@RevisionType= 'Production'
				rollback transaction
				--SELECT * from ZnodePublishVersionEntity 
   
	*/

BEGIN
    BEGIN TRY
    SET NOCOUNT ON 
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
        DECLARE @LocaleIdIn INT, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId()
		, @Counter INT= 1
		, @MaxRowId INT= 0;

	UPDATE ZnodePublishAddonEntity SET ElasticSearchEvent = 0
    -- DECLARE @PimAddOnGroupId VARCHAR(MAX);

		CREATE TABLE #TBL_PublisshIds  (PublishProductId INT , PimProductId INT , PublishCatalogId INT)
		DECLARE @TBL_LocaleId TABLE (RowId    INT IDENTITY(1, 1),LocaleId INT);
		IF OBJECT_ID('tempdb..#VesionIds') is not null
		DROP TABLE #VesionIds
		Create Table #VesionIds(ZnodeCatalogId int , VersionId int , LocaleId int , RevisionType varchar(50) )

  		If @VersionIdString <> ''
		BEGIN
		
			Insert into #VesionIds (ZnodeCatalogId, VersionId, LocaleId, RevisionType)	
			SELECT PV.ZnodeCatalogId, PV.VersionId, PV.LocaleId, PV.RevisionType
			FROM ZnodePublishVersionEntity PV Inner join Split(@VersionIdString,',') S ON PV.VersionId = S.Item
		END
		Else 
		Begin
			If  (@RevisionType like '%Preview%'  OR @RevisionType like '%Production%')
			BEGIN
				Insert into #VesionIds (ZnodeCatalogId, VersionId, LocaleId, RevisionType)	
				SELECT PV.ZnodeCatalogId, PV.VersionId, PV.LocaleId, PV.RevisionType FROM ZnodePublishVersionEntity PV  where PV.IsPublishSuccess =1 
				AND PV.RevisionType ='Preview'
			END
			If  (@RevisionType like '%Production%' OR @RevisionType = 'None')
			BEGIN
				Insert into #VesionIds (ZnodeCatalogId, VersionId, LocaleId, RevisionType)	
				SELECT PV.ZnodeCatalogId, PV.VersionId, PV.LocaleId, PV.RevisionType FROM ZnodePublishVersionEntity PV  where PV.IsPublishSuccess =1 
				AND PV.RevisionType ='Production'
			END					
		End 

	
		IF (@PublishCatalogId IS NULL OR @PublishCatalogId = 0)
		BEGIN 
			INSERT INTO #TBL_PublisshIds 
			EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId,@userid,@PimProductId,1
		END 
		IF ISnull(@PimCategoryHierarchyId,0) <> 0 
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
			GROUP BY ZPP.PublishCatalogId,ZPP.PublishProductId,PimProductId,LocaleId 
		END 
		ELSE 
		Begin
			INSERT INTO #TBL_PublishCatalogId  
			SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,0  VersionId  ,0 LocaleId 
			FROM ZnodePublishProduct ZPP  
			WHERE EXISTS  (SELECT TOP 1 1 FROM #VesionIds ZPCP WHERE (ZPCP.ZnodeCatalogId  = ZPP.PublishCatalogId)) AND
			(EXISTS (SELECT TOP 1 1 FROM #TBL_PublisshIds SP WHERE SP.PublishProductId = ZPP.PublishProductId  
			AND  @PublishCatalogId = '0' )  OR (ZPP.PublishCatalogId =  @PublishCatalogId ))
		END
        DECLARE @TBL_AddonGroupLocale TABLE( PimAddonGroupId INT, DisplayType NVARCHAR(400),AddonGroupName NVARCHAR(MAX),LocaleId INT );
        INSERT INTO @TBL_LocaleId(LocaleId)
        SELECT LocaleId FROM ZnodeLocale MT  WHERE IsActive = 1 AND (EXISTS (SELECT TOP 1 1  FROM @LocaleId RT WHERE RT.Id = MT.LocaleId )
			OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleId ));
        SET @MaxRowId = ISNULL(( SELECT MAX(RowId) FROM @TBL_LocaleId ), 0);
    
        WHILE (@Counter <= @MaxRowId)
        BEGIN
            SET @LocaleIdIn =( SELECT LocaleId FROM @TBL_LocaleId WHERE RowId = @Counter );
            INSERT INTO @TBL_AddonGroupLocale (PimAddonGroupId, DisplayType, AddonGroupName)
            EXEC Znode_GetAddOnGroupLocale '', @LocaleIdIn;
	   		UPDATE @TBL_AddonGroupLocale SET LocaleId = @LocaleIdIn WHERE LocaleId IS NULL 
            SET @Counter = @Counter + 1;
        END;
		
		IF OBJECT_ID('tempdb..#AddonProductPublishedXML') is not null
		drop table #AddonProductPublishedXML
						
		SELECT  ZPPP.PublishProductId,ZPPP.PublishCatalogId,V.LocaleId,v.VersionId,
		ZPPP.[PublishProductId]  ZnodeProductId,
		ZPPP.[PublishCatalogId] ZnodeCatalogId,
		ZPP.PublishProductId  AssociatedZnodeProductId,
		ZPAOP.DisplayOrder DisplayOrder,
		ZPOPD.DisplayOrder AssociatedProductDisplayOrder,
		RequiredType ,
		DisplayType, 
		ISNULL((select ''+AddonGroupName for xml path('')),'') GroupName,
		ISnull(IsDefault,0) IsDefault
		INTO #AddonProductPublishedXML
		FROM [ZnodePimAddOnProductDetail] AS ZPOPD
		INNER JOIN [ZnodePimAddOnProduct] AS ZPAOP ON ZPOPD.[PimAddOnProductId] = ZPAOP.[PimAddOnProductId]
		INNER JOIN #TBL_PublishCatalogId ZPPP ON (ZPPP.PimProductId = ZPAOP.PimProductId )
		INNER JOIN #TBL_PublishCatalogId ZPP ON(ZPP.PimProductId = ZPOPD.[PimChildProductId] AND ZPP.PublishCatalogId = ZPPP.PublishCatalogId  )
		INNER JOIN #VesionIds V ON ZPPP.PublishCatalogId  = V.ZnodeCataLogId
		INNER JOIN @TBL_AddonGroupLocale TBAG ON (TBAG.PimAddonGroupId   = ZPAOP.PimAddonGroupId AND TBAG.LocaleId = V.LocaleId )
	
		If (isnull(@PublishCatalogId ,'') = '' and @VersionIdString = '') OR 
			(isnull(@PublishCatalogId ,'') = 0 and @VersionIdString = '')
		Begin 
			UPDATE ZnodePublishAddonEntity 
			SET ElasticSearchEvent = 2
			WHERE PublishAddonEntityId  IN (
											SELECT PublishAddonEntityId
											FROM ZnodePublishAddonEntity BP
											INNER JOIN #AddonProductPublishedXML A ON BP.ZnodeProductId =A.PublishProductId
												AND BP.ZnodeCatalogId = A.PublishCatalogId AND BP.VersionId = A.VersionId
											) 
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZnodePublishAddonEntity.VersionId) 

			----Delete addon entries having parent data removed
			UPDATE ZPAE 
			SET ElasticSearchEvent = 2
			FROM ZnodePublishAddonEntity ZPAE
			WHERE NOT EXISTS (SELECT * from [ZnodePimAddOnProduct] ZPAOP 
					INNER JOIN ZnodePublishProduct ZPPP ON (ZPPP.PimProductId = ZPAOP.PimProductId )
					WHERE ZPAE.ZnodeProductId = ZPPP.PublishProductId AND ZPAE.ZnodeCatalogId = ZPPP.PublishCatalogId)
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPAE.VersionId)
		End 

		IF NOT EXISTS(SELECT * FROM @PimProductId)
		BEGIN
			----Delete addon entries having parent data present but all addon removed
			SELECT zppp1.PublishCatalogId,ZPPP1.PublishProductId,ZPPP.PublishProductId as AssociatedZnodeProductId  
			INTO #TempAddonProduct
			FROM [ZnodePimAddOnProductDetail] AS ZPOPD
			INNER JOIN [ZnodePimAddOnProduct] AS ZPAOP ON ZPOPD.[PimAddOnProductId] = ZPAOP.[PimAddOnProductId]
			INNER JOIN ZnodePublishProduct ZPPP1 ON (ZPPP1.PimProductId = ZPAOP.PimProductId )
			INNER JOIN ZnodePublishProduct ZPPP ON (ZPPP.PimProductId = ZPOPD.PimChildProductId )

			UPDATE ZPAE 
			SET ElasticSearchEvent = 2
			FROM ZnodePublishAddonEntity ZPAE
			WHERE not EXISTS (SELECT * from #TempAddonProduct ZPPP
					WHERE ZPAE.ZnodeProductId = ZPPP.PublishProductId AND ZPAE.ZnodeCatalogId = ZPPP.PublishCatalogId)
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPAE.VersionId)

			UPDATE ZPAE 
			SET ElasticSearchEvent = 2
			FROM ZnodePublishAddonEntity ZPAE
			WHERE EXISTS (SELECT * from #TempAddonProduct ZPPP
					WHERE ZPAE.ZnodeProductId = ZPPP.PublishProductId AND ZPAE.ZnodeCatalogId = ZPPP.PublishCatalogId)
			AND not EXISTS (SELECT * from #TempAddonProduct ZPPP1
					WHERE ZPAE.AssociatedZnodeProductId = ZPPP1.AssociatedZnodeProductId AND ZPAE.ZnodeProductId = ZPPP1.PublishProductId  AND ZPAE.ZnodeCatalogId = ZPPP1.PublishCatalogId)
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPAE.VersionId)
				
			UPDATE PCE SET PCE.AssociatedProductDisplayOrder = Isnull(CE.AssociatedProductDisplayOrder,0),
				PCE.GroupName = CE.GroupName,PCE.DisplayType = CE.DisplayType,
				PCE.DisplayOrder = CE.DisplayOrder,
				PCE.IsRequired = 1 ,PCE.RequiredType = CE.RequiredType,PCE.IsDefault = CE.IsDefault,
				PCE.ElasticSearchEvent = 2
			FROM #AddonProductPublishedXML CE
			INNER JOIN ZnodePublishAddonEntity PCE ON CE.VersionId = PCE.VersionId
			AND PCE.LocaleId = CE.LocaleId AND PCE.ZnodeCatalogId = CE.PublishCatalogId AND CE.ZnodeProductId = PCE.ZnodeProductId
			AND PCE.AssociatedZnodeProductId = CE.AssociatedZnodeProductId AND PCE.ElasticSearchEvent <> 2
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = PCE.VersionId)
		END
		ELSE
		BEGIN
			----Delete addon entries having parent data present but all addon removed
			SELECT zppp1.PublishCatalogId,ZPPP1.PublishProductId,ZPPP.PublishProductId as AssociatedZnodeProductId  
			INTO #TempAddonProductSingleProduct
			FROM [ZnodePimAddOnProductDetail] AS ZPOPD
			INNER JOIN [ZnodePimAddOnProduct] AS ZPAOP ON ZPOPD.[PimAddOnProductId] = ZPAOP.[PimAddOnProductId]
			INNER JOIN ZnodePublishProduct ZPPP1 ON (ZPPP1.PimProductId = ZPAOP.PimProductId )
			INNER JOIN ZnodePublishProduct ZPPP ON (ZPPP.PimProductId = ZPOPD.PimChildProductId )
			where EXISTS(SELECT * FROM @PimProductId x WHERE ZPAOP.PimProductId = X.Id)

			DELETE ZPAE FROM ZnodePublishAddonEntity ZPAE
			WHERE not EXISTS (SELECT * from #TempAddonProductSingleProduct)
			AND EXISTS(SELECT * FROM @PimProductId x INNER JOIN ZnodePublishProduct ZP ON ZP.PimProductId = x.Id WHERE ZPAE.ZnodeProductId = ZP.PublishProductId) 
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPAE.VersionId)
			
			DELETE ZPAE  
			FROM ZnodePublishAddonEntity ZPAE
			WHERE EXISTS (SELECT * from #TempAddonProductSingleProduct ZPPP
					WHERE ZPAE.ZnodeProductId = ZPPP.PublishProductId AND ZPAE.ZnodeCatalogId = ZPPP.PublishCatalogId)
			AND not EXISTS (SELECT * from #TempAddonProductSingleProduct ZPPP1
					WHERE ZPAE.AssociatedZnodeProductId = ZPPP1.AssociatedZnodeProductId AND ZPAE.ZnodeProductId = ZPPP1.PublishProductId  AND ZPAE.ZnodeCatalogId = ZPPP1.PublishCatalogId)
			AND EXISTS(SELECT * FROM #VesionIds V WHERE V.VersionId = ZPAE.VersionId)
			
		END

			Insert into  ZnodePublishAddonEntity
			(VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,AssociatedProductDisplayOrder,
				LocaleId,GroupName,DisplayType,DisplayOrder,IsRequired,RequiredType,IsDefault,ElasticSearchEvent)
			Select 
				VersionId,ZnodeProductId,ZnodeCatalogId,AssociatedZnodeProductId,Isnull(AssociatedProductDisplayOrder,0),
				LocaleId,GroupName,DisplayType,DisplayOrder,1 IsRequired,RequiredType,IsDefault, 1 AS ElasticSearchEvent
			from #AddonProductPublishedXML CE
			WHERE NOT EXISTS(SELECT * FROM ZnodePublishAddonEntity PCE WHERE CE.VersionId = PCE.VersionId
			AND PCE.LocaleId = CE.LocaleId AND PCE.ZnodeCatalogId = CE.PublishCatalogId AND CE.ZnodeProductId = PCE.ZnodeProductId
			AND PCE.AssociatedZnodeProductId = CE.AssociatedZnodeProductId AND PCE.ElasticSearchEvent <> 2)
		
		SET @Status = 1;
            
    END TRY
    BEGIN CATCH
		SELECT ERROR_MESSAGE(),ERROR_PROCEDURE()
            
        SET @Status = 0;
        DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishAssociatedAddonsJson @PublishCatalogId = '+@PublishCatalogId+',@VersionId='+CAST(@VersionId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
        SELECT 0 AS ID,
            CAST(0 AS BIT) AS Status;
        EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetPublishAssociatedAddonsJson',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
    END CATCH;
END;