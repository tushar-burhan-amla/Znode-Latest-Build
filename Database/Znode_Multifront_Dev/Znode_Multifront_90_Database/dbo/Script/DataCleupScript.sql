

IF EXISTS (SELECT TOP 1 1 FROM Sys.Tables WHERE Name = 'ZnodeMultifront')
BEGIN 
 IF EXISTS (SELECT TOP 1 1 FROM ZnodeMultifront Having Max(BuildVersion) =   903094 )
 BEGIN 
 PRINT 'Script is already executed....'
  SET NOEXEC ON 
 END 
END
ELSE 
BEGIN 
   SET NOEXEC ON
END 
INSERT INTO [dbo].[ZnodeMultifront] ( [VersionName], [Descriptions], [MajorVersion], [MinorVersion], [LowerVersion], [BuildVersion], [PatchIndex], [CreatedBy], 
[CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES ( N'Znode_Multifront_01', N'CleanupScript', 9, 0, 3, 903094, 0, 2, GETDATE(), 2, GETDATE())
GO
if exists (select * from sys.procedures where name ='Znode_DeletePortalByPortalId')
	drop procedure Znode_DeletePortalByPortalId

GO
CREATE PROCEDURE [dbo].[Znode_DeletePortalByPortalId]
(
	 @PortalId	varchar(2000)
	,@Status	bit OUT)
AS
	/*
	 Summary : This Procedure Is Used to delete the all records of portal if order is not place against portal  
	 --Unit Testing   
	 BEGIN TRANSACTION 
	 DECLARE @Status    BIT = 0
	 EXEC Znode_DeletePortalByPortalId @PortalId = 36, @Status   = @Status   OUT
	 SELECT @Status   
	 ROLLBACK TRANSACTION
	*/
BEGIN
	BEGIN TRAN DeletePortalByPortalId;
	--BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @TBL_PortalIds TABLE
		( 
								 PortalId int
		);
		DECLARE @TBL_Promotion TABLE
		( 
								 PromotionId int
		);
		DECLARE @TBL_DeletedUsers TABLE (AspNetUserId NVARCHAR(1000))

		DECLARE @DeletedIds varchar(max)= '';
		-- inserting PortalIds which are not present in Order and Quote

		INSERT INTO @TBL_PortalIds 
		SELECT Item FROM dbo.Split( @PortalId, ',' ) AS SP 
		WHERE NOT EXISTS ( SELECT TOP 1 1 FROM ZnodeOmsOrderDetails AS ZOD WHERE ZOD.PortalId = Sp.Item) 
		AND  NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsQuote AS ZOQ WHERE ZOQ.PortalId = Sp.Item );
		

	     DELETE FROM  ZnodeCustomPortalDetail  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCustomPortalDetail.PortalId);
	     DELETE FROM  ZnodeSupplier WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeSupplier.PortalId)

	     DELETE FROM  ZnodeOmsTemplateLineItem  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP INNER JOIN ZnodeOmsTemplate ZOT ON 
	     TBP.PortalId = ZOT.PortalId AND ZOT.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId);

	     DELETE FROM ZnodeOmsTemplate WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsTemplate.PortalId);
	     DELETE FROM  ZnodeOmsUsersReferralUrl WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsUsersReferralUrl.PortalId)

		DELETE FROM ZnodePortalShipping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalShipping.PortalId);
		DELETE FROM dbo.ZnodePortalPixelTracking  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPixelTracking.PortalId);
		DELETE FROM dbo.ZnodeRobotsTxt  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeRobotsTxt.PortalId);

		DELETE FROM ZnodePortalTaxClass WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalTaxClass.PortalId);
		DELETE FROM ZnodePortalPaymentSetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPaymentSetting.PortalId);
		DELETE FROM ZnodeCMSPortalMessageKeyTag WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalMessageKeyTag.PortalId);
		DELETE FROM ZnodePortalProfile WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalProfile.PortalId);
		DELETE FROM ZnodePortalFeatureMapper WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalFeatureMapper.PortalId);
		DELETE FROM ZnodePortalShippingDetails WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalShippingDetails.PortalId);
		DELETE FROM ZnodePortalUnit WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalUnit.PortalId);
		DELETE FROM ZnodeDomain WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeDomain.PortalId);
		DELETE FROM ZnodePortalSmtpSetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalSmtpSetting.PortalId);
		DELETE FROM ZnodeActivityLog WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeActivityLog.PortalId);
		DELETE FROM ZnodePortalCatalog WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalCatalog.PortalId );
		DELETE FROM ZnodeCMSPortalMessage  WHERE EXISTS  ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalMessage.PortalId );
		--DELETE FROM ZnodeTaxRule WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeTaxRule.PortalId);

		--select * from ZnodeTaxRule  where exists (select top 1 1 from ZnodeTaxClass b where ZnodeTaxRule.TaxClassId = b.TaxClassId 
		--																		AND EXISTS (SELECT TOP  1 1 FROM ZnodePortalTaxClass c WHERE b.TaxClassId = c.TaxClassId AND c.PortalId <> 1))

		DELETE FROM  ZnodeTaxRule  WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeTaxClass b WHERE ZnodeTaxRule.TaxClassId = b.TaxClassId 
																				AND EXISTS (SELECT TOP  1 1 FROM ZnodePortalTaxClass c WHERE b.TaxClassId = c.TaxClassId AND c.PortalId <> 1))


		DELETE FROM ZnodeGoogleTagManager WHERE  EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeGoogleTagManager.PortalId);
		DELETE FROM ZnodeTaxRuleTypes WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeTaxRuleTypes.PortalId);
		DELETE FROM ZnodeCMSContentPagesProfile WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeCMSContentPagesProfile.CMSContentPagesId )
		DELETE FROM ZnodeCMSContentPageGroupMapping WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeCMSContentPageGroupMapping.CMSContentPagesId )
	     DELETE FROM ZnodeCMSContentPagesLocale WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeCMSContentPagesLocale.CMSContentPagesId )
		DELETE FROM ZnodeCMSContentPages WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSContentPages.PortalId);
		DELETE FROM ZnodeCaseRequest WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCaseRequest.PortalId);
		DELETE FROM ZnodePortalLocale WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalLocale.PortalId);
		DELETE FROM ZnodeShippingPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeShippingPortal.PortalId);
		DELETE FROM ZnodePortalDisplaySetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalDisplaySetting.PortalId);
		DELETE FROM ZnodeUserPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeUserPortal.PortalId);
		DELETE FROM AspNetZnodeUser OUTPUT DELETED.AspNetZnodeUserId   INTO @TBL_DeletedUsers WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = AspNetZnodeUser.PortalId )
		
		DELETE FROM ZnodePortalAlternateWarehouse WHERE EXISTS ( SELECT TOP 1 1 FROM ZnodePortalWareHouse AS ZPWH WHERE EXISTS (
				SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZPWH.PortalId ) AND  ZPWH.PortalWarehouseId = ZnodePortalAlternateWarehouse.PortalWarehouseId);
		DELETE FROM ZnodePortalWareHouse WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalWareHouse.PortalId);
		DELETE ZnodePriceListPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePriceListPortal.PortalId );
		
		DELETE FROM ZnodeEmailTemplateMapper WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeEmailTemplateMapper.PortalId);
		DELETE FROM ZnodeGiftCard WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeGiftCard.PortalId );
		DELETE FROM ZnodeCMSPortalProductPage WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalProductPage.PortalId);

		DELETE FROM ZnodeCMSPortalSEOSetting WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalSEOSetting.PortalId);

		DELETE FROM ZnodeCMSPortalTheme WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSPortalTheme.PortalId);

		DELETE FROM ZnodeCMSSEODetailLocale WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP INNER JOIN ZnodeCMSSEODetail AS zcsd ON TBP.PortalId = zcsd.PortalId WHERE zcsd.CMSSEODetailId = ZnodeCMSSEODetailLocale.CMSSEODetailId);

		DELETE FROM ZnodeCMSSEODetail WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSSEODetail.PortalId);
		DELETE FROM ZnodePortalAccount WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalAccount.PortalId);

		DELETE FROM ZnodePortalAddress WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalAddress.PortalId);

		DELETE FROM ZnodeOmsCookieMapping WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsCookieMapping.PortalId);

		DELETE FROM ZnodePortalCountry WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalCountry.PortalId);

		DELETE FROM ZnodeCMSUrlRedirect WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSUrlRedirect.PortalId);
		   
		/* Remove Search index */
		--DELETE FROM ZnodeSearchIndexServerStatus WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP INNER JOIN ZnodePortalIndex AS zpi ON TBP.PortalId = zpi.PortalId
		--		 INNER JOIN ZnodeSearchIndexMonitor AS zsim ON zpi.PortalIndexId = zsim.PortalIndexId WHERE zsim.SearchIndexMonitorId = ZnodeSearchIndexServerStatus.SearchIndexMonitorId);
		--DELETE FROM ZnodeSearchIndexMonitor WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP INNER JOIN ZnodePortalIndex AS zpi ON TBP.PortalId = zpi.PortalId WHERE zpi.PortalIndexId = ZnodeSearchIndexMonitor.PortalIndexId );
		--DELETE FROM ZnodePortalIndex WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalIndex.PortalId);
		/* Remove Search index */
		DELETE FROM ZnodePromotion WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePromotion.PortalId);
		DELETE FROM ZnodeTaxPortaL  WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeTaxPortaL.PortalId);

		INSERT INTO @TBL_Promotion( PromotionId ) SELECT PromotionId FROM ZnodePromotion WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePromotion.PortalId);
		DELETE FROM ZnodePromotionProduct WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionProduct.PromotionId);

		DELETE FROM ZnodePromotionCategory WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionCategory.PromotionId);
		DELETE FROM ZnodePromotionCatalogs WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotionCatalogs.PromotionId);
		DELETE FROM ZnodePromotion WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_Promotion AS TBP WHERE TBP.PromotionId = ZnodePromotion.PromotionId);

		
		DELETE FROM ZnodeBlogNewsLocale where exists (select top 1 1 from ZnodeBlogNews ZBN
													where EXISTS (SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZnodeBlogNewsLocale.BlogNewsId )



		DELETE FROM ZnodeBlogNewsCommentLocale where exists (select top 1 1 from ZnodeBlogNewsComment ZBC
													where exists (select top 1 1 from ZnodeBlogNews ZBN
														where exists (select top 1 1 from @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZBC.BlogNewsId ) and ZBC.BlogNewsCommentId = ZnodeBlogNewsCommentLocale.BlogNewsCommentId)
													



		DELETE FROM ZnodeBlogNewsComment WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeBlogNews ZBN
													WHERE EXISTS (select top 1 1 from @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZBN.PortalId) AND ZBN.BlogNewsId = ZnodeBlogNewsComment.BlogNewsId )



		DELETE FROM ZnodeBlogNews WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeBlogNews.PortalId)

		DELETE FROM ZnodePortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortal.PortalId);

		IF (SELECT Count(1) FROM @TBL_PortalIds) = (SELECT Count(1) FROM dbo.Split( @PortalId, ',' ) )
		BEGIN 
		SELECT 1 AS ID, CAST(1 AS bit) AS Status;
		SET @Status = 1;
		END 
		ELSE 
		BEGIN 
		SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		SET @Status = 0;

		END 
		COMMIT TRAN DeletePortalByPortalId;
	--END TRY
	--BEGIN CATCH
		 
	--	     SET @Status = 0;
	--	     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePortalByPortalId @PortalId = '+@PortalId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
 --            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	--	     ROLLBACK TRAN DeletePortalByPortalId;
 --            EXEC Znode_InsertProcedureErrorLog
	--			@ProcedureName = 'Znode_DeletePortalByPortalId',
	--			@ErrorInProcedure = @Error_procedure,
	--			@ErrorMessage = @ErrorMessage,
	--			@ErrorLine = @ErrorLine,
	--			@ErrorCall = @ErrorCall;
	--END CATCH;
