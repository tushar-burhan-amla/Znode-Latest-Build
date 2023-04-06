CREATE PROCEDURE [dbo].[Znode_GetCMSContentPagesFolderDetails]
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
				    , PageTemplateName NVARCHAR(200),SEOUrl NVARCHAR(max),MetaInformation NVARCHAR(max),SEODescription NVARCHAR(max),SEOTitle NVARCHAR(max),SEOKeywords NVARCHAR(max),CMSContentPageGroupName NVARCHAR(200),RowId INT ,CountNo INT,PublishStatus nvarchar(300)  ,SEOPublishStatus  nvarchar(300), SEOCode NVARCHAR(4000),CanonicalURL VARCHAR(200), RobotTag VARCHAR(50)
					,MediaPath varchar(1000) )
					SET @SQL = '  
						;With CMSContentPages AS (		
						SELECT DISTINCT ZCCP.CMSContentPagesId,ZCCP.PortalId,ZCCP.CMSTemplateId,ZCCPL.PageTitle,ZCCP.PageName,ZCCP.ActivationDate, ZCCP.ExpirationDate,ZCCP.IsActive
						,ZCCP.CreatedBy,ZCCP.CreatedDate,ZCCP.ModifiedBy,ZCCP.ModifiedDate,e.StoreName PortalName   ,ISNULL(ZCCPG.CMSContentPageGroupId,0) CMSContentPageGroupId
						,zct.Name PageTemplateName ,zcsd.SEOUrl,zcsd.MetaInformation,ZCCPGL.Name CMSContentPageGroupName,ZCCPL.LocaleId,ZCSDL.SEODescription,ZCSDL.SEOTitle,ZCSDL.SEOKeywords	,ZCSDL.LocaleId LocaleSeo,ZCCPGL.LocaleId LocaeIdRTR ,tyu.DisplayName  IsPublished
						,zcsd.PublishStateId IsSEOPublished, ISNULL(ZCSD.SEOCode,ZCCP.PageName) as SEOCode, ZCSDL.CanonicalURL, ZCSDL.RobotTag 
					    , [dbo].[Fn_GetThumbnailMediaPath](zct.MediaId,0) as MediaPath
						FROM ZnodeCMSContentPages ZCCP 
						LEFT JOIN  ZnodePublishState tyu ON (tyu.PublishStateId = ZCCP.PublishStateId )
						LEFt Outer JOIN [ZnodeCMSContentPageGroupMapping] ZCCPGM ON (ZCCPGM.CMSContentPagesId = ZCCP.CMSContentPagesId) 
					    LEFt Outer JOIN [ZnodeCMSContentPageGroup] ZCCPG ON (ZCCPG.CMSContentPageGroupId = ZCCPGM.CMSContentPageGroupId)
						LEFt Outer JOIN [ZnodeCMSContentPagesLocale] ZCCPL ON (ZCCP.CMSContentPagesId = ZCCPL.CMSContentPagesId  )
						LEFt Outer JOIN [ZnodeCMSContentPageGroupLocale] ZCCPGL ON (ZCCPGL.CMSContentPageGroupId = ZCCPG.CMSContentPageGroupId AND ZCCPGL.LocaleId = ZCCPL.LocaleId  )					
						LEFT JOIN ZnodeCMSTemplate zct ON (zct.CMSTemplateId = ZCCP.CMSTemplateId )
						LEFT JOIN ZnodeCMSSEODetail zcsd ON (zcsd.SEOCode = ZCCP.PageName AND ZCSD.Portalid = ZCCP.portalId AND zcsd.PortalId IS NOT NULL  AND 
					    EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType zcst WHERE zcst.CMSSEOTypeId = zcsd.CMSSEOTypeId AND zcst.Name = ''Content Page'' ))
					    LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON ZCSDL.CMSSEODetailId = zcsd.CMSSEODetailId    AND zcsdl.LocaleId =  '+CAST(@LocaleId AS VARCHAR(10))+'
						LEFT JOIN  ZnodePublishState tyus ON (tyus.PublishStateId = zcsd.PublishStateId )
						LEFt Outer JOIN ZnodePortal e on ZCCP.PortalId = e.PortalId 
					    WHERE  ZCCPL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
						 ) 
						, Cte_ContaintPageDetails AS (
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEODescription,SEOTitle,SEOKeywords,MetaInformation,IsPublished, IsSEOPublished,SEOCode,CanonicalURL, RobotTag 
									,MediaPath
						 FROM CMSContentPages WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
						)
						, Cte_ContentPage  AS (     	 
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
						, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation,IsPublished,IsSEOPublished,SEOCode,CanonicalURL, RobotTag 	
						,MediaPath
						FROM Cte_ContaintPageDetails 
						UNION ALL 
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
						,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName ,CMSContentPageGroupId , PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation,IsPublished,IsSEOPublished,SEOCode,CanonicalURL, RobotTag 
					    ,MediaPath
						FROM CMSContentPages CCP WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
					    AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_ContaintPageDetails CTCPD WHERE CTCPD.CMSContentPagesId  = CCP.CMSContentPagesId AND  CTCPD.Portalid = CCp.PortalId) -- AND  LocaleSeo = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'

						)				

					    ,Cte_ContenetPageFilter AS (
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,a.IsActive,SEOKeywords,SEOTitle,SEODescription,MetaInformation,a.CreatedBy,a.CreatedDate,a.ModifiedBy,a.ModifiedDate,PortalName   ,CMSContentPageGroupId 
						, PageTemplateName ,SEOUrl,CMSContentPageGroupName, a.IsPublished PublishStatus, 
						CASE WHEN n.DispLayName IS NULL THEN '''' ELSE n.DispLayName END  SEOPublishStatus  , SEOCode,DisplayName,CanonicalURL, RobotTag 
						,MediaPath
						 FROM Cte_ContentPage a 
						 LEFT JOIN ZnodePublishState n ON (n.PublishStateId = a.IsSEOPublished ) ) '
   
						set @SQLWhereClause = @SQL + '
						
						,Cte_ContentFinal AS
						(
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,
						PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,SEOKeywords,SEOTitle,
						SEODescription,MetaInformation,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,
						PortalName,CMSContentPageGroupId, PageTemplateName ,
						SEOUrl,CMSContentPageGroupName,PublishStatus,SEOPublishStatus,'+[dbo].[Fn_GetPagingRowId](@Order_BY,'CMSContentPagesId')+',Count(*)Over() CountNo,SEOCode,CanonicalURL, RobotTag 
						,MediaPath
						FROM Cte_ContenetPageFilter WHERE  1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )

						
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName   ,CMSContentPageGroupId 
						, PageTemplateName ,SEOUrl,CMSContentPageGroupName,RowId,CountNo,SEOKeywords,SEOTitle,SEODescription,MetaInformation,    PublishStatus,SEOPublishStatus, SEOCode,CanonicalURL, RobotTag 
					    ,MediaPath
						FROM Cte_ContentFinal  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
						
						INSERT INTO @TBL_ContenetPageLocale (CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,RowId,CountNo,SEOKeywords,SEOTitle,SEODescription,
									MetaInformation,PublishStatus,SEOPublishStatus,SEOCode,CanonicalURL, RobotTag,MediaPath )
           				
					    EXEC (@SQLWhereClause)      
						                                                     
					    SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ContenetPageLocale) ,0)   
						     
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate,ExpirationDate,IsActive
							   ,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
							   ,PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation,
							   PublishStatus,SEOPublishStatus,SEOCode,CanonicalURL, RobotTag, MediaPath 
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