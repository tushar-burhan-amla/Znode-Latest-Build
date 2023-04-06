CREATE PROCEDURE [dbo].[Znode_GetShippingOptions]
	(@ZipCode    VARCHAR(20),
	 @ProfileId  INT)
AS

/*
summary: Procedure Used to get Shipping details filter by ProfileId

Unit Testing:
EXEC [Znode_GetShippingOptions]  @ZipCode= '71*, 53*',@ProfileId = 1
 
*/
BEGIN
	BEGIN TRY
    SET NOCOUNT ON

	SELECT ZS.ShippingId,ZPS.ProfileId,ShippingCode,ShippingName,HandlingCharge,HandlingChargeBasedOn,DestinationCountryCode,StateCode,CountyFIPS,
	Description,IsActive,ZS.DisplayOrder,ZipCode,ZS.CreatedDate,ZS.ModifiedDate
	 FROM ZnodeShipping ZS INNER JOIN ZnodeProfileShipping ZPS on(ZS.ShippingId = ZPS.ShippingId)
	WHERE ZPs.ProfileId = @ProfileId
	AND ZipCode like '%' + @ZipCode + '%'

	END TRY
	BEGIN CATCH
	        DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetShippingOptions @ZipCode = '+@ZipCode+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetShippingOptions',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH
END