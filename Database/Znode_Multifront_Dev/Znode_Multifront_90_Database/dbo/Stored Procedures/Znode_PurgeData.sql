CREATE PROCEDURE [dbo].[Znode_PurgeData] 
(
 
  @DeleteAllProduct    							BIT = 0 -- This flag 1 will delete all product except ids in @ExceptProductId  table 
 ,@DeleteAllCategory							BIT = 0 -- This flag 1 will delete all category except ids in @ExceptCategoryId  table 
 ,@DeleteAllCatalog								BIT = 0 -- This flag 1 will delete all catalog except ids in @ExceptCatalogId  table 
 ,@DeleteAllSaveCart							Bit = 0	-- This flag 1 will delete all save carts of users.  
 ,@DeleteAllOrder								BIT = 0	-- This flag 1 will delete all orders. 
 ,@DeleteAllAccount								BIT = 0 -- This flag 1 will delete all Account. 
 ,@DeleteAllUser								BIT = 0 -- This flag 1 will delete all user. 
 ,@DeleteAllStore 								BIT = 0 -- This flag 1 will delete all store. 
 ,@DeleteAllGlobalAttribute  					BIT = 0 -- This flag 1 will delete all Global Attribute. 
 ,@DeleteAllProductCategoryAttribute			BIT = 0 -- This flag 1 will delete all Pim Attribute. 
 ,@DeleteAllMedia								BIT = 0 -- This flag 1 will delete all media. 
 ,@DeleteAllWarehouse							BIT = 0 -- This flag 1 will delete all warhouse. 
 ,@DeleteAllPricelist							BIT = 0 -- This flag 1 will delete all price list. 
 ,@DeleteAllProfile								BIT = 0 -- This flag 1 will delete all Profiles. 
 ,@DeleteAllSiteSearchData						BIT = 0	-- This flag 1 will delete all data related to search. 
 ,@DeleteAllCMSData								BIT = 0 -- This flag 1 will delete all CMS data. 
 ,@DeleteAllBrand								BIT = 0 -- This flag 1 will delete all brand. 
 ,@DeleteAllVendor								BIT = 0 -- This flag 1 will delete all vendor. 
 ,@DeleteAllCmsSeoDetails						BIT = 0 -- This flag 1 will delete all Seo details .   
 ,@ResetDomainData								BIT = 0 -- This flag 1 will delete and rest all domain. 
 ,@ExceptProductId								TransferId Readonly
 ,@ExceptCategoryId								TransferId Readonly
 ,@ExceptCatalogId								TransferId Readonly
 ,@ExceptAccountId								TransferId Readonly
 ,@ExceptUserId 								TransferId Readonly
 ,@ExceptStoreId 								TransferId Readonly 
 ,@ExceptGlobalAttributeId 						TransferId Readonly
 ,@ExceptProductCategoryAttributeId 		    TransferId Readonly
 ,@ExceptMediaId								TransferId Readonly
 ,@ExceptWarehouseId							TransferId ReadOnly 
 ,@ExceptPricelistId							TransferId ReadOnly 
 ,@ExceptProfileId								TransferId ReadOnly
 ,@ExceptSeoType								VARCHAR(2000) = ''
 ,@ResetIdentity								BIT = 0    -- Reset identity 
 ,@DeleteAllData								BIT = 0 	
 ,@DeleteAllShippingMethods						BIT = 0 
 ,@DeleteAllPaymentMethods						BIT = 0 
 ,@DeleteAllTaxes								BIT = 0 
 ,@DeleteAllPublishedData BIT = 0
)
AS 
BEGIN 
SET NOCOUNT ON 
 	 BEGIN TRY
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
	    DECLARE @StatusOut Table (Id INT ,Message NVARCHAR(max), Status BIT )
		DECLARE @DeletedIds TransferId 
		DECLARE @PortalId INT , @CMSThemeId INT , @CMSThemeCSSId INT,@PublishCatalogId INT
		,@PimCatalogId INT  

		----Delete all published data
		IF @DeleteAllPublishedData = 1 OR @DeleteAllData =1 
		BEGIN	

			EXECUTE [Znode_DeleteAllPublishedData] @UserId = 2

		END

		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF  Object_Id('elmah_error')	 <> 0 
		BEGIN 
			 TRUNCATE TABLE elmah_error
			 DELETE FROM ZnodeImportLog
			 DELETE FROM ZnodeImportProcesslog
			 DELETE FROM ZnodeActivityLog	
             DELETE FROM ZnodePasswordLog	
             DELETE FROM ZnodeProceduresErrorLog
		END 
	
		DECLARE @DeleteId  NVARCHAR(max)= '', @StoreData NVARCHAR(max),@RunTime INT =1 
		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF  @DeleteAllVendor = 1   OR @DeleteAllData =1 
			BEGIN 
			 	INSERT INTO @DeletedIds 
				SELECT PimVendorId 
				FROM ZnodePimVendor ZP 
							
				INSERT INTO @StatusOut(id ,Status) 
				EXEC [dbo].[Znode_DeleteVendor] @PimVendorIds = @DeletedIds ,@Status = 0  
			    
				DELETE FROM ZnodePimProductAttributeDefaultValue WHERE PimAttributeValueId IN (
				SELECT PimAttributeValueId  
				FROM ZnodePimAttributeValue WHERE PimAttributeId = (SELECT PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode= 'Vendor') )
				
				DELETE FROM ZnodePimAttributeValue WHERE PimAttributeId = (SELECT PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode= 'Vendor') 

				DELETE FROM ZnodePimAttributeDefaultValueLocale  WHERE PimAttributeDefaultValueId IN (
				SELECT PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue WHERE AttributeDefaultValueCode IN (SELECT VendorCode FROM ZnodePimVendor ))
				
				DELETE FROM  ZnodePimAttributeDefaultValue WHERE AttributeDefaultValueCode IN (SELECT VendorCode FROM ZnodePimVendor )

			    PRINT '<-- Vendor Data Deleted Sucessfully-->'
				
			END
			DELETE FROM @DeletedIds DELETE FROM @StatusOut 
			IF  @DeleteAllBrand = 1  OR @DeleteAllData =1 
			BEGIN 
			   INSERT INTO @DeletedIds 
			   SELECT BrandId
			   FROM ZnodeBrandDetails a
			   INSERT INTO @StatusOut (Id ,Status) 
			   EXEC [dbo].[Znode_DeleteBrand] @BrandIds = @DeletedIds, @Status = 0   
			 IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Brand Data Deleted Sucessfully-->'
			   END
			   ELSE 
				BEGIN 
				PRINT '<-- Brand Data Not Deleted Properly -->' 
				END  
		    END 
				
		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF  @DeleteAllProduct = 1  OR @DeleteAllData =1 
		BEGIN 
		   	 INSERT INTO @DeletedIds 
		     SELECT PimProductId 
			 FROM ZnodePimProduct ZPP 
			 WHERE NOT EXISTS (SELECT TOP 1 1 FROM @ExceptProductId WHERE id = ZPP.PimProductId) 
			  INSERT INTO @StatusOut (Id ,Status) 
			  EXEC [dbo].[Znode_DeletePimProducts] @PimProductIds=@DeletedIds , @Status = 0   
			
			SELECT PimAddonGroupId,PimProductId,PimAddOnProductId  
			INTO #Temp_Addon 
			FROM ZnodePimAddOnProduct ZPP
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM @ExceptProductId WHERE id = ZPP.PimProductId)

			DELETE FROM ZnodePimAddOnProductDetail WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Addon t 
							 WHERE t.PimAddOnProductId  = ZnodePimAddOnProductDetail.PimAddOnProductId )
			
			DELETE FROM  ZnodePimAddOnProduct  WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Addon t 
							 WHERE t.PimAddOnProductId  = ZnodePimAddOnProduct.PimAddOnProductId )
			
			DELETE FROM ZnodePimAddonGroupLocale WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Addon t 
							 WHERE t.PimAddonGroupId  = ZnodePimAddonGroupLocale.PimAddonGroupId )
			DELETE FROM ZnodePimAddonGroupProduct 	WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Addon t 
							 WHERE t.PimAddonGroupId  = ZnodePimAddonGroupProduct.PimAddonGroupId )
			DELETE FROM ZnodePimAddonGroup 	WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Temp_Addon t 
							 WHERE t.PimAddonGroupId  = ZnodePimAddonGroup.PimAddonGroupId )
			IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Product Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Product Data Not Deleted Properly -->' 
				END  
			  
	    END 
		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF  @DeleteAllCategory = 1 	  OR @DeleteAllData =1 
		BEGIN   
		   		INSERT INTO @DeletedIds 
				SELECT  PimCategoryId 
				FROM ZnodePimCategory ZPC
				WHERE NOT EXISTS  (SELECT TOP 1 1 FROM @ExceptCategoryId WHERE id =ZPC.PimCategoryId )
				--Remove extra products from catalog
				--INSERT INTO @StatusOut (Id ,Status) 

				EXEC Znode_DeletePimCategory @PimCategoryId = @DeletedIds, @Status = 1;
			
				IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Category Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Category Data Not Deleted Properly -->' 
				END     
		  END
		  DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		  IF  @DeleteAllCatalog = 1 	OR @DeleteAllData =1 
		   BEGIN
		   	 INSERT INTO @DeletedIds 
		   	 SELECT PimCatalogId 
			 FROM ZnodePimCatalog ZP 
			 WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptCatalogId WHERE id = ZP.PimCatalogId)
		   	 INSERT INTO @StatusOut (Id ,Message,Status) 
			 EXEC [dbo].[Znode_DeletePimCatalog] @PimCatalogId = @DeletedIds ,@IsForceFullyDelete = 1  
		      

			 IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Catalog Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Catalog Data Not Deleted Properly -->' 
				END  
           END
		   DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		   IF  @DeleteAllOrder = 1 	OR @DeleteAllData =1 
		   BEGIN

				--Delete return data against order
				DELETE FROM ZnodeRmaRequestItem
				DELETE FROM ZnodeRmaRequest
				DELETE FROM ZnodeRmaReturnNotes
				DELETE FROM ZnodeRmaReturnEmailHistory
				DELETE FROM ZnodeRmaReturnHistory
				DELETE FROM ZnodeRmaReturnPaymentRefund
				DELETE FROM ZnodeRmaReturnPaymentDetails
				DELETE FROM ZnodeRmaReturnLineItems
				DELETE FROM ZnodeRmaReturnDetails

				 INSERT INTO @DeletedIds 
		   		 SELECT OmsOrderID 
				 FROM ZnodeOmsOrder  ZP 
				 INSERT INTO @StatusOut (Id ,Status) 
			     EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIds , @status = 0  
		    
		        IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Order Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Order Data Not Deleted Properly -->' 
				END  
		  END
		
		   DELETE FROM @DeletedIds DELETE FROM @StatusOut
		   IF  @DeleteAllSaveCart = 1  OR @DeleteAllData =1 
		   BEGIN
		      DELETE FROM ZnodeOmsPersonalizeCartItem
			  DELETE FROM ZnodeOmsSavedCartLineItem 
			  DELETE FROM ZnodeOmsSavedCart
			  DELETE FROM ZnodeOmsCookieMapping
			  DELETE FROM ZnodeOmsQuotePersonalizeItem 
			  DELETE FROM ZnodeOmsQuoteLineItem
			  DELETE FROM ZnodeOmsQuote
			  DELETE FROM ZnodeOmsTemplateLineItem 
			  DELETE FROM ZnodeOmsTemplate
			 
		      PRINT '<-- Save Cart & Quote Data Deleted Sucessfully -->'
			  
		   END
		   DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		   IF  @DeleteAllUser = 1 OR @DeleteAllData =1 
		   BEGIN  
		  
			   INSERT INTO @DeletedIds 
			   SELECT UserId
			   FROM ZnodeUser ZU 
			   WHERE NOT EXISTS (SELECT TOP 1  1 FROM @ExceptUserId RT WHERE RT.Id = ZU.UserId) 
			 --  INSERT INTO @StatusOut (Id ,Status) 
			   EXEC Znode_DeleteUserDetails @UserIds =@DeletedIds ,@Status = 0 , @IsForceFullyDelete =1 

			DELETE FROM AspNetUsers  WHERE NOT EXISTS (SELECT TOP 1 1 FROM AspNetZnodeUser rt WHERE rt.AspNetZnodeUserId = AspNetUsers.UserName )

		     	PRINT '<-- User Data Deleted Sucessfully-->'
			  
		   END
	    DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF  @DeleteAllAccount = 1 	OR @DeleteAllData =1 
		  BEGIN    
				 INSERT INTO @DeletedIds 
				  SELECT AccountId
			      FROM ZnodeAccount ZU 
			      WHERE NOT EXISTS (SELECT TOP 1  1 FROM @ExceptAccountId RT WHERE RT.Id = ZU.AccountiD) 
			    -- INSERT INTO @StatusOut (Id ,Status) 
				  EXEC Znode_DeleteAccount @AccountIds =  @DeletedIds,@Status= 0,@IsForceFullyDelete =1  
				 
		     	PRINT '<-- Accouts Data Deleted Sucessfully-->'
			    
		  END
		 DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		 IF  @DeleteAllGlobalAttribute = 1 	 OR @DeleteAllData =1 
		   BEGIN 
		   	INSERT INTO @DeletedIds 
			SELECT GlobalAttributeId 
			FROM ZnodeGlobalAttribute ZP 
			WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptGlobalAttributeId a WHERE a.Id = ZP.GlobalAttributeId)
			AND ISNULL(ZP.IsSystemDefined,0) <> 1
			--INSERT INTO @StatusOut (Id ,Status) 	   
			EXEC [dbo].[Znode_DeleteGlobalAttribute] @GlobalAttributeIds= @DeletedIds,@Status =0 , 	@IsForceFullyDelete= 1    
			
			DELETE FROM ZnodeGlobalFamilyGroupMapper WHERE GlobalAttributeGroupId IN  (SELECT GlobalAttributeGroupId  FROM ZnodeGlobalAttributeGroup WHERE IsSystemDefined <> 1 )
			DELETE FROM ZnodeGlobalAttributeGroupLocale	WHERE GlobalAttributeGroupId IN  (SELECT GlobalAttributeGroupId  FROM ZnodeGlobalAttributeGroup WHERE IsSystemDefined <> 1 )
			DELETE FROM ZnodeGlobalAttributeGroupMapper WHERE GlobalAttributeGroupId IN  (SELECT GlobalAttributeGroupId  FROM ZnodeGlobalAttributeGroup WHERE IsSystemDefined <> 1)
			DELETE FROM ZnodeGlobalGroupEntityMapper WHERE GlobalAttributeGroupId IN  (SELECT GlobalAttributeGroupId  FROM ZnodeGlobalAttributeGroup WHERE IsSystemDefined <> 1)
			DELETE FROM ZnodeFormBuilderAttributeMapper	WHERE GlobalAttributeGroupId IN  (SELECT GlobalAttributeGroupId  FROM ZnodeGlobalAttributeGroup WHERE IsSystemDefined <> 1)
			DELETE FROM ZnodeGlobalAttributeGroup	WHERE GlobalAttributeGroupId IN  (SELECT GlobalAttributeGroupId  FROM ZnodeGlobalAttributeGroup WHERE IsSystemDefined <> 1)  

			   IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Global Attribute Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Global Attribute Not Deleted Properly -->' 
				END 	   
		   END  
		   DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		   IF  @DeleteAllProductCategoryAttribute = 1  OR @DeleteAllData =1 
		   BEGIN 
			   	INSERT INTO @DeletedIds 
				SELECT PimAttributeId
				FROM ZnodePimAttribute ZP 
				WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptProductCategoryAttributeId WHERE id = ZP.PimAttributeId )
				AND ZP.IsSystemDefined <> 1 
				INSERT INTO @StatusOut (Id ,Status) 			
				EXEC Znode_DeletePimAttributeWithReference @PimAttributeIds = @DeletedIds  , @Status = 1  
			  
			    DELETE FROM ZnodePimAttributeGroupLocale 
					WHERE PimAttributeGroupId IN (SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE IsSystemDefined <> 1  )
				DELETE FROM ZnodePimAttributeGroupMapper wHERE PimAttributeGroupId IN (SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE IsSystemDefined <> 1  )
				DELETE FROM ZnodePimFamilyGroupMapper 
					WHERE PimAttributeGroupId IN (SELECT PimAttributeGroupId FROM ZnodePimAttributeGroup WHERE IsSystemDefined <> 1  )
				DELETE FROM ZnodePimAttributeGroup WHERE IsSystemDefined <> 1  
			
				DELETE FROM ZnodePimFamilyLocale WHERE  PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )
				UPDATE ZP SET PimAttributeFamilyId = dbo.Fn_GetDefaultPimProductFamilyId() FROM ZnodePimProduct  ZP  WHERE  PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )
				UPDATE ZP SET PimAttributeFamilyId = dbo.Fn_GetDefaultPimProductFamilyId() FROM ZnodePimCategory  ZP  WHERE  PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )
				UPDATE ZP SET PimAttributeFamilyId = dbo.Fn_GetDefaultPimProductFamilyId() FROM ZnodePimCategoryAttributeValue  ZP  WHERE  PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )
				UPDATE ZP SET PimAttributeFamilyId = dbo.Fn_GetDefaultPimProductFamilyId() FROM ZnodePimAttributeValue  ZP  WHERE  PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )

				DELETE FROM ZnodeImportTemplateMapping WHERE ImportTemplateId IN (SELECT ImportTemplateId FROM ZnodeImportTemplate WHERE PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  ))
				DELETE FROM ZnodeImportTemplate WHERE PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )
				DELETE FROM ZnodePimFamilyGroupMapper 
					WHERE PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  )

				DELETE  FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1 
				UPDATE ZP SET PimAttributeFamilyId = dbo.Fn_GetDefaultPimProductFamilyId() FROM ZnodePimAttributeValue  ZP  WHERE  PimAttributeFamilyId IN (SELECT PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsSystemDefined <> 1  ) 
			   	IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- PIM Attribute Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- PIM Attribute Not Deleted Properly -->' 
				END 
		   END
		   DELETE FROM @DeletedIds 
		   DELETE FROM @StatusOut 
		   IF  @DeleteAllMedia = 1  OR @DeleteAllData =1 
		   BEGIN 
		  		INSERT INTO @DeletedIds 
		   		SELECT  MediaId   
				FROM ZnodeMedia ZP 
				WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptMediaId WHERE id = ZP.Mediaid )
				--INSERT INTO @StatusOut (Id ,Message,Status)
				EXEC Znode_DeleteMedia @MediaIds = @DeletedIds  , @Status = 1  ,@IsCallInternal =1 
				DELETE FROM ZnodeMediaCategory WHERE MediaPathId IN (SELECT MediaPathId FROM ZnodeMediaPath WHERE PathCode<>'Root')
				DELETE FROM ZnodeMediaPathLocale WHERE MediaPathId IN (SELECT MediaPathId FROM ZnodeMediaPath WHERE PathCode<>'Root')
				DELETE FROM ZnodeMediaPath WHERE PathCode<>'Root'
				IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Media Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Media Not Deleted Properly -->' 
				END
				
		   END 
		   DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		
		   IF  @DeleteAllWarehouse = 1 OR @DeleteAllData =1 
		   BEGIN 
		   	
		   		SET @DeleteId =  SUBSTRING((
				SELECT  ',' + CONVERT(NVARCHAR(500), WarehouseId)  
				FROM ZnodeWarehouse ZP 
				WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptWarehouseId WHERE id = ZP.WarehouseId )
				FOR XML PATH ('')
				),2,4000) 
				INSERT INTO @StatusOut (Id ,Status)
			    EXEC Znode_DeleteWarehouse @WarehouseId = @DeleteId  , @Status = 1 
		
				IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Warehouse Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Warehouse Not Deleted Properly -->' 
				END
		    END 
			DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		
			IF  @DeleteAllPricelist = 1   OR @DeleteAllData =1 
		    BEGIN 
				SET @DeleteId =  SUBSTRING((
				SELECT ',' + CONVERT(NVARCHAR(500), PriceListId)  
				FROM ZnodePriceList ZP 
				WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptPricelistId WHERE id = ZP.PriceListId )
				FOR XML PATH ('')
				),2,4000) 
			  
				INSERT INTO @StatusOut (Id ,Status)  
				EXEC Znode_DeletePriceList @PriceListId = @DeleteId  , @Status = 1 
		      
			   IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Price List Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Price List Not Deleted Properly -->' 
				END
		    END
		
			DELETE FROM @DeletedIds DELETE FROM @StatusOut 
			IF  @DeleteAllProfile = 1  OR @DeleteAllData =1 
			 BEGIN 
				SET @DeleteId =  SUBSTRING((
				SELECT ',' + CONVERT(NVARCHAR(500), ProfileId)  
				FROM ZnodeProfile ZP 
				WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptProfileId WHERE id = ZP.ProfileId )
				FOR XML PATH ('')
				),2,4000) 
		   	 	--INSERT INTO @StatusOut (Id ,Status)
				EXEC  Znode_DeleteProfile  @ProfileId=@DeleteId, @Status = 0 ,	@IsForceFullyDelete =1 
				
				IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Profile Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Profile Not Deleted Properly -->' 
				END
		    END
			DELETE FROM @DeletedIds DELETE FROM @StatusOut 
			IF  @DeleteAllSiteSearchData  = 1  OR @DeleteAllData =1 
			BEGIN 
			
					DELETE FROM ZnodeSearchIndexServerStatus
					DELETE FROM ZnodeSearchIndexMonitor
					DELETE FROM ZnodeCatalogIndex
					DELETE FROM ZnodePublishCatalogSearchProfile
					DELETE FROM ZnodeCatalogIndex
					DELETE FROM ZnodeCMSCustomerReview 
					DELETE FROM ZnodePublishPortalLog
					DELETE FROM ZnodeListViewFilter
					DELETE FROM ZnodeListView
			
					PRINT '<-- Site Search Data Deleted Sucessfully-->'
			 END
			 DELETE FROM @DeletedIds DELETE FROM @StatusOut 
			 IF  @DeleteAllCMSData = 1 	OR @DeleteAllData =1 
			 BEGIN 
				IF EXISTS (SELECT TOP 1 1  FROM SYS.Tables WHERE name = '_ZnodeCMSPortalTheme' )
				BEGIN
				 DROP TABLE _ZnodeCMSPortalTheme 
				END 
				SELECT * 
				INTO _ZnodeCMSPortalTheme
				FROM ZnodeCMSPortalTheme
				DELETE FROM ZnodeCMSContentPagesProfile 
				DELETE FROM ZnodeFormWidgetEmailConfiguration
				DELETE FROM ZnodeCMSWidgetTitleConfigurationLocale
				DELETE FROM ZnodeCMSWidgetTitleConfiguration
				DELETE FROM ZnodeCMSTextWidgetConfiguration
				DELETE FROM ZnodeCMSFormWidgetConfiguration
				DELETE FROM ZnodeCMSPortalProductPage
				DELETE FROM ZnodeCMSContentPageGroupMapping
				DELETE FROM ZnodeCMSContentPageGroupLocale
				DELETE FROM ZnodeCMSContentPageGroup
				DELETE FROM ZnodeCMSContentPagesLocale
				DELETE FROM ZnodeCMSContentPages
				DELETE FROM ZnodeCMSContentPagesProfile
				DELETE FROM ZnodeCMSPortalTheme  
			    DELETE FROM ZnodeCMSThemeCSS 
				DELETE FROM ZnodeCMSTheme  
				DELETE FROM ZnodeEmailTemplateMapper
				DELETE FROM ZnodeEmailTemplateLocale
				DELETE FROM ZnodeEmailTemplateAreas
				DELETE FROM ZnodeEmailTemplate
			    DELETE FROM ZnodeCMSWidgetSliderBanner
				DELETE FROM ZnodeCMSSliderBannerLocale
				DELETE FROM ZnodeCMSSliderBanner
				DELETE FROM ZnodeCMSSlider
				DELETE FROM ZnodeCmsPortalMessage
				DELETE FROM ZnodeCMSPortalMessageKeyTag
				DELETE FROM ZnodeCMSMessage 
				DELETE FROM ZnodeCMSMessageKey 
				DELETE FROM ZnodeCMSTemplate
				DELETE FROM ZnodeFormBuilderGlobalAttributeValueLocale 
				DELETE FROM ZnodeFormBuilderGlobalAttributeValue
				DELETE FROM ZnodeFormBuilderAttributeMapper 
				DELETE FROM ZnodeFormBuilderSubmit 
				DELETE FROM ZnodeFormBuilder

				IF NOT EXISTS (SELECT TOP 1 1  FROM ZnodeCMSTheme)
				BEGIN 
			   INSERT INTO ZnodeCMSTheme(Name
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate
			,IsParentTheme
			,ParentThemeId)
			SELECT 'Default',2,@GetDate,2,@GetDate,1,NULL
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSTheme WHERE Name = 'Default')

			SET @CMSThemeId = CASE WHEN @CMSThemeId	 IS NULL THEN (SELECT TOP 1 CMSThemeId FROM ZnodeCMSTheme WHERE Name = 'Default'   )  ELSE  @CMSThemeId END 
			INSERT INTO ZnodeCMSThemeCSS  (CMSThemeId
			,CSSName
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate)
			SELECT @CMSThemeId,'DefaultCSS',2,@GetDate,2,@GetDate
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSThemeCSS WHERE CSSName = 'DefaultCSS')
	  	
			SET @CMSThemeCSSId = CASE WHEN @CMSThemeCSSId IS NULL THEN (SELECT TOP 1 CMSThemeCSSId FROM ZnodeCMSThemeCSS 
			WHERE CSSName = 'DefaultCSS'
			) ELSE 	@CMSThemeCSSId END 

			 INSERT INTO ZnodeCMSPortalTheme (PortalId
					,CMSThemeId
					,CMSThemeCSSId
					,MediaId
					,FavIconId
					,WebsiteTitle
					,CreatedBy
					,CreatedDate
					,ModifiedBy
					,ModifiedDate)
			 SELECT DISTINCT PortalId
					,@CMSThemeId
					,@CMSThemeCSSId
					,MediaId
					,FavIconId
					,WebsiteTitle
					,2
					,@GetDate
					,2
					,@GetDate 
			 FROM _ZnodeCMSPortalTheme	TYU 
			 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodeCMSPortalTheme TY  WHERE TY.PortalId = TYU.PortalId AND TY.CMSThemeId = @CMSThemeId AND TY.CMSThemeCSSId = @CMSThemeCSSId)
			 
			 END 

			 PRINT '<-- CMS Data Deleted Sucessfully-->'	  			   
			 IF  NOT EXISTS (SELECT TOP 1 1  FROM ZnodeCMSContentPageGroup )
			 BEGIN 
			    DECLARE @GroupId INT =0 
				INSERT INTO ZnodeCMSContentPageGroup (ParentCMSContentPageGroupId ,Code,CreatedBy,CreatedDate,ModIFiedBy,ModIFiedDate)
				SELECT NULL,'Root',2,@GetDate,2,@GetDate
			    SET @GroupId = SCOPE_IDENTITY()
				INSERT INTO ZnodeCMSContentPageGroupLocale(CMSContentPageGroupId,Name,LocaleId,CreatedBy,CreatedDate,ModIFiedBy,ModIFiedDate)
			    SELECT @GroupId,'Root',1,2,@GetDate,2,@GetDate
			 END
			 END 
	 		 DELETE FROM @DeletedIds DELETE FROM @StatusOut 
			 IF  @DeleteAllCmsSeoDetails =1 OR @DeleteAllData =1 
			 BEGIN 
			 
			   DELETE FROM ZnodeCMSSEODetailLocale 
			   WHERE CMSSEODetailId IN (SELECT CMSSEODetailId FROM ZnodeCMSSEODetail a 
			   INNER JOIN ZnodeCMSSEOType b ON (b.CMSSEOTypeId = a.CMSSEOTypeId)
			   WHERE NOT EXISTS (SELECT TOP 1 1 FROM dbo.split(@ExceptSeoType,',') t WHERE t.Item = b.Name))
			  
			   DELETE FROM ZnodeCMSSEODetail 
			   WHERE CMSSEODetailId IN (SELECT CMSSEODetailId FROM ZnodeCMSSEODetail a 
			   INNER JOIN ZnodeCMSSEOType b ON (b.CMSSEOTypeId = a.CMSSEOTypeId)
			   WHERE NOT EXISTS (SELECT TOP 1 1 FROM dbo.split(@ExceptSeoType,',') t WHERE t.Item = b.Name))
			   PRINT '<-- SEO Data Deleted Sucessfully-->'
			END 
			
		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF  @DeleteAllStore = 1   OR @DeleteAllData =1 
		   BEGIN 
		   DECLARE @TBL_PortalIds TABLE	 ( PortalId int	);
		   DECLARE @TBL_Promotion TABLE ( PromotionId int	);
		  	IF NOT EXISTS (SELECT TOP 1 1 FROM sys.tables WHERE Name = '_ZnodeDomain')
			BEGIN
				   CREATE TABLE  _ZnodeDomain (PortalId INT  ,DomainName NVARCHAR(max),IsActive BIT ,ApplicationType NVARCHAR(max),CreatedBy INT
		   ,CreatedDate DATETIME ,ModifiedBy INT ,ModIFiedDate DATETIME )
			END 


		   INSERT INTO _ZnodeDomain ( PortalId,DomainName ,IsActive  ,ApplicationType ,CreatedBy,CreatedDate ,ModIFiedBy ,ModIFiedDate) 
		   SELECT PortalId,DomainName ,IsActive  ,ApplicationType ,CreatedBy,CreatedDate ,ModIFiedBy ,ModIFiedDate
		   FROM ZnodeDomain 
		   DECLARE @TBL_DeletedUsers TABLE (AspNetUserId NVARCHAR(1000))

		       SET @DeleteId = Substring((select  ',' + convert(nvarchar(500), PromotionId)  
					FROM ZnodePromotion  SP
					WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptStoreId WHERE id = SP.PortalId )
					for XML Path ('')),2,4000) 
			   
			   

		-- inserting PortalIds which are not present in Order and Quote
		   INSERT INTO @TBL_PortalIds 
		   SELECT PortalId FROM ZnodePortal AS SP
		   WHERE NOT EXISTS (SELECT TOP 1 1  FROM @ExceptStoreId WHERE id = SP.PortalId );

		  	INSERT INTO @StatusOut (Id ,Status)
				EXEC Znode_DeletePromotion  @PromotionId = @DeleteId ,@Status = 1;

		
				IF  EXISTS (SELECT TOP 1 1  FROM @StatusOut WHERE Status = 1 )
				BEGIN 
		     	PRINT '<-- Store Promotion Data Deleted Sucessfully-->'
			    END
			    ELSE 
				BEGIN 
				PRINT '<-- Store Promotion Data Not Deleted Properly -->' 
				END	

