CREATE   PROCEDURE [dbo].[Znode_ImportProcessCustomer](@TblGUID nvarchar(255) = '' ,@ERPTaskSchedulerId  int )  
AS  
BEGIN  
  
 SET NOCOUNT ON;  
 Declare @NewuGuId nvarchar(255)  
 set @NewuGuId = newid()  
 Declare @CurrencyId int ,@PortalId int,@TemplateId INT ,@ImportHeadId INT  
  
 DECLARE @LocaleId  int = dbo.Fn_GetDefaultLocaleId()  
 SELECT TOP 1 @PortalId  = PortalId FROM dbo.ZnodePortal  
  
 Select @CurrencyId = CurrencyId  from ZnodeCulture where CultureCode in (Select FeatureValues from   ZnodeGlobalSetting where FeatureName = 'Currency')  
  
 DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '',  
      @InsertColumnName NVARCHAR(MAX),  
   @ImportTableColumnName NVARCHAR(MAX),  
   @TableName NVARCHAR(255) = 'MAGSOLD',  
   @Sql NVARCHAR(MAX) = '',  
   @PriceListId int,  
   @RowNum int,  
   @MaxRowNum int,  
   @FirstStep nvarchar(255),  
   @PriceTableName  nvarchar(255),  
   @WarehouseCode varchar(100)  
  
    IF OBJECT_ID('tempdb..##Customer', 'U') IS NOT NULL  
  DROP TABLE tempdb.dbo.##Customer  
  
    IF OBJECT_ID('tempdb.dbo.##CustomerAddress', 'U') IS NOT NULL  
  DROP TABLE tempdb.dbo.##CustomerAddress  
  
  --SELECT @CustomerTableName = ImportTableName FROM ZnodeImportTableDetail WHERE ImportTableNature = 'Insert' AND ImportHeadId =6 --AND ImportTableName = 'PRDH'  
  --SELECT @CustomerAddTableName = ImportTableName FROM ZnodeImportTableDetail WHERE ImportTableNature = 'Insert' AND ImportHeadId =7 --AND ImportTableName = 'PRDH'  
  
     SET @TableName = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']'  
     -- User Data  
      SET @InsertColumnName = ''  
   SET @ImportTableColumnName = ''  
   SET @CreateTableScriptSql = ''  
  
   --Create Temp table for customer  
   SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'CustomerTemplate'  
   SELECT @ImportHeadId= ImportHeadId FROM dbo.ZnodeImportHead WHERE Name = 'Customer'  
  
   SELECT @CreateTableScriptSql = 'CREATE TABLE tempdb..##Customer ('+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)'  
   FROM [dbo].[ZnodeImportTemplateMapping]  
   WHERE [ImportTemplateId]= @TemplateId FOR XML PATH ('')),2,4000)+' , GUID nvarchar(255) )'  
   EXEC ( @CreateTableScriptSql )  
  
   SET @Sql = '  
   SELECT @InsertColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ITCD.BaseImportColumn +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  
   ITD.ImportTableId in (SELECT ImportTableId FROM ZnodeImportTableDetail WHERE ImportTableNature = ''Insert''  
   AND ImportHeadId = @ImportHeadId AND ImportTableName = ''MAGSOLD'' )  
   AND ITD.ImportTableName = ''MAGSOLD''  
   AND Isnull(ITCD.BaseImportColumn,'''' ) <> ''''  FOR XML PATH ('''')),2,4000)  
  
   SELECT @ImportTableColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ImportTableColumnName +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  
   ITD.ImportTableId in (SELECT ImportTableId FROM ZnodeImportTableDetail WHERE ImportTableNature = ''Insert''  
   AND ImportHeadId = @ImportHeadId AND ImportTableName = ''MAGSOLD'' )  
   AND ITD.ImportTableName = ''MAGSOLD'' AND Isnull(ITCD.BaseImportColumn,'''' ) <> '''' FOR XML PATH ('''')),2,4000)'  
  
   EXEC sp_executesql @SQL, N'@ImportHeadId int , @TableName VARCHAR(200),@InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT',@ImportHeadId = @ImportHeadId ,  @TableName = @TableName, @InsertColumnName = @InsertColumnName
 OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT  
  
   IF( LEN(@InsertColumnName) > 0 )  
   BEGIN  
    SET @SQL = 'INSERT INTO tempdb..##Customer ( '+@InsertColumnName+',GUID )  
     SELECT '+ @ImportTableColumnName +',''' + @TblGUID  + '''  
     FROM '+ @TableName + ' PRD '  
     EXEC sp_executesql @SQL  
  
    SET @SQL = 'Update tempdb..##Customer  SET IsActive =  1 , LastName = ISNULL(LastName,''.'') , Email = UserName '  
    EXEC sp_executesql @SQL  
  
  
    EXEC Znode_ImportData @TableName = 'tempdb..##Customer' ,@NewGUID = @TblGUID ,@TemplateId = @TemplateId,  
     @UserId = 2,@PortalId = 1,@LocaleId = 1,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = ''--, @IsDebug =1  
    ,@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 1, @StepName = 'Import' ,@ERPTaskSchedulerId  = @ERPTaskSchedulerId  
   END  
  
   -- User Address Data  
   SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'CustomerAddressTemplate'  
   SELECT @ImportHeadId= ImportHeadId FROM dbo.ZnodeImportHead WHERE Name = 'CustomerAddress'  
   SET @InsertColumnName = ''  
   SET @ImportTableColumnName = ''  
   SET @CreateTableScriptSql = ''  
  
   --Create Temp table for customer Address  
   SELECT @CreateTableScriptSql = 'CREATE TABLE tempdb..##CustomerAddress ('+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)'  
   FROM [dbo].[ZnodeImportTemplateMapping]  
   WHERE [ImportTemplateId]= @TemplateId FOR XML PATH ('')),2,4000)+' , GUID nvarchar(255) )'  
   EXEC ( @CreateTableScriptSql )  
  
   SET @Sql = '  
   SELECT @InsertColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ITCD.BaseImportColumn +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  
   ITD.ImportTableId in (SELECT ImportTableId FROM ZnodeImportTableDetail WHERE ImportTableNature = ''Update''  
   AND ImportHeadId =@ImportHeadId AND ImportTableName = ''MAGSOLD'' )  
   AND ITD.ImportTableName = ''MAGSOLD''  AND Isnull(ITCD.BaseImportColumn,'''' ) <> ''''  FOR XML PATH ('''')),2,4000)  
  
   SELECT @ImportTableColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ImportTableColumnName +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  
   ITD.ImportTableId in (SELECT ImportTableId FROM ZnodeImportTableDetail WHERE ImportTableNature = ''Update''  
   AND ImportHeadId =@ImportHeadId AND ImportTableName = ''MAGSOLD'' )  
   AND ITD.ImportTableName = ''MAGSOLD'' AND Isnull(ITCD.BaseImportColumn,'''' ) <> '''' FOR XML PATH ('''')),2,4000)'  
  
   EXEC sp_executesql @SQL, N'@ImportHeadId int ,@TableName VARCHAR(200),@InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT',@ImportHeadId = @ImportHeadId,@TableName = @TableName, @InsertColumnName = @InsertColumnName OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT  
  
   IF( LEN(@InsertColumnName) > 0 )  
   BEGIN  
    SET @SQL = 'INSERT INTO tempdb..##CustomerAddress ( '+@InsertColumnName+',GUID )  
     SELECT '+ @ImportTableColumnName +',''' + @TblGUID  + '''  
     FROM '+ @TableName + ' PRD '  
     EXEC sp_executesql @SQL  
    SET @SQL = 'Update tempdb..##CustomerAddress  SET IsActive =  1 '  
    EXEC sp_executesql @SQL  
   END  
  
   --Append address data from shipping table  
  
  
   SET @InsertColumnName = ''  
   SET @ImportTableColumnName = ''  
   Declare @CustomerTableName  nvarchar(255)  
   SET @CustomerTableName = @TableName  
   SET @TableName = 'MAGSHIP'  
   SET @TableName = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']'  
   SET @Sql = '  
   SELECT @InsertColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ITCD.BaseImportColumn +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  
   ITD.ImportTableId in (SELECT ImportTableId FROM ZnodeImportTableDetail WHERE ImportTableNature = ''Update''  
   AND ImportHeadId =@ImportHeadId AND ImportTableName = ''MAGSHIP'' )  
   AND ITD.ImportTableName = ''MAGSHIP''  AND Isnull(ITCD.BaseImportColumn,'''' ) <> ''''  FOR XML PATH ('''')),2,4000)  
  
   SELECT @ImportTableColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ImportTableColumnName +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  
   ITD.ImportTableId in (SELECT ImportTableId FROM ZnodeImportTableDetail WHERE ImportTableNature = ''Update''  
   AND ImportHeadId =@ImportHeadId AND ImportTableName = ''MAGSHIP'' )  
   AND ITD.ImportTableName = ''MAGSHIP'' AND Isnull(ITCD.BaseImportColumn,'''' ) <> '''' FOR XML PATH ('''')),2,4000)'  
  
   EXEC sp_executesql @SQL, N'@ImportHeadId int , @TableName VARCHAR(200),@InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT', @ImportHeadId=@ImportHeadId , @TableName = @TableName, @InsertColumnName = @InsertColumnName OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT  
  
   IF( LEN(@InsertColumnName) > 0 )  
   BEGIN  
    SET @SQL = 'INSERT INTO tempdb..##CustomerAddress ( '+@InsertColumnName+',GUID )  
     SELECT '+ @ImportTableColumnName +',''' + @TblGUID  + '''  
     FROM '+ @TableName + ' PRD '  
     EXEC sp_executesql @SQL  
  
    SET @SQL = 'Update tempdb..##CustomerAddress  SET IsActive =  1 '  
    EXEC sp_executesql @SQL  
  
    SET @SQL = 'Update A SET A.UserName = b.[EMAIL LOGON ID] from tempdb..##CustomerAddress A INNER JOIN '+@CustomerTableName+' B ON  
                A.ExternalId = b.[Sold-to number] AND A.UserName is null   '  
    EXEC sp_executesql @SQL  
  
    SET @SQL = 'Update tempdb..##CustomerAddress  SET LastName = ISNULL(LastName,''.''),FirstName  = ISNULL(UserName,'''')'  
    EXEC sp_executesql @SQL  
  
    EXEC Znode_ImportData @TableName = 'tempdb..##CustomerAddress' ,@NewGUID = @TblGUID ,@TemplateId = @TemplateId,  
    @UserId = 2,@PortalId = 1,@LocaleId = 1,@DefaultFamilyId = 0,@PriceListId = 0, @CountryCode = ''--, @IsDebug =1  
    ,@IsDoNotCreateJob = 1 , @IsDoNotStartJob = 0, @StepName = 'Import1', @StartStepName  ='Import',@step_id = 2  
       ,@Nextstep_id  = 1,@ERPTaskSchedulerId  =@ERPTaskSchedulerId  
    select 'Job Successfully Started'  
   END  
  
END