END;

GO


if exists (select * from sys.procedures where name ='Znode_DeleteAllProduct')
	drop procedure Znode_DeleteAllProduct

	go
create Procedure [dbo].[Znode_DeleteAllProduct] (@IsAllProduct bit = 0 ,@DeleteProductId VARCHAR(MAX) = '')
	AS
	Begin
	       Declare @Status Bit
		  IF @IsAllProduct  =1 
		  BEGIN
		  WHILE 1=1
			BEGIN 
				SET @DeleteProductId = SUBSTRING((SELECT ','+CAST(PimProductId AS VARCHAR(50)) FROM ZnodePimProduct FOR XML PATH('')), 2, 8000);
				--Remove extra products from catalog
				EXEC Znode_DeletePimProducts @PimProductId = @DeleteProductId, @Status = @Status;
				
				IF ISnull(@DeleteProductId,'') = '' BREAK;
			END          
		 END
		  Else
		  Begin
			 EXEC Znode_DeletePimProducts @PimProductId = @DeleteProductId, @Status = @Status;
			 Select @DeleteProductId
		  END			 
	End

	go
EXEC [Znode_DeleteAllProduct] @IsAllProduct =1 
EXEC [dbo].[Znode_DeleteAllCategory] @IsAllCategory = 1
EXEC [dbo].[Znode_DeleteAllCatalog] @IsAllCatalog = 1 , @IsDeleteFromPublish =0

