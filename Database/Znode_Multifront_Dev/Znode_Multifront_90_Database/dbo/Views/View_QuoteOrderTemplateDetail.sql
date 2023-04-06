CREATE VIEW [dbo].[View_QuoteOrderTemplateDetail]
AS
     SELECT ZOT.OmsTemplateId,
            ZOT.PortalId,
            ZOT.UserId,
            ZOT.TemplateName,
            ZOT.CreatedBy,
            ZOT.CreatedDate,
            ZOT.ModifiedBy,
            ZOT.ModifiedDate,
            COUNT(ZOTL.OmsTemplateLineItemId) Items
     FROM ZnodeOmsTemplate ZOT
          LEFT JOIN ZnodeOmsTemplateLineItem ZOTL ON(ZOTL.OmsTemplateId = ZOT.OmsTemplateId)
     GROUP BY ZOT.OmsTemplateId,
              ZOT.PortalId,
              ZOT.UserId,
              ZOT.TemplateName,
              ZOT.CreatedBy,
              ZOT.CreatedDate,
              ZOT.ModifiedBy,
              ZOT.ModifiedDate;