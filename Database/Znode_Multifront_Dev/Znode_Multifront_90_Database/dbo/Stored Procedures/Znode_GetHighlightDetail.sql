CREATE PROCEDURE [dbo].[Znode_GetHighlightDetail]
( 
	@WhereClause NVARCHAR(MAX),
	@Rows INT= 10,
	@PageNo INT= 1,
	@Order_BY VARCHAR(1000)= '',
	@RowsCount INT= 0 OUT,
	@LocaleId INT= 1,
	@IsFromAdmin BIT = 0,
    @IsAssociated BIT= 0,
    @Isdebug BIT= 0
)
AS
/*
	Summary :- This Procedure is used to get the highlights details
	Unit Testing
	begin tran
	EXEC Znode_GetHighlightDetail '',10,1,'',0,1,0
	rollback tran
*/
BEGIN
BEGIN TRY
SET NOCOUNT ON;

	DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
	DECLARE @SeoId VARCHAR(MAX)= '', @SQL NVARCHAR(MAX);

	DECLARE @TBL_HighlightsDetails TABLE
	(
		Description NVARCHAR(max),
		HighlightId INT,
		HighlightCode VARCHAR(600),
		HighlightType NVARCHAR(400),
		DisplayOrder INT,
		IsActive BIT,
		HighlightLocaleId INT,
		MediaPath NVARCHAR(MAX),
		MediaId INT,
		Hyperlink NVARCHAR(MAX),
		ImageAltTag NVARCHAR(4000),DisplayPopup BIT
	);
	--Get default attributeid for ProductHighlights
	DECLARE @AttributeId INT= [dbo].[Fn_GetProductHighlightsAttributeId]();
	DECLARE @TBL_AttributeDefault TABLE
	(
		PimAttributeId INT,
		AttributeDefaultValueCode varchar(600),
		IsEditable BIT,
		AttributeDefaultValue NVARCHAR(max),
		DisplayOrder INT
	);
   DECLARE @TBL_HighlightsDetail TABLE
	(
		Description NVARCHAR(max),
		HighlightId INT,
		HighlightCode varchar(600),
		HighlightType NVARCHAR(400),
		DisplayOrder INT,
		IsActive BIT,
		HighlightLocaleId INT,
		MediaPath NVARCHAR(max),
		MediaId INT,
		Hyperlink NVARCHAR(max),
		ImageAltTag NVARCHAR(4000),DisplayPopup BIT,
		HighlightName NVARCHAR(max),
		RowId INT,
		CountId INT
	);

	INSERT INTO @TBL_AttributeDefault
	EXEC Znode_GetAttributeDefaultValueLocale @AttributeId, @LocaleId;

	SET @WhereClause = ' '+@WhereClause+
		CASE WHEN @IsAssociated = 1 THEN 
			CASE WHEN @WhereClause = '' THEN ' ' ELSE ' AND 'END
			+' EXISTS ( SELECT TOP 1 1
				FROM ZnodePimAttributeValue ZAV
				INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZAV.PimAttributeId AND ZA.AttributeCode = ''Highlights'')
				INNER JOIN ZnodePimProductAttributeDefaultValue ZAVL ON (ZAV.PimAttributeValueId= ZAVL.PimAttributeValueId )
				INNER JOIN ZnodePimAttributeDefaultValue ZADVL ON (ZAVL.PimAttributeDefaultValueId = ZADVL.PimAttributeDefaultValueId)
				WHERE ( ZADVL.AttributeDefaultValueCode = TMADV.AttributeDefaultValueCode))'
			ELSE CASE WHEN @WhereClause = '' THEN ' 1 = 1  ' ELSE '' END
		END;

	CREATE TABLE #Cte_GetHighlightsBothLocale (RowId INT, Description NVARCHAR(MAX), HighlightId INT, LocaleId INT,HighlightCode VARCHAR(600),HighlightType NVARCHAR(400), DisplayOrder INT, IsActive BIT, HighlightLocaleId INT, MediaPath VARCHAR(1000),MediaId INT, Hyperlink NVARCHAR(MAX),ImageAltTag NVARCHAR(400),DisplayPopup BIT)
	
	--For @IsFromAdmin = 1 getting media from highlight table
	IF @IsFromAdmin = 1
	BEGIN
		INSERT INTO #Cte_GetHighlightsBothLocale(RowId , Description , HighlightId , LocaleId ,HighlightCode ,HighlightType , DisplayOrder , IsActive , HighlightLocaleId , MediaPath ,MediaId , Hyperlink ,ImageAltTag ,DisplayPopup)
		SELECT Row_Number()Over( PARTITION BY   ZH.HighlightCode ORDER BY ZH.MediaId desc) RowId, ZHL.Description, ZH.HighlightId, LocaleId, ZH.HighlightCode,ZPHT.Name HighlightType , ZADV.DisplayOrder, ZH.IsActive, ZHL.HighlightLocaleId, [dbo].[Fn_GetMediaThumbnailMediaPath]( Zm.path ) AS MediaPath
			, ZH.MediaId, Hyperlink,ImageAltTag,DisplayPopup
		FROM ZnodeHighlight AS ZH
		LEFT JOIN ZnodePimAttributeDefaultValue ZADV on ZADV.AttributeDefaultValueCode =ZH.HighlightCode
		LEFT JOIN  ZnodeHighlightLocale AS ZHL ON(ZHL.HighlightId = ZH.HighlightId)  
		LEFT JOIN ZnodeMedia AS ZM ON(ZM.MediaId = ZH.MediaId)
		LEFT JOIN ZnodeHighLightType ZPHT ON (ZPHT.HighlightTypeId = ZH.HighlightTypeId)  
		WHERE LocaleId IN ( @LocaleId, @DefaultLocaleId )
	END
	ELSE
	BEGIN
		INSERT INTO #Cte_GetHighlightsBothLocale(RowId , Description , HighlightId , LocaleId ,HighlightCode ,HighlightType , DisplayOrder , IsActive , HighlightLocaleId , MediaPath ,MediaId , Hyperlink ,ImageAltTag ,DisplayPopup)
		SELECT Row_Number()Over( PARTITION BY   ZH.HighlightCode ORDER BY ZM.MediaId desc) RowId, ZHL.Description, ZH.HighlightId, LocaleId, ZH.HighlightCode,ZPHT.Name HighlightType , ZADV.DisplayOrder, ZH.IsActive, ZHL.HighlightLocaleId, [dbo].[Fn_GetMediaThumbnailMediaPath]( Zm.path ) AS MediaPath
			, ZH.MediaId, Hyperlink,ImageAltTag,DisplayPopup
		FROM ZnodeHighlight AS ZH
		LEFT JOIN ZnodePimAttributeDefaultValue ZADV on ZADV.AttributeDefaultValueCode =ZH.HighlightCode
		LEFT JOIN  ZnodeHighlightLocale AS ZHL ON(ZHL.HighlightId = ZH.HighlightId)  
		LEFT JOIN ZnodeMedia AS ZM ON(ZM.MediaId = ZADV.MediaId)
		LEFT JOIN ZnodeHighLightType ZPHT ON (ZPHT.HighlightTypeId = ZH.HighlightTypeId)  
		WHERE LocaleId IN ( @LocaleId, @DefaultLocaleId )
	END

	;WITH Cte_HighlightsFirstLocale AS 
	(
		SELECT Description, HighlightId, LocaleId, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath
				  , MediaId, Hyperlink,ImageAltTag,DisplayPopup
		FROM #Cte_GetHighlightsBothLocale AS CTGBBL
		WHERE LocaleId = @LocaleId and RowId = 1
	),
	Cte_HighlightsDefaultLocale	AS 
	(
		SELECT Description, HighlightId, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath
				   , MediaId, Hyperlink,ImageAltTag,DisplayPopup
		FROM Cte_HighlightsFirstLocale
		UNION ALL
		SELECT Description, HighlightId, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath
					, MediaId, Hyperlink,ImageAltTag,DisplayPopup
		FROM #Cte_GetHighlightsBothLocale AS CTBBL
		WHERE LocaleId = @DefaultLocaleId and RowId = 1 AND
		  NOT EXISTS
		(
			SELECT TOP 1 1
			FROM Cte_HighlightsFirstLocale AS CTBFL
			WHERE CTBBL.HighlightId = CTBFL.HighlightId
		)
	)
	INSERT INTO @TBL_HighlightsDetails( Description, HighlightId, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath, MediaId, Hyperlink,ImageAltTag,DisplayPopup)
	SELECT Description, HighlightId, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath, MediaId, Hyperlink,ImageAltTag,DisplayPopup
	FROM Cte_HighlightsDefaultLocale AS CTEBD;

	SELECT TBBD.*, TBAD.AttributeDefaultValue AS HighlightName, TBAD.AttributeDefaultValueCode
	INTO #TM_HighlightsLocale
	FROM @TBL_HighlightsDetails AS TBBD
	INNER JOIN @TBL_AttributeDefault AS TBAD  ON(TBAD.AttributeDefaultValueCode = TBBD.HighlightCode);

	SET @SQL = '
	 ;With Cte_HighlightsDetails AS
	(
		SELECT Description ,HighlightId , HighlightCode,HighlightType , DisplayOrder  ,IsActive   ,HighlightLocaleId
	   ,MediaPath ,MediaId ,Hyperlink,ImageAltTag,DisplayPopup
	   ,HighlightName ,'+[dbo].[Fn_GetPagingRowId]( @Order_BY, 'HighlightId DESC' )+',Count(*)Over() CountId
	    FROM #TM_HighlightsLocale TMADV
	    WHERE 1=1
		'+[dbo].[Fn_GetFilterWhereClause]( @WhereClause )+'
	)
	,Cte_Highlights AS
	(
	SELECT Description ,HighlightId , HighlightCode,HighlightType , DisplayOrder  ,IsActive   ,HighlightLocaleId
	,MediaPath ,MediaId ,Hyperlink,ImageAltTag,DisplayPopup
	   ,HighlightName ,RowId  ,CountId
	FROM Cte_HighlightsDetails
	'+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+'
	) 
	SELECT Description ,HighlightId , HighlightCode,HighlightType , DisplayOrder  ,IsActive   ,HighlightLocaleId
	,MediaPath ,MediaId ,Hyperlink,ImageAltTag,DisplayPopup
	   ,HighlightName ,RowId  ,CountId
	FROM Cte_Highlights
	' +[dbo].[Fn_GetOrderByClause]( @Order_BY, 'HighlightId DESC')

	INSERT INTO @TBL_HighlightsDetail( Description, HighlightId, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath, MediaId, Hyperlink,ImageAltTag,DisplayPopup, HighlightName, RowId, CountId )
	EXEC (@SQL);

	SET @RowsCount = ISNULL(( SELECT TOP 1 CountId FROM @TBL_HighlightsDetail),0);

	SELECT DISTINCT HighlightId, Description, HighlightCode,HighlightType, DisplayOrder, IsActive, HighlightLocaleId, MediaPath, MediaId, Hyperlink,ImageAltTag,DisplayPopup, HighlightName
	FROM @TBL_HighlightsDetail
	ORDER BY DisplayOrder;

	END TRY
	BEGIN CATCH
	DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetHighlightDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'@IsFromAdmin ='+CAST(@IsFromAdmin AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
             
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
 
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_GetHighlightDetail',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;