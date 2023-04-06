CREATE PROCEDURE [dbo].[Znode_GetAccountListWithAddress]
(   @WhereClause VARCHAR(1000),
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY    VARCHAR(100)  = '',
	@RowsCount   INT OUT,
	@LocaleId    INT           = 0
)
AS
    
/*
     Summary : This procedure is used to find the Account and related address list 
			   1. ZNodePortalAddress          
			   2. ZnodeAddress	
    Unit Testing
	begin tran	 
    Declare @Status int 
    Exec [Znode_GetAccountListWithAddress] @WhereClause = ' name = ''suchita acc'' ' ,@Rows = 10 ,@PageNo = 1 , @Order_BY = ' AccountId DESC ',@RowsCount = 1   
    rollback tran
    Select @Status
    select * from ZNodePortalAddress where PortalAddressId = 8 
    select * from ZNodeAddress where AddressId in (select AddressId from ZNodePortalAddress where PortalAddressId = 8 )
    addressid : 57
*/
  


     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY

			------Commented query due to slowness
			--SELECT  a.AccountId, a.ExternalId, a.Name,a.ParentAccountId, b.Name AS ParentAccountName ,ZPA.PortalId,ZP.StoreName, PC.CatalogName,
			--(select top 1 PostalCode from ZnodeAddress where AddressId in  (select AddressId from ZnodeAccountAddress where AccountId = a.AccountId AND IsDefaultShipping = 1)) AS ShippingPostalCode,
			--(select top 1 PostalCode from ZnodeAddress where AddressId in  (select AddressId from ZnodeAccountAddress where AccountId = a.AccountId AND IsDefaultBilling = 1)) AS BillingPostalCode
			--into #AccountListAis
			--FROM dbo.ZnodeAccount AS a 
			--LEFT OUTER JOIN dbo.ZnodeAccount AS b ON a.ParentAccountId = b.AccountId
			--LEFT JOIN ZnodePortalAccount ZPA  ON (ZPA.AccountId = a.AccountId)
			--LEFT JOIN ZnodePortal ZP ON (ZP.PortalId = ZPA.PortalId)
			--LEFT JOIN ZnodePortalCatalog ZPC ON ( ZPA.PortalId = ZPC.PortalId )
			--LEFT JOIN ZnodePublishcatalog PC ON ( PC.PublishcatalogId = COALESCE(a.PublishCatalogId, ZPC.PublishcatalogId ) )

			----Re-write above query for optimization
			;with cte_Account as
			(
				SELECT  a.AccountId, a.ExternalId, a.Name,a.ParentAccountId, b.Name AS ParentAccountName ,ZPA.PortalId,ZP.StoreName, PC.CatalogName, a.AccountCode
				FROM dbo.ZnodeAccount AS a 
				LEFT JOIN dbo.ZnodeAccount AS b ON a.ParentAccountId = b.AccountId
				LEFT JOIN ZnodePortalAccount ZPA  ON (ZPA.AccountId = a.AccountId)
				LEFT JOIN ZnodePortal ZP ON (ZP.PortalId = ZPA.PortalId)
				LEFT JOIN ZnodePortalCatalog ZPC ON ( ZPA.PortalId = ZPC.PortalId )
				LEFT JOIN ZnodePublishcatalog PC ON ( PC.PublishcatalogId = COALESCE(a.PublishCatalogId, ZPC.PublishcatalogId ) )
			)
			,cte_ShippingPostalCode as
			(
				select zaa.AccountId, min(PostalCode) as ShippingPostalCode 
				from ZnodeAccountAddress zaa 
				inner join ZnodeAddress za on za.AddressId = zaa.AddressId AND IsDefaultShipping = 1	
				where exists(select * from cte_Account a where zaa.AccountId = a.AccountId )
				and isnull(PostalCode,'0') <> '0'
				group by zaa.AccountId 	
			)
			,cte_BillingPostalCode as
			(
				select zaa.AccountId, min(PostalCode) as BillingPostalCode 
				from ZnodeAccountAddress zaa 
				inner join ZnodeAddress za on za.AddressId = zaa.AddressId AND IsDefaultBilling = 1	
				where exists(select * from cte_Account a where zaa.AccountId = a.AccountId )	
				and isnull(PostalCode,'0') <> '0'
				group by zaa.AccountId
			)
			select a.*, b.ShippingPostalCode, b1.BillingPostalCode
			into #AccountListAis
			from cte_Account a
			left join cte_ShippingPostalCode b on a.AccountId = b.AccountId
			left join cte_BillingPostalCode b1 on a.AccountId = b1.AccountId

             DECLARE @SQL NVARCHAR(MAX), @Rows_start VARCHAR(1000), @Rows_end VARCHAR(1000);
             SET @Rows_start = CASE WHEN @Rows >= 1000000 THEN 0 ELSE(@Rows * (@PageNo - 1)) + 1 END;
             SET @Rows_end = CASE WHEN @Rows >= 1000000THEN @Rows ELSE @Rows * (@PageNo) END;
             SET @SQL = '
			 CREATE TABLE #TBL_AddressDetails (AccountId INT ,Address NVARCHAR(max),IsDefaultBilling BIT ,IsDefaultShipping BIT,RowId INT  )
			 CREATE TABLE #TBL_AddressDetailsFinal (AccountId INT ,Address NVARCHAR(max))
			 
			 SELECT *,RANK()OVER(ORDER BY '+CASE
                                                    WHEN @Order_BY = ''
                                                    THEN ' AccountId ,'
                                                    ELSE @Order_BY+' , '
                                                END+' AccountId ) RowId 
			 into #TBL_AccountsDetails
			 FROM #AccountListAis
			   '+CASE
                        WHEN @WhereClause IS NOT NULL
                             AND @WhereClause <> ''
                        THEN ' WHERE '+@WhereClause
                        ELSE ''
                    END+'
			   '+CASE
                        WHEN @Order_BY = ''
                        THEN ''
                        ELSE ' ORDER BY '+@Order_BY
                    END+'
			    
			 SELECT @COUNT= COUNT(1) FROM #TBL_AccountsDetails

			 INSERT INTO #TBL_AddressDetails (AccountId,Address,IsDefaultBilling,IsDefaultShipping,RowId)
			 SELECT c.AccountId , CASE WHEN D.FirstName IS NULL THEN '''' ELSE D.FirstName END + CASE WHEN D.LastName IS NULL  THEN '''' ELSE '' ''+D.LastName END  
			                    + CASE WHEN D.Address1 IS NULL  THEN  ''''  ELSE '', '' + D.Address1 END 	
								+ CASE WHEN D.Address2 IS NULL THEN ''''  ELSE '', '' + D.Address2 END 
								+ CASE WHEN D.Address3 IS NULL THEN '''' ELSE  '', '' + D.Address3 END 
								+ CASE WHEN D.CityName IS NULL THEN  ''''  ELSE  '', '' + D.CityName  END 
								+ CASE WHEN D.StateName IS NULL THEN ''''  ELSE  '', '' + D.StateName  END 
								+ CASE WHEN D.PostalCode IS NULL THEN  '''' ELSE '', '' + D.PostalCode  END 
								+ CASE WHEN D.CountryName IS NULL THEN '''' ELSE  '', '' + D.CountryName END  									
								+ CASE WHEN D.PhoneNumber IS NULL THEN ''''  ELSE '', PH NO. ''+  D.PhoneNumber END  AS AccountAddress ,ISNULL(d.IsDefaultBilling,0) IsDefaultBilling ,ISNULL(d.IsDefaultShipping,0)IsDefaultShipping
						,ROW_NUMBER()OVER(PARTITION BY c.AccountId ORDER BY  c.AddressId) RowId
			 FROM dbo.ZnodeAccountAddress AS c 
			 LEFT JOIN dbo.ZnodeAddress AS D ON D.AddressId = c.AddressId
			 WHERE EXISTS ( SELECT TOP 1 1 FROM  #TBL_AccountsDetails a  WHERE a.AccountId = c.AccountId AND a.RowId BETWEEN '+@Rows_start+' AND '+@Rows_end+')  
			    
			 ;With AccountAddressShipping AS 
			 (
			 SELECT * FROM #TBL_AddressDetails mn WHERE IsDefaultShipping = 1 
			 )
			 ,  AccountAddressBilling AS 
			 (
				 SELECT * 
				 FROM AccountAddressShipping 
				 UNION ALL 
				 SELECT * 
				 FROM #TBL_AddressDetails mn 
				 WHERE IsDefaultBilling = 1 
				 AND NOT EXISTS (SELECT TOP 1 1 FROM AccountAddressShipping sw WHERE sw.AccountId = mn.AccountId )
			 )


			 INSERT INTO #TBL_AddressDetailsFinal 

			 SELECT AccountId ,Address 
			 FROM AccountAddressBilling 

			    
			 INSERT INTO #TBL_AddressDetailsFinal 
			 SELECT AccountId , Address 
			 FROM #TBL_AddressDetails  q
			 WHERE NOT EXISTS (SELECT  TOP 1 1 FROM #TBL_AddressDetailsFinal  fg WHERE fg.AccountId = q.AccountId )
			 AND RowId = 1 



			 SELECT a.AccountId, a.ExternalId, a.Name,a.ParentAccountId, a.ParentAccountName ,b.[Address] AccountAddress,a.PortalId,a.StoreName, a.CatalogName,ShippingPostalCode,BillingPostalCode, a.AccountCode
			 FROM #TBL_AccountsDetails a 
			 INNER JOIN #TBL_AddressDetailsFinal  b ON (a.AccountId = b.AccountId )
			 WHERE a.RowId BETWEEN '+@Rows_start+' AND '+@Rows_end+'  
			   '+CASE
                        WHEN @Order_BY = ''
                        THEN ''
                        ELSE ' ORDER BY '+@Order_BY
                 END;
           
		   PRINT(@SQL);
             EXEC SP_executesql
                  @SQL,
                  N'@Count INT OUT',
                  @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAccountListWithAddress @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAccountListWithAddress',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
