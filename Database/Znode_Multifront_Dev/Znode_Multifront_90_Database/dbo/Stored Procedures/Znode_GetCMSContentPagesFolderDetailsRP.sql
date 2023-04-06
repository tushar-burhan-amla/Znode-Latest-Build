
CREATE PROCEDURE [dbo].[Znode_GetCMSContentPagesFolderDetailsRP]
( @WhereClause VARCHAR(1000),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(100)  = NULL,
  @RowsCount   INT OUT,
  @PortalId    INT           = NULL,
  @LocaleId    INT           = 1)
AS  
   /* 
    Summary: To get content page folder details 
             Provide output for paging with dynamic where cluase                  
    		 User view : View_CMSContentPagesFolderDetails
    Unit Testing  
    Exec Znode_GetCMSContentPagesFolderDetails '',@RowsCount = 1 
    
	*/
     BEGIN
        BEGIN TRY
          SET NOCOUNT ON;

		     DECLARE @SQL NVARCHAR(MAX);
             DECLARE @DefaultLocaleId VARCHAR(100)= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_ContenetPageLocale TABLE(CMSContentPagesId INT,PortalId INT,CMSTemplateId INT,PageTitle NVARCHAR(200),PageName NVARCHAR(200),ActivationDate DATETIME, ExpirationDate DATETIME,IsActive BIT
				    ,CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME,PortalName  NVARCHAR(max) ,CMSContentPageGroupId INT 
				    , PageTemplateName NVARCHAR(200),SEOUrl NVARCHAR(max),MetaInformation NVARCHAR(max),SEODescription NVARCHAR(max),SEOTitle NVARCHAR(max),SEOKeywords NVARCHAR(max),CMSContentPageGroupName NVARCHAR(200),RowId INT ,CountNo INT )

             SET @SQL = '  
						;With CMSContentPages AS (
		
						SELECT DISTINCT ZCCP.CMSContentPagesId,ZCCP.PortalId,ZCCP.CMSTemplateId,ZCCPL.PageTitle,ZCCP.PageName,ZCCP.ActivationDate, ZCCP.ExpirationDate,ZCCP.IsActive
						,ZCCP.CreatedBy,ZCCP.CreatedDate,ZCCP.ModifiedBy,ZCCP.ModifiedDate,e.StoreName PortalName   ,ZCCPG.CMSContentPageGroupId 
						,zct.Name PageTemplateName ,zcsd.SEOUrl,zcsd.MetaInformation,ZCCPGL.Name CMSContentPageGroupName,ZCCPL.LocaleId,ZCSDL.SEODescription,ZCSDL.SEOTitle,ZCSDL.SEOKeywords	,ZCSDL.LocaleId LocaleSeo,ZCCPGL.LocaleId LocaeIdRTR 
					    FROM ZnodeCMSContentPages ZCCP 
						LEFt Outer JOIN [ZnodeCMSContentPageGroupMapping] ZCCPGM ON (ZCCPGM.CMSContentPagesId = ZCCP.CMSContentPagesId) 
					    LEFt Outer JOIN [ZnodeCMSContentPageGroup] ZCCPG ON (ZCCPG.CMSContentPageGroupId = ZCCPGM.CMSContentPageGroupId)
						LEFt Outer JOIN [ZnodeCMSContentPageGroupLocale] ZCCPGL ON (ZCCPGL.CMSContentPageGroupId = ZCCPG.CMSContentPageGroupId)
						LEFt Outer JOIN [ZnodeCMSContentPagesLocale] ZCCPL ON (ZCCP.CMSContentPagesId = ZCCPL.CMSContentPagesId  )
						LEFT JOIN ZnodeCMSTemplate zct ON (zct.CMSTemplateId = ZCCP.CMSTemplateId )
						LEFT JOIN ZnodeCMSSEODetail zcsd ON (zcsd.SEOId = ZCCP.CMSContentPagesId AND ZCSD.Portalid = ZCCP.portalId AND 
					    EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType zcst WHERE zcst.CMSSEOTypeId = zcsd.CMSSEOTypeId AND zcst.Name = ''Content Page'' ))
					    LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON (ZCSDL.CMSSEODetailId = zcsd.CMSSEODetailId   ) 
						LEFt Outer JOIN ZnodePortal e on ZCCP.PortalId = e.PortalId 
					    WHERE  ZCCPL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
						AND ZCSDL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
						AND ZCCPGL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+') 
						AND zcsd.PortalId IS NOT NULL 
						 ) 
						, Cte_ContaintPageDetails AS 
						(
						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEODescription,SEOTitle,SEOKeywords,MetaInformation
						FROM CMSContentPages
						WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
                        AND LocaleSeo = '+CAST(@LocaleId AS VARCHAR(50))+'
						AND LocaeIdRTR   = '+CAST(@LocaleId AS VARCHAR(50))+'
						)
						, Cte_ContentPage  AS (     
	 
							SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation
							FROM Cte_ContaintPageDetails 

						UNION ALL 

						SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
								,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName ,CMSContentPageGroupId 
								, PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation
					    FROM CMSContentPages CCP 
						WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
					    AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_ContaintPageDetails CTCPD WHERE CTCPD.CMSContentPagesId  = CCP.CMSContentPagesId AND  CTCPD.Portalid = CCp.PortalId)
					    AND  LocaleSeo = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
					    	AND LocaeIdRTR   = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'
						)

					    ,Cte_ContenetPageFilter AS 
					    (
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,SEOKeywords,SEOTitle,SEODescription,MetaInformation
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName   ,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,'+[dbo].[Fn_GetPagingRowId](@Order_BY,'CMSContentPagesId')+',COUNT(*)OVER() CountNo
					    FROM Cte_ContentPage
					    WHERE  1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' 
					    )
   
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName   ,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,RowId,CountNo,SEOKeywords,SEOTitle,SEODescription,MetaInformation
					    FROM Cte_ContenetPageFilter
					    '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
   
					   PRINT @SQL
					    INSERT INTO @TBL_ContenetPageLocale (CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive
									,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
									, PageTemplateName ,SEOUrl,CMSContentPageGroupName,RowId,CountNo,SEOKeywords,SEOTitle,SEODescription,MetaInformation)                                                                                                                                            
					   
				
					    EXEC (@SQL)                                                           
					    SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ContenetPageLocale) ,0)        
					    SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate,ExpirationDate,IsActive
							   ,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PortalName,CMSContentPageGroupId 
							   ,PageTemplateName ,SEOUrl,CMSContentPageGroupName,SEOKeywords,SEOTitle,SEODescription,MetaInformation
						FROM @TBL_ContenetPageLocale

            
    END TRY
    BEGIN CATCH
        DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSContentPagesFolderDetails @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		 
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSContentPagesFolderDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
    END CATCH;
END;