CREATE PROCEDURE [dbo].[Znode_ManageProductListByAttributes]
(   @WhereClause      XML,
	@PimAttributeIds  VARCHAR(3000) = NULL,
	@Rows             INT           = 100,
	@PageNo           INT           = 0,
	@Order_BY         VARCHAR(1000) = '',
	@LocaleId         INT,
	@PimProductId     VARCHAR(max) = NULL,
	@IsProductNotIn   BIT           = 0,
	@RelatedProductId INT           = 0, 
	@IsDebug		    BIT = 0 
	)
AS
   /*  Summary:-  This Procedure is used for get product List with extra column attribute supllied to the procedure 
     Unit Testing 
     DECLARE @EDE INT = 0 
	 exec Znode_ManageProductListByAttributes @WhereClause='',@PimAttributeIds = '115',@Rows = 10,@PageNo=1,@Order_BY = '',@RelatedProductId = 179,@PimProductId = '',@IsProductNotIn= 0 ,@LocaleId=1 --SELECT @EDE 
	SELECT * FROM ZnodePimAttribute 
	*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		  --  SELECT '123112'
             DECLARE @SQL NVARCHAR(MAX), @AttributeCode_filter NVARCHAR(2000), @WhereClauseChanges NVARCHAR(MAX)= '',@OutPimProductIds varchar(max) ;
             SET @WhereClauseChanges = CONVERT(NVARCHAR(MAX), @WhereClause);
             DECLARE @PimAttributeFamilyId INT= Dbo.Fn_GetDefaultValue('PimFamily'), @RowsCount INT, @DefaultLocaleId INT= Dbo.Fn_GetDefaultlocaleId();
             DECLARE @TransferPimProductId TransferId 
			 DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))
			 INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()					 
			
	
			 DECLARE @ProductIdTable TABLE
             (PimProductId INT,
              CountId      INT,
              RowId        INT identity(1,1)
             );
             DECLARE @TBL_PimAttributeId TABLE
             (PimAttributeId INT,
              AttributeCode  VARCHAR(600)
             );
             INSERT INTO @TBL_PimAttributeId
             (PimAttributeId,
              AttributeCode
             )
                    SELECT PimAttributeId,
                           AttributeCode
                    FROM ZnodePimAttribute ZPA
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM dbo.Split(@PimAttributeIds, ',') SP
                        WHERE SP.Item = ZPA.PimAttributeId
                    );
					
             SET @AttributeCode_filter = ISNULL(CAST((
                                                      SELECT CAST('<WhereClauseModel><attributecode>'+ '  = '+''''+TBPA.AttributeCode+''''+'</attributecode></WhereClauseModel>' AS XML )
                                                      FROM @TBL_PimAttributeId TBPA
                                                      FOR XML PATH(''),TYPE
                                                  ) AS NVARCHAR(max)),'');
          
		     SET @WhereClauseChanges = [dbo].[Fn_GetXmlWhereClauseForAttribute](@WhereClauseChanges,@AttributeCode_filter, @LocaleId);
             SET @WhereClause = CONVERT(XML, @WhereClauseChanges);	
	    
		  INSERT INTO @TransferPimProductId
		  SELECT ITEM
		  FROM DBO.SPLIT(@PIMPRODUCTID,',')
		  UNION ALL 
		  SELECT PimProductId 
		  FROM ZnodePimProductTypeAssociation  
		  WHERE PimParentProductId=  @RelatedProductId
		  AND @PIMPRODUCTID = '0'
		
		   DECLARE @AttributeCode NVARCHAR(max)
		   SET @AttributeCode = SUBSTRING ((SELECT ','+AttributeCode FROM [dbo].[Fn_GetProductGridAttributes]() qt WHERE (EXISTS (SELECT TOP 1 1 
				FROM dbo.split(@PimAttributeIds,',') TR WHERE tr.Item = qt.PimAttributeId)  OR AttributeCode = 'ProductType')
		   FOR XML PATH('')  ),2,4000)
	
	SET @IsProductNotIn = CASE WHEN @IsProductNotIn = 1 THEN 0 
	 WHEN @IsProductNotIn = 0 THEN 1  END
     DECLARE  @ProductListIdRTR TransferId
	 DECLARE @TAb Transferid 
	 DECLARE @tBL_mainList TABLE(Id INT , RowId INT )
	
	 INSERT INTO @ProductListIdRTR
	 EXEC Znode_GetProductList @IsProductNotIn ,@TransferPimProductId


	 IF CAST(@WhereClause AS NVARCHAR(max))<> N''
	 BEGIN 
	  
	  SET @SQL = 'SELECT PimProductId FROM ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))
	  --INSERT INTO @TAB 
	  EXEC Znode_GetFilterPimProductId @WhereClause,@ProductListIdRTR,@localeId
	  
      INSERT INTO @TAB 
	  EXEC (@SQL)
	 
	 END 
	 	
	 
	 IF EXISTS (SELECT Top 1 1 FROM @TAb )OR CAST(@WhereClause AS NVARCHAR(max)) <> N''
	 BEGIN 
	
	 SET @AttributeCode = dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC',''))
	 INSERT INTO @TBL_MainList
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId
	
	 END 
	 ELSE 
	 BEGIN
	 SET @AttributeCode = dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC',''))
	 INSERT INTO @TBL_MainList
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId 
	 END 

	
			 INSERT INTO @ProductIdTable
             (PimProductId) 
			 select id
			 from @TBL_MainList
		
             SET @AttributeCode_filter = SUBSTRING(
                                                  (
                                                      SELECT ','+TBPA.AttributeCode
                                                      FROM @TBL_PimAttributeId TBPA
                                                      FOR XML PATH('')
                                                  ), 1, 4000);
     

			 DECLARE @PimProductIds TransferId

			 INSERT INTO @PimProductIds ( Id )
			 SELECT distinct id FROM @TBL_MainList
														      		
             DECLARE @DefaultAttributeCode VARCHAR(MAX)= dbo.Fn_GetDefaultValue('AttributeCode');
          			
			 INSERT INTO @TBL_PimAttributeId
             (PimAttributeId,
              AttributeCode
             )
                    SELECT PimAttributeId,
                           AttributeCode
                    FROM ZnodePimAttribute ZPA
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM dbo.Split(@DefaultAttributeCode, ',') SP
                        WHERE SP.Item = ZPA.AttributeCode
                    );
			
		

			INSERT INTO @TBL_PimAttributeId
             (PimAttributeId,
              AttributeCode
             )
                    SELECT PimAttributeId,
                           'OR_'+AttributeCode
                    FROM ZnodePimAttribute ZPA
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM dbo.Split(@PimAttributeIds, ',') SP
                        WHERE SP.Item = ZPA.PimAttributeId
                    );
             
	
             SET @DefaultAttributeCode = @DefaultAttributeCode + @AttributeCode_filter;
             Create TABLE #TBL_AttributeDetails 
             (
				  PimProductId                INT ,
				  AttributeValue              NVARCHAR(MAX),
				  AttributeCode               VARCHAR(600),
				  PimAttributeId              INT,
				  PimProductTypeAssociationId INT,
				  DisplayOrder                INT,
				  IsNonEditableRow            BIT DEFAULT 0,
				  IsDefault bit
             );
             DECLARE @TBL_AttributeCode TABLE
             (
				  PimAttributeId INT,
				  AttributeCode  VARCHAR(300)
             );
             INSERT INTO @TBL_AttributeCode
             (
				  PimAttributeId,
				  AttributeCode
             )
                    SELECT PimAttributeId,
                           AttributeCode
                    FROM ZnodePimAttribute ZPA
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM dbo.split(@DefaultAttributeCode, ',') SP
                        WHERE Sp.Item = ZPA.AttributeCode
                    );

             DECLARE @TBL_AttributeDefaultValue TABLE
             (
				PimAttributeId            INT,
				AttributeDefaultValueCode VARCHAR(100),
				IsEditable                BIT,
				AttributeDefaultValue     NVARCHAR(MAX),
				DisplayOrder INT
             );
			  
             DECLARE @PimAttributeId VARCHAR(MAX);
             SET @PimAttributeId = SUBSTRING(
                                            (
                                                SELECT ','+CAST(TBAC.PimAttributeId AS VARCHAR(50))
                                                FROM @TBL_AttributeCode TBAC
                                                     INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON(ZPADV.PimAttributeId = TBAC.PimAttributeId)
                                                FOR XML PATH('')
                                            ), 2, 4000);

													
             INSERT INTO @TBL_AttributeDefaultValue
             (
				PimAttributeId,
				AttributeDefaultValueCode,
				IsEditable,
				AttributeDefaultValue,
				DisplayOrder
             )
             EXEC Znode_GetAttributeDefaultValueLocale
                  @PimAttributeId,
                  @LocaleId;

			
             INSERT INTO #TBL_AttributeDetails
             (
				  PimProductId,
				  AttributeValue,
				  AttributeCode,
				  PimAttributeId
             )
             EXEC Znode_GetProductsAttributeValue
                  @PimProductIds,
                  @DefaultAttributeCode,
                  @localeId;
				
			 INSERT INTO #TBL_AttributeDetails
             (
				  PimProductId,
				  AttributeValue,
				  AttributeCode,
				  PimAttributeId
             )
			SELECT ZPAV.PimProductId ,ZPPAVD.PimAttributeDefaultValueId,'OR_'+ZPA.AttributeCode,ZPA.PimAttributeId
			FROM ZnodePimAttributeValue ZPAV 
			INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId) 
			INNER JOIN @ProductIdTable TBL ON (TBL.PimProductId = ZPAV.PimProductId )
			INNER JOIN ZnodePimProductAttributeDefaultValue ZPPAVD ON (ZPPAVD.PimAttributeValueId = ZPAV.PimAttributeValueId  )
			WHERE ZPPAVD.LocaleId = @DefaultLocaleId
			AND EXISTS (SELECT TOP 1 1 FROM dbo.Split(@PimAttributeIds,',') SP WHERE Sp.Item = ZPA.PimAttributeId )

				
			----------------------------------------------------------------------------------------------
			DECLARE @SKU SelectColumnList
			Create TABLE  #Temp_Inventory (Quantity NVARCHAR(MAX),PimProductId INT)

			INSERT INTO @SKU
			SELECT AttributeValue FROM #TBL_AttributeDetails
			WHERE AttributeCode = 'SKU'
						
			INSERT INTO #Temp_Inventory(Quantity,PimProductId)
			EXEC Znode_GetPimProductAttributeInventory @SKU=@SKU, @LocaleId=@LocaleId
						
			---------------------------------------------------------------------------------------------------------

             DECLARE @FamilyDetails TABLE
             (PimProductId         INT,
              PimAttributeFamilyId INT,
              FamilyName           NVARCHAR(3000)
             );

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
			
             --- Update the  product families name locale wise   

				   
			   SELECT TBA.PimProductId , TBA.PimAttributeId 
			   , SUBSTRING( ( SELECT ','+dbo.Fn_GetMediaThumbnailMediaPath (zm.PATH) 
			   FROM ZnodeMedia AS ZM
              
			   INNER JOIN #TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)) )
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)
			   WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId 
			   FOR XML PATH('') ), 2 , 4000) AS AttributeValue 
			   into #TBL_ProductMedia
			   FROM #TBL_AttributeDetails AS TBA 
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId )
                         
   
		       UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue
			   FROM #TBL_AttributeDetails TBAV 
			   INNER JOIN #TBL_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId 
			   AND CTPM.PimAttributeId = TBAV.PimAttributeId;
			  					
		
             UPDATE TBAD
               SET
                   PimProductTypeAssociationId = ZPTA.PimProductTypeAssociationId,
                   DisplayOrder = ZPTA.DisplayOrder, IsDefault = ZPTA.IsDefault
             FROM #TBL_AttributeDetails TBAD
                  INNER JOIN ZnodePimproductTypeAssociation ZPTA ON(ZPTA.PimProductId = TBAD.PimProductId)
             WHERE ZPTA.PimParentProductId = @RelatedProductId;
            
			-- DECLARE @AttributeCode NVARCHAR(4000);
             SET @AttributeCode = SUBSTRING(
                                           (
                                               SELECT DISTINCT
                                                      ','+QUOTENAME(AttributeCode)
                                               FROM @TBL_PimAttributeId
                                               FOR XML PATH('')
                                           ), 2, 4000);
             DECLARE @AttributeCode_Duplicate NVARCHAR(4000)= SUBSTRING(
                                                                       (
                                                                           SELECT 
                                                                                  ', Piv.'+QUOTENAME(AttributeCode)
                                                                           FROM ZnodePimAttribute ZPA
                                                                           WHERE EXISTS
                                                                           (
                                                                               SELECT TOP 1 1
                                                                               FROM dbo.Split(@PimAttributeIds, ',') SP
                                                                               WHERE SP.Item = ZPA.PimAttributeId
                                                                               ORDER BY AttributeCode
                                                                           )
																		   GROUP BY ZPA.AttributeCode,ZPA.DisplayOrder
																		   ORDER BY ZPA.DisplayOrder  DESC
                                                                           FOR XML PATH('')
                                                                       ), 1, 4000);
             DECLARE @AttributeCode_Duplicate_Data NVARCHAR(4000);
			 	 
			

			  SET  @AttributeCode_Duplicate_Data= SUBSTRING(
                                                                       (
                                                                           SELECT 
                                                                                  'AND Piv.'+QUOTENAME('OR_'+AttributeCode) +'= Isa.'+QUOTENAME(AttributeCode)+' '
                                                                           FROM ZnodePimAttribute ZPA
                                                                           WHERE EXISTS
                                                                           (
                                                                               SELECT TOP 1 1
                                                                               FROM dbo.Split(@PimAttributeIds, ',') SP
                                                                               WHERE SP.Item = ZPA.PimAttributeId
                                                                               ORDER BY AttributeCode
                                                                           )
																		   GROUP BY ZPA.AttributeCode,ZPA.DisplayOrder
																		   ORDER BY ZPA.DisplayOrder  DESC
                                                                           FOR XML PATH('')
                                                                       ), 4, 4000) +' '

            -- SET @AttributeCode_Duplicate_Data = REPLACE(SUBSTRING(@AttributeCode_Duplicate, 2, 4000), ',', '+'',''+');
             SELECT PimProductId,
                    AttributeValue,
                    AttributeCode,
                    PimProductTypeAssociationId,
                    DisplayOrder, IsDefault
             INTO #Temp_attribute
             FROM #TBL_AttributeDetails
             ORDER BY DisplayOrder;
             SELECT *
             INTO #temp_Family
             FROM @FamilyDetails;
             
			 DECLARE @IsSelectedAttributeValue TABLE
             (ProductId      INT,
              AttributeValue NVARCHAR(500),
              AttributeCode  NVARCHAR(500),
              PimAttributeId INT,PimAttributeDefaultValueId INT 
             );

			  
		   DECLARE @IsSelectedAttributeValueLocale TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode NVARCHAR(600),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(max),
			  DisplayOrder   INT 
             );
          

		  -- select @PimProductId ,@AttributeCode ,@LocaleId 
		  ;With Cte_AttributeVAkuestest AS 
		  (
		    SELECT ZPAV.PimAttributeId , ZPPAD.PimAttributeDefaultValueId ,ZPAV.PimProductId
			FROM ZnodePimAttributeVAlue ZPAV 
			INNER JOIN ZnodePimProductAttributeDefaultValue ZPPAD ON (ZPPAD.PimAttributeValueId = ZPAV.PimAttributeValueId)
			LEFT JOIN ZnodePimproductTypeAssociation ZPPTA on ZPAV.PimProductId = ZPPTA.PimProductId and ZPPTA.PimParentProductId = @RelatedProductId 
			WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PimAttributeIds,',') SP WHERE SP.Item = ZPAV.PimAttributeId )
			AND EXISTS (SELECT TOP 1 1 FROM dbo.split(@PimProductId,',') SP WHERE SP.Item = ZPAV.PimProductId )-- or @PimProductId = '0')
		) ,Cte_PimAttributeDefaultValueLocale AS 
		(
		  SELECT  AttributeDefaultValue ,PimAttributeId,PimProductId,CTA.PimAttributeDefaultValueId
		  FROM ZnodePimAttributeDefaultValueLocale CTA  
		  INNER JOIN Cte_AttributeVAkuestest CTB ON (CTB.PimAttributeDefaultValueId = CTA.PimAttributeDefaultValueId)		
		  WHERE LocaleId = @DefaultLocaleId 
		  UNION 
		  SELECT  AttributeDefaultValue ,PimAttributeId,PimProductId,CTA.PimAttributeDefaultValueId
		  FROM ZnodePimAttributeDefaultValueLocale CTA 
		  INNER JOIN Cte_AttributeVAkuestest CTB ON (CTB.PimAttributeDefaultValueId = CTA.PimAttributeDefaultValueId)		
		  WHERE LocaleId = @DefaultLocaleId 	
		)
		,Cte_AttributeValueForCode 
		As
		(
		  SELECT AttributeDefaultValue AtributeValue , AttributeCode ,PimProductId ,a.PimAttributeDefaultValueId
		  FROM Cte_PimAttributeDefaultValueLocale a
		  INNER JOIN ZnodePimAttribute b ON (b.PimAttributeId = a.PimAttributeId )
		)
			 INSERT INTO @IsSelectedAttributeValue (ProductId,AttributeCode,AttributeValue,PimAttributeDefaultValueId)
             SELECT PimProductId,AttributeCode,AtributeValue,PimAttributeDefaultValueId
			 FROM Cte_AttributeValueForCode
             
			 --INSERT INTO @IsSelectedAttributeValueLocale
    --         EXEC Znode_GetAttributeDefaultValueLocale
    --              @PimAttributeIds,
    --              @LocaleId;
             
			 --UPDATE izav
    --           SET
    --               izav.AttributeValue = isval.AttributeDefaultValue
    --         FROM @IsSelectedAttributeValue izav
    --              INNER JOIN @IsSelectedAttributeValueLocale isval ON izav.AttributeValue = isval.AttributeDefaultValueCode AND izav.PimAttributeId = isval.PimAttributeId ;
             

			 SELECT * 
			 --SUBSTRING(
    --                         (
    --                             SELECT ','+isav.AttributeValue
    --                             FROM @IsSelectedAttributeValue isav
				--				 INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ISAV.PimAttributeID )
    --                             WHERE isa.ProductId = isav.ProductId
    --                             ORDER BY ZPA.DisplayOrder DESC
    --                             FOR XML PATH('')
    --                         ), 2, 4000) AttributeValue,
							

             INTO #IsSelectedAttribute
             FROM @IsSelectedAttributeValue isa
			; 
				 
			 IF @IsDebug = 1 
			 BEGIN 
			 SELECT * FROM @IsSelectedAttributeValue izav

			 SELECT * FROM #IsSelectedAttribute

			 END 
             --select * from @IsSelectedAttributeValue
             --select @AttributeCode_Duplicate,@AttributeCode_Duplicate_data
             --select * from #IsSelectedAttribute
			 
             SET @AttributeCode = REPLACE(@AttributeCode, ',[DisplayOrder]', '');
             SET @SQL = '
			     
				 ;with Cte_Getvalue AS (
				 SELECT ProductId , '+SUBSTRING(@AttributeCode_Duplicate, 2, 4000)+'
				 FROM ( SELECT ProductId,AttributeCode,PimAttributeDefaultValueId FROM #IsSelectedAttribute gt ) dd 
				 PIVOT ( MAX (PimAttributeDefaultValueId) FOR AttributeCode IN ('+REPLACE(SUBSTRING(@AttributeCode_Duplicate, 2, 4000),'Piv.','')+')  ) PIV 
				 )

				SELECT DISTINCT  piv.PimProductTypeAssociationId, zpp.PimProductid ProductId, Piv.[ProductName],Piv.ProductType ,ISNULL(zf.FamilyName,'''')  AttributeFamily , Piv.[SKU]
						  , CASE WHEN Piv.[IsActive] IS NULL THEN ''false'' ELSE   Piv.[IsActive]  END  [Status],  piv.[ProductImage] ImagePath,Piv.[Assortment],DisplayOrder  ,'+CAST(@LocaleId AS VARCHAR(50))+' LocaleId
						  ,DENSE_RANK()Over(Order By'+SUBSTRING(@AttributeCode_Duplicate, 2, 4000)+') CombinationId '+@AttributeCode_Duplicate+'
					, CASE When isa.ProductId Is Null then 0 ELSE 1 END IsNonEditableRow,'+ CAST(@RelatedProductId AS VARCHAR(50))+' RelatedProductId, IDD.Quantity AvailableInventory, piv.IsDefault
				FROM ZNodePimProduct zpp 
				LEFT JOIN  #temp_Family zf ON (zf.PimProductId = zpp.PimProductId)
				INNER JOIN #Temp_attribute 
				PIVOT 
				(
				Max(AttributeValue) FOR AttributeCode  IN ( '+@AttributeCode+')
				)Piv  
				ON (Piv.PimProductId = zpp.PimProductid) 
				LEFT JOIN #Temp_Inventory IDD ON (IDD.PimProductId = Piv.PimProductId)
				--LEFT JOIN ZnodeMedia zm ON (zm.MediaId = piv.[ProductImage])
				LEFT OUTER JOIN Cte_Getvalue isa ON ('+@AttributeCode_Duplicate_Data+')
				    '+' Order BY '+ISNULL(CASE
                                                  WHEN @Order_BY = ''
                                                  THEN 'DisplayOrder'
                                                  ELSE @Order_BY
                                              END, 'DisplayOrder');
		
	
             -- SELECT '''+SUBSTRINg(REPLACE(@AttributeCode_Duplicate,'Piv.',''),2,4000)+''' Ids
			 
             SELECT AttributeCode
             FROM ZnodePimAttribute ZPA
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM dbo.Split(@PimAttributeIds, ',') SP
                 WHERE SP.Item = ZPA.PimAttributeId
             );
             
			PRINT @SQL
             EXEC SP_executesql
                  @SQL;
        
     IF EXISTS (SELECT Top 1 1 FROM @TAb ) OR CAST(@WhereClause AS NVARCHAR(max)) <> N''
	 BEGIN 

		  SELECT (SELECT COUNT(1) FROM @TAb) AS RowsCount   
	 END 
	 ELSE 
	 BEGIN
	 		  SELECT (SELECT COUNT(1) FROM @ProductListIdRTR) AS RowsCount   
	 END ;

             DROP TABLE #Temp_attribute;
             DROP TABLE #temp_Family;
   
             -- find the all locale values 
         END TRY
         BEGIN CATCH
		  SELECT ERROR_MESSAGE()
                DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageProductListByAttributes @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@PimAttributeIds='+@PimAttributeIds+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimProductId='+@PimProductId+',@IsProductNotIn='+CAST(@IsProductNotIn AS VARCHAR(50))+',@RelatedProductId='+CAST(@RelatedProductId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ManageProductListByAttributes',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;