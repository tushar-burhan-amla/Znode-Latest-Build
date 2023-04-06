CREATE PROCEDURE [dbo].[Znode_GetPublishProductDetail]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT)
AS 
/*
     Summary :- This procedure is used to get the publish products details 
				Result is fetched order by PublishProductId
     Unit Testing 
     EXEC Znode_GetPublishProductDetail '',@RowsCount=0
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_PublishProductDetail TABLE
             (ProductName      NVARCHAR(4000),
			  sku			   NVARCHAR(2000),
              PublishProductId INT,
              PublishCatalogId INT
              ,RowId INT,CountNo INT
             );

             SET @SQL = '
			 ;With Cte_PublishCatalogDetail AS 
			 (SELECT zpd.ProductName,zpd.sku,zpd.PublishProductId,Zp.PublishCatalogId,zpd.LocaleId,Boost FROM ZnodePublishProductDetail zpd 			 
			 INNER JOIN ZnodePublishProduct zp ON zpd.PublishProductId=zp.PublishProductId 
			 LEFT JOIN ZnodeSearchGlobalProductBoost ZSB ON (ZSB.PublishProductId = ZP.PublishProductId )
			 )
			 , Cte_PublishProductDetails AS
			 (SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'PublishProductId DESC')+',Count(*)Over() CountNo  
			 FROM Cte_PublishCatalogDetail CTPC 
			 WHERE 1=1 
			 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			 )

			 SELECT ProductName,sku,PublishProductId ,PublishCatalogId ,RowId,CountNo FROM Cte_PublishProductDetails 			
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
            
             INSERT INTO @TBL_PublishProductDetail
             EXEC (@SQL);
             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_PublishProductDetail), 0);
           
             SELECT ProductName,sku,PublishProductId,PublishCatalogId FROM @TBL_PublishProductDetail;
			
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishProductDetail @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName    = 'Znode_GetPublishProductDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage     = @ErrorMessage,
				@ErrorLine        = @ErrorLine,
				@ErrorCall        = @ErrorCall;
         END CATCH;
     END;