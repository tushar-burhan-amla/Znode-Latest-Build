
CREATE  PROCEDURE [dbo].[Znode_ManageProductList_XML]
(   @WhereClause						 XML,
    @Rows								 INT           = 100,
    @PageNo								 INT           = 1,
    @Order_BY			 VARCHAR(1000) = '',
    @LocaleId			 INT           = 1,
    @PimProductId		 VARCHAR(2000) = 0,
    @IsProductNotIn	 BIT           = 0,
	@IsCallForAttribute BIT		   = 0,
	@AttributeCode      VARCHAR(max ) = '' ,
	@PimCatalogId   INT = 0,
	@IsCatalogFilter   BIT            = 0,
	@IsDebug            Bit		   = 0 )
AS
    
/*
		  Summary:-   This Procedure is used for get product List  
				    Procedure will pivot verticle table(ZnodePimattributeValues) into horizontal table with columns 
				    ProductId,ProductName,ProductType,AttributeFamily,SKU,Price,Quantity,IsActive,ImagePath,Assortment,LocaleId,DisplayOrder
        
		  Unit Testing
		  
exec Znode_ManageProductList_XML @WhereClause=N'',@Rows=50,@PageNo=1,@Order_By=N'',@LocaleId=1,@PimProductId=N'',@IsProductNotIn=1,@IsCallForAttribute=0,@AttributeCode=''
          select * from ZnodeAttributeType  WHERE AttributeValue LIKE '%&%'
		  UPDATE VieW_lOADMANAGEpRODUCT SET  AttributeValue = 'A & B'  WHERE AttributeValue LIKE '% and %' AND PimProductId = 158
    */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED 
             DECLARE @PimProductIds TransferId, --VARCHAR(MAX), 
					 @FirstWhereClause NVARCHAR(MAX)= '', 
					 @SQL NVARCHAR(MAX)= '' ,
					 @OutPimProductIds VARCHAR(max),
					 @ProductXML NVARCHAR(max) ;

             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId()
					 ,@RowsCount INT =0 ;
             DECLARE @TransferPimProductId TransferId 
			 DECLARE @TBL_AttributeDefaultValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder INT 
			  ,PimAttributedefaultValueId INT 
             );
             DECLARE @TBL_AttributeDetails AS TABLE
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT,
			  AttributeDefaultValue NVARCHAR(MAX)
             );
			 Create table #TBL_AttributeDetailsLocale
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT
             );
			 DECLARE @TBL_MultiSelectAttribute TABLE (PimAttributeId INT , AttributeCode VARCHAR(600))
			
			 DECLARE @TBL_MediaAttribute TABLE (Id INT ,PimAttributeId INT ,AttributeCode VARCHAR(600) )
			 
			 DECLARE @TBL_ProductIds TABLE 
			 (
			  PimProductId INT,
			  ModifiedDate DATETIME  
			 )

			 DECLARE @FamilyDetails TABLE
             (
			  PimProductId         INT,
              PimAttributeFamilyId INT,
              FamilyName           NVARCHAR(Max)
             );
             DECLARE @DefaultAttributeFamily INT= dbo.Fn_GetDefaultPimProductFamilyId();
             DECLARE @ProductIdTable TABLE
             (PimProductId INT,
              CountId      INT,
              RowId        INT IDENTITY(1,1)
             );
          		
             IF EXISTS ( SELECT TOP 1 1 FROM @WhereClause.nodes ( '//ArrayOfWhereClauseModel/WhereClauseModel'  ) AS Tbl(Col)
			 WHERE LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''',''))))  =  'Brand'
                OR LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''','')))) = 'Vendor'
                OR LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''',''))))  =  'ShippingCostRules'
                OR LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''','')))) =  'Highlights') and @IsCallForAttribute=1
                 BEGIN
                DECLARE @AttributeCodeValue TABLE (AttributeValue NVARCHAr(max),AttributeCode NVARCHAR(max))

				INSERT INTO @AttributeCodeValue(AttributeValue,AttributeCode)
				SELECT  Tbl.Col.value ( 'attributevalue[1]' , 'NVARCHAR(max)') AS AttributeValue
						 ,Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)') AS AttributeCode
				FROM @WhereClause.nodes ( '//ArrayOfWhereClauseModel/WhereClauseModel'  ) AS Tbl(Col)
				WHERE LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''',''))))  =  'Brand'
                OR LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''',''))))  = 'Vendor'
                OR LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''',''))))  =  'ShippingCostRules'
                OR LTRIM(RTRIM((REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(max)'),' = ',''),'''',''))))  =  'Highlights'
		
				SET @SQL =   
				           ';WIth Cte_DefaultValue AS (
										  SELECT AttributeDefaultValueCode , ZPDF.PimAttributeId ,FNPA.AttributeCode
										  FROM ZnodePImAttributeDefaultValue ZPDF
										  INNER JOIN [dbo].[Fn_GetProductDefaultFilterAttributes] () FNPA ON ( FNPA.PimAttributeId = ZPDF.PimAttributeId) 
										)
										, Cte_productIds AS 
										(
										  SELECT a.PimProductId, c.AttributeCode , CTDV.AttributeDefaultValueCode AttributeValue,b.ModifiedDate 
										  FROM  ZnodePimAttributeValue a
										  LEFT JOIN ZnodePimAttribute c ON(c.PimAttributeId = a.PimAttributeId)
										  LEFT JOIN ZnodePimAttributeValueLocale b ON(b.PimAttributeValueId = a.PimAttributeValueId)  
										  INNER JOIN Cte_DefaultValue CTDV ON (CTDV.AttributeCode = c.AttributeCode 
										  AND EXISTS (SELECT TOP 1 1 FROM dbo.split(b.AttributeValue,'','') SP WHERE SP.Item = CTDV.AttributeDefaultValueCode) )
										  Union all 
										  
											SELECT a.PimProductId,c.AttributeCode,ZPADV.AttributeDefaultValueCode AttributeValue ,a.ModifiedDate 
											FROM ZnodePimProductAttributeDefaultValue ZPPADV
											INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPPADV.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)
											LEFT JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId = ZPPADV.PimAttributeValueId )
											LEFT JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
											INNER JOIN Cte_DefaultValue CTDV ON (CTDV.AttributeCode = c.AttributeCode )
										)										
										SELECT PimProductId ,ModifiedDate
										FROM Cte_productIds WHERE  AttributeCode '+(SELECT TOP 1 AttributeCode  FROM @AttributeCodeValue )+' AND 
										AttributeValue '+(SELECT TOP 1 AttributeValue  FROM @AttributeCodeValue )+' 
										GROUP BY PimProductId,ModifiedDate Order By ModifiedDate DESC ';


					 SET @Order_BY = CASE WHEN @Order_BY = '' THEN 'ModifiedDate DESC' ELSE @Order_BY END 
					 	
					 SET @WhereClause = CAST(REPLACE(CAST(@WhereClause AS NVARCHAR(max)),'<WhereClauseModel><attributecode>'+(SELECT TOP 1 AttributeCode  FROM @AttributeCodeValue )+'</attributecode><attributevalue>'+(SELECT TOP 1 AttributeValue   FROM @AttributeCodeValue )+'</attributevalue></WhereClauseModel>','') AS XML )
					
				     INSERT INTO @TBL_ProductIds ( PimProductId, ModifiedDate )
					 EXEC (@SQL);
			  					 
					
						 INSERT INTO @ProductIdTable( PimProductId )
						 SELECT PimProductId 
						 FROM @TBL_ProductIds
					

                     INSERT INTO @TransferPimProductId
					 SELECT PimProductId
                     FROM @ProductIdTable
                   
				   			  
     DELETE FROM @ProductIdTable;
   --  SET @WhereClause = CAST(REPLACE(CAST(@WhereClause AS NVARCHAR(MAX)), @FirstWhereClause, ' 1 = 1') AS XML);
                 END
	            ELSE IF @PimProductId <> ''
			    BEGIN 
		
				 INSERT INTO @TransferPimProductId(id)
				 SELECT Item 
				 FROM dbo.split(@PimProductId,',')
			    END 
		
			
	 DECLARE  @ProductListIdRTR TransferId
	 DECLARE @TAb Transferid 
	 --DECLARE @tBL_mainList TABLE (Id INT,RowId INT)
	 Create table #TBL_ProductMainList (Id INT,RowId INT)

	 	IF @PimProductId <> ''  OR   @IsCallForAttribute=1 --OR (CAST(@WhereClause AS NVARCHAR(max))= N'' AND @Order_by <> N'' AND @AttributeCode = N'')
		BEGIN 
	 SET @IsProductNotIn = CASE WHEN @IsProductNotIn = 0 THEN 1  
					 WHEN @IsProductNotIn = 1 THEN 0 END 
		END 
	
	 INSERT INTO @ProductListIdRTR
	 EXEC Znode_GetProductList  @IsProductNotIn,@TransferPimProductId
 
	 IF CAST(@WhereClause AS NVARCHAR(max))<> N''
	 BEGIN 
	 
	  SET @SQL = 'SELECT Distinct PimProductId FROM ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))

	  EXEC Znode_GetFilterPimProductId @WhereClause,@ProductListIdRTR,@localeId
	  
      INSERT INTO @TAB 
	  EXEC (@SQL)
	 
	 END 
	 
	

	 IF EXISTS (SELECT Top 1 1 FROM @TAb ) OR CAST(@WhereClause AS NVARCHAR(max)) <> N''
	 BEGIN 
	 
	 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
	 SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO #TBL_ProductMainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId
	 
	 END 
	 ELSE 
	 BEGIN
	      
	 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
	 SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO #TBL_ProductMainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId 
	 END 
          

  			 INSERT INTO @PimProductIds ( Id  )
			 SELECT id FROM #TBL_ProductMainList

			 DECLARE @TBL_PimProductIds transferId 
			 INSERT INTO @TBL_PimProductIds
			 SELECT id 
             FROM @PimProductIds
			 			 	
			 DECLARE @PimAttributeIds TransferId  
			 INSERT INTO @PimAttributeIds
			 SELECT PimAttributeId  
			 FROM [dbo].[Fn_GetProductGridAttributes]()
			 
			

			 INSERT INTO @TBL_AttributeDetails
             (PimProductId,
              AttributeValue,
              AttributeCode,
              PimAttributeId,
			  AttributeDefaultValue
             )
             EXEC Znode_GetProductsAttributeValue_newTesting
                  @TBL_PimProductIds,
                  @PimAttributeIds,
                  @localeId;
			
			
			UPDATE @TBL_AttributeDetails
			SET AttributeValue = ISNULL(AttributeValue,'')
			WHERE AttributeValue IS NULL 

