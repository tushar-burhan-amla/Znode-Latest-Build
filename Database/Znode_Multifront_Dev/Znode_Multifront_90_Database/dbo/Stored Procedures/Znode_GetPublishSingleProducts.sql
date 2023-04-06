CREATE PROCEDURE [dbo].[Znode_GetPublishSingleProducts]
(
	  @PublishCatalogId int= NULL
	  ,@PublishCategoryId varchar(2000)= NULL
	  ,@UserId int
	  ,@NotReturnXML int= NULL
	  ,@PimProductId varchar(2000)= 0, @VersionId int= 0, @IsDebug bit= 0, @TokenId nvarchar(max)= '')
AS
    
/*
    Summary :	Publish Product on the basis of publish catalog
				Retrive all Product details with attributes and insert into following tables 
				1.	ZnodePublishedXml
				2.	ZnodePublishCategoryProduct
				3.	ZnodePublishProduct
				4.	ZnodePublishProductDetail

                Product details include all the type of products link, grouped, configure and bundel products (include addon) their associated products 
				collect their attributes and values into tables variables to process for publish.  
                
				Finally genrate XML for products with their attributes and inserted into ZnodePublishedXml Znode Admin process xml from sql server to mongodb
				one by one.

    Unit Testing
    
    SELECT * FROM ZnodePimCustomField WHERE CustomCode = 'Test'
    SELECT * FROM ZnodePimCatalogCategory WHERE pimCatalogId = 3 AND PimProductId = 181
    SELECT * FROM ZnodePimCustomFieldLocale WHERE PimCustomFieldId = 1
    SELECT * FROM ZnodePublishProduct WHERE PublishProductid = 213 = 30
    select * from znodepublishcatalog
    SELECT * FROM view_loadmanageProduct WHERE Attributecode = 'ProductNAme' AND AttributeValue LIKE '%Apple%'
    SELECT * FROM ZnodePimCategoryProduct WHERE  PimProductId = 181
    SELECT * FROM ZnodePimCatalogcategory WHERE pimcatalogId = 3 
    EXEC Znode_GetPublishProducts  @PublishCatalogId = NULL ,@UserId= 2 ,@NotReturnXML= NULL,@PimProductId = 117,@IsDebug= 1 
    EXEC Znode_GetPublishProducts  @PublishCatalogId = 3,@UserId= 2 ,@NotReturnXML= NULL,@IsDebug= 1  ,@PimProductId = 12
    EXEC Znode_GetPublishProducts  @PublishCatalogId =1,@UserId= 2 ,@RequiredXML= 1	
    SELECT * FROM 	ZnodePimCatalogCategory  WHERE pimcatalogId = 3  
    SELECT * FROM [dbo].[ZnodePimCategoryHierarchy]  WHERE pimcatalogId = 3 
 */

