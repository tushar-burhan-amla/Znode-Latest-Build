CREATE PROCEDURE [dbo].[Znode_GetFilteredPimProductIds]
(@WhereClause       NVARCHAR(3000),
 @Rows              INT            = 10,
 @PageNo            INT            = 1,
 @Order_BY          VARCHAR(1000)  = '',
 @RowsCount         INT OUT,
 @LocaleId          INT            = 1,
 @PimProductId      NVARCHAR(2000) = 0,
 @FilterTheProducts BIT            = 0)
AS 
/*
   Summary:
     This procedre is used for internal database process for finding the productids 
	 Unit Testing:
	 begin tran
     EXEC Znode_GetFilteredPimProductIds @WhereClause = '',@Rows=10,@PageNo = 1 ,@Order_BY='DisplayOrder ASC',@RowsCount=0,@LocaleId= 1,@PimProductId=7,@FilterTheProducts=1
	 rollback tran
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX), @RowsStart VARCHAR(50), @RowsEnd VARCHAR(50);
             SET @RowsStart = CASE
                                  WHEN @Rows >= 1000000
                                  THEN 0
                                  ELSE(@Rows * (@PageNo - 1)) + 1
                              END;
             SET @RowsEnd = CASE
                                WHEN @Rows >= 1000000
                                THEN @Rows
                                ELSE @Rows * (@PageNo)
                            END;
             DECLARE @DefaultLocaleId INT=
             (
                 SELECT TOP 1 FeatureValues
                 FROM ZnodeGlobalSetting
                 WHERE FeatureName = 'Locale'
             );

             DECLARE @ConvertTableData TABLE
             (RowId             INT IDENTITY(1, 1),
              Id                INT,
              CompleteStatement VARCHAR(2000),
              LogicalOprator    VARCHAR(100),
              Oprator           NVARCHAR(100),
              LeftStatement     VARCHAR(2000),
              RightStatement    NVARCHAR(MAX)
             );

             DECLARE @ProductIds TABLE(PimProductId INT);

             DECLARE @FilterTableAdded TABLE
             (RowId         INT IDENTITY(1, 1),
              WherStatement NVARCHAR(MAX)
             );

             DECLARE @ProductAttributeDetials TABLE
             (PimProductId   INT,
              AttributeCode  NVARCHAR(600),
              AttributeValue NVARCHAR(MAX),
              LocaleId       INT
             );

             DECLARE @ProductFinalDetails TABLE
             (PimProductId INT,
              ProductName  NVARCHAR(MAX),
              SKU          NVARCHAR(MAX)
             );

             DECLARE @RowId INT, @IncrementId INT= 1;
             DECLARE @WherStatement NVARCHAR(MAX);

             INSERT INTO @ConvertTableData
             (Id,
              CompleteStatement,
              LogicalOprator,
              Oprator,
              LeftStatement,
              RightStatement
             )
             EXEC Znode_SplitWhereClause
                  @WhereClause;

             INSERT INTO @FilterTableAdded
                    SELECT 'AttributeCode IN (''ProductName'', ''SKU'', ''Price'', ''Quantity'', ''IsActive'',''ProductType'',''ProductImage'',''Assortment'',''DisplayOrder'')'
                    WHERE @WhereClause = ''
                          AND @Order_BY = '';
             IF @FilterTheProducts = 0
                 BEGIN
                     INSERT INTO @FilterTableAdded
                            SELECT '  NOT EXISTS (SELECT TOP 1 1 FROM dbo.split('''+@PimProductId+''','','' ) zp WHERE  zp.Item = zav.PimProductId )';
                 END;
             IF @LocaleId <> @DefaultLocaleId
                 BEGIN
                     INSERT INTO @FilterTableAdded
                            SELECT ' ZAVL.LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(100))+' AND NOT EXISTS ( SELECT TOP 1 1 FROM @Tbl_PimLocaleIds TBPL WHERE TBPL.PimProductId = zav.PimProductId )';
                 END;
             ELSE
                 BEGIN
                     INSERT INTO @FilterTableAdded
                            SELECT ' ZAVL.LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(100));
                 END;
             INSERT INTO @FilterTableAdded
                    SELECT 'AttributeCode = '''+RTRIM(LTRIM(LeftStatement))+''' AND zavl.AttributeValue '+Oprator+' '+RTRIM(LTRIM(RightStatement))
                    FROM @ConvertTableData
                    UNION ALL
                    SELECT ' AttributeCode ='''+LTRIM(RTRIM(REPLACE(REPLACE(@Order_BY, ' DESC', ''), ' ASC', '')))+'''';
            
			 DECLARE Cur_ForFindThePimProductIds CURSOR
             FOR SELECT RowId,
                        WherStatement
                 FROM @FilterTableAdded;
             OPEN Cur_ForFindThePimProductIds;
             FETCH NEXT FROM Cur_ForFindThePimProductIds INTO @RowId, @WherStatement;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                     IF @RowId = 1
                         BEGIN
                             IF @LocaleId <> @DefaultLocaleId
                                 BEGIN
                                     SET @SQL = ' 

			  DECLARE @Tbl_PimProductIds TABLE (PimProductId INT,RowId INT)  
			  DECLARE @Tbl_PimLocaleIds TABLE (PimProductId INT)
		 
			  INSERT INTO @Tbl_PimLocaleIds
			  SELECT PimProductId
			  FROM ZnodePimAttributeValue zav 
			  INNER JOIN ZnodePimAttributeValueLocale ZAVL ON (ZAVL.PimAttributeValueId = ZAV.PimAttributeValueId )   
			  WHERE ZAVL.LocaleId = '+CAST(@LocaleId AS VARCHAR(100))+'	
			  GROUP BY PimProductId

     			 ;With  Cte_ProductSkuList AS ( 
			 
				 SELECT * 
				 FROM @Tbl_PimLocaleIds 

				 UNION ALL 
			 
				 ';
									 END;
								 ELSE
									 BEGIN
										 SET @SQL = ' 
			  DECLARE @Tbl_PimProductIds TABLE (PimProductId INT,RowId INT)  

			 ;With  Cte_ProductSkuList AS ( ';
									 END;
							 END;
						 ELSE
							 BEGIN
								 SET @SQL = @SQL+' , Cte_ProductSkuList_'+CAST(@RowId AS VARCHAR(100))+' AS ( ';
							 END;
						 SET @SQL = @SQL+'

			SELECT PimProductId '+CASE
										WHEN @RowId =
						 (
							 SELECT MAX(ROWId)
							 FROM @FilterTableAdded
						 )
										THEN '

			,DENSE_RANK()OVER(ORDER BY '+CASE
											   WHEN @Order_BY = ''
											   THEN '  '
											   ELSE ' ZAVL.AttributeValue '+CASE
																				WHEN @Order_BY LIKE '%ASC%'
																				THEN 'ASC'
																				ELSE 'DESC'
																			END+' , '
										   END+' PimProductId DESC ) RowId'
										ELSE ''
									END+' 
		 


			FROM ZnodePimAttributeValue zav 
			INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZAV.PimAttributeId)
			INNER JOIN ZnodePimAttributeValueLocale ZAVL ON (ZAVL.PimAttributeValueId = ZAV.PimAttributeValueId )
			WHERE 1=1
			'+CASE
					WHEN @RowId > 1
					THEN ' AND EXISTS (SELECT TOP 1 1 FROM   Cte_ProductSkuList'+CASE
																					 WHEN @RowId - 1 = 1
																					 THEN ' a WHERE a.PimProductId = zav.PimProductId )'
																					 ELSE '_'+CAST(@RowId - 1 AS VARCHAR(100))+' a WHERE a.PimProductId = zav.PimProductId )'
																				 END
					ELSE ''
				END+'
			'+CASE
					WHEN @WherStatement = ''
					THEN ''
					ELSE 'AND ('+@WherStatement+')'
				END+'
			Group By PimProductId 
		

			'+CASE
					WHEN @RowId =
						 (
							 SELECT MAX(ROWId)
							 FROM @FilterTableAdded
						 )
					THEN CASE
							 WHEN @Order_BY = ''
							 THEN '  '
							 ELSE ', ZAVL.AttributeValue '
						 END
					ELSE ''
				END+' 
		
		 
			)

			';
						 SET @IncrementId = @IncrementId + 1;
						 FETCH NEXT FROM Cur_ForFindThePimProductIds INTO @RowId, @WherStatement;
					 END;
				 CLOSE Cur_ForFindThePimProductIds;
				 DEALLOCATE Cur_ForFindThePimProductIds;
				 SET @SQL = @SQL+' 
		 
			 INSERT INTO @Tbl_PimProductIds (PimProductId,RowId) 
			 SELECT * 
			 FROM '+CASE
						  WHEN
				 (
					 SELECT COUNT(1)
					 FROM @FilterTableAdded
				 ) = 1
						  THEN 'Cte_ProductSkuList'
						  ELSE 'Cte_ProductSkuList_'+CAST(@RowId AS VARCHAR(100))
					  END+'

			 '+' SELECT @Count = COUNT(1) FROM @Tbl_PimProductIds '+' SELECT  PimProductId FROM @Tbl_PimProductIds WHERE RowId BETWEEN '+@RowsStart+'  AND '+@RowsEnd;
            
				 EXEC Sp_Executesql
					  @SQL,
					  N' @Count INT OUT ',
					  @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetFilteredPimProductIds @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimProductId='+@PimProductId+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@FilterTheProducts='+CAST(@FilterTheProducts AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetFilteredPimProductIds',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;