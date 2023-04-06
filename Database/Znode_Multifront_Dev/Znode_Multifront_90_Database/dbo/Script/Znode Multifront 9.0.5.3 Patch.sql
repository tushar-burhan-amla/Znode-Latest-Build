
IF EXISTS (SELECT TOP 1 1 FROM Sys.Tables WHERE Name = 'ZnodeMultifront')
BEGIN 
 IF EXISTS (SELECT TOP 1 1 FROM ZnodeMultifront where BuildVersion =   9053  )
 BEGIN 
 PRINT 'Script is already executed....'
  SET NOEXEC ON 
 END 
END
ELSE 
BEGIN 
   SET NOEXEC ON
END 
INSERT INTO [dbo].[ZnodeMultifront] ( [VersionName], [Descriptions], [MajorVersion], [MinorVersion], [LowerVersion], [BuildVersion], [PatchIndex], [CreatedBy], 
[CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES ( N'Znode_Multifront_9_0_5_3', N'Upgrade Patch GA Release by 905',9,0,5,9053,0,2, GETDATE(),2, GETDATE())
GO 
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetCatalogProductSEODetail')
BEGIN 
	DROP PROCEDURE Znode_GetCatalogProductSEODetail
END 
GO


CREATE PROCEDURE [dbo].[Znode_GetCatalogProductSEODetail]
( 
  @WhereClause      NVARCHAR(MAX),
  @Rows             INT           = 100,
  @PageNo           INT           = 1,
  @Order_BY         VARCHAR(1000) = '',
  @RowsCount        INT OUT,
  @LocaleId         INT           = 1,
  @PortalId			INT
 
  )
AS
   
/*
	   Summary:  Get product List  Catalog / category / respective product list   		   
	   Unit Testing   
	   begin tran
	   declare @p7 int = 0  
	   EXEC Znode_GetCatalogProductSEODetail @WhereClause=N'',@Rows=100,@PageNo=1,@Order_By=N'',
	   @RowsCount=@p7 output,@PortalId= 1 ,@LocaleId=1 
	   rollback tran
	  
	     declare @p7 int = 0  
	   EXEC Znode_GetCatalogProductSEODetails @WhereClause=N'',@Rows=10,@PageNo=1,@Order_By=N'',
	   @RowsCount=@p7 output,@PortalId= 5 ,@LocaleId=1 


    */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE  @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @SQL NVARCHAR(MAX), 
					 @PimProductId TransferId,
					 @PimAttributeId VARCHAR(MAX)
					
             DECLARE @TransferPimProductId TransferId 
		
	
			IF OBJECT_ID('TEMPDB..#ProductDetail') IS NOT NULL
			DROP TABLE #ProductDetail

			IF OBJECT_ID('TEMPDB..##TempProductDetail') IS NOT NULL
			DROP TABLE ##TempProductDetail

			IF OBJECT_ID('TEMPDB..#znodeCatalogProduct') IS NOT NULL
		DROP TABLE #znodeCatalogProduct

			Declare @PimCatalogId INT

			SELECT @PimCatalogId = PimCatalogId 
			FROM ZnodePortalCatalog ZPC
			INNER JOIN ZnodePublishCatalog PC ON ZPC.PublishCatalogId = pc.PublishCatalogId WHERE PortalId = @PortalId
				
                SELECT  PimProductid,SKU,ProductName,ProductImage,IsActive,LocaleId
				INTO #ProductDetail
				 FROM 
				 (
				 SELECT c.pimproductId,PA.attributecode,e.AttributeValue,e.LocaleId
				 FROM
				 znodePimProduct c 
				 inner join ZnodePimAttributeValue d on (c.PimProductid = d.PimProductid)
				 inner join ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)
				 inner join ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)
				 where  PA.Attributecode IN ('SKU','ProductName','ProductImage','IsActive')
				-- AND e.localeid = @LocaleId
				 ) piv PIVOT(MAX(AttributeValue) FOR AttributeCode in ( SKU,ProductName,ProductImage,IsActive))AS PVT
				


		SET @SQL = 
		'
		--DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))
		--INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
		--	 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()

		select distinct PimCatalogId,PimProductId into #znodeCatalogProduct
	FROm ZnodePimCatalogCategory

		DECLARE @TBL_MediaValue TABLE (PimAttributeValueId INT,PimProductId INT,MediaPath INT,PimAttributeId INt,LocaleId INT )
		INSERT INTO @TBL_MediaValue
		SELECT ZPAV.PimAttributeValueId	,ZPAV.PimProductId	,ZPPAM.MediaId MediaPath,ZPAV.PimAttributeId , 	ZPPAM.LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN ZnodePimProductAttributeMedia ZPPAM ON ( ZPPAM.PimAttributeValueId = ZPAV.PimAttributeValueId)
					INNER JOIN #ProductDetail PD ON (PD.PimProductId = ZPAV.PimProductId)
					LEFT JOIN ZnodeMedia ZM ON (Zm.Path = ZPPAM.MediaPath) 
					WHERE  ZPAV.PimAttributeId = (select PimAttributeId from ZnodePimAttribute pa where attributecode = ''ProductImage'') 

		;WITH Cte_ProductMedia
               AS (SELECT PD.PimProductId  , 
			   URL+ZMSM.ThumbnailFolderName+''/''+ zm.PATH  AS ProductImagePath 
			   FROM ZnodeMedia AS ZM
               INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)
			   INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
			   INNER JOIN @TBL_MediaValue PD ON (PD.MediaPath = CAST(ZM.MediaId AS VARCHAR(50)))
			   --INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = PD.PimATtributeId )
			   
			   )

		, CTE_ProductDetail AS
	(
		SELECT DISTINCT  CD.pimproductId, SKU,ProductName,
		case WHEN  CD.IsActive = ''true'' THEN 1 ELSE 0 END IsActive, ISNULL(CSD.SEOCode,SKU) as SEOCode, CSD.SEOUrl, CSDL.SEOTitle, CSDL.SEODescription, CSDL.SEOKeywords,
		Case When Isnull(CSD.IsPublish ,0 ) = 0 then ''Draft'' ELSE ''Published'' END  IsPublish  , CPM.ProductImagePath, PC.CatalogName, CD.LocaleId
		FROM #ProductDetail CD
		INNER JOIN #znodeCatalogProduct PCC on CD.PimProductId = PCC.PimProductId
		INNER JOIN ZnodePimCatalog PC on PCC.PimCatalogId = PC.PimCatalogId
		LEFT JOIN ZnodeCMSSEOType CST ON CST.Name = ''Product''
		LEFT JOIN ZnodeCMSSEODetail CSD on LTRIM(RTRIM(CD.SKU)) = LTRIM(RTRIM(CSD.SEOCode)) and CSD.CMSSEOTypeId = CST.CMSSEOTypeId AND CSD.PortalId = '+CAST(@PortalId AS VARCHAR(10))+'
		LEFT JOIN ZnodeCMSSEODetailLocale CSDL ON  CSD.CMSSEODetailId = CSDL.CMSSEODetailId --AND CSDL.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
		
		LEFT JOIN Cte_ProductMedia CPM ON (CPM.PimProductId = CD.PimProductId)
		WHERE PCC.PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(10))+' AND CD.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+', '+CAST(@DefaultLocaleId AS VARCHAR(50))+')
	)

	, CTE_ProductLocale AS
	(
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName, LocaleId
	FROM CTE_ProductDetail CPD
	WHERE CPD.LocaleId ='+CAST(@LocaleId AS VARCHAR(50))+'	
	)

	, CTE_ProductBothLocale AS
	(
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName
	FROM CTE_ProductLocale PL
	UNION ALL 
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName
	FROM CTE_ProductDetail PD 
	WHERE LocaleId ='+CAST(@DefaultLocaleId AS VARCHAR(50))+' AND
	NOT EXISTS (select * from CTE_ProductLocale PCL WHERE PCL.pimproductId = PD.pimproductId AND PCL.CatalogName = PD.CatalogName )
	)

	,CTE_ProductDetail_WhereClause AS
	(
		SELECT  pimproductId, SKU,ProductName,
		cast(IsActive as bit) IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName,'+[dbo].[Fn_GetPagingRowId](@Order_BY, 'PimProductId')+',Count(*)Over() CountId
		FROM CTE_ProductBothLocale CD
		WHERE 1 = 1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
	)
	SELECT  pimproductId, SKU,ProductName,IsActive, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath,CatalogName, CountId
	INTO ##TempProductDetail
	FROM CTE_ProductDetail_WhereClause
	'+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
	print @SQL
	EXEC (@SQL)

	SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM ##TempProductDetail ),0)

	SELECT  pimproductId,LTRIM(RTRIM(SKU)) AS SKU,ProductName,IsActive,LTRIM(RTRIM(SEOCode)) AS SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath 
	FROM ##TempProductDetail
	--GROUP by pimproductId, SKU,ProductName,IsActive, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath 

  
		

			IF OBJECT_ID('TEMPDB..#ProductDetail') IS NOT NULL
			DROP TABLE #ProductDetail

			IF OBJECT_ID('TEMPDB..##TempProductDetail') IS NOT NULL
			DROP TABLE ##TempProductDetail

			
			IF OBJECT_ID('TEMPDB..#znodeCatalogProduct') IS NOT NULL
		DROP TABLE #znodeCatalogProduct


         END TRY
         BEGIN CATCH
		    SELECT ERROR_message()
             DECLARE @Status BIT ;
		     SET @Status = 0;
		 --    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			-- @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCatalogCategoryProducts @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(MAX)),'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			--VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',
			--@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',@PimCategoryId='+ISNULL(CAST(@PimCategoryId AS VARCHAR(50)),'''')+',@PimCatalogId='+ISNULL(CAST(@PimCatalogId AS VARCHAR(50)),'''')+',@IsAssociated='+ISNULL(CAST(@IsAssociated AS VARCHAR(50)),'''')+',
			--@ProfileCatalogId='+ISNULL(CAST(@ProfileCatalogId AS VARCHAR(50)),'''')+',@AttributeCode='''+ISNULL(CAST(@AttributeCode AS VARCHAR(50)),'''''')+''',@PimCategoryHierarchyId='+ISNULL(CAST(@PimCategoryHierarchyId AS VARCHAR(10)),'''');
              			 
   --          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
   --          EXEC Znode_InsertProcedureErrorLog
			--	@ProcedureName = 'Znode_GetCatalogCategoryProducts',
			--	@ErrorInProcedure = 'Znode_GetCatalogCategoryProducts',
			--	@ErrorMessage = @ErrorMessage,
			--	@ErrorLine = @ErrorLine,
			--	@ErrorCall = @ErrorCall;
         END CATCH;
     END;



GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetManageMessagelist')
BEGIN 
	DROP PROCEDURE Znode_GetManageMessagelist
END 
GO

CREATE procedure [dbo].[Znode_GetManageMessagelist]
(  
	 @WhereClause NVARCHAR(Max)     
	,@Rows INT = 100     
	,@PageNo INT = 1     
	,@Order_BY VARCHAR(1000) = ''  
	,@RowsCount INT OUT  
	,@LocaleId INT =1  
)  
AS  
/*
 Summary: Get Managed Message Details list for a PortalId
 Unit Testing:
	declare @p7 int
	set @p7=NULL
	exec sp_executesql N'Znode_GetManageMessagelist @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId',N'@WhereClause nvarchar(57),@Rows int,@PageNo int,@Order_By nvarchar(17),@RowCount int output,@LocaleId int',@WhereClause=N'localeid = 1 and (PortalId in(''1'',''4'',''5'',''6'',''32'',''33''))',@Rows=50,@PageNo=1,@Order_By=N'PublishStatus asc',@RowCount=@p7 output,@LocaleId=1
	select @p7 

 */
BEGIN    
  BEGIN TRY   
    SET NOCOUNT ON      
    DECLARE @SQL NVARCHAR(MAX)  
     	
	DECLARE @DefaultLocaleId VARCHAR(100)= dbo.Fn_GetDefaultLocaleId();
    DECLARE @TBL_ManageMessage TABLE (CMSPortalMessageId INT,CMSMessageId INT,[Message] NVARCHAR(max),Location NVARCHAR(100),StoreName NVARCHAR(max)
								,LocaleId INT,PortalId INT,CMSMessageKeyId INT,MessageTag NVARCHAR(max),RowId INT,CountNo INT,PublishStatus NVARCHAR(max))
              
    SET @SQL = ' 
			  ;With Cte_ManageMessage AS 
			  (
			  SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag ,
			  CASE when Isnull(PublishStatus,0) =0 then ''Draft'' else ''Published'' END PublishStatus
			  FROM View_Getmanagemessagelist
			  WHERE  LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
			  )

			  ,CTE_ManageMessageLocale As
			  (
			   SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId
			   ,MessageTag,PublishStatus
			   FROM Cte_ManageMessage
			   WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'  
			  )
			  ,CTE_ManageMessageBothLocale AS
			  (
			   SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,PublishStatus
			   FROM CTE_ManageMessageLocale
			   UNION ALL
			   SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,PublishStatus
			   FROM Cte_ManageMessage cmm
			   WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+' 
			   AND NOT EXISTS (SELECT TOP 1 1 FROM CTE_ManageMessageLocale CMML WHERE  CMML.CMSMessageKeyId = cmm.CMSMessageKeyId)
			  )

			  ,Cte_ManageMessageFilter AS 
			  (
			   SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId
						,MessageTag,PublishStatus ,'+dbo.Fn_GetPagingRowId(@Order_BY,'CMSPortalMessageId DESC')+',Count(*)Over() CountNo 
			   FROM CTE_ManageMessageBothLocale 
			   WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			  )
	  
			  SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,PublishStatus,RowId,CountNo 
			  FROM Cte_ManageMessageFilter
			  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

			  print @sql


			  INSERT INTO @TBL_ManageMessage (CMSPortalMessageId,CMSMessageId,[Message],Location,StoreName,LocaleId,PortalId,CMSMessageKeyId,MessageTag,PublishStatus,RowId,CountNo)
			  EXEC (@SQL)
			  SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ManageMessage ),0)

			  SELECT CMSPortalMessageId,CMSMessageId,[Message],Location,StoreName,LocaleId,PortalId,CMSMessageKeyId,MessageTag,  PublishStatus
			  FROM @TBL_ManageMessage
    


			END TRY     
			BEGIN CATCH        
	         DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetManageMessagelist @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetManageMessagelist',
				@ErrorInProcedure = 'Znode_GetManageMessagelist',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;  
   END CATCH     
END
GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetPublishCategory')
BEGIN 
	DROP PROCEDURE Znode_GetPublishCategory
END 
GO

CREATE PROCEDURE [dbo].[Znode_GetPublishCategory]
(   @PublishCatalogId INT,
    @UserId           INT,
    @VersionId        INT,
    @Status           BIT = 0 OUT,
    @IsDebug          BIT = 0)
AS 
/*
       Summary:Publish category with their respective products and details 
	            The result is fetched in xml form   
       Unit Testing   
       Begin transaction 
       SELECT * FROM ZnodePIMAttribute 
	   SELECT * FROM ZnodePublishCatalog 
	   SELECT * FROM ZnodePublishCategory WHERE publishCAtegoryID = 167 


       EXEC [Znode_GetPublishCategory] @PublishCatalogId = 5,@VersionId = 0 ,@UserId =2 ,@IsDebug = 1 
     


       Rollback Transaction 
	*/
     BEGIN
         BEGIN TRAN GetPublishCategory;
         BEGIN TRY
             SET NOCOUNT ON;
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @LocaleId INT= 0, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId(), @Counter INT= 1, @MaxId INT= 0, @CategoryIdCount INT;
             DECLARE @IsActive BIT= [dbo].[Fn_GetIsActiveTrue]();
             DECLARE @AttributeIds VARCHAR(MAX)= '', @PimCategoryIds VARCHAR(MAX)= '', @DeletedPublishCategoryIds VARCHAR(MAX)= '', @DeletedPublishProductIds VARCHAR(MAX);
             --get the pim catalog id 
			 DECLARE @PimCatalogId INT=(SELECT PimCatalogId FROM ZnodePublishcatalog WHERE PublishCatalogId = @PublishCatalogId); 

             DECLARE @TBL_AttributeIds TABLE
             (PimAttributeId       INT,
              ParentPimAttributeId INT,
              AttributeTypeId      INT,
              AttributeCode        VARCHAR(600),
              IsRequired           BIT,
              IsLocalizable        BIT,
              IsFilterable         BIT,
              IsSystemDefined      BIT,
              IsConfigurable       BIT,
              IsPersonalizable     BIT,
              DisplayOrder         INT,
              HelpDescription      VARCHAR(MAX),
              IsCategory           BIT,
              IsHidden             BIT,
              CreatedDate          DATETIME,
              ModifiedDate         DATETIME,
              AttributeName        NVARCHAR(MAX),
              AttributeTypeName    VARCHAR(300)
             );
             DECLARE @TBL_AttributeDefault TABLE
             (PimAttributeId            INT,
              AttributeDefaultValueCode VARCHAR(100),
              IsEditable                BIT,
              AttributeDefaultValue     NVARCHAR(MAX)
			  ,DisplayOrder   INT
             );
             DECLARE @TBL_AttributeValue TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              CategoryValue               NVARCHAR(MAX),
              AttributeCode               VARCHAR(300),
              PimAttributeId              INT
             );
             DECLARE @TBL_LocaleIds TABLE
             (RowId     INT IDENTITY(1, 1),
              LocaleId  INT,
              IsDefault BIT
             );
             DECLARE @TBL_PimCategoryIds TABLE
             (PimCategoryId       INT,
              PimParentCategoryId INT,
              DisplayOrder        INT,
              ActivationDate      DATETIME,
              ExpirationDate      DATETIME,
              CategoryName        NVARCHAR(MAX),
              ProfileId           VARCHAR(MAX),
              IsActive            BIT,
			  PimCategoryHierarchyId INT,
			  ParentPimCategoryHierarchyId INT ,
			   CategoryCode  NVARCHAR(MAX)             );


             DECLARE @TBL_PublishPimCategoryIds TABLE
             (PublishCategoryId       INT,
              PimCategoryId           INT,
              PublishProductId        varchar(max),
              PublishParentCategoryId INT ,
			  PimCategoryHierarchyId INT ,parentPimCategoryHierarchyId INT
             );
             DECLARE @TBL_DeletedPublishCategoryIds TABLE
             (PublishCategoryId INT,
              PublishProductId  INT
             );
             DECLARE @TBL_CategoryXml TABLE
             (PublishCategoryId INT,
              CategoryXml       XML,
              LocaleId          INT
             );
             INSERT INTO @TBL_LocaleIds
             (LocaleId,
              IsDefault
             )
			  -- here collect all locale ids
             SELECT LocaleId,IsDefault FROM ZnodeLocale WHERE IsActive = @IsActive;

             INSERT INTO @TBL_PimCategoryIds(PimCategoryId,PimParentCategoryId,DisplayOrder,ActivationDate,ExpirationDate,IsActive,PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
             SELECT DISTINCT ZPCH.PimCategoryId,ZPCH2.PimCategoryId  PimParentCategoryId,ZPCH.DisplayOrder,ZPCH.ActivationDate,ZPCH.ExpirationDate,ZPCH.IsActive ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId
			 FROM ZnodePimCategoryHierarchy AS ZPCH 
			 LEFT JOIN ZnodePimCategoryHierarchy AS ZPCH2 ON (ZPCH2.PimCategoryHierarchyId = ZPCH. ParentPimCategoryHierarchyId ) 
			 WHERE ZPCH.PimCatalogId = @PimCatalogId; 
             -- AND IsActive = @IsActive ; -- As discussed with @anup active flag maintain on demo site 23/12/2016
			 --	SELECT * FROM @TBL_PimCategoryIds
			 -- here is find the deleted publish category id on basis of publish catalog
             INSERT INTO @TBL_DeletedPublishCategoryIds(PublishCategoryId,PublishProductId)
             SELECT ZPC.PublishCategoryId,ZPCP.PublishProductId 
			 FROM ZnodePublishCategoryProduct ZPCP
             INNER JOIN ZnodePublishCategory AS ZPC ON(ZPCP.PublishCategoryId = ZPC.PublishCategoryId AND ZPCP.PublishCatalogId = ZPC.PublishCatalogId)                                                  
             INNER JOIN ZnodePublishProduct ZPP ON(zpp.PublishProductId = zpcp.PublishProductId AND zpp.PublishCatalogId = zpcp.PublishCatalogId)
             INNER JOIN ZnodePublishCatalog ZPCC ON(ZPCC.PublishCatalogId = ZPCP.PublishCatalogId)
             WHERE ZPC.PublishCatalogId = @PublishCataLogId 
			 AND NOT EXISTS(SELECT TOP 1 1 FROM ZnodePimCatalogCategory AS TBPC WHERE TBPC.PimCategoryId = ZPC.PimCategoryId AND TBPC.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId
			 AND TBPC.PimProductId = ZPP.PimProductId AND TBPC.PimCatalogId = ZPCC.PimCatalogId);

			 -- here is find the deleted publish category id on basis of publish catalog
             SET @DeletedPublishCategoryIds = ISNULL(SUBSTRING((SELECT ','+CAST(PublishCategoryId AS VARCHAR(50)) FROM @TBL_DeletedPublishCategoryIds AS ZPC
                                              GROUP BY ZPC.PublishCategoryId FOR XML PATH('') ), 2, 4000), '');
			 -- here is find the deleted publish category id on basis of publish catalog
             SET @DeletedPublishProductIds = '';
			 -- Delete the publish category id 
	
             EXEC Znode_DeletePublishCatalog @PublishCatalogIds = @PublishCatalogId,@PublishCategoryIds = @DeletedPublishCategoryIds,@PublishProductIds = @DeletedPublishProductIds; 
			
             MERGE INTO ZnodePublishCategory TARGET USING  @TBL_PimCategoryIds SOURCE ON
			 (
			 TARGET.PimCategoryId = SOURCE.PimCategoryId 
			 AND TARGET.PublishCatalogId = @PublishCataLogId 
			 AND TARGET.PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId
			 )
			 WHEN MATCHED THEN UPDATE SET TARGET.PimParentCategoryId = SOURCE.PimParentCategoryId,TARGET.CreatedBy = @UserId,TARGET.CreatedDate = @GetDate,
             TARGET.ModifiedBy = @UserId,TARGET.ModifiedDate = @GetDate,PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId,ParentPimCategoryHierarchyId=SOURCE.ParentPimCategoryHierarchyId
             WHEN NOT MATCHED THEN INSERT(PimCategoryId,PublishCatalogId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PimCategoryHierarchyId,ParentPimCategoryHierarchyId) 
			 VALUES(SOURCE.PimCategoryId,@PublishCatalogId,@UserId,@GetDate,@UserId,@GetDate,SOURCE.PimCategoryHierarchyId,SOURCE.ParentPimCategoryHierarchyId)
             OUTPUT INSERTED.PublishCategoryId,INSERTED.PimCategoryId,INSERTED.PimCategoryHierarchyId,INSERTED.parentPimCategoryHierarchyId INTO @TBL_PublishPimCategoryIds(PublishCategoryId,PimCategoryId,PimCategoryHierarchyId,parentPimCategoryHierarchyId);
			
    --         UPDATE TBPC SET PublishParentCategoryId = TBPCS.PublishCategoryId 
			 --FROM @TBL_PublishPimCategoryIds TBPC
    --         INNER JOIN @TBL_PimCategoryIds TBC ON(TBC.PimCategoryId = TBPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId)
    --         INNER JOIN @TBL_PublishPimCategoryIds TBPCS ON(TBC.PimCategoryHierarchyId = TBPCS.parentPimCategoryHierarchyId  ) 
			 --WHERE TBC.parentPimCategoryHierarchyId IS NOT NULL;
           
		     -- here update the publish parent category id
             UPDATE ZPC SET [PimParentCategoryId] =TBPC.[PimCategoryId] 
			 FROM ZnodePublishCategory ZPC
             INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
			 WHERE ZPC.PublishCatalogId =@PublishCatalogId
			 AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL
			 AND TBPC.PublishCatalogId =@PublishCatalogId
			 ;
			 UPDATE a
			 SET  a.PublishParentCategoryId = b.PublishCategoryId
			FROM ZnodePublishCategory a 
			INNER JOIN ZnodePublishCategory b   ON (a.parentpimCategoryHierarchyId = b.pimCategoryHierarchyId)
			WHERE a.parentpimCategoryHierarchyId IS NOT NULL 
			AND a.PublishCatalogId =@PublishCatalogId
			AND b.PublishCatalogId =@PublishCatalogId

			 --UPDATE ZPC SET [PimParentCategoryId] = TBPC.[PimCategoryId] 
			 --FROM ZnodePublishCategory ZPC
    --         INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
			 --WHERE ZPC.PublishCatalogId =@PublishCatalogId
			 --AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL ;

			 -- product are published here 
            --  EXEC Znode_GetPublishProducts @PublishCatalogId,0,@UserId,1,0,0;

             SET @MaxId =(SELECT MAX(RowId)FROM @TBL_LocaleIds);
			 DECLARE @TransferID TRANSFERID 
			 INSERT INTO @TransferID 
			 SELECT DISTINCT  PimCategoryId
			 FROM @TBL_PublishPimCategoryIds 

             SET @PimCategoryIds = SUBSTRING((SELECT ','+CAST(PimCategoryId AS VARCHAR(50)) FROM @TBL_PublishPimCategoryIds FOR XML PATH('')), 2, 4000);
			 
             WHILE @Counter <= @MaxId -- Loop on Locale id 
                 BEGIN
                     SET @LocaleId =(SELECT LocaleId FROM @TBL_LocaleIds WHERE RowId = @Counter);
                   
				     SET @AttributeIds = SUBSTRING((SELECT ','+CAST(ZPCAV.PimAttributeId AS VARCHAR(50)) FROM ZnodePimCategoryAttributeValue ZPCAV 
										 WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC WHERE TBPC.PimCategoryId = ZPCAV.PimCategoryId) GROUP BY ZPCAV.PimAttributeId FOR XML PATH('')), 2, 4000);
                
				     SET @CategoryIdCount =(SELECT COUNT(1) FROM @TBL_PimCategoryIds);

                     INSERT INTO @TBL_AttributeIds (PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,
					 IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName)
                     EXEC [Znode_GetPimAttributesDetails] @AttributeIds,@LocaleId;

                     INSERT INTO @TBL_AttributeDefault (PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)
                     EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @AttributeIds,@LocaleId;

                     INSERT INTO @TBL_AttributeValue (PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
                     EXEC [dbo].[Znode_GetCategoryAttributeValueId] @TransferID,@AttributeIds,@LocaleId;

					-- SELECT * FROM @TBL_AttributeValue WHERE PimCategoryId = 281


                     ;WITH Cte_UpdateDefaultAttributeValue
                     AS (
					  SELECT TBAV.PimCategoryId,TBAV.PimAttributeId,SUBSTRING((SELECT ','+AttributeDefaultValue FROM @TBL_AttributeDefault TBD WHERE TBAV.PimAttributeId = TBD.PimAttributeId
						AND EXISTS(SELECT TOP 1 1 FROM Split(TBAV.CategoryValue, ',') SP WHERE SP.Item = TBD.AttributeDefaultValueCode)FOR XML PATH('')), 2, 4000) DefaultCategoryAttributeValue
						FROM @TBL_AttributeValue TBAV WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_AttributeDefault TBAD WHERE TBAD.PimAttributeId = TBAV.PimAttributeId))
					 
					 -- update the default value with locale 
                     UPDATE TBAV SET CategoryValue = CTUDFAV.DefaultCategoryAttributeValue FROM @TBL_AttributeValue TBAV 
					 INNER JOIN Cte_UpdateDefaultAttributeValue CTUDFAV ON(CTUDFAV.PimCategoryId = TBAV.PimCategoryId AND CTUDFAV.PimAttributeId = TBAV.PimAttributeId)
					 WHERE CategoryValue IS NULL ;
					 
					 -- here is update the media path  
                     WITH Cte_productMedia
                     AS (SELECT TBA.PimCategoryId,TBA.PimAttributeId,[dbo].[FN_GetThumbnailMediaPathPublish](SUBSTRING((SELECT ','+zm.PATH FROM ZnodeMedia ZM WHERE EXISTS
					    (SELECT TOP 1 1 FROM dbo.split(TBA.CategoryValue, ',') SP WHERE SP.Item = CAST(Zm.MediaId AS VARCHAR(50)))FOR XML PATH('')), 2, 4000)) CategoryValue
						FROM @TBL_AttributeValue TBA WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetProductMediaAttributeId]() FNMA WHERE FNMA.PImAttributeId = TBA.PimATtributeId))
                         
					 UPDATE TBAV SET CategoryValue = CTCM.CategoryValue 
					 FROM @TBL_AttributeValue TBAV 
					 INNER JOIN Cte_productMedia CTCM ON(CTCM.PimCategoryId = TBAV.PimCategoryId
					 AND CTCM.PimAttributeId = TBAV.PimAttributeId);

                     WITH Cte_PublishProductIds
					 AS (SELECT TBPC.PublishcategoryId,SUBSTRING((SELECT ','+CAST(PublishProductId AS VARCHAR(50))
					  FROM ZnodePublishCategoryProduct ZPCP 
					  WHERE ZPCP.PublishCategoryId = TBPC.publishCategoryId
					  AND ZPCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId
                      AND ZPCP.PublishCatalogId = @PublishCatalogId FOR XML PATH('')), 2, 8000) PublishProductId ,PimCategoryHierarchyId
					  FROM @TBL_PublishPimCategoryIds TBPC)
                          
					 UPDATE TBPPC SET PublishProductId = CTPP.PublishProductId FROM @TBL_PublishPimCategoryIds TBPPC INNER JOIN Cte_PublishProductIds CTPP ON(TBPPC.PublishCategoryId = CTPP.PublishCategoryId 
					 AND TBPPC.PimCategoryHierarchyId = CTPP.PimCategoryHierarchyId);

                     WITH Cte_CategoryProfile
                     AS (SELECT PimCategoryId,ZPCC.PimCategoryHierarchyId,SUBSTRING(( SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
					 FROM ZnodeProfileCatalog ZPC 
					 INNER JOIN ZnodeProfileCategoryHierarchy ZPRCC ON(ZPRCC.PimCategoryHierarchyId = ZPCC.PimCategoryHierarchyId
                        AND ZPRCC.ProfileCatalogId = ZPC.ProfileCatalogId) 
						WHERE ZPC.PimCatalogId = ZPCC.PimCatalogId FOR XML PATH('')), 2, 4000) ProfileIds
                      
					   FROM ZnodePimCategoryHierarchy ZPCC 
					   WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC 
					   WHERE TBPC.PimCategoryId = ZPCC.PimCategoryId AND ZPCC.PimCatalogId = @PimCatalogId 
					   AND ZPCC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId))
                          
				     UPDATE TBPC SET TBPC.ProfileId = CTCP.ProfileIds FROM @TBL_PimCategoryIds TBPC 
					 LEFT JOIN Cte_CategoryProfile CTCP ON(CTCP.PimCategoryId = TBPC.PimCategoryId AND CTCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId );
                     
					 UPDATE TBPC SET TBPC.CategoryName = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
                     AND EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryNameAttribute]() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId));


					  UPDATE TBPC SET TBPC.CategoryCode = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
					 AND EXISTS(SELECT TOP 1 1 FROM dbo.Fn_GetCategoryCodeAttribute() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId)
					 )


					-- SELECT * FROM @TBL_AttributeValue WHERE pimCategoryId = 369

					 -- here update the publish category details 
                     ;WITH Cte_UpdateCategoryDetails
                     AS (
					 SELECT TBC.PimCategoryId,PublishCategoryId,CategoryName, TBPPC.PimCategoryHierarchyId,CategoryCode
					 FROM @TBL_PimCategoryIds TBC
                     INNER JOIN @TBL_PublishPimCategoryIds TBPPC ON(TBC.PimCategoryId = TBPPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPPC.PimCategoryHierarchyId)
					 )						
                     MERGE INTO ZnodePublishCategoryDetail TARGET USING Cte_UpdateCategoryDetails SOURCE ON(TARGET.PublishCategoryId = SOURCE.PublishCategoryId
					 AND TARGET.LocaleId = @LocaleId)
                     WHEN MATCHED THEN UPDATE SET PublishCategoryId = SOURCE.PublishcategoryId,PublishCategoryName = SOURCE.CategoryName,LocaleId = @LocaleId,ModifiedBy = @userId,ModifiedDate = @GetDate,CategoryCode=SOURCE.CategoryCode
                     WHEN NOT MATCHED THEN INSERT(PublishCategoryId,PublishCategoryName,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CategoryCode) VALUES
                     (SOURCE.PublishCategoryId,SOURCE.CategoryName,@LocaleId,@userId,@GetDate,@userId,@GetDate,SOURCE.CategoryCode);

                     ;WITH Cte_CategoryXML
                     AS (SELECT PublishcategoryId,PimCategoryId,(SELECT @VersionId VersionId,TBPC.PublishCategoryId ZnodeCategoryId,@PublishCatalogId ZnodeCatalogId
																		,THR.PublishParentCategoryId TempZnodeParentCategoryIds,ZPC.CatalogName ,
																		 ISNULL(DisplayOrder, '0') DisplayOrder,@LocaleId LocaleId,ActivationDate 
																		 ,ExpirationDate,TBC.IsActive,ISNULL(CategoryName, '') Name,ProfileId TempProfileIds,ISNULL(PublishProductId, '') TempProductIds,ISNULL(CategoryCode,'') CategoryCode
                        FROM @TBL_PublishPimCategoryIds TBPC 
						INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId= @PublishCatalogId)
						INNER JOIN ZnodePublishCAtegory THR ON (THR.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId AND THR.PimCategoryId = TBPC.PimCategoryId AND THR.PublishCatalogId= @PublishCatalogId )
						INNER JOIN @TBL_PimCategoryIds TBC ON(TBC.PimCategoryId = TBPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId) WHERE TBPC.PublishCategoryId = TBPCO.PublishCategoryId 
						FOR XML PATH('')) CategoryXml 
						FROM @TBL_PublishPimCategoryIds TBPCO),

                     Cte_CategoryAttributeXml
                     AS (SELECT CTCX.PublishCategoryId,'<CategoryEntity>'+ISNULL(CategoryXml, '')+ISNULL((SELECT(SELECT TBA.AttributeCode,TBA.AttributeName,ISNULL(IsUseInSearch, 0) IsUseInSearch,
                        ISNULL(IsHtmlTags, 0) IsHtmlTags,ISNULL(IsComparable, 0) IsComparable,(SELECT ''+TBAV.CategoryValue FOR XML PATH('')) AttributeValues,TBA.AttributeTypeName FROM @TBL_AttributeValue TBAV
                        INNER JOIN @TBL_AttributeIds TBA ON(TBAV.PimAttributeId = TBA.PimAttributeId) LEFT JOIN ZnodePimFrontendProperties ZPFP ON(ZPFP.PimAttributeId = TBA.PimAttributeId)
                        WHERE CTCX.PimCategoryId = TBAV.PimCategoryId AND TBAO.PimAttributeId = TBA.PimAttributeId FOR XML PATH('AttributeEntity'), TYPE) FROM @TBL_AttributeIds TBAO
                        FOR XML PATH('Attributes')), '')+'</CategoryEntity>' CategoryXMl FROM Cte_CategoryXML CTCX)

                     INSERT INTO @TBL_CategoryXml(PublishCategoryId,CategoryXml,LocaleId)
                     SELECT PublishCategoryId,CategoryXml,@LocaleId LocaleId FROM Cte_CategoryAttributeXml;
                   
				     DELETE FROM @TBL_AttributeIds;
                     DELETE FROM @TBL_AttributeDefault;
                     DELETE FROM @TBL_AttributeValue;
                     SET @Counter = @Counter + 1;
                 END;

             UPDATE ZnodePublishCatalogLog SET PublishCategoryId = SUBSTRING((SELECT ','+CAST(PublishCategoryId AS VARCHAR(50)) FROM @TBL_CategoryXml
			 GROUP BY PublishCategoryId																				
             FOR XML PATH('')), 2, 4000), IsCategoryPublished = 1 WHERE PublishCatalogLogId = @VersionId;

             DELETE FROM ZnodePublishedXml WHERE PublishCataloglogId = @VersionId;
            
             INSERT INTO ZnodePublishedXml (PublishCatalogLogId,PublishedId,PublishedXML,IsCategoryXML,IsProductXML,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT @VersionId PublishCataloglogId,PublishCategoryId,CategoryXml,1,0,LocaleId,@UserId,@GetDate,@UserId,@GetDate FROM @TBL_CategoryXml WHERE @VersionId <> 0;
             
			 SELECT CategoryXml  
			 FROM @TBL_CategoryXml 
			 

			 UPDATE ZnodePimCategory 
			 SET IsCategoryPublish =1 
			 WHERE pimCategoryId IN (SELECT PimCategoryId FROM @TBL_PimCategoryIds)

              
             COMMIT TRAN GetPublishCategory;
			 
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE();
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishCategory @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(50))+',@UserId ='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             ROLLBACK TRAN GetPublishCategory;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_GetPublishCategory',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetPublishProductbulk')
BEGIN 
	DROP PROCEDURE Znode_GetPublishProductbulk
END 
GO
CREATE PROCEDURE [dbo].[Znode_GetPublishProductbulk]
(
@PublishCatalogId INT = 0 
,@VersionId       VARCHAR(50) = 0 
,@PimProductId    TransferId Readonly
,@UserId		  INT = 0 
,@PimCategoryHierarchyId  INT = 0 
,@PimCatalogId INT = 0 
)
With RECOMPILE
AS
-- EXEC Znode_GetPublishProductbulk 5, 0 , '' , 2  
BEGIN 
  
 SET NOCOUNT ON 

EXEC Znode_InsertUpdatePimAttributeXML 1 
EXEC Znode_InsertUpdateCustomeFieldXML 1
EXEC Znode_InsertUpdateAttributeDefaultValue 1 

  --DECLARE @PimProductAttributeXML TABLE(PimAttributeXMLId INT  PRIMARY KEY ,PimAttributeId INT,LocaleId INT  )

   IF OBJECT_ID('tempdb..#PimProductAttributeXML') is not null
   BEGIN 
	 DELETE FROM #PimProductAttributeXML
   END

   CREATE TABLE #PimProductAttributeXML (PimAttributeXMLId INT  PRIMARY KEY ,PimAttributeId INT,LocaleId INT  )
  	
   Declare @TBL_CategoryCategoryHierarchyIds TABLE (CategoryId int , ParentCategoryId int ) 
	
   If (@PimCategoryHierarchyId <> 0 AND @PimCatalogId <> 0 )
		INSERT INTO @TBL_CategoryCategoryHierarchyIds(CategoryId , ParentCategoryId )
			Select Distinct PimCategoryId , Null FROM (
				SELECT PimCategoryId,ParentPimCategoryId from DBO.[Fn_GetRecurciveCategoryIds](@PimCategoryHierarchyId,@PimCatalogId)
				Union 
				Select PimCategoryId , null  from ZnodePimCategoryHierarchy where PimCategoryHierarchyId = @PimCategoryHierarchyId 
				Union 
				Select PimCategoryId , null  from [Fn_GetRecurciveCategoryIds_new] (@PimCategoryHierarchyId,@PimCatalogId) ) Category  


   DECLARE @PimDefaultValueLocale  TABLE (PimAttributeDefaultXMLId INT  PRIMARY KEY ,PimAttributeDefaultValueId INT ,LocaleId INT ) 
   DECLARE @ProductNamePimAttributeId INT = dbo.Fn_GetProductNameAttributeId(),@DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId(),@LocaleId INT = 0 
		,@SkuPimAttributeId  INT =  dbo.Fn_GetProductSKUAttributeId() , @IsActivePimAttributeId INT =  dbo.Fn_GetProductIsActiveAttributeId()
   DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
   DECLARE @TBL_LocaleId  TABLE (RowId INT IDENTITY(1,1) PRIMARY KEY  , LocaleId INT )
			INSERT INTO @TBL_LocaleId (LocaleId)
			SELECT  LocaleId
			FROM ZnodeLocale 
			WHERE IsActive = 1
  DECLARE @Counter INT =1 ,@maxCountId INT = (SELECT max(RowId) FROM @TBL_LocaleId ) 
 
 --DECLARE #TBL_PublishCatalogId TABLE(PublishCatalogId INT,PublishProductId INT,PimProductId  INT   , VersionId INT )

 CREATE TABLE #TBL_PublishCatalogId (PublishCatalogId INT,PublishProductId INT,PimProductId  INT   , VersionId INT, PublishCategoryId int  )

 CREATE INDEX idx_#TBL_PublishCatalogIdPimProductId on #TBL_PublishCatalogId(PimProductId)

  CREATE INDEX idx_#TBL_PublishCatalogIdPimPublishCatalogId on #TBL_PublishCatalogId(PublishCatalogId)

  If (@PimCategoryHierarchyId <> 0 AND @PimCatalogId <> 0 )
  BEGIN
			 INSERT INTO #TBL_PublishCatalogId(PublishCatalogId ,PublishProductId ,PimProductId  , VersionId ,PublishCategoryId )  
			 SELECT distinct ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,@versionId ,ZPC.PublishCategoryId
				 FROM ZnodePublishProduct ZPP INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 INNER JOIN ZnodePublishCategoryProduct ZPPP ON ZPP.PublishProductId  = ZPPP.PublishProductId  
				 AND ZPCP.PublishCatalogId = ZPPP.PublishCatalogId
				 INNER JOIN ZnodePublishCategory ZPC ON ZPC.PublishCatalogId = ZPPP.PublishCatalogId AND ZPPP.PublishCategoryId = ZPC.PublishCategoryId 
				 WHERE ZPP.PublishCatalogId = @PublishCatalogId  and  
				 ZPC.PimCategoryId in (Select CategoryId from @TBL_CategoryCategoryHierarchyIds )

			INSERT INTO #TBL_PublishCatalogId(PublishCatalogId ,PublishProductId ,PimProductId  , VersionId ,PublishCategoryId )
			SELECT DISTINCT @publishCatalogId,ZPP.PublishProductId,PimProductId,@versionId,NULL 
				 FROM ZnodePublishProduct ZPP INNER JOIN ZnodePublishCatalogLog ZPCP ON 
				 (ZPCP.PublishCatalogId = ZPP.PublishCatalogId) WHERE
				 (EXISTS (SELECT TOP 1 1 FROM @pimProductId SP WHERE SP.Id = ZPP.PimProductId ))
				 AND (ZPP.PublishCatalogId = @publishCatalogId )
				 AND NOT Exists (Select TOP 1 1 from #TBL_PublishCatalogId TPL where TPL.PublishProductId = ZPP.PublishProductId)


  END
  ELSE 
  BEGIN
			 INSERT INTO #TBL_PublishCatalogId(PublishCatalogId ,PublishProductId,PimProductId ,VersionId ) 
			 SELECT ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId,CASE WHEN @versionId = 0 OR @versionId IS NULL THEN  
											MAX(PublishCatalogLogId) ELSE @versionId END 
				 FROM ZnodePublishProduct ZPP 
				 INNER JOIN ZnodePublishCatalogLog ZPCP ON (ZPCP.PublishCatalogId  = ZPP.PublishCatalogId)
				 WHERE (EXISTS (SELECT TOP 1 1 FROM @PimProductId SP 
				 WHERE SP.Id = ZPP.PimProductId  AND  (@PublishCatalogId IS NULL OR @PublishCatalogId = 0 ))
				 OR  (ZPP.PublishCatalogId = @PublishCatalogId ))
				 GROUP BY ZPP.PublishCatalogId , ZPP.PublishProductId,PimProductId
  END
           
		     DECLARE   @TBL_ZnodeTempPublish TABLE (PimProductId INT , AttributeCode VARCHAR(300) ,AttributeValue NVARCHAR(max) ) 			
			 DECLARE @TBL_AttributeVAlueLocale TABLE(PimProductId INT,PimAttributeId INT,ZnodePimAttributeValueLocaleId INT,LocaleId INT 
			 )

WHILE @Counter <= @maxCountId
BEGIN
 
  SET @LocaleId = (SELECT TOP 1 LocaleId FROM @TBL_LocaleId WHERE RowId = @Counter)
 
  INSERT INTO #PimProductAttributeXML 
  SELECT PimAttributeXMLId ,PimAttributeId,LocaleId
  FROM ZnodePimAttributeXML
  WHERE LocaleId = @LocaleId
  
  IF( @LocaleId <> @DefaultLocaleId )
  BEGIN
	INSERT INTO #PimProductAttributeXML 
	SELECT PimAttributeXMLId ,PimAttributeId,LocaleId
	FROM ZnodePimAttributeXML ZPAX
	WHERE ZPAX.LocaleId = @DefaultLocaleId  
	AND NOT EXISTS (SELECT TOP 1 1 FROM #PimProductAttributeXML ZPAXI WHERE ZPAXI.PimAttributeId = ZPAX.PimAttributeId )
  END

  INSERT INTO @PimDefaultValueLocale
  SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId 
  FROM ZnodePimAttributeDefaultXML
  WHERE localeId = @LocaleId

  IF( @LocaleId <> @DefaultLocaleId )
  BEGIN
	INSERT INTO @PimDefaultValueLocale 
	SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId 
	FROM ZnodePimAttributeDefaultXML ZX
	WHERE localeId = @DefaultLocaleId
	AND NOT EXISTS (SELECT TOP 1 1 FROM @PimDefaultValueLocale TRTR WHERE TRTR.PimAttributeDefaultValueId = ZX.PimAttributeDefaultValueId)
  END
  
  DECLARE @TBL_CustomeFiled TABLE (PimCustomeFieldXMLId INT ,CustomCode VARCHAR(300),PimProductId INT ,LocaleId INT )

  INSERT INTO @TBL_CustomeFiled (PimCustomeFieldXMLId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldXMLId,RTR.PimProductId ,LocaleId,CustomCode
  FROM ZnodePimCustomeFieldXML RTR 
  INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = RTR.PimProductId)
  WHERE RTR.LocaleId = @LocaleId
 
 
  INSERT INTO @TBL_CustomeFiled (PimCustomeFieldXMLId,PimProductId ,LocaleId,CustomCode)
  SELECT  PimCustomeFieldXMLId,ITR.PimProductId ,LocaleId,CustomCode
  FROM ZnodePimCustomeFieldXML ITR
  INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ITR.PimProductId)
  WHERE ITR.LocaleId = @DefaultLocaleId
  AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_CustomeFiled TBL  WHERE ITR.CustomCode = TBL.CustomCode AND ITR.PimProductId = TBL.PimProductId)
       
	 
	 SELECT VIR.PimProductId,PimAttributeId,ZnodePimAttributeValueLocaleId,VIR.LocaleId , VIR.AttributeValue, VIR.AttributeCode ,ROW_NUMBER() Over(Partition By VIR.PimProductId,PimAttributeId ORDER BY VIR.PimProductId,PimAttributeId  ) RowId
	 INTO #TBL_AttributeVAlue
	 FROM View_LoadManageProductInternal VIR
	 WHERE ( LocaleId = @DefaultLocaleId OR LocaleId = @LocaleId )
	 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
	  UNION ALL 
	 SELECT VIR.PimProductId,VIR.PimAttributeId,ZPDE.PimProductAttributeMediaId,ZPDE.LocaleId ,ZPDE.MediaPath AS AttributeValue, d.AttributeCode ,ROW_NUMBER() Over(Partition By VIR.PimProductId,VIR.PimAttributeId ORDER BY VIR.PimProductId,VIR.PimAttributeId  ) RowId
	 FROM ZnodePimAttributeValue  VIR
	 INNER JOIN ZnodePimProductAttributeMedia ZPDE ON (ZPDE.PimAttributeValueId = VIR.PimAttributeValueId )
	 INNER JOIN ZnodePimAttribute d ON ( d.PimAttributeId=VIR.PimAttributeId )
	 WHERE ( ZPDE.LocaleId = @DefaultLocaleId OR ZPDE.LocaleId = @LocaleId )
	 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )
	  Union All
	 SELECT VIR.PimProductId,VIR.PimAttributeId,ZPDVL.PimAttributeDefaultValueLocaleId,ZPDVL.LocaleId ,ZPDVL.AttributeDefaultValue AS AttributeValue, d.AttributeCode ,ROW_NUMBER() Over(Partition By VIR.PimProductId,VIR.PimAttributeId ORDER BY VIR.PimProductId,VIR.PimAttributeId  ) RowId
	 FROM ZnodePimAttributeValue  VIR
	 INNER JOIN ZnodePimAttribute D ON ( D.PimAttributeId=VIR.PimAttributeId AND D.IsPersonalizable =1  )
	 INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON ZPADV.PimAttributeId = D.PimAttributeId
	 INNER JOIN ZnodePimAttributeDefaultValueLocale ZPDVL   on (ZPADV.PimAttributeDefaultValueId = ZPDVL.PimAttributeDefaultValueId)
	 --INNER JOIN ZnodePimProductAttributeDefaultValue ZPDVP ON (ZPDVP.PimAttributeValueId = VIR.PimAttributeValueId AND ZPADV.PimAttributeDefaultValueId = ZPDVP.PimAttributeDefaultValueId )
	 WHERE ( ZPDVL.LocaleId = @DefaultLocaleId OR ZPDVL.LocaleId = @LocaleId )
	 AND EXISTS (SELECT TOP 1 1 FROM #TBL_PublishCatalogId ZPP WHERE (ZPP.PimProductId = VIR.PimProductId) )

	 UPDATE #TBL_AttributeVAlue SET rowid = (SELECT MAX(rowid) from #TBL_AttributeVAlue b where a.PimProductId=b.PimProductId and a.PimAttributeId = b.PimAttributeId )
	 from #TBL_AttributeVAlue a

	 --select * from #TBL_AttributeVAlue
	 --return
  SET @versionId = (SELECT TOP 1 VersionId FROM #TBL_PublishCatalogId) 
  

 IF OBJECT_ID('tempdb..#Cte_GetData') is not null
 BEGIN 
 DROP TABLE #Cte_GetData
 END 

 create table #Cte_GetData (PimProductId int,AttributeCode varchar(600),AttributeValue nvarchar(max))

 create index idx_#Cte_GetDataPimProductId on #Cte_GetData(PimProductId)

insert into #Cte_GetData(PimProductId ,AttributeCode,AttributeValue)
SELECT  a.PimProductId,a.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(a.AttributeValue,'')+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue
FROM #TBL_AttributeVAlue a 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
where a.LocaleId  = CASE WHEN a.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END
and exists (select * from #PimProductAttributeXML b where b.PimAttributeXMLId = c.PimAttributeXMLId)
AND NOT EXISTS (SELECT TOP 1 1 FROM Fn_GetProductMediaAttributeId() TY WHERE TY.PimAttributeId = c.PimAttributeId)
--INNER JOIN #TBL_AttributeVAlue CTE ON (Cte.PimAttributeId = a.PimAttributeId AND Cte.ZnodePimAttributeValueLocaleId = a.ZnodePimAttributeValueLocaleId AND CTE.LocaleId  = CASE WHEN cte.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END )

--insert into #Cte_GetData(PimProductId ,AttributeCode,AttributeValue)
--SELECT  a.PimProductId,c.AttributeCode , '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+''+'</AttributeValues> </AttributeEntity>  </Attributes>'  AttributeValue                 
--FROM ZnodePimAttributeValue  a 
--INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = a.PimAttributeId )
--INNER JOIN ZnodePImAttribute ZPA  ON (ZPA.PimAttributeId = a.PimAttributeId)
--INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = a.PimProductId)
--WHERE ZPA.IsPersonalizable = 1 
--AND NOT EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale q WHERE q.PimAttributeValueId = a.PimAttributeValueId) 
--and exists (select * from #PimProductAttributeXML b where b.PimAttributeXMLId = c.PimAttributeXMLId)



insert into #Cte_GetData(PimProductId ,AttributeCode,AttributeValue)
SELECT THB.PimProductId,'','<Attributes><AttributeEntity>'+CustomeFiledXML+'</AttributeEntity></Attributes>' 
FROM ZnodePimCustomeFieldXML THB 
INNER JOIN @TBL_CustomeFiled TRTE ON (TRTE.PimCustomeFieldXMLId = THB.PimCustomeFieldXMLId)
UNION ALL 
SELECT ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues></AttributeValues>'+'<SelectValues>'+
			   STUFF((
                    SELECT '  '+ DefaultValueXML  FROM ZnodePimAttributeDefaultXML AA 
				 INNER JOIN @PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId)
				 INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
				 WHERE (ZPADV.PimAttributeValueId = ZPAV.PimAttributeValueId)
    FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues> </AttributeEntity></Attributes>' AttributeValue 
FROM ZnodePimAttributeValue ZPAV  
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
and exists (select * from #PimProductAttributeXML b where b.PimAttributeXMLId = c.PimAttributeXMLId)



insert into #Cte_GetData(PimProductId ,AttributeCode,AttributeValue)
SELECT DISTINCT  ZPAV.PimProductId,c.AttributeCode,'<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+SUBSTRING((SELECT DISTINCT  ',' +ZPPG.MediaPath FROM ZnodePimProductAttributeMedia ZPPG
     INNER JOIN #TBL_AttributeVAlue FTRE ON (FTRE.PimProductId = ZPAV.PimProductId AND FTRE.PimAttributeId = ZPAV.PimAttributeId  AND FTRE.LocaleId  = CASE WHEN FTRE.RowId = 2 THEN  @LocaleId ELSE @DefaultLocaleId END )
	 WHERE ZPPG.PimProductAttributeMediaId = FTRE.ZnodePimAttributeValueLocaleId
	 FOR XML PATH ('')
 ),2,4000)+'</AttributeValues></AttributeEntity></Attributes>' AttributeValue 	 
FROM ZnodePimAttributeValue ZPAV 
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPAV.PimAttributeId )
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPAV.PimProductId)
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia ZPADVL WHERE ZPADVL.PimAttributeValueId = ZPAV.PimAttributeValueId)
and exists (select * from #PimProductAttributeXML b where b.PimAttributeXMLId = c.PimAttributeXMLId)

 
insert into #Cte_GetData(PimProductId ,AttributeCode,AttributeValue)
SELECT ZPLP.PimParentProductId ,c.AttributeCode, '<Attributes><AttributeEntity>'+c.AttributeXML+'<AttributeValues>'+ISNULL(SUBSTRING((SELECT ','+CAST(PublishProductId AS VARCHAR(50)) 
							 FROM #TBL_PublishCatalogId ZPPI 
							 INNER JOIN ZnodePimLinkProductDetail ZPLPI ON (ZPLPI.PimProductId = ZPPI.PimProductId)
							 WHERE ZPLPI.PimParentProductId = ZPLP.PimParentProductId
							 AND ZPLPI.PimAttributeId   = ZPLP.PimAttributeId
							 FOR XML PATH ('') ),2,4000),'')+'</AttributeValues></AttributeEntity></Attributes>'   AttributeValue 
							
FROM ZnodePimLinkProductDetail ZPLP  
INNER JOIN #TBL_PublishCatalogId ZPP ON (ZPP.PimProductId = ZPLP.PimParentProductId)
INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = ZPLP.PimAttributeId )
where exists (select * from #PimProductAttributeXML b where b.PimAttributeXMLId = c.PimAttributeXMLId)
GROUP BY ZPLP.PimParentProductId , ZPP.PublishProductId  ,ZPLP.PimAttributeId,c.AttributeCode,c.AttributeXML,ZPP.PublishCatalogId




--  --CREATE INDEX IND_Znode

  DELETE FROM ZnodePublishedXml WHERE PublishCatalogLogId = @versionId AND IsProductXML = 1   AND LocaleId = @LocaleId 

--  --ALTER INDEX ALL ON ZnodePublishedXml  REBUILD WITH (FILLFACTOR = 80 ) 
  If (@PimCategoryHierarchyId <> 0 AND @PimCatalogId <> 0 )
  BEGIN
		
		--Collect index of other categorys
		IF OBJECT_ID('tempdb..#Index') is not null
		BEGIN 
			DROP TABLE #Index
		END 
		CREATE TABLE #Index (RowIndex int ,PublishCategoryId int , PublishProductId  int )		
		insert into  #Index ( RowIndex ,PublishCategoryId , PublishProductId )
		Select CAST(ROW_NUMBER()Over(Partition BY ZPC.PublishProductId 
		Order BY ISNULL(ZPC.PublishCategoryId,'0') desc )   AS VARCHAR(100)),
		ZPC.PublishCategoryId , ZPC.PublishProductId
		FROM ZnodePublishCategoryProduct ZPC where ZPC.PublishCatalogId = @PublishCatalogId
	
		--Publish parent products with index number 
		INSERT INTO ZnodePublishedXml (PublishCatalogLogId,PublishedId,PublishedXML,IsProductXML,LocaleId
		,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishCategoryId)
		SELECT zpp.VersionId,zpp.PublishProductId,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPPDFG.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
		+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
		+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfileCatalog ZPFC 
						INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
						WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+
						'</TempProfileIds>
						 <ProductIndex>'+ 
								--CAST(ROW_NUMBER()Over(Partition BY zpp.PublishProductId Order BY ISNULL(ZPCP.PublishCategoryId,'0') desc ) 
								CAST(Isnull((select RowIndex from #Index WHERE PublishProductId = zpp.PublishProductId
								AND PublishCategoryId = ZPCP.PublishCategoryId  ),0)
								AS VARCHAr(100))+
						'</ProductIndex>
						<IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
		'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
		STUFF(( SELECT '  '+ AttributeValue  FROM #Cte_GetData TY WHERE TY.PimProductId = ZPP.PimProductId   
		FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue,1,@LocaleId,@UserId , @GetDate , @UserId,@GetDate
		,ZPCP.PublishCategoryId
		FROM  #TBL_PublishCatalogId zpp
		INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
		LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId 
		AND ZPP.PublishCategoryId = ZPC.PublishCategoryId 
		)
		LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId AND  ZPCCF.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId
		)
		LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ISNULL(ZPCP.PublishCategoryId,0) AND ZPCD.LocaleId = @LocaleId )
		WHERE ZPPDFG.LocaleId = @LocaleId AND  ZPC.PimCategoryId in (Select CategoryId from @TBL_CategoryCategoryHierarchyIds ) 
		and zpp.PublishCategoryId is NOT NULL
		

	 --Publish only associate product 
	 INSERT INTO ZnodePublishedXml (PublishCatalogLogId,PublishedId,PublishedXML,IsProductXML,LocaleId
		,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishCategoryId)
		SELECT zpp.VersionId,zpp.PublishProductId,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPPDFG.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
		+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
		+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfileCatalog ZPFC 
						INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
						WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+
						'</TempProfileIds>
						 <ProductIndex>'+ 
								--CAST(ROW_NUMBER()Over(Partition BY zpp.PublishProductId Order BY ISNULL(ZPCP.PublishCategoryId,'0') desc ) 
								CAST(Isnull((select RowIndex from #Index WHERE PublishProductId = zpp.PublishProductId  AND (PublishCategoryId = ZPCP.PublishCategoryId  OR PublishCategoryId is null ) 
								) ,0)
								AS VARCHAr(100))+
						'</ProductIndex>
						<IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
		'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
		STUFF(( SELECT '  '+ AttributeValue  FROM #Cte_GetData TY WHERE TY.PimProductId = ZPP.PimProductId   
		FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue,1,@LocaleId,@UserId , @GetDate , @UserId,@GetDate
		,ZPCP.PublishCategoryId
		FROM  #TBL_PublishCatalogId zpp
		INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
		LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId 
		AND ZPP.PublishCategoryId = ZPC.PublishCategoryId 
		AND ZPC.PimCategoryId in (Select CategoryId from @TBL_CategoryCategoryHierarchyIds ))
		LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId AND  ZPCCF.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId
		AND ZPCCF.PimCategoryId in (Select CategoryId from @TBL_CategoryCategoryHierarchyIds ))
		LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ISNULL(ZPCP.PublishCategoryId,0) AND ZPCD.LocaleId = @LocaleId )
		WHERE ZPPDFG.LocaleId = @LocaleId and zpp.PublishCategoryId is  NULL
		

  END
  ELSE
  BEGIN
		INSERT INTO ZnodePublishedXml (PublishCatalogLogId
		,PublishedId
		,PublishedXML
		,IsProductXML
		,LocaleId
		,CreatedBy
		,CreatedDate
		,ModifiedBy
		,ModifiedDate
		,PublishCategoryId)
		SELECT zpp.VersionId,zpp.PublishProductId,'<ProductEntity><VersionId>'+CAST(zpp.VersionId AS VARCHAR(50)) +'</VersionId><ZnodeProductId>'+CAST(zpp.PublishProductId AS VARCHAR(50))+'</ZnodeProductId><ZnodeCategoryIds>'+CAST(ISNULL(ZPCP.PublishCategoryId,'')  AS VARCHAR(50))+'</ZnodeCategoryIds><Name>'+CAST(ISNULL((SELECT ''+ZPPDFG.ProductName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</Name>'+'<SKU>'+CAST(ISNULL((SELECT ''+ZPPDFG.SKU FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKU>'+'<SKULower>'+CAST(ISNULL((SELECT ''+LOWER(ZPPDFG.SKU) FOR XML PATH ('')),'') AS NVARCHAR(2000))+ '</SKULower>'+'<IsActive>'+CAST(ISNULL(ZPPDFG.IsActive ,'0') AS VARCHAR(50))+'</IsActive>' 
		+'<ZnodeCatalogId>'+CAST(ZPP.PublishCatalogId  AS VARCHAR(50))+'</ZnodeCatalogId><IsParentProducts>'+CASE WHEN ZPCD.PublishCategoryId IS NULL THEN '0' ELSE '1' END  +'</IsParentProducts><CategoryName>'+CAST(ISNULL((SELECT ''+PublishCategoryName FOR XML PATH ('')),'') AS NVARCHAR(2000)) +'</CategoryName><CatalogName>'+CAST(ISNULL((SELECT ''+CatalogName FOR XML PATH ('')),'') AS NVARCHAR(2000))+'</CatalogName><LocaleId>'+CAST( @LocaleId AS VARCHAR(50))+'</LocaleId>'
		+'<TempProfileIds>'+ISNULL(SUBSTRING( (SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
						FROM ZnodeProfileCatalog ZPFC 
						INNER JOIN ZnodeProfileCatalogCategory ZPCCH  ON ( ZPCCH.ProfileCatalogId = ZPFC.ProfileCatalogId )
						WHERE ZPCCH.PimCatalogCategoryId = ZPCCF.PimCatalogCategoryId  FOR XML PATH('')),2,8000),'')+'</TempProfileIds><ProductIndex>'+CAST(ROW_NUMBER()Over(Partition BY zpp.PublishProductId Order BY ISNULL(ZPC.PublishCategoryId,'0') ) AS VARCHAr(100))+'</ProductIndex><IndexId>'+CAST( ISNULL(ZPCP.PublishCategoryProductId,'0') AS VARCHAr(100))+'</IndexId>'+
		'<DisplayOrder>'+CAST(ISNULL(ZPCCF.DisplayOrder,'') AS VARCHAR(50))+'</DisplayOrder>'+
		STUFF(( SELECT '  '+ AttributeValue  FROM #Cte_GetData TY WHERE TY.PimProductId = ZPP.PimProductId   
		FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</ProductEntity>' xmlvalue,1,@LocaleId,@UserId , @GetDate , @UserId,@GetDate
		,ZPCP.PublishCategoryId
		FROM  #TBL_PublishCatalogId zpp
		INNER JOIN ZnodePublishCatalog ZPCV ON (ZPCV.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePublishProductDetail ZPPDFG ON (ZPPDFG.PublishProductId =  ZPP.PublishProductId)
		LEFT JOIN ZnodePublishCategoryProduct ZPCP ON (ZPCP.PublishProductId = ZPP.PublishProductId AND ZPCP.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT JOIN ZnodePublishCategory ZPC ON (ZPCP.PublishCatalogId = ZPC.PublishCatalogId AND   ZPC.PublishCategoryId = ZPCP.PublishCategoryId )
		LEFT JOIN ZnodePimCatalogCategory ZPCCF ON (ZPCCF.PimCatalogId = ZPCV.PimCatalogId AND ZPCCF.PimCategoryId = ZPC.PimCategoryId  AND ZPCCF.PimProductId = ZPP.PimProductId AND  ZPCCF.PimCategoryHierarchyId =  ZPC.PimCategoryHierarchyId)
		LEFT JOIN ZnodePublishCategoryDetail ZPCD ON (ZPCD.PublishCategoryId = ISNULL(ZPCP.PublishCategoryId,0) AND ZPCD.LocaleId = @LocaleId )
		WHERE ZPPDFG.LocaleId = @LocaleId
		
END 
 


--DELETE FROM @PimProductAttributeXML
DELETE FROM @TBL_CustomeFiled
DELETE FROM @PimDefaultValueLocale
 IF OBJECT_ID('tempdb..#PimProductAttributeXML') is not null
 BEGIN 
 DELETE FROM #PimProductAttributeXML
 END
 IF OBJECT_ID('tempdb..#Cte_GetData') is not null
 BEGIN 
 DROP TABLE #Cte_GetData
 END
  IF OBJECT_ID('tempdb..#TBL_AttributeVAlue') is not null
 BEGIN 
 DROP TABLE #TBL_AttributeVAlue
 END
SET @Counter = @counter + 1 
END 

END
GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetTextWidgetConfiguration')
BEGIN 
	DROP PROCEDURE Znode_GetTextWidgetConfiguration
END 
GO

CREATE PROCEDURE [dbo].[Znode_GetTextWidgetConfiguration]
(
       @PortalId INT
	   ,@UserId INT =  0  	
	   ,@CMSMappingId INT =0,
	   @LocaleId INT = 0
)
AS
/*
Summary: This Procedure is used to get text widget configuration
Unit Testing :
 EXEC Znode_GetTextWidgetConfiguration 1,2

 exec Znode_GetTextWidgetConfiguration 1,2,213,1

 exec Znode_GetTextWidgetConfiguration @PortalId=1,@UserId=2,@CMSMappingId=213
*/
     BEGIN
         BEGIN TRY
		
             DECLARE @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId() ,@IncrementValue INT= 1;

             DECLARE @LocaleAll TABLE (
                                      RowId    INT IDENTITY(1 , 1) ,
                                      LocaleId INT ,
                                      Code     VARCHAR(300)
                                      );
             INSERT INTO @LocaleAll ( LocaleId , Code
                                    )
                    SELECT LocaleId , Code
                    FROM ZnodeLocale AS a
                    WHERE a.IsActive = 1 AND
					a.LocaleId IN (CASE WHEN  @LocaleId = 0  THEN LocaleId ELSE @LocaleId END)
					;

             DECLARE @ReturnXML TABLE (
                                      ReturnXMl XML
                                      );
             WHILE @IncrementValue <= ( SELECT MAX(RowId)
                                        FROM @LocaleAll
                                      )
                 BEGIN
                     DECLARE @CMSWidgetData TABLE (CMSTextWidgetConfigurationId INT ,LocaleId  INT ,CMSWidgetsId INT ,WidgetsKey NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping   NVARCHAR(100) ,[Text]  NVARCHAR(MAX));
                     
					 DECLARE @CMSWidgetDataFinal TABLE (CMSTextWidgetConfigurationId INT ,LocaleId    INT ,CMSWidgetsId INT ,WidgetsKey  NVARCHAR(256) ,CMSMappingId  INT ,TypeOFMapping NVARCHAR(100) ,[Text]  NVARCHAR(MAX));

                     INSERT INTO @CMSWidgetDataFinal
                            SELECT CMSTextWidgetConfigurationId , LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
                            FROM ZnodeCMSTextWidgetConfiguration AS a
                            WHERE (a.TypeOFMapping = 'ContentPageMapping'
                            AND ( EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSContentPages  WHERE a.CMSMappingId = CMSContentPagesId AND PortalId = @PortalId  ))
                            OR (a.TypeOFMapping = 'PortalMapping' AND a.CMSMappingId = @PortalId )
							AND
                                 ( a.LocaleId IN 
								 ( ( SELECT LocaleId
                                                      FROM @LocaleAll
                                                      WHERE RowId = @IncrementValue
                                                    ) , @DefaultLocaleId 
                                                  ) ) )
										  
						   AND (a.CMSMappingId = @CMSMappingId OR @CMSMappingId = 0  )
						 	  


                     INSERT INTO @CMSWidgetData
                            SELECT CMSTextWidgetConfigurationId , (SELECT  LocaleId FROM @LocaleAll WHERE RowId = @IncrementValue)  AS LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
                            FROM @CMSWidgetDataFinal
                            WHERE LocaleId = ( SELECT LocaleId
                                               FROM @LocaleAll
                                               WHERE RowId = @IncrementValue
                                             );



                     INSERT INTO @CMSWidgetData
                            SELECT CMSTextWidgetConfigurationId , ( SELECT LocaleId FROM @LocaleAll WHERE RowId = @IncrementValue) AS LocaleId , CMSWidgetsId , WidgetsKey , CMSMappingId , TypeOFMapping , [Text]
                            FROM @CMSWidgetDataFinal AS p
                            WHERE p.LocaleId = @DefaultLocaleId
                                  AND
                                  NOT EXISTS ( SELECT TOP 1 1
                                               FROM @CMSWidgetData AS q
                                               WHERE q.CMSWidgetsId = p.CMSWidgetsId
                                                     AND
                                                     q.WidgetsKey = p.WidgetsKey
                                                     AND
                                                     q.TypeOFMapping = p.TypeOFMapping
                                                     AND
                                                     q.CMSMappingId = p.CMSMappingId
                                             );

										

                     INSERT INTO @ReturnXML ( ReturnXMl
                                            )
                            SELECT ( SELECT CMSTextWidgetConfigurationId AS TextWidgetConfigurationId , LocaleId , CMSWidgetsId AS WidgetsId , WidgetsKey , CMSMappingId AS MappingId , TypeOFMapping , [Text] , @PortalId AS PortalId
                                     FROM @CMSWidgetData AS a
                                     WHERE a.CMSTextWidgetConfigurationId = w.CMSTextWidgetConfigurationId 
                                     FOR XML PATH('TextWidgetEntity')
                                   )
                            FROM @CMSWidgetData AS w
						
							;
                     SET @IncrementValue = @IncrementValue + 1;
                     DELETE FROM @CMSWidgetData;
                     DELETE FROM @CMSWidgetDataFinal;
                 END;
             SELECT *
             FROM @ReturnXML;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetTextWidgetConfiguration @PortalId = '+CAST(@PortalId AS VARCHAR(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetTextWidgetConfiguration',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_ImportSEODetails')
BEGIN 
	DROP PROCEDURE Znode_ImportSEODetails
END 
GO
CREATE PROCEDURE [dbo].[Znode_ImportSEODetails](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @LocaleId int= 1,@PortalId int ,@CsvColumnString nvarchar(max))
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import SEO Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
	
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive RoundOff Value from global setting 

		-- Three type of import required three table varible for product , category and brand
		DECLARE @InsertSEODetails TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max), 
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit,
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		DECLARE @InsertSEODetailsOFProducts TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max),
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit, 
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		DECLARE @InsertSEODetailsOFCategory TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max),
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit, 
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		DECLARE @InsertSEODetailsOFBrand TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max), 
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit, 
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		
		DECLARE @InsertedZnodeCMSSEODetail TABLE
		( 
			CMSSEODetailId int , SEOCode Varchar(4000), CMSSEOTypeId int
		);
		
		--SET @SSQL = 'Select RowNumber,ImportType,Code,IsRedirect,MetaInformation,SEOUrl,IsActive,SEOTitle,SEODescription,SEOKeywords,GUID  FROM '+@TableName;
		SET @SSQL = 'Select RowNumber,'+@CsvColumnString+',GUID  FROM '+@TableName;

		INSERT INTO @InsertSEODetails(RowNumber,ImportType,Code,IsRedirect,MetaInformation,SEOUrl,IsActive,SEOTitle,SEODescription,SEOKeywords,RedirectFrom,RedirectTo,EnableRedirection,GUID )
		EXEC sys.sp_sqlexec @SSQL;

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '30', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii 
			   where ii.SEOURL in (Select ISD.SEOURL from @InsertSEODetails ISD Group by ISD.SEOUrl having count(*) > 1 ) 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '10', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii 
			   where EXISTS (Select * from ZnodeCMSSEODetail ZCSD WHERE ZCSD.SEOUrl = ii.SEOUrl AND ZCSD.PortalId = @PortalId) 

		--INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		--	   SELECT '35', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		--	   FROM @InsertSEODetails AS ii 
		--	   where ii.RedirectFrom = ii.RedirectTo 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '35', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii
			   WHERE ltrim(rtrim(isnull(ii.SEOUrl,''))) like '% %' -----space not allowed



		DELETE FROM @InsertSEODetails
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);
		
		SET @SSQL = 'Select RowNumber,' +@CsvColumnString +',GUID  FROM '+@TableName
		+ ' Where ImportType = ''Product'' ';
		INSERT INTO @InsertSEODetailsOFProducts(  RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, GUID )
		EXEC sys.sp_sqlexec @SSQL;

		SET @SSQL = 'Select RowNumber,' +@CsvColumnString +',GUID  FROM '+@TableName
		+ ' Where ImportType = ''Category'' ';
		INSERT INTO @InsertSEODetailsOFCategory( RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection	, GUID )
		EXEC sys.sp_sqlexec @SSQL;

		SET @SSQL = 'Select RowNumber,' +@CsvColumnString +',GUID  FROM '+@TableName
		+ ' Where ImportType = ''Brand'' ';
		INSERT INTO @InsertSEODetailsOFBrand( RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection	, GUID )
		EXEC sys.sp_sqlexec @SSQL;

	    -- start Functional Validation 
		--1. Product
		--2. Category
		--3. Content Page
		--4. Brand
		-----------------------------------------------

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'ImportType', ImportType, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii
			   WHERE ii.ImportType NOT in 
			   (
				   Select NAME from ZnodeCMSSEOType where NAME <> 'Content Page'
			   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'SKU', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetailsOFProducts AS ii
			   WHERE ii.CODE NOT in 
			   (
					SELECT ZPAVL.AttributeValue
					FROM ZnodePimAttributeValue ZPAV 
					inner join ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
					inner join ZnodePimAttribute ZPA on ZPAV.PimAttributeId = ZPA.PimAttributeId
					Where ZPA.AttributeCode = 'SKU'
			   )  AND ImportType = 'Product';


		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'Category', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetailsOFCategory AS ii
			   WHERE ii.CODE NOT in 
			   (
					SELECT ZPCAVL.CategoryValue
					FROM ZnodePimCategoryAttributeValue ZPCAV 
					INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL on ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId
					INNER JOIN ZnodePimAttribute ZPA on ZPCAV.PimAttributeId = ZPA.PimAttributeId
					Where ZPA.AttributeCode = 'CategoryCode'
			   )  AND ImportType = 'Category';

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'Brand', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetailsOFBrand AS ii
			   WHERE ii.CODE NOT in 
			   (
				   Select BrandCode from ZnodeBrandDetails 
			   )  AND ImportType = 'Brand';
		
		
		--Note : Content page import is not required 
		
		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  

		DELETE FROM @InsertSEODetailsOFProducts
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		DELETE FROM @InsertSEODetailsOFCategory
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		DELETE FROM @InsertSEODetailsOFBrand
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		-- Insert Product Data 
		If Exists (Select top 1 1 from @InsertSEODetailsOFProducts)
		Begin
			Update ZCSD SET ZCSD.IsRedirect = ISD.IsRedirect ,
						   ZCSD.MetaInformation =  ISD.MetaInformation,
						   ZCSD.SEOUrl=  ISD.SEOUrl,
						   ZCSD.IsPublish = 0
			FROM 
			@InsertSEODetailsOFProducts ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = 1 AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId  =@PortalId  AND ZCSDL.LocaleId = @LocaleId;
			
			Update ZCSDL SET ZCSDL.SEOTitle = ISD.SEOTitle
							,ZCSDL.SEODescription = ISD.SEODescription
							,ZCSDL.SEOKeywords= ISD.SEOKeywords
 			FROM 
			@InsertSEODetailsOFProducts ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = 1 AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId = @PortalId AND ZCSDL.LocaleId = @LocaleId; 

			Delete from @InsertedZnodeCMSSEODetail
			INSERT INTO ZnodeCMSSEODetail(CMSSEOTypeId,SEOCode,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
			OUTPUT Inserted.CMSSEODetailId,Inserted.SEOCode,Inserted.CMSSEOTypeId INTO @InsertedZnodeCMSSEODetail		
			Select Distinct 1,ISD.Code , ISD.IsRedirect,ISD.MetaInformation,@PortalId,ISD.SEOUrl,@USerId, @GetDate,@USerId, @GetDate from 
			@InsertSEODetailsOFProducts ISD  
			where NOT EXISTS (Select TOP 1 1 from ZnodeCMSSEODetail ZCSD where ZCSD.CMSSEOTypeId = 1 AND ZCSD.SEOCode = ISD.Code and  ZCSD .PortalId =@PortalId   );
		
        	insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			Select Distinct IZCSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate 
			from @InsertedZnodeCMSSEODetail IZCSD 
			INNER JOIN @InsertSEODetailsOFProducts ISD ON IZCSD.SEOCode = ISD.Code 

			-----RedirectUrlInsert
			INSERT INTO ZnodeCMSUrlRedirect ( RedirectFrom,RedirectTo,IsActive,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select RedirectFrom,RedirectTo,EnableRedirection,@PortalId as PortalId ,2 as CreatedBy,getdate() as CreatedDate,2 as ModifiedBy,getdate() as ModifiedDate
			from @InsertSEODetailsOFProducts
			where IsRedirect = 1
		END

		-- Insert Category Data 
		If Exists (Select top 1 1 from @InsertSEODetailsOFCategory)
		Begin

			Update ZCSD SET ZCSD.IsRedirect = ISD.IsRedirect ,
						   ZCSD.MetaInformation =  ISD.MetaInformation,
						   ZCSD.SEOUrl=  ISD.SEOUrl,
						   ZCSD.IsPublish = 0
			FROM 
			@InsertSEODetailsOFCategory ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = 2 AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId  =@PortalId  AND ZCSDL.LocaleId = @LocaleId;
			
			
			Update ZCSDL SET ZCSDL.SEOTitle = ISD.SEOTitle
							,ZCSDL.SEODescription = ISD.SEODescription
							,ZCSDL.SEOKeywords= ISD.SEOKeywords
 			FROM 
			@InsertSEODetailsOFCategory ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = 2 AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId  =@PortalId AND ZCSDL.LocaleId = @LocaleId; 

			Delete from @InsertedZnodeCMSSEODetail

			INSERT INTO ZnodeCMSSEODetail(CMSSEOTypeId,SEOCode,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
			OUTPUT Inserted.CMSSEODetailId,Inserted.SEOCode,Inserted.CMSSEOTypeId INTO @InsertedZnodeCMSSEODetail		
			Select Distinct 2,ISD.Code , ISD.IsRedirect,ISD.MetaInformation,@PortalId,ISD.SEOUrl,@USerId, @GetDate,@USerId, @GetDate 
			from @InsertSEODetailsOFCategory ISD  
			where NOT EXISTS (Select TOP 1 1 from ZnodeCMSSEODetail ZCSD where ZCSD.CMSSEOTypeId = 2 AND ZCSD.SEOCode  = ISD.Code AND ZCSD.PortalId = @PortalId );

			insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			Select Distinct IZCSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate 
			from @InsertedZnodeCMSSEODetail IZCSD 
			INNER JOIN @InsertSEODetailsOFCategory ISD ON IZCSD.SEOCode = ISD.Code 
			WHERE IZCSD.CMSSEOTypeId =2  

			-----RedirectUrlInsert
			INSERT INTO ZnodeCMSUrlRedirect ( RedirectFrom,RedirectTo,IsActive,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT RedirectFrom,RedirectTo,EnableRedirection,@PortalId as PortalId ,2 as CreatedBy,getdate() as CreatedDate,2 as ModifiedBy,getdate() as ModifiedDate
			FROM @InsertSEODetailsOFProducts SDP
			WHERE IsRedirect = 1
		END
										 
		--select 'End'
		--      SET @Status = 1;
		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 2 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH

		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		ROLLBACK TRAN A;
	END CATCH;
END;
GO


GO

Insert  INTO ZnodeActions (AreaName,ControllerName,ActionName,IsGlobalAccess,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select NULL ,'Content','MovePageToFolder',0,2,Getdate(),2,Getdate() where not exists
(select * from ZnodeActions where ControllerName = 'Content' and ActionName = 'MovePageToFolder')
 Go

insert into ZnodeActionMenu ( MenuId,	ActionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
	   (select MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content')	
      ,(select ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'MovePageToFolder')	,2,Getdate(),2,Getdate()
where not exists (select * from ZnodeActionMenu where MenuId = 
       (select MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') and ActionId = 
       (select ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'MovePageToFolder'))
Go

insert into ZnodeMenuActionsPermission ( MenuId,	ActionId, AccessPermissionId,	CreatedBy	,CreatedDate,	ModifiedBy,	ModifiedDate )
select 
(select MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content'),
(select ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'MovePageToFolder')	
,1,2,Getdate(),2,Getdate() where not exists 
(select * from ZnodeMenuActionsPermission where MenuId = 
(select MenuId from ZnodeMenu where MenuName = 'Pages' AND ControllerName = 'Content') and ActionId = 
(select ActionId from ZnodeActions where ControllerName = 'Content' and ActionName= 'MovePageToFolder'))

GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetCatalogProductSEODetail')
BEGIN 
	DROP PROCEDURE Znode_GetCatalogProductSEODetail
END 
GO


CREATE PROCEDURE [dbo].[Znode_GetCatalogProductSEODetail]
( 
  @WhereClause      NVARCHAR(MAX),
  @Rows             INT           = 100,
  @PageNo           INT           = 1,
  @Order_BY         VARCHAR(1000) = '',
  @RowsCount        INT OUT,
  @LocaleId         INT           = 1,
  @PortalId			INT
 
  )
AS
   
/*
	   Summary:  Get product List  Catalog / category / respective product list   		   
	   Unit Testing   
	   begin tran
	   declare @p7 int = 0  
	   EXEC Znode_GetCatalogProductSEODetail @WhereClause=N'',@Rows=100,@PageNo=1,@Order_By=N'',
	   @RowsCount=@p7 output,@PortalId= 1 ,@LocaleId=1 
	   rollback tran
	  
	     declare @p7 int = 0  
	   EXEC Znode_GetCatalogProductSEODetails @WhereClause=N'',@Rows=10,@PageNo=1,@Order_By=N'',
	   @RowsCount=@p7 output,@PortalId= 5 ,@LocaleId=1 


    */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE  @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @SQL NVARCHAR(MAX), 
					 @PimProductId TransferId,
					 @PimAttributeId VARCHAR(MAX)
					
             DECLARE @TransferPimProductId TransferId 
		
	
			IF OBJECT_ID('TEMPDB..#ProductDetail') IS NOT NULL
			DROP TABLE #ProductDetail

			IF OBJECT_ID('TEMPDB..##TempProductDetail') IS NOT NULL
			DROP TABLE ##TempProductDetail

			IF OBJECT_ID('TEMPDB..#znodeCatalogProduct') IS NOT NULL
		DROP TABLE #znodeCatalogProduct

			Declare @PimCatalogId INT

			SELECT @PimCatalogId = PimCatalogId 
			FROM ZnodePortalCatalog ZPC
			INNER JOIN ZnodePublishCatalog PC ON ZPC.PublishCatalogId = pc.PublishCatalogId WHERE PortalId = @PortalId
				
                SELECT  PimProductid,SKU,ProductName,ProductImage,IsActive,LocaleId
				INTO #ProductDetail
				 FROM 
				 (
				 SELECT c.pimproductId,PA.attributecode,e.AttributeValue,e.LocaleId
				 FROM
				 znodePimProduct c 
				 inner join ZnodePimAttributeValue d on (c.PimProductid = d.PimProductid)
				 inner join ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)
				 inner join ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)
				 where  PA.Attributecode IN ('SKU','ProductName','ProductImage','IsActive')
				-- AND e.localeid = @LocaleId
				 ) piv PIVOT(MAX(AttributeValue) FOR AttributeCode in ( SKU,ProductName,ProductImage,IsActive))AS PVT
				


		SET @SQL = 
		'
		--DECLARE @TBL_PimMediaAttributeId TABLE (PimAttributeId INT ,AttributeCode VARCHAR(600))
		--INSERT INTO @TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
		--	 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()

		select distinct PimCatalogId,PimProductId into #znodeCatalogProduct
	FROm ZnodePimCatalogCategory

		DECLARE @TBL_MediaValue TABLE (PimAttributeValueId INT,PimProductId INT,MediaPath INT,PimAttributeId INt,LocaleId INT )
		INSERT INTO @TBL_MediaValue
		SELECT ZPAV.PimAttributeValueId	,ZPAV.PimProductId	,ZPPAM.MediaId MediaPath,ZPAV.PimAttributeId , 	ZPPAM.LocaleId
					FROM ZnodePimAttributeValue ZPAV
					INNER JOIN ZnodePimProductAttributeMedia ZPPAM ON ( ZPPAM.PimAttributeValueId = ZPAV.PimAttributeValueId)
					INNER JOIN #ProductDetail PD ON (PD.PimProductId = ZPAV.PimProductId)
					LEFT JOIN ZnodeMedia ZM ON (Zm.Path = ZPPAM.MediaPath) 
					WHERE  ZPAV.PimAttributeId = (select PimAttributeId from ZnodePimAttribute pa where attributecode = ''ProductImage'') 

		;WITH Cte_ProductMedia
               AS (SELECT PD.PimProductId  , 
			   URL+ZMSM.ThumbnailFolderName+''/''+ zm.PATH  AS ProductImagePath 
			   FROM ZnodeMedia AS ZM
               INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)
			   INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
			   INNER JOIN @TBL_MediaValue PD ON (PD.MediaPath = CAST(ZM.MediaId AS VARCHAR(50)))
			   --INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = PD.PimATtributeId )
			   
			   )

		, CTE_ProductDetail AS
	(
		SELECT DISTINCT  CD.pimproductId, SKU,ProductName,
		case WHEN  CD.IsActive = ''true'' THEN 1 ELSE 0 END IsActive, ISNULL(CSD.SEOCode,SKU) as SEOCode, CSD.SEOUrl, CSDL.SEOTitle, CSDL.SEODescription, CSDL.SEOKeywords,
		Case When Isnull(CSD.IsPublish ,0 ) = 0 then ''Draft'' ELSE ''Published'' END  IsPublish  , CPM.ProductImagePath, PC.CatalogName, CD.LocaleId
		FROM #ProductDetail CD
		INNER JOIN #znodeCatalogProduct PCC on CD.PimProductId = PCC.PimProductId
		INNER JOIN ZnodePimCatalog PC on PCC.PimCatalogId = PC.PimCatalogId
		LEFT JOIN ZnodeCMSSEOType CST ON CST.Name = ''Product''
		LEFT JOIN ZnodeCMSSEODetail CSD on LTRIM(RTRIM(CD.SKU)) = LTRIM(RTRIM(CSD.SEOCode)) and CSD.CMSSEOTypeId = CST.CMSSEOTypeId AND CSD.PortalId = '+CAST(@PortalId AS VARCHAR(10))+'
		LEFT JOIN ZnodeCMSSEODetailLocale CSDL ON  CSD.CMSSEODetailId = CSDL.CMSSEODetailId AND CSDL.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
		
		LEFT JOIN Cte_ProductMedia CPM ON (CPM.PimProductId = CD.PimProductId)
		WHERE PCC.PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(10))+' AND CD.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+', '+CAST(@DefaultLocaleId AS VARCHAR(50))+')
	)

	, CTE_ProductLocale AS
	(
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName, LocaleId
	FROM CTE_ProductDetail CPD
	WHERE CPD.LocaleId ='+CAST(@LocaleId AS VARCHAR(50))+'	
	)

	, CTE_ProductBothLocale AS
	(
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName
	FROM CTE_ProductLocale PL
	UNION ALL 
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName
	FROM CTE_ProductDetail PD 
	WHERE LocaleId ='+CAST(@DefaultLocaleId AS VARCHAR(50))+' AND
	NOT EXISTS (select TOP 1 1 from CTE_ProductLocale PCL WHERE PCL.pimproductId = PD.pimproductId AND PCL.CatalogName = PD.CatalogName )
	)

	,CTE_ProductDetail_WhereClause AS
	(
		SELECT  pimproductId, SKU,ProductName,
		cast(IsActive as bit) IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName,'+[dbo].[Fn_GetPagingRowId](@Order_BY, 'PimProductId')+',Count(*)Over() CountId
		FROM CTE_ProductBothLocale CD
		WHERE 1 = 1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
	)
	SELECT  pimproductId, SKU,ProductName,IsActive, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath,CatalogName, CountId
	INTO ##TempProductDetail
	FROM CTE_ProductDetail_WhereClause
	'+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
	print @SQL
	EXEC (@SQL)

	SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM ##TempProductDetail ),0)

	SELECT  pimproductId,LTRIM(RTRIM(SKU)) AS SKU,ProductName,IsActive,LTRIM(RTRIM(SEOCode)) AS SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath 
	FROM ##TempProductDetail
	--GROUP by pimproductId, SKU,ProductName,IsActive, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath 

  
		

			IF OBJECT_ID('TEMPDB..#ProductDetail') IS NOT NULL
			DROP TABLE #ProductDetail

			IF OBJECT_ID('TEMPDB..##TempProductDetail') IS NOT NULL
			DROP TABLE ##TempProductDetail

			
			IF OBJECT_ID('TEMPDB..#znodeCatalogProduct') IS NOT NULL
		DROP TABLE #znodeCatalogProduct


         END TRY
         BEGIN CATCH
		    SELECT ERROR_message()
             DECLARE @Status BIT ;
		     SET @Status = 0;
		 --    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			-- @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCatalogCategoryProducts @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(MAX)),'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			--VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',
			--@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',@PimCategoryId='+ISNULL(CAST(@PimCategoryId AS VARCHAR(50)),'''')+',@PimCatalogId='+ISNULL(CAST(@PimCatalogId AS VARCHAR(50)),'''')+',@IsAssociated='+ISNULL(CAST(@IsAssociated AS VARCHAR(50)),'''')+',
			--@ProfileCatalogId='+ISNULL(CAST(@ProfileCatalogId AS VARCHAR(50)),'''')+',@AttributeCode='''+ISNULL(CAST(@AttributeCode AS VARCHAR(50)),'''''')+''',@PimCategoryHierarchyId='+ISNULL(CAST(@PimCategoryHierarchyId AS VARCHAR(10)),'''');
              			 
   --          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
   --          EXEC Znode_InsertProcedureErrorLog
			--	@ProcedureName = 'Znode_GetCatalogCategoryProducts',
			--	@ErrorInProcedure = 'Znode_GetCatalogCategoryProducts',
			--	@ErrorMessage = @ErrorMessage,
			--	@ErrorLine = @ErrorLine,
			--	@ErrorCall = @ErrorCall;
         END CATCH;
     END;

	 GO
IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetCatalogCategorySEODetail')
BEGIN 
	DROP PROCEDURE Znode_GetCatalogCategorySEODetail
END 
GO


  ---EXEC Znode_GetCatalogCategorySEODetail @WhereClause = '', @PortalId = 1, @RowsCount = 0
  CREATE PROCEDURE [dbo].[Znode_GetCatalogCategorySEODetail]
  (
	  @WhereClause      VARCHAR(MAX),
	  @Rows             INT           = 100,
	  @PageNo           INT           = 1,
	  @Order_BY         VARCHAR(1000) = '',
	  @RowsCount        INT OUT,
	  @LocaleId         INT           = 1,
	  @PortalId         INT
)
AS
BEGIN
	SET NOCOUNT ON;

	Declare @PimCatalogId INT, @SQL VARCHAR(MAX), @DefaultLocaleId VARCHAR(20)= dbo.Fn_GetDefaultLocaleId()

	SELECT @PimCatalogId = PimCatalogId 
	FROM ZnodePortalCatalog ZPC
	INNER JOIN ZnodePublishCatalog PC ON ZPC.PublishCatalogId = pc.PublishCatalogId WHERE PortalId = @PortalId


	IF OBJECT_ID('TEMPDB..#CategoryDetail') IS NOT NULL
		DROP TABLE #CategoryDetail

	IF OBJECT_ID('TEMPDB..##TempCategoryDetail') IS NOT NULL
		DROP TABLE ##TempCategoryDetail

		IF OBJECT_ID('TEMPDB..#znodeCatalogCategory') IS NOT NULL
		DROP TABLE #znodeCatalogCategory

	SELECT PimCategoryId, CategoryName, CategoryCode, LocaleId
	INTO #CategoryDetail
	FROM
	(
		SELECT ZPCAV.PimCategoryId,ZPA.AttributeCode,ZPCAVL.CategoryValue, ZPCAVL.LocaleId 
		FROM ZnodePimCategoryAttributeValue ZPCAV
		INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL on ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId
		INNER JOIN ZnodePimAttribute ZPA on ZPCAV.PimAttributeId = ZPA.PimAttributeId
		where ZPA.AttributeCode in ( 'CategoryName', 'CategoryCode')
	)TB PIVOT(MAX(CategoryValue) FOR AttributeCode in ( CategoryName, CategoryCode))AS PVT
	
	

	SET @SQL = '

	select distinct PimCatalogId,PimCategoryId into #znodeCatalogCategory
	FROm ZnodePimCatalogCategory

	;With CTE_CategoryDetail AS
	(
		SELECT DISTINCT PC.PimCatalogId, PC.CatalogName, CD.PimCategoryId, CD.CategoryCode, CD.CategoryName , ISNULL(CSD.SEOCode,CD.CategoryCode) as SEOCode , CSD.SEOUrl, CSDL.SEOTitle, CSDL.SEODescription, CSDL.SEOKeywords,
		Case When Isnull(CSD.IsPublish ,0 ) = 0 then ''Draft'' ELSE ''Published'' END  IsPublish , ActivationDate , ExpirationDate, ZPC.IsActive, CD.LocaleId 
		FROM #CategoryDetail CD
		INNER JOIN ZnodePimCategory ZPC ON (ZPC.PimCategoryId = CD.PimCategoryId)
		INNER JOIN  #znodeCatalogCategory PCC on CD.PimCategoryId = PCC.PimCategoryId
		INNER JOIN ZnodePimCatalog PC on PCC.PimCatalogId = PC.PimCatalogId
		LEFT JOIN ZnodePimCategoryHierarchy CH ON (CH.PimCategoryId = CD.PimCategoryId)
		LEFT JOIN ZnodeCMSSEOType CST ON CST.Name = ''Category''
		LEFT JOIN ZnodeCMSSEODetail CSD on CD.CategoryCode = CSD.SEOCode and CSD.CMSSEOTypeId = CST.CMSSEOTypeId AND CSD.PortalId = '+CAST(@PortalId AS VARCHAR(10))+'
		LEFT JOIN ZnodeCMSSEODetailLocale CSDL ON  CSD.CMSSEODetailId = CSDL.CMSSEODetailId AND CSDL.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
		WHERE PCC.PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(10))+' AND CD.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+', '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
	)
	,CTE_CategoryDetail_Locale as
	(
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, LocaleId
		FROM CTE_CategoryDetail CD
		WHERE CD.LocaleId ='+CAST(@LocaleId AS VARCHAR(50))+'
	)
	,CTE_CategoryDetail_BothLocale as
	(
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive
		FROM CTE_CategoryDetail_Locale
		Union All
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive
		FROM CTE_CategoryDetail CDS
		WHERE LocaleId ='+CAST(@DefaultLocaleId AS VARCHAR(50))+' AND
			NOT EXISTS( SELECT TOP 1 1 FROM CTE_CategoryDetail_Locale CSD1
						WHERE CDS.PimCategoryId = CSD1.PimCategoryId AND CDS.CatalogName = CSD1.CatalogName )
	)
	,CTE_CategoryDetail_WhereClause AS
	(
		SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, '+[dbo].[Fn_GetPagingRowId](@Order_BY, 'PimCategoryId')+',Count(*)Over() CountId
		FROM CTE_CategoryDetail_BothLocale CD
		WHERE 1 = 1 '+CASE WHEN @WhereClause = '' THEN '' ELSE ' AND '+@WhereClause END +'
	)
	SELECT PimCatalogId, CatalogName, PimCategoryId, CategoryCode,CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ActivationDate,ExpirationDate,IsActive, CountId
	INTO ##TempCategoryDetail
	FROM CTE_CategoryDetail_WhereClause
	'+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
	print @SQL
	EXEC (@SQL)

	SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM ##TempCategoryDetail ),0)

	SELECT  PimCategoryId, CategoryName, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish, ActivationDate,ExpirationDate,IsActive FROM ##TempCategoryDetail

	IF OBJECT_ID('TEMPDB..#CategoryDetail') IS NOT NULL
		DROP TABLE #CategoryDetail

	IF OBJECT_ID('TEMPDB..##TempCategoryDetail') IS NOT NULL
		DROP TABLE ##TempCategoryDetail

	IF OBJECT_ID('TEMPDB..#znodeCatalogCategory') IS NOT NULL
	DROP TABLE #znodeCatalogCategory

