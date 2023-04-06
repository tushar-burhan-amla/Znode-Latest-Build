CREATE PROCEDURE [dbo].[Znode_GetSKUInventory]
(   @WhereClause VARCHAR(1000),
    @Rows        INT           = 1000,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = NULL,
    @RowsCount   INT OUT,
    @LocaleId    INT           = 1)
AS 
   /* 
    Summary : this procedure is used to Get the inventory list by sku 
    Unit Testing 
     EXEC [Znode_GetSKUInventory]  @WhereClause = '',@RowsCount =1
    
  */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_SKUInventory TABLE (InventoryId INT,WarehouseId INT,WarehouseCode VARCHAR(100), WarehouseName VARCHAR(100),SKU VARCHAR(300),Quantity NUMERIC(28,6),ReOrderLevel NUMERIC(28,6),ProductName NVARCHAR(MAX),RowId INT,CountNo INT)

             DECLARE @DefaultLocaleId VARCHAR(100)= (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale')

             SET @SQL = '
				WITH ZnodeInvenroyList AS 
				(
					SELECT ZI.InventoryId,ZI.WarehouseId,ZW.WarehouseCode,ZW.WarehouseName,ZI.SKU,ZI.Quantity,ZI.ReOrderLevel
					FROM ZnodeInventory ZI 
					INNER JOIN ZnodeWarehouse ZW ON (ZW.WarehouseId = ZI.WarehouseId)  				
							
				)
				, InventoryListFindProduct AS 
				(
				   SELECT ZIL.*,zavl.AttributeValue ProductName,LocaleId , ZA.AttributeCode  
				   FROM ZnodePimAttributeValue  ZAV
				   INNER JOIN ZnodePimAttributeValueLocale ZAVL on (ZAVL.PimAttributeValueId = ZAV.PimAttributeValueId and
				   ZAVL.LocaleId IN ('+@DefaultLocaleId+','+CAST(@LocaleId AS VARCHAR(100))+')	)
				   INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZAV.PimAttributeId ) 
				   INNER JOIN ZnodeInvenroyList ZIL ON ( ZIL.SKU = ZAVL.AttributeValue )
				   AND ZA.AttributeCode IN (''SKU'',''ProductName'')
				   	
				)
				, InventoryListProductNameWithLocale AS 
				( SELECT InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,ProductName
				  FROM InventoryListFindProduct 
				  WHERE LocaleId =  '+CAST(@LocaleId AS VARCHAR(100))+'
				  AND AttributeCode = ''ProductName'' ) 

				, InventoryListWithSKU AS 
				(
				SELECT InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,ProductName 
				FROM InventoryListProductNameWithLocale pl
				UNION ALL
				SELECT InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,ProductName
				FROM InventoryListFindProduct ZL
				WHERE LocaleId =  '+@DefaultLocaleId+'
				AND AttributeCode = ''ProductName'' 
				AND NOT EXISTS (SELECT TOP 1 1 FROM InventoryListProductNameWithLocale ZS WHERE ZS.SKU = ZL.SKU AND ZS.InventoryId = ZL.InventoryId)
				)
                
				, ListDetailForPaging AS 
				(  
				SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'SKU DESC,InventoryId DESC')+',Count(*)Over() CountNo
				FROM InventoryListWithSKU
				WHERE 1=1
			    '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				)
				
				SELECT InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,ProductName,RowId,CountNo
				FROM ListDetailForPaging
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			
			
			INSERT INTO @TBL_SKUInventory (InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,ProductName,RowId,CountNo)
		    EXEC(@SQL)

			SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_SKUInventory),0)

			SELECT InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,ProductName 
			FROM @TBL_SKUInventory
             
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSKUInventory @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetSKUInventory',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
         END CATCH;
     END;