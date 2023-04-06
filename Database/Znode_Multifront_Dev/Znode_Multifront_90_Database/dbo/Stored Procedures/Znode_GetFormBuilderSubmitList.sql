CREATE  PROCEDURE [dbo].[Znode_GetFormBuilderSubmitList]
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

   -- EXEC [Znode_GetFormBuilderSubmitList] @WhereClause = '',@RowsCount = 0
   
   
   	*/

     BEGIN
         BEGIN TRY

		 SET NOCOUNT ON 

		 DECLARE @SQL  NVARCHAR(max) 

		 
			DECLARE @TBL_FormBuilder TABLE (FormBuilderSubmitId INT,FormBuilderId INT,FormCode nvarchar(200), FormDescription nvarchar(200),
			CreatedDate datetime,PortalId int,StoreName nvarchar(max),UserId int,UserName nvarchar(max),FullName nvarchar(max),
			 RowId INT, CountNo INT)


		SET @SQL = '

		;With Cte_GetFormBuilderDetails 
		 AS     (

				SELECT ss.FormBuilderSubmitId, ZFB.FormBuilderId,ZFB.FormCode,ZFB.FormDescription,ss.CreatedDate,ss.PortalId ,ZP.StoreName,
				ss.UserId,zu.UserName,zu.FullName 
				FROM ZnodeFormBuilderSubmit ss 
				inner join ZnodeFormBuilder ZFB on ss.FormBuilderId=ZFB.FormBuilderId
				inner join ZnodePortal ZP on ZP.PortalId =ss.PortalId 
				left  join View_CustomerUserDetail ZU on ZU.UserId=ss.UserId

								
				)



				,Cte_GetFilterFormBuilder
				AS (
				SELECT FormBuilderSubmitId, FormBuilderId,FormCode,FormDescription,CreatedDate,PortalId,StoreName,UserId,UserName,FullName,
				'+dbo.Fn_GetPagingRowId(@Order_BY,'FormBuilderId DESC')+',Count(*)Over() CountNo 
				FROM  Cte_GetFormBuilderDetails CGPTL 
				WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
						
				)
																								
				SELECT FormBuilderSubmitId,FormBuilderId,FormCode,FormDescription,CreatedDate,PortalId,StoreName,UserId,UserName,FullName,RowId,CountNo
				FROM Cte_GetFilterFormBuilder
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
						
					
				INSERT INTO @TBL_FormBuilder(FormBuilderSubmitId,FormBuilderId,FormCode,FormDescription,CreatedDate,PortalId,StoreName,UserId,UserName,FullName,RowId,CountNo)
				EXEC(@SQL)

				SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_FormBuilder ),0)
			
				SELECT FormBuilderSubmitId,FormBuilderId,FormCode,FormDescription,CreatedDate,PortalId,StoreName,UserId,UserName,FullName
				FROM @TBL_FormBuilder


		
		 END TRY
		 BEGIN CATCH
			 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetFormBuilder @WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetFormBuilder',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END