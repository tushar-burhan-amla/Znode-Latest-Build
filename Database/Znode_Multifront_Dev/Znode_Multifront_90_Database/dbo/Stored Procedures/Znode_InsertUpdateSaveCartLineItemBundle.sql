CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveCartLineItemBundle]
 (
	 @SaveCartLineItemType TT_SavecartLineitems READONLY  
	,@Userid  INT = 0
	,@OrderLineItemRelationshipTypeIdBundle INT
	,@OrderLineItemRelationshipTypeIdAddon INT
 )
AS 
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
    --Declared the variables
    DECLARE @GetDate datetime= dbo.Fn_GetDate(); 
   
	DECLARE @OmsInsertedData TABLE (OmsSavedCartLineItemId INT ) 	
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise TABLE (OmsSavedCartLineItemId INT, ParentOmsSavedCartLineItemId INT,BundleProductIds VARCHAR(600) ,PersonalizeCode NVARCHAR(MAX),PersonalizeValue NVARCHAR(MAX),DesignId NVARCHAR(MAX), ThumbnailURL NVARCHAR(MAX))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsSavedCartLineItemId,a.BundleProductIds
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(MAX)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(MAX)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(MAX)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(MAX)' ) AS ThumbnailURL
	FROM @SaveCartLineItemType a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col) 

	CREATE TABLE #NewBundleSavecartLineitemDetails 
	(
		GenId INT IDENTITY(1,1),RowId	INT	,OmsSavedCartLineItemId	INT	 ,ParentOmsSavedCartLineItemId	INT,OmsSavedCartId	INT
		,SKU	NVARCHAR(MAX) ,Quantity	NUMERIC(28,6)	,OrderLineItemRelationshipTypeID	INT	,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	INT	,AutoAddon	VARCHAR(MAX)	,OmsOrderId	INT	,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX)  ,Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX),
		CustomUnitPrice NUMERIC(28,6)
	)
	
	--Getting new save cart data(bundle product)
	INSERT INTO #NewBundleSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU ,
		CustomUnitPrice
	FROM @SaveCartLineItemType a 
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,CustomUnitPrice
	 
	--Getting new bundle product save cart data
	INSERT INTO #NewBundleSavecartLineitemDetails 
	SELECT   Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, BundleProductIds
				,Quantity, @OrderLineItemRelationshipTypeIdBundle, CustomText, CartAddOnDetails, Sequence
				,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU,
				CustomUnitPrice
	FROM @SaveCartLineItemType  a 
	WHERE BundleProductIds <> ''
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, BundleProductIds
	,Quantity,  CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		
	--Getting new Bundle products save cart data if addon is present for any line item
	INSERT INTO #NewBundleSavecartLineitemDetails
	SELECT  Min(RowId )RowId ,OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
	,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
	,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
	,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
			WHEN  GroupProductIds <> '' THEN GroupProductIds 
			WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END     ParentSKU , CustomUnitPrice
	FROM @SaveCartLineItemType  a 
	WHERE AddOnValueIds <> ''
	GROUP BY  OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OmsSavedCartId, AddOnValueIds
	,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
	,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice

    CREATE TABLE #OldBundleSavecartLineitemDetails (OmsSavedCartId INT ,OmsSavedCartLineItemId INT,ParentOmsSavedCartLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
	--Getting the old bundle save cart data if present for same SKU in the new save cart data for bundle product		 
	INSERT INTO #OldBundleSavecartLineitemDetails  
	SELECT  a.OmsSavedCartId,a.OmsSavedCartLineItemId,a.ParentOmsSavedCartLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	FROM ZnodeOmsSavedCartLineItem a   
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType  TY WHERE TY.OmsSavedCartId = a.OmsSavedCartId AND ISNULL(a.SKU,'') = ISNULL(TY.BundleProductIds,'')   )   
    AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle   

	--Getting the old save cart Parent data
	INSERT INTO #OldBundleSavecartLineitemDetails 
	SELECT DISTINCT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsSavedCartLineItem b 
	INNER JOIN #OldBundleSavecartLineitemDetails c ON (c.ParentOmsSavedCartLineItemId  = b.OmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType  TY WHERE TY.OmsSavedCartId = b.OmsSavedCartId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
	AND  b.OrderLineItemRelationshipTypeID IS NULL 

	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldBundleSavecartLineitemDetails

	DELETE a FROM #OldBundleSavecartLineitemDetails a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldBundleSavecartLineitemDetails b WHERE b.ParentOmsSavedCartLineItemId IS NULL AND b.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)
	AND a.ParentOmsSavedCartLineItemId IS NOT NULL 

	--Getting the old bundle product save cart addon data for old line items if present
	INSERT INTO #OldBundleSavecartLineitemDetails 
	SELECT b.OmsSavedCartId,b.OmsSavedCartLineItemId,b.ParentOmsSavedCartLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsSavedCartLineItem b 
	INNER JOIN #OldBundleSavecartLineitemDetails c ON (c.OmsSavedCartLineItemId  = b.ParentOmsSavedCartLineItemId AND c.OmsSavedCartId = b.OmsSavedCartId)
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
											WHERE a.ParentOmsSavedCartLineItemId=b.ParentOmsSavedCartLineItemId and OrderLineItemRelationshipTypeID = 1 ) x 
											FOR XML PATH('')
									), 1, 2, ''
									) AddOns
		INTO #AddOnsExists
		FROM #OldValueForAddon a WHERE a.ParentOmsSavedCartLineItemId is not null and OrderLineItemRelationshipTypeID<>1

		SELECT distinct a.BundleProductIds SKU, STUFF(
										( SELECT  ', ' + x.AddOnValueIds FROM    
										( SELECT DISTINCT b.AddOnValueIds FROM @SaveCartLineItemType b
											WHERE a.SKU=b.SKU ) x
											FOR XML PATH('')
										), 1, 2, ''
									) AddOns
		INTO #AddOnAdded
		FROM @SaveCartLineItemType a

		if NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
		begin
			DELETE FROM #OldBundleSavecartLineitemDetails
		end

		END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSavecartLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1  FROM @SaveCartLineItemType ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldBundleSavecartLineitemDetails a WHERE	
	ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
	AND EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
	BEGIN 
		
		DELETE FROM #OldBundleSavecartLineitemDetails 
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN 

			 DECLARE @parenTofAddon INT  = 0 
			IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise)
			BEGIN
				 SET @parenTofAddon = (SELECT TOP 1 MAX(ParentOmsSavedCartLineItemId) 
				 FROM #OldBundleSavecartLineitemDetails a
				 WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
				 AND (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsSavedCartLineItem  t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId AND   t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
					)
			END
			ELSE
			BEGIN
				SET @parenTofAddon = (SELECT TOP 1 ParentOmsSavedCartLineItemId
				 FROM #OldBundleSavecartLineitemDetails a
				 WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
				 AND (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsSavedCartLineItem  t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId AND   t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
					)
			END
			 DELETE FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId <> @parenTofAddon  AND ParentOmsSavedCartLineItemId IS NOT NULL  

			 DELETE FROM #OldBundleSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT ISNULL(m.ParentOmsSavedCartLineItemId,0) FROM #OldBundleSavecartLineitemDetails m)
			 AND ParentOmsSavedCartLineItemId IS  NULL  
		 
			 IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			  BEGIN 
				DELETE FROM #OldBundleSavecartLineitemDetails
			  END 
			IF (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsSavedCartLineItem   WHERE ParentOmsSavedCartLineItemId =@parenTofAddon AND   OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			BEGIN 
				DELETE FROM #OldBundleSavecartLineitemDetails
			END 

		 END 
		 ELSE IF (SELECT COUNT (OmsSavedCartLineItemId) FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS NULL ) >= 1 
		 BEGIN 
		    DECLARE @TBL_deleteParentOmsSavedCartLineItemId TABLE (OmsSavedCartLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsSavedCartLineItemId 
			SELECT ParentOmsSavedCartLineItemId
			FROM ZnodeOmsSavedCartLineItem a 
			WHERE ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS NULL )
			AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE FROM #OldBundleSavecartLineitemDetails WHERE OmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
			OR ParentOmsSavedCartLineItemId IN (SELECT OmsSavedCartLineItemId FROM @TBL_deleteParentOmsSavedCartLineItemId)
		 END 
		 ELSE IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSavecartLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
		  BEGIN 
		    DELETE FROM #OldBundleSavecartLineitemDetails
		  END 
		   ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsSavedCartLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldBundleSavecartLineitemDetails ty WHERE ty.OmsSavedCartId = wt.OmsSavedCartId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle AND wt.ParentOmsSavedCartLineItemId= ty.OmsSavedCartLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
		      AND EXISTS (SELECT TOP 1 1 FROM @SaveCartLineItemType WHERE ISNULL(AddOnValueIds,'')  = '' )
		 BEGIN 
		   DELETE FROM #OldBundleSavecartLineitemDetails
		 END 

	END 
	
	--Getting the personalise data for old save cart if present
	DECLARE @TBL_Personaloldvalues TABLE (OmsSavedCartLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsSavedCartLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsPersonalizeCartItem  a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldBundleSavecartLineitemDetails TY WHERE TY.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
	IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN 
		DELETE FROM #OldBundleSavecartLineitemDetails
	END 
	ELSE 
	BEGIN 
		 IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) <>
		     (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM ZnodeOmsSavedCartLineItem WHERE ParentOmsSavedCartLineItemId IS nULL and OmsSavedCartLineItemId in (select OmsSavedCartLineItemId FROM #OldBundleSavecartLineitemDetails)  )
		 BEGIN 
		   
			   DELETE FROM #OldBundleSavecartLineitemDetails WHERE OmsSavedCartLineItemId IN (
			   SELECT OmsSavedCartLineItemId FROM #OldBundleSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
			   AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ) ) 
			   OR OmsSavedCartLineItemId IN ( SELECT ParentOmsSavedCartLineItemId FROM #OldBundleSavecartLineitemDetails WHERE OmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues )
			   AND ParentOmsSavedCartLineItemId NOT IN (SELECT OmsSavedCartLineItemId FROM @TBL_Personaloldvalues ))
		 
		 END 
		 ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) > 1 
		 AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) <>
		     (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM ZnodeOmsSavedCartLineItem WHERE ParentOmsSavedCartLineItemId IS nULL and OmsSavedCartLineItemId in (select OmsSavedCartLineItemId FROM #OldBundleSavecartLineitemDetails)  )
		 BEGIN 
		   
			   DELETE n FROM #OldBundleSavecartLineitemDetails n WHERE OmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem WHERE n.OmsSavedCartLineItemId = ZnodeOmsPersonalizeCartItem.OmsSavedCartLineItemId  )
			   OR ParentOmsSavedCartLineItemId  IN (SELECT OmsSavedCartLineItemId FROM ZnodeOmsPersonalizeCartItem   )
		
		 END 
		 ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		        AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsPersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldBundleSavecartLineitemDetails YU WHERE YU.OmsSavedCartLineItemId = m.OmsSavedCartLineItemId )) 
		       AND (SELECT COUNT (DISTINCT OmsSavedCartLineItemId ) FROM #OldBundleSavecartLineitemDetails WHERE ParentOmsSavedCartLineItemId IS nULL ) = 1
		 BEGIN 
		     DELETE FROM #OldBundleSavecartLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		 END 
		END 

	IF EXISTS (SELECT TOP 1 1 FROM #OldBundleSavecartLineitemDetails )
	BEGIN 
		----DELETE old value FROM table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU not having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*
			FROM @SaveCartLineItemType a 
			INNER JOIN #OldBundleSavecartLineitemDetails b on ( a.BundleProductIds = b.SKU or a.SKU = b.sku)
			WHERE isnull(cast(a.PersonalisedAttribute AS varchar(max)),'') = ''
		)
		,cte2 AS
		(
			select a.ParentOmsSavedCartLineItemId
			FROM #OldBundleSavecartLineitemDetails a
			INNER JOIN ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId
		)
		DELETE a FROM #OldBundleSavecartLineitemDetails a
		INNER JOIN cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		INNER JOIN cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)

		----DELETE old value FROM table which having personalise data in ZnodeOmsPersonalizeCartItem but same SKU having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*, 
				a.PersonalizeCode
				,a.PersonalizeValue
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSavecartLineitemDetails b on ( a.BundleProductIds = b.SKU )
			WHERE isnull(a.PersonalizeValue,'') <> ''
		)
		,cte2 AS
		(
			select a.ParentOmsSavedCartLineItemId, b.PersonalizeCode, b.PersonalizeValue
			FROM #OldBundleSavecartLineitemDetails a
			INNER JOIN ZnodeOmsPersonalizeCartItem b on b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId
			WHERE NOT EXISTS(SELECT * FROM cte c WHERE b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and b.PersonalizeCode = c.PersonalizeCode 
								and b.PersonalizeValue = c.PersonalizeValue )
		)
		DELETE a FROM #OldBundleSavecartLineitemDetails a
		INNER JOIN cte b on a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		INNER JOIN cte2 c on (a.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or a.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId)


		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,b.ParentOmsSavedCartLineItemId , a.BundleProductIds AS SKU
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSavecartLineitemDetails b on a.BundleProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE b1
		FROM #OldBundleSavecartLineitemDetails b1 
		WHERE NOT EXISTS(SELECT * FROM cte c WHERE (b1.OmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId or b1.ParentOmsSavedCartLineItemId = c.ParentOmsSavedCartLineItemId))
		AND EXISTS(SELECT * FROM cte)
		-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsSavedCartLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSavecartLineitemDetails b ON a.BundleProductIds = b.SKU
			LEFT JOIN ZnodeOmsPersonalizeCartItem c ON b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId 
			--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY b.OmsSavedCartLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsSavedCartLineItemId,0) as OmsSavedCartLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewBundleSavecartLineitemDetails b ON a.BundleProductIds = b.SKU
			WHERE isnull(a.OmsSavedCartLineItemId,0) = 0
			GROUP BY a.OmsSavedCartLineItemId ,b.SKU
		)
		DELETE c
		from CTE_OldPersonalizeCodeCount a
		inner join CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		inner join #OldBundleSavecartLineitemDetails c on b.SKU = c.SKU and a.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
	
		--Delete parent entry if child not present
		DELETE a FROM #OldBundleSavecartLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldBundleSavecartLineitemDetails b where a.OmsSavedCartLineItemId = b.ParentOmsSavedCartLineItemId)
		AND a.ParentOmsSavedCartLineItemId IS NULL

		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsPersonalizeCartItem and personalize value is same for same line item then Quantity will added
		;WITH cte AS
		(
			SELECT b.OmsSavedCartLineItemId ,a.ParentOmsSavedCartLineItemId , a.BundleProductIds AS SKU
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSavecartLineitemDetails b on a.BundleProductIds = b.SKU
			INNER JOIN ZnodeOmsPersonalizeCartItem c on b.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId
			WHERE a.OmsSavedCartLineItemId = 0
		)
		DELETE c1
		FROM cte a1		  
		INNER JOIN #OldBundleSavecartLineitemDetails b1 on a1.SKU = b1.SKU
		INNER JOIN #OldBundleSavecartLineitemDetails c1 on (b1.ParentOmsSavedCartLineItemId = c1.OmsSavedCartLineItemId OR b1.OmsSavedCartLineItemId = c1.OmsSavedCartLineItemId)
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsPersonalizeCartItem c WHERE a1.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId and a1.PersonalizeValue = c.PersonalizeValue)

		--Updating the cart if old and new cart data matches	
		UPDATE a
		SET a.Quantity = a.Quantity+ty.Quantity,
		a.Custom1 = ty.Custom1,
		a.Custom2 = ty.Custom2,
		a.Custom3 = ty.Custom3,
		a.Custom4 = ty.Custom4,
		a.Custom5 = ty.Custom5,
		a.ModifiedDate = @GetDate,
		a.CustomUnitPrice = ty.CustomUnitPrice
		FROM ZnodeOmsSavedCartLineItem a
		INNER JOIN #OldBundleSavecartLineitemDetails b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		INNER JOIN #NewBundleSavecartLineitemDetails ty ON (ty.SKU = b.SKU)
		WHERE a.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeIdAddon

		 UPDATE a
		 SET a.Quantity = a.Quantity+s.AddOnQuantity,
		 a.ModifiedDate = @GetDate
		 FROM ZnodeOmsSavedCartLineItem a
		 INNER JOIN #OldBundleSavecartLineitemDetails b ON (a.ParentOmsSavedCartLineItemId = b.OmsSavedCartLineItemId)
		 INNER JOIN @SaveCartLineItemType S on a.OmsSavedCartId = s.OmsSavedCartId and a.SKU = s.AddOnValueIds
		 WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon

		 --UPDATE Ab SET ab.Quantity = a.Quantity   
   --      FROM ZnodeOmsSavedCartLineItem a  
   --      INNER JOIN ZnodeOmsSavedCartLineItem ab ON (ab.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)  
   --      INNER JOIN @SaveCartLineItemType b ON (a.OmsSavedCartId = b.OmsSavedCartId  )   
		 --WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle  

		 UPDATE Ab 
		 SET Ab.Quantity = Ab.Quantity+ty.Quantity,
		 Ab.Custom1 = ty.Custom1,
		 Ab.Custom2 = ty.Custom2,
		 Ab.Custom3 = ty.Custom3,
		 Ab.Custom4 = ty.Custom4,
		 Ab.Custom5 = ty.Custom5,
		 ab.ModifiedDate = @GetDate, 
		 ab.CustomUnitPrice = ty.CustomUnitPrice  
         FROM ZnodeOmsSavedCartLineItem a  
         INNER JOIN ZnodeOmsSavedCartLineItem ab ON (ab.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)  
         INNER JOIN @SaveCartLineItemType b ON (a.OmsSavedCartId = b.OmsSavedCartId  ) 
		 INNER JOIN #NewBundleSavecartLineitemDetails ty ON (ty.SKU = b.SKU)  
		 WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle  
		 AND EXISTS(SELECT * FROM #OldBundleSavecartLineitemDetails ov WHERE a.OmsSavedCartLineItemId = ov.OmsSavedCartLineItemId)  

	END 
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldBundleSavecartLineitemDetails )
	BEGIN 
			
		UPDATE #NewBundleSavecartLineitemDetails
		SET ParentSKU = (SELECT TOP 1 SKU FROM #NewBundleSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID IS NULL )
		WHERE OrderLineItemRelationshipTypeID  = @OrderLineItemRelationshipTypeIdAddon 
		AND EXISTS (SELECT TOP 1 1 FROM #NewBundleSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle) 
		
		--Getting the new save cart data and generating row no. for new save cart insert
		SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		   ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon  
		   ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU ,CustomUnitPrice 
		 INTO #InsertNewBundleSavecartLineitem   
		 FROM  #NewBundleSavecartLineitemDetails  
		 GROUP BY ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		   ,CustomText,CartAddOnDetails ,AutoAddon  
		   ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU ,CustomUnitPrice
		 ORDER BY RowId, Id   
   
		--Removing the line item having Quantity <=0 
		DELETE FROM #InsertNewBundleSavecartLineitem WHERE Quantity <=0  

		;WITH Add_Dup AS
		(
			SELECT  Min(NewRowId)NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID 
			FROM  #InsertNewBundleSavecartLineitem
			GROUP BY SKU ,ParentSKU  ,OrderLineItemRelationshipTypeID
			HAVING OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon	
		)
		DELETE FROM #InsertNewBundleSavecartLineitem
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM Add_Dup WHERE Add_Dup.NewRowId = #InsertNewBundleSavecartLineitem.NewRowId)
		AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		--Updating the rowid INTo new save cart line item AS new line item is merged INTo existing save cart item
		 ;WITH VTTY AS   
		(  
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU   
			   FROM #InsertNewBundleSavecartLineitem m  
			INNER JOIN  #InsertNewBundleSavecartLineitem TY1 ON TY1.SKU = m.ParentSKU   
			WHERE m.OrderLineItemRelationshipTypeID IN ( @OrderLineItemRelationshipTypeIdAddon , @OrderLineItemRelationshipTypeIdBundle)   
		)   
		UPDATE m1   
		SET m1.RowId = TYU.RowId  
		FROM #InsertNewBundleSavecartLineitem m1   
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  
      
		--Deleting the new save cart line item if cart line item is merged
		;WITH VTRET AS   
		(  
			SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID   
			FROM #InsertNewBundleSavecartLineitem   
			GROUP BY RowId,id ,SKU ,ParentSKU  ,OrderLineItemRelationshipTypeID  
			Having  SKU = ParentSKU  AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		)   
		DELETE FROM #InsertNewBundleSavecartLineitem WHERE NewRowId  IN (SELECT NewRowId FROM VTRET)   
     
	   --Inserting the new cart line item if not merged in existing save cart line item
       INSERT INTO  ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description,CustomUnitPrice)  
       OUTPUT INSERTED.OmsSavedCartLineItemId  INTO @OmsInsertedData 
	   SELECT NULL ,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description , CustomUnitPrice 
       FROM  #InsertNewBundleSavecartLineitem  TH  

		SELECT  MAX(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
		, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU 
		INTO #Cte_newData 
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0  
		AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			

		UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  #Cte_newData  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
		AND a.ParentOmsSavedCartLineItemId IS nULL  
		AND  EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId ) 
  
		--------------------------------------------------------------------------------------------------------

		SELECT  MIN(a.OmsSavedCartLineItemId ) OmsSavedCartLineItemId 
		, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU  
		INTO #ParentOmsSavedCartId
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsSavedCartLineItemId,0) =0  
		AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			

		UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 OmsSavedCartLineItemId FROM  #ParentOmsSavedCartId  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon   
		AND  EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId ) 
		AND  a.sequence in (SELECT  MIN(ab.sequence) FROM ZnodeOmsSavedCartLineItem ab WITH (NOLOCK) WHERE a.OmsSavedCartId = ab.OmsSavedCartId and 
		 a.SKU = ab.sku and ab.OrderLineItemRelationshipTypeId is not null  ) 

		-----------------------------------------------------------------------------------------------------

		SELECT a.OmsSavedCartLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(Order BY c.OmsSavedCartLineItemId )RowIdNo
		INTO #NewBuldleProduct
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.ParentSKU AND ( CASE WHEN EXISTS (SELECT TOP 1 1 FROM #NewBundleSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle) THEN 0 ELSE 1 END = b.id OR b.Id = 1  ))  
		INNER JOIN ZnodeOmsSavedCartLineItem c on b.sku = c.sku and b.OmsSavedCartId=c.OmsSavedCartId and b.Id = 1
		WHERE ( CASE WHEN EXISTS (SELECT TOP 1 1 FROM #NewBundleSavecartLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle) THEN 0 ELSE 1 END = ISNULL(a.ParentOmsSavedCartLineItemId,0) OR ISNULL(a.ParentOmsSavedCartLineItemId,0) <> 0   )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  AND c.ParentOmsSavedCartLineItemId IS NULL
			AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewBundleSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId)  
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)

	   ;WITH table_update AS 
	   (
		 SELECT * , ROW_NUMBER()Over(Order BY OmsSavedCartLineItemId  ) RowIdNo
		 FROM ZnodeOmsSavedCartLineItem a
		 WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		 AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
		 AND a.ParentOmsSavedCartLineItemId IS NULL  
		 AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewBundleSavecartLineitem ty WHERE ty.OmsSavedCartId = a.OmsSavedCartId )
		 AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
	   )
		UPDATE a SET a.ParentOmsSavedCartLineItemId = (SELECT TOP 1 max(OmsSavedCartLineItemId) 
		FROM #NewBuldleProduct  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU  GROUP BY r.ParentSKU, r.SKU  )   
		FROM table_update a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
		WHERE (SELECT TOP 1 max(OmsSavedCartLineItemId) 
			FROM #NewBuldleProduct  r  
			WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo  GROUP BY r.ParentSKU, r.SKU  )    IS NOT NULL 
	 
  
		;WITH Cte_Th AS   
		(             
			  SELECT RowId    
			 FROM #InsertNewBundleSavecartLineitem a   
			 GROUP BY RowId   
			 HAVING COUNT(NewRowId) <= 1   
		)   
		UPDATE a SET a.Quantity =  NULL,
			a.ModifiedDate = @GetDate   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN #InsertNewBundleSavecartLineitem b ON (a.OmsSavedCartId = b.OmsSavedCartId AND a.SKU = b.SKU AND b.id =0)   
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
		 AND a.OrderLineItemRelationshipTypeId IS NULL   
  
		UPDATE Ab SET ab.Quantity = a.Quantity,
			ab.ModifiedDate = @GetDate, ab.CustomUnitPrice = b.CustomUnitPrice   
		FROM ZnodeOmsSavedCartLineItem a  
		INNER JOIN ZnodeOmsSavedCartLineItem ab ON (ab.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId)  
		INNER JOIN @SaveCartLineItemType b ON (a.OmsSavedCartId = b.OmsSavedCartId  )   
		WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle  

		UPDATE  ZnodeOmsSavedCartLineItem   
		SET GROUPID = NULL   
		WHERE  EXISTS (SELECT TOP 1 1  FROM #InsertNewBundleSavecartLineitem RT WHERE RT.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
		AND OrderLineItemRelationshipTypeId IS NOT NULL     
       
	   ;WITH Cte_UpdateSequence AS   
		 (  
		   SELECT OmsSavedCartLineItemId ,Row_Number()Over(Order By OmsSavedCartLineItemId) RowId , Sequence   
		   FROM ZnodeOmsSavedCartLineItem   
		   WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewBundleSavecartLineitem TH WHERE TH.OmsSavedCartId = ZnodeOmsSavedCartLineItem.OmsSavedCartId )  
		 )   
		UPDATE Cte_UpdateSequence  
		SET  Sequence = RowId  
			
		UPDATE @TBL_Personalise
		SET OmsSavedCartLineItemId = b.OmsSavedCartLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN ZnodeOmsSavedCartLineItem b ON (a.OmsSavedCartLineItemId = b.OmsSavedCartLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsSavedCartLineItemId IS not nULL
		and b.OmsSavedCartLineItemId = (select min(OmsSavedCartLineItemId) FROM @OmsInsertedData d WHERE b.OmsSavedCartLineItemId = d.OmsSavedCartLineItemId)
	 
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