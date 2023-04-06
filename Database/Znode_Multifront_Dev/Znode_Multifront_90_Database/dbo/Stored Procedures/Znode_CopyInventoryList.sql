

CREATE Procedure [dbo].[Znode_CopyInventoryList] 
(
	@InventoryListId    INT, 
	@UserId				INT,
	@ListName			VARCHAR(500),
	@ListCode			VARCHAR(500), 
	@Status				BIT OUT 	 
)
AS 
-----------------------------------------------------------------------------
--Summary: Copy inventory list - Create clone of existing inventory list 
--		   Inventory list name can change and will copy all existing inventory details from 
--		   table ZnodeInventory and inserted with new list name.

--Unit Testing   

--EXEC [Znode_CopyInventory] 17,1
----------------------------------------------------------------------------- 
BEGIN 
BEGIN TRAN  A 
BEGIN TRY 
SET NOCOUNT ON 

	  DECLARE @NewInventoryListId  INT 

	  INSERT INTO ZnodeInventoryList (ListCode,ListName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @ListName,@ListCode,@UserId,GETUTCDATE(),@UserId,GETUTCDATE() 

	  SET @NewInventoryListId = Scope_identity ()
	  
	  INSERT INTO ZnodeInventory (InventoryListId,SKU,Quantity,ReOrderLevel,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
         SELECT @NewInventoryListId,SKU,Quantity,ReOrderLevel,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate FROM ZnodeInventory WHERE InventoryListId = @InventoryListId

  	SELECT 0 ID , CAST(1 AS BIT ) STATUS 
	SET @Status = 1 
COMMIT TRAN A
END TRY 
BEGIN CATCH 
	--SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE()
	SELECT 0 ID , CAST( 0 As BIT ) Status 
	SET @Status = 0
	ROLLBACK TRAN A
END CATCH  

END