----------------------------------------------------------------------------------------------------

			

		    declare @SKU SelectColumnList
			declare @TBL_Inventorydetails table (Quantity NVARCHAR(MAx),PimProductId INT)

			INSERT INTO @SKU
			SELECT AttributeValue 
			FROM @TBL_AttributeDetails
			WHERE AttributeCode = 'SKU'
 
 			INSERT INTO @TBL_Inventorydetails(Quantity,PimProductId)
			EXEC Znode_GetPimProductAttributeInventory @SKU--vishal

			 INSERT INTO @FamilyDetails
             (PimAttributeFamilyId,
              PimProductId
             )
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId]
                  @PimProductIds,
                  1;
             
		 UPDATE a
               SET
                   FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
                  INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                       AND LocaleId = @LocaleId);
             UPDATE a
               SET
                   FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
                  INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                       AND LocaleId = @DefaultLocaleId)
             WHERE a.FamilyName IS NULL
                   OR a.FamilyName = '';
           	
			INSERT INTO @TBL_AttributeDetails             (PimProductId,              AttributeValue,              AttributeCode,              PimAttributeId             )
			SELECT PimProductId ,FamilyName, 'AttributeFamily',NULL
			FROM @FamilyDetails 
			
			INSERT INTO @TBL_AttributeDetails             (PimProductId,              AttributeValue,              AttributeCode,              PimAttributeId             )
			SELECT a.ID PimProductId ,th.DisplayName, 'PublishStatus',NULL
			FROM @PimProductIds a 
			INNER JOIN ZnodePimProduct b ON (b.PimProductId = a.ID)
			LEFT JOIN ZnodePublishState th ON (th.PublishStateId = b.PublishStateId)

	  INSERT INTO #TBL_AttributeDetailsLocale (PimProductId ,PimAttributeId,AttributeCode )
			SELECT  TBLAD.PimProductId ,TBLAD.PimAttributeId,TBLAD.AttributeCode 
			FROM @TBL_AttributeDetails TBLAD 
			GROUP BY  TBLAD.PimProductId ,TBLAD.PimAttributeId,TBLAD.AttributeCode 
       					

	    UPDATE TBLPP 
		SET AttributeValue = CTDD.AttributeValue 
		FROM  @TBL_AttributeDetails CTDD 
		INNER JOIN #TBL_AttributeDetailsLocale TBLPP ON (TBLPP.PimProductId = CTDD.PimProductId AND TBLPP.AttributeCode  = CTDD.AttributeCode)
		WHERE TBLPP.AttributeValue IS NULL 

    	SET @ProductXML =  '<MainProduct>'+ STUFF( (  SELECT '<Product>'+'<PimProductId>'+CAST(TBAD.PimProductId AS VARCHAR(50))+'</PimProductId>'
																		+'<AvailableInventory>'+CAST(ISNULL(IDD.[Quantity],'') AS VARCHAR(50))+'</AvailableInventory>'
		+ STUFF(    (  SELECT '<'+TBADI.AttributeCode+'>'+CAST( (SELECT  ''+TBADI.AttributeValue FOR XML PATH('')) AS NVARCHAR(max))+'</'+TBADI.AttributeCode+'>'   
															FROM #TBL_AttributeDetailsLocale TBADI      
															 WHERE TBADI.PimProductId = TBAD.PimProductId 
															 ORDER BY TBADI.PimProductId DESC
															 FOR XML PATH (''), TYPE
																).value('.', ' Nvarchar(max)'), 1, 0, '')+'</Product>'	   

		FROM #TBL_AttributeDetailsLocale TBAD
		INNER JOIN #TBL_ProductMainList TBPI ON (TBAD.PimProductid = TBPI.id )
		LEFT JOIN @TBL_ProductIds TPT ON TBAD.PimProductId = TPT.PimProductId
		LEFT JOIN @TBL_InventoryDetails IDD ON (TBPI.id = IDD.PimProductId)
		GROUP BY TBAD.pimProductid, TPT.ModifiedDate,TBPI.RowId,IDD.Quantity
		ORDER BY TBPI.RowId 
		FOR XML PATH (''),TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')+'</MainProduct>'
			--FOR XML PATH ('MainProduct'))
 

			SELECT  CAST(@ProductXML AS XML ) ProductXMl
		   
		     SELECT AttributeCode ,  ZPAL.AttributeName
			 FROM ZnodePimAttribute ZPA 
			 LEFT JOIN ZnodePiMAttributeLOcale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId )
             WHERE LocaleId = 1  
			 AND  IsCategory = 0 
			 AND ZPA.IsShowOnGrid = 1  
			 UNION ALL 
			 SELECT 'PublishStatus','Publish Status'

     IF EXISTS (SELECT Top 1 1 FROM @TAb )
	 BEGIN 

		  SELECT (SELECT COUNT(1) FROM @TAb) AS RowsCount   
	 END 
	 ELSE 
	 BEGIN
	 		  SELECT (SELECT COUNT(1) FROM @ProductListIdRTR) AS RowsCount   
	 END 
		;

             -- find the all locale values 
         END TRY
         BEGIN CATCH
		    SELECT ERROR_MESSAGE()
                DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageProductList_XML @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimProductId='+@PimProductId+',@IsProductNotIn='+CAST(@IsProductNotIn AS VARCHAR(50))+',@IsCallForAttribute='+CAST(@IsCallForAttribute AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ManageProductList_XML',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;

         END CATCH;

     END;