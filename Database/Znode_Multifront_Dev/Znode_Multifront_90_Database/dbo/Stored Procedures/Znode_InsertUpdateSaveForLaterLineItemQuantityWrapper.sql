CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveForLaterLineItemQuantityWrapper]
(
	@TemplateLineItemXML XML,
    @UserId              INT,
    @Status              BIT = 0 OUT
)
AS 
   /* 
    Summary: THis Procedure is USed to save and edit the saved cart line item      
    Unit Testing 
	begin tran  
    Exec Znode_InsertUpdateSaveForLaterLineItem @TemplateLineItemXML= '<ArrayOfSavedCartLineItemModel>
  <SavedCartLineItemModel>
    <OmsTemplateLineItemId>0</OmsTemplateLineItemId>
    <ParentOmsTemplateLineItemId>0</ParentOmsTemplateLineItemId>
    <OmsTemplateId>1259</OmsTemplateId>
    <SKU>BlueGreenYellow</SKU>
    <Quantity>1.000000</Quantity>
    <OrderLineItemRelationshipTypeId>0</OrderLineItemRelationshipTypeId>
    <Sequence>1</Sequence>
    <AddonProducts>YELLOW</AddonProducts>
    <BundleProducts />
    <ConfigurableProducts>GREEN</ConfigurableProducts>
  </SavedCartLineItemModel>
  <SavedCartLineItemModel>
    <OmsTemplateLineItemId>0</OmsTemplateLineItemId>
    <ParentOmsTemplateLineItemId>0</ParentOmsTemplateLineItemId>
    <OmsTemplateId>1259</OmsTemplateId>
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
BEGIN TRAN InsertUpdateSaveForLaterLineItem;
BEGIN TRY
SET NOCOUNT ON;

	--Declared the variables
	DECLARE @GetDate datetime= dbo.Fn_GetDate();
	DECLARE @AddOnQuantity numeric(28, 6)= 0;
	DECLARE @IsAddProduct   BIT = 0 
	DECLARE @OmsTemplateLineItemId INT = 0
	
	DECLARE @TBL_SaveForLaterLineitems TABLE
	( 
		RowId int IDENTITY(1,1), OmsTemplateLineItemId int, ParentOmsTemplateLineItemId int, OmsTemplateId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), 
		CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute XML, 
		AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max),
		Custom1	nvarchar(max),Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4
		nvarchar(max),Custom5 nvarchar(max),GroupId NVARCHAR(max) ,ProductName Nvarchar(1000) , Description NVARCHAR(max),AddOnQuantity NVARCHAR(max), CustomUnitPrice numeric(28, 6)
	);

	--Set the OrderLineItemRelationshipTypeId for Addons product
	DECLARE @OrderLineItemRelationshipTypeIdAddon int =
	(
		SELECT TOP 1 OrderLineItemRelationshipTypeId
		FROM ZnodeOmsOrderLineItemRelationshipType
		WHERE [Name] = 'AddOns'
	);
	--Set the OrderLineItemRelationshipTypeId for simple product
	DECLARE @OrderLineItemRelationshipTypeIdSimple int =
	(
		SELECT TOP 1 OrderLineItemRelationshipTypeId
		FROM ZnodeOmsOrderLineItemRelationshipType
		WHERE [Name] = 'Simple'
	);
	--Set the OrderLineItemRelationshipTypeId for group product
	DECLARE @OrderLineItemRelationshipTypeIdGroup int=
	(
		SELECT TOP 1 OrderLineItemRelationshipTypeId
		FROM ZnodeOmsOrderLineItemRelationshipType
		WHERE [Name] = 'Group'
	);
	--Set the OrderLineItemRelationshipTypeId for Configurable product
	DECLARE @OrderLineItemRelationshipTypeIdConfigurable int=
	(
		SELECT TOP 1 OrderLineItemRelationshipTypeId
		FROM ZnodeOmsOrderLineItemRelationshipType
		WHERE [Name] = 'Configurable'
	);
	--Set the OrderLineItemRelationshipTypeId for Bundles product	
	DECLARE @OrderLineItemRelationshipTypeIdBundle int=
	(
		SELECT TOP 1 OrderLineItemRelationshipTypeId
		FROM ZnodeOmsOrderLineItemRelationshipType
		WHERE [Name] = 'Bundles'
	);

	

	--Fetching data FROM xml to table format and inserted into table @TBL_SaveForLaterLineitems
	INSERT INTO @TBL_SaveForLaterLineitems( OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU, Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence, AddOnValueIds, BundleProductIds, ConfigurableProductIds, GroupProductIds, PersonalisedAttribute, AutoAddon, OmsOrderId, ItemDetails,
	Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,Description,AddOnQuantity, CustomUnitPrice )
	SELECT Tbl.Col.value( 'OmsTemplateLineItemId[1]', 'NVARCHAR(2000)' ) AS OmsTemplateLineItemId, Tbl.Col.value( 'ParentOmsTemplateLineItemId[1]', 'NVARCHAR(2000)' ) AS ParentOmsTemplateLineItemId, 
	Tbl.Col.value('OmsTemplateId[1]', 'NVARCHAR(2000)')  AS OmsTemplateId, Tbl.Col.value( 'SKU[1]', 'NVARCHAR(2000)' ) AS SKU, Tbl.Col.value( 'Quantity[1]', 'NVARCHAR(2000)' ) AS Quantity
	, Tbl.Col.value( 'OrderLineItemRelationshipTypeID[1]', 'NVARCHAR(2000)' ) AS OrderLineItemRelationshipTypeID, Tbl.Col.value( 'CustomText[1]', 'NVARCHAR(2000)' ) AS CustomText, Tbl.Col.value( 'CartAddOnDetails[1]', 'NVARCHAR(2000)' ) AS CartAddOnDetails, Tbl.Col.value( 'Sequence[1]', 'NVARCHAR(2000)' ) AS Sequence, s.Item AS AddOnValueIds, ISNULL(Tbl.Col.value( 'BundleProducts[1]', 'NVARCHAR(2000)' ),'') AS BundleProductIds, ISNULL(Tbl.Col.value( 'ConfigurableProducts[1]', 'NVARCHAR(2000)' ),'') AS ConfigurableProductIds, ISNULL(Tbl.Col.value( 'GroupProducts[1]', 'NVARCHAR(Max)' ),'') AS GroupProductIds, 
			Tbl.Col.query('(PersonaliseValuesDetail/node())') AS PersonaliseValuesDetail, Tbl.Col.value( 'AutoAddon[1]', 'NVARCHAR(Max)' ) AS AutoAddon, Tbl.Col.value( 'OmsOrderId[1]', 'NVARCHAR(Max)' ) AS OmsOrderId,
			Tbl.Col.value( 'ItemDetails[1]', 'NVARCHAR(Max)' ) AS ItemDetails,
			Tbl.Col.value( 'Custom1[1]', 'NVARCHAR(Max)' ) AS Custom1,
			Tbl.Col.value( 'Custom2[1]', 'NVARCHAR(Max)' ) AS Custom2,
			Tbl.Col.value( 'Custom3[1]', 'NVARCHAR(Max)' ) AS Custom3,
			Tbl.Col.value( 'Custom4[1]', 'NVARCHAR(Max)' ) AS Custom4,
			Tbl.Col.value( 'Custom5[1]', 'NVARCHAR(Max)' ) AS Custom5,
			Tbl.Col.value( 'GroupId[1]', 'NVARCHAR(Max)' ) AS GroupId,
			Tbl.Col.value( 'ProductName[1]', 'NVARCHAR(Max)' ) AS ProductName,
			Tbl.Col.value( 'Description[1]', 'NVARCHAR(Max)' ) AS Description, 
			Tbl.Col.value( 'AddOnQuantity[1]', 'NVARCHAR(2000)' ) AS AddOnQuantity,
			CASE WHEN ISNULL(Tbl.Col.value( 'CustomUnitPrice[1]', 'NVARCHAR(2000)' ),'') = '' THEN NULL ELSE Tbl.Col.value( 'CustomUnitPrice[1]', 'NVARCHAR(2000)' ) END AS CustomUnitPrice
	FROM @TemplateLineItemXML.nodes( '//ArrayOfAccountTemplateLineItemModel/AccountTemplateLineItemModel' ) AS Tbl(Col)
	CROSS APPLY dbo.split(Tbl.Col.value( 'AddonProducts[1]', 'NVARCHAR(2000)' ),',') as S;

	IF OBJECT_ID('tempdb..#TBL_SaveForLaterLineitems') is not null
		DROP TABLE #TBL_SaveForLaterLineitems

	IF OBJECT_ID('tempdb..#OldValueForAddon') is not null
		DROP TABLE #OldValueForAddon

	SELECT * INTO #TBL_SaveForLaterLineitems FROM @TBL_SaveForLaterLineitems
			
	UPDATE ZnodeOmsTemplate
	SET ModifiedDate = @GetDate
	WHERE OmsTemplateId = (SELECT TOP 1  OmsTemplateId FROM @TBL_SaveForLaterLineitems)

	UPDATE  @TBL_SaveForLaterLineitems
	SET Description = ISNUll(Description,'') 

	--Save cart execution code for bundle product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE BundleProductIds <> '' )
	BEGIN 				
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE BundleProductIds <> '' AND OmsTemplateLineItemId <> 0  ) 
		BEGIN 
			SET @OmsTemplateLineItemId  = (SELECT TOP 1 OmsTemplateLineItemId FROM @TBL_SaveForLaterLineitems WHERE BundleProductIds <> '' AND OmsTemplateLineItemId <> 0 )

			UPDATE ZnodeOmsTemplateLineItem 
			SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SaveForLaterLineitems WHERE BundleProductIds <> '' AND OmsTemplateLineItemId <> 0)
			, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
			WHERE ( OmsTemplateLineItemId = @OmsTemplateLineItemId  
			OR ParentOmsTemplateLineItemId =  @OmsTemplateLineItemId   ) 

			UPDATE ZnodeOmsTemplateLineItem 
			SET Quantity = AddOnQuantity, ModifiedDate = @GetDate,CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
			FROM ZnodeOmsTemplateLineItem ZOSCLI with (nolock)
			INNER JOIN @TBL_SaveForLaterLineitems SCLI ON ZOSCLI.ParentOmsTemplateLineItemId = SCLI.OmsTemplateLineItemId AND ZOSCLI.OmsTemplateId = SCLI.OmsTemplateId AND ZOSCLI.SKU = SCLI.AddOnValueIds
			WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
			AND SCLI.BundleProductIds <> ''

			--After update the existing cart with new save cart then deleting those records which are updated
			DELETE	FROM @TBL_SaveForLaterLineitems WHERE BundleProductIds <> '' AND OmsTemplateLineItemId <> 0
		END 

		--Getting bundle save cart line item entries into table to pass for bundle prodcut sp
		DECLARE @TBL_bundleProduct SaveForLaterLineitems 
		INSERT INTO @TBL_bundleProduct 
		SELECT *  
		FROM @TBL_SaveForLaterLineitems 
		WHERE ISNULL(BundleProductIds,'') <> '' 
				
		EXEC Znode_InsertUpdateSaveForLaterLineItemBundle @TBL_bundleProduct,@userId,@OrderLineItemRelationshipTypeIdBundle,@OrderLineItemRelationshipTypeIdAddon
				 
		DELETE FROM  @TBL_SaveForLaterLineitems WHERE ISNULL(BundleProductIds,'') <> '' 
		SET @OmsTemplateLineItemId = 0 
	END 
	--Save cart execution code for configurable product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE ConfigurableProductIds <> '' )
	BEGIN 				
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE ConfigurableProductIds <> '' AND OmsTemplateLineItemId <> 0  ) 
		BEGIN 

			SET @OmsTemplateLineItemId  = (SELECT TOP 1 OmsTemplateLineItemId FROM @TBL_SaveForLaterLineitems WHERE ConfigurableProductIds <> '' AND OmsTemplateLineItemId <> 0 )
				 
			UPDATE ZnodeOmsTemplateLineItem 
			SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SaveForLaterLineitems WHERE ConfigurableProductIds <> '' AND OmsTemplateLineItemId = @OmsTemplateLineItemId )
			, ModifiedDate = @GetDate,CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
			WHERE OmsTemplateLineItemId = @OmsTemplateLineItemId

			UPDATE ZnodeOmsTemplateLineItem 
			SET Quantity = AddOnQuantity, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
			FROM ZnodeOmsTemplateLineItem ZOSCLI with (nolock)
			INNER JOIN @TBL_SaveForLaterLineitems SCLI ON ZOSCLI.ParentOmsTemplateLineItemId = SCLI.OmsTemplateLineItemId AND ZOSCLI.OmsTemplateId = SCLI.OmsTemplateId AND ZOSCLI.SKU = SCLI.AddOnValueIds
			WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
			AND SCLI.ConfigurableProductIds <> ''

			--After update the existing cart with new save cart then deleting those records which are updated
			DELETE	FROM @TBL_SaveForLaterLineitems WHERE ConfigurableProductIds <> '' AND OmsTemplateLineItemId <> 0
		END 
		--Getting bundle save cart line item entries into table to pass for configurable prodcut sp
		DECLARE @TBL_Configurable SaveForLaterLineitems 
		INSERT INTO @TBL_Configurable 
		SELECT *  
		FROM @TBL_SaveForLaterLineitems 
		WHERE ISNULL(ConfigurableProductIds,'') <> '' 
		  
		EXEC Znode_InsertUpdateSaveForLaterLineItemConfigurable @TBL_Configurable,@userId,@OrderLineItemRelationshipTypeIdConfigurable,@OrderLineItemRelationshipTypeIdAddon
				  
		DELETE FROM @TBL_SaveForLaterLineitems 
		WHERE ISNULL(ConfigurableProductIds,'') <> ''
		SET @OmsTemplateLineItemId = 0  
	END 
	--Save cart execution code for group product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE GroupProductIds <> '' )
	BEGIN 				
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE GroupProductIds <> '' AND OmsTemplateLineItemId <> 0  ) 
		BEGIN 
			--Updating the existing save cart for group product
			SET @OmsTemplateLineItemId  = (SELECT TOP 1 OmsTemplateLineItemId FROM @TBL_SaveForLaterLineitems WHERE GroupProductIds <> '' AND OmsTemplateLineItemId <> 0 )
			UPDATE ZnodeOmsTemplateLineItem 
			SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SaveForLaterLineitems WHERE GroupProductIds <> '' AND OmsTemplateLineItemId = @OmsTemplateLineItemId ), CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
			WHERE OmsTemplateLineItemId = @OmsTemplateLineItemId

			UPDATE ZnodeOmsTemplateLineItem 
			SET Quantity = AddOnQuantity, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
			FROM ZnodeOmsTemplateLineItem ZOSCLI with (nolock)
			INNER JOIN @TBL_SaveForLaterLineitems SCLI ON ZOSCLI.ParentOmsTemplateLineItemId = SCLI.OmsTemplateLineItemId AND ZOSCLI.OmsTemplateId = SCLI.OmsTemplateId AND ZOSCLI.SKU = SCLI.AddOnValueIds
			WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
			AND SCLI.GroupProductIds <> ''

			--After update the existing cart with new save cart then deleting those records which are updated
			DELETE	FROM @TBL_SaveForLaterLineitems WHERE GroupProductIds <> '' AND OmsTemplateLineItemId <> 0
		END 
		--Getting bundle save cart line item entries into table to pass for group prodcut sp
		DECLARE @TBL_Group SaveForLaterLineitems 
		INSERT INTO @TBL_Group 
		SELECT *  
		FROM @TBL_SaveForLaterLineitems 
		WHERE ISNULL(GroupProductIds,'') <> '' 
		
		EXEC Znode_InsertUpdateSaveForLaterLineItemGroup @TBL_Group,@userId,@OrderLineItemRelationshipTypeIdGroup,@OrderLineItemRelationshipTypeIdAddon
		
		--After update the existing cart with new save cart then deleting those records which are updated
		DELETE FROM @TBL_SaveForLaterLineitems WHERE ISNULL(GroupProductIds,'') <> ''
		SET @OmsTemplateLineItemId = 0  
	END 
	
	--This part is for updating the cart for existing line items for simple product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE  OmsTemplateLineItemId <> 0  ) 
	BEGIN 
				 
		SET @OmsTemplateLineItemId  = (SELECT TOP 1 OmsTemplateLineItemId FROM @TBL_SaveForLaterLineitems WHERE  OmsTemplateLineItemId <> 0 )
		UPDATE ZnodeOmsTemplateLineItem 
		SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SaveForLaterLineitems WHERE  OmsTemplateLineItemId = @OmsTemplateLineItemId )
		, ModifiedDate = @GetDate , CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
		WHERE OmsTemplateLineItemId = @OmsTemplateLineItemId

		DECLARE @ParentOmsTemplateLineItemId INT = 0
		SET @ParentOmsTemplateLineItemId = (select ParentOmsTemplateLineItemId from ZnodeOmsTemplateLineItem WHERE OmsTemplateLineItemId = @OmsTemplateLineItemId)

		UPDATE ZnodeOmsTemplateLineItem 
		SET  CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SaveForLaterLineitems)
		WHERE OmsTemplateLineItemId = @ParentOmsTemplateLineItemId

		UPDATE ZnodeOmsTemplateLineItem 
		SET Quantity = AddOnQuantity, ModifiedDate = @GetDate
		FROM ZnodeOmsTemplateLineItem ZOSCLI with (nolock)
		INNER JOIN @TBL_SaveForLaterLineitems SCLI ON ZOSCLI.ParentOmsTemplateLineItemId = @OmsTemplateLineItemId AND ZOSCLI.OmsTemplateId = SCLI.OmsTemplateId AND ZOSCLI.SKU = SCLI.AddOnValueIds
		WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					
		DELETE	FROM @TBL_SaveForLaterLineitems WHERE OmsTemplateLineItemId <> 0
	END 
	--Save cart execution code for Simple product
	DECLARE @OmsInsertedData TABLE (OmsTemplateLineItemId INT )
	--Inserting the personalise data into variable table @TBL_Personalise for inserting the personalise data for new products
	DECLARE @TBL_Personalise TABLE (OmsTemplateLineItemId INT, ParentOmsTemplateLineItemId int,SKU Varchar(600) ,PersonalizeCode NVARCHAr(max),PersonalizeValue NVARCHAr(max),DesignId NVARCHAr(max), ThumbnailURL NVARCHAr(max),PersonalizeName NVARCHAR(max))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsTemplateLineItemId,a.SKU
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
			,Tbl.Col.value( 'PersonalizeName[1]', 'NVARCHAR(Max)' ) AS PersonalizeName
	FROM @TBL_SaveForLaterLineitems a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col) 
			  
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise1 TABLE (OmsTemplateLineItemId INT ,PersonalizeCode NVARCHAr(max),PersonalizeValue NVARCHAr(max),DesignId NVARCHAr(max), ThumbnailURL NVARCHAr(max),PersonalizeName NVARCHAR(max))
	INSERT INTO @TBL_Personalise1
	SELECT DISTINCT a.OmsTemplateLineItemId 
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
			,Tbl.Col.value( 'PersonalizeName[1]', 'NVARCHAR(Max)' ) AS PersonalizeName
	FROM (SELECT TOP 1 OmsTemplateLineItemId,PersonalisedAttribute Valuex FROM  #TBL_SaveForLaterLineitems TRTR ) a 
	CROSS APPLY	a.Valuex.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  
		    
			
	CREATE TABLE #NewSaveForLaterLineitemDetails 
	(
		GenId INT IDENTITY(1,1),RowId	INT	,OmsTemplateLineItemId	INT	 ,ParentOmsTemplateLineItemId	INT,OmsTemplateId	INT
		,SKU	NVARCHAR(MAX) ,Quantity	NUMERIC(28,6)	,OrderLineItemRelationshipTypeID	INT	,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	int	,AutoAddon	varchar(MAX)	,OmsOrderId	INT	,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX)  ,Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX)
	)
	
	--Getting new save cart data
	INSERT INTO #NewSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU 
	FROM @TBL_SaveForLaterLineitems a 
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName
	
	--Getting new simple product save cart data
	INSERT INTO #NewSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, @OrderLineItemRelationshipTypeIdSimple, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU 
	FROM @TBL_SaveForLaterLineitems  a 
	WHERE ISNULL(BundleProductIds,'') =  '' 
	AND  ISNULL(GroupProductIds,'') = ''	AND ISNULL(	ConfigurableProductIds,'') = ''
		GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity,  CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName
			
	--Getting new Group,Bundle and Configurable products save cart data if addon is present for any line item
	INSERT INTO #NewSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
		,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
		,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
		WHEN  GroupProductIds <> '' THEN GroupProductIds 
		WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END     ParentSKU 
	FROM @TBL_SaveForLaterLineitems  a 
	WHERE AddOnValueIds <> ''
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
	,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
	,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU
	
	CREATE TABLE #OldSaveForLaterLineitemDetails (OmsTemplateId INT ,OmsTemplateLineItemId INT,ParentOmsTemplateLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
	--Getting the old save cart data if present for same SKU in the new save cart data for simple product	 
	INSERT INTO #OldSaveForLaterLineitemDetails  
	SELECT  a.OmsTemplateId,a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	FROM ZnodeOmsTemplateLineItem a with (nolock)  
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems  TY WHERE TY.OmsTemplateId = a.OmsTemplateId AND ISNULL(a.SKU,'') = ISNULL(TY.SKU,'')   )   
	AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple   

	--Getting the old save cart Parent data 
	INSERT INTO #OldSaveForLaterLineitemDetails 
	SELECT DISTINCT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsTemplateLineItem b with (nolock)
	INNER JOIN #OldSaveForLaterLineitemDetails c ON (c.ParentOmsTemplateLineItemId  = b.OmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems  TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
	AND  b.OrderLineItemRelationshipTypeID IS NULL 
		 
	DELETE a FROM #OldSaveForLaterLineitemDetails a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldSaveForLaterLineitemDetails b WHERE b.ParentOmsTemplateLineItemId IS NULL AND b.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)
	AND a.ParentOmsTemplateLineItemId IS NOT NULL 
		
	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldSaveForLaterLineitemDetails
		
	--Getting the old save cart addon data for old line items if present
	INSERT INTO #OldSaveForLaterLineitemDetails 
	SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsTemplateLineItem b with (nolock)
	INNER JOIN #OldSaveForLaterLineitemDetails c ON (c.OmsTemplateLineItemId  = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems  TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
	AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

	------Merge Addon for same product
	IF EXISTS(SELECT * FROM @TBL_SaveForLaterLineitems WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN

		INSERT INTO #OldValueForAddon 
		SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
		FROM ZnodeOmsTemplateLineItem b with (nolock)
		INNER JOIN #OldValueForAddon c ON (c.OmsTemplateLineItemId  = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems  TY WHERE TY.OmsTemplateId = b.OmsTemplateId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		SELECT distinct SKU, STUFF(
								( SELECT  ', ' + SKU FROM    
									( SELECT DISTINCT SKU FROM     #OldValueForAddon b 
										where a.OmsTemplateLineItemId=b.ParentOmsTemplateLineItemId and OrderLineItemRelationshipTypeID = 1 ) x 
										FOR XML PATH('')
								), 1, 2, ''
								) AddOns
		INTO #AddOnsExists
		FROM #OldValueForAddon a where a.ParentOmsTemplateLineItemId is not null and OrderLineItemRelationshipTypeID<>1

		SELECT distinct a.SKU, STUFF(
									( SELECT  ', ' + x.AddOnValueIds FROM    
									( SELECT DISTINCT b.AddOnValueIds FROM @TBL_SaveForLaterLineitems b
										where a.SKU=b.SKU ) x
										FOR XML PATH('')
									), 1, 2, ''
								) AddOns
		INTO #AddOnAdded
		FROM @TBL_SaveForLaterLineitems a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldSaveForLaterLineitemDetails
		END

	END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSaveForLaterLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_SaveForLaterLineitems ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldSaveForLaterLineitemDetails a WHERE	
	ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE ISNULL(AddOnValueIds,'')  <> '' )
	BEGIN 
		
		DELETE FROM #OldSaveForLaterLineitemDetails 
	END 
	ELSE 
	BEGIN 
	    
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN 
		 
			DECLARE @parenTofAddon  TABLE( ParentOmsTemplateLineItemId INT  )  
			INSERT INTO  @parenTofAddon 
			SELECT  ParentOmsTemplateLineItemId FROM #OldSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  

			DELETE FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT ParentOmsTemplateLineItemId FROM  @parenTofAddon)   
				AND ParentOmsTemplateLineItemId IS NOT NULL  
				AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

			DELETE FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldSaveForLaterLineitemDetails m)
			AND ParentOmsTemplateLineItemId IS  NULL  
		 
		END 
		ELSE IF (SELECT COUNT (OmsTemplateLineItemId) FROM #OldSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS NULL ) > 1 
		BEGIN 

			DECLARE @TBL_deleteParentOmsTemplateLineItemId TABLE (OmsTemplateLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsTemplateLineItemId 
			SELECT ParentOmsTemplateLineItemId
			FROM ZnodeOmsTemplateLineItem a with (nolock)
			WHERE ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM #OldSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple  )
			AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
			OR ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
		    
			DELETE FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldSaveForLaterLineitemDetails m)
			AND ParentOmsTemplateLineItemId IS  NULL  

		END
		ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplateLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldSaveForLaterLineitemDetails ty WHERE ty.OmsTemplateId = wt.OmsTemplateId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple AND wt.ParentOmsTemplateLineItemId= ty.OmsTemplateLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_SaveForLaterLineitems WHERE ISNULL(AddOnValueIds,'')  = '' )
		BEGIN 

			DELETE FROM #OldSaveForLaterLineitemDetails
		END 
	END 

	--Getting the personalise data for old save cart if present
	DECLARE @TBL_Personaloldvalues TABLE (OmsTemplateLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsTemplateLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsTemplatePersonalizeCartItem  a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldSaveForLaterLineitemDetails TY WHERE TY.OmsTemplateLineItemId = a.OmsTemplateLineItemId)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)

	IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN 
		DELETE FROM #OldSaveForLaterLineitemDetails
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		BEGIN 
		   
			DELETE FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId IN (
			SELECT OmsTemplateLineItemId FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
			AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues ) ) 
			OR OmsTemplateLineItemId IN ( SELECT ParentOmsTemplateLineItemId FROM #OldSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
			AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues ))
		      
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		BEGIN 

			DELETE n FROM #OldSaveForLaterLineitemDetails n WHERE OmsTemplateLineItemId  IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem WHERE n.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId  )
			OR ParentOmsTemplateLineItemId  IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem   )
	   
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplatePersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldSaveForLaterLineitemDetails YU WHERE YU.OmsTemplateLineItemId = m.OmsTemplateLineItemId )) 
		AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) = 1
		BEGIN 
			DELETE FROM #OldSaveForLaterLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		END 
	  
	END 

	--If already exists cart 
	IF EXISTS (SELECT TOP 1 1 FROM #OldSaveForLaterLineitemDetails )
	BEGIN
		----DELETE old value FROM table which having personalise data in ZnodeOmsTemplatePersonalizeCartItem but same SKU not having personalise value for new cart item
		;WITH cte AS
		(
			SELECT distinct b.*
			FROM @TBL_SaveForLaterLineitems a 
			INNER JOIN #OldSaveForLaterLineitemDetails b on ( a.SKU = b.sku)
			where isnull(cast(a.PersonalisedAttribute AS varchar(max)),'') = ''
		)
		,cte2 AS
		(
			SELECT c.ParentOmsTemplateLineItemId
			FROM #OldSaveForLaterLineitemDetails a
			INNER JOIN ZnodeOmsTemplateLineItem c on a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem b on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
		)
		DELETE a FROM #OldSaveForLaterLineitemDetails a
		INNER JOIN cte b on a.OmsTemplateLineItemId = b.OmsTemplateLineItemId
		INNER JOIN cte2 c on (a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or a.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId)

		----DELETE old value FROM table which having personalise data in ZnodeOmsTemplatePersonalizeCartItem but same SKU having personalise value for new cart item
		;WITH cte AS
		(
			SELECT distinct b.*, 
				a.PersonalizeCode
				,a.PersonalizeValue
			FROM @TBL_Personalise a 
			INNER JOIN #OldSaveForLaterLineitemDetails b on ( a.SKU = b.sku)
			where a.PersonalizeValue <> ''
		)
		,cte2 AS
		(
			SELECT a.ParentOmsTemplateLineItemId, b.PersonalizeCode, b.PersonalizeValue
			FROM #OldSaveForLaterLineitemDetails a
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem b on b.OmsTemplateLineItemId = a.OmsTemplateLineItemId
			WHERE NOT EXISTS(SELECT * FROM cte c where b.OmsTemplateLineItemId = c.OmsTemplateLineItemId and b.PersonalizeCode = c.PersonalizeCode 
						and b.PersonalizeValue = c.PersonalizeValue )
		)
		DELETE a FROM #OldSaveForLaterLineitemDetails a
		INNER JOIN cte b on a.OmsTemplateLineItemId = b.OmsTemplateLineItemId
		INNER JOIN cte2 c on (a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or a.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId)

		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,b.ParentOmsTemplateLineItemId , a.SKU AS SKU
				,a.PersonalizeCode
				,a.PersonalizeValue
				,a.DesignId
				,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldSaveForLaterLineitemDetails b on a.SKU = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
			WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE b1
		FROM #OldSaveForLaterLineitemDetails b1 
		WHERE NOT EXISTS(SELECT * FROM cte c where (b1.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or b1.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId))
		AND EXISTS(SELECT * FROM cte)

		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is same for same line item then Quantity will added
	-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsTemplateLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldSaveForLaterLineitemDetails b ON a.SKU = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c ON b.OmsTemplateLineItemId = c.OmsTemplateLineItemId 
			--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0
			GROUP BY b.OmsTemplateLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsTemplateLineItemId,0) as OmsTemplateLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewSaveForLaterLineitemDetails  b ON a.SKU = b.SKU
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0 AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple
			GROUP BY a.OmsTemplateLineItemId ,b.SKU
		)
		DELETE c
		from CTE_OldPersonalizeCodeCount a
		inner join CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		inner join #OldSaveForLaterLineitemDetails c on b.SKU = c.SKU and a.OmsTemplateLineItemId = c.OmsTemplateLineItemId
		
		--Delete parent entry if child not present
		DELETE a FROM #OldSaveForLaterLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldSaveForLaterLineitemDetails b where a.OmsTemplateLineItemId = b.ParentOmsTemplateLineItemId)
		AND a.ParentOmsTemplateLineItemId IS NULL

	--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
	--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is same for same line item then Quantity will added
	
		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,a.ParentOmsTemplateLineItemId , a.SKU
						,d.PersonalizeCode
				,d.PersonalizeValue
				,d.DesignId
				,d.ThumbnailURL
				FROM @TBL_SaveForLaterLineitems a 
				INNER JOIN #OldSaveForLaterLineitemDetails b on a.SKU = b.SKU
				INNER JOIN @TBL_Personalise d on d.SKU = a.SKU
				INNER JOIN ZnodeOmsTemplatePersonalizeCartItem  c  with (nolock)  on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
				WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE b1
		FROM cte a1		  
		INNER JOIN #OldSaveForLaterLineitemDetails b1 on a1.sku = b1.SKU
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsTemplatePersonalizeCartItem c where a1.OmsTemplateLineItemId = c.OmsTemplateLineItemId and a1.PersonalizeValue = c.PersonalizeValue)

		--Updating the cart if old and new cart data matches 
		UPDATE a
		SET a.Quantity = a.Quantity+ty.Quantity,
		a.Custom1 = ty.Custom1,
		a.Custom2 = ty.Custom2,
		a.Custom3 = ty.Custom3,
		a.Custom4 = ty.Custom4,
		a.Custom5 = ty.Custom5, 
		a.ModifiedDate = @GetDate
		FROM ZnodeOmsTemplateLineItem a
		INNER JOIN #OldSaveForLaterLineitemDetails b ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId)
		INNER JOIN #NewSaveForLaterLineitemDetails ty ON (ty.SKU = b.SKU)

	END 
	--Inserting the new save cart data if old and new cart data not match
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldSaveForLaterLineitemDetails )
	BEGIN 
	    --Getting the new save cart data and generating row no. for new save cart insert
		SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU  
		INTO #InsertNewSaveForLaterLineitem   
		FROM  #NewSaveForLaterLineitemDetails  
		GROUP BY ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails ,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU   
		ORDER BY RowId, Id   
       	
		--Removing the line item having Quantity <=0	     
		DELETE FROM #InsertNewSaveForLaterLineitem WHERE Quantity <=0  
  
		--Updating the rowid into new save cart line item as new line item is merged into existing save cart item
		;WITH VTTY AS   
		(  
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU   
			FROM #InsertNewSaveForLaterLineitem m  
			INNER JOIN  #InsertNewSaveForLaterLineitem TY1 ON TY1.SKU = m.ParentSKU   
			WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon   
		)   
		UPDATE m1   
		SET m1.RowId = TYU.RowId  
		FROM #InsertNewSaveForLaterLineitem m1   
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  
        
		--Deleting the new save cart line item if cart line item is merged
		;WITH VTRET AS   
		(  
			SELECT RowId,id,Min(NewRowId) NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID  
			FROM #InsertNewSaveForLaterLineitem   
			GROUP BY RowId,id ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID
			Having  SKU = ParentSKU  AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdSimple
		)   
		DELETE FROM #InsertNewSaveForLaterLineitem WHERE NewRowId IN (SELECT NewRowId FROM VTRET)  


		--Inserting the new cart line item if not merged in existing save cart line item
		INSERT INTO  ZnodeOmsTemplateLineItem (ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description)  
		OUTPUT INSERTED.OmsTemplateLineItemId  INTO @OmsInsertedData 
		SELECT NULL ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description   
		FROM  #InsertNewSaveForLaterLineitem  TH  
		
		SELECT  MAX(a.OmsTemplateLineItemId ) OmsTemplateLineItemId 
		, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU  
		INTO #ParentOmsTemplateId
		FROM ZnodeOmsTemplateLineItem a with (nolock) 
		INNER JOIN #InsertNewSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0  
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		AND CASE WHEN EXISTS (SELECT TOP 1 1 FROM #InsertNewSaveForLaterLineitem TU WHERE TU.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple)  THEN ISNULL(a.OrderLineItemRelationshipTypeID,0) ELSE 0 END = 0 
		GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID

		UPDATE a SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM  #ParentOmsTemplateId  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
		AND a.ParentOmsTemplateLineItemId IS nULL   

		SELECT a.OmsTemplateLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(Order BY c.OmsTemplateLineItemId )RowIdNo
		INTO #NewSimpleProduct
		FROM ZnodeOmsTemplateLineItem a with (nolock) 
		INNER JOIN #InsertNewSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ( b.Id = 1  ))  
		INNER JOIN ZnodeOmsTemplateLineItem c on b.sku = c.sku and b.OmsTemplateId=c.OmsTemplateId and b.Id = 1 
		WHERE ( ISNULL(a.ParentOmsTemplateLineItemId,0) <> 0   )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  and c.ParentOmsTemplateLineItemId is null

		--Updating ParentOmsTemplateLineItemId for newly added save cart line item
		;WITH table_update AS 
		(
			SELECT * , ROW_NUMBER()Over(Order BY OmsTemplateLineItemId  ) RowIdNo
			FROM ZnodeOmsTemplateLineItem a with (nolock)
			WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
			AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
			AND a.ParentOmsTemplateLineItemId IS NULL  
			AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewSaveForLaterLineitem ty WHERE ty.OmsTemplateId = a.OmsTemplateId )
			AND EXISTS (SELECT TOP 1 1 FROM #NewSimpleProduct TI WHERE TI.SKU = a.SKU)
		)
		UPDATE a SET  
		a.ParentOmsTemplateLineItemId =(SELECT TOP 1 max(OmsTemplateLineItemId) 
		FROM #NewSimpleProduct  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU  GROUP BY r.ParentSKU, r.SKU  )   
		FROM table_update a  
		INNER JOIN #InsertNewSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
		WHERE (SELECT TOP 1 max(OmsTemplateLineItemId) 
		FROM #NewSimpleProduct  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU   GROUP BY r.ParentSKU, r.SKU  )    IS NOT NULL 
	 
		;WITH Cte_Th AS   
		(             
			SELECT RowId    
			FROM #InsertNewSaveForLaterLineitem a   
			GROUP BY RowId   
			HAVING COUNT(NewRowId) <= 1   
		)   
		UPDATE a SET a.Quantity =  NULL , a.ModifiedDate = @GetDate  
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =0)   
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
		AND a.OrderLineItemRelationshipTypeId IS NULL   
  
		UPDATE  ZnodeOmsTemplateLineItem   
		SET GROUPID = NULL   
		WHERE  EXISTS (SELECT TOP 1 1  FROM #InsertNewSaveForLaterLineitem RT WHERE RT.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId )  
		AND OrderLineItemRelationshipTypeId IS NOT NULL     

		;WITH Cte_UpdateSequence AS   
		(  
			SELECT OmsTemplateLineItemId ,Row_Number()Over(Order By OmsTemplateLineItemId) RowId , Sequence   
			FROM ZnodeOmsTemplateLineItem with (nolock)  
			WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewSaveForLaterLineitem TH WHERE TH.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId )  
		)   
		UPDATE Cte_UpdateSequence  
		SET  Sequence = RowId  
	
		----To update saved cart item personalise value FROM given line item	
		IF EXISTS(SELECT * FROM @TBL_Personalise1 where isnull(PersonalizeValue,'') <> '' and isnull(OmsTemplateLineItemId,0) <> 0)
		BEGIN
			DELETE FROM ZnodeOmsTemplatePersonalizeCartItem 
			WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise1 yu WHERE yu.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId )

			MERGE INTO ZnodeOmsTemplatePersonalizeCartItem TARGET 
			USING @TBL_Personalise1 SOURCE
			ON (TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId ) 
			WHEN NOT MATCHED THEN 
			INSERT  ( OmsTemplateLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
							,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL, PersonalizeName )
			VALUES (  SOURCE.OmsTemplateLineItemId,  @userId, @getdate, @userId, @getdate
							,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL, SOURCE.PersonalizeName ) ;
		END		
	
		UPDATE @TBL_Personalise
		SET OmsTemplateLineItemId = b.OmsTemplateLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN ZnodeOmsTemplateLineItem b ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsTemplateLineItemId IS NOT NULL 
	
		DELETE FROM ZnodeOmsTemplatePersonalizeCartItem 
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId )
						
		MERGE INTO ZnodeOmsTemplatePersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
		ON (TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId ) 
		WHEN NOT MATCHED THEN 
		INSERT  ( OmsTemplateLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
					,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL, PersonalizeName )
		VALUES (  SOURCE.OmsTemplateLineItemId,  @userId, @getdate, @userId, @getdate
					,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL, SOURCE.PersonalizeName ) ;
  
		
		 
	END 
	SET @Status = 1
COMMIT TRAN InsertUpdateSaveForLaterLineItem;
END TRY
BEGIN CATCH
	SET @Status = 0
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_InsertUpdateSaveForLaterLineItemQuantityWrapper @TemplateLineItemXML = '+CAST(@TemplateLineItemXML
	AS varchar(max))+',@UserId = '+CAST(@UserId AS varchar(50));

	SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
	ROLLBACK TRAN InsertUpdateSaveForLaterLineItem;
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_InsertUpdateSaveForLaterLineItemQuantityWrapper', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END;
