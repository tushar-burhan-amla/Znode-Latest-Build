CREATE PROCEDURE [dbo].[Znode_GetOmsSavedCartLineItemCount]
( 
	@OmsCookieMappingId INT ,
	@UserId INT NULL ,
	@PortalId INT NULL
)
AS 
   /*  
    Summary : This procedure is used to get the count of OmsSavedCartLineItem	 	 
Unit Testing:

    EXEC [Znode_GetOmsSavedCartLineItemCount] @OmsCookieMappingId=83503
	Create Index IX_ZnodeOmsCookieMapping_UserId_PortalId on ZnodeOmsCookieMapping(UserId,PortalId)
       
*/
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		Declare @QuantityOfOthersType Numeric(28,6) ,@QuantityOfBundles Numeric(28,6) ,@OmsSavedCartId int,
				@AddonRelationshipType int , @BundleRelationshipType int 


	    IF @OmsCookieMappingId = 0
		BEGIN
				IF isnull(@UserId,0) = 0
				BEGIN
					Select CAST(@QuantityOfOthersType AS varchar(10))
					Return
				End
			   Select TOP 1 @OmsCookieMappingId =   OmsCookieMappingId from ZnodeOmsCookieMapping WITH (NOLOCK) WHERE UserId = @UserId and PortalId = @PortalId 
		END

		IF Object_id('tempdb..#BundlesProductsQuantity') <>0 
		DROP TABLE #BundlesProductsQuantity

		IF Object_id('tempdb..#ZnodeOmsSavedCartLineItem') <>0 
		DROP TABLE #ZnodeOmsSavedCartLineItem


		Select @OmsSavedCartId = OmsSavedCartId from ZnodeOmsSavedCart  WITH (NOLOCK) WHERE OmsCookieMappingId =@OmsCookieMappingId
		Select @AddonRelationshipType = OrderLineItemRelationshipTypeId	 From ZnodeOmsOrderLineItemRelationshipType WITH (NOLOCK) Where Name = 'AddOns'
		Select @BundleRelationshipType = OrderLineItemRelationshipTypeId From ZnodeOmsOrderLineItemRelationshipType WITH (NOLOCK) Where Name = 'Bundles'
		Select Quantity, OmsSavedCartLineItemId, ParentOmsSavedCartLineItemId, OrderLineItemRelationshipTypeId into #ZnodeOmsSavedCartLineItem
		from ZnodeOmsSavedCartLineItem  WITH (NOLOCK) where  OmsSavedCartId = @OmsSavedCartId  

		--Read quantity of all others type of products except Bundle type
		SELECT @QuantityOfOthersType  = Sum(ISNULL(Quantity,0)) FROM #ZnodeOmsSavedCartLineItem oscl WITH (NOLOCK)
        WHERE (ParentOmsSavedCartLineItemId IS NOT NULL AND OrderLineItemRelationshipTypeId not in  (@AddonRelationshipType ,@BundleRelationshipType) 
		AND OrderLineItemRelationshipTypeId IS NOT NUll)  

		--Read quantity of Bundle type from their parent Quantity only
		Select Distinct ParentOmsSavedCartLineItemId into #BundlesProductsQuantity FROM #ZnodeOmsSavedCartLineItem oscl WITH (NOLOCK) 
		where OrderLineItemRelationshipTypeId = @BundleRelationshipType
		If Exists (Select TOP 1 1 #BundlesProductsQuantity )
			SELECT @QuantityOfBundles = Sum(ISNULL(Quantity,0))   FROM #ZnodeOmsSavedCartLineItem oscl WITH (NOLOCK)
			Where Exists ( Select  TOP 1 1 From #BundlesProductsQuantity BPQ Where BPQ.ParentOmsSavedCartLineItemId = oscl.OmsSavedCartLineItemId ) 
			And (ParentOmsSavedCartLineItemId IS NULL AND OrderLineItemRelationshipTypeId IS NUll)  
		
		SET @QuantityOfOthersType = Isnull(@QuantityOfOthersType,0) + Isnull(@QuantityOfBundles,0)

		Select CAST(@QuantityOfOthersType AS varchar(15))

    END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsSavedCartLineItemCount '+ISNULL(CAST(@OmsCookieMappingId AS VARCHAR(50)),'''');
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetOmsSavedCartLineItemCount',
		@ErrorInProcedure = 'Znode_GetOmsSavedCartLineItemCount',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END