GO 


/*
    This Script will Delete all Orders 
*/
BEGIN TRAN 

DELETE FROM ZnodeOmsOrderWarehouse
PRINT '...<Delete Order Record From ZnodeOmsOrderWarehouse>...'
DELETE FROM ZnodeRmaRequestItem
PRINT '...<Delete Order Record From ZnodeRmaRequestItem>...'
DELETE FROM ZnodeOmsHistory
PRINT '...<Delete Order Record From ZnodeOmsHistory>...'
DELETE FROM znodeOmsNotes  
PRINT '...<Delete Order Record From znodeOmsNotes>...'
DELETE FROM ZnodeOmsOrderAttribute
PRINT '...<Delete Order Record From ZnodeOmsOrderAttribute>...'
DELETE FROM ZnodeOmsOrderDiscount 
PRINT '...<Delete Order Record From ZnodeOmsOrderDiscount>...'
DELETE FROM ZnodeOmsPersonalizeItem 
PRINT '...<Delete Order Record From ZnodeOmsPersonalizeItem>...'
DELETE FROM znodeOmsOrderLineItems 
PRINT '...<Delete Order Record From znodeOmsOrderLineItems>...'
DELETE FROM ZnodeOmsOrderWarehouse
PRINT '...<Delete Order Record From ZnodeOmsOrderWarehouse>...'
DELETE FROM ZnodeOmsOrderWarehouse
PRINT '...<Delete Order Record From ZnodeOmsOrderWarehouse>...'
DELETE FROM ZnodeOmsOrderShipment 
PRINT '...<Delete Order Record From ZnodeOmsOrderShipment>...'
DELETE FROM ZnodeOmsTaxOrderLineDetails 
PRINT '...<Delete Order Record From ZnodeOmsTaxOrderLineDetails>...'
DELETE FROM znodeGiftCardHistory 
PRINT '...<Delete Order Record From znodeGiftCardHistory>...'
DELETE FROM znodeOmsEmailHistory 
PRINT '...<Delete Order Record From znodeOmsEmailHistory>...'
DELETE FROM ZnodeOmsReferralCommission 
PRINT '...<Delete Order Record From ZnodeOmsReferralCommission>...'
DELETE FROM ZnodeOmsTaxOrderDetails 
PRINT '...<Delete Order Record From ZnodeOmsTaxOrderDetails>...'
DELETE FROM ZnodeOmsOrderDetails  
PRINT '...<Delete Order Record From ZnodeOmsOrderDetails>...'
DELETE FROM ZnodeOmsHistory
PRINT '...<Delete Order Record From ZnodeOmsHistory>...'
DELETE FROM ZnodeOmsOrderDetails
PRINT '...<Delete Order Record From ZnodeOmsOrderDetails>...'
DELETE FROM ZnodeOmsOrder
PRINT '...<Delete Order Record From ZnodeOmsOrder>...'
DELETE FROM ZnodeOmsPersonalizeCartItem
PRINT '...<Delete Order Record From ZnodeOmsPersonalizeCartItem>...'
DELETE FROM ZnodeOmsSavedCartLineItem 
PRINT '...<Delete Order Record From ZnodeOmsSavedCartLineItem>...'
DELETE FROM ZnodeOmsSavedCart
PRINT '...<Delete Order Record From ZnodeOmsSavedCart>...'
DELETE FROM ZnodeOmsCookieMapping
PRINT '...<Delete Order Record From ZnodeOmsCookieMapping>...'
COMMIT  TRAN 

