/*Order by ZnodeOrder.OmsOrderId desc  ;*/
CREATE VIEW dbo.View_GetRMASearchRequest
AS
SELECT DISTINCT 
                         RMARequest.RmaRequestId, RMARequest.RequestNumber, ZnodeOrder.OmsOrderId, ZnodeOrder.OmsOrderDetailsId, ZnodeOrder.PortalId, 
                         RMARequest.RmaRequestStatusId, zrs.Name AS RequestStatus, ZnodeOrder.BillingFirstName, ZnodeOrder.BillingLastName, RMARequest.RequestDate, 
                         ISNULL(RMARequest.Total, 0) AS Total, ISNULL(RMARequest.TaxCost, 0) AS TaxCost, ISNULL(RMARequest.SubTotal, 0) AS Subtotal, ISNULL(RMARequest.Discount, 
                         0) AS Discount, ZNodePortal.StoreName, ISNULL(ZU.FirstName, N'') + ' ' + ISNULL(ZU.LastName, N'') AS CustomerName, ANZU.UserName, 
                         dbo.ZnodeOmsOrder.OrderNumber
FROM            dbo.ZnodeOmsOrder INNER JOIN
                         dbo.ZnodeOmsOrderDetails AS ZnodeOrder ON dbo.ZnodeOmsOrder.OmsOrderId = ZnodeOrder.OmsOrderId RIGHT OUTER JOIN
                         dbo.ZnodeRmaRequest AS RMARequest LEFT OUTER JOIN
                         dbo.ZnodeRmaRequestItem AS RMARequestItem ON RMARequest.RmaRequestId = RMARequestItem.RmaRequestId LEFT OUTER JOIN
                         dbo.ZnodeOmsOrderLineItems AS OrderLineItem ON RMARequestItem.OmsOrderLineItemsId = OrderLineItem.OmsOrderLineItemsId ON 
                         ZnodeOrder.OmsOrderDetailsId = OrderLineItem.OmsOrderDetailsId LEFT OUTER JOIN
                         dbo.ZnodePortal AS ZNodePortal ON ZnodeOrder.PortalId = ZNodePortal.PortalId LEFT OUTER JOIN
                         dbo.ZnodeRmaRequestStatus AS zrs ON zrs.RmaRequestStatusId = RMARequest.RmaRequestStatusId LEFT OUTER JOIN
                         dbo.ZnodeUser AS ZU ON ZU.UserId = ZnodeOrder.UserId LEFT OUTER JOIN
                         dbo.AspNetUsers AS ANU ON ZU.AspNetUserId = ANU.Id LEFT OUTER JOIN
                         dbo.AspNetZnodeUser AS ANZU ON ANZU.AspNetZnodeUserId = ANU.UserName
WHERE        (zrs.IsActive = 1)

