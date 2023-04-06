CREATE PROCEDURE [dbo].[Znode_CopyUserProfile]
(
	  @PortalId int, 
	  @ProfileId int,
	  @ProfileName varchar(500),
	  @UserId Int,
	  @Status bit OUT)
AS   
BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		SET NOCOUNT ON;
		
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate()

		DECLARE @ProfileIdNew INT, @PortalProfileIdNew INT

		IF NOT EXISTS(SELECT * FROM ZnodeProfile WHERE ProfileName = @ProfileName)
		BEGIN
			INSERT INTO ZnodeProfile(ProfileName,	ShowOnPartnerSignup	,Weighting,	TaxExempt,	DefaultExternalAccountNo,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	ParentProfileId, PimCatalogId)
			SELECT @ProfileName,	ShowOnPartnerSignup	,Weighting,	TaxExempt,	DefaultExternalAccountNo,	@UserId,@GetDate,@UserId,@GetDate,	@ProfileId ParentProfileId, PimCatalogId
			FROM ZnodeProfile WHERE ProfileId = @ProfileId 
			

			set @ProfileIdNew = @@Identity

			if (isnull(@ProfileIdNew,0) <> 0)
			begin
				insert into ZnodePortalProfile(PortalId,ProfileId,IsDefaultAnonymousProfile,IsDefaultRegistedProfile,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				select @PortalId,@ProfileIdNew,IsDefaultAnonymousProfile,IsDefaultRegistedProfile,@UserId,@GetDate,@UserId,@GetDate
				from ZnodePortalProfile 
				where ProfileId = @ProfileId
				AND PortalId = @PortalId

				set @PortalProfileIdNew  = @@Identity

				insert into ZnodeProfileShipping(ProfileId,ShippingId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder)
				select @ProfileIdNew,ShippingId,@UserId,@GetDate,@UserId,@GetDate,DisplayOrder
				from ZnodeProfileShipping where ProfileId = @ProfileId

				--insert into ZnodeAccountProfile(AccountId,ProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsDefault)
				--select AccountId,@ProfileIdNew,@UserId,@GetDate,@UserId,@GetDate,IsDefault
				
				UPDATE aa
				SET ProfileId = @ProfileIdNew 
					,ModifiedBy = @UserId 
					,ModifiedDate = @GetDate
				from ZnodeAccountProfile aa where ProfileId = @ProfileId
				AND EXISTS (SELECT TOP 1 1  FROM ZnodeUserPortal a INNER JOIN ZnodeUser b ON (b.UserId = a.UserId) 
			    WHERE b.AccountID = aa.AccountID )


				insert into ZnodeCMSContentPagesProfile(ProfileId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				select @ProfileIdNew,CMSContentPagesId,@UserId,@GetDate,@UserId,@GetDate
				from ZnodeCMSContentPagesProfile where ProfileId = @ProfileId

				--insert into ZnodeUserProfile(ProfileId,UserId,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				--select @ProfileIdNew,a.UserId,IsDefault,@UserId,@GetDate,@UserId,@GetDate
				
				UPDATE a 
				SET ProfileId = @ProfileIdNew 
						,ModifiedBy = @UserId 
						,ModifiedDate = @GetDate
				from ZnodeUserProfile a 
				INNER JOIN ZnodeUserPortal b ON (b.UserId = a.UserId )
				where a.ProfileId = @ProfileId
				AND b.PortalId = @PortalID
				
				INSERT INTO ZnodeProfilePaymentSetting(PaymentSettingId,ProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder)
				SELECT PaymentSettingId,@ProfileIdNew,@UserId,@GetDate,@UserId,@GetDate,DisplayOrder
				FROM ZnodeProfilePaymentSetting where ProfileId = @ProfileId

				INSERT INTO ZnodePromotion(PromoCode,Name,Description,PromotionTypeId,Discount,StartDate,EndDate,OrderMinimum,QuantityMinimum,IsCouponRequired,IsAllowedWithOtherCoupons,PromotionMessage,DisplayOrder,IsUnique,PortalId,ProfileId,PromotionProductQuantity,ReferralPublishProductId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				SELECT PromoCode,Name,Description,PromotionTypeId,Discount,StartDate,EndDate,OrderMinimum,QuantityMinimum,IsCouponRequired,IsAllowedWithOtherCoupons,PromotionMessage,DisplayOrder,IsUnique,PortalId,@ProfileIdNew,PromotionProductQuantity,ReferralPublishProductId,@UserId,@GetDate,@UserId,@GetDate
				FROM ZnodePromotion WHERE ProfileId = @ProfileId
				AND PortalId = @PortalId


				INSERT INTO ZnodePriceListProfile (PriceListId,PortalProfileId,Precedence,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
				SELECT ZPLP.PriceListId,@PortalProfileIdNew,ZPLP.Precedence,@UserId,@GetDate,@UserId,@GetDate
				FROM ZnodePriceListProfile  ZPLP 
				INNER JOIN ZnodePortalProfile ZPP
				on ZPLP.PortalProfileId = ZPP.PortalProfileID
				where ZPP.ProfileId =@ProfileId and ZPP.PortalId = @PortalId

				DELETE FROM dbo.ZnodePriceListProfile WHERE PortalProfileId IN (SELECT PortalProfileId FROM ZnodePortalProfile WHERE ProfileId = @ProfileId AND PortalId = @PortalId  )
				DELETE FROM ZnodePortalProfile WHERE ProfileId = @ProfileId AND PortalId = @PortalId 
				

			end
		end

		-- copy all data if New portalId will generate
		IF (Select Count(ProfileId) from ZnodeProfile where ProfileId = @ProfileId and ProfileName =@ProfileName) >= 0
		BEGIN
			
			SELECT @PortalId AS ID, CAST(1 AS bit) AS [Status]; 
			SET @Status = CAST(1 AS bit);
			COMMIT TRAN A;
		END;
		ELSE
		BEGIN
			-- If copy process will not complete successfully then return status 0 
			SELECT @PortalId AS ID, CAST(0 AS bit) AS [Status];
			SET @Status = CAST(1 AS bit);
			ROLLBACK TRAN A;
		END;
		
	END TRY
	BEGIN CATCH 
		    SELECT ERROR_MESSAGE()
		     SET @Status = 0;
		    -- DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_CopyPortal @PortalId = '+CAST(@PortalId AS VARCHAR(200))+',@StoreName='+@StoreName+',@CompanyName='+@CompanyName+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
             ROLLBACK TRAN A; 			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
    --         EXEC Znode_InsertProcedureErrorLog
				--@ProcedureName = 'Znode_CopyPortal',
				--@ErrorInProcedure = @Error_procedure,
				--@ErrorMessage = @ErrorMessage,
				--@ErrorLine = @ErrorLine,
				--@ErrorCall = @ErrorCall;
	END CATCH;
END;