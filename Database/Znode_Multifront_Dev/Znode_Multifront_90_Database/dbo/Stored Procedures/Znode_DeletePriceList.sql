
CREATE PROCEDURE [dbo].[Znode_DeletePriceList](
      @PriceListId VARCHAR(300) ,
      @Status      INT OUT)
AS
/*
Summary: This Procedure is used to delete PriceList
Unit Testing : 
begin tran
EXEC Znode_DeletePriceList @PriceListId = 9, @Status =0
rollback tran
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             BEGIN TRAN A;
             DECLARE @DeletdPriceListId TABLE (
                                              PriceListId INT
                                              );
             INSERT INTO @DeletdPriceListId
                    SELECT Item
                    FROM dbo.split ( @PriceListId , ','
                                   ) AS a;
             DELETE FROM ZnodePriceListUser
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM ZnodePriceListAddress
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM ZnodePriceListPortal
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM ZnodePriceListProfile
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM dbo.ZnodePriceListAccount
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM ZnodePrice
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM ZnodePriceTier
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             DELETE FROM ZnodePriceList
             WHERE PriceListId IN ( SELECT PriceListId
                                    FROM @DeletdPriceListId
                                  );
             IF ( SELECT COUNT(1)
                  FROM @DeletdPriceListId
                ) = ( SELECT COUNT(1)
                      FROM dbo.split ( @PriceListId , ','
                                     ) AS a
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                     SET @Status = 0;
                 END;
             
             COMMIT TRAN A;
			 
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePriceList @PriceListId = '+@PriceListId+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeletePriceList',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
            
            
         END CATCH;
     END;