CREATE PROCEDURE [dbo].[Znode_GetOmsOrderDetail]
( 
	@WhereClause NVARCHAR(MAX),
	@Rows        INT            = 100,
	@PageNo      INT            = 1,
	@Order_BY    VARCHAR(1000)  = '',
	@RowsCount   INT OUT			,
	@UserId	   INT = 0 ,
	@IsFromAdmin int=0,
	@SalesRepUserId int = 0 
 )
AS
    /*
     Summary : This procedure is used to get the oms order detils
			   Records are fetched for those users who placed the order i.e UserId is Present in ZnodeUser and  ZnodeOmsOrderDetails tables
	 Unit Testing:

    EXEC [Znode_GetOmsOrderDetail_SCT] 'PortalId =1',@Order_BY = '',@RowsCount= 0, @UserId = 0 ,@Rows = 50, @PageNo = 1

	declare @p7 int
	set @p7=4
	exec sp_executesql N'Znode_GetOmsOrderDetail_SCT @WhereClause, @Rows,@PageNo,@Order_By,@RowCount OUT,@UserId,@IsFromAdmin',N'@WhereClause nvarchar(30),@Rows int,@PageNo int,@Order_By nvarchar(14),@RowCount int output,@UserId int,@IsFromAdmin int',@WhereClause=N'(PortalId in(''1'',''4'',''5'',''6''))',@Rows=50,@PageNo=1,@Order_By=N'orderdate desc',@RowCount=@p7 output,@UserId=0,@IsFromAdmin=1
	select @p7



   */
