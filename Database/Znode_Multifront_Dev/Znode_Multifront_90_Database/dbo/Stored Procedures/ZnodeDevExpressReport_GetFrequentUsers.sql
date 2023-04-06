CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetFrequentUsers]
(   @BeginDate   DATE         ,
	@EndDate     DATE         ,
	--@PortalId    VARCHAR(MAX) = '',
	--@UserId      VARCHAR(MAX) = '',
	--@FirstName   VARCHAR(MAX) = '',
	--@LastName    VARCHAR(MAX) = ''
	@WhereClause NVARCHAR(max) = ''
	)
AS 
/*
     Summary :- This Procedure is used to get frequently Appear users  
     Unit Testing 
     EXEC ZnodeReport_GetFrequentUsers 
*/
     BEGIN
         BEGIN TRY
        DECLARE @SQL NVARCHAR(MAX)

		 SET @SQL ='
             DECLARE @TBL_ReportOrderUser TABLE (OmsOrderId INT, UserId     INT, PortalId   INT);
		   DECLARE @TBL_ReportOrderDetails TABLE (UserId INT, BillingFirstName nvarchar(200),BillingLastName nvarchar	(200),
		                                           StoreName nvarchar(max), OrderCount INT, Total NVarchar(100),Symbol NVARCHAR(100),PortalId   INT );
            
			 INSERT INTO @TBL_ReportOrderUser (OmsOrderId, UserId,PortalId)
                    
				SELECT MAX(ZOOD.OmsOrderId) OmsOrderId, UserId, PortalId
                    FROM ZnodeOmsOrderDetails ZOOD
                    --WHERE((EXISTS
                    --      (
                    --          SELECT TOP 1 1
                    --          FROM dbo.split(@PortalId, '','') SP
                    --          WHERE CAST(ZOOD.PortalId AS VARCHAR(100)) = SP.Item
                    --                OR @PortalId = ''
                    --      )))
                    GROUP BY UserId,PortalId;


             WITH Cte_ReportUserDetails
                  AS (SELECT ZOOD.UserId, ZOOD.BillingFirstName,ZOOD.BillingLastName, ZOOD.PortalId, ZP.StoreName
                      FROM @TBL_ReportOrderUser O
                           INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = O.OmsOrderId)
                           INNER JOIN ZnodePortal ZP ON(Zp.PortalId = ZOOD.PortalId)
                      --WHERE((EXISTS
                      --      (
                      --          SELECT TOP 1 1
                      --          FROM dbo.split(@PortalId, '','') SP
                      --          WHERE CAST(ZOOD.PortalId AS VARCHAR(100)) = SP.Item
                      --                OR @PortalId = ''
                      --      )))
					  )

                  Insert into @TBL_ReportOrderDetails  (UserID , BillingFirstName ,BillingLastName , StoreName , OrderCount , Total,Symbol ,PortalId )
                  SELECT CTRO.UserId, CTRO.BillingFirstName, CTRO.BillingLastName, StoreName, COUNT(ZOOD.OmsOrderId) AS ''OrderCount'',
                         Dbo.Fn_GetDefaultPriceRoundOff(SUM(ZOOD.Total))AS ''Total'', ISNULL(ZC.Symbol,dbo.Fn_GetDefaultCurrencySymbol()) Symbol,CTRO.PortalId
                  FROM Cte_ReportUserDetails CTRO
                       INNER JOIN ZnodeOmsOrderDetails ZOOD ON(CTRO.UserId = ZOOD.UserId)
                       LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode)
                  WHERE CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE
                                                                 WHEN '''+CAST(@BeginDate AS VARCHAR(30))+''' IS NULL
                                                                 THEN CAST(ZOOD.OrderDate AS DATE)
                                                                 ELSE '''+CAST(@BeginDate AS VARCHAR(30))+'''
                                                             END AND 
												 CASE
                                                                         WHEN '''+CAST(@EndDate AS VARCHAR(30))+''' IS NULL
                                                                         THEN CAST(ZOOD.OrderDate AS DATE)
                                                                         ELSE '''+CAST(@EndDate AS VARCHAR(30))+'''
                                                             END
                      
                  GROUP BY CTRO.UserId,
   CTRO.BillingFirstName,
                           CTRO.BillingLastName,
                           StoreName,ZC.Symbol,CTRO.PortalId
						 
						 ;  WITH Cte_ReportUserDetailsFiltered As (					

					  Select UserID , BillingFirstName ,BillingLastName , StoreName , OrderCount , Total,Symbol,PortalId
					  from @TBL_ReportOrderDetails 
					  WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'

					  )
					  Select UserID , BillingFirstName ,BillingLastName , StoreName , OrderCount , Total,Symbol
					  from Cte_ReportUserDetailsFiltered 
					  ORder by CAST (Total AS Numeric) DESC;

					  '

					  PRINT @SQL
					  EXEC(@SQL)
                 
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetFrequentUsers @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetFrequentUsers',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;