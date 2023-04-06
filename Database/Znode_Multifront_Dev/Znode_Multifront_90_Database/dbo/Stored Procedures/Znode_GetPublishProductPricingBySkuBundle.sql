﻿CREATE PROCEDURE [dbo].[Znode_GetPublishProductPricingBySkuBundle]
(   
	@SKU VARCHAR(MAX),
	@PortalId INT,
	@currentUtcDate VARCHAR(100), -- this date is required for the user date r
	@UserId INT = 0, -- userid is optional
	@ProfileId INT = 0, 
	@IsBundleProduct BIT = 0,
	@ParentSKU VARCHAR(600) = '',
	@PublishProductId TransferId READONLY,	
	@IsDebug BIT = 0
)
AS 
/* 
--Summary: Retrive Price of product FROM pricelist
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
        DECLARE @Tlb_SKU TABLE (SKU VARCHAR(100), SequenceNo INT IDENTITY);

		DECLARE @DefaultLocaleId INT = dbo.FN_GETDEFAULTLocaleId()

		IF @SKU = '' 
		BEGIN 
			INSERT INTO @Tlb_SKU(SKU)
			SELECT SKU
			FROM ZnodePublishProductEntity a with(nolock)
			INNER JOIN @PublishProductId b ON (b.Id = a.ZnodeProductId )
			WHERE LocaleId = @DefaultLocaleId
		END 
		ELSE 
		BEGIN
			INSERT INTO @Tlb_SKU(SKU)
			SELECT Item FROM Dbo.split(@SKU, ',');
		END 

		-- DECLARE #TLB_SKUPRICELIST TABLE
		CREATE TABLE #TLB_SKUPRICELIST
		(
			SKU VARCHAR(100),
			RetailPrice NUMERIC(28, 6),
			SalesPrice NUMERIC(28, 6),
			PriceListId INT,
			TierPrice NUMERIC(28, 6),
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
		(
			PriceListId    INT,
			ActivationDate DATETIME,
			ExpirationDate DATETIME NULL,
			Precedence     INT,
			SKU NVARCHAR(300)
        );
        --Retrive price for respective pricelist   
        CREATE TABLE #Tbl_PriceListWisePriceData 
        (
			PriceListId INT,
			SKU VARCHAR(300),
			SalesPrice NUMERIC(28, 6),
			RetailPrice NUMERIC(28, 6),
			UomId INT,
			UnitSize NUMERIC(28, 6),
			ActivationDate DATETIME,
			ExpirationDate DATETIME NULL,
			TierPrice NUMERIC(28, 6),
			TierQuantity NUMERIC(28, 6),
			TierUomId INT,
			TierUnitSize NUMERIC(28, 6), 
			ExternalId NVARCHAR(2000),
			Custom1 NVARCHAR(MAX),
			Custom2 NVARCHAR(MAX),
			Custom3 NVARCHAR(MAX)
        );
			
	CREATE TABLE #Tbl_SKUWisePriceList(PriceListId INT, SKU NVARCHAR(300))
	 
	IF @IsBundleProduct = 1
	BEGIN
		SELECT ZPAV.PimProductId,ZPAVL.AttributeValue AS Child_SKU, ZPAVL_PAR.AttributeValue AS Parent_SKU,ZPAV_PAR.PimProductId AS Parent_PimProductId
		INTO #BundleProductAssociation
		FROM ZnodePimAttributeValue ZPAV 
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		INNER JOIN ZnodePimProductTypeAssociation ZPPTA ON ZPAV.PimProductId = ZPPTA.PimProductId
		INNER JOIN ZnodePimAttributeValue ZPAV_PAR ON ZPAV_PAR.PimProductId = ZPPTA.PimParentProductId
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL_PAR ON ZPAV_PAR.PimAttributeValueId = ZPAVL_PAR.PimAttributeValueId
		WHERE EXISTS(SELECT * FROM ZnodePimAttribute ZPA WHERE ZPAV.PimAttributeId = ZPA.PimAttributeId AND ZPA.AttributeCode = 'SKU')
		AND EXISTS(SELECT * FROM ZnodePimAttribute ZPA_PAR WHERE ZPAV_PAR.PimAttributeId = ZPA_PAR.PimAttributeId AND ZPA_PAR.AttributeCode = 'SKU')
		AND EXISTS(SELECT SKU FROM @Tlb_SKU SKU WHERE SKU.SKU = ZPAVL.AttributeValue )
	
		INSERT INTO #Tbl_SKUWisePriceList(PriceListId,SKU) 
		SELECT ZP.PriceListId,BPA.Child_SKU 
		FROM ZnodePrice ZP 
		INNER JOIN #BundleProductAssociation BPA ON ZP.SKU = BPA.Parent_SKU
		WHERE ZP.SKU = @ParentSKU
		UNION
		SELECT ZPT.PriceListId,BPA.Child_SKU  
		FROM ZnodePriceTier ZPT 
		INNER JOIN #BundleProductAssociation BPA ON ZPT.SKU = BPA.Parent_SKU
		WHERE ZPT.SKU = @ParentSKU
	END
	ELSE
	BEGIN
		INSERT INTO #Tbl_SKUWisePriceList(PriceListId,SKU) 
		SELECT  PriceListId,SKU FROM ZnodePrice ZP WHERE EXISTS(SELECT SKU FROM @Tlb_SKU SKU WHERE SKU.SKU = ZP.SKU )
		UNION
		SELECT PriceListId,SKU  FROM ZnodePriceTier ZPT WHERE EXISTS(SELECT SKU FROM @Tlb_SKU SKU WHERE SKU.SKU = ZPT.SKU )
	END	
	--1. If userId is null then check for PriceList having sku associated to profile which is associated to Portal having  PortalId and  having higher Precedence and valid ActivationDate and ExpirationDate for PriceList  and SKU also.
    IF @UserId = 0
    BEGIN

		INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
		SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
		FROM ZnodePriceList AS a 
		INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId 
		INNER JOIN ZnodePortalProfile AS c ON b.PortalProfileId = c.PortalProfileID AND  c.IsDefaultAnonymousProfile = 1 INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId 
		INNER JOIN #Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
		WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND c.PortalId = @PortalId
		ORDER BY b.Precedence;
		
			 
		--2. If There is no any PriceList having given sku associated to profile  then check for PriceList associated portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
		IF EXISTS (SELECT top 1 1  FROM #Tbl_SKUWisePriceList tspl WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
					WHERE tspl.SKU = tpwl.SKU))
		BEGIN

			INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
			SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
			FROM ZnodePriceList AS a INNER JOIN ZnodePriceListPortal AS b ON a.PriceListId = b.PriceListId
			INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId   
			INNER JOIN #Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
			AND NOT EXISTS (SELECT TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
			WHERE @CurrentUtcDate BETWEEN a.ActivationDate 
			AND ISNULL(a.ExpirationDate, @GetDate) AND b.PortalId = @PortalId
			ORDER BY b.Precedence
			;				
		END;
    END;
    --3. If userId is not null then check for PriceList having sku associated to User having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
    ELSE
    BEGIN
		INSERT INTO #Tbl_PortalWisePriceList (PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
		SELECT a.PriceListId, ActivationDate,ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
		FROM ZnodePriceList AS a 
		INNER JOIN ZnodePriceListUser AS b ON a.PriceListId = b.PriceListId
		INNER JOIN ZnodePortalunit zupu ON a.CultureId = zupu.CultureId AND zupu.PortalId = @PortalId  
		INNER JOIN #Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
				AND NOT EXISTS (SELECT TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
		WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND b.UserID = @UserId
		ORDER BY b.Precedence ;


		--4. If There is no any PriceList having given sku associated to user  then check for PriceList associated Account having UserId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
		IF EXISTS (SELECT top 1 1  FROM #Tbl_SKUWisePriceList tspl WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
				WHERE tspl.SKU = tpwl.SKU))
		BEGIN

			INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
			SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), c.Precedence,tsw.SKU
			FROM ZnodePriceList AS a INNER JOIN ZnodePriceListAccount AS c ON a.PriceListId = c.PriceListId
				INNER JOIN ZnodeUser AS d ON c.Accountid = d.Accountid INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId   
				AND zupu.PortalId = @PortalId
				INNER JOIN #Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
				AND NOT EXISTS (SELECT TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
			WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND d.UserID = @UserId
			ORDER BY c.Precedence

		END;
            -- 5. If There is no any PriceList having given sku associated to account  then check for PriceList associated Profile having PortalId and having higher   Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
		IF EXISTS (SELECT top 1 1  FROM #Tbl_SKUWisePriceList tspl 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl
			WHERE tspl.SKU = tpwl.SKU))
        BEGIN
            INSERT INTO #Tbl_PortalWisePriceList(PriceListId,ActivationDate,ExpirationDate,Precedence,SKU)
                SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
                FROM ZnodePriceList AS a
                        INNER JOIN ZnodePriceListProfile AS b ON a.PriceListId = b.PriceListId 
						INNER JOIN ZnodePortalProfile AS c ON b.PortalProfileId = c.PortalProfileId  AND c.PortalId = @PortalId 
                        INNER JOIN ZnodePortalunit zupu ON a.CultureId = zupu.CultureId AND zupu.PortalId = @PortalId 
						INNER JOIN #Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
						AND NOT EXISTS (SELECT TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
                WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate)  
				AND EXISTS (SELECT TOP 1 1 FROM dbo.ZnodeUserProfile zup WHERE c.ProfileId = zup.ProfileId AND (IsDefault = 1 OR   @ProfileId <> 0)
				AND (( zup.UserId = @UserId OR  @ProfileId <> 0) AND (ZUP.ProfileId = @ProfileId OR @ProfileId = 0 )))
	
		END;
		---6. If There is no any PriceList having given sku associated to Profile  then check for priceList associated Portal having PortalId and having higher Precedence ActivationDate and ExpirationDate for PriceList and SKU also.
		IF EXISTS (SELECT top 1 1 FROM #Tbl_SKUWisePriceList tspl 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList tpwl WHERE tspl.SKU = tpwl.SKU))
        BEGIN
			INSERT INTO #Tbl_PortalWisePriceList( PriceListId, ActivationDate, ExpirationDate, Precedence,SKU )
			SELECT a.PriceListId, ActivationDate, ISNULL(ExpirationDate, @GetDate), b.Precedence,tsw.SKU
			FROM ZnodePriceList AS a INNER JOIN ZnodePriceListPortal AS b ON a.PriceListId = b.PriceListId
			INNER JOIN ZnodePortalunit AS zupu ON a.CultureId = zupu.CultureId AND  zupu.PortalId = b.PortalId    
			INNER JOIN #Tbl_SKUWisePriceList tsw  ON a.PriceListId = tsw.PriceListId
			AND NOT EXISTS (SELECT TOP 1 1 FROM  #Tbl_PortalWisePriceList tpwl WHERE tpwl.SKU = tsw.SKU )
			WHERE @CurrentUtcDate BETWEEN a.ActivationDate AND ISNULL(a.ExpirationDate, @GetDate) AND b.PortalId = @PortalId
			ORDER BY b.Precedence;
		END;
				
	END;
			
        SET @PriceListId = 0;
        -- Check Activation date and expiry date 
        IF EXISTS( SELECT TOP 1 1 FROM #Tbl_PortalWisePriceList)
        BEGIN

			IF @IsBundleProduct = 1
			BEGIN
				INSERT INTO #Tbl_PriceListWisePriceData( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize , ExternalId ,Custom1,Custom2,Custom3)
				SELECT ZP.PriceListId, TSKU.Child_SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, ZP.ExternalId,
				ZPT.Custom1,ZPT.Custom2,ZPT.Custom3
				FROM [ZnodePrice] AS ZP 
				INNER JOIN #BundleProductAssociation AS TSKU ON (ZP.SKU = TSKU.Parent_SKU )
				LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND ZP.PriceListId = ZPT.PriceListId
				WHERE ZP.SKU = @ParentSKU AND
				ZP.PriceListId IN
				(
					SELECT TOP 1 PriceListId
					FROM #Tbl_PortalWisePriceList AS TBPWPL
					WHERE  TBPWPL.SKU = TSKU.Child_SKU
					ORDER BY Precedence 
				);

			END
			ELSE
			BEGIN
				INSERT INTO #Tbl_PriceListWisePriceData( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize , ExternalId ,Custom1,Custom2,Custom3)
				SELECT ZP.PriceListId, ZP.SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, ZP.ExternalId,
				ZPT.Custom1,ZPT.Custom2,ZPT.Custom3
				FROM [ZnodePrice] AS ZP 
				INNER JOIN @Tlb_SKU AS TSKU ON (ZP.SKU = TSKU.SKU )
				LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND ZP.PriceListId = ZPT.PriceListId
				WHERE ZP.PriceListId IN
				(
					SELECT TOP 1 PriceListId
					FROM #Tbl_PortalWisePriceList AS TBPWPL
					WHERE  TBPWPL.SKU = ZP.SKU
					ORDER BY Precedence 
				);
			END
			-- Check Activation date and expiry date 
            INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3 )
				SELECT DISTINCT  PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
				FROM #Tbl_PriceListWisePriceData
				WHERE @currentUtcDate BETWEEN ActivationDate AND ISNULL(ExpirationDate, @GetDate);
					   
			INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId ,Custom1,Custom2,Custom3)
			SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
			FROM #Tbl_PriceListWisePriceData
			WHERE SKU NOT IN(SELECT SKU FROM #TLB_SKUPRICELIST) and ActivationDate is null 
				
        END;
        -- Retrive data as per precedance FROM ZnodePriceListPortal table  
					
        ELSE
        BEGIN
			SET @PriceListId =( SELECT TOP 1 PriceListId FROM #Tbl_PortalWisePriceList ORDER BY Precedence  );

			IF @IsBundleProduct = 1
			BEGIN
			
				--Retrive pricelist wise price  
				INSERT INTO #Tbl_PriceListWisePriceData( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize, ExternalId ,Custom1,Custom2,Custom3)
				SELECT ZP.PriceListId, TSKU.Child_SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), 
					ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, zp.ExternalId,Custom1,Custom2,Custom3
				FROM [ZnodePrice] AS ZP 
				INNER JOIN #BundleProductAssociation AS TSKU ON ZP.SKU = TSKU.Parent_SKU 
				LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND ZP.PriceListId = ZPT.PriceListId 
				WHERE ZP.SKU = @ParentSKU AND ZP.PriceListId = @PriceListId; 
			END
			ELSE
			BEGIN
				--Retrive pricelist wise price  
				INSERT INTO #Tbl_PriceListWisePriceData( PriceListId, SKU, SalesPrice, RetailPrice, UomId, UnitSize, ActivationDate, ExpirationDate, TierPrice, TierQuantity, TierUomId, TierUnitSize, ExternalId ,Custom1,Custom2,Custom3)
				SELECT ZP.PriceListId, ZP.SKU, ZP.SalesPrice, ZP.RetailPrice, ZP.UomId, ZP.UnitSize, ISNULL(ZP.ActivationDate, @CurrentUtcDate), 
					ISNULL(ZP.ExpirationDate, @GetDate), ZPT.Price, ZPT.Quantity, ZPT.UomId, ZPT.UnitSize, zp.ExternalId,Custom1,Custom2,Custom3
				FROM [ZnodePrice] AS ZP 
				INNER JOIN @Tlb_SKU AS TSKU ON ZP.SKU = TSKU.SKU 	
				LEFT OUTER JOIN ZnodePriceTier AS ZPT ON ZP.SKU = ZPT.SKU AND ZP.PriceListId = ZPT.PriceListId 
				WHERE ZP.PriceListId = @PriceListId; 
			END
				-- Check Activation date and expiry date 
			INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId ,Custom1,Custom2,Custom3)
			SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
			FROM #Tbl_PriceListWisePriceData WHERE @currentUtcDate BETWEEN ActivationDate AND ISNULL(ExpirationDate, @GetDate);
					
			INSERT INTO #TLB_SKUPRICELIST( PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId ,Custom1,Custom2,Custom3)
			SELECT PriceListId, SKU, RetailPrice, SalesPrice, TierPrice, TierQuantity, ExternalId,Custom1,Custom2,Custom3
			FROM #Tbl_PriceListWisePriceData
			WHERE SKU NOT IN ( SELECT SKU FROM #TLB_SKUPRICELIST) and ActivationDate is null;

        END;
		
        SELECT SKU,
            ROUND(RetailPrice, @PriceRoundOff) AS RetailPrice,
            ROUND(SalesPrice, @PriceRoundOff) AS SalesPrice,
            ROUND(TierPrice, @PriceRoundOff) AS TierPrice,
            ROUND(TierQuantity, @PriceRoundOff) AS TierQuantity,
			ZCC.CurrencyCode  AS CurrencyCode,    
            ZC.Symbol AS CurrencySuffix,  ZC.CultureCode,
			TSPL.ExternalId,
			Custom1,Custom2,Custom3
        FROM #TLB_SKUPRICELIST AS TSPL
        INNER JOIN ZnodePriceList AS ZPL ON TSPL.PriceListId = ZPL.PriceListId
        INNER JOIN ZnodeCulture AS ZC ON ZPL.CultureId = ZC.CultureId    
		LEFT JOIN ZnodeCurrency AS ZCC ON ZC.CurrencyId = ZCC.CurrencyId   
		ORDER BY TierQuantity ASC;

END TRY
BEGIN CATCH
    DECLARE @Status BIT ;
	SET @Status = 0;
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductPricingBySkuBundle @SKU = '+@SKU+',@PortalId = '+CAST(@PortalId AS VARCHAR(10))+',@currentUtcDate = '+@currentUtcDate+',@UserId='+CAST(@UserId AS VARCHAR(100))+',@IsBundleProduct='+CAST(@UserId AS VARCHAR(100))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetPublishProductPricingBySkuBundle',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH;
END;