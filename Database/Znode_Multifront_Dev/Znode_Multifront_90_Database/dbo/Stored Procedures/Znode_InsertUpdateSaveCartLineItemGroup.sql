CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItemGroup]
(
	 @SaveCartLineItemType TT_SavecartLineitems READONLY  
	,@Userid  INT = 0 
	,@OrderLineItemRelationshipTypeIdGroup INT
	,@OrderLineItemRelationshipTypeIdAddon INT
)
AS 
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
	--Declared the variables
	DECLARE @GetDate datetime= dbo.Fn_GetDate(); 
	
	DECLARE @OmsInsertedData TABLE (SKU varchar(600),OmsSavedCartLineItemId INT ) 	
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise TABLE (OmsSavedCartLineItemId INT, ParentOmsSavedCartLineItemId INT,GroupProductIds VARCHAR(600) ,PersonalizeCode NVARCHAR(MAX),PersonalizeValue NVARCHAR(MAX),DesignId NVARCHAR(MAX), ThumbnailURL NVARCHAR(MAX))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsSavedCartLineItemId,a.GroupProductIds
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(MAX)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(MAX)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(MAX)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(MAX)' ) AS ThumbnailURL
	FROM @SaveCartLineItemType a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  

	CREATE TABLE #NewGroupSavecartLineitemDetails 
	(
		GenId INT IDENTITY(1,1),RowId	INT ,OmsSavedCartLineItemId	INT,ParentOmsSavedCartLineItemId	INT,OmsSavedCartId	int
		,SKU	NVARCHAR(MAX) ,Quantity	numeric(28,6)	,OrderLineItemRelationshipTypeID	INT ,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	INT ,AutoAddon	VARCHAR(MAX)	,OmsOrderId	INT ,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX)  ,Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX),
		CustomUnitPrice NUMERIC(28,6)
	)
	 
	--Getting new save cart data(group product)
	INSERT INTO #NewGroupSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU ,CustomUnitPrice
	FROM @SaveCartLineItemType a 
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,CustomUnitPrice
	 
	--Getting new group product save cart data
	INSERT INTO #NewGroupSavecartLineitemDetails 
	SELECT   Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, GroupProductIds
				,Quantity, @OrderLineItemRelationshipTypeIdGroup, CustomText, CartAddOnDetails, Sequence
				,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU,CustomUnitPrice  
	FROM @SaveCartLineItemType  a 
	WHERE GroupProductIds <> ''
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, GroupProductIds
		,Quantity,  CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		
	--Getting new group products save cart data if addon is present for any line item
	INSERT INTO #NewGroupSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
		,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
		,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
			WHEN  GroupProductIds <> '' THEN GroupProductIds 
			WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END     ParentSKU ,CustomUnitPrice
	FROM @SaveCartLineItemType  a 
	WHERE AddOnValueIds <> ''
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
		,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		 

	CREATE TABLE #OldGroupSavecartLineitemDetails (OmsSavedCartId INT ,OmsSavedCartLineItemId INT,ParentOmsSavedCartLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
	--Getting the old group save cart data if present for same SKU in the new save cart data for group product	 	 
	INSERT INTO #OldGroupSavecartLineitemDetails  
	SELECT  a.OmsSavedCartId,a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	FROM ZnodeOmsSavedCartLineItem a with (nolock)  
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType  TY WHERE TY.OmsSavedCartId = a.OmsSavedCartId AND ISNULL(a.SKU,'') = ISNULL(TY.GroupProductIds,'')   )   
		AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup   

	--Getting the old save cart Parent data
	INSERT INTO #OldGroupSavecartLineitemDetails 
	SELECT DISTINCT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsSavedCartLineItem b with (nolock)
	INNER JOIN #OldGroupSavecartLineitemDetails c ON (c.ParentOmsSavedCartLineItemId  = b.OmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
		AND  b.OrderLineItemRelationshipTypeID IS NULL 

	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldGroupSavecartLineitemDetails

	DELETE a FROM #OldGroupSavecartLineitemDetails a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldGroupSavecartLineitemDetails b WHERE b.ParentOmsSavedCartLineItemId IS NULL AND b.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)
	AND a.ParentOmsSavedCartLineItemId IS NOT NULL 

	--Getting the old config product save cart addon data for old line items if present
	INSERT INTO #OldGroupSavecartLineitemDetails 
	SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsSavedCartLineItem b with (nolock)
	INNER JOIN #OldGroupSavecartLineitemDetails c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

	------Merge Addon for same product
	IF EXISTS(SELECT * FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN
		INSERT INTO #OldValueForAddon 
		SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
		FROM ZnodeOmsSavedCartLineItem b 
		INNER JOIN #OldValueForAddon c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
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

		SELECT DISTINCT a.GroupProductIds SKU, STUFF(
									( SELECT  ', ' + x.AddOnValueIds FROM    
									( SELECT DISTINCT b.AddOnValueIds FROM @SaveCartLineItemType b
										where a.SKU=b.SKU ) x
										FOR XML PATH('')
									), 1, 2, ''
								) AddOns
		INTO #AddOnAdded
		FROM @SaveCartLineItemType a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldGroupSavecartLineitemDetails
		END
	END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSavecartLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1  FROM @SaveCartLineItemType ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldGroupSavecartLineitemDetails a WHERE	
		ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
		AND EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
	BEGIN
		DELETE FROM #OldGroupSavecartLineitemDetails 
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN
			DECLARE @parenTofAddon  TABLE( ParentOmsSavedCartLineItemId INT  )  
			INSERT INTO  @parenTofAddon 
			SELECT  ParentOmsSavedCartLineItemId FROM #OldGroupSavecartLineitemDetails a
			WHERE a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
			AND (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsSavedCartLineItem  t with (nolock) WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId AND   t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
		  
			DELETE 
			FROM #OldGroupSavecartLineitemDetails 
			WHERE OmsSavedCartLineItemId NOT IN (SELECT ParentOmsSavedCartLineItemId FROM  @parenTofAddon)   
				AND ParentOmsSavedCartLineItemId IS NOT NULL  
				AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

			 DELETE 
			 FROM #OldGroupSavecartLineitemDetails 
			 WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldGroupSavecartLineitemDetails m)
				AND ParentOmsSavedCartLineItemId IS  NULL  

			IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldGroupSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			BEGIN 
				DELETE FROM #OldGroupSavecartLineitemDetails
			END 
			IF (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsSavedCartLineItem with (nolock)  WHERE ParentOmsSavedCartLineItemId IN (SELECT ParentOmsSavedCartLineItemId FROM @parenTofAddon)AND   OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			BEGIN 
				DELETE FROM #OldGroupSavecartLineitemDetails
			END
		END 
		ELSE IF (SELECT COUNT (OmsSavedCartLineItemId) FROM #OldGroupSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS NULL ) > 1 
		BEGIN 
			-- SELECT 3
		    DECLARE @TBL_deleteParentOmsSavedCartLineItemId TABLE (OmsSavedCartLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsSavedCartLineItemId 
			SELECT ParentOmsSavedCartLineItemId
			FROM ZnodeOmsSavedCartLineItem a with (nolock)
			WHERE ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM #OldGroupSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup  )
				AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE 
			FROM #OldGroupSavecartLineitemDetails 
			WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
				OR ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
		    
			DELETE 
			FROM #OldGroupSavecartLineitemDetails 
			WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldGroupSavecartLineitemDetails m)
				AND ParentOmsSavedCartLineItemId IS NULL
		END
		ELSE IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldGroupSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
		BEGIN 
			DELETE FROM #OldGroupSavecartLineitemDetails
		END 
		ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldGroupSavecartLineitemDetails ty WHERE ty.OmsSavedCartId = wt.OmsSavedCartId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup AND wt.ParentOmsSavedCartLineItemId= ty.OmsSavedCartLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
			AND EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'')  = '' )
		BEGIN 
			DELETE FROM #OldGroupSavecartLineitemDetails
		END  
	END

	DECLARE @TBL_Personaloldvalues TABLE (OmsSavedCartLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	--Getting the personalise data for old save cart if present	
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsSavedCartLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsPersonalizeCartItem  a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldGroupSavecartLineitemDetails TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
	IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN
		DELETE FROM #OldGroupSavecartLineitemDetails
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldGroupSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		BEGIN 
			DELETE 
			FROM #OldGroupSavecartLineitemDetails 
			WHERE OmsSavedCartLineItemId IN (
				SELECT OmsSavedCartLineItemId FROM #OldGroupSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
				AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues)) 
				OR OmsSavedCartLineItemId IN ( SELECT ParentOmsSavedCartLineItemId FROM #OldGroupSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
				AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues))
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
			AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldGroupSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
			AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldGroupSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) <>
				(SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM ZnodeOmsSavedCartLineItem WHERE ParentOmsSavedCartLineItemId IS nULL and OmsSavedCartLineItemId in (select OmsSavedCartLineItemId FROM #OldGroupSavecartLineitemDetails)  )
		BEGIN
			DELETE n 
			FROM #OldGroupSavecartLineitemDetails n 
			WHERE OmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem WHERE n.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId  )
			OR ParentOmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem   )
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
			AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldGroupSavecartLineitemDetails YU WHERE YU.OmsSavedCartLineItemId = m.OmsSavedCartLineItemId )) 
			AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldGroupSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) = 1
		BEGIN 
			DELETE FROM #OldGroupSavecartLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		END 
	END
	
	IF EXISTS (SELECT TOP 1 1 FROM #OldGroupSavecartLineitemDetails)
	BEGIN
		----DELETE old value FROM table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU not having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*
			FROM @SaveCartLineItemType a 
					INNER JOIN #OldGroupSavecartLineitemDetails b on ( a.GroupProductIds = b.SKU or a.SKU = b.sku)
					where isnull(cast(a.PersonalisedAttribute AS varchar(max)),'') = ''
		)
		,cte2 AS
		(
			select a.ParentOmsSavedCartLineItemId
			FROM #OldGroupSavecartLineitemDetails a
			INNER JOIN ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId
		)
		DELETE a FROM #OldGroupSavecartLineitemDetails a
		INNER JOIN cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		INNER JOIN cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		----DELETE old value FROM table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*, 
			a.PersonalizeCode
			,a.PersonalizeValue
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSavecartLineitemDetails b on ( a.GroupProductIds = b.SKU)
			where isnull(a.PersonalizeValue,'') <> ''
		)
		,cte2 AS
		(
			select a.ParentOmsSavedCartLineItemId, b.PersonalizeCode, b.PersonalizeValue
			FROM #OldGroupSavecartLineitemDetails a
			INNER JOIN ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId
			WHERE NOT EXISTS(SELECT * FROM cte c where b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and b.PersonalizeCode = c.PersonalizeCode 
								and b.PersonalizeValue = c.PersonalizeValue )
		)
		DELETE a FROM #OldGroupSavecartLineitemDetails a
		INNER JOIN cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		INNER JOIN cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,b.ParentOmsSavedCartLineItemId , a.GroupProductIds
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSavecartLineitemDetails b on a.GroupProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE b1
		FROM #OldGroupSavecartLineitemDetails b1 
		WHERE NOT EXISTS(SELECT * FROM cte c where (b1.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or b1.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId))
		AND EXISTS(SELECT * FROM cte)

		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is same for same line item then Quantity will added
		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,a.ParentOmsSavedCartLineItemId , a.GroupProductIds AS SKU
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSavecartLineitemDetails b on a.GroupProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE c1
		FROM cte a1		  
		INNER JOIN #OldGroupSavecartLineitemDetails b1 on a1.SKU = b1.SKU
		INNER JOIN #OldGroupSavecartLineitemDetails c1 on (b1.ParentOmsSavedCartLineItemId = c1.OmsSavedCartLineItemId OR b1.OmsSavedCartLineItemId = c1.OmsSavedCartLineItemId)
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsPersonalizeCartItem c where a1.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and a1.PersonalizeValue = c.PersonalizeValue)
		-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsSavedCartLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSavecartLineitemDetails b ON a.GroupProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c ON b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId 
				--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY b.OmsSavedCartLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsSavedCartLineItemId,0) as OmsSavedCartLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewGroupSavecartLineitemDetails b ON a.GroupProductIds = b.SKU
			WHERE ISNULL(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY a.OmsSavedCartLineItemId ,b.SKU
		)
		DELETE c
		from CTE_OldPersonalizeCodeCount a
		INNER JOIN CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		INNER JOIN #OldGroupSavecartLineitemDetails c on b.SKU = c.SKU and a.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
		
		--Delete parent entry if child not present
		DELETE a FROM #OldGroupSavecartLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldGroupSavecartLineitemDetails b where a.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId)
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
		INNER JOIN #OldGroupSavecartLineitemDetails b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		INNER JOIN #NewGroupSavecartLineitemDetails ty ON (ty.SKU = b.SKU)
		WHERE a.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeIdAddon

		UPDATE a
		SET a.Quantity = a.Quantity+s.AddOnQuantity,
			a.ModifiedDate = @GetDate
		FROM ZnodeOmsSavedCartLineItem a
		INNER JOIN #OldGroupSavecartLineitemDetails b ON (a.ParentOmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		INNER JOIN @SaveCartLineItemType S on a.OmsSavedCartId = s.OmsSavedCartId and a.SKU = s.AddOnValueIds
		WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
	END

	--Added condition to add new product which is coming with existing product.
	IF OBJECT_ID('tempdb..#NewRowId') IS NOT NULL
		DROP TABLE #NewRowId;

	SELECT N.RowId
	INTO #NewRowId
	FROM #NewGroupSavecartLineitemDetails N
	LEFT JOIN #OldGroupSavecartLineitemDetails O ON N.OmsSavedCartId=O.OmsSavedCartId AND N.SKU=O.SKU
	WHERE O.SKU IS NULL

	--Inserting the new save cart data if old and new cart data not match
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldGroupSavecartLineitemDetails)
		OR EXISTS (SELECT TOP 1 1 FROM #NewRowId) --Added condition to add new product which is coming with existing product.
	BEGIN
		SELECT RowId, Id ,Row_number()Over(ORDER BY RowId, Id,GenId) NewRowId , ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
			,CustomText,CartAddOnDetails,ROW_NUMBER()Over(ORDER BY NewId() ) Sequence ,AutoAddon  
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU ,CustomUnitPrice 
		INTO #InsertNewGroupSavecartLineitem   
		FROM #NewGroupSavecartLineitemDetails
		WHERE RowId IN (SELECT RowId FROM #NewRowId) OR ParentSKU IS NULL
		GROUP BY ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		   ,CustomText,CartAddOnDetails ,AutoAddon  
		   ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU, CustomUnitPrice
		ORDER BY RowId, Id

		--Removing the line item having Quantity <=0 
		DELETE 
		FROM #InsertNewGroupSavecartLineitem
		WHERE Quantity <=0
  
		--Updating the rowid into new save cart line item as new line item is merged into existing save cart item
		;WITH VTTY AS
		(
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU
			FROM #InsertNewGroupSavecartLineitem m
			INNER JOIN  #InsertNewGroupSavecartLineitem TY1 ON TY1.SKU = m.ParentSKU
			WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon
		)
		UPDATE m1   
		SET m1.RowId = TYU.RowId
		FROM #InsertNewGroupSavecartLineitem m1   
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  

		;WITH VTRET AS   
		(
			SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU, OrderLineItemRelationshipTypeId 
			FROM #InsertNewGroupSavecartLineitem   
			GROUP BY RowId,id ,SKU ,ParentSKU  ,OrderLineItemRelationshipTypeId
			HAVING SKU = ParentSKU AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		)
		DELETE 
		FROM #InsertNewGroupSavecartLineitem 
		WHERE NewRowId  IN (SELECT NewRowId FROM VTRET)   
     
		--Inserting the new cart line item if not merged in existing save cart line item
		INSERT INTO  ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
			,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description,CustomUnitPrice)  
		OUTPUT INSERTED.SKU,INSERTED.OmsSavedCartLineItemId  INTO @OmsInsertedData 
		SELECT NULL ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
			,CustomText,CartAddOnDetails,ROW_NUMBER()Over(ORDER BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description, CustomUnitPrice
		FROM  #InsertNewGroupSavecartLineitem  TH  

		SELECT  MAX(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
			,b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU 
		INTO  #Cte_newData
		FROM ZnodeOmsSavedCartLineItem a  with (nolock)
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0  
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
				AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID	  

		UPDATE a 
		SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  #Cte_newData  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  ORDER BY b.RowId )   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
			AND a.ParentOmsSavedCartLineItemId IS nULL   
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
  
		-----------------------------------------------------------------------------------------------------------------------------------

		SELECT  MIN(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId
			,b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU
		INTO #Cte_newData1
		FROM ZnodeOmsSavedCartLineItem a with (nolock)
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0  
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
				AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID	  

		UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  #Cte_newData1  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  ORDER BY b.RowId )   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon   
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
			AND  a.sequence in (SELECT  MIN(ab.sequence) FROM ZnodeOmsSavedCartLineItem ab WITH (NOLOCK) where a.OmsSavedCartId = ab.OmsSavedCartId and 
				a.SKU = ab.sku and ab.OrderLineItemRelationshipTypeId is not null  ) 

		----------------------------------------------------------------------------------------------------------------------------

		SELECT DISTINCT a.OmsSavedCartLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(ORDER BY c.OmsSavedCartLineItemId )RowIdNo
		INTO #Cte_newAddon
		FROM ZnodeOmsSavedCartLineItem a with (nolock) 
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ( b.Id = 1  ))  
		INNER JOIN ZnodeOmsSavedCartLineItem c with (nolock) on b.sku = c.sku and b.OmsSavedCartId=c.OmsSavedCartId and b.Id = 1
		WHERE ( ISNULL(a.ParentOmsSavedCartLineItemId,0) <> 0   )
			AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon
			AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewGroupSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId ) and c.ParentOmsSavedCartLineItemId is null

		---- Added to get distinct records.
		--SELECT *,ROW_NUMBER()OVER(ORDER BY OmsSavedCartLineItemId) RowIdNo 
		--INTO #Cte_newAddon 
		--FROM #Cte_newAddon1

		SELECT DISTINCT OmsSavedCartLineItemId,RowId,SKU,ParentSKU,MIN(RowIdNo) As RowIdNo
		INTO #Cte_newAddon1
		FROM #Cte_newAddon
		GROUP BY OmsSavedCartLineItemId,RowId,SKU,ParentSKU;

		DELETE FROM #Cte_newAddon;

		INSERT INTO #Cte_newAddon
		SELECT OmsSavedCartLineItemId,RowId,SKU,ParentSKU,ROW_NUMBER() OVER (ORDER BY RowId,RowIdNo) As RowIdNo
		FROM #Cte_newAddon1;

		;WITH table_update AS 
		(
			SELECT * , ROW_NUMBER()Over(ORDER BY OmsSavedCartLineItemId  ) RowIdNo
			FROM ZnodeOmsSavedCartLineItem a with (nolock)
			WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
				AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
				AND a.ParentOmsSavedCartLineItemId IS NULL  
				AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewGroupSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
				AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
		)
		UPDATE a 
		SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 max(OmsSavedCartLineItemId) 
		FROM #Cte_newAddon  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo  GROUP BY r.ParentSKU, r.SKU  )   
		FROM table_update a  
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
		WHERE (SELECT TOP 1 max(OmsSavedCartLineItemId)  FROM #Cte_newAddon  r  
			WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo  GROUP BY r.ParentSKU, r.SKU  )    IS nOT NULL 

		;WITH Cte_Th AS   
		(             
			SELECT RowId    
			FROM #InsertNewGroupSavecartLineitem a   
			GROUP BY RowId   
			HAVING COUNT(NewRowId) <= 1   
		 )
		UPDATE a 
		SET a.Quantity =  NULL,
			a.ModifiedDate = @GetDate   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewGroupSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =0)   
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
		 AND a.OrderLineItemRelationshipTypeId IS NULL   
  
		UPDATE  ZnodeOmsSavedCartLineItem   
		SET GROUPID = NULL   
		WHERE  EXISTS (SELECT TOP 1 1  FROM #InsertNewGroupSavecartLineitem RT WHERE RT.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
			AND OrderLineItemRelationshipTypeId IS NOT NULL     
       
	   ;WITH Cte_UpdateSequence AS   
		(  
			SELECT OmsSavedCartLineItemId ,Row_Number()Over(ORDER BY OmsSavedCartLineItemId) RowId , Sequence   
			FROM ZnodeOmsSavedCartLineItem with (nolock)  
			WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewGroupSavecartLineitem TH WHERE TH.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
		)   
		UPDATE Cte_UpdateSequence  
		SET Sequence = RowId  
			
		UPDATE @TBL_Personalise
		SET OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN @TBL_Personalise c ON a.SKU = c.GroupProductIds
		INNER JOIN ZnodeOmsSavedCartLineItem b WITH (NOLOCK) ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsSavedCartLineItemId IS NOT NULL  

		DELETE FROM ZnodeOmsPersonalizeCartItem	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId )
			
		----Inserting saved cart item personalise value FROM given line item
		MERGE INTO ZnodeOmsPersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
			   ON (TARGET.OmsSavedCartLineItemId = SOURCE.OmsSavedCartLineItemId ) 
			   WHEN NOT MATCHED THEN 
				INSERT  ( OmsSavedCartLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
								,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL )
				VALUES (  SOURCE.OmsSavedCartLineItemId,  @userId, @getdate, @userId, @getdate
								,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL ) ;
	END 

	END TRY
	BEGIN CATCH 
		SELECT ERROR_MESSAGE()
	END CATCH 
END