CREATE PROCEDURE [dbo].[Znode_InsertUpdateMoveToCartWrapper]
(
	@OmsTemplateId INT, 
	@UserId INT,
	@PortalId INT,
	@Status INT  = 0 OUT
)
AS 
   /* 
    Summary: THis Procedure is USed to save AND edit the saved cart line item      
    Unit Testing 
	BEGIN tran  
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
	--Declared the variables
	DECLARE @GetDate datetime= dbo.Fn_GetDate();
	DECLARE @AddOnQuantity numeric(28, 6)= 0;
	DECLARE @IsAddProduct   BIT = 0 
	DECLARE @OmsSavedCartLineItemId INT = 0
	DECLARE @CartLineItemXML XML
    DECLARE @OmsSavedCartId int
	DECLARE @OmsCookieMappingId int

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

	IF isNULL(@UserId,0) <> 0  
	BEGIN
		SET @OmsCookieMappingId = (SELECT top 1 OmsCookieMappingId FROM ZnodeOmsCookieMapping WITH (NOLOCK) WHERE isNULL(UserId,0) = @UserID AND isNULL(PortalId,0) = @PortalId)
	END

	IF NOT EXISTS(SELECT top 1 OmsCookieMappingId FROM ZnodeOmsCookieMapping WITH (NOLOCK) WHERE OmsCookieMappingId = @OmsCookieMappingId)
	BEGIN
		INSERT INTO ZnodeOmsCookieMapping (UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT case when @UserId = 0 then NULL else @UserId END ,@PortalId,@UserId,@GetDate,@UserId,@GetDate

		SET @OmsCookieMappingId = @@IDENTITY
	END
	----To get the oms savecartid on basis of @OmsCookieMappingId 
	SET @OmsSavedCartId = (SELECT top 1 OmsSavedCartId FROM ZnodeOmsSavedCart WITH (NOLOCK) WHERE OmsCookieMappingId = @OmsCookieMappingId)
	
	----If omssavecartid not present in ZnodeOmsSavedCart table then inserting new record to generated omssavecartid 
	IF isNULL(@OmsSavedCartId,0) = 0
	BEGIN
		INSERT INTO ZnodeOmsSavedCart(OmsCookieMappingId,SalesTax,RecurringSalesTax,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @OmsCookieMappingId,NULL,NULL,@UserId,@GetDate,@UserId,@GetDate

		SET @OmsSavedCartId = @@IDENTITY
	END
	

DECLARE @TBL_SavecartLineitems TABLE
	( 
		RowId int IDENTITY(1,1), OmsSavedCartLineItemId int, ParentOmsSavedCartLineItemId int, OmsSavedCartId int, SKU nvarchar(600), Quantity numeric(28, 6), OrderLineItemRelationshipTypeID int, CustomText nvarchar(max), 
		CartAddOnDetails nvarchar(max), Sequence int, AddOnValueIds varchar(max), BundleProductIds varchar(max), ConfigurableProductIds varchar(max), GroupProductIds varchar(max), PersonalisedAttribute XML, 
		AutoAddon varchar(max), OmsOrderId int, ItemDetails nvarchar(max),
		Custom1	nvarchar(max),Custom2 nvarchar(max),Custom3 nvarchar(max),Custom4
		nvarchar(max),Custom5 nvarchar(max),GroupId NVARCHAR(max) ,ProductName Nvarchar(1000) , Description NVARCHAR(max),AddOnQuantity NVARCHAR(max), CustomUnitPrice numeric(28, 6)
	);

	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeID,CustomText,CartAddOnDetails,Sequence,AutoAddon,Custom1,Custom2,Custom3,Custom4
			  ,Custom5,GroupId,ProductName,Description,CustomUnitPrice)

	SELECT OmsTemplateLineItemId,@OmsSavedCartId as OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeID,CustomText,CartAddOnDetails,
			Sequence,AutoAddon,Custom1,Custom2,Custom3,Custom4
			,Custom5,GroupId,ProductName,Description,CustomUnitPrice 
	FROM ZnodeOmsTemplateLineItem WHERE OmsTemplateId = @Omstemplateid AND ParentOmsTemplateLineItemId IS NULL

	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeID,CustomText,CartAddOnDetails,
			Sequence , AutoAddon,
		
			 Custom1	,Custom2 ,Custom3,Custom4
			,Custom5 ,GroupId ,ProductName  , Description ,CustomUnitPrice )
	SELECT a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId,@OmsSavedCartId as OmsSavedCartId,a.SKU , a.Quantity , a.OrderLineItemRelationshipTypeID , a.CustomText , a.CartAddOnDetails,
			a.Sequence , a.AutoAddon,
		
			a.Custom1	,a.Custom2 ,a.Custom3,a.Custom4
			,a.Custom5 ,a.GroupId ,a.ProductName  , a.Description ,a.CustomUnitPrice 
	FROM ZnodeOmsTemplateLineItem a
	INNER JOIN @TBL_SavecartLineitems b on a.ParentOmsTemplateLineItemId = b.OmsSavedCartLineItemId
	WHERE OmsTemplateId = @Omstemplateid AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple

	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU , Quantity , OrderLineItemRelationshipTypeID , CustomText , CartAddOnDetails,
			Sequence , AutoAddon,
		
			Custom1	,Custom2 ,Custom3,Custom4
			,Custom5 ,GroupId ,ProductName  , Description ,CustomUnitPrice,BundleProductIds )
	SELECT a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId,@OmsSavedCartId as OmsSavedCartId,b.SKU , a.Quantity , a.OrderLineItemRelationshipTypeID , a.CustomText , a.CartAddOnDetails,
			a.Sequence , a.AutoAddon,
		
			a.Custom1	,a.Custom2 ,a.Custom3,a.Custom4
			,a.Custom5 ,a.GroupId ,a.ProductName  , a.Description ,a.CustomUnitPrice, c.Item
	FROM ZnodeOmsTemplateLineItem a
	INNER JOIN @TBL_SavecartLineitems b on a.ParentOmsTemplateLineItemId = b.OmsSavedCartLineItemId
	cross apply dbo.split(a.sku,',')c 
	WHERE OmsTemplateId = @Omstemplateid AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle

	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU , Quantity , OrderLineItemRelationshipTypeID , CustomText , CartAddOnDetails,
			Sequence , AutoAddon,
		
			Custom1	,Custom2 ,Custom3,Custom4
			,Custom5 ,GroupId ,ProductName  , Description ,CustomUnitPrice,ConfigurableProductIds )
	SELECT a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId,@OmsSavedCartId as OmsSavedCartId,b.SKU , a.Quantity , a.OrderLineItemRelationshipTypeID , a.CustomText , a.CartAddOnDetails,
			a.Sequence , a.AutoAddon,
		
			a.Custom1	,a.Custom2 ,a.Custom3,a.Custom4
			,a.Custom5 ,a.GroupId ,a.ProductName  , a.Description ,a.CustomUnitPrice, a.SKU
	FROM ZnodeOmsTemplateLineItem a
	INNER JOIN @TBL_SavecartLineitems b on a.ParentOmsTemplateLineItemId = b.OmsSavedCartLineItemId
	WHERE OmsTemplateId = @Omstemplateid AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable

	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU , Quantity , OrderLineItemRelationshipTypeID , CustomText , CartAddOnDetails,
			Sequence , AutoAddon,
		
			Custom1	,Custom2 ,Custom3,Custom4
			,Custom5 ,GroupId ,ProductName  , Description ,CustomUnitPrice,GroupProductIds )
	SELECT a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId,@OmsSavedCartId as OmsSavedCartId,b.SKU , a.Quantity , a.OrderLineItemRelationshipTypeID , a.CustomText , a.CartAddOnDetails,
			a.Sequence , a.AutoAddon,
		
			a.Custom1	,a.Custom2 ,a.Custom3,a.Custom4
			,a.Custom5 ,a.GroupId ,a.ProductName  , a.Description ,a.CustomUnitPrice, a.SKU
	FROM ZnodeOmsTemplateLineItem a
	INNER JOIN @TBL_SavecartLineitems b on a.ParentOmsTemplateLineItemId = b.OmsSavedCartLineItemId
	WHERE OmsTemplateId = @Omstemplateid AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup

	update a set a.OrderLineItemRelationshipTypeID = b.OrderLineItemRelationshipTypeID from @TBL_SavecartLineitems a
	inner join @TBL_SavecartLineitems b on a.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId
	where a.OrderLineItemRelationshipTypeID is null and a.ParentOmsSavedCartLineItemId is null and b.OrderLineItemRelationshipTypeID is not null


	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU , Quantity , OrderLineItemRelationshipTypeID , CustomText , CartAddOnDetails,
			Sequence , AutoAddon,
		
			Custom1	,Custom2 ,Custom3,Custom4
			,Custom5 ,GroupId ,ProductName  , Description ,CustomUnitPrice,AddOnValueIds ,AddOnQuantity,ConfigurableProductIds,BundleProductIds,GroupProductIds)
	SELECT a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId, @OmsSavedCartId as OmsSavedCartId,B.SKU , a.Quantity , a.OrderLineItemRelationshipTypeID , a.CustomText , a.CartAddOnDetails,
			a.Sequence , a.AutoAddon,
		
			a.Custom1	,a.Custom2 ,a.Custom3,a.Custom4
			,a.Custom5 ,a.GroupId ,a.ProductName  , a.Description ,a.CustomUnitPrice, a.SKU,a.Quantity,ConfigurableProductIds,BundleProductIds,GroupProductIds
	FROM ZnodeOmsTemplateLineItem a
	INNER JOIN @TBL_SavecartLineitems b on a.ParentOmsTemplateLineItemId = b.OmsSavedCartLineItemId
	WHERE OmsTemplateId = @Omstemplateid AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND B.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdBundle

	INSERT INTO @TBL_SavecartLineitems(OmsSavedCartLineItemId,ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU , Quantity , OrderLineItemRelationshipTypeID , CustomText , CartAddOnDetails,
			Sequence , AutoAddon,
		
			Custom1	,Custom2 ,Custom3,Custom4
			,Custom5 ,GroupId ,ProductName  , Description ,CustomUnitPrice,AddOnValueIds ,AddOnQuantity,ConfigurableProductIds,BundleProductIds,GroupProductIds)
	SELECT a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId, @OmsSavedCartId as OmsSavedCartId,B.SKU , a.Quantity , a.OrderLineItemRelationshipTypeID , a.CustomText , a.CartAddOnDetails,
			a.Sequence , a.AutoAddon,
		
			a.Custom1	,a.Custom2 ,a.Custom3,a.Custom4
			,a.Custom5 ,a.GroupId ,a.ProductName  , a.Description ,a.CustomUnitPrice, a.SKU,a.Quantity,ConfigurableProductIds,BundleProductIds,GroupProductIds
	FROM ZnodeOmsTemplateLineItem a
	INNER JOIN @TBL_SavecartLineitems b on a.ParentOmsTemplateLineItemId = b.ParentOmsSavedCartLineItemId
	WHERE OmsTemplateId = @Omstemplateid AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND B.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle

	--update personalize text
	SELECT  OmsTemplateLineItemId,'<PersonaliseValueModel> <PersonalizeCode>'+isnull(ltrim(rtrim(PersonalizeCode)),'')+'</PersonalizeCode><PersonalizeValue>'+isnull(ltrim(rtrim(PersonalizeValue)),'')
	+'</PersonalizeValue><DesignId>'+isnull(DesignId,'')+'</DesignId><ThumbnailURL>'+isnull(ThumbnailURL,'')+'</ThumbnailURL><PersonalizeName>'
	+isnull(PersonalizeName,'')+'</PersonalizeName><OmsSavedCartLineItemId>0</OmsSavedCartLineItemId></PersonaliseValueModel>' 
	AS PersonalisedAttribute
	INTO #PersonalisedData
	FROM ZnodeOmsTemplatePersonalizeCartItem WHERE OmsTemplateLineItemId IN (
	SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplateLineItem  WHERE OmsTemplateId =@Omstemplateid )

	
	SELECT  OmsTemplateLineItemId, STUFF((SELECT  ',' + PersonalisedAttribute 
	FROM #PersonalisedData B WHERE A.OmsTemplateLineItemId=B.OmsTemplateLineItemId
	FOR XML PATH ('')
	), 1, 1, '')  AS PersonalisedAttribute
	into #MergePersonalizeData
	FROM #PersonalisedData A
	GROUP BY OmsTemplateLineItemId
	

	update #MergePersonalizeData set PersonalisedAttribute = replace(replace(PersonalisedAttribute,'&lt;','<'),'&gt;','>') where PersonalisedAttribute is not null

	Update B set PersonalisedAttribute=A.PersonalisedAttribute
	from #MergePersonalizeData A 
	inner join @TBL_SavecartLineitems B on (A.OmsTemplateLineItemId=B.OmsSavedCartLineItemId )

	--Updating personalize xml string in parent product for simple product
	UPDATE A SET PersonalisedAttribute = B.PersonalisedAttribute
	FROM @TBL_SavecartLineitems A
	INNER JOIN @TBL_SavecartLineitems B ON A.OmsSavedCartLineItemId = B.ParentOmsSavedCartLineItemId 
	AND B.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple
	
	--Updating personalize xml string in parent product for group and configurable product
	UPDATE A SET PersonalisedAttribute = B.PersonalisedAttribute
	FROM @TBL_SavecartLineitems A
	INNER JOIN @TBL_SavecartLineitems B ON A.ParentOmsSavedCartLineItemId = B.OmsSavedCartLineItemId 
	AND B.OrderLineItemRelationshipTypeID in ( @OrderLineItemRelationshipTypeIdGroup, @OrderLineItemRelationshipTypeIdConfigurable) and a.PersonalisedAttribute is null

	--Inserting the personalise data into variable table @TBL_Personalise for inserting the personalise data for new products
	DECLARE @TBL_Personalise TABLE (OmsSavedCartLineItemId INT, ParentOmsSavedCartLineItemId int,SKU Varchar(600) ,PersonalizeCode NVARCHAr(max),PersonalizeValue NVARCHAr(max),DesignId NVARCHAr(max), ThumbnailURL NVARCHAr(max))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsSavedCartLineItemId,a.SKU
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(Max)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(Max)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(Max)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(Max)' ) AS ThumbnailURL
	FROM @TBL_SavecartLineitems a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col) 
	where a.ParentOmsSavedCartLineItemId is not null

	DELETE FROM @TBL_SavecartLineitems 
	WHERE OmsSavedCartLineItemId in (SELECT ParentOmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE OrderLineItemRelationshipTypeID 
	IN (@OrderLineItemRelationshipTypeIdBundle,@OrderLineItemRelationshipTypeIdConfigurable,@OrderLineItemRelationshipTypeIdGroup,@OrderLineItemRelationshipTypeIdSimple) AND ParentOmsSavedCartLineItemId IS NOT NULL)

	--Removed the save cart product to add in the cart which are not associated with the catalog
	DELETE A FROM @TBL_SavecartLineitems A
	WHERE NOT EXISTS(SELECT TOP 1 1 FROM ZnodePublishProductEntity B WHERE A.SKU = B.SKU AND ISNULL(B.ZnodeCategoryIds,0) <> 0)

	UPDATE @TBL_SavecartLineitems SET OmsSavedCartLineItemId = 0 ,ParentOmsSavedCartLineItemId = 0 where OrderLineItemRelationshipTypeID is null
	UPDATE @TBL_SavecartLineitems SET OmsSavedCartLineItemId = 0 ,ParentOmsSavedCartLineItemId = 0 
	where OrderLineItemRelationshipTypeID NOT IN (@OrderLineItemRelationshipTypeIdBundle,@OrderLineItemRelationshipTypeIdConfigurable,@OrderLineItemRelationshipTypeIdGroup)
	UPDATE @TBL_SavecartLineitems SET OmsSavedCartLineItemId = 0 ,ParentOmsSavedCartLineItemId = 0,OrderLineItemRelationshipTypeID=null,Sequence = null 
	where OrderLineItemRelationshipTypeID in (@OrderLineItemRelationshipTypeIdBundle,@OrderLineItemRelationshipTypeIdConfigurable,@OrderLineItemRelationshipTypeIdGroup)

	delete a from @TBL_SavecartLineitems a where A.AddOnValueIds IS NULL
	and exists(select * from @TBL_SavecartLineitems b where b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon and  isnull(a.ConfigurableProductIds,'') = isnull(b.ConfigurableProductIds,'')
	and isnull(a.BundleProductIds,'') = isnull(b.BundleProductIds,'') and isnull(a.GroupProductIds,'') = isnull(b.GroupProductIds,'') and isnull(a.SKU,'') = isnull(b.SKU,'')
	)

	update @TBL_SavecartLineitems set OrderLineItemRelationshipTypeID = null,Sequence = NULL

	--Save cart execution code for bundle product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' )
	BEGIN 				
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0  ) 
		BEGIN 
			SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0 )

			UPDATE ZnodeOmsSavedCartLineItem 
			SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0)
			, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
			WHERE ( OmsSavedCartLineItemId = @OmsSavedCartLineItemId  
			OR ParentOmsSavedCartLineItemId =  @OmsSavedCartLineItemId   ) 

			UPDATE ZnodeOmsSavedCartLineItem 
			SET Quantity = AddOnQuantity, ModifiedDate = @GetDate,CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
			FROM ZnodeOmsSavedCartLineItem ZOSCLI with (nolock)
			INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
			WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
			AND SCLI.BundleProductIds <> ''

			--After update the existing cart with new save cart then deleting those records which are updated
			DELETE	FROM @TBL_SavecartLineitems WHERE BundleProductIds <> '' AND OmsSavedCartLineItemId <> 0
		END 

		--Getting bundle save cart line item entries into table to pass for bundle prodcut sp
		DECLARE @TBL_bundleProduct TT_SavecartLineitems 
		INSERT INTO @TBL_bundleProduct 
		SELECT *  
		FROM @TBL_SavecartLineitems 
		WHERE ISNULL(BundleProductIds,'') <> '' 
				
		EXEC Znode_InsertUpdateSaveCartLineItemBundle @TBL_bundleProduct,@userId,@OrderLineItemRelationshipTypeIdBundle,@OrderLineItemRelationshipTypeIdAddon
				 
		DELETE FROM  @TBL_SavecartLineitems WHERE ISNULL(BundleProductIds,'') <> '' 
		SET @OmsSavedCartLineItemId = 0 
	END 
	--Save cart execution code for configurable product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' )
	BEGIN 				
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0  ) 
		BEGIN 

			SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0 )
				 
			UPDATE ZnodeOmsSavedCartLineItem 
			SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId = @OmsSavedCartLineItemId )
			, ModifiedDate = @GetDate,CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
			WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId

			UPDATE ZnodeOmsSavedCartLineItem 
			SET Quantity = AddOnQuantity, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
			FROM ZnodeOmsSavedCartLineItem ZOSCLI with (nolock)
			INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
			WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
			AND SCLI.ConfigurableProductIds <> ''

			--After update the existing cart with new save cart then deleting those records which are updated
			DELETE	FROM @TBL_SavecartLineitems WHERE ConfigurableProductIds <> '' AND OmsSavedCartLineItemId <> 0
		END 
		--Getting bundle save cart line item entries into table to pass for configurable prodcut sp
		DECLARE @TBL_Configurable TT_SavecartLineitems 
		INSERT INTO @TBL_Configurable 
		SELECT *  
		FROM @TBL_SavecartLineitems 
		WHERE ISNULL(ConfigurableProductIds,'') <> '' 

		EXEC Znode_InsertUpdateSaveCartLineItemConfigurable @TBL_Configurable,@userId,@OrderLineItemRelationshipTypeIdConfigurable,@OrderLineItemRelationshipTypeIdAddon
	  
		DELETE FROM @TBL_SavecartLineitems 
		WHERE ISNULL(ConfigurableProductIds,'') <> ''
		SET @OmsSavedCartLineItemId = 0  
	END 
	--Save cart execution code for group product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' )
	BEGIN 				
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId <> 0  ) 
		BEGIN 
			--Updating the existing save cart for group product
			SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId <> 0 )
			UPDATE ZnodeOmsSavedCartLineItem 
			SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId = @OmsSavedCartLineItemId ), CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
			WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId

			UPDATE ZnodeOmsSavedCartLineItem 
			SET Quantity = AddOnQuantity, ModifiedDate = @GetDate, CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
			FROM ZnodeOmsSavedCartLineItem ZOSCLI with (nolock)
			INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = SCLI.OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
			WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
			AND SCLI.GroupProductIds <> ''

			--After update the existing cart with new save cart then deleting those records which are updated
			DELETE	FROM @TBL_SavecartLineitems WHERE GroupProductIds <> '' AND OmsSavedCartLineItemId <> 0
		END 
		--Getting bundle save cart line item entries into table to pass for group prodcut sp
		DECLARE @TBL_Group TT_SavecartLineitems 
		INSERT INTO @TBL_Group 
		SELECT *  
		FROM @TBL_SavecartLineitems 
		WHERE ISNULL(GroupProductIds,'') <> '' 

		EXEC Znode_InsertUpdateSaveCartLineItemGroup @TBL_Group,@userId,@OrderLineItemRelationshipTypeIdGroup,@OrderLineItemRelationshipTypeIdAddon
		
		--After update the existing cart with new save cart then deleting those records which are updated
		DELETE FROM @TBL_SavecartLineitems WHERE ISNULL(GroupProductIds,'') <> ''
		SET @OmsSavedCartLineItemId = 0  
	END 
	
	--This part is for updating the cart for existing line items for simple product
	IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId <> 0  ) 
	BEGIN 
				 
		SET @OmsSavedCartLineItemId  = (SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId <> 0 )
		UPDATE ZnodeOmsSavedCartLineItem 
		SET Quantity = (SELECT TOP 1 Quantity FROM @TBL_SavecartLineitems WHERE  OmsSavedCartLineItemId = @OmsSavedCartLineItemId )
		, ModifiedDate = @GetDate , CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
		WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId

		DECLARE @ParentOmsSavedCartLineItemId INT = 0
		SET @ParentOmsSavedCartLineItemId = (select ParentOmsSavedCartLineItemId from ZnodeOmsSavedCartLineItem WHERE OmsSavedCartLineItemId = @OmsSavedCartLineItemId)

		UPDATE ZnodeOmsSavedCartLineItem 
		SET  CustomUnitPrice = (SELECT TOP 1 CustomUnitPrice FROM @TBL_SavecartLineitems)
		WHERE OmsSavedCartLineItemId = @ParentOmsSavedCartLineItemId

		UPDATE ZnodeOmsSavedCartLineItem 
		SET Quantity = AddOnQuantity, ModifiedDate = @GetDate
		FROM ZnodeOmsSavedCartLineItem ZOSCLI with (nolock)
		INNER JOIN @TBL_SavecartLineitems SCLI ON ZOSCLI.ParentOmsSavedCartLineItemId = @OmsSavedCartLineItemId AND ZOSCLI.OmsSavedCartId = SCLI.OmsSavedCartId AND ZOSCLI.SKU = SCLI.AddOnValueIds
		WHERE ZOSCLI.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
					
		DELETE	FROM @TBL_SavecartLineitems WHERE OmsSavedCartLineItemId <> 0
	END 
	--Save cart execution code for Simple product
	DECLARE @OmsInsertedData TABLE (OmsSavedCartLineItemId INT )
				
	CREATE TABLE #NewSavecartLineitemDetails 
	(
		GenId INT IDENTITY(1,1),RowId	INT	,OmsSavedCartLineItemId	INT	 ,ParentOmsSavedCartLineItemId	INT,OmsSavedCartId	INT
		,SKU	NVARCHAR(MAX) ,Quantity	NUMERIC(28,6)	,OrderLineItemRelationshipTypeID	INT	,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	int	,AutoAddon	varchar(MAX)	,OmsOrderId	INT	,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX)  ,Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX)
	)
	
	--Getting new save cart data
	INSERT INTO #NewSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU 
	FROM @TBL_SavecartLineitems a 
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName
		
	--Getting new simple product save cart data
	INSERT INTO #NewSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, @OrderLineItemRelationshipTypeIdSimple, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU 
	FROM @TBL_SavecartLineitems  a 
	WHERE ISNULL(BundleProductIds,'') =  '' 
	AND  ISNULL(GroupProductIds,'') = ''	AND ISNULL(	ConfigurableProductIds,'') = ''
		GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity,  CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName
	
	--Getting new Group,Bundle and Configurable products save cart data if addon is present for any line item
	INSERT INTO #NewSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
		,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
		,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
		WHEN  GroupProductIds <> '' THEN GroupProductIds 
		WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END     ParentSKU 
	FROM @TBL_SavecartLineitems  a 
	WHERE AddOnValueIds <> ''
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
	,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
	,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU

	CREATE TABLE #OldSavecartLineitemDetails (OmsSavedCartId INT ,OmsSavedCartLineItemId INT,ParentOmsSavedCartLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
	--Getting the old save cart data if present for same SKU in the new save cart data for simple product	 
	INSERT INTO #OldSavecartLineitemDetails  
	SELECT  a.OmsSavedCartId,a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	FROM ZnodeOmsSavedCartLineItem a with (nolock)  
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = a.OmsSavedCartId AND ISNULL(a.SKU,'') = ISNULL(TY.SKU,'')   )   
	AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple   

	--Getting the old save cart Parent data 
	INSERT INTO #OldSavecartLineitemDetails 
	SELECT DISTINCT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsSavedCartLineItem b with (nolock)
	INNER JOIN #OldSavecartLineitemDetails c ON (c.ParentOmsSavedCartLineItemId  = b.OmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
	AND  b.OrderLineItemRelationshipTypeID IS NULL 
		 
	DELETE a FROM #OldSavecartLineitemDetails a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldSavecartLineitemDetails b WHERE b.ParentOmsSavedCartLineItemId IS NULL AND b.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)
	AND a.ParentOmsSavedCartLineItemId IS NOT NULL 
		
	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldSavecartLineitemDetails
		
	--Getting the old save cart addon data for old line items if present
	INSERT INTO #OldSavecartLineitemDetails 
	SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsSavedCartLineItem b with (nolock)
	INNER JOIN #OldSavecartLineitemDetails c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
	AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

	------Merge Addon for same product
	IF EXISTS(SELECT * FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN

		INSERT INTO #OldValueForAddon 
		SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
		FROM ZnodeOmsSavedCartLineItem b with (nolock)
		INNER JOIN #OldValueForAddon c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		SELECT distinct SKU, STUFF(
								( SELECT  ', ' + SKU FROM    
									( SELECT DISTINCT SKU FROM     #OldValueForAddon b 
										where a.OmsSavedCartLineItemId=b.ParentOmsSavedCartLineItemId and OrderLineItemRelationshipTypeID = 1 ) x 
										FOR XML PATH('')
								), 1, 2, ''
								) AddOns
		INTO #AddOnsExists
		FROM #OldValueForAddon a where a.ParentOmsSavedCartLineItemId is not null and OrderLineItemRelationshipTypeID<>1

		SELECT distinct a.SKU, STUFF(
									( SELECT  ', ' + x.AddOnValueIds FROM    
									( SELECT DISTINCT b.AddOnValueIds FROM @TBL_SavecartLineitems b
										where a.SKU=b.SKU ) x
										FOR XML PATH('')
									), 1, 2, ''
								) AddOns
		INTO #AddOnAdded
		FROM @TBL_SavecartLineitems a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldSavecartLineitemDetails
		END

	END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSavecartLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_SavecartLineitems ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldSavecartLineitemDetails a WHERE	
	ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'')  <> '' )
	BEGIN 
		
		DELETE FROM #OldSavecartLineitemDetails 
	END 
	ELSE 
	BEGIN 
	    
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN 
		 
			DECLARE @parenTofAddon  TABLE( ParentOmsSavedCartLineItemId INT  )  
			INSERT INTO  @parenTofAddon 
			SELECT  ParentOmsSavedCartLineItemId FROM #OldSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  

			DELETE FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT ParentOmsSavedCartLineItemId FROM  @parenTofAddon)   
				AND ParentOmsSavedCartLineItemId IS NOT NULL  
				AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

			DELETE FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldSavecartLineitemDetails m)
			AND ParentOmsSavedCartLineItemId IS  NULL  
		 
		END 
		ELSE IF (SELECT COUNT (OmsSavedCartLineItemId) FROM #OldSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS NULL ) > 1 
		BEGIN 

			DECLARE @TBL_deleteParentOmsSavedCartLineItemId TABLE (OmsSavedCartLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsSavedCartLineItemId 
			SELECT ParentOmsSavedCartLineItemId
			FROM ZnodeOmsSavedCartLineItem a with (nolock)
			WHERE ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM #OldSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple  )
			AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
			OR ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
		    
			DELETE FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldSavecartLineitemDetails m)
			AND ParentOmsSavedCartLineItemId IS  NULL  

		END
		ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldSavecartLineitemDetails ty WHERE ty.OmsSavedCartId = wt.OmsSavedCartId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple AND wt.ParentOmsSavedCartLineItemId= ty.OmsSavedCartLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_SavecartLineitems WHERE ISNULL(AddOnValueIds,'')  = '' )
		BEGIN 

			DELETE FROM #OldSavecartLineitemDetails
		END 
	END 

	--Getting the personalise data for old save cart if present
	DECLARE @TBL_Personaloldvalues TABLE (OmsSavedCartLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsSavedCartLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsPersonalizeCartItem  a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldSavecartLineitemDetails TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)

	IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN 
		DELETE FROM #OldSavecartLineitemDetails
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		BEGIN 
		   
			DELETE FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId IN (
			SELECT OmsSavedCartLineItemId FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
			AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ) ) 
			OR OmsSavedCartLineItemId IN ( SELECT ParentOmsSavedCartLineItemId FROM #OldSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
			AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ))
		      
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		BEGIN 

			DELETE n FROM #OldSavecartLineitemDetails n WHERE OmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem WHERE n.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId  )
			OR ParentOmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem   )
	   
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldSavecartLineitemDetails YU WHERE YU.OmsSavedCartLineItemId = m.OmsSavedCartLineItemId )) 
		AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) = 1
		BEGIN 
			DELETE FROM #OldSavecartLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		END 
	  
	END 
	
	--If already exists cart 
	IF EXISTS (SELECT TOP 1 1 FROM #OldSavecartLineitemDetails )
	BEGIN

		----DELETE old value FROM table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU not having personalise value for new cart item
		;WITH cte AS
		(
			SELECT distinct b.*
			FROM @TBL_SavecartLineitems a 
			INNER JOIN #OldSavecartLineitemDetails b on ( a.SKU = b.sku)
			where isnull(cast(a.PersonalisedAttribute AS varchar(max)),'') = '' and a.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		)
		,cte2 AS
		(
			SELECT c.ParentOmsSavedCartLineItemId
			FROM #OldSavecartLineitemDetails a
			INNER JOIN ZnodeOmsSavedCartLineItem c on a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId
			INNER JOIN ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		)
		DELETE a FROM #OldSavecartLineitemDetails a
		INNER JOIN cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		INNER JOIN cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		----DELETE old value FROM table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU having personalise value for new cart item
		;WITH cte AS
		(
			SELECT distinct b.*, 
				a.PersonalizeCode
				,a.PersonalizeValue
			FROM @TBL_Personalise a 
			INNER JOIN #OldSavecartLineitemDetails b on ( a.SKU = b.sku)
			where a.PersonalizeValue <> ''
		)
		,cte2 AS
		(
			SELECT a.ParentOmsSavedCartLineItemId, b.PersonalizeCode, b.PersonalizeValue
			FROM #OldSavecartLineitemDetails a
			INNER JOIN ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId
			WHERE NOT EXISTS(SELECT * FROM cte c where b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and b.PersonalizeCode = c.PersonalizeCode 
						and b.PersonalizeValue = c.PersonalizeValue )
		)
		DELETE a FROM #OldSavecartLineitemDetails a
		INNER JOIN cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		INNER JOIN cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,b.ParentOmsSavedCartLineItemId , a.SKU AS SKU
				,a.PersonalizeCode
				,a.PersonalizeValue
				,a.DesignId
				,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldSavecartLineitemDetails b on a.SKU = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE b1
		FROM #OldSavecartLineitemDetails b1 
		WHERE NOT EXISTS(SELECT * FROM cte c where (b1.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or b1.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId))
		AND EXISTS(SELECT * FROM cte)

		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is same for same line item then Quantity will added
	-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsSavedCartLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldSavecartLineitemDetails b ON a.SKU = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c ON b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId 
			--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY b.OmsSavedCartLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsSavedCartLineItemId,0) as OmsSavedCartLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewSavecartLineitemDetails  b ON a.SKU = b.SKU
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0 AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple
			GROUP BY a.OmsSavedCartLineItemId ,b.SKU
		)
		DELETE c
		from CTE_OldPersonalizeCodeCount a
		inner join CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		inner join #OldSavecartLineitemDetails c on b.SKU = c.SKU and a.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
		
		--Delete parent entry if child not present
		DELETE a FROM #OldSavecartLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldSavecartLineitemDetails b where a.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId)
		AND a.ParentOmsSavedCartLineItemId IS NULL

	--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
	--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is same for same line item then Quantity will added
	
		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,a.ParentOmsSavedCartLineItemId , a.SKU
						,d.PersonalizeCode
				,d.PersonalizeValue
				,d.DesignId
				,d.ThumbnailURL
				FROM @TBL_SavecartLineitems a 
				INNER JOIN #OldSavecartLineitemDetails b on a.SKU = b.SKU
				INNER JOIN @TBL_Personalise d on d.SKU = a.SKU
				INNER JOIN ZnodeOmsPersonalizeCartItem  c  with (nolock)  on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
				WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE b1
		FROM cte a1		  
		INNER JOIN #OldSavecartLineitemDetails b1 on a1.sku = b1.SKU
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsPersonalizeCartItem c where a1.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and a1.PersonalizeValue = c.PersonalizeValue)

		--Updating the cart if old and new cart data matches 
		UPDATE a
		SET a.Quantity = a.Quantity+ty.Quantity,
		a.Custom1 = ty.Custom1,
		a.Custom2 = ty.Custom2,
		a.Custom3 = ty.Custom3,
		a.Custom4 = ty.Custom4,
		a.Custom5 = ty.Custom5, 
		a.ModifiedDate = @GetDate
		FROM ZnodeOmsSavedCartLineItem a
		INNER JOIN #OldSavecartLineitemDetails b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		INNER JOIN #NewSavecartLineitemDetails ty ON (ty.SKU = b.SKU)

	END 
	
	--Inserting the new save cart data if old and new cart data not match
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldSavecartLineitemDetails )
	BEGIN 
	    --Getting the new save cart data and generating row no. for new save cart insert
		SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU  
		INTO #InsertNewSavecartLineitem   
		FROM  #NewSavecartLineitemDetails  
		GROUP BY ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails ,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU   
		ORDER BY RowId, Id   
       	
		--Removing the line item having Quantity <=0	     
		DELETE FROM #InsertNewSavecartLineitem WHERE Quantity <=0  
  
		--Updating the rowid into new save cart line item as new line item is merged into existing save cart item
		;WITH VTTY AS   
		(  
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU   
			FROM #InsertNewSavecartLineitem m  
			INNER JOIN  #InsertNewSavecartLineitem TY1 ON TY1.SKU = m.ParentSKU   
			WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon   
		)   
		UPDATE m1   
		SET m1.RowId = TYU.RowId  
		FROM #InsertNewSavecartLineitem m1   
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  
        
		--Deleting the new save cart line item if cart line item is merged
		;WITH VTRET AS   
		(  
			SELECT RowId,id,Min(NewRowId) NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID  
			FROM #InsertNewSavecartLineitem   
			GROUP BY RowId,id ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID
			Having  SKU = ParentSKU  AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdSimple
		)   
		DELETE FROM #InsertNewSavecartLineitem WHERE NewRowId IN (SELECT NewRowId FROM VTRET)  

		--Inserting the new cart line item if not merged in existing save cart line item
		INSERT INTO  ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description)  
		OUTPUT INSERTED.OmsSavedCartLineItemId  INTO @OmsInsertedData 
		SELECT NULL ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
		,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description   
		FROM  #InsertNewSavecartLineitem  TH  

		SELECT  MAX(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
		, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU  
		INTO #ParentOmsSavedCartId
		FROM ZnodeOmsSavedCartLineItem a with (nolock) 
		INNER JOIN #InsertNewSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0  
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		AND CASE WHEN EXISTS (SELECT TOP 1 1 FROM #InsertNewSavecartLineitem TU WHERE TU.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdSimple)  THEN ISNULL(a.OrderLineItemRelationshipTypeID,0) ELSE 0 END = 0 
		GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID

		UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  #ParentOmsSavedCartId  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
		AND a.ParentOmsSavedCartLineItemId IS nULL   

		SELECT a.OmsSavedCartLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(Order BY c.OmsSavedCartLineItemId )RowIdNo
		INTO #NewSimpleProduct
		FROM ZnodeOmsSavedCartLineItem a with (nolock) 
		INNER JOIN #InsertNewSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ( b.Id = 1  ))  
		INNER JOIN ZnodeOmsSavedCartLineItem c on b.sku = c.sku and b.OmsSavedCartId=c.OmsSavedCartId and b.Id = 1 
		WHERE ( ISNULL(a.ParentOmsSavedCartLineItemId,0) <> 0   )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  and c.ParentOmsSavedCartLineItemId is null

		--Updating ParentOmsSavedCartLineItemId for newly added save cart line item
		;WITH table_update AS 
		(
			SELECT * , ROW_NUMBER()Over(Order BY OmsSavedCartLineItemId  ) RowIdNo
			FROM ZnodeOmsSavedCartLineItem a with (nolock)
			WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
			AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
			AND a.ParentOmsSavedCartLineItemId IS NULL  
			AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
			AND EXISTS (SELECT TOP 1 1 FROM #NewSimpleProduct TI WHERE TI.SKU = a.SKU)
		)
		UPDATE a SET  
		a.ParentOmsSavedCartLineItemId =(SELECT TOP 1 max(OmsSavedCartLineItemId) 
		FROM #NewSimpleProduct  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU  GROUP BY r.ParentSKU, r.SKU  )   
		FROM table_update a  
		INNER JOIN #InsertNewSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
		WHERE (SELECT TOP 1 max(OmsSavedCartLineItemId) 
		FROM #NewSimpleProduct  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU   GROUP BY r.ParentSKU, r.SKU  )    IS NOT NULL 
	 
		;WITH Cte_Th AS   
		(             
			SELECT RowId    
			FROM #InsertNewSavecartLineitem a   
			GROUP BY RowId   
			HAVING COUNT(NewRowId) <= 1   
		)   
		UPDATE a SET a.Quantity =  NULL , a.ModifiedDate = @GetDate  
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =0)   
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
		AND a.OrderLineItemRelationshipTypeId IS NULL   
  
		UPDATE  ZnodeOmsSavedCartLineItem   
		SET GROUPID = NULL   
		WHERE  EXISTS (SELECT TOP 1 1  FROM #InsertNewSavecartLineitem RT WHERE RT.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
		AND OrderLineItemRelationshipTypeId IS NOT NULL     

		;WITH Cte_UpdateSequence AS   
		(  
			SELECT OmsSavedCartLineItemId ,Row_Number()Over(Order By OmsSavedCartLineItemId) RowId , Sequence   
			FROM ZnodeOmsSavedCartLineItem with (nolock)  
			WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewSavecartLineitem TH WHERE TH.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
		)   
		UPDATE Cte_UpdateSequence  
		SET  Sequence = RowId  
	
		------To update saved cart item personalise value FROM given line item	
		--IF EXISTS(SELECT * FROM @TBL_Personalise1 where isnull(PersonalizeValue,'') <> '' and isnull(OmsSavedCartLineItemId,0) <> 0)
		--BEGIN
		--	DELETE FROM ZnodeOmsPersonalizeCartItem 
		--	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise1 yu WHERE yu.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )

		--	MERGE INTO ZnodeOmsPersonalizeCartItem TARGET 
		--	USING @TBL_Personalise1 SOURCE
		--	ON (TARGET.OmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId ) 
		--	WHEN NOT MATCHED THEN 
		--	INSERT  ( OmsSavedCartLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
		--					,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL )
		--	VALUES (  SOURCE.OmsSavedCartLineItemId,  @userId, @getdate, @userId, @getdate
		--					,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL ) ;
		--END		
	
		UPDATE @TBL_Personalise
		SET OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN ZnodeOmsSavedCartLineItem b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsSavedCartLineItemId IS NOT NULL 
	
		DELETE FROM ZnodeOmsPersonalizeCartItem 
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )
						
		MERGE INTO ZnodeOmsPersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
		ON (TARGET.OmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId ) 
		WHEN NOT MATCHED THEN 
		INSERT  ( OmsSavedCartLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
					,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL )
		VALUES (  SOURCE.OmsSavedCartLineItemId,  @userId, @getdate, @userId, @getdate
					,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL ) ;
  
		
		 
	END 

	--Declare @OutputTable Table (CartCount numeric(28,6))

	--INSERT INTO @OutputTable
	--EXEC [Znode_GetOmsSavedCartLineItemCount] @OmsCookieMappingId = @OmsCookieMappingId,@UserId=@UserId,@PortalId=@UserId
	

	--SELECT CAST(1 AS bit) AS Status,@OmsSavedCartId AS SavedCartId,@OmsCookieMappingId AS CookieMappingId,CartCount
	--FROM @OutputTable

	SELECT @Status = 1
	SELECT cast( 1 as int )as Id, Cast(@Status as bit) Status;
COMMIT TRAN InsertUpdateSaveCartLineItem;
END TRY
BEGIN CATCH
SELECT @Status = 0
	SELECT ERROR_MESSAGE()	
	DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_InsertUpdateSaveCartLineItem @CartLineItemXML = '+CAST(@CartLineItemXML
	AS varchar(max))+',@UserId = '+CAST(@UserId AS varchar(50))+',@PortalId='+CAST(@PortalId AS varchar(10))+',@OmsCookieMappingId='+CAST(@OmsCookieMappingId AS varchar(10));

	SELECT cast( 0 as int ) Id, Cast(@Status as bit) Status;
	ROLLBACK TRAN InsertUpdateSaveCartLineItem;
	EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_InsertUpdateSaveCartLineItemQuantityWrapper', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
END CATCH;
END;