CREATE  PROCEDURE [dbo].[Znode_GetProductsAttributeValueWithCode]
(   
	@PimProductId  VARCHAR(MAX),
    @AttributeCode VARCHAR(MAX),
    @LocaleId      INT = 0,
	@IsPublish bit = 0  
)
AS
/* 
    
     Summary:- This Procedure is used to get the product attribute values 
			   The result is fetched from all locale for ProductId provided
     Unit Testing 
     EXEC Znode_GetProductsAttributeValue_1 '2146','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',0
	 SELECT * FROM ZnodePIMProduct
	 EXEC Znode_GetProductsAttributeValue '121','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',1
	 
	 EXEC Znode_GetProductsAttributeValueWithCode '5','highlight',,@IsPublish =1 
    
*/	
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 		
				DECLARE @TBL_AttributeValue TABLE (PimAttributeValueId INT,PimProductId INT,AttributeValue NVARCHAR(MAX),PimAttributeId INT)
				DECLARE @TBL_AttributeDefault TABLE (PimAttributeId INT,AttributeDefaultValueCode VARCHAR(100),IsEditable BIT,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder INT)
				DECLARE @DefaultLocaleId INT = DBO.FN_GetDefaultLocaleId()
				CREATE TABLE #TBL_MediaValue  (PimAttributeValueId INT,PimProductId INT,MediaPath NVARCHAR(MAX),PimAttributeId INT ,LocaleId INT )
				DECLARE @TBL_PimProductId TABLE (PimProductId INT)
					
				
				INSERT INTO @TBL_PimProductId 
				SELECT Item 
				FROM dbo.Split( @PimProductId, ',' ) AS SP 
				
				INSERT INTO #TBL_MediaValue
					SELECT ZPAV.PimAttributeValueId	
							,PimProductId
							,ZPPAM.MediaId MediaPath
							,ZPAV.PimAttributeId 
							,ZPPAM.LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN ZnodePimProductAttributeMedia ZPPAM ON ( ZPPAM.PimAttributeValueId = ZPAV.PimAttributeValueId)
					LEFT JOIN ZnodeMedia ZM ON (Zm.Path = ZPPAM.MediaPath)  
				

				--;WITH Cte_GetDefaultData 
				--AS 
				--(
					SELECT ZPPADV.PimAttributeValueId ,ZPADVL.AttributeDefaultValueCode AttributeDefaultValue ,ZPPADV.LocaleId LocaleId
					INTO #Cte_GetDefaultData
					FROM ZnodePimProductAttributeDefaultValue ZPPADV 
					INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPPADV.PimAttributeValueId)
					INNER JOIN ZnodePimAttributeDefaultValue ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )
				--)
				;WITH Cte_AttributeValueDefault AS 
				(
				 SELECT Distinct PimAttributeValueId ,AttributeDefaultValue ,@DefaultLocaleId LocaleId 
				 FROM #Cte_GetDefaultData 
				 WHERE LocaleId = @LocaleId 
				 UNION  ALL
				 SELECT PimAttributeValueId ,AttributeDefaultValue ,@DefaultLocaleId LocaleId 
				 FROM #Cte_GetDefaultData a 
				 WHERE LocaleId = @DefaultLocaleId 
				 AND NOT EXISTS (SELECT TOP 1 1 FROM #Cte_GetDefaultData b WHERE b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId= @LocaleId)
     			)

			
				,Cte_AttributeLocaleComma 
				AS 
				(
				SELECT DISTINCT PimAttributeValueId ,SUBSTRING ((SELECT ',' + AttributeDefaultValue 
													FROM Cte_AttributeValueDefault CTEAI 
													WHERE CTEAI.PimAttributeValueId = CTEA.PimAttributeValueId 
													FOR XML PATH ('')   ),2,4000) AttributeDefaultValue , LocaleId
				
				FROM Cte_AttributeValueDefault  CTEA 
				)

			

				--,Cte_AllAttributeData AS 
				--(
					SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,ZPPATV.AttributeValue,ZPAV.PimAttributeId,ZPPATV.LocaleId
					INTO #Cte_AllAttributeData
					FROM ZnodePimAttributeValue ZPAV
					INNER join ZnodePimProductAttributeTextAreaValue ZPPATV ON (ZPPATV.PimAttributeValueId= ZPAV.PimAttributeValueId)
					INNER JOIN @TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
					UNION ALL
					
					SELECT PimAttributeValueId,TBM.PimProductId
							,MediaPath
							,PimAttributeId,LocaleId
					from @TBL_PimProductId TBPP   
					INNER JOIN #TBL_MediaValue TBM ON (TBM.PimProductId = TBPP.PimProductId)

					UNION ALL 
					SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,ZPAVL.AttributeValue,ZPAV.PimAttributeId,ZPAVL.LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN ZnodePimAttributeValueLocale  ZPAVL ON ( ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
					INNER JOIN @TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
					UNION ALL
					SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,CS.AttributeDefaultValue,ZPAV.PimAttributeId,LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN Cte_AttributeLocaleComma CS ON (ZPAV.PimAttributeValueId = CS.PimAttributeValueId)
					INNER JOIN @TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
				--)
				;with Cte_AttributeFirstLocal AS 
				(
					SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
					FROM #Cte_AllAttributeData
					WHERE LocaleId = @LocaleId
				)
				,Cte_DefaultAttributeValue AS 
				(
					SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
					FROM Cte_AttributeFirstLocal
					UNION ALL 
					SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
					FROM #Cte_AllAttributeData CTAAD
					WHERE LocaleId = @DefaultLocaleId
					AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_AttributeFirstLocal CTRT WHERE CTRT.PimAttributeValueId = CTAAD.PimAttributeValueId   )
			 	)

				INSERT INTO @TBL_AttributeValue
				SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
				FROM  Cte_DefaultAttributeValue 
			
					SELECT  PimProductId, AttributeValue,ZPA.AttributeCode,TBAV.PimAttributeId 
					FROM @TBL_AttributeValue TBAV
					INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
			
		 END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
			SET @Status = 0;
			--DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			--@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductsAttributeValue_1 @PimProductId = '+@PimProductId+
			--',@AttributeCode='+@AttributeCode+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			--SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			--EXEC Znode_InsertProcedureErrorLog
			--	@ProcedureName = 'Znode_GetProductsAttributeValue_1',
			--	@ErrorInProcedure = @Error_procedure,
			--	@ErrorMessage = @ErrorMessage,
			--	@ErrorLine = @ErrorLine,
			--	@ErrorCall = @ErrorCall;
         END CATCH;
     END;