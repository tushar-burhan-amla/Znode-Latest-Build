
CREATE PROCEDURE [dbo].[Znode_GetProductsAttributeValue]
(   @PimProductId   TransferId READONLY,
    @AttributeCode VARCHAR(MAX),
    @LocaleId      INT = 0,
	@IsPublish bit = 0  )
AS
/* 
    
     Summary:- This Procedure is used to get the product attribute values 
			   The result is fetched from all locale for ProductId provided
     Unit Testing 
     EXEC Znode_GetProductsAttributeValue_1 '2146','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',0
	 SELECT * FROM ZnodePIMProduct
	 
*/	
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 	
				if object_id('tempdb..#TBL_PimProductId') is not null
					drop table #TBL_PimProductId	

				if object_id('tempdb..#TBL_AttributeValue_Internal') is not null
					drop table #TBL_AttributeValue_Internal

				CREATE TABLE #TBL_AttributeValue_Internal  (PimAttributeValueId INT,PimProductId INT,AttributeValue NVARCHAR(MAX),PimAttributeId INT)
				DECLARE @TBL_AttributeDefault TABLE (PimAttributeId INT,AttributeDefaultValueCode VARCHAR(100),IsEditable BIT,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder INT)
				DECLARE @DefaultLocaleId INT = DBO.FN_GetDefaultLocaleId()
				CREATE TABLE  #TBL_MediaValue (PimAttributeValueId INT,PimProductId INT,MediaPath NVARCHAR(MAX),PimAttributeId INT ,LocaleId INT )
				
				CREATE TABLE #TBL_PimProductId  (PimProductId INT)
							
				INSERT INTO #TBL_PimProductId 
				SELECT Id FROM @PimProductId
				
				INSERT INTO #TBL_MediaValue
					SELECT ZPAV.PimAttributeValueId	
							,PimProductId
							,ZPPAM.MediaId MediaPath
							,ZPAV.PimAttributeId , ZPPAM.LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN ZnodePimProductAttributeMedia ZPPAM ON ( ZPPAM.PimAttributeValueId = ZPAV.PimAttributeValueId)
					LEFT JOIN ZnodeMedia ZM ON (Zm.MediaId = ZPPAM.MediaId)  
					WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBPP  WHERE TBPP.PimProductId = ZPAV.PimProductId)		
					
					SELECT ZPPADV.PimAttributeValueId ,ZPADVL.AttributeDefaultValue ,ZPADVL.LocaleId 
					INTO #Cte_GetDefaultData
					FROM ZnodePimProductAttributeDefaultValue ZPPADV 
					INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPPADV.PimAttributeValueId)
					INNER JOIN ZnodePimAttributeDefaultValueLocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )
					WHERE ZPADVL.LocaleID IN (@LocaleId,@DefaultLocaleId)
					AND EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBPP  WHERE TBPP.PimProductId = ZPAV.PimProductId)
				
				
				;WITH Cte_AttributeValueDefault AS 
				(
				 SELECT PimAttributeValueId ,AttributeDefaultValue ,@DefaultLocaleId LocaleId 
				 FROM #Cte_GetDefaultData 
				 WHERE LocaleId = @LocaleId 
				 UNION  
				 SELECT PimAttributeValueId ,AttributeDefaultValue ,@DefaultLocaleId LocaleId 
				 FROM #Cte_GetDefaultData a 
				 WHERE LocaleId = @DefaultLocaleId 
				 AND NOT EXISTS (SELECT TOP 1 1 FROM #Cte_GetDefaultData b WHERE b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId= @LocaleId)
     			)
				
				SELECT DISTINCT PimAttributeValueId ,SUBSTRING ((SELECT ',' + AttributeDefaultValue 
													FROM Cte_AttributeValueDefault CTEAI 
													WHERE CTEAI.PimAttributeValueId = CTEA.PimAttributeValueId 
													FOR XML PATH ('')   ),2,4000) AttributeDefaultValue , LocaleId
				
				INTO #Cte_AttributeLocaleComma 
				FROM Cte_AttributeValueDefault  CTEA 
								
					SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,ZPPATV.AttributeValue,ZPAV.PimAttributeId,ZPPATV.LocaleId
					INTO #Cte_AllAttributeData
					FROM ZnodePimAttributeValue ZPAV
					INNER join ZnodePimProductAttributeTextAreaValue ZPPATV ON (ZPPATV.PimAttributeValueId= ZPAV.PimAttributeValueId)
					INNER JOIN #TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
					UNION ALL
					
					SELECT PimAttributeValueId,TBM.PimProductId
							,MediaPath
							,PimAttributeId,LocaleId
					from #TBL_PimProductId TBPP   
					INNER JOIN #TBL_MediaValue TBM ON (TBM.PimProductId = TBPP.PimProductId)

					UNION ALL 
					SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,ZPAVL.AttributeValue,ZPAV.PimAttributeId,ZPAVL.LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN ZnodePimAttributeValueLocale  ZPAVL ON ( ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
					INNER JOIN #TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
					INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
					WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@AttributeCode,',') SP WHERE (SP.Item = ZPA.AttributeCode  OR SP.Item = CAST(ZPA.PimATtributeId  AS VARCHAR(50)) )) 
					AND ZPAVL.LocaleId IN (@LocaleId,@DefaultLocaleId)
					 
					UNION ALL
					SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,CS.AttributeDefaultValue,ZPAV.PimAttributeId,LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN #Cte_AttributeLocaleComma CS ON (ZPAV.PimAttributeValueId = CS.PimAttributeValueId)
					INNER JOIN #TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
				
				;With Cte_AttributeFirstLocal AS 
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

				INSERT INTO #TBL_AttributeValue_Internal
				SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
				FROM  Cte_DefaultAttributeValue 
				
				If @IsPublish = 1 
				Begin
				
					DECLARE @Tlb_ReadMultiSelectValue TABLE ( AttributeDefaultValueCode NVARCHAR(300),PimAttributeId INT)
					INSERT INTO @Tlb_ReadMultiSelectValue ( AttributeDefaultValueCode ,PimAttributeId ) 
					SELECT zpav.AttributeDefaultValueCode,zpa.PimAttributeId 
					   FROM ZnodePimAttributeDefaultValue AS zpav 
					   RIGHT  OUTER JOIN dbo.ZnodePimAttribute AS zpa ON zpav.PimAttributeId = zpa.PimAttributeId
					   INNER JOIN dbo.ZnodeAttributeType AS zat ON zpa.AttributeTypeId = zat.AttributeTypeId
					   WHERE 
					   zat.AttributeTypeName IN ('Multi Select')
                    union All 
					 Select ZPA.AttributeCode,ZPA.PimAttributeId   from ZnodePimAttributeValidation ZPAV INNER JOIN ZnodeAttributeInputValidation ZAIV 
					ON ZPAV.InputValidationId = ZAIV.InputValidationId 
					INNER JOIN ZnodePimAttribute ZPA ON ZPAV.PimAttributeId = ZPA.PimAttributeId
					where ZAIV.Name  in ('IsAllowMultiUpload') and ltrim(rtrim(ZPAV.Name)) = 'true'
	
					SELECT PimProductId,AttributeValue,ZPA.AttributeCode,TBAV.PimAttributeId FROM #TBL_AttributeValue_Internal TBAV
					INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
					WHERE NOT Exists (Select TOP 1 1 FROM @Tlb_ReadMultiSelectValue where PimAttributeId = TBAV.PimAttributeId) 
					UNION ALL 
					Select PimProductId, SUBSTRING((Select ','+ CAST(ZPAXML.AttributeValue  AS VARCHAR(50)) from #TBL_AttributeValue_Internal ZPAXML where  
					ZPAXML.PimProductId = TBAV.PimProductId AND ZPAXML.PimAttributeId = TBAV.PimAttributeId FOR XML PATH('') ), 2, 4000) 
					AttributeValue, ZPA.AttributeCode,TBAV.PimAttributeId 
					FROM #TBL_AttributeValue_Internal TBAV
					INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
					WHERE Exists (Select TOP 1 1 FROM @Tlb_ReadMultiSelectValue where PimAttributeId = TBAV.PimAttributeId  ) 
					GROUP BY TBAV.PimProductId ,TBAV.PimAttributeId ,ZPA.AttributeCode
			
				End
				Else 
				Begin	
					SELECT PimProductId, AttributeValue,ZPA.AttributeCode,TBAV.PimAttributeId 
					FROM #TBL_AttributeValue_Internal TBAV
					INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
					WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@AttributeCode,',') SP WHERE (SP.Item = ZPA.AttributeCode  OR SP.Item = CAST(ZPA.PimATtributeId  AS VARCHAR(50)) )) 
				End

				if object_id('tempdb..#TBL_PimProductId') is not null
					drop table #TBL_PimProductId	

				if object_id('tempdb..#TBL_AttributeValue_Internal') is not null
					drop table #TBL_AttributeValue_Internal

		 END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			@ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductsAttributeValue 
			@AttributeCode='+@AttributeCode+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProductsAttributeValue',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;