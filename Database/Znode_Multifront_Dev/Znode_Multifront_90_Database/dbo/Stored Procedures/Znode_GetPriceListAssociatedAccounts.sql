Create PROCEDURE [dbo].[Znode_GetPriceListAssociatedAccounts]
(   @WhereClause VARCHAR(1000),
    @Rows        INT           = 10,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT)
AS 
  /* 
    Summary:  List of accounts associated and not associated to the price list     		   	             
    Unit Testing      			
     DECLARE @RER INT =2 
	 EXEC Znode_GetPriceListAssociatedAccounts @WhereClause = '',@RowsCount= @RER OUT 
 */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
            
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_AccountList TABLE (PortalId INT,StoreName Nvarchar(max),  AccountId INT ,AccountName NVARCHAR(1000),Precedence INT ,PriceListId int,PriceListAccountId INT,ExternalId NVARCHAR(200) ,ParentAccountName NVARCHAR(400),IsAssociated BIT ,RowId INT,CountNo INT, AccountCode nvarchar(100))
					
             SET @SQL = '
					 ;with Cte_Account AS 
					 (
						 SELECT ZP.PortalId, Zp.StoreName, ZA.AccountId, ZA.Name AccountName ,ZPLA.Precedence ,ZPL.PriceListId,PriceListAccountId,ZA.ExternalId,ZAP.Name ParentAccountName ,Case WHEN ZPLA.AccountId IS NULL THEN 0 ELSE 1 END IsAssociated, ZA.AccountCode
						 FROM ZnodeAccount  ZA 
						 CROSS APPLY ZnodePriceList ZPL  
						 LEFT JOIN ZnodeAccount ZAP ON (ZAP.Accountid = ZA.ParentAccountId )
						 INNER JOIN ZnodePortalAccount ZPA ON ZPA.AccountId = ZA.AccountId
						 INNER JOIN ZnodePortal ZP on ZPA.PortalId = ZP.PortalId 
						 LEFT JOIN ZnodePriceListAccount ZPLA ON (ZPLA.AccountId = ZA.AccountId AND ZPL.PriceListId = ZPLA.PriceListId ))

					 , Cte_AccountListRowId AS
						 (SELECT *, '+dbo.Fn_GetPagingRowId(@Order_BY,'AccountId DESC')+',Count(*)Over() CountNo
						 FROM Cte_Account WHERE 1=1
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+')

					 SELECT PortalId,StoreName,AccountId,AccountName,Precedence,PriceListId,PriceListAccountId,ExternalId,ParentAccountName,IsAssociated,RowId,CountNo , AccountCode
					 FROM Cte_AccountListRowId
					 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
				
			 INSERT INTO @TBL_AccountList   
			 EXEC (@SQL)

             SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_AccountList),0)

			 SELECT PortalId,StoreName,AccountId,AccountName,Precedence,PriceListId,PriceListAccountId,ExternalId,ParentAccountName, AccountCode
			 FROM @TBL_AccountList
				 
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPriceListAssociatedAccounts @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='
			+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPriceListAssociatedAccounts',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;