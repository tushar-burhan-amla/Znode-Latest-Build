CREATE PROCEDURE [dbo].[Znode_DeletePortalByPortalId]
(
	 @PortalId	varchar(2000) = '' ,
	 @StoreCode varchar(2000) = '',
	 @Status	bit OUT
)
AS
	/*
	 Summary : This Procedure Is Used to delete the all records of portal if order is not place against portal  
	 --Unit Testing   
	 BEGIN TRANSACTION 
	 DECLARE @Status    BIT = 0
	 EXEC Znode_DeletePortalByPortalId @PortalId = '1,7,8', @Status   = @Status   OUT
	 SELECT @Status   
	 ROLLBACK TRANSACTION

	*/
BEGIN
	BEGIN TRAN DeletePortalByPortalId;
	BEGIN TRY
		SET NOCOUNT ON;

		--SET @PortalId = (SELECT PortalId FROM ZnodePortal ZP  )

		DECLARE @TBL_PortalIds TABLE
		( 
								 PortalId int
		);
		DECLARE @TBL_Promotion TABLE
		( 
								 PromotionId int
		);
		DECLARE @TBL_DeletedUsers TABLE (AspNetUserId NVARCHAR(1000))

		DECLARE @TBL_DeletedZnodeUsers TABLE (UserId NVARCHAR(1000))

		DECLARE @DeletedIds varchar(max)= '';
		-- inserting PortalIds which are not present in Order and Quote

		
		INSERT INTO @TBL_PortalIds 
		SELECT PortalId FROM ZnodePortal ZP
		WHERE CASE WHEN @StoreCode = '' THEN CAST(PortalId AS NVARCHAR(2000)) ELSE StoreCode END IN (

		SELECT Item FROM dbo.Split( CASE WHEN @StoreCode = '' THEN @PortalId ELSE @StoreCode END, ',' ) AS SP ) 
		AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodeOmsOrderDetails AS ZOD WHERE ZOD.PortalId = ZP.PortalId) 
	
		DELETE FROM dbo.ZnodeRobotsTxt WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeRobotsTxt.PortalId)

		DELETE FROM dbo.ZnodePortalPixelTracking WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPixelTracking.PortalId)

		DELETE ZOH
		FROM ZnodeOmsQuotePersonalizeItem ZOH
		INNER JOIN ZnodeOmsQuoteLineItem ZOM ON ZOH.OmsQuoteLineItemId = ZOM.OmsQuoteLineItemId
		WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeOmsQuote ZOQ  
										WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZOQ.PortalId) AND ZOQ.OmsQuoteId = ZOM.OmsQuoteId )

		DELETE ZOH
		FROM ZnodeOmsHistory ZOH
		INNER JOIN ZnodeOmsNotes ZOM ON ZOH.OmsNotesId = ZOM.OmsNotesId
		WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeOmsQuote ZOQ  
										WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZOQ.PortalId) AND ZOQ.OmsQuoteId = ZOM.OmsQuoteId )

		DELETE FROM ZnodeOmsQuoteLineItem WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeOmsQuote ZOQ  
										WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZOQ.PortalId) AND ZOQ.OmsQuoteId = ZnodeOmsQuoteLineItem.OmsQuoteId )

		DELETE FROM ZnodeOmsNotes WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeOmsQuote ZOQ  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZOQ.PortalId) AND ZOQ.OmsQuoteId = ZnodeOmsNotes.OmsQuoteId )

		DELETE FROM ZnodeOmsQuote WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeOmsQuote.PortalId);

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
		DELETE FROM ZnodePortalProfile WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalProfile.PortalId);
		DELETE FROM ZnodePortalFeatureMapper WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalFeatureMapper.PortalId);
		DELETE FROM ZnodePortalShippingDetails WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalShippingDetails.PortalId);
		DELETE FROM ZnodePortalUnit WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalUnit.PortalId);
		DELETE FROM ZnodeDomain WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeDomain.PortalId);
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
		DELETE FROM ZnodeFormWidgetEmailConfiguration WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCMSContentPages ZCCP  
																	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCCP.PortalId) AND ZCCP.CMSContentPagesId = ZnodeFormWidgetEmailConfiguration.CMSContentPagesId )
		DELETE FROM ZnodeCMSContentPages WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCMSContentPages.PortalId);
		
		 DELETE FROM ZnodeCaseRequestHistory WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCaseRequest ZCR  
			WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCR.PortalId) AND ZCR.CaseRequestId = ZnodeCaseRequestHistory.CaseRequestId )

		 DELETE FROM ZnodeNote WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCaseRequest ZCR  
			WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCR.PortalId) AND ZCR.CaseRequestId = ZnodeNote.CaseRequestId )
		
		DELETE FROM ZnodeCaseRequest WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeCaseRequest.PortalId);
		DELETE FROM ZnodePortalLocale WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalLocale.PortalId);
		DELETE FROM ZnodeShippingPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeShippingPortal.PortalId);
		
		DELETE FROM ZnodeSalesRepCustomerUserPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeSalesRepCustomerUserPortal.CustomerPortalId);

		delete from ZnodeSalesRepCustomerUserPortal 
		where exists(select * FROM ZnodeUserPortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeUserPortal.PortalId) and ZnodeSalesRepCustomerUserPortal.UserPortalId = ZnodeUserPortal.UserPortalId);
		
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

		DELETE FROM ZnodePortalCustomCss WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalCustomCss.PortalId);
		   
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

		DELETE ZFBGAVL
		FROM ZnodeFormBuilderGlobalAttributeValueLocale ZFBGAVL
		INNER JOIN ZnodeFormBuilderGlobalAttributeValue ZFBGAV ON ZFBGAVL.FormBuilderGlobalAttributeValueId = ZFBGAV.FormBuilderGlobalAttributeValueId
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeFormBuilderSubmit ZFBS
														WHERE EXISTS (select top 1 1 from @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZFBS.PortalId) AND ZFBS.FormBuilderSubmitId = ZFBGAV.FormBuilderSubmitId )


		DELETE FROM ZnodeFormBuilderGlobalAttributeValue WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeFormBuilderSubmit ZFBS
														WHERE EXISTS (select top 1 1 from @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZFBS.PortalId) AND ZFBS.FormBuilderSubmitId = ZnodeFormBuilderGlobalAttributeValue.FormBuilderSubmitId )

	DELETE FROM ZnodeFormBuilderSubmit WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeFormBuilderSubmit.PortalId)

		DELETE FROM ZnodeCaseRequestHistory WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCaseRequest ZCR  
	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCR.PortalId) AND ZCR.CaseRequestId = ZnodeCaseRequestHistory.CaseRequestId )

		DELETE FROM ZnodeNote WHERE EXISTS (SELECT TOP 1 1 FROM  ZnodeCaseRequest ZCR  
	WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZCR.PortalId) AND ZCR.CaseRequestId = ZnodeNote.CaseRequestId )

	DECLARE @DeleteUserId VARCHAR(MAX)
	  SET @DeleteUserId = SUBSTRING((SELECT TOP 100 ','+CAST(UserId AS VARCHAR(2000)) FROM ZnodeUser WHERE EXISTS 
					(SELECT TOP 1 1 FROM AspNetUsers ANU WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_DeletedUsers TBDU WHERE ANU.UserName = TBDU.AspNetUserId) AND ZnodeUser.AspNetUserId = ANU.Id) 
					FOR XML PATH('')), 2, 4000);
		
		DELETE FROM ZnodePortalRecommendationSetting  
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalRecommendationSetting.PortalId);

		DELETE FROM ZnodePortalPageSetting  
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalPageSetting.PortalId);

		DELETE FROM ZnodePortalSortSetting  
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortalSortSetting.PortalId);

	    EXEC Znode_DeleteUserDetails @UserId = @DeleteUserId, @Status = 0,  @IsCallInternal = 1 

		DELETE FROM ZnodeProductFeed  
		WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodeProductFeed.PortalId);

		DELETE FROM ZnodePortal WHERE EXISTS ( SELECT TOP 1 1 FROM @TBL_PortalIds AS TBP WHERE TBP.PortalId = ZnodePortal.PortalId);
		
			IF (SELECT Count(1) FROM @TBL_PortalIds) = (SELECT Count(1) FROM dbo.Split( CASE WHEN @StoreCode = '' THEN @PortalId ELSE @StoreCode END, ',' ) )	 	
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
	END TRY
	BEGIN CATCH
	
		 
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePortalByPortalId @PortalId = '+ISNULL(@PortalId,'''''')+',@StoreCode = '+ISNULL(@StoreCode,'''''')+',@Status='+ISNULL(CAST(@Status AS VARCHAR(10)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN DeletePortalByPortalId;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePortalByPortalId',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;