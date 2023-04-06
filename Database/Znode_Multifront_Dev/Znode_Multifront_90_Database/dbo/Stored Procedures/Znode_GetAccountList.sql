CREATE PROCEDURE [dbo].[Znode_GetAccountList]
(
	@WhereClause varchar(max),
	@PageSize int = 50
)
/*
	exec Znode_GetAccountList @WhereClause = 'PortalId in (1,2)',@PageSize = 100
	exec Znode_GetAccountList @WhereClause = 'Name like ''%user%''',@PageSize = 100
*/
AS
BEGIN
	BEGIN TRY
	SET NOCOUNT ON;
		
		DECLARE @SQL varchar(max)
		set @SQL='SELECT top '+cast(@PageSize as varchar(10))+' ZPA.AccountId, ZA.Name, ZA.AccountCode
			FROM ZnodePortalAccount ZPA
			INNER JOIN ZnodeAccount ZA ON ZPA.AccountId = ZA.AccountId
			'+dbo.Fn_GetWhereClause(@WhereClause, ' WHERE ')+'
			order by ZA.Name '
			print @SQL
		EXEC (@SQL)

	END TRY
	BEGIN CATCH

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_TP_BestSellerPriceListBySKU @PimAttributeIds = '+@WhereClause;
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetAccountList',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END