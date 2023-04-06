CREATE PROCEDURE [dbo].[Znode_GetAccountAccessPermission]
(   @WhereClause NVarchar(Max) = '',
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY VARCHAR(1000)    = '',
	@RowsCount   INT OUT,
	@LocaleId    INT           = 0)
AS
/*
 Summary: This procedure is used to get access permission of an account
          All account information is fetched from view View_GetAccountAccessPermission
 Unit Testing:
 EXEC Znode_GetAccountAccessPermission @RowsCount = 0
 
*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             
			 DECLARE @TBL_AccountPermission TABLE (AccountPermissionId INT,AccountId INT,AccountPermissionName VARCHAR(300),PermissionsName VARCHAR(200),AccountPermissionAccessId INT, PermissionCode VARCHAR(200),AccessPermissionId INT, RowId INT,CountNo INT)
             DECLARE @SQL NVARCHAR(MAX);
			
			 SET @SQL = '
					    ;WITH CTE_AccountAccessPermission  AS
						(
						SELECT AccountPermissionId,isnull(AccountId,0) AccountId,AccountPermissionName,PermissionsName,AccountPermissionAccessId,PermissionCode,AccessPermissionId
								,'+dbo.Fn_GetPagingRowId(@Order_BY,'AccountPermissionId DESC')+',Count(*)Over() CountNo
						FROM  View_GetAccountAccessPermission
						WHERE 1=1
								'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
							   
						)   
						SELECT AccountPermissionId,AccountId,AccountPermissionName,PermissionsName,AccountPermissionAccessId,PermissionCode,AccessPermissionId,RowId,CountNo
						FROM CTE_AccountAccessPermission
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

						INSERT INTO @TBL_AccountPermission (AccountPermissionId,AccountId,AccountPermissionName,PermissionsName,AccountPermissionAccessId,PermissionCode,AccessPermissionId,RowId,CountNo)
						EXEC(@SQL)

						SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AccountPermission),0)
   
						SELECT AccountPermissionId,AccountId,AccountPermissionName,PermissionsName,AccountPermissionAccessId,PermissionCode,AccessPermissionId
						FROM @TBL_AccountPermission

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAccountAccessPermission @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAccountAccessPermission',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;