CREATE PROCEDURE [dbo].[Znode_GetOmsGroupOrderListExpandDetail]  
( @WhereClause NVARCHAR(MAx),  
  @Rows        INT            = 10,  
  @PageNo      INT            = 1,  
  @Order_BY    VARCHAR(1000)  = '',  
  @UserId      INT = 0 ,  
  @IsFromAdmin int =0  ,
  @Expands    NVARCHAR(2000) = '' 
  )  
AS  
  /*  
     Summary : This procedure is used to get the oms order detils  
      Records are fetched for those users who placed the order i.e UserId is Present in ZnodeUser and  ZnodeOmsOrderDetails tables  
  Unit Testing:  
 
     EXEC _Znode_GetOmsGroupOrderListExpandDetail 'orderstate like ''%submitted%'' ',@Order_BY = '', @UserId = 0 ,@Rows = 50,@IsFromAdmin=1 , @PageNo = 1,@Expands = 'ZnodeOmsOrderShipment,ZnodeOmsOrderLineItems,Znodeomsorderlinepersonalize'  
*/  

BEGIN  
	BEGIN TRY  
		SET NOCOUNT ON;  
		DECLARE @SQL NVARCHAR(MAX), @ProcessType  varchar(50)='Order',@RowsCount INT = 0   
			--SET @Rows = 10 
  
		CREATE TABLE #TBL_OrderList  (OmsOrderId INT,OrderNumber VARCHAR(200),PortalId INT,StoreName NVARCHAR(MAX),CurrencyCode VARCHAR(100),OrderState NVARCHAR(150),ShippingId INT ,  
		PaymentTypeId INT,PaymentSettingId INT,PaymentStatus NVARCHAR(MAX),PaymentType VARCHAR(100),ShippingStatus BIT ,OrderDate DATETIME,UserId INT,UserName VARCHAR(300),PaymentTransactionToken NVARCHAR(600),Total NUMERIC(28,6),  
		OrderItem NVARCHAR(1000),OmsOrderDetailsId INT, ItemCount INT,PODocumentPath NVARCHAR(600),IsInRMA BIT,CreatedByName NVARCHAr(150),ModifiedByName NVARCHAR(150),RowId INT,CountNo INT,Email NVARCHAR(150),PhoneNumber NVARCHAR(150),  
		SubTotal NUMERIC(28,6),TaxCost NUMERIC(28,6),ShippingCost NUMERIC(28,6),BillingPostalCode NVARCHAR(200),ShippingPostalCode NVARCHAR(200),OrderModifiedDate datetime, PaymentDisplayName nvarchar(1200), ExternalId nvarchar(1000)  
		,BillingAddress NVARCHAR(max),ShippingCode nvarchar (max),CreditCardExpMonth	int,CreditCardExpYear	int,CardType	varchar(50),CreditCardNumber varchar(10),Custom1 NVARCHAR(500),Custom2 NVARCHAR(500),Custom3 NVARCHAR(500),Custom4 NVARCHAR(500),Custom5 NVARCHAR(500),PurchaseOrderNumber NVARCHAR(500) , PaymentExternalId nvarchar(1000) 
		,ID int Identity (1,1) Primary Key)  
  
  
		CREATE TABLE #Portal (PortalId int,StoreName varchar(200))
		insert into #Portal
		select PortalId,StoreName
		from ZnodePortal
		--SET  @Rows = 10
     SET @SQL = '  
  
     SELECT Zoo.OmsOrderId,Zoo.OrderNumber, Zp.PortalId,Zp.StoreName ,ZODD.CurrencyCode,case when ZOS.IsShowToCustomer=0 then ZOSC.Description else  ZOS.Description end  OrderState,ZODD.ShippingId,ZODD.  
     PaymentTypeId,ZODD.PaymentSettingId,ZOPS.Name PaymentStatus,ZPS.Name PaymentType,CAST(1 AS BIT) ShippingStatus ,ZODD.OrderDate,ZODD.UserId,ISNULL(ZODD.FirstName,'''')  
      +'' ''+ISNULL(ZODD.LastName,'''') UserName ,ZODD.PaymentTransactionToken ,ZODD.Total ,ZODD.OmsOrderDetailsId,ZODD.PoDocument PODocumentPath,ZODD.SubTotal ,ZODD.TaxCost ,ZODD.ShippingCost,ZODD.BillingPostalCode,  
      ZODD.ModifiedDate AS OrderModifiedDate, ZODD.PaymentDisplayName,isnull(Zoo.ExternalId,0) ExternalId  
       ,BillingFirstName ,BillingLastName ,BillingCompanyName   
    ,BillingStreet1  ,BillingStreet2 ,BillingCity ,BillingStateCode   
    ,BillingCountry ,BillingPhoneNumber ,BillingEmailId,zs.ShippingCode,ZODD.CreditCardExpMonth
	,ZODD.CreditCardExpYear,ZODD.CardType,ZODD.CreditCardNumber,ZODD.Custom1, ZODD.Custom2, ZODD.Custom3, ZODD.Custom4, ZODD.Custom5,
	ZODD.PurchaseOrderNumber,ZODD.PaymentExternalId
     INTO #TempOrderData   
     FROM ZnodeOmsOrder ZOO  with (nolock) 
     INNER JOIN ZnodeOmsOrderDetails ZODD with (nolock)  ON ( ZOO.OmsOrderId = ZODD.OmsOrderId )  
     INNER JOIN #Portal ZP ON (ZP.PortalId = ZODD.portalId )  
     LEFT JOIN ZnodePaymentType ZPS with (nolock)  ON (ZPS.PaymentTypeId = ZODD.PaymentTypeId )  
     LEFT JOIN ZnodeOmsOrderStateShowToCustomer ZOSC with (nolock) ON (ZOSC.OmsOrderStateId = ZODD.OmsOrderStateId)  
     LEFT JOIN ZnodeOmsOrderState ZOS with (nolock) ON (ZOS.OmsOrderStateId = ZODD.OmsOrderStateId)  
     LEFT JOIN ZnodeOmsPaymentState ZOPS with (nolock) ON (ZOPS.OmsPaymentStateId = ZODD.OmsPaymentStateId) 
     LEFT JOIN ZnodeShipping ZS with (nolock) ON (ZS.ShippingId = ZODD.ShippingId)  

     WHERE  ZODD.IsActive = 1 AND (ZODD.UserId ='+ CAST(@UserId AS VARCHAR(50))+'  OR '+ CAST(@UserId AS VARCHAR(50))+' =0  )
  
    ;with Cte_PagingData AS   
    (  
    SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'OmsOrderId DESC,OmsOrderDetailsId DESC')+',Count(*)Over() CountNo   
    FROM #TempOrderData  a   
    WHERE 1= 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'   
    )   
     
    SELECT OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,  
      PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId  
      ,UserName,PaymentTransactionToken,Total,OmsOrderDetailsId, PODocumentPath,RowId,CountNo,SubTotal,TaxCost,ShippingCost, BillingPostalCode
      ,OrderModifiedDate,PaymentDisplayName ,ExternalId,(SELECT BillingFirstName FirstName ,BillingLastName LastName ,BillingCompanyName   
    ,BillingStreet1 Address1  ,BillingStreet2 Address2 ,BillingCity CityName ,BillingStateCode StateName  ,BillingPostalCode PostalCode
    ,BillingCountry CountryName,BillingPhoneNumber PhoneNumber ,BillingEmailId EmailAddress  FOR JSON PATH,WITHOUT_ARRAY_WRAPPER ) BillingAddress, ShippingCode
	,CreditCardExpMonth,CreditCardExpYear,CardType,CreditCardNumber, Custom1, Custom2, Custom3, Custom4, Custom5, PurchaseOrderNumber,PaymentExternalId
    FROM Cte_PagingData   
    '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
    
	INSERT INTO #TBL_OrderList(OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,  
	PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId
	,UserName,PaymentTransactionToken,Total,OmsOrderDetailsId,PODocumentPath,RowId,CountNo,SubTotal,TaxCost,ShippingCost, BillingPostalCode
	,OrderModifiedDate,PaymentDisplayName ,ExternalId,BillingAddress,ShippingCode
	,CreditCardExpMonth,CreditCardExpYear,CardType,CreditCardNumber,Custom1,Custom2,Custom3,Custom4,Custom5,PurchaseOrderNumber,PaymentExternalId)
	EXEC(@SQL)  

	SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM #TBL_OrderList),0)  
   
	SELECT *   
	INTO #ZnodeOmsOrderLineItems   
	FROM ZnodeOmsOrderLineItems a with (nolock)   
	WHERE EXISTS (SELECT TOP 1 1  FROM #TBL_OrderList ty WHERE ty.OmsOrderDetailsId = a.OmsOrderDetailsId )   
	AND (EXISTS (SELECT TOP 1 1 FROM dbo.Split(@Expands, ',') TH WHERE TH.Item = 'ZnodeOmsOrderLineItems' ) 
	OR @Expands = '' ) 

	SELECT  OmsOrderShipmentId ,ShipToFirstName FirstName, ShipToLastName LastName,ShipToCountry CountryName,ShipToStateCode StateName
			, ShipToCity CityName,ShipToPostalCode PostalCode,ShipToPhoneNumber PhoneNumber,ShipToEmailId EmailAddress , 
			ShipToStreet1 Address1,ShipToStreet2 Address2
	INTO   #ZnodeOmsOrderShipment  
	FROM ZnodeOmsOrderShipment a with (nolock)   
	WHERE EXISTS (SELECT TOP 1 1  FROM #ZnodeOmsOrderLineItems TY WHERE TY.OmsOrderShipmentId = a.OmsOrderShipmentId)   
	AND (EXISTS (SELECT TOP 1 1 FROM dbo.Split(@Expands, ',') TH WHERE TH.Item = 'ZnodeOmsOrderShipment')    
  		OR @Expands = '' ) 

	SELECT DISTINCT  h.OmsOrderDetailsId , (SELECT TOP 1  (SELECT DISTINCT  a.OmsOrderShipmentId ,a.FirstName, a.LastName,a.CountryName,a.StateName
			, a.CityName,a.PostalCode,a.PhoneNumber,a.EmailAddress , 
			a.Address1,a.Address2 FOR JSON PATH,WITHOUT_ARRAY_WRAPPER ) ShippingAddress
			FROM #ZnodeOmsOrderShipment a  
			INNER JOIN  #ZnodeOmsOrderLineItems b ON(b.OmsOrderShipmentId = a.OmsOrderShipmentId)
			WHERE h.OmsOrderDetailsId = b.OmsOrderDetailsId)  ShippingAddress
	INTO #ZnodeOmsOrderShipmentOuter
	FROM  #TBL_OrderList H 

   SELECT AttributeCode,AttributeValue,AttributeValueCode ,OmsOrderLineItemsId  
   INTO #ZnodeOmsOrderAttribute  
   FROM ZnodeOmsOrderAttribute a   with (nolock) 
   WHERE EXISTS (SELECT TOP 1  1  FROM #ZnodeOmsOrderLineItems TY WHERE TY.OmsOrderLineItemsId = a.OmsOrderLineItemsId)   

        
   SELECT (SELECT  OmsOrderId,OrderNumber,PortalId,StoreName,CurrencyCode,OrderState,ShippingId,  
   PaymentTypeId,PaymentSettingId,PaymentStatus,PaymentType,ShippingStatus,OrderDate,UserId,UserName,PaymentTransactionToken,Total,  
   OrderItem,OmsOrderDetailsId, ItemCount, PODocumentPath,IsInRMA ,SubTotal
   ,TaxCost,ShippingCost,ShippingPostalCode,OrderModifiedDate,PaymentDisplayName,ExternalId,ShippingCode
   ,CreditCardExpMonth,CreditCardExpYear,CardType,CreditCardNumber,Custom1,Custom2,Custom3,Custom4,Custom5,PurchaseOrderNumber,PaymentExternalId
    ,(SELECT r.*,(SELECT aa.* ,(SELECT *  
       FROM #ZnodeOmsOrderShipment a   
       WHERE a.OmsOrderShipmentId = aa.OmsOrderShipmentId   
       FOR JSON PATH ) ShippingAddress ,(SELECT AttributeCode,AttributeValue,AttributeValueCode  
       FROM #ZnodeOmsOrderAttribute o  
       WHERE o.OmsOrderLineItemsId =aa.OmsOrderLineItemsId   
       FOR JSON PATH ) Attributes   
       FROM #ZnodeOmsOrderLineItems aa   
       WHERE aa.ParentOmsOrderLineItemsId = r.OmsOrderLineItemsId   
	   AND aa.OrderLineItemRelationshipTypeId IS NOT NULL  
	   ORDER BY aa.OmsOrderLineItemsId 
       FOR JSON PATH ) OrderLineItemCollection,(SELECT *  
       FROM #ZnodeOmsOrderShipment a   
       WHERE a.OmsOrderShipmentId = r.OmsOrderShipmentId   
       FOR JSON PATH ) ShippingAddress  
       ,(SELECT AttributeCode,AttributeValue,AttributeValueCode  
       FROM #ZnodeOmsOrderAttribute o  
       WHERE o.OmsOrderLineItemsId = r.OmsOrderLineItemsId   
       FOR JSON PATH ) Attributes  
    FROM #ZnodeOmsOrderLineItems r   
    WHERE r.OmsOrderDetailsId = p.OmsOrderDetailsId 
	AND r.OrderLineItemRelationshipTypeId IS NULL 
	ORDER BY r.OmsOrderLineItemsId 
    FOR JSON PATH) OrderLineItems,(SELECT JSON_QUERY(BillingAddress)
               ) BillingAddress ,(SELECT JSON_QUERY(ShippingAddress)
              FROM #ZnodeOmsOrderShipmentOuter INO
			  WHERE INO.OmsOrderDetailsId = p.OmsOrderDetailsId  ) ShippingAddress  FOR JSON PATH ,WITHOUT_ARRAY_WRAPPER) OrderJSON  ,@RowsCount RowsCount  
   FROM #TBL_OrderList p  
            
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