-------------------------------------------------

			   DELETE FROM ZnodeGiftCardHistory	
                 WHERE EXISTS( SELECT *  FROM ZnodeGiftCard B	
                               WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS a	
                                                              WHERE a.PortalId = B.PortalId	) AND ZnodeGiftCardHistory.GiftCardId = B.GiftCardId );
	
	
                    DELETE FROM ZnodeRmaRequestItem	
                    WHERE EXISTS( SELECT *  FROM ZnodeGiftCard B WHERE EXISTS ( SELECT TOP 1 1	FROM @TBL_PortalIds AS a  WHERE a.PortalId = B.PortalId	
                              ) AND ZnodeRmaRequestItem.GiftCardId = B.GiftCardId );
	
	
                DELETE FROM ZnodeGiftCardLocale
                    WHERE EXISTS( SELECT *  FROM ZnodeGiftCard B	
                                            WHERE EXISTS ( SELECT TOP 1 1	FROM @TBL_PortalIds AS a	
                                             WHERE a.PortalId = B.PortalId	  ) AND ZnodeGiftCardLocale.GiftCardId = B.GiftCardId );
	
                    	
                    DELETE FROM ZnodeGiftCard	
                    WHERE EXISTS ( SELECT TOP 1 1	
                        FROM @TBL_PortalIds AS a	
                        WHERE a.PortalId = ZnodeGiftCard.PortalId	 );
		
                    DELETE FROM ZnodePortalLoginProvider	
                    WHERE EXISTS ( SELECT TOP 1 1	  FROM @TBL_PortalIds AS a	 WHERE a.PortalId = ZnodePortalLoginProvider.PortalId  );

