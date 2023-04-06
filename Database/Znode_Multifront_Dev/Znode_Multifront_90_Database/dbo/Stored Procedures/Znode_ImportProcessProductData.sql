--  [dbo].[Znode_ImportProcessProductData] '1928de37-30d3-4cc1-b5e3-0c498c0da183'
CREATE PROCEDURE [dbo].[Znode_ImportProcessProductData](@TblGUID nvarchar(255),@ERPTaskSchedulerId int )
AS
BEGIN
	SET NOCOUNT ON;
	SET TEXTSIZE 2147483647;
	DECLARE @NewuGuId nvarchar(255),@ImportHeadId INT 
	set @NewuGuId = newid()
    DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	DECLARE @DefaultFamilyId  INT = dbo.Fn_GetDefaultPimProductFamilyId();
	DECLARE @LocaleId  INT = dbo.Fn_GetDefaultLocaleId()
	DECLARE @TemplateId INT , @PortalId INT 
	DECLARE @WebsiteCode Varchar(100) 
	SET  @WebsiteCode = 'b9001'
	SELECT TOP 1 @PortalId  = PortalId   FROM dbo.ZnodePortal

	IF OBJECT_ID('tempdb.dbo.##ProductDetail', 'U') IS NOT NULL 
		DROP TABLE ##ProductDetail

	IF OBJECT_ID('tempdb.dbo.#Attributecode', 'U') IS NOT NULL 
		DROP TABLE #Attributecode
	
	IF OBJECT_ID('tempdb.dbo.#ConfigurableAttributecode', 'U') IS NOT NULL 
		DROP TABLE #ConfigurableAttributecode 
	
	IF OBJECT_ID('tempdb.dbo.#DefaultAttributeCode', 'U') IS NOT NULL 
		DROP TABLE #DefaultAttributeCode 

    IF OBJECT_ID('tempdb.dbo.[##ProductAssociation]', 'U') IS NOT NULL 
		DROP TABLE tempdb.dbo.[##ProductAssociation]

		
	Declare @GlobalTemporaryTable nvarchar(255)
	DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '', 
		    @InsertColumnName   NVARCHAR(MAX), 
			@UpdateTable2Column NVARCHAR(MAX),
			@UpdateTable3Column NVARCHAR(MAX),
			@UpdateTable4Column NVARCHAR(MAX),
			@ImportTableColumnName NVARCHAR(MAX),
			@ImportTableName VARCHAR(200),
			@TableName4 NVARCHAR(255) = 'tempdb..[##PRDDA_' + @TblGUID + ']',
			@Sql NVARCHAR(MAX) = '',
			@Attribute NVARCHAR(MAX)

	DECLARE @Attributecode TABLE ( Attrcode NVARCHAR(255) )

	CREATE TABLE #Attributecode ( Attrcode NVARCHAR(255) )
	CREATE TABLE #ConfigurableAttributecode (SKU NVARCHAR(255) , PimAttributeId  int , DefaultValue nvarchar(255) ,AttributeCode nvarchar(255) ,ParentSKU nvarchar(255)) 
	
	SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'ProductTemplate'
	SELECT @ImportHeadId= ImportHeadId FROM dbo.ZnodeImportHead WHERE Name = 'Product'
	SET @Sql = '
	INSERT INTO ZnodeImportTemplateMapping ( ImportTemplateId, SourceColumnName, TargetColumnName, DisplayOrder, IsActive, IsAllowNull, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )
	SELECT Distinct 1 AS ImportTemplateId,  PA.AttributeCode AS SourceColumnName,  PA.AttributeCode AS TargetColumnName, 0 AS DisplayOrder, 0 AS IsActive, 0 AS IsAllowNull, 2 AS CreatedBy, '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' AS CreatedDate, 2 AS ModifiedBy, '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' AS ModifiedDate
	FROM '+@TableName4+' PRD
	INNER JOIN ZnodePimAttribute PA ON PRD.Attribute = PA.AttributeCode
	WHERE NOT EXISTS ( SELECT * FROM ZNODEIMPORTTEMPLATEMAPPING ITM WHERE ImportTemplateId = ' + CONVERT(NVARCHAR(100), @TemplateId ) + ' AND PRD.ATTRIBUTE =  ITM.SOURCECOLUMNNAME )'


	EXEC ( @Sql )
	
	SELECT @CreateTableScriptSql = 'CREATE TABLE ##ProductDetail ('+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)' 
	FROM [dbo].[ZnodeImportTemplateMapping]
	WHERE [ImportTemplateId]= @TemplateId FOR XML PATH ('')),2,4000)+' , ParentStyle NVARCHAR(MAX),GUID nvarchar(255), BaseProductType nvarchar(255) )'

	EXEC ( @CreateTableScriptSql )
	


	--Merge all the tables which is type is inserted / updated 
	DECLARE Cur_InsertProduct CURSOR FOR
	
	SELECT ImportTableName FROM ZnodeImportTableDetail WHERE ImportTableNature = 'Insert' AND ImportHeadId = @ImportHeadId --AND ImportTableName = 'PRDH'
	
	OPEN Cur_InsertProduct 

	FETCH NEXT FROM Cur_InsertProduct INTO @ImportTableName

	WHILE ( @@FETCH_STATUS = 0 )
	BEGIN
	    SET @GlobalTemporaryTable = 'tempdb..[##' + @ImportTableName + '_' + @TblGUID + ']' 
		--1 simple 
		    SET @Sql = ' 
			SELECT @InsertColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ITCD.BaseImportColumn +'']''  ,''NULL'')
			FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD 
			ON ITCD.ImportTableId = ITD.ImportTableId
			WHERE  ITD.ImportTableName = @ImportTableName 
			AND ITD.ImportHeadId = ' + convert(nvarchar(100),@ImportHeadId)+ '
			  AND ITCD.BaseImportColumn is not null FOR XML PATH ('''')),2,4000)

			SELECT @ImportTableColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ImportTableColumnName +'']''  ,''NULL'') 
			FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD 
			ON ITCD.ImportTableId = ITD.ImportTableId
			WHERE  ITD.ImportTableName = @ImportTableName  AND ITD.ImportHeadId =  
			' + convert(nvarchar(100),@ImportHeadId)+    '
			AND ITCD.BaseImportColumn is not null FOR XML PATH ('''')),2,4000)'


		EXEC sp_executesql @SQL, N'@ImportTableName VARCHAR(200),@InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT', @ImportTableName = @ImportTableName, @InsertColumnName = @InsertColumnName OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT

			
		IF( LEN(@InsertColumnName) > 0 )
		BEGIN
			SET @SQL = 'INSERT INTO ##ProductDetail ( ParentStyle, '+@InsertColumnName+' )	SELECT [Parent Style], '+@ImportTableColumnName +' FROM '+@GlobalTemporaryTable
			SET @SQL = @SQL + ' Where Website = ''' + @WebsiteCode + '''' 
			EXEC sp_executesql @SQL
		END
		
		SELECT @InsertColumnName ='', @GlobalTemporaryTable=''

		FETCH NEXT FROM Cur_InsertProduct INTO @ImportTableName
	END

	CLOSE Cur_InsertProduct
	DEALLOCATE Cur_InsertProduct

	
	DECLARE @UpdateTableColumn varchar(max)


	SET @Sql = 
		'SELECT @UpdateTableColumn = 
		 COALESCE(@UpdateTableColumn + '','', '''') + ''[''+BaseImportColumn+''] = B.[''+BaseImportColumn+'']''
		 FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD 
		 ON ITCD.ImportTableId = ITD.ImportTableId
		 WHERE  ITD.ImportTableName = ''PRDH''  AND ITCD.BaseImportColumn IS NOT NULL AND ITCD.BaseImportColumn <> ''SKU'''

	EXEC sp_executesql @SQL, N'@UpdateTableColumn VARCHAR(200) OUTPUT', @UpdateTableColumn = @UpdateTableColumn OUTPUT
	
	SET @Sql = 
		';WITH CTE AS
		(
			SELECT * FROM ##ProductDetail WHERE ProductName IS NOT NULL
		)
		UPDATE A 
		SET '+@UpdateTableColumn+'
		FROM ##ProductDetail A 
		INNER JOIN CTE B ON B.ParentStyle = A.ParentStyle'
	EXEC ( @Sql )
    
	SET @Sql = 'INSERT INTO #Attributecode ( Attrcode ) SELECT DISTINCT ltrim(rtrim([Attribute])) FROM '+ @TableName4
	 + ' where ltrim(rtrim([Attribute]))  in (Select AttributeCode from ZnodePimAttribute where IsCategory = 0 ) AND RowNumber = ''' + @WebsiteCode + ''''
	EXEC ( @Sql )

	DECLARE Cur_AttributeCode CURSOR FOR SELECT Attrcode FROM #Attributecode where Attrcode is not null 
    OPEN Cur_AttributeCode
    FETCH NEXT FROM Cur_AttributeCode INTO @Attribute
    WHILE ( @@FETCH_STATUS = 0 )
	BEGIN
		SET @SQL = ''
		SET @SQL =  'UPDATE PD SET PD.' + @Attribute  + '= ' +
		 ' Replace(Replace(PDD.[Attribute Value], ''/'', ''''), '' '', '''')' +
		 ' FROM ##ProductDetail PD inner join '+ @TableName4 + ' PDD on PD.SKU = PDD.SKU# WHERE PDD.Attribute =  '''+@Attribute + ''' 
		 AND PDD.RowNumber = ''' + @WebsiteCode + ''''

		EXEC sp_executesql @SQL
		FETCH NEXT FROM Cur_AttributeCode INTO @Attribute
	END
	CLOSE Cur_AttributeCode
	DEALLOCATE Cur_AttributeCode
	
	SET @Sql = 'UPDATE ##ProductDetail SET GUID= '''+@NewuGuId  + ''', BaseProductType = ProductType'
	EXEC sp_executesql @SQL
	
	SET @Sql = 'UPDATE ##ProductDetail SET ProductType =  CASE when [ParentStyle] = SKU 
	then ''ConfigurableProduct'' ELSE ''SimpleProduct'' END ,
	MinimumQuantity = 1 , MaximumQuantity = 10 ,ShippingCostRules = ''WeightBasedRate'',OutOfStockOptions = ''DontTrackInventory''
	,ProductCode = CASE When ProductCode Is Null then SKU ELSE ProductCode  END , 
	IsActive = CASE when Isnull(IsActive,'''') = '''' then 1 END'
	EXEC sp_executesql @SQL
	
	DELETE  FROM ##ProductDetail where isnull(SKU,'') = ''
	---- Read All default data 
	
	-- Product Association data prepartion 
	Create TABLE tempdb..[##ProductAssociation] (ParentSKU nvarchar(255),ChildSKU nvarchar(255), DisplayOrder int,GUID nvarchar(100) )
	SET @Sql = '
	insert into tempdb..[##ProductAssociation]  (ParentSKU ,ChildSKU , DisplayOrder,GUID )
	select [ParentStyle], SKU  ,1, ''' + @NewuGuId + ''' from ##ProductDetail  where [ParentStyle] <>  SKU and [ParentStyle] is not null 
	'
	EXEC (@Sql)
	
	-- Configrable Attributes
	SET @Sql = 'INSERT INTO #ConfigurableAttributecode (PimAttributeId ,DefaultValue ,AttributeCode,ParentSKU)
	            SELECT Distinct ZPA.PimAttributeId,[Attribute Value]	 ,ltrim(rtrim(PDA.[Attribute])), PDA.[Parent Style]  FROM '+ @TableName4
	 + ' PDA Inner join  tempdb..##ProductDetail PD  ON PDA.[SKU#]= PD.SKU and PD.BaseProductType = ''C''
	 AND PDA.Rownumber = ''' + @WebsiteCode + ''' 
	  Inner join ZnodePimAttribute ZPA ON ZPA.AttributeCode = ltrim(rtrim(PDA.[Attribute])) AND ZPA.IsCategory = 0 and ZPA.IsConfigurable =1 '
	EXEC ( @Sql )

	-- Update default vaule of confi attribute in main template
	
	DECLARE @DefaultValue nvarchar(255),@ParentSKU  nvarchar(255),@AttributeName nvarchar(255)
	DECLARE Cur_ConfigAttributeCode CURSOR FOR SELECT DefaultValue, AttributeCode,ParentSKU 
	FROM #ConfigurableAttributecode  where DefaultValue is not null 
    OPEN Cur_ConfigAttributeCode
    FETCH NEXT FROM Cur_ConfigAttributeCode INTO @DefaultValue, @AttributeName,@ParentSKU
    WHILE ( @@FETCH_STATUS = 0 )
	BEGIN
		SET @SQL = ''
		SET @SQL =  'UPDATE ##ProductDetail SET ' + @AttributeName  + ' = ''' +  Replace(Replace(@DefaultValue, '/', ''), ' ', '') + 
		''' WHERE SKU  =  '''+	@ParentSKU + ''''
		EXEC sp_executesql @SQL
		FETCH NEXT FROM Cur_ConfigAttributeCode INTO @DefaultValue, @AttributeName,@ParentSKU
	END
	CLOSE Cur_ConfigAttributeCode
	DEALLOCATE Cur_ConfigAttributeCode
		
	SET @Sql = 'Alter TABLE ##ProductDetail drop column [ParentStyle],[BaseProductType]'
	EXEC sp_executesql @SQL
	

	SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'ProductTemplate'
	---- Import product    
	EXEC Znode_ImportData @TableName = 'tempdb..[##ProductDetail]',	@NewGUID = @TblGUID ,@TemplateId = @TemplateId,
	      @UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = @DefaultFamilyId,@PriceListId = 0, @CountryCode = ''
		 ,@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 1, @StepName = 'Import' ,@ERPTaskSchedulerId  = @ERPTaskSchedulerId 

	If Exists (select TOP 1 1 from #ConfigurableAttributecode ) 
	BEGIN
			SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'ProductAssociation'

			EXEC Znode_ImportData @TableName = 'tempdb..[##ProductAssociation]',	@NewGUID =  @TblGUID ,@TemplateId = @TemplateId,
			 @UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = ''--, @IsDebug =1 
			,@IsDoNotCreateJob = 1 , @IsDoNotStartJob = 0, @StepName = 'Import1', @StartStepName  ='Import',@step_id = 2 
			,@Nextstep_id  = 1,@ERPTaskSchedulerId  = @ERPTaskSchedulerId  
		
	END
	 select 'Job create successfully.' 
	
END