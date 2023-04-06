CREATE PROCEDURE [dbo].[Znode_UpdateInventory]
(
	@SkuXml xml,
	@PortalId int, 
	@UserId int, 
	@Status bit OUT, 
	@IsDebug bit= 0
)
AS 
	/*
	Summary: (13671)   Inventory will be updated on the basis of "InventoryTracking" (Attribute Code) value in locale table 
				    If value id "DisablePurchasing" then subtracts currently selected quantity only when it will greater than zero.
				    If "AllowBackordering" then inventory will become negative
				    If "DontTrackInventory" then don't update inventory.
				    inventory will be get deducted from associated warehouse where quantity is available as per warehouse precedence. 
				    Validate total quantity 
	Input Parameters:
	SKU(Comma separated multiple), PortalId
	Unit Testing   
		Declare @Status bit 
		Exec Znode_UpdateInventory  @SkuXml = '<ArrayOfOrderWarehouseLineItemsModel>
		<OrderWarehouseLineItemsModel>
		<OrderLineItemId>418</OrderLineItemId>
		<SKU>ap1534</SKU>
		<InventoryTracking>DisablePurchasing</InventoryTracking>
		<Quantity>1.000000</Quantity>
		</OrderWarehouseLineItemsModel>
		<OrderWarehouseLineItemsModel>
		<OrderLineItemId>419</OrderLineItemId>
		<SKU>al8907</SKU>
		<InventoryTracking>DisablePurchasing</InventoryTracking>
		<Quantity>1.000000</Quantity>
		</OrderWarehouseLineItemsModel>
		</ArrayOfOrderWarehouseLineItemsModel>',@PortalId = 1,@UserId = 2,@Status = @Status

	*/