GO 

declare @PortalId nvarchar(max),@Status bit 
SET @PortalId = (Select Substring((select ',' + convert(nvarchar(500), portalid)  from ZnodePortal where PortalId <> 1  for XML Path ('')),2,4000) )
EXEC Znode_DeletePortalByPortalId @PortalId = @PortalId  , @Status = @Status Out 
Select @Status 

GO 
declare @PriceListId nvarchar(max),@Status bit 
SET @PriceListId = (Select Substring((select ',' + convert(nvarchar(500), PriceListId)  from ZnodePriceList  for XML Path ('')),2,4000) )
EXEC [dbo].[Znode_DeletePriceList] @PriceListId =  @PriceListId  ,  @Status = @Status  OUT

GO 
declare @WarehouseId nvarchar(max),@Status bit 
SET @WarehouseId = (Select Substring((select ',' + convert(nvarchar(500), WarehouseId)  from ZnodeWarehouse  for XML Path ('')),2,4000) )
exec Znode_DeleteWarehouse @WarehouseId = @WarehouseId , @Status =  @Status OUT

GO 
declare @PimVendorId nvarchar(max),@Status bit 
SET @PimVendorId = (Select Substring((select ',' + convert(nvarchar(500), PimVendorId)  from ZnodePimVendor  for XML Path ('')),2,4000) )
exec [Znode_DeleteVendor] @PimVendorId = @PimVendorId , @Status =  @Status OUT

