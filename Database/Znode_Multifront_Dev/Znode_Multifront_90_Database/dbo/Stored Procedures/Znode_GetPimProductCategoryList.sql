CREATE PROCEDURE [dbo].[Znode_GetPimProductCategoryList]      
(   
	@WhereClause   XML,      
	@Rows          INT           = 100,      
	@PageNo        INT           = 1,      
	@Order_BY      VARCHAR(1000) = '',      
	@RowsCount     INT OUT,      
	@LocaleId      INT           = 1,      
	@PimProductIdInput INT,      
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
                 
	DECLARE @TransferPimCategoryId TransferId       
	CREATE TABLE #TBL_AttributeDetails (PimCategoryAttributeValueId INT,PimCategoryId   INT,AttributeValue NVARCHAR(MAX),  
	AttributeCode  VARCHAR(600),PimAttributeId INT);      
                  
	DECLARE @OrderByDisplay INT= 0;      
	DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();      
                   
	CREATE TABLE #TBL_ProductIdTable ([PimProductId] INT,[CountId] INT,PimCategoryId  INT,RowId INT);      
      
	DECLARE   
	@PimAttributeId VARCHAR(MAX)      
		   
      
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
	
	CREATE TABLE #TBL_PimMediaAttributeId  (PimAttributeId INT ,AttributeCode VARCHAR(600))      
	INSERT INTO #TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)      
	SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetCategoryMediaAttributeId()      
      
	INSERT INTO @TransferPimCategoryId      
	SELECT PimCategoryId 
	FROM ZnodePimCategoryProduct ZCP 
	WHERE ZCP.PimProductId = @PimProductIdInput       
	ORDER BY CASE WHEN @Order_By LIKE '% DESC%'      
				THEN       
					CASE WHEN @OrderByDisplay = 1       
						THEN ZCP.DisplayOrder       
						WHEN @OrderByDisplay = 2       
						THEN ZCP.Status      
						ELSE 1 
					END       
					ELSE 1 
				END DESC,      
				CASE WHEN @Order_By LIKE '% ASC%'       
					THEN      
						CASE WHEN @OrderByDisplay = 1       
							THEN ZCP.DisplayOrder       
							WHEN @OrderByDisplay = 2      
							THEN ZCP.Status      
							ELSE 1 
						END      
						ELSE 1 
					END 
		 
		       
	IF NOT EXISTS (SELECT TOP 1 1 FROM @TransferPimCategoryId  )      
	BEGIN       
		INSERT INTO @TransferPimCategoryId      
		SELECT '0'      
		--SET @IsAssociated = 0       
	END       
        
      
	DECLARE @SQL NVARcHAR(max)= ''      
	DECLARE  @ProductListIdRTR TransferId      
	DECLARE @TAb Transferid       
	DECLARE @tBL_mainList TABLE (Id INT,CountId INT,RowId INT)      
       
	SET @IsAssociated = CASE WHEN @IsAssociated = 0 THEN 1  WHEN @IsAssociated = 1 THEN 0 END       
       
		
	INSERT INTO @ProductListIdRTR      
	EXEC Znode_GetCategoryList  @IsAssociated,@TransferPimCategoryId      
        
 
	DECLARE @CategoryIDS NVARCHAR(2000) = SUBSTRING((SELECT ','+CAST(ID AS VARCHAR(200)) FROM @ProductListIdRTR FOR XML PATH('')), 2, 4000)      
 
       
	BEGIN      
        
	SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'PimCategoryId','CategoryName')      
	SET @order_by = REPLACE(@order_by,'PimCategoryId','CategoryName')      
      
	INSERT INTO @TBL_MainList(id,CountId,RowId)      
	EXEC Znode_GetCategoryIdForPaging @WhereClause , @Rows , @PageNo , @Order_BY , @RowsCount , @LocaleId , @AttributeCode , @CategoryIDS , @IsAssociated;      
       
	END       
	INSERT INTO #TBL_ProductIdTable(PimCategoryId,RowId)       
	SELECT ID ,RowId FROM @TBL_MainList SP       
      
	INSERT INTO @PimProductIds ( Id )      
	SELECT Id FROM @TBL_MainList SP      
      
	UPDATE #TBL_ProductIdTable SET PimProductId = @PimProductIdInput;      
	SET @PimAttributeId = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50))   
	FROM [dbo].[Fn_GetGridPimCategoryAttributes]() FOR XML PATH('')), 2, 4000);      
                   
	INSERT INTO #TBL_AttributeDetails(PimCategoryAttributeValueId,PimCategoryId, AttributeValue,AttributeCode,PimAttributeId)      
	EXEC Znode_GetCategoryAttributeValueId @PimProductIds,@PimAttributeId,@LocaleId;      
                 
      
	;WITH Cte_ProductMedia      
	AS (
		SELECT TBA.PimCategoryId , TBA.PimAttributeId       
		, SUBSTRING( ( SELECT ','+ISNULL(ZMC.CDNURL,ZMC.URL)+ZMSM.ThumbnailFolderName+'/'+ zm.PATH             
		FROM ZnodeMedia AS ZM      
		INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)      
		INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)      
		INNER JOIN #TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)) )      
		INNER JOIN  #TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)      
		WHERE TBAI.PimCategoryId = TBA.PimCategoryId AND TBAI.PimAttributeId = TBA.PimAttributeId       
		FOR XML PATH('') ), 2 , 4000) AS AttributeValue , SUBSTRING( ( SELECT ','+AttributeValue      
		FROM  #TBL_AttributeDetails AS TBAI      
		WHERE TBAI.PimCategoryId = TBA.PimCategoryId AND TBAI.PimAttributeId = TBA.PimAttributeId       
		FOR XML PATH('') ), 2 , 4000) MediaIds        
		FROM #TBL_AttributeDetails AS TBA       
		INNER JOIN  #TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBA.PimATtributeId )      
     )                           
	UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue      
	FROM #TBL_AttributeDetails TBAV       
	INNER JOIN Cte_ProductMedia CTPM ON CTPM.PimCategoryId = TBAV.PimCategoryId  AND CTPM.PimAttributeId = TBAV.PimAttributeId       
	AND CTPM.PimAttributeId = TBAV.PimAttributeId;      
          
      
	SELECT ISNULL(ZPCP.PimCategoryProductId,0) AS PimCategoryProductId, zpp.[PimCategoryId] AS [Categoryid],zpp.[PimCategoryId],ISNULL(ZPCP.[PimProductId],0) AS PimProductId,[CategoryName],[CategoryCode],      
	CASE WHEN ZPCP.Status IS NULL THEN CAST(0 AS BIT) ELSE CAST(ZPCP.Status AS BIT) END AS [Status],      
	piv.[CategoryImage] [ImagePath],ZPCP.DisplayOrder       
          
	FROM #TBL_ProductIdTable AS zpp      
	LEFT JOIN ZnodePimCategoryProduct ZPCP ON(ZPCP.PimProductId = Zpp.PimProductId AND ZPCP.PimCategoryId = Zpp.PimCategoryId)      
	INNER JOIN (SELECT PimCategoryId,AttributeValue,AttributeCode FROM #TBL_AttributeDetails) TB      
	PIVOT(MAX([AttributeValue])       
	FOR [AttributeCode] IN([CategoryName],[CategoryCode],[IsActive],[CategoryImage])) AS Piv ON(Piv.[PimCategoryId] = zpp.[PimCategoryId])      
	ORDER BY CASE WHEN @Order_By LIKE '% DESC%' THEN CASE WHEN @OrderByDisplay = 1 THEN ZPCP.DisplayOrder       
	WHEN @OrderByDisplay = 2 THEN ZPCP.Status ELSE 1 END ELSE 1 END DESC,      
	CASE WHEN @Order_By LIKE '% ASC%' THEN CASE WHEN @OrderByDisplay = 1 THEN ZPCP.DisplayOrder      
	WHEN @OrderByDisplay = 2 THEN ZPCP.Status ELSE 1 END ELSE 1 END,zpp.RowId;      
        
	SELECT @RowsCount=ISNULL((SELECT top 1 countId FROM @TBL_MainList),0)     
    
      
      
END TRY      
BEGIN CATCH      
	SELECT ERROR_MESSAGE()      
	DECLARE @Status BIT ;      
	SET @Status = 0;      
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),      
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimProductCategoryList @WhereClause = '+  
	CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '  
	+CAST(@LocaleId AS VARCHAR(50))+',@PimProductIdInput='+CAST(@PimProductIdInput AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@RowsCount='+  
	CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));      
                        
	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                          
          
	EXEC Znode_InsertProcedureErrorLog      
	@ProcedureName = 'Znode_GetPimProductCategoryList',      
	@ErrorInProcedure = @Error_procedure,      
	@ErrorMessage = @ErrorMessage,      
	@ErrorLine = @ErrorLine,      
	@ErrorCall = @ErrorCall;      
END CATCH;      
END;