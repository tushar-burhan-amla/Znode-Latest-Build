CREATE PROCEDURE [dbo].[Znode_DeleteSaveCartLineItem]
(
	  @OmsSavedCartLineItemId  int,
	  @Status bit OUT 
)
AS 
BEGIN
	BEGIN TRY
	SET NOCOUNT ON;
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	----Adding dummy CookieMappingId if not present
	if not exists(select * from ZnodeOmsCookieMapping where OmsCookieMappingId = 1)
	begin
		SET IDENTITY_INSERT ZnodeOmsCookieMapping ON
		INSERT INTO ZnodeOmsCookieMapping(OmsCookieMappingId,UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT 1,null,(select top 1 PortalId from ZnodePortal order by 1 ASC),2,@GetDate,2,@GetDate
		SET IDENTITY_INSERT ZnodeOmsCookieMapping OFF
	end

	----geting dummy OmsSavedCartId on basis of OmsCookieMappingId = 1 if not present then add
	Declare @OmsSavedCartId int = 0
	SET @OmsSavedCartId = (Select Top 1 OmsSavedCartId  from ZnodeOmsSavedCart With(NoLock) where OmsCookieMappingId = 1)
	If Isnull(@OmsSavedCartId ,0) = 0 
	Begin 
		Insert into ZnodeOmsSavedCart(OmsCookieMappingId,SalesTax,RecurringSalesTax,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		Select  1,null,null,2,@GetDate,2,@GetDate
		SET @OmsSavedCartId  = @@IDENTITY
	end
	
	DECLARE @TBL_DeleteSavecartLineitems TABLE (OmsSavedCartLineItemId int)

	IF OBJECT_ID(N'tempdb..#TBL_ZnodeOmsSavedCartLineItem') IS NOT NULL
		DROP TABLE #TBL_ZnodeOmsSavedCartLineItem

	----Getting date related to @OmsSavedCartLineItemId input parameter into a table
	select OmsSavedCartLineItemId,	ParentOmsSavedCartLineItemId   
	INTO #TBL_ZnodeOmsSavedCartLineItem  
	from ZnodeOmsSavedCartLineItem  with (NOLOCK)
	where OmsSavedCartLineItemId = @OmsSavedCartLineItemId or ParentOmsSavedCartLineItemId =@OmsSavedCartLineItemId 

	--selecting all the record which needs to be deleted
	INSERT INTO @TBL_DeleteSavecartLineitems	
	select OmsSavedCartLineItemId from #TBL_ZnodeOmsSavedCartLineItem
	union
	select ParentOmsSavedCartLineItemId from #TBL_ZnodeOmsSavedCartLineItem
		where not exists (select  OmsSavedCartLineItemId,	ParentOmsSavedCartLineItemId from ZnodeOmsSavedCartLineItem 
				where OmsSavedCartLineItemId != #TBL_ZnodeOmsSavedCartLineItem.OmsSavedCartLineItemId and  ParentOmsSavedCartLineItemId =#TBL_ZnodeOmsSavedCartLineItem.ParentOmsSavedCartLineItemId)
				and ParentOmsSavedCartLineItemId is not null

	BEGIN TRAN DeleteSaveCartLineItem;

	IF exists (select top 1 1 from @TBL_DeleteSavecartLineitems)
	Begin
			Update ZnodeOmsSavedCartLineItemDetails SET OmsSavedCartId = @OmsSavedCartId
			WHERE EXISTS
			(
				SELECT TOP 1 1
				FROM @TBL_DeleteSavecartLineitems DeleteSaveCart
				WHERE DeleteSaveCart.OmsSavedCartLineItemId = ZnodeOmsSavedCartLineItemDetails.OmsSavedCartLineItemId
		
			);
	  
			Update ZnodeOmsSavedCartLineItem SET OmsSavedCartId = @OmsSavedCartId
			WHERE EXISTS
			(
				SELECT TOP 1 1
				FROM @TBL_DeleteSavecartLineitems DeleteSaveCart
				WHERE DeleteSaveCart.OmsSavedCartLineItemId = ZnodeOmsSavedCartLineItem.OmsSavedCartLineItemId
			);
	End	

	SET @Status = 1;
	COMMIT TRAN DeleteSaveCartLineItem;
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		SET @Status = 0;
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_DeleteSaveCartLineItem @OmsSavedCartLineItemId = '+CAST(@OmsSavedCartLineItemId AS varchar(max))
		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		ROLLBACK TRAN DeleteSaveCartLineItem;
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_DeleteSaveCartLineItem', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END