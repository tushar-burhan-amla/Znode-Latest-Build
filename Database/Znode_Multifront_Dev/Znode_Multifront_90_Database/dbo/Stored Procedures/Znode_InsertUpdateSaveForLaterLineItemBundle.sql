CREATE PROCEDURE [dbo].[Znode_InsertUpdateSaveForLaterLineItemBundle]
 (
	 @SaveForLaterLineItemType SaveForLaterLineitems READONLY  
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
   
	DECLARE @OmsInsertedData TABLE (OmsTemplateLineItemId INT ) 	
	----To update saved cart item personalise value FROM given line item
	DECLARE @TBL_Personalise TABLE (OmsTemplateLineItemId INT, ParentOmsTemplateLineItemId INT,BundleProductIds VARCHAR(600) ,PersonalizeCode NVARCHAR(MAX),PersonalizeValue NVARCHAR(MAX),DesignId NVARCHAR(MAX), ThumbnailURL NVARCHAR(MAX),PersonalizeName NVARCHAR(max))
	INSERT INTO @TBL_Personalise
	SELECT DISTINCT Null, a.ParentOmsTemplateLineItemId,a.BundleProductIds
			,Tbl.Col.value( 'PersonalizeCode[1]', 'NVARCHAR(MAX)' ) AS PersonalizeCode
			,Tbl.Col.value( 'PersonalizeValue[1]', 'NVARCHAR(MAX)' ) AS PersonalizeValue
			,Tbl.Col.value( 'DesignId[1]', 'NVARCHAR(MAX)' ) AS DesignId
			,Tbl.Col.value( 'ThumbnailURL[1]', 'NVARCHAR(MAX)' ) AS ThumbnailURL
			,Tbl.Col.value( 'PersonalizeName[1]', 'NVARCHAR(MAX)' ) AS PersonalizeName
	FROM @SaveForLaterLineItemType a 
	CROSS APPLY a.PersonalisedAttribute.nodes( '//PersonaliseValueModel' ) AS Tbl(Col) 

	CREATE TABLE #NewBundleSaveForLaterLineitemDetails 
	(
		GenId INT IDENTITY(1,1),RowId	INT	,OmsTemplateLineItemId	INT	 ,ParentOmsTemplateLineItemId	INT,OmsTemplateId	INT
		,SKU	NVARCHAR(MAX) ,Quantity	NUMERIC(28,6)	,OrderLineItemRelationshipTypeID	INT	,CustomText	NVARCHAR(MAX)
		,CartAddOnDetails	NVARCHAR(MAX),Sequence	INT	,AutoAddon	VARCHAR(MAX)	,OmsOrderId	INT	,ItemDetails	NVARCHAR(MAX)
		,Custom1	NVARCHAR(MAX)  ,Custom2	NVARCHAR(MAX),Custom3	NVARCHAR(MAX),Custom4	NVARCHAR(MAX),Custom5	NVARCHAR(MAX)
		,GroupId	NVARCHAR(MAX) ,ProductName	NVARCHAR(MAX),Description	NVARCHAR(MAX),Id	INT,ParentSKU NVARCHAR(MAX),
		CustomUnitPrice NUMERIC(28,6)
	)
	
	--Getting new save cart data(bundle product)
	INSERT INTO #NewBundleSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,  GroupId ,ProductName,min(Description)Description	,0 Id,NULL ParentSKU ,
		CustomUnitPrice
	FROM @SaveForLaterLineItemType a 
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, SKU
		,Quantity, OrderLineItemRelationshipTypeID, CustomText, CartAddOnDetails, Sequence
		,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,CustomUnitPrice
	 
	--Getting new bundle product save cart data
	INSERT INTO #NewBundleSaveForLaterLineitemDetails 
	SELECT   Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, BundleProductIds
				,Quantity, @OrderLineItemRelationshipTypeIdBundle, CustomText, CartAddOnDetails, Sequence
				,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id,SKU ParentSKU,
				CustomUnitPrice
	FROM @SaveForLaterLineItemType  a 
	WHERE BundleProductIds <> ''
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, BundleProductIds
	,Quantity,  CustomText, CartAddOnDetails, Sequence ,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice
		
	--Getting new Bundle products save cart data if addon is present for any line item
	INSERT INTO #NewBundleSaveForLaterLineitemDetails
	SELECT  Min(RowId )RowId ,OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
	,AddOnQuantity, @OrderLineItemRelationshipTypeIdAddon, CustomText, CartAddOnDetails, Sequence
	,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,min(Description)Description	,1 Id 
	,CASE WHEN ConfigurableProductIds <> ''  THEN ConfigurableProductIds
			WHEN  GroupProductIds <> '' THEN GroupProductIds 
			WHEN BundleProductIds <> '' THEN BundleProductIds 
			ELSE SKU END     ParentSKU , CustomUnitPrice
	FROM @SaveForLaterLineItemType  a 
	WHERE AddOnValueIds <> ''
	GROUP BY  OmsTemplateLineItemId, ParentOmsTemplateLineItemId, OmsTemplateId, AddOnValueIds
	,AddOnQuantity,  CustomText, CartAddOnDetails, Sequence ,ConfigurableProductIds,GroupProductIds,	BundleProductIds
	,AutoAddon, OmsOrderId, ItemDetails,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId,ProductName,SKU,CustomUnitPrice

    CREATE TABLE #OldBundleSaveForLaterLineitemDetails (OmsTemplateId INT ,OmsTemplateLineItemId INT,ParentOmsTemplateLineItemId INT , SKU  NVARCHAr(2000),OrderLineItemRelationshipTypeID INT  )
	--Getting the old bundle save cart data if present for same SKU in the new save cart data for bundle product		 
	INSERT INTO #OldBundleSaveForLaterLineitemDetails  
	SELECT  a.OmsTemplateId,a.OmsTemplateLineItemId,a.ParentOmsTemplateLineItemId , a.SKU  ,a.OrderLineItemRelationshipTypeID 
	FROM ZnodeOmsTemplateLineItem a   
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType  TY WHERE TY.OmsTemplateId = a.OmsTemplateId AND ISNULL(a.SKU,'') = ISNULL(TY.BundleProductIds,'')   )   
    AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle   

	--Getting the old save cart Parent data
	INSERT INTO #OldBundleSaveForLaterLineitemDetails 
	SELECT DISTINCT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsTemplateLineItem b 
	INNER JOIN #OldBundleSaveForLaterLineitemDetails c ON (c.ParentOmsTemplateLineItemId  = b.OmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
	WHERE EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType  TY WHERE TY.OmsTemplateId = b.OmsTemplateId AND ISNULL(b.SKU,'') = ISNULL(TY.SKU,'') AND ISNULL(b.Groupid,'-') = ISNULL(TY.Groupid,'-')  )
	AND  b.OrderLineItemRelationshipTypeID IS NULL 

	------Merge Addon for same product
	SELECT * INTO #OldValueForAddon FROM #OldBundleSaveForLaterLineitemDetails

	DELETE a FROM #OldBundleSaveForLaterLineitemDetails a WHERE NOT EXISTS (SELECT TOP 1 1  FROM #OldBundleSaveForLaterLineitemDetails b WHERE b.ParentOmsTemplateLineItemId IS NULL AND b.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)
	AND a.ParentOmsTemplateLineItemId IS NOT NULL 

	--Getting the old bundle product save cart addon data for old line items if present
	INSERT INTO #OldBundleSaveForLaterLineitemDetails 
	SELECT b.OmsTemplateId,b.OmsTemplateLineItemId,b.ParentOmsTemplateLineItemId , b.SKU  ,b.OrderLineItemRelationshipTypeID  
	FROM ZnodeOmsTemplateLineItem b 
	INNER JOIN #OldBundleSaveForLaterLineitemDetails c ON (c.OmsTemplateLineItemId  = b.ParentOmsTemplateLineItemId AND c.OmsTemplateId = b.OmsTemplateId)
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
											WHERE a.ParentOmsTemplateLineItemId=b.ParentOmsTemplateLineItemId and OrderLineItemRelationshipTypeID = 1 ) x 
											FOR XML PATH('')
									), 1, 2, ''
									) AddOns
		INTO #AddOnsExists
		FROM #OldValueForAddon a WHERE a.ParentOmsTemplateLineItemId is not null and OrderLineItemRelationshipTypeID<>1

		SELECT distinct a.BundleProductIds SKU, STUFF(
										( SELECT  ', ' + x.AddOnValueIds FROM    
										( SELECT DISTINCT b.AddOnValueIds FROM @SaveForLaterLineItemType b
											WHERE a.SKU=b.SKU ) x
											FOR XML PATH('')
										), 1, 2, ''
									) AddOns
		INTO #AddOnAdded
		FROM @SaveForLaterLineItemType a

		IF NOT EXISTS(SELECT * FROM #AddOnsExists a INNER JOIN #AddOnAdded b on a.SKU = b.SKU and a.AddOns = b.AddOns )
		BEGIN
			DELETE FROM #OldBundleSaveForLaterLineitemDetails
		END

		END

	--If addon present in new and old save cart data and not matches the addon data (old and new for merge) then removing the old save cart data FROM #OldSaveForLaterLineitemDetails
	IF NOT EXISTS (SELECT TOP 1 1  FROM @SaveForLaterLineItemType ty WHERE EXISTS (SELECT TOP 1 1 FROM 	#OldBundleSaveForLaterLineitemDetails a WHERE	
	ISNULL(TY.AddOnValueIds,'') = a.SKU AND  a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ))
	AND EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
	BEGIN 
		
		DELETE FROM #OldBundleSaveForLaterLineitemDetails 
	END 
	ELSE 
	BEGIN 
		IF EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'')  <> '' )
		BEGIN 

			 DECLARE @parenTofAddon INT  = 0 
			 SET @parenTofAddon = (SELECT TOP 1 ParentOmsTemplateLineItemId 
			 FROM #OldBundleSaveForLaterLineitemDetails a
			 WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 
			 AND (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsTemplateLineItem  t WHERE t.ParentOmsTemplateLineItemId = a.ParentOmsTemplateLineItemId AND   t.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) = (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			  )

			 DELETE FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId <> @parenTofAddon  AND ParentOmsTemplateLineItemId IS NOT NULL  

			 DELETE FROM #OldBundleSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT ISNULL(m.ParentOmsTemplateLineItemId,0) FROM #OldBundleSaveForLaterLineitemDetails m)
			 AND ParentOmsTemplateLineItemId IS  NULL  
		 
			 IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldBundleSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			  BEGIN 
				DELETE FROM #OldBundleSaveForLaterLineitemDetails
			  END 
			IF (SELECT COUNT (DISTINCT SKU ) FROM  ZnodeOmsTemplateLineItem   WHERE ParentOmsTemplateLineItemId =@parenTofAddon AND   OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
			BEGIN 
				DELETE FROM #OldBundleSaveForLaterLineitemDetails
			END 

		 END 
		 ELSE IF (SELECT COUNT (OmsTemplateLineItemId) FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS NULL ) >= 1 
		 BEGIN 
		    DECLARE @TBL_deleteParentOmsTemplateLineItemId TABLE (OmsTemplateLineItemId INT )
			INSERT INTO @TBL_deleteParentOmsTemplateLineItemId 
			SELECT ParentOmsTemplateLineItemId
			FROM ZnodeOmsTemplateLineItem a 
			WHERE ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS NULL )
			AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon 

			DELETE FROM #OldBundleSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
			OR ParentOmsTemplateLineItemId IN (SELECT OmsTemplateLineItemId FROM @TBL_deleteParentOmsTemplateLineItemId)
		 END 
		 ELSE IF (SELECT COUNT (DISTINCT SKU ) FROM  #OldBundleSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon ) <> (SELECT COUNT (DISTINCT SKU ) FROM  #NewBundleSaveForLaterLineitemDetails  WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  )
		  BEGIN 
		    DELETE FROM #OldBundleSaveForLaterLineitemDetails
		  END 
		   ELSE IF  EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplateLineItem Wt WHERE EXISTS (SELECT TOP 1 1 FROM #OldBundleSaveForLaterLineitemDetails ty WHERE ty.OmsTemplateId = wt.OmsTemplateId AND ty.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle AND wt.ParentOmsTemplateLineItemId= ty.OmsTemplateLineItemId  ) AND wt.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon)
		      AND EXISTS (SELECT TOP 1 1 FROM @SaveForLaterLineItemType WHERE ISNULL(AddOnValueIds,'')  = '' )
		 BEGIN 
		   DELETE FROM #OldBundleSaveForLaterLineitemDetails
		 END 

	END 
	
	--Getting the personalise data for old save cart if present
	DECLARE @TBL_Personaloldvalues TABLE (OmsTemplateLineItemId INT , PersonalizeCode NVARCHAr(max), PersonalizeValue NVARCHAr(max))
	INSERT INTO @TBL_Personaloldvalues
	SELECT OmsTemplateLineItemId , PersonalizeCode, PersonalizeValue
	FROM ZnodeOmsTemplatePersonalizeCartItem  a 
	WHERE EXISTS (SELECT TOP 1 1 FROM #OldBundleSaveForLaterLineitemDetails TY WHERE TY.OmsTemplateLineItemId = a.OmsTemplateLineItemId)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise TU WHERE TU.PersonalizeCode = a.PersonalizeCode AND TU.PersonalizeValue = a.PersonalizeValue)
		
	IF  NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise )
	BEGIN 
		DELETE FROM #OldBundleSaveForLaterLineitemDetails
	END 
	ELSE 
	BEGIN 
		 IF EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		 AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		 AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) <>
		     (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM ZnodeOmsTemplateLineItem WHERE ParentOmsTemplateLineItemId IS nULL and OmsTemplateLineItemId in (select OmsTemplateLineItemId FROM #OldBundleSaveForLaterLineitemDetails)  )
		 BEGIN 
		   
			   DELETE FROM #OldBundleSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId IN (
			   SELECT OmsTemplateLineItemId FROM #OldBundleSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
			   AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues ) ) 
			   OR OmsTemplateLineItemId IN ( SELECT ParentOmsTemplateLineItemId FROM #OldBundleSaveForLaterLineitemDetails WHERE OmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues )
			   AND ParentOmsTemplateLineItemId NOT IN (SELECT OmsTemplateLineItemId FROM @TBL_Personaloldvalues ))
		 
		 END 
		 ELSE IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_Personaloldvalues)
		 AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) > 1 
		 AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) <>
		     (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM ZnodeOmsTemplateLineItem WHERE ParentOmsTemplateLineItemId IS nULL and OmsTemplateLineItemId in (select OmsTemplateLineItemId FROM #OldBundleSaveForLaterLineitemDetails)  )
		 BEGIN 
		   
			   DELETE n FROM #OldBundleSaveForLaterLineitemDetails n WHERE OmsTemplateLineItemId  IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem WHERE n.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId  )
			   OR ParentOmsTemplateLineItemId  IN (SELECT OmsTemplateLineItemId FROM ZnodeOmsTemplatePersonalizeCartItem   )
		
		 END 
		 ELSE IF NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		        AND EXISTS (SELECT TOP 1 1 FROM ZnodeOmsTemplatePersonalizeCartItem m WHERE EXISTS (SELECT Top 1 1 FROM #OldBundleSaveForLaterLineitemDetails YU WHERE YU.OmsTemplateLineItemId = m.OmsTemplateLineItemId )) 
		       AND (SELECT COUNT (DISTINCT OmsTemplateLineItemId ) FROM #OldBundleSaveForLaterLineitemDetails WHERE ParentOmsTemplateLineItemId IS nULL ) = 1
		 BEGIN 
		     DELETE FROM #OldBundleSaveForLaterLineitemDetails WHERE NOT EXISTS (SELECT TOP 1 1  FROM @TBL_Personalise)
		 END 
		END 

	IF EXISTS (SELECT TOP 1 1 FROM #OldBundleSaveForLaterLineitemDetails )
	BEGIN 
		----DELETE old value FROM table which having personalise data in ZnodeOmsTemplatePersonalizeCartItem but same SKU not having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*
			FROM @SaveForLaterLineItemType a 
			INNER JOIN #OldBundleSaveForLaterLineitemDetails b on ( a.BundleProductIds = b.SKU or a.SKU = b.sku)
			WHERE isnull(cast(a.PersonalisedAttribute AS varchar(max)),'') = ''
		)
		,cte2 AS
		(
			select a.ParentOmsTemplateLineItemId
			FROM #OldBundleSaveForLaterLineitemDetails a
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem b on b.OmsTemplateLineItemId = a.OmsTemplateLineItemId
		)
		DELETE a FROM #OldBundleSaveForLaterLineitemDetails a
		INNER JOIN cte b on a.OmsTemplateLineItemId = b.OmsTemplateLineItemId
		INNER JOIN cte2 c on (a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or a.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId)

		----DELETE old value FROM table which having personalise data in ZnodeOmsTemplatePersonalizeCartItem but same SKU having personalise value for new cart item
		;WITH cte AS
		(
			select distinct b.*, 
				a.PersonalizeCode
				,a.PersonalizeValue
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSaveForLaterLineitemDetails b on ( a.BundleProductIds = b.SKU )
			WHERE isnull(a.PersonalizeValue,'') <> ''
		)
		,cte2 AS
		(
			select a.ParentOmsTemplateLineItemId, b.PersonalizeCode, b.PersonalizeValue
			FROM #OldBundleSaveForLaterLineitemDetails a
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem b on b.OmsTemplateLineItemId = a.OmsTemplateLineItemId
			WHERE NOT EXISTS(SELECT * FROM cte c WHERE b.OmsTemplateLineItemId = c.OmsTemplateLineItemId and b.PersonalizeCode = c.PersonalizeCode 
								and b.PersonalizeValue = c.PersonalizeValue )
		)
		DELETE a FROM #OldBundleSaveForLaterLineitemDetails a
		INNER JOIN cte b on a.OmsTemplateLineItemId = b.OmsTemplateLineItemId
		INNER JOIN cte2 c on (a.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or a.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId)

		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,b.ParentOmsTemplateLineItemId , a.BundleProductIds AS SKU
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSaveForLaterLineitemDetails b on a.BundleProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
			WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE b1
		FROM #OldBundleSaveForLaterLineitemDetails b1 
		WHERE NOT EXISTS(SELECT * FROM cte c WHERE (b1.OmsTemplateLineItemId = c.ParentOmsTemplateLineItemId or b1.ParentOmsTemplateLineItemId = c.ParentOmsTemplateLineItemId))
		AND EXISTS(SELECT * FROM cte)

		-----Delete old save cart with multiple personalize data 
		;WITH CTE_OldPersonalizeCodeCount as
		(
			SELECT b.OmsTemplateLineItemId ,b.SKU,count(distinct c.PersonalizeCode) as CntPersonalizeCode				
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSaveForLaterLineitemDetails b ON a.BundleProductIds = b.SKU
			LEFT JOIN ZnodeOmsTemplatePersonalizeCartItem c ON b.OmsTemplateLineItemId = c.OmsTemplateLineItemId 
			--and a.PersonalizeCode = c.PersonalizeCode
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0
			GROUP BY b.OmsTemplateLineItemId ,b.SKU
		)
		,CTE_NewPersonalizeCodeCount as
		(
			SELECT isnull(a.OmsTemplateLineItemId,0) as OmsTemplateLineItemId,b.SKU,count(a.PersonalizeCode) as CntPersonalizeCode
			FROM @TBL_Personalise a 
			INNER JOIN #NewBundleSaveForLaterLineitemDetails b ON a.BundleProductIds = b.SKU
			WHERE isnull(a.OmsTemplateLineItemId,0) = 0
			GROUP BY a.OmsTemplateLineItemId ,b.SKU
		)
		DELETE c
		from CTE_OldPersonalizeCodeCount a
		inner join CTE_NewPersonalizeCodeCount b on a.SKU = b.SKU and a.CntPersonalizeCode <> b.CntPersonalizeCode
		inner join #OldBundleSaveForLaterLineitemDetails c on b.SKU = c.SKU and a.OmsTemplateLineItemId = c.OmsTemplateLineItemId
	
		--Delete parent entry if child not present
		DELETE a FROM #OldBundleSaveForLaterLineitemDetails a
		WHERE NOT EXISTS(SELECT * FROM #OldBundleSaveForLaterLineitemDetails b where a.OmsTemplateLineItemId = b.ParentOmsTemplateLineItemId)
		AND a.ParentOmsTemplateLineItemId IS NULL

		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is different for same line item then New lineItem will generate
		--------If lineitem present in ZnodeOmsTemplatePersonalizeCartItem and personalize value is same for same line item then Quantity will added
		;WITH cte AS
		(
			SELECT b.OmsTemplateLineItemId ,a.ParentOmsTemplateLineItemId , a.BundleProductIds AS SKU
					,a.PersonalizeCode
			  		,a.PersonalizeValue
					,a.DesignId
					,a.ThumbnailURL
			FROM @TBL_Personalise a 
			INNER JOIN #OldBundleSaveForLaterLineitemDetails b on a.BundleProductIds = b.SKU
			INNER JOIN ZnodeOmsTemplatePersonalizeCartItem c on b.OmsTemplateLineItemId = c.OmsTemplateLineItemId
			WHERE a.OmsTemplateLineItemId = 0
		)
		DELETE c1
		FROM cte a1		  
		INNER JOIN #OldBundleSaveForLaterLineitemDetails b1 on a1.SKU = b1.SKU
		INNER JOIN #OldBundleSaveForLaterLineitemDetails c1 on (b1.ParentOmsTemplateLineItemId = c1.OmsTemplateLineItemId OR b1.OmsTemplateLineItemId = c1.OmsTemplateLineItemId)
		WHERE NOT EXISTS(SELECT * FROM ZnodeOmsTemplatePersonalizeCartItem c WHERE a1.OmsTemplateLineItemId = c.OmsTemplateLineItemId and a1.PersonalizeValue = c.PersonalizeValue)

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
		FROM ZnodeOmsTemplateLineItem a
		INNER JOIN #OldBundleSaveForLaterLineitemDetails b ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId)
		INNER JOIN #NewBundleSaveForLaterLineitemDetails ty ON (ty.SKU = b.SKU)
		WHERE a.OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeIdAddon

		 UPDATE a
		 SET a.Quantity = a.Quantity+s.AddOnQuantity,
		 a.ModifiedDate = @GetDate
		 FROM ZnodeOmsTemplateLineItem a
		 INNER JOIN #OldBundleSaveForLaterLineitemDetails b ON (a.ParentOmsTemplateLineItemId = b.OmsTemplateLineItemId)
		 INNER JOIN @SaveForLaterLineItemType S on a.OmsTemplateId = s.OmsTemplateId and a.SKU = s.AddOnValueIds
		 WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdAddon

		 --UPDATE Ab SET ab.Quantity = a.Quantity   
   --      FROM ZnodeOmsTemplateLineItem a  
   --      INNER JOIN ZnodeOmsTemplateLineItem ab ON (ab.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)  
   --      INNER JOIN @SaveForLaterLineItemType b ON (a.OmsTemplateId = b.OmsTemplateId  )   
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
         FROM ZnodeOmsTemplateLineItem a  
         INNER JOIN ZnodeOmsTemplateLineItem ab ON (ab.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)  
         INNER JOIN @SaveForLaterLineItemType b ON (a.OmsTemplateId = b.OmsTemplateId  ) 
		 INNER JOIN #NewBundleSaveForLaterLineitemDetails ty ON (ty.SKU = b.SKU)  
		 WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle  
		 AND EXISTS(SELECT * FROM #OldBundleSaveForLaterLineitemDetails ov WHERE a.OmsTemplateLineItemId = ov.OmsTemplateLineItemId)  

	END 
	IF NOT EXISTS (SELECT TOP 1 1 FROM #OldBundleSaveForLaterLineitemDetails )
	BEGIN 
			
		UPDATE #NewBundleSaveForLaterLineitemDetails
		SET ParentSKU = (SELECT TOP 1 SKU FROM #NewBundleSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID IS NULL )
		WHERE OrderLineItemRelationshipTypeID  = @OrderLineItemRelationshipTypeIdAddon 
		AND EXISTS (SELECT TOP 1 1 FROM #NewBundleSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle) 
		
		--Getting the new save cart data and generating row no. for new save cart insert
		SELECT RowId, Id ,Row_number()Over(Order BY RowId, Id,GenId) NewRowId , ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		   ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewId() ) Sequence ,AutoAddon  
		   ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,min(Description)Description  ,ParentSKU ,CustomUnitPrice 
		 INTO #InsertNewBundleSaveForLaterLineitem   
		 FROM  #NewBundleSaveForLaterLineitemDetails  
		 GROUP BY ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
		   ,CustomText,CartAddOnDetails ,AutoAddon  
		   ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,RowId, Id ,GenId,ParentSKU ,CustomUnitPrice
		 ORDER BY RowId, Id   
   
		--Removing the line item having Quantity <=0 
		DELETE FROM #InsertNewBundleSaveForLaterLineitem WHERE Quantity <=0  

		;WITH Add_Dup AS
		(
			SELECT  Min(NewRowId)NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID 
			FROM  #InsertNewBundleSaveForLaterLineitem
			GROUP BY SKU ,ParentSKU  ,OrderLineItemRelationshipTypeID
			HAVING OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon	
		)
		DELETE FROM #InsertNewBundleSaveForLaterLineitem
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM Add_Dup WHERE Add_Dup.NewRowId = #InsertNewBundleSaveForLaterLineitem.NewRowId)
		AND OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon

		--Updating the rowid INTo new save cart line item AS new line item is merged INTo existing save cart item
		 ;WITH VTTY AS   
		(  
			SELECT m.RowId OldRowId , TY1.RowId , TY1.SKU   
			   FROM #InsertNewBundleSaveForLaterLineitem m  
			INNER JOIN  #InsertNewBundleSaveForLaterLineitem TY1 ON TY1.SKU = m.ParentSKU   
			WHERE m.OrderLineItemRelationshipTypeID IN ( @OrderLineItemRelationshipTypeIdAddon , @OrderLineItemRelationshipTypeIdBundle)   
		)   
		UPDATE m1   
		SET m1.RowId = TYU.RowId  
		FROM #InsertNewBundleSaveForLaterLineitem m1   
		INNER JOIN VTTY TYU ON (TYU.OldRowId = m1.RowId)  
      
		--Deleting the new save cart line item if cart line item is merged
		;WITH VTRET AS   
		(  
			SELECT RowId,id,Min(NewRowId)NewRowId ,SKU ,ParentSKU ,OrderLineItemRelationshipTypeID   
			FROM #InsertNewBundleSaveForLaterLineitem   
			GROUP BY RowId,id ,SKU ,ParentSKU  ,OrderLineItemRelationshipTypeID  
			Having  SKU = ParentSKU  AND OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		)   
		DELETE FROM #InsertNewBundleSaveForLaterLineitem WHERE NewRowId  IN (SELECT NewRowId FROM VTRET)   
     
	   --Inserting the new cart line item if not merged in existing save cart line item
       INSERT INTO  ZnodeOmsTemplateLineItem (ParentOmsTemplateLineItemId ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,Sequence,CreatedBY,CreatedDate,ModifiedBy ,ModifiedDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description,CustomUnitPrice)  
       OUTPUT INSERTED.OmsTemplateLineItemId  INTO @OmsInsertedData 
	   SELECT NULL ,OmsTemplateId,SKU,Quantity,OrderLineItemRelationshipTypeId  
       ,CustomText,CartAddOnDetails,ROW_NUMBER()Over(Order BY NewRowId)  sequence,@UserId,@GetDate,@UserId,@GetDate,AutoAddon  
       ,OmsOrderId,Custom1,Custom2,Custom3 ,Custom4 ,Custom5,GroupId,ProductName ,Description , CustomUnitPrice 
       FROM  #InsertNewBundleSaveForLaterLineitem  TH  

		SELECT  MAX(a.OmsTemplateLineItemId ) OmsTemplateLineItemId 
		, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU 
		INTO #Cte_newData 
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0  
		AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			

		UPDATE a SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM  #Cte_newData  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon  
		AND a.ParentOmsTemplateLineItemId IS nULL  
		AND  EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId ) 
  
		--------------------------------------------------------------------------------------------------------

		SELECT  MIN(a.OmsTemplateLineItemId ) OmsTemplateLineItemId 
		, b.RowId ,b.GroupId ,b.SKU ,b.ParentSKU  
		INTO #ParentOmsTemplateId
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ISNULL(b.GroupId,'-') = ISNULL(a.GroupId,'-')  )  
		WHERE ISNULL(a.ParentOmsTemplateLineItemId,0) =0  
		AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
			AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon
		 GROUP BY b.RowId ,b.GroupId ,b.SKU	,b.ParentSKU,b.OrderLineItemRelationshipTypeID			

		UPDATE a SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 OmsTemplateLineItemId FROM  #ParentOmsTemplateId  r  
		WHERE  r.RowId = b.RowId AND ISNULL(r.GroupId,'-') = ISNULL(a.GroupId,'-')  Order by b.RowId )   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =1  )   
		WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		AND b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon   
		AND  EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId ) 
		AND  a.sequence in (SELECT  MIN(ab.sequence) FROM ZnodeOmsTemplateLineItem ab WHERE a.OmsTemplateId = ab.OmsTemplateId and 
		 a.SKU = ab.sku and ab.OrderLineItemRelationshipTypeId is not null  ) 

		-----------------------------------------------------------------------------------------------------

		SELECT a.OmsTemplateLineItemId , b.RowId  ,b.SKU ,b.ParentSKU  ,Row_number()Over(Order BY c.OmsTemplateLineItemId )RowIdNo
		INTO #NewBuldleProduct
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.ParentSKU AND ( CASE WHEN EXISTS (SELECT TOP 1 1 FROM #NewBundleSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle) THEN 0 ELSE 1 END = b.id OR b.Id = 1  ))  
		INNER JOIN ZnodeOmsTemplateLineItem c on b.sku = c.sku and b.OmsTemplateId=c.OmsTemplateId and b.Id = 1
		WHERE ( CASE WHEN EXISTS (SELECT TOP 1 1 FROM #NewBundleSaveForLaterLineitemDetails WHERE OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdBundle) THEN 0 ELSE 1 END = ISNULL(a.ParentOmsTemplateLineItemId,0) OR ISNULL(a.ParentOmsTemplateLineItemId,0) <> 0   )
		AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  AND c.ParentOmsTemplateLineItemId IS NULL
			AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewBundleSaveForLaterLineitem ty WHERE ty.OmsTemplateId = a.OmsTemplateId)  
			AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId)

	   ;WITH table_update AS 
	   (
		 SELECT * , ROW_NUMBER()Over(Order BY OmsTemplateLineItemId  ) RowIdNo
		 FROM ZnodeOmsTemplateLineItem a
		 WHERE a.OrderLineItemRelationshipTypeId IS NOT NULL   
		 AND a.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon  
		 AND a.ParentOmsTemplateLineItemId IS NULL  
		 AND EXISTS (SELECT TOP 1 1  FROM  #InsertNewBundleSaveForLaterLineitem ty WHERE ty.OmsTemplateId = a.OmsTemplateId )
		 AND EXISTS (SELECT TOP 1 1  FROM @OmsInsertedData ui WHERE ui.OmsTemplateLineItemId = a.OmsTemplateLineItemId )
	   )
		UPDATE a SET a.ParentOmsTemplateLineItemId = (SELECT TOP 1 max(OmsTemplateLineItemId) 
		FROM #NewBuldleProduct  r  
		WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU  GROUP BY r.ParentSKU, r.SKU  )   
		FROM table_update a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.OrderLineItemRelationshipTypeID = @OrderLineItemRelationshipTypeIdAddon AND  b.id =1 )   
		WHERE (SELECT TOP 1 max(OmsTemplateLineItemId) 
			FROM #NewBuldleProduct  r  
			WHERE  r.ParentSKU = b.ParentSKU AND a.SKU = r.SKU AND a.RowIdNo = r.RowIdNo  GROUP BY r.ParentSKU, r.SKU  )    IS NOT NULL 
	 
  
		;WITH Cte_Th AS   
		(             
			  SELECT RowId    
			 FROM #InsertNewBundleSaveForLaterLineitem a   
			 GROUP BY RowId   
			 HAVING COUNT(NewRowId) <= 1   
		)   
		UPDATE a SET a.Quantity =  NULL,
			a.ModifiedDate = @GetDate   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN #InsertNewBundleSaveForLaterLineitem b ON (a.OmsTemplateId = b.OmsTemplateId AND a.SKU = b.SKU AND b.id =0)   
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM Cte_Th TY WHERE TY.RowId = b.RowId )  
		 AND a.OrderLineItemRelationshipTypeId IS NULL   
  
		UPDATE Ab SET ab.Quantity = a.Quantity,
			ab.ModifiedDate = @GetDate, ab.CustomUnitPrice = b.CustomUnitPrice   
		FROM ZnodeOmsTemplateLineItem a  
		INNER JOIN ZnodeOmsTemplateLineItem ab ON (ab.OmsTemplateLineItemId = a.ParentOmsTemplateLineItemId)  
		INNER JOIN @SaveForLaterLineItemType b ON (a.OmsTemplateId = b.OmsTemplateId  )   
		WHERE a.OrderLineItemRelationshipTypeId = @OrderLineItemRelationshipTypeIdBundle  

		UPDATE  ZnodeOmsTemplateLineItem   
		SET GROUPID = NULL   
		WHERE  EXISTS (SELECT TOP 1 1  FROM #InsertNewBundleSaveForLaterLineitem RT WHERE RT.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId )  
		AND OrderLineItemRelationshipTypeId IS NOT NULL     
       
	   ;WITH Cte_UpdateSequence AS   
		 (  
		   SELECT OmsTemplateLineItemId ,Row_Number()Over(Order By OmsTemplateLineItemId) RowId , Sequence   
		   FROM ZnodeOmsTemplateLineItem   
		   WHERE EXISTS (SELECT TOP 1 1 FROM #InsertNewBundleSaveForLaterLineitem TH WHERE TH.OmsTemplateId = ZnodeOmsTemplateLineItem.OmsTemplateId )  
		 )   
		UPDATE Cte_UpdateSequence  
		SET  Sequence = RowId  
			
		UPDATE @TBL_Personalise
		SET OmsTemplateLineItemId = b.OmsTemplateLineItemId
		FROM @OmsInsertedData a 
		INNER JOIN ZnodeOmsTemplateLineItem b ON (a.OmsTemplateLineItemId = b.OmsTemplateLineItemId and b.OrderLineItemRelationshipTypeID <> @OrderLineItemRelationshipTypeIdAddon)
		WHERE b.ParentOmsTemplateLineItemId IS not nULL
		and b.OmsTemplateLineItemId = (select min(OmsTemplateLineItemId) FROM @OmsInsertedData d WHERE b.OmsTemplateLineItemId = d.OmsTemplateLineItemId)
	 
		DELETE FROM ZnodeOmsTemplatePersonalizeCartItem	WHERE EXISTS (SELECT TOP 1 1 FROM @TBL_Personalise yu WHERE yu.OmsTemplateLineItemId = ZnodeOmsTemplatePersonalizeCartItem.OmsTemplateLineItemId )
			
		----Inserting saved cart item personalise value FROM given line item
		MERGE INTO ZnodeOmsTemplatePersonalizeCartItem TARGET 
		USING @TBL_Personalise SOURCE
			   ON (TARGET.OmsTemplateLineItemId = SOURCE.OmsTemplateLineItemId ) 
			   WHEN NOT MATCHED THEN 
				INSERT  ( OmsTemplateLineItemId,  CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
								,PersonalizeCode, PersonalizeValue,DesignId	,ThumbnailURL, PersonalizeName )
				VALUES (  SOURCE.OmsTemplateLineItemId,  @userId, @getdate, @userId, @getdate
								,SOURCE.PersonalizeCode, SOURCE.PersonalizeValue,SOURCE.DesignId	,SOURCE.ThumbnailURL, SOURCE.PersonalizeName) ;
  
		
		END 
END TRY
BEGIN CATCH 
  SELECT ERROR_MESSAGE()
END CATCH 
END
