
CREATE VIEW [dbo].[View_GetOmsOrderNotes]
AS
SELECT        ANZU.UserName, ZON.Notes, ZOOD.OmsOrderId, ZON.CreatedDate, ZOOD.OmsOrderDetailsId, ZON.OmsQuoteId
FROM            dbo.ZnodeOmsNotes AS ZON 
LEFT JOIN dbo.ZnodeOmsOrderDetails AS ZOOD ON ZOOD.OmsOrderDetailsId = ZON.OmsOrderDetailsId 
LEFT OUTER JOIN dbo.ZnodeUser AS ZU ON ZU.UserId = ZON.CreatedBy 
LEFT OUTER JOIN dbo.AspNetUsers AS APNU ON APNU.Id = ZU.AspNetUserId 
LEFT OUTER JOIN dbo.AspNetZnodeUser AS ANZU ON ANZU.AspNetZnodeUserId = APNU.UserName


						 -- SELECT * FROM dbo.ZnodeOmsNotes
GO
