
IF EXISTS (SELECT TOP 1 1 FROM Sys.Tables WHERE Name = 'ZnodeMultifront')
BEGIN 
 IF EXISTS (SELECT TOP 1 1 FROM ZnodeMultifront where BuildVersion =   9054  )
 BEGIN 
 PRINT 'Script is already executed....'
  SET NOEXEC ON 
 END 
END
ELSE 
BEGIN 
   SET NOEXEC ON
END 
INSERT INTO [dbo].[ZnodeMultifront] ( [VersionName], [Descriptions], [MajorVersion], [MinorVersion], [LowerVersion], [BuildVersion], [PatchIndex], [CreatedBy], 
[CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES ( N'Znode_Multifront_9_0_5_4', N'Upgrade Patch GA Release by 905',9,0,5,9054,0,2, GETDATE(),2, GETDATE())
GO 
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_InsertUpdateSaveCartLineItem')
BEGIN 
	DROP PROCEDURE Znode_InsertUpdateSaveCartLineItem
END 
GO



CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItem](
	  @CartLineItemXML xml, @UserId int, @Status bit OUT)
AS 
   /* 
    Summary: THis Procedure is USed to save and edit the saved cart line item      
    Unit Testing 
	begin tran  
    Exec Znode_InsertUpdateSaveCartLineItem @CartLineItemXML= '<ArrayOfSavedCartLineItemModel>
    <SavedCartLineItemModel>
    <OmsSavedCartLineItemId>0</OmsSavedCartLineItemId>
    <ParentOmsSavedCartLineItemId>0</ParentOmsSavedCartLineItemId>
    <OmsSavedCartId>1259</OmsSavedCartId>
    <SKU>BlueGreenYellow</SKU>
    <Quantity>1.000000</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>1</Sequence>
    <AddonProducts>YELLOW</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts>GREEN</ConfigurableProducts>
    </SavedCartLineItemModel>
    <SavedCartLineItemModel>
    <OmsSavedCartLineItemId>0</OmsSavedCartLineItemId>
    <ParentOmsSavedCartLineItemId>0</ParentOmsSavedCartLineItemId>
    <OmsSavedCartId>1259</OmsSavedCartId>
    <SKU>ap1534</SKU>
    <Quantity>1.0</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>2</Sequence>
    <AddonProducts >PINK</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts />
    <PersonaliseValuesList>Address~Hello</PersonaliseValuesList>
  </SavedCartLineItemModel>
</ArrayOfSavedCartLineItemModel>' , @UserId=1 ,@Status=0
	rollback tran
*/
BEGIN
	BEGIN TRAN InsertUpdateSaveCartLineItem;
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		DECLARE @AddOnQuantity numeric(28, 6)= 0;
		DECLARE @SaveCartLineItemIdForGroup int= 0;
		DECLARE @TBL_SavecartLineitems TABLE
		( 
			RowId int IDENTITY(1, 1), OmsSavedCartLineItemId int, ParentOmsSavedCartLineItemId int, OmsSavedCartId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), 
			CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute varchar(max), 
			AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max)
		);
		DECLARE @OrderLineItemRelationshipTypeIdAddon int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'AddOns'
		);
		DECLARE @OrderLineItemRelationshipTypeIdBundle int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Bundles'
		);
		DECLARE @OrderLineItemRelationshipTypeIdConfigurable int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Configurable'
		);
		DECLARE @OrderLineItemRelationshipTypeIdGroup int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Group'
		);
		INSERT INTO @TBL_SavecartLineitems( OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, AddOnValueIds, BundleProductIds, ConfigurableProductIds, GroupProductIds, PersonalisedAttribute, AutoAddon, OmsOrderId, ItemDetails )
			   SELECT Tbl.Col.value( 'OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartLineItemId, Tbl.Col.value( 'ParentOmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS ParentOmsSavedCartLineItemId, Tbl.Col.value( 'OmsSavedCartId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'Quantity[1]', 'NVARCHAR(2000)' ) AS Quantity, Tbl.Col.value( 'OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)' ) AS OrderLineItemRelationshipTypeID, Tbl.Col.value( 'CustomText[1]', 'NVARCHAR(2000)' ) AS CustomText, Tbl.Col.value( 'CartAddOnDetails[1]', 'NVARCHAR(2000)' ) AS CartAddOnDetails, Tbl.Col.value( 'Sequence[1]', 'NVARCHAR(2000)' ) AS Sequence, Tbl.Col.value( 'AddonProducts[1]', 'NVARCHAR(2000)' ) AS AddOnValueIds, Tbl.Col.value( 'BundleProducts[1]', 'NVARCHAR(2000)' ) AS BundleProductIds, Tbl.Col.value( 'ConfigurableProducts[1]', 'NVARCHAR(2000)' ) AS ConfigurableProductIds, Tbl.Col.value( 'GroupProducts[1]', 'NVARCHAR(Max)' ) AS GroupProductIds, 
			          Tbl.Col.value( 'PersonaliseValuesList[1]', 'NVARCHAR(Max)' ) AS GroupProductIds, Tbl.Col.value( 'AutoAddon[1]', 'NVARCHAR(Max)' ) AS AutoAddon, Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(Max)' ) AS OmsOrderId,
					  Tbl.Col.value( 'ItemDetails[1]', 'NVARCHAR(Max)' ) AS ItemDetails
			   FROM @CartLineItemXML.nodes( '//ArrayOfSavedCartLineItemModel/SavedCartLineItemModel' ) AS Tbl(Col);

		DECLARE @OmsSavedCartId int, @OmsSavedCartLineItemId int,@OmsOrderId int;

	 --SELECT * FROM @TBL_SavecartLineitems
		  
		DECLARE @TBL_bundleAddonRows TABLE
		( 
										   RowId int, SequenceId int IDENTITY(1, 1), ParentOmsSavedCartLineItemId int, SKU nvarchar(1000), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), CartAddOnDetails nvarchar(max), AutoAddon varchar(max), OmsOrderId int null ,IsFromAddon INT 
		);
		DECLARE @AddonProductSKU nvarchar(max)=
		(
			SELECT TOP 1 AddOnValueIds
			FROM @TBL_SavecartLineitems
		), @BundleProductSKU nvarchar(max)=
		(
			SELECT TOP 1 BundleProductIds
			FROM @TBL_SavecartLineitems
		);
		SET @OmsSavedCartId =
		(
			SELECT TOP 1 OmsSavedCartId
			FROM @TBL_SavecartLineitems
		);
		SET @OmsOrderId =
		(
			SELECT TOP 1 OmsOrderId
			FROM @TBL_SavecartLineitems
		);
		IF EXISTS
		(
			SELECT TOP 1 1
			FROM ZnodeOmsSavedCartLineItem AS qa
			WHERE EXISTS
			(
				SELECT TOP 1 1
				FROM @TBL_SavecartLineitems AS ssds
				WHERE ssds.sku = qa.SKU
			)
		)
		BEGIN
			DELETE FROM ZnodeOmsPersonalizeCartItem
			WHERE EXISTS
			(
				SELECT TOP 1 1
				FROM ZnodeOmsSavedCartLineItem
				WHERE OmsSavedCartId = @OmsSavedCartId AND 
					  OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId
			);
		
			IF EXISTS (SELECT * FROM @TBL_SavecartLineitems WHERE ItemDetails is not null)
			BEGIN 
				DELETE ZnodeOmsSavedCartLineItemDetails
				WHERE EXISTS
				( 
					SELECT * FROM ZnodeOmsSavedCartLineItem SCLI
					WHERE ZnodeOmsSavedCartLineItemDetails.OmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND SCLI.OmsSavedCartId = @OmsSavedCartId AND SCLI.OmsOrderId = @OmsOrderId	
				)
		    END

			DELETE FROM ZnodeOmsSavedCartLineItem 
			WHERE OmsSavedCartId = @OmsSavedCartId AND OmsOrderId = @OmsOrderId									

		END;	
					
		INSERT INTO @TBL_bundleAddonRows( RowId, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId ,IsFromAddon)
			   SELECT RowID, NULL, q.Item AS SKU, a.Quantity, @OrderLineItemRelationshipTypeIdBundle, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,1
			   FROM @TBL_SavecartLineitems AS a
					CROSS APPLY
					dbo.Split( a.BundleProductIds, ',' ) AS q
			   WHERE a.BundleProductIds IS NOT NULL AND a.BundleProductIds <> ''; 

		INSERT INTO @TBL_bundleAddonRows( RowId, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId ,IsFromAddon)
			   SELECT RowID, NULL, q.Item AS SKU, a.Quantity, @OrderLineItemRelationshipTypeIdConfigurable, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,0
			   FROM @TBL_SavecartLineitems AS a
					CROSS APPLY
					dbo.Split( a.ConfigurableProductIds, ',' ) AS q
			   WHERE a.ConfigurableProductIds IS NOT NULL AND a.ConfigurableProductIds <> '';

		INSERT INTO @TBL_bundleAddonRows( RowId, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,IsFromAddon )
			   SELECT RowID, NULL, CASE WHEN q.Item = '' THEN '' ELSE SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) END  AS SKU, SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000) AS Quantity, @OrderLineItemRelationshipTypeIdGroup, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,0
			   FROM @TBL_SavecartLineitems AS a
					CROSS APPLY
					dbo.Split( a.GroupProductIds, ',' ) AS q
			   WHERE a.GroupProductIds IS NOT NULL AND a.GroupProductIds <> '' ;

		IF EXISTS
		(
			SELECT TOP 1 1
			FROM @TBL_SavecartLineitems
			WHERE GroupProductIds IS NOT NULL OR 
				  ConfigurableProductIds IS NOT NULL
		)
		BEGIN
			SET @AddOnQuantity =
			(
				SELECT MAX(Quantity)
				FROM @TBL_bundleAddonRows
			);
		END;


		INSERT INTO @TBL_bundleAddonRows( RowId, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,IsFromAddon )
			   SELECT a.RowID, NULL, q.Item AS SKU,
											   CASE
											   WHEN @AddOnQuantity = 0 THEN a.Quantity
											   ELSE @AddOnQuantity
											   END, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,1
			   FROM @TBL_SavecartLineitems AS a
					CROSS APPLY
					dbo.Split( a.AddOnValueIds, ',' ) AS q
			   WHERE a.AddOnValueIds IS NOT NULL AND AddOnValueIds <> ''; 
	
		DECLARE @Tbl_SAvecartIds TABLE
		( 
			OmsSavedCartLineItemId int, SKU nvarchar(max), RowId int,ParentOmsSavedCartLineItemId INT 
		);

					
		MERGE INTO ZnodeOmsSavedCartLineItem TARGET
		USING @TBL_SavecartLineitems SOURCE
		ON 1 = 0
		WHEN NOT MATCHED
			  THEN INSERT(ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, OmsOrderId, AutoAddon, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
			  VALUES( NULL, @OmsSavedCartId, Source.SKU, Source.Quantity,CASE
																			WHEN Source.OrderLineItemRelationshipTypeID = 0 
																			THEN NULL
																			ELSE OrderLineItemRelationshipTypeID
																		 END, 
					 Source.CustomText, Source.CartAddOnDetails, Source.Sequence, SOURCE.OmsOrderId, SOURCE.AutoAddon, @UserId, @GetDate, @UserId, @GetDate )
		OUTPUT Inserted.OmsSavedCartLineItemId, Source.SKU, SOURCE.RowID,INSERTED.ParentOmsSavedCartLineItemId
		INTO @Tbl_SAvecartIds;

		--IF EXISTS ( SELECT * FROM @TBL_SavecartLineitems WHERE ItemDetails is not null )
		--BEGIN
			INSERT INTO ZnodeOmsSavedCartLineItemDetails ( OmsSavedCartLineItemId, OmsSavedCartId, [Key], Value, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate ) 
			SELECT SCLI.OmsSavedCartLineItemId, SCLI.OmsSavedCartId, LEFT(ID.item,CHARINDEX('~',ID.item)-1) as [Key], RIGHT(ID.item, LEN(ID.item)-CHARINDEX('~',ID.item)) as Value, @UserId, @GetDate, @UserId, @GetDate
			FROM ZnodeOmsSavedCartLineItem SCLI
			INNER JOIN @Tbl_SAvecartIds TSCI ON SCLI.OmsSavedCartLineItemId = TSCI.OmsSavedCartLineItemId
			INNER JOIN @TBL_SavecartLineitems TSCLI ON TSCI.SKU = TSCLI.SKU AND TSCLI.RowID = TSCLI.RowID
			CROSS APPLY dbo.split ( ItemDetails, ',' ) ID 
			WHERE SCLI.OmsSavedCartId = @OmsSavedCartId AND LEFT(ID.item,CHARINDEX('~',ID.item)-1) IS NOT NULL
			AND EXISTS ( SELECT * FROM @TBL_SavecartLineitems TSCLI1 WHERE TSCLI.SKU = TSCLI.SKU AND TSCLI.RowId = TSCLI.RowId AND TSCLI.ItemDetails IS NOT NULL )
		--END
	
					
        MERGE INTO ZnodeOmsSavedCartLineItem TARGET
		USING ( SELECT b.OmsSavedCartLineItemId, @OmsSavedCartId OmsSavedCartId , a.SKU, Quantity,
																		CASE
																		WHEN OrderLineItemRelationshipTypeID = 0 THEN NULL
																		ELSE OrderLineItemRelationshipTypeID
																		END OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, SequenceId, OmsOrderId, AutoAddon, @UserId CreatedBy , @GetDate CreatedDate, @UserId ModifiedBy, @GetDate ModifiedDate,a.RowId
			   FROM @TBL_bundleAddonRows AS a
					INNER JOIN
					@Tbl_SAvecartIds AS b
					ON(a.RowId = b.RowId)
			   WHERE a.SKU IS NOT NULL AND 
					 a.SKU <> '') SOURCE
		ON 1 = 0
		WHEN NOT MATCHED
			  THEN INSERT ( ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, [Sequence], OmsOrderId, AutoAddon, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )

			  VALUES (SOURCE.OmsSavedCartLineItemId,SOURCE.OmsSavedCartId,SOURCE.SKU,SOURCE.Quantity,SOURCE.OrderLineItemRelationshipTypeID , SOURCE.CustomText,
			  SOURCE.CartAddOnDetails,SOURCE.SequenceId,SOURCE.OmsOrderId,SOURCE.AutoAddon,SOURCE.CreatedBy,SOURCE.CREATEDDATE,SOURCE.ModifiedBy, SOURCE.ModifiedDate
			  
			  )
		OUTPUT Inserted.OmsSavedCartLineItemId, Source.SKU, SOURCE.RowID,INSERTED.ParentOmsSavedCartLineItemId
			   INTO @Tbl_SAvecartIds;
			   
		--IF EXISTS ( SELECT * FROM @TBL_SavecartLineitems WHERE ItemDetails IS NOT NULL )
		--BEGIN
	
					
			INSERT INTO ZnodeOmsSavedCartLineItemDetails ( OmsSavedCartLineItemId, OmsSavedCartId, [Key], Value, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate ) 
			SELECT SCLI.OmsSavedCartLineItemId, SCLI.OmsSavedCartId, CASE WHEN ID.item <> '' THEN LEFT(ID.item,CHARINDEX('~',ID.item)-1) ELSE '' END  as [Key], RIGHT(ID.item, LEN(ID.item)-CHARINDEX('~',ID.item)) as Value, @UserId, @GetDate, @UserId, @GetDate
			FROM ZnodeOmsSavedCartLineItem SCLI
			INNER JOIN @Tbl_SAvecartIds TSCI ON SCLI.OmsSavedCartLineItemId = TSCI.OmsSavedCartLineItemId
			INNER JOIN @TBL_bundleAddonRows BAR ON ( TSCI.SKU = BAR.SKU AND BAR.RowID = TSCI.RowID )
			INNER JOIN @TBL_SavecartLineitems TSCLI ON (BAR.SKU = TSCLI.SKU AND BAR.RowID = TSCLI.RowID )
			CROSS APPLY dbo.split ( TSCLI.ItemDetails, ',' ) ID 
			WHERE SCLI.OmsSavedCartId = @OmsSavedCartId AND LEFT(ID.item,CHARINDEX('~',ID.item)-1) IS NOT NULL
			AND EXISTS ( SELECT * FROM @TBL_SavecartLineitems TSCLI1 WHERE TSCLI.SKU = TSCLI.SKU AND TSCLI.RowId = TSCLI.RowId AND TSCLI.ItemDetails IS NOT NULL AND TSCLI.ItemDetails <> '')
		--END
		--IF EXISTS
		--(
		--	SELECT TOP 1 1
		--	FROM @TBL_SavecartLineitems
		--	WHERE GroupProductIds IS NOT NULL OR 
		--		  ConfigurableProductIds IS NOT NULL
		--)
		--BEGIN
		 --SELECT * FROM @Tbl_SAvecartIds
		 --SELECT * FROM @TBL_bundleAddonRows
		 --SELECT * FROM @TBL_SavecartLineitems
			DECLARE @TBL_SaveCartConfigProduct TABLE (OmsSavedCartLineItemId INT, SKU VARCHAR(2000),RowId INT, PersonalisedAttribute NVARCHAr(max))
			    INSERT @TBL_SaveCartConfigProduct    
			    SELECT DISTINCT    ZOSCL.OmsSavedCartLineItemId   ,ZOSCL.SKU 
				,  ZOSCL.RowId , (SELECT TOP 1 PersonalisedAttribute FROM @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = ZOSCL.SKU AND TRTR.RowID = ZOSCL.RowID )  PersonalisedAttribute
				FROM @Tbl_SAvecartIds AS ZOSCL
				LEFT JOIN @TBL_bundleAddonRows AS TBBR ON (ZOSCL.SKU = TBBR.SKU AND TBBR.RowID = ZOSCL.RowId  )
				WHERE ( EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = ZOSCL.SKU AND TRTR.RowID = ZOSCL.RowID AND TRTR.PersonalisedAttribute IS NOT NULL )
				OR   EXISTS (SELECT TOP 1 1 FROM @TBL_bundleAddonRows TRT WHERE SKU <> ''  AND IsFromAddon <> 1  AND TRT.RowID = ZOSCl.RowID   ) )
				AND ((ZOSCL.SKU = TBBR.SKU AND TBBR.RowID = ZOSCL.RowId) OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_bundleAddonRows RTR WHERE SKU <> '' AND IsFromAddon <> 1   AND RTR.RowID = ZOSCl.RowID) 
				
				)
					
		INSERT INTO ZnodeOmsPersonalizeCartItem( OmsSavedCartLineItemId, PersonalizeCode, PersonalizeValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			   SELECT DISTINCT 
			   b.OmsSavedCartLineItemId 
			  , CASE WHEN ISNULL(q.Item,'') = '' THEN '' ELSE CASE WHEN q.Item = '' THEN '' ELSE  SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) END  END  AS Keyi, SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000) AS Value, @UserId, @GetDate, @UserId, @GetDate
			   FROM @Tbl_SAvecartIds m  
			   LEFT JOIN @TBL_SaveCartConfigProduct AS b ON( b.RowId = m.RowId )
			   CROSS APPLY	dbo.Split( (SELECT TOP 1 PersonalisedAttribute FROM  @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = m.SKU AND TRTR.RowID = m.RowID ), ',' ) AS q
			   WHERE EXISTS (SELECT TOP 1 1 FROM  @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = m.SKU AND TRTR.RowID = m.RowID AND TRTR.PersonalisedAttribute IS NOT NULL AND TRTR.PersonalisedAttribute <> '' )
			   ; 
        	
	SET @Status = 1;
	COMMIT TRAN InsertUpdateSaveCartLineItem;
	END TRY
	BEGIN CATCH

		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_InsertUpdateSaveCartLineItem @CartLineItemXML = '+CAST(@CartLineItemXML
 AS varchar(max))+',@UserId = '+CAST(@UserId AS varchar(50))+',@Status='+CAST(@Status AS varchar(10));

		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		ROLLBACK TRAN InsertUpdateSaveCartLineItem;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_InsertUpdateSaveCartLineItem', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;

GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_InsertUpdateSaveCartLineItemQuantity')
BEGIN 
	DROP PROCEDURE Znode_InsertUpdateSaveCartLineItemQuantity
END 
GO



CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItemQuantity](
	  @CartLineItemXML xml, @UserId int,@Status bit OUT)
AS 
   /* 
    Summary: THis Procedure is USed to save and edit the saved cart line item      
    Unit Testing 
	begin tran  
    Exec Znode_InsertUpdateSaveCartLineItem_aa @CartLineItemXML= '<ArrayOfSavedCartLineItemModel>
  <SavedCartLineItemModel>
    <OmsSavedCartLineItemId>0</OmsSavedCartLineItemId>
    <ParentOmsSavedCartLineItemId>0</ParentOmsSavedCartLineItemId>
    <OmsSavedCartId>30</OmsSavedCartId>
    <SKU>BlueGreenYellow</SKU>
    <Quantity>1.000000</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>1</Sequence>
    <AddonProducts>YELLOW</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts>GREEN</ConfigurableProducts>
  </SavedCartLineItemModel>
  <SavedCartLineItemModel>
    <OmsSavedCartLineItemId>0</OmsSavedCartLineItemId>
    <ParentOmsSavedCartLineItemId>0</ParentOmsSavedCartLineItemId>
    <OmsSavedCartId>30</OmsSavedCartId>
    <SKU>ap1534</SKU>
    <Quantity>1.0</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>2</Sequence>
    <AddonProducts >PINK</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts />
    <PersonaliseValuesList>Address~Hello</PersonaliseValuesList>
  </SavedCartLineItemModel>
</ArrayOfSavedCartLineItemModel>' , @UserId=1 ,@Status=0
	rollback tran
	1259
*/
BEGIN
	BEGIN TRAN InsertUpdateSaveCartLineItem;
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		DECLARE @AddOnQuantity numeric(28, 6)= 0;
		DECLARE @SaveCartLineItemIdForGroup int= 0;
		DECLARE @TBL_SavecartLineitems TABLE
		( 
			RowId int IDENTITY(1, 1), IsAddToCartPage int, OmsSavedCartLineItemId int, ParentOmsSavedCartLineItemId int, OmsSavedCartId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), 
			CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute varchar(max), 
			AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max)
		);
		DECLARE @OrderLineItemRelationshipTypeIdAddon int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'AddOns'
		);
		
		DECLARE @OrderLineItemRelationshipTypeIdBundle int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Bundles'
		);
		DECLARE @OrderLineItemRelationshipTypeIdConfigurable int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Configurable'
		);
		DECLARE @OrderLineItemRelationshipTypeIdGroup int=
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Group'
		);
		INSERT INTO @TBL_SavecartLineitems( IsAddToCartPage, OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, AddOnValueIds, BundleProductIds, ConfigurableProductIds, GroupProductIds, PersonalisedAttribute, AutoAddon, OmsOrderId, ItemDetails )
			   SELECT Tbl.Col.value( 'OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS IsAddToCartPage, Tbl.Col.value( 'OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartLineItemId, Tbl.Col.value( 'ParentOmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS ParentOmsSavedCartLineItemId, Tbl.Col.value( 'OmsSavedCartId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'Quantity[1]', 'NVARCHAR(2000)' ) AS Quantity, Tbl.Col.value( 'OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)' ) AS OrderLineItemRelationshipTypeID, Tbl.Col.value( 'CustomText[1]', 'NVARCHAR(2000)' ) AS CustomText, Tbl.Col.value( 'CartAddOnDetails[1]', 'NVARCHAR(2000)' ) AS CartAddOnDetails, Tbl.Col.value( 'Sequence[1]', 'NVARCHAR(2000)' ) AS Sequence, Tbl.Col.value( 'AddonProducts[1]', 'NVARCHAR(2000)' ) AS AddOnValueIds, Tbl.Col.value( 'BundleProducts[1]', 'NVARCHAR(2000)' ) AS BundleProductIds, Tbl.Col.value( 'ConfigurableProducts[1]', 'NVARCHAR(2000)' ) AS ConfigurableProductIds, Tbl.Col.value( 'GroupProducts[1]', 'NVARCHAR(Max)' ) AS GroupProductIds, 
			          Tbl.Col.value( 'PersonaliseValuesList[1]', 'NVARCHAR(Max)' ) AS GroupProductIds, Tbl.Col.value( 'AutoAddon[1]', 'NVARCHAR(Max)' ) AS AutoAddon, Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(Max)' ) AS OmsOrderId,
					  Tbl.Col.value( 'ItemDetails[1]', 'NVARCHAR(Max)' ) AS ItemDetails
			   FROM @CartLineItemXML.nodes( '//ArrayOfSavedCartLineItemModel/SavedCartLineItemModel' ) AS Tbl(Col);

		DECLARE @OmsSavedCartId int, @OmsSavedCartLineItemId int,@OmsOrderId int;

	
		DECLARE @TBL_AllProductsTypeData TABLE
		( 
			 RowId int, SequenceId int IDENTITY(1, 1), IsAddToCartPage int, ParentOmsSavedCartLineItemId int, SKU nvarchar(1000), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), CartAddOnDetails nvarchar(max), AutoAddon varchar(max), OmsOrderId int null ,IsFromAddon INT 
		);

		DECLARE @AddonProductSKU nvarchar(max)=	(SELECT TOP 1 AddOnValueIds	FROM @TBL_SavecartLineitems	), 
				@BundleProductSKU nvarchar(max)= (SELECT TOP 1 BundleProductIds	FROM @TBL_SavecartLineitems	);
		SET @OmsSavedCartId =( SELECT TOP 1 OmsSavedCartId FROM @TBL_SavecartLineitems);
		SET @OmsOrderId =( SELECT TOP 1 OmsOrderId FROM @TBL_SavecartLineitems );
		

		INSERT INTO @TBL_AllProductsTypeData( RowId, IsAddToCartPage, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId ,IsFromAddon)
			   SELECT RowID, a.IsAddToCartPage, NULL, q.Item AS SKU, a.Quantity, @OrderLineItemRelationshipTypeIdBundle, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,1
			   FROM @TBL_SavecartLineitems AS a	
			   CROSS APPLY	dbo.Split( a.BundleProductIds, ',' ) AS q
			   WHERE a.BundleProductIds IS NOT NULL
			   AND  RTRIM(LTRIM(q.Item)) <> '' ;

		INSERT INTO @TBL_AllProductsTypeData( RowId, IsAddToCartPage, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId ,IsFromAddon)
			   SELECT RowID, a.IsAddToCartPage, NULL, q.Item AS SKU, a.Quantity, @OrderLineItemRelationshipTypeIdConfigurable, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,0
			   FROM @TBL_SavecartLineitems AS a	CROSS APPLY	dbo.Split( a.ConfigurableProductIds, ',' ) AS q
			   WHERE a.ConfigurableProductIds IS NOT NULL
			   AND  RTRIM(LTRIM(q.Item)) <> ''
			   ;

		INSERT INTO @TBL_AllProductsTypeData( RowId, IsAddToCartPage, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,IsFromAddon )
			   SELECT RowID, a.IsAddToCartPage, NULL, SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) AS SKU, SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000) AS Quantity, @OrderLineItemRelationshipTypeIdGroup, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,0
			   FROM @TBL_SavecartLineitems AS a	CROSS APPLY	dbo.Split( a.GroupProductIds, ',' ) AS q
			   WHERE a.GroupProductIds IS NOT NULL AND 	a.GroupProductIds <> ''
			   AND  RTRIM(LTRIM(SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1))) <> ''
			   ;
			   
		IF EXISTS
		(
			SELECT TOP 1 1	FROM @TBL_SavecartLineitems	WHERE GroupProductIds IS NOT NULL OR  ConfigurableProductIds IS NOT NULL
		)
		BEGIN
			SET @AddOnQuantity =(SELECT MAX(Quantity) FROM @TBL_AllProductsTypeData	);
		END;


		INSERT INTO @TBL_AllProductsTypeData( RowId, IsAddToCartPage, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,IsFromAddon )
			   SELECT a.RowID, a.IsAddToCartPage, NULL, q.Item AS SKU,
											   CASE
											   WHEN @AddOnQuantity = 0 THEN a.Quantity
											   ELSE @AddOnQuantity
											   END, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, AutoAddon, OmsOrderId,1
			   FROM @TBL_SavecartLineitems AS a	CROSS APPLY	dbo.Split( a.AddOnValueIds, ',' ) AS q   WHERE a.AddOnValueIds IS NOT NULL
			   AND  RTRIM(LTRIM(q.Item)) <> ''

			  
	DECLARE @Tbl_SaveCartIds TABLE
		( 
			OmsSavedCartLineItemId int, SKU nvarchar(max), RowId int,ParentOmsSavedCartLineItemId INT 
		);

	DECLARE @IsCallForUpdate BIT = 0 
	DECLARE @saveCartLineItemId TABLE (OmsSavedCartLineItemId INT,SKU NVARCHAR(2000),OmsSavedCartId INT,ChildSKU NVARCHAR(max), Sequence INT ,RowId INT Identity(1,1) )

	-- for group,bundle and configure products
	INSERT INTO @saveCartLineItemId (OmsSavedCartLineItemId,SKU,OmsSavedCartId,ChildSKU,Sequence)
	SELECT DISTINCT CLI.OmsSavedCartLineItemId,cli.SKU ,cli.OmsSavedCartId,YU.SKU ,YU.Sequence 
	FROM ZnodeOmsSavedCartLineItem CLI 
																INNER JOIN  @TBL_SavecartLineitems s ON (s.SKU = cli.SKU AND s.OmsSavedCartId = cli.OmsSavedCartId)
																INNER JOIN ZnodeOmsSavedCartLineItem   YU ON (Yu.ParentOmsSavedCartLineItemId = CLI.OmsSavedCartLineItemId)
																INNER JOIN  @TBL_AllProductsTypeData tbad ON (tbad.SKU = YU.SKU )
	 WHERE CLI.ParentOmsSavedCartLineItemId IS NULL 
	

	-- for simple products
	INSERT INTO @SaveCartLineItemId  (OmsSavedCartLineItemId,SKU,OmsSavedCartId,ChildSKU,Sequence)
	SELECT DISTINCT CLI.OmsSavedCartLineItemId,cli.SKU ,cli.OmsSavedCartId,NULL ,CLI.Sequence  
	FROM ZnodeOmsSavedCartLineItem CLI 
	INNER JOIN  @TBL_SavecartLineitems s ON (s.SKU = cli.SKU AND s.OmsSavedCartId = cli.OmsSavedCartId AND CLI.Sequence  = s.Sequence )
	WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_AllProductsTypeData )													
	AND CLI.ParentOmsSavedCartLineItemId IS NULL 
	
	-- for personalized products
	INSERT INTO @SaveCartLineItemId  (OmsSavedCartLineItemId,SKU,OmsSavedCartId,ChildSKU,Sequence)
	SELECT DISTINCT CLI.OmsSavedCartLineItemId,cli.SKU ,cli.OmsSavedCartId,NULL ,CLI.Sequence  
	FROM ZnodeOmsSavedCartLineItem CLI 
	INNER JOIN  @TBL_SavecartLineitems s ON (s.SKU = cli.SKU AND s.OmsSavedCartId = cli.OmsSavedCartId )
	INNER JOIN ZnodeOmsPersonalizeCartItem ZOPCI ON CLI.OmsSavedCartLineItemId = ZOPCI.OmsSavedCartLineItemId 
			AND ZOPCI.PersonalizeCode = SUBSTRING(s.PersonalisedAttribute, 1, CHARINDEX('~', s.PersonalisedAttribute)-1)
			AND ZOPCI.PersonalizeValue = SUBSTRING(s.PersonalisedAttribute, CHARINDEX('~',  s.PersonalisedAttribute)+1, 4000)
	WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_AllProductsTypeData )													
	AND CLI.ParentOmsSavedCartLineItemId IS NULL
	
		IF EXISTS (SELECT TOP  1 1 FROM @TBL_SavecartLineitems WHERE PersonalisedAttribute IS NOT NULL)
	BEGIN
	
		SET @IsCallForUpdate = CASE WHEN EXISTS (SELECT TOP  1 1 FROM @saveCartLineItemId   m
								CROSS APPLY	dbo.Split( (SELECT TOP 1 PersonalisedAttribute FROM  @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = m.SKU AND TRTR.RowID = m.RowID AND TRTR.PersonalisedAttribute IS NOT NULL), ',' ) AS q
								WHERE  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem s WHERE s.OmsSavedCartLineItemId = m.OmsSavedCartLineItemId AND s.PersonalizeCode = SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) 
																																						   AND s.PersonalizeValue = SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000))
																																		 
		)	THEN 0 ELSE 1 END
	END

	ELSE
		BEGIN
		
		SET @IsCallForUpdate = CASE WHEN EXISTS (SELECT TOP  1 1 FROM @saveCartLineItemId --a inner join @TBL_SavecartLineitems s on (a.sku = s.sku) where s.PersonalisedAttribute is null
		)	THEN 0 ELSE 1 END

		END
		  
		UPDATE SI
		 SET  OmsSavedCartLineItemId = CASE WHEN HJRHRH.OmsSavedCartLineItemId IS NULL THEN  SL.OmsSavedCartLineItemId ELSE HJRHRH.OmsSavedCartLineItemId END 
		      ,Sequence = CASE WHEN HJRHRH.Sequence IS NULL THEN  SI.Sequence ELSE HJRHRH.Sequence END 
		 FROM @TBL_SavecartLineitems SI
		 LEFT JOIN ZnodeOmsSavedCartLineItem SL ON (SI.SKU = SL.SKU AND SI.OmsSavedCartId = SL.OmsSavedCartId AND SI.Sequence = SL.Sequence )
		 LEFT JOIN 	@saveCartLineItemId HJRHRH ON (SI.SKU = HJRHRH.SKU AND SI.OmsSavedCartId = HJRHRH.OmsSavedCartId  AND SI.Sequence = HJRHRH.RowId )
		
		----Update OmsSavedCartLineItemId for personalized product update
		UPDATE SI SET SI.OmsSavedCartLineItemId = ZOSCLI.OmsSavedCartLineItemID
		FROM @TBL_SavecartLineitems SI
		INNER JOIN ZnodeOmsSavedCartLineItem ZOSCLI ON SI.OmsSavedCartId = ZOSCLI.OmsSavedCartId AND SI.SKU=ZOSCLI.SKU
		INNER JOIN ZnodeOmsPersonalizeCartItem ZOPCI ON ZOSCLI.OmsSavedCartLineItemId = ZOPCI.OmsSavedCartLineItemId 
			AND ZOPCI.PersonalizeCode = SUBSTRING(SI.PersonalisedAttribute, 1, CHARINDEX('~', SI.PersonalisedAttribute)-1)
			AND ZOPCI.PersonalizeValue = SUBSTRING(SI.PersonalisedAttribute, CHARINDEX('~',  SI.PersonalisedAttribute)+1, 4000)
		 WHERE ISNULL(SI.PersonalisedAttribute,'') <> ''
		 
		MERGE INTO ZnodeOmsSavedCartLineItem TARGET
		USING  (SELECT  DISTINCT SI.IsAddToCartPage, ISNULL(SI.OmsSavedCartLineItemId,-1) AS OmsSavedCartLineItemId,SI.SKU,SI.Quantity,SI.OrderLineItemRelationshipTypeID,SI.CustomText,SI.CartAddOnDetails,SI.Sequence,SI.AutoAddon,SI.RowID,SI.OmsOrderId 
		FROM @TBL_SavecartLineitems SI 
		WHERE (EXISTS (SELECT TOP 1 1  FROM @saveCartLineItemId TY WHERE TY.OmsSavedCartLineItemId = SI.OmsSavedCartLineItemId ))
		 OR (NOT EXISTS (SELECT TOP 1 1  FROM @saveCartLineItemId TY ))) SOURCE 
		ON TARGET.OmsSavedCartId = @OmsSavedCartId AND TARGET.SKU = Source.SKU AND SOURCE.OmsSavedCartLineItemId = TARGET.OmsSavedCartLineItemId AND @IsCallForUpdate = 0
		WHEN MATCHED   THEN
		UPDATE 
		SET   Quantity = CASE WHEN (SOURCE.IsAddToCartPage <> 0 ) THEN Source.Quantity ELSE Target.Quantity + Source.Quantity END ,
		 OrderLineItemRelationshipTypeID = CASE	 WHEN Source.OrderLineItemRelationshipTypeID = 0 
																			THEN NULL
																			ELSE Source.OrderLineItemRelationshipTypeID
																		 END, 
																		 CustomText = Source.CustomText,CartAddOnDetails = Source.CartAddOnDetails,Sequence= Source.Sequence,OmsOrderId = @OmsOrderId,TARGET.AutoAddon = SOURCE.AutoAddon
																		 ,CreatedBy=@UserId,CreatedDate =@GetDate ,ModifiedBy =@UserId,ModifiedDate =@GetDate
																		
		WHEN NOT MATCHED  
			  THEN INSERT(ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, OmsOrderId, AutoAddon, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
			  VALUES( NULL, @OmsSavedCartId, Source.SKU, Source.Quantity,CASE
																			WHEN Source.OrderLineItemRelationshipTypeID = 0 
																			THEN NULL
																			ELSE OrderLineItemRelationshipTypeID
																		 END, 
					 Source.CustomText, Source.CartAddOnDetails, Source.Sequence, SOURCE.OmsOrderId, SOURCE.AutoAddon, @UserId, @GetDate, @UserId, @GetDate )
		OUTPUT Inserted.OmsSavedCartLineItemId, Source.SKU, SOURCE.RowID,INSERTED.ParentOmsSavedCartLineItemId
		INTO @Tbl_SaveCartIds;
	
			 
			INSERT INTO ZnodeOmsSavedCartLineItemDetails ( OmsSavedCartLineItemId, OmsSavedCartId, [Key], Value, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate ) 
			SELECT SCLI.OmsSavedCartLineItemId, SCLI.OmsSavedCartId, LEFT(ID.item,CHARINDEX('~',ID.item)-1) as [Key], RIGHT(ID.item, LEN(ID.item)-CHARINDEX('~',ID.item)) as Value, @UserId, @GetDate, @UserId, @GetDate
			FROM ZnodeOmsSavedCartLineItem SCLI
			INNER JOIN @Tbl_SaveCartIds TSCI ON SCLI.OmsSavedCartLineItemId = TSCI.OmsSavedCartLineItemId
			INNER JOIN @TBL_SavecartLineitems TSCLI ON TSCI.SKU = TSCLI.SKU AND TSCLI.RowID = TSCLI.RowID
			CROSS APPLY dbo.split ( ItemDetails, ',' ) ID 
			WHERE SCLI.OmsSavedCartId = @OmsSavedCartId AND LEFT(ID.item,CHARINDEX('~',ID.item)-1) IS NOT NULL
			AND EXISTS ( SELECT * FROM @TBL_SavecartLineitems TSCLI1 WHERE TSCLI.SKU = TSCLI.SKU AND TSCLI.RowId = TSCLI.RowId AND TSCLI.ItemDetails IS NOT NULL )
			AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItemDetails scd WHERE scd.OmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND scd.OmsSavedCartId = SCLI.OmsSavedCartId )

		
		IF EXISTS (SELECT TOP 1  1  FROM @TBL_AllProductsTypeData )
		BEGIN 
        MERGE INTO ZnodeOmsSavedCartLineItem TARGET
		USING ( SELECT a.IsAddToCartPage,b.OmsSavedCartLineItemId, @OmsSavedCartId OmsSavedCartId , a.SKU, Quantity,
																		CASE
																		WHEN OrderLineItemRelationshipTypeID = 0 THEN NULL
																		ELSE OrderLineItemRelationshipTypeID
																		END OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, SequenceId, OmsOrderId, AutoAddon,@UserId CreatedBy , @GetDate CreatedDate, @UserId ModifiedBy, @GetDate ModifiedDate, a.RowId
			   FROM @TBL_AllProductsTypeData AS a
					INNER JOIN
					@Tbl_SaveCartIds AS b
					ON(a.RowId = b.RowId)
			   WHERE a.SKU IS NOT NULL AND 
					 a.SKU <> '') SOURCE
		ON TARGET.OmsSavedCartId = @OmsSavedCartId AND TARGET.SKU = Source.SKU AND @IsCallForUpdate = 0 AND Target.ParentOmsSavedCartLineItemId = source.OmsSavedCartLineItemId
		WHEN MATCHED THEN
		UPDATE 
		SET ParentOmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId  , Quantity = CASE WHEN (SOURCE.IsAddToCartPage <> 0 ) THEN Source.Quantity ELSE Target.Quantity + Source.Quantity END,
		 OrderLineItemRelationshipTypeID = 	Source.OrderLineItemRelationshipTypeID, 
																		 CustomText = Source.CustomText,CartAddOnDetails = Source.CartAddOnDetails,Sequence= Source.SequenceId,OmsOrderId = SOURCE.OmsOrderId,AutoAddon = SOURCE.AutoAddon
																		 ,CreatedBy=@UserId,CreatedDate =@GetDate ,ModifiedBy =@UserId,ModifiedDate =@GetDate


		WHEN NOT MATCHED
			  THEN INSERT ( ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, [Sequence], OmsOrderId, AutoAddon, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )

			  VALUES (SOURCE.OmsSavedCartLineItemId,SOURCE.OmsSavedCartId,SOURCE.SKU,SOURCE.Quantity,SOURCE.OrderLineItemRelationshipTypeID , SOURCE.CustomText,
			  SOURCE.CartAddOnDetails,SOURCE.SequenceId,SOURCE.OmsOrderId,SOURCE.AutoAddon,SOURCE.CreatedBy,SOURCE.CreatedDate,SOURCE.ModifiedBy, SOURCE.ModifiedDate
			  
			  )
		OUTPUT Inserted.OmsSavedCartLineItemId, Source.SKU, SOURCE.RowID,INSERTED.ParentOmsSavedCartLineItemId
			   INTO @Tbl_SaveCartIds;
			   
		 END 
			  	
			INSERT INTO ZnodeOmsSavedCartLineItemDetails ( OmsSavedCartLineItemId, OmsSavedCartId, [Key], Value, CreatedBy, CreatedDate,ModifiedBy, ModifiedDate ) 
			SELECT SCLI.OmsSavedCartLineItemId, SCLI.OmsSavedCartId, LEFT(ID.item,CHARINDEX('~',ID.item)-1) as [Key], RIGHT(ID.item, LEN(ID.item)-CHARINDEX('~',ID.item)) as Value, @UserId, @GetDate, @UserId, @GetDate
			FROM ZnodeOmsSavedCartLineItem SCLI
			INNER JOIN @Tbl_SaveCartIds TSCI ON SCLI.OmsSavedCartLineItemId = TSCI.OmsSavedCartLineItemId
			INNER JOIN @TBL_AllProductsTypeData BAR ON ( TSCI.SKU = BAR.SKU AND BAR.RowID = TSCI.RowID )
			INNER JOIN @TBL_SavecartLineitems TSCLI ON (BAR.SKU = TSCLI.SKU AND BAR.RowID = TSCLI.RowID )
			CROSS APPLY dbo.split ( TSCLI.ItemDetails, ',' ) ID 
			WHERE SCLI.OmsSavedCartId = @OmsSavedCartId AND LEFT(ID.item,CHARINDEX('~',ID.item)-1) IS NOT NULL
			AND EXISTS ( SELECT * FROM @TBL_SavecartLineitems TSCLI1 WHERE TSCLI.SKU = TSCLI.SKU AND TSCLI.RowId = TSCLI.RowId AND TSCLI.ItemDetails IS NOT NULL )
			AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItemDetails scd WHERE scd.OmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND scd.OmsSavedCartId = SCLI.OmsSavedCartId )
		   	
			DECLARE @TBL_SaveCartConfigProduct TABLE (OmsSavedCartLineItemId INT, SKU VARCHAR(2000),RowId INT, PersonalisedAttribute NVARCHAr(max))
			    INSERT @TBL_SaveCartConfigProduct    
			    SELECT DISTINCT    ZOSCL.OmsSavedCartLineItemId   ,ZOSCL.SKU 
				,  ZOSCL.RowId , (SELECT TOP 1 PersonalisedAttribute FROM @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = ZOSCL.SKU AND TRTR.RowID = ZOSCL.RowID )  PersonalisedAttribute
				FROM @Tbl_SaveCartIds AS ZOSCL
				LEFT JOIN @TBL_AllProductsTypeData AS TBBR ON (ZOSCL.SKU = TBBR.SKU AND TBBR.RowID = ZOSCL.RowId  )
				WHERE ( EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = ZOSCL.SKU AND TRTR.RowID = ZOSCL.RowID AND TRTR.PersonalisedAttribute IS NOT NULL )
				OR   EXISTS (SELECT TOP 1 1 FROM @TBL_AllProductsTypeData TRT WHERE SKU <> ''  AND IsFromAddon <> 1  AND TRT.RowID = ZOSCl.RowID   ) )
				AND ((ZOSCL.SKU = TBBR.SKU AND TBBR.RowID = ZOSCL.RowId) OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_AllProductsTypeData RTR WHERE SKU <> '' AND IsFromAddon <> 1   AND RTR.RowID = ZOSCl.RowID) 
				
				)
					   	 
	INSERT INTO ZnodeOmsPersonalizeCartItem( OmsSavedCartLineItemId, PersonalizeCode, PersonalizeValue, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
			   SELECT DISTINCT 
			   b.OmsSavedCartLineItemId 
			  , CASE WHEN ISNULL(q.Item,'') = '' THEN '' ELSE  SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) END  AS Keyi, SUBSTRING(q.Item, ISNULL(CHARINDEX('~', q.Item),0)+1, 4000) AS Value, @UserId, @GetDate, @UserId, @GetDate
			   FROM @Tbl_SaveCartIds m  
			   LEFT JOIN @TBL_SaveCartConfigProduct AS b ON( b.RowId = m.RowId )
			   CROSS APPLY	dbo.Split( (SELECT TOP 1 PersonalisedAttribute FROM  @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = m.SKU AND TRTR.RowID = m.RowID ), ',' ) AS q
			   WHERE EXISTS (SELECT TOP 1 1 FROM  @TBL_SavecartLineitems TRTR   WHERE TRTR.SKU = m.SKU AND TRTR.RowID = m.RowID AND TRTR.PersonalisedAttribute IS NOT NULL )
			   AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem s WHERE s.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId )-- AND s.PersonalizeCode = SUBSTRING(q.Item, 1, CHARINDEX('~', q.Item)-1) 
																																	   --AND s.PersonalizeValue = SUBSTRING(q.Item, CHARINDEX('~', q.Item)+1, 4000))
			   ;

			  

	SET @Status = 1;
	COMMIT TRAN InsertUpdateSaveCartLineItem;
	END TRY
	BEGIN CATCH

		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_InsertUpdateSaveCartLineItemQuantity @CartLineItemXML = '+CAST(@CartLineItemXML
 AS varchar(max))+',@UserId = '+CAST(@UserId AS varchar(50))+',@Status='+CAST(@Status AS varchar(10));

		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		ROLLBACK TRAN InsertUpdateSaveCartLineItem;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_InsertUpdateSaveCartLineItemQuantity', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;

