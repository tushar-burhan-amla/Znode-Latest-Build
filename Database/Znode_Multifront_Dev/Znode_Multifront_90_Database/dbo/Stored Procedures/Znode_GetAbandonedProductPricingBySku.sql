
CREATE PROCEDURE [dbo].[Znode_GetAbandonedProductPricingBySku]
(   
    @SKU              SelectColumnList READONLY,
    @PortalId         TransferId READONLY,
    @currentUtcDate   VARCHAR(100), -- this date is required for the user date r
    @UserId           INT          = 0 -- userid is optional 
    
	)
AS 
   /* 
    --Summary: Retrive Price of product from pricelist
    --Input Parameters:
    --UserId, SKU(Comma separated multiple), PortalId
    --Conditions :
    --1. If userId is null then check for PriceList having sku associated to profile which is associated to Portal having  PortalId and  having higher Precedence and valid ActivationDate and ExpirationDate for PriceList  and SKU also.
    --Unit Testing : 
    --EXEC Znode_GetPublishProductPricingBySku_2 @SKU = 'apple,apr234' , @PortalId = 34 , @currentUtcDate = '2016-09-17 00:00:00.000';
    --2. If There is no any PriceList having given sku associated to profile  then check for  
    --PriceList associated portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
    --Unit Testing : 
    --EXEC Znode_GetPublishProductPricingBySku_2 @SKU = 'apple,apr234' , @PortalId = 34 , @currentUtcDate = '2016-09-17 00:00:00.000';
    --3. If userId is not null then check for PriceList having sku associated to User having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
    --4. If There is no any PriceList having given sku associated to user  then check for  
    --PriceList associated Account having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
    --5. If There is no any PriceList having given sku associated to account  then check for  
    --PriceList associated Profile having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
    --6. If There is no any PriceList having given sku associated to Profile  then check for  
    --PriceList associated Portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
    --7. If in each case Precedence is same then get PriceList according to higher PriceListId ActivationDate and ExpirationDate for PriceList and SKU also.
    --8. Also get the Tier Price, Tier Quantity of given sku.
    --Unit Testing   
    --Exec Znode_GetPublishProductPricingBySku  @SKU = 'Levi''s T-Shirt & Jeans - Bundle Product',@PortalId = 1, @currentUtcDate = '2016-07-31 00:00:00.000'
	*/
    
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @Tlb_SKU TABLE
             (SKU        VARCHAR(100),
              SequenceNo INT IDENTITY
             );

			  DECLARE @DefaultLocaleId INT = dbo.FN_GETDEFAULTLocaleId()

			IF OBJECT_ID('TEMPDB..#TLB_SKUPRICELIST') IS NOT NULL
			DROP TABLE #TLB_SKUPRICELIST

			IF OBJECT_ID('TEMPDB..#Tbl_PortalWisePriceList') IS NOT NULL
			DROP TABLE #Tbl_PortalWisePriceList

			IF OBJECT_ID('TEMPDB..#Tbl_PriceListWisePrice') IS NOT NULL
			DROP TABLE #Tbl_PriceListWisePrice

			 --IF @SKU = '' 
			 --BEGIN 
			 -- INSERT INTO @Tlb_SKU(SKU)
			 -- 	SELECT (SELECT ''+SKU FOR XML PATH('')) 
				--	FROM ZnodePublishProductDetail a
				--	WHERE LocaleId = @DefaultLocaleId


			 --END 
			 --ELSE 
			 --BEGIN
			 --  INSERT INTO @Tlb_SKU(SKU)
    --                SELECT Item
    --                FROM Dbo.split(@SKU, ',');
			  

			 --END 

          
             CREATE TABLE #TLB_SKUPRICELIST 
             (SKU          VARCHAR(100),
              RetailPrice  NUMERIC(28, 6),
              SalesPrice   NUMERIC(28, 6),
              PriceListId  INT,
              TierPrice    NUMERIC(28, 6),
              TierQuantity NUMERIC(28, 6),
			  ExternalId NVARCHAR(2000),
			  Custom1 NVARCHAR(MAX),
			  Custom2 NVARCHAR(MAX),
			  Custom3 NVARCHAR(MAX)
             );
             DECLARE @PriceListId INT, @PriceRoundOff INT;
             SELECT @PriceRoundOff = CONVERT( INT, FeatureValues)
             FROM ZnodeGlobalSetting
             WHERE FeatureName = 'PriceRoundOff';
		
             --Retrive portal wise pricelist  
             CREATE TABLE #Tbl_PortalWisePriceList 
             (PriceListId    INT,
              ActivationDate DATETIME,
              ExpirationDate DATETIME NULL,
              Precedence     INT,
			  SKU NVARCHAR(300)
             );
             --Retrive price for respective pricelist   
             CREATE TABLE #Tbl_PriceListWisePrice  
             (
				  PriceListId    INT,
				  SKU            VARCHAR(300),
				  SalesPrice     NUMERIC(28, 6),
				  RetailPrice    NUMERIC(28, 6),
				  UomId          INT,
				  UnitSize       NUMERIC(28, 6),
				  ActivationDate DATETIME,
				  ExpirationDate DATETIME NULL,
				  TierPrice      NUMERIC(28, 6),
				  TierQuantity   NUMERIC(28, 6),
				  TierUomId      INT,
				  TierUnitSize   NUMERIC(28, 6), 
				  ExternalId NVARCHAR(2000),
				  Custom1 NVARCHAR(MAX),
				  Custom2 NVARCHAR(MAX),
				  Custom3 NVARCHAR(MAX)
             );
			 DECLARE @Tbl_SKUWisePriceList TABLE (PriceListId INT, SKU NVARCHAR(300))

			 insert into @Tbl_SKUWisePriceList(PriceListId,SKU) 
			 SELECT  PriceListId,SKU from ZnodePrice ZP where exists (Select * from @SKU sku where ZP.SKU = sku.StringColumn)
			 Union
			 SELECT PriceListId,SKU  from ZnodePriceTier ZPT where exists (Select * from @SKU sku where ZPT.SKU = sku.StringColumn) 
			 
			

			 --1. If userId is null then check for PriceList having sku associated to profile which is associated to Portal having  PortalId and  having higher Precedence and valid ActivationDate and ExpirationDate for PriceList  and SKU also.
            IF @UserId = 0
                 BEGIN
					INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
					SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
					FROM ZnodePriceList AS a INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId INNER JOIN ZnodePortalProfile AS c
						ON b.PortalProfileId = c.PortalProfileID AND  c.IsDefaultAnonymousProfile = 1 INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId 
						inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
					WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND 
					 EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = c.PortalId) -- c.PortalId = @PortalId
					ORDER BY b.Precedence;
		
			 
                     --2. If There is no any PriceList having given sku associated to profile  then check for PriceList associated portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
			IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl where NOT Exists (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
				WHERE tspl.SKU = tpwl.SKU))
                         BEGIN
							INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
							SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
							FROM ZnodePriceList AS a INNER JOIN ZnodePriceListPortal AS b ON a.PriceListId = b.PriceListId
							INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId   
							inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
							AND NOT EXISTS (Select TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
							WHERE @CurrentUtcDate BETWEEN a.ActivationDate 
							AND ISNULL(a.ExpirationDate, @GetDate) AND 
							 EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = b.PortalId) --b.PortalId = @PortalId
							ORDER BY b.Precedence
							;
							--Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  #Tbl_PortalWisePriceList )
						
                         END;
                 END;
                     --3. If userId is not null then check for PriceList having sku associated to User having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
             ELSE
                 BEGIN
				 
                     INSERT INTO #Tbl_PortalWisePriceList (PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
                            SELECT a.PriceListId, ActivationDate,ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
                            FROM ZnodePriceList AS a INNER JOIN ZnodePriceListUser AS b ON a.PriceListId = b.PriceListId
                                 INNER JOIN ZnodePortalunit zupu ON (a.CultureId = zupu.CultureId 
								 AND EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = zupu.PortalId) )--zupu.PortalId = @PortalId  )
								 inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
								 AND NOT EXISTS (Select TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
								 
                            WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND b.UserID = @UserId
							ORDER BY b.Precedence ;

                --4. If There is no any PriceList having given sku associated to user  then check for PriceList associated Account having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
				IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl where NOT Exists (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
				WHERE tspl.SKU = tpwl.SKU))
						BEGIN
							INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
								   SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), c.Precedence,tsw.SKU
								   FROM ZnodePriceList AS a INNER JOIN ZnodePriceListAccount AS c ON a.PriceListId = c.PriceListId
										INNER JOIN ZnodeUser AS d ON c.Accountid = d.Accountid INNER JOIN ZnodePortalunit AS zupu ON (a.CultureId = zupu.CultureId   
										AND EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = zupu.PortalId) )--AND zupu.PortalId = @PortalId
										inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
										AND NOT EXISTS (Select TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
								   WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND d.UserID = @UserId
									ORDER BY c.Precedence
							--Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  #Tbl_PortalWisePriceList )
						 END;
                     -- 5. If There is no any PriceList having given sku associated to account  then check for PriceList associated Profile having PortalId and having higher   Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
				IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl 
				where NOT Exists (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
				WHERE tspl.SKU = tpwl.SKU))

                         BEGIN
                             INSERT INTO #Tbl_PortalWisePriceList(PriceListId,ActivationDate,ExpirationDate,Precedence,SKU)
                                    SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
                                    FROM ZnodePriceList AS a
                                         INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId 
										 INNER JOIN ZnodePortalProfile AS c ON (b.PortalProfileId = c.PortalProfileId  
										 AND EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = c.PortalId) )--AND c.PortalId = @PortalId 
                                         INNER JOIN dbo.ZnodeUserProfile zup ON c.ProfileId = zup.ProfileId AND IsDefault = 1
                                         INNER JOIN ZnodePortalunit zupu ON (a.CultureId = zupu.CultureId 
										 AND EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = zupu.PortalId) )--AND zupu.PortalId = @PortalId 
										 inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
										 AND NOT EXISTS (Select TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
                                    WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND zup.UserId = @UserId;
									--Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  #Tbl_PortalWisePriceList )

					     END;
                   

                     ---6. If There is no any PriceList having given sku associated to Profile  then check for priceList associated Portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
                  				IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl 
								where NOT Exists (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
				WHERE tspl.SKU = tpwl.SKU))

                         BEGIN
							INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
							SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
							FROM ZnodePriceList AS a INNER JOIN ZnodePriceListPortal AS b ON a.PriceListId = b.PriceListId
								INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId AND  zupu.PortalId = b.PortalId    
								inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
								AND NOT EXISTS (Select TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
								WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) 
								AND EXISTS (SELECT TOP 1 1 FROM @PortalId WHERE Id = b.PortalId) --AND b.PortalId = @PortalId
							    ORDER BY b.Precedence
								;
								--Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  #Tbl_PortalWisePriceList )
                         END;
						 
				--IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl where NOT Exists (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
				--WHERE tspl.SKU = tpwl.SKU))
				--BEGIN
				
				--	INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
				--	SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
				--	FROM ZnodePriceList AS a INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId INNER JOIN ZnodePortalProfile AS c
				--	ON b.ProfileId = c.ProfileId AND  c.IsDefaultAnonymousProfile = 1 INNER JOIN ZnodePortalunit AS zupu ON a.CurrencyId = zupu.CurrencyId
				--	inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
				--	AND NOT EXISTS (Select TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
				--	WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND c.PortalId = @PortalId;
				--END

                 END;
			
             SET @PriceListId = 0;
             -- Check Activation date and expiry date 
             IF EXISTS( SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList)
                 BEGIN
				
                     -- Declare  @d datetime
                     -- SET @d = @GetDate
                     -- Select ISNULL(ActivationDate,@d)  , ISNULL( ExpirationDate,@GetDate ),b.Precedence,* from ZnodePriceList  a inner join ZnodePriceListPortal b on a.PriceListId = b.PriceListId where @d between ISNULL(ActivationDate,@d) 
                     -- and ISNULL(ExpirationDate,@GetDate ) --and a.PriceListId <>  80
                     -- Order by ISNULL(ActivationDate,@d)  , ISNULL( ExpirationDate,@GetDate ) ,  b.Precedence DESC 
                     --	Retrive pricelist wise price
                   INSERT INTO #Tbl_PriceListWisePrice( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize , ExternalId ,Custom1,Custom2,Custom3)
				   SELECT ZP.PriceListId, ZP.SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, ZP.ExternalId,
				   ZPT.Custom1,ZPT.Custom2,ZPT.Custom3
				   FROM [ZnodePrice] AS ZP 
				   INNER JOIN @SKU AS TSKU ON ZP.SKU = TSKU.StringColumn 
				   LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND ZP.PriceListId = ZPT.PriceListId
				   WHERE ZP.PriceListId IN
				   (
					   SELECT TOP 1 PriceListId
					   FROM #Tbl_PortalWisePriceList AS TBPWPL
					   WHERE  TBPWPL.SKU = ZP.SKU
					   ORDER BY Precedence 
				   );
				   

                     -- Check Activation date and expiry date 
                    INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3 )
					   SELECT DISTINCT  PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
					   FROM #Tbl_PriceListWisePrice
					   WHERE @currentUtcDate BETWEEN ActivationDate AND ISNULL(ExpirationDate, @GetDate);
					   
					  
					INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId ,Custom1,Custom2,Custom3)
					   SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
					   FROM #Tbl_PriceListWisePrice
					   WHERE SKU NOT IN(SELECT SKU FROM #TLB_SKUPRICELIST) and ActivationDate is null 
				
                 END;
                     -- Retrive data as per precedance from ZnodePriceListPortal table  
					
             ELSE
                 BEGIN
                     SET @PriceListId =( SELECT TOP 1 PriceListId FROM #Tbl_PortalWisePriceList ORDER BY Precedence  );

                     --Retrive pricelist wise price  
                     INSERT INTO #Tbl_PriceListWisePrice( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize, ExternalId ,Custom1,Custom2,Custom3)
					 SELECT ZP.PriceListId, ZP.SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), 
							ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, zp.ExternalId,Custom1,Custom2,Custom3
					 FROM [ZnodePrice] AS ZP INNER JOIN @SKU AS TSKU ON ZP.SKU = TSKU.StringColumn LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND 
							   ZP.PriceListId = ZPT.PriceListId WHERE ZP.PriceListId = @PriceListId; 

                     -- Check Activation date and expiry date 
					INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId ,Custom1,Custom2,Custom3)
					SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
					FROM #Tbl_PriceListWisePrice WHERE @currentUtcDate BETWEEN ActivationDate AND ISNULL(ExpirationDate, @GetDate);
					
					INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId ,Custom1,Custom2,Custom3)
					SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
					FROM #Tbl_PriceListWisePrice
					WHERE SKU NOT IN ( SELECT SKU FROM #TLB_SKUPRICELIST) and ActivationDate is null;

                 END;
             SELECT DISTINCT SKU,
                      ROUND(RetailPrice, @PriceRoundOff) AS RetailPrice
             FROM #TLB_SKUPRICELIST AS TSPL
                  INNER JOIN ZnodePriceList AS ZPL ON TSPL.PriceListId = ZPL.PriceListId
                  INNER JOIN ZnodeCulture AS ZC ON ZPL.CultureId = ZC.CultureId    
				  LEFT JOIN ZnodeCurrency AS ZCC ON ZC.CurrencyId = ZCC.CurrencyId   
				  --ORDER BY TierQuantity ASC;

				IF OBJECT_ID('TEMPDB..#TLB_SKUPRICELIST') IS NOT NULL
				DROP TABLE #TLB_SKUPRICELIST

				IF OBJECT_ID('TEMPDB..#Tbl_PortalWisePriceList') IS NOT NULL
				DROP TABLE #Tbl_PortalWisePriceList

				IF OBJECT_ID('TEMPDB..#Tbl_PriceListWisePrice') IS NOT NULL
				DROP TABLE #Tbl_PriceListWisePrice

         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
			SET @Status = 0;
			SELECT ERROR_MESSAGE()
			--DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductPricingBySku @SKU = '+@SKU+',@PortalId = '+CAST(@PortalId AS VARCHAR(10))+',@currentUtcDate = '+@currentUtcDate+',@UserId='+CAST(@UserId AS VARCHAR(100))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			--SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			--EXEC Znode_InsertProcedureErrorLog
			--	@ProcedureName = 'Znode_GetPublishProductPricingBySku',
			--	@ErrorInProcedure = @Error_procedure,
			--	@ErrorMessage = @ErrorMessage,
			--	@ErrorLine = @ErrorLine,
			--	@ErrorCall = @ErrorCall;
         END CATCH;
     END;
	 GO 
	 	  