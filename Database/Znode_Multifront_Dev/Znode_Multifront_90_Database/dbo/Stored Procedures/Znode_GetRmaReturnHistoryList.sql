CREATE PROCEDURE [dbo].[Znode_GetRmaReturnHistoryList]
(
	@WhereClause	VARCHAR(max),
    @Rows			INT           = 100,
    @PageNo			INT           = 1,
    @Order_By		VARCHAR(1000) = '',
	@IsAdmin        BIT=0,
	@RowCount		INT        = 0 OUT,
	@ReturnDate     VARCHAR(500) = '',
	@SalesRepUserId int = 0
)
AS
/* 
	EXEC Znode_GetRmaReturnHistoryList @WhereClause = 'ReturnNumber = ''ROMA-200630-183358-954''',@IsAdmin=0,@Order_By='ReturnDate asc',@ReturnDate='returndate between ''06/05/2020 03:49 am'' and ''07/19/2020 03:49 pm''' 
*/
BEGIN
SET NOCOUNT ON
BEGIN TRY
		
		DECLARE @SQL NVARCHAR(MAX)
		DECLARE @PaginationWhereClause VARCHAR(300)= dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' WHERE RowId')
		DECLARE @PriceRoundOff VARCHAR(10)
		SELECT @PriceRoundOff = FeatureValues FROM Znodeglobalsetting WHERE FeatureName = 'PriceRoundOff'
		CREATE TABLE #TBL_RowCount(RowsCount INT )
		----Verifying that the @SalesRepUserId is having 'Sales Rep' role
		IF NOT EXISTS
		(
			SELECT * FROM ZnodeUser ZU
			INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
			INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
			INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
			WHERE Exists(SELECT * FROM AspNetRoles ANR WHERE Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id) 
			AND ZU.UserId = @SalesRepUserId
		)   
		BEGIN
			SET @SalesRepUserId = 0
		End
		
		DECLARE @Order_BY1 VARCHAR(1000) = @Order_BY 
		--For enhancemet purpose getting order by clause column from exact table
		IF @Order_BY <> ''
		BEGIN
			SET @Order_BY = REPLACE(@Order_BY,'OrderNumber','ZRRD.OrderNumber')
			SET @Order_BY = REPLACE(@Order_BY,'RmaReturnDetailsId','ZRRD.RmaReturnDetailsId')
			SET @Order_BY = REPLACE(@Order_BY,'ReturnNumber','ZRRD.ReturnNumber')
			SET @Order_BY = REPLACE(@Order_BY,'ReturnStatus','ZRRS.ReturnStateName')
			SET @Order_BY = REPLACE(@Order_BY,'ReturnDate','ZRRD.ReturnDate')
			SET @Order_BY = REPLACE(@Order_BY,'CreatedDate','ZRRD.CreatedDate')
			SET @Order_BY = REPLACE(@Order_BY,'TotalExpectedReturnQuantity','ZRRD.TotalExpectedReturnQuantity')
			SET @Order_BY = REPLACE(@Order_BY,'EmailId','ZRRD.EmailId')
			SET @Order_BY = REPLACE(@Order_BY,'UserName','ISNULL(RTRIM(LTRIM(ZRRD.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZRRD.LastName)),'''')')
			SET @Order_BY = REPLACE(@Order_BY,'PortalId','ZRRD.PortalId')
			SET @Order_BY = REPLACE(@Order_BY,'UserID','ZRRD.UserID')
			SET @Order_BY = REPLACE(@Order_BY,'StoreName','ZP.StoreName')
			SET @Order_BY = REPLACE(@Order_BY,'CultureCode','ZRRD.CultureCode')
			SET @Order_BY = REPLACE(@Order_BY,'CurrencyCode','ZRRD.CurrencyCode')
		END
		--For enhancemet purpose getting where clause column from exact table
		IF @WhereClause <> ''
		BEGIN
			SET @WhereClause = REPLACE(@WhereClause,'OrderNumber','ZRRD.OrderNumber')
			SET @WhereClause = REPLACE(@WhereClause,'RmaReturnDetailsId','ZRRD.RmaReturnDetailsId')
			SET @WhereClause = REPLACE(@WhereClause,'ReturnNumber','ZRRD.ReturnNumber')
			SET @WhereClause = REPLACE(@WhereClause,'ReturnStatus','ZRRS.ReturnStateName')
			SET @WhereClause = REPLACE(@WhereClause,'ReturnDate','ZRRD.ReturnDate')
			SET @WhereClause = REPLACE(@WhereClause,'CreatedDate','ZRRD.CreatedDate')
			SET @WhereClause = REPLACE(@WhereClause,'TotalExpectedReturnQuantity','ZRRD.TotalExpectedReturnQuantity')
			SET @WhereClause = REPLACE(@WhereClause,'EmailId','ZRRD.EmailId')
			SET @WhereClause = REPLACE(@WhereClause,'UserName','ISNULL(RTRIM(LTRIM(ZRRD.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZRRD.LastName)),'''')')
			SET @WhereClause = REPLACE(@WhereClause,'PortalId','ZRRD.PortalId')
			SET @WhereClause = REPLACE(@WhereClause,'UserID','ZRRD.UserID')
			SET @WhereClause = REPLACE(@WhereClause,'StoreName','ZP.StoreName')
			SET @WhereClause = REPLACE(@WhereClause,'CultureCode','ZRRD.CultureCode')
			SET @WhereClause = REPLACE(@WhereClause,'CurrencyCode','ZRRD.CurrencyCode')
		END
		IF @ReturnDate <> ''
		BEGIN
			SET @ReturnDate = REPLACE(@ReturnDate,'ReturnDate','ZRRD.ReturnDate')
		END
		
		--To get the maximum rows to fetch
		SET @Rows = @PageNo * @Rows
		
		----Get all data for webstore
		IF @IsAdmin = 0
		BEGIN
			   CREATE TABLE #Cte_RetuenOrder_WhereClause
			   ( 
					RmaReturnDetailsId INT, 
					ReturnNumber NVARCHAR(200),  
					ReturnStatus NVARCHAR(200), 
					ReturnDate DATETIME,
					CreatedDate DATETIME, 
					TotalExpectedReturnQuantity NUMERIC(28,6), 
					UserName  VARCHAR(300), 
					EmailId  VARCHAR(300), 
					StoreName  VARCHAR(300), 
					TotalReturnAmount NUMERIC(28,6), 
					ModifiedDate DATETIME, 
					PortalId INT, 
					UserId INT,
					CurrencyCode VARCHAR(300), 
					CultureCode VARCHAR(300),
					OrderNumber NVARCHAR(200),
					SubTotal NUMERIC(28,6), 
					ReturnShippingCost NUMERIC(28,6), 
					ReturnTaxCost NUMERIC(28,6), 
					DiscountAmount NUMERIC(28,6), 
					CSRDiscount NUMERIC(28,6), 
					ReturnShippingDiscount NUMERIC(28,6), 
					ReturnCharges NUMERIC(28,6),
					RowId INT
				)
			   IF @Order_By = '''' 
			   BEGIN 
					SET @SQL =
					' SELECT COUNT(*)
					 FROM ZnodeRmaReturnDetails ZRRD 
					 INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
					 INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId
					 WHERE (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
					 AND '+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')
				
					INSERT INTO #TBL_RowCount
					EXEC (@SQL)

					SET @SQL =
					' SELECT TOP '+CAST(@Rows AS VARCHAR(10))+' ZRRD.RmaReturnDetailsId,ZRRD.ReturnNumber, ZRRS.ReturnStateName  as ReturnStatus,ZRRD.ReturnDate,ZRRD.CreatedDate, 
					 ZRRD.TotalExpectedReturnQuantity, ISNULL(ZRRD.FirstName,'''')+'' ''+ISNULL(ZRRD.LastName,'''') as UserName, 
					 ZRRD.EmailId as EmailId, ZP.StoreName, ROUND(ZRRD.TotalReturnAmount,'+@PriceRoundOff+') TotalReturnAmount, 
					 ZRRD.PortalId , ZRRD.UserID,ZRRD.ModifiedDate,ZRRD.CurrencyCode, ZRRD.CultureCode,ZRRD.OrderNumber ,
					 ISNULL(ZRRD.SubTotal,0) as SubTotal, ISNULL(ZRRD.ReturnShippingCost,0) ReturnShippingCost, 
					 ISNULL(ZRRD.ReturnTaxCost,0) as ReturnTaxCost, ISNULL(ZRRD.DiscountAmount,0) as DiscountAmount, 
					 ISNULL(ZRRD.CSRDiscount,0) as CSRDiscount, ISNULL(ZRRD.ReturnShippingDiscount,0) as ReturnShippingDiscount, 
					 ISNULL(ZRRD.ReturnCharges,0) as ReturnCharges,
					 Row_Number()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'ZRRD.CreatedDate DESC')+',ZRRD.ReturnDate DESC) RowId 
					 INTO #Cte_RetuenOrder 
					 FROM ZnodeRmaReturnDetails ZRRD 
					 INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
					 INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId
					 WHERE (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
					 AND ZRRD.ReturnDate IS NULL '+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')

					INSERT INTO #Cte_RetuenOrder_WhereClause 
					( 
						RmaReturnDetailsId , ReturnNumber ,ReturnStatus , ReturnDate ,CreatedDate , TotalExpectedReturnQuantity, 
						UserName, EmailId , StoreName, TotalReturnAmount , PortalId , UserId ,ModifiedDate , CurrencyCode, CultureCode,
						OrderNumber, SubTotal , ReturnShippingCost, ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount, 
						ReturnCharges,RowId 
					)
					EXEC (@SQL)
					
						IF ISNULL((SELECT MAX(RowId) FROM #Cte_RetuenOrder_WhereClause),0) <= @Rows
						BEGIN
							SET @SQL =	'
							SELECT TOP '+CAST(@Rows AS VARCHAR(10))+' ZRRD.RmaReturnDetailsId,ZRRD.ReturnNumber, ZRRS.ReturnStateName  as ReturnStatus,ZRRD.ReturnDate,ZRRD.CreatedDate, 
								ZRRD.TotalExpectedReturnQuantity, ISNULL(ZRRD.FirstName,'''')+'' ''+ISNULL(ZRRD.LastName,'''') as UserName, 
								ZRRD.EmailId as EmailId, ZP.StoreName, ROUND(ZRRD.TotalReturnAmount,'+@PriceRoundOff+') TotalReturnAmount, 
								ZRRD.PortalId , ZRRD.UserID,ZRRD.ModifiedDate,ZRRD.CurrencyCode, ZRRD.CultureCode, ZRRD.OrderNumber,
								ISNULL(ZRRD.SubTotal,0) as SubTotal, ISNULL(ZRRD.ReturnShippingCost,0) ReturnShippingCost, 
								ISNULL(ZRRD.ReturnTaxCost,0) as ReturnTaxCost, ISNULL(ZRRD.DiscountAmount,0) as DiscountAmount, 
								ISNULL(ZRRD.CSRDiscount,0) as CSRDiscount, ISNULL(ZRRD.ReturnShippingDiscount,0) as ReturnShippingDiscount, 
								ISNULL(ZRRD.ReturnCharges,0) as ReturnCharges,
								ISNULL((SELECT MAX(RowId) FROM #Cte_RetuenOrder_WhereClause),0)+ROW_NUMBER()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'ZRRD.ModifiedDate DESC')+',ZRRD.ReturnDate DESC) RowId 
							INTO #Cte_RetuenOrder 
							FROM ZnodeRmaReturnDetails ZRRD 
							INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
							INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId 
							WHERE (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
							AND ZRRD.ReturnDate IS NOT NULL'+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')
			
							INSERT INTO #Cte_RetuenOrder_WhereClause 
							( 
								RmaReturnDetailsId , ReturnNumber ,ReturnStatus , ReturnDate ,CreatedDate , TotalExpectedReturnQuantity, 
								UserName, EmailId , StoreName, TotalReturnAmount , PortalId , UserId ,ModifiedDate , CurrencyCode, CultureCode,
								OrderNumber, SubTotal , ReturnShippingCost, ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount, 
								ReturnCharges,RowId 
							)
							EXEC (@SQL)
						END
				 END 
				 ELSE 
				 BEGIN 

					SET @SQL =	' 
						SELECT COUNT(*)
						FROM ZnodeRmaReturnDetails ZRRD 
						INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
						INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId 
						WHERE (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
						'+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')
					INSERT INTO #TBL_RowCount
					EXEC (@SQL)

					SET @SQL =	' 
						SELECT TOP '+CAST(@Rows AS VARCHAR(10))+' ZRRD.RmaReturnDetailsId,ZRRD.ReturnNumber, ZRRS.ReturnStateName  as ReturnStatus,ZRRD.ReturnDate,ZRRD.CreatedDate, 
			 			ZRRD.TotalExpectedReturnQuantity, ISNULL(ZRRD.FirstName,'''')+'' ''+ISNULL(ZRRD.LastName,'''') as UserName, 
			 			ZRRD.EmailId as EmailId, ZP.StoreName, ROUND(ZRRD.TotalReturnAmount,'+@PriceRoundOff+') TotalReturnAmount, 
			 			ZRRD.PortalId , ZRRD.UserID,ZRRD.ModifiedDate,ZRRD.CurrencyCode, ZRRD.CultureCode, ZRRD.OrderNumber,
						ISNULL(ZRRD.SubTotal,0) as SubTotal, ISNULL(ZRRD.ReturnShippingCost,0) ReturnShippingCost, 
						ISNULL(ZRRD.ReturnTaxCost,0) as ReturnTaxCost, ISNULL(ZRRD.DiscountAmount,0) as DiscountAmount, 
						ISNULL(ZRRD.CSRDiscount,0) as CSRDiscount, ISNULL(ZRRD.ReturnShippingDiscount,0) as ReturnShippingDiscount, 
						ISNULL(ZRRD.ReturnCharges,0) as ReturnCharges,
			 			ROW_NUMBER()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'ZRRD.CreatedDate DESC')+',ReturnDate DESC) RowId 
						FROM ZnodeRmaReturnDetails ZRRD 
						INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
						INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId 
						WHERE (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
						'+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')  

					INSERT INTO #Cte_RetuenOrder_WhereClause 
					( 
						RmaReturnDetailsId , ReturnNumber ,ReturnStatus , ReturnDate ,CreatedDate , TotalExpectedReturnQuantity, 
						UserName, EmailId , StoreName, TotalReturnAmount , PortalId , UserId ,ModifiedDate , CurrencyCode, CultureCode,
						OrderNumber, SubTotal , ReturnShippingCost, ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount, 
						ReturnCharges,RowId 
					)
					EXEC (@SQL)

				 END 
			
				SET @SQL =	' 
				SELECT RmaReturnDetailsId, ReturnNumber,  ReturnStatus, ReturnDate, TotalExpectedReturnQuantity, UserName, EmailId, 
					   StoreName, TotalReturnAmount, ModifiedDate, CurrencyCode, CultureCode,OrderNumber, SubTotal , ReturnShippingCost, 
					   ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount, ReturnCharges,RowId 
				INTO #RetuenOrder 
				FROM #Cte_RetuenOrder_WhereClause 
				'+@PaginationWhereClause +' 
				 SELECT RmaReturnDetailsId, ReturnNumber,   dbo.Fn_CamelCase(ReturnStatus) as ReturnStatus, ReturnDate, TotalExpectedReturnQuantity, UserName, EmailId, StoreName, TotalReturnAmount, ModifiedDate, CurrencyCode, CultureCode  ,OrderNumber,
					SubTotal , ReturnShippingCost, ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount
				 FROM #RetuenOrder 
				 ORDER BY RowId'
		PRINT @SQL
		END
		ELSE
		BEGIN
		   ----Get all data for admin except Not submitted
			SET @SQL =
			'
			INSERT INTO #TBL_RowCount
			SELECT COUNT(*)
			FROM ZnodeRmaReturnDetails ZRRD
			INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
			INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId
			WHERE ISNULL(ZRRD.RmaReturnStateId,0) not in (SELECT ISNULL(RmaReturnStateId,0) FROM ZnodeRmaReturnState WHERE ReturnStateName = ''Not Submitted'')
		    and (exists(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
			'+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')+'

			SELECT TOP '+CAST(@Rows AS VARCHAR(10))+' ZRRD.RmaReturnDetailsId,ZRRD.ReturnNumber, ZRRS.ReturnStateName  as ReturnStatus,ZRRD.ReturnDate,
				ZRRD.TotalExpectedReturnQuantity, ISNULL(ZRRD.FirstName,'''')+'' ''+ISNULL(ZRRD.LastName,'''') as UserName,ZRRD.EmailId as EmailId, ZP.StoreName, round(ZRRD.TotalReturnAmount,'+@PriceRoundOff+') TotalReturnAmount,
				ZRRD.PortalId , ZRRD.UserID,ZRRD.ModifiedDate, ZRRD.CurrencyCode, ZRRD.CultureCode,ZRRD.OrderNumber,
				ISNULL(ZRRD.SubTotal,0) as SubTotal, ISNULL(ZRRD.ReturnShippingCost,0) ReturnShippingCost, 
				ISNULL(ZRRD.ReturnTaxCost,0) as ReturnTaxCost, ISNULL(ZRRD.DiscountAmount,0) as DiscountAmount, 
				ISNULL(ZRRD.CSRDiscount,0) as CSRDiscount, ISNULL(ZRRD.ReturnShippingDiscount,0) as ReturnShippingDiscount, 
				ISNULL(ZRRD.ReturnCharges,0) as ReturnCharges,
				Row_Number()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'ZRRD.ReturnDate DESC')+',ZRRD.ModifiedDate DESC) RowId
			INTO #Cte_RetuenOrder
			FROM ZnodeRmaReturnDetails ZRRD
			INNER JOIN ZnodePortal ZP ON ZRRD.PortalId = ZP.PortalId 
			INNER JOIN ZnodeRmaReturnState ZRRS on ZRRD.RmaReturnStateId = ZRRS.RmaReturnStateId
			WHERE ISNULL(ZRRD.RmaReturnStateId,0) NOT IN (SELECT ISNULL(RmaReturnStateId,0) FROM ZnodeRmaReturnState WHERE ReturnStateName = ''Not Submitted'')
		    AND (EXISTS(SELECT * FROM ZnodeSalesRepCustomerUserPortal SalRep WHERE SalRep.SalesRepUserId = '+cast(@SalesRepUserId as VARCHAR(10))+' and ZRRD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as VARCHAR(10))+' = 0) 
			'+dbo.Fn_GetWhereClause(@WhereClause, ' AND ') +dbo.Fn_GetWhereClause(@ReturnDate, ' AND ')+'

			SELECT RmaReturnDetailsId, ReturnNumber,  ReturnStatus, ReturnDate, TotalExpectedReturnQuantity, UserName, EmailId, StoreName, TotalReturnAmount, ModifiedDate, CurrencyCode, CultureCode,OrderNumber, 
				SubTotal , ReturnShippingCost, ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount,RowId,ReturnCharges
			INTO #RetuenOrder
			FROM #Cte_RetuenOrder
			'+@PaginationWhereClause+' '+dbo.Fn_GetOrderByClause(@Order_BY1, 'ReturnDate DESC' )
			+' 
			SELECT RmaReturnDetailsId, ReturnNumber,  dbo.Fn_CamelCase(ReturnStatus) as ReturnStatus, ReturnDate, TotalExpectedReturnQuantity, UserName, EmailId, StoreName, TotalReturnAmount, ModifiedDate, CurrencyCode, CultureCode,OrderNumber,
				SubTotal , ReturnShippingCost, ReturnTaxCost, DiscountAmount, CSRDiscount, ReturnShippingDiscount, ReturnCharges
			FROM #RetuenOrder
			ORDER BY RowId';
		END

		EXEC SP_executesql @SQL;
		SELECT @RowCount= ISNULL(RowsCount ,0) FROM #TBL_RowCount
END TRY
BEGIN CATCH
           
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetRmaReturnHistoryList @WhereClause = '+CAST(@WhereClause AS VARCHAR(10))+',@Order_By='+cast(@Order_By as VARCHAR(10));

	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetRmaReturnHistoryList',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH
END