BEGIN
    BEGIN TRY
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
		DECLARE @SQL NVARCHAR(MAX), @ProcessType  varchar(50)='Order'
		DECLARE @OrderLineItemRelationshipTypeId INT
		SET @OrderLineItemRelationshipTypeId = ( SELECT top 1 OrderLineItemRelationshipTypeId  FROM ZnodeOmsOrderLineItemRelationshipType where Name = 'AddOns' )

		----Verifying that the @SalesRepUserId is having 'Sales Rep' role
		IF NOT EXISTS
		(
			SELECT * FROM ZnodeUser ZU
			INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
			INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
			INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
			Where Exists(select * from AspNetRoles ANR Where Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id) 
			AND ZU.UserId = @SalesRepUserId
		)   
		Begin
			SET @SalesRepUserId = 0
		End

		DECLARE @Fn_GetPaginationWhereClause VARCHAR(500) = dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows),
		@Fn_GetFilterWhereClause NVARCHAR(MAX) = ''
		set @Fn_GetFilterWhereClause=dbo.Fn_GetFilterWhereClause(@WhereClause)
			
		IF @Order_BY = ''
			set @Order_BY = 'OrderDate desc'

			set @Order_BY = replace(@Order_BY,'PortalId','ZODD.PortalId')
			set @Order_BY = replace(@Order_BY,'UserName','ISNULL(RTRIM(LTRIM(ZODD.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZODD.LastName)),'''')')
			set @Order_BY = replace(@Order_BY,'email','ZODD.Email')
			set @Order_BY = replace(@Order_BY,'OrderState','case when ZOS.IsShowToCustomer=0 and '+cast( @IsFromAdmin as varchar(50))+' = 0 then ZOSC.Description else  ZOS.Description end')
			set @Order_BY = replace(@Order_BY,'PaymentStatus','ZOPS.Name')
			set @Order_BY = replace(@Order_BY,'PublishState','ZODPS.DisplayName')
			set @Order_BY = replace(@Order_BY,'StoreName','ZP.StoreName')

			Declare @Fn_GetPagingRowId NVARCHAR(MAX) = ' DENSE_RANK()Over('+ ' Order By '+CASE WHEN Isnull(@Order_BY,'') = '' THEN 'OmsOrderId DESC' ELSE @Order_BY + ',OmsOrderId DESC' END  + ') RowId '
						
			IF OBJECT_ID('tempdb..#TBL_RowCount') is not null
				DROP TABLE #TBL_RowCount

			IF OBJECT_ID('tempdb..#Portal') is not null
				DROP TABLE #Portal
			
			Create table #TBL_RowCount(RowsCount int )
			CREATE TABLE #tbl_GetRecurciveUserId  (ID INT IDENTITY(1,1) Primary key,UserId INT,ParentUserId INT)
			INSERT INTO #tbl_GetRecurciveUserId
			SELECT UserId,ParentUserId FROM dbo.Fn_GetRecurciveUserId (CAST(@UserId AS VARCHAR(50)),@ProcessType ) FNRU
			 
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'PortalId','ZODD.PortalId')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'UserName','ISNULL(RTRIM(LTRIM(ZODD.FirstName)),'''')+'' ''+ISNULL(RTRIM(LTRIM(ZODD.LastName)),'''')')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'email','ZODD.Email')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'OrderState','case when ZOS.IsShowToCustomer=0 and '+cast( @IsFromAdmin as varchar(50))+' = 0 then ZOSC.Description else  ZOS.Description end')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'PaymentStatus','ZOPS.Name')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'PublishState','ZODPS.DisplayName')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'StoreName','ZP.StoreName')
			set @Fn_GetFilterWhereClause = replace(@Fn_GetFilterWhereClause,'orderdate','CAST((FORMAT(orderdate,''yyyy-MM-dd HH:mm'')) as datetime)')			
		
	
		set @Fn_GetPagingRowId = replace(@Fn_GetPagingRowId,'OmsOrderId','Zoo.OmsOrderId')
		
		
		set @Rows = @PageNo * @Rows

		CREATE TABLE #Portal (PortalId int,StoreName varchar(200))
		insert into #Portal
		select PortalId,StoreName
		from ZnodePortal

		SET @SQL = '
		SELECT Distinct top '+cast(@Rows as varchar(10))+' Zoo.OmsOrderId,Zoo.OrderNumber, ZODD.PortalId,ZP.StoreName ,ZODD.CurrencyCode,
		case when ZOS.IsShowToCustomer=0 and '+cast( @IsFromAdmin as varchar(50))+' = 0 then ZOSC.Description else  ZOS.Description end  OrderState,ZODD.ShippingId,ZODD.PaymentTypeId,ZODD.PaymentSettingId
		,ZOPS.Name PaymentStatus,ZPS.Name PaymentType,CAST(1 AS BIT) ShippingStatus ,ZODD.OrderDate,ZODD.UserId,ISNULL(RTRIM(LTRIM(ZODD.FirstName)),'''')
		+'' ''+ISNULL(RTRIM(LTRIM(ZODD.LastName)),'''') UserName ,ZODD.PaymentTransactionToken ,ZODD.OrderTotalWithoutVoucher as Total,ZODD.OmsOrderDetailsId,ZODD.PoDocument,
		ZODD.Email ,ZODD.PhoneNumber ,ZODD.SubTotal ,ZODD.TaxCost ,ZODD.ShippingCost,ZODD.BillingPostalCode,ZODD.RemainingOrderAmount,
		ZODD.ModifiedDate AS OrderModifiedDate,  ZODD.PaymentDisplayName  ,isnull(Zoo.ExternalId,0) ExternalId,ZODD.CreditCardExpMonth,ZODD.CultureCode--,ZODD.TotalAdditionalCost
		,ZODD.CreditCardExpYear,ZODD.CardType,ZODD.CreditCardNumber,ZODD.PaymentExternalId,ZODPS.DisplayName as PublishState,
		'''' ProductName, 0 CountId, CAST (0 as bit) IsInRMA, '+@Fn_GetPagingRowId+'
		INTO #Cte_OrderLineDescribe
		FROM ZnodeOmsOrder (nolock) ZOO 
		INNER JOIN ZnodeOmsOrderDetails (nolock) ZODD ON (ZODD.OmsOrderId = ZOO.OmsOrderId AND  ZODD.IsActive = 1)
		INNER JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOO.PublishStateId)
		INNER JOIN #Portal ZP (nolock) ON ZODD.PortalId = ZP.PortalId
		LEFT JOIN ZnodePaymentType (nolock) ZPS ON (ZPS.PaymentTypeId = ZODD.PaymentTypeId )
		LEFT JOIN ZnodeOmsOrderStateShowToCustomer (nolock) ZOSC ON (ZOSC.OmsOrderStateId = ZODD.OmsOrderStateId)
		LEFT JOIN ZnodeOmsOrderState (nolock) ZOS ON (ZOS.OmsOrderStateId = ZODD.OmsOrderStateId)
		LEFT JOIN ZnodeOmsPaymentState (nolock) ZOPS ON (ZOPS.OmsPaymentStateId = ZODD.OmsPaymentStateId)
		WHERE (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZODD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0 )  
		AND (EXISTS (SELECT TOP 1 1 FROM #tbl_GetRecurciveUserId FNRU WHERE FNRU.UserId = ZODD.UserId ) OR '+cast(@UserId as varchar(10))+'  =0 )'
		+ @Fn_GetFilterWhereClause+' 
		ORDER BY '+CASE WHEN Isnull(@Order_BY,'') = '' THEN 'OmsOrderId DESC' ELSE @Order_BY + ',OmsOrderId DESC' END +' 

		INSERT INTO #TBL_RowCount 
		SELECT count(*)
		FROM ZnodeOmsOrder (nolock) ZOO 
		INNER JOIN ZnodeOmsOrderDetails (nolock) ZODD ON (ZODD.OmsOrderId = ZOO.OmsOrderId AND  ZODD.IsActive = 1)
		INNER JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOO.PublishStateId)
		INNER JOIN #Portal ZP (nolock) ON ZODD.PortalId = ZP.PortalId
		LEFT JOIN ZnodePaymentType (nolock) ZPS ON (ZPS.PaymentTypeId = ZODD.PaymentTypeId )
		LEFT JOIN ZnodeOmsOrderStateShowToCustomer (nolock) ZOSC ON (ZOSC.OmsOrderStateId = ZODD.OmsOrderStateId)
		LEFT JOIN ZnodeOmsOrderState (nolock) ZOS ON (ZOS.OmsOrderStateId = ZODD.OmsOrderStateId)
		LEFT JOIN ZnodeOmsPaymentState (nolock) ZOPS ON (ZOPS.OmsPaymentStateId = ZODD.OmsPaymentStateId)
		WHERE (EXISTS(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and ZODD.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0 )  
		AND (EXISTS (SELECT TOP 1 1 FROM #tbl_GetRecurciveUserId FNRU WHERE FNRU.UserId = ZODD.UserId ) OR '+cast(@UserId as varchar(10))+'  =0 )'
		+ @Fn_GetFilterWhereClause+' 
			
		Create index Ind_OrderLineDescribe_RowId on #Cte_OrderLineDescribe(RowId )

		SELECT OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,
		PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId,UserName,PaymentTransactionToken,Total,
		PN OrderItem,a.OmsOrderDetailsId,CountId ItemCount, PoDocument AS PODocumentPath,IsInRMA ,
		Email,PhoneNumber,SubTotal,TaxCost,ShippingCost,BillingPostalCode,null ShippingPostalCode
		,OrderModifiedDate,PaymentDisplayName, a.RemainingOrderAmount,
		ExternalId,CreditCardExpMonth,CreditCardExpYear,CardType,CreditCardNumber,PaymentExternalId,CultureCode,PublishState --TotalAdditionalCost
		FROM #Cte_OrderLineDescribe a inner join (select OmsOrderDetailsId, min(ProductName) PN, sum(case when OmsOrderLineItemsId is null then 0 else 1 end ) CNT from ZnodeOmsOrderLineItems
		where ParentOmsOrderLineItemsId is not null group by OmsOrderDetailsId ) b on a.OmsOrderDetailsId = b.OmsOrderDetailsId
		' + @Fn_GetPaginationWhereClause +' order by RowId '

		--print @SQL
		EXEC(@SQL)
		Select @RowsCount= isnull(RowsCount  ,0) from #TBL_RowCount
		
		IF OBJECT_ID('tempdb..#TBL_RowCount') is not null
				DROP TABLE #TBL_RowCount

		IF OBJECT_ID('tempdb..#Portal') is not null
			DROP TABLE #Portal
		
    END TRY
    BEGIN CATCH
        DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetOmsOrderDetail @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(max)),'''''')+''',@Rows='''+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+''',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
		@Order_BY='+ISNULL(@Order_BY,'''''')+',@UserId = '+ISNULL(CAST(@UserId AS VARCHAR(50)),'''')+',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',@IsFromAdmin='+ISNULL(CAST(@IsFromAdmin AS VARCHAR(10)),'''');
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    

        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetOmsOrderDetail',
		@ErrorInProcedure = 'Znode_GetOmsOrderDetail',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
    END CATCH;
END;

	 
	 