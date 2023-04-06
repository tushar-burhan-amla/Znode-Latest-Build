CREATE PROCEDURE [dbo].[Znode_GetPublishSingleCategory]
(   @PimCategoryId    INT, 
    @UserId           INT,
    @Status           int = 0 OUT,
	@IsDebug          BIT = 0
	,@LocaleIds		  TransferId READONLY,
	@PimCatalogId     INT = 0 )
AS 
/*
       Summary:Publish category with their respective products and details 
	            The result is fetched in xml form   
       Unit Testing   
	            During Catalog Publish Publish status should be updated 
				   
       Begin transaction 
       SELECT * FROM ZnodePIMAttribute 
	   SELECT * FROM ZnodePublishCatalog 
	   SELECT * FROM ZnodePublishCategory WHERE publishCAtegoryID = 167 

       EXEC [Znode_GetPublishSingleCategory @PublishCatalogId = 5,@VersionId = 0 ,@UserId =2 ,@IsDebug = 1 
       Rollback Transaction 
	*/
     BEGIN
         BEGIN TRAN GetPublishCategory;
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @PublishCatalogLogId int , @PublishCataLogId int , @VersionId  int --,@PimCatalogId int 
			 
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @LocaleId INT= 0, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId(), @Counter INT= 1, @MaxId INT= 0, @CategoryIdCount INT;
             DECLARE @IsActive BIT= [dbo].[Fn_GetIsActiveTrue]();
             DECLARE @AttributeIds VARCHAR(MAX)= '', @PimCategoryIds VARCHAR(MAX)= '', @DeletedPublishCategoryIds VARCHAR(MAX)= '', @DeletedPublishProductIds VARCHAR(MAX);
			 
			 DECLARE @TBL_PublishCatalogId TABLE(PublishCatalogId INT,PimCatalogId  INT , VersionId INT )

			 INSERT INTO @TBL_PublishCatalogId  (PublishCatalogId,PimCatalogId,VersionId ) 
			 SELECT ZPCL.PublishCatalogId, ZPCL.PimCatalogId,ZPCL.PublishCatalogLogId
			 FROM ZnodePimCategoryHierarchy ZPCH 
			 INNER JOIN ZnodePublishCatalogLog  ZPCL  ON ZPCH.PimCatalogId = ZPCL.PimCatalogId and ZPCH.PimCategoryId = @PimCategoryId 
			 where  PublishCatalogLogId in (Select MAX (PublishCatalogLogId) from ZnodePublishCatalogLog ZPCL where 
			 ZPCH.PimCatalogId = ZPCL.PimCatalogId)
			 AND ZPCL.PimCatalogId = CASE WHEN @PimCatalogId <> 0 THEN @PimCatalogId ELSE ZPCL.PimCatalogId END

			 IF NOT EXISTS (Select TOP 1 1 from @TBL_PublishCatalogId) 
			 OR NOT EXISTS (select TOP 1 1  from ZnodePimCategoryProduct ZPCP inner join ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId where ZPCP.PimCategoryId = @PimCategoryId  )
			 Begin
				SET @Status = 1  -- Category not associated or catalog not publish
				ROLLBACK TRAN GetPublishCategory;
				Return 0 ;
			 END 

             DECLARE @TBL_AttributeIds TABLE
             (PimAttributeId       INT,
              ParentPimAttributeId INT,
              AttributeTypeId      INT,
              AttributeCode        VARCHAR(600),
              IsRequired           BIT,
              IsLocalizable        BIT,
              IsFilterable         BIT,
              IsSystemDefined      BIT,
              IsConfigurable       BIT,
              IsPersonalizable     BIT,
              DisplayOrder         INT,
              HelpDescription      VARCHAR(MAX),
              IsCategory           BIT,
              IsHidden             BIT,
              CreatedDate          DATETIME,
              ModifiedDate         DATETIME,
              AttributeName        NVARCHAR(MAX),
              AttributeTypeName    VARCHAR(300)
             );
             DECLARE @TBL_AttributeDefault TABLE
             (
				  PimAttributeId            INT,
				  AttributeDefaultValueCode VARCHAR(100),
				  IsEditable                BIT,
				  AttributeDefaultValue     NVARCHAR(MAX),
				  DisplayOrder   INT
             );
             DECLARE @TBL_AttributeValue TABLE
             (
				  PimCategoryAttributeValueId INT,
				  PimCategoryId               INT,
				  CategoryValue               NVARCHAR(MAX),
				  AttributeCode               VARCHAR(300),
				  PimAttributeId              INT
             );
             DECLARE @TBL_LocaleIds TABLE
             (
				  RowId     INT IDENTITY(1, 1),
				  LocaleId  INT,
				  IsDefault BIT
             );
             DECLARE @TBL_PimCategoryIds TABLE
             (
				  PimCategoryId       INT,
				  PimParentCategoryId INT,
				  DisplayOrder        INT,
				  ActivationDate      DATETIME,
				  ExpirationDate      DATETIME,
				  CategoryName        NVARCHAR(MAX),
				  ProfileId           VARCHAR(MAX),
				  IsActive            BIT,
				  PimCategoryHierarchyId INT,
				  ParentPimCategoryHierarchyId INT,
				  PublishCatalogId INT,
				  PimCatalogId  INT,
				  VersionId INT  ,
				  CategoryCode  NVARCHAR(MAX)          
			 );
             DECLARE @TBL_PublishPimCategoryIds TABLE
             (PublishCategoryId       INT,
              PimCategoryId           INT,
              PublishProductId        varchar(max),
              PublishParentCategoryId INT ,
			  PimCategoryHierarchyId INT ,
			  parentPimCategoryHierarchyId INT,
			  RowIndex INT
             );
             DECLARE @TBL_DeletedPublishCategoryIds TABLE
             (PublishCategoryId INT,
              PublishProductId  INT
             );
             DECLARE @TBL_CategoryXml TABLE
             (PublishCategoryId INT,
              CategoryXml       XML,
              LocaleId          INT
			 
             );
             INSERT INTO @TBL_LocaleIds
             (LocaleId,
              IsDefault
             )
			  -- here collect all locale ids
            SELECT LocaleId,IsDefault FROM ZnodeLocale MT WHERE IsActive = @IsActive
			AND (EXISTS (SELECT TOP 1 1  FROM @LocaleIds RT WHERE RT.Id = MT.LocaleId )
			OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleIds )) ;

             INSERT INTO @TBL_PimCategoryIds(PimCategoryId,PimParentCategoryId,DisplayOrder,ActivationDate,ExpirationDate,IsActive,PimCategoryHierarchyId,ParentPimCategoryHierarchyId,
			 PublishCatalogId,PimCatalogId,VersionId)
			 --select @PimCategoryId, NULL , NULL , NULL , NULL ,NULL , NULL ,NULL 
			 SELECT DISTINCT ZPCH.PimCategoryId,ZPCH2.PimCategoryId  PimParentCategoryId,ZPCH.DisplayOrder,ZPCH.ActivationDate,ZPCH.ExpirationDate,ZPCH.IsActive ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId,
			 PublishCatalogId,PCI.PimCatalogId,VersionId
			 FROM ZnodePimCategoryHierarchy AS ZPCH 
			 LEFT JOIN ZnodePimCategoryHierarchy AS ZPCH2 ON (ZPCH2.PimCategoryHierarchyId = ZPCH. ParentPimCategoryHierarchyId ) 
			 Inner join @TBL_PublishCatalogId PCI on ZPCH.PimCatalogId = PCI.PimCatalogId 
			 WHERE ZPCH.PimCategoryId = @PimCategoryId ; 

			 MERGE INTO ZnodePublishCategory TARGET USING 
			 ( Select PC.PimCategoryId,
					  PC.PimCategoryHierarchyId,
					  PC.PimParentCategoryId,
					  PC.ParentPimCategoryHierarchyId,
					  PC.PublishCatalogId
					  FROM @TBL_PimCategoryIds PC ) 
			 SOURCE ON
			 (
				 TARGET.PimCategoryId = SOURCE.PimCategoryId 
				 AND TARGET.PublishCatalogId = SOURCE.PublishCatalogId 
				 AND TARGET.PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId
			 )
			 WHEN MATCHED THEN UPDATE SET TARGET.PimParentCategoryId = SOURCE.PimParentCategoryId,TARGET.CreatedBy = @UserId,TARGET.CreatedDate = @GetDate,
				TARGET.ModifiedBy = @UserId,TARGET.ModifiedDate = @GetDate,
			 PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId,ParentPimCategoryHierarchyId=SOURCE.ParentPimCategoryHierarchyId
             
			 WHEN NOT MATCHED THEN 
			 INSERT(PimCategoryId,PublishCatalogId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
			 ,PimCategoryHierarchyId,ParentPimCategoryHierarchyId) 
			 VALUES(SOURCE.PimCategoryId,SOURCE.PublishCatalogId,@UserId,@GetDate,@UserId,@GetDate,SOURCE.PimCategoryHierarchyId
			 ,SOURCE.ParentPimCategoryHierarchyId)

				OUTPUT INSERTED.PublishCategoryId,INSERTED.PimCategoryId,INSERTED.PimCategoryHierarchyId,
			 INSERTED.parentPimCategoryHierarchyId 
			 INTO @TBL_PublishPimCategoryIds(PublishCategoryId,PimCategoryId,PimCategoryHierarchyId,parentPimCategoryHierarchyId);
			     	    
			 -- here update the publish parent category id
            UPDATE ZPC SET [PimParentCategoryId] =TBPC.[PimCategoryId] 
			FROM ZnodePublishCategory ZPC
            INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
			WHERE ZPC.PublishCatalogId = TBPC.PublishCatalogId 
			AND TBPC.PublishCatalogId  in (Select PublishCatalogId from @TBL_PublishCatalogId)
			AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL AND 
			ZPC.PimCategoryId = @PimCategoryId  ;

			UPDATE a
			SET  a.PublishParentCategoryId = b.PublishCategoryId
			FROM ZnodePublishCategory a 
			INNER JOIN ZnodePublishCategory b   ON (a.parentpimCategoryHierarchyId = b.pimCategoryHierarchyId)
			WHERE a.parentpimCategoryHierarchyId IS NOT NULL 
			AND a.PublishCatalogId = b.PublishCatalogId AND b.PublishCatalogId in (Select PublishCatalogId from @TBL_PublishCatalogId)
			AND a.PimCategoryId = @PimCategoryId 
			 --UPDATE ZPC SET [PimParentCategoryId] = TBPC.[PimCategoryId] 
			 --FROM ZnodePublishCategory ZPC
			 --INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
			 --WHERE ZPC.PublishCatalogId =@PublishCatalogId
			 --AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL ;

			 -- product are published here 
            --  EXEC Znode_GetPublishProducts @PublishCatalogId,0,@UserId,1,0,0;
			
		     SET @MaxId =(SELECT MAX(RowId)FROM @TBL_LocaleIds);
			 DECLARE @TransferID TRANSFERID 
			 INSERT INTO @TransferID 
			 SELECT DISTINCT  PimCategoryId	 FROM @TBL_PublishPimCategoryIds 

             SET @PimCategoryIds = SUBSTRING((SELECT ','+CAST(PimCategoryId AS VARCHAR(50)) FROM @TBL_PublishPimCategoryIds FOR XML PATH('')), 2, 4000);
			 
             WHILE @Counter <= @MaxId -- Loop on Locale id 
                 BEGIN
                     SET @LocaleId =(SELECT LocaleId FROM @TBL_LocaleIds WHERE RowId = @Counter);
                   
				     SET @AttributeIds = SUBSTRING((SELECT ','+CAST(ZPCAV.PimAttributeId AS VARCHAR(50)) FROM ZnodePimCategoryAttributeValue ZPCAV 
										 WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC WHERE TBPC.PimCategoryId = ZPCAV.PimCategoryId) GROUP BY ZPCAV.PimAttributeId FOR XML PATH('')), 2, 4000);
                
				     SET @CategoryIdCount =(SELECT COUNT(1) FROM @TBL_PimCategoryIds);

                     INSERT INTO @TBL_AttributeIds (PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,
					 IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName)
                     EXEC [Znode_GetPimAttributesDetails] @AttributeIds,@LocaleId;

                     INSERT INTO @TBL_AttributeDefault (PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)
                     EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @AttributeIds,@LocaleId;

                     INSERT INTO @TBL_AttributeValue (PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
                     EXEC [dbo].[Znode_GetCategoryAttributeValueId] @TransferID,@AttributeIds,@LocaleId;

					-- SELECT * FROM @TBL_AttributeValue WHERE PimCategoryId = 281

					--select * from @TBL_AttributeValue



                     ;WITH Cte_UpdateDefaultAttributeValue
                     AS (
					  SELECT TBAV.PimCategoryId,TBAV.PimAttributeId,SUBSTRING((SELECT ','+AttributeDefaultValue FROM @TBL_AttributeDefault TBD WHERE TBAV.PimAttributeId = TBD.PimAttributeId
						AND EXISTS(SELECT TOP 1 1 FROM Split(TBAV.CategoryValue, ',') SP WHERE SP.Item = TBD.AttributeDefaultValueCode)FOR XML PATH('')), 2, 4000) DefaultCategoryAttributeValue
						FROM @TBL_AttributeValue TBAV WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_AttributeDefault TBAD WHERE TBAD.PimAttributeId = TBAV.PimAttributeId))
					 
					 -- update the default value with locale 
                     UPDATE TBAV SET CategoryValue = CTUDFAV.DefaultCategoryAttributeValue FROM @TBL_AttributeValue TBAV 
					 INNER JOIN Cte_UpdateDefaultAttributeValue CTUDFAV ON(CTUDFAV.PimCategoryId = TBAV.PimCategoryId AND CTUDFAV.PimAttributeId = TBAV.PimAttributeId)
					 WHERE CategoryValue IS NULL ;
					 
					 -- here is update the media path  
                     WITH Cte_productMedia
                     AS (SELECT TBA.PimCategoryId,TBA.PimAttributeId,[dbo].[FN_GetThumbnailMediaPathPublish](SUBSTRING((SELECT ','+zm.PATH FROM ZnodeMedia ZM WHERE EXISTS
					    (SELECT TOP 1 1 FROM dbo.split(TBA.CategoryValue, ',') SP WHERE SP.Item = CAST(Zm.MediaId AS VARCHAR(50)))FOR XML PATH('')), 2, 4000)) CategoryValue
						FROM @TBL_AttributeValue TBA WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetProductMediaAttributeId]() FNMA WHERE FNMA.PImAttributeId = TBA.PimATtributeId))
                         
					 UPDATE TBAV SET CategoryValue = CTCM.CategoryValue 
					 FROM @TBL_AttributeValue TBAV 
					 INNER JOIN Cte_productMedia CTCM ON(CTCM.PimCategoryId = TBAV.PimCategoryId
					 AND CTCM.PimAttributeId = TBAV.PimAttributeId);

                     WITH Cte_PublishProductIds
					 AS (SELECT TBPC.PublishcategoryId,SUBSTRING((SELECT ','+CAST(PublishProductId AS VARCHAR(50))
					  FROM ZnodePublishCategoryProduct ZPCP 
					  WHERE ZPCP.PublishCategoryId = TBPC.publishCategoryId
					  AND ZPCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId
                      AND ZPCP.PublishCatalogId in (Select PublishCatalogId from @TBL_PublishCatalogId)
					   FOR XML PATH('')), 2, 8000) PublishProductId ,PimCategoryHierarchyId
					  FROM @TBL_PublishPimCategoryIds TBPC)
                          
					 UPDATE TBPPC SET PublishProductId = CTPP.PublishProductId FROM @TBL_PublishPimCategoryIds TBPPC INNER JOIN Cte_PublishProductIds CTPP ON(TBPPC.PublishCategoryId = CTPP.PublishCategoryId 
					 AND TBPPC.PimCategoryHierarchyId = CTPP.PimCategoryHierarchyId);

                     WITH Cte_CategoryProfile
                     AS (
							SELECT PimCategoryId,ZPCC.PimCategoryHierarchyId,
									SUBSTRING(( SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
									FROM ZnodeProfile ZPC 
									INNER JOIN ZnodePimCategoryHierarchy ZPRCC ON(ZPRCC.PimCategoryHierarchyId = ZPCC.PimCategoryHierarchyId
									AND ZPRCC.PimCatalogId = ZPC.PimCatalogId) 
									WHERE ZPC.PimCatalogId = ZPCC.PimCatalogId FOR XML PATH('')), 2, 4000) ProfileIds
						   FROM ZnodePimCategoryHierarchy ZPCC 
						   WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC 
						   WHERE TBPC.PimCategoryId = ZPCC.PimCategoryId AND ZPCC.PimCatalogId in (Select PimCatalogId from @TBL_PublishCatalogId)
						   AND ZPCC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId)
					   )                          
				      UPDATE TBPC SET TBPC.ProfileId = CTCP.ProfileIds 
					  FROM @TBL_PimCategoryIds TBPC 
					  LEFT JOIN Cte_CategoryProfile CTCP ON(CTCP.PimCategoryId = TBPC.PimCategoryId AND CTCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId );
                     
					 UPDATE TBPC SET TBPC.CategoryName = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
                     AND EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryNameAttribute]() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId))


					 UPDATE TBPC SET TBPC.CategoryCode = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
					 AND EXISTS(SELECT TOP 1 1 FROM dbo.Fn_GetCategoryCodeAttribute() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId)
					 )
					 
					 --select * from @TBL_PimCategoryIds

					 --select * from @TBL_AttributeValue
					-- SELECT * FROM @TBL_AttributeValue WHERE pimCategoryId = 369


					 -- here update the publish category details 
                     ;WITH Cte_UpdateCategoryDetails
                     AS (
					 SELECT TBC.PimCategoryId,PublishCategoryId,CategoryName, TBPPC.PimCategoryHierarchyId,CategoryCode
					 FROM @TBL_PimCategoryIds TBC
                     INNER JOIN @TBL_PublishPimCategoryIds TBPPC ON(TBC.PimCategoryId = TBPPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPPC.PimCategoryHierarchyId)
					 )						
                     MERGE INTO ZnodePublishCategoryDetail TARGET USING Cte_UpdateCategoryDetails SOURCE ON(TARGET.PublishCategoryId = SOURCE.PublishCategoryId
					 AND TARGET.LocaleId = @LocaleId)
                     WHEN MATCHED THEN UPDATE SET PublishCategoryId = SOURCE.PublishcategoryId,PublishCategoryName = SOURCE.CategoryName,LocaleId = @LocaleId,ModifiedBy = @userId,ModifiedDate = @GetDate,CategoryCode= SOURCE.CategoryCode
                     WHEN NOT MATCHED THEN INSERT(PublishCategoryId,PublishCategoryName,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CategoryCode) VALUES
                     (SOURCE.PublishCategoryId,SOURCE.CategoryName,@LocaleId,@userId,@GetDate,@userId,@GetDate,CategoryCode);


					DECLARE @UpdateCategoryLog  TABLE (PublishCatalogLogId INT , LocaleId INT ,PublishCatalogId INT  )
					INSERT INTO @UpdateCategoryLog
					SELECT MAX(PublishCatalogLogId) PublishCatalogLogId , LocaleId , PublishCatalogId 
					FROM ZnodePublishCatalogLog a 
					WHERE a.PublishCatalogId =@PublishCatalogId
					AND  a.LocaleId = @LocaleId 
					GROUP BY 	LocaleId,PublishCatalogId  

					-----------------------------------------------------------------
					IF OBJECT_ID('tempdb..#Index') is not null
					BEGIN 
						DROP TABLE #Index
					END 
					CREATE TABLE #Index (RowIndex int ,PimCategoryId int , PimCategoryHierarchyId  int,ParentPimCategoryHierarchyId int )		
					insert into  #Index ( RowIndex ,PimCategoryId , PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
					SELECT CAST(Row_number() OVER (Partition By TBL.PimCategoryId Order by ISNULL(TBL.PimCategoryId,0) desc) AS VARCHAR(100))
					,ZPC.PimCategoryId, ZPC.PimCategoryHierarchyId, ZPC.ParentPimCategoryHierarchyId
					FROM @TBL_PublishPimCategoryIds TBL
					INNER JOIN ZnodePublishCategory ZPC ON (TBL.PimCategoryId = ZPC.PimCategoryId AND TBL.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId)
					WHERE ZPC.PublishCatalogId = @PublishCatalogId

					UPDATE TBP SET  TBP.[RowIndex]=  IDX.RowIndex 
					FROM @TBL_PublishPimCategoryIds TBP INNER JOIN #Index IDX ON (IDX.PimCategoryId = TBP.PimCategoryId AND IDX.PimCategoryHierarchyId = TBP.PimCategoryHierarchyId)  

					------------------------------------------------------------------

                     ;WITH Cte_CategoryXML
                     AS (SELECT PublishcategoryId,PimCategoryId,(SELECT TY.PublishCatalogLogId,TBPC.PublishCategoryId ZnodeCategoryId,TBC.PublishCatalogId ZnodeCatalogId
																		,THR.PublishParentCategoryId TempZnodeParentCategoryIds,ZPC.CatalogName ,
																		 ISNULL(DisplayOrder, '0') DisplayOrder,@LocaleId LocaleId,ActivationDate 
																		 ,ExpirationDate,TBC.IsActive,ISNULL(CategoryName, '') Name--,ProfileId TempProfileIds
																		 ,ISNULL(PublishProductId, '') TempProductIds,ISNULL(CategoryCode,'') as CategoryCode
																		 ,ISNULL(TBPC.RowIndex,1) CategoryIndex
                        FROM @TBL_PublishPimCategoryIds TBPC 
						INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId in  (Select PublishCatalogId from @TBL_PublishCatalogId))
						INNER JOIN ZnodePublishCAtegory THR ON (THR.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId AND THR.PimCategoryId = TBPC.PimCategoryId AND THR.PublishCatalogId in  (Select PublishCatalogId from @TBL_PublishCatalogId) )
						LEFT JOIN @UpdateCategoryLog TY ON ( TY.PublishCatalogId IN (Select PublishCatalogId from @TBL_PublishCatalogId) AND TY.localeId = @LocaleId  )
						INNER JOIN @TBL_PimCategoryIds TBC ON(TBC.PimCategoryId = TBPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId) WHERE TBPC.PublishCategoryId = TBPCO.PublishCategoryId 
						FOR XML PATH('')) CategoryXml 
						FROM @TBL_PublishPimCategoryIds TBPCO),


                     Cte_CategoryAttributeXml
                     AS (SELECT CTCX.PublishCategoryId,'<CategoryEntity>'+ISNULL(CategoryXml, '')+
					  ISNULL((SELECT(SELECT TBA.AttributeCode,TBA.AttributeName,ISNULL(IsUseInSearch, 0) IsUseInSearch,
                        ISNULL(IsHtmlTags, 0) IsHtmlTags,ISNULL(IsComparable, 0) IsComparable,TBAV.CategoryValue AttributeValues,TBA.AttributeTypeName FROM @TBL_AttributeValue TBAV
                        INNER JOIN @TBL_AttributeIds TBA ON(TBAV.PimAttributeId = TBA.PimAttributeId) LEFT JOIN ZnodePimFrontendProperties ZPFP ON(ZPFP.PimAttributeId = TBA.PimAttributeId)
                        WHERE CTCX.PimCategoryId = TBAV.PimCategoryId AND TBAO.PimAttributeId = TBA.PimAttributeId FOR XML PATH('AttributeEntity'), TYPE) FROM @TBL_AttributeIds TBAO
                        FOR XML PATH('Attributes')), '')+'</CategoryEntity>' CategoryXMl FROM Cte_CategoryXML CTCX)

						
                     INSERT INTO @TBL_CategoryXml(PublishCategoryId,CategoryXml,LocaleId)
                     SELECT PublishCategoryId,CategoryXml,@LocaleId LocaleId FROM Cte_CategoryAttributeXml;

                  
				     DELETE FROM @TBL_AttributeIds;
                     DELETE FROM @TBL_AttributeDefault;
                     DELETE FROM @TBL_AttributeValue;
                     SET @Counter = @Counter + 1;
                 END;
	
			Select PublishCategoryId ,VersionId	, PimCatalogId	, LocaleId,PublishCatalogId, '404test' SeoUrl
			into #OutPublish from @TBL_PublishCatalogId CLI CROSS JOIN @TBL_CategoryXml  
			--group by PimCatalogId,VersionId,PublishCategoryId

			Alter TABLE #OutPublish ADD Id int Identity 
			SET @MaxId =(SELECT COUNT(*) FROM #OutPublish);
			 --SET @PimCategoryIds = SUBSTRING((SELECT ','+CAST(PimCategoryId AS VARCHAR(50)) FROM @TBL_PublishPimCategoryIds FOR XML PATH('')), 2, 4000);
			Declare @ExistingPublishCategoryId  nvarchar(max), @PublishCategoryId  int 
			SET @Counter =1 
            WHILE @Counter <= @MaxId -- Loop on Locale id 
            BEGIN
				SELECT @VersionId = VersionId  ,
				@PublishCategoryId = PublishCategoryId 
				from #OutPublish where ID = @Counter

		----Single category publish. Category count update for verison for specific catalog
		if Exists (select count(1) from @TBL_PublishPimCategoryIds)
	    begin
			UPDATE ZnodePublishCatalogLog 
				SET PublishCategoryId = (select count(distinct a.PimCategoryId)
				from ZnodePublishCategory a 
				inner join ZnodePublishCatalog c on a.PublishCatalogId = c.PublishCatalogId
				inner join ZnodePimCategoryProduct b on  a.PimCategoryId = b.PimCategoryId 
				inner join ZnodePimCategoryHierarchy ZPCH ON b.PimCategoryId = ZPCH.PimCategoryId and c.PimCatalogId = ZPCH.PimCatalogId
				where a.PublishCatalogId = ZnodePublishCatalogLog.PublishCatalogId)
				,ModifiedDate = @GetDate
			FROM ZnodePublishCatalogLog
			WHERE ZnodePublishCatalogLog.PublishCatalogLogId = @VersionId 
			AND exists(select * from ZnodePublishCatalog ZPC where ZnodePublishCatalogLog.PublishCatalogId = ZPC.PublishCatalogId and ZPC.PimCatalogId = @PimCatalogId )
		end

		----Single category publish. Category count update in all associated catalog 
		if isnull(@PimCatalogId,0)=0 and isnull(@PimcategoryId,0)<>0
		begin
			if object_Id('tempdb..#temp_CatalogCategory') is not null
				drop table #temp_CatalogCategory

			select max(c.PublishCatalogLogId) PublishCatalogLogId, C.PublishCatalogId
			into #temp_CatalogCategory
			from ZnodePimCategoryProduct a
			inner join ZnodePimCategoryHierarchy ZPCH ON a.PimCategoryId = ZPCH.PimCategoryId
			inner join ZnodePublishCatalog b on ZPCH.PimCatalogId = b.PimCatalogId
			inner join ZnodePublishCatalogLog c on b.PublishCatalogId = c.PublishCatalogId
			where a.PimCategoryId = @PimcategoryId
			group by C.PublishCatalogId

		   UPDATE ZPCC 
				SET PublishCategoryId = (select count(distinct a.PimCategoryId)
				from ZnodePublishCategory a 
				inner join ZnodePublishCatalog c on a.PublishCatalogId = c.PublishCatalogId
				inner join ZnodePimCategoryProduct b on  a.PimCategoryId = b.PimCategoryId 
				inner join ZnodePimCategoryHierarchy ZPCH ON b.PimCategoryId = ZPCH.PimCategoryId and c.PimCatalogId = ZPCH.PimCatalogId
				where a.PublishCatalogId = ZPCC.PublishCatalogId)
				,ModifiedDate = @GetDate
			FROM ZnodePublishCatalogLog ZPCC
			WHERE exists(select * from #temp_CatalogCategory CC where ZPCC.PublishCatalogLogId = CC.PublishCatalogLogId )
		end

				SET @Counter  = @Counter  + 1  
			END
           	Select distinct 
			SUBSTRING(( SELECT Distinct ',' + CAST(PublishCategoryId AS VARCHAR(50)) FROM #OutPublish CLO
			FOR XML PATH('')), 2, 4000) PublishCategoryId,
			SUBSTRING(( SELECT Distinct ',' + CAST(VersionId	 AS VARCHAR(50)) FROM #OutPublish CLO
			FOR XML PATH('')), 2, 4000) VersionId,	
			SUBSTRING(( SELECT Distinct ',' + CAST(PublishCatalogId	 AS VARCHAR(50)) FROM #OutPublish CLO
			FOR XML PATH('')), 2, 4000) PimCatalogId,
			SUBSTRING(( SELECT Distinct ',' + CAST(LocaleId AS VARCHAR(50)) FROM #OutPublish CLO
			FOR XML PATH('')), 2, 4000) LocaleId
			from #OutPublish
			--group by PimCatalogId,VersionId,PublishCategoryId

			--Select PublishCategoryId ,VersionId	, PimCatalogId, LocaleId  from #OutPublish 

			Select CategoryXml from @TBL_CategoryXml 

			SELECT CategoryCode FROM @TBL_PimCategoryIds
			GROUP BY CategoryCode

			UPDATE ZnodePimCategory	SET IsCategoryPublish = 1 WHERE PimCategoryId = @PimCategoryId 

			Select Distinct ZPC.PortalId, a.PublishCatalogId, a.PublishCategoryId, ZPCD.CategoryCode, ZCSD.SEOUrl
			from #OutPublish a
			inner join ZnodePortalCatalog ZPC on a.PublishCatalogId = ZPC.PublishCatalogId
			inner join ZnodePublishCategoryDetail ZPCD on a.PublishCategoryId = ZPCD.PublishCategoryId
			inner join ZnodePublishCategory PC on a.PublishCatalogId = PC.PublishCatalogId and a.PublishCategoryId = PC.PublishCategoryId
			left join ZnodeCMSSEODetail ZCSD on ZPC.PortalId = ZCSD.PortalId and ZPCD.CategoryCode = ZCSD.SEOCode
			    and CMSSEOTypeId = (select top 1 CMSSEOTypeId from ZnodeCMSSEOType where Name = 'Category')

			Commit TRAN GetPublishCategory;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE();
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishSingleCategory @PimCategoryId= '+CAST(@PimCategoryId AS VARCHAR(50))+',@PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(50))+',@UserId ='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0 -- Publish Falies 
             ROLLBACK TRAN GetPublishCategory;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPublishSingleCategory',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
