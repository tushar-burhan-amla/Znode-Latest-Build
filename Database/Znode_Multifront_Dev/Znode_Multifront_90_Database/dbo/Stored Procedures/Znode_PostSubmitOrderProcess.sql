CREATE PROCEDURE [dbo].[Znode_PostSubmitOrderProcess]
(
	@PostOrderXml XML,
	@InventoryData XML,
	@UserId INT,
	@PortalId INT = 0,
	@Status BIT OUT
)
AS
BEGIN
BEGIN TRY
--BEGIN TRAN PostOrder
	DECLARE @OmsOrderDetailsId INT
	DECLARE @OmsOrderId INT = (SELECT Tbl.Col.value( 'OrderID[1]', 'NVARCHAR(2000)' ) AS OrderId FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col))
	
	DECLARE @IsGuest BIT = (SELECT Tbl.Col.value( 'IsGuest[1]', 'NVARCHAR(2000)' ) AS IsGuest FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col))

	DECLARE @OmsCookieMappingId INT = (SELECT Tbl.Col.value( 'CookieMappingId[1]', 'NVARCHAR(2000)' ) AS CookieMappingId FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col))
	DECLARE @BillingAddressId INT = (SELECT Tbl.Col.value( 'BillingAddressId[1]', 'NVARCHAR(2000)' ) AS BillingAddressId FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col))
	
	DECLARE @IsReferralCommission BIT = (SELECT Tbl.Col.value( 'IsReferralCommission[1]', 'NVARCHAR(2000)' ) AS IsReferralCommission FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col))
	DECLARE @CommissionAmount NUMERIC(26,6) = (SELECT Tbl.Col.value( 'CommissionAmount[1]', 'NVARCHAR(2000)' ) AS CommissionAmount FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col))
	
	SET @OmsOrderDetailsId = (SELECT OmsOrderDetailsId from ZnodeOmsOrderDetails WHERE OmsOrderId = @OmsOrderId AND IsActive = 1)
	DECLARE @SetBillingShippingFlags BIT = (SELECT Tbl.Col.value( 'SetBillingShippingFlags[1]', 'NVARCHAR(2000)' ) AS SetBillingShippingFlags FROM @PostOrderXml.nodes('//PostOrderSubmitModel') AS Tbl(Col)) 
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	
	--Fetching required user details 
	DECLARE @ReferralCommissionTypeId INT, @ReferralCommission NUMERIC(28,6) , @FirstName VARCHAR(100),@LastName VARCHAR(100)
	SELECT @FirstName = FirstName,@LastName = LastName, @ReferralCommission = CASE WHEN ReferralCommission IS NULL THEN 0 ELSE ReferralCommission END, @ReferralCommissionTypeId = ReferralCommissionTypeId
	FROM ZnodeUser WITH (NOLOCK) 
	WHERE UserId = @UserId
	
	---Voucher model fetch
	CREATE TABLE #TempVoucher
	(
		VoucherBalance NUMERIC(28,6),VoucherNumber NVARCHAR(600),VoucherMessage NVARCHAR(MAX),IsVoucherValid BIT,
		IsVoucherApplied BIT,VoucherAmountUsed NUMERIC(28,6),VoucherName NVARCHAR(600),ExpirationDate DATETIME,
		CultureCode NVARCHAR(600),PortalId INT,IsExistInOrder BIT,UserId INT,IsActive BIT,OrderVoucherAmount NUMERIC(28,6)
	)

	INSERT INTO #TempVoucher
	(
		VoucherBalance ,VoucherNumber,VoucherMessage ,IsVoucherValid ,
		IsVoucherApplied ,VoucherAmountUsed ,VoucherName ,ExpirationDate ,
		CultureCode ,PortalId ,IsExistInOrder ,UserId ,IsActive ,OrderVoucherAmount 
	)
	SELECT Tbl.Col.value( 'VoucherBalance[1]', 'NVARCHAR(2000)' ) AS VoucherBalance,
		Tbl.Col.value( 'VoucherNumber[1]', 'NVARCHAR(2000)' ) AS VoucherNumber,
		Tbl.Col.value( 'VoucherMessage[1]', 'NVARCHAR(2000)' ) AS VoucherMessage,
		Tbl.Col.value( 'IsVoucherValid[1]', 'NVARCHAR(2000)' ) AS IsVoucherValid,
		Tbl.Col.value( 'IsVoucherApplied[1]', 'NVARCHAR(2000)' ) AS IsVoucherApplied,
		Tbl.Col.value( 'VoucherAmountUsed[1]', 'NVARCHAR(2000)' ) AS VoucherAmountUsed,
		Tbl.Col.value( 'VoucherName[1]', 'NVARCHAR(2000)' ) AS VoucherName,
		Tbl.Col.value( 'ExpirationDate[1]', 'NVARCHAR(2000)' ) AS ExpirationDate,
		Tbl.Col.value( 'CultureCode[1]', 'NVARCHAR(2000)' ) AS CultureCode,
		Tbl.Col.value( 'PortalId[1]', 'NVARCHAR(2000)' ) AS PortalId,
		Tbl.Col.value( 'IsExistInOrder[1]', 'NVARCHAR(2000)' ) AS IsExistInOrder,
		Tbl.Col.value( 'UserId[1]', 'NVARCHAR(2000)' ) AS UserId,	
		Tbl.Col.value( 'IsActive[1]', 'NVARCHAR(2000)' ) AS IsActive,
		Tbl.Col.value( 'OrderVoucherAmount[1]', 'NVARCHAR(2000)' ) AS OrderVoucherAmount
	FROM @PostOrderXml.nodes('//PostOrderSubmitModel/Vouchers/VoucherModel') AS Tbl(Col)


	----Promotion coupon start
	DECLARE @TempPromotionCoupon DBO.PromotionCoupons

	INSERT INTO @TempPromotionCoupon
	SELECT Tbl.Col.value( 'Code[1]', 'NVARCHAR(2000)' ) AS Code,
		Tbl.Col.value( 'IsExistInOrder[1]', 'NVARCHAR(2000)' ) AS IsExistInOrder,@OmsOrderId AS OmsOrderId
	FROM @PostOrderXml.nodes('//PostOrderSubmitModel/Coupons/CouponModel') AS Tbl(Col)

	--Deduct promotion coupon count
	EXEC [Znode_PromotionCouponDeduct] @PromotionCoupons = @TempPromotionCoupon
	----Promotion coupon END
	
	--Deduct inventory quantity
	EXEC [Znode_UpdateInventoryPostOrder] @SkuXml = @InventoryData,@PortalId = @PortalId, @UserId= @UserId,@OmsOrderId=@OmsOrderId,@Status = 0

	----Voucher start
	IF EXISTS(SELECT * FROM #TempVoucher)
	BEGIN
		INSERT INTO ZnodeGiftCardHistory(GiftCardId,TransactionDate,TransactionAmount,OmsOrderDetailsId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Notes,RemainingAmount)
		SELECT GC.GiftCardId, @GetDate, VoucherAmountUsed,@OmsOrderDetailsId,@UserId, @GetDate, @UserId, @GetDate, 'Voucher is used to make payments for the order' as Notes, NULL AS RemainingAmount
		FROM #TempVoucher V
		INNER JOIN ZnodeGiftCard GC WITH (NOLOCK) ON V.VoucherNumber = GC.CardNumber

		IF EXISTS(SELECT * FROM #TempVoucher WHERE IsExistInOrder IN ('TRUE','1'))
		BEGIN
			UPDATE GC SET GC.RemainingAmount = V.VoucherBalance
			FROM #TempVoucher V
			INNER JOIN ZnodeGiftCard GC ON V.VoucherNumber = GC.CardNumber
			WHERE IsExistInOrder IN ('TRUE','1')  			
		END

		IF EXISTS(SELECT * FROM #TempVoucher WHERE IsExistInOrder IN ('FALSE','0'))
		BEGIN
			UPDATE GC SET GC.RemainingAmount = GC.RemainingAmount - V.VoucherAmountUsed
			FROM #TempVoucher V
			INNER JOIN ZnodeGiftCard GC ON V.VoucherNumber = GC.CardNumber
			WHERE IsExistInOrder IN ('FALSE','0')			
		END
	END

		UPDATE GC SET GC.UserId=@UserId  
		FROM  ZnodeGiftCard GC 
		INNER JOIN #TempVoucher V ON V.VoucherNumber = GC.CardNumber
		WHERE  GC.Userid IS NULL

	----Voucher end
	
	--Update user details
	IF @FirstName IS NULL
	BEGIN
		DECLARE @FirstName1 VARCHAR(300), @LastName1 VARCHAR(300), @PhoneNumber VARCHAR(300)
		SELECT @FirstName1=FirstName, @LastName1 = LastName, @PhoneNumber=PhoneNumber
		FROM ZnodeAddress WITH (NOLOCK)  WHERE AddressId = @BillingAddressId

		UPDATE ZnodeUser 
		SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber
		WHERE UserId = @UserId
	END

	--Update address details
	IF @SetBillingShippingFlags IN ('TRUE','1')
	BEGIN
		UPDATE ZnodeAddress
		SET IsBilling = 1, IsShipping = 1
		WHERE AddressId = @BillingAddressId
	END

	--ReferralCommission details
	IF @IsReferralCommission IN ('TRUE','1')
	BEGIN
		INSERT INTO ZnodeOmsReferralCommission
		(
			UserId,OmsOrderDetailsId,OrderCommission,TransactionId,Description,ReferralCommission,ReferralCommissionTypeId
			,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		)
		SELECT @UserId,@OmsOrderDetailsId,CASE WHEN @ReferralCommissionTypeId =1 THEN ((@CommissionAmount * ISNULL(@ReferralCommission,0)) / 100) ELSE @ReferralCommission END AS OrderCommission,'' AS TransactionId,'' AS Description,@ReferralCommission,@ReferralCommissionTypeId
			,@UserId,@GetDate,@UserId,@GetDate
	END
	
	---OmsTaxOrderSummary details
	IF EXISTS(SELECT * FROM ZnodeOmsTaxOrderSummary WITH (NOLOCK)  WHERE OmsOrderDetailsId = @OmsOrderDetailsId)
	BEGIN
		DELETE FROM ZnodeOmsTaxOrderSummary WHERE OmsOrderDetailsId = @OmsOrderDetailsId
	END

	INSERT INTO ZnodeOmsTaxOrderSummary (OmsOrderDetailsId,Tax,Rate,TaxName,TaxTypeName)
	SELECT 	@OmsOrderDetailsId AS OmsOrderDetailsId,
		Tbl.Col.value( 'Tax[1]', 'NVARCHAR(2000)' ) AS Tax,
		Tbl.Col.value( 'Rate[1]', 'NVARCHAR(2000)' ) AS Rate,
		Tbl.Col.value( 'TaxName[1]', 'NVARCHAR(2000)' ) AS TaxName,
		Tbl.Col.value( 'TaxTypeName[1]', 'NVARCHAR(2000)' ) AS TaxTypeName
	FROM @PostOrderXml.nodes('//PostOrderSubmitModel/TaxSummaryList/TaxSummaryModel') AS Tbl(Col)
	WHERE Tbl.Col.value( 'Tax[1]', 'NVARCHAR(2000)' ) IS NOT NULL
	--------

	--Remove 
	EXEC [Znode_DeleteSavedCartItem] @OmsCookieMappingId = @OmsCookieMappingId, @UserId = @UserId,@PortalId = @PortalId,@Status = 0  

	SET @Status = 1
	--COMMIT TRAN PostOrder
END TRY
BEGIN CATCH
--ROLLBACK TRAN PostOrder
	SET @Status = 0
	
	DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PostSubmitOrderProcess @PostOrderXml ='+CAST(@PostOrderXml AS VARCHAR(MAX))+' , @InventoryData = '+CAST(@InventoryData AS VARCHAR(50))+',@UserId ='+CAST(@UserId AS VARCHAR(50))+',@PortalId ='+CAST(@PortalId AS VARCHAR(50))+',@Status ='+CAST(@Status AS VARCHAR(50));
	
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName    = 'Znode_PostSubmitOrderProcess',
	@ErrorInProcedure = @ERROR_PROCEDURE,
	@ErrorMessage     = @ErrorMessage,
	@ErrorLine        = @ErrorLine,
	@ErrorCall        = @ErrorCall;
END CATCH

END