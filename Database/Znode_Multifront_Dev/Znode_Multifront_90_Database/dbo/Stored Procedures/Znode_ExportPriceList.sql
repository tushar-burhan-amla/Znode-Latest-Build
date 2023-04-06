CREATE PROCEDURE [dbo].[Znode_ExportPriceList](
	  @PriceListId varchar(1000))
AS

/*
Summary :This procedure is used to get details of PriceList filtered by PriceListId
  Unit Testing 
  Exec [Znode_ExportPriceList] 4

*/

BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		SELECT ZP.[SKU], ZP.[RetailPrice], ZP.[SalesPrice], ZP.[ActivationDate] AS SKUActivationDate, ZP.[ExpirationDate] AS SKUExpirationDate, 
		ZPT.[Quantity] AS TierStartQuantity, ZPT.[Price] AS [TierPrice], ZPL.[PriceListId], ZPL.[ListCode] AS [PriceListCode]
		FROM [ZnodePriceList] AS ZPL
			 INNER JOIN
			 [ZnodePrice] AS ZP
			 ON(ZP.[PriceListId] = ZPL.[PriceListId])
			 LEFT JOIN
			 [ZnodePriceTier] AS ZPT
			 ON( ZPT.[PriceListId] = ZPL.[PriceListId] AND 
				 ZP.[SKU] = ZPT.[SKU]
			   )
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM [dbo].[Split]( @PriceListId, ',' ) AS aa
			WHERE aa.[Item] = ZPL.[PriceListId]
		);
	END TRY
	BEGIN CATCH
		DECLARE @Status bit;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_ExportPriceList @PriceListId = '+@PriceListId+',@Status='+CAST(@Status AS varchar(200));
		SET @Status = 0;
		SELECT 0 AS ID, CAST(0 AS bit) AS Status;

		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_ExportPriceList', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;