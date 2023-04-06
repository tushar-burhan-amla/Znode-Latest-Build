CREATE PROCEDURE [dbo].[Znode_GetPimDownloadableProductKeyList]
(
 @WhereClause NVARCHAR(3000),
 @Rows        INT            = 100,
 @PageNo      INT            = 1,
 @Order_BY    VARCHAR(1000)  = 'CreatedDate desc',
 @RowsCount   INT OUT
 )
 
AS

/*
 Summary :
 This procedure use to find the list of  DownloadableProduct Keys

 declare @p7 int
set @p7=8
exec sp_executesql N'Znode_GetPimDownloadableProductKeyList @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT',N'@WhereClause nvarchar(19),@Rows int,@PageNo int,@Order_BY nvarchar(1000),@RowsCount int output',@WhereClause=N'sku like ''%milton%''',@Rows=50,@PageNo=1,@Order_BY=N'DownloadableProductKey desc',@RowsCount=@p7 output
select @p7

*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		  if Isnull(@Order_BY,'') =''
		  Set @Order_BY='CreatedDate desc'

             DECLARE @SQL NVARCHAR(MAX);
             SET @SQL = '
					DECLARE @PimDownloadableProductKey TABLE (PimDownloadableProductKeyId int,PimDownloadableProductId int,SKU Nvarchar(300),
					DownloadableProductKey	nvarchar(250),DownloadableProductURL nvarchar(2000),IsUsed bit,CreatedDate datetime,RowId int)
					

					;With Cte_GetFormBuilderDetails 
					 AS     ( select ZPDPK.PimDownloadableProductKeyId,ZPDPK.PimDownloadableProductId,ZPDP.SKU, ZPDPK.DownloadableProductKey,
								ZPDPK.DownloadableProductURL,ZPDPK.IsUsed,ZPDPK.CreatedDate
							from ZnodePimDownloadableProduct ZPDP 
								INNER JOIN ZnodePimDownloadableProductKey ZPDPK
								ON ZPDP.PimDownloadableProductId =ZPDPK.PimDownloadableProductId 								
							)
				    INSERT INTO @PimDownloadableProductKey
					select PimDownloadableProductKeyId,PimDownloadableProductId,SKU, DownloadableProductKey,
					DownloadableProductURL,IsUsed,CreatedDate, '+[dbo].[Fn_GetPagingRowId](@Order_BY,'PimDownloadableProductKeyId')+'
					from Cte_GetFormBuilderDetails
					WHERE 1=1 '+CASE
									WHEN @WhereClause = ''
									THEN ''
									ELSE ' AND '+@WhereClause
								END
								+' ORDER BY '+CASE
													 WHEN @Order_BY = ''
													 THEN ' 1 '
													 ELSE ' '+ @Order_BY
												 END+' SELECT @Count = COUNT (1) FROM @PimDownloadableProductKey  
												 SELECT * FROM @PimDownloadableProductKey 
												' +[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '+ CASE
													 WHEN @Order_BY = ''
													 THEN ' ORDER BY  1 '
													 ELSE ' ORDER BY  '+ @Order_BY END ; 
													 
					
								 EXEC SP_executesql
									  @SQL,
									  N'@Count INT OUT ',
									  @Count = @RowsCount OUT;

									  
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimDownloadableProductKeyList @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');
              	select ERROR_MESSAGE()		 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimDownloadableProductKeyList',
				@ErrorInProcedure = 'Znode_GetPimDownloadableProductKeyList',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END