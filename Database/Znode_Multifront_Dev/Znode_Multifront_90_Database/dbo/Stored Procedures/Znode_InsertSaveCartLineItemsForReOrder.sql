CREATE PROCEDURE [dbo].[Znode_InsertSaveCartLineItemsForReOrder] 
(
	@OmsOrderId INT, 
	@OmsSavedCartId INT ,
	@UserId INT ,
	@OmsOrderLineItemsId INT = 0,
	@Status BIT = 0 OUT
)
AS 
BEGIN 
BEGIN TRY 
  SET NOCOUNT ON 
  DECLARE @TBL_ZnodeOmsSavedCartLineItem TABLE (OmsSavedCartLineItemId INT , RowId INT)
  DECLARE @GetDate DATETIME = dbo.FN_getDate() , @DefaultLocaleId INT = dbo.fn_getDefaultLocaleId ()
  DECLARE @AddOnOrderLineItemRelationshipTypeId INT = (SELECT TOP 1 OrderLineItemRelationshipTypeId 
															FROM ZnodeOmsOrderLineItemRelationshipType
															WHERE Name = 'Addons')

   DECLARE @BundleOrderLineItemRelationshipTypeId INT = (SELECT TOP 1 OrderLineItemRelationshipTypeId 
															FROM ZnodeOmsOrderLineItemRelationshipType
															WHERE Name = 'Bundles')

   DECLARE @GroupOrderLineItemRelationshipTypeId INT = (SELECT TOP 1 OrderLineItemRelationshipTypeId 
															FROM ZnodeOmsOrderLineItemRelationshipType
															WHERE Name = 'Group')
    DECLARE @versionId INT = (
    SELECT TOP 1 a.VersionId FROM  ZnodePublishVersionEntity a with(nolock) 
    INNER JOIN ZnodePortalCatalog b with(nolock) ON (b.PublishCatalogId = a.ZnodeCatalogId)
    INNER JOIN ZnodeOmsOrderDetails c with(nolock) ON (c.PortalId = b.PortalId)
    INNER JOIN ZnodeOmsOrder d with(nolock) ON (d.OmsOrderId = c.OmsOrderId AND (c.OmsOrderId = @OmsOrderId OR @OmsOrderId = 0  ) ) 
	INNER JOIN ZnodeOmsOrderLineItems f with(nolock) ON (f.OmsOrderDetailsId = c.OmsOrderDetailsId AND (f.OmsOrderLineItemsId = @OmsOrderLineItemsId OR @OmsOrderLineItemsId = 0 ) )
    WHERE a.LocaleId =@DefaultLocaleId
	AND a.RevisionType = (SELECT TOP 1 PublishStateCode FROM ZnodePublishState t WHERE t.PublishStateId = d.PublishStateId ) ) 

   DECLARE @OmsOrderStateId_RETURNED INT = (SELECT TOP 1 ZOOS.OmsOrderStateId FROM ZnodeOmsOrderState ZOOS WHERE ZOOS.OrderStateName = 'RETURNED')
   DECLARE @OmsOrderStateId_CANCELED INT = (SELECT TOP 1 ZOOS.OmsOrderStateId FROM ZnodeOmsOrderState ZOOS WHERE ZOOS.OrderStateName = 'CANCELED')

   CREATE TABLE #ZnodeOmsOrderLineItems_temp (OmsOrderLineItemsId INT ,ParentOmsOrderLineItemsId INT,OmsOrderDetailsId INT , SKU  NVARCHAr(2000),
   OrderLineItemRelationshipTypeID INT ,AutoAddonSKU NVARCHAR(400),Custom1 NVARCHAR(MAX),Custom2 NVARCHAR(MAX),Custom3 NVARCHAR(MAX),Custom4 NVARCHAR(MAX)
   ,Custom5 NVARCHAR(MAX), GroupId nvarchar(MAX),ProductName NVARCHAr(2000), Description nvarchar(MAX),Quantity NUMERIC(28,6), ROWID INT, ParentRowID INT)

  IF EXISTS (SELECT TOP 1 OmsOrderDetailsId  FROM ZnodeOmsOrderDetails WHERE  OmsOrderId =  @OmsOrderId AND IsActive =1 )
  BEGIN 

	  ;WITH CTE_OrderData AS
	  (
		  SELECT MIN(ZOOLI.OmsOrderLineItemsId) AS OmsOrderLineItemsId, MIN(ZOOLI.ParentOmsOrderLineItemsId) AS ParentOmsOrderLineItemsId,ZOOLI.OmsOrderDetailsId,ZOOLI.SKU,ZOOLI.OrderLineItemRelationshipTypeID,ZOOLI.AutoAddonSKU,
			  ZOOLI.Custom1,ZOOLI.Custom2,ZOOLI.Custom3,ZOOLI.Custom4,
			  ZOOLI.Custom5,ZOOLI.GroupId,ZOOLI.ProductName,ZOOLI.Description,SUM(ZOOLI.Quantity) AS Quantity
		  FROM ZnodeOmsOrderLineItems ZOOLI WITH (NOLOCK)
		  INNER JOIN ZnodeOmsOrderDetails ZOOD WITH (NOLOCK) ON ZOOLI.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId
		  WHERE  ZOOD.OmsOrderId =  @OmsOrderId AND ZOOD.IsActive =1 AND Exists(Select TOP 1 1 From ZnodePublishProductEntity ZPP With(NOLOCK) Where ZPP.SKU= ZOOLI.Sku AND ZPP.IsActive=1 AND ZPP.VersionId = @versionId)
  

		  AND CASE WHEN ZOOD.OmsOrderStateId = @OmsOrderStateId_CANCELED THEN 0 ELSE  ZOOLI.OrderLineItemStateId END  <> CASE WHEN ZOOD.OmsOrderStateId = @OmsOrderStateId_CANCELED THEN 1 ELSE  @OmsOrderStateId_RETURNED END 
		  GROUP BY ZOOLI.OmsOrderDetailsId,ZOOLI.SKU,ZOOLI.OrderLineItemRelationshipTypeID,ZOOLI.AutoAddonSKU,
			ZOOLI.Custom1,ZOOLI.Custom2,ZOOLI.Custom3,ZOOLI.Custom4, ZOOLI.Custom5,ZOOLI.GroupId,ZOOLI.ProductName,ZOOLI.Description
	  )
	  INSERT INTO #ZnodeOmsOrderLineItems_temp
	  SELECT *
		,Row_number()Over(Order By OmsOrderLineItemsId )  RowId, NULL ParentRowId
	  FROM CTE_OrderData
	  ORDER BY OmsOrderLineItemsId 
  END

  ELSE 
  BEGIN
		;WITH CTE_OrderData AS
		  (
			  SELECT MIN(OmsOrderLineItemsId) AS OmsOrderLineItemsId, MIN(ParentOmsOrderLineItemsId) AS ParentOmsOrderLineItemsId,
				  ZOO.OmsOrderDetailsId,ZOO.SKU,ZOO.OrderLineItemRelationshipTypeID,ZOO.AutoAddonSKU,
				  ZOO.Custom1,ZOO.Custom2,ZOO.Custom3,ZOO.Custom4,
				  ZOO.Custom5,ZOO.GroupId,ZOO.ProductName,ZOO.Description,SUM(ZOO.Quantity) AS Quantity
			  FROM ZnodeOmsOrderLineItems ZOO WITH (NOLOCK)
			  WHERE ( ZOO.OmsOrderLineItemsId = @OmsOrderLineItemsId OR ZOO.ParentOmsOrderLineItemsId = @OmsOrderLineItemsId ) 
			  AND Exists(Select TOP 1 1 From ZnodePublishProductEntity ZPP With(NOLOCK) Where ZPP.SKU= ZOO.Sku AND ZPP.IsActive=1 AND ZPP.VersionId = @versionId)
			  AND NOT EXISTS(SELECT * FROM ZnodeOmsOrderState ZOOS WHERE ZOO.OrderLineItemStateId = ZOOS.OmsOrderStateId AND ZOOS.OrderStateName IN ( 'RETURNED'))
			  GROUP BY ZOO.OmsOrderDetailsId,ZOO.SKU,ZOO.OrderLineItemRelationshipTypeID,ZOO.AutoAddonSKU,
				ZOO.Custom1,ZOO.Custom2,ZOO.Custom3,ZOO.Custom4, ZOO.Custom5,ZOO.GroupId,ZOO.ProductName,ZOO.Description
		 )
		 INSERT INTO #ZnodeOmsOrderLineItems_temp
		 SELECT *,Row_number()Over(Order By OmsOrderLineItemsId )   RowId, NULL ParentRowId
		 FROM CTE_OrderData

		INSERT INTO #ZnodeOmsOrderLineItems_temp 
		SELECT MIN(ZOO.OmsOrderLineItemsId) AS OmsOrderLineItemsId,MIN(ZOO.ParentOmsOrderLineItemsId) AS ParentOmsOrderLineItemsId,
		ZOO.OmsOrderDetailsId,ZOO.SKU,ZOO.OrderLineItemRelationshipTypeID,ZOO.AutoAddonSKU,
		ZOO.Custom1,ZOO.Custom2,ZOO.Custom3,ZOO.Custom4,
		ZOO.Custom5,ZOO.GroupId,ZOO.ProductName,ZOO.Description,SUM(ZOO.Quantity) AS Quantity
		,0 RowId, NULL ParentRowId
		FROM ZnodeOmsOrderLineItems ZOO WITH (NOLOCK)
		WHERE ZOO.OmsOrderLineItemsId = (SELECT TOP 1 ParentOmsOrderLineItemsId FROM #ZnodeOmsOrderLineItems_temp TY WHERE TY.ParentOmsOrderLineItemsId IS NOT NULL 
		AND TY.OrderLineItemRelationshipTypeID <> @AddOnOrderLineItemRelationshipTypeId )
		AND ZOO.ParentOmsOrderLineItemsId  IS NULL
		AND NOT EXISTS(SELECT * FROM ZnodeOmsOrderState ZOOS WHERE OrderLineItemStateId = ZOOS.OmsOrderStateId AND ZOOS.OrderStateName IN ( 'RETURNED'))
		AND NOT EXISTS(SELECT * FROM #ZnodeOmsOrderLineItems_temp tem WHERE tem.OmsOrderLineItemsId = ZOO.OmsOrderLineItemsId )
		GROUP BY ZOO.OmsOrderDetailsId,ZOO.SKU,ZOO.OrderLineItemRelationshipTypeID,ZOO.AutoAddonSKU,
			ZOO.Custom1,ZOO.Custom2,ZOO.Custom3,ZOO.Custom4,ZOO.Custom5,ZOO.GroupId,ZOO.ProductName,ZOO.Description;

  END

     CREATE TABLE #TBL_OmsSavedCartOld (SKU NVARCHAR(2000), OmsSavedCartLineItemId INT ,ParentSKU NVARCHAR(2000) , ParentOmsSavedCartLineItemId INT ,AddOnSKU NVARCHAR(2000), OmsSavedCartLineItemIdAddOn NVARCHAR(2000) ,PersonalizeCode NVARCHAR(1200), PersonalizeValue  NVARCHAR(MAX) )

	 CREATE TABLE #TBL_OmsSavedCartNew (SKU NVARCHAR(2000), OmsSavedCartLineItemId INT ,ParentSKU NVARCHAR(2000) , ParentOmsSavedCartLineItemId INT ,AddOnSKU NVARCHAR(2000), OmsSavedCartLineItemIdAddOn NVARCHAR(2000) ,PersonalizeCode NVARCHAR(1200), PersonalizeValue  NVARCHAR(MAX) )

	 
	 SELECT SKU , OmsOrderLineItemsId, ParentOmsOrderLineItemsId, OrderLineItemRelationshipTypeID,Quantity 
	 INTO #ZnodeOmsSavedCartLineItemOld
	 FROM #ZnodeOmsOrderLineItems_temp a 

	 SELECT OmsOrderLineItemsId, PersonalizeCode, PersonalizeValue  
	 INTO #ZnodeOmsPersonalizeCartItemOld
	 FROM ZnodeOmsPersonalizeItem a WITH (NOLOCK)
	 WHERE EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsOrderLineItems_temp t WHERE t.OmsOrderLineItemsId = a.OmsOrderLineItemsId)

	 SELECT SKU , OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OrderLineItemRelationshipTypeID, Quantity
	 INTO #ZnodeOmsSavedCartLineItemNew
	 FROM ZnodeOmsSavedCartLineItem a WITH (NOLOCK)
	 WHERE OmsSavedCartId = @OmsSavedCartId 


	 SELECT OmsSavedCartLineItemId, PersonalizeCode, PersonalizeValue   
	 INTO #ZnodeOmsPersonalizeCartItemNew
	 FROM ZnodeOmsPersonalizeCartItem a WITH (NOLOCK)
	 WHERE EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsSavedCartLineItemNew t WHERE t.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)

	 INSERT INTO #TBL_OmsSavedCartOld (SKU,OmsSavedCartLineItemId,ParentSKU,ParentOmsSavedCartLineItemId)
	 SELECT SKU , OmsOrderLineItemsId,(SELECT TOP 1 SKU FROM #ZnodeOmsSavedCartLineItemOld TBL_B WHERE TBL_B.OmsOrderLineItemsId = ISNULL(TBL_A.ParentOmsOrderLineItemsId,0)  ) ParentSKU
				, ParentOmsOrderLineItemsId
	 FROM #ZnodeOmsSavedCartLineItemOld TBL_A
	 WHERE OrderLineItemRelationshipTypeId IS NOT NULL AND OrderLineItemRelationshipTypeId <> @AddOnOrderLineItemRelationshipTypeId

	 ;With Cte_UpdateOld AS 
	 (
		SELECT ParentOmsOrderLineItemsId , SUBSTRING((SELECT ','+SKU FROM #ZnodeOmsSavedCartLineItemOld t WHERE t.ParentOmsOrderLineItemsId = a.ParentOmsOrderLineItemsId FOR XML PATH('') ),2,4000)  SKU
		     , SUBSTRING((SELECT ','+CAST(OmsOrderLineItemsId AS NVARCHAR(max)) FROM #ZnodeOmsSavedCartLineItemOld t WHERE t.ParentOmsOrderLineItemsId = a.ParentOmsOrderLineItemsId FOR XML PATH('') ),2,4000)  OmsSavedCartLineItemId
		FROM #ZnodeOmsSavedCartLineItemOld a 
		WHERE a.OrderLineItemRelationshipTypeId = @AddOnOrderLineItemRelationshipTypeId
	 )
	 UPDATE TBL_O
	 SET TBL_O.AddOnSKU =  TBL_ON.SKU
		,TBL_O.OmsSavedCartLineItemIdAddOn =  TBL_ON.OmsSavedCartLineItemId
	 FROM #TBL_OmsSavedCartOld TBL_O 
	 INNER JOIN Cte_UpdateOld TBL_ON ON (TBL_ON.ParentOmsOrderLineItemsId  = TBL_O.OmsSavedCartLineItemId )

	  INSERT INTO #TBL_OmsSavedCartNew (SKU,OmsSavedCartLineItemId,ParentSKU,ParentOmsSavedCartLineItemId)
	  SELECT SKU , OmsSavedCartLineItemId,(SELECT TOP 1 SKU FROM #ZnodeOmsSavedCartLineItemNew TBL_B WHERE TBL_B.OmsSavedCartLineItemId = ISNULL( TBL_A.ParentOmsSavedCartLineItemId,0)   ) ParentSKU
				, ParentOmsSavedCartLineItemId
	  FROM #ZnodeOmsSavedCartLineItemNew TBL_A
	  WHERE OrderLineItemRelationshipTypeId IS NOT NULL AND OrderLineItemRelationshipTypeId <> @AddOnOrderLineItemRelationshipTypeId
	 
	 ;With Cte_UpdateNew AS 
	 (
		SELECT ParentOmsSavedCartLineItemId , SUBSTRING((SELECT ','+SKU FROM #ZnodeOmsSavedCartLineItemNew t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId FOR XML PATH('') ),2,4000)  SKU
		     , SUBSTRING((SELECT ','+CAST(OmsSavedCartLineItemId AS NVARCHAR(max)) FROM #ZnodeOmsSavedCartLineItemNew t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId FOR XML PATH('') ),2,4000)  OmsSavedCartLineItemId
		FROM #ZnodeOmsSavedCartLineItemNew a 
		WHERE a.OrderLineItemRelationshipTypeId = @AddOnOrderLineItemRelationshipTypeId
	 )	 
	 UPDATE TBL_O
	 SET TBL_O.AddOnSKU =  TBL_ON.SKU
		,TBL_O.OmsSavedCartLineItemIdAddOn =  TBL_ON.OmsSavedCartLineItemId
	 FROM #TBL_OmsSavedCartNew TBL_O 
	 INNER JOIN Cte_UpdateNew TBL_ON ON (TBL_ON.ParentOmsSavedCartLineItemId  = TBL_O.OmsSavedCartLineItemId )

	  UPDATE TBL_O
	  SET TBL_O.PersonalizeCode = SUBSTRING((SELECT ','+TBL_ON.PersonalizeCode FROM #ZnodeOmsPersonalizeCartItemNew TBL_ON WHERE TBL_ON.OmsSavedCartLineItemId  = TBL_O.ParentOmsSavedCartLineItemId   FOR XML PATH ('')),2,4000) 
		,TBL_O.PersonalizeValue =  SUBSTRING((SELECT ','+TBL_ON.PersonalizeValue FROM #ZnodeOmsPersonalizeCartItemNew TBL_ON WHERE TBL_ON.OmsSavedCartLineItemId  = TBL_O.ParentOmsSavedCartLineItemId   FOR XML PATH ('')),2,4000)  
	  FROM #TBL_OmsSavedCartNew TBL_O 
	

	  UPDATE TBL_O
	  SET TBL_O.PersonalizeCode = SUBSTRING((SELECT ','+TBL_ON.PersonalizeCode FROM #ZnodeOmsPersonalizeCartItemOld TBL_ON WHERE  TBL_ON.OmsOrderLineItemsId  = TBL_O.ParentOmsSavedCartLineItemId  FOR XML PATH ('')),2,4000)
		,TBL_O.PersonalizeValue =  SUBSTRING((SELECT ','+TBL_ON.PersonalizeValue FROM #ZnodeOmsPersonalizeCartItemOld TBL_ON WHERE  TBL_ON.OmsOrderLineItemsId  = TBL_O.ParentOmsSavedCartLineItemId  FOR XML PATH ('')),2,4000) 
	  FROM #TBL_OmsSavedCartOld TBL_O 

	  UPDATE a 
	  SET   a.PersonalizeCode = ISNULL((SELECT TOP 1 PersonalizeCode FROM #TBL_OmsSavedCartOld RT WHERE RT.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId  ),a.PersonalizeCode)
	  ,a.PersonalizeValue = ISNULL((SELECT TOP 1 PersonalizeValue FROM #TBL_OmsSavedCartOld RT WHERE RT.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId  ),a.PersonalizeValue)
	  FROM #TBL_OmsSavedCartOld a 
	  WHERE a.ParentOmsSavedCartLineItemId IS NOT NULL 

	  UPDATE a 
	  SET   a.PersonalizeCode = ISNULL((SELECT TOP 1 PersonalizeCode FROM #TBL_OmsSavedCartNew RT WHERE RT.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId  ),a.PersonalizeCode)
	  ,a.PersonalizeValue = ISNULL((SELECT TOP 1 PersonalizeValue FROM #TBL_OmsSavedCartNew RT WHERE RT.OmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId  ),a.PersonalizeValue)
	  FROM #TBL_OmsSavedCartNew a 
	  WHERE a.ParentOmsSavedCartLineItemId IS NOT NULL 

	 UPDATE a 
	 SET  a.Quantity =  a.Quantity+d.Quantity 
	 FROM ZnodeOmsSavedCartLineItem a 
	 INNER JOIN #TBL_OmsSavedCartNew b ON (b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
	 INNER JOIN #TBL_OmsSavedCartOld c ON (c.SKU = b.SKU AND c.ParentSKU = b.ParentSKU AND ISNULL(c.AddOnSKU,'-1') = ISNULL(b.AddOnSKU,'-1') AND ISNULL(c.PersonalizeCode,'-1') = ISNULL(b.PersonalizeCode,'-1') AND ISNULL(c.PersonalizeValue,'-1') = ISNULL(b.PersonalizeValue,'-1')) 
	 INNER JOIN #ZnodeOmsSavedCartLineItemOld d ON (d.OmsOrderLineItemsId = c.OmsSavedCartLineItemId)

	 ;WITH CTE_UpdateOrder 
	 AS 
	 (
	   SELECT Sequence,  ROW_NUMBER()Over(order BY OmsSavedCartLineItemId ASC) RowId
	   FROM ZnodeOmsSavedCartLineItem WITH (NOLOCK)
	   WHERE  OmsSavedCartId = @OmsSavedCartId
	 
	 ) 
	 UPDATE CTE_UpdateOrder 
	 SET Sequence = RowId

		DECLARE @DeletedId TABLE (OmsSavedCartLineItemId INT )

	
		DELETE  FROM #ZnodeOmsOrderLineItems_temp OUTPUT DELETED.OmsOrderLineItemsId INTO @DeletedId WHERE OmsOrderLineItemsId IN (SELECT c.OmsSavedCartLineItemId FROM ZnodeOmsSavedCartLineItem a 
		INNER JOIN #TBL_OmsSavedCartNew b ON (b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
		INNER JOIN #TBL_OmsSavedCartOld c ON (c.SKU = b.SKU AND c.ParentSKU = b.ParentSKU AND ISNULL(c.AddOnSKU,'-1') = ISNULL(b.AddOnSKU,'-1') AND ISNULL(c.PersonalizeCode,'-1') = ISNULL(b.PersonalizeCode,'-1') AND ISNULL(c.PersonalizeValue,'-1') = ISNULL(b.PersonalizeValue,'-1')) 
		INNER JOIN #ZnodeOmsSavedCartLineItemOld d ON (d.OmsOrderLineItemsId = c.OmsSavedCartLineItemId))


		DELETE FROM #ZnodeOmsOrderLineItems_temp WHERE ParentOmsOrderLineItemsId IN (SELECT OmsSavedCartLineItemId FROM @DeletedId)

		DELETE TR FROM #ZnodeOmsOrderLineItems_temp TR 
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsOrderLineItems_temp YU WHERE TR.OmsOrderLineItemsId = YU.ParentOmsOrderLineItemsId  ) 
		AND TR.ParentOmsOrderLineItemsId IS NULL 

		UPDATE B
		SET B.ParentRowID = A.ROWID
		FROM #ZnodeOmsOrderLineItems_temp A
		INNER JOIN #ZnodeOmsOrderLineItems_temp B ON A.OmsOrderLineItemsId = B.ParentOmsOrderLineItemsId
		WHERE A.ParentOmsOrderLineItemsId IS NULL

		UPDATE B
		SET B.ParentRowID = A.ParentRowID
		FROM #ZnodeOmsOrderLineItems_temp A
		INNER JOIN #ZnodeOmsOrderLineItems_temp B ON (A.OrderLineItemRelationshipTypeID=B.OrderLineItemRelationshipTypeID)
		WHERE A.ParentOmsOrderLineItemsId IS NOT NULL AND B.ParentRowID IS NULL

		UPDATE B
		SET B.ParentRowID = A.ROWID
		FROM #ZnodeOmsOrderLineItems_temp A
		INNER JOIN #ZnodeOmsOrderLineItems_temp B ON (A.OmsOrderLineItemsId=B.ParentOmsOrderLineItemsId AND B.OrderLineItemRelationshipTypeID IS NOT NULL)
		WHERE A.ParentOmsOrderLineItemsId IS NOT NULL AND B.ParentRowID IS NULL AND B.OrderLineItemRelationshipTypeID=@AddOnOrderLineItemRelationshipTypeId

	BEGIN TRANSACTION ReorderSaveCart
		INSERT INTO ZnodeOmsSavedCartLineItem (ParentOmsSavedCartLineItemId,OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId,CustomText,CartAddOnDetails
											,Sequence,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,AutoAddon,OmsOrderId,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId
											,ProductName,Description)
		OUTPUT INSERTED.OmsSavedCartLineItemId,INSERTED.Sequence INTO @TBL_ZnodeOmsSavedCartLineItem 
		SELECT NULL,@OmsSavedCartId,SKU,Quantity,OrderLineItemRelationshipTypeId,NULL CustomText, NULL CartAddOnDetails
											,RowId,@UserId,@GetDate,@UserId,@GetDate,AutoAddonSKU,NULL OmsOrderId,Custom1,Custom2,Custom3,Custom4,Custom5,GroupId
											,ProductName,Description
		FROM #ZnodeOmsOrderLineItems_temp 
		ORDER BY OmsOrderLineItemsId 

		UPDATE ab 
		SET ab.ParentOmsSavedCartLineItemId = 
			(SELECT TOP 1 OmsSavedCartLineItemId FROM @TBL_ZnodeOmsSavedCartLineItem av WHERE av.RowId = b.ParentRowID)
		FROM ZnodeOmsSavedCartLineItem ab 
		INNER JOIN @TBL_ZnodeOmsSavedCartLineItem a ON (a.OmsSavedCartLineItemId = ab.OmsSavedCartLineItemId) 
		INNER JOIN #ZnodeOmsOrderLineItems_temp b ON  (b.RowId = a.RowId) 

		INSERT INTO ZnodeOmsPersonalizeCartItem (OmsSavedCartLineItemId,PersonalizeCode,PersonalizeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DesignId,ThumbnailURL)
		SELECT c.OmsSavedCartLineItemId,PersonalizeCode,PersonalizeValue,@UserId,@GetDate,@UserId,@GetDate,DesignId,ThumbnailURL
		FROM ZnodeOmsPersonalizeItem  a 
		INNER JOIN #ZnodeOmsOrderLineItems_temp b ON (a.OmsOrderLineItemsId = b.OmsOrderLineItemsId)
		INNER JOIN @TBL_ZnodeOmsSavedCartLineItem c ON (c.RowId = b.RowId)

		UPDATE ZOSCLI1 set Quantity = null
		from ZnodeOmsSavedCartLineItem ZOSCLI1
		where ParentOmsSavedCartLineItemId is null and OmsSavedCartId = @OmsSavedCartId
		and Quantity is not null
		and not exists(select * from ZnodeOmsSavedCartLineItem ZOSCLI where OrderLineItemRelationshipTypeId IN (@BundleOrderLineItemRelationshipTypeId,@GroupOrderLineItemRelationshipTypeId ) and ZOSCLI1.OmsSavedCartLineItemId = ZOSCLI.ParentOmsSavedCartLineItemId )

		--Update Qty for Parentlineitem except simple product
		UPDATE ZOSCLI
		SET Quantity = ZOSCLI.Quantity+Old.Quantity
		FROM ZnodeOmsSavedCartLineItem ZOSCLI
		INNER JOIN #ZnodeOmsSavedCartLineItemNew New ON (ZOSCLI.OmsSavedCartLineItemId = New.OmsSavedCartLineItemId)
		INNER JOIN #ZnodeOmsSavedCartLineItemOld Old ON (New.SKU = Old.SKU)
		WHERE ZOSCLI.ParentOmsSavedCartLineItemId IS NULL AND OmsSavedCartId = @OmsSavedCartId AND ZOSCLI.Quantity IS NOT NULL
	    --AND EXISTS(SELECT * FROM ZnodeOmsSavedCartLineItem ZOSCLI WHERE OrderLineItemRelationshipTypeId=@BundleOrderLineItemRelationshipTypeId)

		SET @GetDate = dbo.Fn_GetDate();

		UPDATE ZnodeOmsSavedCart
		SET ModifiedDate = @GetDate
		WHERE OmsSavedCartId = @OmsSavedCartId
	COMMIT TRANSACTION ReorderSaveCart
	SET @status = 1 
   
	END TRY 
	BEGIN CATCH 
		ROLLBACK TRANSACTION ReorderSaveCart
		SELECT ERROR_MESSAGE()
		SET @status = 0 

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertSaveCartLineItemsForReOrder @OmsOrderId = '+CAST(@OmsOrderId AS VARCHAR(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@OmsSavedCartId='+CAST(@OmsSavedCartId AS VARCHAR(50))+',@OmsOrderLineItemsId='+CAST(@OmsOrderLineItemsId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_InsertSaveCartLineItemsForReOrder',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH 
END