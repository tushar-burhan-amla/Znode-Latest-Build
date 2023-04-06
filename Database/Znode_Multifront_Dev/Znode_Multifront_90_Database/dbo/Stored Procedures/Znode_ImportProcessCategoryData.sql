


CREATE  PROCEDURE [dbo].[Znode_ImportProcessCategoryData](@TblGUID nvarchar(255) = '',@ERPTaskSchedulerId int  )
AS
BEGIN

	--unit testing
	--EXEC Znode_ImportProcessCategoryData  @TblGUID = '45345354345',@ERPTaskSchedulerId =  19

	--Create Table PRDHA (Category nvarchar(100), [Parent Style] nvarchar(100))
	--insert into PRDHA (Category , [Parent Style]) VALUES ('Soft Shell','9024CLO')


	SET NOCOUNT ON;
	Declare @NewuGuId nvarchar(255)
	set @NewuGuId = newid()
	Declare @CurrencyId int
	DECLARE @TemplateId INT , @PortalId INT
	DECLARE @LocaleId  int = dbo.Fn_GetDefaultLocaleId()
	SELECT TOP 1 @PortalId  = PortalId FROM dbo.ZnodePortal

	DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '',
		    @InsertColumnName NVARCHAR(MAX),
			@ImportTableColumnName NVARCHAR(MAX),
			@DefaultFamilyId  INT = dbo.Fn_GetCategoryDefaultFamilyId(),
			@TableName NVARCHAR(500) = 'PRDHA',
			@TableNameForSKU NVARCHAR(500) = 'PRDD',
			@Sql NVARCHAR(MAX) = '',
			@RowNum int,
			@MaxRowNum int,
			@FirstStep nvarchar(255),
			@PriceTableName  nvarchar(255)

		DECLARE @CategoryAttributId int;

		SET @CategoryAttributId =
		(
			SELECT TOP 1 PimAttributeId
			FROM ZnodePimAttribute AS ZPA
			WHERE ZPA.AttributeCode = 'CategoryName' AND
				  ZPA.IsCategory = 1
		);
	SET @TableName = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']'
	SET @TableNameForSKU = 'tempdb..[##' + @TableNameForSKU + '_' + @TblGUID + ']'

	--SET @TableName = '[' + @TableName + '_' + @TblGUID + ']'
	--SET @TableNameForSKU = '[' + @TableNameForSKU + '_' + @TblGUID + ']'

IF OBJECT_ID('tempdb.dbo.#CategoryData', 'U') IS NOT NULL
		DROP TABLE #CategoryData
	CREATE TABLE #CategoryData (CategoryBanner nvarchar(max),CategoryName nvarchar(max),CategoryCode NVARCHAR(max),CategoryTitle nvarchar(max),DisplayOrderCategory nvarchar(max),
								LongDescription nvarchar(max),ShortDescription nvarchar(max),CategoryImage nvarchar(max) ,IsActive int , guId nvarchar(300))

	SET @SQL =
	'INSERT INTO #CategoryData (CategoryBanner,CategoryName,CategoryCode,CategoryTitle ,DisplayOrderCategory,IsActive,GUID)
	 SELECT  Distinct ltrim(rtrim(Replace(PRD.Category,''"'',''''))),ltrim(rtrim(Replace(PRD.Category,''"'',''''))),REPLACE(ltrim(rtrim(Replace(PRD.Category,''"'',''''))),'' '',''''),
	 ltrim(rtrim(Replace(PRD.Category,''"'',''''))),1,1, ''' + @TblGUID + '''   FROM ' +@TableName+ ' PRD
	 where ltrim(rtrim(Replace(PRD.Category,''"'',''''))) NOT in
	 (SELECT ltrim(rtrim(ZPCAL.CategoryValue))
			   FROM ZnodePimCategoryAttributeValue AS ZPCA	INNER JOIN	ZnodePimCategoryAttributeValueLocale AS ZPCAL
					ON ZPCA.PimAttributeId = ' + convert(nvarchar(100),@CategoryAttributId)  + ' AND
					ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId) '
	EXEC sp_executesql @SQL

	--SET @SQL = 'Select * from #CategoryData'
	--EXEC sp_executesql @SQL

	IF OBJECT_ID('tempdb.dbo.#CategoryDataAssociation', 'U') IS NOT NULL
		DROP TABLE #CategoryDataAssociation
	CREATE TABLE #CategoryDataAssociation (CategoryName nvarchar(max),SKU nvarchar(max),DisplayOrder int,IsActive int , GUID nvarchar(300))

	SET @SQL =
	'INSERT INTO #CategoryDataAssociation (CategoryName,SKU,DisplayOrder,IsActive,GUID)
	 SELECT  Distinct ltrim(rtrim(Replace(PRD.Category,''"'',''''))) Category ,ltrim(rtrim(Replace(PRDD.[Sku#],''"'',''''))) [Sku#] ,1,1, ''' + @TblGUID + '''  FROM ' +@TableName+ ' PRD INNER JOIN ' +@TableNameForSKU+  + ' PRDD ON '
	 + ' ltrim(rtrim(Replace(PRD.[Parent Style],''"'',''''))) = ltrim(rtrim(Replace(PRDD.[Parent Style],''"'','''')))
	 AND ltrim(rtrim(Replace(PRD.[Website],''"'',''''))) = ltrim(rtrim(Replace(PRDD.[Website],''"'','''')))  '

	EXEC sp_executesql @SQL


	if Exists (Select TOP 1 1 from tempdb..[#CategoryData])
	Begin
		SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'CategoryTemplate'

		EXEC Znode_ImportData @TableName = 'tempdb..[#CategoryData]',	@NewGUID = @TblGUID ,@TemplateId = @TemplateId,
		      @UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = @DefaultFamilyId,@PriceListId = 0, @CountryCode = ''
			 ,@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 1, @StepName = 'Import' ,@ERPTaskSchedulerId  = @ERPTaskSchedulerId , @IsDebug =1


		SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'CategoryAssociation'

		EXEC Znode_ImportData @TableName = 'tempdb..[#CategoryDataAssociation]',	@NewGUID =  @TblGUID ,@TemplateId = @TemplateId,
			 @UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = 0,@PriceListId = 0,
			 @CountryCode = ''--, @IsDebug =1
			,@IsDoNotCreateJob = 1 , @IsDoNotStartJob = 0, @StepName = 'Import1'
			,@StartStepName  ='Import',@step_id = 2, @IsDebug =1
			,@Nextstep_id  = 1,@ERPTaskSchedulerId  = @ERPTaskSchedulerId
	END
	ELSE
	Begin
		SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'CategoryAssociation'
		--EXEC Znode_ImportData @TableName = 't' ,@NewGUID = '0e8714de-abcb-423d-ad2a-fb219815c435' ,@TemplateId = 8,
		--@UserId = 2,@PortalId = 1,@LocaleId = 1,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = '',--, @IsDebug =1
		--@ERPTaskSchedulerId  = 19

		EXEC Znode_ImportData @TableName = 'tempdb..[#CategoryDataAssociation]' ,@NewGUID = @TblGUID ,@TemplateId = @TemplateId,
				@UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = '',--, @IsDebug =1
				@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 0,@Nextstep_id  = 1 ,
				@ERPTaskSchedulerId  = @ERPTaskSchedulerId  ,@Isdebug =1
	END
	select 'Job create successfully.'
END