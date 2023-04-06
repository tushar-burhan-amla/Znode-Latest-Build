
-- EXEC Znode_MediaBredCrumbDetails 1042
CREATE  procedure [dbo].[Znode_MediaBredCrumbDetails]
(
	@MediaPathId int,
	@LocaleId    int = 1
)
AS 
	-- To display tree structure of folders for BredCrub
BEGIN
	SET NOCount oN 
 
	;WITH CTE
	AS
	(   

		SELECT  ParentMediaPathId ,ZMPL.[PathName] foldername ,ZMP.MediaPathId
		FROM [dbo].[ZnodeMediaPath] ZMP 
			INNER  JOIN [dbo].[ZnodeMediaPathLocale] ZMPL ON ZMP.MediaPathId  = ZMPL.MediaPathId
   		WHERE ZMP.MediaPathId  = @MediaPathId
		AND LocaleId = @LocaleId
		UNION ALL

		SELECT  ZMP.ParentMediaPathId ,[PathName] foldername,ZMP.MediaPathId
		FROM [dbo].[ZnodeMediaPath] ZMP 
			INNER  JOIN [dbo].[ZnodeMediaPathLocale] ZMPL ON ZMP.MediaPathId  = ZMPL.MediaPathId
			INNER JOIN CTE c ON (c.ParentMediaPathId = ZMP.MediaPathId )
		WHERE LocaleId = @LocaleId
	) 
	SELECT Row_number()Over(Order by ParentMediaPathId) SequenceId,Foldername  FolderName,MediaPathId,ISNULL(NULL,0) RowId
	FROM cte
	ORDER BY ParentMediaPathId
END

