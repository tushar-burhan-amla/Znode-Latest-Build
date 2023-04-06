CREATE  PROCEDURE [dbo].[Znode_GetProductsAttributeValue_Rohit]
(   @PimProductId  VARCHAR(MAX),
    @AttributeCode VARCHAR(MAX),
    @LocaleId      INT = 0,
	@IsPublish bit = 0  )
AS
/* 
    
     Summary:- This Procedure is used to get the product attribute values 
			   The result is fetched from all locale for ProductId provided
     Unit Testing 
     EXEC Znode_GetProductsAttributeValue_Rohit '2146','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',0
	 SELECT * FROM ZnodePIMProduct
	 EXEC Znode_GetProductsAttributeValue '121','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',1
	 
	 EXEC Znode_GetProductsAttributeValue '121','ProductName,SKU,Price,Quantity,IsActive,ProductType,Image,Assortment,DisplayOrder,Style,Material',2,@IsPublish =1 
    
*/	
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
			 		
				DECLARE @TBL_AttributeValue TABLE (PimAttributeValueId INT,PimProductId INT,AttributeValue NVARCHAR(MAX),PimAttributeId INT)
				DECLARE @TBL_AttributeDefault TABLE (PimAttributeId INT,AttributeDefaultValueCode VARCHAR(100),IsEditable BIT,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder INT)
				DECLARE @DefaultLocaleId INT = DBO.FN_GetDefaultLocaleId()
				DECLARE @TBL_MediaValue TABLE (PimAttributeValueId INT,PimProductId INT,MediaPath NVARCHAR(MAX),PimAttributeId INT ,LocaleId INT )
				DECLARE @TBL_PimProductId TABLE (PimProductId INT)
				DECLARE @Cte_GetDefaultData TABLE (PimAttributeValueId INT,PimAttributeDefaultValueId int,LocaleId int )

				Declare @Cte_AttributeLocaleComma TABLE (PimAttributeValueId INT,AttributeDefaultValue nvarchar(4000),LocaleId int,Cnt int )


				    insert into @Cte_GetDefaultData
				    SELECT ZPPADV.PimAttributeValueId ,ZPADVL.PimAttributeDefaultValueId ,ZPADVL.LocaleId 
					FROM ZnodePimAttributeValue ZPAV 
					INNER JOIN ZnodePimProductAttributeDefaultValue ZPPADV ON (ZPAV.PimAttributeValueId = ZPPADV.PimAttributeValueId)
					INNER JOIN ZnodePimAttributeDefaultValueLocale ZPADVL ON (ZPPADV.PimAttributeDefaultValueId = ZPADVL.PimAttributeDefaultValueId )
					Where  ZPADVL.LocaleId in (@DefaultLocaleId, @LocaleId )

					--If @DefaultLocaleId!=@LocaleId
					--	Delete a 
					--	From @Cte_GetDefaultData a
					--	WHere  LocaleId=@DefaultLocaleId 
					--	AND NOT EXISTS (SELECT TOP 1 1 FROM @Cte_GetDefaultData b WHERE b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId= @LocaleId)

    --            insert into @Cte_AttributeLocaleComma
				--(PimAttributeValueId,AttributeDefaultValue,LocaleId,Cnt)
				--SELECT  PimAttributeValueId,LocaleId ,Count(1)		
				--FROM @Cte_GetDefaultData  CTEA 
				--Group by PimAttributeValueId,LocaleId


				Select * from @Cte_GetDefaultData

					
					
				--update	a
				--Set AttributeDefaultValue =isnull(a.AttributeDefaultValue,'') +','+b.AttributeDefaultValue
				--from  @Cte_AttributeLocaleComma a
				--inner join @Cte_GetDefaultData b on  a.PimAttributeValueId = b.PimAttributeValueId 
					
									
				INSERT INTO @TBL_PimProductId 
				SELECT Item FROM dbo.Split( @PimProductId, ',' ) AS SP 
				
				INSERT INTO @TBL_MediaValue
					SELECT ZPAV.PimAttributeValueId	
							,PimProductId
							,ZPPAM.MediaId MediaPath
							,ZPAV.PimAttributeId , ZPPAM.LocaleId
					FROM ZnodePimProductAttributeMedia ZPPAM 
					INNER JOIN ZnodePimAttributeValue ZPAV ON ( ZPPAM.PimAttributeValueId = ZPAV.PimAttributeValueId)
					LEFT JOIN ZnodeMedia ZM ON (Zm.Path = ZPPAM.MediaPath)  
				
				
				--;WITH Cte_AttributeValueDefault AS 
				--(
				-- SELECT PimAttributeValueId ,AttributeDefaultValue ,@DefaultLocaleId LocaleId 
				-- FROM @Cte_GetDefaultData 
				-- WHERE LocaleId = @LocaleId 
				-- UNION  
				-- SELECT PimAttributeValueId ,AttributeDefaultValue ,@DefaultLocaleId LocaleId 
				-- FROM @Cte_GetDefaultData a 
				-- WHERE LocaleId = @DefaultLocaleId 
				-- AND NOT EXISTS (SELECT TOP 1 1 FROM @Cte_GetDefaultData b WHERE b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId= @LocaleId)
    -- 			)
				--,Cte_AttributeLocaleComma 
				--AS 
				--(
				--SELECT DISTINCT PimAttributeValueId ,SUBSTRING ((SELECT ',' + AttributeDefaultValue 
				--									FROM Cte_AttributeValueDefault CTEAI 
				--									WHERE CTEAI.PimAttributeValueId = CTEA.PimAttributeValueId 
				--									FOR XML PATH ('')   ),2,4000) AttributeDefaultValue , LocaleId
				
				--FROM Cte_AttributeValueDefault  CTEA 
				--)
				--,Cte_AllAttributeData AS 
				--(
				--	SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,ZPPATV.AttributeValue,ZPAV.PimAttributeId,ZPPATV.LocaleId
				--	FROM ZnodePimAttributeValue ZPAV
				--	INNER join ZnodePimProductAttributeTextAreaValue ZPPATV ON (ZPPATV.PimAttributeValueId= ZPAV.PimAttributeValueId)
				--	INNER JOIN @TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
				--	UNION ALL
					
				--	SELECT PimAttributeValueId,TBM.PimProductId
				--			,MediaPath
				--			,PimAttributeId,LocaleId
				--	from @TBL_PimProductId TBPP   
				--	INNER JOIN @TBL_MediaValue TBM ON (TBM.PimProductId = TBPP.PimProductId)

				--	UNION ALL 
				--	SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,ZPAVL.AttributeValue,ZPAV.PimAttributeId,ZPAVL.LocaleId
				--	FROM ZnodePimAttributeValue ZPAV
				--	INNER JOIN ZnodePimAttributeValueLocale  ZPAVL ON ( ZPAVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
				--	INNER JOIN @TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
				--	UNION ALL
				--	SELECT ZPAV.PimAttributeValueId,ZPAV.PimProductId,CS.AttributeDefaultValue,ZPAV.PimAttributeId,LocaleId
				--	FROM ZnodePimAttributeValue ZPAV
				--	INNER JOIN Cte_AttributeLocaleComma CS ON (ZPAV.PimAttributeValueId = CS.PimAttributeValueId)
				--	INNER JOIN @TBL_PimProductId TBPP ON (ZPAV.PimProductId = TBPP.PimProductId)
				--)
				--, Cte_AttributeFirstLocal AS 
				--(
				--	SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
				--	FROM Cte_AllAttributeData
				--	WHERE LocaleId = @LocaleId
				--)
				--,Cte_DefaultAttributeValue AS 
				--(
				--	SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
				--	FROM Cte_AttributeFirstLocal
				--	UNION ALL 
				--	SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
				--	FROM Cte_AllAttributeData CTAAD
				--	WHERE LocaleId = @DefaultLocaleId
				--	AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_AttributeFirstLocal CTRT WHERE CTRT.PimAttributeValueId = CTAAD.PimAttributeValueId   )
			 --	)



				--INSERT INTO @TBL_AttributeValue
				--SELECT PimAttributeValueId,PimProductId,AttributeValue,PimAttributeId
				--FROM  Cte_DefaultAttributeValue 

				

				--If @IsPublish = 1 
				--Begin
				
				--	DECLARE @Tlb_ReadMultiSelectValue TABLE ( AttributeDefaultValueCode NVARCHAR(300),PimAttributeId INT)
				--	INSERT INTO @Tlb_ReadMultiSelectValue ( AttributeDefaultValueCode ,PimAttributeId ) 
				--	SELECT zpav.AttributeDefaultValueCode,zpa.PimAttributeId 
				--	   FROM ZnodePimAttributeDefaultValue AS zpav 
				--	   RIGHT  OUTER JOIN dbo.ZnodePimAttribute AS zpa ON zpav.PimAttributeId = zpa.PimAttributeId
				--	   INNER JOIN dbo.ZnodeAttributeType AS zat ON zpa.AttributeTypeId = zat.AttributeTypeId
				--	   WHERE 
				--	   zat.AttributeTypeName IN ('Multi Select')
    --                union All 
				--	 Select ZPA.AttributeCode,ZPA.PimAttributeId   from ZnodePimAttributeValidation ZPAV INNER JOIN ZnodeAttributeInputValidation ZAIV 
				--	ON ZPAV.InputValidationId = ZAIV.InputValidationId 
				--	INNER JOIN ZnodePimAttribute ZPA ON ZPAV.PimAttributeId = ZPA.PimAttributeId
				--	where ZAIV.Name  in ('IsAllowMultiUpload') and ltrim(rtrim(ZPAV.Name)) = 'true'
	
				--	SELECT PimProductId,AttributeValue,ZPA.AttributeCode,TBAV.PimAttributeId FROM @TBL_AttributeValue TBAV
				--	INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
				--	WHERE NOT Exists (Select TOP 1 1 FROM @Tlb_ReadMultiSelectValue where PimAttributeId = TBAV.PimAttributeId) 
				--	UNION ALL 
				--	Select PimProductId, SUBSTRING((Select ','+ CAST(ZPAXML.AttributeValue  AS VARCHAR(50)) from @TBL_AttributeValue ZPAXML where  
				--	ZPAXML.PimProductId = TBAV.PimProductId AND ZPAXML.PimAttributeId = TBAV.PimAttributeId FOR XML PATH('') ), 2, 4000) 
				--	AttributeValue, ZPA.AttributeCode,TBAV.PimAttributeId 
				--	FROM @TBL_AttributeValue TBAV
				--	INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
				--	WHERE Exists (Select TOP 1 1 FROM @Tlb_ReadMultiSelectValue where PimAttributeId = TBAV.PimAttributeId  ) 
				--	GROUP BY TBAV.PimProductId ,TBAV.PimAttributeId ,ZPA.AttributeCode
			
				--End
				--Else 
				--Begin	
				--	SELECT PimProductId, AttributeValue,ZPA.AttributeCode,TBAV.PimAttributeId 
				--	FROM @TBL_AttributeValue TBAV
				--	INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = TBAV.PimAttributeId)
				--End
		 END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
			SET @Status = 0;
		 END CATCH;
     END;