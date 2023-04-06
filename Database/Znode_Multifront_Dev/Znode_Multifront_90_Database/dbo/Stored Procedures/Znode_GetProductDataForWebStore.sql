

CREATE PROCEDURE [dbo].[Znode_GetProductDataForWebStore]  
(   @SKU              [dbo].[SelectColumnList] READONLY,  
    @PublishCatalogId int = 0 ,  
    @PublishProductId VARCHAR(MAX),  
    @PortalId         INT,  
    @LocaleId         INT)  
AS   
  /*    
    Summary: WebStore: SP for getting products data   
       Get average rating of products   
       Get Price / Inventory / SEO details .  
    Unit Testing  
 begin tran     
    EXEC [Znode_GetProductDataForWebStore] 'SKBCA1112,SKWI122,SKFVR123,FVZK0237,SKPMAR123,SKLS232',0,'96,92,98,103,94,97',@PortalId=2 ,@LocaleId=1  
    EXEC [Znode_GetProductDataForWebStore] 'SKPMAR123',3,'',@PortalId=1 ,@LocaleId=0  
 rollback tran  
   */  
     BEGIN  
         BEGIN TRAN A;  
         BEGIN TRY  
             SET NOCOUNT ON;  
  
             DECLARE @Tlb_SKU TABLE  
             (SKU        VARCHAR(100),  
              SequenceNo INT IDENTITY  
             );  
	If EXISTS(select * from @SKU)
		INSERT INTO @Tlb_SKU(SKU) SELECT StringColumn FROM @SKU;  
	Else if @PublishCatalogId > 0   
		INSERT INTO @Tlb_SKU(SKU)   
		select Distinct ZPCP.SKU  from ZnodePublishProductEntity  ZPCP
		where  ZPCP.ZnodeCatalogId = @PublishCatalogId  
  
             DECLARE @Tlb_PublishProduct TABLE  
             (PublishProductId INT,  
              SequenceNo       INT IDENTITY,
			  PublishCatalogId INT
             );  
  
    If @PublishProductId <> '' 
	Begin
		 INSERT INTO @Tlb_PublishProduct(PublishProductId, PublishCatalogId)  
		 SELECT Distinct ZPCP.ZnodeProductId, ZPCP.ZnodeCatalogId 
		 FROM ZnodePublishProductEntity ZPCP 
		 INNER JOIN Dbo.split(@PublishProductId, ',') PPI ON ZPCP.ZnodeProductId = PPI.Item;   
	End
    Else if @PublishCatalogId > 0   
	Begin
		INSERT INTO @Tlb_PublishProduct(PublishProductId,PublishCatalogId)  
		select Distinct ZPCP.ZnodeProductId,ZPCP.ZnodeCatalogId  
		from ZnodePublishProductEntity ZPCP 
		where  ZPCP.ZnodeCatalogId = @PublishCatalogId  
    End
	Else if @PublishCatalogId = 0 AND  @PublishProductId = ''   AND    exists(select * from @SKU)
	Begin
		INSERT INTO @Tlb_PublishProduct(PublishProductId,PublishCatalogId)  
		SELECT Distinct ZPPD.ZnodeProductId,ZPPD.ZnodeCatalogId  
		FROM ZnodePublishProductEntity ZPPD 
		INNER JOIN @Tlb_SKU TSK ON (ZPPD.SKU = TSK.SKU)
		
	End  
  
  
    CREATE TABLE #Tlb_ProductData   
             (PublishProductId INT,  
              SKU              NVARCHAR(100),  
              SEOTitle         NVARCHAR(200),  
              SEODescription   NVARCHAR(MAX),  
              SEOKeywords      NVARCHAR(MAX),  
              SEOUrl           NVARCHAR(MAX),  
              Rating           Numeric(28,6),  
              TotalReviews     INT,
			  IsPublish        bit,
			  CanonicalURL VARCHAR(200),   
			  RobotTag VARCHAR(50),
			  PublishCatalogId INT
             );  
  
             INSERT INTO #Tlb_ProductData (PublishProductId,SKU,PublishCatalogId)  
             SELECT PP.PublishProductId,SK.SKU, PP.PublishCatalogId FROM @Tlb_PublishProduct AS PP INNER JOIN @Tlb_SKU AS SK ON PP.SequenceNo = SK.SequenceNo;  
  
             DECLARE @Tlb_CustomerAverageRatings TABLE  
             (PublishProductId INT,  
              Rating           NUMERIC(28,6),  
              TotalReviews     INT  
             );   
             -- Calculate Average rating   
             INSERT INTO @Tlb_CustomerAverageRatings(PublishProductId,Rating,TotalReviews)  
             SELECT CCR.PublishProductId,SUM(CAST(CCR.Rating AS NUMERIC(28,6)) )/ COUNT(CCR.PublishProductId),COUNT(CCR.PublishProductId)   
    FROM ZnodeCMSCustomerReview AS CCR  
             INNER JOIN #Tlb_ProductData AS PD ON CCR.PublishProductId = PD.PublishProductId AND CCR.Status = 'A'   
    AND  (CCR.PortalId  = @PortalId OR @PortalId = 0 )  
    GROUP BY CCR.PublishProductId    ;  
  
             UPDATE PD SET PD.Rating = CAR.Rating,PD.TotalReviews = CAR.TotalReviews   
    FROM @Tlb_CustomerAverageRatings CAR  
             INNER JOIN #Tlb_ProductData PD ON CAR.PublishProductId = PD.PublishProductId;  
  
    UPDATE PD SET PD.SEOTitle = ZCSDL.SEOTitle,PD.SEODescription = ZCSDL.SEODescription,PD.SEOKeywords = ZCSDL.SEOKeywords,PD.SEOUrl = ZCSO.SEOUrl, PD.IsPublish = ZCSO.IsPublish,
	              PD.CanonicalURL = ZCSDL.CanonicalURL, PD.RobotTag = ZCSDL.RobotTag
    FROM #Tlb_ProductData PD  
             INNER JOIN ZnodeCMSSEODetail ZCSO ON PD.SKU = ZCSO.SEOCode  
             LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSO.CMSSEODetailId AND ZCSDL.LocaleId = @LocaleId)  
             INNER JOIN ZnodeCMSSEOType ZCOT ON ZCOT.CMSSEOTypeId = ZCSO.CMSSEOTypeId AND ZCOT.Name = 'Product'  
    WHERE ZCSO.PortalId = @PortalId  
  
    UPDATE PD SET PD.SEOTitle = ZCPS.ProductTitle,PD.SEODescription = ZCPS.ProductDescription,PD.SEOKeywords = ZCPS.ProductKeyword 
	FROM #Tlb_ProductData PD  
    INNER JOIN ZnodeCMSPortalSEOSetting ZCPS ON ZCPS.PortalId = @PortalId 
	WHERE PD.SEOTitle IS NULL AND PD.SEODescription IS NULL AND PD.SEOKeywords IS NULL AND PD.SEOUrl IS NULL  
     --AND ZCSO.PortalId = @PortalId  

    SELECT PD.PublishCatalogId , PD.PublishProductId,PD.SKU,PD.SEOTitle,PD.SEODescription,PD.SEOKeywords,PD.SEOUrl,PD.Rating,PD.TotalReviews, 
		   CASE WHEN ISNULL(PD.IsPublish,0) = 0  THEN 'Draft' ELSE 'Published' END PublishStatus, PD.CanonicalURL, PD.RobotTag    
    FROM #Tlb_ProductData PD 
	--LEFT Outer join ZnodePublishCategoryProduct ZPCP  ON PD.PublishProductId = ZPCP.PublishProductId ;
	               
    COMMIT TRAN A;  
     
         END TRY  
         BEGIN CATCH  
              DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductDataForWebStore @PublishProductId='+@PublishProductId+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'Znode_GetProductDataForWebStore',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;
GO

