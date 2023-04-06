CREATE PROCEDURE [dbo].[Znode_ImportPromotions]
(
	@TableName			NVARCHAR(100),
	@Status				BIT,
	@UserId				INT,
	@ImportProcessLogId	INT,
	@NewGUId			NVARCHAR(200),
	@LocaleId			INT=1,
	@CsvColumnString	NVARCHAR(MAX),
	@PromotionTypeId	INT , 
	@IsGenerateErrorLog	BIT = 1
)
AS 
/*
	Summary :  Made a provision to import Promotions details.
	
	Unit Testing: 
		EXEC dbo.Znode_ImportPromotions @TableName='',@Status=0,@UserId=2,@ImportProcessLogId=1,@NewGUId='',@LocaleId=1,
			@CsvColumnString=,@PromotionTypeId=17

	SELECT @TableName='##Temp',@Status=0,@UserId=2,@ImportProcessLogId=1,@NewGUId=NEWID(),@LocaleId=1,
			@CsvColumnString='PromoCode,Name,Description,StartDate,EndDate,DisplayOrder,Store,Profile,IsCouponRequired,IsAllowedWithOtherCoupons,PromotionMessage,Code,AvailableQuantity
				,DiscountAmount,MinimumQuantity,MinimumOrderAmount,Brand',
			@PromotionTypeId=17
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @MessageDisplay NVARCHAR(100), @SSQL NVARCHAR(MAX);

	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();

	DECLARE @GetDate1 VARCHAR(100) = CONVERT(VARCHAR(30),@GetDate,121)
	DECLARE @ImportProcessLogId1 VARCHAR(100) = CAST(@ImportProcessLogId AS VARCHAR(15))
	DECLARE @UserId1 VARCHAR(15) = CAST(@UserId AS VARCHAR(15))

	IF ISNULL(@LocaleId,0)=0
	BEGIN
		SELECT @LocaleId = DBO.Fn_GetDefaultLocaleId();
	END

	DECLARE @LocaleId1 VARCHAR(15) = CAST(@LocaleId AS VARCHAR(15))

	IF OBJECT_ID('tempdb..##Promotions') IS NOT NULL
		DROP TABLE ##Promotions;

	SET @SSQL='CREATE TABLE ##Promotions (RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT , '+
		REPLACE(TRIM(LTRIM(@CsvColumnString)), ',' ,' NVARCHAR(MAX),') + CASE WHEN @CsvColumnString = '' THEN '' ELSE ' NVARCHAR(MAX)' END+', GUID NVARCHAR(400)) 
		INSERT INTO ##Promotions (RowNumber,' + @CsvColumnString + ',GUID) 
		SELECT ROW_NUMBER() OVER (ORDER BY PromoCode) As RowNumber,' + @CsvColumnString + ','''+@NewGUId+''' As GUID FROM '+ @TableName +'
		'
		
	EXEC (@SSQL)
	IF NOT EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='PortalId') AND @IsGenerateErrorLog = 1 
	BEGIN
		ALTER TABLE ##Promotions ADD PortalId INT

		UPDATE P
		SET P.PortalId = CASE WHEN LTRIM(TRIM(P.Store))='All Stores' THEN NULL ELSE ISNULL(ZP.PortalId,0) END
		FROM ##Promotions P
		LEFT JOIN ZnodePortal ZP ON P.Store=ZP.StoreCode
	END

	IF NOT EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='ProfileId') AND @IsGenerateErrorLog = 1 
	BEGIN
		ALTER TABLE ##Promotions ADD ProfileId INT

		UPDATE P
		SET P.ProfileId = CASE WHEN LTRIM(TRIM(P.Profile))='All Profiles' THEN NULL ELSE ISNULL(ZP.ProfileId,0) END
		FROM ##Promotions P
		LEFT JOIN ZnodeProfile ZP ON P.Profile=ZP.DefaultExternalAccountNo
	END

	IF NOT EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='PromotionTypeId')
	BEGIN
		ALTER TABLE ##Promotions ADD PromotionTypeId INT
		UPDATE ##Promotions
		SET PromotionTypeId = @PromotionTypeId
	END

	IF NOT EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='IsUnique')
	BEGIN
		ALTER TABLE ##Promotions ADD IsUnique BIT
		UPDATE ##Promotions
		SET IsUnique = 0
	END

	IF NOT EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='InitialQuantity')
	BEGIN
		ALTER TABLE ##Promotions ADD InitialQuantity FLOAT
		UPDATE ##Promotions
		SET InitialQuantity = AvailableQuantity
	END

	UPDATE ##Promotions
	SET IsCouponRequired=0
	WHERE ISNULL(IsCouponRequired,'')='' OR ISNULL(IsCouponRequired,'')='No';

	UPDATE ##Promotions
	SET IsAllowedWithOtherCoupons=0
	WHERE ISNULL(IsAllowedWithOtherCoupons,'')='' OR ISNULL(IsAllowedWithOtherCoupons,'')='No';

	UPDATE ##Promotions
	SET IsCouponRequired=1
	WHERE ISNULL(IsCouponRequired,'')='Yes';

	UPDATE ##Promotions
	SET IsAllowedWithOtherCoupons=1
	WHERE ISNULL(IsAllowedWithOtherCoupons,'')='Yes';

	-- Code for not consider Discount Information for specific Promotion Type
	DECLARE @PromotionTypeName NVARCHAR(50);

	SELECT TOP 1 @PromotionTypeName = [Name] FROM ZnodePromotionType WHERE PromotionTypeId = @PromotionTypeId;

	IF @PromotionTypeName IN ('Amount Off Displayed Product Price','Percent Off Displayed Product Price')
	BEGIN
		UPDATE ##Promotions
		SET IsAllowedWithOtherCoupons=0, IsCouponRequired=0, PromotionMessage='';
	END
	ELSE IF @PromotionTypeName IN ('Call For Pricing')
	BEGIN
		SET @SSQL='
		UPDATE ##Promotions
		SET IsAllowedWithOtherCoupons=0, IsCouponRequired=0, 
			PromotionMessage = '+CASE WHEN @IsGenerateErrorLog = 1 THEN ' CallForPriceMessage ' ELSE 'PromotionMessage' END;
		
		EXEC (@SSQL);
	END
	
	BEGIN TRAN Promotions;
	BEGIN TRY
		-- Start Functional Validation
		-----------------------------------------------
		IF @IsGenerateErrorLog = 1 
		BEGIN
			INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'PromoCode', PromoCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.PromoCode,'')=''

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '82', 'PromoCode', PromoCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE LEN(LTRIM(RTRIM(ii.PromoCode))) > 300 AND ISNULL(ii.PromoCode,'')<>''

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'Name', [Name], @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.[Name],'')=''
				AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '78', 'Name', [Name], @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE LEN(LTRIM(RTRIM(ii.[Name]))) > 100 AND ISNULL(ii.[Name],'')<>''

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '78', 'Description', Description, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE LEN(LTRIM(RTRIM(ii.Description))) > 100 AND ISNULL(ii.Description,'')<>''

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '5', 'StartDate', StartDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISDATE(ii.StartDate)=0 AND LEN(ii.StartDate)>0

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '150', 'StartDate', StartDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE (ISDATE(ii.StartDate)=1 AND CONVERT(DATETIME,ii.StartDate,111)<CONVERT(DATETIME,CONVERT(VARCHAR(10),@GetDate,111),111))

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '151', 'StartDate', StartDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE (ISDATE(ii.StartDate)=1 AND ISDATE(ii.EndDate)=1 AND CONVERT(DATETIME,ii.StartDate,111)>CONVERT(DATETIME,ii.EndDate,111))

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'StartDate', StartDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.StartDate,'')=''
				AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '5', 'EndDate', EndDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISDATE(ii.EndDate)=0 AND LEN(ii.EndDate)>0

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '152', 'EndDate', EndDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE (ISDATE(ii.StartDate)=1 AND ISDATE(ii.EndDate)=1 AND CONVERT(DATETIME,ii.StartDate,111)>CONVERT(DATETIME,ii.EndDate,111))

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'EndDate', EndDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.EndDate,'')=''
				AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
		
			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE (ISNULL(ii.DisplayOrder,0)=0 OR ISNULL(ii.DisplayOrder,'')='')
				AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '149', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNUMERIC(ii.DisplayOrder)=0 AND LEN(ii.DisplayOrder)>0

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '115', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE (ISNUMERIC(ii.DisplayOrder)=1 AND (CAST(ii.DisplayOrder As INT))>999)

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '120', 'Store', Store, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.Store,'')<>''
				AND NOT EXISTS (SELECT * FROM ZnodePortal WHERE PortalId=ii.PortalId)
				AND ISNULL(ii.Store,'')<>'All Stores'

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'Store', Store, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.Store,'')=''
				AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '157', 'Store', Store, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.Store,'')<>''
				AND EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode AND ISNULL(PortalId,0)<>ISNULL(ii.PortalId,0))

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '153', 'Profile', [Profile], @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.Profile,'')<>''
				AND (NOT EXISTS (SELECT * FROM ZnodePortalProfile WHERE ProfileId=ii.ProfileId AND PortalId=ii.PortalId)
					AND ISNULL(ii.Store,'')<>'All Stores' AND ISNULL(ii.Profile,'')<>'All Profiles')
				OR (NOT EXISTS (SELECT * FROM ZnodeProfile WHERE DefaultExternalAccountNo=ii.Profile)
					AND ISNULL(ii.Profile,'')<>'All Profiles' AND ISNULL(ii.Profile,'')<>'')

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '78', 'Profile', [Profile], @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE LEN(LTRIM(RTRIM(ii.Profile))) > 100 AND ISNULL(ii.Profile,'')<>''

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
			SELECT '8', 'Profile', [Profile], @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			FROM ##Promotions AS ii
			WHERE ISNULL(ii.Profile,'')=''
				AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)

			IF @PromotionTypeName NOT IN ('Amount Off Displayed Product Price','Percent Off Displayed Product Price','Call For Pricing')
			BEGIN
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '142', 'IsCouponRequired', IsCouponRequired, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.IsCouponRequired,'') NOT IN ('True','1','Yes','FALSE','0','No')
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '142', 'IsAllowedWithOtherCoupons', IsAllowedWithOtherCoupons, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.IsAllowedWithOtherCoupons,'') NOT IN ('True','1','Yes','FALSE','0','No')
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '154', 'IsAllowedWithOtherCoupons', IsAllowedWithOtherCoupons, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.IsAllowedWithOtherCoupons,'') IN ('True','1','Yes')
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') NOT IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '142', 'PromotionMessage', PromotionMessage, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.PromotionMessage,'')<>''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') NOT IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '82', 'PromotionMessage', PromotionMessage, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE (ISNULL(ii.PromotionMessage,'')<>'' AND LEN(LTRIM(RTRIM(ii.PromotionMessage)))>300)
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '138', 'Code', Code, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.Code,'')<>''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')
					AND EXISTS (SELECT * FROM ZnodePromotionCoupon WHERE Code=ii.Code)

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '138', 'Code', Code, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.Code,'')<>''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')
					AND NOT EXISTS (SELECT * FROM ##Promotions L
									WHERE RowID IN (SELECT MAX(RowId) FROM ##Promotions X WHERE X.Code= L.Code GROUP BY X.Code)
										AND ii.Code= L.Code and ii.PromoCode = L.PromoCode 
									)

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '139', 'Code', Code, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE (ISNULL(ii.Code,'')<>'' AND LEN(LTRIM(RTRIM(ii.Code)))>20)
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '8', 'Code', Code, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.Code,'')=''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '108', 'AvailableQuantity', AvailableQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.AvailableQuantity)=0)
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '141', 'AvailableQuantity', AvailableQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.AvailableQuantity)=1 AND (ii.AvailableQuantity) LIKE '%.%')
					--(ISNUMERIC(ii.AvailableQuantity)=1 AND LEN(PARSENAME(ii.AvailableQuantity,1))>1)
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '140', 'AvailableQuantity', AvailableQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.AvailableQuantity)=1 AND (ISNULL(CAST(ii.AvailableQuantity AS FLOAT),0)<0 OR ISNULL(CAST(ii.AvailableQuantity AS FLOAT),0)>9999))
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
					AND ISNULL(ii.IsCouponRequired,'') IN ('True','1','Yes')
			END
		END 

		IF @IsGenerateErrorLog = 1 
		BEGIN
			DECLARE @Query NVARCHAR(MAX);

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='DiscountAmount')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''109'', ''DiscountAmount'', DiscountAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ((ISNUMERIC(ii.DiscountAmount)=0 AND LEN(ii.DiscountAmount)>0) OR CAST(ii.DiscountAmount As FLOAT)<0)

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''137'', ''DiscountAmount'', DiscountAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.DiscountAmount)=1 AND (LEN(PARSENAME(ii.DiscountAmount,1))>2 AND (ii.DiscountAmount) LIKE ''%.%'' AND CAST(ii.DiscountAmount As FLOAT)>0))
		
				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''136'', ''DiscountAmount'', DiscountAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.DiscountAmount)=1 AND (CAST(ii.DiscountAmount As FLOAT))>100)
					AND '''+@PromotionTypeName+''' LIKE ''%Percent%''

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''161'', ''DiscountAmount'', DiscountAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.DiscountAmount)=1 AND ((CAST(ii.DiscountAmount As FLOAT))>9999999 OR (CAST(ii.DiscountAmount As FLOAT))<0.01))
					AND '''+@PromotionTypeName+''' NOT LIKE ''%Percent%''

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''8'', ''DiscountAmount'', DiscountAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.DiscountAmount,'''')=''''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='MinimumQuantity')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''108'', ''MinimumQuantity'', MinimumQuantity, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.MinimumQuantity)=0 AND LEN(ii.MinimumQuantity)>0) OR CAST(ii.MinimumQuantity As FLOAT)<0

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''141'', ''MinimumQuantity'', MinimumQuantity, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.MinimumQuantity)=1 AND (ii.MinimumQuantity) LIKE ''%.%'')

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''136'', ''MinimumQuantity'', MinimumQuantity, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.MinimumQuantity)=1 AND (CAST(ii.MinimumQuantity As FLOAT))>100)

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''8'', ''MinimumQuantity'', MinimumQuantity, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.MinimumQuantity,'''')=''''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='MinimumOrderAmount')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''109'', ''MinimumOrderAmount'', MinimumOrderAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ((ISNUMERIC(ii.MinimumOrderAmount)=0 AND LEN(ii.MinimumOrderAmount)>0) OR CAST(ii.MinimumOrderAmount As FLOAT)<0)

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''137'', ''MinimumOrderAmount'', MinimumOrderAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.MinimumOrderAmount)=1 AND (LEN(PARSENAME(ii.MinimumOrderAmount,1))>2 AND (ii.MinimumOrderAmount) LIKE ''%.%'' AND CAST(ii.MinimumOrderAmount As FLOAT)>0))

				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''136'', ''MinimumOrderAmount'', MinimumOrderAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE (ISNUMERIC(ii.MinimumOrderAmount)=1 AND (CAST(ii.MinimumOrderAmount As FLOAT))>100)
		
				INSERT INTO ZnodeImportLog (ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''8'', ''MinimumOrderAmount'', MinimumOrderAmount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ISNULL(ii.MinimumOrderAmount,'''')=''''
					AND NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode=ii.PromoCode)
				'
				EXEC SP_EXECUTESQL @Query;
			END
		
			IF EXISTS (SELECT TOP 1 1 FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='CallForPriceMessage')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''CallForPriceMessage'', CallForPriceMessage, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.CallForPriceMessage))) > 100
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Brand')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''143'', ''Brand'', Brand, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE NOT EXISTS (SELECT * FROM ZnodeBrandDetails BD INNER JOIN ZnodePortalBrand PB ON BD.BrandId = PB.BrandId AND PB.PortalId = ii.PortalId WHERE BD.BrandCode=ii.Brand)
					AND ISNULL(ii.Brand,'''')<>''''
					AND ISNULL(ii.Store,'''')<>''All Stores''
					OR (NOT EXISTS (SELECT * FROM ZnodeBrandDetails BD WHERE BD.BrandCode=ii.Brand) AND ISNULL(ii.Brand,'''')<>'''')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''Brand'', Brand, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.Brand))) > 100
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Catalog')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''144'', ''Catalog'', Catalog, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE NOT EXISTS (	SELECT * FROM ZnodePimCatalog PC
									INNER JOIN ZnodePimCatalog PbC ON PC.PimCatalogId=PbC.PimCatalogId
									INNER JOIN ZnodePortalCatalog PtC ON PbC.pimCatalogId=Ptc.PublishCatalogId AND PtC.PortalId = ii.PortalId
									WHERE PC.CatalogCode = ii.Catalog)
					AND ISNULL(ii.Catalog,'''')<>''''
					AND ISNULL(ii.Store,'''')<>''All Stores''
					OR (NOT EXISTS (SELECT * FROM ZnodePimCatalog PC WHERE PC.CatalogCode = ii.Catalog) AND ISNULL(ii.Catalog,'''')<>'''')

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''Catalog'', Catalog, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.Catalog))) > 100
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Category')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''145'', ''Category'', Category, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE NOT EXISTS (	SELECT * FROM ZnodePimCategory PC
									INNER JOIN ZnodePimCategoryAttributeValue PCAV ON PC.PimCategoryId=PCAV.PimCategoryId
									INNER JOIN ZnodePimCategoryAttributeValueLocale PCAVL ON PCAV.PimCategoryAttributeValueId=PCAVL.PimCategoryAttributeValueId
									INNER JOIN ZnodePimAttribute PA ON PCAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''CategoryCode''
									INNER JOIN ZnodePimCategoryHierarchy PCH ON PC.PimCategoryId=PCH.PimCategoryId
									INNER JOIN ZnodePimCatalog PCt ON PCH.PimCatalogId=PCt.PimCatalogId
									INNER JOIN ZnodePortalCatalog PtC ON PCt.PImCatalogId=Ptc.PublishCatalogId AND PtC.PortalId = ii.PortalId
									WHERE LocaleId='+@LocaleId1+' AND PCAVL.CategoryValue=ii.Category)
					AND ISNULL(ii.Category,'''')<>''''
					AND ISNULL(ii.Store,'''')<>''All Stores''
					OR (NOT EXISTS (SELECT * FROM ZnodePimCategoryAttributeValueLocale PCAVL WHERE PCAVL.CategoryValue=ii.Category) AND ISNULL(ii.Category,'''')<>'''')
				
				
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''Category'', Category, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.Category))) > 100
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Shipping')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''148'', ''Shipping'', Shipping, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE NOT EXISTS (	SELECT * FROM ZnodeShipping SP
									INNER JOIN ZnodePortalShipping PS ON SP.ShippingId=PS.ShippingId
									WHERE SP.ShippingCode=ii.Shipping AND PS.PortalId=ii.PortalId)
					AND ISNULL(ii.Shipping,'''')<>''''
					AND ISNULL(ii.Store,'''')<>''All Stores''
					OR (NOT EXISTS (SELECT * FROM ZnodeShipping SP WHERE SP.ShippingCode=ii.Shipping) AND ISNULL(ii.Shipping,'''')<>'''')
				
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''Shipping'', Shipping, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.Shipping))) > 100
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='ProductToDiscount')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''156'', ''ProductToDiscount'', ProductToDiscount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE NOT EXISTS (	SELECT * FROM ZnodePimAttributeValueLocale PAVL
									INNER JOIN ZnodePimAttributeValue PAV ON PAVL.PimAttributeValueId=PAV.PimAttributeValueId
									INNER JOIN ZnodePimAttribute PA ON PAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''SKU''
									INNER JOIN ZnodePimCategoryProduct PCP ON PAV.PimProductId=PCP.PimProductId
									INNER JOIN ZnodePimCategoryHierarchy PCH ON PCP.PimCategoryId=PCH.PimCategoryId
									INNER JOIN ZnodePimCatalog PC ON PCH.PimCatalogId=PC.PimCatalogId
									WHERE LocaleId='+@LocaleId1+' AND PAVL.AttributeValue=ii.ProductToDiscount AND PC.PortalId=ii.PortalId)
					AND ISNULL(ii.ProductToDiscount,'''')<>''''
					AND ISNULL(ii.Store,'''')<>''All Stores''
					OR (NOT EXISTS (SELECT * FROM ZnodePimAttributeValueLocale PAVL WHERE PAVL.AttributeValue=ii.ProductToDiscount) AND ISNULL(ii.ProductToDiscount,'''')<>'''')
				
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''ProductToDiscount'', ProductToDiscount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.ProductToDiscount))) > 100
			
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''147'', ''ProductToDiscount'', ProductToDiscount, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ((LTRIM(RTRIM(ii.ProductToDiscount))) LIKE ''%,%'' OR (LTRIM(RTRIM(ii.ProductToDiscount))) LIKE ''% %'')
					AND ISNULL(ii.ProductToDiscount,'''')<>''''
				'
				EXEC SP_EXECUTESQL @Query;
			END

			IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='RequiredProduct')
			BEGIN
				SET @Query='
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''156'', ''RequiredProduct'', RequiredProduct, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE NOT EXISTS (	SELECT * FROM ZnodePimAttributeValueLocale PAVL
									INNER JOIN ZnodePimAttributeValue PAV ON PAVL.PimAttributeValueId=PAV.PimAttributeValueId
									INNER JOIN ZnodePimAttribute PA ON PAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''SKU''
									INNER JOIN ZnodePimCategoryProduct PCP ON PAV.PimProductId=PCP.PimProductId
									INNER JOIN ZnodePimCategoryHierarchy PCH ON PCP.PimCategoryId=PCH.PimCategoryId
									INNER JOIN ZnodePimCatalog PC ON PCH.PimCatalogId=PC.PimCatalogId
									WHERE LocaleId='+@LocaleId1+' AND PAVL.AttributeValue=ii.RequiredProduct AND PC.PortalId=ii.PortalId)
					AND ISNULL(ii.RequiredProduct,'''')<>''''
					AND ISNULL(ii.Store,'''')<>''All Stores''
					OR (NOT EXISTS (SELECT * FROM ZnodePimAttributeValueLocale PAVL WHERE PAVL.AttributeValue=ii.RequiredProduct) AND ISNULL(ii.RequiredProduct,'''')<>'''')
				
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''78'', ''RequiredProduct'', RequiredProduct, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE LEN(LTRIM(RTRIM(ii.RequiredProduct))) > 100
			
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT ''147'', ''RequiredProduct'', RequiredProduct, '''+@NewGUId+''', RowNumber, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+''', '+@ImportProcessLogId1+'
				FROM ##Promotions AS ii
				WHERE ((LTRIM(RTRIM(ii.RequiredProduct))) LIKE ''%,%'' OR (LTRIM(RTRIM(ii.RequiredProduct))) LIKE ''% %'')
					AND ISNULL(ii.RequiredProduct,'''')<>''''
				'
				EXEC SP_EXECUTESQL @Query;
			END

			UPDATE ZIL
			SET ZIL.ColumnName =   ZIL.ColumnName --+ ' [ Promotion - ' + ISNULL(PromoCode,'') + ' ] '
			FROM ZnodeImportLog ZIL 
			INNER JOIN ##Promotions IPA ON (ZIL.RowNumber = IPA.RowNumber)
			WHERE ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

			--Delete Invalid Data after functional validation
			DELETE FROM ##Promotions
			WHERE RowNumber IN
			(
				SELECT DISTINCT RowNumber
				FROM ZnodeImportLog
				WHERE ImportProcessLogId = @ImportProcessLogId AND RowNumber IS NOT NULL
			);

			-- Update Record count in log
			DECLARE @FailedRecordCount BIGINT;
			DECLARE @SuccessRecordCount BIGINT;

			SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber)
			FROM ZnodeImportLog 
			WHERE RowNumber IS NOT NULL AND ImportProcessLogId = @ImportProcessLogId;

			SELECT @SuccessRecordCount = COUNT(DISTINCT RowNumber) 
			FROM ##Promotions

			UPDATE ZnodeImportProcessLog
			SET FailedRecordcount = @FailedRecordCount ,
				SuccessRecordCount = @SuccessRecordCount ,
				TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
			WHERE ImportProcessLogId = @ImportProcessLogId;
		END

		--To identify duplicate records
		IF OBJECT_ID('tempdb..#DuplicatePromotionsData') IS NOT NULL
			DROP TABLE #DuplicatePromotionsData;

		SELECT PromoCode,RowId,ROW_NUMBER() OVER (PARTITION BY PromoCode ORDER BY RowId DESC) As Rn
		INTO #DuplicatePromotionsData
		FROM ##Promotions;

		IF EXISTS(SELECT * FROM tempdb.sys.columns WHERE [name] = N'DiscountAmount' AND [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
		BEGIN
			SET @Query='
			EXEC tempdb.sys.sp_rename ''tempdb.dbo.##Promotions.DiscountAmount'', ''Discount'' , ''COLUMN''
			'
			EXEC SP_EXECUTESQL @Query;
			PRINT @Query
		END
		
		IF EXISTS(SELECT * FROM tempdb.sys.columns WHERE [name] = N'MinimumQuantity' AND [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
		BEGIN
			SET @Query='
			EXEC tempdb.sys.sp_rename ''tempdb.dbo.##Promotions.MinimumQuantity'', ''QuantityMinimum'' , ''COLUMN''
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS(SELECT * FROM tempdb.sys.columns WHERE [name] = N'MinimumOrderAmount' AND [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
		BEGIN
			SET @Query='
			EXEC tempdb.sys.sp_rename ''tempdb.dbo.##Promotions.MinimumOrderAmount'', ''OrderMinimum'' , ''COLUMN''
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS(SELECT * FROM tempdb.sys.columns WHERE [name] = N'ProductQuantity' AND [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
		BEGIN
			SET @Query='
			EXEC tempdb.sys.sp_rename ''tempdb.dbo.##Promotions.ProductQuantity'', ''PromotionProductQuantity'' , ''COLUMN''
			'
			EXEC SP_EXECUTESQL @Query;
		END

		--For Update Promotions Data
		DECLARE @UpdateColumns NVARCHAR (MAX);

		IF @IsGenerateErrorLog = 1 
		BEGIN
			SELECT @UpdateColumns=STRING_AGG(('ZP.'+COLUMN_NAME+'=CASE WHEN ISNULL(P.'+COLUMN_NAME+','''')='''' THEN ZP.'+COLUMN_NAME+' ELSE P.'+COLUMN_NAME+' END'),',')
			--'ZP.'+COLUMN_NAME+'=P.'+COLUMN_NAME
			FROM INFORMATION_SCHEMA.COLUMNS
			WHERE TABLE_NAME='ZnodePromotion'
				AND COLUMN_NAME IN (SELECT name FROM tempdb.sys.columns WHERE [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
				AND COLUMN_NAME NOT IN ('PromoCode','PromotionTypeId','IsCouponRequired','IsAllowedWithOtherCoupons','IsUnique','ProfileId','PortalId')
				--AND COLUMN_NAME NOT IN ('DiscountAmount','MinimumQuantity','MinimumOrderAmount');
         END
		 ELSE
		 BEGIN 
			SELECT @UpdateColumns=STRING_AGG(('ZP.'+COLUMN_NAME+'=CASE WHEN ISNULL(P.'+COLUMN_NAME+','''')='''' THEN P.'+COLUMN_NAME+' ELSE P.'+COLUMN_NAME+' END'),',')
			--'ZP.'+COLUMN_NAME+'=P.'+COLUMN_NAME
			FROM INFORMATION_SCHEMA.COLUMNS
			WHERE TABLE_NAME='ZnodePromotion'
				AND COLUMN_NAME IN (SELECT name FROM tempdb.sys.columns WHERE [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
				AND COLUMN_NAME NOT IN ('PromoCode','PromotionTypeId','IsUnique','ProfileId')
				-- AND COLUMN_NAME NOT IN ('DiscountAmount','MinimumQuantity','MinimumOrderAmount');
		END

		IF OBJECT_ID('tempdb..#UpdatedPromotions') IS NOT NULL
			DROP TABLE #UpdatedPromotions;

		CREATE TABLE #UpdatedPromotions (PromotionId INT,PromoCode VARCHAR(300),PromotionTypeId INT);

		SET @Query='
		UPDATE ZP SET '+@UpdateColumns+'
		OUTPUT INSERTED.PromotionId, INSERTED.PromoCode, INSERTED.PromotionTypeId
		INTO #UpdatedPromotions (PromotionId, PromoCode, PromotionTypeId)
		FROM ##Promotions P
		INNER JOIN ZnodePromotion ZP ON LTRIM(RTRIM(P.PromoCode)) = ZP.PromoCode
		INNER JOIN #DuplicatePromotionsData DP ON LTRIM(RTRIM(P.PromoCode)) = DP.PromoCode AND DP.Rn=1 AND P.RowId=DP.RowId
		'
		EXEC SP_EXECUTESQL @Query;

		--For Insert Promotions Data
		DECLARE @MatchesColumns NVARCHAR (MAX);

		SELECT @MatchesColumns=STRING_AGG(COLUMN_NAME,',') FROM INFORMATION_SCHEMA.COLUMNS 
		WHERE TABLE_NAME='ZnodePromotion'
			AND COLUMN_NAME IN (SELECT name FROM tempdb.sys.columns WHERE [object_id] = OBJECT_ID(N'tempdb.dbo.##Promotions'))
			AND COLUMN_NAME NOT IN ('ProfileId');

		IF OBJECT_ID('tempdb..#InsertedPromotions') IS NOT NULL
			DROP TABLE #InsertedPromotions;

		CREATE TABLE #InsertedPromotions (PromotionId INT,PromoCode VARCHAR(300),PromotionTypeId INT);

		-- Import New Promotions
		SET @MatchesColumns='
		INSERT INTO ZnodePromotion
			('+@MatchesColumns+',CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
			)
		OUTPUT INSERTED.PromotionId, INSERTED.PromoCode, INSERTED.PromotionTypeId
		INTO #InsertedPromotions (PromotionId, PromoCode, PromotionTypeId)
		SELECT DISTINCT P.'+@MatchesColumns+','+CAST(@UserId AS VARCHAR(15))+','''+CONVERT(NVARCHAR(30),@GetDate,121)+''','+CAST(@UserId AS VARCHAR(15))+','''+CONVERT(NVARCHAR(30),@GetDate,121)+'''
		FROM ##Promotions P
		INNER JOIN #DuplicatePromotionsData DP ON LTRIM(RTRIM(P.PromoCode)) = DP.PromoCode AND DP.Rn=1 AND P.RowId=DP.RowId
		WHERE NOT EXISTS (SELECT * FROM ZnodePromotion WHERE PromoCode = LTRIM(RTRIM(P.PromoCode)))
		'

		EXEC SP_EXECUTESQL @MatchesColumns;

		DECLARE @checkTable TABLE (Code VARCHAR(1000) , valued NVARCHAR(max))

		DECLARE @InsertData int = 1 
		IF @IsGenerateErrorLog = 0  
		BEGIN 
		    
			SET @InsertData= CASE WHEN NOT EXISTS (SELECT TOP 1 1  FROM #UpdatedPromotions)  THEN 1 ELSE 0 END 
 
			INSERT INTO #InsertedPromotions (PromotionId,PromoCode,PromotionTypeId)
			SELECT PromotionId,PromoCode,PromotionTypeId
			FROM #UpdatedPromotions a 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM #InsertedPromotions b WHERE b.PromoCode = a.PromoCode)

			DELETE FROM ZnodePromotionProfileMapper WHERE PromotionId IN (SELECT PromotionId FROM #InsertedPromotions )

			DELETE FROM ZnodePromotionCoupon WHERE  PromotionId IN (SELECT PromotionId FROM #InsertedPromotions )

			IF @PromotionTypeName IN ('Percent Off X If Y Purchased','Amount Off X If Y Purchased')
			BEGIN
				DELETE FROM ZnodePromotionProduct
					WHERE PromotionId IN (SELECT PromotionId FROM #InsertedPromotions)
						AND (SELECT TOP 1 ProductToDiscount FROM ##Promotions) <> ''
			
				DELETE FROM ZnodePromotionBrand WHERE  PromotionId IN (SELECT PromotionId FROM #InsertedPromotions ) 
				AND (SELECT TOP 1 brand FROM ##Promotions ) <> ''

				DELETE FROM ZnodePromotionCategory WHERE  PromotionId IN (SELECT PromotionId FROM #InsertedPromotions ) 
				AND (SELECT TOP 1 category FROM ##Promotions ) <> ''

				DELETE FROM ZnodePromotionCatalogs WHERE  PromotionId IN (SELECT PromotionId FROM #InsertedPromotions ) 
				AND (SELECT TOP 1 catalog FROM ##Promotions ) <> ''
				DELETE FROM ZnodePromotionShipping WHERE  PromotionId IN (SELECT PromotionId FROM #InsertedPromotions ) 
				AND (SELECT TOP 1 Shipping FROM ##Promotions ) <> ''

				SET @InsertData = 1 
			END

			INSERT INTO @checkTable 
			SELECT 'brand' ,(SELECT TOP 1 brand FROM ##Promotions )
			UNION ALL 
			SELECT 'catalog' ,(SELECT TOP 1 catalog FROM ##Promotions )
			UNION ALL 
			SELECT 'category' ,(SELECT TOP 1 category FROM ##Promotions )
			UNION ALL 
			SELECT 'Shipping' ,(SELECT TOP 1 Shipping FROM ##Promotions )
			UNION ALL
			SELECT 'ProductToDiscount' ,(SELECT TOP 1 ProductToDiscount FROM ##Promotions )
			UNION ALL
			SELECT 'RequiredProduct' ,(SELECT TOP 1 RequiredProduct FROM ##Promotions )
		END 
		
		--SELECT 1 
		-- Insert for ZnodePromotionProfileMapper
		--SET @Query='
		--INSERT INTO ZnodePromotionProfileMapper
		--	(PromotionId,ProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		--SELECT DISTINCT IPs.PromotionId, CASE WHEN ISNULL(p.Profile,'''') <> '''' THEN IIF(p.Profile=0,NULL,p.Profile) ELSE    P.Profileid END ,'+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
		--FROM #InsertedPromotions IPs 
		--INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
		--WHERE NOT EXISTS (SELECT * FROM ZnodePromotionProfileMapper WHERE Promotionid = IPs.PromotionId AND ISNULL(Profileid,0)=CASE WHEN ISNULL(p.Profile,'''') <> '''' THEN p.Profile ELSE    P.Profileid END )  
		--'
		--PRINT @Query 
		--EXEC SP_EXECUTESQL @Query;

		SET @Query='
		INSERT INTO ZnodePromotionProfileMapper
			(PromotionId,ProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT DISTINCT IPs.PromotionId, P.ProfileId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
		FROM #InsertedPromotions IPs 
		INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
		WHERE NOT EXISTS (SELECT * FROM ZnodePromotionProfileMapper
				WHERE Promotionid = IPs.PromotionId AND ISNULL(ProfileId,0)=ISNULL(P.ProfileId,0))
		'
		EXEC SP_EXECUTESQL @Query;

		IF @IsGenerateErrorLog = 1
		BEGIN 
			-- Update for ZnodePromotionProfileMapper
			SET @Query='
			DELETE FROM ZnodePromotionProfileMapper WHERE PromotionId IN (SELECT PromotionId FROM #UpdatedPromotions) AND ProfileId IS NULL

			DELETE PPM
			FROM ZnodePromotionProfileMapper PPM
			INNER JOIN #UpdatedPromotions IPs ON PPM.Promotionid = IPs.PromotionId
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode AND P.ProfileId IS NULL

			INSERT INTO ZnodePromotionProfileMapper
				(PromotionId,ProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, P.ProfileId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #UpdatedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			WHERE NOT EXISTS (SELECT * FROM ZnodePromotionProfileMapper
					WHERE Promotionid = IPs.PromotionId AND ISNULL(ProfileId,0)=ISNULL(P.ProfileId,0)) AND ISNULL(P.Profile,'''')<>''''
			'
			EXEC SP_EXECUTESQL @Query;
		END

		-- Insert for ZnodePromotionCoupon
		SET @Query='
		INSERT INTO ZnodePromotionCoupon
			(PromotionId,Code,InitialQuantity,AvailableQuantity,IsActive,IsCustomCoupon,CustomCouponCode,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT DISTINCT IPs.PromotionId, P.Code, P.InitialQuantity, P.AvailableQuantity, 1, 0, '''', '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
		FROM #InsertedPromotions IPs 
		INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			'+ CASE WHEN @IsGenerateErrorLog = 1  THEN '
				INNER JOIN #DuplicatePromotionsData DP ON LTRIM(RTRIM(P.PromoCode)) = DP.PromoCode AND DP.Rn=1 AND P.RowId=DP.RowId
			' ELSE '' END +'
		WHERE ISNULL(P.IsCouponRequired,'''') IN (''True'',''1'',''Yes'')
			AND NOT EXISTS (SELECT * FROM ZnodePromotionCoupon WHERE Code = P.Code)
		'
		EXEC SP_EXECUTESQL @Query;
	
		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Brand' AND @IsGenerateErrorLog = 1 )  OR (@IsGenerateErrorLog = 0 AND (SELECT TOP 1  valued FROM @checkTable WHERE code = 'Brand') <> '' )
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionBrand (PromotionId,BrandId,BrandCode,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, BD.BrandId, BD.BrandCode, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #InsertedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodeBrandDetails BD ON (' +CASE WHEN @IsGenerateErrorLog = 1 THEN ' P.Brand=BD.BrandCode ' ELSE ' BD.BrandId IN (  '+(SELECT TOP 1 Brand FROM ##Promotions )+') ' END +' )  
			WHERE ISNULL(BD.BrandId,0)<>0   '+CASE WHEN @IsGenerateErrorLog = 0 AND @InsertData = 0 THEN ' AND 1=0 ' ELSE '' END 
			
			EXEC SP_EXECUTESQL @Query; 
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Brand' AND @IsGenerateErrorLog = 1 )
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionBrand (PromotionId,BrandId,BrandCode,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, BD.BrandId, BD.BrandCode, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #UpdatedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodeBrandDetails BD ON P.Brand=BD.BrandCode
			WHERE NOT EXISTS (SELECT * FROM ZnodePromotionBrand	WHERE Promotionid = IPs.PromotionId AND BrandId=BD.BrandId)
			'
			EXEC SP_EXECUTESQL @Query; 
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Catalog' AND @IsGenerateErrorLog = 1) OR (@IsGenerateErrorLog = 0 AND (SELECT TOP 1  valued FROM @checkTable WHERE code = 'Catalog') <> '' )
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionCatalogs (PromotionId,PublishCatalogId,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, PbC.PimCatalogId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #InsertedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
		   '+ CASE WHEN @IsGenerateErrorLog = 1  THEN '
			INNER JOIN ZnodePimCatalog PC     ON  (  P.Catalog=PC.CatalogCode  ) 
			' ELSE '' END +'
			INNER JOIN ZnodePimcatalog PbC  ON  (' + CASE WHEN @IsGenerateErrorLog = 1 THEN ' PC.PimCatalogId=PbC.PimCatalogId ' ELSE ' PbC.PimCatalogId IN (  '+(SELECT TOP 1 Catalog FROM ##Promotions )+') ' END +' )  
			WHERE ISNULL(PbC.PimCatalogId,0)<>0
			'+CASE WHEN @IsGenerateErrorLog = 0 AND @InsertData = 0 THEN ' AND 1=0 ' ELSE '' END 

			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Catalog' AND @IsGenerateErrorLog = 1)
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionCatalogs (PromotionId,PublishCatalogId,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, PC.PimCatalogId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #UpdatedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodePimCatalog PC ON (P.Catalog=PC.CatalogCode) 
			WHERE NOT EXISTS (SELECT * FROM ZnodePromotionCatalogs WHERE Promotionid = IPs.PromotionId AND PublishCatalogId=PC.PimCatalogId)
			'
			EXEC SP_EXECUTESQL @Query;
		END
		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Category' AND @IsGenerateErrorLog = 1) OR (@IsGenerateErrorLog = 0 AND (SELECT TOP 1 valued FROM @checkTable WHERE code = 'Category') <> '' )
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionCategory (PromotionId,PublishCategoryId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT  DISTINCT IPs.PromotionId, PbC.PimCategoryHierarchyId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #InsertedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			'+ CASE WHEN @IsGenerateErrorLog = 1 THEN +'
			INNER JOIN ZnodePimCategoryAttributeValueLocale PCAVL  ON P.Category=PCAVL.CategoryValue
			INNER JOIN ZnodePimCategoryAttributeValue PCAV ON PCAVL.PimCategoryAttributeValueId=PCAV.PimCategoryAttributeValueId
			INNER JOIN ZnodePimAttribute PA ON PCAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''CategoryCode''
			INNER JOIN ZnodePimCategory PC ON PCAV.PimCategoryId=PC.PimCategoryId 
			' ELSE '' END +'
			INNER JOIN ZnodePimCategoryHierarchy PbC ON  (' + CASE WHEN @IsGenerateErrorLog = 1 THEN ' PC.PimCategoryId=PbC.PimCategoryId ' ELSE ' PbC.PimCategoryHierarchyId IN (  '+(SELECT TOP 1 Category FROM ##Promotions )+') ' END +' )
			WHERE ISNULL(PbC.PimCategoryId,0)<>0 '+CASE WHEN @IsGenerateErrorLog = 0 AND @InsertData = 0 THEN ' AND 1=0 ' ELSE '' END +'
			GROUP BY IPs.PromotionId,PbC.PimCategoryHierarchyId
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Category' AND @IsGenerateErrorLog = 1)
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionCategory (PromotionId,PublishCategoryId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, PbC.PimCategoryHierarchyId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #UpdatedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodePimCategoryAttributeValueLocale PCAVL  ON P.Category=PCAVL.CategoryValue
			INNER JOIN ZnodePimCategoryAttributeValue PCAV ON PCAVL.PimCategoryAttributeValueId=PCAV.PimCategoryAttributeValueId
			INNER JOIN ZnodePimAttribute PA ON PCAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''CategoryCode''
			INNER JOIN ZnodePimCategory PC ON PCAV.PimCategoryId=PC.PimCategoryId 
			INNER JOIN ZnodePimCategoryHierarchy PbC ON PC.PimCategoryId=PbC.PimCategoryId
			WHERE NOT EXISTS (SELECT * FROM ZnodePromotionCategory WHERE Promotionid = IPs.PromotionId AND PublishCategoryId=PbC.PimCategoryHierarchyId)
			GROUP BY IPs.PromotionId,PbC.PimCategoryHierarchyId
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Shipping' AND @IsGenerateErrorLog = 1 ) OR (@IsGenerateErrorLog = 0 AND (SELECT TOP 1 valued FROM @checkTable WHERE code = 'Shipping') <> '' )
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionShipping (PromotionId,ShippingId,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, SP.ShippingId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #InsertedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodeShipping SP ON   (' + CASE WHEN @IsGenerateErrorLog = 1 THEN '   P.Shipping=SP.ShippingCode ' ELSE ' sp.shippingId  IN (  '+(SELECT TOP 1 Shipping FROM ##Promotions )+') ' END +' ) 
			
			WHERE ISNULL(SP.ShippingId,0)<>0 
			'+CASE WHEN @IsGenerateErrorLog = 0 AND @InsertData = 0 THEN ' AND 1=0 ' ELSE '' END
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='Shipping' AND @IsGenerateErrorLog = 1)
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionShipping (PromotionId,ShippingId,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, SP.ShippingId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #UpdatedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodeShipping SP ON P.Shipping=SP.ShippingCode 
			WHERE NOT EXISTS (SELECT * FROM ZnodePromotionShipping WHERE Promotionid = IPs.PromotionId AND ShippingId=SP.ShippingId)
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='ProductToDiscount' AND @IsGenerateErrorLog = 1  )  OR (@IsGenerateErrorLog = 0 AND (SELECT TOP 1  valued FROM @checkTable WHERE code = 'ProductToDiscount')  <> '' )
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionProduct (PromotionId,PublishProductId,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, PP.PimProductId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #InsertedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			'+ CASE WHEN @IsGenerateErrorLog = 1 THEN '
			INNER JOIN ZnodePimAttributeValueLocale PAVL ON LTRIM(RTRIM(P.ProductToDiscount))=PAVL.AttributeValue
			INNER JOIN ZnodePimAttributeValue PAV ON PAVL.PimAttributeValueId=PAV.PimAttributeValueId
			INNER JOIN ZnodePimAttribute PA ON PAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''SKU''
			' ELSE '' END + '
			INNER JOIN ZnodePimProduct PP  ON   (' + CASE WHEN @IsGenerateErrorLog = 1 THEN '   PAV.PimProductId=PP.PimProductId ' ELSE ' PP.PimProductId  IN (  '+(SELECT TOP 1 ProductToDiscount FROM ##Promotions )+') ' END +' )  
			WHERE ISNULL(PP.PimProductId,0)<>0 '+CASE WHEN @IsGenerateErrorLog = 0 AND @InsertData = 0 THEN ' AND 1=0 ' ELSE '' END +'
			GROUP BY IPs.PromotionId,PP.PimProductId
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='ProductToDiscount' AND @IsGenerateErrorLog = 1)
		BEGIN
			SET @Query='
			INSERT INTO ZnodePromotionProduct (PromotionId,PublishProductId,CreatedBy,CreatedDate,ModifedBy,ModifiedDate)
			SELECT DISTINCT IPs.PromotionId, PP.PimProductId, '+@UserId1+', '''+@GetDate1+''', '+@UserId1+', '''+@GetDate1+'''
			FROM #UpdatedPromotions IPs 
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			INNER JOIN ZnodePimAttributeValueLocale PAVL ON LTRIM(RTRIM(P.ProductToDiscount))=PAVL.AttributeValue
			INNER JOIN ZnodePimAttributeValue PAV ON PAVL.PimAttributeValueId=PAV.PimAttributeValueId
			INNER JOIN ZnodePimAttribute PA ON PAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''SKU''
			INNER JOIN ZnodePimProduct PP  ON PAV.PimProductId=PP.PimProductId 
			WHERE NOT EXISTS (SELECT * FROM ZnodePromotionProduct WHERE Promotionid = IPs.PromotionId AND PublishProductId=PP.PimProductId)
			GROUP BY IPs.PromotionId,PP.PimProductId
			'
			EXEC SP_EXECUTESQL @Query;
		END

		IF EXISTS (SELECT * FROM Tempdb.Sys.Columns WHERE OBJECT_ID = OBJECT_ID('tempdb..##Promotions') AND name='RequiredProduct'  AND @IsGenerateErrorLog = 1 )  OR (@IsGenerateErrorLog = 0 AND (SELECT TOP 1  valued FROM @checkTable WHERE code = 'RequiredProduct')<> '' )
		BEGIN
			SET @Query='
			UPDATE ZP
			SET ZP.PromotionProductQuantity=P.PromotionProductQuantity, ZP.ReferralPublishProductId=PP.PimProductId, ModifiedDate='''+@GetDate1+'''
			FROM ZnodePromotion ZP
			INNER JOIN #InsertedPromotions IPs ON ZP.PromotionId=IPs.PromotionId
			INNER JOIN ##Promotions P ON IPs.PromoCode=P.PromoCode
			'+ CASE WHEN @IsGenerateErrorLog = 1  THEN '
			INNER JOIN ZnodePimAttributeValueLocale PAVL ON LTRIM(RTRIM(P.RequiredProduct))=PAVL.AttributeValue
			INNER JOIN ZnodePimAttributeValue PAV ON PAVL.PimAttributeValueId=PAV.PimAttributeValueId
			INNER JOIN ZnodePimAttribute PA ON PAV.PimAttributeId=PA.PimAttributeId AND PA.AttributeCode=''SKU''
			' ELSE '' END +'
			INNER JOIN ZnodePimProduct PP ON   (' + CASE WHEN @IsGenerateErrorLog = 1 THEN '   PAV.PimProductId=PP.PimProductId ' ELSE ' PP.PimProductId  IN (  '+(SELECT TOP 1 RequiredProduct FROM ##Promotions )+') ' END +' )  
			'
			EXEC SP_EXECUTESQL @Query;
		END

		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		DROP TABLE IF EXISTS ##Promotions;

		COMMIT TRAN Promotions;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN Promotions

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPromotions
					@TableName = '+CAST(@TableName AS VARCHAR(MAX)) +',
					@Status='+ CAST(@Status AS VARCHAR(10))+',
					@UserId = '+CAST(@UserId AS VARCHAR(50))+',
					@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',
					@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',
					@LocaleId='+CAST(@LocaleId AS VARCHAR(200))+',
					@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(MAX))+',
					@PromotionTypeId='+CAST(@PromotionTypeId AS VARCHAR(20))+',
					@IsGenerateErrorLog='+CAST(@IsGenerateErrorLog AS VARCHAR(10));

		IF @IsGenerateErrorLog = 1
		BEGIN
			---Import process updating fail due to database error
			UPDATE ZnodeImportProcessLog
			SET Status = dbo.Fn_GetImportStatus(3), ProcessCompletedDate = @GetDate
			WHERE ImportProcessLogId = @ImportProcessLogId;

			---Loging error for Import process due to database error
			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

			--Updating total and fail record count
			UPDATE ZnodeImportProcessLog 
			SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , 
				SuccessRecordCount = 0,
				TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
			WHERE ImportProcessLogId = @ImportProcessLogId;
		END

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportPromotions',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;