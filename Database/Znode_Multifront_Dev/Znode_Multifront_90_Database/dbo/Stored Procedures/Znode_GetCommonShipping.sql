CREATE PROCEDURE [dbo].[Znode_GetCommonShipping]
(
	@PortalIds VARCHAR(2000) = '',
	@ProfileIds VARCHAR(2000) = '',
	@ShippingIds VARCHAR(2000) = '' OUT
)
AS
 /*
  Summary:- This procedure is used to get the shipping details
  Unit Testing
  EXEC Znode_GetCommonShipping
 */
BEGIN
BEGIN TRY
SET NOCOUNT ON
     
	DECLARE @TBL_ShippingIds TABLE(ShippingId INT , ProfileId INT )

	INSERT INTO @TBL_ShippingIds (ShippingId,ProfileId)
	SELECT ShippingId,ProfileId
	FROM ZnodeProfileShipping ZPPS
	WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@ProfileIds,',') SP WHERE ZPPS.ProfileId = SP.Item)

	IF EXISTS (SELECT TOP 1 1 FROM @TBL_ShippingIds )
	BEGIN
		SET @ShippingIds = ISNULL(SUBSTRING(( SELECT ','+CAST(ShippingId  AS VARCHAR(50))
		FROM ZnodePortalShipping ZPPS
		WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PortalIds,',') SP WHERE ZPPS.PortalId = SP.Item)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_ShippingIds  TBPS WHERE TBPS.ShippingId = ZPPS.ShippingId ) FOR XML PATH ('')),2,4000),'')
	END

	IF NOT EXISTS (SELECT TOP 1 1 FROM @TBL_ShippingIds ) OR @ShippingIds= ''
	BEGIN
		SET @ShippingIds = SUBSTRING((  SELECT ','+CAST(ShippingId  AS VARCHAR(50))
		FROM ZnodePortalShipping ZPPS
		WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PortalIds,',') SP WHERE ZPPS.PortalId = SP.Item) FOR XML PATH ('')),2,4000)
	END

END TRY
BEGIN CATCH
	DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCommonShipping @PortalIds = '+@PortalIds+',@ProfileIds='+@ProfileIds+',@ShippingIds='+@ShippingIds+',@Status='+CAST(@Status AS VARCHAR(10));
             
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
 
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetCommonShipping',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;

END CATCH
END