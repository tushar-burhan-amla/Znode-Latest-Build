CREATE PROCEDURE [dbo].[Znode_DeleteInventoryList]
(
   @InventoryListId VARCHAR(300)
  ,@Status INT OUT
)
AS 
--Remove inventory list 
BEGIN 
	BEGIN TRY
	SET NOCOUNT ON; 
	BEGIN TRANSACTION A; 
		DECLARE @DeletdAttributeId TABLE ([InventoryListId] INT );
		INSERT INTO @DeletdAttributeId
		SELECT [Item] 
		FROM [dbo].[Split](@InventoryListId,',') a; 
	
		DELETE FROM [ZnodeInventory]  WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId DAI WHERE DAI.[InventoryListId] = [ZnodeInventory].[InventoryListId] );

		DELETE FROM [ZnodeInventoryWarehouse] WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId DAI WHERE DAI.[InventoryListId] = [ZnodeInventoryWarehouse].[InventoryListId] ); 

		DELETE FROM [ZnodeInventoryList] WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdAttributeId DAI WHERE DAI.[InventoryListId] = [ZnodeInventoryList].[InventoryListId] );
	 	
		IF (SELECT COUNT (1) FROM @DeletdAttributeId ) = (SELECT COUNT (1) FROM [dbo].[Split](@InventoryListId,',') a  )
		BEGIN	
			SELECT 1 [Id] , CAST(1 AS BIT ) [Status]; 
		END; 
		ELSE 
		BEGIN 
			SELECT 0 [Id] , CAST(0 AS BIT ) [Status]; 
		END; 
		SET @Status = 1; 
	COMMIT TRANSACTION A; 
 END TRY
 BEGIN  CATCH 
	 SELECT 0 [Id] , CAST(0 AS BIT ) [Status]; 
	 --SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE()
	 SET @Status = 0; 
	 ROLLBACK TRANSACTION A; 
 END CATCH;  
 END;