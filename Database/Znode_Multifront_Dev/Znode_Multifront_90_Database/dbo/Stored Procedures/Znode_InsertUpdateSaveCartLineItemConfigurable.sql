CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItemConfigurable]
(
	 @SaveCartLineItemType TT_SavecartLineitems READONLY
	,@Userid INT = 0 
	,@OrderLineItemRelationshipTypeIdConfigurable INT
	,@OrderLineItemRelationshipTypeIdAddon INT	
)
AS 
BEGIN 
	BEGIN TRY
	SET NOCOUNT ON 
	--Declared the variables
	DECLARE @GetDate datetime= dbo.Fn_GetDate(); 

	DECLARE @OmsInsertedData TABLE (OmsSavedCartLineItemId INT, OmsSavedCartId INT,SKU NVARCHAr(max),GroupId NVARCHAr(max),ParentOmsSavedCartLineItemId INT,OrderLineItemRelationshipTypeId INT ) 
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise TABLE (OmsSavedCartLineItemId INT, ParentOmsSavedCartLineItemId INT,ConfigurableProductIds VARCHAR(600) ,PersonalizeCode NVARCHAR(MAX),PersonalizeValue NVARCHAR(MAX),DesignId NVARCHAR(MAX), ThumbnailURL NVARCHAR(MAX))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsSavedCartLineItemId,a.ConfigurableProductIds
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(MAX)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(MAX)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(MAX)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(MAX)' ) AS ThumbnailURL
	FROM @SaveCartLineItemType a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)

	 CREATE TABLE #NewConfigSavecartLineitemDetails 
	 (
		 GenId INT IDENTITY(1,1), RowId	INT, OmsSavedCartLineItemId	INT, ParentOmsSavedCartLineItemId INT,OmsSavedCartId INT
		,SKU NVARCHAR(MAX) ,Quantity	NUMERIC(28,6), OrderLineItemRelationshipTypeID	INT	,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	INT	,AutoAddon	VARCHAR(MAX)	,OmsOrderId	INT	,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX), Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX),
		CustomUnitPrice NUMERIC(28,6)
	)
	 
	--Getting new save cart data(configurable product)
	INSERT INTO #NewConfigSavecartLineitemDetails
	SELECT MIN(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5, GroupId ,ProductName,MIN(Description) Description, 0 Id,NULL ParentSKU ,
		CustomUnitPrice
	FROM @SaveCartLineItemType a 
	GROUP BY OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,CustomUnitPrice
	 
	--Getting new configurable product save cart data
	INSERT INTO #NewConfigSavecartLineitemDetails 
	SELECT MIN(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, ConfigurableProductIds
		,Quantity, @OrderLineItemRelationshipTypeIdConfigurable, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU,
		CustomUnitPrice
	FROM @SaveCartLineItemType a 
	WHERE ConfigurableProductIds <> ''
	GROUP BY OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, ConfigurableProductIds
		,Quantity, CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,
		CustomUnitPrice
	
	--Getting new configurable products save cart data if addon is present for any line item
	INSERT INTO #NewConfigSavecartLineitemDetails
	SELECT MIN(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
		,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
		,CASE WHEN ConfigurableProductIds <> '' THEN ConfigurableProductIds
			WHEN GroupProductIds <> '' THEN GroupProductIds 
			WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END ParentSKU ,CustomUnitPrice
	FROM @SaveCartLineItemType a 
	WHERE AddOnValueIds <> ''
	GROUP BY OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
		,AddOnQuantity, CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		 
	SELECT * 
	INTO #ZnodeOmsSavedCartLineItem_NT
	FROM ZnodeOmsSavedCartLineItem a 
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType TY WHERE TY.OmsSavedCartId = a.OmsSavedCartId)

	CREATE TABLE #OldConfigSavecartLineitemDetails (OmsSavedCartId INT ,OmsSavedCartLineItemId INT,ParentOmsSavedCartLineItemId INT , SKU NVARCHAr(2000),OrderLineItemRelationshipTypeID INT )
	--Getting the old configurable save cart data if present for same SKU in the new save cart data for configurable product	 
	INSERT INTO #OldConfigSavecartLineitemDetails
	SELECT a.OmsSavedCartId,a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId , a.SKU ,a.OrderLineItemRelationshipTypeID 
	FROM #ZnodeOmsSavedCartLineItem_NT a
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType TY WHERE TY.OmsSavedCartId = a.OmsSavedCartId AND ISNULL(a.SKU,'') = ISNULL(TY.ConfigurableProductIds,''))
		AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable

	--Getting the old save cart Parent data
	INSERT INTO #OldConfigSavecartLineitemDetails 
	SELECT DISTINCT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU, b.OrderLineItemRelationshipTypeID
	FROM #ZnodeOmsSavedCartLineItem_NT b 
	INNER JOIN #OldConfigSavecartLineitemDetails c ON (c.ParentOmsSavedCartLineItemId = b.OmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-') )
		AND b.OrderLineItemRelationshipTypeID IS NULL 
		
	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldConfigSavecartLineitemDetails

	DELETE a 
	FROM #OldConfigSavecartLineitemDetails a 
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM #OldConfigSavecartLineitemDetails b WHERE b.ParentOmsSavedCartLineItemId IS NULL AND b.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)
		AND a.ParentOmsSavedCartLineItemId IS NOT NULL 

	INSERT INTO #OldConfigSavecartLineitemDetails 
	SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU, b.OrderLineItemRelationshipTypeID
	FROM #ZnodeOmsSavedCartLineItem_NT b 
	INNER JOIN #OldConfigSavecartLineitemDetails c ON (c.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon
		
	------Merge Addon for same product
	IF EXISTS(SELECT * FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN		
		INSERT INTO #OldValueForAddon 
		SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU, b.OrderLineItemRelationshipTypeID
		FROM #ZnodeOmsSavedCartLineItem_NT b 
		INNER JOIN #OldValueForAddon c ON (c.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		SELECT DISTINCT SKU, STUFF(
									( SELECT ', ' + SKU FROM
										( SELECT DISTINCT SKU FROM #OldValueForAddon b 
											where a.OmsSavedCartLineItemId=b.ParentOmsSavedCartLineItemId AND OrderLineItemRelationshipTypeID = 1 ) x 
											FOR XML PATH('')
									), 1, 2, ''
									) AddOns
		INTO #AddOnsExists
		FROM #OldValueForAddon a
		WHERE a.ParentOmsSavedCartLineItemId IS NOT NULL AND OrderLineItemRelationshipTypeID<>1

		SELECT DISTINCT a.ConfigurableProductIds SKU, STUFF(
										( SELECT ', ' + x.AddOnValueIds FROM
										( SELECT DISTINCT b.AddOnValueIds FROM @SaveCartLineItemType b
											where a.SKU=b.SKU ) x
											FOR XML PATH('')
										), 1, 2, ''
									) AddOns
		INTO #AddOnAdded
		FROM @SaveCartLineItemType a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b ON a.SKU = b.SKU AND a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldConfigSavecartLineitemDetails
		END
	END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSavecartLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldConfigSavecartLineitemDetails a WHERE	
		ISNULL(TY.AddOnValueIds,'') = a.SKU AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
		AND EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN
		DELETE FROM #OldConfigSavecartLineitemDetails 
	END
	ELSE 
	BEGIN
		 IF EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
		 BEGIN 
			 DECLARE @parenTofAddon TABLE( ParentOmsSavedCartLineItemId INT)
			 INSERT INTO @parenTofAddon 
			 SELECT ParentOmsSavedCartLineItemId 
			 FROM #OldConfigSavecartLineitemDetails a 
			 WHERE a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
				AND (SELECT COUNT (DISTINCT SKU ) FROM #ZnodeOmsSavedCartLineItem_NT t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId AND t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon)

			 DELETE 
			 FROM #OldConfigSavecartLineitemDetails 
			 WHERE OmsSavedCartLineItemId NOT IN (SELECT ParentOmsSavedCartLineItemId FROM @parenTofAddon)
				AND ParentOmsSavedCartLineItemId IS NOT NULL 
				AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

			 DELETE 
			 FROM #OldConfigSavecartLineitemDetails 
			 WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldConfigSavecartLineitemDetails m)
				AND ParentOmsSavedCartLineItemId IS NULL 
		 
			 IF (SELECT COUNT (DISTINCT SKU ) FROM #OldConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon )
			 BEGIN 
				DELETE FROM #OldConfigSavecartLineitemDetails 
			 END 
			 IF (SELECT COUNT (DISTINCT SKU ) FROM #ZnodeOmsSavedCartLineItem_NT WHERE ParentOmsSavedCartLineItemId IN (SELECT ParentOmsSavedCartLineItemId FROM @parenTofAddon)AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon )
			 BEGIN 
				DELETE FROM #OldConfigSavecartLineitemDetails 
			 END 
		 END 
		 ELSE IF (SELECT COUNT (OmsSavedCartLineItemId) FROM #OldConfigSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS NULL ) > 1 
		 BEGIN 
			-- SELECT 3
			DECLARE @TBL_deleteParentOmsSavedCartLineItemId TABLE (OmsSavedCartLineItemId INT )

			INSERT INTO @TBL_deleteParentOmsSavedCartLineItemId 
			SELECT ParentOmsSavedCartLineItemId
			FROM #ZnodeOmsSavedCartLineItem_NT a
			WHERE ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM #OldConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable )
				AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

			DELETE 
			FROM #OldConfigSavecartLineitemDetails WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
				OR ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
		 
			 DELETE 
			 FROM #OldConfigSavecartLineitemDetails 
			 WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldConfigSavecartLineitemDetails m)
				AND ParentOmsSavedCartLineItemId IS NULL
		 END
		 ELSE IF (SELECT COUNT (DISTINCT SKU ) FROM #OldConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon )
		 BEGIN 
			DELETE FROM #OldConfigSavecartLineitemDetails 
		 END 
		 ELSE IF EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsSavedCartLineItem_NT Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldConfigSavecartLineitemDetails ty WHERE ty.OmsSavedCartId = wt.OmsSavedCartId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable AND wt.ParentOmsSavedCartLineItemId= ty.OmsSavedCartLineItemId ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
			AND EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'') = '' )
		 BEGIN 
			DELETE FROM #OldConfigSavecartLineitemDetails 
		 END 
	END 

	DECLARE @TBL_Personaloldvalues TABLE (OmsSavedCartLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	--Getting the personalise data for old save cart if present		
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsSavedCartLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsPersonalizeCartItem a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldConfigSavecartLineitemDetails TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
	IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN 
		 DELETE FROM #OldConfigSavecartLineitemDetails 
	END 
	ELSE 
	BEGIN 
		 IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
			AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldConfigSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		 BEGIN 
			 DELETE FROM #OldConfigSavecartLineitemDetails WHERE OmsSavedCartLineItemId IN (
			 SELECT OmsSavedCartLineItemId 
			 FROM #OldConfigSavecartLineitemDetails 
			 WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
				 AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ) ) 
				 OR OmsSavedCartLineItemId IN ( SELECT ParentOmsSavedCartLineItemId FROM #OldConfigSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
				 AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ))
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
			AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldConfigSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		BEGIN 
			 DELETE n
			 FROM #OldConfigSavecartLineitemDetails n 
			 WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem WHERE n.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )
				OR ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem )
	
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise)
			 AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldConfigSavecartLineitemDetails YU WHERE YU.OmsSavedCartLineItemId = m.OmsSavedCartLineItemId )) 
			 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldConfigSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) = 1
		BEGIN 
			DELETE FROM #OldConfigSavecartLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise)
		END 
	END 

	IF EXISTS (SELECT TOP 1 1 FROM #OldConfigSavecartLineitemDetails )
	BEGIN
		--------If lineitem present in ZnodeOmsPersonalizeCartItem AND personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsPersonalizeCartItem AND personalize value is same for same line item then Quantity will added
		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,a.ParentOmsSavedCartLineItemId , a.ConfigurableProductIds AS SKU
					,a.PersonalizeCode
			 		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldConfigSavecartLineitemDetails b ON a.ConfigurableProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c ON b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE c1
		FROM cte a1		 
		INNER JOIN #OldConfigSavecartLineitemDetails b1 ON a1.SKU = b1.SKU
		INNER JOIN #OldConfigSavecartLineitemDetails c1 ON (b1.ParentOmsSavedCartLineItemId = c1.OmsSavedCartLineItemId OR b1.OmsSavedCartLineItemId = c1.OmsSavedCartLineItemId)
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsPersonalizeCartItem c where a1.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId AND a1.PersonalizeValue = c.PersonalizeValue)
		-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsSavedCartLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldConfigSavecartLineitemDetails b ON a.ConfigurableProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c ON b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId 
			--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY b.OmsSavedCartLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsSavedCartLineItemId,0) as OmsSavedCartLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewConfigSavecartLineitemDetails b ON a.ConfigurableProductIds = b.SKU
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY a.OmsSavedCartLineItemId ,b.SKU
		)
		DELETE c
		FROM CTE_OldPersonalizeCodeCount a
		INNER JOIN CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		INNER JOIN #OldConfigSavecartLineitemDetails c on b.SKU = c.SKU and a.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
		
		--Delete parent entry if child not present
		DELETE a FROM #OldConfigSavecartLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldConfigSavecartLineitemDetails b where a.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId)
		AND a.ParentOmsSavedCartLineItemId IS NULL

		--Updating the cart if old and new cart data matches
		UPDATE a
		SET a.Quantity = a.Quantity+ty.Quantity,
		a.Custom1 = ty.Custom1,
		a.Custom2 = ty.Custom2,
		a.Custom3 = ty.Custom3,
		a.Custom4 = ty.Custom4,
		a.Custom5 = ty.Custom5,
		a.ModifiedDate = @GetDate ,
		a.CustomUnitPrice = ty.CustomUnitPrice
		FROM ZnodeOmsSavedCartLineItem a
		INNER JOIN #OldConfigSavecartLineitemDetails b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		INNER JOIN #NewConfigSavecartLineitemDetails ty ON (ty.SKU = b.SKU)
		WHERE a.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeIdAddon
		 
		 UPDATE a
		 SET a.Quantity = a.Quantity+s.AddOnQuantity,
		 a.ModifiedDate = @GetDate
		 FROM ZnodeOmsSavedCartLineItem a
		 INNER JOIN #OldConfigSavecartLineitemDetails b ON (a.ParentOmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		 INNER JOIN @SaveCartLineItemType S ON a.OmsSavedCartId = s.OmsSavedCartId AND a.SKU = s.AddOnValueIds
		 WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
	END

	--Added condition to add new product which is coming with existing product.
	IF OBJECT_ID('tempdb..#NewRowId') IS NOT NULL
		DROP TABLE #NewRowId;

	SELECT N.RowId
	INTO #NewRowId
	FROM #NewConfigSavecartLineitemDetails N
	LEFT JOIN #OldConfigSavecartLineitemDetails O ON N.OmsSavedCartId=O.OmsSavedCartId AND N.SKU=O.SKU
	WHERE O.SKU IS NULL

	--Inserting the new save cart data if old and new cart data not match
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldConfigSavecartLineitemDetails)
		OR EXISTS (SELECT TOP 1 1 FROM #NewRowId) --Added condition to add new product which is coming with existing product.
	BEGIN
		--Getting the new save cart data and generating row no. for new save cart insert
		SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			 ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon 
			 ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description ,ParentSKU,CustomUnitPrice 
		INTO #InsertNewConfigSavecartLineitem 
		FROM #NewConfigSavecartLineitemDetails
		WHERE RowId IN (SELECT RowId FROM #NewRowId)
		GROUP BY ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			,CustomText,CartAddOnDetails ,AutoAddon 
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU ,CustomUnitPrice 
		ORDER BY RowId, Id 
 	 
		--Removing the line item having Quantity <=0 
		DELETE 
		FROM #InsertNewConfigSavecartLineitem 
		WHERE Quantity <=0 
 
		--Updating the rowid INTo new save cart line item as new line item is merged INTo existing save cart item
		 ;WITH VTTY AS 
		( 
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU 
			FROM #InsertNewConfigSavecartLineitem m 
			INNER JOIN #InsertNewConfigSavecartLineitem TY1 ON TY1.SKU = m.ParentSKU 
			WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
		) 
		UPDATE m1 
		SET m1.RowId = TYU.RowId 
		FROM #InsertNewConfigSavecartLineitem m1 
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId) 
 
		;WITH VTRET AS 
		( 
			SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID 
			FROM #InsertNewConfigSavecartLineitem 
			GROUP BY RowId,id ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID 
			Having SKU = ParentSKU AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		) 
		DELETE FROM #InsertNewConfigSavecartLineitem WHERE NewRowId IN (SELECT NewRowId FROM VTRET) 
 
	 --Inserting the new cart line item if not merged in existing save cart line item
		INSERT INTO ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			,CustomText,CartAddOnDetails,Sequence,	CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon 
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description,CustomUnitPrice ) 
		OUTPUT INSERTED.OmsSavedCartLineItemId,INSERTED.OmsSavedCartId,inserted.SKU,inserted.GroupId,INSERTED.ParentOmsSavedCartLineItemId,INSERTED.OrderLineItemRelationshipTypeId INTO @OmsInsertedData 
		SELECT NULL ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId) sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon 
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description ,CustomUnitPrice 
		FROM #InsertNewConfigSavecartLineitem TH 

		SELECT MAX(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
			, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU
		INTO #Cte_newData
		FROM @OmsInsertedData a 
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-') ) 
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0 
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			 
	
		UPDATE a 
		SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM #Cte_newData r 
		WHERE r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-') Order by b.RowId ) 
		FROM ZnodeOmsSavedCartLineItem a 
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1 ) 
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon 
			AND a.ParentOmsSavedCartLineItemId IS nULL 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )

	---------------------------------------------------------------------------------------------------------------------

		SELECT MIN(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
			, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU 
		INTO #Cte_newData1 
		FROM @OmsInsertedData a 
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-') ) 
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			

		Select * into #ZnodeOmsSavedCartLineItem_MinSequence  from ZnodeOmsSavedCartLineItem a  with (nolock)  where exists
		(Select TOP 1 1 From #InsertNewConfigSavecartLineitem X where  X.OmsSavedCartId = a.OmsSavedCartId and a.SKU = X.SKU  and X.id =1)
		
		UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM #Cte_newData1 r 
		WHERE r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-') Order by b.RowId ) 
		FROM ZnodeOmsSavedCartLineItem a 
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1 ) 
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
			AND a.sequence in ( SELECT MIN(ab.sequence) FROM #ZnodeOmsSavedCartLineItem_MinSequence ab with (nolock) where a.OmsSavedCartId = ab.OmsSavedCartId AND 
			a.SKU = ab.sku AND ab.OrderLineItemRelationshipTypeId is not null )

			--AND a.sequence in (SELECT MIN(ab.sequence) FROM ZnodeOmsSavedCartLineItem ab with (nolock) where a.OmsSavedCartId = ab.OmsSavedCartId AND 
			--a.SKU = ab.sku AND ab.OrderLineItemRelationshipTypeId is not null ) 
 
	---------------------------------------------------------------------------------------------------------------------------

		SELECT a.OmsSavedCartLineItemId , b.RowId ,b.SKU ,b.ParentSKU ,Row_number()Over(Order BY c.OmsSavedCartLineItemId )RowIdNo
		INTO #NewConfigProduct
		FROM ZnodeOmsSavedCartLineItem a with (nolock) 
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ( b.Id = 1 )) 
		INNER JOIN ZnodeOmsSavedCartLineItem c with (nolock) ON b.sku = c.sku AND b.OmsSavedCartId=c.OmsSavedCartId AND b.Id = 1 
		WHERE ( ISNULL(a.ParentOmsSavedCartLineItemId,0) <> 0 )
			AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId ) AND c.ParentOmsSavedCartLineItemId is null
 
		SELECT * , ROW_NUMBER()Over(Order BY OmsSavedCartLineItemId ) RowIdNo
			 into #table_update
			 FROM ZnodeOmsSavedCartLineItem a with (nolock)
			 WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			 AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
			 AND a.ParentOmsSavedCartLineItemId IS NULL 
			 AND EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
			 AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )

		 --;WITH table_update AS 
		 --(
			-- SELECT * , ROW_NUMBER()Over(Order BY OmsSavedCartLineItemId ) RowIdNo
			-- FROM ZnodeOmsSavedCartLineItem a with (nolock)
			-- WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			-- AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
			-- AND a.ParentOmsSavedCartLineItemId IS NULL 
			-- AND EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
			-- AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
		 --)
		UPDATE a 
		SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 MAX(OmsSavedCartLineItemId) 
		FROM #NewConfigProduct r 
		WHERE r.ParentSKU = b.ParentSKU AND c.SKU = r.SKU AND c.RowIdNo = r.RowIdNo GROUP BY r.ParentSKU, r.SKU ) 
		FROM ZnodeOmsSavedCartLineItem a
		INNER JOIN #table_update c ON a.OmsSavedCartLineItemId=c.OmsSavedCartLineItemId
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (c.OmsSavedCartId = b.OmsSavedCartId AND c.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND b.id =1 ) 
		WHERE (SELECT TOP 1 MAX(OmsSavedCartLineItemId) 
		FROM #NewConfigProduct r 
		WHERE r.ParentSKU = b.ParentSKU AND c.SKU = r.SKU AND c.RowIdNo = r.RowIdNo GROUP BY r.ParentSKU, r.SKU ) IS NOT NULL 

		;WITH Cte_Th AS 
		( 
			 SELECT RowId 
			 FROM #InsertNewConfigSavecartLineitem a 
			 GROUP BY RowId 
			 HAVING COUNT(NewRowId) <= 1 
		) 
		UPDATE a 
		SET a.Quantity = NULL,a.ModifiedDate = @GetDate 
		FROM ZnodeOmsSavedCartLineItem a 
		INNER JOIN #InsertNewConfigSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =0) 
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_Th TY WHERE TY.RowId = b.RowId ) 
			AND a.OrderLineItemRelationshipTypeId IS NULL 
 
		UPDATE ZnodeOmsSavedCartLineItem 
		SET GROUPID = NULL 
		WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSavecartLineitem RT WHERE RT.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId ) 
		AND OrderLineItemRelationshipTypeId IS NOT NULL 
 
		;WITH Cte_UpdateSequence AS 
		( 
			 SELECT OmsSavedCartLineItemId ,Row_Number()Over(Order By OmsSavedCartLineItemId) RowId , Sequence 
			 FROM ZnodeOmsSavedCartLineItem 
			 WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSavecartLineitem TH WHERE TH.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId ) 
		) 
		UPDATE Cte_UpdateSequence 
		SET Sequence = RowId 

		UPDATE @TBL_Personalise
		SET OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN @TBL_Personalise c ON a.SKU = c.ConfigurableProductIds
		INNER JOIN ZnodeOmsSavedCartLineItem b with (nolock) ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsSavedCartLineItemId IS not nULL 
	
		DELETE 
		FROM ZnodeOmsPersonalizeCartItem 
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )
		
		----Inserting saved cart item personalise value FROM given line item
		MERGE INTO ZnodeOmsPersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
			 ON (TARGET.OmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId ) 
			 WHEN NOT MATCHED THEN 
				INSERT ( OmsSavedCartLineItemId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
								,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL )
				VALUES ( SOURCE.OmsSavedCartLineItemId, @userId, @getdate, @userId, @getdate
								,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL ) ;
	END
	END TRY
	BEGIN CATCH 
		SELECT ERROR_MESSAGE()
	END CATCH 
END