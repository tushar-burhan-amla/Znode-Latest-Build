
CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItem](
	  @CartLineItemXML xml, @UserId int,@Status bit OUT )
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
		DECLARE @IsAddProduct   BIT = 0 
		
		DECLARE @TBL_SavecartLineitems TABLE
		( 
			RowId int , OmsSavedCartLineItemId int, ParentOmsSavedCartLineItemId int, OmsSavedCartId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), 
			CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute XML, 
			AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max),
			Custom1	nvarchar(max),Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4
			nvarchar(max),Custom5 nvarchar(max),GroupId NVARCHAR(max) ,ProductName Nvarchar(1000) , Description NVARCHAR(max)
		);
		DECLARE @OrderLineItemRelationshipTypeIdAddon int =
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'AddOns'
		);
		DECLARE @OrderLineItemRelationshipTypeIdSimple int =
		(
			SELECT TOP 1 OrderLineItemRelationshipTypeId
			FROM ZnodeOmsOrderLineItemRelationshipType
			WHERE [Name] = 'Simple'
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
		INSERT INTO @TBL_SavecartLineitems( RowId,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, AddOnValueIds, BundleProductIds, ConfigurableProductIds, GroupProductIds, PersonalisedAttribute, AutoAddon, OmsOrderId, ItemDetails,
		Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,Description )
			   SELECT DENSE_RANK()Over(Order BY Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' )) RowId ,Tbl.Col.value( 'OmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartLineItemId, Tbl.Col.value( 'ParentOmsSavedCartLineItemId[1]', 'NVARCHAR(2000)' ) AS ParentOmsSavedCartLineItemId, Tbl.Col.value( 'OmsSavedCartId[1]', 'NVARCHAR(2000)' ) AS OmsSavedCartId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'Quantity[1]', 'NVARCHAR(2000)' ) AS Quantity
			   , Tbl.Col.value( 'OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)' ) AS OrderLineItemRelationshipTypeID, Tbl.Col.value( 'CustomText[1]', 'NVARCHAR(2000)' ) AS CustomText, Tbl.Col.value( 'CartAddOnDetails[1]', 'NVARCHAR(2000)' ) AS CartAddOnDetails, Tbl.Col.value( 'Sequence[1]', 'NVARCHAR(2000)' ) AS Sequence, Tbl.Col.value( 'AddonProducts[1]', 'NVARCHAR(2000)' ) AS AddOnValueIds, ISNULL(Tbl.Col.value( 'BundleProducts[1]', 'NVARCHAR(2000)' ),'') AS BundleProductIds, ISNULL(Tbl.Col.value( 'ConfigurableProducts[1]', 'NVARCHAR(2000)' ),'') AS ConfigurableProductIds, ISNULL(Tbl.Col.value( 'GroupProducts[1]', 'NVARCHAR(Max)' ),'') AS GroupProductIds, 
			          Tbl.Col.query('(PersonaliseValuesDetail/node())') AS PersonaliseValuesDetail, Tbl.Col.value( 'AutoAddon[1]', 'NVARCHAR(Max)' ) AS AutoAddon, Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(Max)' ) AS OmsOrderId,
					  Tbl.Col.value( 'ItemDetails[1]', 'NVARCHAR(Max)' ) AS ItemDetails,
					  Tbl.Col.value( 'Custom1[1]', 'NVARCHAR(Max)' ) AS Custom1,
					  Tbl.Col.value( 'Custom2[1]', 'NVARCHAR(Max)' ) AS Custom2,
					  Tbl.Col.value( 'Custom3[1]', 'NVARCHAR(Max)' ) AS Custom3,
					  Tbl.Col.value( 'Custom4[1]', 'NVARCHAR(Max)' ) AS Custom4,
					  Tbl.Col.value( 'Custom5[1]', 'NVARCHAR(Max)' ) AS Custom5,
					  Tbl.Col.value( 'GroupId[1]', 'NVARCHAR(Max)' ) AS GroupId,
					  Tbl.Col.value( 'ProductName[1]', 'NVARCHAR(Max)' ) AS ProductName,
					  Tbl.Col.value( 'Description[1]', 'NVARCHAR(Max)' ) AS Description
			   FROM @CartLineItemXML.nodes( '//ArrayOfSavedCartLineItemModel/SavedCartLineItemModel' ) AS Tbl(Col);

			    DELETE FROM ZnodeOmsPersonalizeCartItem
			WHERE EXISTS
			(
				SELECT TOP 1 1
				FROM ZnodeOmsSavedCartLineItem TYF
				WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_SavecartLineitems TY WHERE TYF.OmsSavedCartId = TY.OmsSavedCartId  ) 
				AND TYF.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId
			);
	  DELETE ZnodeOmsSavedCartLineItemDetails
				WHERE EXISTS
			(
				SELECT TOP 1 1
				FROM ZnodeOmsSavedCartLineItem TYF
				WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_SavecartLineitems TY WHERE TYF.OmsSavedCartId = TY.OmsSavedCartId  ) 
				AND TYF.OmsSavedCartLineItemId = ZnodeOmsSavedCartLineItemDetails.OmsSavedCartLineItemId
			)
	
		EXEC Znode_InsertUpdateSaveCartLineItemQuantity @CartLineItemXML , @UserId, @Status
        		
	SET @Status = 1;
	COMMIT TRAN InsertUpdateSaveCartLineItem;
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_InsertUpdateSaveCartLineItem @CartLineItemXML = '+CAST(@CartLineItemXML
 AS varchar(max))+',@UserId = '+CAST(@UserId AS varchar(50))+',@Status='+CAST(@Status AS varchar(10));

		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		ROLLBACK TRAN InsertUpdateSaveCartLineItem;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_InsertUpdateSaveCartLineItem', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;