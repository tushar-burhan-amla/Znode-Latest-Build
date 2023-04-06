
CREATE PROCEDURE [dbo].[Znode_GetMediaFolderDetails_Test]
( @WhereClause VARCHAR(1000),
  @MediaPathId INT,
  @Rows        INT           = 1000,
  @PageNo      INT           = 0,
  @Order_BY    VARCHAR(1000) = '',
  @RowsCount   INT OUT,
  @LocaleId    INT           = 1)
AS
/*
Summary: This Procedure is Used to Get Details of Media Folder
  Unit Testing:
  begin tran
	DECLARE @RowsCount BIGINT  
	EXEC Znode_GetMediaFolderDetails @MediaPathId = -1 , @WhereClause='',@Rows=2147483647,@PageNo=1 ,@Order_By='', @RowsCount = @RowsCount OUT  
  rollback tran

*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @Rows_start VARCHAR(1000), @Rows_end VARCHAR(1000);
             SET @Rows_start = CASE
                                   WHEN @Rows >= 1000000
                                   THEN 0
                                   ELSE(@Rows * (@PageNo - 1)) + 1
                               END;
             SET @Rows_end = CASE
                                 WHEN @Rows >= 1000000
                                 THEN @Rows
                                 ELSE @Rows * (@PageNo)
                             END;
             DECLARE @V_SQL NVARCHAR(MAX);
		

             SET @Order_BY = REPLACE(@Order_BY, 'MediaPathId', 'Convert(numeric,MediaPathId)');
             SET @Order_BY = REPLACE(@Order_BY, 'Size', 'Convert(numeric,Size)');
             SET @Order_BY = REPLACE(@Order_BY, 'MediaId', 'Convert(numeric,MediaId)');
             SET @Order_BY = REPLACE(@Order_BY, 'CreatedBy', 'Convert(numeric,CreatedBy)');
             SET @Order_BY = REPLACE(@Order_BY, 'MediaCategoryId', 'Convert(numeric,MediaCategoryId)');
             SET @V_SQL = ' DECLARE @V_MediaServerPath  VARCHAR(max) , @V_MediaServerThumbnailPath  VARCHAR(MAx)  
			 SET @V_MediaServerPath = (SELECT URL FROM ZnodeMediaConfiguration WHERE IsActive=1)
			 SET @V_MediaServerThumbnailPath = (SELECT URL+ThumbnailFolderName+''/'' FROM ZnodeMediaConfiguration ZMC 
			 INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMC.MediaServerMasterId = ZMSM.MediaServerMasterId ) WHERE IsActive=1)
			 SELECT RANK()OVER(ORDER BY '+CASE
                                                 WHEN @Order_BY IS NULL
                                                      OR @Order_BY = ''
                                                 THEN ''
                                                 ELSE @Order_BY+' ,'
                                          END+'MediaId ) RowId, [MediaCategoryId],[MediaPathId],[Folder],[FileName],[Size],
			 [MediaType],[CreatedDate],[ModifiedDate],[MediaId],[Path],@V_MediaServerPath+[Path] MediaServerPath,
			 @V_MediaServerThumbnailPath+[Path] MediaServerThumbnailPath,[FamilyCode],[CreatedBy],[ShortDescription],[DisplayName] 
			 INTO #MediaPathDetail FROM '+CASE
                                                 WHEN @MediaPathId = -1
                                                 THEN ' View_GetAllMediaInRoot '
                                                 ELSE ' View_GetMediaPathDetail ZMC '
                                          END+' WHERE  '+CASE
                                                                  
                                                                            WHEN @MediaPathId = -1
                                                                            THEN '1=1  '
                                                                            ELSE '  exists (select top 1 1 from DBO.FN_GetMediaPathHierarchy('+CAST(@MediaPathId AS VARCHAR(1000))+') Q 
			 where Q.MediaPathId = ZMC.MediaPathId )   '
                                                                  
                                                               END+' Order BY '+CASE
                                                                                    WHEN @Order_BY IS NULL
                                                                                         OR @Order_BY = ''
                                                                                    THEN ' MediaCategoryId DESC'
                                                                                    ELSE @Order_BY
                                                                                END+' SELECT  @Count=ISNULL(Count(1),0) FROM  #MediaPathDetail  SELECT [MediaCategoryId],[MediaPathId],[Folder],[FileName],[Size],
			 [MediaType],[CreatedDate],[ModifiedDate],[MediaId],[Path],MediaServerPath, MediaServerThumbnailPath,
			 [FamilyCode],[CreatedBy],[ShortDescription],[DisplayName] FROM #MediaPathDetail 
			 WHERE RowId BETWEEN '+@Rows_start+' AND '+@Rows_end+' Order BY '+CASE
                                                                                   WHEN @Order_BY IS NULL
                                                                                        OR @Order_BY = ''
                                                                                   THEN ' MediaCategoryId DESC '
                                                                                   ELSE @Order_BY
                                                                              END;
    
             EXEC SP_executesql
                  @V_SQL,
                  N'@Count INT OUT',
                  @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaFolderDetails_Test @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@MediaPathId='+CAST(@WhereClause AS VARCHAR(100))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetMediaFolderDetails_Test',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;                                  
         END CATCH;
     END;