GO
------brand
declare @BrandId nvarchar(max),@Status bit 
SET @BrandId = (Select Substring((select ',' + convert(nvarchar(500), BrandId)  from ZnodeBrandDetails  for XML Path ('')),2,4000) )
exec Znode_DeleteBrand @BrandId = @BrandId , @Status =  @Status OUT
go

------CMSContentPageGroups
declare @CMSContentPageGroupId nvarchar(max),@Status bit 
SET @CMSContentPageGroupId = (Select Substring((select ',' + convert(nvarchar(500), CMSContentPageGroupId)  from ZnodeCMSContentPageGroup  for XML Path ('')),2,4000) )
exec [dbo].[Znode_DeleteContentPageGroups] @CMSContentPageGroupId = @CMSContentPageGroupId ,@Status = @Status out
go

------CMSContentPage
declare @CMSContentPageId nvarchar(max),@Status bit 
SET @CMSContentPageId = (Select Substring((select ',' + convert(nvarchar(500), CMSContentPagesId)  from ZnodeCMSContentPages  for XML Path ('')),2,4000) )
exec [dbo].[Znode_DeleteContentPage] @CMSContentPageId = @CMSContentPageId ,@Status = @Status out
go


------CMSThemeId
declare @CMSThemeId nvarchar(max),@Status bit 
SET @CMSThemeId = (Select Substring((select ',' + convert(nvarchar(500), CMSThemeId)  from ZnodeCMSTheme  for XML Path ('')),2,4000) )
exec [dbo].[Znode_DeleteCMSTheme] @CMSThemeId = @CMSThemeId,@Status = @Status out
go

