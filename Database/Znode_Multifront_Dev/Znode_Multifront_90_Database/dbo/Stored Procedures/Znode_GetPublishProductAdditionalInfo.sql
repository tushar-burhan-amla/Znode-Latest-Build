
CREATE PROCEDURE [dbo].[Znode_GetPublishProductAdditionalInfo](
       @SKU            VARCHAR(300) ,
       @PortalId       INT ,
	   -- @currentUtcDate date is required for the user date 
       @currentUtcDate VARCHAR(100) 
)
AS
/*
Summary: This Procedure is used to get Publish Product details 
         - How much quantity is present in stock,RetailPrice and Salesprice of a publish product
Unit Testing:

 EXEC Znode_GetPublishProductAdditionalInfo  @sku = 'st3245' ,@portalId = 1,@currentUtcDate ='2017-01-09 12:37:43.363'
-- exec sp_executesql N'Znode_GetPublishProductAdditionalInfo @sku,@portalId,@currentUtcDate',N'@sku nvarchar(7),@portalId int,@currentUtcDate nvarchar(10)',@sku=N'pu789709',@portalId=1,@currentUtcDate=N'02-15-2017'

*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TLB_SKUPRICELIST TABLE 
			 (  QuantityOnHand NUMERIC(28 , 6),
				ReOrderLevel   NUMERIC(28 , 6),
				RetailPrice    NUMERIC(28 , 6),
				SalesPrice     NUMERIC(28 , 6)
			 );
             DECLARE @TLB_SKUSumInventory TABLE 
			 (	SKU            VARCHAR(600) ,
				QuantityOnHand NUMERIC(28 , 6) ,
				ReOrderLevel   NUMERIC(28 , 6) ,
				PortalId       INT
			 );
             DECLARE @TBL_AllwareHouseToportal TABLE 
			 (	WarehouseId       INT ,
				PortalId          INT ,
				PortalWarehouseId INT
			 );

             INSERT INTO @TBL_AllwareHouseToportal
             SELECT ZPw.WarehouseId , zp.PortalId , zpw.PortalWarehouseId FROM [dbo].ZnodePortal AS zp INNER JOIN [ZnodePortalWarehouse] AS zpw 
			 ON ( zpw.PortalId = zp.PortalId AND zp.PortalId = @PortalId );

             INSERT INTO @TBL_AllwareHouseToportal
             SELECT DISTINCT zpaw.WarehouseId , @PortalId AS PortalId , zpaw.PortalWarehouseId
             FROM [dbo].[ZnodePortalAlternateWarehouse] AS zpaw INNER JOIN @TBL_AllwareHouseToportal AS TAHT ON ( zpaw.PortalWarehouseId = TAHT.PortalWarehouseId );
					
             INSERT INTO @TLB_SKUSumInventory
             SELECT ZI.SKU ,SUM(ISNULL(ZI.Quantity,0)) QuantityOnHand,SUM(ISNULL(ZI.ReOrderLevel,0)) ReOrderLevel , zpw.PortalId 
             FROM  @TBL_AllwareHouseToportal zpw INNER JOIN [dbo].[ZnodeInventory] ZI ON (ZI.WarehouseId = zpw.WarehouseId) GROUP BY ZI.SKU, zpw.PortalId 

             INSERT INTO @TLB_SKUPRICELIST
             SELECT QuantityOnHand , ReOrderLevel , ISNULL(zp.RetailPrice , 0) AS RetailPrice , ISNULL(zp.SalesPrice , 0) AS SalesPrice
             FROM @TLB_SKUSumInventory AS zpw LEFT JOIN [dbo].[ZnodePrice] AS zp ON ( zp.SKU = zpw.SKU AND EXISTS ( SELECT TOP 1 1 FROM [ZnodePriceListPortal] AS zplp
             WHERE zplp.PortalId = zpw.portalid AND EXISTS ( SELECT TOP 1 1 FROM [ZnodePriceList] AS zpl WHERE zpl.PriceListId = zplp.PriceListId AND
             CAST(@currentUtcDate AS DATE) BETWEEN CAST(zpl.ActivationDate AS DATE) AND CAST(zpl.ExpirationDate AS DATE) AND zp.PriceListId = zpl.PriceListId)) 
			 AND CAST(@currentUtcDate AS DATE) BETWEEN CAST(zp.ActivationDate AS DATE) AND CAST(zp.ExpirationDate AS DATE) );
             
			 -- Use table for default Display 0 if record is empty
			 IF NOT EXISTS ( SELECT TOP 1 1 FROM @TLB_SKUPRICELIST)
                 BEGIN  	  
                     SELECT QuantityOnHand , ReOrderLevel , RetailPrice , SalesPrice FROM @TLB_SKUPRICELIST                     
                     UNION ALL
                     SELECT 0 AS QuantityOnHand, 0 AS ReOrderLevel, 0 AS RetailPrice, 0 AS SalesPrice;
                 END;
             ELSE
                 BEGIN
                     SELECT QuantityOnHand , ReOrderLevel , RetailPrice , SalesPrice FROM @TLB_SKUPRICELIST;                    
                 END;
				
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductAdditionalInfo @SKU ='+@SKU+', @PortalId = '+CAST(@PortalId AS VARCHAR(10))+',@currentUtcDate = '+@currentUtcDate+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPublishProductAdditionalInfo',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;