--------------------------------------------------------------------------------------------------------

		 DELETE FROM  ZnodeBrandPortal  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeBrandPortal.PortalId);
	    DELETE FROM  ZnodeCustomPortalDetail  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCustomPortalDetail.PortalId);
		
		 DELETE FROM  ZnodeSupplier WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeSupplier.PortalId)

	     DELETE FROM  ZnodeOmsTemplateLineItem  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP INNER JOIN ZnodeOmsTemplate ZOT ON 
	     TBP.PortalId = ZOT.PortalId AND ZOT.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId);

	     DELETE FROM ZnodeOmsTemplate WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsTemplate.PortalId);
	     DELETE FROM  ZnodeOmsUsersReferralUrl WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsUsersReferralUrl.PortalId)

		DELETE FROM ZnodePortalShipping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalShipping.PortalId);
		DELETE FROM ZnodePortalTaxClass WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalTaxClass.PortalId);
		DELETE FROM ZnodePortalPaymentSetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPaymentSetting.PortalId);
		DELETE FROM ZnodeCMSPortalMessageKeyTag WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalMessageKeyTag.PortalId);
		DELETE FROM ZnodePriceListProfile WHERE PortalProfileID IN (SELECT PortalProfileID FROM ZnodePortalProfile WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalProfile.PortalId))
		DELETE FROM ZnodePortalProfile WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalProfile.PortalId);
		DELETE FROM ZnodePortalFeatureMapper WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalFeatureMapper.PortalId);
		DELETE FROM ZnodePortalShippingDetails WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalShippingDetails.PortalId);
		DELETE FROM ZnodePortalUnit WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalUnit.PortalId);
		DELETE FROM ZnodeDomain WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeDomain.PortalId);
		
		DELETE FROM ZnodePortalSearchProfile   WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalSearchProfile.PortalId);
		DELETE FROM dbo.ZnodePortalPixelTracking WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPixelTracking.PortalId); 
		DELETE FROM ZnodeRobotsTxt WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeRobotsTxt.PortalId);
		DELETE FROM ZnodePortalSmtpSetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalSmtpSetting.PortalId);
		DELETE FROM ZnodeActivityLog WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeActivityLog.PortalId);
		DELETE FROM ZnodePortalCatalog WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalCatalog.PortalId );
		DELETE FROM ZnodeCMSPortalMessage  WHERE EXISTS  ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalMessage.PortalId );
		DELETE FROM ZnodeGoogleTagManager WHERE  EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeGoogleTagManager.PortalId);
		DELETE FROM ZnodeTaxRuleTypes WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeTaxRuleTypes.PortalId);
		DELETE FROM ZnodeCMSContentPagesProfile WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
											WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeCMSContentPagesProfile.CMSContentPagesId )
		DELETE FROM ZnodeCMSContentPageGroupMapping WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeCMSContentPageGroupMapping.CMSContentPagesId )
	     DELETE FROM ZnodeCMSContentPagesLocale WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeCMSContentPagesLocale.CMSContentPagesId )
		
		DELETE FROM ZnodeBlogNewsCommentLocale WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeBlogNewsComment ZBC
													WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeBlogNews ZBN
														WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZBC.BlogNewsId ) and ZBC.BlogNewsCommentId = ZnodeBlogNewsCommentLocale.BlogNewsCommentId)
		DELETE FROM ZnodeBlogNewsComment WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeBlogNews ZBN
													WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZnodeBlogNewsComment.BlogNewsId )
		 
		DELETE FROM ZnodeBlogNewsContent WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeBlogNews ZBN
													WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZnodeBlogNewsContent.BlogNewsId )
		DELETE FROM ZnodeBlogNewsLocale WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeBlogNews ZBN
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZnodeBlogNewsLocale.BlogNewsId )
													
		DELETE FROM ZnodeBlogNews WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeBlogNews.PortalId)
		DELETE FROM  ZnodeFormWidgetEmailConfiguration 	WHERE CMSContentPagesId IN (SELECT CMSContentPagesId FROM ZnodeCMSContentPages WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSContentPages.PortalId))
		DELETE FROM ZnodeCMSContentPages WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSContentPages.PortalId);
		DELETE FROM ZnodeCaseRequestHistory WHERE CaseRequestId IN (SELECT CaseRequestId   FROM ZnodeCaseRequest WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCaseRequest.PortalId))
		DELETE FROM ZnodeCaseRequest WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCaseRequest.PortalId);
		DELETE FROM ZnodePortalLocale WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalLocale.PortalId);
		DELETE FROM ZnodeShippingPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeShippingPortal.PortalId);
		
		--Delete return data against order
		DELETE FROM ZnodeRmaRequestItem
		DELETE FROM ZnodeRmaRequest
		DELETE FROM ZnodeRmaReturnNotes
		DELETE FROM ZnodeRmaReturnEmailHistory
		DELETE FROM ZnodeRmaReturnHistory
		DELETE FROM ZnodeRmaReturnPaymentRefund
		DELETE FROM ZnodeRmaReturnPaymentDetails
		DELETE FROM ZnodeRmaReturnLineItems
		DELETE FROM ZnodeRmaReturnDetails
		--Delete orders against store
		DELETE FROM   @DeletedIds
		INSERT INTO @DeletedIds 
		SELECT distinct OmsOrderID 
		FROM ZnodeOmsOrderDetails  ZP 
		WHERE NOT EXISTS(SELECT * FROM @ExceptStoreId A WHERE ZP.PortalId = A.Id)
		--INSERT INTO @StatusOut (Id ,Status) 
		EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIds , @status = 0  

		DELETE FROM   @DeletedIds
		INSERT INTO @DeletedIds 
		SELECT DISTINCT UserId 
		FROM ZnodeUserPortal 
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeUserPortal.PortalId);

		--INSERT INTO @StatusOut (Id ,Status) 
		EXEC [dbo].Znode_DeleteUserDetails @UserIds = @DeletedIds , @status = 0,@IsForceFullyDelete =1 ,@IsCallInternal=1 
		
		DELETE FROM AspNetZnodeUser OUTPUT DELETED.AspNetZnodeUserId   INTO @TBL_DeletedUsers WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = AspNetZnodeUser.PortalId )
		
		DELETE FROM ZnodePortalAlternateWarehouse WHERE EXISTS ( SELECT TOP 1 1 FROM ZnodePortalWareHouse AS ZPWH WHERE EXISTS (
				SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPWH.PortalId ) AND  ZPWH.PortalWarehouseId = ZnodePortalAlternateWarehouse.PortalWarehouseId);
		DELETE FROM ZnodePortalWareHouse WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalWareHouse.PortalId);
		DELETE ZnodePriceListPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePriceListPortal.PortalId );
		
		DELETE FROM ZnodeEmailTemplateMapper WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeEmailTemplateMapper.PortalId);
		DELETE FROM ZnodeGIFtCard WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeGIFtCard.PortalId );
		DELETE FROM ZnodeCMSPortalProductPage WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalProductPage.PortalId);

		DELETE FROM ZnodeCMSPortalSEOSetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalSEOSetting.PortalId);

		DELETE FROM ZnodeCMSPortalTheme WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalTheme.PortalId);

		DELETE FROM ZnodeCMSSEODetailLocale WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP INNER JOIN ZnodeCMSSEODetail AS zcsd ON TBP.PortalId = zcsd.PortalId WHERE zcsd.CMSSEODetailId = ZnodeCMSSEODetailLocale.CMSSEODetailId);
		 
		DELETE FROM ZnodeCMSSEODetail WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSSEODetail.PortalId);
		DELETE FROM ZnodePortalAccount WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalAccount.PortalId);
		DELETE FROM ZnodePortalAddress WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalAddress.PortalId);
		DELETE FROM ZnodePortalCountry WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalCountry.PortalId);
		DELETE FROM ZnodeCMSUrlRedirect WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSUrlRedirect.PortalId);
		DELETE FROM ZnodeTaxPortaL  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeTaxPortaL.PortalId);
	   	INSERT INTO @TBL_Promotion( PromotionId ) SELECT PromotionId FROM ZnodePromotion WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePromotion.PortalId);
		DELETE FROM ZnodePromotionProduct WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionProduct.PromotionId);
		DELETE FROM dbo.ZnodePromotionCoupon  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionCoupon.PromotionId);
		DELETE FROM ZnodePromotionCategory WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionCategory.PromotionId);
		DELETE FROM ZnodePromotionCatalogs WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionCatalogs.PromotionId);
		DELETE FROM ZnodePromotion WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotion.PromotionId);
		DELETE FROM ZnodeBlogNewsLocale WHERE exists (select top 1 1 from ZnodeBlogNews ZBN
													WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZnodeBlogNewsLocale.BlogNewsId )
		
        DELETE a FROM ZnodeFormBuilderGlobalAttributeValueLocale	a 
			INNER JOIN ZnodeFormBuilderGlobalAttributeValue aa ON (a.FormBuilderGlobalAttributeValueId = aa.FormBuilderGlobalAttributeValueId)INNER JOIN ZnodeFormBuilderSubmit b ON (b.FormBuilderSubmitId =aa.FormBuilderSubmitId)
			 WHERE EXISTS ( SELECT TOP 1 1
									   FROM @TBL_PortalIds AS TBDL
									   WHERE TBDL.PortalId = b.PortalId      
									 ); 
		DELETE a FROM ZnodeFormBuilderGlobalAttributeValue a INNER JOIN ZnodeFormBuilderSubmit b ON (b.FormBuilderSubmitId =a.FormBuilderSubmitId)
		 WHERE EXISTS ( SELECT TOP 1 1
								   FROM @TBL_PortalIds AS TBDL
								   WHERE TBDL.PortalId = b.PortalId      
								 ); 
		DELETE FROM ZnodeFormBuilderSubmit 
		WHERE EXISTS ( SELECT TOP 1 1
								   FROM @TBL_PortalIds AS TBDL
								   WHERE TBDL.PortalId = ZnodeFormBuilderSubmit.PortalId      
								 );   
		DELETE FROM   @DeletedIds
		INSERT INTO @DeletedIds 
		SELECT DISTINCT a.OmsOrderId 
		FROM ZnodeOmsOrder A 
		INNER JOIN ZnodeOMsOrderDetails b  ON (b.OmsOrderId = a.OmsOrderId )
		WHERE   EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = b.PortalId)

		INSERT INTO @StatusOut (Id ,Status) 
		EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIds , @status = 0 

		DELETE FROM @DeletedIds DELETE FROM @StatusOut 									  
		
		DELETE FROM dbo.ZnodeSearchSynonyms	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodeSearchSynonyms.PublishCatalogId )
		DELETE FROM ZnodePublishedXml 
		DELETE FROM ZnodePublishCatalogLog	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCatalogLog.PublishCatalogId )
		DELETE FROM ZnodePublishCatalogSearchProfile WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCatalogSearchProfile.PublishCatalogId )
		DELETE FROM ZnodePublishCategoryProduct   WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCategoryProduct.PublishCatalogId )
		DELETE FROM ZnodePublishCategoryDetail 	WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePublishCategoryProduct Tt WHERE Tt.PublishCategoryId = ZnodePublishCategoryDetail.PublishCategoryId )
		DELETE FROM ZnodePublishProductDetail WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePublishCategoryProduct Tt WHERE Tt.PublishCategoryId = ZnodePublishProductDetail.PublishProductId )
		
		DELETE FROM ZnodeCMSWidgetCategory WHERE PublishCategoryId IN (SELECT PublishCategoryId FROM ZnodePublishCategory   WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCategory.PublishCatalogId ))

		DELETE FROM ZnodePublishCategory   WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCategory.PublishCatalogId )
		DELETE FROM dbo.ZnodeCMSWidgetProduct	WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePublishCategoryProduct Tt WHERE Tt.PublishProductId = ZnodeCMSWidgetProduct.PublishProductId )
		DELETE FROM dbo.ZnodeSearchGlobalProductBoost	WHERE PublishProductId IN (SELECT PublishProductId FROM ZnodePublishProduct	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishProduct.PublishCatalogId ))
		DELETE FROM ZnodeCMSCustomerReview 
			WHERE PublishProductId IN (SELECT PublishProductId FROM ZnodePublishProduct	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishProduct.PublishCatalogId ))
		DELETE FROM ZnodePublishProduct	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishProduct.PublishCatalogId )
		DELETE FROM dbo.ZnodeSearchIndexServerStatus WHERE SearchIndexMonitorId IN (SELECT SearchIndexMonitorId FROM dbo.ZnodeSearchIndexMonitor WHERE CatalogIndexId IN (SELECT CatalogIndexId FROM ZnodeCatalogIndex	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodeCatalogIndex.PublishCatalogId )))
		DELETE FROM dbo.ZnodeSearchIndexMonitor WHERE CatalogIndexId IN (SELECT CatalogIndexId FROM ZnodeCatalogIndex	 WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodeCatalogIndex.PublishCatalogId ))
		DELETE FROM  ZnodeCatalogIndex   WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodeCatalogIndex.PublishCatalogId )
		DELETE FROM ZnodeSearchDocumentMapping WHERE PublishCatalogId IN (SELECT PublishCatalogId FROM ZnodePublishCatalog	  WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCatalog.PublishCatalogId ))
		
		DELETE FROM ZnodeSearchKeywordsRedirect 
		WHERE EXISTS(SELECT * FROM ZnodePublishCatalog	WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCatalog.PublishCatalogId ) AND ZnodeSearchKeywordsRedirect.PublishCatalogId = ZnodePublishCatalog.PublishCatalogId)

		DELETE FROM ZnodePublishCatalog	  WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalCatalog Tt WHERE Tt.PublishCatalogId = ZnodePublishCatalog.PublishCatalogId )
		DELETE FROM ZnodeOmsPersonalizeCartItem WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsSavedCartLineItem WHERE OmsSavedCartId IN (SELECT OmsSavedCartId FROM ZnodeOmsSavedCart WHERE OmsCookieMappingId IN (SELECT OmsCookieMappingId FROM ZnodeOmsCookieMapping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsCookieMapping.PortalId) ) 	 )) 
		DELETE FROM ZnodeOmsSavedCartLineItem WHERE OmsSavedCartId IN (SELECT OmsSavedCartId FROM ZnodeOmsSavedCart WHERE OmsCookieMappingId IN (SELECT OmsCookieMappingId FROM ZnodeOmsCookieMapping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsCookieMapping.PortalId) ) 	 )
		DELETE FROM ZnodeOmsSavedCart WHERE OmsCookieMappingId IN (SELECT OmsCookieMappingId FROM ZnodeOmsCookieMapping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsCookieMapping.PortalId) ) 
		DELETE FROM ZnodeOmsCookieMapping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsCookieMapping.PortalId); 
		DELETE FROM ZnodeDomain WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeDomain.PortalId);
		
		DELETE FROM ZnodeSalesRepCustomerUserPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeSalesRepCustomerUserPortal.CustomerPortalId);

		delete from ZnodeSalesRepCustomerUserPortal 
		where exists(select * FROM ZnodeUserPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeUserPortal.PortalId) and ZnodeSalesRepCustomerUserPortal.UserPortalId = ZnodeUserPortal.UserPortalId);
		

		DELETE FROM ZnodePortalRecommendationSetting  
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalRecommendationSetting.PortalId);
		
		DELETE FROM ZnodePortalPageSetting  
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPageSetting.PortalId);

		DELETE FROM ZnodePortalBrand
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalBrand.PortalId);
	
		DELETE FROM ZnodePortalSortSetting
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalSortSetting.PortalId);

		DELETE FROM ZnodeImpersonificationLog
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeImpersonificationLog.PortalId);

		DELETE FROM ZnodePortalCustomCss
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalCustomCss.PortalId);

		DELETE FROM ZnodeSearchActivity
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeSearchActivity.PortalId);
		
		DELETE FROM ZnodeProductFeed  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeProductFeed.PortalId);
		
		DELETE FROM ZnodePortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortal.PortalId);

		PRINT '<-- Store Data Deleted Sucessfully-->'
	   
	   IF  NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortal) 
	   BEGIN 
	        SET @CMSThemeId = NULL 
			SET @CMSThemeCSSId = NULL  
			
	        SET IDENTITY_INSERT [dbo].[ZnodePortal] ON 
			INSERT [dbo].[ZnodePortal] ([PortalId], [CompanyName], [StoreName], [LogoPath], [UseSSL], [AdminEmail], [SalesEmail], [CustomerServiceEmail], [SalesPhoneNumber], [CustomerServicePhoneNumber], [ImageNotAvailablePath], [ShowSwatchInCategory], [ShowAlternateImageInCategory], [ExternalID], [MobileLogoPath], [DefaultOrderStateID], [DefaultReviewStatus], [SplashCategoryID], [SplashImageFile], [MobileTheme], [CopyContentBasedOnPortalId], [CreatedBy], [CreatedDate], [ModIFiedBy], [ModIFiedDate], [InStockMsg], [OutOfStockMsg], [BackOrderMsg], [OrderAmount], [Email], [StoreCode]) 
			VALUES (1, N'DemoStore', N'DemoStore', NULL, 0, N'test@znode.com', N'test@znode.com', N'test@znode.com', N'123456789', N'123456789', N'', 0, 0, NULL, NULL, 50, N'N', NULL, NULL, NULL, NULL, 2, CAST(N'2018-04-23T01:05:48.620' AS DateTime), 2, CAST(N'2018-04-23T01:05:48.620' AS DateTime), N'Demo', N'Demo', N'Demo', NULL, NULL, 'DemoStore')
			SET IDENTITY_INSERT [dbo].[ZnodePortal] OFF
			SET @PortalId  = 1
			INSERT INTO ZnodePimCatalog (CatalogName,IsActive,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT 'DefaultCatalog' , 1 ,NULL,2 ,@GetDate,2,@GetDate 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCatalog r WHERE r.CatalogName = 'DefaultCatalog' )
			SET @PimCatalogId = CASE WHEN @PimCatalogId IS nULL THEN (SELECT TOP 1 PimCatalogId FROM ZnodePimCatalog WHERE	CatalogName = 'DefaultCatalog'  ) ELSE  @PimCatalogId END 	
			INSERT INTO ZnodePublishCatalog (PimCatalogId
			,CatalogName
			,ExternalId
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate
			,Token)
			SELECT @PimCatalogId,'DefaultCatalog' , '',2,@GetDate,2,@GetDate,''
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalog WHERE CatalogName = 'DefaultCatalog')
			SET  @PublishCatalogId = CASE WHEN  @PublishCatalogId IS nULL THEN (SELECT TOP 1 PublishCatalogId  FROM ZnodePublishCatalog WHERE CatalogName = 'DefaultCatalog'   )  ELSE @PublishCatalogId END 
			INSERT INTO ZnodeCMSTheme(Name
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate
			,IsParentTheme
			,ParentThemeId)
			SELECT 'Default',2,@GetDate,2,@GetDate,1,NULL
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSTheme WHERE Name = 'Default')
			SET  @CMSThemeId = CASE WHEN @CMSThemeId IS nULL THEN (SELECT TOP 1 CMSThemeId FROM ZnodeCMSTheme WHERE Name = 'Default'   )  ELSE @CMSThemeId END  
		
			INSERT INTO ZnodeCMSThemeCSS  (CMSThemeId
			,CSSName
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate)
			SELECT @CMSThemeId,'DefaultCSS',2,@GetDate,2,@GetDate
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSThemeCSS WHERE CSSName = 'DefaultCSS')
			SET  @CMSThemeCSSId = CASE WHEN @CMSThemeCSSId IS nULL THEN (SELECT TOP 1 CMSThemeCSSId FROM ZnodeCMSThemeCSS WHERE CSSName = 'DefaultCSS'  )  ELSE @CMSThemeCSSId END  
		
			INSERT INTO ZnodeCMSPortalTheme (PortalId
			,CMSThemeId
			,CMSThemeCSSId
			,MediaId
			,FavIconId
			,WebsiteTitle
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate	)    
			SELECT  @PortalId,@CMSThemeId,@CMSThemeCSSId,NULL,NULL,NULL,2,@GetDate,2,@GetDate
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSPortalTheme WHERE PortalId  = @PortalId  )
			INSERT INTO ZnodePortalCatalog (PortalId
			,PublishCatalogId
			,CreatedBy
			,CreatedDate
			,ModIFiedBy
			,ModIFiedDate)
			SELECT @PortalId,@PublishCatalogId,2,@GetDate,2,@GetDate
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePortalCatalog WHERE PortalId  = @PortalId )
			
			INSERT INTO ZnodeDomain (PortalId,DomainName ,IsActive,ApplicationType,CreatedBy ,CreatedDate ,ModifiedBy ,ModifiedDate)
			SELECT DISTINCT 1,DomainName ,IsActive,ApplicationType,CreatedBy ,@GetDate CreatedDate ,ModifiedBy ,@GetDate ModifiedDate 
			FROM _ZnodeDomain  TR
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeDomain TY WHERE TR.PortalId = TY.PortalId AND TR.DomainName = TY.DomainName) 
			GROUP BY DomainName ,IsActive,ApplicationType,CreatedBy,ModifiedBy

			INSERT INTO ZnodePortalUnit (PortalId,CurrencyId,WeightUnit,DimensionUnit,CurrencySuffix,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT @PortalId,(SELECT TOP 1 CurrencyId FROM ZnodeCulture WHERE CultureCode = 'en-US' ) 
			,'Lbs','IN','USD',2,@GetDate,2,@GetDate
			WHERE NOT EXISTS (SELECT TOP 1 1  FROM ZnodePortalUnit  WHERE PortalId = @PortalId AND CurrencyId = (SELECT TOP 1 CurrencyId FROM ZnodeCulture WHERE CultureCode = 'en-US')  )

	   END 
	   
		
		   END  
			 DELETE FROM @DeletedIds DELETE FROM @StatusOut 
			 IF  @ResetDomainData = 1  OR @DeleteAllData =1     
			 BEGIN 
			  DECLARE @OneportalId INT = (SELECT TOP 1 PortalId  FROM ZnodePortal)
			  DELETE FROM ZnodeDomain 
			  INSERT INTO znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModIFiedBy	,ModIFiedDate)
			  SELECT @OneportalId, 'localhost:6766',1, '115915F1-7E6B-4386-A623-9779F27D9A5E','Admin',2,@GetDate,2,@GetDate
			  WHERE NOT EXISTS(SELECT * FROM znodedomain WHERE DomainName = 'localhost:6766'  and PortalId = (SELECT TOP 1 PortalId FROM ZnodePortal ))
			  
			  INSERT INTO znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModIFiedBy	,ModIFiedDate)
			  SELECT @OneportalId, 'localhost:3288',1, 'c58cc0c0-1349-4001-8416-cf1cea7960e8','WebStore',2,@GetDate,2,@GetDate
			  WHERE NOT EXISTS(SELECT * FROM znodedomain WHERE DomainName = 'localhost:3288'  and PortalId = (SELECT TOP 1 PortalId FROM ZnodePortal ))
			  
			  INSERT INTO znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModIFiedBy	,ModIFiedDate)
			  SELECT @OneportalId, 'localhost:44762',1, '8a8b4931-7d57-42e8-a005-b1c0cce49f1d','Api',2,@GetDate,2,@GetDate
			  WHERE NOT EXISTS(SELECT * FROM znodedomain WHERE DomainName = 'localhost:44762' and PortalId = (SELECT TOP 1 PortalId FROM ZnodePortal ) )
			  
			  INSERT INTO znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModIFiedBy	,ModIFiedDate)
			  SELECT  @OneportalId  , 'localhost',1, '115915F1-7E6B-4386-A623-9779F27D9A5E','Admin',2,@GetDate,2,@GetDate
			  WHERE NOT EXISTS(SELECT * FROM znodedomain WHERE DomainName = 'localhost'  and PortalId = (SELECT TOP 1 PortalId FROM ZnodePortal ))
			 PRINT '<-- Domain reset Sucessfully-->'
			 END 
		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF @DeleteAllShippingMethods = 1 OR @DeleteAllData =1 
		BEGIN 
		
		   	SET @DeleteId = Substring((select  ',' + convert(nvarchar(500), ShippingId)  
					FROM ZnodeShipping for XML Path ('')),2,4000) 
		  -- INSERT INTO @StatusOut (Id ,Status)

		   EXEC Znode_DeleteShipping  @ShippingId = @DeleteId , @Status =0 ,@IsForceFullyDelete =1 
		   
		 PRINT '<-- Shipping Methods are Deleted Sucessfully-->'
			   
		END
		DELETE FROM @DeletedIds DELETE FROM @StatusOut  
		IF @DeleteAllPaymentMethods	 = 1  OR @DeleteAllData =1 
		BEGIN 
			 	 
		INSERT INTO @DeletedIds 
		SELECT DISTINCT a.OmsOrderId 
		FROM ZnodeOmsOrder A 
		INNER JOIN ZnodeOMsOrderDetails b  ON (b.OmsOrderId = a.OmsOrderId )
		WHERE   EXISTS ( SELECT TOP 1 1 FROM ZnodePaymentSetting AS TBP WHERE TBP.PaymentSettingId = b.PaymentSettingId)

		INSERT INTO @StatusOut (Id ,Status) 
		EXEC [dbo].[Znode_DeleteOrderById] @OmsOrderIds = @DeletedIds , @status = 0 
		 
		 DELETE FROM ZnodePortalPaymentSetting 
		 DELETE FROM ZnodeProfilePaymentSetting
		 DELETE FROM ZnodePaymentSetting
		
			  
		 PRINT '<-- Payment Methods are Deleted Sucessfully-->'
			   
		END 
		DELETE FROM @DeletedIds DELETE FROM @StatusOut 
		IF @DeleteAllTaxes	= 1 OR @DeleteAllData =1 		
		BEGIN 
		 
		  	SET @DeleteId = Substring((select  ',' + convert(nvarchar(500), TaxClassId)  
					FROM ZnodeTaxClass for XML Path ('')),2,4000) 
		-- INSERT INTO @StatusOut (Id ,Status) 
		 EXEC [dbo].[Znode_DeleteTaxClass] @TaxClassId =  @DeleteId, @status = 0 , @IsForceFullyDelete =1 
		 DELETE FROM ZnodeTaxRuleTypes   
     	 PRINT '<-- Taxes Data Deleted Sucessfully-->'
		 		 
		END 
		IF  @ResetIdentity =1  OR @DeleteAllData =1 
		 BEGIN
		 DECLARE @table_name varchar(100)= NULL, @showReport bit= 0, @debug bit= 0
	
		IF  OBJECT_ID('tempdb..#reseed_temp1') < 0 
			Drop TABLE #reseed_temp1 
		CREATE TABLE   #reseed_temp1 
		( 
					 tbame varchar(100), mvalue varchar(20) DEFAULT 0
		)

			DECLARE @Tablename varchar(256), @columnname varchar(256), @IndentValue numeric, @query varchar(4000), @query1 nvarchar(4000), @id int;

			DECLARE Cur_Reseed CURSOR LOCAL FAST_FORWARD
			FOR SELECT b.name, c.name
				FROM sys.objects AS a, sys.objects AS b, sys.columns AS c
				WHERE a.type = 'PK' AND 
					  a.parent_object_id = b.object_id AND 
					  b.object_id = c.object_id AND 
					  c.column_id = 1 AND 
					  is_identity <> 0 AND 
					  b.name NOT LIKE '%-%' AND 
					  b.name NOT LIKE '%(%' AND 
					  RTRIM(LTRIM(b.name)) = RTRIM(LTRIM(COALESCE(@table_name, b.name)));

			OPEN Cur_Reseed;

			FETCH NEXT FROM Cur_Reseed INTO @Tablename, @columnname;

			WHILE(@@FETCH_STATUS = 0)

			BEGIN

				 IF  @columnname <> ''

				BEGIN

					SET @query = 'insert into #reseed_temp1  (tbame, mvalue) ( select  '''+@Tablename+''', max( '+@columnname+') from '+@Tablename+')';

					EXECUTE (@query);

					SELECT @Tablename = tbame, @IndentValue = isnull(mvalue,1)
					FROM #reseed_temp1 ;



					DBCC CHECKIDENT(@Tablename, RESEED, @IndentValue);



				END;

				FETCH NEXT FROM Cur_Reseed INTO @Tablename, @columnname;

			END;
			CLOSE Cur_Reseed;
			DEALLOCATE Cur_Reseed;
			DROP TABLE #reseed_temp1
		PRINT '<---Reset Identity Sucessfully-->'
		END   
		--COMMIT TRAN  CleanUpProcess
	 END TRY 
	 BEGIN CATCH 
	 SELECT ERROR_MESSAGE()
	--ROLLBACK TRAN CleanUpProcess
	 END CATCH  
END