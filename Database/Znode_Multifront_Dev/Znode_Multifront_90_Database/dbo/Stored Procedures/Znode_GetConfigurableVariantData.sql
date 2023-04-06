CREATE PROCEDURE [dbo].[Znode_GetConfigurableVariantData]
(   
	@PortalId         INT,
	@UserId			  INT = 2,
	@catalogVersionId int, 
	@currentUtcDate    VARCHAR(200) = '',
	@publishProductId   int

)

AS 
     BEGIN
         BEGIN TRY
		  DECLARE @PublishProductIds  NVARCHAR(max) ,@SKU NVARCHAR(max) 
	
		       CREATE TABLE #Tlb_ProductData 
				(
				  ZnodeProductId INT,
				  SKU              NVARCHAR(100),
				  Name NVARCHAR(100),
				  Attributes nvarchar(max),
				  IsActive bit,
				  AssociatedProductDisplayOrder int,
				  ConfigurableAttributeCodes nvarchar(max)
			     );
				Create TABLE #Tbl_PriceList
                (
				  SKU            VARCHAR(300),
				  RetailPrice    NUMERIC(28, 6),
				  SalesPrice     NUMERIC(28, 6),
				  TierPrice      NUMERIC(28, 6),
				  TierQuantity   NUMERIC(28, 6),
				  CurrencyCode   Varchar(100),
				  CurrencySuffix Varchar(1000),
				  CultureCode      VARCHAr(1000),
				  ExternalId NVARCHAR(2000),
				  Custom1        NVARCHAR(MAX),
				  Custom2        NVARCHAR(MAX),
				  Custom3        NVARCHAR(MAX)
                );

				Create TABLE #Tbl_Inventory
				(
				  SKU            VARCHAR(300),
				  Quantity    NUMERIC(28, 6),
				  ReOrderLevel     NUMERIC(28, 6),
				  PortalId      int,
				  WarehouseName	varchar(100),
			      WarehouseCode	varchar(100),
			      DefaultInventoryCount varchar(1000),
				
			    );

				 insert into #Tlb_ProductData (ZnodeProductId, SKU, Name, Attributes, IsActive, AssociatedProductDisplayOrder,ConfigurableAttributeCodes)
				 select DISTINCT ZPE.ZnodeProductId, ZPE.SKU, ZPE.Name,zpe.Attributes, zpe.IsActive, ZPCE.AssociatedProductDisplayOrder, ZPCE.ConfigurableAttributeCodes from ZnodePublishProductEntity as ZPE
				 inner join ZnodePublishConfigurableProductEntity as ZPCE on ZPCE.AssociatedZnodeProductId = ZPE.ZnodeProductId
				 where ZPCE.ZnodeProductId = @publishProductId and ZPCE.VersionId = @catalogVersionId and ZPE.VersionId = @catalogVersionId and ZPE.IsActive = 1

				SELECT @SKU = Substring((SELECT ',' + SKU FROM #Tlb_ProductData FOR XML PAth('')), 2, 4000)

				INSERT INTO #Tbl_PriceList( SKU, RetailPrice,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode,ExternalId,Custom1,Custom2,Custom3)
				EXEC Znode_GetPublishProductPricingBySku @SKU = @SKU ,@PortalId = @PortalId ,@currentUtcDate = @currentUtcDate,@UserId = @UserId
		
				insert into #Tbl_Inventory (SKU,	Quantity,	ReOrderLevel,	PortalId, WarehouseName, WarehouseCode, DefaultInventoryCount)
				EXEC Znode_GetInventoryBySkus @SKUs=@SKU,@PortalId=@PortalId

				select DISTINCT PD.ZnodeProductId AS PublishProductId, PD.SKU, PD.Name,PD.Attributes, PD.IsActive, PD.AssociatedProductDisplayOrder,PD.ConfigurableAttributeCodes,
				pl.RetailPrice, pl.SalesPrice, pl.CultureCode, pl.CurrencySuffix, tl.Quantity, tl.ReOrderLevel, tl.DefaultInventoryCount, tl.WarehouseName
				from #Tlb_ProductData as PD 
				left join #Tbl_PriceList as PL on pd.SKU = pl.SKU
				left join #Tbl_Inventory as TL on pd.SKU = tl.sku
		END TRY 
			

		 BEGIN CATCH

		 	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
			@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetConfigurableVariantData
			@PortalId = '+CAST(@PortalId AS int)+',
			@UserId='+CAST(@UserId AS int)+',
			@catalogVersionId='+CAST(@catalogVersionId AS int)+',
			@currentUtcDate='+CAST(@currentUtcDate AS VARCHAR(200))+',
			@publishProductId='+CAST(@publishProductId AS int);

             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetConfigurableVariantData',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		  END CATCH
	End