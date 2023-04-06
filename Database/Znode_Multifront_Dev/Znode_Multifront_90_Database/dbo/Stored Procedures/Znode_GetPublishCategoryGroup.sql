CREATE  PROCEDURE [dbo].[Znode_GetPublishCategoryGroup]
(   
	@PublishCatalogId INT,
    @UserId           INT,
    @VersionId        INT,
    @Status           BIT = 0 OUT,
	@PimCategoryHierarchyId int = 0, 
    @IsDebug          BIT = 0,
	@LocaleId TransferId READONLY,
	@PublishStateId INT = 0 
)
AS 
/*

       Summary:Publish category with their respective products and details 
	            The result is fetched in xml form   
       Unit Testing   
       Begin transaction 
       SELECT * FROM ZnodePIMAttribute 
	   SELECT * FROM ZnodePublishCatalog 
	   SELECT * FROM ZnodePublishCategory WHERE publishCAtegoryID = 167 


       EXEC [Znode_GetPublishCategory] @PublishCatalogId = 5,@VersionId = 0 ,@UserId =2 ,@IsDebug = 1 
       EXEC [Znode_GetPublishCategory] @PublishCatalogId = 5,@VersionId = 0 ,@UserId =2 ,@IsDebug = 1 ,@PimCategoryHierarchyId = ? 


       Rollback Transaction 
	*/
     BEGIN
         BEGIN TRAN GetPublishCategory;
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @LocaleIdIn INT= 0, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId(), @Counter INT= 1, @MaxId INT= 0, @CategoryIdCount INT;
             DECLARE @IsActive BIT= [dbo].[Fn_GetIsActiveTrue]();
             DECLARE @AttributeIds VARCHAR(MAX)= '', @PimCategoryIds VARCHAR(MAX)= '', @DeletedPublishCategoryIds VARCHAR(MAX)= '', @DeletedPublishProductIds VARCHAR(MAX);
             --get the pim catalog id 
			 DECLARE @PimCatalogId INT=(SELECT PimCatalogId FROM ZnodePublishcatalog WHERE PublishCatalogId = @PublishCatalogId); 

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
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder   INT
             );
             DECLARE @TBL_AttributeValue TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              CategoryValue               NVARCHAR(MAX),
              AttributeCode               VARCHAR(300),
              PimAttributeId              INT
             );
             DECLARE @TBL_LocaleIds TABLE
             (RowId     INT IDENTITY(1, 1),
              LocaleId  INT,
              IsDefault BIT
             );
             DECLARE @TBL_PimCategoryIds TABLE
             (PimCategoryId       INT,
              PimParentCategoryId INT,
              DisplayOrder        INT,
              ActivationDate      DATETIME,
              ExpirationDate      DATETIME,
              CategoryName        NVARCHAR(MAX),
              ProfileId           VARCHAR(MAX),
              IsActive            BIT,PimCategoryHierarchyId INT,ParentPimCategoryHierarchyId INT   ,
			  CategoryCode  NVARCHAR(MAX)    );


             DECLARE @TBL_PublishPimCategoryIds TABLE
             (PublishCategoryId       INT,
              PimCategoryId           INT,
              PublishProductId        varchar(max),
              PublishParentCategoryId INT ,
			  PimCategoryHierarchyId INT ,parentPimCategoryHierarchyId INT
			  
             );

			  DECLARE @TBL_PublishPimCategoryIdsLatest TABLE
             (PublishCategoryId       INT,
              PimCategoryId           INT,
              PublishProductId        varchar(max),
              PublishParentCategoryId INT ,
			  PimCategoryHierarchyId INT ,parentPimCategoryHierarchyId INT,PublishCatalogLogId INT,LocaleId INT 
			  ,RowIndex INT 
             );

             DECLARE @TBL_DeletedPublishCategoryIds TABLE
             (PublishCategoryId INT,
              PublishProductId  INT
             );

			     DECLARE @TBL_DeletedPublishCategoryIds_new TABLE
             (PublishCategoryId INT,
              PublishProductId  INT
             );
             DECLARE @TBL_CategoryXml TABLE
             (PublishCategoryId INT,
              CategoryXml       XML,
              LocaleId          INT
			  ,PublishCatalogLogId INT
             );
             INSERT INTO @TBL_LocaleIds
             (LocaleId,
              IsDefault
             )
			  -- here collect all locale ids
             SELECT LocaleId,IsDefault FROM ZnodeLocale mt WHERE IsActive = @IsActive
			  AND (EXISTS (SELECT TOP 1 1  FROM @LocaleId RT WHERE RT.Id = MT.LocaleId )
			 OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleId ));


			IF @PimCategoryHierarchyId > 0 
			Begin 
				 DECLARE @TBL_CategoryCategoryHierarchyIds TABLE (CategoryId int,ParentCategoryId int,PimCategoryHierarchyId INT ,ParentPimCategoryHierarchyId INT  ) 
				 INSERT INTO @TBL_CategoryCategoryHierarchyIds(CategoryId , ParentCategoryId, PimCategoryHierarchyId , ParentPimCategoryHierarchyId)
				 Select Distinct PimCategoryId , Null,PimCategoryHierarchyId,NULL FROM (
				 SELECT PimCategoryId,ParentPimCategoryId,PimCategoryHierarchyId,ParentPimCategoryHierarchyId from DBO.[Fn_GetRecurciveCategoryIds_PimCategoryHierarchy](@PimCategoryHierarchyId,@PimCatalogId)
				 Union 
				 Select PimCategoryId , null,PimCategoryHierarchyId,NULL  from ZnodePimCategoryHierarchy where PimCategoryHierarchyId = @PimCategoryHierarchyId 
				 Union 
				 Select PimCategoryId , null,PimCategoryHierarchyId,NULL  from [Fn_GetRecurciveCategoryIds_PimCategoryHierarchyIdNew] (@PimCategoryHierarchyId,@PimCatalogId) ) Category  

			
				 INSERT INTO @TBL_PimCategoryIds(PimCategoryId,PimParentCategoryId,DisplayOrder,ActivationDate,ExpirationDate,IsActive,PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
				
				 SELECT DISTINCT ZPCH.PimCategoryId,ZPCH2.PimCategoryId  PimParentCategoryId,ZPCH.DisplayOrder,ZPCH.ActivationDate,ZPCH.ExpirationDate,ZPCH.IsActive ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId
				 FROM ZnodePimCategoryHierarchy AS ZPCH 
				 LEFT JOIN ZnodePimCategoryHierarchy AS ZPCH2 ON (ZPCH2.PimCategoryHierarchyId = ZPCH. ParentPimCategoryHierarchyId ) 
				 WHERE ZPCH.PimCatalogId = @PimCatalogId  AND ZPCH.PimCategoryHierarchyId in 
				 (SELECT PimCategoryHierarchyId from @TBL_CategoryCategoryHierarchyIds where CategoryId is not null )  ; 
				
				-- Delete from @TBL_PimCategoryIds where PimCategoryId  in (
				-- select PimCategoryId  from ZnodePublishCategory where PublishCatalogId = @PublishCatalogId 
				--)
		
	
		  
				SELECT @VersionId  = PublishCatalogLogId from ZnodePublishCatalogLog where PublishCatalogId = @PublishCatalogId  and IsCatalogPublished =1 

			

			 	 INSERT INTO @TBL_DeletedPublishCategoryIds (PublishCategoryId,PublishProductId)
				 SELECT ZPC.PublishCategoryId,ZPCP.PublishProductId 
				 FROM ZnodePublishCategory AS ZPC 
				 LEFT JOIN  ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishCategoryId = ZPC.PublishCategoryId AND ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND  ZPCP.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId  )                                                  
				 LEFT JOIN ZnodePublishProduct ZPP ON (zpp.PublishProductId = zpcp.PublishProductId AND zpp.PublishCatalogId = zpcp.PublishCatalogId)
				 LEFT JOIN ZnodePublishCatalog ZPCC ON (ZPCC.PublishCatalogId = ZPCP.PublishCatalogId)
				 WHERE ZPC.PublishCatalogId = @PublishCataLogId 
				 AND ZPC.ParentPimCategoryHierarchyId  in (@PimCategoryHierarchyId)
				 AND ZPC.PimCategoryHierarchyId NOT IN (select PimCategoryHierarchyId FROM @TBL_PimCategoryIds)  ;
				

				 INSERT INTO @TBL_DeletedPublishCategoryIds_new (PublishCategoryId,PublishProductId)
				  SELECT ZPC.PublishCategoryId,ZPCP.PublishProductId 
				 FROM ZnodePublishCategory AS ZPC 
				 LEFT JOIN  ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishCategoryId = ZPC.PublishCategoryId AND ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND  ZPCP.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId  )                                                  
				 LEFT JOIN ZnodePublishProduct ZPP ON (zpp.PublishProductId = zpcp.PublishProductId AND zpp.PublishCatalogId = zpcp.PublishCatalogId)
				 LEFT JOIN ZnodePublishCatalog ZPCC ON (ZPCC.PublishCatalogId = ZPCP.PublishCatalogId)
				 WHERE ZPC.PublishCatalogId = @PublishCataLogId 
				 AND ZPC.ParentPimCategoryHierarchyId  in (@PimCategoryHierarchyId)
				 AND ZPC.PimCategoryHierarchyId NOT IN (select PimCategoryHierarchyId FROM @TBL_PimCategoryIds)
				 AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryHierarchy TU WHERE TU.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId )  ;

			End
			ELSE 
			Begin
				INSERT INTO @TBL_PimCategoryIds(PimCategoryId,PimParentCategoryId,DisplayOrder,ActivationDate,ExpirationDate,IsActive,PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
				SELECT DISTINCT ZPCH.PimCategoryId,ZPCH2.PimCategoryId  PimParentCategoryId,ZPCH.DisplayOrder,ZPCH.ActivationDate,ZPCH.ExpirationDate,ZPCH.IsActive ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId
				FROM ZnodePimCategoryHierarchy AS ZPCH 
				LEFT JOIN ZnodePimCategoryHierarchy AS ZPCH2 ON (ZPCH2.PimCategoryHierarchyId = ZPCH. ParentPimCategoryHierarchyId ) 
				WHERE ZPCH.PimCatalogId = @PimCatalogId; 

			 -- AND IsActive = @IsActive ; -- As discussed with @anup active flag maintain on demo site 23/12/2016
			 --	SELECT * FROM @TBL_PimCategoryIds
			 -- here is find the deleted publish category id on basis of publish catalog

             INSERT INTO @TBL_DeletedPublishCategoryIds_new(PublishCategoryId,PublishProductId)
			 SELECT ZPC.PublishCategoryId,ZPCP.PublishProductId 
				 FROM ZnodePublishCategoryProduct ZPCP
				 INNER JOIN ZnodePublishCategory AS ZPC ON(ZPCP.PublishCategoryId = ZPC.PublishCategoryId AND ZPCP.PublishCatalogId = ZPC.PublishCatalogId)                                                  
				 INNER JOIN ZnodePublishProduct ZPP ON(zpp.PublishProductId = zpcp.PublishProductId AND zpp.PublishCatalogId = zpcp.PublishCatalogId)
				 INNER JOIN ZnodePublishCatalog ZPCC ON(ZPCC.PublishCatalogId = ZPCP.PublishCatalogId)
				 WHERE ZPC.PublishCatalogId = @PublishCataLogId 
				 AND NOT EXISTS
				 (SELECT TOP 1 1 FROM ZnodePimCategoryProduct AS TBPC 
				 inner join ZnodePimCategoryHierarchy ZPCH ON TBPC.PimCategoryId = ZPCH.PimCategoryId
				 WHERE TBPC.PimCategoryId = ZPC.PimCategoryId 
				 AND ZPCH.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId AND TBPC.PimProductId = ZPP.PimProductId 
				 AND ZPCH.PimCatalogId = ZPCC.PimCatalogId);

			End
			
          

			 -- here is find the deleted publish category id on basis of publish catalog
             SET @DeletedPublishCategoryIds = ISNULL(SUBSTRING((SELECT ','+CAST(PublishCategoryId AS VARCHAR(50)) FROM @TBL_DeletedPublishCategoryIds_new AS ZPC
                                              GROUP BY ZPC.PublishCategoryId FOR XML PATH('') ), 2, 4000), '');
			 -- here is find the deleted publish category id on basis of publish catalog
             SET @DeletedPublishProductIds = '';
			 -- Delete the publish category id 
	         EXEC Znode_DeletePublishCatalog @PublishCatalogIds = @PublishCatalogId,@PublishCategoryIds = @DeletedPublishCategoryIds,@PublishProductIds = @DeletedPublishProductIds; 
			
			

             MERGE INTO ZnodePublishCategory TARGET USING  @TBL_PimCategoryIds SOURCE ON
			 (
				 TARGET.PimCategoryId = SOURCE.PimCategoryId 
				 AND TARGET.PublishCatalogId = @PublishCataLogId 
				 AND TARGET.PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId
			 )
			 WHEN MATCHED THEN UPDATE SET TARGET.PimParentCategoryId = SOURCE.PimParentCategoryId,TARGET.CreatedBy = @UserId,TARGET.CreatedDate = @GetDate,
             TARGET.ModifiedBy = @UserId,TARGET.ModifiedDate = @GetDate,PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId,ParentPimCategoryHierarchyId=SOURCE.ParentPimCategoryHierarchyId
             WHEN NOT MATCHED THEN INSERT(PimCategoryId,PublishCatalogId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PimCategoryHierarchyId,ParentPimCategoryHierarchyId) 
			 VALUES(SOURCE.PimCategoryId,@PublishCatalogId,@UserId,@GetDate,@UserId,@GetDate,SOURCE.PimCategoryHierarchyId
			 ,SOURCE.ParentPimCategoryHierarchyId)
             OUTPUT INSERTED.PublishCategoryId,INSERTED.PimCategoryId,INSERTED.PimCategoryHierarchyId,
			 INSERTED.parentPimCategoryHierarchyId 
			 INTO @TBL_PublishPimCategoryIds(PublishCategoryId,PimCategoryId,PimCategoryHierarchyId,parentPimCategoryHierarchyId);
			       
				   
		     -- here update the publish parent category id
             UPDATE ZPC SET [PimParentCategoryId] =TBPC.[PimCategoryId] 
				FROM ZnodePublishCategory ZPC
				INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
				WHERE ZPC.PublishCatalogId =@PublishCatalogId
				AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL
				AND TBPC.PublishCatalogId =@PublishCatalogId
				AND ZPC.PimCategoryId in (select PimCategoryId FROM @TBL_PimCategoryIds);
				;
			 UPDATE a
				SET  a.PublishParentCategoryId = b.PublishCategoryId
				FROM ZnodePublishCategory a 
				INNER JOIN ZnodePublishCategory b   ON (a.parentpimCategoryHierarchyId = b.pimCategoryHierarchyId)
				WHERE a.parentpimCategoryHierarchyId IS NOT NULL 
				AND a.PublishCatalogId =@PublishCatalogId
				AND b.PublishCatalogId =@PublishCatalogId
				AND a.PimCategoryId in (select PimCategoryId FROM @TBL_PimCategoryIds);

			 --UPDATE ZPC SET [PimParentCategoryId] = TBPC.[PimCategoryId] 
			 --FROM ZnodePublishCategory ZPC
    --         INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
			 --WHERE ZPC.PublishCatalogId =@PublishCatalogId
			 --AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL ;

			 -- product are published here 
            --  EXEC Znode_GetPublishProducts @PublishCatalogId,0,@UserId,1,0,0;

             SET @MaxId =(SELECT MAX(RowId)FROM @TBL_LocaleIds);
			 DECLARE @TransferID TRANSFERID 
			 INSERT INTO @TransferID 
			 SELECT DISTINCT  PimCategoryId
			 FROM @TBL_PublishPimCategoryIds 

		          
			 INSERT INTO @TBL_PublishPimCategoryIdsLatest (PublishCategoryId,PimCategoryId,PublishProductId,
			PublishParentCategoryId,PimCategoryHierarchyId,parentPimCategoryHierarchyId, PublishCatalogLogId,
			LocaleId ) 
			 SELECT a.*,Max(b.PublishCatalogLogId) PublishCatalogLogId,b.LocaleId
			 FROM @TBL_PublishPimCategoryIds a
			 LEFT JOIN ZnodePublishCatalogLog b ON (b.PublishCatalogId = @PublishCatalogId)
			 WHERE EXISTS (SELECT TOP 1 1  FROM @LocaleId YTU WHERE YTU.Id = b.LocaleId )
			 AND b.PublishStateId = @PublishStateId
			 GROUP BY a.PublishCategoryId  ,PimCategoryId ,a.PublishProductId ,PublishParentCategoryId ,
			  PimCategoryHierarchyId  ,parentPimCategoryHierarchyId,b.LocaleId

			 			 
             WHILE @Counter <= @MaxId -- Loop on Locale id 
                 BEGIN
                     SET @LocaleIdIn =(SELECT LocaleId FROM @TBL_LocaleIds WHERE RowId = @Counter);
                   
				     SET @AttributeIds = SUBSTRING((SELECT ','+CAST(ZPCAV.PimAttributeId AS VARCHAR(50)) FROM ZnodePimCategoryAttributeValue ZPCAV 
										 WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC WHERE TBPC.PimCategoryId = ZPCAV.PimCategoryId) GROUP BY ZPCAV.PimAttributeId FOR XML PATH('')), 2, 4000);
                
				     SET @CategoryIdCount =(SELECT COUNT(1) FROM @TBL_PimCategoryIds);

                     INSERT INTO @TBL_AttributeIds (PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,
					 IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName)
                     EXEC [Znode_GetPimAttributesDetails] @AttributeIds,@LocaleIdIn;

                     INSERT INTO @TBL_AttributeDefault (PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)
                     EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @AttributeIds,@LocaleIdIn;

                     INSERT INTO @TBL_AttributeValue (PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
                     EXEC [dbo].[Znode_GetCategoryAttributeValueId] @TransferID,@AttributeIds,@LocaleIdIn;

					-- SELECT * FROM @TBL_AttributeValue WHERE PimCategoryId = 281


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
                      AND ZPCP.PublishCatalogId = @PublishCatalogId FOR XML PATH('')), 2, 8000) PublishProductId ,PimCategoryHierarchyId
					  FROM @TBL_PublishPimCategoryIds TBPC)
                          
					 UPDATE TBPPC SET PublishProductId = CTPP.PublishProductId FROM @TBL_PublishPimCategoryIds TBPPC INNER JOIN Cte_PublishProductIds CTPP ON(TBPPC.PublishCategoryId = CTPP.PublishCategoryId 
					 AND TBPPC.PimCategoryHierarchyId = CTPP.PimCategoryHierarchyId);

					 WITH Cte_CategoryProfile AS 
					 (
						SELECT PimCategoryId,ZPCC.PimCategoryHierarchyId,SUBSTRING(( SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfile ZPC 
						INNER JOIN ZnodePimCategoryHierarchy ZPRCC ON(ZPRCC.PimCategoryHierarchyId = ZPCC.PimCategoryHierarchyId
						AND ZPRCC.PimCatalogId = ZPC.PimCatalogId) 
						WHERE ZPC.PimCatalogId = ZPCC.PimCatalogId FOR XML PATH('')), 2, 4000) ProfileIds
                      
						FROM ZnodePimCategoryHierarchy ZPCC 
						WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC 
						WHERE TBPC.PimCategoryId = ZPCC.PimCategoryId AND ZPCC.PimCatalogId = @PimCatalogId 
						AND ZPCC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId)
					)                          
				     UPDATE TBPC SET TBPC.ProfileId = CTCP.ProfileIds FROM @TBL_PimCategoryIds TBPC 
					 LEFT JOIN Cte_CategoryProfile CTCP ON(CTCP.PimCategoryId = TBPC.PimCategoryId AND CTCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId );
                     
					 UPDATE TBPC SET TBPC.CategoryName = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
                     AND EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryNameAttribute]() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId));

					   UPDATE TBPC SET TBPC.CategoryCode = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
					 AND EXISTS(SELECT TOP 1 1 FROM dbo.Fn_GetCategoryCodeAttribute() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId)
					 )


					-- SELECT * FROM @TBL_AttributeValue WHERE pimCategoryId = 369

					 -- here update the publish category details 
                     ;WITH Cte_UpdateCategoryDetails
                     AS 
					 (
							 SELECT TBC.PimCategoryId,PublishCategoryId,CategoryName, TBPPC.PimCategoryHierarchyId,CategoryCode
							 FROM @TBL_PimCategoryIds TBC
							 INNER JOIN @TBL_PublishPimCategoryIds TBPPC ON(TBC.PimCategoryId = TBPPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPPC.PimCategoryHierarchyId)
					 )						
                     MERGE INTO ZnodePublishCategoryDetail TARGET USING Cte_UpdateCategoryDetails SOURCE ON(
					 TARGET.PublishCategoryId = SOURCE.PublishCategoryId
					 AND TARGET.LocaleId = @LocaleIdIn)
                     WHEN MATCHED THEN UPDATE SET PublishCategoryId = SOURCE.PublishcategoryId,PublishCategoryName = SOURCE.CategoryName,LocaleId = @LocaleIdIn,ModifiedBy = @userId,ModifiedDate = @GetDate,CategoryCode=SOURCE.CategoryCode
                     WHEN NOT MATCHED THEN INSERT(PublishCategoryId,PublishCategoryName,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CategoryCode) VALUES
                     (SOURCE.PublishCategoryId,SOURCE.CategoryName,@LocaleIdIn,@userId,@GetDate,@userId,@GetDate,SOURCE.CategoryCode);

