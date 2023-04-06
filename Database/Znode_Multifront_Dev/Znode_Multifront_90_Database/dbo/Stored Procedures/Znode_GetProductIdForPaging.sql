CREATE PROCEDURE [dbo].[Znode_GetProductIdForPaging]
(
	@WhereClauseXML  XML           = NULL,
	@Rows            INT           = 10,
	@PageNo          INT           = 1,
	@Order_BY        VARCHAR(1000) = '',
	@RowsCount       INT OUT,
	@LocaleId        INT           = 1,
	@AttributeCode   VARCHAR(MAX)  = '',
	@PimProductId    TransferId READONLY , 
	@IsProductNotIn  BIT           = 0,
	@OutProductId    VARCHAR(MAX)	= 0 OUT )
AS	
 /* Summary :- This procedure is used to find the product ids with paging 
     Unit Testing
	 begin tran
	 DECLARE @ttr NVARCHAR(max)
	 DECLARE @PimProductId TransferId 
	 INSERT INTO @PimProductId
	 SELECT -1
     EXEC Znode_GetProductIdForPaging   N'' ,  10 ,  2 ,'productname desc',0, 1, '',@PimProductId ,0 ,@ttr OUT SELECT @ttr
	 rollback tran

	 begin tran

	 DECLARE @ttr NVARCHAR(max)
	 DECLARE @PimProductId TransferId 
	 INSERT INTO @PimProductId
	 SELECT 1
	EXEC Znode_GetProductIdForPaging N'', 50,1,'productname desc',0,1,'',@PimProductId,0,@ttr OUT SELECT @ttr
  rollback tran





	Create Index ZnodePimAttributeValue_ForPaging_Include ON  ZnodePimAttributeValue(PimAttributeId) include (PimAttributeValueId  ,PimProductId,CreatedDate,ModifiedDate )
	Create Index ZnodePimProductAttributeDefaultValue_ForPaging_Include ON  ZnodePimProductAttributeDefaultValue(PimAttributeValueId) include (PimAttributeDefaultValueId)
	Create Index ZnodePimFamilyGroupMapper_PimAttributeFamilyId ON ZnodePimFamilyGroupMapper(PimAttributeFamilyId,PimAttributeId)

	create  index IDX_ZnodePimAttributeValue_PimAttributeId on ZnodePimAttributeValue(PimAttributeId)
*/
BEGIN
 BEGIN TRY 

  SET NOCOUNT ON 

       DECLARE @SQL NVARCHAR(MAX) = '',
			   @InternalSQL NVARCHAR(MAX) = ''
	   DECLARE @UseCtePart VARCHAR(1000) = ''
	   DECLARE @InternalOrderby VARCHAR(1000) = ''
	   DECLARE @InternalWhereClause NVARCHAR(MAX) = '',
			   @InternalProductWhereClause NVARCHAR(MAX) = '',
			   @InternalUpperWhereClause NVARCHAR(MAX)='',
			   @InternaleProductJoin NVARCHAR(MAX) = ''

	   DECLARE @TBL_PimProductId  TABLE (PimProductId INT  , CountNo INT)
	   DECLARE @TBL_DefaultAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))
	  
	   DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleId()
	   DECLARE @PimProductIds TransferId 
	   INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)
	   SELECT PimAttributeId,AttributeCode FROM  [dbo].[Fn_GetDefaultAttributeId] ()

	   DECLARE @TBL_Attributeids TABLE(PimAttributeId INT , AttributeCode VARCHAr(600))

	  
	   INSERT INTO @PimProductIds (id)
	   SELECT Id
	   FROM @PimProductId 


	   IF  EXISTS (SELECT TOP 1 1 FROM @PimProductId )  AND @IsProductNotIn = 1 
	   BEGIN 
	    	     SET @InternalWhereClause = ' WHERE NOT EXISTS ( SELECT TOP 1 1 FROM @TBL_ProductFilter TBLFP WHERE  TBLFP.PimProductId = ZPP.PimProductId )'
	   END 
	   ELSE IF @IsProductNotIn = 0 AND EXISTS (SELECT TOP 1 1 FROM @PimProductId )  
	   BEGIN 
		  SET @InternalWhereClause = ''
		  SET @InternalUpperWhereClause = ' A '
	   END 	  

	
	   SET @SQl = ' 
				DECLARE @TBL_ProductFilter TABLE (PimProductId INT PRIMARY KEY,RowId INT IDENTITY(1,1)  ) '

				IF (@AttributeCode like  '%highlights%' or @AttributeCode like '%brand%' or @AttributeCode like '%vendor%' )
				BEGIN
			
					SET @SQl = @SQl + 'INSERT INTO @TBL_ProductFilter  (PimProductId )
										SELECT a.Id 
										FROM @TransferId a
										INNER JOIN ZnodePimAttributeValue ZPAV ON a.Id = ZPAV.PimProductId
										LEFT JOIN ZnodePimAttribute PA ON ( PA.PimAttributeId = ZPAV.PimAttributeId ) 
										WHERE PA.AttributeCode in (select top 1 item from dbo.Split ('''+@AttributeCode+''','',''))
										ORDER BY ZPAV.ModifiedDate desc;'

				END
				ELSE
				BEGIN 

					SET @SQl = @SQl + 'INSERT INTO @TBL_ProductFilter  (PimProductId )
										SELECT Id 
										FROM @TransferId '
				END

				
				SET @SQl = @SQl+'							
								DECLARE @TBL_PimProductId TABLE (PimProductId  INT PRIMARY KEY,RowId INT, CountNo INT)
								  
										SELECT ZPP.PimProductId , ZPP.PimAttributeFamilyId , '+CASE WHEN @InternalUpperWhereClause <> '' THEN ' TBPF.RowId ' ELSE ' 1 ' END+'  DisplayOrder 
										INTO #Cte_PimProductId 
										FROM ZnodePimProduct ZPP 
									'+CASE WHEN @InternalUpperWhereClause <> '' AND @InternalWhereClause = '' THEN ' INNER JOIN @TBL_ProductFilter TBPF ON (TBPF.PimProductId = ZPP.PimProductId ) ' ELSE '' END +'
										'+@InternalWhereClause+' 
								  '
	
	 SET @AttributeCode = @AttributeCode +CASE WHEN @Order_By <> '' THEN ','+ REPLACE(REPLACE(RTRIM(LTRIM(@Order_By)),'DESC',''),'ASC','') ELSE '' END 
	 
	 IF @AttributeCode = ''
	 BEGIN 
	 SET @AttributeCode = SUBSTRING( (SELECT ','+AttributeCode  FROM [dbo].[Fn_GetProductGridAttributes]() FOR XML PATH ('') ),2,4000)	
	 END 

	   INSERT INTO @TBL_Attributeids (PimAttributeId, AttributeCode )
	   SELECT PimAttributeId,AttributeCode
	   FROM [dbo].[Fn_GetProductGridAttributes]() FNGA  
	   WHERE EXISTS (SELECT Top 1 1 FROM dbo.split(@AttributeCode , ',') SP WHERE sp.Item = FNGA.AttributeCode   )

	  -- 	 SELECT @AttributeCode

	   

	    DECLARE @PimAttributeIds VARCHAR(MAX) = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) 
																		FROM @TBL_Attributeids FNGA  
																		FOR XML PATH ('') ) ,2,4000)


	SET @InternalOrderby = dbo.FN_trim(REPLACE(REPLACE(@Order_By,'DESC',''),'ASC',''))
	
	IF EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetProductGridAttributes]() FN WHERE FN.AttributeCode = @InternalOrderby) OR @InternalOrderby = 'AttributeFamily'
	BEGIN
		
		SET @InternalWhereClause = 'AttributeCode = '''+@InternalOrderby+''''
		SET @InternalOrderby = 'AttributeValue '+CASE WHEN @Order_By LIKE '% DESC' THEN 'DESC' ELSE 'ASC' END
				
	END
	ELSE IF  @InternalOrderby IN ('CreatedDate','ModifiedDate')
	
	BEGIN
	
	 SET @InternalOrderby = @Order_By
	 SET @InternalWhereClause = ' AttributeCode = ''SKU'' ' 
	END 
	ELSE 
	BEGIN 
	 SET @InternalOrderby = CASE WHEN @InternalOrderby = 'DisplayOrder' THEN @Order_By ELSE ' CTLA.PimProductId DESC ' END  
	 SET @InternalWhereClause = '' 
	END    

  
    IF CAST(@WhereClauseXML AS NVARCHAR(max)) NOT LIKE '%AttributeCode%' AND @InternalWhereClause = '' 
	BEGIN 
	
	 SET @SQL = @SQL + '  SELECT PimProductId ,'+[dbo].[Fn_GetPagingRowId](@InternalOrderby,'PimProductId DESC')+',Count(*)Over() CountNo
								   INTO #Cte_FilterData 
								   FROM #Cte_PimProductId CTLA
								 
								 INSERT INTO  @TBL_PimProductId  (PimProductId,RowId,CountNo )
								 SELECT DISTINCT PimProductId,RowId ,CountNo
								 FROM #Cte_FilterData 
								 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)


	END 
	ELSE IF CAST(@WhereClauseXML AS NVARCHAR(max)) NOT LIKE '%AttributeCode%' AND @InternalWhereClause <> '' 
	BEGIN

						 
	   SET @InternalSQL = '
						   DECLARE @TBL_FamilyLocale  TABLE(PimAttributeFamilyId INT PRIMARY KEY ,FamilyCode NVARCHAR(600),IsSystemDefined BIT,IsDefaultFamily BIT,IsCategory BIT ,AttributeFamilyName NVARCHAR(max) ) 
						   DECLARE @TBL_DefaultValue  TABLE(PimAttributeId INT,AttributeDefaultValueCode NVARCHAR(600),IsEditable BIT,AttributeDefaultValue NVARCHAR(max),DisplayOrder INT,PimAttributeDefaultValueId INT )
						   
						   INSERT INTO @TBL_FamilyLocale(PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName)
						   EXEC [dbo].[Znode_GetFamilyValueLocale] '''','+CAST(@LocaleId AS VARCHAR(50))+'	
						   
						   INSERT INTO @TBL_DefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId)
						   EXEC [dbo].[Znode_GetAttributeDefaultValueLocaleNew] '''+@PimAttributeIds+''','+CAST(@LocaleId AS VARCHAR(50))+''

							SET @SQL = @SQL + '  
										SELECT CTP.PimProductId , ZPA.AttributeCode  ,ZPAV.PimAttributeValueId  ,ZPA.PimAttributeId
										,ZPAV.CreatedDate,ZPAV.ModifiedDate,CTP.DisplayOrder     
										INTO #Cte_getAttributeValue
										FROM #Cte_PimProductId CTP             
										INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = CTP.PimProductId)             
										INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)    
										INNER JOIN ZnodePimFamilyGroupMapper ZPFGM ON (ZPFGM.PimAttributeFamilyId = CTP.PimAttributeFamilyId                      
										AND ZPFGM.PimAttributeId = ZPA.PimAttributeId)   
									 
										SELECT ZPA.PimProductId , ZPA.AttributeCode  ,  ZPAVL.AttributeValue ,ZPA.CreatedDate,ZPA.ModifiedDate,ZPA.DisplayOrder             
										INTO #Cte_CollectData
										FROM #Cte_getAttributeValue   ZPA INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPA.PimAttributeValueId = ZPAVL.PimAttributeValueId ) 
										WHERE  ZPA.PimAttributeId IN ('+@PimAttributeIds+') AND ZPAVL.LocaleId in  (' + CAST(@DefaultLocaleId AS VARCHAR(50)) +')
										
										UNION ALL 
										
										SELECT CTP.PimProductId , ''AttributeFamily'', TBFM.AttributeFamilyName AttributeValue,NULL,NULL,CTP.DisplayOrder
										FROM #Cte_PimProductId CTP INNER JOIN @TBL_FamilyLocale  TBFM ON (TBFM.PimAttributeFamilyId = CTP.PimAttributeFamilyId)
										

										UNION ALL 
										
										SELECT DISTINCT PimProductId, AttributeCode , SUBSTRING((SELECT '',''+AttributeDefaultValue 
										FROM @TBL_DefaultValue TBDV WHERE (TBDV.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )                            
										FOR XML PATH ('''')),2,4000) AttributeValue  , CTETY.CreatedDate,CTETY.ModifiedDate,DisplayOrder         
										FROM ZnodePimProductAttributeDefaultValue ZPPADV INNER JOIN Cte_getAttributeValue CTETY 
										ON (CTETY.PimAttributeValueId = ZPPADV.PimAttributeValueId)
										WHERE  CTETY.PimAttributeId IN ('+@PimAttributeIds+')
								 
									SELECT PimProductId,'+CASE WHEN @InternalOrderby LIKE '%DisplayOrder%' THEN 'DisplayOrder' ELSE   'AttributeValue'  END+'  , 1 DefaultOrderBy      
									INTO #Cte_GetAttributeValueI
									FROM  #Cte_CollectData CTCD
									WHERE '+@InternalWhereClause+'
									UNION ALL 
									SELECT CTP.PimProductId , NULL , 2 
									FROM #Cte_PimProductId CTP 	
									where NOT EXISTS (SELECT TOP 1 1 FROM  #Cte_CollectData CTCD
									WHERE '+@InternalWhereClause+' AND CTCD.PimProductId = CTP.PimProductId  )				  
								 
									SELECT DISTINCT PimProductId,'+[dbo].[Fn_GetPagingRowId](' DefaultOrderBy , '+@InternalOrderby+' ','PimProductId DESC')+'
									INTO #Cte_FilterData
									FROM Cte_GetAttributeValueI CTCD
								 
									SELECT PimProductId ,RowId ,Count(*)Over() CountNo
									INTO #Cte_GetAllData
									FROM #Cte_FilterData
									GROUP BY PimProductId ,RowId 
								 
								INSERT INTO  @TBL_PimProductId  (PimProductId,RowId,CountNo )
								SELECT  PimProductId,RowId ,CountNo
								FROM  #Cte_GetAllData
								'+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)

		SET @SQL =  @InternalSQL + @SQL
			
	    END
		ELSE 
		BEGIN 
		 
		  SET @InternalSQL = ''
		  DECLARE @AttachINDefault  VARCHAr(max) = ''
		  SET  @InternalProductWhereClause = 
							STUFF( (  SELECT ' INNER JOIN #Cte_AttributeValueLocale AS CTAL'+CAST(ID AS VARCHAR(50))
							+' ON ( CTAL'+CAST(ID AS VARCHAR(50))+'.PimProductId = CTAL'+CASE WHEN ID-1= 0 THEN '' ELSE CAST(ID-1 AS VARCHAR(50)) END +'.PimProductId AND '+
							REPLACE(REPLACE(WhereClause,'AttributeCode ',' CTAL'+CAST(ID AS VARCHAR(50))+'.AttributeCode '),' AttributeValue ',' CTAL'+CAST(ID AS VARCHAR(50))+'.AttributeValue ')+' )'
							FROM dbo.Fn_GetWhereClauseXML(@WhereClauseXML)      
							FOR XML PATH (''), TYPE).value('.', ' Nvarchar(max)'), 1, 0, '')

          SET @SQL = @SQL + '   	
										SELECT CTP.PimProductId , ZPA.AttributeCode  ,ZPAV.PimAttributeValueId  ,ZPA.PimAttributeId
														,ZPAV.CreatedDate,ZPAV.ModifiedDate,CTP.DisplayOrder     
										INTO #Cte_getAttributeValue
										FROM #Cte_PimProductId CTP             
										INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = CTP.PimProductId)       
										INNER JOIN ZnodePimAttribute ZPA ON (ZPAV.PimAttributeId = ZPA.PimAttributeId)    
										INNER JOIN ZnodePimFamilyGroupMapper ZPFGM ON 
										(CTP.PimAttributeFamilyId = ZPFGM.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPA.PimAttributeId) 
										WHERE (EXISTS (SELECT TOP 1 1 FROM dbo.split('''+@AttributeCode+''','','') SP WHERE Sp.Item = ZPA.AttributeCode )  OR NOT EXISTS (SELECT TOP 1 1 FROM dbo.split('''+@AttributeCode+''','','') ))
								 '
		  IF EXISTS (SELECT Top 1 1 FROM @TBL_Attributeids TBH WHERE NOT EXISTS (SELECT Top 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBH.PimAttributeId)
		  AND TBH.AttributeCode <> 'AttributeFamily' )
         BEGIN 
		    SET @SQL = @SQL+'   
									SELECT ZPA.PimProductId , ZPA.AttributeCode  ,  ZPAVL.AttributeValue,ZPAVL.LocaleId ,
											ZPA.PimAttributeId,ZPA.CreatedDate,ZPA.ModifiedDate,ZPA.DisplayOrder             
									INTO #Cte_CollectData
									FROM #Cte_getAttributeValue ZPA          
									INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPA.PimAttributeValueId  = ZPAVL.PimAttributeValueId )
									where ZPA.PimAttributeId IN ('+@PimAttributeIds+')
									AND  ZPAVL.LocaleId IN ( '+CAST(@LocaleId AS VARCHAR(50))+','+CAST(@DefaultLocaleId AS VARCHAR(50))+')
								 
								 
										SELECT PimProductId,AttributeCode,AttributeValue,PimAttributeId,CreatedDate,ModifiedDate,DisplayOrder
										INTO #Cte_FilterDataLocale 
										FROM #Cte_CollectData
										WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
										UNION All 
										SELECT M.PimProductId,M.AttributeCode,M.AttributeValue ,M.PimAttributeId,M.CreatedDate,M.ModifiedDate,M.DisplayOrder
										FROM #Cte_CollectData M 
										WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
										AND NOT Exists (Select TOP 1 1  from #Cte_CollectData X where  M.PimProductId = X.PimProductId AND M.AttributeCode = X.AttributeCode And X.localeId =  '+CAST(@LocaleId AS VARCHAR(50))+' )
								 '

						SET @AttachINDefault =			'SELECT PimProductId,AttributeCode,CTFD.AttributeValue ,CreatedDate,ModifiedDate,CTFD.DisplayOrder	 
										INTO #Cte_AttributeValueLocale FROM #Cte_FilterDataLocale CTFD'

		 END 
		 
		 IF EXISTS (SELECT Top 1 1 FROM @TBL_Attributeids TBH WHERE EXISTS (SELECT Top 1 1 FROM @TBL_DefaultAttributeId TBL WHERE TBL.PimAttributeId = TBH.PimAttributeId))
         BEGIN 
		 SET  @InternalSQL = 'DECLARE @TBL_DefaultValue  TABLE(PimAttributeId INT,AttributeDefaultValueCode NVARCHAR(600),IsEditable BIT,AttributeDefaultValue NVARCHAR(max),DisplayOrder INT,PimAttributeDefaultValueId INT  ) 
		                     INSERT INTO @TBL_DefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId)
						     EXEC [dbo].[Znode_GetAttributeDefaultValueLocaleNew] '''+@PimAttributeIds+''','+CAST(@LocaleId AS VARCHAR(50))
	   
			  SET @AttachINDefault = CASE WHEN @AttachINDefault = '' THEN '' ELSE @AttachINDefault+' UNION ALL ' END + ' SELECT PimProductId, AttributeCode , 
										SUBSTRING(
											(SELECT '',''+AttributeDefaultValue FROM @TBL_DefaultValue TBDV 
											WHERE (TBDV.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )                            
											FOR XML PATH (''''))
										,2,4000) 
										AttributeValue  , CTETY.CreatedDate,CTETY.ModifiedDate,DisplayOrder         
										FROM #Cte_getAttributeValue CTETY
										INNER JOIN  ZnodePimProductAttributeDefaultValue ZPPADV ON (CTETY.PimAttributeValueId =ZPPADV.PimAttributeValueId  )
										WHERE  CTETY.PimAttributeId IN ('+@PimAttributeIds+') '

		 END
		 IF EXISTS (SELECT Top 1 1 FROM @TBL_Attributeids TBH WHERE AttributeCode = 'attributefamily')
         BEGIN 
		 


		 SET  @InternalSQL = @InternalSQL +' DECLARE @TBL_FamilyLocale  TABLE(PimAttributeFamilyId INT PRIMARY KEY ,FamilyCode NVARCHAR(600),IsSystemDefined BIT,IsDefaultFamily BIT,IsCategory BIT ,AttributeFamilyName NVARCHAR(max) ) 
		                    INSERT INTO @TBL_FamilyLocale(PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName)
						   EXEC [dbo].[Znode_GetFamilyValueLocale] '''','+CAST(@LocaleId AS VARCHAR(50))
						   
			SET @AttachINDefault = CASE WHEN @AttachINDefault = '' THEN '' ELSE @AttachINDefault+' UNION ALL '	END + 'SELECT CTP.PimProductId , ''AttributeFamily'' AttributeCode, TBFM.AttributeFamilyName AttributeValue,NULL CreatedDate, NULL ModifiedDate,DisplayOrder	
										FROM #Cte_PimProductId CTP 
										INNER JOIN @TBL_FamilyLocale  TBFM ON (TBFM.PimAttributeFamilyId = CTP.PimAttributeFamilyId)'
		 END
		
								--Print '---------3--------'
									SET @SQL = @SQL + ' 
									 '+@AttachINDefault+'										
									 '
									  

									--Print '---------4--------'
									SET @SQL = @SQL + ' 

									 
										 SELECT CTAL.PimProductId 
										 INTO #Cte_AttributeLocale 
										 FROM #Cte_AttributeValueLocale  CTAL
										 '+@InternalProductWhereClause+'
										 GROUP BY CTAL.PimProductId 
									 
									
										SELECT DISTINCT CTLA.PimProductId, 1 DefaultOrderBy  ,'+[dbo].[Fn_GetPagingRowId](@InternalOrderby,' CTLA.PimProductId DESC')+' 
										INTO #Cte_FinalProductData 
										FROM #Cte_AttributeValueLocale  CTAVL
										INNER JOIN #Cte_AttributeLocale CTLA ON (CTLA.PimProductId = CTAVL.PimProductId)
							        	'+CASE WHEN @InternalWhereClause <> '' THEN  ' WHERE CTAVL.'+dbo.FN_Trim(@InternalWhereClause) ELSE '' END +'
									

										  SELECT PimProductId, DefaultOrderBy,RowId
										  INTO #Cte_GEtSortingProduct 
										  FROM #Cte_FinalProductData 
										  UNION ALL 
										  SELECT PimProductId ,2 DefaultOrderBy,'+[dbo].[Fn_GetPagingRowId]( 'DERE.PimProductId',' DERE.PimProductId ')+'
										  FROM #Cte_AttributeLocale DERE 
										  WHERE NOT EXISTS (SELECT TOP 1 1 FROM #Cte_FinalProductData TTRR WHERE TTRR.PimProductId = DERE.PimProductId  ) 
									  
									  
										  SELECT PimProductId ,'+[dbo].[Fn_GetPagingRowId](' DefaultOrderBy ',' RowId ')+' ,Count(*)Over() CountNo
										  INTO #Cte_getPagingData
										  FROM #Cte_GEtSortingProduct
									  


										  SELECT PimProductId ,RowId ,CountNo
										  INTO #Cte_GetAllData 
										  FROM #Cte_getPagingData
										  GROUP BY PimProductId ,RowId ,CountNo
									
									'
									--Print @SQL
									--Print '---------5--------'
									SET @SQL = @SQL + ' 

								 INSERT INTO  @TBL_PimProductId  (PimProductId,RowId,CountNo )
								 SELECT  PimProductId,RowId , CountNo
								 FROM #Cte_GetAllData '
								 +[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)

			SET @SQl = @InternalSQL+@SQL
     		END
			SET @SQl = @SQl + ' 
								SET @OutProductId = ISNULL(SUBSTRING((SELECT '',''+CAST(PimProductid AS VARCHAR(50)) 
										FROM @TBL_PimProductId TBPP
										ORDER BY RowId
										FOR XML PATH ('''')   ),2,4000),'''')
								SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM  @TBL_PimProductId TBPP),0)
							 
								IF OBJECT_ID(''#Cte_PimProductId'', ''U'') IS NOT NULL DROP TABLE #Cte_PimProductId
								IF OBJECT_ID(''#Cte_getAttributeValue'', ''U'') IS NOT NULL DROP TABLE #Cte_getAttributeValue
								IF OBJECT_ID(''#Cte_CollectData'', ''U'') IS NOT NULL DROP TABLE #Cte_CollectData
								IF OBJECT_ID(''#Cte_FilterDataLocale'', ''U'') IS NOT NULL DROP TABLE #Cte_FilterDataLocale
								IF OBJECT_ID(''#Cte_AttributeValueLocale'', ''U'') IS NOT NULL DROP TABLE #Cte_AttributeValueLocale
								IF OBJECT_ID(''#Cte_AttributeLocale'', ''U'') IS NOT NULL DROP TABLE #Cte_AttributeLocale
								IF OBJECT_ID(''#Cte_FinalProductData'', ''U'') IS NOT NULL DROP TABLE #Cte_FinalProductData
								IF OBJECT_ID(''#Cte_GEtSortingProduct'', ''U'') IS NOT NULL DROP TABLE #Cte_GEtSortingProduct
								IF OBJECT_ID(''#Cte_GetAllData'', ''U'') IS NOT NULL DROP TABLE #Cte_GetAllData
								IF OBJECT_ID(''#Cte_getPagingData'', ''U'') IS NOT NULL DROP TABLE #Cte_getPagingData
							 '
 
			EXEC SP_EXECUTESQl  @SQL ,N' @OutProductId VARCHAR(max) OUT ,@RowsCount INT OUT ,@TransferId TransferId READONLY  ',  @OutProductId = @OutProductId OUT, @RowsCount = @RowsCount OUT ,@TransferId  = @PimProductIds
		
     END TRY 
    BEGIN CATCH
	 DECLARE @Status BIT ;
		     SET @Status = 0;
			 SELECT ERROR_MESSAGE()
		  --   DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductIdForPaging @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@AccountList='+CAST(@AccountList AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
    --         SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
    --         EXEC Znode_InsertProcedureErrorLog
				--@ProcedureName = 'Znode_GetProductIdForPaging',
				--@ErrorInProcedure = @Error_procedure,
				--@ErrorMessage = @ErrorMessage,
				--@ErrorLine = @ErrorLine,
				--@ErrorCall = @ErrorCall;
	 END CATCH 
     END