CREATE PROCEDURE Znode_DeleteInventory
(
	@InventoryId VARCHAR(2000),
	@Status int OUT
)
AS
BEGIN
BEGIN TRY
		SET NOCOUNT ON;
		BEGIN TRAN A;
		DECLARE @TBL_DeletdInventoryId TABLE
		( 
			InventoryId int
		);
		DECLARE @TBL_DeletdPimDownloadableProductKeyid TABLE
		( 
			PimDownloadableProductKeyId int
		);
		INSERT INTO @TBL_DeletdInventoryId
			   SELECT Item
			   FROM dbo.split( @InventoryId, ',' ) AS a;

		INSERT INTO @TBL_DeletdPimDownloadableProductKeyid
		SELECT ZPDPK.PimDownloadableProductKeyId from ZnodePimDownloadableProductKey ZPDPK WHERE PimDownloadableProductId IN 
		(
			SELECT PimDownloadableProductId FROM ZnodePimDownloadableProduct ZPDP
			INNER JOIN ZnodeInventory ZI ON ZPDP.SKU = ZI.SKU
			WHERE ZI.InventoryId IN  (SELECT Item from Split(@InventoryId, ','))
		)
		DELETE FROM ZnodeInventory
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM @TBL_DeletdInventoryId AS a
			WHERE a.InventoryId = ZnodeInventory.InventoryId
		);
		DELETE FROM ZnodePimDownloadableProductKey
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM @TBL_DeletdPimDownloadableProductKeyid AS da
			WHERE da.PimDownloadableProductKeyId = ZnodePimDownloadableProductKey.PimDownloadableProductKeyId
			AND IsUsed = 0
		);

		IF
		(
			SELECT COUNT(1)
			FROM @TBL_DeletdInventoryId
		) =
		(
			SELECT COUNT(1)
			FROM dbo.split( @InventoryId, ',' ) AS a
		)
		BEGIN
			SELECT 1 AS ID, CAST(1 AS bit) AS Status;
		END;
		ELSE
		BEGIN
			SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		END;
		SET @Status = 1;
		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
		 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteInventory @InventoryId = '+@InventoryId+',@Status='+CAST(@Status AS
 VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteInventory',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
	END CATCH;
	
END