BEGIN
	BEGIN TRAN UpdateInventory;
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @RoundOffValue INT= dbo.Fn_GetDefaultValue('InventoryRoundOff');
		DECLARE @TBL_XmlReturnToTable TABLE
		( 
			OrderLineItemId int, SKU nvarchar(max), InventoryTracking nvarchar(1000), Quantity numeric(28, 6),
			AllowBackOrder VARCHAR(100), SequenceNo int IDENTITY, OrderLineItemRelationshipTypeId int 
		);
		DECLARE @TBL_ErrorInventoryTracking TABLE
		( 
			SKU nvarchar(max), Quantity numeric(28, 6), InventoryTracking nvarchar(2000), AllowBackOrder varchar(100)
		);
		INSERT INTO @TBL_XmlReturnToTable( OrderLineItemId, SKU, InventoryTracking, Quantity,AllowBackOrder )
			   SELECT Tbl.Col.value( 'OrderLineItemId[1]', 'INT' ) AS OrderLineItemId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'InventoryTracking[1]', 'NVARCHAR(2000)' ) AS InventoryTracking, Tbl.Col.value( 'Quantity[1]', 'Numeric(28,6)' ) AS Quantity
			   , Tbl.Col.value( 'AllowBackOrder[1]', 'VARCHAR(100)' ) AS AllowBackOrder
			   FROM @SkuXml.nodes( '//ArrayOfOrderWarehouseLineItemsModel/OrderWarehouseLineItemsModel' ) AS Tbl(Col)
			   WHERE Tbl.Col.value( 'InventoryTracking[1]', 'NVARCHAR(2000)' ) <> 'DontTrackInventory' 
			   AND Tbl.Col.value( 'Quantity[1]', 'Numeric(28,6)' ) > 0
			   ;
		DECLARE @Cur_WarehouseId int, @Cur_PortalId int, @Cur_PortalWarehouseId int, @Cur_WarehouseSequence int, @Cur_SKU varchar(200), @Cur_Quantity numeric(28, 6), @Cur_ReOrderLevel numeric(28, 6), @Cur_InventoryId int;

		DECLARE @RecurringQuantity numeric(28, 6), @BalanceQuantity numeric(28, 6)
		SET @Status = 0; 
   
		Update X SET x.OrderLineItemRelationshipTypeId = b.OrderLineItemRelationshipTypeId 
		FROM ZnodeOmsOrderLineItems a
		inner join ZnodeOmsOrderLineItems b on a.OmsOrderLineItemsId = b.ParentOmsOrderLineItemsId 
		inner join @TBL_XmlReturnToTable X on x.OrderLineItemId = a.OmsOrderLineItemsId
		where a.ParentOmsOrderLineItemsId is null
		and b.ParentOmsOrderLineItemsId is not null


		DECLARE @TBL_CalculateQuntity TABLE
		( 
			WarehouseId int, SKU varchar(200), MainQuantity numeric(28, 6), InventoryId int, OrderQuantity numeric(28, 6), UpdatedQuantity numeric(28, 6), WarehouseSequenceId int, InventoryTracking nvarchar(2000)
			, AllowBackOrder varchar(100)
		);
		
		DECLARE @TBL_AllwareHouseToportal TABLE
		( 
			WarehouseId int, PortalId int, PortalWarehouseId int, WarehouseSequenceFirst int, WarehouseSequence int, SKU varchar(200), Quantity numeric(28, 6), ReOrderLevel numeric(28, 6), InventoryId int
		);
		
	
		INSERT INTO @TBL_AllwareHouseToportal( WarehouseId, PortalId, PortalWarehouseId, SKU, Quantity, ReOrderLevel, InventoryId, WarehouseSequence, WarehouseSequenceFirst )
			SELECT ZPW.WarehouseId, ZPW.PortalId, zpw.PortalWarehouseId, zi.SKU, zi.Quantity, zi.ReOrderLevel, zi.InventoryId, DENSE_RANK() OVER(ORDER BY ZPW.WarehouseId), 1
			FROM dbo.ZnodeInventory AS zi
			LEFT JOIN [ZnodePortalWarehouse] AS ZPW ON ZPW.WarehouseId = ZI.WareHouseId AND  ZPW.PortalId = @PortalId
			WHERE EXISTS ( SELECT TOP 1 1  FROM @TBL_XmlReturnToTable AS TBXRT WHERE RTRIM(LTRIM(TBXRT.SKU)) = RTRIM(LTRIM(zi.SKU))) 
			AND ZPW.WarehouseId IS NOT NULL
			ORDER BY ZPW.Precedence DESC
			
			INSERT INTO @TBL_AllwareHouseToportal( WarehouseId, PortalId, PortalWarehouseId, SKU, Quantity, ReOrderLevel, InventoryId, WarehouseSequence, WarehouseSequenceFirst )
			SELECT zpaw.WarehouseId, @PortalId, zpaw.PortalWarehouseId, zi.SKU, zi.Quantity, zi.ReOrderLevel, zi.InventoryId, DENSE_RANK() OVER (ORDER BY zpaw.WarehouseId),
			CASE WHEN EXISTS ( SELECT TOP 1 1 FROM @TBL_AllwareHouseToportal WE WHERE WE.SKU = ZI.SKU  ) THEN 2 ELSE 1 END
			FROM dbo.ZnodeInventory AS zi 
			INNER JOIN [dbo].[ZnodePortalAlternateWarehouse] AS zpaw ON zpaw.WarehouseId = ZI.WareHouseId
			WHERE EXISTS ( SELECT TOP 1 1 FROM [ZnodePortalWarehouse] AS a WHERE zpaw.PortalWarehouseId = a.PortalWarehouseId AND  a.PortalId = @PortalId) AND 
			EXISTS ( SELECT TOP 1 1 FROM @TBL_XmlReturnToTable AS TBXRT WHERE RTRIM(LTRIM(TBXRT.SKU)) = RTRIM(LTRIM(zi.SKU))) ORDER BY ZPAW.Precedence DESC;

		--Total Avaialble qunatity in all warehouse 
		IF EXISTS ( SELECT TOP 1 1 FROM @TBL_XmlReturnToTable WHERE InventoryTracking = 'DisablePurchasing')
		BEGIN
			INSERT INTO @TBL_ErrorInventoryTracking 
			SELECT TBAHL.SKU, SUM(TBAHL.Quantity), InventoryTracking,TBXML.AllowBackOrder 
			FROM @TBL_XmlReturnToTable AS TBXML
			LEFT JOIN @TBL_AllwareHouseToportal AS TBAHL ON(TBAHL.SKU = TBXML.SKU) 
			INNER JOIN ZnodeOmsOrderLineItems b on (TBXML.orderlineitemid = b.OmsOrderLineItemsId)
			WHERE InventoryTracking = 'DisablePurchasing'
			AND b.OrderLineItemRelationshipTypeId IS NOT NULL
			GROUP BY TBXML.AllowBackOrder ,TBAHL.SKU, InventoryTracking HAVING SUM(TBAHL.Quantity) < 1 ORDER BY TBAHL.SKU;
			IF EXISTS ( SELECT TOP 1 1 FROM @TBL_ErrorInventoryTracking )
			BEGIN
				SET @Status = 0;
				RAISERROR(15600, -1, -1, 'DisablePurchasing');
			END;
		END;

		---Getting data for simple , configurable and group
		INSERT INTO @TBL_CalculateQuntity
		SELECT WarehouseId, TBXML.SKU, TBAHL.Quantity, TBAHL.InventoryId, TBXML.Quantity, NULL AS UpdatedQuantity, DENSE_RANK() 
		OVER(ORDER BY WarehouseSequenceFirst,WarehouseSequence), InventoryTracking, TBXML.AllowBackOrder
		FROM @TBL_XmlReturnToTable AS TBXML	
		INNER  JOIN @TBL_AllwareHouseToportal AS TBAHL ON(TBAHL.SKU = TBXML.SKU)
		where exists(select * from ZnodeOmsOrderLineItems ZOOLI where TBXML.OrderLineItemId = ZOOLI.OmsOrderLineItemsId
			and ZOOLI.ParentOmsOrderLineItemsId is not null) 
		AND isnull(TBXML.OrderLineItemRelationshipTypeId,0) <> (Select top 1 OrderLineItemRelationshipTypeId From ZnodeOmsOrderLineItemRelationshipType Where Name IN ('Bundles','Configurable','Group'))
		ORDER BY  WarehouseSequenceFirst,WarehouseSequence;

		---Getting data for bundle products
		INSERT INTO @TBL_CalculateQuntity
		SELECT WarehouseId, TBXML.SKU, TBAHL.Quantity, TBAHL.InventoryId, TBXML.Quantity, NULL AS UpdatedQuantity, DENSE_RANK() 
		OVER(ORDER BY WarehouseSequenceFirst,WarehouseSequence), InventoryTracking, TBXML.AllowBackOrder
		FROM @TBL_XmlReturnToTable AS TBXML	
		INNER  JOIN @TBL_AllwareHouseToportal AS TBAHL ON(TBAHL.SKU = TBXML.SKU)
		where exists(select * from ZnodeOmsOrderLineItems ZOOLI where TBXML.OrderLineItemId = ZOOLI.OmsOrderLineItemsId and ZOOLI.ParentOmsOrderLineItemsId is null)
		AND TBXML.OrderLineItemRelationshipTypeId = (Select top 1 OrderLineItemRelationshipTypeId From ZnodeOmsOrderLineItemRelationshipType Where Name IN ('Bundles','Configurable','Group'))
		ORDER BY  WarehouseSequenceFirst,WarehouseSequence;

		;with cte as
		(
			select WarehouseId,	a.SKU,	MainQuantity,	InventoryId,	WarehouseSequenceId,	a.InventoryTracking,	AllowBackOrder,sum(OrderQuantity) as OrderQuantity
			from @TBL_CalculateQuntity A
			group by WarehouseId,	a.SKU,	MainQuantity,	InventoryId,	WarehouseSequenceId,	InventoryTracking,	AllowBackOrder
		)
		update b set  b.OrderQuantity = a.OrderQuantity
		from cte a
		inner join @TBL_CalculateQuntity b on a.SKU = b.SKU and a.InventoryId = b.InventoryId and a.WarehouseId = b.WarehouseId

		
		UPDATE @TBL_CalculateQuntity 
		SET UpdatedQuantity = MainQuantity - OrderQuantity  
		WHERE WarehouseSequenceId = 1;
		 		
		DECLARE @CountToRepeate int= ( SELECT count( distinct WarehouseId) FROM @TBL_CalculateQuntity),@Initializationofloop int= 2;
       
		WHILE @Initializationofloop <= @CountToRepeate 
				AND EXISTS (SELECT TOP 1 1 FROM @TBL_CalculateQuntity AS a 
					WHERE UpdatedQuantity < 0  )
		BEGIN
		 
			UPDATE a  
			SET a.UpdatedQuantity = a.MainQuantity + b.UpdatedQuantity 
			FROM @TBL_CalculateQuntity a 
			INNER JOIN @TBL_CalculateQuntity b ON (a.Sku = b.Sku AND b.WarehouseSequenceId = (@Initializationofloop - 1)) 
			WHERE a.WarehouseSequenceId = @Initializationofloop 
			AND b.UpdatedQuantity < 0
			AND b.UpdatedQuantity IS NOT NULL 
			--AND a.WarehouseSequenceId = @Initializationofloop - 1

			UPDATE @TBL_CalculateQuntity 
			SET UpdatedQuantity  = 0 
			WHERE WarehouseSequenceId = @Initializationofloop -1 
			--AND  InventoryTracking <> 'AllowBackordering'
			AND UpdatedQuantity < 0
			AND UpdatedQuantity IS NOT NULL 
			--AND WarehouseSequenceId = @Initializationofloop - 1


			SET @Initializationofloop = @Initializationofloop + 1;
		END; 
		
		
		   IF EXISTS (SELECT TOP 1 1 FROM @TBL_CalculateQuntity WHERE ISNULL(UpdatedQuantity,0) < 0 AND @CountToRepeate >1)
		   BEGIN 
		    UPDATE a  
			SET a.UpdatedQuantity = b.UpdatedQuantity 
			FROM @TBL_CalculateQuntity a 
			INNER JOIN @TBL_CalculateQuntity b ON (a.Sku = b.Sku AND b.WarehouseSequenceId = @Initializationofloop -1 ) 
			WHERE a.WarehouseSequenceId = 1 
			AND ISNULL(b.UpdatedQuantity,0) < 0

			UPDATE @TBL_CalculateQuntity 
			SET UpdatedQuantity  = 0 
			WHERE WarehouseSequenceId = @Initializationofloop -1 
			--AND  InventoryTracking <> 'AllowBackordering'
		    AND ISNULL(UpdatedQuantity,0) < 0
		   END 
			
			
		--If "AllowBackordering" then inventory will go to be negative conside only single warehouse
		IF EXISTS(SELECT TOP 1 1 FROM @TBL_XmlReturnToTable WHERE InventoryTracking = 'AllowBackordering')
		BEGIN
			UPDATE ZI SET Quantity = Isnull(TBCQ.UpdatedQuantity,0) 
			FROM dbo.ZnodeInventory ZI 
			INNER JOIN @TBL_CalculateQuntity TBCQ ON(Zi.InventoryId = TBCQ.InventoryId)
			WHERE InventoryTracking = 'AllowBackordering' AND  UpdatedQuantity IS NOT NULL  ;
			SET @Status = 1;
		END;
		
		-- If @InventoryTracking is "DisablePurchasing" then subtracts currently selected quantity only when it will greater than zero.
		IF EXISTS ( SELECT TOP 1 1 FROM @TBL_XmlReturnToTable WHERE InventoryTracking = 'DisablePurchasing')
		BEGIN
			SET @BalanceQuantity = 1;
			UPDATE ZI 
			SET Quantity = CASE WHEN TBCQ.UpdatedQuantity < 0 THEN 0 ELSE TBCQ.UpdatedQuantity END 
			FROM dbo.ZnodeInventory ZI 
			INNER JOIN  @TBL_CalculateQuntity TBCQ ON(Zi.InventoryId = TBCQ.InventoryId) WHERE InventoryTracking = 'DisablePurchasing'
			AND TBCQ.UpdatedQuantity IS NOT NULL 
			;
		END;
		
		INSERT INTO ZnodeOmsOrderWarehouse( OmsOrderLineItemsId, WarehouseId,Quantity, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
		SELECT OrderLineItemId, WarehouseId,CASE WHEN UpdatedQuantity = 0 THEN MainQuantity ELSE MainQuantity - UpdatedQuantity END , @UserId, dbo.Fn_GetDate(), @UserId, dbo.Fn_GetDate() 
		FROM @TBL_CalculateQuntity AS TBCQ 
		INNER JOIN @TBL_XmlReturnToTable AS TBXR
			   ON(TBXR.SKU = TBCQ.SKU) WHERE UpdatedQuantity IS NOT NULL;
  
		SELECT DISTINCT SKU,
		 [dbo].[Fn_GetDefaultPriceRoundOffReturnNumeric](UpdatedQuantity) AS Quantity,
		  InventoryTracking,CAST(AllowBackOrder AS BIT) AllowBackOrder
		FROM @TBL_CalculateQuntity
		WHERE UpdatedQuantity IS NOT NULL;

		SET @Status = 1;
		COMMIT TRAN UpdateInventory;
	END TRY
	BEGIN CATCH
	    
		SET @Status = 0;

		SELECT SKU, Quantity, InventoryTracking,AllowBackOrder
		FROM @TBL_ErrorInventoryTracking;
		
		ROLLBACK TRAN UpdateInventory;		
		
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'Znode_UpdateInventory @SkuXml = '+CAST(@SkuXml AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@PortalId='+CAST(@PortalId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10))+',@IsDebug ='+CAST(@IsDebug AS VARCHAR(10));
         
        EXEC Znode_InsertProcedureErrorLog

			@ProcedureName = 'Znode_UpdateInventory',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;

	END CATCH;  
END;