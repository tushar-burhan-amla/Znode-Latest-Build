CREATE PROCEDURE [dbo].[Znode_DeleteBlogNews]
( 
@BlogNewsId VARCHAR(2000),
@Status           BIT OUT
  )
AS 
  /*  
     Summary : Remove blog/news details with their reference data 
			   Here complete delete the blog(s)/News and their references without any check  
			   If passed @BlogNewsIds are matched with deleted count then data set return true other wise false 
			   dbo.Split function use to make comma separeted data in table rows 
			   1 ZnodeBlogNews
			   2 ZnodeBlogNewsLocale
			   3 ZnodeCMSSEODetail
			   4 ZnodeCMSSEODetailLocale
			   5 ZnodeBlogNewsContent
    
       
    */
	 BEGIN
         BEGIN TRAN DeleteBlogNews;
         BEGIN TRY
             SET NOCOUNT ON;
             
			 DECLARE @TBL_DeleteBlogNews TABLE(BlogNewsId INT,BlogNewsCode NVARCHAR(4000));  -- table holds the BlogNewsId id and BlogNewsCode
             INSERT INTO @TBL_DeleteBlogNews
                    SELECT a.BlogNewsId, a.BlogNewsCode
                    FROM [dbo].[ZnodeBlogNews] AS a
                         INNER JOIN dbo.Split(@BlogNewsId, ',') AS b ON(a.BlogNewsId = b.Item); -- dbo.Split function use to make ',' separeted data in table rows 
           
             DELETE FROM ZnodeCMSSEODetailLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSSEODetail
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_DeleteBlogNews AS TBDCP
                     WHERE TBDCP.BlogNewsCode = ZnodeCMSSEODetail.SEOCode
                 )
                       AND ZnodeCMSSEODetail.CMSSEOTypeId IN
                 (
                     SELECT CMSSEOTypeId
                     FROM ZnodeCMSSEOType
                     WHERE NAME = 'BlogNews'
                 )
                       AND ZnodeCMSSEODetail.CMSSEODetailId = ZnodeCMSSEODetailLocale.CMSSEODetailId
             );
             DELETE FROM ZnodeCMSSEODetail
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteBlogNews AS TBDCP
                 WHERE TBDCP.BlogNewsCode = ZnodeCMSSEODetail.SEOCode
             )
                   AND ZnodeCMSSEODetail.CMSSEOTypeId IN
             (
                 SELECT CMSSEOTypeId
                 FROM ZnodeCMSSEOType
                 WHERE NAME = 'BlogNews'
             );

			 DELETE FROM ZnodeBlogNewsCommentLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeBlogNewsComment
                 WHERE EXISTS
                 (
                     SELECT TOP 1 1
                     FROM @TBL_DeleteBlogNews AS TBDCP
                     WHERE TBDCP.BlogNewsId = ZnodeBlogNewsComment.BlogNewsId
                 )
				   AND ZnodeBlogNewsComment.BlogNewsCommentId = ZnodeBlogNewsCommentLocale.BlogNewsCommentId
             );
          
			DELETE FROM ZnodeBlogNewsComment
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteBlogNews AS TBDCP
                 WHERE TBDCP.BlogNewsId = ZnodeBlogNewsComment.BlogNewsId
             );

			 DELETE FROM ZnodeBlogNewsContent
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteBlogNews AS TBDCP
                 WHERE TBDCP.BlogNewsId = ZnodeBlogNewsContent.BlogNewsId
             );

			 DELETE FROM ZnodeBlogNewsLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteBlogNews AS TBDCP
                 WHERE TBDCP.BlogNewsId = ZnodeBlogNewsLocale.BlogNewsId
             );

			 DELETE FROM ZnodeBlogNews
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteBlogNews AS TBDCP
                 WHERE TBDCP.BlogNewsId = ZnodeBlogNews.BlogNewsId
             );


             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeleteBlogNews
             ) =
             (   -- if count are equal then  dataset status are return true other wise false 
                 SELECT COUNT(1)
                 FROM dbo.Split(@BlogNewsId, ',')
             ) 
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS [Status];
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteBlogNews;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteBlogNews @BlogNewsId = '+@BlogNewsId+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
             ROLLBACK TRAN DeleteBlogNews;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteBlogNews',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;