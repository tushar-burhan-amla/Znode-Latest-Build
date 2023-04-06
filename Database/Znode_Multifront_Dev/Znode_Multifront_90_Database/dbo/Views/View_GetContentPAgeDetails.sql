
CREATE VIEW [dbo].[View_GetContentPageDetails]
AS
     SELECT ZCCP.CMSContentPagesId,
            PageName,
            PageTitle,
            ActivationDate,
            ExpirationDate,
            SEODescription,
            SEOKeywords,
            SEOTitle,
            SEOUrl,
            IsActive,
			IsRedirect,
            StoreName,
            CMSTemplateId,
            MetaInformation,
            ZCCP.PortalId,
            ZCCPGM.CMSContentPageGroupId,
            ZCCPL.LocaleId,
			ZCSEDL.CanonicalURL,
			ZCSEDL.RobotTag
     FROM ZnodeCMSContentPages ZCCP
          LEFT JOIN ZnodeCMSContentPagesLocale ZCCPL ON(ZCCPL.CMSContentPagesId = ZCCP.CMSContentPagesId)
          LEFT JOIN ZnodeCMSContentPageGroupMapping ZCCPGM ON(ZCCPGM.CMSContentPagesId = ZCCp.CMSContentPagesId)
          LEFT JOIN ZnodeCMSContentPageGroup ZCCPG ON(ZCCPG.CMSContentPageGroupId = ZCCPGM.CMSContentPageGroupId)
          LEFT JOIN ZnodeCMSContentPageGroupLocale ZCCPGL ON(ZCCPGL.CMSContentPageGroupId = ZCCPG.CMSContentPageGroupId
                                                             AND ZCCPGL.LocaleId = ZCCPL.LocaleId)
          LEFT JOIN ZnodeCMSSeoDetail ZCSED ON(ZCSED.SEOCode = ZCCp.PageName
                                               AND EXISTS
                                              (
                                                  SELECT TOP 1 1
                                                  FROM ZnodeCMSSEOType ZCST
                                                  WHERE ZCST.CMSSEOTypeId = ZCSED.CMSSEOTypeId
                                                        AND ZCST.Name = 'Content Page'
                                              ))
          LEFT JOIN ZnodeCMSSEODetailLocale ZCSEDL ON(ZCSEDL.CMSSEODetailId = ZCSED.CMSSEODetailId
                                                      AND ZCSEDL.LocaleId = ZCCPL.LocaleId)
          LEFT JOIN ZnodePortal ZP ON(ZP.PortalId = ZCCP.POrtalId);