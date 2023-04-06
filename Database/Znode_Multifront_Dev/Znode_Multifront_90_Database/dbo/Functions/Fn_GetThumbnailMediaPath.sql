CREATE FUNCTION [dbo].[Fn_GetThumbnailMediaPath]  
(  
  @MediaId Varchar(1000)   
  ,@IsrequiredId BIT = 0   
)  
RETURNS VARCHAr(4000)  
AS  
BEGIN  
 -- Declare the return variable here  
    DECLARE @V_MediaServerThumbnailPath VARCHAr(4000)  
 SET @V_MediaServerThumbnailPath = (SELECT ISNULL(CASE WHEN a.CDNURL = '' THEN NULL ELSE a.CDNURL END,a.URL)+ThumbnailFolderName+'/' 
 FROM ZnodeMediaConfiguration a 
 INNER JOIN ZnodeMediaServerMaster b ON (a.MediaServerMasterId = b.MediaServerMasterId ) WHERE IsActive=1)    
   
 DECLARE @V_MediaDetails TABLE (MediaId INT , [Path] VARCHAr(300))  
  
 INSERT INTO @V_MediaDetails  
 SELECT MediaId , [Path]   
 FROM ZnodeMedia q   
 INNER JOIN  dbo.Split(@MediaId,',') a ON( q.MediaId = a.item)  
 ORDER BY a.id  
   
 SET  @V_MediaServerThumbnailPath = CASE WHEN @IsrequiredId = 1 THEN  SUBSTRING ((SELECT ',',@V_MediaServerThumbnailPath+[Path] 
 FROM @V_MediaDetails  FOR XML PATH ('') ) ,2,4000) +'~'+SUBSTRING ((SELECT ','+CAST(MediaId AS VARCHAr(1000) )  
 FROM @V_MediaDetails  FOR XML PATH ('') ) ,2,4000) ELSE SUBSTRING ((SELECT ',',@V_MediaServerThumbnailPath+[Path] 
 FROM @V_MediaDetails FOR XML PATH ('') ) ,2,4000)  END       
   
  
  
 RETURN ISNULL(@V_MediaServerThumbnailPath,CASE WHEN @IsrequiredId = 1 THEN '/MediaFolder/no-image.png~' ELSE '/MediaFolder/no-image.png' END )  
  
END