------CMSSlider
declare @CMSSliderId nvarchar(max),@Status bit 
SET @CMSSliderId = (Select Substring((select ',' + convert(nvarchar(500), CMSSliderId)  from ZnodeCMSSlider  for XML Path ('')),2,4000) )
exec Znode_DeleteCMSSlider @CMSSliderId = @CMSSliderId,@Status = @Status out
go

------Media
declare @MediaId nvarchar(max),@Status bit 
SET @MediaId = (Select Substring((select ',' + convert(nvarchar(500), MediaId)  from ZnodeMedia  for XML Path ('')),2,4000) )
exec Znode_DeleteMedia @MediaId = @MediaId,@Status = @Status out
go

----- ZnodeCMSSEODetail
delete from ZnodeCMSSEODetailLocale
delete from ZnodeCMSSEODetail

go
---CMSSliderBanner
delete from ZnodeCMSSliderBannerLocale
delete from ZnodeCMSSliderBanner
go
---CMSSliderBanner
delete from ZnodeHighlightLocale
delete from ZnodeHighlight
go
---UserWishList
delete from ZnodeUserWishList
go
---PimAttribute
declare @PimAttributeId nvarchar(max),@Status bit 
SET @PimAttributeId = (Select Substring((select ',' + convert(nvarchar(500), PimAttributeId)  from ZnodePimAttribute  where IsSystemDefined <> 1 for XML Path ('')),2,4000) )
exec Znode_DeletePimAttributeWithReference  @PimAttributeId = @PimAttributeId ,@Status = @Status out
go
----elmah_error
delete from elmah_error

---Profile
declare @ProfileId nvarchar(max),@Status bit 
SET @ProfileId = (Select Substring((select ',' + convert(nvarchar(500), ProfileId)  from ZnodeProfile for XML Path ('')),2,4000) )
exec Znode_DeleteProfile  @ProfileId = @ProfileId ,@Status = @Status out
go
---PimAttributeFamily
declare @PimAttributeFamilyId nvarchar(max),@Status bit 
SET @PimAttributeFamilyId = (Select Substring((select ',' + convert(nvarchar(500), PimAttributeFamilyId)  from ZnodePimAttributeFamily for XML Path ('')),2,4000) )
exec Znode_DeletePimFamily  @PimAttributeFamilyId = @PimAttributeFamilyId ,@Status = @Status out
go
---PimAttributegroup
declare @PimAttributegroupedId nvarchar(max),@Status bit 
SET @PimAttributegroupedId = (Select Substring((select ',' + convert(nvarchar(500), PimAttributeGroupId)  from ZnodePimAttributegroup for XML Path ('')),2,4000) )
exec Znode_DeleteAttributeGroup  @PimAttributegroupedId = @PimAttributegroupedId ,@Status = @Status out
go
---BlogNews
declare @BlogNewsId nvarchar(max),@Status bit 
SET @BlogNewsId = (Select Substring((select ',' + convert(nvarchar(500), BlogNewsId)  from ZnodeBlogNews for XML Path ('')),2,4000) )
exec Znode_DeleteBlogNews  @BlogNewsId = @BlogNewsId ,@Status = @Status out
go
---User
declare @UserId nvarchar(max),@Status bit 
SET @UserId = (Select Substring((select ',' + convert(nvarchar(500), UserId)  from ZnodeUser where UserId <> 2  for XML Path ('')),2,4000) )
exec Znode_DeleteUserDetails  @UserId = @UserId ,@Status = @Status out
go
---Promotion
declare @PromotionId nvarchar(max),@Status bit 
SET @PromotionId = (Select Substring((select ',' + convert(nvarchar(500), PromotionId)  from ZnodePromotion for XML Path ('')),2,4000) )
exec Znode_DeletePromotion  @PromotionId = @PromotionId ,@Status = @Status out
go
---OmsOrderDetails
declare @OrderDetailId nvarchar(max),@Status bit 
SET @OrderDetailId = (Select Substring((select ',' + convert(nvarchar(500), OmsOrderDetailsId)  from ZnodeOmsOrderDetails for XML Path ('')),2,4000) )
exec Znode_DeleteOrderById  @OrderDetailId = @OrderDetailId ,@Status = @Status out
go

