CREATE PROCEDURE [dbo].[Znode_MergeOmsSavedCartLineItems] 
(
	@OmsSavedCartId INT  
	,@OldOmsSavedCartId INT 
	,@UserId  INT 
	,@Status  BIT = 0  OUT
)
AS 
BEGIN 
 BEGIN TRY 
 SET NOCOUNT ON 

     DECLARE @AddOnOrderLineItemRelationshipTypeId INT = (SELECT TOP 1 OrderLineItemRelationshipTypeId 
															FROM ZnodeOmsOrderLineItemRelationshipType
															WHERE Name = 'Addons')
	 DECLARE @TBL_OmsSavedCartOld TABLE(SKU NVARCHAR(2000), OmsSavedCartLineItemId INT ,ParentSKU NVARCHAR(2000) , ParentOmsSavedCartLineItemId INT ,AddOnSKU NVARCHAR(2000), OmsSavedCartLineItemIdAddOn NVARCHAR(2000) ,PersonalizeCode NVARCHAR(1200), PersonalizeValue  NVARCHAr(max)  )

	 DECLARE @TBL_OmsSavedCartNew TABLE(SKU NVARCHAR(2000), OmsSavedCartLineItemId INT ,ParentSKU NVARCHAR(2000) , ParentOmsSavedCartLineItemId INT ,AddOnSKU NVARCHAR(2000), OmsSavedCartLineItemIdAddOn NVARCHAR(2000) ,PersonalizeCode NVARCHAR(1200), PersonalizeValue  NVARCHAr(max)  )
	 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	 ----Adding dummy CookieMappingId if not present
	IF NOT EXISTS(SELECT * FROM ZnodeOmsCookieMapping where OmsCookieMappingId = 1)
	BEGIN
		SET IDENTITY_INSERT ZnodeOmsCookieMapping ON
		INSERT INTO ZnodeOmsCookieMapping(OmsCookieMappingId,UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT 1,null,(select top 1 PortalId from ZnodePortal order by 1 ASC),2,@GetDate,2,@GetDate
		SET IDENTITY_INSERT ZnodeOmsCookieMapping OFF
	END

	----geting dummy OmsSavedCartId on basis of OmsCookieMappingId = 1 if not present then add
	Declare @OmsSavedCartIdDummy int = 0
	SET @OmsSavedCartIdDummy = (Select Top 1 OmsSavedCartId  from ZnodeOmsSavedCart With(NoLock) where OmsCookieMappingId = 1)
	IF ISNULL(@OmsSavedCartIdDummy ,0) = 0 
	BEGIN 
		INSERT INTO ZnodeOmsSavedCart(OmsCookieMappingId,SalesTax,RecurringSalesTax,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT  1,null,null,2,@GetDate,2,@GetDate
		SET @OmsSavedCartIdDummy  = @@IDENTITY
	END

	 SELECT OmsSavedCartLineItemId,  ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeId
	 INTO #ZnodeOmsSavedCartLineItemOld
	 FROM ZnodeOmsSavedCartLineItem a with(nolock)
	 WHERE OmsSavedCartId = @OldOmsSavedCartId 


	 SELECT OmsSavedCartLineItemId, PersonalizeCode, PersonalizeValue
	 INTO #ZnodeOmsPersonalizeCartItemOld
	 FROM ZnodeOmsPersonalizeCartItem a with(nolock)
	 WHERE EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsSavedCartLineItemOld t WHERE t.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)

	 SELECT OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, SKU, Quantity, OrderLineItemRelationshipTypeId
	 INTO #ZnodeOmsSavedCartLineItemNew
	 FROM ZnodeOmsSavedCartLineItem a with(nolock)
	 WHERE OmsSavedCartId = @OmsSavedCartId 


	 SELECT OmsSavedCartLineItemId, PersonalizeCode, PersonalizeValue 
	 INTO #ZnodeOmsPersonalizeCartItemNew
	 FROM ZnodeOmsPersonalizeCartItem a with(nolock)
	 WHERE EXISTS (SELECT TOP 1 1 FROM #ZnodeOmsSavedCartLineItemNew t WHERE t.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)

	 

	 INSERT INTO @TBL_OmsSavedCartOld (SKU,OmsSavedCartLineItemId,ParentSKU,ParentOmsSavedCartLineItemId)
	 SELECT SKU , OmsSavedCartLineItemId,(SELECT TOP 1 SKU FROM #ZnodeOmsSavedCartLineItemOld TBL_B WHERE TBL_B.OmsSavedCartLineItemId = ISNULL(TBL_A.ParentOmsSavedCartLineItemId,0)  ) ParentSKU
				, ParentOmsSavedCartLineItemId
	 FROM #ZnodeOmsSavedCartLineItemOld TBL_A
	 WHERE OrderLineItemRelationshipTypeId IS NOT NULL AND OrderLineItemRelationshipTypeId <> @AddOnOrderLineItemRelationshipTypeId
	 
	

	 ;With Cte_UpdateOld AS 
	 (
		SELECT ParentOmsSavedCartLineItemId , SUBSTRING((SELECT ','+SKU FROM #ZnodeOmsSavedCartLineItemOld t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId FOR XML PATH('') ),2,4000)  SKU
		     , SUBSTRING((SELECT ','+CAST(OmsSavedCartLineItemId AS NVARCHAR(max)) FROM #ZnodeOmsSavedCartLineItemOld t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId FOR XML PATH('') ),2,4000)  OmsSavedCartLineItemId
		FROM #ZnodeOmsSavedCartLineItemOld a 
		WHERE a.OrderLineItemRelationshipTypeId = @AddOnOrderLineItemRelationshipTypeId
	 )
	 UPDATE TBL_O
	 SET TBL_O.AddOnSKU =  TBL_ON.SKU
		,TBL_O.OmsSavedCartLineItemIdAddOn =  TBL_ON.OmsSavedCartLineItemId
	 FROM @TBL_OmsSavedCartOld TBL_O 
	 INNER JOIN Cte_UpdateOld TBL_ON ON (TBL_ON.ParentOmsSavedCartLineItemId  = TBL_O.OmsSavedCartLineItemId )
	   
     UPDATE TBL_O
	 SET TBL_O.PersonalizeCode = TBL_ON.PersonalizeCode
		,TBL_O.PersonalizeValue =  TBL_ON.PersonalizeValue
	 FROM @TBL_OmsSavedCartOld TBL_O 
	 INNER JOIN #ZnodeOmsPersonalizeCartItemOld TBL_ON ON (TBL_ON.OmsSavedCartLineItemId  = TBL_O.OmsSavedCartLineItemId )

	  INSERT INTO @TBL_OmsSavedCartNew (SKU,OmsSavedCartLineItemId,ParentSKU,ParentOmsSavedCartLineItemId)
	    SELECT SKU , OmsSavedCartLineItemId,(SELECT TOP 1 SKU FROM #ZnodeOmsSavedCartLineItemNew TBL_B WHERE TBL_B.OmsSavedCartLineItemId = ISNULL( TBL_A.ParentOmsSavedCartLineItemId,0)   ) ParentSKU
					, ParentOmsSavedCartLineItemId
		FROM #ZnodeOmsSavedCartLineItemNew TBL_A
		WHERE OrderLineItemRelationshipTypeId IS NOT NULL AND OrderLineItemRelationshipTypeId <> @AddOnOrderLineItemRelationshipTypeId
	 
	 ;WITH Cte_UpdateNew AS 
	 (
		SELECT ParentOmsSavedCartLineItemId , SUBSTRING((SELECT ','+SKU FROM #ZnodeOmsSavedCartLineItemNew t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId FOR XML PATH('') ),2,4000)  SKU
		     , SUBSTRING((SELECT ','+CAST(OmsSavedCartLineItemId AS NVARCHAR(max)) FROM #ZnodeOmsSavedCartLineItemNew t WHERE t.ParentOmsSavedCartLineItemId = a.ParentOmsSavedCartLineItemId FOR XML PATH('') ),2,4000)  OmsSavedCartLineItemId
		FROM #ZnodeOmsSavedCartLineItemNew a 
		WHERE a.OrderLineItemRelationshipTypeId = @AddOnOrderLineItemRelationshipTypeId
	 )
	 UPDATE TBL_O
	 SET TBL_O.AddOnSKU =  TBL_ON.SKU
		,TBL_O.OmsSavedCartLineItemIdAddOn =  TBL_ON.OmsSavedCartLineItemId
	 FROM @TBL_OmsSavedCartNew TBL_O 
	 INNER JOIN Cte_UpdateNew TBL_ON ON (TBL_ON.ParentOmsSavedCartLineItemId  = TBL_O.OmsSavedCartLineItemId )
	   
	 UPDATE TBL_O
	 SET TBL_O.PersonalizeCode = TBL_ON.PersonalizeCode
		,TBL_O.PersonalizeValue =  TBL_ON.PersonalizeValue
	 FROM @TBL_OmsSavedCartNew TBL_O 
	 INNER JOIN #ZnodeOmsPersonalizeCartItemNew TBL_ON ON (TBL_ON.OmsSavedCartLineItemId  = TBL_O.OmsSavedCartLineItemId )

	BEGIN TRAN DELETEOLDSAVECART
		IF EXISTS (SELECT * FROM ZnodeOmsSavedCartLineItem WHERE OmsSavedCartId=@OldOmsSavedCartId)
		BEGIN
			UPDATE a 
			SET  a.Quantity =  a.Quantity+d.Quantity 
			FROM ZnodeOmsSavedCartLineItem a 
			INNER JOIN @TBL_OmsSavedCartNew b ON (b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
			INNER JOIN @TBL_OmsSavedCartOld c ON (c.SKU = b.SKU AND c.ParentSKU = b.ParentSKU AND ISNULL(c.AddOnSKU,'-1') = ISNULL(b.AddOnSKU,'-1') AND ISNULL(c.PersonalizeCode,'-1') = ISNULL(b.PersonalizeCode,'-1') AND ISNULL(c.PersonalizeValue,'-1') = ISNULL(b.PersonalizeValue,'-1')) 
			INNER JOIN #ZnodeOmsSavedCartLineItemOld d ON (d.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId)
		END
		ELSE
		BEGIN
			UPDATE a 
			SET  a.Quantity =  d.Quantity 
			FROM ZnodeOmsSavedCartLineItem a 
			INNER JOIN @TBL_OmsSavedCartNew b ON (b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId)
			INNER JOIN @TBL_OmsSavedCartOld c ON (c.SKU = b.SKU AND c.ParentSKU = b.ParentSKU AND ISNULL(c.AddOnSKU,'-1') = ISNULL(b.AddOnSKU,'-1') AND ISNULL(c.PersonalizeCode,'-1') = ISNULL(b.PersonalizeCode,'-1') AND ISNULL(c.PersonalizeValue,'-1') = ISNULL(b.PersonalizeValue,'-1')) 
			INNER JOIN #ZnodeOmsSavedCartLineItemOld d ON (d.OmsSavedCartLineItemId = c.OmsSavedCartLineItemId)
		END
	 
		 UPDATE  a
		 SET a.OmsSavedCartId = @OmsSavedCartId
		 FROM ZnodeOmsSavedCartLineItem a 
		 WHERE NOT EXISTS (SELECT TOP 1 1 FROM @TBL_OmsSavedCartOld b 
			 INNER JOIN @TBL_OmsSavedCartNew c ON (c.SKU = b.SKU AND c.ParentSKU = b.ParentSKU AND ISNULL(c.AddOnSKU,'-1') = ISNULL(b.AddOnSKU,'-1') AND ISNULL(c.PersonalizeCode,'-1') = ISNULL(b.PersonalizeCode,'-1') AND ISNULL(c.PersonalizeValue,'-1') = ISNULL(b.PersonalizeValue,'-1'))
			 WHERE b.OmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
		 AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_OmsSavedCartOld b 
			 INNER JOIN @TBL_OmsSavedCartNew c ON (c.SKU = b.SKU AND c.ParentSKU = b.ParentSKU AND ISNULL(c.AddOnSKU,'-1') = ISNULL(b.AddOnSKU,'-1') AND ISNULL(c.PersonalizeCode,'-1') = ISNULL(b.PersonalizeCode,'-1') AND ISNULL(c.PersonalizeValue,'-1') = ISNULL(b.PersonalizeValue,'-1'))
			 WHERE b.ParentOmsSavedCartLineItemId = a.OmsSavedCartLineItemId )
		 AND OmsSavedCartId = @OldOmsSavedCartId 

		 Update ZnodeOmsSavedCartLineItem SET OmsSavedCartId = @OmsSavedCartIdDummy  WHERE OmsSavedCartId = @OldOmsSavedCartId

		 ;WITH CTE_UpdateOrder 
		 AS 
		 (
		   SELECT Sequence,  ROW_NUMBER()Over(order BY OmsSavedCartLineItemId ASC) RowId
		   FROM ZnodeOmsSavedCartLineItem
		   WHERE  OmsSavedCartId = @OmsSavedCartId
	 
		 ) 
		 UPDATE CTE_UpdateOrder 
		 SET Sequence =RowId

		SET @GetDate = dbo.Fn_GetDate();

		 UPDATE ZnodeOmsSavedCart
		 SET ModifiedDate = @GetDate
		 WHERE OmsSavedCartId = @OmsSavedCartId

	 COMMIT TRAN DELETEOLDSAVECART
	 SET @Status = 1 

 END TRY 
 BEGIN CATCH 
 ROLLBACK TRAN DELETEOLDSAVECART
  SELECT ERROR_MESSAGE()
   SET @Status = 0

	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_MergeOmsSavedCartLineItems @OmsSavedCartId = '+CAST(@OmsSavedCartId AS VARCHAR(max))+',@OldOmsSavedCartId='+CAST(@OldOmsSavedCartId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status = '+CAST(@Status AS VARCHAR(50));

    SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

    EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_MergeOmsSavedCartLineItems',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
 END CATCH 
END