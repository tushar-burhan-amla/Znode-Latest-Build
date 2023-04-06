-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Fn_GetThumbnailNoImagePath]
()
RETURNS VARCHAR(4000)
AS
     BEGIN
         DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);
        
         SET @V_MediaServerThumbnailPath = '/MediaFolder/no-image.png'
         RETURN @V_MediaServerThumbnailPath



     END;