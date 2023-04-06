
CREATE PROCEDURE [dbo].[Znode_ManageProductList]
(   @WhereClause		 XML,
	@Rows				 INT           = 10,
	@PageNo			     INT           = 1,
	@Order_BY			 VARCHAR(1000) = '',
	@RowsCount			 INT OUT,
	@LocaleId			 INT           = 1,
	@PimProductId		 VARCHAR(2000) = 0,
	@IsProductNotIn	     BIT           = 0,
	@IsCallForAttribute  BIT	       = 0,
	@AttributeCode       VARCHAR(max)  = ''
	)
AS
    
/*
 Summary:-  This Procedure is used for getting product List  
			Procedure will pivot verticle table(ZnodePimattributeValues) into horizontal table with columns 
			ProductId,ProductName,ProductType,AttributeFamily,SKU,Price,Quantity,IsActive,ImagePath,Assortment,LocaleId,DisplayOrder        
 Unit Testing
		  DECLARE @D INT= 1 
		  EXEC  [dbo].[Znode_ManageProductList]   @WhereClause = N'' , @Rows = 100 , @PageNo = 1 ,@Order_BY = '',@LocaleId= 1,@PimProductId = '',@IsProductNotIn = 1 , @RowsCount = @D OUT SELECT @D
    
    */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @PimProductIds TransferId, --VARCHAR(MAX), 
					 @PimAttributeId VARCHAR(MAX), 
					 @FirstWhereClause VARCHAR(MAX)= '', 
					 @SQL NVARCHAR(MAX)= '' ,
					 @OutPimProductIds VARCHAR(max);

             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TransferPimProductId TransferId 
			 DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))
			 INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()

			 DECLARE @TBL_AttributeDefaultValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder INT 
             );
             DECLARE @TBL_AttributeDetails AS TABLE
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT
             );
             DECLARE @FamilyDetails TABLE
             (PimProductId         INT,
              PimAttributeFamilyId INT,
              FamilyName           NVARCHAR(3000)
             );
             DECLARE @DefaultAttributeFamily INT= dbo.Fn_GetDefaultPimProductFamilyId();
             DECLARE @ProductIdTable TABLE
             (PimProductId INT,
              CountId      INT,
              RowId        INT IDENTITY(1,1)
             );
             SET @FirstWhereClause =
             (SELECT WhereClause FROM dbo.Fn_GetWhereClauseXML(@WhereClause) WHERE id = 1);

             IF (@FirstWhereClause LIKE '%Brand%'
                OR @FirstWhereClause LIKE '%Vendor%'
                OR @FirstWhereClause LIKE '%ShippingCostRules%'
                OR @FirstWhereClause LIKE '%Highlights%') and @IsCallForAttribute=1
                 BEGIN

				 SET @SQL = ' DECLARE @TBL_ProductIds TABLE (PimProductId INT,ModifiedDate DATETIME  )
				            ;WIth Cte_DefaultValue AS (
						    SELECT AttributeDefaultValueCode , ZPDF.PimAttributeId ,FNPA.AttributeCode FROM ZnodePImAttributeDefaultValue ZPDF
						    INNER JOIN [dbo].[Fn_GetProductDefaultFilterAttributes] () FNPA ON ( FNPA.PimAttributeId = ZPDF.PimAttributeId))
							
							, Cte_productIds AS 
							(SELECT a.PimProductId, c.AttributeCode , CTDV.AttributeDefaultValueCode AttributeValue,b.ModifiedDate 
							FROM  ZnodePimAttributeValue a
							LEFT JOIN ZnodePimAttribute c ON(c.PimAttributeId = a.PimAttributeId)
							LEFT JOIN ZnodePimAttributeValueLocale b ON(b.PimAttributeValueId = a.PimAttributeValueId)  
							INNER JOIN Cte_DefaultValue CTDV ON (CTDV.AttributeCode = c.AttributeCode AND EXISTS (SELECT TOP 1 1 FROM dbo.split(b.AttributeValue,'','') SP WHERE SP.Item = CTDV.AttributeDefaultValueCode) )
 
						    )
							INSERT INTO @TBL_ProductIds (PimProductId ,ModifiedDate)
							SELECT PimProductId ,ModifiedDate
							FROM Cte_productIds WHERE   '+@FirstWhereClause+' GROUP BY PimProductId,ModifiedDate Order By ModifiedDate DESC 
										
							SELECT PimProductId FROM @TBL_ProductIds GROUP BY PimProductId,ModifiedDate ORDER BY ModifiedDate DESC';
					 
					 Print @sql
                     INSERT INTO @ProductIdTable(PimProductId)
                     EXEC (@SQL);

                     INSERT INTO @TransferPimProductId
					 SELECT PimProductId
                     FROM @ProductIdTable
                     UNION ALL 
					 SELECT 0
					 
					                                    
                     DELETE FROM @ProductIdTable;

                     SET @WhereClause = CAST(REPLACE(CAST(@WhereClause AS NVARCHAR(MAX)), '<WhereClauseModel><WhereClause>'+@FirstWhereClause+'</WhereClause></WhereClauseModel>', '') AS XML);
                 END
				 ELSE IF @PimProductId <> ''
			    BEGIN 
				 INSERT INTO @TransferPimProductId(id)
				 SELECT Item 
				 FROM dbo.split(@PimProductId,',')
			    END 
					                    
             EXEC Znode_GetProductIdForPaging @whereClauseXML = @WhereClause,@Rows = @Rows,@PageNo = @PageNo,@Order_BY = @Order_BY,@RowsCount = @RowsCount OUT,
             @LocaleId = @LocaleId,@AttributeCode = @AttributeCode,@PimProductId = @TransferPimProductId, @IsProductNotIn = @IsProductNotIn,@OutProductId = @OutPimProductIds OUT;
			
			 INSERT INTO @ProductIdTable (PimProductId)              
			 SELECT item 
			 FROM dbo.split(@OutPimProductIds,',') SP 
				 
				            
             --SET @PimProductIds = SUBSTRING((SELECT ','+CAST(PimProductId AS VARCHAR(100)) FROM @ProductIdTable FOR XML PATH('')), 2, 4000);
			 INSERT INTO @PimProductIds ( Id )
			 SELECT PimProductId FROM @ProductIdTable

             SET @PimAttributeId = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) FROM [dbo].[Fn_GetGridPimAttributes]() FOR XML PATH('')), 2, 4000);;
            
			 INSERT INTO @TBL_AttributeDefaultValue
             (PimAttributeId,
              AttributeDefaultValueCode,
              IsEditable,
              AttributeDefaultValue,
			  DisplayOrder
             )
             EXEC Znode_GetAttributeDefaultValueLocale
                  @PimAttributeId,
                  @LocaleId;

             INSERT INTO @TBL_AttributeDetails
             (PimProductId,
              AttributeValue,
              AttributeCode,
              PimAttributeId
             )
             EXEC [Znode_GetProductsAttributeValue]
                  @PimProductIds,
                  @PimAttributeId,
                  @localeId;
     
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
            
			;WITH Cte_ProductMedia
               AS (SELECT TBA.PimProductId , TBA.PimAttributeId 
			   , SUBSTRING( ( SELECT ','+ dbo.Fn_GetMediaThumbnailMediaPath(zm.PATH) 
			   FROM ZnodeMedia AS ZM
               INNER JOIN @TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)) )
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)
			   WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId 
			   FOR XML PATH('') ), 2 , 4000) AS AttributeValue 
			   FROM @TBL_AttributeDetails AS TBA 
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId ))
                          
		      UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue
			  FROM @TBL_AttributeDetails TBAV 
			  INNER JOIN Cte_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId 
			  AND CTPM.PimAttributeId = TBAV.PimAttributeId;


			
			 SELECT zpp.PimProductid AS ProductId,
                    [ProductName],
                    ProductType,
                    ISNULL(zf.FamilyName, '') AS AttributeFamily,
                    [SKU],
                    [Price],
                    [Quantity],
                    CASE
                        WHEN [IsActive] IS NULL
                        THEN CAST(0 AS BIT)
                        ELSE CAST([IsActive] AS BIT)
                    END AS [IsActive],
                   piv.[ProductImage] AS ImagePath,
                    [Assortment],
                    @LocaleId AS LocaleId,
                    [DisplayOrder]
             FROM @ProductIdTable AS zpp
                  LEFT JOIN @FamilyDetails AS zf ON(zf.PimProductId = zpp.PimProductId)
                  INNER JOIN
             (
                 SELECT PimProductId,
                        AttributeValue,
                        AttributeCode
                 FROM @TBL_AttributeDetails
             ) TB PIVOT(MAX(AttributeValue) FOR AttributeCode IN([ProductName],
                                                                 [SKU],
                                                                 [Price],
                                                                 [Quantity],
                                                                 [IsActive],
                                                                 [ProductType],
                                                                 [ProductImage],
                                                                 [Assortment],
                                                                 [DisplayOrder])) AS Piv ON(Piv.PimProductId = zpp.PimProductid)
                --  LEFT JOIN ZnodeMedia AS zm ON(zm.MediaId = piv.[ProductImage])
             ORDER BY zpp.RowId;
         
         END TRY
         BEGIN CATCH
               DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ManageProductList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimProductId='+@PimProductId+',@IsProductNotIn='+CAST(@IsProductNotIn AS VARCHAR(50))+',@IsCallForAttribute='+CAST(@IsCallForAttribute AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ManageProductList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;