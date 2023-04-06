CREATE PROCEDURE [dbo].[Znode_GetCustomReport]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT
 )
AS

/*
 Summary : This Procedure is used to get CustomReport  
 Unit Testing
 EXEC [Znode_GetCustomReport]  @RowsCount = 0
 
*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY

		  DECLARE @SQL NVARCHAR(MAX);
		  DECLARE @TBL_GetCustomReport TABLE (CustomReportTemplateId INT,Name NVARCHAR(100),ReportTypeId INT,ReportType VARCHAR(250) ,RowId INT,CountNo INT )

		   SET @SQL = '
				;with Cte_GetCustomReportDetail AS 
				(  SELECT ZCRT.CustomReportTemplateId,ZCRT.ReportName as Name, ZIH.ImportHeadId as ReportTypeId,ZIH.Name AS ReportType
		          FROM  ZnodeCustomReportTemplate ZCRT INNER JOIN ZnodeImportHead ZIH on (ZCRT.ImportHeadId= ZIH.ImportHeadId )					 				
				 ) 
				 ,  Cte_GetCustomReport AS 
				 (SELECT  * ,'+dbo.Fn_GetPagingRowId(@Order_BY,'CustomReportTemplateId DESC')+',Count(*)Over() CountNo
				  FROM Cte_GetCustomReportDetail
				  WHERE 1=1 
				  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'				 
				 )

				 SELECT CustomReportTemplateId,Name,ReportTypeId,ReportType,RowId,CountNo
				 from Cte_GetCustomReport
				 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

				 INSERT INTO @TBL_GetCustomReport
				 EXEC(@SQL);

				 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetCustomReport), 0);

				 SELECT CustomReportTemplateId,Name,ReportTypeId,ReportType FROM @TBL_GetCustomReport
		
		END TRY
		BEGIN CATCH
		     DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCustomReport @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCustomReport',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH

END