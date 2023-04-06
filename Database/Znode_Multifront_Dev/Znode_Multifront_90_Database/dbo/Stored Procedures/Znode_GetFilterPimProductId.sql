
CREATE PROCEDURE [dbo].[Znode_GetFilterPimProductId]
(
  @WhereClause XML 
 ,@PimProductId TransferId READONLY 
 ,@LocaleId   INT,
 @IsProductNotIn BIT = 1
)
AS 
BEGIN 
SET NOCOUNT ON 

		DECLARE  @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleID()
		DECLARE @SQL NVARCHAR(MAX)
		DECLARE @InternalProductWhereClause NVARCHAR(MAX)

		DECLARE @WorkingProcess INT = 0 

		DECLARE @TBL_FilterClause TABLE (ID INT IDENTITY(1,1),AttributeValue NVARCHAR(MAX),AttributeCode NVARCHAr(MAX),PimAttributeId INT ,AttributeTypeName VARCHAR(300),AttributeCodeOrg VARCHAR(600))

		DECLARE @WhereClauseXML XML = @WhereClause 

		SET @SQL  = ''

		IF EXISTS (SELECT TOP 1 1 FROM @WhereClauseXml.nodes ( '//ArrayOfWhereClauseModel/WhereClauseModel'  ) AS Tbl(Col) 
		WHERE Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(MAX)')  LIKE  '% in (%')
		BEGIN 
			SET @WorkingProcess = 1

				INSERT INTO @TBL_FilterClause (AttributeValue,AttributeCode,AttributeTypeName,PimAttributeId,AttributeCodeOrg)
			SELECT  Tbl.Col.value ( 'attributevalue[1]' , 'NVARCHAR(MAX)') AS AttributeValue
			,Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(MAX)') AS AttributeValue,ZTY.AttributeTypeName,ZPA.PimAttributeId,AttributeCode AttributeCodeOrg
			FROM @WhereClauseXml.nodes ( '//ArrayOfWhereClauseModel/WhereClauseModel'  ) AS Tbl(Col)
			LEFT JOIN  ZnodePimAttribute ZPA  ON ((Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(MAX)')  LIKE '%in (%' OR dbo.Fn_Trim(REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(MAX)'),' = ',''),'''','')) 
												= ZPA.AttributeCode ) AND IsCategory = 0 
			AND ( ZPA.IsShowOnGrid = 1 OR ZPA.IsConfigurable =1  )  )
			LEFT JOIN ZnodeAttributeType ZTY ON (ZTY.AttributeTypeId = ZPA.AttributeTypeId)

		END 
		ELSE 
		BEGIN 

			INSERT INTO @TBL_FilterClause (AttributeValue,AttributeCode,AttributeTypeName,PimAttributeId,AttributeCodeOrg)
			SELECT  Tbl.Col.value ( 'attributevalue[1]' , 'NVARCHAR(MAX)') AS AttributeValue
			,Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(MAX)') AS AttributeValue,ZTY.AttributeTypeName,ZPA.PimAttributeId,AttributeCode AttributeCodeOrg
			FROM @WhereClauseXml.nodes ( '//ArrayOfWhereClauseModel/WhereClauseModel'  ) AS Tbl(Col)
			LEFT JOIN ZnodePimAttribute ZPA  ON (dbo.Fn_Trim(REPLACE(REPLACE(Tbl.Col.value ( 'attributecode[1]' , 'NVARCHAR(MAX)'),' = ',''),'''','')) 
												= ZPA.AttributeCode AND ZPA.IsCategory = 0 
			AND ( ZPA.IsShowOnGrid = 1 OR ZPA.IsConfigurable =1  )  )
			LEFT JOIN ZnodeAttributeType ZTY ON (ZTY.AttributeTypeId = ZPA.AttributeTypeId)

		END 

		CREATE TABLE #TBL_PimProductId (PimProductId INT)

		CREATE TABLE #TBL_PimProductIdDelete (PimProductId INT )

		INSERT INTO #TBL_PimProductId (PimProductId )
		SELECT Id 
		FROM @PimProductId

		SELECT ZPAV.PimProductId ,PimAttributeValueId ,ZPAV.CreatedDate,ZPAV.ModifiedDate,TBLA.AttributeCodeOrg AttributeCode
		INTO #TBL_AttributeValueId 
		FROM  ZnodePimAttributeValue ZPAV 
		INNER JOIN @TBL_FilterClause TBLA ON (TBLA.PimAttributeId = ZPAV.PimAttributeId)
		INNER JOIN #TBL_PimProductId YT ON (YT.PimProductId = ZPAV.PimProductId OR NOT EXISTS (SELECT TOP 1 1 #TBL_PimProductId))

		DELETE FROM #TBL_PimProductId

		INSERT INTO #TBL_PimProductId (PimProductId )
		SELECT DISTINCT PimProductId 
		FROM #TBL_AttributeValueId

		IF @WorkingProcess =1 
		BEGIN 
				DECLARE @PimAttributeId_in TransferId 

				INSERT INTO @PimAttributeId_in 
				SELECT PimAttributeId
				FROM  @TBL_FilterClause 
				WHERE AttributeTypeName IN ('Simple Select','Multi Select') 
				AND AttributeCode LIKE '%in (%'

				CREATE TABLE #TBL_AttributeDefaultValue_in ( PimAttributeId INT ,
							AttributeDefaultValueCode VARCHAR(MAX),IsEditable INT,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder INT,PimAttributeDefaultValueId INT,IsDefault Bit  )    
				INSERT INTO #TBL_AttributeDefaultValue_in(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId,IsDefault)
				EXEC Znode_GetAttributeDefaultValueLocaleNew_TansferId @PimAttributeId_in, @LocaleId;
 
				DECLARE @WhereClauseInCom NVARCHAR(MAX) = (SELECT TOP 1 AttributeValue FROM @TBL_FilterClause WHERE AttributeCode LIKE '%in (%') 


			SET @SQL = '
			;With Cte_AttributeValue AS 
			(
			SELECT PimAttributeValueId 
			FROM ZnodePimAttributeValueLocale 
			WHERE AttributeValue '+@WhereClauseInCom+'
			UNION ALL 
			SELECT ZPADV.PimAttributeValueID 
			FROM ZnodePimProductAttributeDefaultValue ZPADV 
			INNER JOIN #TBL_AttributeDefaultValue_in TBL ON (TBL.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)
			WHERE TBL.AttributeDefaultValue '+@WhereClauseInCom+'
			)
   
			SELECT PimProductId 
			FROM #TBL_AttributeValueId ZPAV 
			INNER JOIN  Cte_AttributeValue CTAC ON (CTAC.PimAttributeValueId = ZPAV.PimAttributeVaLueId )
			GROUP BY PimProductId
			UNION ALL  
			SELECT PimProductId 
			FROM ZnodePimProduct a
			INNER JOIN ZnodePimFamilyLocale b ON (b.PimAttributeFamilyId = a.PimAttributeFamilyId) 
			WHERE b.AttributeFamilyName '+@WhereClauseInCom+'
			GROUP BY PimProductId
			UNION ALL 
			SELECT  TBLAV.PimProductId 
			FROM ZnodePimProduct TBLAV
			WHERE CASE WHEN TBLAV.IsProductPublish  IS NULL THEN ''Not Published'' 
					WHEN TBLAV.IsProductPublish = 0 THEN ''Draft''
					ELSE  ''Published'' END '+@WhereClauseInCom+'
			GROUP BY TBLAV.PimProductId 
			'
   
			DELETE FROM #TBL_PimProductIdDelete 
			INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
			EXEC (@SQL)
			DELETE FROM #TBL_PimProductId
			INSERT INTO #TBL_PimProductId
			SELECT PimProductId FROM #TBL_PimProductIdDelete
			INSERT INTO #TBL_PimProductId
			SELECT -1 
			WHERE NOT EXISTS (SELECT TOP 1 1  FROM #TBL_PimProductId)

			DELETE  FROM @TBL_FilterClause WHERE AttributeCode LIKE '% in (%'

			DROP TABLE #TBL_AttributeDefaultValue_in
			SET @WorkingProcess  = 0 
   
		END 

		IF EXISTS (SELECT TOP 1 1 FROM @TBL_FilterClause WHERE AttributeCode <> '' AND ISNULL(AttributeValue,'') = '')
		BEGIN 
  
				SET  @InternalProductWhereClause = STUFF( (  SELECT ' INNER JOIN #TBL_AttributeValueId AS ZPAVL'+CAST(ID AS VARCHAR(200))+
												' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
												' )'				
										FROM @TBL_FilterClause
										WHERE ISNULL(AttributeValue,'') = ''
										FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				----Change for configurable product varient page seach
				SET @SQL = ' 
							SELECT  TBLAV.PimProductId 
							FROM #TBL_AttributeValueId TBLAV '+@InternalProductWhereClause+' 
							WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId )
							GROUP BY TBLAV.PimProductId 
						'
  

				DELETE FROM #TBL_PimProductIdDelete 
				INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
				EXEC (@SQL)
				DELETE FROM #TBL_PimProductId
				INSERT INTO #TBL_PimProductId
				SELECT PimProductId FROM #TBL_PimProductIdDelete
				INSERT INTO #TBL_PimProductId
				SELECT -1 
				WHERE NOT EXISTS (SELECT TOP 1 1  FROM #TBL_PimProductId)
				DELETE FROM @TBL_FilterClause WHERE ISNULL(AttributeValue,'') = ''
				IF NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) AND EXISTS (SELECT TOP 1 1  FROM @PimProductId Having Max (ID) = 0 )
				BEGIN
				INSERT INTO  #TBL_PimProductId (PimProductId)
				SELECT 0 
			END
  
		END 

		IF EXISTS (SELECT TOP 1 1 FROM @TBL_FilterClause WHERE AttributeTypeName IN ('Simple Select','Multi Select') )
		BEGIN
			DECLARE @PimAttributeId TransferId 

			INSERT INTO @PimAttributeId 
			SELECT DISTINCT PimAttributeId
			FROM  @TBL_FilterClause WHERE AttributeTypeName IN ('Simple Select','Multi Select') 

			CREATE TABLE #TBL_AttributeDefaultValue ( PimAttributeId INT ,
						AttributeDefaultValueCode VARCHAR(MAX),IsEditable INT,AttributeDefaultValue NVARCHAR(MAX),DisplayOrder INT,PimAttributeDefaultValueId INT,IsDefault BIT  )    
			INSERT INTO #TBL_AttributeDefaultValue(PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder,PimAttributeDefaultValueId,IsDefault)
			EXEC Znode_GetAttributeDefaultValueLocaleNew_TansferId @PimAttributeId, @LocaleId;
 
			IF @DefaultLocaleId = @LocaleID AND  @WorkingProcess = 0 
			BEGIN 

				IF @IsProductNotIn = 1 AND EXISTS(SELECT * FROM @TBL_FilterClause WHERE AttributeCodeOrg in ('Brand','Highlights'))
				BEGIN
					SET  @InternalProductWhereClause = STUFF( (  SELECT ' INNER JOIN Cte_AttributeValue AS ZPAVL'+CAST(ID AS VARCHAR(200))+
													' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
													+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+
													'.LocaleId='+CAST(@LocaleID AS VARCHAR(200))+' )'				
											FROM @TBL_FilterClause
											WHERE AttributeTypeName IN ('Simple Select','Multi Select')
											AND AttributeValue <> ''
											AND AttributeValue IS NOT NULL
											FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				END
				ELSE IF @IsProductNotIn = 0  AND EXISTS(SELECT * FROM @TBL_FilterClause WHERE AttributeCodeOrg not in ('Brand','Highlights'))
				BEGIN
					SET  @InternalProductWhereClause = STUFF( (  SELECT ' INNER JOIN Cte_AttributeValue AS ZPAVL'+CAST(ID AS VARCHAR(200))+
													' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
													+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+
													'.LocaleId='+CAST(@LocaleID AS VARCHAR(200))+' )'				
											FROM @TBL_FilterClause
											WHERE AttributeTypeName IN ('Simple Select','Multi Select')
											AND AttributeValue <> ''
											AND AttributeValue IS NOT NULL
											FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				END
				ELSE
				BEGIN
					SET  @InternalProductWhereClause = STUFF( (  SELECT ' INNER JOIN Cte_AttributeValue AS ZPAVL'+CAST(ID AS VARCHAR(200))+
													' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+
													'.LocaleId='+CAST(@LocaleID AS VARCHAR(200))+' )'				
											FROM @TBL_FilterClause
											WHERE AttributeTypeName IN ('Simple Select','Multi Select')
											AND AttributeValue <> ''
											AND AttributeValue IS NOT NULL
											FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				END
				
				SET @SQL = ' ;With Cte_AttributeValue AS 
							(
							SELECT TBLAV.PimAttributeValueId ,SUBSTRING((SELECT '',''+AttributeDefaultValueCode FROM #TBL_AttributeDefaultValue TTR 
							INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (TTR.PimAttributeDefaultValueId = ZPAVL.PimAttributeDefaultValueId )
							WHERE ZPAVL.PimAttributeValueId = TBLAV.PimAttributeValueId  
							AND ZPAVL.LocaleId = '+Cast(@localeId AS VARCHAR(200))+'
							FOR XML PATH('''') ),2,4000) AttributeValue
								,  '+Cast(@localeId AS VARCHAR(200))+' LocaleId,TBLAV.AttributeCode,TBLAV.PimProductId
							FROM #TBL_AttributeValueId TBLAV
							'+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END+'
							GROUP BY TBLAV.PimAttributeValueId,TBLAV.AttributeCode,TBLAV.PimProductId
							)
  
						SELECT  TBLAV.PimProductId
						FROM #TBL_AttributeValueId TBLAV
						'+@InternalProductWhereClause+'	GROUP BY TBLAV.PimProductId '
			
				
			END 
			ELSE IF  @WorkingProcess = 0 
			BEGIN 

				SET  @InternalProductWhereClause = 
										STUFF( (  SELECT ' INNER JOIN Cte_AttributeValue AS ZPAVL'+CAST(ID AS VARCHAR(200))+
												' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
												' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+'  )'				
										FROM @TBL_FilterClause
										WHERE AttributeTypeName IN ('Simple Select','Multi Select')
										AND AttributeValue <> ''
										AND AttributeValue IS NOT NULL
										FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				SET @SQL = '  			 
							SELECT TBLAV.PimAttributeValueId,ZPAVL.PimAttributeDefaultValueId , ZPAVL.LocaleId ,COUNT(*)Over(Partition By TBLAV.PimAttributeValueId ,TBLAV.PimProductId ORDER BY TBLAV.PimAttributeValueId ,TBLAV.PimProductId  ) RowId
							INTO #temp_Table 
							FROM #TBL_AttributeValueId TBLAV 
							INNER JOIN ZnodePimProductAttributeDefaultValue ZPAVL ON (ZPAVL.PimAttributeValueId = TBLAV.PimAttributeValueId)
							WHERE (ZPAVL.LocaleId = '+Cast(@localeId AS VARCHAR(200))+' OR ZPAVL.LocaleId = '+Cast(@DefaultlocaleId AS VARCHAR(200))+')
				
							;with Cte_AttributeValue AS 
							(
							SELECT TBLAV.PimAttributeValueId ,SUBSTRING((SELECT '',''+AttributeDefaultValueCode FROM #TBL_AttributeDefaultValue TTR 
							INNER JOIN #temp_Table  ZPAVL ON (TTR.PimAttributeDefaultValueId = ZPAVL.PimAttributeDefaultValueId )
							WHERE ZPAVL.PimAttributeValueId = TBLAV.PimAttributeValueId  
							AND ZPAVL.LocaleId = CASE WHEN ZPAVL.RowId = 2 THEN '+CAST(@LocaleId AS Varchar(300))+' ELSE '+Cast(@DefaultLocaleId AS Varchar(300))+' END  
							FOR XML PATH('''') ),2,4000) AttributeValue,TBLAV.AttributeCode ,TBLAV.PimProductId 
							FROM #TBL_AttributeValueId TBLAV
							'+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END+'
							GROUP BY TBLAV.PimAttributeValueId,TBLAV.AttributeCode ,TBLAV.PimProductId 
							)
  
							SELECT   TBLAV.PimProductId
							FROM  #TBL_AttributeValueId TBLAV
							'+@InternalProductWhereClause+'	GROUP BY TBLAV.PimProductId '

			END 

			DELETE FROM #TBL_PimProductIdDelete 
			INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
			EXEC (@SQL)
			DELETE FROM #TBL_PimProductId
			INSERT INTO #TBL_PimProductId
			SELECT PimProductId FROM #TBL_PimProductIdDelete
 
			DROP TABLE #TBL_AttributeDefaultValue

		END 

		IF EXISTS (SELECT TOP 1 1 FROM @TBL_FilterClause WHERE AttributeTypeName IN ('Text','Number','Datetime','Yes/No','Date') )
		BEGIN  
   
				IF @DefaultLocaleId = @LocaleID AND @WorkingProcess = 0 
				BEGIN 
					SET  @InternalProductWhereClause = 
											STUFF( (  SELECT ' INNER JOIN View_PimProducttextValue AS ZPAVL'+CAST(ID AS VARCHAR(200))+
													' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
													' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+
													'.LocaleId='+CAST(@LocaleID AS VARCHAR(200))+' )'				
											FROM @TBL_FilterClause
											WHERE AttributeTypeName IN ('Text','Number','Datetime','Yes/No','Date')
											FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
 
					SET @SQL = '	SELECT  TBLAV.PimProductId 
								FROM #TBL_AttributeValueId TBLAV
								'+@InternalProductWhereClause+'
								'+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
											ELSE ' WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId   ) ' END 
								+' GROUP BY TBLAV.PimProductId '

			END 
			ELSE IF @WorkingProcess = 0 
			BEGIN 
				SET  @InternalProductWhereClause = 
										STUFF( (  SELECT ' INNER JOIN Cte_AttributeDetails AS ZPAVL'+CAST(ID AS VARCHAR(200))+
												' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
												' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+
												'.LocaleId = CASE WHEN ZPAVL'+CAST(ID AS VARCHAR(200))+'.RowId = 2 THEN  '+CAST(@LocaleId AS Varchar(300))+' ELSE '+Cast(@DefaultLocaleId AS Varchar(300))+' END  )'				
										FROM @TBL_FilterClause
										WHERE AttributeTypeName IN ('Text','Number','Datetime','Yes/No','Date')
										FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				SET @SQL = ' 
					;With Cte_AttributeDetails AS 
					(
					SELECT TBLAV.PimProductId,ZPAVL.AttributeValue,TBLAV.AttributeCode,ZPAVL.LocaleId ,COUNT(*)Over(Partition By TBLAV.PimProductId,TBLAV.AttributeCode ORDER BY TBLAV.PimProductId,TBLAV.AttributeCode  ) RowId
					FROM #TBL_AttributeValueId TBLAV 
					INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAVL.PimAttributeValueId = TBLAV.PimAttributeValueId )
					WHERE (LocaleId = '+Cast(@DefaultLocaleId AS Varchar(300))+' OR LocaleId = '+CAST(@LocaleId AS Varchar(300))+' )'+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' AND EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END +'
					) 
					SELECT  TBLAV.PimProductId 
  					FROM #TBL_AttributeValueId TBLAV
					'+@InternalProductWhereClause+'
					GROUP BY TBLAV.PimProductId 
					'
			END 
			DELETE FROM #TBL_PimProductIdDelete 
			INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
			EXEC (@SQL)
			DELETE FROM #TBL_PimProductId
			INSERT INTO #TBL_PimProductId
			SELECT PimProductId FROM #TBL_PimProductIdDelete

		END 

		IF EXISTS (SELECT TOP 1 1 FROM @TBL_FilterClause WHERE AttributeTypeName IN ('Text Area') )
		BEGIN    
			IF @DefaultLocaleId = @LocaleID AND @WorkingProcess = 0 
			BEGIN 
				SET  @InternalProductWhereClause = 
											STUFF( (  SELECT ' INNER JOIN View_PimProductTextAreaValue AS ZPAVL'+CAST(ID AS VARCHAR(200))+
												' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
												' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+
												'.LocaleId='+CAST(@LocaleID AS VARCHAR(200))+' )'				
										FROM @TBL_FilterClause
										WHERE AttributeTypeName IN ('Text Area')
										FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
 
				SET @SQL = '
							SELECT  TBLAV.PimProductId 
							FROM #TBL_AttributeValueId TBLAV
							'+@InternalProductWhereClause+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END 
										+' GROUP BY TBLAV.PimProductId '
			END 
			ELSE IF @WorkingProcess = 0 
			BEGIN 
				SET  @InternalProductWhereClause = 
										STUFF( (  SELECT ' INNER JOIN Cte_AttributeDetails AS ZPAVL'+CAST(ID AS VARCHAR(200))+
												' ON ( TBLAV.PimProductId = ZPAVL'+CAST(ID AS VARCHAR(200))+'.PimProductId AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeCode '+AttributeCode+
												' AND ZPAVL'+CAST(ID AS VARCHAR(200))+'.AttributeValue '+AttributeValue+' AND ZPAVL'+CAST(ID AS VARCHAR(200))+
												'.LocaleId = CASE WHEN ZPAVL'+CAST(ID AS VARCHAR(200))+'.RowId = 2 THEN  '+CAST(@LocaleId AS Varchar(300))+' ELSE '+Cast(@DefaultLocaleId AS Varchar(300))+' END  )'				
										FROM @TBL_FilterClause
										WHERE AttributeTypeName IN ('Text Area')
										FOR XML PATH (''), TYPE).value('.', ' Nvarchar(MAX)'), 1, 0, '')
				SET @SQL = ' 
					;With Cte_AttributeDetails AS 
					(
					SELECT TBLAV.PimProductId,TBLAV.AttributeCode,ZPAVL.AttributeValue,ZPAVL.LocaleId ,COUNT(*)Over(Partition By TBLAV.PimProductId,TBLAV.AttributeCode ORDER BY TBLAV.PimProductId,TBLAV.AttributeCode  ) RowId
					FROM #TBL_AttributeValueId TBLAV 
					INNER JOIN ZnodePimProductAttributeTextAreaValue ZPAVL ON (ZPAVL.PimAttributeValueId = TBLAV.PimAttributeValueId )
					WHERE (LocaleId = '+Cast(@DefaultLocaleId AS Varchar(300))+' OR LocaleId = '+CAST(@LocaleId AS Varchar(300))+' )
	 
					) 
					SELECT  TBLAV.PimProductId 
  					FROM #TBL_AttributeValueId TBLAV
					'+@InternalProductWhereClause+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' WHERE EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END 
										+' 
										GROUP BY TBLAV.PimProductId 	
										'
				END 
			DELETE FROM #TBL_PimProductIdDelete 
			INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
			EXEC (@SQL)
			DELETE FROM #TBL_PimProductId
			INSERT INTO #TBL_PimProductId
			SELECT PimProductId FROM #TBL_PimProductIdDelete

		END 
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_FilterClause WHERE AttributeCode  LIKE '%PublishStatus%' )
		BEGIN    
 
				SET @SQL = '
							SELECT  TBLAV.PimProductId 
							FROM ZnodePimProduct TBLAV
							WHERE CASE WHEN TBLAV.IsProductPublish  IS NULL THEN ''Not Published'' 
							WHEN TBLAV.IsProductPublish = 0 THEN ''Draft''
							ELSE  ''Published'' END '+(SELECT TOP 1 AttributeValue FROM @TBL_FilterClause WHERE AttributeCode LIKE '%PublishStatus%')+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' AND EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END 
										+' GROUP BY TBLAV.PimProductId '
  
				DELETE FROM #TBL_PimProductIdDelete 
				INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
				EXEC (@SQL)
				DELETE FROM #TBL_PimProductId
				INSERT INTO #TBL_PimProductId
				SELECT PimProductId FROM #TBL_PimProductIdDelete

		END 
		IF EXISTS (SELECT TOP 1 1 FROM @TBL_FilterClause WHERE AttributeCode  LIKE '%AttributeFamily%' )
		BEGIN 

				;With Cte_attributeValue AS 
				(
					SELECT ZPAF.PimAttributeFamilyId,FamilyCode,AttributeFamilyName ,ZPFL.LocaleId
					FROM ZnodePimAttributeFamily ZPAF
					INNER JOIN ZnodePimFamilyLocale ZPFL ON (ZPFL.PimAttributeFamilyId = ZPAF.PimAttributeFamilyId) 
					WHERE ZPFL.LocaleId IN (@DefaultLocaleId,@LocaleId)
				) 
				, Cte_AttributeValueAttribute AS 
				(
					SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
					FROM Cte_attributeValue RTY 
					WHERE LocaleId = @LocaleId
				)
				, Cte_AttributeValueTht AS 
				(
					SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
					FROM Cte_AttributeValueAttribute
					UNION ALL 
					SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
					FROM Cte_attributeValue TYY  
					WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_AttributeValueAttribute THE WHERE THE.PimAttributeFamilyId = TYY.PimAttributeFamilyId )
					AND TYY.LocaleId = @DefaultLocaleId
				)
				SELECT PimAttributeFamilyId,FamilyCode,AttributeFamilyName
				INTO #TBL_FamilyLocale
				FROM Cte_AttributeValueTht 


				SET @SQL = '
							SELECT  TBLAV.PimProductId 
							FROM ZnodePimProduct TBLAV 
							INNER JOIN #TBL_FamilyLocale THY ON (THY.PimAttributeFamilyId = TBLAV.PimAttributeFamilyId )
							WHERE AttributeFamilyName '+(SELECT TOP 1 AttributeValue FROM @TBL_FilterClause WHERE AttributeCode LIKE '%AttributeFamily%')+CASE WHEN NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId ) THEN '' 
										ELSE ' AND EXISTS (SELECT TOP 1 1 FROM #TBL_PimProductId TBLP WHERE TBLP.PimProductId = TBLAV.PimProductId ) ' END 
										+' GROUP BY TBLAV.PimProductId '
  

			DELETE FROM #TBL_PimProductIdDelete 
			INSERT INTO #TBL_PimProductIdDelete  (PimProductId)
			EXEC (@SQL)
			DELETE FROM #TBL_PimProductId
			INSERT INTO #TBL_PimProductId
			SELECT PimProductId FROM #TBL_PimProductIdDelete

		END 
	
		SET @SQL = '
		IF EXISTS ( SELECT TOP 1 1 FROM tempdb..sysobjects WHERE name = ''##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))+''' )
		BEGIN 
			DROP TABLE ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))+'
		END 
		CREATE TABLE ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))+' (PimProductId INT )
		INSERT INTO  ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))+'
		SELECT PimProductId 
		FROM #TBL_PimProductId
		'
		EXEC (@SQL)
		DROP TABLE #TBL_PimProductId
		DROP TABLE #TBL_AttributeValueId
		DROP TABLE #TBL_PimProductIdDelete
 END
 