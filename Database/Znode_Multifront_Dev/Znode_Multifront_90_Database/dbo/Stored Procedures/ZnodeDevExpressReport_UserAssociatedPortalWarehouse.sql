CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_UserAssociatedPortalWarehouse] 
(
  --@PortalId		VARCHAR(max)  = '',
  @WhereClause NVARCHAR(max) = ''
)
AS 
/*
	 Summary :- This Procedure is used to get the login user id Portal Warehouse 
	 Unit Testing 
	 EXEC ZnodeReport_UserAssociatedPortalWarehouse 2

*/
 BEGIN 
  BEGIN TRY 
  SET NOCOUNT ON

  	DECLARE @SQL NVARCHAR(MAX)

		 SET @SQL ='

		 ;WITH CTE_GetUserAssociatedPortalWarehouse AS
		(   

	SELECT  ZW.WarehouseId,ZW.WarehouseName ,ZPW.PortalId
	FROM  ZnodeWarehouse ZW 
	INNER JOIN ZnodePortalWarehouse ZPW ON ( ZW.WarehouseId = ZPW.WarehouseId ) 
	LEFT JOIN ZnodePortalAlternateWarehouse ZPAW ON (ZPAW.PortalWarehouseId = ZPW.PortalWarehouseId ) 
	--WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(@PortalId,'','') TBPS WHERE (TBPS.Item = ZPW.PortalId OR @PortalId = '''' ))
	GROUP BY ZW.WarehouseId,ZW.WarehouseName ,ZPW.PortalId
	--ORDER BY ZW.WarehouseName
	)
	,CTE_GetFilteredUserAssociated AS
			(
			SELECT WarehouseId,WarehouseName FROM CTE_GetUserAssociatedPortalWarehouse
			WHERE 1=1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
			)

			SELECT WarehouseId,WarehouseName FROM  CTE_GetFilteredUserAssociated
			ORDER BY WarehouseName

	'

	    PRINT @SQL
		EXEC(@SQL)


  END TRY 
  BEGIN CATCH 
  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_UserAssociatedPortalWarehouse @Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_UserAssociatedPortalWarehouse',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
  END CATCH 
 END