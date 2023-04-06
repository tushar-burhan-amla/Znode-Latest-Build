CREATE FUNCTION [dbo].[Fn_GetMediaPathServer]  
(@path VARCHAR(1000)  
  
)  
RETURNS VARCHAR(4000)  
AS  
     BEGIN  
         DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);  
         DECLARE @V_MediaServerThumbnailPathWithMedia VARCHAR(4000);  
         SET @V_MediaServerThumbnailPath =  
         (  
             SELECT ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL) 
			 FROM ZnodeMediaConfiguration ZMC Inner join ZnodeMedia ZM ON ZMC.MediaConfigurationId = ZM.MediaConfigurationId    
            Inner join ZnodeMediaCategory ZMCT ON ZM.MediaId = ZMCT.MediaId  
            Inner join ZnodeMediaPath ZMP ON ZMCT.MediaPathId = ZMP.MediaPathId where ZMC.IsActive=1  
            and ZM.[Path] =  @path  
         );  
         --SET @V_MediaServerThumbnailPathWithMedia = SUBSTRING(  
         --                                                    (  
         --                                                        SELECT ',',  
         --                                                               @V_MediaServerThumbnailPath+item  
         --                                                        FROM dbo.Split(@path, ',') a  
         --                                                        FOR XML PATH('')  
         --                                                    ), 2, 4000);  
         RETURN CASE  
                    WHEN (@V_MediaServerThumbnailPath IS NULL  
                         OR RTRIM(LTRIM(@V_MediaServerThumbnailPath)) = '')  
                         --OR @V_MediaServerThumbnailPath = @V_MediaServerThumbnailPath  
                    THEN '/MediaFolder/no-image.png'  
                    ELSE @V_MediaServerThumbnailPath+@path  
                END;  
   --RETURN @V_MediaServerThumbnailPath  
     END;