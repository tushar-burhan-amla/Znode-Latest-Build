CREATE PROCEDURE [dbo].[Znode_GetPimCategoryProductList]
(   
	@WhereClause   XML,
    @Rows          INT           = 100,
    @PageNo        INT           = 1,
    @Order_BY      VARCHAR(1000) = '',
    @RowsCount     INT OUT,
    @LocaleId      INT           = 1,
    @PimCategoryId INT,
    @IsAssociated  BIT           = 0,
	@AttributeCode VARCHAR(max) = ''
)
AS
/*
     Summary :- This Procedure is used to get the product list for category products
				The result is fetched order by DisplayOrder or status as per requirement in both asc and desc

     Unit Testing
	 begin tran
     EXEC Znode_GetPimCategoryProductList '',@RowsCount = 0, @PimCategoryId = 22,@Order_BY ='DisplayOrder asc'
	 rollback tran
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;

			  SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

             DECLARE @TBL_AttributeDefaultValue TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX),
			  DisplayOrder INT
             );
			 DECLARE @TransferPimProductId TransferId
             DECLARE @TBL_AttributeDetails AS TABLE
             (PimProductId   INT,
              AttributeValue NVARCHAR(MAX),
              AttributeCode  VARCHAR(600),
              PimAttributeId INT
             );
             DECLARE @TBL_FamilyDetails TABLE
             (PimProductId         INT,
              PimAttributeFamilyId INT,
              FamilyName           NVARCHAR(3000)
             );
             DECLARE @OrderByDisplay INT= 0;
             DECLARE @DefaultAttributeFamilyId INT= dbo.Fn_GetDefaultPimProductFamilyId(), @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();

			 DECLARE @TBL_ProductIdTable TABLE
             ([PimProductId] INT,
              [CountId]      INT,
              PimCategoryId  INT,
              RowId          INT
             );

             DECLARE @PimProductId TransferId ,--VARCHAR(MAX)= '',
					 @PimAttributeId VARCHAR(MAX),
					 @OutPimProductIds VARCHAR(MAX);

			 DECLARE @PimProductIds TransferId

             IF @Order_BY LIKE '%DisplayOrder%'
                 BEGIN
                     SET @OrderByDisplay = 1;
                 END;
             ELSE
             IF @Order_BY LIKE '%Status%'
                 BEGIN
                     SET @OrderByDisplay = 2;
                 END;
			 DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))
			 INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()

            INSERT INTO @TransferPimProductId
			SELECT PimProductId
			FROM ZnodePimCategoryProduct ZCP
            WHERE ZCP.PimCategoryId = @PimCategoryId
			ORDER BY CASE WHEN @Order_By LIKE '% DESC%'
            THEN
			CASE WHEN @OrderByDisplay = 1
					THEN ZCP.DisplayOrder
				WHEN @OrderByDisplay = 2
					THEN ZCP.Status
				 ELSE 1 END
				 ELSE 1 END DESC,
            CASE WHEN @Order_By LIKE '% ASC%'
				THEN
					CASE WHEN @OrderByDisplay = 1
					THEN ZCP.DisplayOrder
						WHEN @OrderByDisplay = 2
							THEN ZCP.Status
							 ELSE 1 END
							  ELSE 1 END
	         IF NOT EXISTS (SELECT TOP 1 1 FROM @TransferPimProductId  )
			 BEGIN
			   INSERT INTO @TransferPimProductId
			   SELECT '0'
			   SET @IsAssociated = 0
             END


   DECLARE @SQL NVARcHAR(max)= ''
		 DECLARE  @ProductListIdRTR TransferId
	 DECLARE @TAb Transferid
	 DECLARE @tBL_mainList TABLE (Id INT,RowId INT)
	 --	IF   @IsCallForAttribute=1
		--BEGIN
	 SET @IsAssociated = CASE WHEN @IsAssociated = 0 THEN 1
					 WHEN @IsAssociated = 1 THEN 0 END
		--END
	 INSERT INTO @ProductListIdRTR
	 EXEC Znode_GetProductList  @IsAssociated,@TransferPimProductId

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
	 --SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO @TBL_MainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId,@PimCategoryId=@PimCategoryId

	 END
	 ELSE
	 BEGIN

	 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
	 --SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO @TBL_MainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId,@PimCategoryId=@PimCategoryId
	 END

			 INSERT INTO @TBL_ProductIdTable(PimProductId,RowId)
			 SELECT ID ,RowId FROM @TBL_MainList SP

			 INSERT INTO @PimProductIds ( Id )
			 SELECT Id FROM @TBL_MainList SP

             UPDATE @TBL_ProductIdTable SET PimCategoryId = @PimCategoryId;
             SET @PimAttributeId = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) FROM [dbo].[Fn_GetGridPimAttributes]() FOR XML PATH('')), 2, 4000);
 
             INSERT INTO @TBL_AttributeDetails(PimProductId, AttributeValue,AttributeCode,PimAttributeId)
			 EXEC Znode_GetProductsAttributeValue @PimProductIds,@PimAttributeId,@LocaleId;
             
	
			   DROP TABLE IF EXISTS #Cte_ProductMedia
			 
			   SELECT TBA.PimProductId , TBA.PimAttributeId
			   , SUBSTRING( ( SELECT ','+ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL)+ZMSM.ThumbnailFolderName+'/'+ zm.PATH  
			  
			   FROM ZnodeMedia AS ZM
               INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)
			   INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
			   INNER JOIN @TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)) )
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)
			   WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId
			   FOR XML PATH('') ), 2 , 4000) AS AttributeValue , SUBSTRING( ( SELECT ','+AttributeValue
			   FROM  @TBL_AttributeDetails AS TBAI
			   WHERE TBAI.PimProductId = TBA.PimProductId AND TBAI.PimAttributeId = TBA.PimAttributeId
			   FOR XML PATH('') ), 2 , 4000) MediaIds
			    
			   INTO #Cte_ProductMedia
			   
			   FROM @TBL_AttributeDetails AS TBA
			   INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId )
			   
			   
			
		      UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue
			  FROM @TBL_AttributeDetails TBAV
			  INNER JOIN #Cte_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId
			  AND CTPM.PimAttributeId = TBAV.PimAttributeId;


			 INSERT INTO @TBL_FamilyDetails(PimAttributeFamilyId,PimProductId)
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId] @PimProductId,1;

             UPDATE TFD SET FamilyName = ZPFL.AttributeFamilyName FROM @TBL_FamilyDetails TFD INNER JOIN ZnodePimFamilyLocale ZPFL
			 ON(TFD.PimAttributeFamilyId = ZPFL.PimAttributeFamilyId AND LocaleId = @LocaleId);

             UPDATE TFD SET FamilyName = ZPFL.AttributeFamilyName FROM @TBL_FamilyDetails TFD INNER JOIN ZnodePimFamilyLocale ZPFL
			 ON(TFD.PimAttributeFamilyId = ZPFL.PimAttributeFamilyId AND LocaleId = @DefaultLocaleId) WHERE TFD.FamilyName IS NULL OR TFD.FamilyName = '';


             SELECT  zpp.[PimProductId] AS [Productid],zpp.[PimProductId],ZPCP.[PimCategoryId],TBFD.FamilyName,Piv.[ProductName],Piv.[SKU],Piv.[ProductType],Piv.[Assortment],
				CASE WHEN PP.IsActive IS NULL THEN CAST(0 AS BIT) ELSE CAST(PP.IsActive AS BIT) END AS [IsActive],
				piv.[ProductImage] [ImagePath],ZPCP.DisplayOrder

			 FROM @TBL_ProductIdTable AS zpp
			 LEFT JOIN ZnodePimCategoryProduct ZPCP ON(ZPCP.PimProductId = Zpp.PimProductId AND ZPCP.PimCategoryId = Zpp.PimCategoryId)
             INNER JOIN (SELECT PimProductId,AttributeValue,AttributeCode FROM @TBL_AttributeDetails) TB
			  PIVOT(MAX([AttributeValue])
			 FOR [AttributeCode] IN([ProductName],[IsActive],[ProductImage],[SKU],[ProductType],[Assortment])) AS Piv ON(Piv.[PimProductId] = zpp.[PimProductId])
             LEFT JOIN @TBL_FamilyDetails TBFD ON(TBFD.PimProductId = zpp.[PimProductId])
			 LEFT JOIN ZnodePimProduct PP ON PP.PimProductId = Zpp.PimProductId
             ORDER BY CASE WHEN @Order_By LIKE '% DESC%' THEN CASE WHEN @OrderByDisplay = 1 THEN ZPCP.DisplayOrder
			 WHEN @OrderByDisplay = 2 THEN ZPCP.Status ELSE 1 END ELSE 1 END DESC,
             CASE WHEN @Order_By LIKE '% ASC%' THEN CASE WHEN @OrderByDisplay = 1 THEN ZPCP.DisplayOrder
             WHEN @OrderByDisplay = 2 THEN ZPCP.Status ELSE 1 END ELSE 1 END,zpp.RowId;

		IF EXISTS (SELECT Top 1 1 FROM @TAb )
		BEGIN
			SELECT @RowsCount = (SELECT COUNT(1) FROM @TAb)
		END
		ELSE
		BEGIN
			SELECT @RowsCount=(SELECT COUNT(1) FROM @ProductListIdRTR)
		END

    END TRY
    BEGIN CATCH
	SELECT ERROR_MESSAGE()
    DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimCategoryProductList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimCategoryId='+CAST(@PimCategoryId AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));

        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetPimCategoryProductList',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;