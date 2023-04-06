CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveForLaterLineItemConfigurable]
(
	 @SaveForLaterLineItemType SaveForLaterLineitems READONLY
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

	DECLARE @OmsInsertedData TABLE (OmsTemplateLineItemId INT, OmsTemplateId INT,SKU NVARCHAr(max),GroupId NVARCHAr(max),ParentOmsTemplateLineItemId INT,OrderLineItemRelationshipTypeId INT ) 
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise TABLE (OmsTemplateLineItemId INT, ParentOmsTemplateLineItemId INT,ConfigurableProductIds VARCHAR(600) ,PersonalizeCode NVARCHAR(MAX),PersonalizeValue NVARCHAR(MAX),DesignId NVARCHAR(MAX), ThumbnailURL NVARCHAR(MAX),PersonalizeName NVARCHAR(max))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsTemplateLineItemId,a.ConfigurableProductIds
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(MAX)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(MAX)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(MAX)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(MAX)' ) AS ThumbnailURL
			,Tbl.Col.value( 'PersonalizeName[1]', 'NVARCHAR(MAX)' ) AS PersonalizeName
	FROM @SaveForLaterLineItemType a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)

	 CREATE TABLE #NewConfigSaveForLaterLineitemDetails 
	 (
		 GenId INT IDENTITY(1,1), RowId	INT, OmsTemplateLineItemId	INT, ParentOmsTemplateLineItemId INT,OmsTemplateId INT
		,SKU NVARCHAR(MAX) ,Quantity	NUMERIC(28,6), OrderLineItemRelationshipTypeID	INT	,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	INT	,AutoAddon	VARCHAR(MAX)	,OmsOrderId	INT	,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX), Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX),
		CustomUnitPrice NUMERIC(28,6)
	)
	 
	--Getting new save cart data(configurable product)
	INSERT INTO #NewConfigSaveForLaterLineitemDetails
	SELECT MIN(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5, GroupId ,ProductName,MIN(Description) Description, 0 Id,NULL ParentSKU ,
		CustomUnitPrice
	FROM @SaveForLaterLineItemType a 
	GROUP BY OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,CustomUnitPrice
	 
	--Getting new configurable product save cart data
	INSERT INTO #NewConfigSaveForLaterLineitemDetails 
	SELECT MIN(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, ConfigurableProductIds
		,Quantity, @OrderLineItemRelationshipTypeIdConfigurable, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU,
		CustomUnitPrice
	FROM @SaveForLaterLineItemType a 
	WHERE ConfigurableProductIds <> ''
	GROUP BY OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, ConfigurableProductIds
		,Quantity, CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,
		CustomUnitPrice
	
	--Getting new configurable products save cart data if addon is present for any line item
	INSERT INTO #NewConfigSaveForLaterLineitemDetails
	SELECT MIN(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
		,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
		,CASE WHEN ConfigurableProductIds <> '' THEN ConfigurableProductIds
			WHEN GroupProductIds <> '' THEN GroupProductIds 
			WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END ParentSKU ,CustomUnitPrice
	FROM @SaveForLaterLineItemType a 
	WHERE AddOnValueIds <> ''
	GROUP BY OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
		,AddOnQuantity, CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		 
	SELECT * 
	INTO #ZnodeOmsTemplateLineItem_NT
	FROM ZnodeOmsTemplateLineItem a 
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType TY WHERE TY.OmsTemplateId = a.OmsTemplateId)

	CREATE TABLE #OldConfigSaveForLaterLineitemDetails (OmsTemplateId INT ,OmsTemplateLineItemId INT,ParentOmsTemplateLineItemId INT , SKU NVARCHAr(2000),OrderLineItemRelationshipTypeID INT )
	--Getting the old configurable save cart data if present for same SKU in the new save cart data for configurable product	 
	INSERT INTO #OldConfigSaveForLaterLineitemDetails
	SELECT a.OmsTemplateId,a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId , a.SKU ,a.OrderLineItemRelationshipTypeID 
	FROM #ZnodeOmsTemplateLineItem_NT a
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType TY WHERE TY.OmsTemplateId = a.OmsTemplateId AND ISNULL(a.SKU,'') = ISNULL(TY.ConfigurableProductIds,''))
		AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable

	--Getting the old save cart Parent data
	INSERT INTO #OldConfigSaveForLaterLineitemDetails 
	SELECT DISTINCT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU, b.OrderLineItemRelationshipTypeID
	FROM #ZnodeOmsTemplateLineItem_NT b 
	INNER JOIN #OldConfigSaveForLaterLineitemDetails c ON (c.ParentOmsTemplateLineItemId = b.OmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-') )
		AND b.OrderLineItemRelationshipTypeID IS NULL 
		
	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldConfigSaveForLaterLineitemDetails

	DELETE a 
	FROM #OldConfigSaveForLaterLineitemDetails a 
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM #OldConfigSaveForLaterLineitemDetails b WHERE b.ParentOmsTemplateLineItemId IS NULL AND b.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)
		AND a.ParentOmsTemplateLineItemId IS NOT NULL 

	INSERT INTO #OldConfigSaveForLaterLineitemDetails 
	SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU, b.OrderLineItemRelationshipTypeID
	FROM #ZnodeOmsTemplateLineItem_NT b 
	INNER JOIN #OldConfigSaveForLaterLineitemDetails c ON (c.OmsTemplateLineItemId = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon
		
	------Merge Addon for same product
	IF EXISTS(SELECT * FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN		
		INSERT INTO #OldValueForAddon 
		SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU, b.OrderLineItemRelationshipTypeID
		FROM #ZnodeOmsTemplateLineItem_NT b 
		INNER JOIN #OldValueForAddon c ON (c.OmsTemplateLineItemId = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType TY WHERE TY.OmsTemplateId = b.OmsTemplateId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		SELECT DISTINCT SKU, STUFF(
									( SELECT ', ' + SKU FROM
										( SELECT DISTINCT SKU FROM #OldValueForAddon b 
											where a.OmsTemplateLineItemId=b.ParentOmsTemplateLineItemId AND OrderLineItemRelationshipTypeID = 1 ) x 
											FOR XML PATH('')
									), 1, 2, ''
									) AddOns
		INTO #AddOnsExists
		FROM #OldValueForAddon a
		WHERE a.ParentOmsTemplateLineItemId IS NOT NULL AND OrderLineItemRelationshipTypeID<>1

		SELECT DISTINCT a.ConfigurableProductIds SKU, STUFF(
										( SELECT ', ' + x.AddOnValueIds FROM
										( SELECT DISTINCT b.AddOnValueIds FROM @SaveForLaterLineItemType b
											where a.SKU=b.SKU ) x
											FOR XML PATH('')
										), 1, 2, ''
									) AddOns
		INTO #AddOnAdded
		FROM @SaveForLaterLineItemType a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b ON a.SKU = b.SKU AND a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldConfigSaveForLaterLineitemDetails
		END
	END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSaveForLaterLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldConfigSaveForLaterLineitemDetails a WHERE	
		ISNULL(TY.AddOnValueIds,'') = a.SKU AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
		AND EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN
		DELETE FROM #OldConfigSaveForLaterLineitemDetails 
	END
	ELSE 
	BEGIN
		 IF EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
		 BEGIN 
			 DECLARE @parenTofAddon TABLE( ParentOmsTemplateLineItemId INT)
			 INSERT INTO @parenTofAddon 
			 SELECT ParentOmsTemplateLineItemId 
			 FROM #OldConfigSaveForLaterLineitemDetails a 
			 WHERE a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
				AND (SELECT COUNT (DISTINCT SKU ) FROM #ZnodeOmsTemplateLineItem_NT t WHERE t.ParentOmsTemplateLineItemId = a.ParentOmsTemplateLineItemId AND t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon)

			 DELETE 
			 FROM #OldConfigSaveForLaterLineitemDetails 
			 WHERE OmsTemplateLineItemId NOT IN (SELECT ParentOmsTemplateLineItemId FROM @parenTofAddon)
				AND ParentOmsTemplateLineItemId IS NOT NULL 
				AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

			 DELETE 
			 FROM #OldConfigSaveForLaterLineitemDetails 
			 WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldConfigSaveForLaterLineitemDetails m)
				AND ParentOmsTemplateLineItemId IS NULL 
		 
			 IF (SELECT COUNT (DISTINCT SKU ) FROM #OldConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon )
			 BEGIN 
				DELETE FROM #OldConfigSaveForLaterLineitemDetails 
			 END 
			 IF (SELECT COUNT (DISTINCT SKU ) FROM #ZnodeOmsTemplateLineItem_NT WHERE ParentOmsTemplateLineItemId IN (SELECT ParentOmsTemplateLineItemId FROM @parenTofAddon)AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon )
			 BEGIN 
				DELETE FROM #OldConfigSaveForLaterLineitemDetails 
			 END 
		 END 
		 ELSE IF (SELECT COUNT (OmsTemplateLineItemId) FROM #OldConfigSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS NULL ) > 1 
		 BEGIN 
			-- SELECT 3
			DECLARE @TBL_deleteParentOmsTemplateLineItemId TABLE (OmsTemplateLineItemId INT )

			INSERT INTO @TBL_deleteParentOmsTemplateLineItemId 
			SELECT ParentOmsTemplateLineItemId
			FROM #ZnodeOmsTemplateLineItem_NT a
			WHERE ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM #OldConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable )
				AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

			DELETE 
			FROM #OldConfigSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
				OR ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
		 
			 DELETE 
			 FROM #OldConfigSaveForLaterLineitemDetails 
			 WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldConfigSaveForLaterLineitemDetails m)
				AND ParentOmsTemplateLineItemId IS NULL
		 END
		 ELSE IF (SELECT COUNT (DISTINCT SKU ) FROM #OldConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM #NewConfigSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon )
		 BEGIN 
			DELETE FROM #OldConfigSaveForLaterLineitemDetails 
		 END 
		 ELSE IF EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsTemplateLineItem_NT Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldConfigSaveForLaterLineitemDetails ty WHERE ty.OmsTemplateId = wt.OmsTemplateId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdConfigurable AND wt.ParentOmsTemplateLineItemId= ty.OmsTemplateLineItemId ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
			AND EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'') = '' )
		 BEGIN 
			DELETE FROM #OldConfigSaveForLaterLineitemDetails 
		 END 
	END 

	DECLARE @TBL_Personaloldvalues TABLE (OmsTemplateLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	--Getting the personalise data for old save cart if present		
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsTemplateLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsTemplatePersonalizeCartItem a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldConfigSaveForLaterLineitemDetails TY WHERE TY.OmsTemplateLineItemId = a.OmsTemplateLineItemId)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
	IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN 
		 DELETE FROM #OldConfigSaveForLaterLineitemDetails 
	END 
	ELSE 
	BEGIN 
		 IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
			AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldConfigSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		 BEGIN 
			 DELETE FROM #OldConfigSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId IN (
			 SELECT OmsTemplateLineItemId 
			 FROM #OldConfigSaveForLaterLineitemDetails 
			 WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
				 AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues ) ) 
				 OR OmsTemplateLineItemId IN ( SELECT ParentOmsTemplateLineItemId FROM #OldConfigSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
				 AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues ))
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
			AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldConfigSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		BEGIN 
			 DELETE n
			 FROM #OldConfigSaveForLaterLineitemDetails n 
			 WHERE OmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem WHERE n.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId )
				OR ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem )
	
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise)
			 AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplatePersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldConfigSaveForLaterLineitemDetails YU WHERE YU.OmsTemplateLineItemId = m.OmsTemplateLineItemId )) 
			 AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldConfigSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) = 1
		BEGIN 
			DELETE FROM #OldConfigSaveForLaterLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise)
		END 
	END 

	IF EXISTS (SELECT TOP 1 1 FROM #OldConfigSaveForLaterLineitemDetails )
	BEGIN
		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem AND personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem AND personalize value is same for same line item then Quantity will added
		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,a.ParentOmsTemplateLineItemId , a.ConfigurableProductIds AS SKU
					,a.PersonalizeCode
			 		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldConfigSaveForLaterLineitemDetails b ON a.ConfigurableProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c ON b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
			WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE c1
		FROM cte a1		 
		INNER JOIN #OldConfigSaveForLaterLineitemDetails b1 ON a1.SKU = b1.SKU
		INNER JOIN #OldConfigSaveForLaterLineitemDetails c1 ON (b1.ParentOmsTemplateLineItemId = c1.OmsTemplateLineItemId OR b1.OmsTemplateLineItemId = c1.OmsTemplateLineItemId)
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsTemplatePersonalizeCartItem c where a1.OmsTemplateLineItemId = c.OmsTemplateLineItemId AND a1.PersonalizeValue = c.PersonalizeValue)
		-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsTemplateLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldConfigSaveForLaterLineitemDetails b ON a.ConfigurableProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c ON b.OmsTemplateLineItemId = c.OmsTemplateLineItemId 
			--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0
			GROUP BY b.OmsTemplateLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsTemplateLineItemId,0) as OmsTemplateLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewConfigSaveForLaterLineitemDetails b ON a.ConfigurableProductIds = b.SKU
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0
			GROUP BY a.OmsTemplateLineItemId ,b.SKU
		)
		DELETE c
		FROM CTE_OldPersonalizeCodeCount a
		INNER JOIN CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		INNER JOIN #OldConfigSaveForLaterLineitemDetails c on b.SKU = c.SKU and a.OmsTemplateLineItemId = c.OmsTemplateLineItemId
		
		--Delete parent entry if child not present
		DELETE a FROM #OldConfigSaveForLaterLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldConfigSaveForLaterLineitemDetails b where a.OmsTemplateLineItemId = b.ParentOmsTemplateLineItemId)
		AND a.ParentOmsTemplateLineItemId IS NULL

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
		FROM ZnodeOmsTemplateLineItem a
		INNER JOIN #OldConfigSaveForLaterLineitemDetails b ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId)
		INNER JOIN #NewConfigSaveForLaterLineitemDetails ty ON (ty.SKU = b.SKU)
		WHERE a.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeIdAddon
		 
		 UPDATE a
		 SET a.Quantity = a.Quantity+s.AddOnQuantity,
		 a.ModifiedDate = @GetDate
		 FROM ZnodeOmsTemplateLineItem a
		 INNER JOIN #OldConfigSaveForLaterLineitemDetails b ON (a.ParentOmsTemplateLineItemId = b.OmsTemplateLineItemId)
		 INNER JOIN @SaveForLaterLineItemType S ON a.OmsTemplateId = s.OmsTemplateId AND a.SKU = s.AddOnValueIds
		 WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
	END

	--Added condition to add new product which is coming with existing product.
	IF OBJECT_ID('tempdb..#NewRowId') IS NOT NULL
		DROP TABLE #NewRowId;

	SELECT N.RowId
	INTO #NewRowId
	FROM #NewConfigSaveForLaterLineitemDetails N
	LEFT JOIN #OldConfigSaveForLaterLineitemDetails O ON N.OmsTemplateId=O.OmsTemplateId AND N.SKU=O.SKU
	WHERE O.SKU IS NULL

	--Inserting the new save cart data if old and new cart data not match
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldConfigSaveForLaterLineitemDetails)
		OR EXISTS (SELECT TOP 1 1 FROM #NewRowId) --Added condition to add new product which is coming with existing product.
	BEGIN
		--Getting the new save cart data and generating row no. for new save cart insert
		SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			 ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon 
			 ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description ,ParentSKU,CustomUnitPrice 
		INTO #InsertNewConfigSaveForLaterLineitem 
		FROM #NewConfigSaveForLaterLineitemDetails
		WHERE RowId IN (SELECT RowId FROM #NewRowId)
		GROUP BY ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			,CustomText,CartAddOnDetails ,AutoAddon 
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU ,CustomUnitPrice 
		ORDER BY RowId, Id 
 	 
		--Removing the line item having Quantity <=0 
		DELETE 
		FROM #InsertNewConfigSaveForLaterLineitem 
		WHERE Quantity <=0 
 
		--Updating the rowid INTo new save cart line item as new line item is merged INTo existing save cart item
		 ;WITH VTTY AS 
		( 
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU 
			FROM #InsertNewConfigSaveForLaterLineitem m 
			INNER JOIN #InsertNewConfigSaveForLaterLineitem TY1 ON TY1.SKU = m.ParentSKU 
			WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
		) 
		UPDATE m1 
		SET m1.RowId = TYU.RowId 
		FROM #InsertNewConfigSaveForLaterLineitem m1 
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId) 
 
		;WITH VTRET AS 
		( 
			SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID 
			FROM #InsertNewConfigSaveForLaterLineitem 
			GROUP BY RowId,id ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID 
			Having SKU = ParentSKU AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		) 
		DELETE FROM #InsertNewConfigSaveForLaterLineitem WHERE NewRowId IN (SELECT NewRowId FROM VTRET) 
 
	 --Inserting the new cart line item if not merged in existing save cart line item
		INSERT INTO ZnodeOmsTemplateLineItem (ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			,CustomText,CartAddOnDetails,Sequence,	CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon 
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description,CustomUnitPrice ) 
		OUTPUT INSERTED.OmsTemplateLineItemId,INSERTED.OmsTemplateId,inserted.SKU,inserted.GroupId,INSERTED.ParentOmsTemplateLineItemId,INSERTED.OrderLineItemRelationshipTypeId INTO @OmsInsertedData 
		SELECT NULL ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId 
			,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId) sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon 
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description ,CustomUnitPrice 
		FROM #InsertNewConfigSaveForLaterLineitem TH 

		SELECT MAX(a.OmsTemplateLineItemId ) OmsTemplateLineItemId 
			, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU
		INTO #Cte_newData
		FROM @OmsInsertedData a 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-') ) 
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0 
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			 
	
		UPDATE a 
		SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM #Cte_newData r 
		WHERE r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-') Order by b.RowId ) 
		FROM ZnodeOmsTemplateLineItem a 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1 ) 
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon 
			AND a.ParentOmsTemplateLineItemId IS nULL 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )

	---------------------------------------------------------------------------------------------------------------------

		SELECT MIN(a.OmsTemplateLineItemId ) OmsTemplateLineItemId 
			, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU 
		INTO #Cte_newData1 
		FROM @OmsInsertedData a 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-') ) 
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			

		UPDATE a SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM #Cte_newData1 r 
		WHERE r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-') Order by b.RowId ) 
		FROM ZnodeOmsTemplateLineItem a 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1 ) 
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
			AND a.sequence in (SELECT MIN(ab.sequence) FROM ZnodeOmsTemplateLineItem ab where a.OmsTemplateId = ab.OmsTemplateId AND 
			a.SKU = ab.sku AND ab.OrderLineItemRelationshipTypeId is not null ) 
 
	---------------------------------------------------------------------------------------------------------------------------

		SELECT a.OmsTemplateLineItemId , b.RowId ,b.SKU ,b.ParentSKU ,Row_number()Over(Order BY c.OmsTemplateLineItemId )RowIdNo
		INTO #NewConfigProduct
		FROM ZnodeOmsTemplateLineItem a with (nolock) 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ( b.Id = 1 )) 
		INNER JOIN ZnodeOmsTemplateLineItem c with (nolock) ON b.sku = c.sku AND b.OmsTemplateId=c.OmsTemplateId AND b.Id = 1 
		WHERE ( ISNULL(a.ParentOmsTemplateLineItemId,0) <> 0 )
			AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
			AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId ) AND c.ParentOmsTemplateLineItemId is null
 
		 ;WITH table_update AS 
		 (
			 SELECT * , ROW_NUMBER()Over(Order BY OmsTemplateLineItemId ) RowIdNo
			 FROM ZnodeOmsTemplateLineItem a with (nolock)
			 WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL 
			 AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
			 AND a.ParentOmsTemplateLineItemId IS NULL 
			 AND EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSaveForLaterLineitem ty WHERE ty.OmsTemplateId = a.OmsTemplateId )
			 AND EXISTS (SELECT TOP 1 1 FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
		 )
		UPDATE a 
		SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 max(OmsTemplateLineItemId) 
		FROM #NewConfigProduct r 
		WHERE r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo GROUP BY r.ParentSKU, r.SKU ) 
		FROM table_update a 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND b.id =1 ) 
		WHERE (SELECT TOP 1 max(OmsTemplateLineItemId) 
		FROM #NewConfigProduct r 
		WHERE r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo GROUP BY r.ParentSKU, r.SKU ) IS nOT NULL 

		;WITH Cte_Th AS 
		( 
			 SELECT RowId 
			 FROM #InsertNewConfigSaveForLaterLineitem a 
			 GROUP BY RowId 
			 HAVING COUNT(NewRowId) <= 1 
		) 
		UPDATE a 
		SET a.Quantity = NULL,a.ModifiedDate = @GetDate 
		FROM ZnodeOmsTemplateLineItem a 
		INNER JOIN #InsertNewConfigSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =0) 
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_Th TY WHERE TY.RowId = b.RowId ) 
			AND a.OrderLineItemRelationshipTypeId IS NULL 
 
		UPDATE ZnodeOmsTemplateLineItem 
		SET GROUPID = NULL 
		WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSaveForLaterLineitem RT WHERE RT.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId ) 
		AND OrderLineItemRelationshipTypeId IS NOT NULL 
 
		;WITH Cte_UpdateSequence AS 
		( 
			 SELECT OmsTemplateLineItemId ,Row_Number()Over(Order By OmsTemplateLineItemId) RowId , Sequence 
			 FROM ZnodeOmsTemplateLineItem 
			 WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewConfigSaveForLaterLineitem TH WHERE TH.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId ) 
		) 
		UPDATE Cte_UpdateSequence 
		SET Sequence = RowId 

		UPDATE @TBL_Personalise
		SET OmsTemplateLineItemId = b.OmsTemplateLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN @TBL_Personalise c ON a.SKU = c.ConfigurableProductIds
		INNER JOIN ZnodeOmsTemplateLineItem b with (nolock) ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsTemplateLineItemId IS not nULL 
	
		DELETE 
		FROM ZnodeOmsTemplatePersonalizeCartItem 
		WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId )
		
		----Inserting saved cart item personalise value FROM given line item
		MERGE INTO ZnodeOmsTemplatePersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
			 ON (TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId ) 
			 WHEN NOT MATCHED THEN 
				INSERT ( OmsTemplateLineItemId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
								,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL, PersonalizeName )
				VALUES ( SOURCE.OmsTemplateLineItemId, @userId, @getdate, @userId, @getdate
								,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL, SOURCE.PersonalizeName ) ;
	END
	END TRY
	BEGIN CATCH 
		SELECT ERROR_MESSAGE()
	END CATCH 
END
