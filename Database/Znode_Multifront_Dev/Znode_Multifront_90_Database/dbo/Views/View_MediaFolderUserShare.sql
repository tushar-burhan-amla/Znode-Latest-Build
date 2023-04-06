
CREATE  VIEW [dbo].[View_MediaFolderUserShare] AS 
   SELECT zmp.MediaPathId,zmpl.[PathName] ,zmp.ParentMediaPathId
   FROM ZnodeMediaPath zmp
   LEFT JoIN ZnodeMediaFolderUser zmfu ON (zmfu.MediaPathId = zmp.MediaPathId)
   INNER JOIN ZnodeMediaPathLocale  zmpl ON (zmpl.MediaPathId = zmp.MediaPathId)