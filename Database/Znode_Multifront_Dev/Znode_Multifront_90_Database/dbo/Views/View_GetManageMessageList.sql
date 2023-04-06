

 CREATE  View [dbo].[View_GetManageMessageList]
AS
SELECT  c.CMSPortalMessageId ,a.CMSMessageId , a.[Message]  , b.MessageKey Location,
e.StoreName,
a.LocaleId,e.PortalId,c.CMSMessageKeyId
,ZCPMT.TagXML as MessageTag  , TY.DisplayName StateName 
,a.IsPublished PublishStatus,
Case when e.PortalId is null then 'True' else 'False' end as IsGlobalContentBlock FROM  [dbo].[ZnodeCMSMessage] a 
INNER JOIN [dbo].[ZnodeCMSPortalMessage] c ON (a.CMSMessageId = c.CMSMessageId)
LEFT JOIN  [dbo].[ZnodeCMSMessageKey] b ON (b.CMSMessageKeyId  = c.CMSMessageKeyId)
LEFT JOIN  [dbo].ZnodeCMSPortalMessageKeyTag ZCPMT ON (ISNULL(ZCPMT.Portalid,-1) = ISNULL(C.PortalId,-1) AND C.CMSMessageKeyId = ZCPMT.CMSMessageKeyId )
Left JOIN  [dbo].ZnodePortal e ON (e.PortalId = c.PortalId)
LEFT JOIN  ZnodePublishState TY ON (TY.PublishStateId = a.PublishStateId )