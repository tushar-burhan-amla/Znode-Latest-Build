CREATE   FUNCTION [dbo].[Fn_ZnodeMediaRecurcivePath]
(
 @MediaPathId int 
 ,@LocaleId INT = 1 
 )
RETURNS VARCHAR(8000)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @return VARCHAR(8000)
;WITH CTE
AS
(
    SELECT  ParentMediaPathId ,m2.[PathName] foldername ,m2.[PathName]+'\' f1 
    FROM [dbo].[ZnodeMediaPath] m1 
        INNER  JOIN [dbo].[ZnodeMediaPathLocale] m2 ON M1.MediaPathId  = M2.MediaPathId
   	WHERE M1.MediaPathId  = @MediaPathId
	AND  m2.LocaleId = @LocaleId
    
	UNION ALL

    SELECT  m1.ParentMediaPathId ,[PathName] foldername,[PathName]+'\' f1
    FROM [dbo].[ZnodeMediaPath] m1 
        INNER  JOIN [dbo].[ZnodeMediaPathLocale] m2 ON M1.MediaPathId  = M2.MediaPathId
		INNER JOIN CTE c ON (c.ParentMediaPathId = M1.MediaPathId )
	AND  m2.LocaleId = @LocaleId
)
SELECT  @return = SUBSTRING(( SELECT ''+F1 FROM CTE 
ORDER BY ParentMediaPathId
FOR XML PATH ('') ),1,8000)
	RETURN @return

END
