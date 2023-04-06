
CREATE  PROCEDURE [dbo].[Znode_GetFormBuilderList]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT

)
AS 
   /* 
   SUMMARY : Stored Procedure to Get list of searchProfileid 
   Unit Testing:

   -- EXEC Znode_GetFormBuilderList @WhereClause = '',@RowsCount = 0
   
   
   	*/

     BEGIN
         BEGIN TRY

		 SET NOCOUNT ON 

		 DECLARE @SQL  NVARCHAR(max) 

		 
			DECLARE @TBL_FormBuilder TABLE (FormBuilderId INT,FormCode nvarchar(200), FormDescription nvarchar(200), RowId INT, CountNo INT)


		SET @SQL = '

		;With Cte_GetFormBuilderDetails 
		 AS     (

				SELECT  ZFB.FormBuilderId,ZFB.FormCode,ZFB.FormDescription
				FROM ZnodeFormBuilder ZFB
				--INNER JOIN ZnodePortal ZP ON (ZP.PortalId = ZFB.PortalId)
								
				)



				,Cte_GetFilterFormBuilder
				AS (
				SELECT FormBuilderId,FormCode,FormDescription,
				'+dbo.Fn_GetPagingRowId(@Order_BY,'FormBuilderId DESC')+',Count(*)Over() CountNo 
				FROM  Cte_GetFormBuilderDetails CGPTL 
				WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						
				)
																								
				SELECT FormBuilderId,FormCode,FormDescription,RowId,CountNo
				FROM Cte_GetFilterFormBuilder
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
						
						
				INSERT INTO @TBL_FormBuilder(FormBuilderId,FormCode,FormDescription,RowId,CountNo)
				EXEC(@SQL)

				SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_FormBuilder ),0)
			
				SELECT FormBuilderId,FormCode,FormDescription
				FROM @TBL_FormBuilder

			
				
		
		 END TRY
		 BEGIN CATCH
			 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetFormBuilderList @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetFormBuilderList',
				@ErrorInProcedure = 'Znode_GetFormBuilderList',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END