BEGIN
	
	BEGIN TRY
		--SET NOCOUNT ON;
		DECLARE @start_time datetime;

	
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		DECLARE @PimCatalogId int= ISNULL(
										 (
											 SELECT PimCatalogId
											 FROM ZnodePublishcatalog
											 WHERE PublishCatalogId = @PublishCatalogId
										 ), 0);  --- this variable is used to carry y pim catalog id by using published catalog id
		-- This variable used to carry the locale in loop 
		-- This variable is used to carry the default locale which is globaly set

		DECLARE @LocaleId int, @DefaultLocaleId int= Dbo.Fn_GetDefaultLocaleId(), @ProductNamePimAttributeId int= dbo.Fn_GetProductNameAttributeId(), @SkuPimAttributeId int= dbo.Fn_GetProductSKUAttributeId(), @IsActivePimAttributeId int= dbo.Fn_GetProductIsActiveAttributeId();
			 
		-- This variable is used in loop to increment the counter

		DECLARE @Counter int= 1, @MaxRowId int= 0, @DefaultPimAttributeFamilyId int= Dbo.Fn_GetDefaultPimProductFamilyId();
		DECLARE @DeletePublishProductId varchar(max)= '', @PimProductIds varchar(max)= '', @PimAttributeId varchar(max)= '';
             
		-- This table will used to hold the all currently active locale ids  

		DECLARE @TBL_LocaleIds TABLE
		( 
									 RowId int IDENTITY(1, 1), LocaleId int PRIMARY KEY
		);             
			 
		-- This table is used to hold the Product xml product wise
		DECLARE @TBL_ProductAttributeXml TABLE
		( 
											   PublishProductId int, ProductXml xml, LocaleId int
		);
			 
		-- This table hold the complete xml of product with other information like category and catalog

		DECLARE @TBL_PimProductIds TABLE
		( 
										 PimProductId int, PimCategoryId int, PimCatalogId int, PublishCatalogId int, IsParentProducts bit, DisplayOrder int, ProductName nvarchar(max), SKU nvarchar(max), IsActive nvarchar(max), PimAttributeFamilyId int, ProfileId varchar(max), CategoryDisplayOrder int, ProductIndex int, PRIMARY KEY(PimCatalogId, PimCategoryId, PimProductId)
		);
		DECLARE @TBL_PimProductIds_in TABLE
		( 
											PimProductId int, PimCategoryId int, PimCatalogId int, PublishCatalogId int, IsParentProducts bit, DisplayOrder int, ProductName nvarchar(max), SKU nvarchar(max), IsActive nvarchar(max), PimAttributeFamilyId int, ProfileId varchar(max), CategoryDisplayOrder int, ProductIndex int, PRIMARY KEY(PimCatalogId, PimCategoryId, PimProductId)
		);


		-- This table is used to hold the product which publish in current process 
		DECLARE @TBL_PublishProductIds TABLE
		( 
											 PublishProductId int, PimProductId int, PublishCatalogId int, PublishCategoryId varchar(max), CategoryProfileIds varchar(max), VersionId int, PRIMARY KEY(PimProductId, PublishProductId, PublishCatalogId)
		); 
			 
		 
		-- This table is used hold the published products for finding the attributes details 

		--DECLARE @TBL_AttributeValue TABLE
		--( 
		--								  PimProductId int, AttributeValue nvarchar(max), AttributeCode varchar(300), PimAttributeId int, DefaultValueXML nvarchar(max) PRIMARY KEY(PimProductId, AttributeCode), INDEX TBL_AttributeValue_1(PimProductId, PimAttributeId)
		--);


		
		DECLARE @TBL_PimAttributeIdsLocale TABLE
		( 
			PimAttributeId int, ParentPimAttributeId int, AttributeTypeId int, AttributeCode varchar(300), IsRequired bit, IsLocalizable bit, IsFilterable bit, IsSystemDefined bit, IsConfigurable bit, IsPersonalizable bit, DisplayOrder int, HelpDescription varchar(max), IsCategory bit, IsHidden bit, CreatedDate datetime, ModifiedDate datetime, AttributeName nvarchar(max), AttributeTypeName varchar(300), IsCustomeField bit, LocaleId int,IsSwatch BIT, PRIMARY KEY(PimAttributeId, LocaleId),
			Index Index_116 (LocaleId ),	Index Index_121 (AttributeCode,LocaleId )
		);
		DECLARE @TBL_AttributesDetails TABLE
		( 
			Id Int Identity ,PimAttributeId int, AttributeCode varchar(300), IsUseInSearch bit, IsHtmlTags bit, IsComparable bit, IsFacets bit, AttributeValue varchar(max)
			, IsLinkAttribute bit, PimProductId int, IsSwatch Bit , PRIMARY KEY(Id),
			Index Index_117(PimAttributeId ),Index Index_118(IsLinkAttribute )
		);
				
		DECLARE @TBL_PimAttributeValueId TABLE
		( 
				RowId int, PimAttributeId int, PimAttributeValueId int, PimProductId int, AttributeCode varchar(300) 
				Index Index_118(PimProductId) ,Index Index_119( PimAttributeValueId), Index Index_120( PimProductId,PimAttributeId), Index Index_121( RowId)
		);
		DECLARE @TBL_PimAttributeValueLocale TABLE
		( 
				 Id int Identity,  PimAttributeId int, AttributeValue nvarchar(max), PimProductId int, AttributeCode varchar(300), LocaleId int, PRIMARY KEY(Id),
				 Index Index_113 (LocaleId),Index Index_115(PimProductId, AttributeCode, LocaleId)
		);
		DECLARE @TBL_PimAttributeDefaultValue TABLE
		( 
				ID int Identity , PimAttributeId int, AttributeValue nvarchar(max), PimProductId int, AttributeCode varchar(300), LocaleId int, DisplayOrder int, AttributeDefaultValueCode varchar(600),SwatchText VARCHAR(100),MediaPath VARCHAR(max),Primary Key (ID ),
				Index Index_301 (LocaleId),
				Index Index_302(AttributeCode,PimProductId,LocaleId),
				Index Index_303(AttributeCode,PimProductId),
				Index Index_304(PimProductId,localeId)
		);
		DECLARE @TBL_PimAttributeMediaValue TABLE
		( 
				Id int Identity ,PimAttributeId int, AttributeValue nvarchar(max), PimProductId int, AttributeCode varchar(300), LocaleId int, MediaConfigurationId int, MediaId int
				Primary Key (id ), Index Index_116(AttributeCode ,PimProductId,LocaleId), Index Index_112(LocaleId)
		);
		DECLARE @TBL_ProductCustomeAttribute TABLE
		( 
			PimProductId int, CustomCode varchar(300), CustomKey nvarchar(600), CustomKeyValue nvarchar(600), LocaleId int, PRIMARY KEY(PimProductId, CustomCode, LocaleId),
			Index Index_110 (LocaleId)
		);
		INSERT INTO @Tbl_LocaleIds( LocaleId )
			   SELECT LocaleId
			   FROM ZnodeLocale
			   WHERE IsActive = 1 AND 
					 @NotReturnXML IS NULL ; 
		-- this check is used when this procedure is call by internal procedure to publish only product and no need to return publish xml;    
		--Collected list of products for  publish 
		INSERT INTO @TBL_PimProductIds_in( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId, CategoryDisplayOrder, PublishCatalogId )
			   SELECT ZPCC.PimProductId, ZPCC.PimCategoryId, 1 AS IsParentProducts, NULL AS DisplayOrder, ZPCC.PimCatalogId, ZPCC.DisplayOrder, ZPC.PublishCatalogId
			   FROM ZnodePimCatalogCategory AS ZPCC
					INNER JOIN
					ZnodePublishCatalog AS ZPC
					ON ZPC.PimCatalogId = ZPCC.PimCatalogId
			   WHERE( ZPCC.PimCatalogId = @PimCatalogId OR 
					  EXISTS
					(
						SELECT TOP 1 1
						FROM dbo.split( @PimProductId, ',' ) AS SP
						WHERE SP.Item = ZPCC.PimProductId
					)
					) AND 
					ZPCC.PimProductId IS NOT NULL;
			

		--Collected list of link products for  publish
		INSERT INTO @TBL_PimProductIds_in( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId, PublishCatalogId )
			   SELECT ZPLPD.PimProductId, ZPCC.PimCategoryId, 0 AS IsParentProducts, NULL AS DisplayOrder, CTPP.PimCatalogId, CTPP.PublishCatalogId
			   FROM ZnodePimLinkProductDetail AS ZPLPD
					INNER JOIN
					@TBL_PimProductIds_in AS CTPP
					ON ZPLPD.PimParentProductId = CTPP.PimProductId AND 
					   IsParentProducts = 1
					INNER JOIN
					ZnodePimCatalogCategory AS ZPCC
					ON ZPCC.PimProductId = ZPLPD.PimProductId AND 
					   ZPCC.PimCatalogId = CTPP.PimCatalogId
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_PimProductIds_in AS CTPPI
				   WHERE CTPPI.PimProductId = ZPLPD.PimProductId
			   ) 
					 -- AND EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeValue AS VILMP WHERE VILMP.PimProductId = ZPLPD.PimProductId ) 
					 AND 
					 ZPCC.PimProductId IS NOT NULL
			   GROUP BY ZPLPD.PimProductId, ZPCC.PimCategoryId, CTPP.PimCatalogId, CTPP.PublishCatalogId; 
		--Collected list of Addon products for  publish

		INSERT INTO @TBL_PimProductIds_in( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId, PublishCatalogId )
			   SELECT ZPAPD.PimChildProductId, ISNULL(ZPCC.PimCategoryId, 0) AS PublishCategoryId, 0 AS IsParentProducts, NULL AS DisplayOrder, CTALP.PimCatalogId, CTALP.PublishCatalogId
			   FROM ZnodePimAddOnProductDetail AS ZPAPD
					INNER JOIN
					ZnodePimAddOnProduct AS ZPAP
					ON ZPAP.PimAddOnProductId = ZPAPD.PimAddOnProductId
					INNER JOIN
					@TBL_PimProductIds_in AS CTALP
					ON CTALP.PimProductId = ZPAP.PimProductId AND 
					   IsParentProducts = 1
					LEFT JOIN
					ZnodePimCatalogCategory AS ZPCC
					ON ZPCC.PimProductId = ZPAPD.PimChildProductId AND 
					   ZPCC.PimCatalogId = CTALP.PimCatalogId
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_PimProductIds_in AS CTALPI
				   WHERE CTALPI.PimProductId = ZPAPD.PimChildProductId
			   ) 
			   ---	 AND EXISTS(SELECT TOP 1 1FROM ZnodePimAttributeValue AS VILMP WHERE VILMP.PimProductId = ZPAPD.PimChildProductId)  
			   GROUP BY ZPAPD.PimChildProductId, ZPCC.PimCategoryId, CTALP.PimCatalogId, CTALP.PublishCatalogId;

		--Collected list of Bundle / Group / Config products for  publish
		INSERT INTO @TBL_PimProductIds_in( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId, PublishCatalogId )
			   SELECT ZPTA.PimProductId, ISNULL(ZPCC.PimCategoryId, 0), 0 AS IsParentProducts, NULL AS DisplayOrder, CTAAP.PimCatalogId, CTAAP.PublishCatalogId
			   FROM ZnodePimProductTypeAssociation AS ZPTA
					INNER JOIN
					@TBL_PimProductIds_in AS CTAAP
					ON CTAAP.PimProductId = ZPTA.PimParentProductId AND 
					   IsParentProducts = 1
					LEFT JOIN
					ZnodePimCatalogCategory AS ZPCC
					ON ZPCC.PimProductId = ZPTA.PimProductId AND 
					   ZPCC.PimCatalogId = CTAAP.PimCatalogId
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_PimProductIds_in AS CTAAPI
				   WHERE CTAAPI.PimProductId = ZPTA.PimProductId
			   )
			   --AND EXISTS(SELECT TOP 1 1 FROM ZnodePimAttributeValue AS VILMP WHERE VILMP.PimProductId = ZPTA.PimProductId)
			   GROUP BY ZPTA.PimProductId, ZPCC.PimCategoryId, CTAAP.PimCatalogId, CTAAP.PublishCatalogId;
		INSERT INTO @TBL_PimProductIds( PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId, PublishCatalogId, ProductIndex )
			   SELECT PimProductId, PimCategoryId, IsParentProducts, DisplayOrder, PimCatalogId, PublishCatalogId, ROW_NUMBER() OVER(PARTITION BY PimProductId ORDER BY ISNULL(PimCategoryId, 0)) AS ProductIndex
			   FROM @TBL_PimProductIds_in; 			  
			  
		   
		--UPDATE TBLP 
		--SET Productindex = 0 --CTEPI.ProductIndex 
		--FROM @TBL_PimProductIds TBLP
		--  INNER JOIN Cte_GetProductIndex CTEPI ON (CTEPI.PimProductId = TBLP.PimProductId AND CTEPI.PimCategoryId = TBLP.PimCategoryId)
		-- SELECT * FROM @TBL_PimProductIds

		UPDATE TBPP
		  SET PimAttributeFamilyId = ISNULL(ZP.PimAttributeFamilyId, @DefaultPimAttributeFamilyId)
		FROM @TBL_PimProductIds TBPP
			 INNER JOIN
			 ZnodePimProduct ZP
			 ON Zp.PimProductId = TBPP.PimProductId;
		UPDATE TBPP
		  SET PublishCatalogId = ZPC.PublishCatalogId
		FROM @TBL_PimProductIds TBPP
			 INNER JOIN
			 ZnodePublishCatalog ZPC
			 ON ZpC.PimCatalogId = TBPP.PimCatalogId;
		WHILE 1 = 1 AND 
			  @NotReturnXML IS NULL
		BEGIN
			SET @DeletePublishProductId = SUBSTRING(
												   (
													   SELECT TOP 100 ','+CAST(PublishProductId AS varchar(50))
													   FROM ZnodePublishProduct AS ZPP
													   WHERE NOT EXISTS
													   (
														   SELECT TOP 1 1
														   FROM @TBL_PimProductIds AS TBP
														   WHERE ZPP.PimProductId = TBP.PimProductId
													   ) AND 
															 ZPP.PublishCatalogId = @PublishCatalogId
													   FOR XML PATH('')
												   ), 2, 4000);

			--Remove extra products from catalog
			EXEC dbo.Znode_DeletePublishCatalog @PublishCatalogIds = @PublishCatalogId, @PublishProductIds = @DeletePublishProductId;
			IF ISNULL(@DeletePublishProductId, '') = ''
			BEGIN BREAK
			END;
		END;
		
		WITH Cte_UpdateProduct
			 AS (SELECT PimProductId, PublishCatalogId
				 FROM @TBL_PimProductIds AS TBP
				 GROUP BY PimProductId, PublishCatalogId) 
			
			 -- This merge statement is used for crude oprtaion with publisgh product table
			 MERGE INTO ZnodePublishProduct TARGET
			 USING Cte_UpdateProduct SOURCE
			 ON --check for if already exists then just update otherwise insert the product  
			 TARGET.PimProductId = SOURCE.PimProductId AND 
			 TARGET.PublishCatalogId = SOURCE.PublishCataLogId
			 WHEN MATCHED
				   THEN UPDATE SET TARGET.CreatedBy = @UserId, TARGET.CreatedDate = @GetDate, TARGET.ModifiedBy = @UserId, TARGET.ModifiedDate = @GetDate
			 WHEN NOT MATCHED
				   THEN INSERT(PimProductId, PublishCatalogId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES( SOURCE.PimProductId, SOURCE.PublishCatalogId, @UserId, @GetDate, @UserId, @GetDate )
			 OUTPUT INSERTED.PublishProductId, INSERTED.PimProductId, INSERTED.PublishCatalogId
					INTO @TBL_PublishProductIds(PublishProductId, PimProductId, PublishCatalogId); 
			
					
		-- Here used the ouput clause to catch what data inserted or updated into variable table
		WITH Cte_GetPublishCategory
			 AS (SELECT PublishProductId, ZPC.PublishCategoryId, ZPC.PublishCatalogId
				 FROM ZnodePublishCategory AS ZPC
					  INNER JOIN
					  @TBL_PimProductIds AS TBP
					  ON ISNULL(TBP.PimCategoryId, 0) = ISNULL(ZPC.PimCategoryId, -1)
					  INNER JOIN
					  @TBL_PublishProductIds AS TBPP
					  ON TBP.PimProductId = TBPP.PimProductId AND 
						 ZPC.PublishCatalogId = TBPP.PublishCatalogId)
			 -- This merge staetment is used for crude opration with  ZnodePublishCategoryProduct table
			 MERGE INTO ZnodePublishCategoryProduct TARGET
			 USING Cte_GetPublishCategory SOURCE
			 ON TARGET.PublishProductId = SOURCE.PublishProductId AND 
				ISNULL(TARGET.PublishCategoryId, 0) = ISNULL(SOURCE.PublishCategoryId, 0) AND 
				TARGET.PublishCatalogId = SOURCE.PublishCatalogId
			 WHEN NOT MATCHED
				   THEN INSERT(PublishProductId, PublishCategoryId, PublishCatalogId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES( SOURCE.PublishProductId, SOURCE.PublishCategoryId, SOURCE.PublishCatalogId, @UserId, @GetDate, @userId, @GetDate );
            
		-- Update the Version
		UPDATE TBAE
		  SET VersionId = ZPCL.PublishCatalogLogId
		FROM @TBL_PublishProductIds TBAE
			 INNER JOIN
			 ZnodePublishCatalogLog ZPCL
			 ON ZPCL.PublishCatalogLogId =
		(
			SELECT MAX(ZPCLI.PublishCatalogLogId)
			FROM ZnodePublishCatalogLog AS ZPCLI
			WHERE ZPCLI.PublishcatalogId = TBAE.PublishCatalogId
		)
		WHERE @PublishCatalogId IS NULL;
		-- Profile id updated here
		Declare @Cte_ProductProfile TABLE (Id int Identity , PimProductId int , PimCategoryId int , PimCatalogId int , ProfileIds nvarchar(max)
		Primary Key (Id), Index Index_601 (PimProductId,PimCategoryId))
		--WITH Cte_ProductProfile
		--	 AS 
		Insert into @Cte_ProductProfile (PimProductId, PimCategoryId, PimCatalogId, ProfileIds)
			 (SELECT ZPCC.PimProductId, ZPCC.PimCategoryId, ZPCC.PimCatalogId, SUBSTRING(
																						   (
																							   SELECT DISTINCT 
																									  ','+CAST(ProfileId AS varchar(50))
																							   FROM ZnodeProfileCatalog AS ZPC
																									INNER JOIN
																									ZnodeProfileCatalogCategory AS ZPRCC
																									ON ZPRCC.ProfileCatalogId = ZPC.ProfileCatalogId
																							   WHERE ZPC.PimCatalogId = ZPCC.PimCatalogId AND 
																									 ZPRCC.PimCatalogCategoryId = ZPCC.PimCatalogCategoryId
																							   FOR XML PATH('')
																						   ), 2, 4000) AS ProfileIds
				 FROM ZnodePimCatalogCategory AS ZPCC
					  INNER JOIN
					  @TBL_PimProductIds AS TBP
					  ON( TBP.PimCatalogId = ZPCC.PimCatalogId AND 
						  TBP.PimCategoryId = ZPCC.PimCategoryId AND 
						  TBP.PimProductId = ZPCC.PimProductId
						))


			 UPDATE TBP
			   SET ProfileId = CTCP.ProfileIds
			 FROM @TBL_PimProductIds TBP
				  INNER JOIN
				  @Cte_ProductProfile  CTCP
				  ON CTCP.PimProductId = TBP.PimProductId AND 
					 CTCP.PimCategoryId = TBP.PimCategoryId;
			
		-- SELECT * FROM @TBL_PimProductIds

			
		INSERT INTO @TBL_AttributesDetails( PimAttributeId, AttributeCode, IsComparable, IsUseInSearch, IsHtmlTags, IsFacets,IsSwatch, IsLinkAttribute, PimProductId )
			   SELECT ZPA.PimAttributeId, AttributeCode, IsComparable, IsUseInSearch, IsHtmlTags, IsFacets,IsSwatch, 0, 0
			   FROM ZnodePimAttribute AS ZPA
					LEFT JOIN
					ZnodePimFrontendProperties AS ZPFP
					ON ZPFP.PimAttributeId = ZPA.PimATtributeId
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM ZnodePimAttributeValue AS ZPAV
						INNER JOIN
						@TBL_PimProductIds AS TBP
						ON TBP.PimProductId = ZPAV.PimProductId
						INNER JOIN
						@TBL_PublishProductIds AS TBPP
						ON TBPP.PimProductId = TBP.PimProductId
				   WHERE ZPAV.PimAttributeId = ZPA.PimAttributeId AND 
						 ( EXISTS
						 (
							 SELECT TOP 1 1
							 FROM ZnodePimFamilyGroupMapper AS ZPFGM
							 WHERE ZPFGM.PimAttributeId = ZPA.PimAttributeId AND 
								   ZPFGM.PimAttributeFamilyId = TBP.PimAttributeFamilyId
						 ) OR 
						   EXISTS
						 (
							 SELECT TOP 1 1
							 FROM ZnodePimAttribute AS ZPAI
							 WHERE ZPAV.PimAttributeId = ZPAI.PimAttributeId AND 
								   ZPAI.IsPersonalizable = 1
						 )
						 )
			   ) AND 
				   IsCategory = 0
			   GROUP BY ZPA.PimAttributeId, AttributeCode, IsComparable, IsUseInSearch, IsHtmlTags, IsFacets,IsSwatch
			   UNION ALL
			   SELECT DISTINCT ZPLPD.PimAttributeId, ZPA.AttributeCode, IsComparable, IsUseInSearch, IsHtmlTags, IsFacets,IsSwatch, 1, PimParentProductId
			   FROM ZnodePIMAttribute AS ZPA
					INNER JOIN
					ZnodePimLinkProductDetail AS ZPLPD
					ON ZPLPD.pimAttributeId = ZPA.PimATtributeId
					LEFT JOIN
					ZnodePimFrontendProperties AS ZPFP
					ON ZPFP.PimAttributeId = ZPLPD.PimATtributeId
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_PublishProductIds AS TBPP
				   WHERE TBPP.PimProductId = ZPLPD.PimParentProductId
			   ) AND 
					 IsCategory = 0
			   GROUP BY ZPLPD.PimAttributeId, AttributeCode, IsComparable, IsUseInSearch, IsHtmlTags, IsFacets,IsSwatch, PimParentProductId;

		-- SELECT * FROM @TBL_AttributesDetails
		-- Begin the loop with while on active locale ids  

		INSERT INTO @TBL_PimAttributeValueId( RowId, PimAttributeId, PimAttributeValueId, PimProductId, AttributeCode )
			   SELECT DENSE_RANK() OVER(ORDER BY ZPAV.PimProductId) AS RowId, ZPAV.PimAttributeId, PimAttributeValueId, ZPAV.PimProductId, TBLA.AttributeCode
			   FROM ZnodePimAttributeValue AS ZPAV
					INNER JOIN
					@TBL_AttributesDetails AS TBLA
					ON(TBLA.PimAttributeId = ZPAV.PimAttributeId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_PimProductIds AS tbl
				   WHERE ZPAV.PimProductId = tbl.PimProductId
			   );



		INSERT INTO @TBL_PimAttributeValueLocale( PimProductId, PimAttributeId, AttributeValue, LocaleId, AttributeCode )
			   SELECT PimProductId, PimAttributeId, ZPAVL.AttributeValue, ZPAVL.LocaleId, AttributeCode
			   FROM ZnodePimAttributeValueLocale AS ZPAVL
					INNER JOIN
					@TBL_PimAttributeValueId AS TBLP
					ON(TBLP.PimAttributeValueId = ZPAVL.PimAttributeValueId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_LocaleIds AS TBLL
				   WHERE TBLL.LocaleId = ZPAVL.LocaleId
			   )
			   UNION ALL
			   SELECT PimProductId, PimAttributeId, ZPAVL.AttributeValue, ZPAVL.LocaleId, AttributeCode
			   FROM ZnodePimProductAttributeTextAreaValue AS ZPAVL
					INNER JOIN
					@TBL_PimAttributeValueId AS TBLP
					ON(TBLP.PimAttributeValueId = ZPAVL.PimAttributeValueId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_LocaleIds AS TBLL
				   WHERE TBLL.LocaleId = ZPAVL.LocaleId
			   );

		INSERT INTO @TBL_PimAttributeDefaultValue( PimProductId, PimAttributeId, AttributeValue, LocaleId, AttributeCode, DisplayOrder, AttributeDefaultValueCode,SwatchText,MediaPath )
			   SELECT TBLP.PimProductId, TBLP.PimAttributeId, ZPADVL.AttributeDefaultValue, ZPADVL.LocaleId, AttributeCode, ZPADV.DisplayOrder, ZPADV.AttributeDefaultValueCode,SwatchText,ZM.[Path]
			   FROM @TBL_PimAttributeValueId AS TBLP
					INNER JOIN
					ZnodePimProductAttributeDefaultValue AS ZPADVM
					ON(TBLP.PimAttributeValueId = ZPADVM.PimAttributeValueId)
					INNER JOIN
					ZnodePimAttributeDefaultValue AS ZPADV
					ON(ZPADVM.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)
					INNER JOIN
					ZnodePimAttributeDefaultValueLocale AS ZPADVL
					ON(ZPADVM.PimAttributeDefaultValueId = ZPADVL.PimAttributeDefaultValueId)
					LEFT JOIN ZnodeMedia ZM ON (Zm.MediaId = ZPADV.MediaId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_LocaleIds AS TBLL
				   WHERE TBLL.LocaleId = ZPADVL.LocaleId
			   ) 
			 --AND 
			 --ZPADVM.LocaleId = ZPADVL.LocaleId;



		INSERT INTO @TBL_PimAttributeMediaValue( PimProductId, PimAttributeId, AttributeValue, LocaleId, AttributeCode, MediaConfigurationId, MediaId )
			   SELECT PimProductId, PimAttributeId, ZM.Path AS AttributeValue, LocaleId, AttributeCode, MediaConfigurationId, Zm.MediaId
			   FROM ZnodePimProductAttributeMedia AS ZPAM
					INNER JOIN
					@TBL_PimAttributeValueId AS TBLP
					ON(TBLP.PimAttributeValueId = ZPAM.PimAttributeValueId)
					INNER JOIN
					ZnodeMedia AS ZM
					ON(ZM.MediaId = ZPAM.MediaId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_LocaleIds AS TBLL
				   WHERE TBLL.LocaleId = ZPAM.LocaleId
			   );
		INSERT INTO @TBL_ProductCustomeAttribute( PimProductId, CustomCode, CustomKey, CustomKeyValue, LocaleId )
			   SELECT ZPCF.PimProductId, ZPCF.CustomCode, ZPCFL.CustomKey, CustomKeyValue, ZPCFL.LocaleId
			   FROM ZnodePimCustomField AS ZPCF
					INNER JOIN
					ZnodePimCustomFieldLocale AS ZPCFL
					ON(ZPCFL.PimCustomFieldId = ZPCF.PimCustomFieldId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_LocaleIds AS TBLL
				   WHERE TBLL.LocaleId = ZPCFL.LocaleId
			   ) AND 
					 EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_PimAttributeValueId AS TBLP
				   WHERE TBLP.PimProductId = ZPCF.PimProductId
			   );
		INSERT INTO @TBL_PimAttributeIdsLocale( PimAttributeId, ParentPimAttributeId, AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsFilterable, IsSystemDefined, IsConfigurable, IsPersonalizable, DisplayOrder, HelpDescription, IsCategory, IsHidden, CreatedDate, ModifiedDate, AttributeName, AttributeTypeName, LocaleId,IsSwatch )
			   SELECT ZPA.PimAttributeId, ZPA.ParentPimAttributeId, ZPA.AttributeTypeId, ZPA.AttributeCode, ZPA.IsRequired, ZPA.IsLocalizable, ZPA.IsFilterable, ZPA.IsSystemDefined, ZPA.IsConfigurable, ZPA.IsPersonalizable, ZPA.DisplayOrder, ZPA.HelpDescription, ZPA.IsCategory, ZPA.IsHidden, ZPA.CreatedDate, ZPA.ModifiedDate, ZPAL.AttributeName, ZAT.AttributeTypeName, ZPAL.LocaleId,IsSwatch
			   FROM ZnodePimAttribute AS ZPA
					INNER JOIN
					ZnodePimAttributeLocale AS ZPAL
					ON(ZPAL.PimAttributeId = ZPA.PimAttributeId)
					INNER JOIN
					ZnodeAttributeType AS ZAT
					ON(ZAT.AttributeTypeId = ZPA.AttributeTypeId)
			   WHERE EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_LocaleIds AS TBLL
				   WHERE TBLL.LocaleId = ZPAL.LocaleId
			   ) AND 
					 ZPA.IsCategory = 0 AND 
					 EXISTS
			   (
				   SELECT TOP 1 1
				   FROM @TBL_AttributesDetails AS TBLA
				   WHERE(TBLA.PimAttributeId = ZPA.PimAttributeId)
			   );
		INSERT INTO @TBL_AttributesDetails( PimProductId, AttributeCode, AttributeValue, IsLinkAttribute )
			   SELECT DISTINCT 
					  PimProductId, CustomCode, '', 0
			   FROM @TBL_ProductCustomeAttribute;

		-- SELECT * FROM @TBL_PimAttributeValueId
		SET @MaxRowId = ISNULL(
							  (
								  SELECT MAX(RowId)
								  FROM @TBL_LocaleIds
							  ), 0);
		DECLARE @TBL_PimProductIdToCalculate TABLE
		( 
		PimProductId int PRIMARY KEY
		);
		DECLARE @ProductCount int=
		(
			SELECT COUNT(DISTINCT PimProductId)
			FROM @TBL_PimAttributeValueId
		), @ProductCounter int= 0;
		IF @ProductCount < 100
		BEGIN
			SET @ProductCount = 100;
		END; 
		--SELECT *,@MaxRowId,@ProductCount FROM @TBL_PimAttributeValueId  
		------------------START 
	   WHILE @ProductCounter < @ProductCount AND 
			  @NotReturnXML IS NULL
		BEGIN
			BEGIN TRAN GetPublishProducts;
			SET @start_time = GETDATE();
			IF @ProductCounter = 0
			BEGIN
				INSERT INTO @TBL_PimProductIdToCalculate( PimProductId )
					   SELECT PimProductId
					   FROM @TBL_PimAttributeValueId
					   WHERE RowId BETWEEN 1 AND @ProductCounter + 100
					   GROUP BY PimProductId;
			END;
			ELSE
			BEGIN
				INSERT INTO @TBL_PimProductIdToCalculate( PimProductId )
					   SELECT PimProductId
					   FROM @TBL_PimAttributeValueId
					   WHERE RowId BETWEEN @ProductCounter + 1 AND @ProductCounter + 100
					   GROUP BY PimProductId;
			END;

			WHILE @Counter <= @MaxRowId
			BEGIN -- Start Active Locale Loop 
				-- set the locale id
					--IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE INFORMATION_SCHEMA.TABLES.TABLE_NAME = '@TBL_AttributeValue')
					--BEGIN
					--	Drop table @TBL_AttributeValue
					--END
					
					   Declare @TBL_AttributeValue TABLE 
					   (PimProductId    INT,
					   AttributeValue  NVARCHAR(MAX),
					   AttributeCode   VARCHAR(300),
					   PimAttributeId  INT,
					   DefaultValueXML NVARCHAR(MAX)
				          ,
                       Index TBL_AttributeValue_0 (PimProductId, AttributeCode),
					   Index TBL_AttributeValue_1 (PimProductId,PimAttributeId),
					    Index TBL_AttributeValue_2 (PimProductId)
					  );

				   
				   DECLARE @TBL_CollectPublishedDefaultXML TABLE
				   ( 
						ID int Identity , PimProductId int, PimAttributeId int, AttributeValue nvarchar(max), PRIMARY KEY(ID),
						Index Index_109 (PimProductId, PimAttributeId)
				   );
				   DECLARE @TBL_PimAttributeIds TABLE
				   ( 
						PimAttributeId int, ParentPimAttributeId int, AttributeTypeId int, AttributeCode varchar(300), IsRequired bit, IsLocalizable bit, IsFilterable bit, IsSystemDefined bit, IsConfigurable bit, IsPersonalizable bit, DisplayOrder int, HelpDescription varchar(max), IsCategory bit, IsHidden bit, CreatedDate datetime
						, ModifiedDate datetime, AttributeName nvarchar(max), AttributeTypeName varchar(300), IsCustomeField bit,IsSwatch BIT
						,Index Index_22 (AttributeCode)
				   );

				   DECLARE @TBL_CollectPublishedMediaXML TABLE
				   ( 
						PimProductId int, PimAttributeId int, AttributeValue nvarchar(max), PRIMARY KEY(PimProductId, PimAttributeId)
				   );
				   DECLARE @TBL_CustomeFiled TABLE
				   ( 
					   PimProductId int, CustomCode varchar(300), CustomKey nvarchar(max), CustomKeyValue nvarchar(max), PRIMARY KEY(PimProductId)
				   );
					DECLARE @TBL_ProductAttributeXMLInner TABLE
					( 
						Id int Identity,  PimProductId int, PublishProductId int, PimAttributeId int, AttributeValue nvarchar(max), PRIMARY KEY(Id),
						Index Index_101 (PublishProductId), Index Index_102 (PimProductId, PimAttributeId)
					);
					DECLARE @TBL_ProductDetailsXML TABLE
					( 
						Id int Identity , PublishCatalogId int, PublishProductId int, PublishCategoryId int, AttributeValue nvarchar(max), PRIMARY KEY(ID)
					);
				SET @LocaleId =
				(
					SELECT LocaleId
					FROM @TBL_LocaleIds
					WHERE RowID = @Counter
				); 
				-- publish attribute collection

				WITH Cte_Attributedefaultlocale
					 AS (
							 SELECT a.PimProductId, AttributeValue, AttributeCode, PimAttributeId
							 FROM @TBL_PimAttributeValueLocale a 
							 INNER JOIN	 @TBL_PimProductIdToCalculate AS b ON(b.PimProductId = a.PimProductId)
							 WHERE LocaleId = @LocaleId
							 UNION ALL
							 SELECT TBLAV.PimProductId, AttributeValue, AttributeCode, PimAttributeId
							 FROM @TBL_PimAttributeValueLocale AS TBLAV
							 INNER JOIN	 @TBL_PimProductIdToCalculate AS b ON(b.PimProductId = TBLAV.PimProductId)
							 WHERE LocaleId = @DefaultLocaleId AND 
						     NOT EXISTS
							 (
								 SELECT TOP 1 1
								 FROM @TBL_PimAttributeValueLocale AS TBLPO
								 INNER JOIN	 @TBL_PimProductIdToCalculate AS b ON(b.PimProductId = TBLPO.PimProductId)
								 WHERE TBLPO.PimProductId = TBLAV.PimProductId AND 
									   TBLPO.AttributeCode = TBLAV.AttributeCode AND 
									   TBLPO.LocaleId = @LocaleId
							 )
					     )


					 INSERT INTO @TBL_AttributeValue( PimProductId, AttributeValue, AttributeCode, PimAttributeId )
							SELECT a.PimProductId, AttributeValue, AttributeCode, PimAttributeId
							FROM Cte_Attributedefaultlocale AS a
								 --INNER JOIN
								 --@TBL_PimProductIdToCalculate AS b
								 --ON(b.PimProductId = a.PimProductId);

	

				INSERT INTO @TBL_AttributeValue( PimProductId, AttributeCode, PimAttributeId )
					   SELECT DISTINCT 
							  a.PimProductId, a.AttributeCode, a.PimAttributeId
					   FROM @TBL_PimAttributeValueId AS a
							INNER JOIN
							@TBL_PimProductIdToCalculate AS b
							ON(b.PimProductId = a.PimProductId)
					   WHERE NOT EXISTS
					   (
						   SELECT TOP 1 1
						   FROM @TBL_AttributeValue AS b
						   WHERE a.PimProductId = b.PimProductId AND 
								 a.PimAttributeId = b.PimAttributeId
					   );
					INSERT INTO @TBL_PimAttributeIds( PimAttributeId, ParentPimAttributeId, AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsFilterable, IsSystemDefined, IsConfigurable, IsPersonalizable, DisplayOrder, HelpDescription, IsCategory, IsHidden, CreatedDate, ModifiedDate, AttributeName, AttributeTypeName,IsSwatch )
					 SELECT PimAttributeId, ParentPimAttributeId, AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsFilterable, IsSystemDefined, IsConfigurable
						, IsPersonalizable, DisplayOrder, HelpDescription, IsCategory, IsHidden, CreatedDate, ModifiedDate, AttributeName, AttributeTypeName,IsSwatch
					 FROM @TBL_PimAttributeIdsLocale
					 WHERE localeId = @localeId
					 AND  PimAttributeId IS NOT NULL
					 UNION ALL
					 SELECT PimAttributeId, ParentPimAttributeId, AttributeTypeId, AttributeCode, IsRequired, IsLocalizable
					 , IsFilterable, IsSystemDefined, IsConfigurable, IsPersonalizable, DisplayOrder
					 , HelpDescription, IsCategory, IsHidden, CreatedDate, ModifiedDate, AttributeName, AttributeTypeName,IsSwatch
					 FROM @TBL_PimAttributeIdsLocale AS TBLPA
					 WHERE localeId = @DefaultlocaleId AND 
						   NOT EXISTS
					 (
						 SELECT TOP 1 1
						 FROM @TBL_PimAttributeIdsLocale AS TBLPAS
						 WHERE TBLPAS.AttributeCode = TBLPA.AttributeCode AND 
							   TBLPAS.localeId = @localeId
					 )
					 AND  PimAttributeId IS NOT NULL
					
					 
							--SELECT PimAttributeId, ParentPimAttributeId, AttributeTypeId, AttributeCode, IsRequired, IsLocalizable, IsFilterable, IsSystemDefined, IsConfigurable, IsPersonalizable, DisplayOrder, HelpDescription, IsCategory, IsHidden, CreatedDate, ModifiedDate, AttributeName, AttributeTypeName,IsSwatch
							--FROM Cte_AttributeDetails AS a
							--WHERE PimAttributeId IS NOT NULL;
				--WITH Cte_CustomeValue
				--	 AS (

					  INSERT INTO @TBL_CustomeFiled( PimProductId, CustomCode, CustomKey, CustomKeyValue )
					 SELECT a.PimProductId, CustomCode, CustomKey, CustomKeyValue
					 FROM @TBL_ProductCustomeAttribute a 
					 INNER JOIN  @TBL_PimProductIdToCalculate AS b
								 ON(b.PimProductId = a.PimProductId)
					 WHERE LocaleId = @LocaleId
					 UNION
					 SELECT TBLPA.PimProductId, CustomCode, CustomKey, CustomKeyValue
					 FROM @TBL_ProductCustomeAttribute AS TBLPA
					 INNER JOIN  @TBL_PimProductIdToCalculate AS b
								 ON(b.PimProductId = TBLPA.PimProductId)

					 WHERE LocaleId = @DefaultLocaleId AND 
						   NOT EXISTS
					 (
						 SELECT TOP 1 1
						 FROM @TBL_ProductCustomeAttribute AS TBLPAI
						 INNER JOIN  @TBL_PimProductIdToCalculate AS b ON TBLPAI.PimProductId = b.PimProductId
						 WHERE TBLPAI.PimProductId = TBLPA.PimProductId AND
							   TBLPAI.CustomCode = TBLPA.CustomCode AND 
							   TBLPAI.LocaleId = @LocaleId
					 )
					 
					 
					
							--SELECT a.PimProductId, CustomCode, CustomKey, CustomKeyValue
							--FROM Cte_CustomeValue AS a;
								 --INNER JOIN  @TBL_PimProductIdToCalculate AS b
								 --ON(b.PimProductId = a.PimProductId);
					 
				
				-- get custome fields of products
				-- Get Link Product value
				-- Changes related to performance 
				--Declare @ChildLinkPublishProductId TABLE (PimProductId int, AttributeValue nvarchar(max))
				--select * from ZnodePimLinkProductDetail
	
				 Declare @Cte_UpdateDefaultValue TABLE (id int identity , PimProductId int , PimAttributeId int ,AttributeCode varchar(300),AttributeValue nvarchar(max),
Primary Key (Id), Index Index_602(PimProductId))

				INSERT INTO @Cte_UpdateDefaultValue 
				
				SELECT TBAV.PimProductId, TBAV.PimAttributeId, TBAV.AttributeCode, SUBSTRING(
																									 (
																										 SELECT ','+CAST(PublishProductId AS Varchar(50))
																										 FROM ZnodePublishProduct AS ZPP
																											  INNER JOIN
																											  ZnodePimLinkProductDetail AS ZPLP
																											  ON(ZPLP.PimProductId = ZPP.PimProductId)
																										 WHERE ZPLP.PimParentProductId = TBAV.PimProductId AND 
																											   ZPLP.PimAttributeId = TBAV.PimAttributeId
																										 FOR XML PATH('')
																									 ), 2, 8000) AS AttributeValue
						 FROM @TBL_AttributesDetails AS TBAV
						 WHERE IsLinkAttribute = 1 and TBAV.PimProductId IN (SELECT PimProductId FROM  @TBL_PimProductIdToCalculate)

					 INSERT INTO @TBL_AttributeValue( PimProductId, AttributeValue, AttributeCode, PimAttributeId )
							SELECT a.PimProductId, AttributeValue, AttributeCode, PimAttributeId
							FROM @Cte_UpdateDefaultValue AS a
							--	 INNER JOIN
							--	 @TBL_PimProductIdToCalculate AS b
							--	 ON(b.PimProductId = a.PimProductId)
							WHERE a.PimProductId <> 0; 
				-- Get default value xml +


				
				Declare @Cte_AttributeDefault_0 TABLE (Id int Identity, AttributeValue Nvarchar(max), DisplayOrder int 
											, AttributeDefaultValueCode VARCHAR(300), PimProductId int , PimAttributeId int
											, SwatchText VARCHAR(100),MediaPath VARCHAR(max) 
				,Primary Key (Id), Index Index_401 (PimProductId,PimAttributeId)
				)

			

				--WITH Cte_AttributeDefault
				--	 AS (
					Insert into @Cte_AttributeDefault_0 (AttributeValue, DisplayOrder, AttributeDefaultValueCode, PimProductId, PimAttributeId,SwatchText,MediaPath)
					 SELECT AttributeValue, DisplayOrder, AttributeDefaultValueCode, TBADV.PimProductId, PimAttributeId,SwatchText,MediaPath
					 FROM @TBL_PimAttributeDefaultValue AS TBADV   
					
					  --where EXISTS (SELECT top 1 1 FROM @TBL_PimProductIdToCalculate WHERE PimProductId = TBADV.PimProductId)
					 
					 INNER JOIN  @TBL_PimProductIdToCalculate AS b  ON(b.PimProductId = TBADV.PimProductId)
					 WHERE TBADV.LocaleId = @LocaleId

					 Insert into @Cte_AttributeDefault_0 (AttributeValue, DisplayOrder, AttributeDefaultValueCode, PimProductId, PimAttributeId,SwatchText,MediaPath)
					 SELECT AttributeValue, DisplayOrder, AttributeDefaultValueCode, TBADV.PimProductId, PimAttributeId,SwatchText,MediaPath
					 FROM @TBL_PimAttributeDefaultValue AS TBADV 
					 -- where EXISTS (SELECT top 1 1 FROM @TBL_PimProductIdToCalculate WHERE PimProductId = TBADV.PimProductId)
					 INNER JOIN  @TBL_PimProductIdToCalculate AS b  ON(b.PimProductId = TBADV.PimProductId)
					  WHERE  TBADV.LocaleId = @DefaultLocaleId 
					 AND 
					 NOT EXISTS
					 (
						 SELECT TOP 1 1
						 FROM @Cte_AttributeDefault_0 AS TBADVI
						 WHERE TBADVI.AttributeDefaultValueCode = TBADV.AttributeDefaultValueCode AND 
							   TBADVI.PimProductId = TBADV.PimProductId 
					 );

					 ;With Cte_UpdateDefaultValue
					 AS (SELECT TBAV.PimProductId, TBAV.PimAttributeId, '<SelectValues>'+
						 (
							 SELECT DISTINCT AttributeValue AS Value, ISNULL(DisplayOrder, 0) AS DisplayOrder, AttributeDefaultValueCode AS Code
										,ISNULL(SwatchText,'')SwatchText,ISNULL(MediaPath,'') [Path]
							 FROM @Cte_AttributeDefault_0 AS TBADV
							 WHERE TBADV.PimProductId = TBAV.PimProductId 
							 AND TBADV.PimAttributeId = TBAV.PimAttributeId 
							 FOR XML PATH('SelectValuesEntity')
						 )+'</SelectValues>' AS AttributeValue
						 FROM @TBL_PimAttributeValueId AS TBAV --where EXISTS (SELECT top 1 1 FROM @TBL_PimProductIdToCalculate WHERE PimProductId = TBAV.PimProductId))
					 INNER JOIN @TBL_PimProductIdToCalculate AS b ON(b.PimProductId = TBAV.PimProductId))
										
					 INSERT INTO @TBL_CollectPublishedDefaultXML( pimproductid, PimAttributeId, AttributeValue )
							SELECT pimproductid, PimAttributeId, AttributeValue
							FROM Cte_UpdateDefaultValue
							WHERE AttributeValue <> '<SelectValue/>';

		
				--Declare @Cte_AttributeDefault_1 TABLE (Id int Identity, AttributeValue Nvarchar(max), DisplayOrder int , AttributeDefaultValueCode VARCHAR(300), PimProductId int , PimAttributeId int
				--Primary Key (Id), Index Index_401 (PimProductId,PimAttributeId)
				--)
				
				----WITH Cte_AttributeDefault
				----	 AS (
				--	 insert into @Cte_AttributeDefault_1 (AttributeValue, DisplayOrder, AttributeDefaultValueCode, TBADV.PimProductId, PimAttributeId)
				--	 SELECT AttributeValue, DisplayOrder, AttributeDefaultValueCode, TBADV.PimProductId, PimAttributeId
				--	 FROM @TBL_PimAttributeDefaultValue AS TBADV INNER JOIN @TBL_PimProductIdToCalculate AS b ON(b.PimProductId = TBADV.PimProductId)
				--	 WHERE TBADV.LocaleId = @LocaleId
				--	 UNION ALL
				--	 SELECT AttributeValue, DisplayOrder, AttributeDefaultValueCode, TBADV.PimProductId, PimAttributeId
				--	 FROM @TBL_PimAttributeDefaultValue AS TBADV INNER JOIN @TBL_PimProductIdToCalculate AS b  ON(TBADV.PimProductId = b.PimProductId) 
				--	 WHERE TBADV.LocaleId = @DefaultLocaleId 
						   
				--	 AND NOT EXISTS
				--	 (
				--		 SELECT TOP 1 1
				--		 FROM @TBL_PimAttributeDefaultValue AS TBADVI
				--		 WHERE TBADVI.AttributeCode = TBADV.AttributeCode AND 
				--			   TBADVI.PimProductId = TBADV.PimProductId AND 
				--			   TBADVI.LocaleId = @LocaleId
				--	 )--),

				--	 Declare @Cte_AttributeValueUpdate TABLE (Id int Identity,  PimProductId int ,PimAttributeId int,AttributeValue Nvarchar(max) Primary Key (Id),Index Index_401 (PimProductId,PimAttributeId))

				--	 --;With Cte_AttributeValueUpdate
				--	 --AS 
				--	 insert into @Cte_AttributeValueUpdate (PimProductId ,PimAttributeId ,AttributeValue)
				--	 (SELECT TBAV.PimProductId, TBAV.PimAttributeId, SUBSTRING(
				--																 (
				--																	 SELECT ','+ AttributeValue-- AttributeDefaultValueCode
				--																	 FROM @Cte_AttributeDefault_1 AS TBAVI
				--																	 WHERE TBAV.PimProductId = TBAVI.PimProductId and TBAV.PimAttributeId = TBAVI.PimAttributeId 
				--																	 FOR XML PATH('')
				--																 ), 2, 4000) AS AttributeValue
				--		 FROM @TBL_PimAttributeValueId AS TBAV  
				--		 INNER JOIN  @TBL_PimProductIdToCalculate AS b  ON(b.PimProductId = TBAV.PimProductId)
				--		 WHERE EXISTS
				--		 (
				--			 SELECT TOP 1 1
				--			 FROM @Cte_AttributeDefault_1 AS TRT
				--			 WHERE TRT.PimProductId = TBAV.PimProductId AND  TRT.PimAttributeId = TBAV.PimAttributeId ))

				--	 UPDATE TBAV
				--	   SET AttributeValue = CTAVD.AttributeValue
				--	 FROM @TBL_AttributeValue TBAV
				--		  INNER JOIN
				--		  @Cte_AttributeValueUpdate CTAVD
				--		  ON( TBAV.PimProductId = CTAVD.PimProductId AND TBAV.pimAttributeID = CTAVD.pimAttributeID );


				--- Code commented as per requirement change on 08-05-2017
		

				Declare @Cte_Attributemedia TABLE (Id int Identity, [Path] nvarchar(max) , PimProductId int , PimAttributeId int , MediaConfigurationId int 
				, AttributeCode Varchar(300) Primary Key (Id), Index Index_501(PimProductId,PimAttributeId))
				--WITH Cte_Attributemedia 
				--	 AS 
				Insert into @Cte_Attributemedia ([Path], PimProductId , PimAttributeId , MediaConfigurationId , AttributeCode )
					 (
					 SELECT AttributeVAlue AS [Path], TBADV.PimProductId, PimAttributeId, MediaConfigurationId, AttributeCode
					 FROM @TBL_PimAttributeMediaValue AS TBADV
						  INNER JOIN
						  @TBL_PimProductIdToCalculate AS b
						  ON(b.PimProductId = TBADV.PimProductId)
					 WHERE LocaleId = @LocaleId
					 UNION
					 SELECT AttributeValue AS [Path], TBADV.PimProductId, PimAttributeId, MediaConfigurationId, AttributeCode
					 FROM @TBL_PimAttributeMediaValue AS TBADV
						  INNER JOIN
						  @TBL_PimProductIdToCalculate AS b
						  ON(b.PimProductId = TBADV.PimProductId)
					 WHERE LocaleId = @DefaultLocaleId AND 
						   NOT EXISTS
					 (
						 SELECT TOP 1 1
						 FROM @TBL_PimAttributeMediaValue AS TBADVI
						 WHERE TBADVI.AttributeCode = TBADV.AttributeCode AND 
							   TBADVI.PimProductId = TBADV.PimProductId AND 
							   TBADVI.LocaleId = @LocaleId
					 ));

					 Declare @Cte_MediaAttributeValueUpdate TABLE (Id int identity , PimProductId int , PimAttributeId int ,AttributeValue nvarchar(max),
					 Primary Key (Id), Index Index_508(PimProductId,PimAttributeId))

					 --With Cte_AttributeValueUpdate
					 --AS
					  Insert into @Cte_MediaAttributeValueUpdate ( PimProductId  , PimAttributeId  ,AttributeValue )
					  (SELECT TBAV.PimProductId, TBAV.PimAttributeId, SUBSTRING(
																				 (
																					 SELECT ','+[Path]
																					 FROM @Cte_Attributemedia  AS TBAVI
																					 WHERE TBAV.PimProductId = TBAVI.PimProductId AND TBAV.PimAttributeId = TBAVI.PimAttributeId  
																					 FOR XML PATH('')
																				 ), 2, 4000) AS AttributeValue
						 FROM @TBL_PimAttributeValueId AS TBAV
							  INNER JOIN
							  @TBL_PimProductIdToCalculate AS b
							  ON(b.PimProductId = TBAV.PimProductId)
						 WHERE EXISTS
						 (
							 SELECT TOP 1 1
							 FROM @Cte_Attributemedia  AS TRT
							 WHERE TRT.PimProductId = TBAV.PimProductId AND 
								   TRT.PimAttributeId = TBAV.PimAttributeId
						 ))
					 UPDATE TBAV
					   SET AttributeValue = CTAVD.AttributeValue
					 FROM @TBL_AttributeValue TBAV
						  INNER JOIN
						  @Cte_MediaAttributeValueUpdate CTAVD
						  ON( TBAV.PimProductId  = CTAVD.PimProductId  AND 
							  TBAV.pimAttributeID = CTAVD.pimAttributeID 
							);

				--update the product name and sku -- 24/12/2016 added the isActive 
				UPDATE TBP
				  SET ProductName = TBAV.AttributeValue
				FROM @TBL_PimProductIds TBP
					 INNER JOIN
					 @TBL_AttributeValue TBAV
					 ON TBAV.PimProductId = TBP.PimProductId AND 
						TBAV.pimAttributeId = @ProductNamePimAttributeId;
				--AND TBAV.pimAttributeId  = ( SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA  WHERE ZPA.AttributeCode = 'ProductName' AND ZPA.IsCategory = 0);
				UPDATE TBP
				  SET SKU = TBAVI.AttributeValue
				FROM @TBL_PimProductIds TBP
					 INNER JOIN
					 @TBL_AttributeValue TBAVI
					 ON TBAVI.PimProductId = TBP.PimProductId AND 
						TBAVI.pimAttributeId = @SKUPimAttributeId;
				--AND TBAVI.pimAttributeId  = ( SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA  WHERE ZPA.AttributeCode = 'SKU' AND ZPA.IsCategory = 0);
				UPDATE TBP
				  SET IsActive = TBAVIO.AttributeValue
				FROM @TBL_PimProductIds TBP
					 INNER JOIN
					 @TBL_AttributeValue TBAVIO
					 ON TBAVIO.PimProductId = TBP.PimProductId AND 
						TBAVIO.pimAttributeId = @IsActivePimAttributeId;
				-- AND TBAVIO.pimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPA WHERE ZPA.AttributeCode = 'IsActive' AND ZPA.IsCategory = 0);


				WITH Cte_UpdateProductDetails
					 AS (SELECT PublishProductId, ProductName, SKU, IsActive, @LocaleId AS LocaleId, @UserId AS UserId, @GetDate AS CurrentDate
						 FROM @TBL_PimProductIds AS TBP
							  INNER JOIN
							  @TBL_PublishProductIds AS TBPP
							  ON TBPP.PimProductId = TBP.PimProductId
							  INNER JOIN
							  @TBL_PimProductIdToCalculate AS b
							  ON(b.PimProductId = TBP.PimProductId)
						 GROUP BY PublishProductId, ProductName, SKU, IsActive)


					 MERGE INTO ZnodePublishProductDetail TARGET  -- update the publish product details 
					 USING Cte_UpdateProductDetails SOURCE
					 ON TARGET.PublishProductId = Source.PublishProductId AND 
						Target.LocaleId = SOURCE.LocaleId
					 WHEN MATCHED
						   THEN UPDATE SET ProductName = SOURCE.ProductName, SKU = SOURCE.SKU, IsActive = SOurce.IsActive, ModifiedBy = SOURCE.UserId, ModifiedDate = SOURCE.CurrentDate
					 WHEN NOT MATCHED
						   THEN INSERT(PublishProductId, ProductName, SKU, IsActive, LocaleId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES( SOURCE.PublishProductId, SOURCE.ProductName, SOURCE.SKU, SOurce.IsActive, SOURCE.LocaleId, SOURCE.UserId, SOURCE.CurrentDate, SOURCE.UserId, SOURCE.CurrentDate );
                       
				    	
				-- Added the custome fileds 
				INSERT INTO @TBL_PimAttributeIds( AttributeCode, IsCustomeField, AttributeName )
					   SELECT CustomCode, 1, CustomKey
					   FROM @TBL_CustomeFiled AS a
							INNER JOIN
							@TBL_PimProductIdToCalculate AS b
							ON(b.PimProductId = a.PimProductId);
				INSERT INTO @TBL_AttributeValue( PimProductId, AttributeCode, AttributeValue )
					   SELECT a.PimProductId, CustomCode, CustomKeyValue
					   FROM @TBL_CustomeFiled AS a
							INNER JOIN
							@TBL_PimProductIdToCalculate AS b
							ON(b.PimProductId = a.PimProductId);

	
                 
				-- SELECT * FROM @TBL_PimProductIds
				--- create the basic xml tag of product
				INSERT INTO @TBL_ProductDetailsXML( PublishCatalogId, PublishProductId, PublishCategoryId, AttributeValue )
					   SELECT TBPPI.PublishCatalogId, TBPPI.PublishProductId, ISNULL(ZPC.PublishCategoryId, 0),
					   (
						   SELECT ISNULL(TBPP.VersionId, @VersionId) AS VersionId, TBPP.PublishProductId AS ZnodeProductId, ZPC.PublishCategoryId AS ZnodeCategoryIds, TBP.ProductName AS Name, TBP.SKU, TBP.IsActive, TBPP.PublishCatalogId AS ZnodeCatalogId, TBP.IsParentProducts, TBP.ProfileId AS TempProfileIds, ZPCD.PublishCategoryName AS CategoryName, ZPCA.CatalogName AS CatalogName, TBP.CategoryDisplayOrder AS DisplayOrder, @LocaleId AS LocaleId, TBP.ProductIndex
						   FROM @TBL_PublishProductIds AS TBPP
								INNER JOIN
								@TBL_PimProductIds AS TBP
								ON( TBP.PimProductId = TBPP.PimProductId AND 
									TBP.PublishCatalogId = TBPP.PublishCatalogId
								  )
								INNER JOIN
								@TBL_PimProductIdToCalculate AS b
								ON(b.PimProductId = TBP.PimProductId)
						   WHERE TBPP.PublishProductId = TBPPI.PublishProductId AND 
								 (TBP.PimCategoryId = ZPC.PimCategoryId) AND 
								 TBPP.PublishCatalogId = TBPPI.PublishCatalogId
						   GROUP BY TBPP.PublishProductId, TBP.ProductName, TBP.SKU, TBPP.PublishCatalogId, TBP.IsParentProducts, TBP.ProfileId, TBPP.VersionId, TBP.IsActive, TBP.CategoryDisplayOrder, TBP.ProductIndex
						   FOR XML PATH('')
					   ) AS ProductXml
					   FROM @TBL_PublishProductIds AS TBPPI
							INNER JOIN
							@TBL_PimProductIds AS TBPI
							ON TBPI.PimProductId = TBPPI.PimProductId AND 
							   TBPI.PublishCatalogId = TBPPI.PublishCatalogId
							INNER JOIN
							ZnodePublishCategory AS ZPC WITH (NOLOCK)
							ON( ZPC.PublishCatalogId = TBPPI.PublishCatalogId AND 
								(ZPC.PimCategoryId = TBPI.PimCategoryId)
							  )
							INNER JOIN
							ZnodePublishCategoryDetail AS ZPCD WITH (NOLOCK)
							ON( ZPCD.PublishCategoryId = ZPC.PublishCategoryId AND 
								ZPCD.LocaleId = @LocaleId
							  )
							INNER JOIN
							ZnodePublishCatalog AS ZPCA WITH (NOLOCK)
							ON(ZPCA.PublishCatalogId = TBPPI.PublishCatalogId)
							INNER JOIN
							@TBL_PimProductIdToCalculate AS b
							ON(b.PimProductId = TBPI.PimProductId);
                             
				--- this is for is parent product 0 condition 

				INSERT INTO @TBL_ProductDetailsXML( PublishCatalogId, PublishProductId, PublishCategoryId, AttributeValue )
					   SELECT TBPPI.PublishCatalogId, TBPPI.PublishProductId, 0,
					   (
						   SELECT ISNULL(TBPP.VersionId, @VersionId) AS VersionId, TBPP.PublishProductId AS ZnodeProductId, TBP.ProductName AS Name, TBP.SKU, TBP.IsActive, TBPP.PublishCatalogId AS ZnodeCatalogId, TBP.IsParentProducts, TBP.ProfileId AS TempProfileIds, ZPCA.CatalogName AS CatalogName, TBP.CategoryDisplayOrder AS DisplayOrder, @LocaleId AS LocaleId, TBP.ProductIndex
						   FROM @TBL_PublishProductIds AS TBPP
								INNER JOIN
								@TBL_PimProductIds AS TBP
								ON( TBP.PimProductId = TBPP.PimProductId AND 
									TBP.PublishCatalogId = TBPP.PublishCatalogId
								  )
								INNER JOIN
								@TBL_PimProductIdToCalculate AS b
								ON(b.PimProductId = TBP.PimProductId)
						   WHERE TBPP.PublishProductId = TBPPI.PublishProductId AND 
								 TBPP.PublishCatalogId = TBPPI.PublishCatalogId
						   GROUP BY TBPP.PublishProductId, TBP.ProductName, TBP.SKU, TBPP.PublishCatalogId, TBP.IsParentProducts, TBP.ProfileId, TBPP.VersionId, TBP.IsActive, TBP.CategoryDisplayOrder, TBP.ProductIndex
						   FOR XML PATH('')
					   ) AS ProductXml
					   FROM @TBL_PublishProductIds AS TBPPI
							INNER JOIN
							@TBL_PimProductIds AS TBPI
							ON TBPI.PimProductId = TBPPI.PimProductId AND 
							   TBPI.PublishCatalogId = TBPPI.PublishCatalogId
							INNER JOIN
							ZnodePublishCatalog AS ZPCA WITH (NOLOCK)
							ON(ZPCA.PublishCatalogId = TBPPI.PublishCatalogId)
							INNER JOIN
							@TBL_PimProductIdToCalculate AS b
							ON(b.PimProductId = TBPI.PimProductId)
					   WHERE TBPI.IsParentProducts = 0;



				INSERT INTO @TBL_ProductAttributeXMLInner( PimProductId, PublishProductId, PimAttributeId, AttributeValue )
					   SELECT DISTINCT 
							  TBADF.PimProductId, TBADF.PublishProductId, ISNULL(TBAVI.PimAttributeId, 0),
					   (
						   SELECT DISTINCT 
								  TBAV.AttributeCode, AttributeName, ISNULL(IsUseInSearch, 0) AS IsUseInSearch, ISNULL(IsHtmlTags, 0) AS IsHtmlTags, 
								  ISNULL(IsComparable, 0) AS IsComparable, ISNULL(IsFacets, 0) AS IsFacets, ISNULL(TBAV.AttributeValue, '') AS AttributeValues, 
								  AttributeTypeName, IsPersonalizable, ISNULL(TBA.IsCustomeField, 0) AS IsCustomeField, ISNULL(IsConfigurable, 0) AS IsConfigurable
								  ,ISNULL(TBAD.IsSwatch,0)IsSwatch
						   FROM @TBL_AttributeValue AS TBAV 
						   INNER JOIN @TBL_AttributesDetails AS TBAD
								ON ( TBAV.PimProductId = TBAD.PimProductId  OR TBAD.IsLinkAttribute = 0 ) AND TBAV.AttributeCode = TBAD.AttributeCode 
								INNER JOIN  @TBL_PimAttributeIds AS TBA	ON TBA.AttributeCode = TBAV.AttributeCode
						   WHERE TBAV.PimProductId = TBAVI.PimProductId AND 
								 TBA.AttributeCode = TBAVI.AttributeCode
						   FOR XML PATH('AttributeEntity')
					   )
					   FROM @TBL_AttributeValue AS TBAVI
							INNER JOIN
							@TBL_PublishProductIds AS TBADF
							ON TBADF.PimProductId = TBAVI.PimProductId;
					
				-- FOR XML PATH('Attributes') ) AttributeValue 

				UPDATE TBI
				  SET TBI.AttributeValue = REPLACE(TBI.AttributeValue, '</AttributeEntity>', ISNULL(TBC.AttributeValue, '')+'</AttributeEntity>')
				FROM @TBL_ProductAttributeXMLInner TBI
					 INNER JOIN
					 @TBL_CollectPublishedDefaultXML TBC
					 ON( TBI.PimProductid = TBC.PimProductId AND 
						 TBI.PimAttributeId = TBC.pimAttributeId
					   );
				UPDATE TBI
				  SET TBI.AttributeValue = REPLACE(TBI.AttributeValue, '</AttributeEntity>', ISNULL(TBC.AttributeValue, '')+'</AttributeEntity>')
				FROM @TBL_ProductAttributeXMLInner TBI
					 INNER JOIN
					 @TBL_CollectPublishedMediaXML TBC
					 ON( TBI.PimProductid  = TBC.PimProductId AND 
						 TBI.PimAttributeId = TBC.pimAttributeId
					   );
				UPDATE @TBL_ProductAttributeXMLInner
				  SET AttributeValue = '<Attributes>'+AttributeValue+'</Attributes>';

				INSERT INTO @TBL_ProductAttributeXml( PublishProductId, ProductXml, LocaleId )
					   SELECT PublishProductId, CAST('<ProductEntity>'+TBPDL.AttributeValue+STUFF(
																								 (
																									 SELECT AttributeValue
																									 FROM @TBL_ProductAttributeXMLInner AS TBPAX
																									 WHERE TBPAX.PublishProductId = TBPDL.PublishProductId
																									 FOR XML PATH(''), TYPE
																								 ).value( '.', ' Nvarchar(max)' ), 1, 0, '')+'</ProductEntity>' AS xml), @LocaleId
					   FROM @TBL_ProductDetailsXML AS TBPDL; 
			
				-- create the attribute xml of product 
				
				--DELETE FROM @Cte_AttributeDefault_1
				DELETE FROM @Cte_Attributemedia
				DELETE FROM @Cte_MediaAttributeValueUpdate
				DELETE FROM @Cte_UpdateDefaultValue
				DELETE FROM @Cte_AttributeDefault_0
				Delete from @TBL_AttributeValue;
				DELETE FROM @TBL_PimAttributeIds;
				DELETE FROM @TBL_CollectPublishedDefaultXML;
				DELETE FROM @TBL_CollectPublishedMediaXML;
				DELETE FROM @TBL_CustomeFiled;
				DELETE FROM @TBL_ProductAttributeXMLInner;
				DELETE FROM @TBL_ProductDetailsXML;
				SET @Counter = @Counter + 1; -- increment the counter 
			END; -- END Active Locale Loop 
			-- SELECT * FROM ZnodePublishCatalogLog  
		--	SELECT * FROM @TBL_ProductAttributeXMLInner
				--SELECT ProductXml
				--FROM @TBL_ProductAttributeXml;


			-- IF @IsDebug =1 
			--BEGIN 
			--SELECT * from @TBL_PimAttributeValueId 
			--END;	
			if @ProductCounter = 0
				DELETE FROM ZnodePublishedXml WHERE PublishCatalogLogId = @VersionId ;
			
			INSERT INTO ZnodePublishedXml( PublishCatalogLogId, PublishedId, PublishedXML, IsProductXML, LocaleId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
				   SELECT @VersionId, PublishProductId, ProductXml, 1, LocaleId, @UserId, @GetDate, @UserId, @GetDate
				   FROM @TBL_ProductAttributeXml
				   WHERE @VersionId <> 0;
			 
			-- return the XML 
			IF @PublishCatalogId IS NULL
			BEGIN
				SELECT ProductXml
				FROM @TBL_ProductAttributeXml;
				SELECT PublishProductId AS ProductXml
				FROM @TBL_PublishProductIds
				GROUP BY PublishProductId;
			END;
			SET @Counter = 1;
			DELETE FROM @TBL_ProductAttributeXml;
			DELETE FROM @TBL_PimProductIdToCalculate;
			SET @ProductCounter = @ProductCounter + 100;
			COMMIT TRAN GetPublishProducts
		

		END;
		-------------------END
		

		UPDATE ZnodePublishCatalogLog
		  SET PublishProductId = SUBSTRING(
										  (
											  SELECT ','+CAST(PublishedId AS Varchar(50))
											  FROM ZnodePublishedXml
											  WHERE PublishCatalogLogId = @VersionId AND 
													IsProductXML = 1
											  GROUP BY PublishedId
											  FOR XML PATH('')
										  ), 2, 4000), IsProductPublished = 1, Tokem = @TokenId
		WHERE PublishCatalogLogId = @VersionId AND 
			  @VersionId <> 0 AND 
			  @NotReturnXML IS NULL;
		UPDATE ZnodePimProduct
		  SET IsProductPublish = 1
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM ZnodePublishProduct AS ZPP
			WHERE ZPp.PimProductId = ZnodePimProduct.PimProductId
		) AND 
			  @NotReturnXML IS NULL;
		SET @ProductCounter = 0;
		IF @ProductCount < 100
		BEGIN
			SET @ProductCount = 100;
		END; 
		--SELECT *,@MaxRowId,@ProductCount FROM @TBL_PimAttributeValueId  

		WHILE @ProductCounter < @ProductCount
		      AND @NotReturnXML IS NULL
		    BEGIN
		        INSERT INTO @TBL_PimProductIdToCalculate(PimProductId)
		               SELECT PimProductId
		               FROM @TBL_PimAttributeValueId
		               WHERE RowId BETWEEN CASE
		                                       WHEN @ProductCounter = 0
		                                       THEN 1
		                                       ELSE @ProductCounter + 1
		                                   END AND @ProductCounter + 100
		               GROUP BY PimProductId;
		        SET @PimProductids = SUBSTRING(
		                                      (
		                                          SELECT ','+CAST(PimProductId AS Varchar(50))
		                                          FROM @TBL_PimProductIdToCalculate
		                                          FOR XML PATH('')
		                                      ), 2, 4000);
		        --EXEC Znode_GetPublishAssociatedProductsInnerOut
		        --     @PublishCatalogId,
		        --     @PimProductids,
		        --     'BundleProduct',
		        --     @VersionId,
		        --     @UserId;
		        --EXEC Znode_GetPublishAssociatedProductsInnerOut
		        --     @PublishCatalogId,
		        --     @PimProductids,
		        --     'GroupedProduct',
		        --     @VersionId,
		        --     @UserId;
		        --EXEC Znode_GetPublishAssociatedProductsInnerOut
		        --     @PublishCatalogId,
		        --     @PimProductids,
		        --     'ConfigurableProduct',
		        --     @VersionId,
		        --     @UserId;
		        --EXEC Znode_GetPublishAssociatedAddonsInnerOut
		        --     @PublishCatalogId,
		        --     @PimProductids,
		        --     @VersionId,
		        --     @UserId;
		        SET @ProductCounter = @ProductCounter + 100;
				DELETE FROM @TBL_PimProductIdToCalculate;
		END;
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE();
		DECLARE @Status bit;
		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_GetPublishProducts @PublishCatalogId = '+CAST(@PublishCatalogId AS varchar(max))+',@PublishCategoryId='+@PublishCategoryId+',@UserId='+CAST(@UserId AS Varchar(50))+',@NotReturnXML='+CAST(@NotReturnXML AS Varchar(50))+',@UserId = '+CAST(@UserId AS Varchar(50))+',@PimProductId='+CAST(@PimProductId AS Varchar(50))+',@VersionId='+CAST(@VersionId AS Varchar(50))+',@TokenId='+CAST(@TokenId AS varchar(max))+',@Status='+CAST(@Status AS varchar(10));
		SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		ROLLBACK TRAN GetPublishProducts;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_GetPublishProducts', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;