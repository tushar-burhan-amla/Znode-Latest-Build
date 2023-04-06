
CREATE PROCEDURE [dbo].[Znode_DeletePortalAddress](
       @PortalAddressId VARCHAR(300) = '' ,
	   @StoreLocationCode VARCHAR (300) = '',
       @Status          INT OUT)
AS 
    /*
     Summary : Remove portal address with their mapping in table ZNodePortalAddress and finally delete from table ZnodeAddress 
     Sequence For Delete Data  
     Validation :  AddressId is only associated with ZNodePortalAddress will not associated with other data 
     1. ZNodePortalAddress          
     2. ZnodeAddress	
    Unit Testing
	begin tran	 
    Declare @Status int 
    Exec Znode_DeletePortalAddress @StoreLocationCode = 'test'  , @Status=@Status
   rollback tran
    Exec Znode_DeletePortalAddress @PortalAddressId = 10  , @Status=@Status
   */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @DeletedPortalAddressId TABLE (PortalAddressId INT );

             --INSERT INTO @DeletedPortalAddressId
             --       SELECT Item
             --       FROM dbo.split ( @PortalAddressId , ','
             --                      ) AS a;


			 INSERT INTO @DeletedPortalAddressId
					SELECT PortalAddressId FROM ZNodePortalAddress PA
					WHERE CASE WHEN @StoreLocationCode = '' THEN CAST(PortalAddressId AS NVARCHAR(2000)) ELSE StoreLocationCode END IN 
										(SELECT Item FROM dbo.Split (CASE WHEN @StoreLocationCode = '' THEN @PortalAddressId ELSE @StoreLocationCode END, ',' ) AS SP)



             DECLARE @DeletedAddressId TABLE (
                                             AddressId INT
                                             );


             INSERT INTO @DeletedAddressId
                    SELECT AddressID
                    FROM ZNodePortalAddress AS ZPA INNER JOIN @DeletedPortalAddressId AS DPA ON ZPA.PortalAddressId = DPA.PortalAddressId;


             DELETE FROM ZNodePortalAddress
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedPortalAddressId AS DPA
                            WHERE DPA.PortalAddressId = ZNodePortalAddress.PortalAddressId
                          );
             DELETE FROM ZnodeAddress
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @DeletedAddressId AS DA
                            WHERE DA.AddressId = ZnodeAddress.AddressId
                          );
             IF ( SELECT COUNT(1) FROM @DeletedAddressId) = ( SELECT COUNT(1) FROM dbo.split ( CASE WHEN @StoreLocationCode = '' THEN @PortalAddressId ELSE @StoreLocationCode END, ',' ))

                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePortalAddress @PortalAddressId = '+@PortalAddressId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeletePortalAddress',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;