delete from ZnodeDomain

insert into znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModifiedBy	,ModifiedDate)
select (select top 1 PortalId from ZnodePortal ), 'localhost:6766',1, '115915F1-7E6B-4386-A623-9779F27D9A5E','Admin',2,GETDATE(),2,GETDATE()
where not exists(select * from znodedomain where DomainName = 'localhost:6766'  and PortalId = (select top 1 PortalId from ZnodePortal ))

insert into znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModifiedBy	,ModifiedDate)
select (select top 1 PortalId from ZnodePortal ), 'localhost:3288',1, 'c58cc0c0-1349-4001-8416-cf1cea7960e8','WebStore',2,GETDATE(),2,GETDATE()
where not exists(select * from znodedomain where DomainName = 'localhost:3288'  and PortalId = (select top 1 PortalId from ZnodePortal ))

insert into znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModifiedBy	,ModifiedDate)
select (select top 1 PortalId from ZnodePortal ), 'localhost:44762',1, '8a8b4931-7d57-42e8-a005-b1c0cce49f1d','Api',2,GETDATE(),2,GETDATE()
where not exists(select * from znodedomain where DomainName = 'localhost:44762' and PortalId = (select top 1 PortalId from ZnodePortal ) )

insert into znodedomain(PortalId,DomainName,	IsActive,	ApiKey,	ApplicationType,	CreatedBy,	CreatedDate	,ModifiedBy	,ModifiedDate)
select (select top 1 PortalId from ZnodePortal ), 'localhost',1, '115915F1-7E6B-4386-A623-9779F27D9A5E','Admin',2,GETDATE(),2,GETDATE()
where not exists(select * from znodedomain where DomainName = 'localhost'  and PortalId = (select top 1 PortalId from ZnodePortal ))

go
delete from ZnodeSearchIndexServerStatus
delete from ZnodeSearchIndexMonitor
delete from ZnodeCatalogIndex
delete from ZnodePublishCatalogSearchProfile
delete from ZnodeCatalogIndex
delete from ZnodePublishCatalogLog
delete from ZnodePublishCategoryDetail
delete from ZnodePublishCategoryProduct
delete from ZnodePublishProductDetail
delete from ZnodePublishCategory
delete from dbo.ZnodeCMSCustomerReview 
delete from ZnodeCMSWidgetProduct
delete from ZnodePublishProduct
delete from dbo.ZnodePortalCatalog 
delete from ZnodePublishCatalog
delete from ZnodePublishedXml
delete from ZnodePublishPortalLog
delete from ZnodeListViewFilter
delete from ZnodeListView
GO 


Declare @PimCatalogId int, @PublishCatalogId   int 
insert into ZnodePimCatalog(CatalogName	,IsActive	,ExternalId	,CreatedBy	,CreatedDate	,ModifiedBy	,ModifiedDate)
Select 'FineFoodCatalog',1,null,2,GETDATE(),2,GETDATE()

SET @PimCatalogId  = @@Identity

SET @PublishCatalogId    =0 
insert into ZnodePublishCatalog(PimCatalogId	,CatalogName	,ExternalId	,CreatedBy	,CreatedDate	,ModifiedBy	,ModifiedDate	,Tokem)
Select @PimCatalogId  , 'FineFood' , null, 2 ,Getdate(), 2 , Getdate() , ''
SET @PublishCatalogId    =@@Identity 

insert into ZnodePortalCatalog (PortalId,	PublishCatalogId	,CreatedBy	,CreatedDate	,ModifiedBy	,ModifiedDate) 
Select (Select TOP 1 portalid from ZnodePortal), @PublishCatalogId ,2 , GETDATE(),2 , GETDATE()

GO


BEGIN
	DECLARE @table_name varchar(100)= NULL, @showReport bit= 0, @debug bit= 0
	
	If OBJECT_ID('tempdb..#reseed_temp1') < 0 
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

			IF @columnname <> ''

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
	END
