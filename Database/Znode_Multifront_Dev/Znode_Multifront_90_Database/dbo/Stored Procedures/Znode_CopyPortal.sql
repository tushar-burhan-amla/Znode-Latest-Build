
    
CREATE PROCEDURE [dbo].[Znode_CopyPortal]    
(    
   @PortalId int,     
   @StoreName varchar(500),    
   @CompanyName varchar(500),     
   @UserId int,    
   @StoreCode nvarchar(200),    
   @Status bit OUT)    
AS    
    /*    
     
 Summary: Create copy of existing portal    
 Copy all corresponding data into following list of tables Catalog,Profile,Units,Countries,Shipping,Locale,SMTP    
    
 ZnodeTaxClass ZnodeTaxClassPortal;ZnodeTaxRuleTypes ZnodeCaseRequest,ZnodeUserPortal ZnodeDomain,AspNetZnodeUser ZnodePortalAccount,    
 ZnodePortal ZnodePortalAddress,ZnodePortalCatalog ZnodePortalLocale,ZnodePortalFeatureMapper, ZnodeOmsUsersReferralUrl,ZnodePortalProfile ,ZnodePortalSetting    
 ZnodePortalShippingDetails ZnodePortalSmtpSetting,ZnodePortalWarehouse ,ZnodeOmsCookieMapping,ZnodePortalUnit ,ZnodePromotion,    
 ZnodePriceListPortal ,ZnodeActivityLog,ZnodeShippingPortal,ZnodeGiftCard ,ZnodeCMSContentPages ,ZnodeCMSPortalMessage,    
 ZnodeCMSPortalProductPage ,ZnodeCMSPortalSEOSetting ,ZnodeCMSPortalTheme    
        
 Unit Testing       
    ------------------------------------------------------------------------------    
               
    begin tran    
    DECLARE @Status  bit     
       EXEC Znode_CopyPortal @PortalId =2 ,@StoreName  ='copy OF Maxwells FF' , @CompanyName = 'copy OF Maxwells FF' ,@UserId = 2, @StoreCode = '',@Status = @Status OUT     
       rollback tran    
    select @Status    
       SELECT * FROM dbo.ZnodePortal zp WHERE zp.CompanyName = 'copy OF Maxwells FF'    
       SELECT * FROM ZnodePortalCatalog WHERE dbo.ZnodePortalCatalog.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
       SELECT * FROM ZnodePortalProfile WHERE dbo.ZnodePortalProfile.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
       SELECT * FROM ZnodePortalUnit WHERE dbo.ZnodePortalUnit.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
       SELECT * FROM ZnodePortalCountry WHERE dbo.ZnodePortalCountry.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
       SELECT * FROM ZnodePortalShippingDetails WHERE dbo.ZnodePortalShippingDetails.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
       SELECT * FROM ZnodePortalLocale WHERE dbo.ZnodePortalLocale.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
       SELECT * FROM ZnodePortalSmtpSetting WHERE dbo.ZnodePortalSmtpSetting.PortalId IN (SELECT portalid FROM dbo.ZnodePortal zp  WHERE zp.CompanyName = 'copy OF Maxwells FF')    
          
    ---------------------------------------------------------------------------     
     */    
