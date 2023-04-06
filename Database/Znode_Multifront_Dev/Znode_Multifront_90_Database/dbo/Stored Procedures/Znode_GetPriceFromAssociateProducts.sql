CREATE  PROCEDURE [dbo].[Znode_GetPriceFromAssociateProducts]
(   
	@PortalId         INT,
    @SKU			  NVARCHAR(300),
	@PimProductId	  INT,
	@UserId			  INT = 2,
	@ProductType	  VARCHAR(200),
	@LocaleId		  INT )
AS 
  /*  
    Summary: WebStore: Calculate price from associate product and assign to parent products
	EXEC [Znode_GetPriceFromAssociateProducts]
	@PortalId         = 1 ,
	@SKU	= 'gr990',
	@PimProductId	=  97 ,
	@UserId	= 2,
	@ProductType	= 'Configurable Product',
	@LocaleId	= 1 

 */
BEGIN
    BEGIN TRAN A;
    BEGIN TRY
        SET NOCOUNT ON;
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @TBL_ListOfAssociateProducts TABLE (PimProductId int ,AssociatedProductId int,ParentSKU NVARCHAR(300),
		ChildSKU NVARCHAR(300),RetailPrice  numeric(28,6),AssociatedProductDisplayOrder int ,
		TypeOfProduct nvarchar(100),SalesPrice  numeric(28,6))
		DECLARE @tbl_PricingListOfAssociatedProduct TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
		TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000), CultureCode varchar(100), ExternalId NVARCHAR(2000),Custom1 NVARCHAR(MAX),Custom2 NVARCHAR(MAX), Custom3 NVARCHAR(MAX))				

		DECLARE @PimAttributeId INT,@currentUtcDate   VARCHAR(200) = ''
		SET @currentUtcDate   = @GetDate
		SET @PimAttributeId =   DBO.Fn_GetProductSKUAttributeId()
						Declare @ChildProductIds TABLE (Id int, AssociatedProductDisplayOrder int )
		INSERT INTO @ChildProductIds (ID,AssociatedProductDisplayOrder) 
		SELECT ZPPT.PimProductId , ZPPT.DisplayOrder  from ZnodePimProductTypeAssociation ZPPT 
		WHERE ZPPT.PimParentProductId= @PimProductId
			
		--Price logic for Associate products
		INSERT INTO @TBL_ListOfAssociateProducts
		(AssociatedProductId,ChildSKU,ParentSKU,PimProductId,RetailPrice,SalesPrice,TypeOfProduct,AssociatedProductDisplayOrder)
		SELECT ZPAV.PimProductId,ZPAVL.AttributeValue,@SKU ,@PimProductId,NULL , NULL,@PimProductId,
		CPI.AssociatedProductDisplayOrder 
		FROM ZnodePimAttributeValue ZPAV INNER JOIN ZnodePimAttributeValueLocale ZPAVL 
		ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		Inner join @ChildProductIds  CPI ON CPI.ID = ZPAV.PimProductId  
		where ZPAV.PimAttributeId  = @PimAttributeId 

		SELECT @SKU = Substring((SELECT ',' + Convert(nvarchar(100),ChildSKU) 
		FROM @TBL_ListOfAssociateProducts where AssociatedProductId is not null FOR XML PAth('')),2,4000) 

		INSERT INTO @tbl_PricingListOfAssociatedProduct (SKU,RetailPrice ,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix, CultureCode, ExternalId,Custom1,Custom2,Custom3)	
		EXEC Znode_GetPublishProductPricingBySku  @Sku ,@portalID  ,@currentUtcDate,@UserId 

		update PLC SET PLC.RetailPrice = PLCA.RetailPrice ,
		PLC.SalesPrice = PLCA.SalesPrice 
		from @TBL_ListOfAssociateProducts PLC inner join @tbl_PricingListOfAssociatedProduct
		PLCA on PLC.ChildSKU = PLCA.sku
			
		If @ProductType in ('Configurable Product','grouped product')
			select Min(RetailPrice)  RetailPrice  , Min(SalesPrice) SalesPrice  from @TBL_ListOfAssociateProducts			
				
		COMMIT TRAN A;
			
    END TRY
    BEGIN CATCH
        DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
		@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductDataForWebStore_ver1 @PortalId='+CAST(@PortalId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetProductDataForWebStore_ver1',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;