CREATE PROCEDURE [dbo].[Znode_ImportProcessPriceData](@TblGUID nvarchar(255) = '',@ERPTaskSchedulerId int  )  
AS  
BEGIN  
  
--EXEC [Znode_ImportProcessPriceData] @TblGUID = '93519c74-f252-40ec-bed9-37ae8270d4da',@ERPTaskSchedulerId  = 14  
--select * from [TPRICE_93519c74-f252-40ec-bed9-37ae8270d4da]  
--EXEC Znode_ImportProcessPriceData](@TblGUID nvarchar(255) = '',@ERPTaskSchedulerId int  )  
--select * into [TPRICE_93519c74-f252-40ec-bed9-37ae8270d4da] from [##TPRICE_93519c74-f252-40ec-bed9-37ae8270d4da]  
--Select * from Tempdb..sysobjects where name  like  '%TPR%'  
  
 SET NOCOUNT ON;  
 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
 Declare @NewuGuId nvarchar(255)  
 set @NewuGuId = newid()  
 Declare @CurrencyId int  
 DECLARE @TemplateId INT , @PortalId INT  
 DECLARE @LocaleId  int = dbo.Fn_GetDefaultLocaleId()  
 SELECT TOP 1 @PortalId  = PortalId FROM dbo.ZnodePortal  
 Select @CurrencyId = CurrencyId  from ZnodeCulture where CultureCode in (Select FeatureValues from   ZnodeGlobalSetting where FeatureName = 'Currency')  
 IF OBJECT_ID('tempdb.dbo.##PriceDetail', 'U') IS NOT NULL  
    DROP TABLE ##PriceDetail  
  
 DECLARE @CreateTableScriptSql NVARCHAR(MAX) = '',  
      @InsertColumnName NVARCHAR(MAX),  
   @ImportTableColumnName NVARCHAR(MAX),  
   @TableName NVARCHAR(500) = 'TPRICE',  
   @Sql NVARCHAR(MAX) = '',  
   @PriceListId int,  
   @ListCode nvarchar(255),  
   @RowNum int,  
   @MaxRowNum int,  
   @FirstStep nvarchar(255),  
   @PriceTableName  nvarchar(255)  
  
  
 SELECT @TableName = ImportTableName FROM ZnodeImportTableDetail WHERE ImportTableNature = 'Insert' AND ImportHeadId =2 --AND ImportTableName = 'PRDH'  
 SET @TableName = 'tempdb..[##' + @TableName + '_' + @TblGUID + ']'  
 --SET @TableName = '[' + @TableName + '_' + @TblGUID + ']'  
  
 IF OBJECT_ID('tempdb.dbo.##PriceListcode', 'U') IS NOT NULL  
  DROP TABLE #PriceListcode  
 CREATE TABLE #PriceListcode (RowNum int Identity, ListCode nvarchar(255), ListName nvarchar(255) , CurrencyId int)  
  
 SET @SQL =  
 'INSERT INTO #PriceListcode ( ListCode,ListName,CurrencyId )  
 SELECT  Distinct ltrim(rtrim(Replace(PRD.PricelistCode,''"'',''''))),ltrim(rtrim(Replace(PRD.PricelistCode,''"'',''''))), '+ Convert (nvarchar(30),@CurrencyId ) + '  FROM ' +@TableName+ ' PRD '  
 EXEC sp_executesql @SQL  
  
 SET @SQL =  
 'INSERT INTO ZnodePriceList ( ListCode,ListName,CurrencyId , CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)  
 SELECT  Distinct PRD.ListCode,PRD.ListCode,'+ Convert (nvarchar(30),@CurrencyId ) + ',2,'''+CONVERT(NVARCHAR(30),@GetDate,121)+''',2,'''+CONVERT(NVARCHAR(30),@GetDate,121)+''' FROM  #PriceListcode PRD  
 WHERE NOT EXISTS ( SELECT TOP 1 1  FROM ZnodePriceList ZPL WHERE ZPL.ListCode = PRD.ListCode ) AND PRD.ListCode is not null '  
 EXEC sp_executesql @SQL  
  
 SET @Rownum  =1  
  
 Select  @MaxRowNum = count(RowNum) from #PriceListcode TPLC INNER JOIN ZnodePriceList ZPL On  
 TPLC.ListCode = ZPL.ListCode  where ZPL.ListCode    is not null  
 AND  Isnull(TPLC.ListCode,'') <> ''  
  
 DECLARE Cur_ListCode CURSOR FOR SELECT ZPL.PriceListId, TPLC.ListCode  
 FROM #PriceListcode TPLC INNER JOIN ZnodePriceList ZPL On  
 TPLC.ListCode = ZPL.ListCode  where ZPL.ListCode    is not null  Order by  TPLC.RowNum  
    OPEN Cur_ListCode  
    FETCH NEXT FROM Cur_ListCode INTO @PriceListId, @ListCode  
    WHILE ( @@FETCH_STATUS = 0 )  
 BEGIN  
  
      SET @InsertColumnName = ''  
   SET @ImportTableColumnName = ''  
   SET @CreateTableScriptSql = ''  
  
      IF OBJECT_ID('tempdb.dbo.##' + @ListCode , 'U') IS NOT NULL  
   BEGIN  
    SET @Sql = 'DROP TABLE tempdb.dbo.##' + @ListCode  
    EXEC sp_executesql @SQL  
   END  
   SELECT @TemplateId= ImportTemplateId FROM dbo.ZnodeImportTemplate WHERE TemplateName = 'PriceTemplate'  
  
   --Create Temp table for price with respective their code  
   SELECT @CreateTableScriptSql = 'CREATE TABLE tempdb..##' + @ListCode + '('+SUBSTRING ((Select ',' +  ISNULL([TargetColumnName] ,'NULL')+ ' nvarchar(max)'  
   FROM [dbo].[ZnodeImportTemplateMapping]  
   WHERE [ImportTemplateId]= @TemplateId FOR XML PATH ('')),2,4000)+' , GUID nvarchar(255) )'  
  
   EXEC ( @CreateTableScriptSql )  
  
   SET @Sql = '  
   SELECT @InsertColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ITCD.BaseImportColumn +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  ITD.ImportTableName = ''TPRICE'' AND ITCD.BaseImportColumn is not null FOR XML PATH ('''')),2,4000)  
  
   SELECT @ImportTableColumnName = SUBSTRING ((Select '','' +  ISNULL(''[''+ ImportTableColumnName +'']''  ,''NULL'')  
   FROM [ZnodeImportTableColumnDetail] ITCD INNER JOIN [ZnodeImportTableDetail] ITD  
   ON ITCD.ImportTableId = ITD.ImportTableId  
   WHERE  ITD.ImportTableName = ''TPRICE''  AND ITCD.BaseImportColumn is not null FOR XML PATH ('''')),2,4000)'  
  
   EXEC sp_executesql @SQL, N'@TableName VARCHAR(200),@InsertColumnName NVARCHAR(MAX) OUTPUT, @ImportTableColumnName  NVARCHAR(MAX) OUTPUT', @TableName = @TableName, @InsertColumnName = @InsertColumnName OUTPUT, @ImportTableColumnName = @ImportTableColumnName OUTPUT  
  
   IF( LEN(@InsertColumnName) > 0 )  
   BEGIN  
    SET @SQL = 'INSERT INTO tempdb..##' + @ListCode+'  ( '+@InsertColumnName+',GUID )  
     SELECT '+ @ImportTableColumnName +',''' + @TblGUID  + '''  
     FROM '+ @TableName + ' PRD  
     WHERE  
     ltrim(rtrim(replace(PRD.PricelistCode,''"'',''''))) = ''' +  @ListCode + ''' AND  
     EXISTS ( SELECT TOP 1 1  FROM ZnodePriceList ZPL WHERE  
     ZPL.ListCode = ltrim(rtrim(replace(PRD.PricelistCode,''"'',''''))) )'  
    EXEC sp_executesql @SQL  
  
    SET @PriceTableName  ='tempdb..[##' + @ListCode +']'  

    If @RowNum = 1  
     Begin  
      IF @RowNum <> @MaxRowNum  
      BEGIN  
       --Print 'Create Job  ' + Convert(nvarchar(100),@RowNum)  
       EXEC Znode_ImportData @TableName = @PriceTableName,@NewGUID = @TblGUID ,@TemplateId = @TemplateId,  
        @UserId = 2,@PortalId = @PortalId, @LocaleId = @LocaleId, @DefaultFamilyId = 0,@PriceListId = @PriceListId, @CountryCode = ''--, @IsDebug =1  
       ,@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 1, @StepName = @ListCode,@ERPTaskSchedulerId  =@ERPTaskSchedulerId  
       SET @FirstStep = @ListCode  
      END  
      ELSE If  @MaxRowNum = 1  
      BEGIN  
       --Print 'Start Job in case of single  ' + Convert(nvarchar(100),@RowNum)  
       EXEC Znode_ImportData @TableName = @PriceTableName,@NewGUID = @TblGUID ,@TemplateId = @TemplateId,  
        @UserId = 2,@PortalId = @PortalId, @LocaleId = @LocaleId, @DefaultFamilyId = 0,@PriceListId = @PriceListId, @CountryCode = ''--, @IsDebug =1  
       ,@IsDoNotCreateJob = 0 , @IsDoNotStartJob = 0,  
        @ERPTaskSchedulerId  = @ERPTaskSchedulerId  

      END  
     END  
    ELSE If @RowNum = @MaxRowNum  
     Begin  
      EXEC Znode_ImportData @TableName = @PriceTableName, @NewGUID =  @TblGUID ,@TemplateId = @TemplateId,  
       @UserId = 2,@PortalId = @PortalId, @LocaleId = @LocaleId, @DefaultFamilyId = 0,@PriceListId = @PriceListId, @CountryCode = ''--, @IsDebug =1  
      ,@IsDoNotCreateJob = 1 , @IsDoNotStartJob = 0, @StepName = @ListCode, @StartStepName  = @FirstStep ,@step_id = @RowNum --, @IsDebug =1  
      ,@Nextstep_id  = 1,@ERPTaskSchedulerId  = @ERPTaskSchedulerId  
     END  
    ELSE  
     BEGIN   
      EXEC Znode_ImportData @TableName = @PriceTableName , @NewGUID = @TblGUID ,@TemplateId = @TemplateId,  
      @UserId = 2,@PortalId = @PortalId, @LocaleId = @LocaleId, @DefaultFamilyId = 0,@PriceListId = @PriceListId, @CountryCode = ''--, @IsDebug =1  
      ,@IsDoNotCreateJob = 1 , @IsDoNotStartJob = 1, @StepName = @ListCode ,@step_id = @RowNum  
      ,@ERPTaskSchedulerId  = @ERPTaskSchedulerId  
     END  
   SET @RowNum = @RowNum +1  
   END  
  
  FETCH NEXT FROM Cur_ListCode INTO  @PriceListId, @ListCode  
 END  
  select 'Job Successfully Started'  
 CLOSE Cur_ListCode  
 DEALLOCATE Cur_ListCode  
END