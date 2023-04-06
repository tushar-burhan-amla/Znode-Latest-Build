CREATE PROCEDURE [dbo].[Znode_GetProductsAttributeValue_newTesting]  
(   
	@PimProductId  transferid readonly ,  
    @AttributeId transferid readonly,  
    @LocaleId      INT = 0,  
	@IsPublish bit = 0, 
	@IsFilesNameRequired Bit =0
)  
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
        
	 --  DECLARE #TBL_AttributeValue1 TABLE (PimAttributeValueId INT , PimAttributeId INT , PimProductId INT,AttributeCode VARCHAR(200)  )  
     DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleID()  
  
	 -- INSERT INTO  (PimAttributeValueId , PimAttributeId , PimProductId,AttributeCode )  
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
    
	 DECLARE @url VARCHAR(1000)
	 SELECT @url= CONCAT (FeatureValues,FeatureSubValues) FROM ZnodeGlobalSetting WHERE FeatureName='ProductImagePath'
			 
     SELECT ZPPADV.PimAttributeValueId   
     ,ZPADVL.AttributeDefaultValue AttributeDefaultValue ,ZPADVL.localeID LocaleId ,ZPPADV.PimProductId   
     ,ZPPADV.AttributeCode,ZPPADV.PimAttributeId,TEY.AttributeDefaultValueCode    
     into #Cte_GetDefaultData
	 FROM #TBL_AttributeValue ZPPADV   
     INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeValueId = ZPPADV.PimAttributeValueId)  
     INNER JOIN ZnodePimAttributeDefaultValue TEY ON (TEY.PimAttributeDefaultValueId  = ZPPADV.PimAttributeDefaultValueId )  
     INNER JOIN ZnodePimAttributeDefaultValuelocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = TEY.PimAttributeDefaultValueId )  
     WHERE ZPPADV.localeID  IN (@LocaleId,@DefaultLocaleId)  
     AND TypeOfData = 2   
     AND ZPPADV.LocaleId  = CASE WHEN RowId >= 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
   
	--insert into Cte_AttributeValueDefault
     SELECT AttributeDefaultValue  ,PimProductId ,AttributeCode,PimAttributeId ,AttributeDefaultValueCode   
     into #Cte_AttributeValueDefault
	 FROM #Cte_GetDefaultData   
     WHERE LocaleId = @LocaleId   
     --UNION 
	 insert into #Cte_AttributeValueDefault   
     SELECT  AttributeDefaultValue  ,PimProductId ,AttributeCode,PimAttributeId,AttributeDefaultValueCode   
     FROM #Cte_GetDefaultData a   
     WHERE LocaleId = @DefaultLocaleId   
     AND NOT EXISTS (SELECT TOP 1 1 FROM #Cte_GetDefaultData b WHERE b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId= @LocaleId)  
      --  )  
    
	create table #AttributeValueDefaultData( PimProductId int,	AttributeValue varchar(max),	AttributeCode varchar(300),	PimAttributeId int,	AttributeDefaultValue varchar(2000), PimAttributeValueLocaleId int,FilesName varchar(max) Null)  
    
	SELECT ZM.MediaId,[dbo].[Fn_GetMediaThumbnailMediaPath]( zm.PATH) MediaPath,ZM.FileName
	into #ZnodeMediaFileNameDetails
	FROM ZnodeMedia ZM 
	WHERE Exists (Select 1
	FROM #TBL_AttributeValue TBLAV   
	WHERE  LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
	AND TypeOfData = 3 
	and ZM.MediaId = TBLAV.AttributeValue  ) 


	insert into #AttributeValueDefaultData(PimProductId,AttributeValue,AttributeCode,PimAttributeId,AttributeDefaultValue)  
    SELECT  PimProductId, AttributeValue,  AttributeCode,  PimAttributeId, NULL AttributeDefaultValue    
    FROM  #TBL_AttributeValue   
             WHERE LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
    AND TypeOfData = 1    

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
     	--UNION ALL   
		If @IsFilesNameRequired =1
		Begin
			insert into #AttributeValueDefaultData( PimProductId ,	AttributeValue,	AttributeCode ,	PimAttributeId ,	AttributeDefaultValue , PimAttributeValueLocaleId,FilesName )  
			SELECT DISTINCT PimProductId,  
			SUBSTRING ((SELECT ','+MediaPath FROM #ZnodeMediaFileNameDetails ZM 
			WHERE ZM.MediaId = TBLAV.AttributeValue  FOR XML PATH (''), TYPE).value('.', 'varchar(Max)') ,2,4000)  ,
			AttributeCode,PimAttributeId,NULL AttributeDefaultValue , PimAttributeValueLocaleId,
			SUBSTRING ((SELECT ','+ZM.FileName FROM #ZnodeMediaFileNameDetails ZM WHERE ZM.MediaId = TBLAV.AttributeValue   
			FOR XML PATH (''), TYPE).value('.', 'varchar(Max)') ,2,4000)
			FROM #TBL_AttributeValue TBLAV   
			WHERE  LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
			AND TypeOfData = 3  
			--and AttributeCode <>'ProductImage'
			-- GROUP BY PimAttributeId,PimProductId,AttributeCode  
			--Custom changes issues occured in fornt end code to include new stored procedure. Making change in base sp 
			--for immidiate requirment.
			--PCNA replace ProductImage by PrimaryImage attribute.
			--Declare @PimProductImageAttributeId int 
			--select @PimProductImageAttributeId  = PimAttributeId from ZnodePimAttribute where AttributeCode ='ProductImage'  and Iscategory =0 

			--INSERT INTO #AttributeValueDefaultData(PimProductId,AttributeValue,AttributeCode,PimAttributeId,AttributeDefaultValue,PimAttributeValueLocaleId )  
			--SELECT DISTINCT PimProductId,  
			--Isnull(@url+'/' + isnull(TBLAV.AttributeValue ,''),'' )
			--,'ProductImage' ,@PimProductImageAttributeId   , NULL AttributeDefaultValue , PimAttributeValueLocaleId
			--FROM #TBL_AttributeValue TBLAV   
			--WHERE  LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
			--AND TBLAV.AttributeCode = 'PrimaryImage'

		End
		Else
		Begin
			insert into #AttributeValueDefaultData( PimProductId ,	AttributeValue,	AttributeCode ,	PimAttributeId ,	AttributeDefaultValue , PimAttributeValueLocaleId,FilesName )  
			SELECT DISTINCT PimProductId,  
			SUBSTRING ((SELECT ','+MediaPath FROM #ZnodeMediaFileNameDetails ZM 
			WHERE ZM.MediaId = TBLAV.AttributeValue  FOR XML PATH (''), TYPE).value('.', 'varchar(Max)') ,2,4000)  ,
			AttributeCode,PimAttributeId,NULL AttributeDefaultValue , PimAttributeValueLocaleId,
			NULL 
			FROM #TBL_AttributeValue TBLAV   
			WHERE  LocaleId  = CASE WHEN RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END   
			AND TypeOfData = 3  
			-- GROUP BY PimAttributeId,PimProductId,AttributeCode  
		End


		If @IsFilesNameRequired =1
		Begin
			select PimProductId,AttributeValue,AttributeCode,PimAttributeId,AttributeDefaultValue,FilesName
			from #AttributeValueDefaultData
			order by PimAttributeValueLocaleId  
		End
		Else
		Begin
			select PimProductId,AttributeValue,AttributeCode,PimAttributeId,AttributeDefaultValue
			from #AttributeValueDefaultData
			order by PimAttributeValueLocaleId
		End

   END TRY  
         BEGIN CATCH  
    
		SELECT ERROR_MESSAGE()  
		DECLARE @Status BIT ;  
		SET @Status = 0;  
   
         END CATCH;  
     END;