CREATE PROCEDURE [dbo].[Znode_InsertUpdatePromotions]   
(
	@PromotionXML XML,
	@UserId INT,
	@Status BIT=0 OUT
)
/*  Unit Testing:-
	begin tran  
 Exec Znode_InsertUpdatePromotions @PromotionXML='<PromotionModel>
  <PromotionId>0</PromotionId>
  <PromoCode>Aditya</PromoCode>
  <Name>Aditya</Name>
  <PromotionTypeId>5</PromotionTypeId>
  <PromotionTypeName>Amount Off Order</PromotionTypeName>
  <Discount>10.0</Discount>
  <StartDate>2022-08-25T00:00:00</StartDate>
  <EndDate>2022-09-24T00:00:00</EndDate>
  <OrderMinimum>1.0</OrderMinimum>
  <QuantityMinimum>0</QuantityMinimum> p2:nil="true" xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" />
  <IsCouponRequired>true</IsCouponRequired>
  <DisplayOrder>99</DisplayOrder>
  <IsUnique>false</IsUnique>
  <PortalId>0</PortalId> p2:nil="true" xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" />
  <ProfileId>0</ProfileId> p2:nil="true" xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" />
  <ProfileIdsList>
    <int>80</int>
	</ProfileIdsList>
  <PromotionProductQuantity>0.0</PromotionProductQuantity>
  <ReferralPublishProductId>0</ReferralPublishProductId>
  <CouponList>
    <PageIndex>0</PageIndex> p3:nil="true" xmlns:p3="http://www.w3.org/2001/XMLSchema-instance" />
    <PageSize>0</PageSize> p3:nil="true" xmlns:p3="http://www.w3.org/2001/XMLSchema-instance" />
    <TotalResults>0</TotalResults>
    <CouponList>
      <CouponModel>
        <PromotionCouponId>0</PromotionCouponId>
        <PromotionId>0</PromotionId>
        <Code>Aditya</Code>
        <InitialQuantity>6</InitialQuantity>
        <AvailableQuantity>6</AvailableQuantity>
        <IsEnableUrl>false</IsEnableUrl>
        <IsActive>true</IsActive>
        <CouponApplied>false</CouponApplied>
        <CouponValid>false</CouponValid>
        <CustomCouponCode>0</CustomCouponCode>
        <IsCustomCoupon>false</IsCustomCoupon>
        <IsExistInOrder>false</IsExistInOrder>
      </CouponModel>
    </CouponList>
  </CouponList>
  <PortalAllowsMultipleCoupon>false</PortalAllowsMultipleCoupon>
  <IsAllowedWithOtherCoupons>true</IsAllowedWithOtherCoupons>
  <AssociatedCatelogIds>0</AssociatedCatelogIds>
  <AssociatedCategoryIds>0</AssociatedCategoryIds>
  <AssociatedProductIds>0</AssociatedProductIds>
  <AssociatedBrandIds>0</AssociatedBrandIds>
  <AssociatedShippingIds>0</AssociatedShippingIds>
  <IsAllowWithOtherPromotionsAndCoupons>false</IsAllowWithOtherPromotionsAndCoupons>
  <IsActive>false</IsActive>
  <IsUsedInOrder>false</IsUsedInOrder>
</PromotionModel>',@UserID=1,@Status=1
rollback tran
*/
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DROP TABLE IF EXISTS tempdb..#TEMP
	SELECT Tbl.Col.value('PromotionId[1]','NVARCHAR(600)') AS PromotionId  
		,Tbl.Col.value('PromoCode[1]','NVARCHAR(600)') AS [PromoCode]
		,Tbl.Col.value('Description[1]','NVARCHAR(Max)') AS [Description] 
		,Tbl.Col.value('Name[1]','NVARCHAR(600)') AS [Name]
		,Tbl.Col.value ('PromotionTypeId[1]','NVARCHAR(600)') AS PromotionTypeId
		,Tbl.Col.value ('PromotionTypeName[1]','NVARCHAR(600)') AS PromotionTypeName
		,Tbl.Col.value ('Discount[1]','NVARCHAR(600)') AS Discount
		,Tbl.Col.value ('StartDate[1]','NVARCHAR(600)') AS StartDate
		,Tbl.Col.value ('EndDate[1]','NVARCHAR(600)') AS EndDate
		,Tbl.Col.value ('OrderMinimum[1]','NVARCHAR(600)') AS OrderMinimum
		,Tbl.Col.value ('QuantityMinimum[1]','NVARCHAR(600)') AS QuantityMinimum
		,Tbl.Col.value ('IsCouponRequired[1]','NVARCHAR(600)') AS IsCouponRequired
		,Tbl.Col.value ('DisplayOrder[1]','NVARCHAR(600)') AS DisplayOrder
		,Tbl.Col.value ('IsUnique[1]','NVARCHAR(600)') AS IsUnique
		,Tbl.Col.value ('PortalId[1]','NVARCHAR(600)') AS PortalId
		,Tbl.Col.value ('ProfileId[1]','NVARCHAR(600)') AS ProfileIds
		,Tbl3.Col3.value ('.','NVARCHAR(600)') AS ProfileId
		,Tbl.Col.value ('PromotionProductQuantity[1]','NVARCHAR(600)') AS PromotionProductQuantity
		,Tbl.Col.value ('ReferralPublishProductId[1]','NVARCHAR(600)') AS ReferralPublishProductId
		,Tbl2.Col2.value ('PageIndex[1]','NVARCHAR(600)') AS PageIndex
		,Tbl2.Col2.value ('PageSize[1]','NVARCHAR(600)') AS PageSize
		,Tbl2.Col2.value ('TotalResults[1]','NVARCHAR(600)') AS TotalResults
		,Tbl.Col.value ('PortalAllowsMultipleCoupon[1]','NVARCHAR(600)') AS PortalAllowsMultipleCoupon
		,Tbl.Col.value ('PromotionMessage[1]','NVARCHAR(600)') AS PromotionMessage
		,Tbl.Col.value ('IsAllowedWithOtherCoupons[1]','NVARCHAR(600)') AS IsAllowedWithOtherCoupons
		,Tbl.Col.value ('AssociatedCatelogIds[1]','NVARCHAR(600)') AS AssociatedCatelogIds
		,Tbl.Col.value ('AssociatedCategoryIds[1]','NVARCHAR(600)') AS AssociatedCategoryIds
		,Tbl.Col.value ('AssociatedProductIds[1]','NVARCHAR(600)') AS AssociatedProductIds
		,Tbl.Col.value ('AssociatedBrandIds[1]','NVARCHAR(600)') AS AssociatedBrandIds
		,Tbl.Col.value ('AssociatedShippingIds[1]','NVARCHAR(600)') AS AssociatedShippingIds
		,Tbl.Col.value ('IsAllowWithOtherPromotionsAndCoupons[1]','NVARCHAR(600)') AS IsAllowWithOtherPromotionsAndCoupons
		,Tbl.Col.value ('IsActive[1]','NVARCHAR(600)') AS IsActiveCoupon
		,Tbl.Col.value ('IsUsedInOrder[1]','NVARCHAR(600)') AS IsUsedInOrder
	INTO tempdb..#TEMP
	FROM @PromotionXML.nodes('PromotionModel') AS Tbl(Col)
	CROSS APPLY Tbl.Col.nodes('CouponList') AS Tbl2(Col2)
	CROSS APPLY Tbl.Col.nodes('//ProfileIds/int') AS Tbl3(Col3)
	
	CREATE TABLE #coupnModel (PromotionCouponId NVARCHAR(600), PromotionCouponModelId NVARCHAR(600), Code NVARCHAR(600),InitialQuantity NVARCHAR(600)
	,AvailableQuantity NVARCHAR(600),IsEnableUrl NVARCHAR(600),IsActive NVARCHAR(600),CouponApplied NVARCHAR(600)
	,CouponValid NVARCHAR(600),CustomCouponCode NVARCHAR(600) ,IsCustomCoupon NVARCHAR(600),IsExistInOrder NVARCHAR(600))
	
	IF CAST(@PromotionXML AS nvarchar(max))  like '%<CouponModel>%'
	BEGIN 
	INSERT INTO  #coupnModel 
	SELECT  Tbl1.Col1.value ('PromotionCouponId[1]','NVARCHAR(600)') AS PromotionCouponId
		,Tbl1.Col1.value ('PromotionId[1]','NVARCHAR(600)') AS PromotionCouponModelId
		,Tbl1.Col1.value ('Code[1]','NVARCHAR(600)') AS Code
		,Tbl1.Col1.value ('InitialQuantity[1]','NVARCHAR(600)') AS InitialQuantity
		,Tbl1.Col1.value ('AvailableQuantity[1]','NVARCHAR(600)') AS AvailableQuantity
		,Tbl1.Col1.value ('IsEnableUrl[1]','NVARCHAR(600)') AS IsEnableUrl
		,Tbl1.Col1.value ('IsActive[1]','NVARCHAR(600)') AS IsActive
		,Tbl1.Col1.value ('CouponApplied[1]','NVARCHAR(600)') AS CouponApplied
		,Tbl1.Col1.value ('CouponValid[1]','NVARCHAR(600)') AS CouponValid
		,Tbl1.Col1.value ('CustomCouponCode[1]','NVARCHAR(600)') AS CustomCouponCode
		,Tbl1.Col1.value ('IsCustomCoupon[1]','NVARCHAR(600)') AS IsCustomCoupon
		,Tbl1.Col1.value ('IsExistInOrder[1]','NVARCHAR(600)') AS IsExistInOrder
	--INTO #coupnModel 
	FROM @PromotionXML.nodes('//PromotionModel/CouponList/CouponList/CouponModel') AS Tbl1(Col1)
	
	--SELECT * FROM #coupnModel

	END 
	ELSE
	BEGIN 
		INSERT INTO #coupnModel (PromotionCouponId)
		SELECT 0
	END 
		
	UPDATE #TEMP SET QuantityMinimum = 0 WHERE ISNULL(QuantityMinimum,'') = ''

	DECLARE @SQL VARCHAR(MAX)
	DECLARE @GUID VARCHAR(100) = CAST(NEWID() AS VARCHAR(100))
	DECLARE @TableName VARCHAR(500) = '##Temp_Promotion_'+CAST(@@SPID AS varchar(30))
	DECLARE @LocaleId INT = DBO.Fn_GetDefaultLocaleId();
	DECLARE @PromotionTypeId INT = (SELECT TOP 1 PromotionTypeId FROM tempdb..#TEMP);
	
    CREATE TABLE #Temp_Promotion_ (PromoCode NVARCHAR(500),Name NVARCHAR(500),Description NVARCHAR(500),StartDate NVARCHAR(500),
		EndDate NVARCHAR(500),DisplayOrder NVARCHAR(500),PortalId NVARCHAR(500),ProfileId NVARCHAR(500),IsCouponRequired NVARCHAR(500),
		IsAllowedWithOtherCoupons NVARCHAR(500),PromotionMessage NVARCHAR(500),Code NVARCHAR(500),AvailableQuantity NVARCHAR(500),
		Brand NVARCHAR(500),CallForPriceMessage NVARCHAR(500),Catalog NVARCHAR(500),Category NVARCHAR(500),DiscountAmount NVARCHAR(500),
		MinimumOrderAmount NVARCHAR(500),MinimumQuantity NVARCHAR(500),ProductQuantity NVARCHAR(500),ProductToDiscount NVARCHAR(500),
		RequiredProduct NVARCHAR(500),Shipping NVARCHAR(500), InitialQuantity NVARCHAR(500),IsUnique NVARCHAR(500) )

	INSERT INTO #Temp_Promotion_
	SELECT DISTINCT PromoCode,Name,ISNULL(Description,''),StartDate,EndDate,DisplayOrder,IIF(PortalId='0' OR PortalId ='' , NULL,PortalId),
		IIF(ProfileId='0' OR ProfileId ='' , NULL,ProfileId) ProfileId,IsCouponRequired,IsAllowedWithOtherCoupons,PromotionMessage,Code,
		AvailableQuantity,AssociatedBrandIds,'',AssociatedCatelogIds,AssociatedCategoryIds,Discount,OrderMinimum,QuantityMinimum,
		PromotionProductQuantity,AssociatedProductIds,ReferralPublishProductId,AssociatedShippingIds,InitialQuantity,IsUnique 
	FROM tempdb..#TEMP 
	CROSS APPLY #coupnModel

	UPDATE ZP SET ZP.PromoCode=T.PromoCode
	FROM ZnodePromotion ZP
	INNER JOIN tempdb..#TEMP T ON ZP.PromotionId=CAST(T.PromotionId As INT)
	WHERE (CASE WHEN T.PromotionId IS NULL OR  T.PromotionId ='' THEN '0' ELSE CAST(T.PromotionId AS INT) END) <>0
	
	EXEC dbo.Znode_ImportPromotions @TableName='#Temp_Promotion_',@Status=0,@UserId=1,@ImportProcessLogId=0,@NewGUId=@GUID,@LocaleId=@LocaleId,
		@CsvColumnString='PromoCode,Name,Description,StartDate,EndDate,DisplayOrder,PortalId,ProfileId,IsCouponRequired,IsAllowedWithOtherCoupons,PromotionMessage,Code,AvailableQuantity,Brand,CallForPriceMessage,Catalog,Category,DiscountAmount,MinimumOrderAmount,MinimumQuantity,ProductQuantity,ProductToDiscount,RequiredProduct,Shipping,InitialQuantity,IsUnique'
		,@PromotionTypeId=@PromotionTypeId ,@IsGenerateErrorLog =0 

	SET @Status = 1
	
	SELECT * 
	FROM ZnodePromotion a 
	WHERE PromoCode = (SELECT TOP 1 PromoCode FROM #Temp_Promotion_ )

	END TRY
              			 
	BEGIN CATCH
		SELECT ERROR_MESSAGE() 
		
		SET @Status = 0
		
		SELECT 1 ID , CAST(0 AS BIT ) Status
		
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'Znode_InsertUpdatePromotions	
					@PromotionXML='+CAST(@PromotionXML AS VARCHAR(MAX))+',
					@UserId='+CAST(@UserId AS VARCHAR(10))+',
					@Status='+CAST(@Status AS VARCHAR(10));
		
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_InsertUpdatePromotions',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH
END