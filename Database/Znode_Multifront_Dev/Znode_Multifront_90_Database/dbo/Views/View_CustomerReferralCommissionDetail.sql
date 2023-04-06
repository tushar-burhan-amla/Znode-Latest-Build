/* DROP View View_CustomerReferralCommissionDetail*/
CREATE VIEW [dbo].[View_CustomerReferralCommissionDetail]
AS
SELECT DISTINCT A.UserId, C.ReferralCommission, A.ReferralStatus, B.Name, C.ReferralCommissionTypeId, C.OmsReferralCommissionId, 
	C.OmsOrderDetailsId, C.Description, C.OrderCommission, F.CurrencyCode, H.OrderNumber, ZC.CultureCode
FROM dbo.ZnodeUser AS A
INNER JOIN dbo.ZnodeOmsReferralCommission AS C ON A.UserId = C.UserId 
INNER JOIN dbo.ZnodeReferralCommissionType AS B ON C.ReferralCommissionTypeId = B.ReferralCommissionTypeId 
INNER JOIN dbo.ZnodeUserPortal AS D ON A.UserId = D.UserId 
INNER JOIN dbo.ZnodePortalUnit AS E ON D.PortalId = E.PortalId 
INNER JOIN dbo.ZnodeCurrency AS F ON E.CurrencyId = F.CurrencyId 
INNER JOIN dbo.ZnodeOmsOrderDetails AS G ON C.OmsOrderDetailsId = G.OmsOrderDetailsId 
INNER JOIN dbo.ZnodeOmsOrder AS H ON G.OmsOrderId = H.OmsOrderId
INNER JOIN dbo.ZnodeCulture ZC ON F.CurrencyId = ZC.CurrencyId AND ZC.IsDefault = 1
GO

