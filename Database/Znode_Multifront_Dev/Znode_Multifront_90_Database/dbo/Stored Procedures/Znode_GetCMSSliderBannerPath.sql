CREATE  PROCEDURE [dbo].[Znode_GetCMSSliderBannerPath]
( @WhereClause NVARCHAR(max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT)
AS
/*
 Summary: Get CMS Slider Banner Information with Banner Path
 Unit Testing:
 EXEC Znode_GetCMSSliderBannerPath 'CMSSliderId=1' ,@RowsCount = 0
*/
 BEGIN
   BEGIN TRY
    SET NOCOUNT ON;
      DECLARE @SQL NVARCHAR(MAX);
	  DECLARE @TBL_SlidderBanner TABLE (CMSSliderId INT,SliderName NVARCHAR(200),CMSSliderBannerId INT,MediaPath VARCHAR(300),Title NVARCHAr(1000),ImageAlternateText NVARCHAR(1000)
									,ButtonLabelName NVARCHAR(1200),ButtonLink NVARCHAR(600),TextAlignment NVARCHAR(200) ,BannerSequence INT,MediaId INT , [FileName] NVARCHAR(1000)
									,[Description] NVARCHAr(max),ActivationDate DATETIME,ExpirationDate DATETIME,RowId INT,CountNo INT )
									
      SET @SQL = '   
      ;with Cte_SliderBanner AS 
	  (
	  SELECT  ZCS.CMSSliderId,ZCS.Name SliderName,ZCSB.CMSSliderBannerId,dbo.FN_GetMediaThumbnailMediaPath(ZM.Path) MediaPath,Title,ZCSBL.ImageAlternateText,ZCSBL.ButtonLabelName,ZCSBL.ButtonLink,ZCSB.TextAlignment,ZCSB.BannerSequence,ZCSBL.Description,ZCSB.ActivationDate
	  ,ZCSB.ExpirationDate , ZCSBL.LocaleId ,ZCSBL.MediaId ,ZM.FileName

	  FROM ZnodeCMSSlider ZCS
	  INNER join [dbo].[ZnodeCMSSliderBanner] ZCSB   ON (ZCSB.CMSSliderId = ZCS.CMSSliderId)
	  INNER JOIN ZnodeCMSSliderBannerLocale ZCSBL ON (ZCSBL.CMSSliderBannerId = ZCSB.CMSSliderBannerId )
	  left join ZnodeMedia ZM ON (ZM.mediaId = ZCSBL.MediaId)
	
	  )
     ,Cte_SlideBannerDetails AS
     (
      SELECT CMSSliderId,SliderName,CMSSliderBannerId,MediaPath,Title,ImageAlternateText,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence ,FileName ,MediaId 
				,Description,ActivationDate,ExpirationDate,'+dbo.Fn_GetPagingRowId(@Order_BY,'CMSSliderId')+' ,Count(*)Over() CountNo
	  FROM Cte_SliderBanner  '+'WHERE 1=1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
     )
	 SELECT CMSSliderId,SliderName,CMSSliderBannerId,MediaPath,Title,ImageAlternateText,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,FileName ,MediaId 
				,Description,ActivationDate,ExpirationDate,RowId,CountNo 
	 FROM Cte_SlideBannerDetails 
	'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
      
	 INSERT INTO @TBL_SlidderBanner (CMSSliderId,SliderName,CMSSliderBannerId,MediaPath,Title,ImageAlternateText,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,[FileName] ,MediaId 
				,[Description],ActivationDate,ExpirationDate,RowId,CountNo)
	 EXEC (@SQL)      

	 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_SlidderBanner),0)

	 SELECT CMSSliderId,SliderName,CMSSliderBannerId,MediaPath,Title,ImageAlternateText,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,[FileName] ,MediaId 
				,[Description],ActivationDate,ExpirationDate
	 FROM @TBL_SlidderBanner

     END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSSliderBannerPath @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSSliderBannerPath',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;