GO


IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetCategoryHierarchy')
BEGIN 
	DROP PROCEDURE Znode_GetCategoryHierarchy
END 
GO


CREATE PROCEDURE [dbo].[Znode_GetCategoryHierarchy]
( @PimCatalogId     INT,
  @LocaleId         INT = NULL,
  @ProfileCatalogId INT = 0,
  @PimCategoryId    INT = NULL)
AS
/*
     Summary :- This Procedure is used to get category hierarchy 
     Unit Testing 
     EXEC [dbo].[Znode_GetCategoryHierarchy] @PimCatalogId=2,@LocaleId=1,@ProfileCatalogId=0
	
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             SET @LocaleId = dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_ProfileCatalogCategory TABLE
             (ProfileCatalogId       INT,
              PimCategoryHierarchyId INT
             );
             IF @ProfileCatalogId > 0
                 BEGIN
                     INSERT INTO @TBL_ProfileCatalogCategory(ProfileCatalogId,PimCategoryHierarchyId)
                            SELECT ZPC.ProfileCatalogId,ZPCH.PimCategoryHierarchyId                                  
                            FROM ZnodeProfileCategoryHierarchy ZPCH
                            INNER JOIN ZnodeProfileCatalog ZPC ON(ZPCH.ProfileCatalogId = ZPC.ProfileCatalogId)
                            WHERE ZPC.ProfileCatalogId = @ProfileCatalogId;
                 END;

             DECLARE @TBL_PimCategoryHierarchy TABLE
             (PimCategoryHierarchyId INT,
              PimCatalogId           INT,
              PimCategoryId          INT,
              CategoryValue          NVARCHAR(600),
              ParentPimCategoryHierarchyId    INT,
              DisplayOrder           INT
             );
             INSERT INTO @TBL_PimCategoryHierarchy
                    SELECT a.PimCategoryHierarchyId,a.PimCatalogId,a.PimCategoryId,VIPCAV.CategoryValue,ParentPimCategoryHierarchyId,ISNULL(a.DisplayOrder, 0)
                    FROM ZnodePimCategoryHierarchy AS a
                    LEFT JOIN ZnodePimCatalogCategory AS s ON(a.PimCatalogId = s.PimCatalogId
                                                              AND a.PimCategoryId = s.PimCategoryId)
                    LEFT JOIN View_PimCategoryAttributeValue VIPCAV ON (VIPCAV.PimCategoryId = a.PimCategoryId
                                                                       AND VIPCAV.LocaleId = @LocaleId)
                    WHERE A.PimCatalogId = @PimCatalogId
                          AND VIPCAV.AttributeCode = 'CategoryName'
                          AND (EXISTS
                              (
                                  SELECT TOP 1 1
                                  FROM @TBL_ProfileCatalogCategory TBPCC
                                  WHERE TBPCC.PimCategoryHierarchyId = a.PimCategoryHierarchyId
                              )
                               OR @ProfileCatalogId = 0)
                    GROUP BY a.PimCategoryHierarchyId,a.PimCatalogId,a.PimCategoryId,VIPCAV.CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder
                    ORDER BY a.PimCatalogId,a.PimCategoryId,a.DisplayOrder;

             IF @PimCategoryId IS NULL
                 BEGIN
                     SELECT Category.PimCategoryHierarchyId,Category.PimCatalogId,Category.PimCategoryId,Category.CategoryValue,Category.ParentPimCategoryHierarchyId,Category.DisplayOrder
                     FROM
                     (
                         SELECT 0 AS PimCategoryHierarchyId,a.PimCatalogId,0 AS PimCategoryId,a.CatalogName AS CategoryValue,-1 AS ParentPimCategoryHierarchyId,0 AS DisplayOrder
                         FROM ZnodePimCatalog AS a
                         WHERE a.PimCatalogId = @PimCatalogId
                         UNION ALL
                         SELECT a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder     
                         FROM @TBL_PimCategoryHierarchy AS a
                              LEFT JOIN ZnodePimCatalogCategory AS s ON(a.PimCatalogId = s.PimCatalogId
                                                                        AND a.PimCategoryId = s.PimCategoryId
                                                                        AND S.PimProductId IS NULL)
						GROUP BY a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder     
                     ) AS Category 
                     ORDER BY Category.DisplayOrder, PimCategoryId;
                             
                 END;
             ELSE
                 BEGIN
				  
                     SELECT Category.PimCategoryHierarchyId,Category.PimCatalogId,Category.PimCategoryId,Category.CategoryValue,Category.ParentPimCategoryHierarchyId,Category.DisplayOrder
                     FROM
                     (
                         SELECT DISTINCT 0 AS PimCategoryHierarchyId,a.PimCatalogId,0 AS PimCategoryId,a.CatalogName AS CategoryValue,NULL AS ParentPimCategoryHierarchyId,0 AS DisplayOrder
                         FROM ZnodePimCatalog AS a
                         WHERE a.PimCatalogId = @PimCatalogId
                         UNION ALL
                         SELECT a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder     
                         FROM @TBL_PimCategoryHierarchy AS a
                              LEFT JOIN ZnodePimCatalogCategory AS s ON(a.PimCatalogId = s.PimCatalogId
                                                                        AND a.PimCategoryId = s.PimCategoryId
                                                                        AND S.PimProductId IS NULL)
						 GROUP BY a.PimCategoryHierarchyId ,a.PimCatalogId,a.PimCategoryId,CategoryValue,ParentPimCategoryHierarchyId,a.DisplayOrder) AS Category
                         WHERE Category.ParentPimCategoryHierarchyId = @PimCategoryId                    
                         ORDER BY Category.DisplayOrder,PimCategoryId;
                              
                 END;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryHierarchy @PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@ProfileCatalogId='+CAST(@ProfileCatalogId AS VARCHAR(50))+',@PimCategoryId='+CAST(@PimCategoryId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCategoryHierarchy',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END

	 GO

	 
INSERT INTO ZnodePimAttributeGroupMapper
		(PimAttributeGroupId,PimAttributeId,AttributeDisplayOrder,IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		Select PimAttributeGroupId,
		(select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode') PimAttributeId,
		AttributeDisplayOrder,	IsSystemDefined,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate 
		from ZnodePimAttributeGroupMapper ZPAG  where PimAttributeId in  
		(select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryName')
		AND  NOT EXISTS (select * from ZnodePimAttributeGroupMapper X where X.PimAttributeGroupId =
		ZPAG.PimAttributeGroupId  AND
		X.PimAttributeId = (select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode') )
		

		insert into ZnodePimFamilyGroupMapper(PimAttributeFamilyId	,PimAttributeGroupId	,PimAttributeId	,GroupDisplayOrder	,IsSystemDefined	,CreatedBy	,
		CreatedDate	,ModifiedBy	,ModifiedDate)
		select PimAttributeFamilyId,PimAttributeGroupId, (select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode')
		,GroupDisplayOrder	,IsSystemDefined	,CreatedBy	,
		CreatedDate	,ModifiedBy	,ModifiedDate
		from ZnodePimFamilyGroupMapper ZPFGM
		where PimAttributeId in  
		(select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryName')
		AND 
		NOT EXISTS (select * from ZnodePimFamilyGroupMapper where PimAttributeFamilyId =
		ZPFGM.PimAttributeFamilyId AND PimAttributeGroupId = ZPFGM.PimAttributeGroupId 
		and PimAttributeId=  (select PimAttributeId from znodePimattribute where AttributeCode = 'CategoryCode') )

