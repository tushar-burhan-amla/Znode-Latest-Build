  CREATE  PROCEDURE [dbo].[Znode_ImportProcessInventoryData](@TblGUID nvarchar(255) = '' ,@ERPTaskSchedulerId  int )  
AS  
BEGIN  
  
 SET NOCOUNT ON;  
 Declare @NewuGuId nvarchar(255)  
 set @NewuGuId = newid()  
 Declare @CurrencyId int ,@PortalId int ,@TemplateId INT ,@ImportHeadId INT   
 SELECT @CurrencyId = CurrencyId  from ZnodeCulture where CultureCode in (Select FeatureValues from   ZnodeGlobalSetting where FeatureName = 'Currency')   
   
 DECLARE @LocaleId  int = dbo.Fn_GetDefaultLocaleId()  
 SELECT TOP 1 @PortalId  = PortalId FROM dbo.ZnodePortal  
  
 DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '',   
      @InsertColumnName NVARCHAR(MAX),   
   @ImportTableColumnName NVARCHAR(MAX),  
   @TableName NVARCHAR(255) = 'MAGINV',     
   @Sql NVARCHAR(MAX) = '',  
   @PriceListId int,  
   @ListCode nvarchar(255) = 'TempMAGINV' ,  
   @RowNum int,   
   @MaxRowNum int,  
   @FirstStep nvarchar(255),  
   @PriceTableName  nvarchar(255),  
   @WarehouseCode varchar(100)  
  
 Select TOP 1  @WarehouseCode = ZW.WarehouseCode from ZnodePortalWarehouse zpw inner join ZnodeWarehouse ZW on zpw.WarehouseId = ZW.WarehouseId  
 where PortalId =@PortalId  
   
    IF OBJECT_ID('tempdb.dbo.##Inventory', 'U') IS NOT NULL   
  DROP TABLE tempdb.dbo.##Inventory  
  
 SELECT @ImportHeadId= ImportHeadId FROM dbo.ZnodeImportHead WHERE Name = 'Inventory'  
 if Isnull(@WarehouseCode ,'') <> ''   
 BEGIN   
  SELECT @TableName = ImportTableName FROM ZnodeImportTableDetail WHERE ImportTableNature = 'Insert' AND ImportHeadId =@ImportHeadId --AND ImportTableName = 'PRDH'  
     SET @TableName = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']'   
   
      SET @InsertColumnName = ''    
   SET @ImportTableColumnName = ''  
   SET @CreateTableScriptSql = ''  
     
   --Create Temp table for price with respective their code   
   SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'InventoryTemplate'  
   SELECT @CreateTableScriptSql = 'CREATE TABLE tempdb.dbo.##Inventory ('+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)'   
   FROM [dbo].[ZnodeImportTemplateMapping]  
   WHERE [ImportTemplateId]= @TemplateId FOR XML PATH ('')),2,4000)+' , GUID nvarchar(255) )'  
    
   EXEC ( @CreateTableScriptSql )  
  
   SET @Sql = '   
   SELECT @InsertColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ITCD.BaseImportColumn +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD   
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  ITD.ImportTableName = ''MAGINV''  AND Isnull(ITCD.BaseImportColumn,'''' ) <> ''''  FOR XML PATH ('''')),2,4000)  
  
   SELECT @ImportTableColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ImportTableColumnName +'']''  ,''NULL'')   
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD   
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  ITD.ImportTableName = ''MAGINV'' AND Isnull(ITCD.BaseImportColumn,'''' ) <> '''' FOR XML PATH ('''')),2,4000)'  
  
   EXEC sp_executesql @SQL, N'@TableName VARCHAR(200),@InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT', @TableName = @TableName, @InsertColumnName = @InsertColumnName OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT  
   
  
   IF( LEN(@InsertColumnName) > 0 )  
   BEGIN  
    SET @SQL = 'INSERT INTO tempdb.dbo.##Inventory ( '+@InsertColumnName+',GUID )  
     SELECT '+ @ImportTableColumnName +',''' + @TblGUID  + '''   
     FROM '+ @TableName + ' PRD '  
     EXEC sp_executesql @SQL  
    SET @SQL = 'Update tempdb.dbo.##Inventory  SET WarehouseCode = ''' + @WarehouseCode + ''''  
    EXEC sp_executesql @SQL  
    --SET @SQL = 'Select * from tempdb..##' + @ListCode  
    --EXEC sp_executesql @SQL  
     
  
    EXEC Znode_ImportData @TableName = 'tempdb..##Inventory' ,@NewGUID = @TblGUID ,@TemplateId = @TemplateId,  
    @UserId = 2,@PortalId = @PortalId,@LocaleId = @LocaleId,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = ''--, @IsDebug =1   
    ,@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 0,@Nextstep_id  = 1 ,@ERPTaskSchedulerId  = @ERPTaskSchedulerId    
    select 'Job Successfully Started'  
   END  
     
 END  
END