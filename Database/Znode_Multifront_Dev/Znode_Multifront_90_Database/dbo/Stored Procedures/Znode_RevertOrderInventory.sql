CREATE PROCEDURE [dbo].[Znode_RevertOrderInventory]
(   
	@OmsOrderDetailsId INT,
    @OmsOrderLineItemIds VARCHAR(2000) = '',
    @Status BIT OUT,
    @UserId INT,
	@IsRevertAll BIT = 0
)
AS 
   /* Summary: this proceedure is used to revert the order  inventory in case of order revert
      Unit Testing:
	  begin tran
	  EXEC  Znode_RevertOrderInventory 
      rollback tran
    */
BEGIN
BEGIN TRAN ZROI;
BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		DECLARE @Revert NVARCHAR(MAX), @SQL NVARCHAR(MAX)
		CREATE TABLE  #OmsOrderLineItemsId (OmsOrderLineItemsId INT);
		--Getting line item ids
		INSERT INTO #OmsOrderLineItemsId
		SELECT item
		FROM dbo.split(@OmsOrderLineItemIds, ',');

		--Getting parent line item ids for downloadable product
		INSERT INTO #OmsOrderLineItemsId
		SELECT ZOOLI.ParentOmsOrderLineItemsId 
		FROM ZnodeOmsOrderLineItems ZOOLI WITH (NOLOCK)
		INNER JOIN #OmsOrderLineItemsId OLI on ZOOLI.OmsOrderLineItemsId=OLI.OmsOrderLineItemsId
		WHERE EXISTS (SELECT * FROM ZnodeOmsDownloadableProductKey Z where Z.OmsOrderLineItemsId=ZOOLI.ParentOmsOrderLineItemsId)
		AND NOT EXISTS (SELECT * FROM #OmsOrderLineItemsId O where O.OmsOrderLineItemsId=ZOOLI.ParentOmsOrderLineItemsId)

		--Getting parent line item ids for downloadable product on basis of OmsOrderDetailsId
		INSERT INTO #OmsOrderLineItemsId
		SELECT ZOOLI.OmsOrderLineItemsId 
		FROM ZnodeOmsOrderLineItems ZOOLI WITH (NOLOCK)
		WHERE ZOOLI.OmsOrderDetailsId = @OmsOrderDetailsId and ZOOLI.OrderLineItemStateId=40
		AND EXISTS (SELECT * FROM ZnodeOmsDownloadableProductKey Z WHERE Z.OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId)
		AND NOT EXISTS (SELECT * FROM #OmsOrderLineItemsId O WHERE O.OmsOrderLineItemsId=ZOOLI.OmsOrderLineItemsId)

		SET @GetDate = dbo.Fn_GetDate();
		--Updating the used key for revert order of downloadable product
		IF EXISTS(SELECT * FROM #OmsOrderLineItemsId)
		BEGIN
			UPDATE ZnodePimDownloadableProductKey
			SET IsUsed = 0,	
				ModifiedDate= @GetDate
			WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeOmsDownloadableProductKey ZODPK 
				INNER JOIN #OmsOrderLineItemsId OOLI ON OOLI.OmsOrderLineItemsId= ZODPK.OmsOrderLineItemsId
				WHERE ZnodePimDownloadableProductKey.PimDownloadableProductKeyId = ZODPK.PimDownloadableProductKeyId)
		END
		
		--Reverting the quantity of produucts after return
		SET @SQL = '
			
		UPDATE ZI
		SET
			ZI.Quantity = ZI.Quantity + ZOOW.Quantity,
			ZI.MOdifiedBy = '''+CAST(@UserId AS VARCHAR(1000))+''',
			ZI.ModifiedDate = '''+CAST(@GetDate AS VARCHAR(1000))+'''
		FROM ZnodeOmsOrderWarehouse ZOOW
			INNER JOIN ZnodeOmsOrderLineItems ZOOLI ON(ZOOLI.OmsOrderLineItemsId = ZOOW.OmsOrderLineItemsId)
			INNER JOIN ZnodeInventory ZI ON(ZI.WarehouseId = ZOOW.WarehouseId
											AND ZI.SKU = ZOOLI.SKU)
		WHERE 
		ZOOLI.OmsOrderDetailsId = '+CAST(@OmsOrderDetailsId AS VARCHAR(1000))+'
			AND EXISTS
		(
			SELECT TOP 1 1     FROM #OmsOrderLineItemsId SP WHERE Sp.OmsOrderLineItemsId = ZOOLI.OmsOrderLineItemsId OR Sp.OmsOrderLineItemsId = ZOOLI.ParentOmsOrderLineItemsId OR '''+@OmsOrderLineItemIds+''' = ''''
		)  
		' +CASE WHEN @IsRevertAll = 0 THEN 'AND NOT EXISTS   (SELECT TOP  1 1 FROM ZnodeOmsOrderState OOS WHERE OOS.OrderStateName = ''RETURNED'' AND OOS.OmsOrderStateId = ZOOLI.OrderLineItemStateId) ' ELSE '' END

		EXEC(@SQL)

		SET @Status = 1;
		SELECT 1 ID, CAST(1 AS BIT) Status;

COMMIT TRAN ZROI;
END TRY
BEGIN CATCH
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_RevertOrderInventory @OmsOrderDetailsId = '+CAST(@OmsOrderDetailsId AS VARCHAR(200))+',@OmsOrderLineItemIds='+@OmsOrderLineItemIds+',@UserId='+CAST(@UserId AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(200));
    SET @Status = 0;
    SELECT 0 AS ID,
        CAST(0 AS BIT) AS Status;
	ROLLBACK TRAN ZROI;
    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_RevertOrderInventory',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;
END CATCH;
END;