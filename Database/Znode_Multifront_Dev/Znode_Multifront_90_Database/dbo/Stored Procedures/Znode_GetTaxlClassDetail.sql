CREATE PROCEDURE [dbo].[Znode_GetTaxlClassDetail]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(1000) = '',
    @RowsCount   INT OUT,
    @LocaleId    INT           = 1,
    @TaxClassId  INT		   = 0  ,
    @IsDebug bit = 0 )
AS 
   /* 
     Summary : get tax with product name and other details 
     Sequence For Delete Data 
	 SELECT * FROM ZnodeTaxClassSKU
     EXEC Znode_GetTaxlClassDetail  '' , @RowsCount = 1  ,@TaxClassId = -1 

   */
     BEGIN
         BEGIN TRY
           SET NOCOUNT ON;
           
		    DECLARE @PimProductIds TransferId,--NVARCHAR(max)= '', 
					@OutPimProductIds NVARCHAR(max)= '',
					@PimAttributeId NVARCHAR(max)=''
			 DECLARE @pimSkuAttributeId VARCHAR(50) = [dbo].[Fn_GetProductSKUAttributeId] ()
			 DECLARE @PimProductNameAttributeId VARCHAR(50) = [dbo].[Fn_GetProductNameAttributeId]()
			 DECLARE @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId()
			 ,@IsProductNotIn INT = 0 
			 DECLARE @TransferPimProductId TransferId 
			 DECLARE @ProductIdTable TABLE
             (PimProductId INT,
              CountId      INT,
              RowId        INT IDENTITY(1,1)
             );
			  DECLARE @TBL_AttributeDetails AS TABLE
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT
             );
               IF @TaxClassId <> 0
                 BEGIN
				insert into @TransferPimProductId 
				  SELECT DISTINCT PimProductId 
					 FROM  [dbo].[View_PimProductAttributeValueLocale] VIMP
					 INNER JOIN ZnodeTaxClassSKU  ZP ON (dbo.FN_trim(Zp.SKU) = dbo.FN_Trim(VIMP.AttributeValue) )
					 WHERE VIMP.PimAttributeid = @pimSkuAttributeId
					 AND VIMP.LocaleId = @DefaultLocaleId
					 AND (ZP.TaxClassId = @TaxClassId OR @TaxClassId = -1 )
				UNION ALL 
				SELECT -1 
					--If @IsDebug =1 
					--Begin
					--   select * from @TransferPimProductId
					--END 
      --          SET @PimProductIds = SUBSTRING(( SELECT DISTINCT ','+CAST(PimProductId AS VARCHAR(50))
					 --FROM  [dbo].[View_PimProductAttributeValueLocale] VIMP
					 --INNER JOIN ZnodeTaxClassSKU  ZP ON (dbo.FN_trim(Zp.SKU) = dbo.FN_Trim(VIMP.AttributeValue) )
					 --WHERE VIMP.PimAttributeid = @pimSkuAttributeId
					 --AND VIMP.LocaleId = @DefaultLocaleId
					 --AND (ZP.TaxClassId = @TaxClassId OR @TaxClassId = -1 )
					 --FOR XML PATH('')),2,8000)
					
				SET @Rows = CASE WHEN @TaxClassId = -1  THEN  214748368  ELSE @Rows END 
                  
			  END;
          DECLARE @AttributeCode NVARCHAR(1000), @SQL NVARCHAR(max)=''
		 DECLARE  @ProductListIdRTR TransferId
	 DECLARE @TAb Transferid 
	 DECLARE @tBL_mainList TABLE (Id INT,RowId INT)
	 --	IF @PimProductId <> ''  OR   @IsCallForAttribute=1
		--BEGIN 
	 --SET @IsProductNotIn = CASE WHEN @IsProductNotIn = 0 THEN 1  
		--			 WHEN @IsProductNotIn = 1 THEN 0 END 
		--END 
	 INSERT INTO @ProductListIdRTR
	 EXEC Znode_GetProductList  1,@TransferPimProductId
	 
	 IF CAST(@WhereClause AS NVARCHAR(max))<> N''
	 BEGIN 
	 
	  SET @SQL = 'SELECT PimProductId FROM ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))

	  EXEC Znode_GetFilterPimProductId @WhereClause,@ProductListIdRTR,@localeId
	  
      INSERT INTO @TAB 
	  EXEC (@SQL)
	 
	 END 

	 IF EXISTS (SELECT Top 1 1 FROM @TAb ) OR CAST(@WhereClause AS NVARCHAR(max)) <> N''
	 BEGIN 
	 
		 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
		 SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
		 INSERT INTO @TBL_MainList(id,RowId)
		 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId
	 
	 END 
	 ELSE 
	 BEGIN
	    -- SELECT @order_by,@AttributeCode
		 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
		 
		 SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
		 INSERT INTO @TBL_MainList(id,RowId)
		 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId 
	 END 

			 INSERT INTO @ProductIdTable (PimProductId)              
			 SELECT Id  
			 FROM @TBL_MainList
				  	           
             --SET @PimProductIds = SUBSTRING((SELECT ','+CAST(PimProductId AS VARCHAR(100)) FROM @ProductIdTable FOR XML PATH('')), 2, 4000);
			 INSERT INTO @PimProductIds ( Id )
			 SELECT PimProductId FROM @ProductIdTable

             SET @PimAttributeId = @pimSkuAttributeId +','+@PimProductNameAttributeId;
				

             INSERT INTO @TBL_AttributeDetails
             (PimProductId,
              AttributeValue,
              AttributeCode,
              PimAttributeId
             )
             EXEC Znode_GetProductsAttributeValue
                  @PimProductIds,
                  @PimAttributeId,
                  @localeId;

		    ;With Cte_pimProductDetails AS
			(
			  SELECT PimProductId,
              AttributeValue,
              AttributeCode
			  FROM @TBL_AttributeDetails
			)

			SELECT PIV.PimProductId,ProductName,PIV.SKU ,TaxClassId,TaxClassSKUId
			FROM Cte_pimProductDetails CTEPD
			PIVOT
			(
			
			Max(AttributeValue) FOR AttributeCode IN ([ProductName],[SKU])
			
			) PIV
		    LEFT JOIN ZnodeTaxClassSKU  ZP ON (dbo.FN_TRIM(Zp.SKU) = dbo.FN_TRIM(PIV.SKU))
		 	INNER JOIN @ProductIdTable XP ON (XP.PimProductId = PIV.PimProductId)
			WHERE (ZP.TaxClassId = @TaxClassId	  OR   @TaxClassId =  -1 ) 
		    ORDER BY RowId 
		  IF EXISTS (SELECT Top 1 1 FROM @TAb )
	 BEGIN 

		  SELECT @RowsCount= (SELECT COUNT(1) FROM @TAb)   
	 END 
	 ELSE 
	 BEGIN
	 		  SELECT  @RowsCount= (SELECT COUNT(1) FROM @ProductListIdRTR) 
	 END 

	 
         END TRY
         BEGIN CATCH
		    SELECT ERROR_MESSAGE()
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
			 @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			  @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTaxlClassDetail @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
			  +',@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',@TaxClassId = '+ISNULL(CAST(@TaxClassId AS VARCHAR(50)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetTaxlClassDetail',
					@ErrorInProcedure = 'Znode_GetTaxlClassDetail',
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
         END CATCH;
     END;