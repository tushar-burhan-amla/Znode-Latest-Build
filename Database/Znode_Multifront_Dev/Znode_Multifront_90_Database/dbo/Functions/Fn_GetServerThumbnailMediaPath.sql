CREATE FUNCTION [dbo].[Fn_GetServerThumbnailMediaPath]()  
RETURNS VARCHAR(4000)  
AS  
     BEGIN  
         DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);  
         DECLARE @V_MediaServerThumbnailPathWithMedia VARCHAR(4000);  
         SET @V_MediaServerThumbnailPath =  
         (  
             SELECT ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL)+ZMSM.ThumbnailFolderName+'/'    
             FROM ZnodeMediaConfiguration ZMC   
    INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)  
             WHERE IsActive = 1  
         );  
         SET @V_MediaServerThumbnailPathWithMedia = @V_MediaServerThumbnailPath  
         RETURN @V_MediaServerThumbnailPathWithMedia  
  
  
   --CASE  
   --                 WHEN @V_MediaServerThumbnailPathWithMedia IS NULL  
   --                      OR RTRIM(LTRIM(@V_MediaServerThumbnailPathWithMedia)) = ''  
   --                      OR @V_MediaServerThumbnailPath = @V_MediaServerThumbnailPathWithMedia  
   --                 THEN '/MediaFolder/no-image.png'  
   --                 ELSE @V_MediaServerThumbnailPathWithMedia  
   --             END;  
     END;