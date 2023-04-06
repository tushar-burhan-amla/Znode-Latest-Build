CREATE PROCEDURE [dbo].[Znode_UpdatePimDownloadableProductKey]
(   
	@OrderLineItemDataModel XML,
	@Status INT = 0 OUT
)
AS
  /* 
	Summary :- This Procedure is used to update the Znode_UpdatePimDownloadableProductKey 
				
	
   */
BEGIN 
BEGIN TRY
BEGIN TRAN UpdatePimDownloadableProductKey;
SET NOCOUNT ON;
	CREATE TABLE #SKU (Sku INT);
			 
	IF OBJECT_ID('tempdb..#SkuQuantityUpdate') IS NOT NULL 
		DROP TABLE #SkuQuantityUpdate
    --Getting xml data into table
	SELECT  Tbl.Col.value ( 'OmsOrderLineItemsId[1]' , 'INT') AS OmsOrderLineItemsId
		,Tbl.Col.value ( 'ParentOmsOrderLineItemsId[1]' , 'INT') AS ParentOmsOrderLineItemsId,
		Tbl.Col.value ( 'Sku[1]' , 'NVARCHAR(max)') AS Sku
		,Tbl.Col.value ( 'Quantity[1]' , 'Numeric(28,6)') AS Quantity
	INTO #SkuQuantityUpdate
	FROM @OrderLineItemDataModel.nodes ( '//ArrayOfOrderLineItemDataModel/OrderLineItemDataModel'  ) AS Tbl(Col)

	ALTER TABLE #SkuQuantityUpdate ADD OmsLineItemId INT,PimDownloadableProductKeyId INT;

	UPDATE #SkuQuantityUpdate
	SET OmsLineItemId = OmsOrderLineItemsId;

	--Updating the ParentOmsOrderLineItemsId
	UPDATE OLI
	SET OmsLineItemId =  ZOOLI.ParentOmsOrderLineItemsId,PimDownloadableProductKeyId=Z.PimDownloadableProductKeyId
	FROM ZnodeOmsOrderLineItems ZOOLI WITH (NOLOCK)
	INNER JOIN #SkuQuantityUpdate OLI ON ZOOLI.OmsOrderLineItemsId=OLI.OmsOrderLineItemsId
	INNER JOIN ZnodeOmsDownloadableProductKey Z WITH (NOLOCK) ON Z.OmsOrderLineItemsId=ZOOLI.ParentOmsOrderLineItemsId
	WHERE NOT EXISTS (SELECT * FROM #SkuQuantityUpdate O WHERE O.OmsOrderLineItemsId=ZOOLI.ParentOmsOrderLineItemsId)

    IF OBJECT_ID('tempdb..#FinalProductKey') IS NOT NULL 
		DROP TABLE #FinalProductKey
		
	--Getting the product key of downloadable products
	SELECT ZPDPK.PimDownloadableProductKeyId,S.Quantity, RANK() OVER(PARTITION BY ZPDPK.PimDownloadableProductKeyId ORDER BY ZPDPK.PimDownloadableProductKeyId DESC) RNK  
	INTO #FinalProductKey 
	FROM ZnodePimDownloadableProductKey ZPDPK WITH (NOLOCK)
	INNER JOIN ZnodePimDownloadableProduct ZPDP WITH (NOLOCK) ON ZPDP.PimDownloadableProductId = ZPDPK.PimDownloadableProductId
	INNER JOIN #SkuQuantityUpdate S ON  ZPDP.SKU=S.Sku AND S.PimDownloadableProductKeyId = ZPDPK.PimDownloadableProductKeyId

	--Update IsUsed for downloadable products for return 
	UPDATE ZnodePimDownloadableProductKey
	SET IsUsed = 0		
	WHERE PimDownloadableProductKeyId IN (SELECT ISNULL(PimDownloadableProductKeyId,0) FROM #FinalProductKey WHERE RNK <= Quantity )

	SET @Status = 1
	SELECT 1 AS ID,CAST(@Status AS BIT) AS Status; 

COMMIT TRAN UpdatePimDownloadableProductKey;
END TRY
BEGIN CATCH
           	     
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UpdatePimDownloadableProductKey @OrderLineItemDataModel = '+ cast(@OrderLineItemDataModel as varchar(2000));
    SET @Status = 0          			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	ROLLBACK TRAN UpdatePimDownloadableProductKey;	  
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_UpdatePimDownloadableProductKey',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END;