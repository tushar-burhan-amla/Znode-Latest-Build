
CREATE PROCEDURE [dbo].[Znode_CopyPricingList](
	  @PriceListId    int, 
	  @NewListName    varchar(600),
	  @NewListCode    varchar(200), 
	  @ActivationDate datetime= NULL, 
	  @ExpirationDate datetime= NULL, 
	  @CurrencyId     int= NULL, 
	  @CultureId     int= NULL, 
	  @Status         bit OUT)
AS
   /*
    
	 Summary : Create copy of existing Price List, If Activation date and Expiration date is 
	           '1753-01-01' as a input from frontend then it is set to null 
			   three tables are affected here "ZnodePriceList" create the new price list 
			   "ZnodePrice" copy  sku's and quantity and "ZnodePriceTier" copy tier price 
     
	 Unit Testing   	
	 begin tran
     EXEC [Znode_CopyPricingList] 2,"Copy Of  C","Copy Of C",'7/5/2016 12:00:00 AM','7/21/2016 12:00:00 AM',41,0
	 rollback tran
    */
BEGIN
	
	BEGIN TRY
		SET NOCOUNT ON;
		-- hold the newly created price list id 
		DECLARE @NewPriceListId int= 0; 

		IF EXISTS
		(
			SELECT TOP 1 1
			FROM ZnodePriceList AS ZPL
			WHERE ZPL.PriceListId = @PriceListId AND 
				  @NewListCode <> ''
		)
		BEGIN  
		   -- Check if @PriceListId contain invalid price list id that not exists 
		   INSERT INTO ZnodePriceList( ListCode, ListName, CurrencyId,CultureId, ActivationDate, ExpirationDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
	       SELECT @NewListName, @NewListCode, @CurrencyId,@CultureId,
		   -- here check the Activation date and Expiration date is'1753-01-01' then its set it to be null
		   CASE WHEN @ActivationDate = '1753-01-01' THEN NULL ELSE @ActivationDate END,
           CASE WHEN @ExpirationDate = '1753-01-01' THEN NULL ELSE @ExpirationDate END, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
		   FROM ZnodePriceList
		   WHERE PriceListId = @PriceListId;
		   SET @NewPriceListId = @@Identity;
		END;

	
		IF @NewPriceListId > 0
		BEGIN
		BEGIN TRAN A;
		   -- Copy sku and quantity 
		   INSERT INTO ZnodePrice( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
		   SELECT @NewPriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
		   FROM ZnodePrice
		   WHERE PricelistId = @PriceListId; 

		   -- Copy tier price 
		   INSERT INTO ZnodePriceTier( PriceListId, SKU, Quantity, Price, UomId, UnitSize, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
		   SELECT @NewPriceListId, SKU, Quantity, Price, UomId, UnitSize, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
		   FROM ZnodePriceTier
		   WHERE PricelistId = @PriceListId;
	       -- If copy process will complete successfully then return status 1 
		   SELECT @NewPriceListId AS ID, CAST(1 AS bit) AS [Status];
		   SET @Status = 1;
		   COMMIT TRAN A;
		END;
		ELSE
		BEGIN
			-- If copy process will not complete successfully then return status 0 
			SELECT @NewPriceListId AS ID, CAST(0 AS bit) AS [Status];
			SET @Status = 0;
			
		END;
	END TRY
	BEGIN CATCH	
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_CopyPricingList @PriceListId = '+CAST(@PriceListId AS varchar(100))+' ,@NewListName='+@NewListName+' ,@NewListCode= '+@NewListCode+',@ActivationDate='+CAST(@ActivationDate AS varchar(200))+',@ExpirationDate='+CAST(@ExpirationDate AS varchar(200))+' ,@CurrencyId = '+CAST(@CurrencyId AS varchar(100))+' ,@CultureId = '+CAST(@CultureId AS varchar(100))+',@Status='+CAST(@Status AS varchar(200));
		SELECT @PriceListId AS ID, CAST(0 AS bit) AS [Status];
		SET @Status = 0;
		ROLLBACK TRAN A;
		EXEC Znode_InsertProcedureErrorLog
			 @ProcedureName = 'Znode_CopyPricingList', 
			 @ErrorInProcedure = @Error_procedure, 
			 @ErrorMessage = @ErrorMessage, 
			 @ErrorLine = @ErrorLine, 
			 @ErrorCall = @ErrorCall;
	END CATCH;
END;