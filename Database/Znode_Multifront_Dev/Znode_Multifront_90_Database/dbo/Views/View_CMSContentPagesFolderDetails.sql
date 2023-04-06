



CREATE view [dbo].[View_CMSContentPagesFolderDetails] AS 

  SELECT a.CMSContentPagesId,a.PortalId,a.CMSTemplateId,a.PageTitle,a.PageName,a.ActivationDate, a.ExpirationDate,a.IsActive,a.CreatedBy,a.CreatedDate,a.ModifiedBy,a.ModifiedDate,e.StoreName   ,c.CMSContentPageGroupId 
		,zct.Name TemplateName ,zcsd.SEOUrl
		  FROM ZnodeCMSContentPages a 
   LEft Outer JOIN [ZnodeCMSContentPageGroupMapping] b ON (b.CMSContentPagesId = a.CMSContentPagesId) 
   LEft Outer JOIN [ZnodeCMSContentPageGroup] c ON (c.CMSContentPageGroupId = b.CMSContentPageGroupId)
   LEft Outer JOIN [ZnodeCMSContentPageGroupLocale] d ON (d.CMSContentPageGroupId = c.CMSContentPageGroupId)
   LEFT JOIN ZnodeCMSTemplate zct ON (zct.CMSTemplateId = a.CMSTemplateId )
   LEFT JOIN ZnodeCMSSEODetail zcsd ON (zcsd.SEOId = a.CMSContentPagesId AND 
   EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType zcst WHERE zcst.CMSSEOTypeId = zcsd.CMSSEOTypeId AND zcst.Name = 'Content Page' ))
   LEft Outer JOIN ZnodePortal e on a.PortalId = e.PortalId