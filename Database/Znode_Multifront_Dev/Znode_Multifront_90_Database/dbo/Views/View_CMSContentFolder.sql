

CREATE  VIEW [dbo].[View_CMSContentFolder] AS 
   SELECT zmp.CMSContentPageGroupId,zmpl.[Name] ,zmp.ParentCMSContentPageGroupId
   FROM ZnodeCMSContentPageGroup zmp
   INNER JOIN ZnodeCMSContentPageGroupLocale  zmpl ON (zmpl.CMSContentPageGroupId = zmp.CMSContentPageGroupId)