
CREATE PROCEDURE [dbo].[Znode_GetPortalBrandDetail]  
( @WhereClause  NVARCHAR(MAX)='',  
  @Rows    INT           = 2147483647,  
  @PageNo   INT           = 1,  
  @Order_BY   VARCHAR(1000) = '',  
  @RowsCount  INT           = 0 OUT,  
  @LocaleId   INT           = 1,
  @PortalId     INT           = null ,     
  @IsAssociated  BIT           = 0,  
  @PromotionId      INT     = 0   
)  
AS  
  /*  
     Summary :- This Procedure is used to get brand localies   
     Unit Testing   
  begin tran  
     EXEC [Znode_GetPortalBrandDetail_R] @WhereClause='',@PortalId=null, @IsAssociated = 0
	 EXEC [Znode_GetPortalBrandDetail_R] @WhereClause   = '' ,    
     @Rows                    = 100 ,    
     @PageNo                  = 1 ,    
     @Order_BY      = '' ,    
     @RowsCount     =0  ,    
     @PortalId= 1 ,    
     @IsAssociated            = 1  
  rollback tran    
 */  
BEGIN  
BEGIN TRY  
        SET NOCOUNT ON;  
        DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();  
        DECLARE @SeoId VARCHAR(MAX)= '', @SQL NVARCHAR(MAX); --, @SeoCode NVARCHAR(MAX) ;
		DECLARE @PaginationWhereClause VARCHAR(300)= dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' WHERE RowId')
        
		DECLARE @TBL_BrandDetails TABLE  
        (
			Description         NVARCHAR(MAX),  
			BrandId             INT,  
			BrandCode           VARCHAR(600),  
			DisplayOrder        INT,  
			IsActive            BIT,  
			WebsiteLink         NVARCHAR(1000),  
			BrandDetailLocaleId INT,  
			SEOFriendlyPageName NVARCHAR(600),  
			MediaPath           NVARCHAR(MAX),  
			MediaId             INT,  
			ImageName           VARCHAR(300),
			BrandName			VARCHAR(100),	
			Custom1				NVARCHAR(MAX),	
			Custom2				NVARCHAR(MAX),
			Custom3				NVARCHAR(MAX),
			Custom4				NVARCHAR(MAX),
			Custom5				NVARCHAR(MAX),
			PortalId			Int,
			IsAssociated        Bit 
        );  
  
    DECLARE @AttributeId INT= [dbo].[Fn_GetProductBrandAttributeId]();  
             
	DECLARE @TBL_AttributeDefault TABLE  
    (
		PimAttributeId            INT,  
		AttributeDefaultValueCode VARCHAR(600),  
		IsEditable                BIT,  
		AttributeDefaultValue     NVARCHAR(MAX),
		DisplayOrder			  INT   
    );  

    DECLARE @TBL_SeoDetails TABLE  
    (
		CMSSEODetailId       INT,  
		SEOTitle             NVARCHAR(MAX),  
		SEOKeywords          NVARCHAR(MAX),  
		SEOURL               NVARCHAR(MAX),  
		ModifiedDate         DATETIME,  
		SEODescription       NVARCHAR(MAX),  
		MetaInformation      NVARCHAR(MAX),  
		IsRedirect           BIT,  
		CMSSEODetailLocaleId INT,  
		--SEOId                INT ,
		PublishStatus        NVARCHAR(20),
		SEOCode				 NVARCHAR(4000),
		CanonicalURL		 VARCHAR(200),
		RobotTag			 VARCHAR(50)			   
    );  

    DECLARE @TBL_BrandDetail TABLE  
    (
		Description          NVARCHAR(MAX),  
		BrandId              INT,  
		BrandCode            VARCHAR(600),  
		DisplayOrder         INT,  
		IsActive             BIT,  
		WebsiteLink          NVARCHAR(1000),  
		BrandDetailLocaleId  INT,  
		MediaPath            NVARCHAR(MAX),  
		MediaId              INT,  
		ImageName      VARCHAr(300) ,  
		CMSSEODetailId       INT,  
		SEOTitle             NVARCHAR(MAX),  
		SEOKeywords          NVARCHAR(MAX),  
		SEOURL               NVARCHAR(MAX),  
		ModifiedDate         DATETIME,  
		SEODescription       NVARCHAR(MAX),  
		MetaInformation      NVARCHAR(MAX),  
		IsRedirect           BIT,  
		CMSSEODetailLocaleId INT,  
		--SEOId                INT,  
		BrandName            NVARCHAR(MAX),  
		RowId                INT,  
		CountId              INT ,
		SEOCode              NVARCHAR(4000), 
		Custom1              NVARCHAR(MAX),
		Custom2              NVARCHAR(MAX),
		Custom3              NVARCHAR(MAX),
		Custom4              NVARCHAR(MAX),
		Custom5              NVARCHAR(MAX),
		PortalId			 INT
    );  
             
	IF @PromotionId > 0  
    BEGIN         
		 SET @SeoId = ISNULL(SUBSTRING((SELECT ','+CAST(BrandId AS VARCHAR(50))  
		 FROM ZnodePromotionBrand   
		 WHERE PromotionId= @PromotionId  FOR XML PATH ('') ),2,4000),'0')  
  
		 SET @WhereClause = CASE WHEN @IsAssociated = 1 THEN ' BrandId IN (' ELSE ' BrandId NOT IN (' END  +@SeoId+') AND '+CASE WHEN @WhereClause = '' THEN '1=1' ELSE @WhereClause END   
		 SET @SeoId = ''  
    END    
  
    ;WITH Cte_GetBrandBothLocale AS 
	(
		SELECT ZBDL.Description,ZBD.BrandId,LocaleId,ZBD.BrandCode,isnull(ZPB.DisplayOrder,999) as DisplayOrder,ZBD.IsActive,ZBD.WebsiteLink,ZBDl.BrandDetailLocaleId,  
			SEOFriendlyPageName,[dbo].[Fn_GetMediaThumbnailMediaPath](Zm.path) MediaPath,ZBD.MediaId,Zm.path ImageName, ZBDL.BrandName, ZBD.Custom1, ZBD.Custom2, ZBD.Custom3, ZBD.Custom4, ZBD.Custom5, ZPB.PortalId,
			CASE WHEN ZPB.PortalBrandId IS NULL THEN 0 ELSE 1 END IsAssociated
		FROM ZnodeBrandDetails ZBD 
		LEFT JOIN ZnodePortalBrand ZPB ON ZBD.BrandId = ZPB.BrandId AND (ZPB.PortalId = @PortalId OR isnull(@PortalId,0) = 0 )
		LEFT JOIN ZnodeBrandDetailLocale ZBDL ON(ZBD.BrandId = ZBDL.BrandId)  
		LEFT JOIN ZnodeMedia ZM ON(ZM.MediaId = ZBD.MediaId)  
		WHERE LocaleId IN(@LocaleId, @DefaultLocaleId)  
		
              
    ),  
    Cte_BrandFirstLocale AS 
	(
		SELECT Description,BrandId,LocaleId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName , BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId , IsAssociated
        FROM Cte_GetBrandBothLocale CTGBBL  
        WHERE LocaleId = @LocaleId
	),  
    Cte_BrandDefaultLocale AS 
	(
		SELECT Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName, BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated  
        FROM Cte_BrandFirstLocale  
        UNION ALL  
        SELECT Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,SEOFriendlyPageName,MediaPath,MediaId,ImageName , BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated
		FROM Cte_GetBrandBothLocale CTBBL  
		WHERE LocaleId = @DefaultLocaleId  
		AND NOT EXISTS  
		(  
			SELECT TOP 1 1  
			FROM Cte_BrandFirstLocale CTBFL  
			WHERE CTBBL.BrandId = CTBFL.BrandId  
		)
	)    
    INSERT INTO @TBL_BrandDetails (Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,MediaPath,MediaId,ImageName, BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated)  
    SELECT Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,MediaPath,MediaId,ImageName , BrandName, Custom1, Custom2, Custom3, Custom4, Custom5, PortalId, IsAssociated
    FROM Cte_BrandDefaultLocale CTEBD;  
       
	-----Update BrandName from attributedefault value
	;WITH Cte_GetBrandNameLocale AS 
	(
		select d.brandcode, a.AttributeDefaultValueCode, b.AttributeDefaultValue, b.LocaleId 
		from ZnodePimAttributeDefaultValue a
		inner join ZnodePimAttributeDefaultValueLocale b on a.PimAttributeDefaultValueId = b.PimAttributeDefaultValueId 
		inner join ZnodePimAttribute c on a.PimAttributeId = c.PimAttributeId
		inner join @TBL_BrandDetails d on a.AttributeDefaultValueCode = d.brandcode
		where c.attributecode = 'brand' and b.LocaleId IN(@LocaleId, @DefaultLocaleId)
              
    )
	,Cte_BrandNameFirstLocale AS 
	(
		SELECT brandcode, AttributeDefaultValueCode, AttributeDefaultValue, LocaleId  
        FROM Cte_GetBrandNameLocale CTGBBL  
        WHERE LocaleId = @LocaleId
	)
	,Cte_BrandDefaultLocale AS 
	(
		SELECT brandcode, AttributeDefaultValueCode, AttributeDefaultValue, LocaleId  
        FROM Cte_BrandNameFirstLocale  
        UNION ALL  
        SELECT brandcode, AttributeDefaultValueCode, AttributeDefaultValue, LocaleId  
		FROM Cte_GetBrandNameLocale CTBBL  
		WHERE LocaleId = @DefaultLocaleId  
		AND NOT EXISTS  
		(  
			SELECT TOP 1 1  
			FROM Cte_BrandNameFirstLocale CTBFL  
			WHERE CTBBL.brandcode = CTBFL.brandcode  
		)
	)  
	update b1 set b1.brandname = a1.AttributeDefaultValue
	from Cte_BrandDefaultLocale a1
	inner join @TBL_BrandDetails b1 on a1.brandcode = b1.brandcode
	
	DECLARE @SeoCode SelectColumnList
	INSERT INTO @SeoCode
	SELECT BrandCode FROM @TBL_BrandDetails
				
    INSERT INTO @TBL_SeoDetails 
	(
		CMSSEODetailId,SEOTitle,SEOKeywords,SEOURL,ModifiedDate,SEODescription,MetaInformation,IsRedirect,
		CMSSEODetailLocaleId,PublishStatus,SEOCode,CanonicalURL,RobotTag
	)  
    EXEC Znode_GetSeoDetails @SeoCode, 'Brand', @LocaleId;  
			              
    SELECT DISTINCT TBBD.*,TBSD.SEOTitle,TBSD.SEOKeywords,TBSD.SEOURL,TBSD.SEODescription,TBSD.MetaInformation,TBSD.IsRedirect,
		TBSD.PublishStatus,TBSD.SEOCode,TBSD.CanonicalURL,TBSD.RobotTag 
    INTO #TM_BrandLocale  
    FROM @TBL_BrandDetails TBBD  
    LEFT JOIN @TBL_SeoDetails TBSD ON(TBSD.SEOCode = TBBD.BrandCode)  
    --INNER JOIN @TBL_AttributeDefault TBAD ON(TBAD.AttributeDefaultValueCode = TBBD.BrandCode);  
  
	SET @SQL = 
	'  
     SELECT * ,Count(*)Over() CountId  
     into #Cte_BrandDetails
	 FROM #TM_BrandLocale TMADV  
     WHERE IsAssociated = '+cast(@IsAssociated as varchar(10))+'  
     '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'  
     
    SELECT '+[dbo].[Fn_GetPagingRowId](@Order_BY, 'BrandId DESC')+',Description  , BrandId , BrandCode , DisplayOrder  ,IsActive  ,WebsiteLink ,BrandDetailLocaleId   
         , MediaPath ,MediaId,ImageName  ,SEOTitle ,SEOKeywords , SEOURL   
          ,  SEODescription   ,MetaInformation   ,IsRedirect   
         ,BrandName  ,CountId ,SEOCode,  Custom1, Custom2, Custom3, Custom4, Custom5, PortalId  
    into #BrandDetails 
	FROM #Cte_BrandDetails  
    '+[dbo].[Fn_GetOrderByClause](@Order_BY, 'BrandId DESC')+' 
	
	select Description  , BrandId , BrandCode , DisplayOrder  ,IsActive  ,WebsiteLink ,BrandDetailLocaleId   
         , MediaPath ,MediaId,ImageName  ,SEOTitle ,SEOKeywords , SEOURL   
          ,  SEODescription   ,MetaInformation   ,IsRedirect   
         ,BrandName ,RowId  ,CountId ,SEOCode,  Custom1, Custom2, Custom3, Custom4, Custom5, PortalId 
	from #BrandDetails'+@PaginationWhereClause+' '+dbo.Fn_GetOrderByClause(@Order_By, 'BrandId DESC');  
  
	print @SQL
     INSERT INTO @TBL_BrandDetail  
     (  
		Description,BrandId,BrandCode,DisplayOrder,IsActive,WebsiteLink,  
		BrandDetailLocaleId,MediaPath,MediaId,ImageName,SEOTitle,  
		SEOKeywords,SEOURL,SEODescription,MetaInformation,IsRedirect,  
		BrandName,RowId,CountId ,SEOCode , Custom1, Custom2, Custom3, Custom4, Custom5, PortalId   
     )  
     EXEC (@SQL);  
             
	SET @RowsCount = ISNULL(  
                        (  
                            SELECT TOP 1 CountId  
                            FROM @TBL_BrandDetail  
                        ), 0);  
    SELECT BrandId,Description,BrandCode,DisplayOrder,IsActive,WebsiteLink,BrandDetailLocaleId,MediaPath,MediaId,ImageName,SEOTitle,SEOKeywords,SEOURL SEOFriendlyPageName,SEODescription,MetaInformation,IsRedirect,BrandName,@PromotionId PromotionId   
    ,SEOCode,Custom1, Custom2, Custom3, Custom4, Custom5, PortalId
	FROM @TBL_BrandDetail;  
	
END TRY  
BEGIN CATCH  
	DECLARE @Status BIT ;  
	SET @Status = 0;  
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),   
	@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalBrandDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))
	+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@PromotionId='+CAST(@PromotionId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
		EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName = 'Znode_GetBrandDetailsLocale',  
	@ErrorInProcedure = @Error_procedure,  
	@ErrorMessage = @ErrorMessage,  
	@ErrorLine = @ErrorLine,  
	@ErrorCall = @ErrorCall;  
END CATCH;  
END;