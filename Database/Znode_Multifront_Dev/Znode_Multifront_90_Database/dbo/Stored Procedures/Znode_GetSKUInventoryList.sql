CREATE PROCEDURE [dbo].[Znode_GetSKUInventoryList]
(   @WhereClause VARCHAR(1000),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT,
    @LocaleId    INT           = 1)
AS 
    /*
    Summary : this procedure is used to Get the inventory list by sku 
    Unit Testing 
     EXEC Znode_GetSKUInventoryList  '' ,@RowsCount= 1,@PageNo= 1 ,@Rows = 100
     SELECT * FROM ZnodePublishProduct WHERE PimProductid  = 4
    */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 CREATE TABLE #TBL_InventoryList  (PimProductId int,InventoryId INT ,WarehouseId INT ,WarehouseCode NVARCHAR(100),WarehouseName VARCHAR(100),SKU  VARCHAR(300)
			 ,Quantity NUMERIC (28,6),ReOrderLevel NUMERIC (28,6),BackOrderQuantity NUMERIC(28,6),BackOrderExpectedDate Datetime,IsDownloadable bit default 0,ProductName NVARCHAR(max),ModifiedByName NVARCHAR(max),ModifiedDate DATETIME,RowId INT,CountNo INT);

             DECLARE @DefaultLocaleId VARCHAR(100)= Dbo.Fn_GetDefaultValue('Locale');
             
             IF  OBJECT_ID('tempdb..#TBL_AttributeVAlue') is not null
			 BEGIN 
				DROP TABLE #TBL_AttributeVAlue
			 END 	

			 CREATE TABLE #TBL_AttributeVAlue (PimProductId int,	PimAttributeId int,	AttributeValue VARCHAR(1000),AttributeCode	VARCHAR(300),LocaleId int, IsDownloadable bit not null  default 0,ModifiedByName NVARCHAR(max),ModifiedDate DATETIME, Quantity INT )

			 DECLARE @PimAttributeSKUId INT = dbo.FN_GetProductSKUAttributeid()
			 ,@PimAttributeProductNameId INT = dbo.FN_GetProductNameAttributeid()

			 if @DefaultLocaleId <> @LocaleId
			 begin
				 INSERT INTO #TBL_AttributeValue(PimProductId, PimAttributeId, AttributeValue, AttributeCode, LocaleId)
				 SELECT VI.PimProductId,VI.PimAttributeId,VI2.AttributeValue,Case when VI.PimAttributeId =@PimAttributeSKUId then 'SKU'
						Else 'ProductName' END  AttributeCode,VI2.LocaleId --,COUNT(*)Over(Partition By VI.PimProductId,VI.PimAttributeId ORDER BY VI.PimProductId,VI.PimAttributeId  ) RowId
				 FROM  ZnodePimAttributeValue  VI 
				 INNER JOIN ZnodePimAttributeValueLocale VI2 ON (VI.PimAttributeValueId = VI2.PimAttributeValueId )
				 WHERE ( LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId )
				 AND  VI.PimAttributeId IN ( @PimAttributeSKUId,@PimAttributeProductNameId)	
			end
			else
			begin
				INSERT INTO #TBL_AttributeValue(PimProductId, PimAttributeId, AttributeValue, AttributeCode, LocaleId)
				SELECT VI.PimProductId,VI.PimAttributeId,VI2.AttributeValue,Case when VI.PimAttributeId =@PimAttributeSKUId then 'SKU'
						Else 'ProductName' END  AttributeCode,VI2.LocaleId --,COUNT(*)Over(Partition By VI.PimProductId,VI.PimAttributeId ORDER BY VI.PimProductId,VI.PimAttributeId  ) RowId
				 FROM  ZnodePimAttributeValue  VI 
				 INNER JOIN ZnodePimAttributeValueLocale VI2 ON (VI.PimAttributeValueId = VI2.PimAttributeValueId )
				 WHERE (LocaleId = @DefaultLocaleId )
				 AND  VI.PimAttributeId IN ( @PimAttributeSKUId,@PimAttributeProductNameId)	
			end


			Update a  	
			SET a.IsDownloadable=1	  	
				,a.Quantity = (SELECT COUNT(1) FROM ZnodePimDownloadableProductKey m WHERE m.PimDownloadableProductId= b.PimDownloadableProductId AND m.IsUsed = 0 )        		
			From #TBL_AttributeVAlue a  	
			INNER JOIN ZnodePimDownloadableProduct b on  a.AttributeValue=b.SKU	
			 WHERE a.AttributeCode = 'SKU'  

			SELECT  CTE.PimProductId , CTEI.AttributeValue ProductName,ZW.WarehouseCode,ZW.WarehouseName , CTEI.LocaleId,SKU,SPN.InventoryId,SPN.WarehouseId
				,CASE WHEN cte.IsDownloadable   = 1  THEN cte.Quantity ELSE  SPN.Quantity END Quantity,SPN.ReOrderLevel,SPN.BackOrderQuantity,SPN.BackOrderExpectedDate,
				cte.IsDownloadable,SPN.ModifiedDate, ZVGDI.UserName as ModifiedByName
			into #CTE_InventoryListWithSKU
			FROM #TBL_AttributeVAlue CTE
			INNER JOIN #TBL_AttributeVAlue CTEI ON (CTEI.PimProductId = CTE.Pimproductid 
									AND CTEI.AttributeCode = 'ProductName' )
			INNER JOIN ZnodeInventory  SPN ON (SPN.SKU  = CTE.AttributeValue)
			LEFT JOIN ZnodeWarehouse ZW ON (ZW.WarehouseId = SPN.WarehouseId)
			LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGDI ON (ZVGDI.UserId = SPN.ModifiedBy) 
			WHERE CTE.AttributeCode = 'SKU' 		
					
			 SET @SQL = '
					SELECT PimProductId, InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,BackOrderQuantity,BackOrderExpectedDate,IsDownloadable,ProductName,ModifiedDate,ModifiedByName
					,'+dbo.Fn_GetPagingRowId(@Order_BY,'InventoryId DESC')+',Count(*)Over() CountNo 
					into #CTE_ListDetailForPaging
					FROM #CTE_InventoryListWithSKU
					WHERE 1=1 
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				
				SELECT PimProductId,InventoryId,WarehouseId,WarehouseCode,WarehouseName,dbo.Fn_Trim(SKU)SKU,Quantity,ReOrderLevel,BackOrderQuantity,BackOrderExpectedDate,IsDownloadable,dbo.Fn_Trim(ProductName)ProductName,ModifiedDate,ModifiedByName,RowId,CountNo
				FROM #CTE_ListDetailForPaging 
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)



				INSERT INTO #TBL_InventoryList(PimProductId,InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,BackOrderQuantity,BackOrderExpectedDate,IsDownloadable,ProductName,ModifiedDate,ModifiedByName,RowId,CountNo)
				EXEC (@SQL);

            SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM #TBL_InventoryList), 0);

            SELECT PimProductId,InventoryId,WarehouseId,WarehouseCode,WarehouseName,SKU,Quantity,ReOrderLevel,BackOrderQuantity,BackOrderExpectedDate,IsDownloadable,ProductName,ModifiedDate,ModifiedByName
			FROM #TBL_InventoryList;
         
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSKUInventoryList @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSKUInventoryList',
				@ErrorInProcedure = 'Znode_GetSKUInventoryList',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;