BEGIN    
 BEGIN TRAN A;    
 BEGIN TRY    
  SET NOCOUNT ON;    
  DECLARE @GetDate DATETIME = dbo.Fn_GetDate();    
  -- hold the newly created pim catalog id     
  DECLARE @NewPortalId int;     
  --Check if store name is not already exist then process copy    
      
      
  IF @StoreCode NOT IN (select  storecode from ZnodePortal )    
  BEGIN    
      
  IF EXISTS ( SELECT TOP 1 1 FROM dbo.ZnodePortal AS zp WHERE PortalId = @PortalId  )     
  AND @CompanyName <> '' AND @StoreName <> '' AND @StoreCode <> '' --AND  @StoreCode <> (select  storecode from ZnodePortal )     
      
  BEGIN    
    
   INSERT INTO dbo.ZnodePortal(    
   CompanyName, StoreName, LogoPath, UseSSL, AdminEmail, SalesEmail, CustomerServiceEmail, SalesPhoneNumber, CustomerServicePhoneNumber, ImageNotAvailablePath, ShowSwatchInCategory, ShowAlternateImageInCategory, ExternalID, MobileLogoPath, DefaultOrderStateID, DefaultReviewStatus, SplashCategoryID, SplashImageFile, MobileTheme,InStockMsg,OutOfStockMsg,BackOrderMsg,CreatedBy, CreatedDate, ModifiedBy, ModifiedDate ,StoreCode,UserVerificationType)    
   SELECT @CompanyName, @StoreName, LogoPath, UseSSL, AdminEmail, SalesEmail, CustomerServiceEmail, SalesPhoneNumber, CustomerServicePhoneNumber, ImageNotAvailablePath, ShowSwatchInCategory, ShowAlternateImageInCategory, ExternalID, MobileLogoPath, DefaultOrderStateID, DefaultReviewStatus, SplashCategoryID, SplashImageFile, MobileTheme,InStockMsg,OutOfStockMsg,BackOrderMsg, @UserId, @GetDate, @UserId, @GetDate ,@StoreCode,UserVerificationType   
   FROM ZnodePortal    
   WHERE PortalId = @PortalId;    
   SET @NewPortalId = SCOPE_IDENTITY();    
  END;    
  END    
    
  -- copy all data if New portalId will generate    
  IF @NewPortalId > -0     
  BEGIN    
   INSERT INTO dbo.ZnodePortalCatalog(    
   PortalId, PublishCatalogId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, PublishCatalogId, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodePortalCatalog    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodePortalProfile(    
   PortalId, ProfileId, IsDefaultAnonymousProfile, IsDefaultRegistedProfile, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, ProfileId, IsDefaultAnonymousProfile, IsDefaultRegistedProfile, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodePortalProfile    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodePortalUnit(       
   PortalId, CurrencyId,CultureId, WeightUnit, DimensionUnit,CurrencySuffix, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, CurrencyId,CultureId, WeightUnit, DimensionUnit,CurrencySuffix, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodePortalUnit    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodePortalCountry(    
   PortalId, CountryCode, IsDefault, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, CountryCode, IsDefault, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodePortalCountry    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodeShippingPortal(        
   PortalId, ShippingOriginAddress1, ShippingOriginAddress2, ShippingOriginCity, ShippingOriginStateCode, ShippingOriginZipCode, ShippingOriginCountryCode, ShippingOriginPhone, FedExAccountNumber, FedExLTLAccountNumber, FedExMeterNumber, FedExProductionKey, FedExSecurityCode, FedExDropoffType, FedExPackagingType, FedExUseDiscountRate, FedExAddInsurance, UPSUserName, UPSPassword, UPSKey, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, ShippingOriginAddress1, ShippingOriginAddress2, ShippingOriginCity, ShippingOriginStateCode, ShippingOriginZipCode, ShippingOriginCountryCode, ShippingOriginPhone, FedExAccountNumber, FedExLTLAccountNumber, FedExMeterNumber, FedExProductionKey, FedExSecurityCode, FedExDropoffType, FedExPackagingType, FedExUseDiscountRate, FedExAddInsurance, UPSUserName, UPSPassword, UPSKey, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodeShippingPortal    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodePortalLocale(      
   PortalId, LocaleId, IsDefault, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, LocaleId, IsDefault, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodePortalLocale    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodePortalSmtpSetting(     
   PortalId, ServerName, UserName, Password, Port, IsEnableSsl, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate,DisableAllEmails )    
       SELECT @NewPortalId, ServerName, UserName, Password, Port, IsEnableSsl, @UserId, @GetDate, @UserId, @GetDate,DisableAllEmails    
 FROM ZnodePortalSmtpSetting    
       WHERE PortalId = @PortalId;    
  
   INSERT INTO dbo.ZnodeCMSPortalTheme(       
   PortalId, CMSThemeId, CMSThemeCSSId, MediaId, WebsiteTitle, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate,WebsiteDescription )    
       SELECT @NewPortalId, CMSThemeId, CMSThemeCSSId, MediaId, WebsiteTitle, @UserId, @GetDate, @UserId, @GetDate ,WebsiteDescription   
       FROM ZnodeCMSPortalTheme    
       WHERE PortalId = @PortalId;    
   INSERT INTO dbo.ZnodePortalFeatureMapper(     
   PortalId, PortalFeatureId, PortalFeatureMapperValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
       SELECT @NewPortalId, PortalFeatureId, PortalFeatureMapperValue, @UserId, @GetDate, @UserId, @GetDate    
       FROM ZnodePortalFeatureMapper    
       WHERE PortalId = @PortalId;    

		INSERT INTO ZnodePortalRecommendationSetting
		SELECT @NewPortalId,IsHomeRecommendation,IsPDPRecommendation,IsCartRecommendation,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate FROM ZnodePortalRecommendationSetting
		WHERE PortalId=@PortalId

      EXEC Znode_CopyPortalMessageAndContentPages @PortalId,@NewPortalId,@userId,0    
          
   EXEC Znode_CopyPortalEmailTemplate @NewPortalId,@PortalId,@userId    
   -- If copy process will complete successfully then return status 1     
   -- return the data set if     
   SELECT @PortalId AS ID, CAST(1 AS bit) AS [Status];     
   SET @Status = CAST(1 AS bit);    
   COMMIT TRAN A;    
  END;    
  ELSE    
  BEGIN    
   -- If copy process will not complete successfully then return status 0     
   SELECT @PortalId AS ID, CAST(0 AS bit) AS [Status];    
   SET @Status = CAST(0 AS bit);    
   ROLLBACK TRAN A;    
  END;    
 END TRY    
 BEGIN CATCH     
   select ERROR_MESSAGE()    
       SET @Status = 0;    
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_CopyPortal @PortalId = '+CAST(@PortalId AS VARCHAR(200))+',@StoreName='+@StoreName+',@CompanyName='+@CompanyName+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                        
        
             EXEC Znode_InsertProcedureErrorLog    
    @ProcedureName = 'Znode_CopyPortal',    
    @ErrorInProcedure = @Error_procedure,    
    @ErrorMessage = @ErrorMessage,    
    @ErrorLine = @ErrorLine,    
    @ErrorCall = @ErrorCall;    
 END CATCH;    
END;