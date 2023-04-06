CREATE PROCEDURE [dbo].[Znode_ImportSEODetails]
(  
   @TableName NVARCHAR(100), 
   @Status BIT OUT, 
   @UserId INT, 
   @ImportProcessLogId INT, 
   @NewGUId NVARCHAR(200), 
   @LocaleId INT= 1,
   @PortalId INT ,
   @CsvColumnString NVARCHAR(max)
)  
AS  
 --------------------------------------------------------------------------------------  
 -- Summary :  Import SEO Details  
   
 -- Unit Testing :   
 --------------------------------------------------------------------------------------  
  
BEGIN  
BEGIN TRAN A;  
BEGIN TRY  
   
	DECLARE @MessageDisplay NVARCHAR(100), @SSQL NVARCHAR(max);  
	DECLARE @GetDate DATETIME= dbo.Fn_GetDate();  
    
    
	DECLARE @CMSSEOTypeProduct INT ,@CMSSEOTypeCategory INT  
  
	SELECT @CMSSEOTypeProduct = CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Product'  
	SELECT @CMSSEOTypeCategory = CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Category'  
  
  
	-- Three type of import required three table varible for product , category AND brAND  
	DECLARE @InsertSEODetails TABLE  
	(   
		RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, ImportType varchar(20), Code NVARCHAR(300),   
		IsRedirect BIT ,MetaInformation NVARCHAR(max),PortalId INT ,SEOUrl NVARCHAR(max),IsActive varchar(10),  
		SEOTitle NVARCHAR(max),SEODescription NVARCHAR(max),SEOKeywords NVARCHAR(max),   
		RedirectFROM NVARCHAR(max),RedirectTo NVARCHAR(max), EnableRedirection BIT, CanonicalURL VARCHAR(200),   
		RobotTag VARCHAR(50), GUID NVARCHAR(400)  
	);  
  
	DECLARE @InsertSEODetailsOFProducts TABLE  
	(   
		RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, ImportType varchar(20), Code NVARCHAR(300),   
		IsRedirect BIT ,MetaInformation NVARCHAR(max),PortalId INT ,SEOUrl NVARCHAR(max),IsActive varchar(10),  
		SEOTitle NVARCHAR(max),SEODescription NVARCHAR(max),SEOKeywords NVARCHAR(max),  
		RedirectFROM NVARCHAR(max),RedirectTo NVARCHAR(max), EnableRedirection BIT, CanonicalURL VARCHAR(200),  
		RobotTag VARCHAR(50),GUID NVARCHAR(400) 
	);  
  
	DECLARE @InsertSEODetailsOFCategory TABLE  
	(   
		RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, ImportType varchar(20), Code NVARCHAR(300),   
		IsRedirect BIT ,MetaInformation NVARCHAR(max),PortalId INT ,SEOUrl NVARCHAR(max),IsActive varchar(10),  
		SEOTitle NVARCHAR(max),SEODescription NVARCHAR(max),SEOKeywords NVARCHAR(max),  
		RedirectFROM NVARCHAR(max),RedirectTo NVARCHAR(max), EnableRedirection BIT, CanonicalURL VARCHAR(200), 
		RobotTag VARCHAR(50),GUID NVARCHAR(400)
	);  
  
	DECLARE @InsertSEODetailsOFBrAND TABLE  
	(   
		RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, ImportType varchar(20), Code NVARCHAR(300),   
		IsRedirect BIT ,MetaInformation NVARCHAR(max),PortalId INT ,SEOUrl NVARCHAR(max),IsActive varchar(10),  
		SEOTitle NVARCHAR(max),SEODescription NVARCHAR(max),SEOKeywords NVARCHAR(max),   
		RedirectFROM NVARCHAR(max),RedirectTo NVARCHAR(max), EnableRedirection BIT, CanonicalURL VARCHAR(200),  
		RobotTag VARCHAR(50),GUID NVARCHAR(400) 
	);  
  
    
	DECLARE @InsertedZnodeCMSSEODetail TABLE  
	(   
		CMSSEODetailId INT , SEOCode Varchar(4000), CMSSEOTypeId INT  
	);  
    
	--SET @SSQL = 'SELECT RowNumber,ImportType,Code,IsRedirect,MetaInformation,SEOUrl,IsActive,SEOTitle,SEODescription,SEOKeywords,GUID  FROM '+@TableName;  
	SET @SSQL = 'SELECT RowNumber,'+@CsvColumnString+',GUID  FROM '+@TableName;  
  
	INSERT INTO @InsertSEODetails(RowNumber,ImportType,Code,IsRedirect,MetaInformation,SEOUrl,IsActive,SEOTitle,SEODescription,SEOKeywords,RedirectFrom,RedirectTo,EnableRedirection,CanonicalURL,RobotTag,GUID )  
	EXEC sys.sp_sqlexec @SSQL;  
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '30', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii   
	WHERE ii.SEOURL IN (SELECT ISD.SEOURL FROM @InsertSEODetails ISD Group by ISD.SEOUrl having count(*) > 1 ) 
	    
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '10', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii   
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetail ZCSD WHERE ZCSD.SEOUrl = ii.SEOUrl AND ZCSD.PortalId = @PortalId
	AND ZCSD.SEOCode <> ii.Code  AND EXISTS  
	(SELECT TOP 1 1 FROM ZnodeCMSSEODetailLocale dl WHERE dl.CMSSEODetailId = ZCSD.CMSSEODetailId AND dl.LocaleId = @LocaleId  
	AND dl.SEODescription = ii.SEODescription AND dl.SEOTitle = ii.SEOTitle AND dl.SEOKeywords = ii.SEOKeywords))   
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '53', 'RedirectFrom', RedirectFrom, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii   
	WHERE ii.RedirectFROM IN (SELECT ISD.RedirectFROM FROM @InsertSEODetails ISD Group by ISD.RedirectFROM having count(*) > 1 )   
	AND (ii.RedirectFROM <> '' )

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '35', 'RedirectFrom\RedirectTo', RedirectFROM + '  ' + RedirectTo  , @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii   
	WHERE ii.RedirectFROM = ii.RedirectTo  
	AND (ii.RedirectFROM <> '' AND ii.RedirectTo <> '' )
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '133', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii  
	WHERE LTRIM(RTRIM(ISNULL(ii.SEOUrl,''))) like '% %' -----space not allowed  
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '103', 'ImportType', ImportType, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii  
	WHERE ii.ImportType NOT IN   
	(  
		SELECT NAME FROM ZnodeCMSSEOType WHERE NAME NOT IN ('Content Page','BlogNews','Brand')  
	);  

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '68', 'IsActive', IsActive, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii  
	WHERE ii.IsActive not IN ('True','1','Yes','FALSE','0','No')
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '9', 'RobotTag', RobotTag, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetails AS ii  
	WHERE ii.RobotTag not IN ( 'INDEX_FOLLOW','NOINDEX_NOFOLLOW','NOINDEX_FOLLOW','INDEX_NOFOLLOW','1','2','3','4') 
	AND ISNULL(ii.RobotTag,'')<>''

	UPDATE ZIL
	SET ZIL.ColumnName =   ZIL.ColumnName + ' [ SEOCode - ' + ISNULL(Code,'') + ' ] '
	FROM ZnodeImportLog ZIL 
	INNER JOIN @InsertSEODetails IPA ON (ZIL.RowNumber = IPA.RowNumber)
	WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

	
	-------------------------------------------------------------------------------------------------------------------------------  
  
	INSERT INTO @InsertSEODetailsOFProducts(  RowNumber , ImportType , Code ,   
	IsRedirect ,MetaInformation ,SEOUrl ,IsActive ,  
	SEOTitle ,SEODescription ,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, CanonicalURL, RobotTag, GUID )  
	SELECT RowNumber , ImportType , Code , IsRedirect ,MetaInformation ,SEOUrl , 
	CASE WHEN IsActive IN ('True','1','Yes') 
	Then 1 
	ELSE 0
	END as IsActive, SEOTitle ,SEODescription ,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, CanonicalURL, RobotTag, GUID  
	FROM @InsertSEODetails WHERE ImportType = 'Product'  
  
  
	INSERT INTO @InsertSEODetailsOFCategory( RowNumber , ImportType , Code ,   
	IsRedirect ,MetaInformation,SEOUrl ,IsActive ,  
	SEOTitle ,SEODescription ,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, CanonicalURL, RobotTag , GUID )  
	SELECT RowNumber , ImportType , Code , IsRedirect ,MetaInformation ,SEOUrl , 
		CASE WHEN IsActive IN ('True','1','Yes') Then 1 ELSE 0 END as IsActive, SEOTitle ,
		SEODescription ,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, CanonicalURL, RobotTag, GUID  
	FROM @InsertSEODetails WHERE ImportType = 'Category'  
  
	INSERT INTO @InsertSEODetailsOFBrand( RowNumber , ImportType , Code ,   
	IsRedirect ,MetaInformation ,SEOUrl ,IsActive ,  
	SEOTitle ,SEODescription ,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, CanonicalURL, RobotTag , GUID )  
	SELECT RowNumber , ImportType , Code , IsRedirect ,MetaInformation ,SEOUrl ,
		CASE WHEN IsActive IN ('True','1','Yes') Then 1 ELSE 0 END as IsActive, SEOTitle ,
		SEODescription ,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, CanonicalURL, RobotTag, GUID  
	FROM @InsertSEODetails WHERE ImportType = 'Brand'  
  
  
	-- start Functional Validation   
	--1. Product  
	--2. Category  
	--3. Content Page  
	--4. BrAND  
	-----------------------------------------------  
  
    
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '103', 'SKU', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetailsOFProducts AS ii  
	WHERE ii.CODE NOT IN   
	(  
		SELECT ZPAVL.AttributeValue  
		FROM ZnodePimAttributeValue ZPAV   
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId  
		INNER JOIN ZnodePimAttribute ZPA on ZPAV.PimAttributeId = ZPA.PimAttributeId  
		WHERE ZPA.AttributeCode = 'SKU' AND ZPAVL.AttributeValue IS NOT NULL   
	)  AND ImportType = 'Product';  
  

	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '103', 'Category', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetailsOFCategory AS ii  
	WHERE ii.CODE NOT IN   
	(  
		SELECT ZPCAVL.CategoryValue  
		FROM ZnodePimCategoryAttributeValue ZPCAV   
		INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL on ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId  
		INNER JOIN ZnodePimAttribute ZPA on ZPCAV.PimAttributeId = ZPA.PimAttributeId  
		WHERE ZPA.AttributeCode = 'CategoryCode' AND ZPCAVL.CategoryValue IS NOT NULL  
	)  AND ImportType = 'Category';  
  
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
	SELECT '103', 'Brand', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
	FROM @InsertSEODetailsOFBrAND AS ii  
	WHERE ii.CODE NOT IN   
	(  
		SELECT BrandCode FROM ZnodeBrandDetails WHERE BrandCode IS NOT NULL  
	)  AND ImportType = 'Brand';  
    
	
	--Note : Content page import is not required   
    
	-- End Function Validation    
	-----------------------------------------------  
	--- Delete Invalid Data after functional validatin    
 
	DELETE FROM @InsertSEODetails  
	WHERE RowNumber IN  
	(  
		SELECT DISTINCT   
		RowNumber  
		FROM ZnodeImportLog  
		WHERE ImportProcessLogId = @ImportProcessLogId  AND RowNumber is not null   
	);  


	DELETE FROM @InsertSEODetailsOFProducts  
	WHERE RowNumber IN  
	(  
		SELECT DISTINCT   
		RowNumber  
		FROM ZnodeImportLog  
		WHERE ImportProcessLogId = @ImportProcessLogId  AND RowNumber is not null   
	);  
  
	DELETE FROM @InsertSEODetailsOFCategory  
	WHERE RowNumber IN  
	(  
		SELECT DISTINCT   
		RowNumber  
		FROM ZnodeImportLog  
		WHERE ImportProcessLogId = @ImportProcessLogId  AND RowNumber is not null   
	);  
  
	DELETE FROM @InsertSEODetailsOFBrAND  
	WHERE RowNumber IN  
	(  
		SELECT DISTINCT   
		RowNumber  
		FROM ZnodeImportLog  
		WHERE ImportProcessLogId = @ImportProcessLogId  AND RowNumber is not null   
	);  
  
 
	-- Insert Product Data   
	IF EXISTS (SELECT TOP 1 1 FROM @InsertSEODetailsOFProducts)  
	BEGIN  
		UPDATE ZCSD SET ZCSD.IsRedirect = ISD.IsRedirect ,  
			ZCSD.MetaInformation =  ISD.MetaInformation,  
			ZCSD.SEOUrl=  ISD.SEOUrl,  
			ZCSD.IsPublish = 0  
		FROM @InsertSEODetailsOFProducts ISD    
		INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code  
		INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId  
		WHERE  ZCSD.PortalId  =@PortalId;  
     
		UPDATE ZCSDL SET ZCSDL.SEOTitle = ISD.SEOTitle  
			,ZCSDL.SEODescription = ISD.SEODescription  
			,ZCSDL.SEOKeywords= ISD.SEOKeywords
			,ZCSDL.CanonicalURL = ISD.CanonicalURL
			,ZCSDL.RobotTag = ISD.RobotTag  
		FROM @InsertSEODetailsOFProducts ISD    
		INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code  
		INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId  
		WHERE  ZCSD.PortalId = @PortalId AND ZCSDL.LocaleId = @LocaleId;   
  
		----Making product as draft if SEOUrl is changed for part of partial publish
		UPDATE ZPP SET ZPP.PublishStateId = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName = 'Draft')
		FROM ZnodePimProduct ZPP
		INNER JOIN ZnodePimAttributeValue ZPAV ON ZPP.PimProductId = ZPAV.PimProductId
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		WHERE EXISTS (SELECT * FROM ZnodePimAttribute zpa WHERE zpa.AttributeCode = 'SKU' AND ZPAV.PimAttributeId = zpa.PimAttributeId)
		AND EXISTS(SELECT * FROM @InsertSEODetailsOFProducts ISD    
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code
			WHERE ZPAVL.AttributeValue = ZCSD.SEOCode)
     
		INSERT INTO ZnodeCMSSEODetailLocale (CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)  
		SELECT DISTINCT CSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate, CanonicalURL,RobotTag  
		FROM ZnodeCMSSEODetail CSD  
		INNER JOIN @InsertSEODetailsOFProducts ISD ON CSD.SEOCode = ISD.Code AND CSD.CMSSEOTypeId = @CMSSEOTypeProduct   
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetailLocale CSDL WHERE CSDL.LocaleId = @LocaleId AND CSD.CMSSEODetailId = CSDL.CMSSEODetailId )  
		AND CSD.portalId = @PortalId  
  
     
		DELETE FROM @InsertedZnodeCMSSEODetail  

		INSERT INTO ZnodeCMSSEODetail(CMSSEOTypeId,SEOCode,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)    
		OUTPUT Inserted.CMSSEODetailId,Inserted.SEOCode,Inserted.CMSSEOTypeId INTO @InsertedZnodeCMSSEODetail    
		SELECT DISTINCT @CMSSEOTypeProduct,ISD.Code , ISD.IsRedirect,ISD.MetaInformation,@PortalId,ISD.SEOUrl,@USerId, @GetDate,@USerId, @GetDate FROM   
		@InsertSEODetailsOFProducts ISD    
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetail ZCSD WHERE ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code AND  ZCSD.PortalId =@PortalId   );  
    
		INSERT INTO ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)  
		SELECT DISTINCT IZCSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate,CanonicalURL,RobotTag   
		FROM @InsertedZnodeCMSSEODetail IZCSD   
		INNER JOIN @InsertSEODetailsOFProducts ISD ON IZCSD.SEOCode = ISD.Code   

		----Making product as draft if SEOUrl is inserted for part of partial publish
		UPDATE ZPP SET ZPP.PublishStateId = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName = 'Draft')
		FROM ZnodePimProduct ZPP
		INNER JOIN ZnodePimAttributeValue ZPAV ON ZPP.PimProductId = ZPAV.PimProductId
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		WHERE EXISTS (SELECT * FROM ZnodePimAttribute zpa WHERE zpa.AttributeCode = 'SKU' AND ZPAV.PimAttributeId = zpa.PimAttributeId)
		AND EXISTS(SELECT * FROM @InsertedZnodeCMSSEODetail IZCSD WHERE ZPAVL.AttributeValue = IZCSD.SEOCode)
  
		-----RedirectUrlInsert  
		INSERT INTO ZnodeCMSUrlRedirect ( RedirectFrom,RedirectTo,IsActive,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)  
		SELECT RedirectFrom,RedirectTo,
		EnableRedirection,@PortalId as PortalId ,2 as CreatedBy,@GetDate as CreatedDate,2 as ModifiedBy,@GetDate as ModifiedDate  
		FROM @InsertSEODetailsOFProducts SDP  
		WHERE IsRedirect = 1
		AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSUrlRedirect ZCR 
			WHERE ZCR.RedirectFROM = SDP.RedirectFROM AND ZCR.RedirectTo = SDP.RedirectTo)
			AND (SDP.RedirectFROM <> '' AND SDP.RedirectTo <> '' )
  
	END  
  
	-- Insert Category Data   
	IF EXISTS (SELECT TOP 1 1 FROM @InsertSEODetailsOFCategory)  
	BEGIN  
  
		UPDATE ZCSD SET ZCSD.IsRedirect = ISD.IsRedirect ,  
			ZCSD.MetaInformation =  ISD.MetaInformation,  
			ZCSD.SEOUrl=  ISD.SEOUrl,  
			ZCSD.IsPublish = 0  
		FROM @InsertSEODetailsOFCategory ISD    
		INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeCategory AND ZCSD.SEOCode = ISD.Code  
		INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId  
		WHERE  ZCSD.PortalId  =@PortalId;  
     
     
		UPDATE ZCSDL SET ZCSDL.SEOTitle = ISD.SEOTitle  
			,ZCSDL.SEODescription = ISD.SEODescription  
			,ZCSDL.SEOKeywords= ISD.SEOKeywords 
			,CanonicalURL = ISD.CanonicalURL
			,RobotTag = ISD.RobotTag
		FROM @InsertSEODetailsOFCategory ISD    
		INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeCategory AND ZCSD.SEOCode = ISD.Code  
		INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId  
		WHERE  ZCSD.PortalId  =@PortalId AND ZCSDL.LocaleId = @LocaleId;   
  
		INSERT INTO ZnodeCMSSEODetailLocale (CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)  
		SELECT DISTINCT CSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate,CanonicalURL,RobotTag  
		FROM ZnodeCMSSEODetail CSD  
		INNER JOIN @InsertSEODetailsOFProducts ISD ON CSD.SEOCode = ISD.Code AND CSD.CMSSEOTypeId = @CMSSEOTypeCategory   
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetailLocale CSDL WHERE CSDL.LocaleId = @LocaleId AND CSD.CMSSEODetailId = CSDL.CMSSEODetailId )  
		AND CSD.portalId = @PortalId  
  
  
		DELETE FROM @InsertedZnodeCMSSEODetail  

		INSERT INTO ZnodeCMSSEODetail(CMSSEOTypeId,SEOCode,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)    
		OUTPUT Inserted.CMSSEODetailId,Inserted.SEOCode,Inserted.CMSSEOTypeId INTO @InsertedZnodeCMSSEODetail    
		SELECT DISTINCT @CMSSEOTypeCategory,ISD.Code , ISD.IsRedirect,ISD.MetaInformation,@PortalId,ISD.SEOUrl,@USerId, @GetDate,@USerId, @GetDate   
		FROM @InsertSEODetailsOFCategory ISD    
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetail ZCSD WHERE ZCSD.CMSSEOTypeId = @CMSSEOTypeCategory AND ZCSD.SEOCode  = ISD.Code AND ZCSD.PortalId = @PortalId );  
  
		INSERT INTO ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CanonicalURL,RobotTag)  
		SELECT DISTINCT IZCSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate,CanonicalURL,RobotTag   
		FROM @InsertedZnodeCMSSEODetail IZCSD   
		INNER JOIN @InsertSEODetailsOFCategory ISD ON IZCSD.SEOCode = ISD.Code   
		WHERE IZCSD.CMSSEOTypeId =@CMSSEOTypeCategory    

		-----RedirectUrlInsert  
		INSERT INTO ZnodeCMSUrlRedirect ( RedirectFrom,RedirectTo,IsActive,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)  
		SELECT RedirectFrom,RedirectTo,
		EnableRedirection,@PortalId as PortalId ,2 as CreatedBy,@GetDate as CreatedDate,2 as ModifiedBy,@GetDate as ModifiedDate  
		FROM @InsertSEODetailsOFCategory SDP  
		WHERE IsRedirect = 1 
		AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSUrlRedirect ZCR 
			WHERE ZCR.RedirectFROM = SDP.RedirectFROM AND ZCR.RedirectTo = SDP.RedirectTo)
			AND (SDP.RedirectFROM <> '' AND SDP.RedirectTo <> '' )
	END  

	-- UPDATE Record count IN log 
	DECLARE @FailedRecordCount BIGINT
	DECLARE @SuccessRecordCount BIGINT
	SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
	SELECT @SuccessRecordCount = count(DISTINCT RowNumber) FROM @InsertSEODetails
	UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount , 
	TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
	WHERE ImportProcessLogId = @ImportProcessLogId;

	SET @GetDate = dbo.Fn_GetDate();
	--Updating the import process status
	UPDATE ZnodeImportProcessLog
	SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
						WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
						WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
					END, 
		ProcessCompletedDate = @GetDate
	WHERE ImportProcessLogId = @ImportProcessLogId; 
  
COMMIT TRAN A;  
END TRY  
BEGIN CATCH  
ROLLBACK TRAN A;

	SET @Status = 0;  
	SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE(); 

	DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportSEODetails @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(max)) +',@PortalId='+CAST(@PortalId AS VARCHAR(max)) +',@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max)) ;

	---Import process updating fail due to database error
	UPDATE ZnodeImportProcessLog
	SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
	WHERE ImportProcessLogId = @ImportProcessLogId;

	---Loging error for Import process due to database error
	INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

	--Updating total AND fail record count
	UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
	TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId)
	WHERE ImportProcessLogId = @ImportProcessLogId;

	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_ImportSEODetails',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
    
END CATCH;  
END;