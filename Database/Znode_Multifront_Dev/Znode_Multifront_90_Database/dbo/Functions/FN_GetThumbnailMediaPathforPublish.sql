-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[FN_GetThumbnailMediaPathforPublish]
(
  @MediaId Varchar(1000) 

)
RETURNS VARCHAr(4000)
AS
BEGIN
	-- Declare the return variable here
     DECLARE @V_MediaServerThumbnailPath VARCHAr(4000)
	
	DECLARE @V_MediaDetails TABLE (MediaId INT , [Path] VARCHAr(300))

	INSERT INTO @V_MediaDetails
	SELECT MediaId , [Path] 
	FROM ZnodeMedia q 
	WHERE  EXISTS (SELECT TOP 1 1 FROM dbo.Split(@MediaId,',') a WHERE q.MediaId = a.item)

	SET  @V_MediaServerThumbnailPath =  SUBSTRING ((SELECT ','+[Path] FROM @V_MediaDetails FOR XML PATH ('') ) ,2,4000)   
	


	RETURN ISNULL(@V_MediaServerThumbnailPath,'no-image.png')

END