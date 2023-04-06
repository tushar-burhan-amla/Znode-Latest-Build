CREATE View [dbo].[View_MediaPathDetails]   
AS   
SELECT ZM.MediaId  
,ZM.MediaConfigurationId  
,ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL)+ZM.Path Path      
,ZM.FileName  
,ZM.Size  
,ZM.Height  
,ZM.Width  
,ZM.Length  
,ZM.Type  
,ZM.CreatedBy  
,ZM.CreatedDate  
,ZM.ModifiedBy  
,ZM.ModifiedDate  
FROM ZnodeMedia ZM   
LEFT JOIN ZnodeMediaConfiguration ZMC ON (ZMC.MediaConfigurationId = ZM.MediaConfigurationId)  
LEFT JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)