END

GO

IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_GetCMSContentPagesFolderDetails')
BEGIN 
	DROP PROCEDURE Znode_GetCMSContentPagesFolderDetails
END 
GO

CREATE   PROCEDURE [dbo].[Znode_GetCMSContentPagesFolderDetails]
( @WhereClause NVARCHAR(max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(100)  = NULL,
  @RowsCount   INT OUT,
  @LocaleId    INT           = 1)
AS  
   /* 
    Summary: To get content page folder details 
             Provide output for paging with dynamic where cluase                  
    		 User view : View_CMSContentPagesFolderDetails
    Unit Testing  
    Exec Znode_GetCMSContentPagesFolderDetails '',@RowsCount = 0
    
	*/
     BEGIN
        BEGIN TRY
          SET NOCOUNT ON;

		     DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @SQLWhereClause nvarchar(max)

			 
             DECLARE @DefaultLocaleId VARCHAR(100)= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_ContenetPageLocale TABLE(CMSContentPagesId INT,PortalId INT,CMSTemplateId INT,PageTitle NVARCHAR(200),PageName NVARCHAR(200),ActivationDate DATETIME, ExpirationDate DATETIME,IsActive BIT
				    ,CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME,PortalName  NVARCHAR(max) ,CMSContentPageGroupId INT 
				    , PageTemplateName NVARCHAR(200),SEOUrl NVARCHAR(max),MetaInformation NVARCHAR(max),SEODescription NVARCHAR(max),SEOTitle NVARCHAR(max),SEOKeywords NVARCHAR(max),CMSContentPageGroupName NVARCHAR(200),RowId INT ,CountNo INT,PublishStatus nvarchar(300)  ,SEOPublishStatus  nvarchar(300), SEOCode NVARCHAR(4000) )
			--SET @SQLWhereClause  = [dbo].[Fn_GetFilterWhereClause](@WhereClause) 
					SET @SQL = '  
						;With CMSContentPages AS (		
						SELECT DISTINCT ZCCP.CMSContentPagesId,ZCCP.PortalId,ZCCP.CMSTemplateId,ZCCPL.PageTitle,ZCCP.PageName,ZCCP.ActivationDate, ZCCP.ExpirationDate,ZCCP.IsActive
						,ZCCP.CreatedBy,ZCCP.CreatedDate,ZCCP.ModifiedBy,ZCCP.ModifiedDate,e.StoreName PortalName   ,ISNULL(ZCCPG.CMSContentPageGroupId,0) CMSContentPageGroupId
						,zct.Name PageTemplateName ,zcsd.SEOUrl,zcsd.MetaInformation,ZCCPGL.Name CMSContentPageGroupName,ZCCPL.LocaleId,ZCSDL.SEODescription,ZCSDL.SEOTitle,ZCSDL.SEOKeywords	,ZCSDL.LocaleId LocaleSeo,ZCCPGL.LocaleId LocaeIdRTR ,ZCCP.IsPublished
						, zcsd.IsPublish IsSEOPublished, ISNULL(ZCSD.SEOCode,ZCCP.PageName) as SEOCode
					    FROM ZnodeCMSContentPages ZCCP 
						LEFt Outer JOIN [ZnodeCMSContentPageGroupMapping] ZCCPGM ON (ZCCPGM.CMSContentPagesId = ZCCP.CMSContentPagesId) 
					    LEFt Outer JOIN [ZnodeCMSContentPageGroup] ZCCPG ON (ZCCPG.CMSContentPageGroupId = ZCCPGM.CMSContentPageGroupId)
						LEFt Outer JOIN [ZnodeCMSContentPagesLocale] ZCCPL ON (ZCCP.CMSContentPagesId = ZCCPL.CMSContentPagesId  )
						LEFt Outer JOIN [ZnodeCMSContentPageGroupLocale] ZCCPGL ON (ZCCPGL.CMSContentPageGroupId = ZCCPG.CMSContentPageGroupId AND ZCCPGL.LocaleId = ZCCPL.LocaleId  )					
						LEFT JOIN ZnodeCMSTemplate zct ON (zct.CMSTemplateId = ZCCP.CMSTemplateId )
						LEFT JOIN ZnodeCMSSEODetail zcsd ON (zcsd.SEOCode = ZCCP.PageName AND ZCSD.Portalid = ZCCP.portalId AND zcsd.PortalId IS NOT NULL  AND 
					    EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType zcst WHERE zcst.CMSSEOTypeId = zcsd.CMSSEOTypeId AND zcst.Name = ''Content Page'' ))
					    LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSDL.CMSSEODetailId = zcsd.CMSSEODetailId    AND zcsdl.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
						LEFt Outer JOIN ZnodePortal e on ZCCP.PortalId = e.PortalId 
					    WHERE  ZCCPL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
						--AND ZCSDL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
						--AND ZCCPGL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
					--	AND zcsd.PortalId IS NOT NULL
						 ) 
						, Cte_ContaintPageDetails AS (
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEODescription,SEOTitle,SEOKeywords,MetaInformation,IsPublished	, IsSEOPublished,SEOCode FROM CMSContentPages WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
      --                  AND LocaleSeo = '+CAST(@LocaleId AS VARCHAR(50))+'
						--AND LocaeIdRTR   = '+CAST(@LocaleId AS VARCHAR(50))+'
						)
						, Cte_ContentPage  AS (     	 
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
						, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation,IsPublished,IsSEOPublished,SEOCode	FROM Cte_ContaintPageDetails 
						UNION ALL 
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
						,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName ,CMSContentPageGroupId , PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation,IsPublished,IsSEOPublished,SEOCode
					    FROM CMSContentPages CCP WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
					    AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_ContaintPageDetails CTCPD WHERE CTCPD.CMSContentPagesId  = CCP.CMSContentPagesId AND  CTCPD.Portalid = CCp.PortalId) -- AND  LocaleSeo = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
					   -- AND LocaeIdRTR   = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
						)				

					    ,Cte_ContenetPageFilter AS (
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,SEOKeywords,SEOTitle,SEODescription,MetaInformation,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName   ,CMSContentPageGroupId 
						, PageTemplateName ,SEOUrl,CMSContentPageGroupName,Case When Isnull(IsPublished ,0 ) = 0 then ''Draft'' ELSE ''Published'' END PublishStatus
						,Case When Isnull(IsSEOPublished ,0 ) = 0 then ''Draft'' ELSE ''Published'' END SEOPublishStatus  , SEOCode
						
						 FROM Cte_ContentPage) '
   
						set @SQLWhereClause = @SQL + '
						
						,Cte_ContentFinal AS
						(
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,
						PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,SEOKeywords,SEOTitle,
						SEODescription,MetaInformation,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,
						PortalName,CMSContentPageGroupId, PageTemplateName ,
						SEOUrl,CMSContentPageGroupName,PublishStatus,SEOPublishStatus,'+[dbo].[Fn_GetPagingRowId](@Order_BY,'CMSContentPagesId')+',Count(*)Over() CountNo,SEOCode
						FROM Cte_ContenetPageFilter WHERE  1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )

						
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName   ,CMSContentPageGroupId 
						, PageTemplateName ,SEOUrl,CMSContentPageGroupName,RowId,CountNo,SEOKeywords,SEOTitle,SEODescription,MetaInformation,    PublishStatus,SEOPublishStatus, SEOCode
					    FROM Cte_ContentFinal  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
						
						--select @SQLWhereClause
						print @SQLWhereClause


					    INSERT INTO @TBL_ContenetPageLocale (CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,RowId,CountNo,SEOKeywords,SEOTitle,SEODescription,
									MetaInformation,PublishStatus,SEOPublishStatus,SEOCode)
           				
					    EXEC (@SQLWhereClause)      
						                                                     
					    SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ContenetPageLocale) ,0)   
						     
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate,ExpirationDate,IsActive
							   ,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
							   ,PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation,
							   PublishStatus,SEOPublishStatus,SEOCode
						FROM @TBL_ContenetPageLocale

           
    END TRY
	
    BEGIN CATCH
        DECLARE @Status BIT ;
		     SET @Status = 0;
			 select ERROR_MESSAGE()
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSContentPagesFolderDetails @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''');


              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		 
        EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSContentPagesFolderDetails',
				@ErrorInProcedure = 'Znode_GetCMSContentPagesFolderDetails',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
    END CATCH;
END;

GO


IF EXISTS (SELECT TOP 1 1 FROM SYS.procedures WHERE name = 'Znode_ImportSEODetails')
BEGIN 
	DROP PROCEDURE Znode_ImportSEODetails
END 
GO


CREATE PROCEDURE [dbo].[Znode_ImportSEODetails](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @LocaleId int= 1,@PortalId int ,@CsvColumnString nvarchar(max))
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import SEO Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
	
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		
		
		DECLARE @CMSSEOTypeProduct INT ,@CMSSEOTypeCategory INT

		SELECT @CMSSEOTypeProduct = CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Product'
		SELECT @CMSSEOTypeCategory = CMSSEOTypeId FROM ZnodeCMSSEOType WHERE Name = 'Category'


		-- Three type of import required three table varible for product , category and brand
		DECLARE @InsertSEODetails TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max), 
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit,
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		DECLARE @InsertSEODetailsOFProducts TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max),
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit, 
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		DECLARE @InsertSEODetailsOFCategory TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max),
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit, 
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		DECLARE @InsertSEODetailsOFBrand TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, ImportType varchar(20), Code nvarchar(300), 
			IsRedirect	bit	,MetaInformation	nvarchar(max),PortalId	int	,SEOUrl	nvarchar(max),IsActive bit,
			SEOTitle	nvarchar(max),SEODescription	nvarchar(max),SEOKeywords	nvarchar(max), 
			RedirectFrom nvarchar(max),RedirectTo nvarchar(max), EnableRedirection bit, 
			GUID nvarchar(400)
			--Index Ind_ImportType (ImportType),Index Ind_Code (Code)
		);

		
		DECLARE @InsertedZnodeCMSSEODetail TABLE
		( 
			CMSSEODetailId int , SEOCode Varchar(4000), CMSSEOTypeId int
		);
		
		--SET @SSQL = 'Select RowNumber,ImportType,Code,IsRedirect,MetaInformation,SEOUrl,IsActive,SEOTitle,SEODescription,SEOKeywords,GUID  FROM '+@TableName;
		SET @SSQL = 'Select RowNumber,'+@CsvColumnString+',GUID  FROM '+@TableName;

		INSERT INTO @InsertSEODetails(RowNumber,ImportType,Code,IsRedirect,MetaInformation,SEOUrl,IsActive,SEOTitle,SEODescription,SEOKeywords,RedirectFrom,RedirectTo,EnableRedirection,GUID )
		EXEC sys.sp_sqlexec @SSQL;

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '30', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii 
			   where ii.SEOURL in (Select ISD.SEOURL from @InsertSEODetails ISD Group by ISD.SEOUrl having count(*) > 1 ) 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '10', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii 
			   where EXISTS (Select TOP 1 1 from ZnodeCMSSEODetail ZCSD WHERE ZCSD.SEOUrl = ii.SEOUrl AND ZCSD.PortalId = @PortalId  AND EXISTS
			  (SELECT TOP 1 1 FROM ZnodeCMSSEODetailLocale dl WHERE dl.CMSSEODetailId = ZCSD.CMSSEODetailId AND dl.LocaleId = @LocaleId
											AND dl.SEODescription = ii.SEODescription AND dl.SEOTitle = ii.SEOTitle AND dl.SEOKeywords = ii.SEOKeywords)) 

		--INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		--	   SELECT '35', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		--	   FROM @InsertSEODetails AS ii 
		--	   where ii.RedirectFrom = ii.RedirectTo 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '35', 'SEOUrl', SEOUrl, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii
			   WHERE ltrim(rtrim(isnull(ii.SEOUrl,''))) like '% %' -----space not allowed

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'ImportType', ImportType, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetails AS ii
			   WHERE ii.ImportType NOT in 
			   (
				   Select NAME from ZnodeCMSSEOType where NAME NOT IN ('Content Page','BlogNews','Brand')
			   );


		DELETE FROM @InsertSEODetails
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);
		
	

-------------------------------------------------------------------------------------------------------------------------------

		INSERT INTO @InsertSEODetailsOFProducts(  RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, GUID )
			SELECT RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, GUID
			FROM @InsertSEODetails WHERE ImportType = 'Product'


		INSERT INTO @InsertSEODetailsOFCategory( RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection	, GUID )
			SELECT RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, GUID
			FROM @InsertSEODetails WHERE ImportType = 'Category'

		INSERT INTO @InsertSEODetailsOFBrand( RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection	, GUID )
			SELECT RowNumber , ImportType , Code , 
			IsRedirect	,MetaInformation	,SEOUrl	,IsActive ,
			SEOTitle	,SEODescription	,SEOKeywords, RedirectFrom, RedirectTo,EnableRedirection, GUID
			FROM @InsertSEODetails WHERE ImportType = 'Brand'


	    -- start Functional Validation 
		--1. Product
		--2. Category
		--3. Content Page
		--4. Brand
		-----------------------------------------------

		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'SKU', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetailsOFProducts AS ii
			   WHERE ii.CODE NOT in 
			   (
					SELECT ZPAVL.AttributeValue
					FROM ZnodePimAttributeValue ZPAV 
					inner join ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
					inner join ZnodePimAttribute ZPA on ZPAV.PimAttributeId = ZPA.PimAttributeId
					Where ZPA.AttributeCode = 'SKU' AND ZPAVL.AttributeValue IS NOT NULL 
			   )  AND ImportType = 'Product';

		
		--INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )

		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'Category', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetailsOFCategory AS ii
			   WHERE ii.CODE NOT in 
			   (
					SELECT ZPCAVL.CategoryValue
					FROM ZnodePimCategoryAttributeValue ZPCAV 
					INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL on ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId
					INNER JOIN ZnodePimAttribute ZPA on ZPCAV.PimAttributeId = ZPA.PimAttributeId
					Where ZPA.AttributeCode = 'CategoryCode' AND ZPCAVL.CategoryValue IS NOT NULL
			   )  AND ImportType = 'Category';

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '19', 'Brand', CODE, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
			   FROM @InsertSEODetailsOFBrand AS ii
			   WHERE ii.CODE NOT in 
			   (
				   Select BrandCode from ZnodeBrandDetails WHERE BrandCode IS NOT NULL
			   )  AND ImportType = 'Brand';
		
		
		--Note : Content page import is not required 
		
		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  

		DELETE FROM @InsertSEODetailsOFProducts
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		DELETE FROM @InsertSEODetailsOFCategory
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		DELETE FROM @InsertSEODetailsOFBrand
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

	
		-- Insert Product Data 
		If Exists (Select top 1 1 from @InsertSEODetailsOFProducts)
		Begin
			Update ZCSD SET ZCSD.IsRedirect = ISD.IsRedirect ,
						   ZCSD.MetaInformation =  ISD.MetaInformation,
						   ZCSD.SEOUrl=  ISD.SEOUrl,
						   ZCSD.IsPublish = 0
			FROM 
			@InsertSEODetailsOFProducts ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId  =@PortalId  AND ZCSDL.LocaleId = @LocaleId;
			
			Update ZCSDL SET ZCSDL.SEOTitle = ISD.SEOTitle
							,ZCSDL.SEODescription = ISD.SEODescription
							,ZCSDL.SEOKeywords= ISD.SEOKeywords
 			FROM 
			@InsertSEODetailsOFProducts ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId = @PortalId AND ZCSDL.LocaleId = @LocaleId; 

			
			insert into ZnodeCMSSEODetailLocale (CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT distinct CSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate
			FROM ZnodeCMSSEODetail CSD
			INNER JOIN @InsertSEODetailsOFProducts ISD ON CSD.SEOCode = ISD.Code AND CSD.CMSSEOTypeId = @CMSSEOTypeProduct AND CSD.SEOUrl = ISD.SEOUrl
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetailLocale CSDL WHERE CSDL.LocaleId = @LocaleId AND CSD.CMSSEODetailId = CSDL.CMSSEODetailId )
			AND CSD.portalId = @PortalId

			
			Delete from @InsertedZnodeCMSSEODetail

			IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetail SD INNER JOIN @InsertSEODetailsOFProducts DP ON SD.SEOCode = DP.Code AND SD.PortalId =  @PortalId
																								AND  SD.CMSSEOTypeId = @CMSSEOTypeProduct)
			BEGIN
			INSERT INTO ZnodeCMSSEODetail(CMSSEOTypeId,SEOCode,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
			OUTPUT Inserted.CMSSEODetailId,Inserted.SEOCode,Inserted.CMSSEOTypeId INTO @InsertedZnodeCMSSEODetail		
			Select Distinct @CMSSEOTypeProduct,ISD.Code , ISD.IsRedirect,ISD.MetaInformation,@PortalId,ISD.SEOUrl,@USerId, @GetDate,@USerId, @GetDate from 
			@InsertSEODetailsOFProducts ISD  
			where NOT EXISTS (Select TOP 1 1 from ZnodeCMSSEODetail ZCSD where ZCSD.CMSSEOTypeId = @CMSSEOTypeProduct AND ZCSD.SEOCode = ISD.Code and  ZCSD.PortalId =@PortalId   );
		
        	insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			Select Distinct IZCSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate 
			from @InsertedZnodeCMSSEODetail IZCSD 
			INNER JOIN @InsertSEODetailsOFProducts ISD ON IZCSD.SEOCode = ISD.Code 

			END
			

			-----RedirectUrlInsert
			INSERT INTO ZnodeCMSUrlRedirect ( RedirectFrom,RedirectTo,IsActive,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select RedirectFrom,RedirectTo,EnableRedirection,@PortalId as PortalId ,@USerId as CreatedBy,@GetDate as CreatedDate,@USerId as ModifiedBy,@GetDate as ModifiedDate
			from @InsertSEODetailsOFProducts
			where IsRedirect = 1
		END

		-- Insert Category Data 
		If Exists (Select top 1 1 from @InsertSEODetailsOFCategory)
		Begin

			Update ZCSD SET ZCSD.IsRedirect = ISD.IsRedirect ,
						   ZCSD.MetaInformation =  ISD.MetaInformation,
						   ZCSD.SEOUrl=  ISD.SEOUrl,
						   ZCSD.IsPublish = 0
			FROM 
			@InsertSEODetailsOFCategory ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeCategory AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId  =@PortalId  AND ZCSDL.LocaleId = @LocaleId;
			
			
			Update ZCSDL SET ZCSDL.SEOTitle = ISD.SEOTitle
							,ZCSDL.SEODescription = ISD.SEODescription
							,ZCSDL.SEOKeywords= ISD.SEOKeywords
 			FROM 
			@InsertSEODetailsOFCategory ISD  
			INNER JOIN ZnodeCMSSEODetail ZCSD ON  ZCSD.CMSSEOTypeId = @CMSSEOTypeCategory AND ZCSD.SEOCode = ISD.Code
			INNER JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId
			where  ZCSD.PortalId  =@PortalId AND ZCSDL.LocaleId = @LocaleId; 

			insert into ZnodeCMSSEODetailLocale (CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT distinct CSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate
			FROM ZnodeCMSSEODetail CSD
			INNER JOIN @InsertSEODetailsOFProducts ISD ON CSD.SEOCode = ISD.Code AND CSD.CMSSEOTypeId = @CMSSEOTypeCategory AND CSD.SEOUrl = ISD.SEOUrl
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetailLocale CSDL WHERE CSDL.LocaleId = @LocaleId AND CSD.CMSSEODetailId = CSDL.CMSSEODetailId )
			AND CSD.portalId = @PortalId


			Delete from @InsertedZnodeCMSSEODetail

			IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEODetail SD INNER JOIN @InsertSEODetailsOFProducts DP ON SD.SEOCode = DP.Code AND SD.PortalId =  @PortalId
																								AND  SD.CMSSEOTypeId = @CMSSEOTypeCategory)
			BEGIN
			INSERT INTO ZnodeCMSSEODetail(CMSSEOTypeId,SEOCode,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
			OUTPUT Inserted.CMSSEODetailId,Inserted.SEOCode,Inserted.CMSSEOTypeId INTO @InsertedZnodeCMSSEODetail		
			Select Distinct @CMSSEOTypeCategory,ISD.Code , ISD.IsRedirect,ISD.MetaInformation,@PortalId,ISD.SEOUrl,@USerId, @GetDate,@USerId, @GetDate 
			from @InsertSEODetailsOFCategory ISD  
			where NOT EXISTS (Select TOP 1 1 from ZnodeCMSSEODetail ZCSD where ZCSD.CMSSEOTypeId = @CMSSEOTypeCategory AND ZCSD.SEOCode  = ISD.Code AND ZCSD.PortalId = @PortalId );

			insert into ZnodeCMSSEODetailLocale(CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			Select Distinct IZCSD.CMSSEODetailId,@LocaleId,ISD.SEOTitle,ISD.SEODescription,ISD.SEOKeywords,@USerId, @GetDate,@USerId, @GetDate 
			from @InsertedZnodeCMSSEODetail IZCSD 
			INNER JOIN @InsertSEODetailsOFCategory ISD ON IZCSD.SEOCode = ISD.Code 
			WHERE IZCSD.CMSSEOTypeId =@CMSSEOTypeCategory  
			END

			-----RedirectUrlInsert
			INSERT INTO ZnodeCMSUrlRedirect ( RedirectFrom,RedirectTo,IsActive,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT RedirectFrom,RedirectTo,EnableRedirection,@PortalId as PortalId ,2 as CreatedBy,@GetDate as CreatedDate,2 as ModifiedBy,@GetDate as ModifiedDate
			FROM @InsertSEODetailsOFProducts SDP
			WHERE IsRedirect = 1
		END
										 
		--select 'End'
		--      SET @Status = 1;
		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 2 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH

		UPDATE ZnodeImportProcessLog
		  SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		ROLLBACK TRAN A;
	END CATCH;
END;

GO