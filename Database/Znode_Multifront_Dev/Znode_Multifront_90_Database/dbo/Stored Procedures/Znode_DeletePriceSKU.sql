
CREATE PROCEDURE [dbo].[Znode_DeletePriceSKU]
      ( @PriceIds    VARCHAR(1000) ,
       @PriceListId INT ,
       @Status      BIT OUT)
AS
   /* 
    Summary:  Delete price data of sku from table znodePrice  and ZnodeTierPricing
    Unit Testing   
    Begin 
    	Begin Transaction 
    		Exec Znode_DeletePriceSKU    @PriceIds = '70,71',  @PriceListId =37 , @Status = 0   
     SELECT * FROM ZnodePrice 
     SELECT * FROM ZnodePriceTier
    	Rollback Transaction 
    ENd  
   */
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_ZnodePrice TABLE (
                                           PriceId     INT ,
                                           PriceListId INT ,
                                           SKU         VARCHAR(300)
                                           );
             --- declare the table for store the price list and pricelistid and sku to delete from multiple tables  
             DECLARE @TBL_DeletedIds TABLE (
                                           Id   INT ,
                                           Item VARCHAR(300)
                                           );
             --- declare the table to store the comma separeted values into record format 

             INSERT INTO @TBL_DeletedIds
                    SELECT ID , Item
                    FROM dbo.Split ( @PriceIds , ','
                                   ) AS sl;  -- insert the data into variable table 


             INSERT INTO @TBL_ZnodePrice
                    SELECT zp.PriceId , zp.PriceListId , SKU
                    FROM ZnodePrice AS zp INNER JOIN @TBL_DeletedIds AS sl ON ( sl.item = zp.PriceId )
                    WHERE zp.PriceListId = @PriceListId;
             DELETE FROM ZnodePrice
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_ZnodePrice AS zp
                            WHERE zp.PriceId = ZnodePrice.PriceId
                          );
             DELETE FROM ZnodePriceTier
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_ZnodePrice AS zp
                            WHERE zp.SKU = ZnodePriceTier.SKU
                                  AND
                                  zp.PriceListId = ZnodePriceTier.PriceListId
                          );
             SET @Status = 1;
             IF ( SELECT COUNT(1)
                  FROM @TBL_ZnodePrice
                ) = ( SELECT COUNT(1)
                      FROM @TBL_DeletedIds
                    )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeletePriceSKU @PriceIds = '+@PriceIds+',@PriceListId='+CAST(@PriceListId AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeletePriceSKU',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;