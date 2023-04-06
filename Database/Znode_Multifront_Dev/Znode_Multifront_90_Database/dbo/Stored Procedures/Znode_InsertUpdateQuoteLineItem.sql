CREATE PROCEDURE [dbo].[Znode_InsertUpdateQuoteLineItem]
(
	@OmsQuoteId			INT ,
	@UserId				INT = 0,
	@OmsSavedCartId		INT = 0,
	@Status				BIT OUT ,
	@SKUPriceForQuote [dbo].[SKUPriceForQuote] ReadOnly
)
AS
/* 
	Summary: This Procedure is used to save and edit the quote line item
	Unit Testing 
	Exec Znode_InsertUpdateQuoteLineItem 2000,1527,0
*/
BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @OmsQuoteLineItemId INT;
		DECLARE @TempOmsQuoteLineItem TABLE (OmsQuoteLineItemId INT , OmsSavedCartLineItemId INT , ParentOmsSavedCartLineItemId INT, Custom1 nvarchar(MAX), Custom2 nvarchar(MAX),
			Custom3	nvarchar(MAX),Custom4	nvarchar(MAX),Custom5	nvarchar(MAX))
		DECLARE @OrderLineItemRelationshipTypeIdBundle INT = (SELECT TOP 1 OrderLineItemRelationshipTypeId FROM ZnodeOmsOrderLineItemRelationshipType WHERE Name = 'Bundles')

		CREATE TABLE #ZnodeOmsSavedCartLineItem
		(
			OmsSavedCartLineItemId	int,
			ParentOmsSavedCartLineItemId	int,
			OmsSavedCartId	int,
			SKU	nvarchar(200),
			Quantity	numeric(28,	13) ,
			OrderLineItemRelationshipTypeId	int,
			CustomText	nvarchar(MAX),
			CartAddOnDetails	nvarchar(MAX),
			Sequence	int,
			CreatedBy	int,
			CreatedDate	datetime,
			ModifiedBy	int,
			ModifiedDate	datetime,
			AutoAddon	nvarchar(MAX),
			OmsOrderId	int,
			Custom1	nvarchar(MAX),
			Custom2	nvarchar(MAX),
			Custom3	nvarchar(MAX),
			Custom4	nvarchar(MAX),
			Custom5	nvarchar(MAX),
			GroupId	nvarchar(MAX),
			ProductName	nvarchar(2000),
			Description	nvarchar(MAX),
			Price numeric(28,6),
			ShippingCost numeric(28,6),
			InitialPrice numeric(28,6),
			InitialShippingCost numeric(28,6),
			IsPriceEdit bit
		)
			 
		IF (ISNULL(@OmsSavedCartId,0) <> 0)
		BEGIN
			INSERT INTO #ZnodeOmsSavedCartLineItem
				(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId,CustomText,
					CartAddOnDetails,Sequence,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,AutoAddon,OmsOrderId,Custom1,Custom2,Custom3,Custom4,
					Custom5,GroupId,ProductName,Description)
			SELECT a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId,a.OmsSavedCartId,a.SKU,a.Quantity,a.OrderLineItemRelationshipTypeId,
				a.CustomText,a.CartAddOnDetails,a.Sequence,a.CreatedBy,a.CreatedDate,a.ModifiedBy,a.ModifiedDate,a.AutoAddon,a.OmsOrderId,
				a.Custom1,a.Custom2,a.Custom3,a.Custom4,a.Custom5,a.GroupId,a.ProductName,a.Description 				
			FROM ZnodeOmsSavedCartLineItem a 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsQuoteLineItem ty WHERE ty.OmsQuoteId = @OmsQuoteId) and a.OmsSavedCartId=@OmsSavedCartId
		END
		ELSE
		BEGIN
			INSERT INTO #ZnodeOmsSavedCartLineItem
				(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId,CustomText,
					CartAddOnDetails,Sequence,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,AutoAddon,OmsOrderId,Custom1,Custom2,Custom3,
					Custom4,Custom5,GroupId,ProductName,Description)
			SELECT a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId,a.OmsSavedCartId,a.SKU,a.Quantity,a.OrderLineItemRelationshipTypeId,
				a.CustomText,a.CartAddOnDetails,a.Sequence,a.CreatedBy,a.CreatedDate,a.ModifiedBy,a.ModifiedDate,a.AutoAddon,a.OmsOrderId,
				a.Custom1,a.Custom2,a.Custom3,a.Custom4,a.Custom5,a.GroupId,a.ProductName,a.Description 
			FROM ZnodeOmsSavedCartLineItem a 
			INNER JOIN ZnodeOmsSavedCart b ON (b.OmsSavedCartId = a.OmsSavedCartId)
			INNER JOIN ZnodeOmsCookieMapping c ON (c.OmsCookieMappingId = b.OmsCookieMappingId)
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsQuoteLineItem ty WHERE ty.OmsQuoteId = @OmsQuoteId) 
			AND c.UserId = @UserId
		END
			
		UPDATE ZOSLI set ZOSLI.Price = SPQ.Price, ZOSLI.ShippingCost = SPQ.ShippingCost, ZOSLI.InitialPrice = SPQ.InitialPrice,
		ZOSLI.InitialShippingCost = SPQ.InitialShippingCost, ZOSLI.IsPriceEdit = SPQ.IsPriceEdit,
		ZOSLI.Custom1= SPQ.Custom1, ZOSLI.Custom2 = SPQ.Custom2, ZOSLI.Custom3 = SPQ.Custom3 , ZOSLI.Custom4 = SPQ.Custom4 , ZOSLI.Custom5 = SPQ.Custom5
		FROM #ZnodeOmsSavedCartLineItem ZOSLI
		INNER JOIN @SKUPriceForQuote SPQ ON ZOSLI.OmsSavedCartLineItemId = SPQ.OmsSavedCartLineItemId

		--To update price and ShippingCost for child entries of bundle product
		UPDATE ZOSLI set ZOSLI.Price = SPQ.Price, ZOSLI.ShippingCost = SPQ.ShippingCost, ZOSLI.InitialPrice = SPQ.InitialPrice,
		ZOSLI.InitialShippingCost = SPQ.InitialShippingCost, ZOSLI.IsPriceEdit= SPQ.IsPriceEdit,
		ZOSLI.Custom1= SPQ.Custom1, ZOSLI.Custom2 = SPQ.Custom2, ZOSLI.Custom3 = SPQ.Custom3 , ZOSLI.Custom4 = SPQ.Custom4 , ZOSLI.Custom5 = SPQ.Custom5
		FROM #ZnodeOmsSavedCartLineItem ZOSLI
		INNER JOIN @SKUPriceForQuote SPQ ON ZOSLI.ParentOmsSavedCartLineItemId = SPQ.OmsSavedCartLineItemId
		WHERE ZOSLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle


		IF OBJECT_ID('Tempdb..#desupdate1') IS NOT NULL
		DROP TABLE Tempdb..#desupdate1 
		SELECT ZnodeProductId,des
		INTO #desupdate1 
		FROM 
		(
			SELECT ZnodeProductId, CONCAT( 'Qty: ',cast(cast(AssociatedProductBundleQuantity as int) as varchar(5)),'| ',sku,' - ', 
				productname, CHAR(10) ) des
			FROM ZnodePublishBundleProductEntity ZPBPE1
			inner join ZnodePublishProductDetail ZPP on ZPBPE1.AssociatedZnodeProductId = ZPP.PublishProductId
		) asd GROUP BY ZnodeProductId, des


		IF OBJECT_ID('Tempdb..#desupdate') IS NOT NULL
		DROP TABLE Tempdb..#desupdate 

		SELECT ZPBPE.des, asd.* 
		INTO #desupdate 
		FROM
		(
			SELECT VLMP.PimProductId PPI , ZOSCL.* 
			FROM #ZnodeOmsSavedCartLineItem ZOSCL 
			INNER JOIN [dbo].[View_LoadManageProduct] VLMP ON VLMP.AttributeCode ='SKU' AND VLMP.AttributeValue= ZOSCL.sku
			INNER JOIN (SELECT * FROM ZnodepimAttributevalue WHERE PimAttributeId = 10 AND PimAttributeValueId IN (
							SELECT PimAttributeValueId FROM ZnodePimProductAttributeDefaultValue WHERE PimAttributeDefaultValueId in (
							SELECT PimAttributeDefaultValueId FROM ZnodePimAttributeDefaultValue WHERE AttributeDefaultValueCode = 'BundleProduct' ))) BP
										ON BP.PimProductId = VLMP.PimProductId 
		) asd
		CROSS APPLY
		(
			SELECT *
			FROM (SELECT ZnodeProductId, '<p><span style="font-family: Arial, Helvetica, sans-serif; font-size: 10px;">' +
				(SELECT des +' </br> '
								FROM #desupdate1 AS p 
								WHERE p.ZnodeProductId = a.ZnodeProductId
								FOR XML PATH(''))
							
			+ '</span></p>' AS des
						FROM #desupdate1 a
						GROUP BY ZnodeProductId) ZPBPE1
			INNER JOIN ZnodePublishProduct ZPP on ZPBPE1.ZnodeProductId = ZPP.PublishProductId
			WHERE Zpp.PimProductId = asd.PPI
		) 
			ZPBPE

		UPDATE ZOSLI set ZOSLI.Description = SPQ.des
		FROM #ZnodeOmsSavedCartLineItem ZOSLI
		INNER JOIN #desupdate SPQ ON ZOSLI.OmsSavedCartLineItemId = SPQ.OmsSavedCartLineItemId or ZOSLI.ParentOmsSavedCartLineItemId = SPQ.OmsSavedCartLineItemId

		
		MERGE INTO ZnodeOmsQuoteLineItem TARGET 
		USING #ZnodeOmsSavedCartLineItem SOURCE 
		ON (1=0 )
		WHEN NOT MATCHED THEN 
		INSERT 
			(ParentOmsQuoteLineItemId,OmsQuoteId,SKU,Quantity,OrderLineItemRelationshipTypeId,CustomText,CartAddOnDetails,Sequence,
				CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,GroupId,ProductName,Description, Price, ShippingCost,InitialPrice, 
				InitialShippingCost, IsPriceEdit, Custom1, Custom2, Custom3, Custom4, Custom5)
			 
		VALUES (NULL,@OmsQuoteId,SOURCE.SKU,SOURCE.Quantity,SOURCE.OrderLineItemRelationshipTypeId
					,SOURCE.CustomText,SOURCE.CartAddOnDetails,SOURCE.Sequence,SOURCE.CreatedBy,SOURCE.CreatedDate,SOURCE.ModifiedBy,
					SOURCE.ModifiedDate,SOURCE.GroupId,SOURCE.ProductName,SOURCE.Description, SOURCE.Price, SOURCE.ShippingCost,
					SOURCE.InitialPrice, SOURCE.InitialShippingCost, SOURCE.IsPriceEdit, SOURCE.Custom1, SOURCE.Custom2, SOURCE.Custom3, 
					SOURCE.Custom4, SOURCE.Custom5) 
			
		OUTPUT Inserted.OmsQuoteLineItemId , SOURCE.OmsSavedCartLineItemId,Source.ParentOmsSavedCartLineItemId, Source.Custom1, Source.Custom2, Source.Custom3, Source.Custom4, Source.Custom5
		INTO @TempOmsQuoteLineItem ;

		UPDATE a
		SET a.ParentOmsQuoteLineItemId =( SELECT TOP 1 b1.OmsQuoteLineItemId FROM @TempOmsQuoteLineItem b1 WHERE b.ParentOmsSavedCartLineItemId = b1.OmsSavedCartLineItemId)  
		FROM ZnodeOmsQuoteLineItem a
		INNER JOIN @TempOmsQuoteLineItem b ON (b.OmsQuoteLineItemId = a.OmsQuoteLineItemId)
		WHERE b.ParentOmsSavedCartLineItemId IS NOT NULL

		INSERT INTO ZnodeOmsQuotePersonalizeItem 
			(OmsQuoteLineItemId,PersonalizeCode,PersonalizeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DesignId,ThumbnailURL)
		SELECT b.OmsQuoteLineItemId,PersonalizeCode,PersonalizeValue,a.CreatedBy,a.CreatedDate,a.ModifiedBy,a.ModifiedDate
			,DesignId,ThumbnailURL
		FROM ZnodeOmsPersonalizeCartItem a
		INNER JOIN @TempOmsQuoteLineItem b ON (b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
			
		SET @Status = 1;
		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateQuoteLineItem @CartLineItemXML = '+CAST('' AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
		 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;
		ROLLBACK TRAN A;
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_InsertUpdateQuoteLineItem',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;