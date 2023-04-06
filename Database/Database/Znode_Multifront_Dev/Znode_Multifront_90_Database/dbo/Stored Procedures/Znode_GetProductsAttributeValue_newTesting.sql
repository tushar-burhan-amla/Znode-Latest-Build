CREATE   PROCEDURE [dbo].[Znode_GetProductsAttributeValue_newTesting]  
(   @PimProductId  transferid readonly ,  
    @AttributeId transferid readonly,  
    @LocaleId      INT = 0,  
 @IsPublish bit = 0  )  
AS  
/*   
      
     Summary:- This Procedure is used to get the product attribute values   
      The result is fetched from all locale for ProductId provided  
     Unit Testing   
     EXEC Znode_GetProductsAttributeValue_1 '2146','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',0  
  SELECT * FROM ZnodePIMProduct  
  DECLARE @Tyr TransferId   
  ,  @Tyr1   TransferId   
  INSERT INTO @Tyr   
  SELECT   
  EXEC Znode_GetProductsAttributeValue '121','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',1  
    
  EXEC Znode_GetProductsAttributeValue '121','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',2,@IsPublish =1   
      
*/   
  BEGIN  
         BEGIN TRY  
             SET NOCOUNT ON;  
        
   DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleID()  
    
           SELECT PimAttributeValueId , ZPAV.PimAttributeId , PimProductId,AttributeCode   
     INTO #TBL_AttributeValue1  
     FROM ZnodePimAttributeValue ZPAV   
     INNER JOIN  ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)    
     WHERE EXISTS (SELECT TOP 1 1 FROM @PimProductId TBLP WHERE TBLP.Id = ZPAV.PimProductId )  
     AND EXISTS (SELECT TOP 1 1 FROM @AttributeId TBLA WHERE TBLA.Id = ZPAV.PimAttributeId  )  
     
   CREATE TABLE #TBL_AttributeValue  (PimAttributeValueId INT  , PimAttributeId  INT , PimProductId INT ,AttributeCode NVARCHAR(600),AttributeValue NVARCHAR(max),TypeOfData INT   
      ,PimAttributeValueLocaleId INT ,LocaleId INT , RowId INT,PimAttributeDefaultValueId INT  )  
          INSERT INTO #TBL_AttributeValue (PimAttributeValueId   , PimAttributeId   , PimProductId  ,AttributeCode ,AttributeValue ,TypeOfData    
      ,PimAttributeValueLocaleId  ,LocaleId  , RowId    )     
    SELECT TBLAV.PimAttributeValueId , TBLAV.PimAttributeId , PimProductId,AttributeCode,ZPAVL.AttributeValue,1 TypeOfData  
      ,ZPAVL.ZnodePimAttributeValueLocaleId PimAttributeValueLocaleId,LocaleId,COUNT(*)Over(Partition By TBLAV.PimProductId,PimAttributeId ORDER BY TBLAV.PimProductId,PimAttributeId  ) RowId  
        FROM ZnodePimAttributeValueLocale  ZPAVL   
    INNER JOIN #TBL_AttributeValue1 TBLAV ON (TBLAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)  
    WHERE LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId   
     
    INSERT INTO #TBL_AttributeValue (PimAttributeValueId   , PimAttributeId   , PimProductId  ,AttributeCode ,AttributeValue ,TypeOfData    
      ,PimAttributeValueLocaleId  ,LocaleId  , RowId )  
    SELECT TBLAV.PimAttributeValueId , TBLAV.PimAttributeId , PimProductId,AttributeCode,ZPAVL.AttributeValue,1 TypeOfData  
      ,ZPAVL.PimProductAttributeTextAreaValueId PimAttributeValueLocaleId,LocaleId,COUNT(*)Over(Partition By TBLAV.PimProductId,PimAttributeId ORDER BY TBLAV.PimProductId,PimAttributeId  ) RowId  
    FROM ZnodePimProductAttributeTextAreaValue  ZPAVL   
    INNER JOIN #TBL_AttributeValue1 TBLAV ON (TBLAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)  
    WHERE LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId   
     
    INSERT INTO #TBL_AttributeValue (PimAttributeValueId   , PimAttributeId   , PimProductId  ,AttributeCode ,AttributeValue ,TypeOfData    
      ,PimAttributeValueLocaleId  ,LocaleId  , RowId ,PimAttributeDefaultValueId   )  
    SELECT TBLAV.PimAttributeValueId , TBLAV.PimAttributeId , PimProductId,AttributeCode,  
           CAST( ZPAVL.PimAttributeDefaultValueId AS VARCHAR(2000)),2 TypeOfData  
      ,ZPAVL.PimProductAttributeDefaultValueId PimAttributeValueLocaleId,LocaleId  
      ,COUNT(*)Over(Partition By TBLAV.PimProductId,PimAttributeId ORDER BY TBLAV.PimProductId,PimAttributeId  ) RowId  
      ,PimAttributeDefaultValueId  
    FROM ZnodePimProductAttributeDefaultValue  ZPAVL   
    INNER JOIN #TBL_AttributeValue1 TBLAV ON (TBLAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)  
    WHERE LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId   
      
    INSERT INTO #TBL_AttributeValue (PimAttributeValueId   , PimAttributeId   , PimProductId  ,AttributeCode ,AttributeValue ,TypeOfData    
      ,PimAttributeValueLocaleId  ,LocaleId  , RowId   )  
    SELECT TBLAV.PimAttributeValueId , TBLAV.PimAttributeId , PimProductId,AttributeCode,CAST( ZPAVL.MediaId AS VARCHAR(2000)) ,3 TypeOfData  
      ,ZPAVL.PimProductAttributeMediaId PimAttributeValueLocaleId,ZPAVL.LocaleId,COUNT(*)Over(Partition By TBLAV.PimProductId,PimAttributeId ORDER BY TBLAV.PimProductId,PimAttributeId  ) RowId  
    FROM ZnodePimProductAttributeMedia  ZPAVL   
    INNER JOIN #TBL_AttributeValue1 TBLAV ON (TBLAV.PimAttributeValueId = ZPAVL.PimAttributeValueId)  
    WHERE LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId   
      
     SELECT ZPPADV.PimAttributeValueId   
     ,ZPADVL.AttributeDefaultValue AttributeDefaultValue ,ZPADVL.localeID LocaleId ,ZPPADV.PimProductId   
     ,ZPPADV.AttributeCode,ZPPADV.PimAttributeId,TEY.AttributeDefaultValueCode    
     INTO #Cte_GetDefaultData
	 FROM #TBL_AttributeValue ZPPADV   
     INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPPADV.PimAttributeValueId)  
     INNER JOIN ZnodePimAttributeDefaultValue TEY ON (TEY.PimAttributeDefaultValueId  = ZPPADV.PimAttributeDefaultValueId )  
     INNER JOIN ZnodePimAttributeDefaultValuelocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = TEY.PimAttributeDefaultValueId )  
     WHERE ZPADVL.localeID  IN (@LocaleId,@DefaultLocaleId)  
     AND TypeOfData = 2   
     AND ZPPADV.LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
     
	 SELECT AttributeDefaultValue  ,PimProductId ,AttributeCode,PimAttributeId ,AttributeDefaultValueCode   
     INTO #Cte_AttributeValueDefault
	 FROM #Cte_GetDefaultData   
     WHERE LocaleId = @LocaleId   
     UNION    
     SELECT  AttributeDefaultValue  ,PimProductId ,AttributeCode,PimAttributeId,AttributeDefaultValueCode   
     FROM #Cte_GetDefaultData a   
     WHERE LocaleId = @DefaultLocaleId   
     AND NOT EXISTS (SELECT TOP 1 1 FROM #Cte_GetDefaultData b WHERE b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId= @LocaleId)  
     
    SELECT  PimProductId, AttributeValue,  AttributeCode,  PimAttributeId, NULL AttributeDefaultValue    
    FROM  #TBL_AttributeValue   
             WHERE LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
    AND TypeOfData = 1    
  
    UNION ALL   
  
    SELECT DISTINCT PimProductId,  
            SUBSTRING ((SELECT ','+[dbo].[Fn_GetMediaThumbnailMediaPath]( zm.PATH) FROM ZnodeMedia ZM WHERE ZM.MediaId = TBLAV.AttributeValue   
      FOR XML PATH (''), TYPE).value('.', 'varchar(Max)') ,2,4000)  ,AttributeCode,PimAttributeId, NULL AttributeDefaultValue  
    FROM #TBL_AttributeValue TBLAV   
    WHERE  LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
    AND TypeOfData = 3  
    UNION ALL  
   SELECT DISTINCT PimProductId,
			 SUBSTRING ((SELECT ',' + AttributeDefaultValue 
													FROM #Cte_AttributeValueDefault CTEAI 
													WHERE CTEAI.PimProductId = CTEA.PimProductId 
													AND CTEAI.PimAttributeId = CTEA.PimAttributeId
													FOR XML PATH ('')   ),2,4000) AttributeDefaultValue,AttributeCode ,PimAttributeId
			 ,SUBSTRING ((SELECT ',' + AttributeDefaultValueCode 
													FROM #Cte_AttributeValueDefault CTEAI1 
													WHERE CTEAI1.PimProductId = CTEA.PimProductId 
													AND CTEAI1.PimAttributeId = CTEA.PimAttributeId
													FOR XML PATH ('')   ),2,4000) AttributeDefaultValue
				
			FROM #Cte_AttributeValueDefault  CTEA 
     
    
   END TRY  
         BEGIN CATCH  
    DECLARE @Status BIT ;
			SET @Status = 0;
			
      SELECT ERROR_MESSAGE()  
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductsAttributeValue_newTesting 
			,@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProductsAttributeValue_newTesting',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall; 
   
         END CATCH;  
     END;