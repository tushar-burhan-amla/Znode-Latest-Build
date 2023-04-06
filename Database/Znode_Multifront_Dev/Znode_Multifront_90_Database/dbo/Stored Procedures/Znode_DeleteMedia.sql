CREATE  PROCEDURE [dbo].[Znode_DeleteMedia]
( @MediaId VARCHAR(2000)= '',
  @Status  BIT           = 1 OUT,
  @MediaIds TransferId READONLY ,
  @IsCallInternal BIT = 0 )
AS
/*
   
     Summary : Remove media with their reference data 
			   This Procedure is used to check the media is associated to the product or not 
			   If passed @MediaIds are matched with deleted count then data set return true other wise false 
			   dbo.Split function use to make comma separeted data in table rows 
			   1 ZnodeMediaAttributeValue
			   2 ZnodeMediaCategory
			   3 ZnodeMedia
     Unit Testing 
	 begin tran
     Declare @Status bit 
     EXEC Znode_DeleteMedia 32,@Status= @Status
	 rollback tran
  
*/
     BEGIN
         
         BEGIN TRY 

             ----- Declare the variable table for store the comma separeted value in row
          
             --- this is for media path with server 
             DECLARE @TBL_MediaIds TABLE(MediaId INT);
             DECLARE @TBL_DeletedMediaId TABLE
             (
			  MediaId   INT,
              MediaPath VARCHAR(3000),
			  MediaConfigurationId INT
             );
             DECLARE @TBL_DeletedMediaId_forOther TABLE
             (MediaId   INT,
              MediaPath VARCHAR(3000),
			   MediaConfigurationId INT
             );
			 -- dbo.Split function use to make comma separeted data in table rows
             INSERT INTO @TBL_MediaIds(MediaId)
                    SELECT Item
                    FROM dbo.Split(@MediaId, ','  
                    ) AS a
					WHERE @MediaId <> '';
			 
			 INSERT INTO @TBL_MediaIds
			 SELECT	id 
			 FROM @MediaIds 

             INSERT INTO @TBL_DeletedMediaId_forOther
                    SELECT zm.MediaId,
                           zm.[Path],
						   zm.MediaConfigurationId
                    FROM ZnodeMedia AS zm
                         INNER JOIN @TBL_MediaIds AS tm ON(tm.MediaId = zm.MediaId);

             /*------------------------------------------------------This code comment after requirment change ---------------------------------------------------------*/

             /*		   WHERE NOT EXISTS (SELECT TOP 1 1  																											   */

             /*         FROM  ZnodePimAttribute a 																													   */

             /*         INNEr JOIN ZnodeAttributeType b ON (a.AttributeTypeId   = b.AttributeTypeId )																   */

             /*         INNER JOIN ZnodePimAttributeValue c ON (c.PimAttributeId = a.PimAttributeId) 																   */

             /*         INNER JOIN ZnodePimAttributeValueLocale d on( d.PimAttributeValueId= c.PimAttributeValueId )												   */

             /*         WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(d.AttributeValue , ',')  qw WHERE qw.Item = CAST(zm.MediaId AS VARCHAr(1000))	)				   */

             /*         AND b.AttributeTypeName IN ('Image','File','Audio','Video'))	 																			   */

             /*         AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductImage zpi  WHERE zpi.MediaId = zm.MediaId)												   */

             /*         AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSPortalTheme zct WHERE zct.MediaId = zm.MediaId)												   */

             /*         -- when its complicated to handle all where conditions within single query 																	   */

             -------------------------------------------------------------------------------------------------------------------------------------------------------------
             INSERT INTO @TBL_DeletedMediaId
                    SELECT MediaId,
                           MediaPath,
						    MediaConfigurationId 
                    FROM @TBL_DeletedMediaId_forOther AS zm;

             /*------------------------------------------------------This code commented after requirment change ---------------------------------------------------------*/

             /*        WHERE NOT EXISTS (SELECT TOP 1 1																												*/

             /*        FROM  ZnodePimAttribute a 																														*/

             /*        INNEr JOIN ZnodeAttributeType b ON (a.AttributeTypeId   = b.AttributeTypeId )																	*/

             /*        INNER JOIN ZnodePimCategoryAttributeValue c ON (c.PimAttributeId = a.PimAttributeId) 															*/

             /*        INNER JOIN ZnodePimCategoryAttributeValueLocale d on( d.PimCategoryAttributeValueId= c.PimCategoryAttributeValueId )							*/

             /*        WHERE EXISTS (SELECT TOP 1 1 FROM dbo.split(d.CategoryValue , ',')  qw WHERE qw.Item = CAST(zm.MediaId AS VARCHAr(1000))	)					*/

             /*        AND b.AttributeTypeName IN ('Image','File','Audio','Video'))																					*/

             /*        AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimGlobalDisplaySetting zpg WHERE zpg.MediaId = zm.MediaId)											*/

             /*        AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSWidgetTitleConfiguration zcwtc WHERE zcwtc.mediaId = zm.MediaId)									*/

             /*        AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSliderBanner zcb WHERE zcb.MediaId = zm.MediaId)													*/

             /*        --- Above conditions check the media are associated with product or portal or theme if associated the n not deleted other wise deleted			*/

             ------------------------------------------------------------------------------------------------------------------------------------------------------------
			-- If multiple MediaIds are passed in csv than it will it insert records matching with ZnodePimProductAttributeMedia
			-- into #temp_delete and than delete records else print errror message.

			SELECT ZM.MediaId ,'Media is associated with PimProductAttributeMedia' AS [MessageDetails]
			into #temp_delete
			FROM  ZnodePimProductAttributeMedia ZM WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_DeletedMediaId AS td
                     WHERE td.mediaId = ZM.MediaId
                 )				
				delete from @TBL_DeletedMediaId where mediaId in (select mediaid from #temp_delete)

		BEGIN TRAN DeleteMedia;
             DELETE FROM ZnodeMediaAttributeValue
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeMediaCategory AS zmc
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_DeletedMediaId AS td
                     WHERE td.mediaId = zmc.MediaId
                 )
                       AND zmc.MediaCategoryId = ZnodeMediaAttributeValue.MediaCategoryId
             );
             DELETE FROM ZnodeMediaCategory
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeMediaCategory.MediaId
             );
			 DELETE FROM ZnodeBlogNewsContent WHERE BlogNewsId IN (SELECT BlogNewsId FROM ZnodeBlogNews
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeBlogNews.MediaId
             ))
			 DELETE FROM ZnodeBlogNewsCommentLocale WHERE BlogNewsCommentId IN (SELECT BlogNewsCommentId 
			 FROM ZnodeBlogNewsComment WHERE BlogNewsId IN (SELECT BlogNewsId FROM ZnodeBlogNews
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeBlogNews.MediaId
             )))
			 DELETE FROM ZnodeBlogNewsComment WHERE BlogNewsId IN (SELECT BlogNewsId FROM ZnodeBlogNews
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeBlogNews.MediaId
             ))
			 DELETE FROM ZnodeBlogNewsLocale WHERE BlogNewsId IN (SELECT BlogNewsId FROM ZnodeBlogNews
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeBlogNews.MediaId
             ))
			  
			 DELETE FROM ZnodeBlogNews
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeBlogNews.MediaId
             );
             DELETE FROM ZnodeMedia
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMediaId AS td
                 WHERE td.mediaId = ZnodeMedia.MediaId
             );

             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_MediaIds
             ) =
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeletedMediaId -- here check if count of both table are same then its return true other wise false 
             )
                 BEGIN
				     IF @IsCallInternal =1 
					 BEGIN 
					  SELECT 0 id  , '' MessageDetail, 1 Status 
					 END 
                     SELECT MediaId Id ,
                            MediaPath MessageDetails  ,
						   CAST(1 AS BIT) AS [Status]
                     FROM @TBL_DeletedMediaId TBLD
				    SET @Status = 1;
                 END
             ELSE  
                 BEGIN
				     IF @IsCallInternal =1 
					 BEGIN 
					  SELECT 0 id  , '' MessageDetail, 0 Status 
					 END
                     SELECT MediaId Id ,
                            MediaPath MessageDetails,
                            CAST(0 AS BIT) AS [Status]
                     FROM @TBL_DeletedMediaId;
                     SET @Status = 0;
                 END;

				 
             COMMIT TRAN DeleteMedia;
			 select * from #temp_delete
         END TRY
         BEGIN CATCH
		     SELECT ERROR_MESSAGE()
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		    @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMedia @MediaId = '+@MediaId+',
		    @Status='+CAST(@Status AS VARCHAR(50));
             SELECT 1 AS ID,
                    '' AS [MessageDetails],
                    CAST(0 AS BIT) AS [Status];
             SET @Status = 0;
             ROLLBACK TRAN DeleteMedia;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteMedia',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;

	 

 
  
	 

