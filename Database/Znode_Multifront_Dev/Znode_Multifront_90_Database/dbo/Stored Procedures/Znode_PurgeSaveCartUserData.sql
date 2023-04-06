CREATE PROCEDURE [dbo].[Znode_PurgeSaveCartUserData]
As
Begin
	BEGIN TRY
		SET NOCOUNT ON;

		Declare @OmsSavedCartId int
	    Select Top 1 @OmsSavedCartId = OmsSavedCartId  from ZnodeOmsSavedCart With(NoLock) where OmsCookieMappingId = 1 


		DELETE pci
		FROM ZnodeOmsPersonalizeCartItem pci
		INNER JOIN ZnodeOmsSavedCartLineItem sci ON pci.OmsSavedCartLineItemId = sci.OmsSavedCartLineItemId 
		INNER JOIN ZnodeOmsSavedCart sc ON sci.OmsSavedCartId = sc.OmsSavedCartId
		WHERE sc.OmsSavedCartId = @OmsSavedCartId

		DELETE scid 
		FROM ZnodeOmsSavedCartLineItemDetails scid
		INNER JOIN ZnodeOmsSavedCartLineItem sci ON scid.OmsSavedCartLineItemId = sci.OmsSavedCartLineItemId 
		INNER JOIN ZnodeOmsSavedCart sc ON sci.OmsSavedCartId = sc.OmsSavedCartId
		WHERE sc.OmsSavedCartId = @OmsSavedCartId

		DELETE sci
		FROM ZnodeOmsSavedCartLineItem sci 
		INNER JOIN ZnodeOmsSavedCart sc ON sci.OmsSavedCartId = sc.OmsSavedCartId
		WHERE sc.OmsSavedCartId = @OmsSavedCartId

		--Delete Orphan items
		DELETE sosclid FROM ZnodeOmsSavedCartLineItemDetails sosclid 
		LEFT JOIN ZnodeOmsSavedCart zosc ON sosclid.OmsSavedCartId=zosc.OmsSavedCartId
		WHERE zosc.OmsSavedCartId IS NULL

		DELETE zoscli FROM ZnodeOmsSavedCartLineItem zoscli
			LEFT JOIN ZnodeOmsSavedCart zosc ON zoscli.OmsSavedCartId=zosc.OmsSavedCartId
			WHERE zosc.OmsSavedCartId IS NULL

		DELETE zosc FROM ZnodeOmsSavedCart zosc
			LEFT JOIN ZnodeOmsSavedCartLineItem zoscli ON zosc.OmsSavedCartId=zoscli.OmsSavedCartId
			WHERE zoscli.OmsSavedCartId IS NULL 
			AND zosc.OmsSavedCartId<>@OmsSavedCartId  


	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()	
		DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max) = 'EXEC Znode_PurgeSaveCartUserData'
		SELECT 0 AS ID, CAST(0 AS bit) AS Status,ERROR_MESSAGE();
		EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_PurgeSaveCartUserData', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
End