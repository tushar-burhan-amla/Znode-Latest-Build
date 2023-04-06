CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveForLaterLineItemGroup]
(
	 @SaveForLaterLineItemType SaveForLaterLineitems READONLY  
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
	
	DECLARE @OmsInsertedData TABLE (SKU varchar(600),OmsTemplateLineItemId INT ) 	
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise TABLE (OmsTemplateLineItemId INT, ParentOmsTemplateLineItemId INT,GroupProductIds VARCHAR(600) ,PersonalizeCode NVARCHAR(MAX),PersonalizeValue NVARCHAR(MAX),DesignId NVARCHAR(MAX), ThumbnailURL NVARCHAR(MAX),PersonalizeName NVARCHAR(max))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsTemplateLineItemId,a.GroupProductIds
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(MAX)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(MAX)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(MAX)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(MAX)' ) AS ThumbnailURL
			,Tbl.Col.value( 'PersonalizeName[1]', 'NVARCHAR(MAX)' ) AS PersonalizeName
	FROM @SaveForLaterLineItemType a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col)  

	CREATE TABLE #NewGroupSaveForLaterLineitemDetails 
	(
		GenId INT IDENTITY(1,1),RowId	INT ,OmsTemplateLineItemId	INT,ParentOmsTemplateLineItemId	INT,OmsTemplateId	int
		,SKU	NVARCHAR(MAX) ,Quantity	numeric(28,6)	,OrderLineItemRelationshipTypeID	INT ,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	INT ,AutoAddon	VARCHAR(MAX)	,OmsOrderId	INT ,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX)  ,Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX),
		CustomUnitPrice NUMERIC(28,6)
	)
	 
	--Getting new save cart data(group product)
	INSERT INTO #NewGroupSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU ,CustomUnitPrice
	FROM @SaveForLaterLineItemType a 
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,CustomUnitPrice
	 
	--Getting new group product save cart data
	INSERT INTO #NewGroupSaveForLaterLineitemDetails 
	SELECT   Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, GroupProductIds
				,Quantity, @OrderLineItemRelationshipTypeIdGroup, CustomText, CartAddOnDetails, Sequence
				,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU,CustomUnitPrice  
	FROM @SaveForLaterLineItemType  a 
	WHERE GroupProductIds <> ''
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, GroupProductIds
		,Quantity,  CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		
	--Getting new group products save cart data if addon is present for any line item
	INSERT INTO #NewGroupSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
		,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
		,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
			WHEN  GroupProductIds <> '' THEN GroupProductIds 
			WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END     ParentSKU ,CustomUnitPrice
	FROM @SaveForLaterLineItemType  a 
	WHERE AddOnValueIds <> ''
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
		,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		 

	CREATE TABLE #OldGroupSaveForLaterLineitemDetails (OmsTemplateId INT ,OmsTemplateLineItemId INT,ParentOmsTemplateLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
	--Getting the old group save cart data if present for same SKU in the new save cart data for group product	 	 
	INSERT INTO #OldGroupSaveForLaterLineitemDetails  
	SELECT  a.OmsTemplateId,a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	FROM ZnodeOmsTemplateLineItem a with (nolock)  
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType  TY WHERE TY.OmsTemplateId = a.OmsTemplateId AND ISNULL(a.SKU,'') = ISNULL(TY.GroupProductIds,'')   )   
		AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup   

	--Getting the old save cart Parent data
	INSERT INTO #OldGroupSaveForLaterLineitemDetails 
	SELECT DISTINCT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsTemplateLineItem b with (nolock)
	INNER JOIN #OldGroupSaveForLaterLineitemDetails c ON (c.ParentOmsTemplateLineItemId  = b.OmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType  TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
		AND  b.OrderLineItemRelationshipTypeID IS NULL 

	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldGroupSaveForLaterLineitemDetails

	DELETE a FROM #OldGroupSaveForLaterLineitemDetails a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldGroupSaveForLaterLineitemDetails b WHERE b.ParentOmsTemplateLineItemId IS NULL AND b.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)
	AND a.ParentOmsTemplateLineItemId IS NOT NULL 

	--Getting the old config product save cart addon data for old line items if present
	INSERT INTO #OldGroupSaveForLaterLineitemDetails 
	SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsTemplateLineItem b with (nolock)
	INNER JOIN #OldGroupSaveForLaterLineitemDetails c ON (c.OmsTemplateLineItemId  = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType  TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
		AND  b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

	------Merge Addon for same product
	IF EXISTS(SELECT * FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'') <> '' )
	BEGIN
		INSERT INTO #OldValueForAddon 
		SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
		FROM ZnodeOmsTemplateLineItem b 
		INNER JOIN #OldValueForAddon c ON (c.OmsTemplateLineItemId  = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
		WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType  TY WHERE TY.OmsTemplateId = b.OmsTemplateId )--AND ISNULL(b.SKU,'') = ISNULL(TY.AddOnValueIds,'') )
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

		SELECT DISTINCT a.GroupProductIds SKU, STUFF(
									( SELECT  ', ' + x.AddOnValueIds FROM    
									( SELECT DISTINCT b.AddOnValueIds FROM @SaveForLaterLineItemType b
										where a.SKU=b.SKU ) x
										FOR XML PATH('')
									), 1, 2, ''
								) AddOns
		INTO #AddOnAdded
		FROM @SaveForLaterLineItemType a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldGroupSaveForLaterLineitemDetails
		END
	END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSaveForLaterLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1  FROM @SaveForLaterLineItemType ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldGroupSaveForLaterLineitemDetails a WHERE	
		ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
		AND EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
	BEGIN
		DELETE FROM #OldGroupSaveForLaterLineitemDetails 
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN
			DECLARE @parenTofAddon  TABLE( ParentOmsTemplateLineItemId INT  )  
			INSERT INTO  @parenTofAddon 
			SELECT  ParentOmsTemplateLineItemId FROM #OldGroupSaveForLaterLineitemDetails a
			WHERE a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
			AND (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsTemplateLineItem  t with (nolock) WHERE t.ParentOmsTemplateLineItemId = a.ParentOmsTemplateLineItemId AND   t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
		  
			DELETE 
			FROM #OldGroupSaveForLaterLineitemDetails 
			WHERE OmsTemplateLineItemId NOT IN (SELECT ParentOmsTemplateLineItemId FROM  @parenTofAddon)   
				AND ParentOmsTemplateLineItemId IS NOT NULL  
				AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon

			 DELETE 
			 FROM #OldGroupSaveForLaterLineitemDetails 
			 WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldGroupSaveForLaterLineitemDetails m)
				AND ParentOmsTemplateLineItemId IS  NULL  

			IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldGroupSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			BEGIN 
				DELETE FROM #OldGroupSaveForLaterLineitemDetails
			END 
			IF (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsTemplateLineItem with (nolock)  WHERE ParentOmsTemplateLineItemId IN (SELECT ParentOmsTemplateLineItemId FROM @parenTofAddon)AND   OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			BEGIN 
				DELETE FROM #OldGroupSaveForLaterLineitemDetails
			END
		END 
		ELSE IF (SELECT COUNT (OmsTemplateLineItemId) FROM #OldGroupSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS NULL ) > 1 
		BEGIN 
			-- SELECT 3
		    DECLARE @TBL_deleteParentOmsTemplateLineItemId TABLE (OmsTemplateLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsTemplateLineItemId 
			SELECT ParentOmsTemplateLineItemId
			FROM ZnodeOmsTemplateLineItem a with (nolock)
			WHERE ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM #OldGroupSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup  )
				AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE 
			FROM #OldGroupSaveForLaterLineitemDetails 
			WHERE OmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
				OR ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
		    
			DELETE 
			FROM #OldGroupSaveForLaterLineitemDetails 
			WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldGroupSaveForLaterLineitemDetails m)
				AND ParentOmsTemplateLineItemId IS NULL
		END
		ELSE IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldGroupSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewGroupSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
		BEGIN 
			DELETE FROM #OldGroupSaveForLaterLineitemDetails
		END 
		ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplateLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldGroupSaveForLaterLineitemDetails ty WHERE ty.OmsTemplateId = wt.OmsTemplateId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdGroup AND wt.ParentOmsTemplateLineItemId= ty.OmsTemplateLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
			AND EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'')  = '' )
		BEGIN 
			DELETE FROM #OldGroupSaveForLaterLineitemDetails
		END  
	END

	DECLARE @TBL_Personaloldvalues TABLE (OmsTemplateLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	--Getting the personalise data for old save cart if present	
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsTemplateLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsTemplatePersonalizeCartItem  a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldGroupSaveForLaterLineitemDetails TY WHERE TY.OmsTemplateLineItemId = a.OmsTemplateLineItemId)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
	IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN
		DELETE FROM #OldGroupSaveForLaterLineitemDetails
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldGroupSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		BEGIN 
			DELETE 
			FROM #OldGroupSaveForLaterLineitemDetails 
			WHERE OmsTemplateLineItemId IN (
				SELECT OmsTemplateLineItemId FROM #OldGroupSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
				AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues)) 
				OR OmsTemplateLineItemId IN ( SELECT ParentOmsTemplateLineItemId FROM #OldGroupSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
				AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues))
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
			AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldGroupSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
			AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldGroupSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) <>
				(SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM ZnodeOmsTemplateLineItem WHERE ParentOmsTemplateLineItemId IS nULL and OmsTemplateLineItemId in (select OmsTemplateLineItemId FROM #OldGroupSaveForLaterLineitemDetails)  )
		BEGIN
			DELETE n 
			FROM #OldGroupSaveForLaterLineitemDetails n 
			WHERE OmsTemplateLineItemId  IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem WHERE n.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId  )
			OR ParentOmsTemplateLineItemId  IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem   )
		END 
		ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
			AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplatePersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldGroupSaveForLaterLineitemDetails YU WHERE YU.OmsTemplateLineItemId = m.OmsTemplateLineItemId )) 
			AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldGroupSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) = 1
		BEGIN 
			DELETE FROM #OldGroupSaveForLaterLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		END 
	END
	
	IF EXISTS (SELECT TOP 1 1 FROM #OldGroupSaveForLaterLineitemDetails)
	BEGIN
		----DELETE old value FROM table which having personalise data in ZnodeOmsTemplatePersonalizeCartItem but same SKU not having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*
			FROM @SaveForLaterLineItemType a 
					INNER JOIN #OldGroupSaveForLaterLineitemDetails b on ( a.GroupProductIds = b.SKU or a.SKU = b.sku)
					where isnull(cast(a.PersonalisedAttribute AS varchar(max)),'') = ''
		)
		,cte2 AS
		(
			select a.ParentOmsTemplateLineItemId
			FROM #OldGroupSaveForLaterLineitemDetails a
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem b on b.OmsTemplateLineItemId = a.OmsTemplateLineItemId
		)
		DELETE a FROM #OldGroupSaveForLaterLineitemDetails a
		INNER JOIN cte b on a.OmsTemplateLineItemId = b.OmsTemplateLineItemId
		INNER JOIN cte2 c on (a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or a.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId)

		----DELETE old value FROM table which having personalise data in ZnodeOmsTemplatePersonalizeCartItem but same SKU having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*, 
			a.PersonalizeCode
			,a.PersonalizeValue
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSaveForLaterLineitemDetails b on ( a.GroupProductIds = b.SKU)
			where isnull(a.PersonalizeValue,'') <> ''
		)
		,cte2 AS
		(
			select a.ParentOmsTemplateLineItemId, b.PersonalizeCode, b.PersonalizeValue
			FROM #OldGroupSaveForLaterLineitemDetails a
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem b on b.OmsTemplateLineItemId = a.OmsTemplateLineItemId
			WHERE NOT EXISTS(SELECT * FROM cte c where b.OmsTemplateLineItemId = c.OmsTemplateLineItemId and b.PersonalizeCode = c.PersonalizeCode 
								and b.PersonalizeValue = c.PersonalizeValue )
		)
		DELETE a FROM #OldGroupSaveForLaterLineitemDetails a
		INNER JOIN cte b on a.OmsTemplateLineItemId = b.OmsTemplateLineItemId
		INNER JOIN cte2 c on (a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or a.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId)

		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,b.ParentOmsTemplateLineItemId , a.GroupProductIds
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSaveForLaterLineitemDetails b on a.GroupProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
			WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE b1
		FROM #OldGroupSaveForLaterLineitemDetails b1 
		WHERE NOT EXISTS(SELECT * FROM cte c where (b1.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or b1.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId))
		AND EXISTS(SELECT * FROM cte)

		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is same for same line item then Quantity will added
		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,a.ParentOmsTemplateLineItemId , a.GroupProductIds AS SKU
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSaveForLaterLineitemDetails b on a.GroupProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
			WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE c1
		FROM cte a1		  
		INNER JOIN #OldGroupSaveForLaterLineitemDetails b1 on a1.SKU = b1.SKU
		INNER JOIN #OldGroupSaveForLaterLineitemDetails c1 on (b1.ParentOmsTemplateLineItemId = c1.OmsTemplateLineItemId OR b1.OmsTemplateLineItemId = c1.OmsTemplateLineItemId)
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsTemplatePersonalizeCartItem c where a1.OmsTemplateLineItemId = c.OmsTemplateLineItemId and a1.PersonalizeValue = c.PersonalizeValue)
		-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsTemplateLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldGroupSaveForLaterLineitemDetails b ON a.GroupProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c ON b.OmsTemplateLineItemId = c.OmsTemplateLineItemId 
				--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0
			GROUP BY b.OmsTemplateLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsTemplateLineItemId,0) as OmsTemplateLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewGroupSaveForLaterLineitemDetails b ON a.GroupProductIds = b.SKU
			WHERE ISNULL(a.OmsTemplateLineItemId,0) = 0
			GROUP BY a.OmsTemplateLineItemId ,b.SKU
		)
		DELETE c
		from CTE_OldPersonalizeCodeCount a
		INNER JOIN CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		INNER JOIN #OldGroupSaveForLaterLineitemDetails c on b.SKU = c.SKU and a.OmsTemplateLineItemId = c.OmsTemplateLineItemId
		
		--Delete parent entry if child not present
		DELETE a FROM #OldGroupSaveForLaterLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldGroupSaveForLaterLineitemDetails b where a.OmsTemplateLineItemId = b.ParentOmsTemplateLineItemId)
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
		INNER JOIN #OldGroupSaveForLaterLineitemDetails b ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId)
		INNER JOIN #NewGroupSaveForLaterLineitemDetails ty ON (ty.SKU = b.SKU)
		WHERE a.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeIdAddon

		UPDATE a
		SET a.Quantity = a.Quantity+s.AddOnQuantity,
			a.ModifiedDate = @GetDate
		FROM ZnodeOmsTemplateLineItem a
		INNER JOIN #OldGroupSaveForLaterLineitemDetails b ON (a.ParentOmsTemplateLineItemId = b.OmsTemplateLineItemId)
		INNER JOIN @SaveForLaterLineItemType S on a.OmsTemplateId = s.OmsTemplateId and a.SKU = s.AddOnValueIds
		WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon
	END

	--Added condition to add new product which is coming with existing product.
	IF OBJECT_ID('tempdb..#NewRowId') IS NOT NULL
		DROP TABLE #NewRowId;

	SELECT N.RowId
	INTO #NewRowId
	FROM #NewGroupSaveForLaterLineitemDetails N
	LEFT JOIN #OldGroupSaveForLaterLineitemDetails O ON N.OmsTemplateId=O.OmsTemplateId AND N.SKU=O.SKU
	WHERE O.SKU IS NULL

	--Inserting the new save cart data if old and new cart data not match
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldGroupSaveForLaterLineitemDetails)
		OR EXISTS (SELECT TOP 1 1 FROM #NewRowId) --Added condition to add new product which is coming with existing product.
	BEGIN
		SELECT RowId, Id ,Row_number()Over(ORDER BY RowId, Id,GenId) NewRowId , ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
			,CustomText,CartAddOnDetails,ROW_NUMBER()Over(ORDER BY NewId() ) Sequence ,AutoAddon  
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU ,CustomUnitPrice 
		INTO #InsertNewGroupSaveForLaterLineitem   
		FROM #NewGroupSaveForLaterLineitemDetails
		WHERE RowId IN (SELECT RowId FROM #NewRowId) OR ParentSKU IS NULL
		GROUP BY ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		   ,CustomText,CartAddOnDetails ,AutoAddon  
		   ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU, CustomUnitPrice
		ORDER BY RowId, Id

		--Removing the line item having Quantity <=0 
		DELETE 
		FROM #InsertNewGroupSaveForLaterLineitem
		WHERE Quantity <=0
  
		--Updating the rowid into new save cart line item as new line item is merged into existing save cart item
		;WITH VTTY AS
		(
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU
			FROM #InsertNewGroupSaveForLaterLineitem m
			INNER JOIN  #InsertNewGroupSaveForLaterLineitem TY1 ON TY1.SKU = m.ParentSKU
			WHERE m.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon
		)
		UPDATE m1   
		SET m1.RowId = TYU.RowId
		FROM #InsertNewGroupSaveForLaterLineitem m1   
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  

		;WITH VTRET AS   
		(
			SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU, OrderLineItemRelationshipTypeId 
			FROM #InsertNewGroupSaveForLaterLineitem   
			GROUP BY RowId,id ,SKU ,ParentSKU  ,OrderLineItemRelationshipTypeId
			HAVING SKU = ParentSKU AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		)
		DELETE 
		FROM #InsertNewGroupSaveForLaterLineitem 
		WHERE NewRowId  IN (SELECT NewRowId FROM VTRET)   
     
		--Inserting the new cart line item if not merged in existing save cart line item
		INSERT INTO  ZnodeOmsTemplateLineItem (ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
			,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description,CustomUnitPrice)  
		OUTPUT INSERTED.SKU,INSERTED.OmsTemplateLineItemId  INTO @OmsInsertedData 
		SELECT NULL ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
			,CustomText,CartAddOnDetails,ROW_NUMBER()Over(ORDER BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
			,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description, CustomUnitPrice
		FROM  #InsertNewGroupSaveForLaterLineitem  TH  

		SELECT  MAX(a.OmsTemplateLineItemId ) OmsTemplateLineItemId 
			,b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU 
		INTO  #Cte_newData
		FROM ZnodeOmsTemplateLineItem a  with (nolock)
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0  
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
				AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID	  

		UPDATE a 
		SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM  #Cte_newData  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  ORDER BY b.RowId )   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
			AND a.ParentOmsTemplateLineItemId IS nULL   
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
  
		-----------------------------------------------------------------------------------------------------------------------------------

		SELECT  MIN(a.OmsTemplateLineItemId ) OmsTemplateLineItemId
			,b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU
		INTO #Cte_newData1
		FROM ZnodeOmsTemplateLineItem a with (nolock)
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0  
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
				AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID	  

		UPDATE a SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM  #Cte_newData1  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  ORDER BY b.RowId )   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon   
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
			AND  a.sequence in (SELECT  MIN(ab.sequence) FROM ZnodeOmsTemplateLineItem ab where a.OmsTemplateId = ab.OmsTemplateId and 
				a.SKU = ab.sku and ab.OrderLineItemRelationshipTypeId is not null  ) 

		----------------------------------------------------------------------------------------------------------------------------

		SELECT DISTINCT a.OmsTemplateLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(ORDER BY c.OmsTemplateLineItemId )RowIdNo
		INTO #Cte_newAddon
		FROM ZnodeOmsTemplateLineItem a with (nolock) 
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ( b.Id = 1  ))  
		INNER JOIN ZnodeOmsTemplateLineItem c with (nolock) on b.sku = c.sku and b.OmsTemplateId=c.OmsTemplateId and b.Id = 1
		WHERE ( ISNULL(a.ParentOmsTemplateLineItemId,0) <> 0   )
			AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon
			AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewGroupSaveForLaterLineitem ty WHERE ty.OmsTemplateId = a.OmsTemplateId )
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId ) and c.ParentOmsTemplateLineItemId is null

		---- Added to get distinct records.
		--SELECT *,ROW_NUMBER()OVER(ORDER BY OmsTemplateLineItemId) RowIdNo 
		--INTO #Cte_newAddon 
		--FROM #Cte_newAddon1

		;WITH table_update AS 
		(
			SELECT * , ROW_NUMBER()Over(ORDER BY OmsTemplateLineItemId  ) RowIdNo
			FROM ZnodeOmsTemplateLineItem a with (nolock)
			WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
				AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
				AND a.ParentOmsTemplateLineItemId IS NULL  
				AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewGroupSaveForLaterLineitem ty WHERE ty.OmsTemplateId = a.OmsTemplateId )
				AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
		)
		UPDATE a 
		SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 max(OmsTemplateLineItemId) 
		FROM #Cte_newAddon  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo  GROUP BY r.ParentSKU, r.SKU  )   
		FROM table_update a  
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
		WHERE (SELECT TOP 1 max(OmsTemplateLineItemId)  FROM #Cte_newAddon  r  
			WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo  GROUP BY r.ParentSKU, r.SKU  )    IS nOT NULL 

		;WITH Cte_Th AS   
		(             
			SELECT RowId    
			FROM #InsertNewGroupSaveForLaterLineitem a   
			GROUP BY RowId   
			HAVING COUNT(NewRowId) <= 1   
		 )
		UPDATE a 
		SET a.Quantity =  NULL,
			a.ModifiedDate = @GetDate   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewGroupSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =0)   
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
		 AND a.OrderLineItemRelationshipTypeId IS NULL   
  
		UPDATE  ZnodeOmsTemplateLineItem   
		SET GROUPID = NULL   
		WHERE  EXISTS (SELECT TOP 1 1  FROM #InsertNewGroupSaveForLaterLineitem RT WHERE RT.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId )  
			AND OrderLineItemRelationshipTypeId IS NOT NULL     
       
	   ;WITH Cte_UpdateSequence AS   
		(  
			SELECT OmsTemplateLineItemId ,Row_Number()Over(ORDER BY OmsTemplateLineItemId) RowId , Sequence   
			FROM ZnodeOmsTemplateLineItem with (nolock)  
			WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewGroupSaveForLaterLineitem TH WHERE TH.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId )  
		)   
		UPDATE Cte_UpdateSequence  
		SET Sequence = RowId  
			
		UPDATE @TBL_Personalise
		SET OmsTemplateLineItemId = b.OmsTemplateLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN @TBL_Personalise c ON a.SKU = c.GroupProductIds
		INNER JOIN ZnodeOmsTemplateLineItem b WITH (NOLOCK) ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsTemplateLineItemId IS NOT NULL  

		DELETE FROM ZnodeOmsTemplatePersonalizeCartItem	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId )
			
		----Inserting saved cart item personalise value FROM given line item
		MERGE INTO ZnodeOmsTemplatePersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
			   ON (TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId ) 
			   WHEN NOT MATCHED THEN 
				INSERT  ( OmsTemplateLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
								,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL, PersonalizeName )
				VALUES (  SOURCE.OmsTemplateLineItemId,  @userId, @getdate, @userId, @getdate
								,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL, SOURCE.PersonalizeName ) ;
	END 

	END TRY
	BEGIN CATCH 
		SELECT ERROR_MESSAGE()
	END CATCH 
END