------------------------------------------------------------------
					IF OBJECT_ID('tempdb..#Index') is not null
					BEGIN 
						DROP TABLE #Index
					END 
					CREATE TABLE #Index (RowIndex int ,PimCategoryId int , PimCategoryHierarchyId  int,ParentPimCategoryHierarchyId int )		
					insert into  #Index ( RowIndex ,PimCategoryId , PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
					SELECT CAST(Row_number() OVER (Partition By TBL.PimCategoryId Order by ISNULL(TBL.PimCategoryId,0) desc) AS VARCHAR(100))
					,ZPC.PimCategoryId, ZPC.PimCategoryHierarchyId, ZPC.ParentPimCategoryHierarchyId
					FROM @TBL_PublishPimCategoryIdsLatest TBL
					INNER JOIN ZnodePublishCategory ZPC ON (TBL.PimCategoryId = ZPC.PimCategoryId AND TBL.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId)
					WHERE ZPC.PublishCatalogId = @PublishCatalogId

					UPDATE TBP SET  TBP.[RowIndex]=  IDX.RowIndex 
					FROM @TBL_PublishPimCategoryIdsLatest TBP INNER JOIN #Index IDX ON (IDX.PimCategoryId = TBP.PimCategoryId AND IDX.PimCategoryHierarchyId = TBP.PimCategoryHierarchyId)  

					------------------------------------------------------------------


                     ;WITH Cte_CategoryXML
                     AS (SELECT PublishCatalogLogId,PublishcategoryId,PimCategoryId,(SELECT PublishCatalogLogId VersionId,TBPC.PublishCategoryId ZnodeCategoryId,@PublishCatalogId ZnodeCatalogId
																		,THR.PublishParentCategoryId TempZnodeParentCategoryIds,ZPC.CatalogName ,
																		 ISNULL(DisplayOrder, '0') DisplayOrder,@LocaleIdIn LocaleId,ActivationDate 
																		 ,ExpirationDate,TBC.IsActive,ISNULL(CategoryName, '') Name,ProfileId TempProfileIds,ISNULL(PublishProductId, '') TempProductIds,ISNULL(CategoryCode,'') CategoryCode 
																		 ,ISNULL(TBPC.RowIndex,1) CategoryIndex
                        FROM @TBL_PublishPimCategoryIdsLatest TBPC 
						INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId= @PublishCatalogId)
						INNER JOIN ZnodePublishCAtegory THR ON (THR.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId AND THR.PimCategoryId = TBPC.PimCategoryId AND THR.PublishCatalogId= @PublishCatalogId )
						INNER JOIN @TBL_PimCategoryIds TBC ON(TBC.PimCategoryId = TBPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId) 
						WHERE TBPC.PublishCategoryId = TBPCO.PublishCategoryId 
						AND TBPC.LocaleId = @localeIdIn
						FOR XML PATH('')) CategoryXml 
						FROM @TBL_PublishPimCategoryIdsLatest TBPCO 
						WHERE LocaleId = @localeIdIn),

                     Cte_CategoryAttributeXml
                     AS (SELECT PublishCatalogLogId, CTCX.PublishCategoryId,'<CategoryEntity>'+ISNULL(CategoryXml, '')+ISNULL((SELECT(SELECT TBA.AttributeCode,TBA.AttributeName,ISNULL(IsUseInSearch, 0) IsUseInSearch,
                        ISNULL(IsHtmlTags, 0) IsHtmlTags,ISNULL(IsComparable, 0) IsComparable,(SELECT ''+TBAV.CategoryValue FOR XML PATH('')) AttributeValues,TBA.AttributeTypeName FROM @TBL_AttributeValue TBAV
                        INNER JOIN @TBL_AttributeIds TBA ON(TBAV.PimAttributeId = TBA.PimAttributeId) LEFT JOIN ZnodePimFrontendProperties ZPFP ON(ZPFP.PimAttributeId = TBA.PimAttributeId)
                        WHERE CTCX.PimCategoryId = TBAV.PimCategoryId AND TBAO.PimAttributeId = TBA.PimAttributeId FOR XML PATH('AttributeEntity'), TYPE) FROM @TBL_AttributeIds TBAO
                        FOR XML PATH('Attributes')), '')+'</CategoryEntity>' CategoryXMl FROM Cte_CategoryXML CTCX)

                     INSERT INTO @TBL_CategoryXml(PublishCategoryId,CategoryXml,LocaleId,PublishCatalogLogId)
                     SELECT PublishCategoryId,CategoryXml,@localeIdIn LocaleId,PublishCatalogLogId 
					 FROM Cte_CategoryAttributeXml;
                   
				     DELETE FROM @TBL_AttributeIds;
                     DELETE FROM @TBL_AttributeDefault;
                     DELETE FROM @TBL_AttributeValue;
                     SET @Counter = @Counter + 1;
                 END;

				

				 -----------------------
			IF @PimCategoryHierarchyId > 0 
			Begin 
				Select PublishCategoryId ,PublishCatalogLogId VersionId	, @PimCatalogId PimCatalogId	, LocaleId
				into #OutPublish  
				FROM @TBL_CategoryXml  
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
					from #OutPublish where Id = @Counter

					SELECT @ExistingPublishCategoryId  = PublishCategoryId 
					FROM ZnodePublishCatalogLog ZPCL 
					where ZPCL.PublishCatalogLogId = @VersionId  and IsCategoryPublished =1 

			IF NOT EXISTS (SELECT TOP 1 1 FROM Split(@ExistingPublishCategoryId  , ',') SP WHERE SP.Item = Convert(nvarchar(50),  @PublishCategoryId) )
					BEGIN
					
						If Isnull(@ExistingPublishCategoryId,'')  = '' 
							SET @ExistingPublishCategoryId  = Convert(nvarchar(100),@PublishCategoryId )
						else 
							SET @ExistingPublishCategoryId  = Isnull(@ExistingPublishCategoryId,'') + ',' +  Convert(nvarchar(100),@PublishCategoryId )

							
				
						UPDATE ZnodePublishCatalogLog SET PublishCategoryId = @MaxId ,
						ModifiedDate = @GetDate
						WHERE PublishCatalogLogId = @VersionId;
					END
					DELETE FROM ZnodePublishedXml where  IsCategoryXML =1  and  PublishCataloglogId = @VersionId  and  PublishedId = @PublishCategoryId 
					SET @Counter  = @Counter  + 1  
				END
			END 
			ElSE
			Begin
				 UPDATE ZnodePublishCatalogLog 
				 SET PublishCategoryId = (SELECT COUNT(PublishCategoryId)  FROM @TBL_CategoryXml
				 GROUP BY PublishCategoryId																				
				 ), IsCategoryPublished = 1 WHERE PublishCatalogLogId = @VersionId;

				 DELETE FROM ZnodePublishedXml WHERE PublishCataloglogId = @VersionId;
             End
             
			 INSERT INTO ZnodePublishedXml (PublishCatalogLogId,PublishedId,PublishedXML,IsCategoryXML,IsProductXML,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT PublishCatalogLogId PublishCataloglogId,PublishCategoryId,CategoryXml,1,0,LocaleId,@UserId,@GetDate,@UserId,@GetDate 
			 FROM @TBL_CategoryXml 
		
			
			-----------------------------------------------------------------------------------------------------------------------------
			---To get published categories which are published but removed after last publish
			;With Cte_RecursiveAccountId AS
			(
			   SELECT ZPCH.PublishCategoryId ,PimCategoryHierarchyId ,ParentPimCategoryHierarchyId --,PimCategoryId
			   FROM ZnodePublishCategory   ZPCH 
			   WHERE PimCategoryHierarchyId = @PimCategoryHierarchyId
			   UNION ALL 
			   SELECT ZPCH.PublishCategoryId,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId --,ZPCH.PimCategoryId 
			   FROM ZnodePublishCategory   ZPCH 
			   INNER JOIN Cte_RecursiveAccountId CTRA ON (CTRA.PimCategoryHierarchyId = ZPCH.ParentPimCategoryHierarchyId )
			  )
			  select PublishCategoryId from Cte_RecursiveAccountId
			  UNION
			  Select PublishCategoryId from @TBL_DeletedPublishCategoryIds
			  UNION
			  --not published parentcategory
			  SELECT PublishCategoryId FROM ZnodePublishCategory A
			  INNER JOIN @TBL_PimCategoryIds B ON (A.PimCategoryId = B.PimCategoryId)
			  WHERE A.PublishCatalogId = @PublishCataLogId AND B.PimCategoryHierarchyId = A.PimCategoryHierarchyId


			 SELECT CategoryXml 
			 FROM @TBL_CategoryXml 
			
			 UPDATE ZnodePimCategory SET IsCategoryPublish =1,PublishStateId = @PublishStateId WHERE pimCategoryId IN (SELECT PimCategoryId FROM @TBL_PimCategoryIds)
             COMMIT TRAN GetPublishCategory;
			 
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE();
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishCategoryGroup @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(50))+',@UserId ='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             ROLLBACK TRAN GetPublishCategory;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPublishCategoryGroup',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;