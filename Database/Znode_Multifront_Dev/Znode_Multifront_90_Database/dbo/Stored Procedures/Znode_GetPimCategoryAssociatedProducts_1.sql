

CREATE PROCEDURE [dbo].[Znode_GetPimCategoryAssociatedProducts](
      @WhereClause         NVARCHAR(3000) ,
      @Rows                INT            = 100 ,
      @PageNo              INT            = 1 ,
      @Order_BY            VARCHAR(1000)  = '' ,
      @RowsCount           INT OUT ,
      @LocaleId            INT            = 1 ,
      @PimCategoryId       INT ,
      @IsAccociatedProduct BIT            = 0 ,
      @PimProductId        NVARCHAR(2000) = NULL ,
      @IsProductNotIn      BIT            = 0)
AS
--Summary : 

-- This Procedure is used for get product List 


--Unit testing : -- declare @p7 int = 0  EXEC Znode_GetPimCategoryAssociatedProducts @WhereClause=N'',@Rows=10,@PageNo=2,@Order_By=N'DisplayOrder asc',@RowsCount=@p7 output,@PimCategoryId=2,@IsAccociatedProduct=1,@LocaleId=1 SELECT @p7

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
	        --
            DECLARE 
		  @LocaleIdDefault INT= ( SELECT TOP 1 FeatureValues  FROM ZnodeGlobalSetting  WHERE FeatureName = 'Locale'),
		  @Rows_start VARCHAR(1000) ,
		  @Rows_end VARCHAR(1000) ,
		  @IsAccociatedProduct_New INT= CASE  WHEN @IsAccociatedProduct = 'true'  THEN 1  ELSE 0 END;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @ProductIdTable TABLE (
                                           PimProductId INT
                                           );
             SET @Rows_start = CASE
                                   WHEN @Rows >= 1000000
                                   THEN 0
                                   ELSE ( @Rows * ( @PageNo - 1 ) ) + 1
                               END;
             SET @Rows_end = CASE
                                 WHEN @Rows >= 1000000
                                 THEN @Rows
                                 ELSE @Rows * ( @PageNo )
                             END;
             DECLARE @CHECKDESK VARCHAR(MAX);
             DECLARE @TableValue TABLE (
                                       Id           INT IDENTITY(1 , 1) ,
                                       FilterColumn VARCHAR(3000) ,
                                       FilterClause VARCHAR(MAX)
                                       );
             DECLARE @String VARCHAR(MAX) , @WhereClause_inner VARCHAR(MAX)= '';
             SET @Order_BY = CASE
                                 WHEN @Order_BY LIKE '%pimcategoryid%'
                                 THEN ''
                                 ELSE @Order_BY
                             END;
		   -- Split where clause with their operators
             INSERT INTO @TableValue ( FilterColumn , FilterClause
                                     )
             EXEC [dbo].[Znode_SplitWhereClause] @WhereClause , 1;
             -- Insert attri code for order by cluase
		   INSERT INTO @TableValue ( FilterColumn , FilterClause
                                     )

                    SELECT 'ORDER BY ' , 'AttributeCode = '''+RTRIM(LTRIM(REPLACE(REPLACE(@Order_BY , 'ASC' , '') , 'DESC' , '')))+''''
                    WHERE CASE
                              WHEN @Order_BY = ''
                                   OR
                                   @Order_BY LIKE '%Display%'
                              THEN 1
                              ELSE 0
                          END = 0;
             DECLARE @ValueId INT= 1;

		   -- Declare table variable @COUNTROWS  to insert data of RowId Between @Rows_start AND @Rows_End;
		   -- Insert data into @PimProductIds table variable by usting different filters pass in where cluase which is we have already split it and inserted into 
		   -- table @TableValue
		 
             SET @SQL = 'DECLARE @COUNTROWS TABLE (PimProductId INT ,AttributeValue Nvarchar(max),RowId INT  ) 
					DECLARE @PimProductIds TABLE (PimProductId INT) 
					INSERT INTO @PimProductIds 
					SELECT Item FROM dbo.Split(ISNULL(@PimProductId,0),'','') a 
	 

		  ;With ';
		  -- execute query in loop for multiple records of tables TableValue
             WHILE @ValueId <= ISNULL( ( SELECT MAX(Id)
                                         FROM @TableValue
                                       ) , 1)
                 BEGIN
		  -- Use with cluase and create dynamic table TestCheck with @ValueId and insert data for default locale and required localeId 
		  -- Du=ynamic query created and stored in variable @String
                     SET @String = CASE
                                       WHEN @ValueId = 1
                                       THEN ' TestCheck'+CAST(@ValueId AS VARCHAR(100))
                                       ELSE ', TestCheck'+CAST(@ValueId AS VARCHAR(100))
                                   END+'  AS ( SELECT a.PimProductId 
			'+CASE
                     WHEN @ValueId = ISNULL( ( SELECT MAX(Id)
                                               FROM @TableValue
                                             ) , 1)
                     THEN 
				 -- Used "RANK()Over(Order By" to generate rowid with diffrent products in order DisplayOrder if passed and a.PimProductId
				 ' 
				, RANK()Over(Order By '+CASE
                                                WHEN @Order_BY LIKE '%Display%'
                                                THEN REPLACE(@Order_BY , 'DisplayOrder' , ' CASE WHEN CAST (pcc.DisplayOrder AS INT ) IS NULL THEN 1 ELSE 0 END, CAST (pcc.DisplayOrder AS INT ) ')+','
                                                ELSE ''
                                            END+' a.PimProductId  ) RowId '+CASE
                                                                                WHEN @WhereClause <> ''
                                                                                     OR
                                                                                     ( @Order_BY <> ''
                                                                                       AND
                                                                                       @Order_BY NOT LIKE '%Display%' )
                                                                                THEN ',b.AttributeValue  '
                                                                                ELSE ''
                                                                            END
                     ELSE ''
                 END+'
				'+CASE
                          WHEN @Order_BY LIKE '%Display%'
                          THEN ' ,pcc.DisplayOrder '
                          ELSE ''
                      END+'
				FROM   ZnodePimAttributeValue a 
				INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId )
				INNER JOIN  ZnodePimAttributeValueLocale b ON ( b.PimAttributeValueId = a.PimAttributeValueId AND b.LocaleId IN ( ' +CAST( @LocaleId AS VARCHAR(100)) + ',' + CAST( @LocaleIdDefault AS VARCHAR(100)) + ') )  
				LEFT JOIN ZnodePimCategoryProduct pcc ON (pcc.PimProductId = a.PimProductId AND pcc.PimCategoryId = @pimcategoryIdin  )   
				WHERE '+ISNULL( ( SELECT REPLACE(FilterClause , 'AttributeValue' , 'b.AttributeValue')
                                      FROM @TableValue
                                      WHERE Id = @ValueId
                                    ) , '')+'
				'+CASE
                          WHEN @ValueId = 1
                          THEN ''
                          ELSE ' AND EXISTS (SELECT TOP 1 1 FROM TestCheck'+CAST(@ValueId - 1 AS VARCHAR(100))+' q WHERE q.PimProductId = a.PimProductId )'
                      END+' '+CASE
                                  WHEN @ValueId = 1
                                  THEN CASE
                                           WHEN EXISTS ( SELECT FilterClause
                                                         FROM @TableValue
                                                         WHERE Id = @ValueId
                                                       )
                                           THEN 'AND '
                                           ELSE ''
                                       END+' CASE WHEN pcc.PimCategoryId IS NULL THEN 0 ELSE 1 END = @IsAccociatedProductIn AND c.AttributeCode IN (''ProductName'',''IsActive'',''Image''
				'+CASE
                          WHEN @WhereClause_inner = ''
                          THEN ''
                          ELSE ','+@WhereClause_inner
                      END+' )'
                                  ELSE ''
                              END+' GROUP BY a.PimProductId '+CASE
                                                                  WHEN @ValueId = ISNULL( ( SELECT MAX(Id)
                                                                                            FROM @TableValue
                                                                                          ) , 1)
                                                                  THEN ' 
				'+CASE
                          WHEN @WhereClause <> ''
                               OR
                               ( @Order_BY <> ''
                                 AND
                                 @Order_BY NOT LIKE '%Display%' )
                          THEN ',b.AttributeValue  '
                          ELSE ''
                      END
                                                                  ELSE ''
                                                              END+' 
				'+CASE
                          WHEN @Order_BY LIKE '%Display%'
                          THEN ' , pcc.DisplayOrder '
                          ELSE ' '
                      END+'
				) ';
                     SET @SQL = @SQL + @String;
                     SET @ValueId = @ValueId + 1;
                 END;

             SET @SQL = @SQL+'INSERT INTO @COUNTROWS (PimProductId '+CASE
                                                                          WHEN @WhereClause <> ''
                                                                               OR
                                                                               ( @Order_BY <> ''
                                                                                 AND
                                                                                 @Order_BY NOT LIKE '%Display%' )
                                                                          THEN ',a.AttributeValue  '
                                                                          ELSE ''
                                                                      END+',RowId) SELECT PimProductId'
														+CASE
															 WHEN @WhereClause <> ''
																    OR
																    ( @Order_BY <> ''
																    AND
																    @Order_BY NOT LIKE '%Display%' )
															 THEN ',AttributeValue  '
															 ELSE ''
														END+',Row_NUMBER()OVER(ORDER BY '
														+CASE
															 WHEN @Order_BY LIKE '%Display%'
                                                                            THEN REPLACE(@Order_BY , 'DisplayOrder' , ' RoWId ')+','
                                                                            ELSE ''
                                                                        END+''
														  +CASE
                                                                            WHEN ( @Order_BY = ''
                                                                                OR
                                                                                @Order_BY LIKE '%Display%' )
                                                                                OR
                                                                                @Order_BY = 'PimPRoductId'
                                                                            THEN 'PimProductId'
                                                                            ELSE ' RTRIM(LTRIM(AttributeValue)) '
														+CASE
                                                                            WHEN @Order_BY LIKE '% DESC%'
                                                                            THEN ' DESC'
                                                                            ELSE ' ASC'
                                                                        END
														  END+' ) FROM  TestCheck'+CAST(@ValueId - 1 AS VARCHAR(100))++' rtr WHERE '
														  +CASE
                                                                            WHEN @IsProductNotIn <> 1
                                                                            THEN ' NOT '
                                                                            ELSE ''
                                                                        END+' EXISTS (SELECT TOP 1 1 FROM  @PimProductIds PId  WHERE PId.PimProductId = rtr.PimProductId ) '+' OPTION  (MAXDOP 1, RECOMPILE) SELECT @COunt = COUNT (1) FROM @COUNTROWS  
														  SELECT PimProductId FROM @COUNTROWS rtr '+'WHERE rtr.RowId Between '+@Rows_start+' AND '+@Rows_End;
             PRINT @SQL;
             INSERT INTO @ProductIdTable
             EXEC SP_executesql @SQL , N'@Count INT OUT ,@PimProductId VARCHAR(2000),@pimcategoryIdin INT,@IsAccociatedProductIn INT   ' , @PimProductId = @PimProductId , @pimcategoryIdin = @PimCategoryId , @IsAccociatedProductIn = @IsAccociatedProduct_New , @Count = @RowsCount OUT;
             DECLARE @AttributeDetails_locale TABLE (
                                                    PimProductId        INT ,
                                                    AttributeValue      NVARCHAR(MAX) ,
                                                    AttributeCode       VARCHAR(600) ,
                                                    AttributeFamilyName NVARCHAR(MAX) ,
                                                    LocaleId            INT
                                                    );
             DECLARE @AttributeDetails TABLE (
                                             PimProductId        INT ,
                                             AttributeValue      NVARCHAR(MAX) ,
                                             AttributeCode       VARCHAR(600) ,
                                             AttributeFamilyName NVARCHAR(MAX) ,
                                             LocaleId            INT
                                             );
             WITH PimAttributeGet
                  AS (SELECT zpa.PimAttributeId , zpa.ParentPimAttributeId , zpa.AttributeTypeId , zpa.AttributeCode , zpa.IsRequired , zpa.IsLocalizable , zpa.IsFilterable , zpa.IsSystemDefined , zpa.IsConfigurable , zpa.IsPersonalizable , zpa.DisplayOrder , zpa.HelpDescription , zpa.IsCategory , zpal.AttributeName , zpal.Description , zpal.LocaleId
                      FROM ZnodePimAttribute AS zpa INNER JOIN ZnodePimAttributeLocale AS zpal ON ( zpa.PimAttributeId = zpal.PimAttributeId
                                                                                                    AND
                                                                                                    zpal.localeId = @LocaleId )
                      WHERE zpa.AttributeCode IN ( 'ProductName' , 'IsActive' , 'Image' , 'DisplayOrder'
                                                 ) ) ,
                  PimAttributeIds
                  AS (
                  SELECT *
                  FROM PimAttributeGet AS a
                  UNION ALL
                  SELECT zpa.PimAttributeId , zpa.ParentPimAttributeId , zpa.AttributeTypeId , zpa.AttributeCode , zpa.IsRequired , zpa.IsLocalizable , zpa.IsFilterable , zpa.IsSystemDefined , zpa.IsConfigurable , zpa.IsPersonalizable , zpa.DisplayOrder , zpa.HelpDescription , zpa.IsCategory , zpal.AttributeName , zpal.Description , zpal.LocaleId
                  FROM ZnodePimAttribute AS zpa INNER JOIN ZnodePimAttributeLocale AS zpal ON ( zpa.PimAttributeId = zpal.PimAttributeId
                                                                                                AND
                                                                                                zpal.localeId = @LocaleIdDefault )
                  WHERE zpa.AttributeCode IN ( 'ProductName' , 'IsActive' , 'Image' , 'DisplayOrder'
                                             )
                        AND
                        NOT EXISTS ( SELECT TOP 1 1
                                     FROM PimAttributeGet AS wdsd
                                     WHERE wdsd.PimAttributeId = zpa.PimAttributeId
                                   ) )
                  INSERT INTO @AttributeDetails_locale
                         SELECT zpav.PimProductId , zpavl.AttributeValue , zpa.AttributeCode , '' AS AttributeFamilyName , zpa.LocaleId
                         FROM PimAttributeIds AS zpa INNER JOIN ZnodePimAttributeValue AS zpav ON ( zpa.PimAttributeId = zpav.PimAttributeId )
                                                     INNER JOIN ZnodePimAttributeValueLocale AS zpavl ON ( zpavl.PimAttributeValueId = zpav.PimAttributeValueId )
                         WHERE zpa.AttributeCode IN ( 'ProductName' , 'IsActive' , 'Image' , 'DisplayOrder'
                                                    )
                               AND
                               EXISTS ( SELECT TOP 1 1
                                        FROM @ProductIdTable AS cq
                                        WHERE cq.PimProductId = zpav.PimProductId
                                      )
                               AND
                               zpavl.LocaleId IN ( @LocaleId , @LocaleIdDefault
                                                 ); 

             --SELECT * FROM @AttributeDetails_locale

             INSERT INTO @AttributeDetails
                    SELECT *
                    FROM @AttributeDetails_locale
                    WHERE LocaleId = @LocaleId;
             INSERT INTO @AttributeDetails
                    SELECT *
                    FROM @AttributeDetails_locale AS q
                    WHERE LocaleId = @LocaleIdDefault
                          AND
                          NOT EXISTS ( SELECT TOP 1 1
                                       FROM @AttributeDetails AS a
                                       WHERE a.PimProductId = q.PimProductId
                                             AND
                                             a.AttributeCode = q.AttributeCode
                                     );
             --- find the specific attributes and values ---- 




             SELECT zpp.PimProductId , zf.PimCategoryProductId , @PimCategoryId AS PimCategoryId , [ProductName] , CAST(CASE
                                                                                                                            WHEN zf.DisplayOrder IS NULL
                                                                                                                            THEN PIv.DisplayOrder
                                                                                                                            ELSE zf.DisplayOrder
                                                                                                                        END AS INT) AS [DisplayOrder] ,
                                                                                                                                       CASE
                                                                                                                                           WHEN zf.[Status] IS NULL
                                                                                                                                           THEN CAST(0 AS BIT)
                                                                                                                                           ELSE zf.[Status]
                                                                                                                                       END AS [Status] , [dbo].FN_GetMediaThumbnailMediaPath ( zm.Path
                                                                                                                                                                                             ) AS ImagePath , Piv.LocaleId
             INTO #StoreForOrderBy
             FROM ZNodePimProduct AS zpp LEFT JOIN ZnodePimCategoryProduct AS zf ON ( zf.PimProductId = zpp.PimProductId
                                                                                      AND
                                                                                      zf.PimCategoryId = @PimCategoryId )
                                         INNER JOIN @AttributeDetails PIVOT(MAX(AttributeValue) FOR AttributeCode IN([ProductName] , [IsActive] , [Image] , [DisplayOrder])) AS Piv ON ( Piv.PimProductId = zpp.PimProductId )
                                         LEFT JOIN ZnodeMedia AS zm ON ( zm.MediaId = piv.[Image] );
             SET @SQL = 'SELECT * FROM #StoreForOrderBy ORDER BY '+CASE
                                                                       WHEN @Order_BY = ''
                                                                       THEN 'PimProductId'
                                                                       ELSE @Order_BY
                                                                   END;
             EXEC (@SQL);
	
             -- find the all locale values 
         END TRY
         BEGIN CATCH
             SELECT ERROR_LINE() , ERROR_MESSAGE() , ERROR_NUMBER();
         END CATCH;
     END;