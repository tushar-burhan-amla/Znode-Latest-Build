


CREATE PROCEDURE [dbo].[Znode_DeleteVendor]
(
	  @PimVendorId varchar(2000)= '',
	  @Status int OUT ,
	  @PimVendorIds TransferId READONLY 
 )
AS
/*
     Summary: Delete Vendor detail  from ZnodePimVendor with their respective address
	 Unit Testing :
	 EXEC Znode_DeleteVendor
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		BEGIN TRAN A;
		DECLARE @TBL_DeletdVendorId TABLE (  PimVendorId INT );
		DECLARE @TBL_DeletdAddressid TABLE ( AddressId INT   );	 
		DECLARE @FinalCount INT = 0  
		INSERT INTO @TBL_DeletdVendorId
			   SELECT Item
			   FROM dbo.split( @PimVendorId, ',' ) AS a;
		INSERT INTO @TBL_DeletdVendorId 
		    SELECT Id 
			FROM @PimVendorIds

		INSERT INTO @TBL_DeletdAddressid
			   SELECT zpv.AddressId
			   FROM ZnodePimVendor AS zpv
					INNER JOIN @TBL_DeletdVendorId AS da
					ON zpv.PimVendorId = da.PimVendorID;
		DELETE FROM ZnodePimVendor
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM @TBL_DeletdVendorId AS a
			WHERE a.PimVendorId = ZnodePimVendor.PimVendorId
		);
		DELETE FROM ZnodeAddress
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM @TBL_DeletdAddressid AS da
			WHERE da.AddressId = ZnodeAddress.AddressId
		);

		  SET @FinalCount = (	SELECT COUNT(1)
							FROM dbo.split( @PimVendorId, ',' ) AS a
							)
			SET @FinalCount = CASE WHEN @FinalCount = 0 THEN  (	SELECT COUNT(1)
							FROM @PimVendorIds AS a
							)	ELSE @FinalCount END 

		IF
		(
			SELECT COUNT(1)
			FROM @TBL_DeletdVendorId
		) =	 @FinalCount
		
		BEGIN
			SELECT 1 AS ID, CAST(1 AS bit) AS Status;
		END;
		ELSE
		BEGIN
			SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		END;
		SET @Status = 1;
		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
		 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteVendor @PimVendorId = '+@PimVendorId+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteVendor',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
	END CATCH;
END;