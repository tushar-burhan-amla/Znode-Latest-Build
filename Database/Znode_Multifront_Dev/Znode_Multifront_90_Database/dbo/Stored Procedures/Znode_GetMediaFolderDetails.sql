CREATE PROCEDURE [dbo].[Znode_GetMediaFolderDetails]
( 
	@WhereClause VARCHAR(1000),
	@MediaPathId INT,
	@Rows        INT           = 1000,
	@PageNo      INT           = 0,
	@Order_BY    VARCHAR(1000) = '',
	@RowsCount   INT OUT,
	@LocaleId    INT           = 1
)
AS
/*
	Summary: This Procedure is Used to Get Details of Media Folder

	Unit Testing:
	begin tran
		DECLARE @RowsCount BIGINT  
		EXEC Znode_GetMediaFolderDetails @MediaPathId = -1 , @WhereClause='',@Rows=2147483647,@PageNo=1 ,@Order_By='', @RowsCount = @RowsCount OUT  
	rollback tran

	begin tran
		DECLARE @RowsCount BIGINT  
		EXEC Znode_GetMediaFolderDetails @MediaPathId = 1 , @WhereClause='' ,@Rows=10,@PageNo=1 ,@RowsCount =@RowsCount
	rollback tran
*/
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
	DECLARE @DisplayNameId INT,
	@DescriptionId INT

	SELECT @DisplayNameId =MediaAttributeId FROM ZnodeMediaAttribute WHERE AttributeCode = 'DisplayName';
	SELECT @DescriptionId =MediaAttributeId FROM ZnodeMediaAttribute WHERE AttributeCode = 'Description';

	DROP TABLE IF EXISTS #GetMediaPathDetail;
	DROP TABLE IF EXISTS ##GetMediaPathHierarchy;

	CREATE TABLE #ZnodeMediaAttributeValue_DisplayName (MediaCategoryId INT,AttributeValue VARCHAR(500));

	INSERT INTO #ZnodeMediaAttributeValue_DisplayName
	SELECT MediaCategoryId, AttributeValue 
	FROM ZnodeMediaAttributeValue 
	WHERE MediaAttributeId =@DisplayNameId;

	CREATE TABLE #ZnodeMediaAttributeValue_Description (MediaCategoryId INT,AttributeValue VARCHAR(500));

	INSERT INTO #ZnodeMediaAttributeValue_Description
	SELECT MediaCategoryId, AttributeValue 
	FROM ZnodeMediaAttributeValue 
	WHERE MediaAttributeId =@DescriptionId;

	CREATE TABLE #GetMediaPathDetail
	(MediaCategoryId INT, MediaPathId INT, [Folder] VARCHAR(1000), [FileName] VARCHAR(1000), Size VARCHAR(30), Height VARCHAR(30) , 
		Width VARCHAR(30), [Type] VARCHAR(100), [MediaType] VARCHAR(100), CreatedDate DATETIME, ModifiedDate DATETIME,
		MediaId INT, [Path] VARCHAR(1000), MediaServerPath VARCHAR(1000), MediaServerThumbnailPath VARCHAR(1000), FamilyCode VARCHAR(100),
		CreatedBy INT, [DisplayName] VARCHAR(5000), [ShortDescription] VARCHAR(1000), [PathName] VARCHAR(1000), [Version] INT
	);

	INSERT INTO #GetMediaPathDetail
	(MediaCategoryId, MediaPathId, [Folder], [FileName], Size, Height, Width, Type, [MediaType], CreatedDate, ModifiedDate, MediaId,
		Path, MediaServerPath, MediaServerThumbnailPath, FamilyCode, CreatedBy, [DisplayName], [ShortDescription], [PathName], Version
	)
	SELECT Zmc.MediaCategoryId, ZMPL.MediaPathId, ZMPL.[PathName] [Folder], zM.[FileName], Zm.Size, Zm.Height, Zm.Width, Zm.Type, 
		Zm.Type [MediaType], zm.CreatedDate CreatedDate, zm.ModifiedDate ModifiedDate, Zm.MediaId, 
		ISNULL(ZMCF.CDNUrl, ZMCF.URL)+ZMSM.ThumbnailFolderName+'\'+zM.Path MediaThumbnailPath, ISNULL(ZMCF.CDNUrl, ZMCF.URL)+zM.Path MediaServerPath, 
		zM.Path, zmafl.FamilyCode FamilyCode, Zm.CreatedBy,ZMAVD.AttributeValue,ZMAVS.AttributeValue,ZMPL.[PathName], Zm.Version
	FROM ZnodeMediaCategory ZMC
	LEFT JOIN ZnodeMediaAttributeFamily zmafl ON(zmc.MediaAttributeFamilyId = zmafl.MediaAttributeFamilyId)
	INNER JOIN ZnodeMediaPathLocale ZMPL ON(ZMC.MediaPathId = ZMPL.MediaPathId)
	INNER JOIN ZnodeMedia ZM ON(Zm.MediaId = Zmc.MediaId)
	LEFT JOIN ZnodeMediaConfiguration ZMCF ON (ZMCF.MediaConfigurationId = ZM.MediaConfigurationId AND ZMCF.IsActive = 1)
	LEFT JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMCF.MediaServerMasterId)
	LEFT JOIN #ZnodeMediaAttributeValue_DisplayName ZMAVD ON ZMAVD.MediaCategoryId = Zmc.MediaCategoryId AND ZMAVD.AttributeValue IS NOT NULL
	LEFT JOIN #ZnodeMediaAttributeValue_Description ZMAVS ON ZMAVS.MediaCategoryId = Zmc.MediaCategoryId AND ZMAVS.AttributeValue IS NOT NULL;

	CREATE INDEX Ind_#GetMediaPathDetail_MediaCategoryId ON #GetMediaPathDetail (MediaPathId);

	DECLARE @Rows_start VARCHAR(1000), @Rows_end VARCHAR(1000);

	SET @MediaPathId = CASE WHEN @MediaPathId = -1 THEN 1 ELSE @MediaPathId END;

	SET @Rows_start = CASE WHEN @Rows >= 1000000 THEN 0 ELSE(@Rows * (@PageNo - 1)) + 1 END;
				 
	SET @Rows_end = CASE WHEN @Rows >= 1000000 THEN @Rows ELSE @Rows * (@PageNo) END;
				 
	DECLARE @SQL NVARCHAR(MAX);

	SET @Order_BY = REPLACE(@Order_BY, 'MediaPathId', 'Convert(numeric,MediaPathId)');
	SET @Order_BY = REPLACE(@Order_BY, 'Size', 'Convert(numeric,Size)');
	SET @Order_BY = REPLACE(@Order_BY, 'MediaId', 'Convert(numeric,MediaId)');
	SET @Order_BY = REPLACE(@Order_BY, 'CreatedBy', 'Convert(numeric,CreatedBy)');
	SET @Order_BY = REPLACE(@Order_BY, 'MediaCategoryId', 'Convert(numeric,MediaCategoryId)');

	SET @SQL = '
		SELECT * INTO ##GetMediaPathHierarchy FROM DBO.FN_GetMediaPathHierarchy('+CAST( @MediaPathId  AS VARCHAR(1000))+')';

	EXEC SP_executesql @SQL;

	SET @SQL = ' 
		DECLARE @V_MediaServerPath  VARCHAR(MAX) , @V_MediaServerThumbnailPath  VARCHAR(MAX)  
		SELECT RANK()OVER(ORDER BY '+CASE
										WHEN @Order_BY IS NULL
											OR @Order_BY = ''
										THEN ''
										ELSE @Order_BY+' ,'
								END+'MediaId ) RowId, [MediaCategoryId],[MediaPathId],[Folder],[FileName],[Size],[Height],[Width],
			[MediaType],[CreatedDate],[ModifiedDate],[MediaId],[Path],ISNULL(MediaServerPath,'''') AS MediaServerPath,
			ISNULL(MediaServerThumbnailPath,'''') AS MediaServerThumbnailPath,[FamilyCode],[CreatedBy],[ShortDescription],[DisplayName], [Version]
		INTO #MediaPathDetail FROM '+CASE
										WHEN @MediaPathId = -1
										THEN ' View_GetAllMediaInRoot '
										ELSE ' #GetMediaPathDetail ZMC '
								END+' WHERE 1=1 '+CASE
													WHEN @WhereClause = ''
														OR @WhereClause IS NULL
														OR @WhereClause = '-1'
													THEN 'AND exists (select top 1 1 FROM ##GetMediaPathHierarchy Q
		WHERE Q.MediaPathId = ZMC.MediaPathId )'
													ELSE CASE
														WHEN @MediaPathId = -1
														THEN ' AND '+@WhereClause
														ELSE ' AND exists (select top 1 1 FROM ##GetMediaPathHierarchy Q
		WHERE Q.MediaPathId = ZMC.MediaPathId ) and  '+@WhereClause
														END
													END+' Order BY '+CASE
																		WHEN @Order_BY IS NULL
																				OR @Order_BY = ''
																		THEN ' MediaCategoryId DESC'
																		ELSE @Order_BY
																	END+' SELECT  @Count=ISNULL(Count(1),0) FROM  #MediaPathDetail  SELECT [MediaCategoryId],[MediaPathId],[Folder],[FileName],[Size],[Height],[Width],
			[MediaType],[CreatedDate],[ModifiedDate],[MediaId],[Path],ISNULL(MediaServerPath,'''') AS MediaServerPath, ISNULL(MediaServerThumbnailPath,'''') AS MediaServerThumbnailPath,
			[FamilyCode],[CreatedBy],[ShortDescription],[DisplayName],[Version]
		FROM #MediaPathDetail
		WHERE RowId BETWEEN '+@Rows_start+' AND '+@Rows_end+' Order BY '+CASE
																			WHEN @Order_BY IS NULL
																				OR @Order_BY = ''
																			THEN ' MediaCategoryId DESC '
																			ELSE @Order_BY
																		END;
		
		EXEC SP_executesql @SQL,
					  N'@Count INT OUT',
					  @Count = @RowsCount OUT;

	DROP TABLE IF EXISTS #GetMediaPathDetail;
	DROP TABLE IF EXISTS ##GetMediaPathHierarchy;

	END TRY
	BEGIN CATCH
		DECLARE @Status BIT;
		SET @Status = 0;

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(),
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetMediaFolderDetails 
					@WhereClause = '''+ISNULL(@WhereClause,'''''')+''',
					@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',
					@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',
					@Order_BY='''+ISNULL(@Order_BY,'''''')+''',
					@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',
					@MediaPathId='+ISNULL(CAST(@WhereClause AS VARCHAR(100)),'''')+',
					@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''');
             
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetMediaFolderDetails',
		@ErrorInProcedure = 'Znode_GetMediaFolderDetails',
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;                                
	END CATCH;
END;