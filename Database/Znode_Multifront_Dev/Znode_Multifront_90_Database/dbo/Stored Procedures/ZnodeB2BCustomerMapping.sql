--- ZnodeB2BCustomerMapping @ImportHeadName = 'B2BCustomer', @TableName = 'tempdb..##B2BCustomer_7a6e2c3f-ad0c-4ba1-8d4d-827f6748db35'
CREATE PROCEDURE [dbo].[ZnodeB2BCustomerMapping]
(
	@ImportHeadName     VARCHAR(200),
    @TableName          VARCHAR(200)
)
AS
Begin
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	declare @SQL varchar(max)
	DECLARE @TemplateId INT
	
	--------- template mapping 
	SELECT TOP 1 @TemplateId = ImportTemplateId FROM dbo.ZnodeImportTemplate 
	WHERE ImportHeadId = (SELECT ImportHeadId FROM ZnodeImportHead WHERE Name = @ImportHeadName )
	
	 IF OBJECT_ID ('tempdb..##GlobalAttributeColumnForMapping') is not null
			DROP TABLE tempdb..##GlobalAttributeColumnForMapping
	
	SET @SQL = '
			select isnull(Name ,'''') Name
			into ##GlobalAttributeColumnForMapping
			from tempdb.sys.columns a
			inner join ZnodeGlobalAttribute ZGA on a.Name = ZGA.AttributeCode where object_id = object_id('''+@TableName+''')'
	--print @SQL
	EXEC (@SQL)
		
		DELETE FROM ZnodeImportTemplateMapping WHERE ImportTemplateId = @TemplateId
		AND NOT EXISTS( SELECT * FROM ZnodeImportTemplate TM inner join ZnodeImportTemplateMapping ITM on TM.ImportTemplateId = ITM.ImportTemplateId
					WHERE TM.TemplateName='CustomerTemplate' and ZnodeImportTemplateMapping.SourceColumnName = ITM.SourceColumnName  )

		INSERT [dbo].[ZnodeImportTemplateMapping] ( [ImportTemplateId], [SourceColumnName], [TargetColumnName], [DisplayOrder], [IsActive], [IsAllowNull], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
		SELECT @TemplateId , CM.name as [SourceColumnName], CM.name AS [TargetColumnName], 1, 1, 1, 2, @GetDate, 2, @GetDate
		FROM ##GlobalAttributeColumnForMapping CM
		WHERE NOT EXISTS ( SELECT * FROM [ZnodeImportTemplateMapping] TM WHERE TM.ImportTemplateId = @TemplateId and CM.name = TM.SourceColumnName )

		IF OBJECT_ID ('tempdb..##GlobalAttributeColumnForMapping') is not null
			DROP TABLE tempdb..##GlobalAttributeColumnForMapping

END