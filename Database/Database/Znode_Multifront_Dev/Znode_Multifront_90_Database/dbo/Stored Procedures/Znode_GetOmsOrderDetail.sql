 

CREATE PROCEDURE [dbo].[Znode_GetOmsOrderDetail]
( @WhereClause NVARCHAR(MAx),
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT			,
  @UserId	   INT = 0 ,
  @IsFromAdmin int=0
  )
AS
    /*
     Summary : This procedure is used to get the oms order detils
			   Records are fetched for those users who placed the order i.e UserId is Present in ZnodeUser and  ZnodeOmsOrderDetails tables
	 Unit Testing:

     EXEC Znode_GetOmsOrderDetail '',@Order_BY = '',@RowsCount= 0, @UserId = 0 ,@Rows = 80, @PageNo = 1

*/
     BEGIN
         BEGIN TRY
			SET NOCOUNT ON;
			SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
			DECLARE @SQL NVARCHAR(MAX), @ProcessType  varchar(50)='Order'
			DECLARE @OrderLineItemRelationshipTypeId INT
			SET @OrderLineItemRelationshipTypeId = ( SELECT top 1 OrderLineItemRelationshipTypeId  FROM ZnodeOmsOrderLineItemRelationshipType where Name = 'AddOns' )

			IF OBJECT_ID('tempdb..#OrderList') is not null
			DROP TABLE #OrderList
			IF OBJECT_ID('tempdb..#Cte_OrderLineItem') is not null
			DROP TABLE #Cte_OrderLineItem
			IF OBJECT_ID('tempdb..#TBL_OrderList') is not null
			DROP TABLE #TBL_OrderList

			DECLARE @tbl_GetRecurciveUserId TABLE (ID INT IDENTITY(1,1) Primary key,UserId INT,ParentUserId INT)
			INSERT INTO @tbl_GetRecurciveUserId
			SELECT UserId,ParentUserId FROM dbo.Fn_GetRecurciveUserId (CAST(@UserId AS VARCHAR(50)),@ProcessType ) FNRU
			 
			CREATE TABLE #TBL_OrderList (OmsOrderId INT,OrderNumber VARCHAR(200),PortalId INT,StoreName NVARCHAR(MAX),CurrencyCode VARCHAR(100),OrderState NVARCHAR(MAX),ShippingId INT ,
			PaymentTypeId INT,PaymentSettingId INT,PaymentStatus NVARCHAR(MAX),PaymentType VARCHAR(100),ShippingStatus BIT ,OrderDate DATETIME,UserId INT,UserName VARCHAR(300),PaymentTransactionToken NVARCHAR(600),Total NUMERIC(28,6),
			OrderItem NVARCHAR(1000),OmsOrderDetailsId INT, ItemCount INT,PODocumentPath NVARCHAR(600),IsInRMA BIT,CreatedByName NVARCHAr(max),ModifiedByName NVARCHAR(max),RowId INT,CountNo INT,Email NVARCHAR(MAX),PhoneNumber NVARCHAR(MAX),
			SubTotal NUMERIC(28,6),TaxCost NUMERIC(28,6),ShippingCost NUMERIC(28,6),BillingPostalCode NVARCHAR(200),ShippingPostalCode NVARCHAR(200),OrderModifiedDate datetime, PaymentDisplayName nvarchar(1200), ExternalId nvarchar(1000)
			,CreditCardExpMonth	int,CreditCardExpYear	int,CardType	varchar(50),CreditCardNumber varchar(10),PaymentExternalId nvarchar(1000),CultureCode nvarchar(1000),PublishState nvarchar(600))
