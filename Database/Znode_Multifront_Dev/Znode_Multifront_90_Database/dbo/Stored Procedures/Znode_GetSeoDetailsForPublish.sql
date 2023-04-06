CREATE PROCEDURE [dbo].[Znode_GetSeoDetailsForPublish]
(   
	@PortalId INT=0,
	@IsBrand  Bit = 0  ,
	@SeoCode    NVARCHAR(4000) = '',
	@SeoType  NVARCHAR(200) = '',
	@LocaleId int = 0 
)
AS 
   /*
     Summary:- This Procedure is used to get the Seo Detials on the bassis of Portal 
     Unit Testing 
	 Znode_GetSeoDetailsForPublish @PortalId=0, @IsBrand=0, @SeoType = 'Product'
	 EXEC [Znode_GetSeoDetailsForPublish]  @PortalId=14, @IsBrand=0 ,@SeoType = 'category,product,brand',@LocaleId=1
   */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();

		IF OBJECT_ID('tempdb..#CMSTempDetails') IS NOT NULL
		DROP TABLE #CMSTempDetails

		CREATE TABLE #CMSTempDetails(
			[PortalId] [int] NULL,
			[CMSSEODetailId] [int] NOT NULL,
			[SEOTitle] [nvarchar](max) NULL,
			[SEOKeywords] [nvarchar](max) NULL,
			[SEOUrl] [nvarchar](max) NULL,
			[ModifiedDate] [datetime] NOT NULL,
			[SEODescription] [nvarchar](max) NULL,
			[MetaInformation] [nvarchar](max) NULL,
			[IsRedirect] [bit] NULL,
			[CMSSEODetailLocaleId] [int] NOT NULL,
			[LocaleId] [int] NULL,
			[CMSSEOTypeId] [int] NOT NULL,
			[SEOTypeName] [nvarchar](100) NULL,
			[PublishStatus] [varchar](50) NULL,
			[SEOCode] [nvarchar](2000) NULL,
			[CanonicalURL]  [VARCHAR] (200) NULL,
			[RobotTag] [VARCHAR] (50)  NULL
		)
		        
				
				SELECT CMSSEODetailId,CMSSEOTypeId ,SEOUrl,PortalId,SEOCOde, Row_number()Over(PARTITION BY CMSSEOTypeId ,SEOUrl,PortalId ORDER BY CMSSEOTypeId ,SEOUrl,PortalId) RowId
				INTO #temp_Data
				FROM ZnodeCMSSEODetail ZCOD 
				WHERE PortalId = @PortalId
				AND ( CMSSEOTypeId IN  (SELECT  CMSSEOTypeId FROM  ZnodeCMSSEOType WHERE Name IN  ( SELECT item FROM dbo.split(@SeoType,','))) OR @SeoType = '') 
				
			
				
				IF EXISTS (SELECT TOP 1 1  FROM dbo.split(@SeoType,',') WHERE item = 'category')
				BEGIN 
				  DELETE #temp_Data WHERE CMSSEOTypeId <> (SELECT TOP 1 CMSSEOTypeId FROM  ZnodeCMSSEOType WHERE Name = 'category')

				  DELETE FROM #temp_Data WHERE SEOCode IN (SELECT TR.CategoryValue FROM View_PimCategoryAttributeValue TR WHERE TR.AttributeCode = 'CategoryCode' 
				    AND EXISTS (SELECT TOP 1  1  FROM ZnodePimCategoryHierarchy TY WHERE TY.PimCategoryId = TR.PimCategoryId 
					AND EXISTS (SELECT TOP 1 1 FROM ZnodePortalCatalog TYU WHERE TYU.PortalId = @PortalId AND 
					EXISTS (SELECT TOP 1  1  FROM ZnodePublishCatalog UI WHERE UI.PublishCatalogId = TYU.PublishCatalogId AND UI.PimCatalogId = TY.PimCatalogId)
					 )					)				   )

				DELETE RT FROM ZnodeCMSSEODetailLocale RT WHERE EXISTS (SELECT TOP 1 1 FROM #temp_Data  ty WHERE ty.CMSSEODetailId = RT.CMSSEODetailId    )
				DELETE RT FROM ZnodeCMSSEODetail RT WHERE EXISTS (SELECT TOP 1 1 FROM #temp_Data  ty WHERE ty.CMSSEODetailId = RT.CMSSEODetailId    )

				END 

				 

				


				INSERT INTO #CMSTempDetails
				SELECT ZCSD.PortalId, ZCSD.CMSSEODetailId, ZCSDL.SEOTitle, ZCSDL.SEOKeywords, lower(ZCSD.SEOUrl) as SEOUrl,ZCSD.ModifiedDate,ZCSDL.SEODescription, ZCSD.MetaInformation,ZCSD.IsRedirect,
					   ZCSDL.CMSSEODetailLocaleId, ZCSDL.LocaleId, ZCSD.CMSSEOTypeId, ZCST.Name AS SEOTypeName, TYU.DisplayName   AS PublishStatus,ZCSD.SEOCode,ZCSDL.CanonicalURL, ZCSDL.RobotTag 
				FROM ZnodeCMSSEODetail AS ZCSD
				LEFT JOIN ZnodePublishState  TYU ON (TYU.PublishStateId = ZCSD.PublishStateId) 
				INNER JOIN ZnodeCMSSEOType AS ZCST ON ( ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId )                                                 
				INNER JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON ( ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId )
				WHERE (ZCSD.PortalId= @PortalId OR @PortalId = 0 ) AND ZCST.Name <> 'Brand'
				AND EXISTS (SELECT TOP 1 1 FROM dbo.Split(@SeoCode,',') SP WHERE SP.Item = ZCSD.SEOCode  OR @SeoCode = '')
				AND EXISTS (SELECT TOP 1 1 FROM dbo.Split(@SeoType,',') SP WHERE SP.Item = ZCST.Name  OR @SeoType = '')
				AND ZCSDL.LocaleId IN (@LocaleId,@DefaultLocaleId)
				UNION ALL
				SELECT ZCSD.PortalId, ZCSD.CMSSEODetailId, ZCSDL.SEOTitle, ZCSDL.SEOKeywords, lower(ZCSD.SEOUrl) as SEOUrl,ZCSD.ModifiedDate,ZCSDL.SEODescription, ZCSD.MetaInformation,ZCSD.IsRedirect,
					   ZCSDL.CMSSEODetailLocaleId, ZCSDL.LocaleId, ZCSD.CMSSEOTypeId, ZCST.Name AS SEOTypeName, TYU.DisplayName AS PublishStatus,ZCSD.SEOCode , ZCSDL.CanonicalURL, ZCSDL.RobotTag
				FROM ZnodeCMSSEODetail AS ZCSD 
				LEFT JOIN ZnodePublishState  TYU ON (TYU.PublishStateId = ZCSD.PublishStateId)
				INNER JOIN ZnodeCMSSEOType AS ZCST ON ( ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId )                                                  
				INNER JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON ( ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId )
				WHERE @IsBrand = 1 AND ZCST.Name = 'Brand' --AND (ZCSD.PortalId= @PortalId OR @PortalId = 0 ) 
				AND EXISTS (SELECT TOP 1 1 FROM dbo.Split(@SeoCode,',') SP WHERE SP.Item = ZCSD.SEOCode  OR @SeoCode = '' )
				AND EXISTS (SELECT TOP 1 1 FROM dbo.Split(@SeoType,',') SP WHERE SP.Item = ZCST.Name  OR @SeoType = '')
				AND ZCSDL.LocaleId IN (@LocaleId,@DefaultLocaleId)
				

				
				

						
			SELECT PortalId, CMSSEODetailId, SEOTitle, SEOKeywords, SEOUrl, SEODescription, MetaInformation, IsRedirect, CMSSEODetailLocaleId, LocaleId, CMSSEOTypeId, SEOTypeName, PublishStatus,SEOCode, CanonicalURL, RobotTag    
			FROM #CMSTempDetails
			where LocaleId = @LocaleId  
			UNION ALL
			SELECT PortalId, CMSSEODetailId, SEOTitle, SEOKeywords, SEOUrl, SEODescription, MetaInformation, IsRedirect, CMSSEODetailLocaleId, @LocaleId LocaleId, CMSSEOTypeId, SEOTypeName, PublishStatus,SEOCode  , CanonicalURL, RobotTag  
			FROM #CMSTempDetails CSD
			WHERE 
			NOT EXISTS( SELECT * FROM #CMSTempDetails CSD1 
			            --INNER JOIN ZnodeLocale ZL1 ON CSD1.LocaleId = ZL1.LocaleId AND ZL1.IsActive = 1 AND ZL1.IsDefault = 0
						WHERE CSD.CMSSEOTypeId = CSD1.CMSSEOTypeId AND CSD.SEOCode = CSD1.SEOCode AND  CSD1.LocaleId = @LocaleId)
			AND LocaleId = @DefaultLocaleId

			
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		    SET @Status = 0;
			SELECT ERROR_MESSAGE(); 

		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSeoDetailsForPublish @PortalId = '+CAST(@PortalId AS VARCHAR(10))+',@IsBrand ='+CAST(@IsBrand AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSeoDetailsForPublish',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;