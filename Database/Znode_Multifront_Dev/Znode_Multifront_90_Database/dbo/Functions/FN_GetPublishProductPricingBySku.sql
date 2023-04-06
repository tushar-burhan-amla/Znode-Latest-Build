﻿CREATE  FUNCTION  [dbo].[FN_GetPublishProductPricingBySku]  
(     
    @SKU              VARCHAR(MAX),  
    @PortalId         INT,  
    @currentUtcDate   VARCHAR(100), -- this date is required for the user date r  
    @UserId           INT          = 0, -- userid is optional   
    @PublishProductId TransferId READONLY
 )  
 RETURNS  @tbl_ProductPricingSkuOrder TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
						TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000),CultureCode varchar(2000), ExternalId NVARCHAR(2000))
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
      
    DECLARE @GetDate DATETIME = dbo.Fn_GetDate();  
             DECLARE @Tlb_SKU TABLE  
             (SKU        VARCHAR(100),  
              SequenceNo INT IDENTITY  
             );  
  
     DECLARE @DefaultLocaleId INT = dbo.FN_GETDEFAULTLocaleId()  
  
    IF @SKU = ''   
    BEGIN   
     INSERT INTO @Tlb_SKU(SKU)  
      SELECT (SELECT ''+SKU FOR XML PATH(''))   
     FROM ZnodePublishProductDetail a  
     INNER JOIN @PublishProductId b ON (b.Id = a.PublishProductId )  
     WHERE LocaleId = @DefaultLocaleId  
  
  
    END   
    ELSE   
    BEGIN  
      INSERT INTO @Tlb_SKU(SKU)  
                    SELECT Item  
                    FROM Dbo.split(@SKU, ',');  
       
  
    END   
  
             
             DECLARE @TLB_SKUPRICELIST TABLE  
             (SKU          VARCHAR(100),  
              RetailPrice  NUMERIC(28, 6),  
              SalesPrice   NUMERIC(28, 6),  
              PriceListId  INT,  
              TierPrice    NUMERIC(28, 6),  
              TierQuantity NUMERIC(28, 6),  
     ExternalId NVARCHAR(2000)  
             );  
             DECLARE @PriceListId INT, @PriceRoundOff INT;  
             SELECT @PriceRoundOff = CONVERT( INT, FeatureValues)  
             FROM ZnodeGlobalSetting  
             WHERE FeatureName = 'PriceRoundOff';  
    
             --Retrive portal wise pricelist    
             DECLARE @Tbl_PortalWisePriceList TABLE  
             (PriceListId    INT,  
              ActivationDate DATETIME,  
              ExpirationDate DATETIME NULL,  
              Precedence     INT,  
     SKU NVARCHAR(300)  
             );  
             --Retrive price for respective pricelist     
             DECLARE @Tbl_PriceListWisePrice TABLE  
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
      ExternalId NVARCHAR(2000)  
             );  
    DECLARE @Tbl_SKUWisePriceList TABLE (PriceListId INT, SKU NVARCHAR(300))  
  
    insert into @Tbl_SKUWisePriceList(PriceListId,SKU)   
    SELECT  PriceListId,SKU from ZnodePrice where (SELECT ''+SKU FOR XML PATH('')) in (Select SKU from @Tlb_SKU )  
    Union  
    SELECT PriceListId,SKU  from ZnodePriceTier where (SELECT ''+SKU FOR XML PATH('')) in (Select SKU from @Tlb_SKU )  
      
    --1. If userId is null then check for PriceList having sku associated to profile which is associated to Portal having  PortalId and  having higher Precedence and valid ActivationDate and ExpirationDate for PriceList  and SKU also.  
            IF @UserId = 0  
                 BEGIN  
     INSERT INTO @Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )  
     SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU  
     FROM ZnodePriceList AS a INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId INNER JOIN ZnodePortalProfile AS c  
      ON b.PortalProfileId = c.PortalProfileID AND  c.IsDefaultAnonymousProfile = 1 INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId 
      inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
     WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND c.PortalId = @PortalId  
     ORDER BY b.Precedence;  
    
      
                     --2. If There is no any PriceList having given sku associated to profile  then check for PriceList associated portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.  
   IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl where NOT Exists (SELECT TOP 1 1 FROM @Tbl_PortalWisePriceList tpwl  
    WHERE tspl.SKU = tpwl.SKU))  
                         BEGIN  
       INSERT INTO @Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )  
       SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU  
       FROM ZnodePriceList AS a INNER JOIN ZnodePriceListPortal AS b ON a.PriceListId = b.PriceListId  
       INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId   
       inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
       AND NOT EXISTS (Select TOP 1 1 FROM  @Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )  
       WHERE @CurrentUtcDate BETWEEN a.ActivationDate   
       AND ISNULL(a.ExpirationDate, @GetDate) AND b.PortalId = @PortalId  
       ORDER BY b.Precedence  
       ;  
       --Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  @Tbl_PortalWisePriceList )  
        
                         END;  
                 END;  
                     --3. If userId is not null then check for PriceList having sku associated to User having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.  
             ELSE  
                 BEGIN  
       
                     INSERT INTO @Tbl_PortalWisePriceList (PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )  
                            SELECT a.PriceListId, ActivationDate,ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU  
                            FROM ZnodePriceList AS a INNER JOIN ZnodePriceListUser AS b ON a.PriceListId = b.PriceListId  
                                 INNER JOIN ZnodePortalunit zupu ON a.CultureId = zupu.CultureId AND zupu.PortalId = @PortalId  
         inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
         AND NOT EXISTS (Select TOP 1 1 FROM  @Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )  
                            WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND b.UserID = @UserId  
       ORDER BY b.Precedence ;  
  
                --4. If There is no any PriceList having given sku associated to user  then check for PriceList associated Account having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.  
    IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl where NOT Exists (SELECT TOP 1 1 FROM @Tbl_PortalWisePriceList tpwl  
    WHERE tspl.SKU = tpwl.SKU))  
      BEGIN  
       INSERT INTO @Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )  
           SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), c.Precedence,tsw.SKU  
           FROM ZnodePriceList AS a INNER JOIN ZnodePriceListAccount AS c ON a.PriceListId = c.PriceListId  
          INNER JOIN ZnodeUser AS d ON c.Accountid = d.Accountid INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId  
          AND zupu.PortalId = @PortalId  
          inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
          AND NOT EXISTS (Select TOP 1 1 FROM  @Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )  
           WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND d.UserID = @UserId  
         ORDER BY c.Precedence  
       --Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  @Tbl_PortalWisePriceList )  
       END;  
                     -- 5. If There is no any PriceList having given sku associated to account  then check for PriceList associated Profile having PortalId and having higher   Precedence ActivationDate and ExpirationDate for PriceList and SKU also.  
    IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl   
    where NOT Exists (SELECT TOP 1 1 FROM @Tbl_PortalWisePriceList tpwl  
    WHERE tspl.SKU = tpwl.SKU))  
  
                         BEGIN  
                             INSERT INTO @Tbl_PortalWisePriceList(PriceListId,ActivationDate,ExpirationDate,Precedence,SKU)  
                                    SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU  
                                    FROM ZnodePriceList AS a  
                                         INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId   
           INNER JOIN ZnodePortalProfile AS c ON b.PortalProfileId = c.PortalProfileId  AND c.PortalId = @PortalId   
                                         INNER JOIN dbo.ZnodeUserProfile zup ON c.ProfileId = zup.ProfileId AND IsDefault = 1  
                                         INNER JOIN ZnodePortalunit zupu ON a.CultureId = zupu.CultureId AND zupu.PortalId = @PortalId  
           inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
           AND NOT EXISTS (Select TOP 1 1 FROM  @Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )  
                                    WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND zup.UserId = @UserId;  
         --Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  @Tbl_PortalWisePriceList )  
  
          END;  
                     
  
                     ---6. If There is no any PriceList having given sku associated to Profile  then check for priceList associated Portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.  
                      IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl   
        where NOT Exists (SELECT TOP 1 1 FROM @Tbl_PortalWisePriceList tpwl  
    WHERE tspl.SKU = tpwl.SKU))  
  
                         BEGIN  
       INSERT INTO @Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )  
       SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU  
       FROM ZnodePriceList AS a INNER JOIN ZnodePriceListPortal AS b ON a.PriceListId = b.PriceListId  
        INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId AND  zupu.PortalId = b.PortalId  
        inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
        AND NOT EXISTS (Select TOP 1 1 FROM  @Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )  
        WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND b.PortalId = @PortalId  
           ORDER BY b.Precedence  
        ;  
        --Delete from @Tbl_SKUWisePriceList where PriceListId in (Select PriceListId from  @Tbl_PortalWisePriceList )  
                         END;  
         
    --IF Exists (Select top 1 1  FROM @Tbl_SKUWisePriceList tspl where NOT Exists (SELECT TOP 1 1 FROM @Tbl_PortalWisePriceList tpwl  
    --WHERE tspl.SKU = tpwl.SKU))  
    --BEGIN  
      
    -- INSERT INTO @Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )  
    -- SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU  
    -- FROM ZnodePriceList AS a INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId INNER JOIN ZnodePortalProfile AS c  
    -- ON b.ProfileId = c.ProfileId AND  c.IsDefaultAnonymousProfile = 1 INNER JOIN ZnodePortalunit AS zupu ON a.CurrencyId = zupu.CurrencyId  
    -- inner join @Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId  
    -- AND NOT EXISTS (Select TOP 1 1 FROM  @Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )  
    -- WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND c.PortalId = @PortalId;  
    --END  
  
                 END;  
     
             SET @PriceListId = 0;  
             -- Check Activation date and expiry date   
             IF EXISTS( SELECT TOP 1 1 FROM @Tbl_PortalWisePriceList)  
                 BEGIN  
      
                     -- Declare  @d datetime  
                     -- SET @d = @GetDate  
                     -- Select ISNULL(ActivationDate,@d)  , ISNULL( ExpirationDate,@GetDate ),b.Precedence,* from ZnodePriceList  a inner join ZnodePriceListPortal b on a.PriceListId = b.PriceListId where @d between ISNULL(ActivationDate,@d)   
                     -- and ISNULL(ExpirationDate,@GetDate ) --and a.PriceListId <>  80  
                     -- Order by ISNULL(ActivationDate,@d)  , ISNULL( ExpirationDate,@GetDate ) ,  b.Precedence DESC   
                     -- Retrive pricelist wise price  
                   INSERT INTO @Tbl_PriceListWisePrice( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize , ExternalId )  
       SELECT ZP.PriceListId, ZP.SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, ZP.ExternalId  
       FROM [ZnodePrice] AS ZP   
       INNER JOIN @Tlb_SKU AS TSKU ON (SELECT ''+ZP.SKU FOR XML PATH ('')) = TSKU.SKU   
       LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND ZP.PriceListId = ZPT.PriceListId  
       WHERE ZP.PriceListId IN  
       (  
        SELECT TOP 1 PriceListId  
        FROM @Tbl_PortalWisePriceList AS TBPWPL  
        WHERE  TBPWPL.SKU = ZP.SKU  
        ORDER BY Precedence   
       );  
          
  
                     -- Check Activation date and expiry date   
                    INSERT INTO @TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId )  
        SELECT DISTINCT  PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId  
        FROM @Tbl_PriceListWisePrice  
        WHERE @currentUtcDate BETWEEN ActivationDate AND ISNULL(ExpirationDate, @GetDate);  
          
         
     INSERT INTO @TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId )  
        SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId  
        FROM @Tbl_PriceListWisePrice  
        WHERE SKU NOT IN(SELECT SKU FROM @TLB_SKUPRICELIST) and ActivationDate is null   
      
                 END;  
                     -- Retrive data as per precedance from ZnodePriceListPortal table    
       
             ELSE  
                 BEGIN  
                     SET @PriceListId =( SELECT TOP 1 PriceListId FROM @Tbl_PortalWisePriceList ORDER BY Precedence  );  
  
                     --Retrive pricelist wise price    
                     INSERT INTO @Tbl_PriceListWisePrice( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize, ExternalId )  
      SELECT ZP.PriceListId, ZP.SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate),   
       ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, zp.ExternalId  
      FROM [ZnodePrice] AS ZP INNER JOIN @Tlb_SKU AS TSKU ON ZP.SKU = TSKU.SKU LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND   
          ZP.PriceListId = ZPT.PriceListId WHERE ZP.PriceListId = @PriceListId;   
  
                     -- Check Activation date and expiry date   
     INSERT INTO @TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId )  
     SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId  
     FROM @Tbl_PriceListWisePrice WHERE @currentUtcDate BETWEEN ActivationDate AND ISNULL(ExpirationDate, @GetDate);  
       
     INSERT INTO @TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId )  
     SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId  
     FROM @Tbl_PriceListWisePrice  
     WHERE SKU NOT IN ( SELECT SKU FROM @TLB_SKUPRICELIST) and ActivationDate is null;  
  
                 END;  
INSERT INTO @tbl_ProductPricingSkuOrder
             SELECT DISTINCT SKU,  
                    ROUND(RetailPrice, @PriceRoundOff) AS RetailPrice,  
                    ROUND(SalesPrice, @PriceRoundOff) AS SalesPrice,  
                    NULL AS TierPrice,  
                   NULL AS TierQuantity,  
					ZCC.CurrencyCode  AS CurrencyCode,    
                    ZC.Symbol AS CurrencySuffix,  ZC.CultureCode, 
     TSPL.ExternalId  
             FROM @TLB_SKUPRICELIST AS TSPL  
                  INNER JOIN ZnodePriceList AS ZPL ON TSPL.PriceListId = ZPL.PriceListId  
                  INNER JOIN ZnodeCulture AS ZC ON ZPL.CultureId = ZC.CultureId    
				  LEFT JOIN ZnodeCurrency AS ZCC ON ZC.CurrencyId = ZCC.CurrencyId   
      ORDER BY TierQuantity ASC;  
        RETURN 
  END;