--			TotalAdditionalCost Numeric(28,6) Default(0))
			
			SELECT Zoo.OmsOrderId,Zoo.OrderNumber, Zp.PortalId,Zp.StoreName ,ZODD.CurrencyCode,case when ZOS.IsShowToCustomer=0 and cast( @IsFromAdmin as varchar(50)) = 0 then ZOSC.Description else  ZOS.Description end  OrderState,ZODD.ShippingId,ZODD.PaymentTypeId,ZODD.PaymentSettingId
			,ZOPS.Name PaymentStatus,ZPS.Name PaymentType,CAST(1 AS BIT) ShippingStatus ,ZODD.OrderDate,ZODD.UserId,ISNULL(RTRIM(LTRIM(ZODD.FirstName)),'')
			+' '+ISNULL(RTRIM(LTRIM(ZODD.LastName)),'') UserName ,ZODD.PaymentTransactionToken ,ZODD.Total ,ZODD.OmsOrderDetailsId,ZODD.PoDocument,ZVGD.UserName CreatedBy , ZVGDI.UserName ModifiedBy
			,ZU.Email ,ZU.PhoneNumber ,ZODD.SubTotal ,ZODD.TaxCost ,ZODD.ShippingCost,ZODD.BillingPostalCode,
			ZODD.ModifiedDate AS OrderModifiedDate,  ZODD.PaymentDisplayName  ,isnull(Zoo.ExternalId,0) ExternalId,ZODD.CreditCardExpMonth,ZODD.CultureCode--,ZODD.TotalAdditionalCost
			,ZODD.CreditCardExpYear,ZODD.CardType,ZODD.CreditCardNumber,ZODD.PaymentExternalId,ZODPS.DisplayName as PublishState
			INTO #OrderList
			FROM ZnodeOmsOrder (nolock) ZOO 
			INNER JOIN ZnodeOmsOrderDetails (nolock) ZODD ON (ZODD.OmsOrderId = ZOO.OmsOrderId AND  ZODD.IsActive = 1)
			INNER JOIN ZnodePortal (nolock) ZP ON (ZP.PortalId = ZODD.portalId )
			INNER JOIN ZnodePublishState ZODPS ON (ZODPS.PublishStateId = ZOO.PublishStateId)
			LEFT JOIN ZnodePaymentType (nolock) ZPS ON (ZPS.PaymentTypeId = ZODD.PaymentTypeId )
			LEFT JOIN  ZnodeOmsOrderStateShowToCustomer (nolock) ZOSC ON (ZOSC.OmsOrderStateId = ZODD.OmsOrderStateId)
			LEFT JOIN ZnodeOmsOrderState (nolock) ZOS ON (ZOS.OmsOrderStateId = ZODD.OmsOrderStateId)
			LEFT JOIN ZnodeOmsPaymentState (nolock) ZOPS ON (ZOPS.OmsPaymentStateId = ZODD.OmsPaymentStateId)
			LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZODD.UserId)
			LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGD ON (ZVGD.UserId = ZODD.CreatedBy )
			LEFT JOIN [dbo].[View_GetUserDetails]  (nolock) ZVGDI ON (ZVGDI.UserId = ZODD.ModifiedBy)
			LEFT JOIN ZnodeShipping ZS ON (ZS.ShippingId = ZODD.ShippingId)
			LEFT OUTER JOIN ZnodePaymentSetting (nolock) ZPSS ON (ZPSS.PaymentSettingId = ZODD.PaymentSettingId)
			LEFT JOIN ZnodePortalPaymentSetting (nolock) ZPPS ON (ZPPS.PaymentSettingId = ZPSS.PaymentSettingId  AND ZPPS.PortalId = ZODD.PortalId   )
			WHERE (EXISTS (SELECT TOP 1 1 FROM @tbl_GetRecurciveUserId FNRU WHERE FNRU.UserId = ZU.UserId ) OR @UserId  =0 )
			
			ALTER TABLE #OrderList ADD ShippingPostalCode VARCHAR(50)    
			
			UPDATE OL SET OL.ShippingPostalCode=ZOOS.ShipToPostalCode
			from #OrderList OL
			INNER JOIN ZnodeOmsOrderLineItems (nolock) ZOOLI ON (ZOOLI.OmsOrderDetailsId = OL.OmsOrderDetailsId)
			INNER JOIN ZnodeOmsOrderShipment (nolock) ZOOS ON (ZOOS.OmsOrderShipmentId = ZOOLI.OmsOrderShipmentId)
			
			SELECT ZOOLI.ProductName,ZOOLI.Price,Count(ZOOLI.OmsOrderLineItemsId)Over(PARTITION BY Ol.OmsOrderId Order by ZOOLI.OmsOrderDetailsId) CountId
			,Row_Number()Over( PARTITION BY Ol.OmsOrderId Order BY ZOOLI.Price DESC, ZOOLI.ProductName) RowId,Ol.OmsOrderId
			,CAST(Case when ZRRLI.RmaRequestItemId IS NULL THEN 0 ELSE 1 END AS BIT )  IsInRMA  ,OL.CreatedBy ,OL.ModifiedBy
			into #Cte_OrderLineItem
			FROM ZnodeOmsOrderLineItems (nolock) ZOOLI
			LEFT JOIN #OrderList OL ON ( OL.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId )
			LEFT JOIN ZnodeRmaRequestItem (Nolock) ZRRLI ON (ZRRLI.OmsOrderLineItemsId = ZOOLI.OmsOrderLineItemsId )
			WHERE ZOOLI.Quantity > 0 AND ParentOmsOrderLineItemsId IS NOT NULL
			AND OrderLineItemRelationshipTypeId <> @OrderLineItemRelationshipTypeId
			
			CREATE NONCLUSTERED INDEX [Idx_#Cte_OrderLineItem_RowId] ON [dbo].[#Cte_OrderLineItem] ([RowId])

			SELECT OL.OmsOrderId,OL.OrderNumber,OL.PortalId,OL.StoreName,OL.CurrencyCode,OL.OrderState,OL.ShippingId,OL.PaymentTypeId,OL.PaymentSettingId,
			OL.PaymentStatus,OL.PaymentType,OL.ShippingStatus ,OL.OrderDate,OL.UserId,OL.UserName ,OL.PaymentTransactionToken ,OL.Total ,OL.OmsOrderDetailsId,
			OL.PoDocument,OL.CreatedBy,OL.ModifiedBy,OL.Email ,OL.PhoneNumber ,OL.SubTotal ,OL.TaxCost ,OL.ShippingCost,OL.BillingPostalCode,
			OrderModifiedDate,PaymentDisplayName  ,ExternalId,CreditCardExpMonth,CultureCode,PublishState--TotalAdditionalCost
			,OL.CreditCardExpYear,OL.CardType,OL.CreditCardNumber,OL.PaymentExternalId,OL.ShippingPostalCode,
			CTOLI.ProductName,CountId ,IsInRMA
			INTO #Cte_GetOrderData
			FROM #OrderList OL
			LEFT JOIN #Cte_OrderLineItem CTOLI ON (CTOLI.OmsOrderId = OL.OmsOrderId AND CTOLI.RowId = 1 )

			SET @SQL = '
			;with Cte_OrderLineDescribe AS 
			(
			SELECT DISTINCT OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,PaymentTypeId,PaymentSettingId,
			PaymentStatus,PaymentType,ShippingStatus ,OrderDate,UserId,UserName ,PaymentTransactionToken ,Total ,OmsOrderDetailsId,
			PoDocument,CreatedBy,ModifiedBy,Email ,PhoneNumber ,SubTotal ,TaxCost ,ShippingCost,BillingPostalCode,
			OrderModifiedDate,PaymentDisplayName  ,ExternalId,CreditCardExpMonth,CultureCode,PublishState--TotalAdditionalCost
			,CreditCardExpYear,CardType,CreditCardNumber,PaymentExternalId,ShippingPostalCode,
			ProductName,CountId ,IsInRMA,'+dbo.Fn_GetPagingRowId(@Order_BY,'OmsOrderId DESC,OmsOrderDetailsId DESC')+',Count(*)Over() CountNo
			FROM #Cte_GetOrderData
			WHERE 1= 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			)
			SELECT OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,
			PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId,UserName,PaymentTransactionToken,Total,
			ProductName OrderItem,OmsOrderDetailsId,CountId ItemCount, PoDocument AS PODocumentPath,IsInRMA ,CASE WHEN CreatedBy IS NULL THEN email  ELSE CreatedBy END AS CreatedByName ,ModifiedBy as ModifiedByName,RowId,CountNo,
			Email,PhoneNumber,SubTotal,TaxCost,ShippingCost,BillingPostalCode, ShippingPostalCode,OrderModifiedDate,PaymentDisplayName, ExternalId,CreditCardExpMonth
			,CreditCardExpYear,CardType,CreditCardNumber,PaymentExternalId,CultureCode,PublishState--TotalAdditionalCost
			FROM Cte_OrderLineDescribe
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			 
			INSERT INTO #TBL_OrderList(OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,
			PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId,UserName,PaymentTransactionToken,Total,
			OrderItem,OmsOrderDetailsId, ItemCount, PODocumentPath,IsInRMA ,CreatedByName ,ModifiedByName,RowId,CountNo,Email,PhoneNumber,SubTotal,TaxCost,ShippingCost,BillingPostalCode,ShippingPostalCode,OrderModifiedDate,PaymentDisplayName ,ExternalId,CreditCardExpMonth
			,CreditCardExpYear,CardType,CreditCardNumber ,PaymentExternalId,CultureCode,PublishState)--TotalAdditionalCost)
			EXEC(@SQL)
			
			SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM #TBL_OrderList),0)
			
			SELECT OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,
			PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId,UserName,PaymentTransactionToken,Total,
			OrderItem,OmsOrderDetailsId, ItemCount, PODocumentPath,IsInRMA ,CreatedByName ,ModifiedByName,Email,PhoneNumber,SubTotal,TaxCost,ShippingCost,BillingPostalCode,ShippingPostalCode,OrderModifiedDate,PaymentDisplayName,ExternalId,CreditCardExpMonth,CreditCardExpYear,CardType,CreditCardNumber,PaymentExternalId,CultureCode,PublishState--TotalAdditionalCost
			FROM #TBL_OrderList
			order by RowId
			
			IF OBJECT_ID('tempdb..#OrderList') is not null
			DROP TABLE #OrderList
			IF OBJECT_ID('tempdb..#Cte_OrderLineItem') is not null
			DROP TABLE #Cte_OrderLineItem
			IF OBJECT_ID('tempdb..#TBL_OrderList') is not null
			DROP TABLE #TBL_OrderList
			
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