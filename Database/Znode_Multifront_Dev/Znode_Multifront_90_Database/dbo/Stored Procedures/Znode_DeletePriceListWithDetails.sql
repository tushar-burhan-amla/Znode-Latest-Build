
CREATE  PROCEDURE [dbo].[Znode_DeletePriceListWithDetails](
       @PriceListId VARCHAR(300) ,
       @Status      INT OUT)
AS
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
             --SET @Status = 1 
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT 0 AS ID , CAST(0 AS BIT) AS Status; 
             --SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE()
             SET @Status = 0;
             ROLLBACK TRAN A;
         END CATCH;
     END;