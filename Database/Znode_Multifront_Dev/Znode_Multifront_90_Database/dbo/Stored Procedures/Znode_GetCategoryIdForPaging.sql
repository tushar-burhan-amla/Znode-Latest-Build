CREATE PROCEDURE [dbo].[Znode_GetCategoryIdForPaging]
( @WhereClauseXML XML           = '',
  @Rows           INT           = 10,
  @PageNo         INT           = 1,
  @Order_BY       VARCHAR(1000) = '',
  @RowsCount      INT,
  @LocaleId       INT           = 1,
  @AttributeCode  VARCHAR(MAX)  = '',
  @PimCategoryId  VARCHAR(MAX) = 0,
  @IsAssociated   BIT           = 0,
  @IsDebug    int  = 0)
AS 
/*
     Summary:- This Procedure is used to get CategoryDetails With paging from XML
     Unit Testing 
	 begin tran
	 -- SELECT * FROM ZnodePimCategoryAttributeValueLocale WHERE CategoryValue LIKE '%test%'
     EXEC Znode_GetCategoryIdForPaging '' ,10,1,'',0,1,'','29,26,28',1
	rollback tran
	*/

     BEGIN
         BEGIN TRY
             DECLARE @WhereClause TABLE
             (Id          INT IDENTITY(1, 1),
              WhereClause NVARCHAR(MAX)
             );
             DECLARE @SQL NVARCHAR(MAX)= '', @OrderClause NVARCHAR(MAX), @JoinWhereClause NVARCHAR(MAX)= '', @DefaultLocaleId VARCHAR(20)= dbo.Fn_GetDefaultLocaleId(), @LocaleIds VARCHAR(20)= @LocaleId;
             DECLARE @ValueId INT= 1, @MaxValueId INT= 0;
             DECLARE @TBL_PimCategoryId TABLE (PimCategoryId INT ,RowId INT , CountNo INT )
				
			 IF @PimCategoryId <> '0' AND @PimCategoryId <> ''
                
                 BEGIN
                     SET @SQL = ' 
					     DECLARE @TBL_PimCategoryId TABLE (PimCategoryId INT )
						INSERT INTO @TBL_PimCategoryId
						SELECT Item FROM dbo.Split('''+@PimCategoryId+''','','') SP ';
                     IF @IsAssociated = 0
                         BEGIN
                             SET @JoinWhereClause = ' AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PimCategoryId TBPC WHERE TBPC.PimCategoryid = ZPCAV.PimCategoryId )';
                         END;
                     ELSE
                         BEGIN
                             SET @JoinWhereClause = ' AND EXISTS (SELECT TOP 1 1 FROM @TBL_PimCategoryId TBPC WHERE TBPC.PimCategoryid = ZPCAV.PimCategoryId )';
                         END;
                 END;
             IF @Order_BY LIKE '%CategoryId%'
                 BEGIN
                     SET @OrderClause = REPLACE(@Order_BY, 'PimCategoryId', 'CTCDL.PimCategoryId');
                 END;
             ELSE
             IF @Order_BY = '%Family%'
                 BEGIN
                     SET @OrderClause = REPLACE(@Order_BY, 'PimCategoryId', 'CTCDL.PimCategoryId');
                 END;
             ELSE
             IF @Order_BY = ''
                 BEGIN
                     SET @OrderClause = 'CTCDL.PimCategoryId DESC';
                 END;;
             SET @SQL = @SQL+'  
			 ;WITH Cte_AttributeFamilyLocale
                  AS (SELECT ZPAF.PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,ZPFL.AttributeFamilyName,ZPFL.LocaleId
                      FROM ZnodePimAttributeFamily ZPAF
                      INNER JOIN ZnodePimFamilyLocale ZPFL ON(ZPFL.PimAttributeFamilyId = ZPAF.PimAttributeFamilyId)
                      WHERE LocaleId IN('+CAST(@LocaleId AS VARCHAR(50))+', '+CAST(@DefaultLocaleId AS VARCHAR(50))+')
                       ),

                  Cte_AttributeFirstLocale
                  AS (SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                      FROM Cte_AttributeFamilyLocale
                      WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'),

                  Cte_AttributeBothLocale
                  AS (
                  SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                  FROM Cte_AttributeFirstLocale
                  UNION ALL
                  SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                  FROM Cte_AttributeFamilyLocale CTAFL
                  WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
                        AND NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_AttributeFirstLocale CTAFL
                      WHERE CTAFL.PimAttributeFamilyId = CTAFL.PimAttributeFamilyId
                  ))
                
			 		 
			,Cte_CategoryAttributeValue AS  
		(
		  SELECT PimCategoryId,ZPA.AttributeCode ,ZPCAVL.CategoryValue AttributeValue , ZPCAVL.LocaleId
		  FROM ZnodePimCategoryAttributeValueLocale ZPCAVL  
		  LEFT JOIN ZnodePimCategoryAttributeValue ZPCAV ON (ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId)
		  LEFT JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPCAV.PimAttributeId )  
		  WHERE LocaleId  IN ('+@LocaleIds+' , '+@DefaultLocaleId+')
		  AND EXISTS (SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryGridAttributeDetails]() FNGCGDA WHERE FNGCGDA.PimAttributeId = ZPA.PimAttributeId )
		  '+@JoinWhereClause+'
		 )
		 , Cte_CategoryAttributeValueFirstLocale AS 
		 (
		   SELECT PimCategoryId,AttributeCode ,AttributeValue  
		   FROM Cte_CategoryAttributeValue  CTCAV 
		   WHERE LocaleId = '+@LocaleIds+'
		 )
		 , Cte_CategoryDefaultLocale AS 
		 (
		   SELECT PimCategoryId,AttributeCode ,AttributeValue 
		   FROM Cte_CategoryAttributeValueFirstLocale 
		   UNION ALL 
		   SELECT PimCategoryId,AttributeCode ,AttributeValue 
		   FROM Cte_CategoryAttributeValue CTCAV 
		   WHERE LocaleId = '+@DefaultLocaleId+'
		   AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_CategoryAttributeValueFirstLocale CTCAVFL WHERE CTCAVFL.PimCategoryId = CTCAV.PimCategoryId AND CTCAVFL.AttributeCode = CTCAV.AttributeCode)
		   UNION ALL 
		   SELECT ZPCAV.PimCategoryId , ''attributefamily'' , AttributeFamilyName AttributeValue
		   FROM ZnodePimCategory ZPCAV 
		   LEFT JOIN Cte_AttributeBothLocale		TBLF ON (TBLF.PimAttributeFamilyId = ZPCAV.PimAttributeFamilyId AND TBLF.IScategory = 1 )  
		   WHERE 1=1 '+@JoinWhereClause+'
		 ) ';
             INSERT INTO @WhereClause(WhereClause)
                    SELECT WhereClause
                    FROM dbo.Fn_GetWhereClauseXML(@WhereClauseXML);

					If @IsDebug =1 
					Begin 
						Select * from @WhereClause
					End 
             SET @MaxValueId =
             (
                 SELECT MAX(Id)
                 FROM @WhereClause
             );
             WHILE @ValueId <= @MaxValueId
                 BEGIN
                     SET @SQL = @SQL+' , Cte_CategoryDetails_'+CAST(@ValueId AS VARCHAR(10))+' AS  
		   ( 
						SELECT CTCDL.PimCategoryId 
					    FROM '+CASE
                                        WHEN @ValueId = 1
                                        THEN 'Cte_CategoryDefaultLocale CTCDL'
                                        ELSE 'Cte_CategoryDetails_'+CAST(@ValueId - 1 AS VARCHAR(10))+' CTCDN '
                                    END+' 
						'+CASE
                                    WHEN @ValueId = 1
                                    THEN ''
                                    ELSE ' INNER JOIN  Cte_CategoryDefaultLocale CTCDL ON (CTCDL.PimCategoryId = CTCDN.PimCategoryId ) '
                                END+'
						WHERE '+
                     (
                         SELECT TOP 1 WhereClause
                         FROM @WhereClause
                         WHERE id = @ValueId
                     )+'		             
		   )
		   ';
                     SET @ValueId = @ValueId + 1;
                 END;
             SET @JoinWhereClause = CASE
                                        WHEN @OrderClause IS NULL
                                        THEN 'AND CTCDLD.AttributeCode = '''+dbo.Fn_Trim(REPLACE(REPLACE(@Order_BY, 'DESC', ''), 'ASC', ''))+''''
                                        ELSE ''
                                    END;
             SET @OrderClause = ISNULL(@OrderClause, 'CTCDLD.AttributeValue'+CASE
                                                                                 WHEN @Order_BY LIKE '% DESC%'
                                                                                 THEN ' DESC'
                                                                                 ELSE ' ASC '
                                                                             END);
             SET @SQL = @SQL+' ,Cte_finalCategoryDetails AS 
		(
			SELECT CTCDL.PimCategoryId ,'+[dbo].[Fn_GetPagingRowId](@OrderClause, 'CTCDL.PimCategoryId')+',Count(*)Over() CountId  
			FROM '+CASE
                          WHEN NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM @WhereClause
             )
                          THEN 'Cte_CategoryDefaultLocale CTCDL'
                          ELSE 'Cte_CategoryDetails_'+CAST((@ValueId - 1) AS VARCHAR(10))+' CTCDL '
                      END+'
			'+CASE
                     WHEN NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM @WhereClause
             )
                          AND @Order_BY = ''
                     THEN ''
                     ELSE ' LEFT JOIN  Cte_CategoryDefaultLocale CTCDLD ON (CTCDL.PimCategoryId = CTCDLD.PimCategoryId '+@JoinWhereClause+') '
                 END+'  
			GROUP BY CTCDL.PimCategoryId'+','+REPLACE(REPLACE(ISNULL(@OrderClause, 'CTCDLD.AttributeValue'), 'DESC', ''), 'ASC', '')+'				   
		) 
		
		SELECT PimCategoryId , CountId ,RowId
		FROM Cte_finalCategoryDetails		'+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
         
		 
		     PRINT @SQL
			 EXEC (@SQL);

          
		 END TRY
         BEGIN CATCH
    --         DECLARE @Status BIT ;
		  --   SET @Status = 0;
		  --   DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryIdForPaging @WhereClause = '+CAST(@WhereClause AS NVARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
    --         SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
    --         EXEC Znode_InsertProcedureErrorLog
				--@ProcedureName = 'Znode_GetCategoryIdForPaging',
				--@ErrorInProcedure = @Error_procedure,
				--@ErrorMessage = @ErrorMessage,
				--@ErrorLine = @ErrorLine,
				--@ErrorCall = @ErrorCall;
				select Error_message();
         END CATCH;
     END;