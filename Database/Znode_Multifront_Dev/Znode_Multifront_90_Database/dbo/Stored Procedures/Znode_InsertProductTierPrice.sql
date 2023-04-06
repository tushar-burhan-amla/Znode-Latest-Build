CREATE PROCEDURE [dbo].[Znode_InsertProductTierPrice]
(   @SKU         VARCHAR(300),
    @PriceListId INT,
    @PriceTierId VARCHAR(3000),
    @Price       VARCHAR(3000),
    @Quantity    VARCHAR(3000),
    @User        INT,
    @UomId       INT            = NULL,
    @UnitSize    NUMERIC(28, 6)  = NULL,
	@Custom1 NVARCHAR(MAX) = NULL,--=N'21,,45',
	@Custom2 NVARCHAR(MAX) = NULL,--=N',21,',
	@Custom3 NVARCHAR(MAX) = NULL --=N',,',
	)
AS 
    /*
     Summary : This procedure used to save the multiple tier price of sku 
     SELECT * FROM ZnodePriceTier WHERE SKU ='SGHJ150' AND  PriceListId=103
	 BEGIN TRAN
     EXEC Znode_InsertProductTierPrice @SKU='SGHJ150',@PriceTierId ='0,0,0' ,@PriceListId=103,@Price='1,2,1',@Quantity='1,2,1',@User=2
	 ROLLBACK TRAN
   
	*/
     BEGIN  
         BEGIN TRAN;
         BEGIN TRY 
		 SET NOCOUNT ON
		 	 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             ----- Declare The varibale and Required varibale table for procedure Internal use-----
             DECLARE @TBL_TierPriceCheckData TABLE
             (RowId        INT IDENTITY(1, 1),
              PriceTierId  INT,
              PriceListId  INT,
              SKU          VARCHAR(300),
              Price        NUMERIC(28, 6),
              Quantity     NUMERIC(28, 6),
              UomId        INT,
              UnitSize     NUMERIC(28, 6),
              CreatedBy    INT,
              CreatedDate  DATETIME,
              ModifiedBy   INT,
              ModifiedDate DATETIME,
			  Custom1 NVARCHAR(MAX),
			  Custom2 NVARCHAR(MAX),
			  Custom3 NVARCHAR(MAX)
             );
             DECLARE @TBL_TierPrice TABLE
             (PriceTierId  INT,
              PriceListId  INT,
              SKU          VARCHAR(300),
              Price        NUMERIC(28, 6),
              Quantity     NUMERIC(28, 6),
              UomId        INT,
              UnitSize     NUMERIC(28, 6),
              CreatedBy    INT,
              CreatedDate  DATETIME,
              ModifiedBy   INT,
              ModifiedDate DATETIME,
			  Custom1 NVARCHAR(MAX),
			  Custom2 NVARCHAR(MAX),
			  Custom3 NVARCHAR(MAX)
             );
             DECLARE @TBL_Quantity TABLE
             (Id       INT,
              Quantity NUMERIC(28, 6)
             );
             DECLARE @TBL_Price TABLE
             (Id    INT,
              Price NUMERIC(28, 6)
             );
             DECLARE @TBL_PriceTierId TABLE
             (Id          INT,
              PriceTierId INT
             );
			 DECLARE @TBL_Custom1 TABLE
             (Id       INT,
              Custom1 NVARCHAR(MAX)
             );
			 DECLARE @TBL_Custom2 TABLE
             (Id       INT,
              Custom2 NVARCHAR(MAX)
             );
			 DECLARE @TBL_Custom3 TABLE
             (Id       INT,
              Custom3 NVARCHAR(MAX)
             );
             DECLARE @ErrorInRecord TABLE(ID INT); 
             ------ Insert the  comma separeted data into variable table -------
             INSERT INTO @TBL_Quantity
                    SELECT Id,
                           Item
                    FROM Dbo.split(@Quantity, ',') AS a;
             INSERT INTO @TBL_Price
                    SELECT Id,
                           Item
                    FROM Dbo.split(@Price, ',') AS a;
             INSERT INTO @TBL_PriceTierId
                    SELECT Id,
                           Item
                    FROM Dbo.split(@PriceTierId, ',') AS a;

					INSERT INTO @TBL_Custom1
                    SELECT Id,
                           Item
                    FROM Dbo.split(@Custom1, ',') AS a;
					
					INSERT INTO @TBL_Custom2
                    SELECT Id,
                           Item
                    FROM Dbo.split(@Custom2, ',') AS a;

					INSERT INTO @TBL_Custom3
                    SELECT Id,
                           Item
                    FROM Dbo.split(@Custom3, ',') AS a;

             INSERT INTO @TBL_TierPriceCheckData
             (PriceTierId,PriceListId,SKU,Price,Quantity,UomId,UnitSize,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Custom1,Custom2,Custom3)
             SELECT tpt.PriceTierId,@PriceListId,@SKU,tp.Price,tq.Quantity,@UomId,@UnitSize,@user,@GetDate,@User,@GetDate,Custom1,Custom2,Custom3
             FROM @TBL_Quantity AS tq
             INNER JOIN @TBL_Price AS tp ON(tq.Id = tp.Id)
             INNER JOIN @TBL_PriceTierId AS tpt ON(tpt.Id = tp.Id)
			 INNER JOIN @TBL_Custom1 AS tpc1 ON(tpc1.Id = tp.Id)
			 INNER JOIN @TBL_Custom2 AS tpc2 ON(tpc2.Id = tp.Id)
			 INNER JOIN @TBL_Custom3 AS tpc3 ON(tpc3.Id = tp.Id)
			 ;
			
             IF EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_TierPriceCheckData AS zpt
                 GROUP BY SKU,
                          Quantity
                 HAVING COUNT(PriceTierId) >= 2
             )
                 BEGIN
                     INSERT INTO @ErrorInRecord(id)
                            SELECT MIN(RowId)
                            FROM @TBL_TierPriceCheckData AS a
                            GROUP BY SKU,
                                     Quantity,
                                     PriceListid;


                     INSERT INTO @TBL_TierPrice
                     (PriceTierId,PriceListId,SKU,Price,Quantity,UomId,UnitSize,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Custom1,Custom2,Custom3)
                     SELECT PriceTierId,PriceListId,SKU,Price,Quantity,UomId,UnitSize,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Custom1,Custom2,Custom3
                     FROM @TBL_TierPriceCheckData AS a
                     INNER JOIN @ErrorInRecord AS b ON(a.RowId = b.Id);
                 END;
             ELSE
                 BEGIN
                     INSERT INTO @TBL_TierPrice
                     (PriceTierId,
                      PriceListId,
                      SKU,
                      Price,
                      Quantity,
                      UomId,
                      UnitSize,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate,
					  Custom1,
					  Custom2,
			          Custom3
                     )
                            SELECT PriceTierId,
                                   PriceListId,
                                   SKU,
                                   Price,
                                   Quantity,
                                   UomId,
                                   UnitSize,
                                   CreatedBy,
                                   CreatedDate,
                                   ModifiedBy,
                                   ModifiedDate,
								   Custom1,
						           Custom2,
			                       Custom3
                            FROM @TBL_TierPriceCheckData AS a;

                 END;
             UPDATE ztp
               SET
                   ztp.Price = a.Price,
                   ztp.Quantity = a.Quantity,
				   ztp.Custom1 = a.Custom1,
				   ztp.Custom2 = a.Custom2,
				   ztp.Custom3 = a.Custom3,
                   ztp.ModifiedBy = a.ModifiedBy,
                   ztp.ModifiedDate = a.ModifiedDate
             FROM ZnodePriceTier ztp
                  INNER JOIN @TBL_TierPrice a ON(a.SKU = ztp.SKU
                                                 AND a.PriceTierId = ztp.PriceTierId
                                                 AND a.PriceListId = ztp.PriceListid);
											
             INSERT INTO ZnodePriceTier
             (PriceListId,
              SKU,
              Price,
              Quantity,
              UomId,
              UnitSize,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate,
			  Custom1,
			  Custom2,
			  Custom3
             )
                    SELECT PriceListId,
                           SKU,
                           Price,
                           Quantity,
                           UomId,
                           UnitSize,
                           CreatedBy,
                           CreatedDate,
                           ModifiedBy,
                           ModifiedDate,
						   Custom1,
						   Custom2,
			               Custom3
                    FROM @TBL_TierPrice AS a
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePriceTier AS ztp
                        WHERE ztp.SKU = a.SKU
                              AND ztp.Quantity = a.Quantity
                              AND a.PriceListId = ztp.PriceListid
                    );
				 
             SELECT 0 AS ID,
                    CASE
                        WHEN NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM @ErrorInRecord
             )
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END AS Status;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
		     DECLARE @Status BIT ;
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertProductTierPrice @SKU = '+@SKU+',@PriceListId='+CAST(@PriceListId AS VARCHAR(200))+',@PriceTierId='+@PriceTierId+',@Price ='+@Price+',@Quantity='+@Quantity+',@User='+CAST(@User AS VARCHAR(200))+',@UomId='+CAST(@UomId AS VARCHAR(200))+',@UnitSize='+CAST(@UnitSize AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN ;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_InsertProductTierPrice',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;